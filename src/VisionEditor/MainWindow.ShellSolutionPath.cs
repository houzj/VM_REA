using System.Diagnostics;
using System.IO;
using System.Windows;

namespace VisionEditor;

public partial class MainWindow
{
    bool shellSolutionNameSubscribed;
    // External shell operation is injected so callers can verify the exact selected path.
    Action<string> revealSolutionFile = path => Process.Start(new ProcessStartInfo("explorer.exe", "/select,\"" + path + "\"") { UseShellExecute = true });

    void SolutionPathButtonLoaded(object sender, RoutedEventArgs e)
    {
        if (!shellSolutionNameSubscribed)
        {
            shellSolutionNameSubscribed = true;
            doc.Changed += UpdateShellSolutionName;
            doc.SolutionReplaced += UpdateShellSolutionName;
        }
        UpdateShellSolutionName();
    }

    void UpdateShellSolutionName()
    {
        if (doc.FilePath is not { Length: > 0 } path) { WindowProjectName.Text = string.Empty; return; }
        var name = Path.GetFileName(path);
        WindowProjectName.Text = name.EndsWith(".vmx.json", StringComparison.OrdinalIgnoreCase)
            ? name[..^".vmx.json".Length]
            : Path.GetFileNameWithoutExtension(name);
    }

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
