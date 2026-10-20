using System.Text.Json;
using VisionCore;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
using VisionPlugins.CameraMapping;
using VisionPlugins.ImageSource;
using VisionPlugins.MapCalibration;

var root = Path.GetFullPath(".");
var output = Path.Combine(root, "analysis", "joint-audit", "pass8-cal-map");
Directory.CreateDirectory(output);
var aPath = Path.Combine(output, "camera-a.iwcal");
var bPath = Path.Combine(output, "camera-b.iwcal");
var mapPath = Path.Combine(output, "mapped.xml");
foreach (var path in new[] { aPath, bPath, mapPath }) File.Delete(path);
var registry = new ModuleRegistry();
WorkflowCalls.Register(registry);
var plugins = new ToolPluginRegistry();
var camera = new CameraMappingPlugin();
var map = new MapCalibrationPlugin();
var loader = new ReadCalibFilePlugin();
var coordinate = new CoordinateModulePlugin();
var image = new ImageSourcePlugin();
plugins.Register(camera, registry);
plugins.Register(map, registry);
plugins.Register(loader, registry);
plugins.Register(coordinate, registry);
plugins.Register(image, registry);
Dictionary<string, JsonElement> Defaults(IVisionToolPlugin plugin) => plugin.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
var aValues = Defaults(camera);
aValues["CalibPathName"] = JsonSerializer.SerializeToElement(aPath);
aValues["RefreshFileEnable"] = JsonSerializer.SerializeToElement(true);
var bValues = Defaults(camera);
bValues["CalibPathName"] = JsonSerializer.SerializeToElement(bPath);
bValues["RefreshFileEnable"] = JsonSerializer.SerializeToElement(true);
var mapValues = Defaults(map);
mapValues["InputType"] = JsonSerializer.SerializeToElement(2);
mapValues["CalibPath0"] = JsonSerializer.SerializeToElement(aPath);
mapValues["CalibPath1"] = JsonSerializer.SerializeToElement(bPath);
mapValues["CalibPathName"] = JsonSerializer.SerializeToElement(mapPath);
mapValues["RefreshFileEnable"] = JsonSerializer.SerializeToElement(true);
var loadValues = Defaults(loader);
loadValues["FilePath"] = JsonSerializer.SerializeToElement(mapPath);
loadValues["OutputSchema"] = JsonSerializer.SerializeToElement(JsonSerializer.Serialize(new[] { new ReadCalibFilePort("CalibMatrix", DataKind.NumberArray) }));
var coordinateValues = Defaults(coordinate);
coordinateValues["CordinateCreate"] = JsonSerializer.SerializeToElement(1);
coordinateValues["InputWay"] = JsonSerializer.SerializeToElement(2);
var imageValues = Defaults(image);
imageValues["ImageSourceType"] = JsonSerializer.SerializeToElement(3);
imageValues["StitchEnable"] = JsonSerializer.SerializeToElement(false);

using var graph = new GraphEditorDocument(registry);
var group = graph.AddNode(WorkflowCalls.ModuleId, WorkflowCalls.Version, 0, 0);
var body = graph.EnterOwnedGroup(group);
var a = graph.AddNode(CameraMappingPlugin.Id, "1.0.0", 0, 0, aValues);
var b = graph.AddNode(CameraMappingPlugin.Id, "1.0.0", 160, 0, bValues);
var m = graph.AddNode(MapCalibrationPlugin.Id, "1.0.0", 320, 0, mapValues);
var r = graph.AddNode(ReadCalibFilePlugin.Id, "1.0.0", 480, 0, loadValues);
graph.ConnectExecution(a, b);
graph.ConnectExecution(b, m);
graph.Connect(b, "CalibStatus", m, "RefreshSignal");
graph.Connect(m, "CalibStatus", r, "RefreshSignal");
var subscriptions = new List<WorkflowSubscription>();
void Pair(string node, string side, int pair, double[] x, double[] y)
{
    subscriptions.Add(new(node, side + "PointX" + pair, false,
        new(WorkflowSourceKind.NumberArrayConstant, JsonSerializer.Serialize(x))));
    subscriptions.Add(new(node, side + "PointY" + pair, false,
        new(WorkflowSourceKind.NumberArrayConstant, JsonSerializer.Serialize(y))));
}
foreach (var (id, dx, dy) in new[] { (a, 10d, -5d), (b, 20d, -2d) })
{
    Pair(id, "Object", 1, [0, 10], [0, 0]);
    Pair(id, "Target", 1, [dx, dx + 20], [dy, dy]);
    Pair(id, "Object", 2, [0, 10], [10, 10]);
    Pair(id, "Target", 2, [dx, dx + 20], [dy + 20, dy + 20]);
}
graph.SetWorkflowConfiguration(body, new()
{
    Subscriptions = subscriptions,
    Outputs = [new("matrix", "Matrix", DataKind.NumberArray, r, "CalibMatrix")]
});
graph.LeaveGroup();
var imageId = graph.AddNode(image.Manifest.Id, image.Manifest.Version, 0, 200, imageValues);
var coordinateId = graph.AddNode(CoordinateModulePlugin.Id, "1.0.0", 180, 200, coordinateValues);
graph.Connect(group, "Matrix", coordinateId, "CalibMatrix");
graph.Connect(imageId, "Image", coordinateId, "InputImage");
using var scope = new ModuleExecutionScope();
image.SubmitImage(scope.ForProcedure(graph.CurrentProcedureId), imageId, new GrayImage(2, 2, [10, 20, 30, 40]));
var session = await new SolutionExecutor(registry).RunAsync(new(graph.Document, graph.BaseDirectory), "main", scope: scope);
var child = session.Calls[group];
var nodes = child.Nodes.ToDictionary(n => n.NodeId);
var coordinateRecord = session.Nodes.Single(n => n.NodeId == coordinateId);
foreach (var (name, id) in new[] { ("camera-a", a), ("camera-b", b), ("map-file", m), ("read", r) })
    Console.WriteLine($"{name}: {nodes[id].Status}/{nodes[id].ErrorCode} {nodes[id].Message}");
Console.WriteLine($"group={session.Nodes.Single(n => n.NodeId == group).Status}, coordinate={coordinateRecord.Status}/{coordinateRecord.ErrorCode}");
void Check(bool condition, string reason) { if (!condition) throw new Exception(reason); }
Check(nodes[a].Status == NodeRunStatus.Success && nodes[b].Status == NodeRunStatus.Success, "both real camera mapping graph executions");
Check(File.Exists(aPath) && File.Exists(bPath) && new FileInfo(aPath).Length == 36 && new FileInfo(bPath).Length == 36, "two camera binary files");
Check(nodes[m].Status == NodeRunStatus.Success && File.Exists(mapPath), "map file mode composes and writes XML");
Check(nodes[r].Status == NodeRunStatus.Success && nodes[r].Outputs["CalibMatrix"].As<IReadOnlyList<double>>().Count == 9, "read module publishes nine float matrix entries");
Check(session.Nodes.Single(n => n.NodeId == group).Status == NodeRunStatus.Success && coordinateRecord.Status == NodeRunStatus.Success, "Group carries file matrix to coordinate");
Check(coordinateRecord.Outputs["CorCalibMatrix"].As<IReadOnlyList<double>>().Count == 9, "coordinate publishes nine entries");
Check(!camera.Descriptor.Outputs.Any(p => p.Name == "CalibMatrix") &&
      !map.Descriptor.Outputs.Any(p => p.Name == "CalibMatrix"), "neither producer publishes a direct matrix port");
Check(MapCalibrationFiles.Read(aPath).ErrorCode == 0 &&
      ReadCalibFileData.Read(aPath).ErrorCode == 0xe0000201, "same 36-byte camera file is Map input, not ReadCalibFile XML");
Check(ReadCalibFileData.Read(mapPath).Data!.Values()["CalibMatrix"].As<IReadOnlyList<double>>().Count == 9,
    "Map output XML is valid calibration-loader input");
Console.WriteLine($"files={new FileInfo(aPath).Length}/{new FileInfo(bPath).Length}/{new FileInfo(mapPath).Length}; matrix={string.Join(',', nodes[r].Outputs["CalibMatrix"].As<IReadOnlyList<double>>().Select(v => v.ToString("G9", System.Globalization.CultureInfo.InvariantCulture)))}");
var solution = Path.Combine(output, "camera-map-chain.vmx.json");
graph.Save(solution);
foreach (var path in new[] { aPath, bPath, mapPath }) File.Delete(path);
using var reopened = new GraphEditorDocument(registry);
reopened.Load(solution);
using var reopenedScope = new ModuleExecutionScope();
image.SubmitImage(reopenedScope.ForProcedure(reopened.CurrentProcedureId), imageId, new GrayImage(2, 2, [10, 20, 30, 40]));
var second = await new SolutionExecutor(registry).RunAsync(new(reopened.Document, reopened.BaseDirectory), "main", scope: reopenedScope);
Check(second.Calls[group].Nodes.All(n => n.Status == NodeRunStatus.Success) &&
      second.Nodes.Single(n => n.NodeId == group).Status == NodeRunStatus.Success &&
      second.Nodes.Single(n => n.NodeId == coordinateId).Status == NodeRunStatus.Success &&
      new[] { aPath, bPath, mapPath }.All(File.Exists),
    "saved and reopened graph repeats producer/file/Group/coordinate chain");
Console.WriteLine("saved/reopened: four inner modules, Group and Coordinate success");
Console.WriteLine("CalibrationMapProducer.Checks: real two-camera to map-file to loader to Group to coordinate chain passed");
