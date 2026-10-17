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
