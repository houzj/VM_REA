using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace VisionEditor;

// Metadata discovery only. Loading or executing a module requires an explicit registry mapping.
public sealed record DiscoveredTool(string DirectoryName, string NameKey, double Priority,
    string DisplayType, string DirectoryPath, string? ToolTipKey);

public sealed record DiscoveredToolGroup(string DirectoryName, string NameKey, double Priority,
    string DirectoryPath, bool IsGlobal, bool IsDeferred, IReadOnlyList<DiscoveredTool> Tools);

public sealed record ToolCatalogDiagnostic(string Path, string Reason);

public sealed record ToolCatalogSnapshot(IReadOnlyList<DiscoveredToolGroup> Groups,
    IReadOnlyList<ToolCatalogDiagnostic> Diagnostics);

public static class ToolDirectoryCatalog
{
    static readonly HashSet<string> GlobalTools = new(StringComparer.Ordinal)
    {
        "ProcedureModule", "GlobalCameraModule", "DataQueueModule"
    };

    public static ToolCatalogSnapshot Discover(string modulesDirectory, string? priorityStrategy = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modulesDirectory);
        var diagnostics = new List<ToolCatalogDiagnostic>();
        if (!Directory.Exists(modulesDirectory))
            return new ToolCatalogSnapshot([], [new(modulesDirectory, "Module directory is missing.")]);

        var groups = new List<DiscoveredToolGroup>();
        foreach (var directory in EnumerateDirectories(modulesDirectory, diagnostics))
        {
            var id = Path.GetFileName(directory);
            bool global = id == "Global";
            var groupXml = Path.Combine(directory, "ToolGroup.xml");
            if (!global && !File.Exists(groupXml)) continue;

            var metadata = global ? new ToolMetadata("Lang_Tools", 0, "", null, null) : ReadMetadata(groupXml, diagnostics);
            if (metadata is null) continue;
            bool deferred = !global && id is not ("UserTools" or "GroupTools");
            var priority = StrategyPriority(metadata, priorityStrategy);
            var tools = deferred ? Array.Empty<DiscoveredTool>() : ReadTools(directory, global, diagnostics);
            groups.Add(new DiscoveredToolGroup(id, metadata.NameKey, priority, directory,
                global, deferred, tools));
        }
        return new ToolCatalogSnapshot(SortGroups(groups), diagnostics);
    }

    public static ToolCatalogSnapshot Expand(ToolCatalogSnapshot snapshot, string directoryName)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryName);
        var group = snapshot.Groups.SingleOrDefault(g => g.DirectoryName == directoryName);
        if (group is null) throw new KeyNotFoundException("Unknown tool category: " + directoryName);
        if (!group.IsDeferred) return snapshot;
        var diagnostics = snapshot.Diagnostics.ToList();
        var tools = ReadTools(group.DirectoryPath, false, diagnostics);
        return new ToolCatalogSnapshot(snapshot.Groups.Select(g => g.DirectoryName == directoryName
            ? g with { IsDeferred = false, Tools = tools } : g).ToArray(), diagnostics);
    }

    static IReadOnlyList<DiscoveredToolGroup> SortGroups(IEnumerable<DiscoveredToolGroup> groups) =>
        groups.OrderBy(g => g.Priority).ThenBy(g => g.DirectoryName, StringComparer.Ordinal).ToArray();

    static DiscoveredTool[] ReadTools(string directory, bool global, List<ToolCatalogDiagnostic> diagnostics)
    {
        var tools = new List<DiscoveredTool>();
        foreach (var subdirectory in EnumerateDirectories(directory, diagnostics))
        {
            var id = Path.GetFileName(subdirectory);
            if (global && !GlobalTools.Contains(id)) continue;
            var xmlPath = Path.Combine(subdirectory, "ToolItemInfo.xml");
            if (!File.Exists(xmlPath)) continue;
            var metadata = ReadMetadata(xmlPath, diagnostics);
            if (metadata is null) continue;
            tools.Add(new DiscoveredTool(id, metadata.NameKey, metadata.Priority,
                metadata.DisplayType, subdirectory, metadata.ToolTipKey));
        }
        return tools.OrderBy(t => t.Priority).ThenBy(t => t.DirectoryName, StringComparer.Ordinal).ToArray();
    }

    static ToolMetadata? ReadMetadata(string path, List<ToolCatalogDiagnostic> diagnostics)
    {
        try
        {
            using var reader = XmlReader.Create(path, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null
            });
            var root = XDocument.Load(reader, LoadOptions.None).Root;
            if (root?.Name.LocalName != "ToolBoxItemData")
                throw new InvalidDataException("Expected ToolBoxItemData root.");
            string? Field(string name) => root.Elements().FirstOrDefault(e => e.Name.LocalName == name)?.Value.Trim();
            var name = Field("name");
            if (string.IsNullOrWhiteSpace(name)) throw new InvalidDataException("Tool name is empty.");
            var rawPriority = Field("priority");
            if (rawPriority is not null && !double.TryParse(rawPriority, NumberStyles.Float,
                    CultureInfo.InvariantCulture, out _))
                throw new InvalidDataException("Tool priority is invalid.");
            var priority = rawPriority is null ? 0 : double.Parse(rawPriority, CultureInfo.InvariantCulture);
            if (!double.IsFinite(priority)) throw new InvalidDataException("Tool priority is not finite.");
            return new ToolMetadata(name, priority, Field("displayType") ?? "", Field("toolTip"),
                Field("priorityStrategyInfo"));
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or System.Xml.XmlException or InvalidDataException)
        {
            diagnostics.Add(new ToolCatalogDiagnostic(path, error.Message));
            return null;
        }
    }

    static double StrategyPriority(ToolMetadata metadata, string? strategy)
    {
        if (string.IsNullOrEmpty(strategy) || string.IsNullOrEmpty(metadata.PriorityStrategyInfo))
            return metadata.Priority;
        var priority = metadata.Priority;
        foreach (var part in metadata.PriorityStrategyInfo.Split(';'))
        {
            var pair = part.Split(':', 2);
            if (pair.Length == 2 && pair[0] == strategy &&
                double.TryParse(pair[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var value) &&
                double.IsFinite(value)) priority = value;
        }
        return priority;
    }

    static string[] EnumerateDirectories(string path, List<ToolCatalogDiagnostic> diagnostics)
    {
        try { return Directory.GetDirectories(path); }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            diagnostics.Add(new ToolCatalogDiagnostic(path, error.Message));
            return [];
        }
    }

    sealed record ToolMetadata(string NameKey, double Priority, string DisplayType,
        string? ToolTipKey, string? PriorityStrategyInfo);
}
