using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Button = System.Windows.Controls.Button;
using Image = System.Windows.Controls.Image;
using MenuItem = System.Windows.Controls.MenuItem;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;

namespace VisionEditor;

/// <summary>
/// 由原厂 menuitems.xml 驱动的菜单栏与工具栏构建（对应框架单元 A05.1）。
/// 数据：Resources/shell-menuitems.xml（原厂原件逐字节副本）。
/// 文案：Styles/ShellMenuStrings.xaml（逐键取自原厂 Lang/zh-cn.xaml）。
/// 命令：ShellCommandRouter（42 个 methodName，未接入项禁用并给原因）。
/// </summary>
public partial class MainWindow
{
    ShellMenuCatalog? shellMenuCatalog;
    ShellCommandRouter? shellCommandRouter;

    /// <summary>测试用：已加载的原厂菜单目录。</summary>
    internal ShellMenuCatalog? ShellCatalog => shellMenuCatalog;

    /// <summary>测试用：已接线的命令路由。</summary>
    internal ShellCommandRouter? ShellRouter => shellCommandRouter;

    void BuildShellChrome()
    {
        var catalog = ShellMenuCatalog.LoadDefault();
        catalog.Verify();
        var router = new ShellCommandRouter();
        RegisterShellCommands(router);
        router.VerifyCoverage(catalog.Methods);
        shellMenuCatalog = catalog;
        shellCommandRouter = router;
        BuildShellMenuBar(catalog, router);
        BuildShellToolBar(catalog, router);
    }

    /// <summary>把原厂 42 个命令中本项目已有实现的 18 个接到主窗口动作上。</summary>
    void RegisterShellCommands(ShellCommandRouter router)
    {
        router.Register("CreateNewSolution", New);
        router.Register("OpenSolution", Open);
        router.Register("OpenSamples", Demo);
        router.Register("SaveSolution", () => Save(false));
        router.Register("SaveAsSolution", () => Save(true));
        router.Register("Exit", Close);
        router.Register("Undo", UndoShell);
        router.Register("Redo", RedoShell);
        // 原厂 AreaLocking / AreaUnLocking 是同一把工作区锁的两向命令。
        router.Register("AreaLocking", RequestWorkAreaLock);
        router.Register("AreaUnLocking", RequestWorkAreaLock);
        router.Register("SigRun", () => { _ = Run(); });
        router.Register("MutiRun", StartContinuous);
        router.Register("StopRun", () => activeRun?.Cancel());
        router.Register("ProcRunSet", OpenRunModeSettings);
        router.Register("VerInfo", ShowAbout);
        router.Register("OnlineHelpDoc", OpenOnlineHelpDocument);
        router.Register("WecPage", ShowStartupGuide);
    }

    void ShowStartupGuide()
    {
        var guide = new StartupGuideWindow { Owner = this };
        guide.ShowDialog();
    }

    void BuildShellMenuBar(ShellMenuCatalog catalog, ShellCommandRouter router)
    {
        MenuHost.Items.Clear();
        foreach (var menu in catalog.Menus)
        {
            var root = CreateMenuRoot(MenuText(menu));
            foreach (var child in menu.Children) AddMenuChild(root, child, router);
            MenuHost.Items.Add(root);
        }
        MenuHost.Items.Add(CreateIndependentEntryMenu());
    }

    MenuItem CreateMenuRoot(string caption)
    {
        var root = new MenuItem
        {
            Header = caption,
            Foreground = new SolidColorBrush(Color.FromRgb(192, 192, 192)),
            Padding = new Thickness(10, 4, 10, 4)
        };
        root.SubmenuOpened += (_, _) => SetShellMenuFocus(root);
        return root;
    }

    void AddMenuChild(MenuItem parent, ShellMenuNode node, ShellCommandRouter router)
    {
        // 原厂 isDisplayInMenu=false 的项只出现在工具栏（如「控制器管理」），不进菜单。
        if (!node.IsDisplayInMenu) return;
        if (string.Equals(node.DynamicItems, "GetRecentlySolutions", StringComparison.Ordinal))
        {
            parent.Items.Add(CreateRecentSolutionsMenu());
            return;
        }
        if (node.Children.Count > 0)
        {
            var submenu = new MenuItem { Header = MenuText(node) };
            foreach (var child in node.Children) AddMenuChild(submenu, child, router);
            parent.Items.Add(submenu);
            return;
        }
        parent.Items.Add(CreateMenuLeaf(node, router));
        if (node.IsShowSeparator) parent.Items.Add(new Separator());
    }

    MenuItem CreateMenuLeaf(ShellMenuNode node, ShellCommandRouter router)
    {
        var caption = MenuText(node);
        var item = new MenuItem { Header = caption };
        if (node.IsExternalTool)
        {
            item.IsEnabled = false;
            item.Foreground = Brushes.Gray;
            item.ToolTip = $"未接入：外部工具未随附（{node.Info}）";
            return item;
        }
        var command = router.Resolve(node.MethodName);
        var implemented = command.State == ShellCommandState.Implemented;
        item.IsEnabled = implemented;
        if (command.Execute is not null) item.Click += (_, _) => Safe(command.Execute);
        if (!implemented)
        {
            item.Foreground = Brushes.Gray;
            item.ToolTip = command.Reason ?? "未接入";
            // WPF 默认不给禁用元素显示 ToolTip，这里显式打开，保证"为什么点不了"可读。
            ToolTipService.SetShowOnDisabled(item, true);
        }
        return item;
    }

    /// <summary>原厂「最近打开方案」的动态子项（GetRecentlySolutions）。</summary>
    MenuItem CreateRecentSolutionsMenu()
    {
        recentSolutionsMenu = new MenuItem { Header = TryFindResource("Lang_OpenLastSol") as string ?? "最近打开方案" };
        recentSolutionsMenu.SubmenuOpened += (_, _) => RefreshRecentSolutionsMenu();
        RefreshRecentSolutionsMenu();
        return recentSolutionsMenu;
    }

    /// <summary>本批新增：承载项目自有、原厂无对应的入口，避免污染原厂五个菜单。</summary>
    MenuItem CreateIndependentEntryMenu()
    {
        var root = CreateMenuRoot("独立入口");
        root.ToolTip = "本项目自有能力，原厂菜单无对应项";
        root.Items.Add(CreateIndependentLeaf("所选模块参数", () => OpenSelected()));
        root.Items.Add(CreateIndependentLeaf("流程校验", () => Validate()));
        root.Items.Add(CreateIndependentLeaf("适应画布", () => Fit()));
        root.Items.Add(CreateIndependentLeaf("模块目录", () => ShowToolPicker("all")));
        return root;
    }

    MenuItem CreateIndependentLeaf(string caption, Action action)
    {
        var item = new MenuItem { Header = caption };
        item.Click += (_, _) => Safe(action);
        return item;
    }

    void BuildShellToolBar(ShellMenuCatalog catalog, ShellCommandRouter router)
    {
        ToolBar.Children.Clear();
        var currentGroup = int.MinValue;
        foreach (var item in catalog.ToolbarItems)
        {
            if (currentGroup != int.MinValue && item.GroupIndex != currentGroup) Sep();
            currentGroup = item.GroupIndex;
            if (item.IsGroupData)
            {
                foreach (var child in item.Children) ToolBar.Children.Add(CreateToolbarButton(child, router));
                continue;
            }
            ToolBar.Children.Add(CreateToolbarButton(item, router));
        }
    }

    Button CreateToolbarButton(ShellMenuNode node, ShellCommandRouter router)
    {
        var caption = MenuText(node);
        var command = router.Resolve(node.MethodName);
        var implemented = command.State == ShellCommandState.Implemented;
        var enabledAsset = ToolbarAsset(node.ImageEnable);
        var disabledAsset = ToolbarAsset(node.ImageDisable);

        var button = new Button
        {
            Width = 44,
            Height = 42,
            Margin = new Thickness(0),
            Padding = new Thickness(8),
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            IsEnabled = implemented
        };
        button.Template = PlainTemplate();
        if (enabledAsset is not null) button.Content = new Image { Source = Asset(enabledAsset), Width = 26, Height = 26, Stretch = Stretch.Uniform };
        button.ToolTip = implemented ? caption : $"{caption}（{command.Reason ?? "未接入"}）";
        if (!implemented)
        {
            button.Opacity = 0.45;
            ToolTipService.SetShowOnDisabled(button, true);
        }
        if (command.Execute is not null) button.Click += (_, _) => Safe(command.Execute);
        button.Click += (_, _) => SetShellMenuFocus(button);

        switch (node.MethodName)
        {
            case "Undo":
                undoToolButton = button;
                if (enabledAsset is not null && disabledAsset is not null) SetToolbarStateIcon(button, enabledAsset, disabledAsset);
                break;
            case "Redo":
                redoToolButton = button;
                if (enabledAsset is not null && disabledAsset is not null) SetToolbarStateIcon(button, enabledAsset, disabledAsset);
                break;
            case "AreaLocking":
                lockToolButton = button;
                break;
            case "MutiRun":
                continuousToolButton = button;
                break;
            case "StopRun":
                stopToolButton = button;
                break;
        }
        return button;
    }

    /// <summary>
    /// 原厂图标路径 <c>Resources\ToolBar\x.png</c> 对应本仓库已提取的 <c>toolbar_x.png</c>
    /// （analysis/ui-assets）。缺失时回退原名，仍缺失则返回 null 由调用方降级。
    /// </summary>
    string? ToolbarAsset(string? originalPath)
    {
        if (string.IsNullOrWhiteSpace(originalPath)) return null;
        var file = Path.GetFileName(originalPath.Replace('\\', '/'));
        if (string.IsNullOrWhiteSpace(file)) return null;
        foreach (var candidate in new[] { "toolbar_" + file, file })
            if (AssetExists(candidate)) return candidate;
        return null;
    }

    static bool AssetExists(string name) =>
        File.Exists(Path.Combine(AppContext.BaseDirectory, "ui-assets", name)) ||
        File.Exists(Path.Combine(AppContext.BaseDirectory, "ui-assets", "wpf", name));

    string MenuText(ShellMenuNode node) => TryFindResource(node.CaptionKey) as string ?? node.CaptionKey;
}
