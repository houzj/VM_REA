using VisionPluginContracts;

namespace VisionPlugins.Calibration;

/// <summary>Original IMVSNPointCalibModuControl HidePhysicInfoOperation; no caption parsing.</summary>
public sealed class NPointPhysicalVisibilityRule : IToolConfigurationRule
{
    public void Apply(ToolConfigurationState state, ConfigurationOperationRequest request)
    {
        var target = Target(state, request);
        if (target is null) return;
        var acquisition = Required(state, request, "CalibPointGet");
        var input = Required(state, request, "CalibPointInput");
        var angle = Required(state, request, "WorldRotateAngle");
        if (acquisition is null || input is null || angle is null) return;
        var visible = false;
        if (acquisition.Value == "0" && !Bound(angle))
        {
            if (input.Value == "0")
            {
                var point = Required(state, request, "PhyPoint");
                if (point is null) return;
                visible = !Bound(point);
            }
            else
            {
                var x = Required(state, request, "PhyPointX");
                var y = Required(state, request, "PhyPointY");
                if (x is null || y is null) return;
                visible = !Bound(x) && !Bound(y);
            }
        }
        state.SetProgrammatic(target, "CustomVisible", visible.ToString());
    }

    public void Revert(ToolConfigurationState state, ConfigurationOperationRequest request)
    {
        // Original GetOriginalValue always records true, not current visibility.
        if (Target(state, request) is { } target)
            state.SetProgrammatic(target, "CustomVisible", "True");
    }

    static bool Bound(ConfigurationNodeState node) => node.SubscriptionBinding is not null || !string.IsNullOrEmpty(node.Value);

    static ConfigurationNodeState? Target(ToolConfigurationState state, ConfigurationOperationRequest request)
    {
        var target = request.Target ?? (request.TargetName is { } name ? state.Find(name) : null);
        if (target?.IsCategory == true) return target;
        state.ReportDiagnostic(new("MissingRuleTarget", "HidePhysicInfoOperation", request.Source.Name,
            "The original rule requires a resolved Category target."));
        return null;
    }

    static ConfigurationNodeState? Required(ToolConfigurationState state, ConfigurationOperationRequest request, string name)
    {
        var node = state.Find(name);
        if (node is null)
            state.ReportDiagnostic(new("MissingRuleNode", "HidePhysicInfoOperation", request.Source.Name,
                "Required original model node is missing: " + name));
        return node;
    }
}
