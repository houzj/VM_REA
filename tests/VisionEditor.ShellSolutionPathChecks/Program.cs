using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Globalization;
using VisionEditor;
using VisionEditor.Core;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        var window = new MainWindow { ShowInTaskbar = false, ShowActivated = false, Left = -20000, Top = -20000 };
        var file = Path.GetFullPath("analysis/full-audit/mf01-solution-path.vmx.json");
        var longFile = Path.GetFullPath("analysis/full-audit/" + new string('长', 30) + ".vmx.json");
        try
        {
            window.Show(); window.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
            var document = (GraphEditorDocument)typeof(MainWindow).GetField("doc", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(window)!;
            var button = (Button)window.FindName("SolutionPathButton")!;
            var status = (TextBlock)window.FindName("StatusText")!;
            string? selected = null;
            typeof(MainWindow).GetField("revealSolutionFile", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(window, (Action<string>)(path => selected = path));
            document.NewSolution(false);
            var title = (TextBlock)window.FindName("WindowProjectName")!;
            if (title.Text != string.Empty) throw new Exception("未保存方案的标题不应显示示例文件名");
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, button));
            if (selected is not null) throw new Exception("未保存方案不应打开文件管理器");
            Directory.CreateDirectory(Path.GetDirectoryName(file)!);
            document.Save(file);
            if (title.Text != "mf01-solution-path") throw new Exception("保存后标题没有更新为实际方案文件名");
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, button));
            if (selected != file || !status.Text.Contains(file, StringComparison.Ordinal))
                throw new Exception("标题点击未将真实方案文件交给文件管理器入口");
            Console.WriteLine("PASS: 标题方案名点击后定位已保存的实际方案文件");
            selected = null;
            File.Delete(file);
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, button));
            if (selected is not null || status.Text != "方案不存在!")
                throw new Exception("方案文件丢失时仍启动了文件管理器");
            Console.WriteLine("PASS: 丢失的方案文件阻止定位并报告明确错误");
            document.Save(longFile);
            if (!title.Text.EndsWith("...", StringComparison.Ordinal) || title.Text.Contains(".vmx", StringComparison.Ordinal))
                throw new Exception("长方案名未按原版像素宽度截短");
            var measured = new FormattedText(title.Text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
                new Typeface("Microsoft YaHei"), 13, Brushes.Black, 1).WidthIncludingTrailingWhitespace;
            if (measured > 128) throw new Exception("方案名仍超出原版 128 像素上限");
            window.UpdateLayout();
            var header = (Grid)window.FindName("ShellHeader")!;
            var titleLeft = title.TranslatePoint(new Point(0, 0), header).X;
            if (title.ActualWidth <= 0 || titleLeft < 184 || titleLeft + title.ActualWidth > header.ActualWidth)
                throw new Exception("标题栏右列裁掉了方案名或挤出窗口边界");
            var capture = new RenderTargetBitmap((int)Math.Ceiling(header.ActualWidth), (int)Math.Ceiling(header.ActualHeight), 96, 96, PixelFormats.Pbgra32);
            capture.Render(header);
            var png = new PngBitmapEncoder(); png.Frames.Add(BitmapFrame.Create(capture));
            using (var output = File.Create("analysis/full-audit/shell-long-title.png")) png.Save(output);
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, button));
            if (selected != longFile) throw new Exception("截短标题点击后丢失真实方案路径");
            File.Delete(longFile);
            Console.WriteLine("PASS: 长方案名截短显示并保留完整点击路径");
        }
        finally { if (File.Exists(file)) File.Delete(file); if (File.Exists(longFile)) File.Delete(longFile); window.Close(); app.Shutdown(); }
    }
}
