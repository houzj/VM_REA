using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using VisionCore;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.ImageSource;

static class Program
{
    const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    static readonly List<object> Observations = [];

    [STAThread]
    static int Main()
    {
        MainWindow? window = null;
        try
        {
            AppContext.SetSwitch("VisionEditor.DisableAutomaticCameraDiscovery", true);
            var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
            window = new MainWindow { ShowActivated = false, ShowInTaskbar = false, Left = -20000, Top = -20000 };
            var doc = Field<GraphEditorDocument>(window, "doc");
            var registry = Field<ToolPluginRegistry>(window, "plugins");
            var sourcePlugin = (ImageSourcePlugin)registry.Find("vision.image-source")!;
            doc.NewSolution();
            var sourceParameters = sourcePlugin.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
            sourceParameters["ImageSourceType"] = JsonSerializer.SerializeToElement(3);
            sourceParameters["Interval"] = JsonSerializer.SerializeToElement(0);
            var sourceId = doc.AddNode("vision.image-source", sourcePlugin.Manifest.Version, 100, 100, sourceParameters);
            var binaryId = doc.AddNode("vision.image-binary", "1.0.0", 100, 250);
            doc.Connect(sourceId, "Image", binaryId, "Image");
            window.Show();
            var uiThread = Environment.CurrentManagedThreadId;
            var timer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromMilliseconds(10) };
            var ticks = 0;
            timer.Tick += (_, _) => ticks++;
            timer.Start();

            Task Begin(byte[] pixels)
            {
                var scope = Field<ModuleExecutionScope>(window, "executionScope");
                sourcePlugin.SubmitImage(scope.ForProcedure(doc.CurrentProcedureId), sourceId, new GrayImage(4096, 4096, pixels));
                return (Task)typeof(MainWindow).GetMethod("StartRunCore", Private)!.Invoke(window, [false, null])!;
            }

            var pixels = Enumerable.Repeat((byte)127, 4096 * 4096).ToArray();
            var fullRunWatch = Stopwatch.StartNew();
            var callWatch = Stopwatch.StartNew();
            var run = Begin(pixels);
            callWatch.Stop();
            var completedAtReturn = run.IsCompleted;
            var tickBefore = ticks;
            PumpUntil(() => ticks > tickBefore || run.IsCompleted);
            var heartbeatsWhilePending = ticks - tickBefore;
            PumpUntil(() => run.IsCompleted);
            run.GetAwaiter().GetResult();
            fullRunWatch.Stop();
            var totalHeartbeatsWhileRun = ticks - tickBefore;
            var sessions = Field<Dictionary<string, ExecutionSessionResult>>(window, "workflowSessions");
            var first = sessions[doc.CurrentProcedureId];
            if (first.Nodes.Single(n => n.NodeId == binaryId).Status != NodeRunStatus.Success)
                throw new Exception("The real image binary chain did not succeed.");
            if (completedAtReturn || heartbeatsWhilePending == 0 || callWatch.Elapsed > TimeSpan.FromSeconds(2))
                throw new Exception("The UI did not respond before the real image run finished.");
            Observations.Add(new { scenario = "normal", uiThread, callReturnMs = callWatch.Elapsed.TotalMilliseconds,
                totalRunMs = fullRunWatch.Elapsed.TotalMilliseconds, completedAtReturn, heartbeatsWhilePending,
                totalHeartbeatsWhileRun, binaryStatus = "Success" });

            var cancelRun = Begin(pixels);
            var cancelWatch = Stopwatch.StartNew();
            var cancelStartTick = ticks;
            PumpUntil(() => ticks >= cancelStartTick + 5 || cancelRun.IsCompleted);
            if (cancelRun.IsCompleted) throw new Exception("Cancellation scenario finished before a cancel request could be observed.");
            Field<CancellationTokenSource>(window, "activeRun").Cancel();
            PumpUntil(() => cancelRun.IsCompleted);
            cancelRun.GetAwaiter().GetResult();
            cancelWatch.Stop();
            var cancelStatus = sessions[doc.CurrentProcedureId].Status;
            if (cancelStatus != SessionStatus.Cancelled)
                throw new Exception("Cancellation did not produce a Cancelled session.");
            Observations.Add(new { scenario = "cancel", cancelCompletionMs = cancelWatch.Elapsed.TotalMilliseconds,
                sessionStatus = cancelStatus.ToString(), uiTicks = ticks });

            var closeRun = Begin(pixels);
            var closeWatch = Stopwatch.StartNew();
            var ownerBeforeClose = (ExecutionOwnerStamp)typeof(MainWindow).GetMethod("CaptureExecutionOwner", Private)!.Invoke(window, null)!;
            var closeStartTick = ticks;
            PumpUntil(() => ticks >= closeStartTick + 2 || closeRun.IsCompleted);
            if (closeRun.IsCompleted) throw new Exception("Close scenario finished before closure could be observed.");
            var scopeBeforeClose = Field<ModuleExecutionScope>(window, "executionScope");
            var scopeStopping = scopeBeforeClose.Stopping;
            window.Close();
            PumpUntil(() => closeRun.IsCompleted);
            closeRun.GetAwaiter().GetResult();
            PumpUntil(() => !window.IsVisible);
            closeWatch.Stop();
            if (!scopeStopping.IsCancellationRequested)
                throw new Exception("Closing did not cancel the old scope.");
            var ownerStillCurrent = (bool)typeof(MainWindow).GetMethod("IsCurrentExecutionOwner", Private)!.Invoke(window, [ownerBeforeClose])!;
            if (!Field<bool>(window, "windowClosed") || ownerStillCurrent)
                throw new Exception("Closing did not invalidate the execution owner.");
            var sessionAfterClose = sessions.GetValueOrDefault(doc.CurrentProcedureId);
            var postClosePulse = false;
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => postClosePulse = true));
            PumpUntil(() => postClosePulse);
            if (!ReferenceEquals(sessionAfterClose, sessions.GetValueOrDefault(doc.CurrentProcedureId)))
                throw new Exception("A late result was published after the window closed.");
            Observations.Add(new { scenario = "close", closeCompletionMs = closeWatch.Elapsed.TotalMilliseconds,
                windowClosed = Field<bool>(window, "windowClosed"), ownerStillCurrent,
                scopeCancelled = true, lateSessionPublished = false, uiTicks = ticks });
            timer.Stop();
            app.Shutdown();
            Console.WriteLine(JsonSerializer.Serialize(Observations, new JsonSerializerOptions { WriteIndented = true }));
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error);
            Console.WriteLine(JsonSerializer.Serialize(Observations, new JsonSerializerOptions { WriteIndented = true }));
            return 1;
        }
        finally { window?.Close(); }
    }

    static T Field<T>(object owner, string name) =>
        (T)owner.GetType().GetField(name, Private)!.GetValue(owner)!;

    static void PumpUntil(Func<bool> ready)
    {
        var watch = Stopwatch.StartNew();
        while (!ready() && watch.Elapsed < TimeSpan.FromSeconds(20))
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => frame.Continue = false));
            Dispatcher.PushFrame(frame);
        }
        if (!ready()) throw new TimeoutException("Controlled WPF operation did not settle within 20 seconds.");
    }
}
