using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace VisionEditor;

/// <summary>
/// 应用启动时序，对应原厂 <c>VisionMaster.App.OnStartup</c>
/// （docs/reverse/C01-startup-chain-behavior-contract-20261111.md S01-S09）。
/// 时序：欢迎页（独立线程/独立 Dispatcher）∥ 后台初始化 → 主线程预加载 →
/// 双线屏障 → 关闭欢迎页 → AppAfterStart（命令行与方案中心门控）→ 主窗口。
/// </summary>
internal sealed class VisionEditorApp : System.Windows.Application
{
    readonly string[] arguments;
    readonly MainWindowData data = new();
    readonly ManualResetEventSlim initializationCompleted = new(false);
    readonly ManualResetEventSlim welcomeReady = new(false);

    Window? welcomeWindow;
    Exception? initializationFailure;
    Exception? welcomeFailure;
    MainWindow? shell;
    bool shuttingDown;

    internal MainWindowData ShellData => data;

    internal VisionEditorApp(string[] arguments)
    {
        // 原厂不设置 ShutdownMode（默认 OnLastWindowClose），进程退出由 App.MainWindow_Closing
        // 里的 Environment.Exit 承担。本项目显式使用 OnExplicitShutdown，使关停语义不依赖
        // 窗口计数（契约第 6 节约束 1）。证据等级：独立实现选择。
        ShutdownMode = ShellLifecycle.ShutdownModeChoice;
        this.arguments = arguments;
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        if (StartupSettings.IsStartTimerGC) StartGcTimer();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        StartWelcome();
        StartInitialization();
        PreloadShellData();
        initializationCompleted.Wait();
        if (initializationFailure is not null)
        {
            DoneException(initializationFailure);
            return;
        }
        CloseWelcome();
        if (welcomeFailure is not null) StartupLaunch.WriteFailure(welcomeFailure);
        AppAfterStart();
        if (shuttingDown) return;
        ShowShellWindow();
        AfterShellWindowShown();
        StartForwardingListener();
    }

    void StartGcTimer()
    {
        // 原厂 EnableTimerGC（App.cs:61-79）。
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        timer.Tick += (_, _) => GC.Collect();
        timer.Start();
    }

    /// <summary>S02：欢迎页在独立 STA 线程与独立 Dispatcher 上显示，不参与应用生命周期。</summary>
    void StartWelcome()
    {
        if (!StartupSettings.ShowEnable)
        {
            welcomeReady.Set();
            return;
        }
        var thread = new Thread(() =>
        {
            try
            {
                var window = new StartupWelcomeWindow();
                welcomeWindow = window;
                window.Show();
                welcomeReady.Set();
                Dispatcher.Run();
            }
            catch (Exception error)
            {
                welcomeFailure = error;
                welcomeReady.Set();
            }
        })
        { IsBackground = true, Name = "VisionEditor.Welcome" };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        welcomeReady.Wait();
    }

    /// <summary>S03：后台初始化线程，最高优先级 STA，异常合并到主线程。</summary>
    void StartInitialization()
    {
        var thread = new Thread(() =>
        {
            try
            {
                StartupLaunch.CompleteInitialization();
            }
            catch (Exception error)
            {
                initializationFailure = error;
            }
            finally
            {
                initializationCompleted.Set();
            }
        })
        { IsBackground = true, Priority = ThreadPriority.Highest, Name = "VisionEditor.Initialization" };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
    }

    /// <summary>S04：主线程预加载宿主数据（原厂 <c>Program.InitMainWindow</c> 的启动壳部分）。</summary>
    void PreloadShellData() => data.PreLoad();

    /// <summary>S05：屏障放行后关闭欢迎页。原厂调用 <c>welcome.Dispatcher.InvokeShutdown()</c>。</summary>
    void CloseWelcome()
    {
        var window = welcomeWindow;
        welcomeWindow = null;
        if (window is null) return;
        try
        {
            window.Dispatcher.InvokeShutdown();
        }
        catch (Exception error) when (error is InvalidOperationException or TaskCanceledException)
        {
            // 欢迎页线程已自行结束。
        }
    }

    /// <summary>S06：命令行方案分流与方案中心门控。</summary>
    void AppAfterStart()
    {
        StartupLaunch.EnterSolutionSelection();
        if (ShouldShowGuide()) ShowGuide();
    }

    bool ShouldShowGuide()
    {
        // 原厂谓词（契约 S06）：
        // !flag && GuideHelper.IsGuideWindowShow() && !IsAutoLoad() && !IsSdkMode()
        //         && !VmSolution.IsTakeOver && loginUser != 3
        // 本项目无 SDK 自动加载、无接管状态、无登录用户，对应项恒为 false / 恒为真。
        var hasCommandLineSolution =
            StartupSolutionStore.CommandLinePath is not null ||
            StartupSolutionStore.UnsupportedOriginalSolutionPath is not null;
        return !hasCommandLineSolution
            && StartupSettings.ShowEnable
            && !IsSdkMode();
    }

    bool IsSdkMode()
    {
        // 原厂 Program.IsSdkMode：Arguments[0] 含 "tcp://"（Program.cs:636-643）。
        var first = arguments.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
        return first is not null && first.Contains("tcp://", StringComparison.OrdinalIgnoreCase);
    }

    void ShowGuide()
    {
        var guide = new StartupGuideWindow();
        guide.ShowDialog();
        StartupSolutionStore.StartNewSolution = guide.StartNewSolution;
        if (guide.SelectedPath is not { Length: > 0 } selected) return;
        if (string.Equals(Path.GetExtension(selected), ".sol", StringComparison.OrdinalIgnoreCase))
            StartupSolutionStore.UnsupportedOriginalSolutionPath = selected;
        else
            StartupSolutionStore.CommandLinePath = selected;
    }

    /// <summary>S08：主窗口展示，顺序与原厂 <c>App.ShowMainWindow</c> 一致。</summary>
    void ShowShellWindow()
    {
        var editor = new VisionEditor.MainWindow();
        shell = editor;
        MainWindow = editor;                                     // Application.Current.MainWindow
        editor.DataContext = data;
        editor.SetBinding(VisionEditor.MainWindow.IsShowProgressMaskProperty, new System.Windows.Data.Binding(nameof(MainWindowData.IsShowProgressMask)));
        editor.SetBinding(VisionEditor.MainWindow.MaskMessageProperty, new System.Windows.Data.Binding(nameof(MainWindowData.MaskMessage)));
        ShellLifecycle.Attach(editor, data);
        editor.Show();
        data.ConfigZoomIndex = StartupSettings.ZoomIndex;        // Program.ConfigZoomIndex()
        data.RecoverMainWindowState(editor);                     // MainWindowData.RecoverMainWindowState()
        StartupLaunch.EnterEditor();
    }

    /// <summary>S09：主窗口显示后的窗口状态与授权提醒调用点。</summary>
    void AfterShellWindowShown()
    {
        if (shell is null) return;
        if (StartupSettings.IsMaximizedForVM && shell.WindowState != WindowState.Maximized)
            shell.WindowState = WindowState.Maximized;
        if (StartupSettings.IsFullScreenForVM)
        {
            shell.Left = 0;
            shell.Top = 0;
            shell.Width = SystemParameters.PrimaryScreenWidth;
            shell.Height = SystemParameters.PrimaryScreenHeight;
        }
        // 原厂此处启动授权提醒线程（Program.IsNeedRemind）。本轮为骨架：恒不提醒。
        if (StartupLaunch.AuthGate.ShouldRemind()) shell.ShowAuthorizationReminder();
    }

    /// <summary>S11：二次实例命令行转发 → 激活主窗口并按转发参数打开方案。</summary>
    void StartForwardingListener()
    {
        StartupForwarding.Listen(forwarded => Dispatcher.BeginInvoke(new Action(() =>
        {
            if (shell is null) return;
            if (shell.WindowState == WindowState.Minimized) shell.WindowState = WindowState.Normal;
            shell.Activate();
            var path = forwarded.FirstOrDefault(value => !value.StartsWith('-'));
            if (!string.IsNullOrWhiteSpace(path)) shell.OpenForwardedSolution(path);
        })));
    }

    /// <summary>S12：启动失败出口，对应原厂 <c>App.DoneException</c>。</summary>
    void DoneException(Exception error)
    {
        if (shuttingDown) return;
        shuttingDown = true;
        StartupLaunch.WriteFailure(error);
        try
        {
            System.Windows.MessageBox.Show(
                $"VisionEditor 启动失败。\n日志：{StartupLaunch.FailureLogPath}",
                StartupSettings.ApplicationName,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        catch (InvalidOperationException)
        {
            // 没有可用的消息循环时直接退出。
        }
        ExitProcess(1);
    }

    static void ExitProcess(int code) => ShellLifecycle.Exit.Exit(code);

    void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        StartupLaunch.WriteFailure(e.Exception);
        // 原厂 App_DispatcherUnhandledException 恒置 Handled = true（App.cs:479）。
        e.Handled = true;
        if (StartupLaunch.Stage != StartupStage.Editor) DoneException(e.Exception);
    }

    void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception error)
        {
            StartupLaunch.WriteFailure(error);
        }
        ExitProcess(1);
    }

    void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        StartupLaunch.WriteFailure(e.Exception);
        e.SetObserved();
    }
}
