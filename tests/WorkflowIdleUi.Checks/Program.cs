using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using VisionCore;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.ImageSource;
using VisionPlugins.ImageBinary;

static class Program
{
    const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    static volatile bool active = true;
    static volatile string activity = "setup";
    static long lastPulse;
    static int dumpStarted;
    static bool collectDump;
    static readonly List<object> segments = [];

    [STAThread]
    static int Main(string[] args)
    {
        MainWindow? window = null;
        DispatcherTimer? heartbeat = null;
        var output = Path.GetFullPath(Path.Combine("analysis", "joint-audit", "idle-ui", args.Length > 0 ? args[0] : "yielded"));
        var frameSize = args.Length > 1 ? int.Parse(args[1]) : 2048;
        collectDump = args.Contains("--dump");
        Directory.CreateDirectory(output);
        try
        {
            AppContext.SetSwitch("VisionEditor.DisableAutomaticCameraDiscovery", true);
            var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
            window = new MainWindow { ShowActivated = false, ShowInTaskbar = false, Left = -20000, Top = -20000 };
            var document = Field<GraphEditorDocument>(window, "doc");
            var registry = Field<ToolPluginRegistry>(window, "plugins");
            var source = (ImageSourcePlugin)registry.Find("vision.image-source")!;
            document.NewSolution();
            var sourceParameters = source.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
            sourceParameters["ImageSourceType"] = JsonSerializer.SerializeToElement(3);
            sourceParameters["Interval"] = JsonSerializer.SerializeToElement(0);
            var sourceId = document.AddNode("vision.image-source", source.Manifest.Version, 100, 100, sourceParameters);
            var binaryId = document.AddNode("vision.image-binary", "1.0.0", 100, 250);
            document.Connect(sourceId, "Image", binaryId, "Image");
            var procedureId = document.CurrentProcedureId;
            var otherProcedure = document.AddProcedure("受控空白流程");
            document.SelectProcedure(procedureId);
            window.Show();

            var pulseCount = 0;
            var maxPulseGapMs = 0d;
            var uiManagedThread = Environment.CurrentManagedThreadId;
            var uiOsThread = GetCurrentThreadId();
            lastPulse = Stopwatch.GetTimestamp();
            heartbeat = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromMilliseconds(20) };
            heartbeat.Tick += (_, _) =>
            {
                var now = Stopwatch.GetTimestamp();
                maxPulseGapMs = Math.Max(maxPulseGapMs, 1000d * (now - lastPulse) / Stopwatch.Frequency);
                Interlocked.Exchange(ref lastPulse, now);
                pulseCount++;
            };
            heartbeat.Start();
            var watcher = new Thread(() => Watchdog(output, uiOsThread)) { IsBackground = true, Name = "Idle UI probe watchdog" };
            watcher.Start();

            void Measure(string label, Action work)
            {
                activity = label;
                var before = pulseCount;
                var timer = Stopwatch.StartNew();
                work();
                timer.Stop();
                var endGapMs = 1000d * (Stopwatch.GetTimestamp() - Interlocked.Read(ref lastPulse)) / Stopwatch.Frequency;
                segments.Add(new { label, elapsedMs = timer.Elapsed.TotalMilliseconds, endGapMs, pulseDelta = pulseCount - before });
                activity = "idle";
                if (window.IsVisible)
                    PumpUntil(() => pulseCount > before);
            }

            activity = "real-image-run";
            source.SubmitImage(Field<ModuleExecutionScope>(window, "executionScope").ForProcedure(procedureId), sourceId,
                new GrayImage(frameSize, frameSize, Enumerable.Repeat((byte)127, frameSize * frameSize).ToArray()));
            var run = (Task)typeof(MainWindow).GetMethod("StartRunCore", Private)!.Invoke(window, [false, null])!;
            PumpUntil(() => run.IsCompleted);
            run.GetAwaiter().GetResult();
            var sessions = Field<Dictionary<string, ExecutionSessionResult>>(window, "workflowSessions");
            if (sessions[procedureId].Nodes.Single(n => n.NodeId == binaryId).Status != NodeRunStatus.Success)
                throw new Exception("The real binary node did not succeed.");
            activity = "idle";
            var idleBefore = pulseCount;
            var idleDelay = Task.Delay(1500);
            PumpUntil(() => idleDelay.IsCompleted);
            var idleTicks = pulseCount - idleBefore;
            if (idleTicks < 10) throw new Exception("Insufficient idle Dispatcher heartbeats.");

            var refresh = typeof(MainWindow).GetMethod("RefreshAll", Private)!;
            for (var index = 0; index < 6; index++)
                Measure("RefreshAll-" + index, () => refresh.Invoke(window, null));
            var selectNode = typeof(MainWindow).GetMethod("SelectWorkflowNode", Private)!;
            Measure("select-source", () => selectNode.Invoke(window, [procedureId, document.CurrentProcedure.Nodes.Single(n => n.Id == sourceId)]));
            Measure("select-binary", () => selectNode.Invoke(window, [procedureId, document.CurrentProcedure.Nodes.Single(n => n.Id == binaryId)]));
            var selectWorkflow = typeof(MainWindow).GetMethod("SelectWorkflow", Private)!;
            Measure("switch-away", () => selectWorkflow.Invoke(window, [otherProcedure]));
            Measure("switch-back", () => selectWorkflow.Invoke(window, [procedureId]));

            // The factory-only branch isolates constructor work from Show/layout without changing product code.
            if (args.Contains("--factory-split"))
            {
                var frame = new GrayImage(frameSize, frameSize, Enumerable.Repeat((byte)127, frameSize * frameSize).ToArray());
                var node = document.CurrentProcedure.Nodes.Single(n => n.Id == binaryId);
                var context = new ToolEditorContext(node, document.BaseDirectory, _ => { }, _ => throw new NotSupportedException())
                { PreviewImage = () => frame };
                Window? child = null;
                Measure("factory-constructor", () => child = new ImageBinaryWindow(new ImageBinaryPlugin(), context));
                Measure("factory-show", () => { child!.Owner = window; child.Show(); });
                Measure("factory-close", () => child!.Close());
            }
            else
            {
                // The configuration host uses Window.Show(); the owner and child stay on this test desktop.
                Measure("open-binary-config", () => typeof(MainWindow).GetMethod("OpenNode", Private)!.Invoke(window, [binaryId]));
                Measure("close-binary-config", () => typeof(MainWindow).GetMethod("CloseToolConfiguration", Private)!.Invoke(window, null));
            }
            var saved = Path.Combine(output, "controlled-idle-solution.vmx.json");
            Measure("document-save", () => document.Save(saved));
            if (!File.Exists(saved)) throw new Exception("Controlled solution was not saved.");
            Measure("window-close", window.Close);
            PumpUntil(() => !window.IsVisible);
            heartbeat.Stop();
            active = false;
            if (dumpStarted != 0) throw new Exception("Dispatcher heartbeat exceeded one second; inspect retained dump/event.");
            var result = new { frameSize, collectDump, uiManagedThread, uiOsThread, idleTicks, pulseCount, maxPulseGapMs,
                windowClosed = Field<bool>(window, "windowClosed"), dumpTriggered = dumpStarted != 0, segments };
            Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
            app.Shutdown();
            return 0;
        }
        catch (Exception error)
        {
            active = false;
            heartbeat?.Stop();
            Console.Error.WriteLine(error);
            Console.WriteLine(JsonSerializer.Serialize(new { segments, dumpTriggered = dumpStarted != 0 }, new JsonSerializerOptions { WriteIndented = true }));
            return 1;
        }
        finally { active = false; window?.Close(); }
    }

    static void Watchdog(string output, uint uiOsThread)
    {
        var watch = Stopwatch.StartNew();
        while (active)
        {
            Thread.Sleep(100);
            var gap = 1000d * (Stopwatch.GetTimestamp() - Interlocked.Read(ref lastPulse)) / Stopwatch.Frequency;
            if (gap > 1000 && Interlocked.CompareExchange(ref dumpStarted, 1, 0) == 0)
            {
                try
                {
                    File.WriteAllText(Path.Combine(output, "watchdog-event.json"), JsonSerializer.Serialize(new
                    { elapsedMs = watch.Elapsed.TotalMilliseconds, gapMs = gap, activity, uiOsThread, processId = Environment.ProcessId }));
                    if (collectDump)
                    {
                        using var stream = File.Create(Path.Combine(output, "idle-ui-threaddump.dmp"));
                        var okay = MiniDumpWriteDump(Process.GetCurrentProcess().Handle, (uint)Environment.ProcessId,
                            stream.SafeFileHandle.DangerousGetHandle(), 0x1000, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                        if (!okay) File.WriteAllText(Path.Combine(output, "dump-error.txt"), Marshal.GetLastWin32Error().ToString());
                    }
                }
                catch (Exception error) { File.WriteAllText(Path.Combine(output, "dump-error.txt"), error.ToString()); }
            }
            if (watch.Elapsed > TimeSpan.FromSeconds(20))
            {
                File.WriteAllText(Path.Combine(output, "watchdog-timeout.txt"), $"Own test process timed out at activity {activity}.");
                Environment.Exit(124);
            }
        }
    }

    static T Field<T>(object owner, string name) => (T)owner.GetType().GetField(name, Private)!.GetValue(owner)!;
    static void PumpUntil(Func<bool> ready)
    {
        var watch = Stopwatch.StartNew();
        while (!ready() && watch.Elapsed < TimeSpan.FromSeconds(15))
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => frame.Continue = false));
            Dispatcher.PushFrame(frame);
        }
        if (!ready()) throw new TimeoutException("Controlled UI operation did not settle in 15 seconds.");
    }

    [DllImport("kernel32.dll")] static extern uint GetCurrentThreadId();
    [DllImport("dbghelp.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool MiniDumpWriteDump(IntPtr process, uint pid, IntPtr file, uint type,
        IntPtr exception, IntPtr userStream, IntPtr callback);
}
