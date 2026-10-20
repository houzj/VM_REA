using System.Xml.Linq;
using VisionFramework;

namespace VisionPluginContracts;

/// <summary>The source reset button clears ValueBase subscription modes, then
/// writes the backend Algorithm.xml list in order. It does not reset every editor field.</summary>
public static class ToolConfigurationAlgorithmReset
{
    public static ConfigurationOperationResult Execute(ModuleDescriptor descriptor,
        ToolConfigurationState state, IToolConfigurationDispatcher parameters,
        IToolConfigurationSubscriptionService? subscriptions, string algorithmXml,
        Action? synchronize = null,
        Func<string,string,ConfigurationOperationResult>? writeBackendDefault = null)
    {
        foreach (var node in state.Nodes.Where(n => n.IsInteger32 ||
                     n.Element.Tag is "Float" or "Boolean" or "String" or "OpenFileForCalibDialog"))
        {
            var mode = parameters.Dispatch(new(ConfigurationOperationKind.ReadUserValue,
                "GetSubscriptionMode", node, null,
                ["@" + (node.Name ?? "") + (node.Get("ComTailName") ?? "")],
                ConfigurationChangeKind.Initialization));
            var modeText = mode.Handled && mode.Value is not null ? mode.Value : node.Get("IsSub");
            if (!bool.TryParse(modeText, out var enabled) || !enabled) continue;
            state.SetProgrammatic(node, "IsSub", "False");
            var query = ConfigurationSubscriptionValidation.FromElement(node.Element,
                ConfigurationSubscriptionDomain.Parameter);
            try
            {
                var current = query is null ? null : subscriptions?.Read(query.Destination);
                if (node.SubscriptionBinding is not null || !string.IsNullOrEmpty(node.Get("Subscription")) || current?.Binding is not null)
                {
                    var cleared = query is null ? null : subscriptions?.Bind(query.Destination, null);
                    if (cleared is not null && state.ApplySubscription(node, cleared, true))
                        state.RefreshParameter(node);
                    else state.ReportSubscriptionDiagnostic(node, "ResetRunParamBind",
                        cleared?.Diagnostic ?? "Parameter subscription could not be cleared.");
                }
            }
            catch (Exception error)
            {
                state.ReportSubscriptionDiagnostic(node, "ResetRunParamBind", error.Message);
            }
            // A successful unbind may retain the module's last effective value.
            // Read that authority before persisting unrelated configuration user data.
            synchronize?.Invoke();
            state.PersistSubscriptionMode(node, false);
        }

        string? lastError = null;
        foreach (var item in XDocument.Parse(algorithmXml).Root!.Elements("ParamItem"))
        {
            string name = item.Element("Name")!.Value;
            var node = state.Find(name);
            if (node is null)
            {
                if (writeBackendDefault is not null && descriptor.Parameters.Any(p=>p.Name==name))
                {
                    var written=writeBackendDefault(name,item.Element("DefaultValue")!.Value);
                    if(!written.Handled||written.Error is not null)
                        lastError=written.Error??"Algorithm backend default was not applied: "+name;
                }
                else lastError = "Algorithm default has no configuration member: " + name;
                continue;
            }
            var result = parameters.Dispatch(new(ConfigurationOperationKind.WriteParameter,
                "ResetAlgorithmParameter", node, null, [], ConfigurationChangeKind.Command,
                item.Element("DefaultValue")!.Value));
            if (!result.Handled || result.Error is not null)
                lastError = result.Error ?? "Algorithm default was not applied: " + name;
        }
        // Earlier successful writes and unbinds remain applied if a later write fails.
        if (lastError is not null) return new(true, Error: lastError);
        var known = descriptor.Parameters.Select(p => p.Name).ToHashSet(StringComparer.Ordinal);
        foreach (var node in state.Nodes)
            if (node.Name is { } name && known.Contains(name)) state.RefreshParameter(node);
        return new(true);
    }
}
