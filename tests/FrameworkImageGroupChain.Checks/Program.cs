using System.Text.Json;
using VisionCore;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.ImageBinary;
using VisionPlugins.ImageSource;

var source = new ImageSourcePlugin();
var buffer = new ImageBufferPlugin();
var binary = new ImageBinaryPlugin();
var registry = new ModuleRegistry();
WorkflowCalls.Register(registry);
var plugins = new ToolPluginRegistry();
plugins.Register(source, registry);
plugins.Register(buffer, registry);
plugins.Register(binary, registry);
var sourceParameters = source.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
sourceParameters["ImageSourceType"] = JsonSerializer.SerializeToElement(3);
sourceParameters["StitchEnable"] = JsonSerializer.SerializeToElement(false);
sourceParameters["OutMono8"] = JsonSerializer.SerializeToElement(false);
var binaryParameters = binary.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
var sample = new GrayImage(2, 2, [10, 40, 100, 200]);
var outputRoot = Path.GetFullPath(Path.Combine("analysis", "joint-audit", "image-group-chain"));
Directory.CreateDirectory(outputRoot);
var file = Path.Combine(outputRoot, "owned-image-chain.vmx.json");
using var editor = new GraphEditorDocument(registry);
var group = editor.AddNode(WorkflowCalls.ModuleId, WorkflowCalls.Version, 0, 0);
var body = editor.EnterOwnedGroup(group);
var sourceNode = editor.AddNode(source.Manifest.Id, source.Manifest.Version, 0, 0, sourceParameters);
var bufferNode = editor.AddNode(buffer.Manifest.Id, buffer.Manifest.Version, 180, 0);
var firstBinary = editor.AddNode(binary.Manifest.Id, binary.Manifest.Version, 360, 0, binaryParameters);
var secondBinary = editor.AddNode(binary.Manifest.Id, binary.Manifest.Version, 540, 0, binaryParameters);
editor.Connect(sourceNode, "Image", bufferNode, "InputImage");
editor.Connect(bufferNode, "OutImage0Image", firstBinary, "Image");
editor.Connect(firstBinary, "Image", secondBinary, "Image");
editor.SetWorkflowConfiguration(body, new()
{
    Outputs = [new("out", "Result", DataKind.GrayImage, secondBinary, "Image")]
});
editor.LeaveGroup();
var consumer = editor.AddNode(binary.Manifest.Id, binary.Manifest.Version, 180, 0, binaryParameters);
editor.Connect(group, "Result", consumer, "Image");

void CheckIdentity(string stage)
{
    var doc = editor.Document;
    var parent = doc.Procedures.Single(p => p.Id == "main");
    var child = doc.Procedures.Single(p => p.Id == body);
    if (child.GroupOwner?.NodeId != group || child.GroupOwner.ProcedureId != parent.Id ||
        parent.Nodes.Single(n => n.Id == group).Call?.ProcedureId != body ||
        parent.Connections.Single(c => c.ToNode == consumer).FromPort != "Result" ||
        child.Connections.Count != 3 || child.Nodes.Select(n => n.Id).Distinct().Count() != 4)
        throw new Exception($"{stage}: Group identity, ports or internal chain changed");
    Console.WriteLine($"PASS {stage}: owned Group identity/ports/three internal links");
}

async Task Run(string stage, ModuleExecutionScope scope, NodeRunStatus groupStatus,
    NodeRunStatus consumerStatus, string? consumerError, bool expectOutput)
{
    source.SubmitImage(scope.ForProcedure(body), sourceNode, sample);
    var result = await new SolutionExecutor(registry).RunAsync(
        new(editor.Document, editor.BaseDirectory), "main", scope: scope);
    var groupRecord = result.Nodes.Single(n => n.NodeId == group);
    var consumerRecord = result.Nodes.Single(n => n.NodeId == consumer);
    var bodyRun = result.Calls[group];
    var bodyNodes = bodyRun.Nodes.ToDictionary(n => n.NodeId);
    var hasOutput = consumerRecord.Outputs.TryGetValue("Image", out var value);
    if (groupRecord.Status != groupStatus || consumerRecord.Status != consumerStatus ||
        consumerRecord.ErrorCode != consumerError || hasOutput != expectOutput ||
        bodyNodes.Count != 4 || bodyNodes[sourceNode].Status != NodeRunStatus.Success ||
        bodyNodes[bufferNode].Status != NodeRunStatus.Success ||
        bodyNodes[firstBinary].Status != (expectOutput ? NodeRunStatus.Success : NodeRunStatus.Skipped) ||
        bodyNodes[secondBinary].Status != (expectOutput ? NodeRunStatus.Success : NodeRunStatus.Skipped) ||
        (hasOutput && (value!.Kind != DataKind.GrayImage || value.As<GrayImage>().Width != 2)))
        throw new Exception($"{stage}: Group={groupRecord.Status}/{groupRecord.ErrorCode}, consumer={consumerRecord.Status}/{consumerRecord.ErrorCode}, output={hasOutput}");
    Console.WriteLine($"PASS {stage}: Group={groupRecord.Status}, consumer={consumerRecord.Status}/{consumerRecord.ErrorCode ?? "none"}, output={hasOutput}");
}

CheckIdentity("created");
using (var scope = new ModuleExecutionScope())
{
    await Run("initial-success", scope, NodeRunStatus.Success, NodeRunStatus.Success, null, true);
    editor.SetProcedureInputSubscription(body, bufferNode, "BufferCount",
        new(WorkflowSourceKind.IntegerConstant, "0"));
    await Run("zero-capacity-failure", scope, NodeRunStatus.Failed, NodeRunStatus.Skipped,
        "ExecutionPredecessorUnavailable", false);
    editor.Undo();
    CheckIdentity("undo-restored");
    await Run("undo-success", scope, NodeRunStatus.Success, NodeRunStatus.Success, null, true);
    editor.Redo();
    await Run("redo-failure", scope, NodeRunStatus.Failed, NodeRunStatus.Skipped,
        "ExecutionPredecessorUnavailable", false);
    editor.Undo();
}
editor.Save(file);
using var reopened = new GraphEditorDocument(registry);
reopened.Load(file);
if (reopened.Document.Procedures.Single(p => p.Id == body).GroupOwner?.NodeId != group ||
    reopened.Document.Procedures.Single(p => p.Id == "main").Nodes.Single(n => n.Id == group).Call?.ProcedureId != body ||
    reopened.Document.Procedures.Single(p => p.Id == body).Connections.Count != 3 ||
    reopened.Document.Procedures.Single(p => p.Id == "main").Connections.Single().FromPort != "Result")
    throw new Exception("saved/reopened Group identity or connection changed");
using (var freshScope = new ModuleExecutionScope())
{
    source.SubmitImage(freshScope.ForProcedure(body), sourceNode, sample);
    var run = await new SolutionExecutor(registry).RunAsync(
        new(reopened.Document, reopened.BaseDirectory), "main", scope: freshScope);
    var groupRecord = run.Nodes.Single(n => n.NodeId == group);
    var consumerRecord = run.Nodes.Single(n => n.NodeId == consumer);
    if (groupRecord.Status != NodeRunStatus.Success || consumerRecord.Status != NodeRunStatus.Success ||
        run.Calls[group].Nodes.Count != 4 ||
        consumerRecord.Outputs["Image"].Kind != DataKind.GrayImage)
        throw new Exception($"saved/reopened run: Group={groupRecord.Status}, consumer={consumerRecord.Status}/{consumerRecord.ErrorCode}");
    Console.WriteLine("PASS saved-reopened-fresh-scope: real consumer received GrayImage");
}
Console.WriteLine("FrameworkImageGroupChain.Checks: 5 executions plus identity/save checks passed.");
