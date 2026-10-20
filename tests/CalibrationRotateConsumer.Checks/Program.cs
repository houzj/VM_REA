using System.Text.Json;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
using VisionPlugins.CameraMapping;

var output = Path.GetFullPath("analysis/joint-audit/pass9-cal-rotate");
Directory.CreateDirectory(output);
var binary = Path.Combine(output, "camera.iwcal");
var absent = Path.Combine(output, "absent.iwcal");
File.Delete(binary);
File.Delete(absent);
var registry = new ModuleRegistry();
var catalog = new ToolPluginRegistry();
var camera = new CameraMappingPlugin();
var rotate = new RotateCalibrationPlugin();
catalog.Register(camera, registry);
catalog.Register(rotate, registry);
registry.Register(SampleSource.Definition, () => new SampleSource());
Dictionary<string, JsonElement> Defaults(IVisionToolPlugin plugin) => plugin.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
var cameraValues = Defaults(camera);
cameraValues["CalibPathName"] = JsonSerializer.SerializeToElement(binary);
cameraValues["RefreshFileEnable"] = JsonSerializer.SerializeToElement(true);
var rotateValues = Defaults(rotate);
rotateValues["FilePath"] = JsonSerializer.SerializeToElement(binary);
using var graph = new GraphEditorDocument(registry);
var producer = graph.AddNode(CameraMappingPlugin.Id, "1.0.0", 0, 0, cameraValues);
var source = graph.AddNode(SampleSource.Id, "1", 150, 0);
var consumer = graph.AddNode(RotateCalibrationPlugin.Id, "1.0.0", 300, 0, rotateValues);
graph.ConnectExecution(producer, source);
graph.ConnectExecution(source, consumer);
graph.Connect(producer, "CalibStatus", consumer, "RefreshSignal");
var subscriptions = new List<WorkflowSubscription>();
void Pair(string side, int pair, double[] x, double[] y)
{
    subscriptions.Add(new(producer, side + "PointX" + pair, false, new(WorkflowSourceKind.NumberArrayConstant, JsonSerializer.Serialize(x))));
    subscriptions.Add(new(producer, side + "PointY" + pair, false, new(WorkflowSourceKind.NumberArrayConstant, JsonSerializer.Serialize(y))));
}
Pair("Object", 1, [0, 10], [0, 0]);
Pair("Target", 1, [10, 30], [-5, -5]);
Pair("Object", 2, [0, 10], [10, 10]);
Pair("Target", 2, [10, 30], [15, 15]);
foreach (var (input, outputName) in new[] { ("RotNum", "Count"), ("ImagePointX", "X"), ("ImagePointY", "Y"), ("WorldRotateAngle", "Angle") })
    subscriptions.Add(new(consumer, input, false, new(WorkflowSourceKind.NodeOutput, source, outputName)));
graph.SetWorkflowConfiguration(graph.CurrentProcedureId, new() { Subscriptions = subscriptions });
void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); Console.WriteLine("PASS " + message); }
Check(RotateCalibrationFile.Read(absent, new float[9], out var missingLoaded) == 0xe0000201 && !missingLoaded, "missing binary returns file-open error with invalid cache");
var xmls = new[] { "analysis/joint-audit/pass6-cal-result/npoint-generated.xml", "analysis/joint-audit/pass8-cal-map/mapped.xml", "analysis/joint-audit/pass8-cal-translation/npoint-generated.xml" };
foreach (var xml in xmls)
{
    var matrix = new float[9]; var path = Path.GetFullPath(xml);
    uint code = RotateCalibrationFile.Read(path, matrix, out var loaded);
    Console.WriteLine($"XML {Path.GetFileName(Path.GetDirectoryName(path))}/{Path.GetFileName(path)} code=0x{code:x8} loaded={loaded} m00={matrix[0]:G9}");
    Check(code == 0 && loaded, "Rotate accepts producer XML: " + Path.GetFileName(Path.GetDirectoryName(path)));
}
using var scope = new ModuleExecutionScope();
for (int i = 0; i < 3; i++)
{
    var result = await new SolutionExecutor(registry).RunAsync(new(graph.Document, graph.BaseDirectory), "main", scope: scope);
    var runs = result.Nodes.ToDictionary(x => x.NodeId);
    Console.WriteLine($"run{i + 1}: camera={runs[producer].Status}/0x{runs[producer].NativeErrorCode:x8}; source={runs[source].Status}; rotate={runs[consumer].Status}/0x{runs[consumer].NativeErrorCode:x8}");
    Check(runs[producer].Status == NodeRunStatus.Success && File.Exists(binary) && new FileInfo(binary).Length == 36, "Camera generated binary before Rotate run " + (i + 1));
    Check(runs[consumer].Status == NodeRunStatus.Success, "Rotate consumed Camera binary on graph run " + (i + 1));
    var fields = runs[consumer].Outputs;
    Console.WriteLine($"remain={fields["remainCount"].As<long>()}, image-center={fields["RotCenterImageX"].As<double>():G9},{fields["RotCenterImageY"].As<double>():G9}");
    Check(fields["remainCount"].As<long>() == 2 - i, "sequential sample state remains in execution scope " + (i + 1));
}
Console.WriteLine("binary-sha256=" + Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(binary))));
var savedBytes = File.ReadAllBytes(binary);
using var recovery = new GraphEditorDocument(registry);
var recoverySource = recovery.AddNode(SampleSource.Id, "1", 0, 0);
var recoveryRotate = recovery.AddNode(RotateCalibrationPlugin.Id, "1.0.0", 160, 0, rotateValues);
recovery.ConnectExecution(recoverySource, recoveryRotate);
var recoveryBindings = new List<WorkflowSubscription>();
foreach (var (input, outputName) in new[] { ("RotNum", "Count"), ("ImagePointX", "X"), ("ImagePointY", "Y"), ("WorldRotateAngle", "Angle"), ("RefreshSignal", "Refresh") })
    recoveryBindings.Add(new(recoveryRotate, input, false, new(WorkflowSourceKind.NodeOutput, recoverySource, outputName)));
recovery.SetWorkflowConfiguration(recovery.CurrentProcedureId, new() { Subscriptions = recoveryBindings });
File.Delete(binary);
using var recoveryScope = new ModuleExecutionScope();
var failed = await new SolutionExecutor(registry).RunAsync(new(recovery.Document, recovery.BaseDirectory), "main", scope: recoveryScope);
var failedNode = failed.Nodes.Single(n => n.NodeId == recoveryRotate);
Console.WriteLine($"missing-recovery: rotate={failedNode.Status}/0x{failedNode.NativeErrorCode:x8}, outputs={string.Join(',', failedNode.Outputs.Keys)}");
Check(failedNode.Status == NodeRunStatus.Failed && failedNode.NativeErrorCode == 0xe0000008, "missing file fails Rotate Process with uninitialized matrix");
File.WriteAllBytes(binary, savedBytes);
var recovered = await new SolutionExecutor(registry).RunAsync(new(recovery.Document, recovery.BaseDirectory), "main", scope: recoveryScope);
var recoveredNode = recovered.Nodes.Single(n => n.NodeId == recoveryRotate);
Console.WriteLine($"restored-recovery: rotate={recoveredNode.Status}/0x{recoveredNode.NativeErrorCode:x8}, remain={recoveredNode.Outputs["remainCount"].As<long>()}");
Check(recoveredNode.Status == NodeRunStatus.Success && recoveredNode.Outputs["remainCount"].As<long>() == 2, "RefreshSignal reloads restored binary in same scope");

sealed class SampleSource : IModule
{
    public const string Id = "test.calibration.rotate.sample";
    static int next;
    public static ModuleDescriptor Definition { get; } = new(Id, "1", "sample", [],
        [new("X", DataKind.NumberArray), new("Y", DataKind.NumberArray), new("Angle", DataKind.NumberArray), new("Count", DataKind.Integer), new("Refresh", DataKind.Integer)], []);
    public ModuleDescriptor Descriptor => Definition;
    public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context, CancellationToken token)
    {
        var i = Interlocked.Increment(ref next) - 1;
        var theta = 2 * Math.PI * (i % 3) / 3;
        return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success, new Dictionary<string, PortValue>
        {
            ["X"] = PortValue.Numbers([50 + 10 * Math.Cos(theta)]),
            ["Y"] = PortValue.Numbers([50 + 10 * Math.Sin(theta)]),
            ["Angle"] = PortValue.Numbers([120]),
            ["Count"] = PortValue.Integer(3),
            ["Refresh"] = PortValue.Integer(1)
        }));
    }
}
