namespace VisionEditor.Core;

public sealed partial class GraphEditorDocument
{
    sealed class ConfigurationHistory(string solutionId)
    {
        public string SolutionId { get; } = solutionId;
        public bool NeedsRoot { get; set; } = true;
        public bool ForbidBuild { get; set; } = true;
    }
    ConfigurationHistory? configurationHistory;

    /// <summary>
    /// Runs immediate configuration writes under one document history root.
    /// The root is created by the first successful edit, so opening an unchanged
    /// configuration window does not create an undo entry. Nested calls share
    /// the root's boolean: an inner completion enables recording again.
    /// </summary>
    /// <remarks>
    /// Snapshot grouping is the local adapter for the original root/part model.
    /// Finally cleanup is deliberate local hardening; the original Group dialog
    /// has a paired true/false call without an exception cleanup region.
    /// </remarks>
    public void ExecuteConfigurationEdit(Action<GraphEditorDocument> change)
    {
        ArgumentNullException.ThrowIfNull(change);
        var previous = configurationHistory;
        var session = previous is not null && previous.SolutionId == SolutionId
            ? previous : new ConfigurationHistory(SolutionId);
        configurationHistory = session;
        session.ForbidBuild = true;
        if (!session.NeedsRoot) SetHistoryRecordingForbidden(true);
        try { change(this); }
        finally
        {
            session.ForbidBuild = false;
            if (session.SolutionId == SolutionId && !session.NeedsRoot)
                SetHistoryRecordingForbidden(false);
            configurationHistory = previous;
        }
    }

    /// <summary>Sets the current history root's flag; an empty undo stack is unaffected.</summary>
    public void SetHistoryRecordingForbidden(bool forbidden)
    {
        if (undo.TryPeek(out var root)) root.ForbidBuild = forbidden;
    }

    void RecordEditHistory()
    {
        if (configurationHistory is { NeedsRoot: true } session && session.SolutionId == SolutionId)
        {
            undo.Push(Capture());
            session.NeedsRoot = false;
            SetHistoryRecordingForbidden(session.ForbidBuild);
        }
        else if (!undo.TryPeek(out var root) || !root.ForbidBuild)
            undo.Push(Capture());
    }
}
