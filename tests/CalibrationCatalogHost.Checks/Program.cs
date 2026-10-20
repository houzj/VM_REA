using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;

static class Program
{
    const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    static readonly string[] Expected =
    [
        "vision.n-point-calibration", "vision.n-image-calibration", "vision.calibration-board",
        "vision.camera-mapping", "vision.map-calibration", "vision.distortion-calibration",
        "vision.translation-calibration", "vision.rotate-calibration", "vision.coordinate-module",
        "vision.read-calib-file"
    ];

    [STAThread]
    static int Main()
    {
        AppContext.SetSwitch("VisionEditor.DisableAutomaticCameraDiscovery", true);
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        MainWindow? window = null;
        try
        {
            window = new MainWindow { ShowActivated = false, ShowInTaskbar = false, Left = -20000, Top = -20000 };
            var registry = (ModuleRegistry)typeof(MainWindow).GetField("modules", Private)!.GetValue(window)!;
            var plugins = (ToolPluginRegistry)typeof(MainWindow).GetField("plugins", Private)!.GetValue(window)!;
            var graph = (GraphEditorDocument)typeof(MainWindow).GetField("doc", Private)!.GetValue(window)!;
            var groups = (Array)typeof(MainWindow).GetProperty("ToolGroups", Private)!.GetValue(window)!;
            var calibration = groups.Cast<object>().Single(x => (string)x.GetType().GetProperty("Id")!.GetValue(x)! == "Calibration");
            var loaded = typeof(MainWindow).GetMethod("LoadToolCategory", Private)!.Invoke(window, [calibration])
                ?? throw new Exception("Calibration catalog failed to load.");
            var items = ((Array)loaded.GetType().GetProperty("Items")!.GetValue(loaded)!).Cast<object>().ToArray();
            var discovered = items.Select(x => new
            {
                CatalogId = (string)x.GetType().GetProperty("Id")!.GetValue(x)!,
                ModuleId = (string?)x.GetType().GetProperty("ModuleId")!.GetValue(x),
                Name = (string)x.GetType().GetProperty("Name")!.GetValue(x)!
            }).ToArray();
            if (discovered.Length != 10 || discovered.Any(x => x.ModuleId is null) ||
                !Expected.ToHashSet(StringComparer.Ordinal).SetEquals(discovered.Select(x => x.ModuleId!)))
                throw new Exception("The current calibration category does not contain the expected ten distinct module identities.");

            graph.NewSolution();
            var observations = new List<object>();
            for (var index = 0; index < discovered.Length; index++)
            {
                var item = discovered[index];
                var plugin = plugins.Find(item.ModuleId!) ?? throw new Exception("Plugin is not registered: " + item.ModuleId);
                var descriptor = registry.GetDescriptor(plugin.Manifest.Id, plugin.Manifest.Version);
                if (descriptor.Id != item.ModuleId || plugin.Descriptor.Id != descriptor.Id ||
                    plugin.Manifest.Version != descriptor.Version)
                    throw new Exception("Catalog, manifest and registry disagree: " + item.ModuleId);
                var runtime = registry.Create(descriptor.Id, descriptor.Version);
                var id = graph.AddNode(descriptor.Id, descriptor.Version, 100 + index * 160, 200);
                var node = graph.CurrentProcedure.Nodes.Single(n => n.Id == id);
                if (node.ModuleId != item.ModuleId || runtime.Descriptor.Id != item.ModuleId)
                    throw new Exception("Added graph node or runtime lost module identity: " + item.ModuleId);
                observations.Add(new
                {
                    item.CatalogId, item.Name, item.ModuleId, descriptor.Version,
                    DescriptorInputs = descriptor.Inputs.Select(p => new { p.Name, Kind = p.Kind.ToString(), p.Cardinality }),
                    DescriptorOutputs = descriptor.Outputs.Select(p => new { p.Name, Kind = p.Kind.ToString(), p.Cardinality }),
                    HasDedicatedWpfEditor = plugin is IWpfToolEditorFactory,
                    HasConfigurationProvider = plugin is IToolConfigurationProvider,
                    CreatedNodeId = id
                });
                Console.WriteLine("PASS catalog→registry→factory→node " + item.ModuleId);
            }
            if (graph.CurrentProcedure.Nodes.Count != 10) throw new Exception("Graph did not retain all ten nodes.");
            var path = Path.GetFullPath("analysis/joint-audit/calibration-catalog-host/catalog-host.json");
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, JsonSerializer.Serialize(observations, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine("PASS ten unique calibration nodes in real WPF host; evidence " + path);
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error);
            return 1;
        }
        finally
        {
            window?.Close();
            app.Shutdown();
        }
    }
}
