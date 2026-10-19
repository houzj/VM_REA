using VisionFramework;

namespace VisionEditor.Core;

public sealed partial class GraphEditorDocument
{
    /// <summary>Persists per-container view data without creating a structural history root.</summary>
    public void SetContainerZoomIndex(string procedureId, int index)
    {
        if (index is < 0 or > 19) throw new ArgumentOutOfRangeException(nameof(index));
        var procedure = Procedure(procedureId);
        if (procedure.CanvasZoomIndex == index) return;
        document = document with
        {
            Procedures = document.Procedures.Select(p => p.Id == procedureId
                ? p with { CanvasZoomIndex = index } : p).ToArray()
        };
        // UserData belongs to the current view, rather than a new structural edit.
        // Existing document snapshots carry it; redo and canvas gesture revisions survive.
        IsDirty = true;
        Changed?.Invoke();
    }
}
