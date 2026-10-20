using System.Text.Json;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;

var evidence = Path.GetFullPath("analysis/joint-audit/pass16-cal-read-display");
Directory.CreateDirectory(evidence);
var file = Path.Combine(evidence, "dynamic.xml");
string Xml(string name, string value = "1") =>
    $"<R><CalibInputParam/><CalibOutputParam><P ParamName='{name}' DataType='FloatList'>" +
    $"<V>{value}</V><V>0</V><V>0</V><V>0</V><V>1</V><V>0</V><V>0</V><V>0</V><V>1</V>" +
    "</P></CalibOutputParam></R>";
var valid = Xml("CalibMatrix");
File.WriteAllText(file, valid);
var registry = new ModuleRegistry();
WorkflowCalls.Register(registry);
var plugins = new ToolPluginRegistry();
var loader = new ReadCalibFilePlugin();
plugins.Register(loader, registry);
var values = loader.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
values["FilePath"] = JsonSerializer.SerializeToElement(file);
values["OutputSchema"] = JsonSerializer.SerializeToElement(ReadCalibFilePlugin.Schema(ReadCalibFileData.Parse(valid).Data!));
using var graph = new GraphEditorDocument(registry);
var group = graph.AddNode(WorkflowCalls.ModuleId, WorkflowCalls.Version, 0, 0);
var body = graph.EnterOwnedGroup(group);
var read = graph.AddNode(ReadCalibFilePlugin.Id, "1.0.0", 0, 0, values);
graph.SetWorkflowConfiguration(body, new()
{
    Subscriptions = [new(read, "RefreshSignal", false, new(WorkflowSourceKind.IntegerConstant, "1"))],
    Outputs = [new("matrix", "Matrix", DataKind.NumberArray, read, "CalibMatrix")]
});
graph.LeaveGroup();
void Check(bool condition, string message) { if (!condition) throw new Exception(message); Console.WriteLine("PASS " + message); }
using var scope = new ModuleExecutionScope();
async Task<(NodeRunRecord Read, NodeRunRecord Group, ToolDisplayFrame? Frame)> Run(string label)
{
    var session = await new SolutionExecutor(registry).RunAsync(new(graph.Document, graph.BaseDirectory), "main", scope: scope);
    var inner = session.Calls[group].Nodes.Single(n => n.NodeId == read);
    var outer = session.Nodes.Single(n => n.NodeId == group);
    var node = graph.Document.Procedures.Single(p => p.Id == body).Nodes.Single(n => n.Id == read);
    var frame = loader.LastDisplay(node, graph.BaseDirectory, scope.ForProcedure(body));
    Console.WriteLine($"{label}: read={inner.Status}/{inner.ErrorCode}, status={inner.Outputs.GetValueOrDefault("ModuStatus")?.As<long>()}, group={outer.Status}/{outer.ErrorCode}, exports={string.Join(',', outer.Outputs.Keys)}, record='{frame?.RecordText}'");
    return (inner, outer, frame);
}
Check(loader.LastDisplay(graph.Document.Procedures.Single(p => p.Id == body).Nodes.Single(n => n.Id == read), graph.BaseDirectory, scope.ForProcedure(body)) is null, "unexecuted ReadCalib node has no display snapshot");
var baseline = await Run("valid");
Check(baseline.Read.Status == NodeRunStatus.Success && baseline.Group.Status == NodeRunStatus.Success && baseline.Group.Outputs["Matrix"].As<IReadOnlyList<double>>().Count == 9, "valid file exports nine-float matrix to owned Group");
Check(baseline.Frame is { RecordVisible: true, TextVisible: false, ImageVisible: false, ResultListVisible: false } && baseline.Frame.RecordText == "Module Status:1", "original Data Record alone displays status one");
File.WriteAllText(file, "<broken");
var malformed = await Run("malformed-retained");
Check(malformed.Read.Status == NodeRunStatus.Success && malformed.Group.Status == NodeRunStatus.Success && malformed.Frame?.RecordText == "Module Status:1", "malformed refresh retains accepted file output and display status");
File.WriteAllText(file, "<R><CalibInputParam/></R>");
var absent = await Run("missing-output-section");
Check(absent.Read.Status == NodeRunStatus.Success && absent.Read.Outputs["ModuStatus"].As<long>() == 0 && absent.Group.Status == NodeRunStatus.Failed && absent.Group.Outputs.Count == 0, "well-formed missing output section clears matrix and Group export");
Check(absent.Frame?.RecordText == "Module Status:0", "status-zero display replaces prior status-one snapshot");
File.WriteAllText(file, Xml("RenamedMatrix"));
var renamed = await Run("renamed-output");
Check(renamed.Read.Status == NodeRunStatus.Success && renamed.Read.Outputs["ModuStatus"].As<long>() == 1 && renamed.Group.Status == NodeRunStatus.Failed && renamed.Frame?.RecordText == "Module Status:1", "renamed dynamic leaf changes Group result but Data Record reflects output collection only");
File.WriteAllText(file, valid);
var restored = await Run("original-restored");
Check(restored.Group.Status == NodeRunStatus.Success && restored.Frame?.RecordText == "Module Status:1", "restoring exact file name and type restores Group matrix");
// Independent finite PortValue safety case, not an assertion about native NaN behavior.
File.WriteAllText(file, Xml("CalibMatrix", "NaN"));
var unsafeData = await Run("nonfinite-safety-error");
Check(unsafeData.Read.Status == NodeRunStatus.Failed && unsafeData.Read.ErrorCode == "INVALID_CALIBRATION_NUMBER" && unsafeData.Group.Status == NodeRunStatus.Failed, "nonfinite file is rejected by independent finite transport");
Check(unsafeData.Frame?.RecordText == "Module Status:1", "early failure leaves previous display snapshot: current stale-view gap");
File.WriteAllText(file, valid);
var recovered = await Run("recovered-after-safety-error");
Check(recovered.Read.Status == NodeRunStatus.Success && recovered.Group.Status == NodeRunStatus.Success && recovered.Frame?.RecordText == "Module Status:1", "same scope recovers successful file and Group output");
var other = new NodeDocument("other", ReadCalibFilePlugin.Id, "1.0.0", "other", values);
Check(loader.LastDisplay(other, graph.BaseDirectory, scope.ForProcedure(body)) is null, "another unexecuted ReadCalib node does not inherit prior snapshot");
Console.WriteLine("CalibrationReadCalibDisplayChain.Checks: completed");
