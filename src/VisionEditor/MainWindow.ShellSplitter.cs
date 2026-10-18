using System.Windows.Controls.Primitives;

namespace VisionEditor;

public partial class MainWindow
{
    // MainViewControl.GridSplitter_DragStarted caps the right result column
    // so the workflow side retains the native 65-pixel workarea allowance.
    void WorkareaSplitterDragStarted(object sender, DragStartedEventArgs e)
    {
        RightWorkareaColumn.MaxWidth = Math.Max(
            RightWorkareaColumn.MinWidth, WorkareaGrid.ActualWidth - 65.0);
    }
}
