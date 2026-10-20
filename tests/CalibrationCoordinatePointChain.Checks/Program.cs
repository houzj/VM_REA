using System.Text.Json;
using VisionCore;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
using VisionPlugins.ImageSource;

var root = Path.GetFullPath(".");
var output = Path.Combine(root, "analysis", "joint-audit", "pass9-cal-coordinate-point");
Directory.CreateDirectory(output);
var calibrationFile = Path.Combine(output, "npoint-generated.xml");
var solutionFile = Path.Combine(output, "calibration-chain.vmx.json");
var registry = new ModuleRegistry();
WorkflowCalls.Register(registry);
var plugins = new ToolPluginRegistry();
var npoint = new TranslationPlugin();
var loader = new ReadCalibFilePlugin();
var coordinate = new CoordinateModulePlugin();
var imageSource = new ImageSourcePlugin();
plugins.Register(npoint, registry);
plugins.Register(loader, registry);
plugins.Register(coordinate, registry);
plugins.Register(imageSource, registry);

using (var incompatible = new GraphEditorDocument(registry))
{
    var producer = incompatible.AddNode(TranslationPlugin.Id, "1.0.0", 0, 0);
    var consumer = incompatible.AddNode(CoordinateModulePlugin.Id, "1.0.0", 180, 0);
    var rejected = false;
    try { incompatible.Connect(producer, "CalibData", consumer, "CalibMatrix"); }
    catch (InvalidOperationException error) when (error.Message == "Port type mismatch.") { rejected = true; }
    if (!rejected || incompatible.CurrentProcedure.Connections.Count != 0)
        throw new Exception("Independent CalibrationData must not be silently coerced to native float-list CalibMatrix");
    Console.WriteLine("direct-CalibData-to-CalibMatrix: rejected without mutating graph");
}

Dictionary<string, JsonElement> Defaults(IVisionToolPlugin plugin) =>
    plugin.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());

var pointValues = Defaults(npoint);
using var fixture = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "docs", "plugins", "n-point-solver-evidence.json")));
var sample = fixture.RootElement.GetProperty("cases").EnumerateArray()
    .Single(x => x.GetProperty("name").GetString() == "state-rotation-three");
var points = sample.GetProperty("points").EnumerateArray().ToArray();
var rotations = sample.GetProperty("rotation").EnumerateArray().ToArray();
pointValues["CalibPointGet"] = JsonSerializer.SerializeToElement(1);
pointValues["CalibPointTotalNum"] = JsonSerializer.SerializeToElement(points.Length);
pointValues["RotPointTotalNum"] = JsonSerializer.SerializeToElement(rotations.Length);
pointValues["WeightFun"] = JsonSerializer.SerializeToElement(sample.GetProperty("weight").GetInt32());
pointValues["HomoFreedom"] = JsonSerializer.SerializeToElement(sample.GetProperty("freedom").GetInt32());
pointValues["RefreshFileEnable"] = JsonSerializer.SerializeToElement(true);
pointValues["CalibPathName"] = JsonSerializer.SerializeToElement(calibrationFile);
var keys = new[] { "ImagePointX", "ImagePointY", "WorldPointX", "WorldPointY", "DeltaAngle" };
for (var i = 0; i < points.Length + rotations.Length; i++)
{
    var row = i < points.Length ? points[i] : rotations[i - points.Length];
    for (var axis = 0; axis < keys.Length; axis++)
        pointValues[keys[axis] + i] = JsonSerializer.SerializeToElement(
            axis < row.GetArrayLength() ? row[axis].GetDouble() : 0);
}

var loaderValues = Defaults(loader);
loaderValues["FilePath"] = JsonSerializer.SerializeToElement(calibrationFile);
var coordinateValues = Defaults(coordinate);
coordinateValues["CordinateCreate"] = JsonSerializer.SerializeToElement(1);
coordinateValues["InputWay"] = JsonSerializer.SerializeToElement(2);
var sourceValues = Defaults(imageSource);
sourceValues["ImageSourceType"] = JsonSerializer.SerializeToElement(3);
sourceValues["StitchEnable"] = JsonSerializer.SerializeToElement(false);

using var editor = new GraphEditorDocument(registry);
var groupId = editor.AddNode(WorkflowCalls.ModuleId, WorkflowCalls.Version, 0, 0);
var bodyId = editor.EnterOwnedGroup(groupId);
var pointId = editor.AddNode(TranslationPlugin.Id, "1.0.0", 0, 0, pointValues);
var loaderId = editor.AddNode(ReadCalibFilePlugin.Id, "1.0.0", 180, 0, loaderValues);
editor.Connect(pointId, "CalibStatus", loaderId, "RefreshSignal");
editor.SetWorkflowConfiguration(bodyId, new()
{
    Outputs = [new("matrix", "Matrix", DataKind.NumberArray, loaderId, "CalibMatrix"),
               new("image-x", "ImageX", DataKind.NumberArray, pointId, "OutImagePointX"),
               new("image-y", "ImageY", DataKind.NumberArray, pointId, "OutImagePointY")]
});
editor.LeaveGroup();
var sourceId = editor.AddNode(imageSource.Manifest.Id, imageSource.Manifest.Version, 0, 200, sourceValues);
var coordinateId = editor.AddNode(CoordinateModulePlugin.Id, "1.0.0", 180, 200, coordinateValues);
editor.Connect(groupId, "Matrix", coordinateId, "CalibMatrix");
editor.Connect(sourceId, "Image", coordinateId, "InputImage");
var xyValues = Defaults(coordinate);
xyValues["CordinateCreate"] = JsonSerializer.SerializeToElement(1);
xyValues["InputWay"] = JsonSerializer.SerializeToElement(1);
var xyId = editor.AddNode(CoordinateModulePlugin.Id, "1.0.0", 360, 200, xyValues);
editor.Connect(groupId, "ImageX", xyId, "SrcPointX");
editor.Connect(groupId, "ImageY", xyId, "SrcPointY");
editor.Connect(sourceId, "Image", xyId, "InputImage");
editor.SetWorkflowConfiguration(editor.CurrentProcedureId, new()
{
    Subscriptions = [new(xyId, "SrcAngle", false, new(WorkflowSourceKind.NumberArrayConstant, "[0]"))]
});

async Task Check(GraphEditorDocument graph, string label)
{
    using var scope = new ModuleExecutionScope();
    imageSource.SubmitImage(scope.ForProcedure(graph.CurrentProcedureId), sourceId,
        new GrayImage(2, 2, [10, 20, 30, 40]));
    var session = await new SolutionExecutor(registry).RunAsync(
        new(graph.Document, graph.BaseDirectory), "main", scope: scope);
    var children = session.Calls[groupId].Nodes.ToDictionary(n => n.NodeId);
    var point = children[pointId];
    var read = children[loaderId];
    var group = session.Nodes.Single(n => n.NodeId == groupId);
    var coordinateRecord = session.Nodes.Single(n => n.NodeId == coordinateId);
    var xyRecord = session.Nodes.Single(n => n.NodeId == xyId);
    if (point.Status != NodeRunStatus.Success || read.Status != NodeRunStatus.Success ||
        group.Status != NodeRunStatus.Success || coordinateRecord.Status != NodeRunStatus.Success ||
        xyRecord.Status != NodeRunStatus.Success ||
        !File.Exists(calibrationFile) ||
        point.Outputs["CalibStatus"].As<long>() != 1 ||
        read.Outputs["CalibMatrix"].Kind != DataKind.NumberArray ||
        !group.Outputs.TryGetValue("Matrix", out var matrix) ||
        matrix.Kind != DataKind.NumberArray || matrix.As<IReadOnlyList<double>>().Count != 9 ||
        !coordinateRecord.Outputs.TryGetValue("CorCalibMatrix", out var result) ||
        result.Kind != DataKind.NumberArray || result.As<IReadOnlyList<double>>().Count != 9)
        throw new Exception($"{label}: Translation={point.Status}/{point.ErrorCode}, loader={read.Status}/{read.ErrorCode}, group={group.Status}/{group.ErrorCode}, coordinate={coordinateRecord.Status}/{coordinateRecord.ErrorCode}");
    var producer = point.Outputs["CalibData"].As<CalibrationData>().ImageToWorld;
    var viaFile = read.Outputs["CalibMatrix"].As<IReadOnlyList<double>>();
    var transported = matrix.As<IReadOnlyList<double>>();
    if (producer.Where((value, index) =>
            Math.Abs(value - viaFile[index]) > Math.Max(1e-6, Math.Abs(value) * 1e-6)).Any())
        throw new Exception($"{label}: Translation matrix and file matrix disagree beyond recorded G8 float text precision");
    var xyMatrix = xyRecord.Outputs["CorCalibMatrix"].As<IReadOnlyList<double>>();
    if (xyMatrix.Count != 9 || Math.Abs(xyMatrix[2] + (float)points[0][0].GetDouble()) > 1e-5 ||
        Math.Abs(xyMatrix[5] + (float)points[0][1].GetDouble()) > 1e-5)
        throw new Exception($"{label}: coordinate XY path did not use first transported point");
    if (viaFile.Where((value, index) => value != transported[index]).Any())
        throw new Exception($"{label}: Group did not retain exact loaded float matrix");
    Console.WriteLine($"{label}: translation={point.Status}, XML={File.Exists(calibrationFile)}, read={read.Status}, group={group.Status}, coordinate={coordinateRecord.Status}, producerMatrix={producer.Count}, fileMatrix={viaFile.Count}, groupMatrix={transported.Count}, resultMatrix={result.As<IReadOnlyList<double>>().Count}");
}

await Check(editor, "initial");
editor.Save(solutionFile);
using var reopened = new GraphEditorDocument(registry);
reopened.Load(solutionFile);
await Check(reopened, "reopened");
Console.WriteLine("CalibrationCoordinatePointChain.Checks: matrix and point paths passed twice");
