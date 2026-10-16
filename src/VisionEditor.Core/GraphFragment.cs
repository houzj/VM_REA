using VisionFramework;
namespace VisionEditor.Core;
// Independent snapshot clipboard. Native VM ProcedurePaste remains an SDK boundary.
public sealed record GraphFragment(IReadOnlyList<NodeDocument> Nodes,IReadOnlyList<ConnectionDocument> Connections,IReadOnlyList<ExecutionLinkDocument> ExecutionLinks);
