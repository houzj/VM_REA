using VisionFramework;

namespace VisionEditor.Core;

/// <summary>基线校验结果。Code 便于断言，Message 供人读。</summary>
public sealed record ToolBaselineIssue(string Code, string Message);

public sealed record ToolBaselineReport(string ModuleId, IReadOnlyList<ToolBaselineIssue> Issues)
{
    public bool Passed => Issues.Count == 0;
}

/// <summary>
/// 工具接入基线校验器。对应 W01 契约第 6 节六项能力：
/// 发现元数据 / 节点外观 / 参数与显示列表 / 执行 / 结果投影 / 生命周期。
/// 只做静态结构校验，不执行工具，不访问原生引擎。
/// </summary>
public static class ToolBaseline
{
    public static ToolBaselineReport Validate(ModuleRegistry registry, string moduleId, string version)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleId);
        ModuleDescriptor descriptor;
        try { descriptor = registry.GetDescriptor(moduleId, version); }
        catch (KeyNotFoundException)
        {
            return new(moduleId, [new("discovery", $"模块未注册：{moduleId}@{version}")]);
        }
        return Validate(descriptor);
    }

    /// <summary>直接按已解析的描述符校验，便于测试与离线检视。</summary>
    public static ToolBaselineReport Validate(ModuleDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        var moduleId = descriptor.Id;
        var issues = new List<ToolBaselineIssue>();

        // 1. 发现与元数据
        if (string.IsNullOrWhiteSpace(descriptor.DisplayName)) issues.Add(new("discovery", "缺少 DisplayName"));
        if (descriptor.Id != moduleId) issues.Add(new("discovery", "Id 与注册键不一致"));

        // 2. 节点外观：零输入（图像源、常量）与零输出（控制/终点类）都属于合法形态，
        //    这里只校验端口声明本身不重复，具体连线能力由画布与校验器负责。
        var duplicatePort = descriptor.Outputs.GroupBy(port => port.Name, StringComparer.Ordinal).FirstOrDefault(group => group.Count() > 1);
        if (duplicatePort is not null) issues.Add(new("node-appearance", $"输出端口重名：{duplicatePort.Key}"));
        var hasImageOutput = descriptor.Outputs.Any(port => DataKinds.IsImage(port.Kind));

        // 3. 参数与显示列表：表格型输出必须能被投影层识别（见下方 result-projection 校验）
        var tabular = descriptor.Outputs.FirstOrDefault(port =>
            port.Kind is DataKind.Components or DataKind.Circle or DataKind.Point or DataKind.NumberArray);

        // 4/5. 执行与结果投影：用空记录构造一次投影，列必须非空且与输出类型匹配
        var node = new NodeDocument("baseline-node", descriptor.Id, descriptor.Version, descriptor.DisplayName,
            new Dictionary<string, System.Text.Json.JsonElement>());
        var projection = NodeProjectionBuilder.Build(node, descriptor, null);
        if (projection.Columns.Count == 0) issues.Add(new("result-projection", "无法生成结果列"));
        if (tabular is not null && !projection.HasTable)
            issues.Add(new("result-projection", $"表格型输出 {tabular.Name} 未启用当前结果页"));
        if (hasImageOutput && projection.Images.Count == 0)
            issues.Add(new("result-projection", "图像输出未进入图像列表"));

        // 6. 生命周期：换源时必须能重建列（签名随输出类型变化）
        var session = new NodeProjectionSession();
        session.Update(node, descriptor, null);
        if (session.RebuildCount != 1) issues.Add(new("lifecycle", "首次投影未按签名重建列"));
        session.Clear();
        if (session.Current.Columns.Count != 0) issues.Add(new("lifecycle", "Clear 未复位投影"));

        return new(moduleId, issues);
    }

    /// <summary>批量校验，返回失败项。</summary>
    public static IReadOnlyList<ToolBaselineReport> ValidateAll(ModuleRegistry registry, IEnumerable<(string Id, string Version)> modules)
    {
        var reports = new List<ToolBaselineReport>();
        foreach (var (id, version) in modules)
        {
            var report = Validate(registry, id, version);
            if (!report.Passed) reports.Add(report);
        }
        return reports;
    }
}
