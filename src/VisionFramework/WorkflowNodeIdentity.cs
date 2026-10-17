namespace VisionFramework;

/// <summary>
/// Identifies one node instance across a solution, including imported workflows that
/// happen to reuse the same node ID. ModuleId identifies a tool type, not an instance.
/// </summary>
public readonly record struct WorkflowNodeIdentity(string SolutionId, string ProcedureId, string NodeId)
{
    public static WorkflowNodeIdentity Create(string solutionId, string procedureId, string nodeId)
    {
        if (string.IsNullOrWhiteSpace(solutionId)) throw new ArgumentException("Solution id is required.", nameof(solutionId));
        if (string.IsNullOrWhiteSpace(procedureId)) throw new ArgumentException("Procedure id is required.", nameof(procedureId));
        if (string.IsNullOrWhiteSpace(nodeId)) throw new ArgumentException("Node id is required.", nameof(nodeId));
        return new WorkflowNodeIdentity(solutionId, procedureId, nodeId);
    }
}
