using System.Windows.Controls;
using System.Windows.Input;

namespace VisionEditor;

public partial class MainWindow
{
    void ToolBarPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (sender is not ScrollViewer scroll) return;
        var amount = Math.Abs(e.Delta / 2.0);
        var target = e.Delta <= 0
            ? Math.Min(scroll.ScrollableWidth, scroll.HorizontalOffset + amount)
            : Math.Max(0, scroll.HorizontalOffset - amount);
        if (target == scroll.HorizontalOffset) return;
        scroll.ScrollToHorizontalOffset(target);
        e.Handled = true;
    }
}
