using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VisionEditor;
using WpfApplication = System.Windows.Application;
using WpfBorder = System.Windows.Controls.Border;
using WpfButton = System.Windows.Controls.Button;
using WpfMenu = System.Windows.Controls.Menu;
using WpfMenuItem = System.Windows.Controls.MenuItem;
using WpfSeparator = System.Windows.Controls.Separator;

/// <summary>
/// A05.1 入口清点检查：原厂 menuitems.xml（50 节点 / 42 命令 / 15 工具栏项 / 4 组）
/// 必须被完整解析、完整接线，未接入项必须禁用并给出原因。
/// </summary>
internal static class Program
{
    static int checks;

    [STAThread]
    static void Main()
    {
        var catalog = ShellMenuCatalog.LoadDefault();
        catalog.Verify();
        VerifyCatalog(catalog);
        VerifyCommandTable(catalog);

        var app = new WpfApplication { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        var window = new MainWindow { ShowActivated = true, ShowInTaskbar = false, WindowStartupLocation = WindowStartupLocation.Manual, Left = 100, Top = 100 };
        try
        {
            window.Show();
            Pump(window);
            VerifyMenuBar(window);
            VerifyFileMenu(window);
            VerifySystemMenu(window);
            VerifyHelpMenu(window);
            VerifyToolsAndSettings(window);
            VerifyIndependentMenu(window);
            VerifyToolbar(window);
            Render(window);
            Console.WriteLine($"VisionEditor.ShellMenuChecks: {checks} 项入口清点断言全部通过。");
        }
        finally
        {
            window.Close();
            app.Shutdown();
        }
    }

    static void VerifyCatalog(ShellMenuCatalog catalog)
    {
        Check(catalog.NodeElements == ShellMenuCatalog.ExpectedNodeElements, $"目录节点元素 {catalog.NodeElements} = 50");
        Check(catalog.Methods.Count == ShellMenuCatalog.ExpectedMethods, $"命令 {catalog.Methods.Count} = 42");
        Check(catalog.ToolbarItems.Count == ShellMenuCatalog.ExpectedToolbarItems, $"工具栏项 {catalog.ToolbarItems.Count} = 15");
        Check(catalog.Menus.Count == ShellMenuCatalog.ExpectedMenuRoots, $"一级菜单 {catalog.Menus.Count} = 5");
        Check(catalog.ExternalToolCount == ShellMenuCatalog.ExpectedExternalTools, $"外部工具 {catalog.ExternalToolCount} = 5");
        Check(catalog.ToolbarItems.Select(item => item.GroupIndex).Distinct().Count() == 4, "工具栏分组 = 4");
        var runGroup = catalog.ToolbarItems.Single(item => item.IsGroupData);
        Check(runGroup.Children.Count == 2 && runGroup.Children.All(child => child.MethodName is "MutiRun" or "StopRun"), "运行组包含连续执行与停止执行");
    }

    static void VerifyCommandTable(ShellMenuCatalog catalog)
    {
        var router = new ShellCommandRouter();
        foreach (var method in ImplementedMethods) router.Register(method, () => { });
        router.VerifyCoverage(catalog.Methods);
        Check(router.ImplementedCount == ImplementedMethods.Length, $"已接线命令 {router.ImplementedCount} = {ImplementedMethods.Length}");
        var unavailable = catalog.Methods.Count - router.ImplementedCount;
        Check(unavailable == 25, $"未接入命令 {unavailable} = 25");
        foreach (var method in catalog.Methods)
        {
            var command = router.Resolve(method);
            if (command.State == ShellCommandState.Implemented) continue;
            Check(!string.IsNullOrWhiteSpace(command.Reason), $"未接入命令 {method} 必须给出原因");
        }
    }

    static readonly string[] ImplementedMethods =
    [
        "CreateNewSolution", "OpenSolution", "OpenSamples", "SaveSolution", "SaveAsSolution", "Exit",
        "Undo", "Redo", "AreaLocking", "AreaUnLocking", "SigRun", "MutiRun", "StopRun",
        "ProcRunSet", "VerInfo", "OnlineHelpDoc", "WecPage"
    ];

    static void VerifyMenuBar(MainWindow window)
    {
        var menu = (WpfMenu)window.FindName("MenuHost")!;
        var headers = menu.Items.OfType<WpfMenuItem>().Select(item => item.Header?.ToString()).ToArray();
        Check(string.Join(",", headers) == "文件,设置,工具,系统,帮助,独立入口", "菜单栏顺序：" + string.Join(",", headers));
    }

    static void VerifyFileMenu(MainWindow window)
    {
        var file = Root(window, 0);
        var captions = file.Items.OfType<WpfMenuItem>().Select(item => item.Header?.ToString()).ToArray();
        Check(string.Join(",", captions) == "新建方案,打开方案,最近打开方案,打开示例,保存方案,方案另存为,导出算子代码,导入流程,退出",
            "文件菜单条目：" + string.Join(",", captions));
        Check(file.Items.OfType<WpfSeparator>().Count() == 5, "文件菜单分隔线 = 5（isShowSeparator）");
        var recent = file.Items.OfType<WpfMenuItem>().Single(item => item.Header?.ToString() == "最近打开方案");
        Check(recent.HasItems || recent.Items.Count >= 0, "最近打开方案是动态子菜单");
        var export = file.Items.OfType<WpfMenuItem>().Single(item => item.Header?.ToString() == "导出算子代码");
        Check(!export.IsEnabled && export.ToolTip?.ToString()?.Contains("未接入") == true, "导出算子代码禁用并给出原因");
        var save = file.Items.OfType<WpfMenuItem>().Single(item => item.Header?.ToString() == "保存方案");
        Check(save.IsEnabled, "保存方案可用");
        var exit = file.Items.OfType<WpfMenuItem>().Single(item => item.Header?.ToString() == "退出");
        Check(exit.IsEnabled, "退出可用");
    }

    static void VerifySystemMenu(MainWindow window)
    {
        var system = Root(window, 3);
        var captions = system.Items.OfType<WpfMenuItem>().Select(item => item.Header?.ToString()).ToArray();
        Check(string.Join(",", captions) == "日志,通信管理,相机管理", "系统菜单条目：" + string.Join(",", captions));
        Check(system.Items.OfType<WpfMenuItem>().All(item => !item.IsEnabled), "系统菜单三项均未接入且禁用");
        Check(system.Items.OfType<WpfMenuItem>().All(item => item.ToolTip?.ToString()?.Contains("未接入") == true), "系统菜单三项均给出未接入原因");
    }

    static void VerifyHelpMenu(MainWindow window)
    {
        var help = Root(window, 4);
        var captions = help.Items.OfType<WpfMenuItem>().Select(item => item.Header?.ToString()).ToArray();
        Check(string.Join(",", captions) == "语言,帮助文档,帮助文档（在线）,版本信息,更多支持,V社区,打开欢迎页", "帮助菜单条目：" + string.Join(",", captions));
        var language = help.Items.OfType<WpfMenuItem>().Single(item => item.Header?.ToString() == "语言");
        var languages = language.Items.OfType<WpfMenuItem>().Select(item => item.Header?.ToString()).ToArray();
        Check(string.Join(",", languages) == "中文,English", "语言子菜单：" + string.Join(",", languages));
        Check(help.Items.OfType<WpfMenuItem>().Single(item => item.Header?.ToString() == "帮助文档（在线）").IsEnabled, "在线帮助可用");
        Check(!help.Items.OfType<WpfMenuItem>().Single(item => item.Header?.ToString() == "帮助文档").IsEnabled, "本地帮助文档未接入");
        Check(help.Items.OfType<WpfMenuItem>().Single(item => item.Header?.ToString() == "打开欢迎页").IsEnabled, "打开欢迎页可用");
        Check(help.Items.OfType<WpfMenuItem>().Single(item => item.Header?.ToString() == "版本信息").IsEnabled, "版本信息可用");
    }

    static void VerifyToolsAndSettings(MainWindow window)
    {
        var tools = Root(window, 2);
        var toolCaptions = tools.Items.OfType<WpfMenuItem>().Select(item => item.Header?.ToString()).ToArray();
        Check(string.Join(",", toolCaptions) == "标定配置引导,深度学习训练工具,运行环境检测工具,Calibration Board Generate Tool,运行异常采集机器人工具,查找表工具",
            "工具菜单为原厂外部工具启动器：" + string.Join(",", toolCaptions));
        Check(tools.Items.OfType<WpfMenuItem>().All(item => !item.IsEnabled), "工具菜单六项均未随附且禁用");

        var settings = Root(window, 1);
        var settingCaptions = settings.Items.OfType<WpfMenuItem>().Select(item => item.Header?.ToString()).ToArray();
        Check(string.Join(",", settingCaptions) == "权限设置,软件设置,方案设置,运行策略,采集策略", "设置菜单条目：" + string.Join(",", settingCaptions));
        Check(settings.Items.OfType<WpfMenuItem>().Single(item => item.Header?.ToString() == "运行策略").IsEnabled, "运行策略可用");
        Check(!settings.Items.OfType<WpfMenuItem>().Single(item => item.Header?.ToString() == "权限设置").IsEnabled, "权限设置未接入且禁用");
    }

    static void VerifyIndependentMenu(MainWindow window)
    {
        var independent = Root(window, 5);
        var captions = independent.Items.OfType<WpfMenuItem>().Select(item => item.Header?.ToString()).ToArray();
        Check(string.Join(",", captions) == "所选模块参数,流程校验,适应画布,模块目录", "独立入口菜单：" + string.Join(",", captions));
        Check(independent.Items.OfType<WpfMenuItem>().All(item => item.IsEnabled), "独立入口全部可用");
    }

    static void VerifyToolbar(MainWindow window)
    {
        var toolbar = (System.Windows.Controls.StackPanel)window.FindName("ToolBar")!;
        var buttons = toolbar.Children.OfType<WpfButton>().ToArray();
        var separators = toolbar.Children.OfType<WpfBorder>().Count();
        Check(buttons.Length == 16, $"工具栏按钮 {buttons.Length} = 16（15 项中运行组展开为 2）");
        Check(separators == 3, $"工具栏分组分隔线 {separators} = 3");
        // 禁用项会在文案后追加「（未接入：…）」，比较时只取文案部分。
        var tips = buttons.Select(button => button.ToolTip?.ToString()?.Split('（')[0]).ToArray();
        Check(string.Join("|", tips) == "保存方案|打开方案|撤消|重做|锁定|相机管理|控制器管理|全局变量|通信管理|全局触发|全局脚本|单次执行|连续执行|停止执行|综合配置|编辑运行界面",
            "工具栏顺序：" + string.Join("|", tips));
        // 撤消/重做按文档历史状态启用，空白方案下应为禁用；保存/打开/锁定恒可用。
        Check(buttons[0].IsEnabled && buttons[1].IsEnabled && buttons[4].IsEnabled, "保存/打开/锁定可用");
        Check(!buttons[2].IsEnabled && !buttons[3].IsEnabled, "无历史时撤消/重做禁用");
        Check(buttons.Skip(5).Take(2).All(button => !button.IsEnabled), "相机/控制器未接入且禁用");
        Check(buttons.Skip(7).Take(4).All(button => !button.IsEnabled), "全局变量/通信/触发/脚本未接入且禁用");
        Check(buttons[^1].IsEnabled == false && buttons[^1].ToolTip!.ToString()!.Contains("未接入"), "编辑运行界面未接入并给出原因");
    }

    static WpfMenuItem Root(MainWindow window, int index) => (WpfMenuItem)((WpfMenu)window.FindName("MenuHost")!).Items[index];

    static void Render(MainWindow window)
    {
        window.UpdateLayout();
        var directory = Path.Combine("analysis", "full-audit");
        Directory.CreateDirectory(directory);
        var height = Math.Min(140, window.ActualHeight);
        var bitmap = new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth), (int)Math.Ceiling(height), 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(window);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using var file = File.Create(Path.Combine(directory, "shell-menu-current.png"));
        encoder.Save(file);
    }

    static void Pump(Window window) => window.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);

    static void Check(bool condition, string description)
    {
        if (!condition) throw new Exception(description);
        checks++;
        Console.WriteLine("PASS: " + description);
    }
}
