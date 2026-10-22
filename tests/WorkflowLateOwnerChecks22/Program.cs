using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;

static class Program
{
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 [STAThread] static int Main(string[] args)
 {
  if(args.Length!=1)throw new ArgumentException("Supply an owned audit directory.");
  string output=Path.GetFullPath(args[0]);Directory.CreateDirectory(output);
  AppContext.SetSwitch("VisionEditor.DisableAutomaticCameraDiscovery",true);
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
  SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
  MainWindow? window=null;
  try
  {
   window=new MainWindow{ShowActivated=false,ShowInTaskbar=false,Left=-20000,Top=-20000};
   object? Field(string name)=>typeof(MainWindow).GetField(name,Private)!.GetValue(window);
   Task Run()=>(Task)typeof(MainWindow).GetMethod("StartRunCore",Private)!.Invoke(window,[false,null])!;
   var doc=(GraphEditorDocument)Field("doc")!;var modules=(ModuleRegistry)Field("modules")!;
   var gates=new Queue<Gate>();var descriptor=new ModuleDescriptor("test.late-owner-22","1","late-owner",[],[new PortDescriptor("Value",DataKind.Integer)],[]);
   modules.Register(descriptor,()=>new BlockingModule(descriptor,gates.Dequeue()));
   doc.NewSolution();string flow=doc.CurrentProcedureId;string node=doc.AddNode(descriptor.Id,"1",100,100);
   string saved=Path.Combine(output,"same-id.vmx.json");doc.Save(saved);
   window.Show();

   var first=new Gate();gates.Enqueue(first);var stopped=Run();PumpUntil(first.Entered.Task);
   ((CancellationTokenSource)Field("activeRun")!).Cancel();first.Release.TrySetResult();PumpUntil(stopped);
   var sessions=(Dictionary<string,ExecutionSessionResult>)Field("workflowSessions")!;
   if(!sessions.TryGetValue(flow,out var stopSession)||stopSession.Status!=SessionStatus.Cancelled||stopSession.Nodes.Single(n=>n.NodeId==node) is not {Status:NodeRunStatus.Cancelled,Outputs.Count:0})
    throw new Exception("Stopped noncooperative module published its late success output.");
   Console.WriteLine("PASS stop retains a cancelled session without late success output.");

   var second=new Gate();gates.Enqueue(second);var oldRun=Run();PumpUntil(second.Entered.Task);
   doc.Load(saved); // Real replacement event; saved document reuses solution/procedure/node IDs.
   if(doc.CurrentProcedureId!=flow||doc.CurrentProcedure.Nodes.Single().Id!=node||sessions.Count!=0)
    throw new Exception("Same-identity replacement did not clear the old session.");
   second.Release.TrySetResult();PumpUntil(oldRun);PumpSteps(12);
   if(sessions.Count!=0)throw new Exception("Late old-generation session entered replacement cache.");
   Console.WriteLine("PASS same-ID solution replacement rejects late workflow session.");

   var third=new Gate();gates.Enqueue(third);var closingRun=Run();PumpUntil(third.Entered.Task);
   window.Close();PumpSteps(6);
   if(!window.IsVisible||!((bool)Field("closeDrainPending")!)||closingRun.IsCompleted)
    throw new Exception("Window closed before the pending workflow module drained.");
   third.Release.TrySetResult();PumpUntil(closingRun);
   var closeWatch=Stopwatch.StartNew();while(window.IsVisible&&closeWatch.Elapsed<TimeSpan.FromSeconds(25))PumpSteps(1);
   if(window.IsVisible||!(bool)Field("windowClosed")!)throw new Exception("Window failed to finish draining.");
   if(sessions.Values.Any(s=>s.Status==SessionStatus.Success||s.Nodes.Any(n=>n.Outputs.Count!=0)))
    throw new Exception("Closing workflow published late success output.");
   int closedSessionCount=sessions.Count;PumpSteps(12);
   if(sessions.Count!=closedSessionCount)throw new Exception("Workflow session cache changed after window closed.");
   Console.WriteLine("PASS close waits for pending module and rejects late success output.");
   app.Shutdown();
   var root=Directory.GetCurrentDirectory();
   string Hash(string path)=>Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));
   var hostSources=new[]{"src/VisionEditor/WorkflowExecution.cs","src/VisionEditor/ToolExecutionScope.cs","src/VisionEditor/MainWindow.FrameworkExecutionOwnership.cs"};
   var frameworkSources=new[]{"src/VisionFramework/GraphExecutor.cs"};var sourcePaths=hostSources.Concat(frameworkSources).ToArray();
   void CheckPdb(string assemblyPath,IReadOnlyList<string> paths){using var pdb=MetadataReaderProvider.FromPortablePdbStream(File.OpenRead(Path.ChangeExtension(assemblyPath,".pdb")));var reader=pdb.GetMetadataReader();
    var documents=reader.Documents.Select(handle=>reader.GetDocument(handle)).Select(d=>new{path=reader.GetString(d.Name).Replace('\\','/'),hash=reader.GetBlobBytes(d.Hash)}).ToArray();
    foreach(var relative in paths){var matches=documents.Where(d=>d.path.EndsWith("/"+relative,StringComparison.OrdinalIgnoreCase)).ToArray();
     if(matches.Length!=1||!matches[0].hash.SequenceEqual(SHA256.HashData(File.ReadAllBytes(Path.Combine(root,relative)))))throw new Exception("Assembly PDB source checksum does not match "+relative);}}
   CheckPdb(typeof(MainWindow).Assembly.Location,hostSources);CheckPdb(typeof(GraphExecutor).Assembly.Location,frameworkSources);
   var evidence=new{passed=3,stoppedSession=stopSession.Status.ToString(),stoppedNode=stopSession.Nodes.Single().Status.ToString(),sameIdentity=true,lateReplacementSessionCount=0,closeDrained=true,closedSessionCount=sessions.Count,closedSessionStatuses=sessions.Values.Select(s=>s.Status.ToString()).ToArray(),
    source=sourcePaths.Select(p=>new{path=p,sha256=Hash(Path.Combine(root,p))}).ToArray(),hostPdb=Hash(Path.ChangeExtension(typeof(MainWindow).Assembly.Location,".pdb")),frameworkPdb=Hash(Path.ChangeExtension(typeof(GraphExecutor).Assembly.Location,".pdb")),pdbSourceChecks=sourcePaths.Length,
    hostAssembly=Hash(typeof(MainWindow).Assembly.Location),scope="Actual offscreen WPF main host with a controlled noncooperative synthetic module; no vendor SDK, device or interactive desktop."};
   File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(evidence,new JsonSerializerOptions{WriteIndented=true}));
   return 0;
  }
  catch(Exception error){Console.Error.WriteLine(error);return 1;}
  finally{if(window is {IsVisible:true}){window.Close();PumpSteps(4);}app.Shutdown();}
 }
 static void PumpUntil(Task task){var sw=Stopwatch.StartNew();while(!task.IsCompleted&&sw.Elapsed<TimeSpan.FromSeconds(25))PumpSteps(1);if(!task.IsCompleted)throw new TimeoutException("Controlled run did not settle.");task.GetAwaiter().GetResult();}
 static void PumpSteps(int count){for(int i=0;i<count;i++){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);Thread.Sleep(1);}}
 sealed class Gate{public TaskCompletionSource Entered=new(TaskCreationOptions.RunContinuationsAsynchronously);public TaskCompletionSource Release=new(TaskCreationOptions.RunContinuationsAsynchronously);}
 sealed class BlockingModule(ModuleDescriptor descriptor,Gate gate):IModule
 {
  public ModuleDescriptor Descriptor=>descriptor;
  public async ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token){gate.Entered.TrySetResult();await gate.Release.Task;return new(ModuleStatus.Success,new Dictionary<string,PortValue>{{"Value",PortValue.Integer(7)}});}
 }
}
