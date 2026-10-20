using System.IO;
using System.Text.Json;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.CameraMapping;

static class Program {
 static int checks;
 static void Check(bool passed,string reason){if(!passed)throw new Exception(reason);checks++;}
 static readonly string[] Leaves=["TargetPointX1","TargetPointY1","ObjectPointX1","ObjectPointY1"];
 static readonly CameraMappingPair[] Original=[new(0,0,10,-5),new(10,0,30,-5),new(0,10,10,15),new(10,10,30,15)];
 [STAThread]static int Main(){string output=Path.GetFullPath(Environment.GetEnvironmentVariable("CAMERAMAPPING_INPUT_CHECK_OUTPUT")??"analysis/camera-mapping/b3-input-lifecycle-source-20261019/input-verification");Directory.CreateDirectory(output);try{Run(output);File.WriteAllText(Path.Combine(output,"checks.json"),JsonSerializer.Serialize(new{passed=true,checks,originalRuntime=false,scope="actual CameraMapping runtime and injected numeric reader; source-confirmed failure/cache order"}));Console.WriteLine($"PASS {checks} camera mapping input lifecycle checks");return 0;}catch(Exception error){Console.Error.WriteLine(error);return 1;}}
 static void Run(string output){
  using var scope=new ModuleExecutionScope();var plugin=new CameraMappingPlugin();var runtime=plugin.CreateRuntime();
  var parameters=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());
  var reader=new Reader(Original);scope.RegisterService<IToolNumericInputReader>(reader);
  ModuleResult Execute()=>runtime.ExecuteAsync(new("input-check",output,new Dictionary<string,PortValue>(),parameters){Scope=scope.ForProcedure("input")},default).Result;
  var first=Execute();Check(first.Status==ModuleStatus.Success,"optional numeric reader feeds actual production runtime without materialized inputs");
  string[] expected=Leaves.Select(n=>n+":0").Concat(new[]{"TargetPointX2:0","TargetPointY2:0","ObjectPointX2:0","ObjectPointY2:0"}).Concat(Enumerable.Range(0,4).SelectMany(i=>Leaves.Select(n=>n+":"+i))).ToArray();
  Check(reader.Calls.SequenceEqual(expected),"count probes precede all point values, with exact TargetX TargetY ObjectX ObjectY order");
  reader.Calls.Clear();reader.Override=(name,index)=>name=="TargetPointY1"?new(0,3,0):null;
  var mismatch=Execute();Check(mismatch.NativeErrorCode==0xe0000001,"mismatched target count returns original invalid parameter error");Check(reader.Calls.SequenceEqual(Leaves.Take(2).Select(n=>n+":0")),"target count mismatch stops before reading object leaves");
  foreach(uint error in new[]{0xe000031bu,0xe000031cu,0xe0000014u}){
   reader.Calls.Clear();reader.Override=(name,index)=>name=="TargetPointX1"?new(error,4,0):null;
   var skipped=Execute();Check(skipped.NativeErrorCode==0xe0000309,"count error is folded into source status and excludes equal-count group: "+error.ToString("x"));Check(reader.Calls.Count==8,"excluded group receives no value pass");
  }
  reader.Override=null;parameters["Data1"]=JsonSerializer.SerializeToElement("Data1#0");parameters["Data2"]=JsonSerializer.SerializeToElement("Data2#0");reader.Calls.Clear();Check(Execute().NativeErrorCode==0xe00000ff&&reader.Calls.Count==0,"no active group preserves source no-input error without any reader calls");
  parameters["Data1"]=JsonSerializer.SerializeToElement("Data1#1");parameters["Data2"]=JsonSerializer.SerializeToElement("Data2#1");
  foreach(string failure in Leaves){
   reader.Points=Original;reader.Override=null;Check(Execute().Status==ModuleStatus.Success,"establish full cache before "+failure);
   var changed=Original.Select(p=>new CameraMappingPair(p.ObjectX+2,p.ObjectY+2,p.TargetX+67,p.TargetY+18)).ToArray();reader.Points=changed;reader.Calls.Clear();reader.Override=(name,index)=>name==failure&&index==2?new(0xe0000314,4,0):null;
   var failed=Execute();Check(failed.NativeErrorCode==0xe0000314&&failed.Outputs.Count==0,"value-stage native error propagates without fabricated output: "+failure);Check(reader.Calls.Last()==failure+":2","value-stage failure stops at first failing leaf: "+failure);
   var node=new NodeDocument("input-check",CameraMappingPlugin.Id,"1","Input",parameters);var editor=new ToolEditorContext(node,output,_=>{},_=>throw new InvalidOperationException()){Scope=scope.ForProcedure("input")};
   bool invalid=false;try{plugin.SaveCalibration(editor,Path.Combine(output,"invalid-"+failure+".iwcal"));}catch(CameraMappingOperationException error){invalid=error.NativeErrorCode==0xe0000018;}Check(invalid,"failed input read invalidates explicit save: "+failure);
   reader.Points=Original.Take(2).ToArray();reader.Override=null;Check(Execute().Status==ModuleStatus.Success,"shorter successful run revalidates without shrinking capacity: "+failure);
   int stop=Array.IndexOf(Leaves,failure);var retained=Original.ToArray();var partial=retained[2];if(stop>0)partial=partial with{TargetX=changed[2].TargetX};if(stop>1)partial=partial with{TargetY=changed[2].TargetY};if(stop>2)partial=partial with{ObjectX=changed[2].ObjectX};retained[2]=partial;
   string file=Path.Combine(output,"partial-"+failure+".iwcal");plugin.SaveCalibration(editor,file);var saved=CameraMappingFile.Read(file);var wanted=CameraMappingSolver.Solve(retained,1,3,20);Check(wanted.Success&&saved.Zip(wanted.Matrix).All(p=>Math.Abs(p.First-p.Second)<.003),"explicit save observes retained capacity and only source-confirmed successful partial fields: "+failure);
  }
  using var other=new ModuleExecutionScope();reader.Calls.Clear();var isolated=runtime.ExecuteAsync(new("input-check",output,new Dictionary<string,PortValue>(),parameters){Scope=other.ForProcedure("input")},default).Result;Check(isolated.NativeErrorCode==0xe0000309&&reader.Calls.Count==0,"reader service is confined to its execution scope");
 }
 sealed class Reader(CameraMappingPair[] points):IToolNumericInputReader {
  public CameraMappingPair[] Points=points;public readonly List<string> Calls=[];public Func<string,int,ToolNumericInputReadResult?>? Override;
  public ToolNumericInputReadResult Read(ModuleExecutionContext context,string inputName,int index){Calls.Add(inputName+":"+index);if(Override?.Invoke(inputName,index) is {} result)return result;if(!inputName.EndsWith('1'))return new(0xe0000314,0,0);if(index>=Points.Length)return new(0xe0000014,Points.Length,0);var p=Points[index];double value=inputName switch{"TargetPointX1"=>p.TargetX,"TargetPointY1"=>p.TargetY,"ObjectPointX1"=>p.ObjectX,"ObjectPointY1"=>p.ObjectY,_=>throw new InvalidOperationException(inputName)};return new(0,Points.Length,value);}
 }
}
