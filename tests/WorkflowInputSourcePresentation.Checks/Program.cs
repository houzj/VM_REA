using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.ImageSource;

static class Program
{
    const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

    [STAThread]
    static int Main()
    {
        AppContext.SetSwitch("VisionEditor.DisableAutomaticCameraDiscovery", true);
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        var window = new MainWindow { ShowActivated = false, ShowInTaskbar = false, Left = -20000, Top = -20000 };
        try
        {
            var doc = (GraphEditorDocument)typeof(MainWindow).GetField("doc", Private)!.GetValue(window)!;
            doc.NewSolution();
            var source = new ImageSourcePlugin();
            var buffer = new ImageBufferPlugin();
            var sourceId = doc.AddNode(source.Manifest.Id, source.Manifest.Version, 0, 0);
            var bufferId = doc.AddNode(buffer.Manifest.Id, buffer.Manifest.Version, 200, 0);
            var sourceName = doc.CurrentProcedure.Nodes.Single(n => n.Id == sourceId).Name;

            ToolEditorContext Context()
            {
                var basic = (ToolEditorContext)typeof(MainWindow).GetMethod("CreateToolEditorContext", Private)!.Invoke(window, [bufferId])!;
                return (ToolEditorContext)typeof(MainWindow).GetMethod("ConnectToolEditorServices", Private)!.Invoke(window, [basic, bufferId])!;
            }

            var context = Context();
            string Label() => context.SubscriptionLabel!("InputImage", false)!;
            if (Label() != "") throw new Exception("Unconnected image input showed a source.");
            Console.WriteLine("PASS unconnected input is empty.");

            doc.Connect(sourceId, "Image", bufferId, "InputImage");
            if (Label() != sourceName + ".Image") throw new Exception("Connected image edge was missing from the configuration source label.");
            using (var editor = new EditorWindow(buffer.CreateWpfEditor(context)))
                if (editor.Input.Text != Label()) throw new Exception("The actual ImageBuffer editor did not display the connected source.");
            Console.WriteLine("PASS connected input names its actual source edge.");

            doc.Disconnect(doc.CurrentProcedure.Connections.Single(c => c.ToNode == bufferId));
            if (Label() != "") throw new Exception("Disconnected image input retained a stale source label.");
            Console.WriteLine("PASS disconnection clears the source label.");

            doc.Connect(sourceId, "Image", bufferId, "InputImage");
            if (Label() != sourceName + ".Image") throw new Exception("Reconnected image input did not update its label.");
            Console.WriteLine("PASS reconnection follows the current edge.");

            var path = Path.Combine(Path.GetFullPath("analysis/joint-audit/input-source-presentation"), "connected-image.vmx.json");
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            doc.Save(path);
            doc.Load(path);
            context = Context();
            if (Label() != sourceName + ".Image") throw new Exception("Reloaded image connection was not presented to the configuration editor.");
            using (var editor = new EditorWindow(buffer.CreateWpfEditor(context)))
                if (editor.Input.Text != Label()) throw new Exception("The reopened editor did not display the saved source.");
            Console.WriteLine("PASS saved and reopened graph keeps the selected source label.");

            doc.SetProcedureInputSubscription(doc.CurrentProcedureId, bufferId, "InputImage",
                new WorkflowValueReference(WorkflowSourceKind.NodeOutput, sourceId, "Image"));
            if (doc.CurrentProcedure.Connections.Any(c => c.ToNode == bufferId) || Label() != sourceName + ".Image")
                throw new Exception("Replacing the wire with a typed subscription lost the actual source presentation.");
            Console.WriteLine("PASS typed subscription replaces the wire and keeps its source identity.");

            doc.SetProcedureInputSubscription(doc.CurrentProcedureId, bufferId, "InputImage", null);
            if (Label() != "") throw new Exception("Cleared subscription retained a stale source label.");
            doc.Undo();
            if (Label() != sourceName + ".Image") throw new Exception("Undo did not restore the selected source label.");
            Console.WriteLine("PASS subscription clear and Undo update the source label.");
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error);
            return 1;
        }
        finally
        {
            window.Close();
            app.Shutdown();
        }
    }

    sealed class EditorWindow(Window window) : IDisposable
    {
        public TextBox Input => Find<TextBox>(window, "ValueInputImage") ?? throw new Exception("ImageBuffer input field is absent.");
        public void Dispose() => window.Close();
        static T? Find<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            if (parent is T element && element.Name == name) return element;
            foreach (var child in LogicalTreeHelper.GetChildren(parent).OfType<DependencyObject>())
                if (Find<T>(child, name) is { } found) return found;
            return null;
        }
    }
}
