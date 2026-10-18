using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using VisionEditor;
using VisionFramework;

internal static class Program
{
 [STAThread] static void Main()
 {
  var count=0;void Check(bool ok,string name){if(!ok)throw new Exception(name);Console.WriteLine("PASS "+name);count++;}
  var registry=new ModuleRegistry();WorkflowCalls.Register(registry);
  var descriptor=new ModuleDescriptor("test.group.stop-recovery","1","慢速工具",[],[new("value",DataKind.Integer)],[]);
  var state=new ProbeState();registry.Register(descriptor,()=>new Probe(descriptor,state));
  var body=new ProcedureDocument("body","Group",[new("probe",descriptor.Id,"1","慢速工具",new())],[],[]){GroupOwner=new("main","group"),Configuration=new(){Outputs=[new("out","out",DataKind.Integer,"probe","value")]}};
  var group=new NodeDocument("group",WorkflowCalls.ModuleId,WorkflowCalls.Version,"Group",new(){["EnableLoop"]=JsonSerializer.SerializeToElement(false)}){Call=WorkflowCalls.From(body)};
  var parent=new ProcedureDocument("main","流程",[group],[],[]);var solution=new SolutionDocument(SolutionStore.CurrentVersion,"stop","stop",[parent,body],[]);
  ExecutionSessionResult? result=null;CancellationTokenSource? cancellation=null;var stops=0;
  async Task Run(){using var token=new CancellationTokenSource();cancellation=token;result=await new GroupDebugExecutor(registry).RunAsync(new(solution,Environment.CurrentDirectory),"main","group",cancellationToken:token.Token);cancellation=null;}
  void Stop(){stops++;cancellation?.Cancel();}
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
  var window=new SubprogramCallWindow(()=>parent,()=>group,()=>solution.Procedures,_=>{},(_,_)=>{},_=>[],Run,Stop,()=>result){ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};window.Show();Pump();
  Button Button(string name)=> (Button)typeof(SubprogramCallWindow).GetField(name,BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!;
  TextBlock Error()=> (TextBlock)typeof(SubprogramCallWindow).GetField("Error",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!;
  void Click(Button button)=>button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
  void Until(Func<bool> condition){for(var i=0;i<300&&!condition();i++){Pump();Thread.Sleep(10);}if(!condition())throw new Exception("WPF run did not reach expected state");}
  state.HoldNext();Click(Button("Repeat"));Until(()=>state.Entered==1);Check(window.IsExecuting,"Continuous Group run enters actual module");
  Click(Button("Stop"));Check(stops==0&&window.IsExecuting,"Stop clears repeat request without cancelling in-flight native-style SelfRun");
  state.Release();Until(()=>!window.IsExecuting);Check(state.Entered==1&&result?.Status==SessionStatus.Success,"Continuous Stop lets current Group result complete and prevents next iteration");
  Click(Button("RunOnce"));Until(()=>!window.IsExecuting);Check(state.Entered==2&&result?.Status==SessionStatus.Success&&result.Calls["group"].WorkflowOutputs["out"].As<long>()==2,"Single run remains available after continuous Stop with actual output");
  state.FailNext=true;Click(Button("RunOnce"));Until(()=>!window.IsExecuting);Check(state.Entered==3&&result?.Status==SessionStatus.Failed&&Error().Text.Contains("Failed"),"Failed Group run exits busy state and shows error");
  Click(Button("RunOnce"));Until(()=>!window.IsExecuting);Check(state.Entered==4&&result?.Status==SessionStatus.Success&&result.Calls["group"].WorkflowOutputs["out"].As<long>()==4&&Error().Text.Length==0,"Group run recovers from failure and refreshes successful output");
  state.HoldNext();Click(Button("RunOnce"));Until(()=>state.Entered==5);Click(Button("Stop"));Check(stops==0&&window.IsExecuting,"Stop during one-shot does not cancel the current run");state.Release();Until(()=>!window.IsExecuting);Check(result?.Status==SessionStatus.Success,"One-shot result is retained after Stop click");
  state.HoldNext();Click(Button("RunOnce"));Until(()=>state.Entered==6);window.Close();Until(()=>!window.IsVisible);Check(stops==1,"Closing a running Group window still cancels and cleans up the active task");
  app.Shutdown();Console.WriteLine($"PASS {count} Group stop/recovery checks");
 }
 static void Pump(){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);}
}
sealed class ProbeState
{
 TaskCompletionSource? gate;public int Entered;public bool FailNext;
 public void HoldNext()=>gate=new(TaskCreationOptions.RunContinuationsAsynchronously);
 public void Release(){gate?.TrySetResult();gate=null;}
 public async Task<ModuleResult> Run(CancellationToken token){Interlocked.Increment(ref Entered);var wait=gate;if(wait is not null)await wait.Task.WaitAsync(token);if(FailNext){FailNext=false;return new(ModuleStatus.Failed,new Dictionary<string,PortValue>(),"ProbeFailed","controlled failure");}return new(ModuleStatus.Success,new Dictionary<string,PortValue>{{"value",PortValue.Integer(Entered)}});}
}
sealed class Probe(ModuleDescriptor descriptor,ProbeState state):IModule{public ModuleDescriptor Descriptor=>descriptor;public async ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>await state.Run(token);}
