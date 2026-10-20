using VisionEditor.Core;
using VisionFramework;

namespace VisionEditor;

public partial class MainWindow
{
    readonly ExecutionOwnerEpoch executionOwnerEpoch = new();
    FrameworkCallbackQueue<Action>? workflowCallbacks;
    FrameworkCallbackQueue<Action> WorkflowCallbacks => workflowCallbacks ??= new FrameworkCallbackQueue<Action>(DispatchWorkflowCallback);
    ExecutionOwnerStamp CaptureExecutionOwner() => executionOwnerEpoch.Capture(doc.SolutionId,executionScope);
    bool IsCurrentExecutionOwner(ExecutionOwnerStamp stamp) => !windowClosed &&
        executionOwnerEpoch.IsCurrent(stamp,doc.SolutionId,executionScope);
    void DispatchWorkflowCallback(Action callback)
    {
        if(windowClosed)return;
        if(Dispatcher.CheckAccess())callback();
        else _=Dispatcher.BeginInvoke(callback);
    }
}
