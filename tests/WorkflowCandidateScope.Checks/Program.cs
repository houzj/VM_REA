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

            var frontSources = ((IEnumerable<WorkflowSourceOption>)typeof(MainWindow)
                .GetMethod("WorkflowFrontSources", Private)!.Invoke(window, [graph.CurrentProcedureId, b, DataKind.GrayImage])!).ToArray();
            var frontIds = frontSources.Select(x => x.NodeId).ToHashSet(StringComparer.Ordinal);
            if (!frontIds.Contains(a) || frontIds.Contains(b) || frontIds.Contains(c))
                throw new Exception("Tool input candidates escaped the execution predecessor scope.");
            Console.WriteLine("PASS tool input candidates include A and exclude independent C.");

            WorkflowSourceOption[] PluginPickerOptions() => ((IReadOnlyList<WorkflowSourceOption>)typeof(MainWindow)
                .GetMethod("ToolSubscriptionOptions", Private)!.Invoke(window,
                    [graph.CurrentProcedureId, b, "Image", false, DataKind.GrayImage])!).ToArray();
            var pluginOptions = PluginPickerOptions();
            var pluginIds = pluginOptions.Where(x => x.Reference?.Kind == WorkflowSourceKind.NodeOutput)
                .Select(x => x.NodeId).ToHashSet(StringComparer.Ordinal);
            if (!pluginIds.SetEquals([a]) || pluginOptions.Count(x => x.Reference is null) != 1)
                throw new Exception("Plugin-facing picker included a disconnected producer or lost its null option.");
            graph.SetNodeEnabled(a, false);
            if (PluginPickerOptions().Any(x => x.Reference?.Kind == WorkflowSourceKind.NodeOutput))
                throw new Exception("Plugin-facing picker included a disabled predecessor.");
            graph.SetNodeEnabled(a, true);
            var executionLink = graph.CurrentProcedure.ExecutionLinks!.Single(x => x.FromNode == a && x.ToNode == b);
            graph.DisconnectExecution(executionLink);
            if (PluginPickerOptions().Any(x => x.Reference?.Kind == WorkflowSourceKind.NodeOutput))
                throw new Exception("Plugin-facing picker retained a disconnected execution predecessor.");
            graph.ConnectExecution(a, b);
            if (!PluginPickerOptions().Any(x => x.NodeId == a && x.Reference?.Kind == WorkflowSourceKind.NodeOutput))
                throw new Exception("Plugin-facing picker did not restore the reconnected predecessor.");
            var d = graph.AddNode(binary.Manifest.Id, binary.Manifest.Version, 400, 100);
            graph.Connect(b, "Image", d, "Image");
            var downstream = ((IReadOnlyList<WorkflowSourceOption>)typeof(MainWindow)
                .GetMethod("ToolSubscriptionOptions", Private)!.Invoke(window,
                    [graph.CurrentProcedureId, d, "Image", false, DataKind.GrayImage])!).ToArray();
            var downstreamIds = downstream.Where(x => x.Reference?.Kind == WorkflowSourceKind.NodeOutput)
                .Select(x => x.NodeId).ToHashSet(StringComparer.Ordinal);
            if (!downstreamIds.SetEquals([a, b]))
                throw new Exception("Plugin-facing picker lost a transitive predecessor or included an unrelated node.");
            Console.WriteLine("PASS plugin-facing picker tracks execution predecessors, disable and reconnect.");

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
            var legacyPath = Path.GetFullPath("analysis/joint-audit/candidate-scope/legacy-no-links.vmx.json");
            var current = graph.Document;
            SolutionStore.Save(legacyPath, current with
            {
                Procedures = current.Procedures.Select(p => p with { ExecutionLinks = null }).ToArray()
            });
            graph.Load(legacyPath);
            if (graph.CurrentProcedure.ExecutionLinks is null ||
                !PluginPickerOptions().Where(x => x.Reference?.Kind == WorkflowSourceKind.NodeOutput)
                    .Select(x => x.NodeId).ToHashSet(StringComparer.Ordinal).SetEquals([a]))
                throw new Exception("Legacy data-link migration did not restore the exact execution predecessor.");
            Console.WriteLine("PASS legacy load derives the plugin-facing predecessor from saved data links.");
            var output = Path.GetFullPath("analysis/joint-audit/candidate-scope/candidate-scope.json");
            Directory.CreateDirectory(Path.GetDirectoryName(output)!);
            File.WriteAllText(output, JsonSerializer.Serialize(new
            {
                Nodes = new { A = a, B = b, C = c, D = d },
                GenericCandidates = workflowSources.Select(x => new { x.NodeId, x.Port, x.Kind, x.Label }),
                ToolInputCandidates = frontSources.Select(x => new { x.NodeId, x.Port, x.Kind, x.Label }),
                PluginPickerCandidates = pluginOptions.Select(x => new { x.NodeId, x.Port, x.Kind, x.Label, SourceKind = x.Reference?.Kind.ToString() }),
                PluginPickerTopologyTransitions = new[] { "A connected", "A disabled", "A disconnected", "A reconnected" },
                DownstreamPluginPickerNodeIds = downstreamIds.Order(),
                LegacyLoadDerivedExecutionLinksFromDataLinks = true,
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
