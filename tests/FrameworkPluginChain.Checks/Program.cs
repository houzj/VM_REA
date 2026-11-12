using System.Text.Json;
using VisionCore;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.ImageBinary;
using VisionPlugins.ImageSource;
using VisionPlugins.Blob;

var source = new ImageSourcePlugin();
var binary = new ImageBinaryPlugin();
var blob = new BlobPlugin();
var registry = new ModuleRegistry();
var plugins = new ToolPluginRegistry();
plugins.Register(source, registry);
plugins.Register(binary, registry);
plugins.Register(blob, registry);
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
var blobParameters = blob.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
var blobModule = registry.Create(blob.Manifest.Id, blob.Manifest.Version);
var blobResult = await blobModule.ExecuteAsync(new ModuleExecutionContext("blob", Environment.CurrentDirectory,
    new Dictionary<string, PortValue> { ["Image"] = PortValue.Image(new GrayImage(4, 1, [0, 120, 130, 255])) }, blobParameters), CancellationToken.None);
if (blobResult.Status != ModuleStatus.Success || !blobResult.Outputs.ContainsKey("Components"))
    throw new Exception("blob baseline did not produce component output");
checkedCases++;
Console.WriteLine("PASS blob-baseline: managed component output");
var maskedParameters = blobParameters.ToDictionary(p => p.Key, p => p.Value.Clone());
maskedParameters["MinArea"] = JsonSerializer.SerializeToElement(1);
var maskedResult = await blobModule.ExecuteAsync(new ModuleExecutionContext("blob-mask", Environment.CurrentDirectory,
    new Dictionary<string, PortValue>
    {
        ["Image"] = PortValue.Image(new GrayImage(4, 1, [0, 120, 130, 255])),
        ["InputMask"] = PortValue.Image(new GrayImage(4, 1, [0, 255, 0, 0]))
    }, maskedParameters), CancellationToken.None);
if (maskedResult.Status != ModuleStatus.Success ||
    maskedResult.Outputs["Components"].As<IReadOnlyList<Component>>().Count != 1)
    throw new Exception("blob baseline mask input was not applied");
checkedCases++;
Console.WriteLine("PASS blob-mask-baseline: input mask applied");
var connectivityParameters = blobParameters.ToDictionary(p => p.Key, p => p.Value.Clone());
connectivityParameters["MinArea"] = JsonSerializer.SerializeToElement(1);
connectivityParameters["Connectivity"] = JsonSerializer.SerializeToElement(4);
var diagonalInput = new Dictionary<string, PortValue> { ["Image"] = PortValue.Image(new GrayImage(2, 2, [120, 0, 0, 120])) };
var fourConnected = await blobModule.ExecuteAsync(new ModuleExecutionContext("blob-4", Environment.CurrentDirectory, diagonalInput, connectivityParameters), CancellationToken.None);
connectivityParameters["Connectivity"] = JsonSerializer.SerializeToElement(8);
var eightConnected = await blobModule.ExecuteAsync(new ModuleExecutionContext("blob-8", Environment.CurrentDirectory, diagonalInput, connectivityParameters), CancellationToken.None);
if (fourConnected.Status != ModuleStatus.Success || eightConnected.Status != ModuleStatus.Success ||
    fourConnected.Outputs["Components"].As<IReadOnlyList<Component>>().Count != 2 ||
    eightConnected.Outputs["Components"].As<IReadOnlyList<Component>>().Count != 1)
    throw new Exception("blob connectivity parameter was not applied");
checkedCases++;
Console.WriteLine("PASS blob-connectivity: 4/8 connectivity split");
var unsupportedBlobParameters = blobParameters.ToDictionary(p => p.Key, p => p.Value.Clone());
unsupportedBlobParameters["ThresholdType"] = JsonSerializer.SerializeToElement(1);
var unsupportedBlob = await blobModule.ExecuteAsync(new ModuleExecutionContext("blob-mode", Environment.CurrentDirectory,
    new Dictionary<string, PortValue> { ["Image"] = PortValue.Image(new GrayImage(1, 1, [120])) }, unsupportedBlobParameters), CancellationToken.None);
if (unsupportedBlob.Status != ModuleStatus.Failed || unsupportedBlob.ErrorCode != "BlobModeUnavailable")
    throw new Exception("blob unsupported threshold mode was not rejected");
checkedCases++;
Console.WriteLine("PASS blob-mode-boundary: unsupported threshold rejected");
using (var newSolutionEditor = new GraphEditorDocument(registry))
{
    newSolutionEditor.NewSolution(true);
    if (!newSolutionEditor.HasCurrentProcedure || newSolutionEditor.CurrentProcedure.Nodes.Count != 0 ||
        newSolutionEditor.CurrentProcedure.Name != "Main")
        throw new Exception("new solution did not create its main execution procedure");
    checkedCases++;
    Console.WriteLine("PASS new-solution: created main execution procedure");
}
using (var startupEditor = new GraphEditorDocument(registry))
{
    var startupSource = startupEditor.AddNode(source.Manifest.Id, source.Manifest.Version, 180, 100,
        source.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone()));
    var startupBinary = startupEditor.AddNode(binary.Manifest.Id, binary.Manifest.Version, 540, 100,
        binary.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone()));
    startupEditor.BindInput(startupSource, "Image", startupBinary, "Image", addExecutionLink: true);
    var startupProcedure = startupEditor.CurrentProcedure;
    if (startupProcedure.Connections.Count != 1 ||
        ProcedureGraph.ExecutionEdges(startupProcedure).Count() != 1 ||
        !GraphValidator.Validate(startupProcedure, registry).IsValid)
        throw new Exception("startup solution graph did not retain its data and execution carriers");
    var startupPath = Path.Combine("analysis", "joint-audit", "framework-plugin-chain-roundtrip", "startup-solution.vmx.json");
    startupEditor.Save(startupPath);
    startupEditor.Load(startupPath);
    var reopened = startupEditor.CurrentProcedure;
    if (reopened.Connections.Count != 1 || ProcedureGraph.ExecutionEdges(reopened).Count() != 1 ||
        !GraphValidator.Validate(reopened, registry).IsValid)
        throw new Exception("reopened startup solution lost its graph carriers");
    var sample = Path.GetFullPath(Path.Combine("src", "VisionEditor", "bin", "Release", "net10.0-windows", "assets", "circle-sample.bmp"));
    if (!File.Exists(sample)) throw new FileNotFoundException("startup execution sample is missing", sample);
    startupEditor.SetParameter(startupSource, "ImageSourceType", JsonSerializer.SerializeToElement(1));
    startupEditor.SetParameter(startupSource, "CurrentImagePath", JsonSerializer.SerializeToElement(sample));
    startupEditor.SetParameter(startupSource, "ImagePaths", JsonSerializer.SerializeToElement(JsonSerializer.Serialize(new[] { sample })));
    using var startupScope = new ModuleExecutionScope();
    var startupRun = await new GraphExecutor(registry).RunAsync(startupEditor.CurrentProcedure, Environment.CurrentDirectory, scope: startupScope);
    if (startupRun.Status != SessionStatus.Success || startupRun.Nodes.Count != 2 ||
        startupRun.Nodes.Any(node => node.Status != NodeRunStatus.Success) ||
        !startupRun.Nodes.Single(node => node.NodeId == startupBinary).Outputs.ContainsKey("Image"))
        throw new Exception("startup solution execution did not carry image data through the binary tool");
    var startupSolution = new SolutionDocument(SolutionStore.CurrentVersion, "startup-solution", "启动方案",
        [startupEditor.CurrentProcedure], []);
    using var solutionScope = new ModuleExecutionScope();
    var solutionRun = await new SolutionExecutor(registry).RunAsync(
        new SolutionWorkspace(startupSolution, Environment.CurrentDirectory), "main", scope: solutionScope);
    if (solutionRun.Status != SessionStatus.Success || solutionRun.Nodes.Count != 2 ||
        solutionRun.Nodes.Any(node => node.Status != NodeRunStatus.Success))
        throw new Exception("solution executor did not consume the persisted execution carrier");
    checkedCases++;
    Console.WriteLine("PASS startup-solution: persisted data, execution carriers and real execution");
}
using (var blobSolutionEditor = new GraphEditorDocument(registry))
{
    var sourceNode = blobSolutionEditor.AddNode(source.Manifest.Id, source.Manifest.Version, 160, 100,
        source.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone()));
    var blobNode = blobSolutionEditor.AddNode(blob.Manifest.Id, blob.Manifest.Version, 520, 100,
        blob.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone()));
    var sample = Path.GetFullPath(Path.Combine("src", "VisionEditor", "bin", "Release", "net10.0-windows", "assets", "circle-sample.bmp"));
    blobSolutionEditor.SetParameter(sourceNode, "ImageSourceType", JsonSerializer.SerializeToElement(1));
    blobSolutionEditor.SetParameter(sourceNode, "CurrentImagePath", JsonSerializer.SerializeToElement(sample));
    blobSolutionEditor.SetParameter(sourceNode, "ImagePaths", JsonSerializer.SerializeToElement(JsonSerializer.Serialize(new[] { sample })));
    blobSolutionEditor.BindInput(sourceNode, "Image", blobNode, "Image", addExecutionLink: true);
    var blobBaselinePath = Path.Combine("analysis", "joint-audit", "framework-plugin-chain-roundtrip", "blob-baseline.vmx.json");
    blobSolutionEditor.Save(blobBaselinePath);
    blobSolutionEditor.Load(blobBaselinePath);
    if (blobSolutionEditor.CurrentProcedure.Connections.Count != 1 ||
        ProcedureGraph.ExecutionEdges(blobSolutionEditor.CurrentProcedure).Count() != 1 ||
        !GraphValidator.Validate(blobSolutionEditor.CurrentProcedure, registry).IsValid)
        throw new Exception("saved and reopened blob baseline lost its graph carriers");
    using var blobScope = new ModuleExecutionScope();
    var blobSolution = new SolutionDocument(SolutionStore.CurrentVersion, "blob-baseline", "当前 Blob 基线",
        [blobSolutionEditor.CurrentProcedure], []);
    var blobRun = await new SolutionExecutor(registry).RunAsync(
        new SolutionWorkspace(blobSolution, Environment.CurrentDirectory), "main", scope: blobScope);
    if (blobRun.Status != SessionStatus.Success || blobRun.Nodes.Count != 2 ||
        blobRun.Nodes.Any(node => node.Status != NodeRunStatus.Success) ||
        !blobRun.Nodes.Single(node => node.NodeId == blobNode).Outputs.ContainsKey("Components"))
        throw new Exception("image-source to blob solution baseline did not execute");
    checkedCases++;
    Console.WriteLine("PASS image-source-blob-solution: end-to-end current baseline");
}
Console.WriteLine($"FrameworkPluginChain.Checks: {checkedCases} graph cases passed.");
