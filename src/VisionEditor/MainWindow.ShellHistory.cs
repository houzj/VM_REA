namespace VisionEditor;

public partial class MainWindow
{
    // The original menu command rejects undo/redo while a module editor is open.
    // Keep both the menu and toolbar on the same guarded path.
    void UndoShell() => ChangeHistory(redo: false);
    void RedoShell() => ChangeHistory(redo: true);

    void ChangeHistory(bool redo)
    {
        if (parameterEditorOpen)
        {
            StatusText.Text = "打开模块配置时，不能执行撤消重做";
            return;
        }
        if (windowClosed || isLocked || activeRun is not null) return;
        if (redo) doc.Redo(); else doc.Undo();
    }
}
