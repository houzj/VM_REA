namespace VisionEditor;

/// <summary>
/// 授权门禁结果。仅承载"是否放行 + 提示键"，不含加密狗读取。
/// </summary>
internal readonly record struct AuthGateResult(bool Allowed, string? FailureMessageKey, string? ReminderMessageKey);

/// <summary>
/// 授权门禁接口占位。原厂对应 <c>Program.SoftEncrypt</c> /
/// <c>AccessManagerHelper.LoadAccessInfo</c> / <c>EncryptAPI</c> 一族
/// （见契约 S03）。本轮不实现加密狗校验，后续替换为本项目自有加密方案。
/// </summary>
internal interface IAuthGate
{
    AuthGateResult Evaluate();

    bool ShouldRemind();
}

/// <summary>
/// 恒放行实现：保留原厂调用点与提示结构，不做任何校验。
/// 证据等级：待补骨架（契约 U05/U06）。
/// </summary>
internal sealed class NullAuthGate : IAuthGate
{
    internal static readonly NullAuthGate Instance = new();

    public AuthGateResult Evaluate() => new(true, null, null);

    public bool ShouldRemind() => false;
}
