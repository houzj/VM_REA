using System.Text.Json;
using VisionCore;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.ImageBinary;
using VisionPlugins.ImageSource;

var source = new ImageSourcePlugin();
var binary = new ImageBinaryPlugin();
var registry = new ModuleRegistry();
WorkflowCalls.Register(registry);
var plugins = new ToolPluginRegistry();
plugins.Register(source, registry);
plugins.Register(binary, registry);
var sourceParameters = source.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
sourceParameters["ImageSourceType"] = JsonSerializer.SerializeToElement(3);
sourceParameters["StitchEnable"] = JsonSerializer.SerializeToElement(false);
sourceParameters["OutMono8"] = JsonSerializer.SerializeToElement(false);
var binaryParameters = binary.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
var gray = new GrayImage(2, 2, [10, 40, 100, 200]);
var rgb = new RgbImage(2, 2, [33, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120]);
var outputRoot = Path.GetFullPath(Path.Combine("analysis", "joint-audit", "image-type-group"));
Directory.CreateDirectory(outputRoot);
var file = Path.Combine(outputRoot, "image-type-group.vmx.json");
using var editor = new GraphEditorDocument(registry);
var group = editor.AddNode(WorkflowCalls.ModuleId, WorkflowCalls.Version, 0, 0);
var body = editor.EnterOwnedGroup(group);
var sourceNode = editor.AddNode(source.Manifest.Id, source.Manifest.Version, 0, 0, sourceParameters);
var binaryNode = editor.AddNode(binary.Manifest.Id, binary.Manifest.Version, 180, 0, binaryParameters);
editor.Connect(sourceNode, "Image", binaryNode, "Image");
editor.SetWorkflowConfiguration(body, new()
{
    Outputs = [new("out", "Result", DataKind.GrayImage, binaryNode, "Image")]
});
editor.LeaveGroup();
var consumer = editor.AddNode(binary.Manifest.Id, binary.Manifest.Version, 180, 0, binaryParameters);
editor.Connect(group, "Result", consumer, "Image");

async Task Run(GraphEditorDocument graph, string label, IImageData image,
    NodeRunStatus expectedBinaryStatus, string? expectedBinaryError,
    NodeRunStatus expectedGroupStatus, NodeRunStatus expectedConsumerStatus,
    bool expectMissingMaskOutput = false)
{
    using var scope = new ModuleExecutionScope();
    source.SubmitImage(scope.ForProcedure(body), sourceNode, image);
    var result = await new SolutionExecutor(registry).RunAsync(
        new(graph.Document, graph.BaseDirectory), "main", scope: scope);
    var bodyRecords = result.Calls[group].Nodes.ToDictionary(n => n.NodeId);
    var sourceRecord = bodyRecords[sourceNode];
    var binaryRecord = bodyRecords[binaryNode];
    var groupRecord = result.Nodes.Single(n => n.NodeId == group);
    var consumerRecord = result.Nodes.Single(n => n.NodeId == consumer);
    var inputPort = graph.Document.Procedures.Single(p => p.Id == body).Connections.Single().FromPort;
    var outMono = graph.Document.Procedures.Single(p => p.Id == body).Nodes.Single(n => n.Id == sourceNode)
        .Parameters["OutMono8"].GetBoolean();
    var hasBinaryOutput = binaryRecord.Outputs.TryGetValue("Image", out var binaryImage);
    var hasConsumerOutput = consumerRecord.Outputs.TryGetValue("Image", out var consumerImage);
    var hasMono = sourceRecord.Outputs.TryGetValue("ImageMono8", out var sourceMono);
    var missingMask = result.Calls[group].OutputErrors.Any(e => e.StartsWith("MaskResult:", StringComparison.Ordinal));
    if (sourceRecord.Status != NodeRunStatus.Success ||
        hasMono != (image is RgbImage && outMono) ||
        (hasMono && sourceMono!.Kind != DataKind.GrayImage) ||
        binaryRecord.Status != expectedBinaryStatus || binaryRecord.ErrorCode != expectedBinaryError ||
        groupRecord.Status != expectedGroupStatus || consumerRecord.Status != expectedConsumerStatus ||
        (expectedGroupStatus == NodeRunStatus.Failed && groupRecord.ErrorCode != "SubprogramFailed") ||
        (expectedConsumerStatus == NodeRunStatus.Skipped && consumerRecord.ErrorCode != "ExecutionPredecessorUnavailable") ||
        missingMask != expectMissingMaskOutput ||
        hasBinaryOutput != (expectedBinaryStatus == NodeRunStatus.Success) ||
        hasConsumerOutput != (expectedConsumerStatus == NodeRunStatus.Success) ||
        (hasBinaryOutput && (binaryImage!.Kind != DataKind.GrayImage || binaryImage.As<GrayImage>().Width != 2)) ||
        (hasConsumerOutput && (consumerImage!.Kind != DataKind.GrayImage || consumerImage.As<GrayImage>().Width != 2)))
        throw new Exception($"{label}: unexpected status, error or actual downstream payload");
    Console.WriteLine($"{label}: input={image.Format}, port={inputPort}, OutMono8={outMono}, " +
        $"source={sourceRecord.Status}, binary={binaryRecord.Status}/{binaryRecord.ErrorCode}, " +
        $"group={groupRecord.Status}/{groupRecord.ErrorCode}, consumer={consumerRecord.Status}/{consumerRecord.ErrorCode}, " +
        $"binaryOutput={binaryRecord.Outputs.GetValueOrDefault("Image")?.Kind}, " +
        $"consumerOutput={consumerRecord.Outputs.GetValueOrDefault("Image")?.Kind}");
}

await Run(editor, "gray-direct", gray, NodeRunStatus.Success, null,
    NodeRunStatus.Success, NodeRunStatus.Success);
await Run(editor, "rgb-direct", rgb, NodeRunStatus.Failed, "InputTypeMismatch",
    NodeRunStatus.Failed, NodeRunStatus.Skipped);
editor.EnterOwnedGroup(group);
editor.Disconnect(editor.Document.Procedures.Single(p => p.Id == body).Connections.Single());
editor.Connect(sourceNode, "ImageMono8", binaryNode, "Image");
editor.LeaveGroup();
await Run(editor, "rgb-mono-disabled", rgb, NodeRunStatus.Skipped, "UpstreamUnavailable",
    NodeRunStatus.Failed, NodeRunStatus.Skipped);
editor.EnterOwnedGroup(group);
var node = editor.Document.Procedures.Single(p => p.Id == body).Nodes.Single(n => n.Id == sourceNode);
var enabled = node.Parameters.ToDictionary(p => p.Key, p => p.Value.Clone());
enabled["OutMono8"] = JsonSerializer.SerializeToElement(true);
editor.UpdateNode(sourceNode, node.Name, node.Enabled, node.TimeoutMs, enabled);
editor.LeaveGroup();
await Run(editor, "rgb-mono-enabled", rgb, NodeRunStatus.Success, null,
    NodeRunStatus.Success, NodeRunStatus.Success);
editor.Save(file);
using var reopened = new GraphEditorDocument(registry);
reopened.Load(file);
await Run(reopened, "rgb-mono-reopened", rgb, NodeRunStatus.Success, null,
    NodeRunStatus.Success, NodeRunStatus.Success);
reopened.EnterOwnedGroup(group);
reopened.SetWorkflowConfiguration(body, new()
{
    Outputs =
    [
        new("out", "Result", DataKind.GrayImage, binaryNode, "Image"),
        new("mask", "MaskResult", DataKind.GrayImage, binaryNode, "OutMask")
    ]
});
reopened.LeaveGroup();
await Run(reopened, "optional-mask-disabled", rgb, NodeRunStatus.Success, null,
    NodeRunStatus.Failed, NodeRunStatus.Skipped, expectMissingMaskOutput: true);
reopened.EnterOwnedGroup(group);
var tool = reopened.Document.Procedures.Single(p => p.Id == body).Nodes.Single(n => n.Id == binaryNode);
var maskEnabled = tool.Parameters.ToDictionary(p => p.Key, p => p.Value.Clone());
maskEnabled["OutputMaskImage"] = JsonSerializer.SerializeToElement(true);
reopened.UpdateNode(binaryNode, tool.Name, tool.Enabled, tool.TimeoutMs, maskEnabled);
reopened.LeaveGroup();
await Run(reopened, "optional-mask-enabled", rgb, NodeRunStatus.Success, null,
    NodeRunStatus.Success, NodeRunStatus.Success);
Console.WriteLine("FrameworkImageTypeGroup.Checks: 7 real graph executions passed.");
