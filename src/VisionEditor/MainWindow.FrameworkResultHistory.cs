using VisionEditor.Core;
using VisionFramework;

namespace VisionEditor;

public partial class MainWindow
{
    readonly FrameworkRunHistory frameworkRunHistory = new();

    /// <summary>模块级执行账本：历史结果/当前结果/图像三个视图都从它取数。</summary>
    internal ModuleRunLedger ModuleRuns { get; } = new();

    void InitializeFrameworkResultHistory()
    {
        doc.SolutionReplaced += ResetFrameworkRunHistory;
        ResetFrameworkRunHistory();
    }

    void RecordFrameworkRunHistory(string procedureId, ExecutionSessionResult session)
    {
        var name = doc.Document.Procedures.FirstOrDefault(procedure => procedure.Id == procedureId)?.Name ?? procedureId;
        var elapsed = session.Nodes.Sum(record => record.ElapsedMilliseconds);
        frameworkRunHistory.Append(procedureId, name, session.Status, elapsed, DateTime.Now);
        // 模块级记录：每个节点一条，供历史结果/当前结果/图像列表消费。
        ModuleRuns.AppendSession(procedureId, session,
            nodeId => doc.Document.Procedures.SelectMany(procedure => procedure.Nodes)
                .FirstOrDefault(node => node.Id == nodeId)?.Name ?? nodeId);
        AppendShellLog(session.Status == SessionStatus.Success ? ShellLogKind.Info : ShellLogKind.Warning,
            "流程「" + name + "」执行" + (session.Status == SessionStatus.Success ? "完成" : "结束") + "：" + session.Status
            + "，" + elapsed.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + " ms");
    }

    void ResetFrameworkRunHistory()
    {
        // Independent solution replacement policy for the host's own summary.
        // The original native/global module cache is not touched by this method.
        frameworkRunHistory.Clear();
        ModuleRuns.Clear();
        ProcessTimeText.Text = "0.00";
        ModuleTimeText.Text = "0.00";
        AlgorithmTimeText.Text = FrameworkResultTiming.Unavailable;
    }

    void RefreshFrameworkResultTiming()
    {
        var session = doc.HasCurrentProcedure && !clearedWorkflowResultViews.Contains(doc.CurrentProcedureId)
            ? workflowSessions.GetValueOrDefault(doc.CurrentProcedureId) : null;
        var timing = FrameworkResultTiming.From(session, results.Values, selected);
        ProcessTimeText.Text = timing.ProcessText;
        ModuleTimeText.Text = timing.ModuleText;
        AlgorithmTimeText.Text = timing.AlgorithmText;
    }
}
