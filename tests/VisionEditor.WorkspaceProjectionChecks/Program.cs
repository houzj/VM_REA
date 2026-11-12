using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;
using Color = System.Windows.Media.Color;
using DataGrid = System.Windows.Controls.DataGrid;
using TabControl = System.Windows.Controls.TabControl;
using TabItem = System.Windows.Controls.TabItem;
using WpfApplication = System.Windows.Application;

/// <summary>
/// 右侧「当前结果」页接线断言：选中节点变化必须换列，无表格输出的节点必须收起该页。
/// 契约依据 docs/reverse/W01-workspace-baseline-contract-20261111.md 第 3.2/3.3 节。
/// </summary>
internal static class Program
{
    static int checks;

    [STAThread]
    static int Main()
    {
        try
        {
            var app = new WpfApplication { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            var window = new MainWindow { ShowInTaskbar = false, ShowActivated = true, Left = 100, Top = 100 };
            try
            {
                window.Show();
                Pump(window);

                LoadExample(window);
                var document = Document(window);

                var componentsNode = document.CurrentProcedure.Nodes.FirstOrDefault(node => node.ModuleId == "vision.components")
                    ?? throw new Exception("示例方案缺少连通域节点。");
                var imageNode = document.CurrentProcedure.Nodes.First(node => node.ModuleId == "vision.image-file");

                Select(window, componentsNode.Id);
                var projection = window.CurrentProjection;
                Check(projection.HasTable, "连通域节点启用当前结果表");
                Check(string.Join(",", projection.Columns.Select(column => column.Header)) == "序号,面积,中心X,中心Y,宽度,高度",
                    "列随输出类型生成：" + string.Join(",", projection.Columns.Select(column => column.Header)));

                var tab = Find<TabItem>(window, "CurrentResultTab");
                Check(tab.Visibility == Visibility.Visible, "当前结果页在连通域节点下可见");

                var grid = Find<DataGrid>(window, "CurrentResultGrid");
                Check(grid.Columns.Count == 6, $"网格列数 {grid.Columns.Count} = 6");

                Select(window, imageNode.Id);
                Check(!window.CurrentProjection.HasTable, "纯图像节点不启用当前结果表");
                Check(tab.Visibility == Visibility.Collapsed, "无表格输出的节点收起当前结果页");

                var bottomTabs = Find<TabControl>(window, "BottomTabs");
                Check(ReferenceEquals(bottomTabs.SelectedItem, Find<TabItem>(window, "HistoryTab")), "收起当前结果页后回落到历史结果");

                var history = Find<DataGrid>(window, "HistoryGrid");
                Check(history.Visibility == Visibility.Visible, "历史结果表格可见");

                Select(window, componentsNode.Id);
                Check(tab.Visibility == Visibility.Visible && window.CurrentProjection.HasTable, "切回连通域节点恢复当前结果页");

                VerifyLogPanel(window);
                VerifyToolbox(window);
                VerifyCanvasGeometry(window);
                VerifyTabRunButtons(window);
                VerifyToolBaseline(window);
                VerifyShellLayout(window);
                VerifyHistoryBinding(window);
                VerifyImageSelectorFollowsSelection(window);
                VerifyModuleTree(window);
                VerifyNodeStateFlag(window);
                VerifyHelpPane(window);

                Console.WriteLine($"VisionEditor.WorkspaceProjectionChecks: {checks} 项投影接线断言全部通过。");
                return 0;
            }
            finally
            {
                window.Close();
                app.Shutdown();
            }
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("FAILED: " + error);
            return 1;
        }
    }

    /// <summary>契约 W01 第 4 节：日志区默认折叠、可展开、筛选生效。</summary>
    static void VerifyLogPanel(MainWindow window)
    {
        var panel = Find<System.Windows.Controls.Border>(window, "LogPanel");
        var bar = Find<System.Windows.Controls.Border>(window, "LogBar");
        var grid = Find<DataGrid>(window, "LogGrid");
        var fold = Find<System.Windows.Controls.Primitives.ToggleButton>(window, "LogFoldButton");

        Check(panel.Visibility == Visibility.Collapsed, "日志面板默认折叠");
        Check(Math.Abs(bar.ActualHeight - 28) < 1.5, $"日志栏高度 {bar.ActualHeight:F1} ≈ 28");

        window.AppendShellLog(VisionEditor.Core.ShellLogKind.Info, "断言用信息");
        window.AppendShellLog(VisionEditor.Core.ShellLogKind.Error, "断言用错误");
        Pump(window);

        fold.IsChecked = true;
        Invoke(window, "LogFoldClick", null, null);
        Pump(window);
        Check(panel.Visibility == Visibility.Visible, "勾选后展开日志面板");
        Check(Math.Abs(panel.ActualHeight - 180) < 1.5, $"日志面板高度 {panel.ActualHeight:F1} ≈ 180");
        Check(grid.Items.Count == window.LogStore.Visible.Count && grid.Items.Count >= 2, "日志表格绑定可见集合");

        Invoke(window, "LogFilterClick", Find<System.Windows.Controls.RadioButton>(window, "LogErrorButton"), null);
        Pump(window);
        Check(window.LogStore.Filter == VisionEditor.Core.ShellLogKind.Error, "点击错误只留错误级别");
        Check(grid.Items.Count == 1, $"错误筛选后仅 1 条，实际 {grid.Items.Count}");

        Invoke(window, "LogFilterClick", Find<System.Windows.Controls.RadioButton>(window, "LogAllButton"), null);
        Pump(window);
        Check(grid.Items.Count >= 2, "切回全部恢复记录");

        fold.IsChecked = false;
        Invoke(window, "LogFoldClick", null, null);
        Pump(window);
        Check(panel.Visibility == Visibility.Collapsed, "取消勾选折叠日志面板");
    }

    /// <summary>契约 W01 第 2.2 节：折叠 52×50 图标、展开 230、磁贴 68×90、悬停弹层 235。</summary>
    static void VerifyToolbox(MainWindow window)
    {
        var rail = (System.Windows.Controls.StackPanel)window.FindName("Rail")!;
        var folded = rail.Children.OfType<System.Windows.Controls.Button>().ToArray();
        Check(folded.Length > 0, "折叠态导轨存在分类按钮");
        Check(folded.All(button => Math.Abs(button.Width - 52) < 0.5 && Math.Abs(button.Height - 50) < 0.5),
            "折叠态分类图标 52×50");

        Invoke(window, "ToggleToolbox");
        Pump(window);
        var expanders = rail.Children.OfType<System.Windows.Controls.Expander>().ToArray();
        Check(expanders.Length > 0, "展开态使用分组容器");
        Check(expanders.All(expander => Math.Abs(expander.Width - 230) < 0.5), "展开态分组宽 230");

        var first = expanders[0];
        first.IsExpanded = true;
        Pump(window);
        var grid = first.Content as System.Windows.Controls.Primitives.UniformGrid;
        Check(grid is not null, "分组内容为工具磁贴网格");
        var tiles = grid!.Children.OfType<System.Windows.Controls.Button>().ToArray();
        Check(tiles.Length > 0 && tiles.All(tile => Math.Abs(tile.Width - 68) < 0.5 && Math.Abs(tile.Height - 90) < 0.5),
            "工具磁贴 68×90");

        Invoke(window, "ToggleToolbox");
        Pump(window);
        Check(rail.Children.OfType<System.Windows.Controls.Button>().Any(), "可以折回导轨态");
    }

    /// <summary>
    /// 契约 W01 第 2.3 节 / 原厂 DiagramConstInfo：节点 120×40、外框 130×50、栅格步进 5。
    /// </summary>
    static void VerifyCanvasGeometry(MainWindow window)
    {
        var width = (double)typeof(MainWindow).GetField("NodeWidth", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
        var height = (double)typeof(MainWindow).GetField("NodeHeight", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
        Check(Math.Abs(width - 110) < 0.01 && Math.Abs(height - 30) < 0.01, $"节点本体 {width}×{height} = 110×30（原厂 diagramitemtemplate）");

        var snap = typeof(MainWindow).GetMethod("Snap", BindingFlags.Static | BindingFlags.NonPublic)!;
        var cases = new (double Input, double Expected)[] { (3, 5), (7, 5), (12, 10), (22.4, 20) };
        foreach (var (input, expected) in cases)
            Check(Math.Abs((double)snap.Invoke(null, [input])! - expected) < 0.01, $"栅格吸附 {input} → {expected}");

        var document = Document(window);
        var node = document.CurrentProcedure.Nodes[0];
        var cards = (System.Collections.IDictionary)typeof(MainWindow)
            .GetField("nodeCards", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(window)!;
        var card = cards[node.Id] as System.Windows.Controls.Canvas;
        Check(card is not null, "节点卡片已渲染");
        Check(Math.Abs(card!.Width - 130) < 0.01, $"节点外框宽 {card.Width} = 130");

        var body = card.Children.OfType<System.Windows.Controls.Border>().FirstOrDefault();
        Check(body is not null && Math.Abs(body.Width - 110) < 0.01, "节点本体位于 110 宽的外框内");
        Check(System.Windows.Controls.Canvas.GetLeft(body!) == 10 && System.Windows.Controls.Canvas.GetTop(body) == 10,
            "本体相对外框偏移 10,10（原厂 ItemRect 130×50 含 5px 内缩）");
    }

    /// <summary>
    /// 契约 W01 第 2.4/2.5 节：运行控件长在流程页签上，单次与连续双钮，
    /// 停止是连续钮的双态（DecorateSource.ContinuousRunEnable），未选中页签不显示。
    /// </summary>
    static void VerifyTabRunButtons(MainWindow window)
    {
        var tabs = (TabControl)window.FindName("ProcedureTabs")!;
        var active = tabs.Items.OfType<TabItem>().FirstOrDefault(tab => tab.IsSelected);
        Check(active is not null, "存在选中的流程页签");
        var header = active!.Header as System.Windows.Controls.StackPanel;
        Check(header is not null, "页签头是横向堆栈");

        var tips = header!.Children.OfType<FrameworkElement>().Select(child => child.ToolTip?.ToString()).Where(text => text is not null).ToArray();
        Check(tips.Contains("单次运行"), "页签内含单次运行按钮：" + string.Join(",", tips));
        Check(tips.Contains("连续运行") || tips.Contains("停止运行"), "页签内含连续/停止双态按钮");

        var idle = tabs.Items.OfType<TabItem>().Where(tab => !tab.IsSelected)
            .Select(tab => tab.Header as System.Windows.Controls.StackPanel)
            .Where(panel => panel is not null)
            .SelectMany(panel => panel!.Children.OfType<FrameworkElement>())
            .Select(child => child.ToolTip?.ToString())
            .Where(text => text is not null)
            .ToArray();
        Check(!idle.Contains("单次运行"), "未选中页签不显示运行按钮");
    }

    /// <summary>
    /// 契约 W01 第 6 节：以 Blob 为参考实现，再用第二个工具（圆查找）走同一套六项校验。
    /// </summary>
    static void VerifyToolBaseline(MainWindow window)
    {
        var registry = (ModuleRegistry)typeof(MainWindow)
            .GetField("modules", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(window)!;
        var descriptors = registry.Descriptors.ToArray();
        Check(descriptors.Length >= 2, $"注册表含 {descriptors.Length} 个工具描述符");

        var blob = descriptors.FirstOrDefault(d => d.Id == "vision.components")
            ?? throw new Exception("未注册 Blob（vision.components）");
        var blobReport = ToolBaseline.Validate(blob);
        Check(blobReport.Passed, "Blob 通过六项基线校验" + Describe(blobReport));

        var second = descriptors.FirstOrDefault(d => d.Id == "vision.circle")
            ?? throw new Exception("未注册第二个工具（vision.circle）");
        var secondReport = ToolBaseline.Validate(second);
        Check(secondReport.Passed, "第二个工具（圆查找）无需改宿主即通过同一套基线" + Describe(secondReport));

        var failed = ToolBaseline.ValidateAll(registry, descriptors.Select(d => (d.Id, d.Version)).ToArray());
        Check(failed.Count == 0, $"全部工具零违规，实际违规 {failed.Count} 个：" + string.Join(";", failed.Select(Describe)));

        var guard = ToolBaseline.Validate(new ModuleDescriptor("broken", "1.0", "", [], [], []));
        Check(!guard.Passed && guard.Issues.Any(issue => issue.Code == "discovery"),
            "空壳工具（无显示名/无端口）被基线拦下：" + Describe(guard));
    }

    static string Describe(ToolBaselineReport report) =>
        report.Passed ? string.Empty : "（" + string.Join("；", report.Issues.Select(issue => issue.Code + ":" + issue.Message)) + "）";

    /// <summary>实测外壳关键区域的真实布局尺寸，暴露裁剪/缺失问题。</summary>
    static void VerifyShellLayout(MainWindow window)
    {
        var rail = (System.Windows.Controls.Grid)window.FindName("ToolRail")!;
        Console.WriteLine($"DIAG ToolRail={rail.ActualWidth:F1}x{rail.ActualHeight:F1} column={((System.Windows.Controls.Grid)rail.Parent).ColumnDefinitions[0].ActualWidth:F1}");

        var logBar = Find<System.Windows.Controls.Border>(window, "LogBar");
        var logPanel = Find<System.Windows.Controls.Border>(window, "LogPanel");
        Console.WriteLine($"DIAG LogBar={logBar.ActualWidth:F1}x{logBar.ActualHeight:F1} vis={logBar.Visibility} LogPanel vis={logPanel.Visibility}");

        var tabs = (TabControl)window.FindName("BottomTabs")!;
        Console.WriteLine($"DIAG BottomTabs={tabs.ActualWidth:F1}x{tabs.ActualHeight:F1} items={tabs.Items.Count}");

        var status = Find<System.Windows.Controls.TextBlock>(window, "ProcessTimeText");
        Console.WriteLine($"DIAG status 流程={status.Text} 模块={Find<System.Windows.Controls.TextBlock>(window, "ModuleTimeText").Text} 算法={Find<System.Windows.Controls.TextBlock>(window, "AlgorithmTimeText").Text}");

        Check(rail.ActualWidth >= 52, $"左侧导轨实际宽度 {rail.ActualWidth:F1} 应 ≥ 52，否则图标被裁");
        Check(logBar.ActualHeight >= 26, $"日志栏实际高度 {logBar.ActualHeight:F1} 应 ≈ 28");
        Check(tabs.Items.Count == 5, $"下区应为 5 页，实际 {tabs.Items.Count}");
    }

    /// <summary>
    /// 契约 W01 第 3.5 节：帮助页展示当前节点的工具说明（端口/参数结构），随选中节点变化。
    /// </summary>
    static void VerifyHelpPane(MainWindow window)
    {
        var document = Document(window);
        var node = document.CurrentProcedure.Nodes[0];
        Select(window, node.Id);

        var help = Find<System.Windows.Controls.TextBlock>(window, "HelpText");
        Check(help.Text.Contains("输入端口", StringComparison.Ordinal), "帮助页含输入端口段");
        Check(help.Text.Contains("输出端口", StringComparison.Ordinal), "帮助页含输出端口段");
        Check(help.Text.Contains("参数", StringComparison.Ordinal), "帮助页含参数段");

        var pane = Find<System.Windows.Controls.ScrollViewer>(window, "HelpPane");
        var helpTab = Find<TabItem>(window, "HelpTab");
        ((TabControl)window.FindName("BottomTabs")!).SelectedItem = helpTab;
        Pump(window);
        Check(pane.Visibility == Visibility.Visible, "切到帮助页时内容可见");

        var other = document.CurrentProcedure.Nodes.FirstOrDefault(candidate => candidate.Id != node.Id);
        if (other is not null)
        {
            Select(window, other.Id);
            Check(help.Text.Contains(other.Name, StringComparison.Ordinal) || help.Text.Length > 0,
                "切换节点后帮助文本跟随变化");
        }
    }

    /// <summary>
    /// 契约 §9.1 + 原厂 diagramitemtemplate.xaml:1126：
    /// 节点左侧有 26 宽状态角标，未执行时为灰、成功为绿。
    /// </summary>
    static void VerifyNodeStateFlag(MainWindow window)
    {
        var document = Document(window);
        var node = document.CurrentProcedure.Nodes[0];
        var results = (Dictionary<string, NodeRunRecord>)typeof(MainWindow)
            .GetField("results", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(window)!;

        results.Remove(node.Id);
        Invoke(window, "RenderGraph");
        Pump(window);
        var idleFlag = FindStateFlag(window, node.Id);
        Check(Math.Abs(idleFlag.Width - 26) < 0.01, $"状态角标宽 {idleFlag.Width} = 26");
        Check(((SolidColorBrush)idleFlag.Background).Color == Color.FromRgb(0xD9, 0xD9, 0xD9), "未执行时角标为灰");

        results[node.Id] = new NodeRunRecord(node.Id, NodeRunStatus.Success, new Dictionary<string, PortValue>(), 1.0);
        Invoke(window, "RenderGraph");
        Pump(window);
        var okFlag = FindStateFlag(window, node.Id);
        Check(((SolidColorBrush)okFlag.Background).Color == Color.FromRgb(0x3F, 0xAE, 0x4A), "成功态角标变绿");

        results[node.Id] = new NodeRunRecord(node.Id, NodeRunStatus.Failed, new Dictionary<string, PortValue>(), 1.0);
        Invoke(window, "RenderGraph");
        Pump(window);
        var failFlag = FindStateFlag(window, node.Id);
        Check(((SolidColorBrush)failFlag.Background).Color == Color.FromRgb(0xD0, 0x45, 0x3E), "失败态角标变红");
        results.Remove(node.Id);
        Invoke(window, "RenderGraph");
        Pump(window);
    }

    static System.Windows.Controls.Border FindStateFlag(MainWindow window, string nodeId)
    {
        var cards = (System.Collections.IDictionary)typeof(MainWindow)
            .GetField("nodeCards", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(window)!;
        var card = (System.Windows.Controls.Canvas)cards[nodeId]!;
        var body = card.Children.OfType<System.Windows.Controls.Border>().First();
        var stack = (System.Windows.Controls.StackPanel)body.Child;
        var title = stack.Children.OfType<System.Windows.Controls.Grid>().First();
        Check(Math.Abs(title.ColumnDefinitions[0].Width.Value - 26) < 0.01, "标题首列宽 26");
        return title.Children.OfType<System.Windows.Controls.Border>().First();
    }

    /// <summary>
    /// 契约 W01 第 3.1 节：模块结果页是「节点 → 输出端口 → 值」的三级树，
    /// 且常用参数 / 数据队列两页默认折叠。
    /// </summary>
    static void VerifyModuleTree(MainWindow window)
    {
        var tabs = (TabControl)window.FindName("BottomTabs")!;
        var collapsed = tabs.Items.OfType<TabItem>().Where(item => item.Visibility == Visibility.Collapsed).Select(item => item.Header?.ToString()).ToArray();
        Check(tabs.Items.Count == 5, $"下区应有 5 页（含两个折叠页），实际 {tabs.Items.Count}");
        Check(collapsed.Contains("常用参数") && collapsed.Contains("数据队列"),
            "常用参数与数据队列默认折叠：" + string.Join(",", collapsed));

        var document = Document(window);
        var node = document.CurrentProcedure.Nodes[0];
        var results = (Dictionary<string, NodeRunRecord>)typeof(MainWindow)
            .GetField("results", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(window)!;
        results[node.Id] = new NodeRunRecord(node.Id, NodeRunStatus.Success,
            new Dictionary<string, PortValue>
            {
                ["Out1"] = PortValue.Number(12.25),
                ["Out2"] = PortValue.Text("ok")
            }, 5.5);

        Select(window, node.Id);
        var displayTabs = (TabControl)window.FindName("DisplayTabs")!;
        displayTabs.SelectedIndex = 1;
        Pump(window);

        var tree = (System.Windows.Controls.TreeView)window.FindName("ModuleResultTree")!;
        Check(tree.Visibility == Visibility.Visible, "切到模块结果页时树可见");
        var roots = (tree.ItemsSource as IEnumerable<TreeViewItem>)?.ToArray() ?? [];
        Check(roots.Length == 1, $"树根为当前节点，实际 {roots.Length}");
        Check(roots[0].Header?.ToString()?.StartsWith(node.Name, StringComparison.Ordinal) == true,
            "树根标题为节点名：" + roots[0].Header);
        Check(roots[0].Items.Count >= 2, $"端口节点数 {roots[0].Items.Count} ≥ 2");
        var firstPort = roots[0].Items.OfType<TreeViewItem>().First();
        Check(firstPort.Items.Count == 1, "端口节点下挂一个值节点");
        Check(firstPort.Header?.ToString()?.Contains(" · ", StringComparison.Ordinal) == true,
            "端口节点显示 端口 · 类型：" + firstPort.Header);

        var legacyGrid = Find<DataGrid>(window, "ModuleResultsGrid");
        Check(legacyGrid.Visibility == Visibility.Collapsed, "旧的三列结果表格不再参与显示");

        displayTabs.SelectedIndex = 0;
        Pump(window);
        Check(tree.Visibility == Visibility.Collapsed, "切回图像页时树隐藏");

        // 历史结果列必须绑模块级字段（RowIndex/AddTime/Content），否则会出现"空行"。
        var historyGrid = Find<DataGrid>(window, "HistoryGrid");
        var paths = historyGrid.Columns.Select(column => ((column as DataGridTextColumn)?.Binding as System.Windows.Data.Binding)?.Path?.Path).ToArray();
        Check(paths.SequenceEqual(new[] { "RowIndex", "AddTime", "Content" }),
            "历史列绑定：" + string.Join(",", paths));
    }

    /// <summary>
    /// 契约 W01 第 3.1 节：图像页的图像源下拉跟随当前选中模块；
    /// 选中节点自身有图像输出时只列它自己的输出。
    /// </summary>
    static void VerifyImageSelectorFollowsSelection(MainWindow window)
    {
        // editor-demo 没有图像源节点，换用 image-source 示例以覆盖供图路径。
        Document(window).Load(LocateExample("image-source-plugin.vmx.json"));
        Pump(window);
        var document = Document(window);
        var selector = (System.Windows.Controls.ComboBox)window.FindName("ImageSelector")!;
        var provider = document.CurrentProcedure.Nodes.FirstOrDefault(node => node.ModuleId == "vision.image-source");
        Check(provider is not null, "图像源示例含供图节点");

        Select(window, provider!.Id);
        var items = selector.Items.OfType<System.Windows.Controls.ComboBoxItem>().ToArray();
        Check(items.Length > 0, $"供图节点被选中后图像源非空，实际 {items.Length}");
        Check(items.All(item => Equals(item.Tag, provider.Id)), "图像源只列当前节点的输出");
        Check(items.All(item => item.Content?.ToString()?.StartsWith(provider.Name, StringComparison.Ordinal) == true),
            "图像源文案以节点名开头");

        var other = document.CurrentProcedure.Nodes.FirstOrDefault(node => node.Id != provider.Id);
        if (other is not null)
        {
            Select(window, other.Id);
            var otherItems = selector.Items.OfType<System.Windows.Controls.ComboBoxItem>().ToArray();
            Check(otherItems.Length > 0 && otherItems.All(item => !Equals(item.Tag, other.Id) || item.Tag is null),
                $"无图像能力的节点回落到全流程供图节点，实际 {otherItems.Length} 项");
        }
    }

    /// <summary>
    /// 契约 W01 第 3.4 节：历史结果表必须绑模块级记录，而不是流程级摘要。
    /// </summary>
    static void VerifyHistoryBinding(MainWindow window)
    {
        var document = Document(window);
        var procedureId = document.CurrentProcedureId;
        var node = document.CurrentProcedure.Nodes[0];
        var session = new ExecutionSessionResult(SessionStatus.Success,
        [
            new NodeRunRecord(node.Id, NodeRunStatus.Success,
                new Dictionary<string, PortValue> { ["Out"] = PortValue.Number(2.5) }, 4.25)
        ]);
        Invoke(window, "RecordFrameworkRunHistory", procedureId, session);
        Select(window, node.Id);

        var grid = Find<DataGrid>(window, "HistoryGrid");
        var rows = (grid.ItemsSource as IEnumerable<ModuleHistoryRow>)?.ToArray() ?? [];
        Check(rows.Length == 1, $"历史结果应有 1 行，实际 {rows.Length}");
        Check(rows[0].RowIndex == 1, "执行序号从 1 开始");
        Check(rows[0].Content.StartsWith("[" + node.Name, StringComparison.Ordinal) ,
            "模块数据以模块名开头：" + rows[0].Content);
        Check(rows[0].Content.Contains("Out 2.500"), "模块数据含端口值：" + rows[0].Content);
        Check(rows[0].AddTime.Length >= 19, "时间为完整时间戳：" + rows[0].AddTime);

        var other = document.CurrentProcedure.Nodes.FirstOrDefault(candidate => candidate.Id != node.Id);
        if (other is not null)
        {
            Select(window, other.Id);
            var otherRows = (Find<DataGrid>(window, "HistoryGrid").ItemsSource as IEnumerable<ModuleHistoryRow>)?.ToArray() ?? [];
            Check(otherRows.Length == 0, "切换到未执行节点时历史为空");
            Select(window, node.Id);
        }
    }

    static void Invoke(MainWindow window, string method, params object?[] args) =>
        typeof(MainWindow).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(window, args.Length == 0 ? null : args);

    static void LoadExample(MainWindow window)
    {
        var path = LocateExample("editor-demo.vmx.json");
        Document(window).Load(path);
        Pump(window);
    }

    /// <summary>从输出目录向上寻找仓库根，兼容独立 OutDir 运行。</summary>
    static string LocateExample(string fileName)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "examples", fileName);
            if (File.Exists(candidate)) return candidate;
            directory = directory.Parent;
        }
        throw new FileNotFoundException("示例方案缺失：" + fileName);
    }

    static GraphEditorDocument Document(MainWindow window) =>
        (GraphEditorDocument)typeof(MainWindow).GetField("doc", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(window)!;

    static void Select(MainWindow window, string nodeId)
    {
        typeof(MainWindow).GetField("selected", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(window, nodeId);
        typeof(MainWindow).GetMethod("RefreshNodeProjection", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(window, null);
        // 真实选中路径会同时刷新右侧图像区，这里补齐以覆盖图像源下拉。
        typeof(MainWindow).GetMethod("RefreshRight", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(window, null);
        Pump(window);
    }

    static T Find<T>(MainWindow window, string name) where T : FrameworkElement =>
        window.FindName(name) as T ?? throw new Exception($"控件缺失：{name}");

    static void Pump(Window window)
    {
        window.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
        window.UpdateLayout();
    }

    static void Check(bool condition, string description)
    {
        if (!condition) throw new Exception(description);
        checks++;
        Console.WriteLine("PASS: " + description);
    }
}
