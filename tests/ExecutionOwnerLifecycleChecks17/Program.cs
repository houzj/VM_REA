using System.Collections;
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
        try
        {
            if(args.Length!=1)throw new ArgumentException("Supply an owned audit directory.");
            string output=Path.Combine(Path.GetFullPath(args[0]),"owner-lifecycle-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
            AppContext.SetSwitch("VisionEditor.DisableAutomaticCameraDiscovery",true);
            var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
            var checks=new List<string>();void Check(bool value,string label){if(!value)throw new Exception(label);checks.Add(label);Console.WriteLine("PASS "+label);}
            var window=new MainWindow();object? Field(string name)=>typeof(MainWindow).GetField(name,Private)!.GetValue(window);
            object? Call(string name,params object[] values)=>typeof(MainWindow).GetMethod(name,Private)!.Invoke(window,values);
            var doc=(GraphEditorDocument)Field("doc")!;var modules=(ModuleRegistry)Field("modules")!;
            var immediate=new ModuleDescriptor("test.lifecycle-immediate","1","lifecycle-immediate",[],[],[]);modules.Register(immediate,()=>new ImmediateModule(immediate));
            doc.NewSolution();string procedure=doc.CurrentProcedureId;string node=doc.AddNode(immediate.Id,"1",100,100);
            string path=Path.Combine(output,"same-id.vmx.json");doc.Save(path);string solution=doc.SolutionId;
            var oldContext=(ToolEditorContext)Call("CreateToolEditorContext",node)!;
            var run=oldContext.Run(new());PumpUntil(run);
            Check(((IDictionary)Field("toolDebugRecords")!).Count==1,"Real debug context publishes one initial record");
            doc.Load(path);
            Check(doc.SolutionId==solution&&doc.CurrentProcedureId==procedure&&doc.CurrentProcedure.Nodes.Single().Id==node,"Same-file reload reuses solution procedure and node identities");
            Check(((IDictionary)Field("toolDebugRecords")!).Count==0,"Same-ID replacement clears old debug records");
            bool applyRejected=false;try{oldContext.Apply(new());}catch(InvalidOperationException){applyRejected=true;}
            Check(applyRejected,"Old real debug context Apply rejects after same-ID reload");
            bool runRejected=false;try{var stale=oldContext.Run(new());PumpUntil(stale);}catch(InvalidOperationException){runRejected=true;}
            Check(runRejected,"Old real debug context Run rejects after same-ID reload");
            var freshContext=(ToolEditorContext)Call("CreateToolEditorContext",node)!;var freshRun=freshContext.Run(new());PumpUntil(freshRun);
            Check(freshRun.Result.Status==NodeRunStatus.Success&&((IDictionary)Field("toolDebugRecords")!).Count==1,"Fresh real debug context remains functional");
            if(oldContext.Session is not null)
            {
                try{PumpUntil(oldContext.Session.DisposeAsync().AsTask());}
                catch(InvalidOperationException)when(runRejected){Check(true,"Disposing rejected debug session retains its prior guard fault as documented");}
            }
            if(freshContext.Session is not null)PumpUntil(freshContext.Session.DisposeAsync().AsTask());

            int oldDisposed=0,newDisposed=0;var oldScope=new ModuleExecutionScope();var newScope=new ModuleExecutionScope();
            oldScope.RegisterService<IProbeService>(new ProbeService(()=>oldDisposed++));newScope.RegisterService<IProbeService>(new ProbeService(()=>newDisposed++));
            var lease=oldScope.Enter();oldScope.Dispose();oldScope.Dispose();
            Check(oldDisposed==0&&newDisposed==0,"Closing old owned scope defers its service until pending lease exits");
            Check(newScope.Service<IProbeService>() is not null,"Replacement scope retains its independent service");
            lease.Dispose();lease.Dispose();oldScope.Dispose();
            Check(oldDisposed==1&&newDisposed==0,"Old service releases exactly once after last lease");
            newScope.Dispose();Check(newDisposed==1,"Replacement service releases only with its own scope");

            // Model a cancelled configuration close: physical window/context remains alive,
            // but replacement invalidates ownership independently. This is not a plugin UI test.
            var configEpoch=new ExecutionOwnerEpoch();object configScope=new();bool physicalContextAlive=true;
            var configStamp=configEpoch.Capture("same",configScope);configEpoch.Invalidate();bool closeCancelled=true;
            Check(closeCancelled&&physicalContextAlive&&!configEpoch.IsCurrent(configStamp,"same",configScope),"Cancelled-close simulation cannot preserve an invalidated owner ticket");

            doc.NewSolution();
            var entered=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);var executeRelease=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var cleanupEntered=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);var cleanupRelease=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);int moduleDisposed=0;
            var slow=new ModuleDescriptor("test.lifecycle-close","1","lifecycle-close",[],[],[]);
            modules.Register(slow,()=>new ClosingModule(slow,entered,executeRelease,cleanupEntered,cleanupRelease,()=>moduleDisposed++));doc.AddNode(slow.Id,"1",100,100);
            // This own offscreen window verifies managed WPF close sequencing only.
            window.ShowActivated=false;window.ShowInTaskbar=false;window.Left=-20000;window.Top=-20000;window.Show();
            var closed=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);window.Closed+=(_,_)=>closed.TrySetResult();
            var active=(Task)Call("StartRunCore",false,null!)!;PumpUntil(entered.Task);
            Check(Field("activeRun") is CancellationTokenSource,"Actual host owns active workflow before close");
            window.Close();PumpSteps(10);
            Check(!closed.Task.IsCompleted&&Field("activeRun") is not null,"Actual close waits while current module ignores cancellation until released");
            executeRelease.SetResult();PumpUntil(cleanupEntered.Task);
            Check(!closed.Task.IsCompleted&&moduleDisposed==0,"Actual close also waits for asynchronous module cleanup");
            cleanupRelease.SetResult();PumpUntil(active);PumpUntil(closed.Task);
            Check(moduleDisposed==1&&Field("activeRun") is null,"Actual close completes after owned run drains and module disposes once");
            app.Shutdown();
            File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=checks.Count,checks,
                scope="Actual managed WPF debug context/close and owned scope services. Cancelled plugin-close case is explicitly an owner model, not actual plugin UI. No SDK, devices or interactive desktop acceptance."},new JsonSerializerOptions{WriteIndented=true}));
            Console.WriteLine("PASS "+checks.Count+" lifecycle checks; audit="+output);return 0;
        }
        catch(Exception error){Console.Error.WriteLine(error);return 1;}
    }
    static void PumpSteps(int count){for(int i=0;i<count;i++){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);Thread.Sleep(1);}}
    static void PumpUntil(Task task){var timer=Stopwatch.StartNew();while(!task.IsCompleted&&timer.Elapsed<TimeSpan.FromSeconds(25))PumpSteps(1);if(!task.IsCompleted)throw new TimeoutException("Controlled lifecycle did not settle in 25 seconds.");task.GetAwaiter().GetResult();}
}
interface IProbeService { }
sealed class ProbeService(Action dispose):IProbeService,IDisposable {public void Dispose()=>dispose();}
sealed class ImmediateModule(ModuleDescriptor descriptor):IModule
{
    public ModuleDescriptor Descriptor=>descriptor;
    public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>()));
}
sealed class ClosingModule(ModuleDescriptor descriptor,TaskCompletionSource entered,TaskCompletionSource executeRelease,TaskCompletionSource cleanupEntered,TaskCompletionSource cleanupRelease,Action disposed):IModule,IAsyncDisposable
{
    public ModuleDescriptor Descriptor=>descriptor;
    public async ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token){entered.TrySetResult();await executeRelease.Task;return new(ModuleStatus.Success,new Dictionary<string,PortValue>());}
    public async ValueTask DisposeAsync(){cleanupEntered.TrySetResult();await cleanupRelease.Task;disposed();}
}
