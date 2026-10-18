using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Globalization;

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
        var fullName = name.EndsWith(".vmx.json", StringComparison.OrdinalIgnoreCase)
            ? name[..^".vmx.json".Length]
            : Path.GetFileNameWithoutExtension(name);
        WindowProjectName.Text = CompactShellSolutionName(fullName);
    }

    internal static string CompactShellSolutionName(string name)
    {
        const double maximumWidth = 128;
        var font = new Typeface(new System.Windows.Media.FontFamily("Microsoft YaHei"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        double Width(string text) => new FormattedText(text, CultureInfo.InvariantCulture,
            System.Windows.FlowDirection.LeftToRight, font, 13, System.Windows.Media.Brushes.Black, 1).WidthIncludingTrailingWhitespace;
        if (Width(name) <= maximumWidth) return name;
        for (var length = name.Length - 1; length >= 0; length--)
        {
            var candidate = name[..length] + "...";
            if (Width(candidate) <= maximumWidth) return candidate;
        }
        return "...";
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
