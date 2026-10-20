using System.Text.Json;
using VisionCore;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
using VisionPlugins.ImageSource;

var evidence = Path.GetFullPath("analysis/joint-audit/pass13-cal-coordinate-terminal");
Directory.CreateDirectory(evidence);
var registry = new ModuleRegistry();
WorkflowCalls.Register(registry);
var plugins = new ToolPluginRegistry();
var image = new ImageSourcePlugin();
var coordinate = new CoordinateModulePlugin();
plugins.Register(image, registry);
plugins.Register(coordinate, registry);
Dictionary<string, JsonElement> Defaults(IVisionToolPlugin p) => p.Descriptor.Parameters.ToDictionary(x => x.Name, x => x.DefaultValue!.Value.Clone());
var imageValues = Defaults(image);
imageValues["ImageSourceType"] = JsonSerializer.SerializeToElement(3);
imageValues["StitchEnable"] = JsonSerializer.SerializeToElement(false);
var coordinateValues = Defaults(coordinate);
coordinateValues["CordinateCreate"] = JsonSerializer.SerializeToElement(0);
using var graph = new GraphEditorDocument(registry);
var group = graph.AddNode(WorkflowCalls.ModuleId, WorkflowCalls.Version, 0, 0);
var body = graph.EnterOwnedGroup(group);
var source = graph.AddNode(image.Manifest.Id, image.Manifest.Version, 0, 0, imageValues);
var terminal = graph.AddNode(coordinate.Manifest.Id, coordinate.Manifest.Version, 200, 0, coordinateValues);
graph.Connect(source, "Image", terminal, "InputImage");
graph.SetProcedureToolConfigurationRoiGroup(body, terminal, "RoiType", coordinateValues,
    [new RoiDocument("draw", RoiKind.Coordinate, 20, 30, AngleDegrees: 90)]);
graph.SetWorkflowConfiguration(body, new() { Outputs =
[
    new("matrix", "Matrix", DataKind.NumberArray, terminal, "CorCalibMatrix"),
    new("origin-x", "OriginX", DataKind.Number, terminal, "CoordinateOriPointX"),
    new("axis-yy", "AxisYY", DataKind.Number, terminal, "CoordinateAxisYY")
]});
graph.LeaveGroup();
void Check(bool ok, string detail) { if (!ok) throw new Exception(detail); Console.WriteLine("PASS " + detail); }
async Task<(NodeRunRecord Inner, NodeRunRecord Outer, ToolDisplayFrame? Frame)> Run(GraphEditorDocument editor, ModuleExecutionScope scope, string label)
{
    image.SubmitImage(scope.ForProcedure(body), source, new GrayImage(100, 80, new byte[8000]));
    var session = await new SolutionExecutor(registry).RunAsync(new(editor.Document, editor.BaseDirectory), "main", scope: scope);
    var inner = session.Calls[group].Nodes.Single(n => n.NodeId == terminal);
    var outer = session.Nodes.Single(n => n.NodeId == group);
    var node = editor.Document.Procedures.Single(p => p.Id == body).Nodes.Single(n => n.Id == terminal);
    var frame = coordinate.LastDisplay(node, editor.BaseDirectory, scope.ForProcedure(body));
    Console.WriteLine($"{label}: inner={inner.Status}/{inner.ErrorCode}, outer={outer.Status}/{outer.ErrorCode}, groupOut={string.Join(',', outer.Outputs.Keys)}, display={frame?.Graphics.Count}");
    return (inner, outer, frame);
}
using var warmScope = new ModuleExecutionScope();
var baseline = await Run(graph, warmScope, "draw");
Check(baseline.Inner.Status == NodeRunStatus.Success && baseline.Outer.Status == NodeRunStatus.Success, "Draw terminal and owned Group execute");
var matrix = baseline.Outer.Outputs["Matrix"].As<IReadOnlyList<double>>();
Check(matrix.Count == 9 && Math.Abs(matrix[0]) < 1e-6 && matrix[1] == 1 && matrix[2] == -30 && matrix[3] == -1 && matrix[5] == 20, "nine-float matrix leaves native-format Draw path");
Check(baseline.Outer.Outputs["OriginX"].As<double>() == 20 && Math.Abs(baseline.Outer.Outputs["AxisYY"].As<double>()) < 1e-5, "scalar origin and final axis leaf export through Group");
var graphic = baseline.Frame!.Graphics.Single(x => x.Name == "Coordinate System");
Check(baseline.Frame.Image?.Width == 100 && graphic.Points.Single().X == 20 && graphic.Points.Single().Y == 30 && graphic.XVector.Y == 50 && graphic.YVector.X == -50, "same execution scope displays source image and mapped terminal axes");
graph.SetProcedureToolConfigurationRoiGroup(body, terminal, "RoiType", coordinateValues, []);
var warmMissing = await Run(graph, warmScope, "warm-missing-roi");
Check(warmMissing.Inner.Status == NodeRunStatus.Success && warmMissing.Outer.Status == NodeRunStatus.Success && warmMissing.Inner.Outputs["ModuStatus"].As<long>() == 1 && warmMissing.Outer.Outputs["Matrix"].As<IReadOnlyList<double>>().SequenceEqual(matrix), "warm missing ROI retains prior matrix and exports it");
Check(warmMissing.Frame!.Graphics.Single(x => x.Name == "Coordinate System").Points.Single().X == 20, "warm retained state also reaches terminal display");
using var coldScope = new ModuleExecutionScope();
var coldMissing = await Run(graph, coldScope, "cold-missing-roi");
Check(coldMissing.Inner.Status == NodeRunStatus.Failed && coldMissing.Inner.ErrorCode == "NativeUninitializedMatrix" && coldMissing.Outer.Status == NodeRunStatus.Failed && coldMissing.Outer.Outputs.Count == 0, "cold unknown native matrix is rejected before Group export");
Check(coldMissing.Frame is { Graphics.Count: 0 }, "cold unsafe matrix does not invent display graphic");
graph.Undo();
var restored = await Run(graph, coldScope, "undo-restored-roi");
Check(restored.Inner.Status == NodeRunStatus.Success && restored.Outer.Status == NodeRunStatus.Success, "Undo restores valid terminal graph in cold scope");
var file = Path.Combine(evidence, "coordinate-terminal.vmx.json");
graph.Save(file);
using var reopened = new GraphEditorDocument(registry);
reopened.Load(file);
using var reopenedScope = new ModuleExecutionScope();
var replay = await Run(reopened, reopenedScope, "reopened");
Check(replay.Inner.Status == NodeRunStatus.Success && replay.Outer.Status == NodeRunStatus.Success && replay.Outer.Outputs["Matrix"].As<IReadOnlyList<double>>().SequenceEqual(matrix), "saved and reopened owned Group preserves terminal mappings");
Console.WriteLine("CalibrationCoordinateTerminal.Checks: completed");
