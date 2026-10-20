using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using VisionEditor;
using VisionEditor.Core;
using VisionPluginContracts;

static class Program
{
    const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    static readonly (string Id, string Parameter, int Value)[] Cases =
    [
        ("vision.n-image-calibration", "CalibPointTotalNum", 8),
        ("vision.camera-mapping", "WeightFactor", 30)
    ];
    static readonly string[] Remaining =
    [
        "vision.n-point-calibration", "vision.calibration-board", "vision.map-calibration",
        "vision.distortion-calibration", "vision.translation-calibration", "vision.rotate-calibration",
        "vision.coordinate-module", "vision.read-calib-file"
    ];
    static T Field<T>(object value, string name) => (T)value.GetType().GetField(name, Private)!.GetValue(value)!;
    static object? Call(object value, string name, params object[] args) => value.GetType().GetMethod(name, Private)!.Invoke(value, args);
    static void Check(bool condition, string label) { if (!condition) throw new Exception(label); Console.WriteLine("PASS " + label); }

    [STAThread]
    static int Main()
    {
        AppContext.SetSwitch("VisionEditor.DisableAutomaticCameraDiscovery", true);
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        var shell = new MainWindow { Left = -20000, Top = -20000, ShowActivated = false, ShowInTaskbar = false,
            WindowStartupLocation = WindowStartupLocation.Manual };
        shell.Show();
        try
        {
            var doc = Field<GraphEditorDocument>(shell, "doc");
            var plugins = Field<ToolPluginRegistry>(shell, "plugins");
            doc.NewSolution();
            var saved = new Dictionary<string, string>();
            foreach (var (moduleId, parameter, value) in Cases)
            {
                var plugin = plugins.Find(moduleId) ?? throw new Exception("Missing plugin " + moduleId);
                Check(plugin is IToolConfigurationProvider, moduleId + " provides shared configuration model");
                var nodeId = doc.AddNode(moduleId, plugin.Manifest.Version, 100 + saved.Count * 200, 100);
                saved[moduleId] = nodeId;
                Call(shell, "OpenToolConfiguration", nodeId);
                var host = Field<ToolConfigurationWindowHost>(shell, "toolConfigurationWindows");
                Check(host.Current is ToolConfigurationModelWindow, moduleId + " enters actual ordinary configuration window");
                var window = (ToolConfigurationModelWindow)host.Current!;
                var session = Field<ToolConfigurationSession>(window, "session");
                var initial = doc.CurrentProcedure.Nodes.Single(n => n.Id == nodeId).Parameters[parameter].GetInt32();
                var revision = doc.Revision;
                session.State.CommitUser(parameter, value.ToString(), true);
                var applied = doc.CurrentProcedure.Nodes.Single(n => n.Id == nodeId).Parameters[parameter].GetInt32();
                Check(applied == value && doc.Revision > revision, moduleId + " UI commit reaches graph document");
                Call(shell, "CloseToolConfiguration");
                doc.Undo();
                Check(doc.CurrentProcedure.Nodes.Single(n => n.Id == nodeId).Parameters[parameter].GetInt32() == initial,
                    moduleId + " configuration edit can undo");
                doc.Redo();
                Check(doc.CurrentProcedure.Nodes.Single(n => n.Id == nodeId).Parameters[parameter].GetInt32() == value,
                    moduleId + " configuration edit can redo");
            }
            foreach (var moduleId in Remaining)
            {
                var plugin = plugins.Find(moduleId) ?? throw new Exception("Missing plugin " + moduleId);
                var nodeId = doc.AddNode(moduleId, plugin.Manifest.Version, 100 + saved.Count * 200, 100);
                saved[moduleId] = nodeId;
                Call(shell, "OpenToolConfiguration", nodeId);
                var host = Field<ToolConfigurationWindowHost>(shell, "toolConfigurationWindows");
                Check(host.Current is ToolConfigurationModelWindow,
                    moduleId + " actual ordinary window; provider=" + (plugin is IToolConfigurationProvider) +
                    " dedicated=" + (plugin is IWpfToolEditorFactory));
                Call(shell, "CloseToolConfiguration");
            }
            Check(saved.Count == 10, "ten distinct calibration configuration entries checked");
            var path = Path.GetFullPath("analysis/joint-audit/pass7-cal-config/config-bridge-solution.vmproj");
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            doc.Save(path);
            doc.Load(path);
            foreach (var (moduleId, parameter, value) in Cases)
            {
                var nodeId = saved[moduleId];
                Check(doc.CurrentProcedure.Nodes.Single(n => n.Id == nodeId).Parameters[parameter].GetInt32() == value,
                    moduleId + " saved parameter survives solution reopen");
            }
            Console.WriteLine("PASS configuration bridge evidence " + path);
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
        finally { Call(shell, "CloseToolConfiguration"); shell.Close(); app.Shutdown(); }
    }
}
