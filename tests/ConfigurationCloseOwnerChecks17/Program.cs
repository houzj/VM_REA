using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;

static class Program
{
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 [STAThread] static int Main(string[] args)
 {
  try{
   if(args.Length!=1)throw new ArgumentException("Supply an owned audit directory.");
   string output=Path.Combine(Path.GetFullPath(args[0]),"configuration-close-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
   AppContext.SetSwitch("VisionEditor.DisableAutomaticCameraDiscovery",true);
   var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
   var checks=new List<string>();void Check(bool value,string label){if(!value)throw new Exception(label);checks.Add(label);Console.WriteLine("PASS "+label);}
   var window=new MainWindow();object Field(string name)=>typeof(MainWindow).GetField(name,Private)!.GetValue(window)!;
   object? Call(string name,params object[] values)=>typeof(MainWindow).GetMethod(name,Private)!.Invoke(window,values);
   var doc=(GraphEditorDocument)Field("doc");var modules=(ModuleRegistry)Field("modules");
   ToolEditorContext Context(string target){var basic=new ToolEditorContext(doc.CurrentProcedure.Nodes.Single(n=>n.Id==target),doc.BaseDirectory,_=>{},_=>throw new Exception("Base run must be replaced")){ConfigurationActive=()=>true};return (ToolEditorContext)Call("ConnectToolEditorServices",basic,target)!;}
   var phaseEntered=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);var phaseRelease=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
   var phaseDescriptor=new ModuleDescriptor("test.config-replace-counters","1","config-replace-counters",[],[],[]);modules.Register(phaseDescriptor,()=>new PendingConfigurationModule(phaseDescriptor,phaseEntered,phaseRelease));
   doc.NewSolution();string phaseNode=doc.AddNode(phaseDescriptor.Id,"1",100,100);string phaseProcedure=doc.CurrentProcedureId;string phaseSolution=doc.SolutionId;
   string phaseFile=Path.Combine(output,"same-id.vmx.json");doc.Save(phaseFile);
   var oldPhase=Context(phaseNode).Run(new());PumpUntil(phaseEntered.Task);var oldRelease=phaseRelease;
   doc.Load(phaseFile);
   var counts=(Dictionary<string,int>)Field("configurationRunsByProcedure");
   Check(doc.SolutionId==phaseSolution&&doc.CurrentProcedureId==phaseProcedure&&doc.CurrentProcedure.Nodes.Single().Id==phaseNode,"Pending configuration replacement reuses all document identities");
   Check((int)Field("activeConfigurationRuns")==0&&counts.Count==0,"Replacement resets new-generation configuration counts");
   phaseEntered=new(TaskCreationOptions.RunContinuationsAsynchronously);phaseRelease=new(TaskCreationOptions.RunContinuationsAsynchronously);
   var newPhase=Context(phaseNode).Run(new());PumpUntil(phaseEntered.Task);
   Check((int)Field("activeConfigurationRuns")==1&&counts.GetValueOrDefault(phaseProcedure)==1,"New configuration owns exactly its own pending count");
   oldRelease.SetResult();PumpUntil(oldPhase);
   Check((int)Field("activeConfigurationRuns")==1&&counts.GetValueOrDefault(phaseProcedure)==1,"Old same-ID finally cannot decrement new pending counts");
   var phaseResults=(Dictionary<string,Dictionary<string,NodeRunRecord>>)Field("configurationResults");
   Check(phaseResults.GetValueOrDefault(phaseProcedure)?.ContainsKey(phaseNode)!=true,"Old same-ID configuration result remains unpublished");
   phaseRelease.SetResult();PumpUntil(newPhase);
   Check((int)Field("activeConfigurationRuns")==0&&counts.Count==0,"Current configuration finally releases its own count exactly once");
   Check(phaseResults.GetValueOrDefault(phaseProcedure)?.ContainsKey(phaseNode)==true,"Current configuration still publishes its result");
   var entered=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);var release=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
   var descriptor=new ModuleDescriptor("test.pending-config-close","1","pending-config-close",[],[],[]);modules.Register(descriptor,()=>new PendingConfigurationModule(descriptor,entered,release));
   doc.NewSolution();string node=doc.AddNode(descriptor.Id,"1",100,100);string procedure=doc.CurrentProcedureId;
   var scope=(ModuleExecutionScope)Field("executionScope");int disposed=0;scope.RegisterService<IOwnedService>(new OwnedService(()=>disposed++));
   var epoch=(ExecutionOwnerEpoch)Field("executionOwnerEpoch");var stamp=epoch.Capture(doc.SolutionId,scope);
   var source=new ToolEditorContext(doc.CurrentProcedure.Nodes.Single(),doc.BaseDirectory,_=>{},_=>throw new Exception("Base run must be replaced")){ConfigurationActive=()=>true};
   var context=(ToolEditorContext)Call("ConnectToolEditorServices",source,node)!;
   var running=context.Run(new());PumpUntil(entered.Task);
   Check((int)Field("activeConfigurationRuns")==1,"Actual configuration run is pending with owned scope lease");
   Check(Field("activeRun") is null,"Fixture isolates configuration completion rather than workflow drain");
   window.ShowActivated=false;window.ShowInTaskbar=false;window.Left=-20000;window.Top=-20000;window.Show();
   var closed=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);window.Closed+=(_,_)=>closed.TrySetResult();window.Close();PumpUntil(closed.Task);
   Check(!running.IsCompleted,"Host close safely invalidates ownership without awaiting configuration completion");
   Check(!epoch.IsCurrent(stamp,doc.SolutionId,scope),"Actual OnClosed epoch rejects old pending configuration owner");
   Check(disposed==0,"Closed scope keeps owned service alive until pending configuration lease exits");
   int closedPendingCount=(int)Field("activeConfigurationRuns");
   release.SetResult();PumpUntil(running);
   var results=(Dictionary<string,Dictionary<string,NodeRunRecord>>)Field("configurationResults");
   Check(results.GetValueOrDefault(procedure)?.ContainsKey(node)!=true,"Late configuration completion does not publish after actual host close");
   Check(disposed==1,"Lease finally releases owned service once after host close");
   Check((int)Field("activeConfigurationRuns")==closedPendingCount,"Old finally does not mutate the invalidated closed generation counter");
   scope.Dispose();Check(disposed==1,"Repeated scope disposal cannot double-release owned service");
   app.Shutdown();File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=checks.Count,checks,scope="actual WPF main close with controlled configuration task and owned lease; safe invalidation/deferred release, no claim close waits configuration, SDK or plugin window parity"},new JsonSerializerOptions{WriteIndented=true}));
   Console.WriteLine("PASS "+checks.Count+" configuration-close checks; audit="+output);return 0;
  }catch(Exception error){Console.Error.WriteLine(error);return 1;}
 }
 static void PumpUntil(Task task){var timer=Stopwatch.StartNew();while(!task.IsCompleted&&timer.Elapsed<TimeSpan.FromSeconds(25)){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);Thread.Sleep(1);}if(!task.IsCompleted)throw new TimeoutException("Controlled configuration close timed out.");task.GetAwaiter().GetResult();}
}
interface IOwnedService { }
sealed class OwnedService(Action release):IOwnedService,IDisposable {public void Dispose()=>release();}
sealed class PendingConfigurationModule(ModuleDescriptor descriptor,TaskCompletionSource entered,TaskCompletionSource release):IModule
{
 public ModuleDescriptor Descriptor=>descriptor;
 public async ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token){entered.TrySetResult();await release.Task;return new(ModuleStatus.Success,new Dictionary<string,PortValue>());}
}
