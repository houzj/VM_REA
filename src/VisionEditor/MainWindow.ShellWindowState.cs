using System.Windows;

namespace VisionEditor;

public partial class MainWindow
{
    void MaximizeButtonLoaded(object sender, RoutedEventArgs e) => UpdateMaximizeButton();
    void ShellWindowStateChanged(object? sender, EventArgs e) => UpdateMaximizeButton();

    void UpdateMaximizeButton()
    {
        if (MaximizeButton is null || MaximizeIcon is null) return;
        var restored = WindowState == WindowState.Maximized;
        MaximizeButton.ToolTip = restored ? "向下还原" : "最大化";
        MaximizeIcon.Source = Asset(restored ? "windowshift_ic_normal.png" : "windowshift_ic_maximize.png");
    }
}
