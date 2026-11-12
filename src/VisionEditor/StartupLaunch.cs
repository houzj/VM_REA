using System.IO;

namespace VisionEditor;

internal enum StartupStage
{
    Entry,
    InstanceGuard,
    DevelopmentAuthenticationSkipped,
    SolutionSelection,
    Editor
}

/// <summary>
/// 启动阶段状态与失败记录。原厂对应 <c>Program.Main</c> 的工作目录切换、
/// <c>Program.BeforeStartPart</c> 的后台初始化与 <c>App.DoneException</c> 的日志出口
/// （契约 S00 / S03 / S12）。
/// </summary>
internal static class StartupLaunch
{
    internal static string FailureLogPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VisionEditor", "startup-error.log");

    internal static StartupStage Stage { get; private set; }

    /// <summary>
    /// 授权门禁。原厂为 <c>SoftEncrypt</c> + <c>AccessManagerHelper.LoadAccessInfo</c>；
    /// 本项目只保留接口，恒放行（契约 U05/U06）。
    /// </summary>
    internal static IAuthGate AuthGate { get; set; } = NullAuthGate.Instance;

    /// <summary>
    /// 对应原厂 <c>Directory.SetCurrentDirectory</c> 与命令行参数分流（契约 S00 / S06）。
    /// </summary>
    internal static void Prepare(string[] args)
    {
        Stage = StartupStage.Entry;
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        Stage = StartupStage.InstanceGuard;
        StartupSolutionStore.CommandLinePath = null;
        StartupSolutionStore.UnsupportedOriginalSolutionPath = null;
        var path = args.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value) && !value.StartsWith('-'));
        if (string.IsNullOrWhiteSpace(path)) return;
        path = Path.GetFullPath(path);
        if (string.Equals(Path.GetExtension(path), ".sol", StringComparison.OrdinalIgnoreCase))
            StartupSolutionStore.UnsupportedOriginalSolutionPath = path;
        else
            StartupSolutionStore.CommandLinePath = path;
        Stage = StartupStage.SolutionSelection;
    }

    /// <summary>
    /// 后台初始化阶段。原厂 <c>Program.BeforeStartPart</c> 在独立 STA 最高优先级线程内
    /// 依次执行 SoftEncrypt / InitColor / TrialVersionInit / CPUConfig / 权限加载 /
    /// 方案占用状态 / IOC 注册（契约 S03）。本项目无对应服务，只保留授权门禁调用点。
    /// </summary>
    internal static void CompleteInitialization()
    {
        Stage = StartupStage.DevelopmentAuthenticationSkipped;
        var result = AuthGate.Evaluate();
        if (!result.Allowed)
            throw new InvalidOperationException("授权校验未通过：" + (result.FailureMessageKey ?? "未提供失败原因"));
    }

    internal static void EnterSolutionSelection() => Stage = StartupStage.SolutionSelection;

    internal static void EnterEditor() => Stage = StartupStage.Editor;

    internal static void WriteFailure(Exception error)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FailureLogPath)!);
            File.WriteAllText(FailureLogPath, $"{DateTimeOffset.Now:O}\nStage: {Stage}\n{error}");
        }
        catch (Exception writeError) when (writeError is IOException or UnauthorizedAccessException)
        {
            // 日志写失败不得再抛。
        }
    }
}
