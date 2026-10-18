using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
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
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, button));
            if (selected is not null) throw new Exception("未保存方案不应打开文件管理器");
            Directory.CreateDirectory(Path.GetDirectoryName(file)!);
            document.Save(file);
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
        }
        finally { if (File.Exists(file)) File.Delete(file); window.Close(); app.Shutdown(); }
    }
}
