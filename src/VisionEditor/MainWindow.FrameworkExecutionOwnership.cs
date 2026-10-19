using VisionEditor.Core;

namespace VisionEditor;

public partial class MainWindow
{
    readonly ExecutionOwnerEpoch executionOwnerEpoch = new();
    ExecutionOwnerStamp CaptureExecutionOwner() => executionOwnerEpoch.Capture(doc.SolutionId,executionScope);
    bool IsCurrentExecutionOwner(ExecutionOwnerStamp stamp) => !windowClosed &&
        executionOwnerEpoch.IsCurrent(stamp,doc.SolutionId,executionScope);
}
