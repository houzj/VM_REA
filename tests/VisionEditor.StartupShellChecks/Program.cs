using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using WpfApplication = System.Windows.Application;
using WpfListView = System.Windows.Controls.ListView;

internal static class Program
{
    sealed class SpyExit : IProcessExit
    {
        internal int? Code { get; private set; }
        public void Exit(int code) => Code = code;
    }

    sealed class SpyOperation : IApplicationOperation
    {
        internal bool Disposed { get; private set; }
        public void Dispose() => Disposed = true;
    }

    [STAThread]
    static void Main()
    {
        var app = new WpfApplication { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        StartupWelcomeWindow? welcome = null;
        StartupGuideWindow? guide = null;
        MainWindow? main = null;
        var exit = new SpyExit();
        var previousExit = ShellLifecycle.Exit;
        var previousOperation = ShellLifecycle.Operation;
        var previousSaveCheck = ShellLifecycle.SolutionSaveCheck;
        var previousStatePath = MainWindowData.StatePath;
        var previousConfigPath = StartupSettings.ConfigPath;
        try
        {
            VerifyConfigurationBaseline();
            VerifyStartupArgumentRouting();
            VerifyShutdownModeInvariant();
            StartupLaunchProbe();
            VisionEditor.StartupShellChecks.StartupCrashRegression.VerifyDetachedExecutionPortDoesNotRequireCardLookup();

            welcome = new StartupWelcomeWindow { ShowInTaskbar = false, Left = -20000, Top = -20000 };
            welcome.Show();
            welcome.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
            if (welcome.Width != 640 || welcome.Height != 360) throw new Exception("WelcomeDialog dimensions differ from the source XAML contract.");
            if (welcome.FindName("LoadingPicture") is not System.Windows.Forms.PictureBox picture || picture.SizeMode != System.Windows.Forms.PictureBoxSizeMode.StretchImage)
                throw new Exception("WelcomeDialog does not host the original stretched GIF picture box.");

            guide = new StartupGuideWindow { ShowInTaskbar = false, Left = -20000, Top = -20000 };
            guide.Show();
            guide.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
            if (guide.Width != 800 || guide.Height != 546) throw new Exception("GuideWindow dimensions differ from the source XAML contract.");
            if (guide.FindName("RecentList") is not WpfListView) throw new Exception("GuideWindow recent-solution list is missing.");
            RenderGuide(guide);
            VerifyGuideAppearance(guide);

            main = new MainWindow { ShowInTaskbar = false, Left = -20000, Top = -20000 };
            main.Show();
            main.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
            VerifyMainWindowHost(main);
            VerifySolutionEntityLifecycle(main);
            VerifyCommandLineSolutionHydration();
            VerifyCloseChain(app, exit);

            var welcomeAsset = Path.Combine(AppContext.BaseDirectory, "ui-assets", "original-guide", "WelcomePage.GIF");
            var guideAsset = Path.Combine(AppContext.BaseDirectory, "ui-assets", "original-guide", "guide_general.png");
            if (!File.Exists(welcomeAsset) || !File.Exists(guideAsset)) throw new Exception("Original startup assets were not copied to the Release output.");
            Console.WriteLine("VisionEditor.StartupShellChecks: WelcomeDialog, GuideWindow, host data and close-chain contracts passed.");
        }
        finally
        {
            ShellLifecycle.Exit = previousExit;
            ShellLifecycle.Operation = previousOperation;
            ShellLifecycle.SolutionSaveCheck = previousSaveCheck;
            MainWindowData.StatePath = previousStatePath;
            StartupSettings.ConfigPath = previousConfigPath;
            guide?.Close();
            welcome?.Close();
            main?.Close();
            app.Shutdown();
        }
    }

    /// <summary>契约第 1 节：缺键时回退原厂默认值。</summary>
    static void VerifyConfigurationBaseline()
    {
        var probe = Path.Combine(Path.GetTempPath(), "visioneditor-settings-probe.config");
        File.Delete(probe);
        StartupSettings.ConfigPath = probe;
        if (!StartupSettings.ShowEnable) throw new Exception("ShowEnable must default to true like the original app.config.");
        if (StartupSettings.ApplicationName != "VisionMaster") throw new Exception("ApplicationName must default to VisionMaster.");
        if (StartupSettings.IsMultiApp) throw new Exception("IsMultiApp must default to the single-instance branch.");
        if (Math.Abs(StartupSettings.ZoomIndex - 1.0) > 0.0001) throw new Exception("ZoomIndex must default to 1.0.");

        StartupSettings.SetShowEnable(false);
        if (StartupSettings.ShowEnable) throw new Exception("ShowEnable must be written back to the configuration file.");
        StartupSettings.SetShowEnable(true);

        var restored = Path.Combine(AppContext.BaseDirectory, "VisionEditor.exe.config");
        StartupSettings.ConfigPath = restored;
        if (!File.Exists(restored)) throw new Exception("The product configuration file is missing from the build output.");
    }

    /// <summary>契约 S00 / S06：命令行方案分流。</summary>
    static void VerifyStartupArgumentRouting()
    {
        StartupLaunch.Prepare(new[] { @"C:\does-not-exist\sample.vmx.json" });
        if (StartupSolutionStore.CommandLinePath is null || StartupSolutionStore.UnsupportedOriginalSolutionPath is not null)
            throw new Exception("A JSON solution path must route to the independent command-line solution slot.");
        StartupLaunch.Prepare(new[] { @"C:\does-not-exist\sample.sol" });
        if (StartupSolutionStore.UnsupportedOriginalSolutionPath is null || StartupSolutionStore.CommandLinePath is not null)
            throw new Exception("An original .sol path must route to the explicit unsupported slot.");
        StartupSolutionStore.CommandLinePath = null;
        StartupSolutionStore.UnsupportedOriginalSolutionPath = null;
        StartupLaunch.Prepare(Array.Empty<string>());
        if (StartupSolutionStore.CommandLinePath is not null || StartupSolutionStore.UnsupportedOriginalSolutionPath is not null)
            throw new Exception("Empty arguments must not select a solution.");
    }

    /// <summary>契约第 6 节约束 1：关停语义不得依赖窗口计数。</summary>
    static void VerifyShutdownModeInvariant()
    {
        if (ShellLifecycle.ShutdownModeChoice != ShutdownMode.OnExplicitShutdown)
            throw new Exception("The shell must not let a closed window drive application shutdown.");
    }

    static void StartupLaunchProbe() => StartupLaunch.CompleteInitialization();

    /// <summary>
    /// 方案中心外观契约：几何取自原厂 GuideWindow.xaml，配色取自 colorstyle1.xaml，
    /// 资源取自 g.resources 提取目录。详见 docs/reverse/C01-guide-window-visual-parity-20261111.md。
    /// </summary>
    static void VerifyGuideAppearance(StartupGuideWindow guide)
    {
        var shell = (System.Windows.Controls.Border)guide.FindName("GuideShell")!;
        if (shell.CornerRadius.TopLeft != 4) throw new Exception("GuideWindow shell must keep the source 4px corner radius.");
        if (shell.BorderBrush is not SolidColorBrush { Color: var border } || border != (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#404040"))
            throw new Exception("GuideWindow shell border must use Col_GuideWindow_Background_BorderBrush #404040.");

        var titleBar = (FrameworkElement)guide.FindName("TitleBar")!;
        if (Math.Abs(titleBar.ActualHeight - 130) > 0.5) throw new Exception($"GuideWindow title bar must be 130 high, got {titleBar.ActualHeight}.");

        var cardBlock = (FrameworkElement)guide.FindName("CardBlock")!;
        // 原厂几何：130 + 22 + 130 = 282；窗口客户端比 546 少 1px 时实测 281，
        // 与原厂同样只可能裁掉最后 1px 像素，因此按 280..282 区间断言并单独校验行位置。
        if (cardBlock.ActualHeight < 280 || cardBlock.ActualHeight > 282)
            throw new Exception($"Card block must stay within 280..282 (130+22+130), got {cardBlock.ActualHeight}.");

        var cards = new[] { "GeneralCard", "PositionCard", "DefectCard", "RecognitionCard" }
            .Select(name => guide.FindName(name) as GuideCard ?? throw new Exception($"Guide card missing: {name}")).ToArray();
        var expectedTypes = new[] { "General", "Position", "Defect", "Recognition" };
        for (var index = 0; index < cards.Length; index++)
        {
            if (cards[index].BackImage is not BitmapImage || cards[index].BackImageClick is not BitmapImage)
                throw new Exception($"Guide card {expectedTypes[index]} must bind the original normal and hover images.");
            if (cards[index].ToolBoxType != expectedTypes[index])
                throw new Exception($"Guide card {expectedTypes[index]} lost its ToolBoxType.");
            if (string.IsNullOrWhiteSpace(cards[index].HelpContent))
                throw new Exception($"Guide card {expectedTypes[index]} lost its help text.");
        }

        var origins = cards.Select(card => card.TransformToAncestor(cardBlock).Transform(new System.Windows.Point(0, 0))).ToArray();
        if (Math.Abs(origins[1].X - 210) > 0.5) throw new Exception($"Cards must sit in two 210px columns, second card x={origins[1].X}.");
        if (Math.Abs(origins[2].Y - 152) > 0.5) throw new Exception($"Second card row must start 152px below the first, y={origins[2].Y}.");
        if (origins[2].Y + 130 > cardBlock.ActualHeight + 1.5)
            throw new Exception($"The second card row overflows the card block by more than the 1px client tolerance (row bottom={origins[2].Y + 130}, block={cardBlock.ActualHeight}).");

        var list = (WpfListView)guide.FindName("RecentList")!;
        if (Math.Abs(list.ActualHeight - cardBlock.ActualHeight) > 0.5)
            throw new Exception($"Recent list height must follow the card block ({cardBlock.ActualHeight}), got {list.ActualHeight}.");
        if (!ReferenceEquals(list.ItemsSource, guide.RecentlySolutions))
            throw new Exception("GuideWindow recent solutions must come from the RecentlySolutions dependency property.");

        var toggle = guide.FindName("HideGuide") as System.Windows.Controls.Primitives.ToggleButton
            ?? throw new Exception("GuideWindow 'do not show' control must be the source toggle button, not a checkbox.");
        if (toggle.IsChecked != !StartupSettings.ShowEnable)
            throw new Exception("GuideWindow 'do not show' state must mirror ShowEnable.");

        var footer = guide.FindName("GuideFooter") as System.Windows.Controls.Menu
            ?? throw new Exception("GuideWindow footer must be the source menu host.");
        var footerItems = footer.Items.OfType<GuideIconMenuItem>().ToArray();
        var separators = footer.Items.OfType<System.Windows.Controls.Separator>().ToArray();
        if (footerItems.Length != 3 || separators.Length != 2)
            throw new Exception($"GuideWindow footer must hold 3 icon entries and 2 separators, got {footerItems.Length}/{separators.Length}.");
        if (footerItems.Any(item => item.NormalIcon is not BitmapImage || item.HoverIcon is not BitmapImage || item.ClickIcon is not BitmapImage))
            throw new Exception("GuideWindow footer entries must bind the original normal/hover/click icons.");
        if (footerItems.Select(item => item.Header?.ToString()).SequenceEqual(new[] { "学习使用VISIONMASTER", "查看示例方案", "获取更多支持和帮助" }) == false)
            throw new Exception("GuideWindow footer captions differ from the source XAML.");

        var openButton = guide.FindName("OpenFileButton") as GuideIconMenuItem
            ?? throw new Exception("GuideWindow open-file icon must live in the second column header.");
        if (openButton.NormalIcon is not BitmapImage) throw new Exception("GuideWindow open-file normal icon is missing.");
        if (openButton.HoverIcon is not BitmapImage)
            throw new Exception("The original 'guide_open_click .png' entry (space before the extension) was not resolved.");

        foreach (var key in new[] { "Img_CardGeneral", "Img_CheckOff", "Img_CheckOn", "Img_OpenClick", "Img_Ic1Normal", "Img_Ic3Click" })
            if (guide.Resources[key] is not BitmapImage) throw new Exception($"GuideWindow dynamic image resource missing: {key}.");
    }

    /// <summary>1:1 渲染方案中心，供人工逐像素对照原厂截图。</summary>
    static void RenderGuide(StartupGuideWindow guide)
    {
        guide.UpdateLayout();
        var directory = Path.Combine("analysis", "full-audit");
        Directory.CreateDirectory(directory);
        var bitmap = new RenderTargetBitmap((int)Math.Ceiling(guide.ActualWidth), (int)Math.Ceiling(guide.ActualHeight), 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(guide);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using var file = File.Create(Path.Combine(directory, "guide-window-current.png"));
        encoder.Save(file);
    }

    /// <summary>契约 S04 / S08 / W04 / W05：宿主数据、绑定与进度遮罩。</summary>
    static void VerifyMainWindowHost(MainWindow main)
    {
        if (main.Title != StartupSettings.ApplicationName) throw new Exception("MainWindow title must come from ApplicationName.");
        if (main.MinWidth != 800 || main.MinHeight != 600) throw new Exception("MainWindow minimum size contract changed.");

        var data = new MainWindowData();
        data.PreLoad();
        if (Math.Abs(data.ConfigZoomIndex - StartupSettings.ZoomIndex) > 0.0001) throw new Exception("PreLoad must seed ConfigZoomIndex from ZoomIndex.");
        if (data.IsNormalMode != (StartupSettings.RunningMode != 1)) throw new Exception("PreLoad must derive IsNormalMode from RunningMode.");

        main.DataContext = data;
        main.SetBinding(MainWindow.IsShowProgressMaskProperty, new System.Windows.Data.Binding(nameof(MainWindowData.IsShowProgressMask)));
        main.SetBinding(MainWindow.MaskMessageProperty, new System.Windows.Data.Binding(nameof(MainWindowData.MaskMessage)));
        var root = (System.Windows.Controls.Grid)main.FindName("ShellRoot")!;
        var baseline = root.Children.Count;
        data.ShowMask("正在初始化");
        main.Dispatcher.Invoke(() => { }, DispatcherPriority.DataBind);
        if (!main.IsShowProgressMask || main.MaskMessage != "正在初始化") throw new Exception("Progress mask bindings did not follow MainWindowData.");
        if (root.Children.Count != baseline + 1) throw new Exception("The progress mask layer was not added to the shell root.");
        data.HideMask();
        main.Dispatcher.Invoke(() => { }, DispatcherPriority.DataBind);
        if (main.IsShowProgressMask || root.Children.Count != baseline) throw new Exception("The progress mask layer was not removed.");
    }

    /// <summary>
    /// G2：启动载荷必须由同一个 GraphEditorDocument 实体贯穿新建、保存和重开。
    /// 这里只验证当前独立格式的真实合同；原厂 .sol 字段与容器仍由原厂证据决定。
    /// </summary>
    static void VerifySolutionEntityLifecycle(MainWindow main)
    {
        var field = typeof(MainWindow).GetField("doc", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?? throw new Exception("MainWindow solution document field is missing.");
        var document = field.GetValue(main) as GraphEditorDocument
            ?? throw new Exception("MainWindow must own one GraphEditorDocument instance.");
        var identity = document.SolutionId;
        if (string.IsNullOrWhiteSpace(identity) || !document.HasCurrentProcedure)
            throw new Exception("Startup must expose a non-empty solution identity and current procedure.");

        var path = Path.Combine(Path.GetTempPath(), "VisionEditor-G2-" + Guid.NewGuid().ToString("N"), "roundtrip.vmx.json");
        try
        {
            document.Save(path);
            if (!string.Equals(document.FilePath, Path.GetFullPath(path), StringComparison.OrdinalIgnoreCase))
                throw new Exception("Save must bind the same solution entity to its canonical path.");
            if (!string.Equals(document.SolutionId, identity, StringComparison.Ordinal))
                throw new Exception("Save must not replace the solution identity.");

            document.Load(path);
            if (!string.Equals(document.SolutionId, identity, StringComparison.Ordinal))
                throw new Exception("Reopen must restore the persisted solution identity.");
            if (!string.Equals(document.CurrentProcedureId, "main", StringComparison.Ordinal))
                throw new Exception("Reopen must restore the persisted main procedure selection.");
            if (document.IsDirty)
                throw new Exception("A successful reopen must clear the dirty state.");
        }
        finally
        {
            var root = Path.GetDirectoryName(path);
            if (root is not null && Directory.Exists(root)) Directory.Delete(root, recursive: true);
        }
    }

    /// <summary>
    /// C01/S06：命令行方案必须在主窗口构造期间进入同一个真实文档实体，
    /// 并保留其 ID、流程选择和规范化文件路径；不能只记住参数后回落默认方案。
    /// </summary>
    static void VerifyCommandLineSolutionHydration()
    {
        var previous = StartupSolutionStore.CommandLinePath;
        var previousOriginal = StartupSolutionStore.UnsupportedOriginalSolutionPath;
        var root = Path.Combine(Path.GetTempPath(), "VisionEditor-C01-" + Guid.NewGuid().ToString("N"));
        var path = Path.Combine(root, "command-line.vmx.json");
        const string identity = "command-line-startup-identity";
        try
        {
            Directory.CreateDirectory(root);
            SolutionStore.Save(path, new SolutionDocument(
                SolutionStore.CurrentVersion,
                identity,
                "命令行方案",
                [new ProcedureDocument("main", "Main", [], [], [])], []));
            StartupSolutionStore.CommandLinePath = path;
            StartupSolutionStore.UnsupportedOriginalSolutionPath = null;

            var shell = new MainWindow { ShowInTaskbar = false, ShowActivated = false, Left = -20000, Top = -20000 };
            var field = typeof(MainWindow).GetField("doc", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?? throw new Exception("MainWindow solution document field is missing for command-line hydration.");
            var document = field.GetValue(shell) as GraphEditorDocument
                ?? throw new Exception("Command-line startup did not create a GraphEditorDocument.");
            if (document.SolutionId != identity)
                throw new Exception($"Command-line startup loaded {document.SolutionId}, expected {identity}.");
            if (document.CurrentProcedureId != "main" || document.IsDirty)
                throw new Exception("Command-line startup must select clean persisted main procedure.");
            if (!string.Equals(document.FilePath, Path.GetFullPath(path), StringComparison.OrdinalIgnoreCase))
                throw new Exception("Command-line startup must bind the canonical solution path.");
            Console.WriteLine("PASS: command-line solution hydrates the real document entity");
        }
        finally
        {
            StartupSolutionStore.CommandLinePath = previous;
            StartupSolutionStore.UnsupportedOriginalSolutionPath = previousOriginal;
            if (Directory.Exists(root)) Directory.Delete(root, recursive: true);
        }
    }

    /// <summary>契约 S10：只有应用主窗口关闭才触发释放与退出；保存检查可取消关闭。</summary>
    static void VerifyCloseChain(WpfApplication app, SpyExit exit)
    {
        var operation = new SpyOperation();
        ShellLifecycle.Operation = operation;
        ShellLifecycle.Exit = exit;
        ShellLifecycle.SolutionSaveCheck = null;
        MainWindowData.StatePath = Path.Combine(Path.GetTempPath(), "visioneditor-window-state.txt");
        File.Delete(MainWindowData.StatePath);

        var data = new MainWindowData();

        var standalone = NewShellWindow();
        ShellLifecycle.Attach(standalone, data);
        standalone.Close();
        if (exit.Code is not null) throw new Exception("Closing a window that is not the application main window must not exit the process.");
        if (operation.Disposed) throw new Exception("The application operation was disposed for a non-main window.");

        var shell = NewShellWindow();
        ShellLifecycle.Attach(shell, data);
        app.MainWindow = shell;
        ShellLifecycle.SolutionSaveCheck = () => true;
        shell.Close();
        if (exit.Code is not null || !shell.IsVisible) throw new Exception("A failed save check must cancel the close.");
        if (operation.Disposed) throw new Exception("Cancelled close must not dispose the application operation.");

        ShellLifecycle.SolutionSaveCheck = () => false;
        shell.Close();
        if (exit.Code != 0) throw new Exception("Closing the application main window must exit with code 0.");
        if (!operation.Disposed) throw new Exception("The application operation must be disposed before exiting.");
        if (!File.Exists(MainWindowData.StatePath)) throw new Exception("The window state must be persisted before exiting.");
    }

    static MainWindow NewShellWindow()
    {
        var window = new MainWindow { ShowInTaskbar = false, Left = -20000, Top = -20000 };
        window.Show();
        window.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
        return window;
    }

}
