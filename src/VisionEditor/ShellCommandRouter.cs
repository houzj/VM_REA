namespace VisionEditor;

internal enum ShellCommandState
{
    /// <summary>主窗口已提供实现。</summary>
    Implemented,

    /// <summary>原厂有该入口，但本项目尚无对应能力，禁用并给出原因。</summary>
    Unavailable
}

internal sealed record ShellCommand(ShellCommandState State, Action? Execute, string? Reason);

/// <summary>
/// 原厂 menuitems.xml 的 42 个 <c>methodName</c> 与主窗口实现的映射表。
/// 39 个内部命令 + 3 个外部工具启动命令（OpenExe / OpenExeByLang / OpenModuleParamTool）。
/// 未接入项一律禁用并说明原因，不创建占位窗口。
/// </summary>
internal sealed class ShellCommandRouter
{
    readonly Dictionary<string, Action> actions = new(StringComparer.Ordinal);

    internal void Register(string method, Action execute) => actions[method] = execute;

    internal bool Has(string method) => actions.ContainsKey(method);

    internal ShellCommand Resolve(string? method)
    {
        if (string.IsNullOrWhiteSpace(method)) return new ShellCommand(ShellCommandState.Unavailable, null, "未接入：菜单项未声明命令");
        if (actions.TryGetValue(method, out var action)) return new ShellCommand(ShellCommandState.Implemented, action, null);
        var reason = ShellCommandReasons.For(method);
        return new ShellCommand(ShellCommandState.Unavailable, null, reason);
    }

    /// <summary>覆盖性校验：目录中的每个命令都必须能落到实现或明确的未接入原因上。</summary>
    internal void VerifyCoverage(IEnumerable<string> catalogMethods)
    {
        var missing = new List<string>();
        foreach (var method in catalogMethods)
        {
            if (actions.ContainsKey(method)) continue;
            if (ShellCommandReasons.For(method) is null) missing.Add(method);
        }
        if (missing.Count > 0) throw new InvalidOperationException("菜单命令缺少实现或未接入原因：" + string.Join(", ", missing));
    }

    internal int ImplementedCount => actions.Count;

    /// <summary>42 个命令中已实现的个数（用于清单与测试断言）。</summary>
    internal static int TotalCommandCount => ShellCommandReasons.Total;
}

/// <summary>
/// 未接入命令的原因表。每条都能对应到框架台账中缺失的能力。
/// </summary>
internal static class ShellCommandReasons
{
    internal const int Total = 42;

    static readonly Dictionary<string, string> Reasons = new(StringComparer.Ordinal)
    {
        ["AuthSet"] = "未接入：权限设置服务（AccessManager 未重建）",
        ["ChSet"] = "未接入：界面语言切换（仅保留中文字典）",
        ["EnSet"] = "未接入：界面语言切换（仅保留中文字典）",
        ["CollectSet"] = "未接入：采集策略配置",
        ["Solset"] = "未接入：方案设置",
        ["VMSet"] = "未接入：软件设置",
        ["ComprehensiveConfig"] = "未接入：综合配置（系统配置服务）",
        ["EditFrontendUI"] = "未接入：运行界面编辑（FrontendUI 原件缺失）",
        ["GlobalVariable"] = "未接入：全局变量模型",
        ["GlobalScript"] = "未接入：脚本引擎",
        ["GlobalTrigger"] = "未接入：全局触发器",
        ["CommSet"] = "未接入：通信管理服务",
        ["CameraSet"] = "未接入：相机管理服务",
        ["LightSet"] = "未接入：视觉控制器服务",
        ["LogShow"] = "未接入：运行日志服务",
        ["HelpDoc"] = "未接入：帮助文档未随附",
        ["MoreInfo"] = "未接入：更多支持入口",
        ["OpenVClub"] = "未接入：V 社区在线入口",
        ["ExportAlgorithm"] = "未接入：算子代码导出",
        ["ImportProcess"] = "未接入：原厂方案流程导入（.sol 容器未闭合）",
        ["UndoList"] = "未接入：撤消历史列表",
        ["CreateCalibGuide"] = "未接入：标定配置引导",
        ["OpenExe"] = "未接入：外部工具未随附",
        ["OpenExeByLang"] = "未接入：外部工具未随附",
        ["OpenModuleParamTool"] = "未接入：外部工具未随附"
    };

    internal static string? For(string method) => Reasons.TryGetValue(method, out var reason) ? reason : null;
}
