using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using VisionEditor.Core;
using VisionFramework;

// This is the independent main framework baseline, not an original SDK/device workload.
if(args.Length is <1 or >3)throw new ArgumentException("Usage: FrameworkStabilityProbe13 <owned-output-parent> [duration-seconds >= 1800] [baseline-sha]");
int duration=args.Length>1?int.Parse(args[1],System.Globalization.CultureInfo.InvariantCulture):1800;
if(duration<1800||duration>86400)throw new ArgumentOutOfRangeException(nameof(duration),"A stability baseline requires 1800..86400 seconds; short runs are not accepted.");
string parent=Path.GetFullPath(args[0]);
Directory.CreateDirectory(parent);
string output=Path.Combine(parent,"stability-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
string marker=Path.Combine(output,"owned-run.json");
var options=new JsonSerializerOptions{WriteIndented=true};
using var process=Process.GetCurrentProcess();
string processPath=Environment.ProcessPath??throw new InvalidOperationException("Missing process identity.");
File.WriteAllText(marker,JsonSerializer.Serialize(new{
  kind="framework-stability-probe-13",id=Path.GetFileName(output),pid=Environment.ProcessId,
  processStartUtc=process.StartTime.ToUniversalTime(),processPath,processSha256=Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(processPath))),
  baselineSha=args.Length>2?args[2]:"unspecified",durationSeconds=duration,warmupSeconds=120,sampleSeconds=10,
  workload="One fresh GraphEditorDocument per cycle: two renames, undo/redo, nonactive zoom, procedure duplicate, Group creation and independent zoom; every 32 cycles Save/Load/SaveAs into two reused owned files; 10ms minimum pause each cycle.",
  scope="Managed main/Core only. No UI, original VM, SDK, plugins, device, parallel scheduling, native parity or manual acceptance.",
  runtime=RuntimeInformation.FrameworkDescription,os=RuntimeInformation.OSDescription,architecture=RuntimeInformation.ProcessArchitecture.ToString(),processorCount=Environment.ProcessorCount,
  assemblies=new[]{typeof(GraphEditorDocument).Assembly,typeof(SolutionStore).Assembly}.Select(a=>new{a.FullName,a.Location,sha256=Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(a.Location)))}),
  localDiagnosticBudgets=new{privateMedianGrowthBytes=64L*1024*1024,threadMedianGrowth=8,handleMedianGrowth=64,
    rationale="Explicit conservative local regression envelope after 120s warmup. These are diagnostic budgets, not vendor-derived performance requirements or proof of absence of leaks."}
},options));
Console.WriteLine("OWNED_RUN="+output);Console.Out.Flush();
long cycles=0,assertions=0,saves=0;bool cancelled=false;
Console.CancelKeyPress+=(_,e)=>{e.Cancel=true;cancelled=true;};
var clock=Stopwatch.StartNew();var samples=new List<Sample>();double nextSample=0;
string firstFile=Path.Combine(output,"workload.vmx.json"),secondFile=Path.Combine(output,"save-as","workload.vmx.json");
var registry=new ModuleRegistry();WorkflowCalls.Register(registry);
string? failure=null;
void Require(bool value,string label){assertions++;if(!value)throw new InvalidOperationException(label+" at cycle "+cycles);}
void SampleNow(){process.Refresh();var row=new Sample(clock.Elapsed.TotalSeconds,cycles,assertions,saves,
    process.PrivateMemorySize64,process.WorkingSet64,GC.GetTotalMemory(false),process.Threads.Count,process.HandleCount,process.TotalProcessorTime.TotalSeconds);
  samples.Add(row);File.AppendAllText(Path.Combine(output,"samples.jsonl"),JsonSerializer.Serialize(row)+Environment.NewLine);
  WriteStatus(false);
}
void WriteStatus(bool finished){
  string temp=Path.Combine(output,"status.pending.json"),dest=Path.Combine(output,"status.json");
  File.WriteAllText(temp,JsonSerializer.Serialize(new{pid=Environment.ProcessId,finished,requestedSeconds=duration,elapsedSeconds=clock.Elapsed.TotalSeconds,cycles,assertions,saves,samples=samples.Count,cancelled,failure},options));
  File.Move(temp,dest,true);
}
try{
  while(clock.Elapsed.TotalSeconds<duration&&!cancelled){
    if(clock.Elapsed.TotalSeconds>=nextSample){SampleNow();nextSample+=10;}
    var doc=new GraphEditorDocument(registry);string root=doc.CurrentProcedureId;
    doc.RenameProcedure(root,"first");doc.RenameProcedure(root,"second");doc.Undo();Require(doc.CurrentProcedure.Name=="first","Undo restores old name");
    doc.Redo();Require(doc.CurrentProcedure.Name=="second","Redo restores new name");
    string other=doc.AddProcedure("other");int index=(int)(cycles%20);doc.SetContainerZoomIndex(root,index);
    Require(doc.CurrentProcedureId==other&&doc.Document.Procedures.Single(p=>p.Id==root).CanvasZoomIndex==index,"Inactive viewport isolation");
    string copy=doc.DuplicateProcedure(root,"copy");Require(doc.Document.Procedures.Single(p=>p.Id==copy).CanvasZoomIndex==index,"Duplicate preserves viewport");
    doc.SelectProcedure(root);string node=doc.AddOwnedGroup(100,100);string group=doc.EnterOwnedGroup(node);doc.SetContainerZoomIndex(group,19-index);
    Require(doc.Document.Procedures.Single(p=>p.Id==root).CanvasZoomIndex==index&&doc.CurrentProcedure.CanvasZoomIndex==19-index,"Group viewport independence");
    if(cycles%32==0){
      doc.Save(firstFile);saves++;var loaded=new GraphEditorDocument(registry);loaded.Load(firstFile);
      Require(loaded.Document.Procedures.Count==4,"Owned Group survives save and reload");
      Require(loaded.Document.Procedures.Single(p=>p.Id==root).CanvasZoomIndex==index,"Root viewport survives persistence");
      Require(loaded.Document.Procedures.Single(p=>p.Id==group).CanvasZoomIndex==19-index,"Group viewport survives persistence");
      loaded.Save(secondFile);saves++;loaded.Load(secondFile);Require(!loaded.IsDirty&&loaded.Document.Procedures.Count==4,"SaveAs reload is clean");
    }
    cycles++;Thread.Sleep(10);
  }
}catch(Exception e){failure=e.ToString();File.WriteAllText(Path.Combine(output,"failure.txt"),failure);}
try{SampleNow();}catch(Exception e){failure=(failure??"")+Environment.NewLine+"Final sampling failed: "+e;File.WriteAllText(Path.Combine(output,"failure.txt"),failure);}
clock.Stop();
var measured=samples.Where(s=>s.ElapsedSeconds>=120).ToArray();
double Median(IEnumerable<double> values){var sorted=values.Order().ToArray();return sorted.Length==0?double.NaN:sorted.Length%2==1?sorted[sorted.Length/2]:(sorted[sorted.Length/2-1]+sorted[sorted.Length/2])/2;}
double Growth(Func<Sample,double> select)=>measured.Length<10?double.NaN:Median(measured.TakeLast(5).Select(select))-Median(measured.Take(5).Select(select));
double privateGrowth=Growth(s=>s.PrivateBytes),threadGrowth=Growth(s=>s.Threads),handleGrowth=Growth(s=>s.Handles);
bool enough=clock.Elapsed.TotalSeconds>=duration&&measured.Length>=10;
bool withinBudgets=double.IsFinite(privateGrowth)&&privateGrowth<=64L*1024*1024&&threadGrowth<=8&&handleGrowth<=64;
bool passed=enough&&!cancelled&&failure is null&&withinBudgets&&cycles>0&&saves>0;
File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{
  passed,completedDuration=enough,requestedSeconds=duration,elapsedSeconds=clock.Elapsed.TotalSeconds,pid=Environment.ProcessId,cycles,assertions,saves,
  samples=samples.Count,postWarmupSamples=measured.Length,cancelled,failure,
  diagnostics=new{privateGrowthBytes=double.IsFinite(privateGrowth)?(double?)privateGrowth:null,threadGrowth=double.IsFinite(threadGrowth)?(double?)threadGrowth:null,handleGrowth=double.IsFinite(handleGrowth)?(double?)handleGrowth:null,withinBudgets},
  limitations="This is a 30 minute or longer managed main-document baseline. No claim of original VM equivalence, full host stability, concurrent SDK/callback safety or universal absence of leaks. Budget failures and interrupted runs remain evidence; they are not silently retried."
},options));
WriteStatus(true);
var fileHashes=Directory.EnumerateFiles(output,"*",SearchOption.AllDirectories).Where(p=>!p.EndsWith("evidence-hashes.json",StringComparison.OrdinalIgnoreCase))
  .Select(p=>new{path=Path.GetRelativePath(output,p),sha256=Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(p)))}).ToArray();
File.WriteAllText(Path.Combine(output,"evidence-hashes.json"),JsonSerializer.Serialize(fileHashes,options));
Console.WriteLine((passed?"PASS":"FAIL")+" cycles="+cycles+" elapsed="+clock.Elapsed.TotalSeconds.ToString("F1")+" output="+output);
Environment.ExitCode=passed?0:1;

sealed record Sample(double ElapsedSeconds,long Cycles,long Assertions,long Saves,long PrivateBytes,long WorkingSetBytes,long ManagedBytes,int Threads,int Handles,double CpuSeconds);
