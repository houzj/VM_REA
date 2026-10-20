using System;
using VisionPlugins.ContourMatch;

// Production project reference; no vendor execution.
public static class ContourResultPolicyChecks
{
    public static int Main()
    {
        int checks = 0;
        void Check(bool condition, string name)
        {
            if (!condition) throw new Exception(name);
            checks++;
        }
        var off = new FloatLimit(0, 1, 0);
        var limits = new Limits(new CountLimit(0, 0, 0), off, off, off, off, off, off, off, off, off);
        var edges = new[] { new EdgePoint(4, 5, 0f) };
        var good = new Match(1, 1, 2, 3, 4, 0, 5, 6, 1, 1, 1, .9f, 0, 0, edges);
        var bad = good with { Score = .1f };
        var scoreLimits = limits with { Score = new FloatLimit(1, .5f, 1f) };
        var packet = ContourResultPolicy.Build(new[] { good, bad }, new[] { 3, 9 }, scoreLimits, false, 1, true);
        Check(packet.Published.Count == 1 && packet.ActualMatchCount == 2, "published cap");
        Check(!packet.ModuleStatus && packet.Published[0].Status, "unpublished failure judged");
        Check(packet.Published[0].OriginalModelSlot == 9, "ordinal maps original slot");
        Check(packet.Outline.Count == 2 && packet.Outline[1].ContourIndex == 1, "outline not capped");
        Check(packet.Outline[0].Score == 0f && packet.Outline[1].Score == 2f, "per-match outline status");
        edges[0] = new EdgePoint(99, 99, 99);
        Check(packet.Outline[0].X == 4 && packet.Published[0].Value.Edges[0].X == 4, "owned snapshot");
        var countOnly = limits with { Count = new CountLimit(1, 2, 3) };
        packet = ContourResultPolicy.Build(new[] { good }, new[] { 3, 9 }, countOnly, false, 1, true);
        Check(!packet.ModuleStatus && packet.Published[0].Status && packet.Outline[0].Score == 99, "count gate independent");
        Check(ContourResultPolicy.Build(Array.Empty<Match>(), Array.Empty<int>(), limits, true, 1, true).ModuleStatus, "empty allowed");
        Check(!ContourResultPolicy.Build(Array.Empty<Match>(), Array.Empty<int>(), limits, false, 1, true).ModuleStatus, "empty rejected");
        Check(new FloatLimit(2, 5, 0).Accept(1), "enabled exactly one");
        Check(new FloatLimit(1, 0, 1).Accept(1 + 1.1920928955078125e-7f), "epsilon inclusive");
        Check(!new FloatLimit(1, 0, 1).Accept(1 + 2 * 1.1920928955078125e-7f), "outside epsilon");
        Check(new CountLimit(1, 1, 2).Accept(1) && new CountLimit(1, 1, 2).Accept(2), "count endpoints");
        Check(ContourSourceConfiguration.OrderedAlgorithmDefaults.Count == 56, "original ordered defaults count");
        Check(ContourSourceConfiguration.OrderedAlgorithmDefaults[0] == new ContourSourceConfiguration.DefaultEntry("AngleLimitLow", "-180"), "original first default");
        Check(ContourSourceConfiguration.Definition() is not null, "original configuration resources parse");
        Check(ContourSourceConfiguration.DisplayObjects.Count > 0, "original display resources parse");
        var parameters = ContourParameterDescriptors.Parameters;
        Check(parameters.Count(p => p.Name is not "ConfigurationUserData" && p.Name != ContourModelCollection.ParameterName && p.Name != VisionPluginContracts.ToolDisplayService.SettingsParameter && p.Name != VisionPluginContracts.ToolDisplayObjectStore.ParameterName) == 56, "active source parameters declared");
        Check(parameters.Single(p=>p.Name=="MaxMatchNum") is { Kind: VisionFramework.ParameterKind.Integer, Minimum: 1, Maximum: 50000 }, "SDK integer range");
        Check(parameters.Single(p=>p.Name=="MinScore") is { Kind: VisionFramework.ParameterKind.Number, Minimum: 0, Maximum: 1 }, "SDK double range");
        Check(parameters.Single(p=>p.Name=="UseMatchAllMode").Kind == VisionFramework.ParameterKind.Boolean, "SDK bool accessor");
        Check(parameters.Single(p=>p.Name=="Polarity").Minimum is null && parameters.Single(p=>p.Name=="Polarity").Maximum is null, "SDK enum setter has no numeric bound");
        Check(parameters.All(p=>p.Name is not ("NMSMaxOverlap" or "NMSSortType")), "unsupported extra SDK wrappers not writable");
        var plugin = new ContourMatchPlugin();
        Check(plugin.Descriptor.Id==ContourMatchPlugin.Id && plugin is VisionPluginContracts.IToolConfigurationProvider, "production provider integration");
        Console.WriteLine($"PASS {checks} production result/configuration checks");
        return 0;
    }
}

