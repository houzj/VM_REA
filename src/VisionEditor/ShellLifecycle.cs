using System.Windows;

namespace VisionEditor;

/// <summary>
/// 主窗口关闭时执行的应用级操作。原厂对应 <c>Ioc.Get&lt;IApplicationOperation&gt;()?.Dispose()</c>。
/// </summary>
internal interface IApplicationOperation
{
    void Dispose();
}

internal sealed class EmptyApplicationOperation : IApplicationOperation
{
    internal static readonly EmptyApplicationOperation Instance = new();

    public void Dispose() { }
}

/// <summary>
/// 进程退出出口。抽成接口是为了让关闭链可被测试观测，生产实现就是
/// <see cref="Environment.Exit(int)"/>（原厂 <c>App.cs:395</c> 的行为）。
/// </summary>
internal interface IProcessExit
{
    void Exit(int code);
}

internal sealed class EnvironmentProcessExit : IProcessExit
{
    public void Exit(int code) => Environment.Exit(code);
}

/// <summary>
/// 主窗口关闭链，对应原厂 <c>App.MainWindow_Closing</c> + <c>Program.IsCancelClose</c>
/// （契约 S10 / S11）。原厂判定顺序：SDK 模式 → 只读用户 → 关闭密码 → 保存检查 →
/// 第三方运行。本项目无 SDK、无登录用户、无关闭密码，仅保留保存检查插槽。
/// 证据等级：判定框架源码确认；各分支实现为本项目独立实现（契约 U05/U07）。
/// </summary>
internal static class ShellLifecycle
{
    /// <summary>
    /// 应用关停模式。原厂不显式设置（默认 OnLastWindowClose），进程退出由
    /// <see cref="EnvironmentProcessExit"/> 承担；本项目固定为 OnExplicitShutdown，
    /// 使"关闭任意窗口"不再参与关停判定（契约第 6 节约束 1）。证据等级：独立实现选择。
    /// </summary>
    internal static ShutdownMode ShutdownModeChoice => ShutdownMode.OnExplicitShutdown;

    internal static IProcessExit Exit { get; set; } = new EnvironmentProcessExit();

    internal static IApplicationOperation Operation { get; set; } = EmptyApplicationOperation.Instance;

    /// <summary>
    /// 保存检查插槽，对应原厂 <c>ProjectManager.SolutionSaveCheck()</c>。
    /// 未接入时返回 false（不阻止关闭），由调用方标注为独立实现。
    /// </summary>
    internal static Func<bool>? SolutionSaveCheck { get; set; }

    /// <summary>
    /// 对应原厂 <c>Program.IsCancelClose()</c> 的放行判定。
    /// 返回 true 表示取消关闭。
    /// </summary>
    internal static bool IsCancelClose()
    {
        // 原厂分支 1：IsSdkMode → 隐藏窗口并取消。本项目无 SDK 模式，恒为 false。
        // 原厂分支 2：只读用户（loginUser == 3）→ 直接放行。本项目无登录用户。
        if (StartupSettings.IsRunBackground) return false;
        return SolutionSaveCheck?.Invoke() ?? false;
    }

    /// <summary>
    /// 把关闭链挂到被 <see cref="System.Windows.Application.MainWindow"/> 持有的窗口上。
    /// 只有作为应用主窗口的实例才会走到退出路径，测试中构造的窗口不受影响。
    /// </summary>
    internal static void Attach(Window window, MainWindowData data)
    {
        window.Closing += (sender, args) =>
        {
            if (args.Cancel) return;
            if (!ReferenceEquals(System.Windows.Application.Current?.MainWindow, sender)) return;
            if (IsCancelClose())
            {
                args.Cancel = true;
                return;
            }
            data.SaveWindowState(window);
            Operation.Dispose();
            Exit.Exit(0);
        };
    }
}
