using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using VisionCore;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.ImageBinary;
using VisionPlugins.ImageSource;
using VisionUi;

static class Program
{
    const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    static readonly List<object> phases = [];
    static readonly Dictionary<string, int> eventCounts = new(StringComparer.Ordinal);
    static readonly Stopwatch clock = Stopwatch.StartNew();
    static volatile string activity = "setup";
    static volatile bool active = true;
    static long lastPulse;
    static int pulseCount;

    [STAThread]
    static int Main(string[] args)
    {
        var mode = args.Length > 0 ? args[0] : "factory";
        var size = args.Length > 1 ? int.Parse(args[1]) : 64;
        var output = Path.GetFullPath(Path.Combine("analysis", "joint-audit", "ui-show", $"{mode}-{size}"));
        Directory.CreateDirectory(output);
        MainWindow? main = null;
        Window? child = null;
        DispatcherTimer? timer = null;
        try
        {
            AppContext.SetSwitch("VisionEditor.DisableAutomaticCameraDiscovery", true);
            var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
            EventManager.RegisterClassHandler(typeof(ImageBinaryWindow), FrameworkElement.LoadedEvent,
                new RoutedEventHandler((sender, e) => { if (ReferenceEquals(sender, e.OriginalSource)) Mark("binary-Loaded"); }), true);
            EventManager.RegisterClassHandler(typeof(GroupRoiEditor), FrameworkElement.LoadedEvent,
                new RoutedEventHandler((sender, e) => { if (ReferenceEquals(sender, e.OriginalSource)) Mark("roi-Loaded"); }), true);
            main = new MainWindow { ShowActivated = false, ShowInTaskbar = false, Left = -20000, Top = -20000 };
            var doc = Field<GraphEditorDocument>(main, "doc");
            var registry = Field<ToolPluginRegistry>(main, "plugins");
            var source = (ImageSourcePlugin)registry.Find("vision.image-source")!;
            doc.NewSolution();
            var parameters = source.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
            parameters["ImageSourceType"] = JsonSerializer.SerializeToElement(3);
            parameters["Interval"] = JsonSerializer.SerializeToElement(0);
            var sourceId = doc.AddNode("vision.image-source", source.Manifest.Version, 100, 100, parameters);
            var binaryId = doc.AddNode("vision.image-binary", "1.0.0", 100, 250);
            doc.Connect(sourceId, "Image", binaryId, "Image");
            var procedureId = doc.CurrentProcedureId;
            var image = new GrayImage(size, size, Enumerable.Repeat((byte)127, size * size).ToArray());
            main.Show();
            lastPulse = Stopwatch.GetTimestamp();
            timer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromMilliseconds(20) };
            timer.Tick += (_, _) => { Interlocked.Exchange(ref lastPulse, Stopwatch.GetTimestamp()); pulseCount++; };
            timer.Start();
            new Thread(() => Watchdog(output)) { IsBackground = true, Name = "Own WPF show phase watchdog" }.Start();
            source.SubmitImage(Field<ModuleExecutionScope>(main, "executionScope").ForProcedure(procedureId), sourceId, image);
            var run = (Task)typeof(MainWindow).GetMethod("StartRunCore", Private)!.Invoke(main, [false, null])!;
            PumpUntil(() => run.IsCompleted);
            run.GetAwaiter().GetResult();
            var sessions = Field<Dictionary<string, ExecutionSessionResult>>(main, "workflowSessions");
            if (sessions[procedureId].Nodes.Single(n => n.NodeId == binaryId).Status != NodeRunStatus.Success)
                throw new Exception("Real ImageSource → Binary execution did not succeed.");
            Mark("real-flow-completed");
            var idle = Task.Delay(1000);
            PumpUntil(() => idle.IsCompleted);
            Mark("idle-after-run");

            void Measure(string name, Action work)
            {
                activity = name;
                var count = pulseCount;
                var start = clock.Elapsed.TotalMilliseconds;
                work();
                var end = clock.Elapsed.TotalMilliseconds;
                phases.Add(new { name, startMs = start, endMs = end, elapsedMs = end - start,
                    endHeartbeatGapMs = 1000d * (Stopwatch.GetTimestamp() - Interlocked.Read(ref lastPulse)) / Stopwatch.Frequency,
                    pulseDelta = pulseCount - count });
                activity = "idle";
                PumpUntil(() => pulseCount > count);
            }

            if (mode == "host")
            {
                var host = (ToolConfigurationWindowHost)typeof(MainWindow).GetMethod("ConfigurationWindows", Private)!.Invoke(main, null)!;
                host.Changed += () => Mark("host-Changed-after-Show-and-RefreshAll");
                Measure("host-OpenNode", () => typeof(MainWindow).GetMethod("OpenNode", Private)!.Invoke(main, [binaryId]));
                child = host.Current;
                if (child is null) throw new Exception("Host did not retain a configuration window.");
            }
            else if (mode == "factory")
            {
                var node = doc.CurrentProcedure.Nodes.Single(n => n.Id == binaryId);
                var context = new ToolEditorContext(node, doc.BaseDirectory, _ => { }, _ => throw new NotSupportedException())
                { PreviewImage = () => image };
                Measure("resource-SourceControlResources.Current", () => _ = SourceControlResources.Current);
                Measure("factory-ImageBinaryWindow.ctor", () => child = new ImageBinaryWindow(new ImageBinaryPlugin(), context));
                child!.SourceInitialized += (_, _) => Mark("binary-SourceInitialized");
                child.Loaded += (_, _) => Mark("binary-Loaded-handler");
                child.ContentRendered += (_, _) => Mark("binary-ContentRendered");
                child.LayoutUpdated += (_, _) => Mark("binary-LayoutUpdated");
                child.Owner = main;
                Measure("factory-Window.Show", child.Show);
            }
            else if (mode is "roi" or "roi-noimage")
            {
                GroupRoiEditor? roi = null;
                Measure("roi-GroupRoiEditor.ctor", () => roi = new GroupRoiEditor([], ImageBinaryPlugin.RoiCapabilities));
                if (mode == "roi") Measure("roi-SetImage", () => roi!.SetImage(image));
                child = new Window { Width = 1180, Height = 760, Content = roi, ShowInTaskbar = false, ShowActivated = false, Owner = main };
                child.SourceInitialized += (_, _) => Mark("roi-window-SourceInitialized");
                child.Loaded += (_, _) => Mark("roi-window-Loaded-handler");
                child.ContentRendered += (_, _) => Mark("roi-window-ContentRendered");
                child.LayoutUpdated += (_, _) => Mark("roi-window-LayoutUpdated");
                Measure("roi-Window.Show", child.Show);
            }
            else if (mode == "empty")
            {
                child = new Window { Width = 1180, Height = 760, Content = new Grid(), ShowInTaskbar = false, ShowActivated = false, Owner = main };
                child.SourceInitialized += (_, _) => Mark("empty-SourceInitialized");
                child.Loaded += (_, _) => Mark("empty-Loaded");
                child.ContentRendered += (_, _) => Mark("empty-ContentRendered");
                child.LayoutUpdated += (_, _) => Mark("empty-LayoutUpdated");
                Measure("empty-Window.Show", child.Show);
            }
            else throw new ArgumentException("mode must be host, factory, roi, roi-noimage, or empty");

            var settle = Task.Delay(300);
            PumpUntil(() => settle.IsCompleted);
            Mark("post-show-settled");
            Measure("child-Close", () => child!.Close());
            Measure("main-Close", main.Close);
            timer.Stop();
            active = false;
            var result = new { mode, size, uiThread = GetCurrentThreadId(), mainClosed = Field<bool>(main, "windowClosed"), eventCounts, phases };
            var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path.Combine(output, "phases.json"), json);
            Console.WriteLine(json);
            app.Shutdown();
            return 0;
        }
        catch (Exception ex)
        {
            active = false;
            Console.Error.WriteLine(ex);
            var json = JsonSerializer.Serialize(new { mode, size, eventCounts, phases }, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path.Combine(output, "failure.json"), json);
            return 1;
        }
        finally { active = false; child?.Close(); main?.Close(); timer?.Stop(); }
    }

    static void Mark(string name)
    {
        if (name.Contains("LayoutUpdated", StringComparison.Ordinal))
        {
            var count = eventCounts.GetValueOrDefault(name) + 1;
            eventCounts[name] = count;
            if (count > 10) return; // Avoid measurement overhead from a busy layout event.
        }
        phases.Add(new { name, timeMs = clock.Elapsed.TotalMilliseconds, activity });
    }
    static T Field<T>(object owner, string name) => (T)owner.GetType().GetField(name, Private)!.GetValue(owner)!;
    static void PumpUntil(Func<bool> ready)
    {
        var wait = Stopwatch.StartNew();
        while (!ready() && wait.Elapsed < TimeSpan.FromSeconds(15))
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => frame.Continue = false));
            Dispatcher.PushFrame(frame);
        }
        if (!ready()) throw new TimeoutException("Own WPF phase did not settle in 15 seconds.");
    }
    static void Watchdog(string output)
    {
        var watch = Stopwatch.StartNew();
        while (active)
        {
            Thread.Sleep(100);
            var gap = 1000d * (Stopwatch.GetTimestamp() - Interlocked.Read(ref lastPulse)) / Stopwatch.Frequency;
            if (gap > 1000)
            {
                var path = Path.Combine(output, "stall-events.jsonl");
                File.AppendAllText(path, JsonSerializer.Serialize(new { elapsedMs = watch.Elapsed.TotalMilliseconds, gapMs = gap, activity }) + Environment.NewLine);
            }
            if (watch.Elapsed > TimeSpan.FromSeconds(30))
            {
                File.WriteAllText(Path.Combine(output, "watchdog-timeout.txt"), activity);
                Environment.Exit(124); // The PID is this exclusive test host only.
            }
        }
    }
    [DllImport("kernel32.dll")] static extern uint GetCurrentThreadId();
}
