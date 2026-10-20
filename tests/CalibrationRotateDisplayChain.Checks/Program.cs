using System.Text.Json;
using VisionCore;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
using VisionPlugins.ImageSource;

var evidence = Path.GetFullPath("analysis/joint-audit/pass14-cal-c-display");
Directory.CreateDirectory(evidence);
var matrixFile = Path.Combine(evidence, "rotate-matrix.iwcal");
File.WriteAllBytes(matrixFile, new float[] { 2, 0, 10, 0, 2, -5, 0, 0, 1 }.SelectMany(BitConverter.GetBytes).ToArray());
var registry = new ModuleRegistry();
var plugins = new ToolPluginRegistry();
var image = new ImageSourcePlugin();
var rotate = new RotateCalibrationPlugin();
plugins.Register(image, registry);
plugins.Register(rotate, registry);
Dictionary<string, JsonElement> Defaults(IVisionToolPlugin p) => p.Descriptor.Parameters.ToDictionary(x => x.Name, x => x.DefaultValue!.Value.Clone());
var imageParameters = Defaults(image);
imageParameters["ImageSourceType"] = JsonSerializer.SerializeToElement(3);
imageParameters["StitchEnable"] = JsonSerializer.SerializeToElement(false);
var rotateParameters = Defaults(rotate);
rotateParameters["FilePath"] = JsonSerializer.SerializeToElement(matrixFile);
using var graph = new GraphEditorDocument(registry);
var source = graph.AddNode(image.Manifest.Id, image.Manifest.Version, 0, 0, imageParameters);
var target = graph.AddNode(rotate.Manifest.Id, rotate.Manifest.Version, 200, 0, rotateParameters);
graph.Connect(source, "Image", target, "InputImage");
void Configure(int count) => graph.SetWorkflowConfiguration(graph.CurrentProcedureId, new()
{
    Subscriptions =
    [
        new(target, "RotNum", false, new(WorkflowSourceKind.IntegerConstant, count.ToString())),
        new(target, "RefreshSignal", false, new(WorkflowSourceKind.IntegerConstant, "0")),
        new(target, "ImagePointX", false, new(WorkflowSourceKind.NumberArrayConstant, "[50]")),
        new(target, "ImagePointY", false, new(WorkflowSourceKind.NumberArrayConstant, "[40]")),
        new(target, "WorldRotateAngle", false, new(WorkflowSourceKind.NumberArrayConstant, "[17]"))
    ]
});
void Check(bool value, string message) { if (!value) throw new Exception(message); Console.WriteLine("PASS " + message); }
using var scope = new ModuleExecutionScope();
async Task<(NodeRunRecord Source, NodeRunRecord Target, ToolDisplayFrame? Frame)> Run(string label)
{
    image.SubmitImage(scope.ForProcedure(graph.CurrentProcedureId), source, new GrayImage(80, 60, new byte[4800]));
    var session = await new SolutionExecutor(registry).RunAsync(new(graph.Document, graph.BaseDirectory), "main", scope: scope);
    var runs = session.Nodes.ToDictionary(n => n.NodeId);
    var frame = rotate.LastDisplay(graph.CurrentProcedure.Nodes.Single(n => n.Id == target), graph.BaseDirectory, scope.ForProcedure(graph.CurrentProcedureId));
    Console.WriteLine($"{label}: source={runs[source].Status}, rotate={runs[target].Status}/0x{runs[target].NativeErrorCode:x8}, outputs={string.Join(',', runs[target].Outputs.Keys)}, frame={frame?.RecordText}");
    return (runs[source], runs[target], frame);
}
Check(rotate.LastDisplay(graph.CurrentProcedure.Nodes.Single(n => n.Id == target), graph.BaseDirectory, scope.ForProcedure(graph.CurrentProcedureId)) is null, "unexecuted Rotate node has no display snapshot");
Configure(3);
var success = await Run("sample1");
Check(success.Source.Status == NodeRunStatus.Success && success.Target.Status == NodeRunStatus.Success && success.Target.Outputs["remainCount"].As<long>() == 2, "real image and first Rotate sample execute");
Check(success.Frame?.Image?.Width == 80 && success.Frame.Image.Height == 60, "display retains current upstream image");
Check(success.Frame!.RecordText.Contains("剩余标定次数:2", StringComparison.Ordinal) && success.Frame.ResultList.Count == 7 && success.Frame.Color == "#66ff00", "source-declared data record, seven columns and success style mapped");
Configure(2);
var failed = await Run("bad-count");
Check(failed.Target.Status == NodeRunStatus.Failed && failed.Target.NativeErrorCode == 0xe0000001, "invalid RotNum triggers original parameter error");
Check(!failed.Target.Outputs.ContainsKey("remainCount") && !failed.Target.Outputs.ContainsKey("RotCenterImageX") && failed.Frame?.RecordText.Contains("E0000001", StringComparison.OrdinalIgnoreCase) == true, "failed display shows diagnostic without stale success data");
Check(failed.Frame?.Image?.Width == 80 && failed.Frame.ResultList.All(c => c.Value == "") && failed.Frame.Color == "#ff0000", "failed frame owns this-run image but clears prior result values");
Configure(3);
var recovered = await Run("recovered");
Check(recovered.Target.Status == NodeRunStatus.Success && recovered.Target.Outputs["remainCount"].As<long>() == 2 && !recovered.Frame!.RecordText.Contains("错误码", StringComparison.Ordinal), "same scope recovers with fresh sample/result display");
var foreign = new NodeDocument("other", RotateCalibrationPlugin.Id, "1.0.0", "other", rotateParameters);
Check(rotate.LastDisplay(foreign, graph.BaseDirectory, scope.ForProcedure(graph.CurrentProcedureId)) is null, "selection of another unexecuted Rotate node cannot retrieve prior node snapshot");
Console.WriteLine("CalibrationRotateDisplayChain.Checks: completed");
