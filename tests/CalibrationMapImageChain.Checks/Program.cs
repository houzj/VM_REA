using System.IO;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionCore;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
using VisionPlugins.ImageSource;
using VisionPlugins.MapCalibration;

var output = Path.GetFullPath("analysis/joint-audit/pass10-cal-map-image");
Directory.CreateDirectory(output);
var mapFile = Path.Combine(output, "mapped-image.xml");
var solutionFile = Path.Combine(output, "mapped-image.vmx.json");
File.Delete(mapFile);
GrayImage Load(string name)
{
    var path = Path.GetFullPath($"worktrees/tool-plugins/analysis/full-audit/n-image-calibration/solver-fixture/{name}");
    var bitmap = BitmapDecoder.Create(new Uri(path), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad).Frames[0];
    var gray = new FormatConvertedBitmap(bitmap, PixelFormats.Gray8, null, 0);
    var pixels = new byte[gray.PixelWidth * gray.PixelHeight];
    gray.CopyPixels(pixels, gray.PixelWidth, 0);
    return new GrayImage(gray.PixelWidth, gray.PixelHeight, pixels);
}
var sourceFrame = Load("frame-0.bmp");
var targetFrame = Load("frame-4.bmp");
var registry = new ModuleRegistry();
WorkflowCalls.Register(registry);
var plugins = new ToolPluginRegistry();
var image = new ImageSourcePlugin();
var map = new MapCalibrationPlugin();
var loader = new ReadCalibFilePlugin();
var coordinate = new CoordinateModulePlugin();
plugins.Register(image, registry);
plugins.Register(map, registry);
plugins.Register(loader, registry);
plugins.Register(coordinate, registry);
Dictionary<string, JsonElement> Defaults(IVisionToolPlugin tool) =>
    tool.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
var imageValues = Defaults(image);
imageValues["ImageSourceType"] = JsonSerializer.SerializeToElement(3);
imageValues["StitchEnable"] = JsonSerializer.SerializeToElement(false);
var mapValues = Defaults(map);
mapValues["InputType"] = JsonSerializer.SerializeToElement(1);
mapValues["WeightFun"] = JsonSerializer.SerializeToElement(1);
mapValues["CalibPathName"] = JsonSerializer.SerializeToElement(mapFile);
mapValues["RefreshFileEnable"] = JsonSerializer.SerializeToElement(true);
var loadValues = Defaults(loader);
loadValues["FilePath"] = JsonSerializer.SerializeToElement(mapFile);
loadValues["OutputSchema"] = JsonSerializer.SerializeToElement(JsonSerializer.Serialize(new[] {
    new ReadCalibFilePort("CalibMatrix", DataKind.NumberArray)
}));
var coordinateValues = Defaults(coordinate);
coordinateValues["CordinateCreate"] = JsonSerializer.SerializeToElement(1);
coordinateValues["InputWay"] = JsonSerializer.SerializeToElement(2);
using var graph = new GraphEditorDocument(registry);
var group = graph.AddNode(WorkflowCalls.ModuleId, WorkflowCalls.Version, 0, 0);
var body = graph.EnterOwnedGroup(group);
var source = graph.AddNode(image.Manifest.Id, image.Manifest.Version, 0, 0, imageValues);
var target = graph.AddNode(image.Manifest.Id, image.Manifest.Version, 0, 150, imageValues);
var mapper = graph.AddNode(MapCalibrationPlugin.Id, "1.0.0", 180, 0, mapValues);
var read = graph.AddNode(ReadCalibFilePlugin.Id, "1.0.0", 360, 0, loadValues);
graph.ConnectExecution(source, target);
graph.Connect(source, "Image", mapper, "InputImage1");
graph.Connect(target, "Image", mapper, "InputImage2");
graph.Connect(mapper, "CalibStatus", read, "RefreshSignal");
graph.SetWorkflowConfiguration(body, new() {
    Outputs = [new("matrix", "Matrix", DataKind.NumberArray, read, "CalibMatrix")]
});
graph.LeaveGroup();
var displaySource = graph.AddNode(image.Manifest.Id, image.Manifest.Version, 0, 250, imageValues);
var consumer = graph.AddNode(CoordinateModulePlugin.Id, "1.0.0", 220, 250, coordinateValues);
graph.Connect(group, "Matrix", consumer, "CalibMatrix");
graph.Connect(displaySource, "Image", consumer, "InputImage");

void Check(bool okay, string reason)
{
    if (!okay) throw new InvalidOperationException(reason);
    Console.WriteLine("PASS " + reason);
}
async Task Run(GraphEditorDocument document, string label)
{
    using var scope = new ModuleExecutionScope();
    image.SubmitImage(scope.ForProcedure(body), source, sourceFrame);
    image.SubmitImage(scope.ForProcedure(body), target, targetFrame);
    image.SubmitImage(scope.ForProcedure(document.CurrentProcedureId), displaySource,
        new GrayImage(2, 2, [10, 20, 30, 40]));
    var run = await new SolutionExecutor(registry).RunAsync(
        new(document.Document, document.BaseDirectory), "main", scope: scope);
    var children = run.Calls[group].Nodes.ToDictionary(n => n.NodeId);
    var groupRecord = run.Nodes.Single(n => n.NodeId == group);
    var consumerRecord = run.Nodes.Single(n => n.NodeId == consumer);
    Console.WriteLine($"{label}: source={children[source].Status}, target={children[target].Status}, map={children[mapper].Status}/{children[mapper].ErrorCode}, read={children[read].Status}/{children[read].ErrorCode}, group={groupRecord.Status}, coordinate={consumerRecord.Status}");
    Check(children[source].Status == NodeRunStatus.Success && children[target].Status == NodeRunStatus.Success,
        label + ": two real image sources execute");
    Check(children[mapper].Status == NodeRunStatus.Success &&
          children[mapper].Outputs["CurCornersPointNum"].As<long>() == 168 &&
          children[mapper].Outputs["ObjCornersPointNum"].As<long>() == 168,
        label + ": Map image solver consumes both frames and finds 168+168 corners");
    Check(File.Exists(mapFile) && children[read].Status == NodeRunStatus.Success &&
          children[read].Outputs["CalibMatrix"].As<IReadOnlyList<double>>().Count == 9,
        label + ": image-mode Map XML loads as nine-element calibration matrix");
    Check(groupRecord.Status == NodeRunStatus.Success && consumerRecord.Status == NodeRunStatus.Success &&
          groupRecord.Outputs["Matrix"].As<IReadOnlyList<double>>().SequenceEqual(
              children[read].Outputs["CalibMatrix"].As<IReadOnlyList<double>>()),
        label + ": Group transports exact loaded matrix to Coordinate");
}
await Run(graph, "first");
graph.Save(solutionFile);
File.Delete(mapFile);
using var reopened = new GraphEditorDocument(registry);
reopened.Load(solutionFile);
await Run(reopened, "reopened");
Console.WriteLine("CalibrationMapImageChain.Checks: image-to-file-to-consumer passed twice");
