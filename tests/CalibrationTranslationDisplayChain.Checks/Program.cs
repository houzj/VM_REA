using System.Text.Json;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;

var evidence = Path.GetFullPath("analysis/joint-audit/pass15-cal-translation-display");
Directory.CreateDirectory(evidence);
var registry = new ModuleRegistry();
WorkflowCalls.Register(registry);
var plugins = new ToolPluginRegistry();
var translation = new TranslationPlugin();
plugins.Register(translation, registry);
var parameters = translation.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
parameters["CalibPointGet"] = JsonSerializer.SerializeToElement(1);
parameters["CalibPointTotalNum"] = JsonSerializer.SerializeToElement(4);
double[,] points = { { 0, 0, 10, -5 }, { 10, 0, 30, -5 }, { 0, 10, 10, 15 }, { 10, 10, 30, 15 } };
string[] names = ["ImagePointX", "ImagePointY", "WorldPointX", "WorldPointY"];
for (int i = 0; i < 4; i++) for (int axis = 0; axis < 4; axis++) parameters[names[axis] + i] = JsonSerializer.SerializeToElement(points[i, axis]);
using var graph = new GraphEditorDocument(registry);
var group = graph.AddNode(WorkflowCalls.ModuleId, WorkflowCalls.Version, 0, 0);
var body = graph.EnterOwnedGroup(group);
var nodeId = graph.AddNode(TranslationPlugin.Id, "1.0.0", 0, 0, parameters);
graph.SetWorkflowConfiguration(body, new() { Outputs =
[
    new("status", "CalibStatus", DataKind.Integer, nodeId, "CalibStatus"),
    new("images", "ImageXs", DataKind.NumberArray, nodeId, "OutImagePointX")
]});
graph.LeaveGroup();
void Check(bool condition, string message) { if (!condition) throw new Exception(message); Console.WriteLine("PASS " + message); }
using var scope = new ModuleExecutionScope();
async Task<(NodeRunRecord Inner, NodeRunRecord Outer, ToolDisplayFrame? Frame)> Run(string label)
{
    var session = await new SolutionExecutor(registry).RunAsync(new(graph.Document, graph.BaseDirectory), "main", scope: scope);
    var inner = session.Calls[group].Nodes.Single(n => n.NodeId == nodeId);
    var outer = session.Nodes.Single(n => n.NodeId == group);
    var node = graph.Document.Procedures.Single(p => p.Id == body).Nodes.Single(n => n.Id == nodeId);
    var frame = translation.LastDisplay(node, graph.BaseDirectory, scope.ForProcedure(body));
    Console.WriteLine($"{label}: inner={inner.Status}/0x{inner.NativeErrorCode:x8}, outer={outer.Status}/{outer.ErrorCode}, innerOut={string.Join(',', inner.Outputs.Keys)}, groupOut={string.Join(',', outer.Outputs.Keys)}, display={string.Join(',', frame?.Graphics.Select(g => g.Name) ?? [])}");
    return (inner, outer, frame);
}
Check(translation.LastDisplay(graph.Document.Procedures.Single(p => p.Id == body).Nodes.Single(n => n.Id == nodeId), graph.BaseDirectory, scope.ForProcedure(body)) is null, "unexecuted Translation node has no display snapshot");
var success = await Run("manual-success");
Check(success.Inner.Status == NodeRunStatus.Success && success.Outer.Status == NodeRunStatus.Success && success.Outer.Outputs["CalibStatus"].As<long>() == 1, "manual four-point calibration publishes completed status through owned Group");
Check(success.Outer.Outputs["ImageXs"].As<IReadOnlyList<double>>().SequenceEqual(new double[] { 0, 10, 0, 10 }), "Group exports four actual image X sample positions");
Check(success.Frame!.Graphics.Any(g => g.Name == "Image Point" && g.Points.Count == 4) && success.Frame.ResultList.Count == 4 && success.Frame.Color == "#66ff00", "Display.xml path and four-column result list consume completed source outputs");
Check(success.Frame.RecordText.Contains("CalibProgress:4/4", StringComparison.Ordinal), "record maps current and total calibration counts");
graph.SelectProcedure(body);
var failedParameters = parameters.ToDictionary(p => p.Key, p => p.Value.Clone());
// Deliberately malformed programmatic transition: the actual Clear button also
// zeroes all point parameters in the same Apply. This isolates stale-cache behavior.
failedParameters["CalibPointGet"] = JsonSerializer.SerializeToElement(0);
failedParameters["ClearRevision"] = JsonSerializer.SerializeToElement(1);
graph.SetParameters(nodeId, failedParameters);
var failed = await Run("missing-capture-input");
Check(failed.Inner.Status == NodeRunStatus.Failed && failed.Outer.Status == NodeRunStatus.Failed && failed.Outer.Outputs.Count == 0, "missing input after reset fails child and Group without former exports");
Check(failed.Frame is { Graphics.Count: 0, Color: "#ff0000" } && failed.Frame.ResultList.All(c => c.Value == "") && failed.Frame.RecordText == "", "failed display snapshot clears path, result values and record");
var recoveredParameters = failedParameters.ToDictionary(p => p.Key, p => p.Value.Clone());
recoveredParameters["CalibPointGet"] = JsonSerializer.SerializeToElement(1);
graph.SetParameters(nodeId, recoveredParameters);
var unchangedPoints = await Run("manual-mode-restored-without-point-edit");
Check(unchangedPoints.Inner.Status == NodeRunStatus.Failed && unchangedPoints.Outer.Status == NodeRunStatus.Failed && unchangedPoints.Frame is { Graphics.Count: 0 }, "synthetic revision-only clear leaves point cache empty until parameters change");
for (int i = 0; i < 4; i++) for (int axis = 0; axis < 4; axis++)
    recoveredParameters[names[axis] + i] = JsonSerializer.SerializeToElement(points[i, axis] + 1);
graph.SetParameters(nodeId, recoveredParameters);
var restored = await Run("manual-recovered-after-point-reentry");
Check(restored.Inner.Status == NodeRunStatus.Success && restored.Outer.Status == NodeRunStatus.Success && restored.Frame!.Graphics.Any(g => g.Name == "Image Point" && g.Points.Count == 4), "same scope recovers result export and path after actual point reentry");
var other = new NodeDocument("other", TranslationPlugin.Id, "1.0.0", "other", recoveredParameters);
Check(translation.LastDisplay(other, graph.BaseDirectory, scope.ForProcedure(body)) is null, "another unexecuted Translation node has no stale display snapshot");
Console.WriteLine("CalibrationTranslationDisplayChain.Checks: completed");
