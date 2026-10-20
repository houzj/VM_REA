using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;

static class Program
{
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 [STAThread] static int Main(string[] args)
 {
  string? output=null;
  try{
   AppContext.SetSwitch("VisionEditor.DisableAutomaticCameraDiscovery",true);
   if(args.Length!=1)throw new ArgumentException("Supply an owned audit directory.");
   output=Path.Combine(Path.GetFullPath(args[0]),"host-recovery-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
   var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
   var checks=new List<string>();var observations=new List<object>();void Check(bool value,string label){if(!value)throw new Exception(label);checks.Add(label);Console.WriteLine("PASS "+label);}
   var window=new MainWindow();object? Field(string name)=>typeof(MainWindow).GetField(name,Private)!.GetValue(window);
   object? Call(string name,params object[] values)=>typeof(MainWindow).GetMethod(name,Private)!.Invoke(window,values);
   var doc=(GraphEditorDocument)Field("doc")!;var modules=(ModuleRegistry)Field("modules")!;
   var sessions=(Dictionary<string,ExecutionSessionResult>)Field("workflowSessions")!;
   var plan=new RecoveryPlan();var descriptor=new ModuleDescriptor("test.host-recovery","1","host-recovery",[],[new("Value",DataKind.Number)],[]);
   modules.Register(descriptor,()=>new RecoveryModule(descriptor,plan));
   string Setup(string mode,bool stopOnNg){doc.NewSolution();plan.Reset(mode);string id=doc.AddNode(descriptor.Id,"1",100,100);doc.SetProcedureRunPolicy(doc.CurrentProcedureId,1,stopOnNg);if(mode=="Timeout")doc.SetTimeout(id,40);return id;}
   bool Idle()=>Field("activeRun") is null&&Field("runningProcedureId") is null&&!(bool)Field("continuousRun")!;
   void Verify(string label,string node,NodeRunStatus expected,string? code){var s=sessions[doc.CurrentProcedureId];var record=s.Nodes.Single(x=>x.NodeId==node);Check(record.Status==expected&&(code is null||record.ErrorCode==code),label+" publishes supported node status/code");Check(Idle(),label+" releases actual activeRun owner and loop state");observations.Add(new{label,sessionStatus=s.Status,nodeStatus=record.Status,record.ErrorCode,record.Message,plan.Calls,plan.DisposeStarted,plan.DisposeCompleted,statusText=((TextBlock)window.FindName("StatusText")).Text});}
   var faults=new[]{("Throw",NodeRunStatus.Failed,"ModuleException"),("Failed",NodeRunStatus.Failed,"FixtureFailure"),("InvalidOutput",NodeRunStatus.Failed,"InvalidOutput"),("DisposeFault",NodeRunStatus.Failed,"ModuleException"),("Cancel",NodeRunStatus.Cancelled,"Cancelled"),("Timeout",NodeRunStatus.TimedOut,"Timeout")};
   foreach(var item in faults){
    string node=Setup(item.Item1,false);var run=(Task)Call("StartRunCore",false,null!)!;
    if(item.Item1=="Cancel"){PumpUntil(plan.Entered.Task);((CancellationTokenSource)Field("activeRun")!).Cancel();}
    PumpUntil(run);Verify("single "+item.Item1,node,item.Item2,item.Item3);
    Check(plan.DisposeStarted==1&&plan.DisposeCompleted==1,"single "+item.Item1+" awaits one module cleanup even on fault");
    plan.Reset("Success");doc.SetTimeout(node,0);var restart=(Task)Call("StartRunCore",false,null!)!;PumpUntil(restart);
    Verify("restart after "+item.Item1,node,NodeRunStatus.Success,null);
    Check(((FrameworkRunHistory)Field("frameworkRunHistory")!).Entries.Count==2,"restart after "+item.Item1+" records a new actual host run");
   }
   foreach(var item in faults){
    string node=Setup(item.Item1,true);Call("StartContinuous");
    if(item.Item1=="Cancel"){PumpUntil(plan.Entered.Task);((CancellationTokenSource)Field("activeRun")!).Cancel();}
    Wait(Idle);Verify("continuous stop-on-NG "+item.Item1,node,item.Item2,item.Item3);
    Check(plan.Calls==1,"continuous stop-on-NG "+item.Item1+" cannot launch a second failed iteration");
    plan.Reset("Success");doc.SetTimeout(node,0);PumpUntil((Task)Call("StartRunCore",false,null!)!);
    Verify("restart after continuous "+item.Item1,node,NodeRunStatus.Success,null);
   }
   foreach(var item in faults.Where(x=>x.Item1!="Cancel")){
    string node=Setup(item.Item1,false);plan.StopAfterSuccess=true;Call("StartContinuous");Wait(Idle);
    Verify("continuous continue-on-NG "+item.Item1,node,NodeRunStatus.Success,null);
    Check(plan.Calls==2,"continuous continue-on-NG "+item.Item1+" recovers on next actual iteration");
    var history=(FrameworkRunHistory)Field("frameworkRunHistory")!;
    Check(history.Entries.Count==2&&history.Entries[0].Data.EndsWith("Success",StringComparison.Ordinal)&&!history.Entries[1].Data.EndsWith("Success",StringComparison.Ordinal),"continuous "+item.Item1+" retains fault then recovery in real history");
   }
   Setup("Close",false);int serviceDisposed=0;var scope=(ModuleExecutionScope)Field("executionScope")!;scope.RegisterService<IOwnedService>(new OwnedService(()=>serviceDisposed++));
   window.ShowActivated=false;window.ShowInTaskbar=false;window.Left=-20000;window.Top=-20000;window.Show();
   var closed=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);window.Closed+=(_,_)=>closed.TrySetResult();
   var closing=(Task)Call("StartRunCore",false,null!)!;PumpUntil(plan.Entered.Task);window.Close();PumpSteps(5);
   Check(!closed.Task.IsCompleted&&Field("activeRun") is not null&&serviceDisposed==0,"Actual close requests cancellation but waits for owned execution and scope");
   plan.ExecuteRelease.SetResult();PumpUntil(plan.CleanupEntered.Task);
   Check(!closed.Task.IsCompleted&&plan.DisposeCompleted==0&&serviceDisposed==0,"Actual close waits asynchronous module cleanup before scope disposal");
   plan.CleanupRelease.SetResult();PumpUntil(closing);PumpUntil(closed.Task);
   Check(Idle()&&plan.DisposeCompleted==1&&serviceDisposed==1,"Actual close drains owner and releases module and owned scope service once");
   app.Shutdown();
   File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=checks.Count,checks,observations,
     scope="Actual managed MainWindow StartRunCore/continuous/close with controlled modules; finite host fault/restart matrix only. No 30 minute stability, concurrency growth, native SDK/device or interactive desktop parity claim."},new JsonSerializerOptions{WriteIndented=true}));
   Console.WriteLine("PASS "+checks.Count+" actual host recovery checks; audit="+output);return 0;
  }catch(Exception error){try{if(output is not null)File.WriteAllText(Path.Combine(output,"failed.json"),JsonSerializer.Serialize(new{failed=true,error=error.ToString()},new JsonSerializerOptions{WriteIndented=true}));}catch(Exception e){Console.Error.WriteLine(e);}Console.Error.WriteLine(error);return 1;}
 }
 static void PumpSteps(int count){for(int i=0;i<count;i++){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);Thread.Sleep(1);}}
 static void Wait(Func<bool> complete){var watch=Stopwatch.StartNew();while(!complete()&&watch.Elapsed<TimeSpan.FromSeconds(25))PumpSteps(1);if(!complete())throw new TimeoutException("Controlled host state did not settle within 25 seconds.");}
 static void PumpUntil(Task task){Wait(()=>task.IsCompleted);task.GetAwaiter().GetResult();}
}
sealed class RecoveryPlan
{
 public string Mode="Success";public int Calls,DisposeStarted,DisposeCompleted;public bool StopAfterSuccess;
 public TaskCompletionSource Entered=new(TaskCreationOptions.RunContinuationsAsynchronously),ExecuteRelease=new(TaskCreationOptions.RunContinuationsAsynchronously),CleanupEntered=new(TaskCreationOptions.RunContinuationsAsynchronously),CleanupRelease=new(TaskCreationOptions.RunContinuationsAsynchronously);
 public void Reset(string mode){Mode=mode;Calls=DisposeStarted=DisposeCompleted=0;StopAfterSuccess=false;Entered=new(TaskCreationOptions.RunContinuationsAsynchronously);ExecuteRelease=new(TaskCreationOptions.RunContinuationsAsynchronously);CleanupEntered=new(TaskCreationOptions.RunContinuationsAsynchronously);CleanupRelease=new(TaskCreationOptions.RunContinuationsAsynchronously);}
}
sealed class RecoveryModule(ModuleDescriptor descriptor,RecoveryPlan plan):IModule,IAsyncDisposable
{
 string mode="Success";
 public ModuleDescriptor Descriptor=>descriptor;
 public async ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token){
  mode=Interlocked.Increment(ref plan.Calls)==1?plan.Mode:"Success";plan.Entered.TrySetResult();
  if(mode=="Throw")throw new InvalidOperationException("controlled-module-throw");
  if(mode is "Cancel" or "Timeout")await Task.Delay(Timeout.Infinite,token);
  if(mode=="Close")await plan.ExecuteRelease.Task;
  if(mode=="Failed")return new(ModuleStatus.Failed,new Dictionary<string,PortValue>(),"FixtureFailure","controlled failure");
  if(mode=="InvalidOutput")return new(ModuleStatus.Success,new Dictionary<string,PortValue>{{"Value",PortValue.Text("wrong-type")}});
  if(mode=="Success"&&plan.StopAfterSuccess)context.Control?.RequestStop();
  return new(ModuleStatus.Success,new Dictionary<string,PortValue>{{"Value",PortValue.Number(7)}});
 }
 public async ValueTask DisposeAsync(){Interlocked.Increment(ref plan.DisposeStarted);if(mode=="Close"){plan.CleanupEntered.TrySetResult();await plan.CleanupRelease.Task;}Interlocked.Increment(ref plan.DisposeCompleted);if(mode=="DisposeFault")throw new IOException("controlled-async-dispose-fault");}
}
interface IOwnedService { }
sealed class OwnedService(Action dispose):IOwnedService,IDisposable{public void Dispose()=>dispose();}
