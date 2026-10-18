using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;

namespace VisionEditor;

public partial class ShellAboutWindow : Window
{
    public sealed record VersionRow(string Title, string Value);

    public ShellAboutWindow()
    {
        InitializeComponent();
        VersionInfoList.ItemsSource = new[]
        {
            new VersionRow("版本信息", VersionOf(typeof(MainWindow).Assembly)),
            new VersionRow("框架版本", VersionOf(typeof(VisionFramework.ModuleRegistry).Assembly)),
            new VersionRow("编辑器核心版本", VersionOf(typeof(VisionEditor.Core.GraphEditorDocument).Assembly))
        };
        var logo = System.IO.Path.Combine(AppContext.BaseDirectory, "ui-assets", "logo_vm.png");
        if (System.IO.File.Exists(logo)) AboutLogo.Source = new BitmapImage(new Uri(logo));
    }

    static string VersionOf(Assembly assembly) => assembly.GetName().Version?.ToString() ?? "未知";
}
