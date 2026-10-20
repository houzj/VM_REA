using System.Text.Json;
using VisionCore;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
using VisionPlugins.ImageSource;

var root = Path.GetFullPath(".");
var output = Path.Combine(root, "analysis", "joint-audit", "pass7-cal-failure");
Directory.CreateDirectory(output);
var path = Path.Combine(output, "matrix.xml");
var registry = new ModuleRegistry();
WorkflowCalls.Register(registry);
var loader = new ReadCalibFilePlugin();
var coordinate = new CoordinateModulePlugin();
var image = new ImageSourcePlugin();
var plugins = new ToolPluginRegistry();
plugins.Register(loader, registry);
plugins.Register(coordinate, registry);
plugins.Register(image, registry);

Dictionary<string, JsonElement> Defaults(IVisionToolPlugin plugin) =>
    plugin.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
string MatrixXml(int count, bool translated = false) => "<R><CalibInputParam/><CalibOutputParam><P ParamName='CalibMatrix' DataType='FloatList'>" +
    string.Concat(Enumerable.Range(0, count).Select(i => $"<V>{(translated && i == 2 ? 5 : i is 0 or 4 or 8 ? 1 : 0)}</V>")) +
    "</P></CalibOutputParam></R>";
File.WriteAllText(path, MatrixXml(9));
var readValues = Defaults(loader);
readValues["FilePath"] = JsonSerializer.SerializeToElement(path);
readValues["OutputSchema"] = JsonSerializer.SerializeToElement(JsonSerializer.Serialize(new[] { new ReadCalibFilePort("CalibMatrix", DataKind.NumberArray) }));
var coordinateValues = Defaults(coordinate);
coordinateValues["CordinateCreate"] = JsonSerializer.SerializeToElement(1);
coordinateValues["InputWay"] = JsonSerializer.SerializeToElement(2);
var imageValues = Defaults(image);
imageValues["ImageSourceType"] = JsonSerializer.SerializeToElement(3);
imageValues["StitchEnable"] = JsonSerializer.SerializeToElement(false);

using var graph = new GraphEditorDocument(registry);
var group = graph.AddNode(WorkflowCalls.ModuleId, WorkflowCalls.Version, 0, 0);
var body = graph.EnterOwnedGroup(group);
var read = graph.AddNode(ReadCalibFilePlugin.Id, "1.0.0", 0, 0, readValues);
graph.SetWorkflowConfiguration(body, new()
{
    Subscriptions = [new(read, "RefreshSignal", false, new(WorkflowSourceKind.IntegerConstant, "1"))],
    Outputs = [new("matrix", "Matrix", DataKind.NumberArray, read, "CalibMatrix")]
});
graph.LeaveGroup();
var source = graph.AddNode(image.Manifest.Id, image.Manifest.Version, 0, 200, imageValues);
var consumer = graph.AddNode(CoordinateModulePlugin.Id, "1.0.0", 180, 200, coordinateValues);
graph.Connect(group, "Matrix", consumer, "CalibMatrix");
graph.Connect(source, "Image", consumer, "InputImage");

using var scope = new ModuleExecutionScope();
async Task<(NodeRunRecord Reader, NodeRunRecord Group, NodeRunRecord Consumer, ExecutionSessionResult Child)> Run(string label, ModuleExecutionScope runScope)
{
    image.SubmitImage(runScope.ForProcedure(graph.CurrentProcedureId), source, new GrayImage(2, 2, [10, 20, 30, 40]));
    var session = await new SolutionExecutor(registry).RunAsync(new(graph.Document, graph.BaseDirectory), "main", scope: runScope);
    var child = session.Calls[group];
    var reader = child.Nodes.Single(n => n.NodeId == read);
    var groupRecord = session.Nodes.Single(n => n.NodeId == group);
    var coordinateRecord = session.Nodes.Single(n => n.NodeId == consumer);
    Console.WriteLine($"{label}: read={reader.Status}/{reader.ErrorCode}/status={reader.Outputs.GetValueOrDefault("ModuStatus")?.As<long>()}/matrix={reader.Outputs.GetValueOrDefault("CalibMatrix")?.As<IReadOnlyList<double>>().Count}, group={groupRecord.Status}/{groupRecord.ErrorCode}, coordinate={coordinateRecord.Status}/{coordinateRecord.ErrorCode}/status={coordinateRecord.Outputs.GetValueOrDefault("ModuStatus")?.As<long>()}/matrix={coordinateRecord.Outputs.GetValueOrDefault("CorCalibMatrix")?.As<IReadOnlyList<double>>().Count}, child={child.Status}/{string.Join(';', child.OutputErrors)}");
    return (reader, groupRecord, coordinateRecord, child);
}
void Check(bool condition, string reason) { if (!condition) throw new Exception(reason); }
var good = await Run("good9", scope);
Check(good.Reader.Status == NodeRunStatus.Success && good.Group.Status == NodeRunStatus.Success && good.Consumer.Status == NodeRunStatus.Success, "baseline 9 matrix must flow");
File.Delete(path);
var missing = await Run("missing-refresh", scope);
Check(missing.Reader.Status == NodeRunStatus.Success && missing.Reader.Outputs["CalibMatrix"].As<IReadOnlyList<double>>().Count == 9 && missing.Group.Status == NodeRunStatus.Success && missing.Consumer.Status == NodeRunStatus.Success, "unreadable refreshed file should retain accepted data");
File.WriteAllText(path, "broken");
var malformed = await Run("malformed-refresh", scope);
Check(malformed.Reader.Status == NodeRunStatus.Success && malformed.Group.Status == NodeRunStatus.Success && malformed.Consumer.Status == NodeRunStatus.Success, "malformed refreshed XML should retain accepted data");
File.WriteAllText(path, "<R><CalibInputParam><P ParamName='NewInput' DataType='float'><V>7</V></P></CalibInputParam></R>");
var noOutput = await Run("missing-output-section", scope);
Check(noOutput.Reader.Status == NodeRunStatus.Success && !noOutput.Reader.Outputs.ContainsKey("CalibMatrix") && noOutput.Group.Status == NodeRunStatus.Failed && noOutput.Consumer.Status == NodeRunStatus.Skipped, "well-formed but missing output section should clear matrix and fail Group output");
File.WriteAllText(path, MatrixXml(9));
var recovered = await Run("recovered9", scope);
Check(recovered.Reader.Status == NodeRunStatus.Success && recovered.Group.Status == NodeRunStatus.Success && recovered.Consumer.Status == NodeRunStatus.Success, "valid file should restore downstream graph");
File.WriteAllText(path, MatrixXml(8, translated: true));
var warm8 = await Run("warm8", scope);
var warmMatrix = warm8.Consumer.Outputs["CorCalibMatrix"].As<IReadOnlyList<double>>();
Check(warm8.Reader.Status == NodeRunStatus.Success && warm8.Group.Status == NodeRunStatus.Success && warm8.Consumer.Status == NodeRunStatus.Success && warmMatrix.Count == 9 && warmMatrix[2] == 5 && warmMatrix[8] == 1, "warm shorter matrix should update prefix and retain written tail");
for (int count = 1; count <= 8; count++)
{
    File.WriteAllText(path, MatrixXml(count));
    using var coldScope = new ModuleExecutionScope();
    var cold = await Run($"cold{count}", coldScope);
    Check(cold.Reader.Status == NodeRunStatus.Success && cold.Group.Status == NodeRunStatus.Success && cold.Consumer.Status == NodeRunStatus.Failed && cold.Consumer.ErrorCode == "NativeUninitializedMatrix", $"cold {count} must diagnose unknown tail");
}
Console.WriteLine("CalibrationFailureChain.Checks: 14 failure/recovery graph states passed");
