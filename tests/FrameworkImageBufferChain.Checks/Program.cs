using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.ImageBinary;
using VisionPlugins.ImageSource;

var source = new ImageSourcePlugin();
var buffer = new ImageBufferPlugin();
var binary = new ImageBinaryPlugin();
var registry = new ModuleRegistry();
var plugins = new ToolPluginRegistry();
plugins.Register(source, registry);
plugins.Register(buffer, registry);
plugins.Register(binary, registry);
using var scope = new ModuleExecutionScope();
var executor = new GraphExecutor(registry);
var sourceParameters = source.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
sourceParameters["ImageSourceType"] = JsonSerializer.SerializeToElement(3);
sourceParameters["StitchEnable"] = JsonSerializer.SerializeToElement(false);
sourceParameters["OutMono8"] = JsonSerializer.SerializeToElement(false);
var binaryParameters = binary.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
var gray = new GrayImage(2, 2, [10, 40, 100, 200]);
var rgb = new RgbImage(2, 2, [33, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120]);
var passed = 0;

async Task Check(string name, IImageData image, int flag, int count, int outputSlot,
    NodeRunStatus bufferStatus, string? bufferError, int[] cachedKinds, byte[] firstPixels,
    NodeRunStatus binaryStatus, string? binaryError, DataKind? binaryImageKind, uint? nativeError = null)
{
    var procedure = new ProcedureDocument("buffer-chain", name,
        [new NodeDocument("source", source.Manifest.Id, source.Manifest.Version, "图像源", sourceParameters),
         new NodeDocument("buffer", buffer.Manifest.Id, buffer.Manifest.Version, "缓存图像", new()),
         new NodeDocument("binary", binary.Manifest.Id, binary.Manifest.Version, "图像二值化", binaryParameters)],
        [new ConnectionDocument("source", "Image", "buffer", "InputImage"),
         new ConnectionDocument("buffer", $"OutImage{outputSlot}Image", "binary", "Image")],
        [new ExecutionLinkDocument("source", "buffer"), new ExecutionLinkDocument("buffer", "binary")])
    {
        Configuration = new()
        {
            Subscriptions =
            [
                new("buffer", "RefreshFlag", false, new(WorkflowSourceKind.IntegerConstant, flag.ToString())),
                new("buffer", "BufferCount", false, new(WorkflowSourceKind.IntegerConstant, count.ToString()))
            ]
        }
    };
    if (!GraphValidator.Validate(procedure, registry).IsValid)
        throw new Exception($"{name}: graph validation failed");
    source.SubmitImage(scope.ForProcedure(procedure.Id), "source", image);
    var run = await executor.RunAsync(procedure, Environment.CurrentDirectory, scope: scope);
    var records = run.Nodes.ToDictionary(n => n.NodeId);
    var sourceRecord = records["source"];
    var bufferRecord = records["buffer"];
    var binaryRecord = records["binary"];
    var actualKinds = Enumerable.Range(0, 15)
        .Select(i => bufferRecord.Outputs.TryGetValue($"OutImage{i}Image", out var value) ? (int?)value.Kind : null)
        .Where(k => k.HasValue).Select(k => k!.Value).ToArray();
    var actualPixels = Enumerable.Range(0, 15)
        .Select(i => bufferRecord.Outputs.TryGetValue($"OutImage{i}Image", out var value)
            ? (byte?)value.As<IImageData>().Pixels[0] : null)
        .Where(p => p.HasValue).Select(p => p!.Value).ToArray();
    DataKind? actualBinaryKind = binaryRecord.Outputs.TryGetValue("Image", out var output) ? output.Kind : null;
    if (sourceRecord.Status != NodeRunStatus.Success ||
        bufferRecord.Status != bufferStatus || bufferRecord.ErrorCode != bufferError ||
        !actualKinds.SequenceEqual(cachedKinds) || !actualPixels.SequenceEqual(firstPixels) ||
        (bufferRecord.NativeErrorCode ?? 0) != (nativeError ?? 0) ||
        binaryRecord.Status != binaryStatus || binaryRecord.ErrorCode != binaryError ||
        actualBinaryKind != binaryImageKind)
        throw new Exception($"{name}: source={sourceRecord.Status}, buffer={bufferRecord.Status}/{bufferRecord.ErrorCode}/{bufferRecord.NativeErrorCode} [{string.Join(',', actualKinds)}; {string.Join(',', actualPixels)}], binary={binaryRecord.Status}/{binaryRecord.ErrorCode}/{actualBinaryKind}");
    passed++;
    Console.WriteLine($"PASS {name}: buffer={bufferRecord.Status}/{bufferRecord.ErrorCode ?? "none"} [{string.Join(',', actualKinds)}; {string.Join(',', actualPixels)}], binary={binaryRecord.Status}/{binaryRecord.ErrorCode ?? "none"}");
}

var G = (int)DataKind.GrayImage;
var R = (int)DataKind.RgbImage;
await Check("gray-refresh-15-slot0", gray, 1, 15, 0, NodeRunStatus.Success, null, [G], [10], NodeRunStatus.Success, null, DataKind.GrayImage);
await Check("rgb-refresh-15-slot0", rgb, 1, 15, 0, NodeRunStatus.Success, null, [G, R], [10, 33], NodeRunStatus.Success, null, DataKind.GrayImage);
await Check("rgb-hold-15-slot1", rgb, 0, 15, 1, NodeRunStatus.Success, null, [G, R], [10, 33], NodeRunStatus.Failed, "InputTypeMismatch", null);
await Check("gray-refresh-0-slot0", gray, 1, 0, 0, NodeRunStatus.Success, null, [], [], NodeRunStatus.Skipped, "UpstreamUnavailable", null);
await Check("gray-refresh-15-empty-slot1", gray, 1, 15, 1, NodeRunStatus.Success, null, [G], [10], NodeRunStatus.Skipped, "UpstreamUnavailable", null);
await Check("rgb-refresh-16-slot0", rgb, 1, 16, 0, NodeRunStatus.Failed, "InvalidBufferCount", [], [], NodeRunStatus.Skipped, "ExecutionPredecessorUnavailable", null, 0xe000031c);
await Check("rgb-hold-15-preserved-slot0", rgb, 0, 15, 0, NodeRunStatus.Success, null, [G], [10], NodeRunStatus.Success, null, DataKind.GrayImage);
await Check("rgb-refresh-15-slot1", rgb, 1, 15, 1, NodeRunStatus.Success, null, [G, R], [10, 33], NodeRunStatus.Failed, "InputTypeMismatch", null);
Console.WriteLine($"FrameworkImageBufferChain.Checks: {passed} graph cases passed.");
