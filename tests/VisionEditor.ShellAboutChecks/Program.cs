using System.Reflection;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VisionEditor;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        var main = new MainWindow { ShowInTaskbar = false, ShowActivated = false, Left = -20000, Top = -20000 };
        try
        {
            main.Show();
            main.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
            var menu = (StackPanel)main.FindName("MenuHost")!;
            var help = menu.Children.OfType<Button>().Single(b => Equals(b.Content, "帮助"));
            var about = help.ContextMenu.Items.OfType<MenuItem>().Single(i => Equals(i.Header, "关于"));
            var examined = false;
            main.Dispatcher.BeginInvoke(new Action(() =>
            {
                var dialog = main.OwnedWindows.OfType<ShellAboutWindow>().Single();
                var version = (TextBlock)dialog.FindName("VersionText")!;
                var actual = typeof(MainWindow).Assembly.GetName().Version?.ToString() ?? "未知";
                if (dialog.Owner != main || !dialog.IsVisible || dialog.Width != 400 || dialog.Height != 440 || version.Text != actual)
                    throw new Exception("关于窗口未展示真实程序集版本或窗口属性不符");
                dialog.UpdateLayout();
                var image = new RenderTargetBitmap((int)dialog.ActualWidth, (int)dialog.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                image.Render(dialog);
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                Directory.CreateDirectory("analysis/full-audit");
                using (var output = File.Create("analysis/full-audit/mf01-about-window.png")) encoder.Save(output);
                examined = true;
                dialog.Close();
            }), DispatcherPriority.Background);
            about.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent, about));
            if (!examined || main.OwnedWindows.OfType<ShellAboutWindow>().Any()) throw new Exception("关于窗口没有完成模态打开/关闭");
            Console.WriteLine("PASS: 帮助菜单打开关于窗口并显示当前程序真实版本，关闭后返回主窗口");
        }
        finally { main.Close(); app.Shutdown(); }
    }
}
