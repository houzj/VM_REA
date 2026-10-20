using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;

static class Program
{
    const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

    [STAThread]
    static int Main()
    {
        MainWindow? window = null;
        using var entered = new ManualResetEventSlim();
        using var release = new ManualResetEventSlim();
        try
        {
            AppContext.SetSwitch("VisionEditor.DisableAutomaticCameraDiscovery", true);
            var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
            window = new MainWindow { ShowActivated = false, ShowInTaskbar = false, Left = -20000, Top = -20000 };
            var modules = (ModuleRegistry)typeof(MainWindow).GetField("modules", Private)!.GetValue(window)!;
            var document = (GraphEditorDocument)typeof(MainWindow).GetField("doc", Private)!.GetValue(window)!;
            var descriptor = new ModuleDescriptor("test.workflow-dispatch", "1", "受控同步模块", [], [], []);
            modules.Register(descriptor, () => new BlockingModule(descriptor, entered, release));
            document.NewSolution();
            document.AddNode(descriptor.Id, "1", 100, 100);
            window.Show();

            var uiThread = Environment.CurrentManagedThreadId;
            var callTime = Stopwatch.StartNew();
            var run = (Task)typeof(MainWindow).GetMethod("StartRunCore", Private)!.Invoke(window, [false, null])!;
            callTime.Stop();
            if (callTime.Elapsed > TimeSpan.FromSeconds(2))
                throw new Exception("The synchronous workflow blocked the UI before returning a Task.");
            PumpUntil(() => entered.IsSet);
            var pulse = false;
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => pulse = true));
            PumpUntil(() => pulse);
            if (run.IsCompleted)
                throw new Exception("The workflow finished before the controlled module was released.");
            Console.WriteLine($"PASS UI dispatcher handled a pulse while the synchronous module was pending; UI thread={uiThread}, module thread={BlockingModule.LastThread}");
            if (BlockingModule.LastThread == uiThread)
                throw new Exception("The synchronous module still ran on the UI thread.");
            release.Set();
            PumpUntil(() => run.IsCompleted);
            run.GetAwaiter().GetResult();
            Console.WriteLine("PASS workflow completed and returned ownership after the worker was released.");
            window.Close();
            app.Shutdown();
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error);
            return 1;
        }
        finally
        {
            release.Set();
            window?.Close();
        }
    }

    static void PumpUntil(Func<bool> condition)
    {
        var timeout = Stopwatch.StartNew();
        while (!condition() && timeout.Elapsed < TimeSpan.FromSeconds(10))
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => frame.Continue = false));
            Dispatcher.PushFrame(frame);
        }
        if (!condition()) throw new TimeoutException("The controlled UI operation did not settle.");
    }
}

sealed class BlockingModule(ModuleDescriptor descriptor, ManualResetEventSlim entered, ManualResetEventSlim release) : IModule
{
    public static int LastThread;
    public ModuleDescriptor Descriptor => descriptor;

    public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context, CancellationToken cancellationToken)
    {
        LastThread = Environment.CurrentManagedThreadId;
        entered.Set();
        if (!release.Wait(TimeSpan.FromSeconds(10))) throw new TimeoutException("Controlled module was not released.");
        return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success, new Dictionary<string, PortValue>()));
    }
}
