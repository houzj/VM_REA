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
var plugins = new ToolPluginRegistry();
plugins.Register(source, registry);
plugins.Register(binary, registry);
var checkedCases = 0;

async Task Verify(string name, IImageData image, string port, bool outMono,
    NodeRunStatus expectedStatus, string? expectedError, DataKind? expectedOutput)
{
    using var scope = new ModuleExecutionScope();
    var sourceParameters = source.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
    sourceParameters["ImageSourceType"] = JsonSerializer.SerializeToElement(3);
    sourceParameters["OutMono8"] = JsonSerializer.SerializeToElement(outMono);
    sourceParameters["StitchEnable"] = JsonSerializer.SerializeToElement(false);
    var binaryParameters = binary.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
    var proposed = new ProcedureDocument("source-binary", name,
        [new NodeDocument("source", source.Manifest.Id, source.Manifest.Version, "图像源", sourceParameters),
         new NodeDocument("binary", binary.Manifest.Id, binary.Manifest.Version, "图像二值化", binaryParameters)],
        [new ConnectionDocument("source", port, "binary", "Image")],
        [new ExecutionLinkDocument("source", "binary")]);
    var outputDirectory = Path.GetFullPath(Path.Combine("analysis", "joint-audit", "framework-plugin-chain-roundtrip"));
    Directory.CreateDirectory(outputDirectory);
    var initialPath = Path.Combine(outputDirectory, name + "-initial.vmx.json");
    var savedPath = Path.Combine(outputDirectory, name + "-saved.vmx.json");
    SolutionStore.Save(initialPath, new(SolutionStore.CurrentVersion, name, name, [proposed], []));
    using var editor = new GraphEditorDocument(registry);
    editor.Load(initialPath);
    editor.Save(savedPath);
    editor.Load(savedPath);
    var procedure = editor.CurrentProcedure;
    if (procedure.Connections.Single().FromPort != port ||
        procedure.Connections.Single().ToPort != "Image" ||
        ProcedureGraph.ExecutionEdges(procedure).Count() != 1 ||
        procedure.Nodes.Single(n => n.Id == "source").Parameters["OutMono8"].GetBoolean() != outMono)
        throw new Exception($"{name}: editor save/reload changed the image binding or conditional output flag");
    if (!GraphValidator.Validate(procedure, registry).IsValid)
        throw new Exception($"{name}: static graph was unexpectedly rejected");
    source.SubmitImage(scope.ForProcedure(procedure.Id), "source", image);
    var run = await new GraphExecutor(registry).RunAsync(procedure, Environment.CurrentDirectory, scope: scope);
    var sourceRecord = run.Nodes.Single(n => n.NodeId == "source");
    var binaryRecord = run.Nodes.Single(n => n.NodeId == "binary");
    if (sourceRecord.Status != NodeRunStatus.Success || binaryRecord.Status != expectedStatus ||
        binaryRecord.ErrorCode != expectedError ||
        (binaryRecord.Outputs.TryGetValue("Image", out var output) ? output.Kind : null) != expectedOutput)
        throw new Exception($"{name}: source={sourceRecord.Status}, binary={binaryRecord.Status}/{binaryRecord.ErrorCode}, output={binaryRecord.Outputs.GetValueOrDefault("Image")?.Kind}");
    checkedCases++;
    Console.WriteLine($"PASS {name}: {binaryRecord.Status}/{binaryRecord.ErrorCode ?? "none"}");
}

var gray = new GrayImage(2, 2, [10, 40, 100, 200]);
var rgb = new RgbImage(2, 2, [10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120]);
await Verify("gray-main", gray, "Image", false, NodeRunStatus.Success, null, DataKind.GrayImage);
await Verify("rgb-main", rgb, "Image", false, NodeRunStatus.Failed, "InputTypeMismatch", null);
await Verify("rgb-mono-enabled", rgb, "ImageMono8", true, NodeRunStatus.Success, null, DataKind.GrayImage);
await Verify("rgb-mono-disabled", rgb, "ImageMono8", false, NodeRunStatus.Skipped, "UpstreamUnavailable", null);
Console.WriteLine($"FrameworkPluginChain.Checks: {checkedCases} graph cases passed.");
