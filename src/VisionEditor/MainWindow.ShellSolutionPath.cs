using System.Diagnostics;
using System.IO;
using System.Windows;

namespace VisionEditor;

public partial class MainWindow
{
    // External shell operation is injected so callers can verify the exact selected path.
    Action<string> revealSolutionFile = path => Process.Start(new ProcessStartInfo("explorer.exe", "/select,\"" + path + "\"") { UseShellExecute = true });

    void SolutionPathTooltipOpening(object sender, RoutedEventArgs e) => SolutionPathButton.ToolTip = "路径：" + (doc.FilePath ?? "");

    void RevealSolutionPath(object sender, RoutedEventArgs e)
    {
        var path = doc.FilePath;
        if (string.IsNullOrWhiteSpace(path)) return;
        var directory = Path.GetDirectoryName(path);
        if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
        {
            StatusText.Text = "方案路径不存在!";
            return;
        }
        if (!File.Exists(path))
        {
            StatusText.Text = "方案不存在!";
            return;
        }
        revealSolutionFile(path);
        StatusText.Text = "已定位方案：" + path;
    }
}
