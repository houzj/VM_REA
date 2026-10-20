using VisionFramework;
namespace VisionEditor.Core;
// Native UI stores source identities, then exports/copies current module data on paste.
// OwnedProcedures is the managed transport for that graph; it is not a .gro binary.
public sealed record GraphFragment(IReadOnlyList<NodeDocument> Nodes,IReadOnlyList<ConnectionDocument> Connections,IReadOnlyList<ExecutionLinkDocument> ExecutionLinks)
{
 public IReadOnlyList<ProcedureDocument> OwnedProcedures{get;init;}=[];
 public string? SourceSolutionId{get;init;}
 public string? SourceProcedureId{get;init;}
 public string? SourcePath{get;init;}
 public IReadOnlyList<WorkflowSubscription> Subscriptions{get;init;}=[];
}

// Transient host hook, never serialized with a solution or clipboard fragment.
public sealed record GraphPasteContext(string? SourceProcedureId,string TargetProcedureId,IReadOnlyDictionary<string,string> NodeIds,IReadOnlyList<NodeDocument> SourceNodes,IReadOnlyList<NodeDocument> Nodes,IReadOnlyList<WorkflowSubscription> Subscriptions,IReadOnlyList<ExecutionLinkDocument> ExecutionLinks);
public sealed record GraphPastePreparation(IReadOnlyList<WorkflowSubscription> Subscriptions,Action? Committed=null){public IReadOnlyList<NodeDocument>? Nodes{get;init;}}
