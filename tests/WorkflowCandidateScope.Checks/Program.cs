using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.ImageBinary;
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
            var graph = (GraphEditorDocument)typeof(MainWindow).GetField("doc", Private)!.GetValue(window)!;
            var registry = (ModuleRegistry)typeof(MainWindow).GetField("modules", Private)!.GetValue(window)!;
            graph.NewSolution();
            var source = new ImageSourcePlugin();
            var binary = new ImageBinaryPlugin();
            var a = graph.AddNode(source.Manifest.Id, source.Manifest.Version, 0, 0);
            var b = graph.AddNode(binary.Manifest.Id, binary.Manifest.Version, 200, 0);
            var c = graph.AddNode(source.Manifest.Id, source.Manifest.Version, 0, 200);
            graph.Connect(a, "Image", b, "Image");
            var procedure = graph.CurrentProcedure;
            if (procedure.ExecutionLinks is null ||
                !procedure.ExecutionLinks.Any(e => e.FromNode == a && e.ToNode == b) ||
                procedure.ExecutionLinks.Any(e => e.FromNode == c || e.ToNode == c))
                throw new Exception("The A→B/C-disconnected execution topology was not constructed.");

            var workflowSources = ((IEnumerable<WorkflowSourceOption>)typeof(MainWindow)
                .GetMethod("WorkflowSources", Private)!.Invoke(window, [graph.CurrentProcedureId, DataKind.GrayImage])!).ToArray();
            var genericIds = workflowSources.Select(x => x.NodeId).ToHashSet(StringComparer.Ordinal);
            if (!genericIds.Contains(a) || !genericIds.Contains(c))
                throw new Exception("Generic WorkflowSources did not expose both compatible image producers.");
            Console.WriteLine("PASS WorkflowSources(GrayImage) includes A and independent C.");

            var service = new WorkflowConfigurationSubscriptionService(() => graph.CurrentProcedure,
                registry, b, (_, _) => throw new NotSupportedException("Read-only probe"));
            var destination = new ConfigurationSubscriptionDestination(ConfigurationSubscriptionDomain.Combination, "Image", "IMAGE");
            var candidates = service.Enumerate(new(destination, ["IMAGE"]));
            if (candidates.Status != ConfigurationSubscriptionStatus.Available)
                throw new Exception("Subscription candidate query failed: " + candidates.Diagnostic);
            var ancestorIds = candidates.Roots.SelectMany(x => x.Children)
                .Select(x => x.Binding?.Reference).OfType<ConfigurationOutputReference>()
                .Select(x => x.NodeId).ToHashSet(StringComparer.Ordinal);
            if (!ancestorIds.Contains(a) || ancestorIds.Contains(c))
                throw new Exception("Ancestor-scoped subscription candidates were incorrect.");
            Console.WriteLine("PASS typed subscription service includes A but excludes independent C.");

            var proposed = procedure with
            {
                Connections = [],
                Configuration = procedure.Configuration with
                {
                    Subscriptions = [new WorkflowSubscription(b, "Image", false,
                        new WorkflowValueReference(WorkflowSourceKind.NodeOutput, c, "Image"))]
                }
            };
            var issue = GraphValidator.Validate(proposed, registry).Issues
                .SingleOrDefault(i => i.Code == "DataExecutionOrder" && i.NodeId == b);
            if (issue is null)
                throw new Exception("Validator did not reject C as a producer with no execution path to B.");
            Console.WriteLine("PASS GraphValidator rejects C→B subscription: " + issue.Code + " / " + issue.Message);

            try
            {
                graph.SetProcedureInputSubscription(graph.CurrentProcedureId, b, "Image",
                    new WorkflowValueReference(WorkflowSourceKind.NodeOutput, c, "Image"));
                throw new Exception("Editor accepted the invalid C→B subscription.");
            }
            catch (InvalidOperationException ex) when (ex.Message == issue.Message)
            {
                Console.WriteLine("PASS GraphEditorDocument rejects invalid C→B edit before persistence.");
            }
            if (graph.CurrentProcedure.Connections.SingleOrDefault(x => x.ToNode == b)?.FromNode != a)
                throw new Exception("Rejected subscription changed the existing A→B wire.");
            var output = Path.GetFullPath("analysis/joint-audit/candidate-scope/candidate-scope.json");
            Directory.CreateDirectory(Path.GetDirectoryName(output)!);
            File.WriteAllText(output, JsonSerializer.Serialize(new
            {
                Nodes = new { A = a, B = b, C = c },
                GenericCandidates = workflowSources.Select(x => new { x.NodeId, x.Port, x.Kind, x.Label }),
                ScopedCandidateNodeIds = ancestorIds.Order(),
                ValidatorIssue = new { issue.Code, issue.Message, issue.NodeId },
                RejectedEditPreservedEdge = true
            }, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine("PASS evidence: " + output);
            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            return 1;
        }
        finally
        {
            window.Close();
            app.Shutdown();
        }
    }
}
