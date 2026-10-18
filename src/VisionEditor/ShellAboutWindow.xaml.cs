using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;

namespace VisionEditor;

public partial class ShellAboutWindow : Window
{
    public ShellAboutWindow()
    {
        InitializeComponent();
        var assembly = typeof(MainWindow).Assembly;
        VersionText.Text = assembly.GetName().Version?.ToString() ?? "未知";
        var logo = System.IO.Path.Combine(AppContext.BaseDirectory, "ui-assets", "logo_vm.png");
        if (System.IO.File.Exists(logo)) AboutLogo.Source = new BitmapImage(new Uri(logo));
    }
}
