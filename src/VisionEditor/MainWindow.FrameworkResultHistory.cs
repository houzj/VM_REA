using VisionEditor.Core;
using VisionFramework;

namespace VisionEditor;

public partial class MainWindow
{
    readonly FrameworkRunHistory frameworkRunHistory = new();

    void InitializeFrameworkResultHistory()
    {
        HistoryGrid.ItemsSource = frameworkRunHistory.Entries;
        doc.SolutionReplaced += ResetFrameworkRunHistory;
        ResetFrameworkRunHistory();
    }

    void RecordFrameworkRunHistory(string procedureId, ExecutionSessionResult session)
    {
        var name = doc.Document.Procedures.FirstOrDefault(procedure => procedure.Id == procedureId)?.Name ?? procedureId;
        frameworkRunHistory.Append(procedureId, name, session.Status,
            session.Nodes.Sum(record => record.ElapsedMilliseconds), DateTime.Now);
    }

    void ResetFrameworkRunHistory()
    {
        // Independent solution replacement policy for the host's own summary.
        // The original native/global module cache is not touched by this method.
        frameworkRunHistory.Clear();
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
