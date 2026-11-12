namespace VisionEditor;

/// <summary>
/// 进程入口，对应原厂 <c>Program.Main</c>（契约 S00）：
/// 工作目录 → IsMultiApp 分流 → 单实例 → 构造 App → Run。
/// </summary>
static class Program
{
    [STAThread]
    static int Main(string[] args)
    {
        StartupLaunch.Prepare(args);
        if (StartupSettings.IsMultiApp) return RunApplication(args);
        using var instance = StartupInstance.TryAcquire();
        if (instance is null)
        {
            // 已有实例：转发参数后退出（契约 S00 / S11）。
            StartupForwarding.Forward(args);
            return 0;
        }
        return RunApplication(args);
    }

    static int RunApplication(string[] args)
    {
        try
        {
            return new VisionEditorApp(args).Run();
        }
        catch (Exception error)
        {
            StartupLaunch.WriteFailure(error);
            System.Windows.MessageBox.Show(
                $"VisionEditor 启动失败。\n日志：{StartupLaunch.FailureLogPath}",
                StartupSettings.ApplicationName,
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
            return 1;
        }
    }
}
