using System.Text.Json;
using VisionFramework;
using VisionEditor.Core;

var descriptor = new ModuleDescriptor("test.branch-selector", "1", "branch selector", [], [], []);
var selection = (IReadOnlyList<string>?)null;
var observed = new List<IReadOnlyList<ExecutionSuccessor>>();
var registry = new ModuleRegistry();
registry.Register(descriptor, () => new BranchModule(descriptor, () => selection, observed));
var nodes = new[] { "root", "left", "right" }.Select(id => new NodeDocument(id, descriptor.Id, descriptor.Version, id, new Dictionary<string, JsonElement>())).ToArray();
var graph = new ProcedureDocument("branch", "branch", nodes, [], [new("root", "left"), new("root", "right")]);
var executor = new GraphExecutor(registry);
int checks = 0;
void Check(bool condition, string name) { if (!condition) throw new Exception(name); checks++; }
async Task<ExecutionSessionResult> Run(IReadOnlyList<string>? selected) { selection = selected; observed.Clear(); return await executor.RunAsync(graph, "."); }
var baseline = await Run(null);
Check(baseline.Nodes.All(n => n.Status == NodeRunStatus.Success), "null selection retains legacy fan-out");
Check(observed[0].Select(x => x.NodeId).SequenceEqual(["left", "right"]) && observed[0].All(x => x.NumericId is null), "source sees ordered actual successors without invented numeric IDs");
var left = await Run(["left"]);
Check(left.Nodes.Single(x => x.NodeId == "left").Status == NodeRunStatus.Success && left.Nodes.Single(x => x.NodeId == "right").Status == NodeRunStatus.Skipped, "selected branch only");
var none = await Run([]);
Check(none.Nodes.Where(x => x.NodeId != "root").All(x => x.Status == NodeRunStatus.Skipped), "empty selection skips all successors");
var invalid = await Run(["unknown"]);
Check(invalid.Nodes.Single(x => x.NodeId == "root").ErrorCode == "InvalidExecutionSuccessor" && invalid.Nodes.Where(x => x.NodeId != "root").All(x => x.Status == NodeRunStatus.Skipped), "foreign successor rejected");
selection = ["left"]; observed.Clear();
var isolated = graph with { Nodes = [nodes[0]], ExecutionLinks = [], Connections = [] };
var selfRun = await executor.RunAsync(isolated, ".", executionSuccessorRead: id => id == "root" ? [new("left"), new("right")] : null);
Check(selfRun.Nodes.Count == 1 && selfRun.Nodes[0].Status == NodeRunStatus.Success && observed[0].Select(x => x.NodeId).SequenceEqual(["left", "right"]), "isolated self-run observes original successors without dispatching absent nodes");
selection = ["left"]; observed.Clear();
var debugRun = await new NodeDebugExecutor(registry).RunAsync(graph,"root",".",new Dictionary<string,JsonElement>());
Check(debugRun.Status==NodeRunStatus.Success&&observed.Count==1&&observed[0].Select(x=>x.NodeId).SequenceEqual(["left","right"]),
    "node debug observes original successors without dispatching absent nodes");
var editor = new GraphEditorDocument(registry);
var first = editor.AddNode(descriptor.Id, descriptor.Version, 0, 0);
var second = editor.AddNode(descriptor.Id, descriptor.Version, 100, 0);
int topologyChanges = 0;
editor.PrepareExecutionTopologyChange = (before, after) => { topologyChanges++; return after; };
editor.ConnectExecution(first, second);
Check(topologyChanges == 1, "real connection edit invokes topology preparation");
editor.SetExecutionPath(new(first, second), [new WirePointDocument(20, 10), new WirePointDocument(80, 10)]);
editor.Undo(); editor.Redo();
Check(topologyChanges == 1, "geometry edit and undo/redo do not synthesize topology change");
editor.DisconnectExecution(new(first, second));
Check(topologyChanges == 2, "real disconnect invokes topology preparation");
var failedDescriptor = new ModuleDescriptor("test.failed-publish", "1", "failed publish", [], [new PortDescriptor("ModuStatus", DataKind.Integer)], []);
var failedRegistry = new ModuleRegistry();
failedRegistry.Register(failedDescriptor, () => new FailedPublishingModule(failedDescriptor, true));
var failedNodes = new[] { "failed", "downstream" }.Select(id => new NodeDocument(id, failedDescriptor.Id, failedDescriptor.Version, id, new Dictionary<string, JsonElement>())).ToArray();
var failedGraph = new ProcedureDocument("failed-publish", "failed-publish", failedNodes, [], [new("failed", "downstream")]);
var published = await new GraphExecutor(failedRegistry).RunAsync(failedGraph, ".");
Check(published.Nodes[0].Status == NodeRunStatus.Failed && published.Nodes[0].Outputs["ModuStatus"].As<long>() == 0 && published.Nodes[0].NativeErrorCode == 0xe0000314,
    "opted-in failed result retains validated output and native code");
Check(published.Nodes[1].Status == NodeRunStatus.Skipped && published.Nodes[1].Outputs.Count == 0,
    "failed result with published fields still blocks successors");
var defaultRegistry = new ModuleRegistry();
defaultRegistry.Register(failedDescriptor, () => new FailedPublishingModule(failedDescriptor, false));
var defaultFailure = await new GraphExecutor(defaultRegistry).RunAsync(failedGraph, ".");
Check(defaultFailure.Nodes[0].Status == NodeRunStatus.Failed && defaultFailure.Nodes[0].Outputs.Count == 0,
    "ordinary module failure still discards outputs by default");
var restoreDocument = new GraphEditorDocument(registry);
restoreDocument.AddNode(descriptor.Id,descriptor.Version,0,0);
Directory.CreateDirectory(Path.Combine("analysis","next-batch","restore-contract"));
var restorePath=Path.GetFullPath(Path.Combine("analysis","next-batch","restore-contract","branch-load.vmx.json"));
restoreDocument.Save(restorePath);
int loadPreparations=0,replacements=0;
restoreDocument.SolutionReplaced+=()=>replacements++;
restoreDocument.PrepareLoad=(loaded,_)=>{loadPreparations++;return loaded;};
restoreDocument.AddNode(descriptor.Id,descriptor.Version,100,0);
restoreDocument.Undo();restoreDocument.Redo();restoreDocument.Save(restorePath);
Check(loadPreparations==0,"Save, Undo, and Redo do not synthesize a restore event");
var oldRevision=restoreDocument.Revision;var oldPath=restoreDocument.FilePath;var oldNodeCount=restoreDocument.CurrentProcedure.Nodes.Count;
restoreDocument.PrepareLoad=(_,_)=>{loadPreparations++;throw new InvalidDataException("restoration rejected");};
try{restoreDocument.Load(restorePath);throw new Exception("accepted failed restore");}catch(InvalidDataException ex)when(ex.Message=="restoration rejected"){}
Check(restoreDocument.Revision==oldRevision&&restoreDocument.FilePath==oldPath&&restoreDocument.CurrentProcedure.Nodes.Count==oldNodeCount&&replacements==0,
    "failed restore leaves document, path, revision and replacement event untouched");
restoreDocument.PrepareLoad=(loaded,_)=>{loadPreparations++;return loaded;};
restoreDocument.Load(restorePath);
Check(loadPreparations==2&&replacements==1,"only successful real Load commits one restore callback and replacement event");
Console.WriteLine($"PASS {checks} branch execution contract checks");

sealed class BranchModule(ModuleDescriptor descriptor, Func<IReadOnlyList<string>?> selected, List<IReadOnlyList<ExecutionSuccessor>> observed) : IModule {
    public ModuleDescriptor Descriptor => descriptor;
    public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context, CancellationToken cancellationToken) {
        observed.Add(context.ExecutionSuccessors);
        return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success, new Dictionary<string, PortValue>()) { SelectedExecutionSuccessors = context.NodeId == "root" ? selected() : null });
    }
}

sealed class FailedPublishingModule(ModuleDescriptor descriptor, bool preserve) : IModule {
    public ModuleDescriptor Descriptor => descriptor;
    public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context, CancellationToken cancellationToken) =>
        ValueTask.FromResult(new ModuleResult(ModuleStatus.Failed,
            new Dictionary<string, PortValue> { ["ModuStatus"] = PortValue.Integer(0) }, "AllInputsUnreadable") {
            NativeErrorCode = 0xe0000314,
            PreserveFailedOutputs = preserve
        });
}
