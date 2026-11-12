using System.Text.Json;
using VisionFramework;

namespace VisionEditor.Core;

/// <summary>Source-guided local list operations. ImageListPanelBase fixes six
/// extensions, directory ordering and one level of child directories.
/// JSON storage and case-insensitive duplicates belong to this managed host.</summary>
public static class ImageSourceListOperations
{
    public const string FileFilter = "图像文件|*.bmp;*.jpeg;*.jpg;*.png;*.tif;*.tiff";
    static readonly HashSet<string> extensions = new(StringComparer.OrdinalIgnoreCase) { ".bmp", ".jpeg", ".jpg", ".png", ".tif", ".tiff" };
    public static bool SupportsFile(string path) => extensions.Contains(Path.GetExtension(path));
    public static bool CanDrop(IReadOnlyList<string> paths) => paths.Count > 0 && paths.All(p => File.Exists(p) && SupportsFile(p));
    public static IEnumerable<string> EnumerateFolder(string directory) => Enumerate(directory, 1);
    static IEnumerable<string> Enumerate(string directory, int depth)
    {
        if (!Directory.Exists(directory)) yield break;
        foreach (var path in Directory.GetFileSystemEntries(directory).OrderBy(p => p.Length).ThenBy(p => p))
            if (File.Exists(path)) { if (SupportsFile(path)) yield return path; }
            else if (depth <= 1) foreach (var child in Enumerate(path, depth + 1)) yield return child;
    }
    public static IReadOnlyList<string> Paths(NodeDocument node, string key = "ImagePaths") =>
        node.Parameters.TryGetValue(key, out var value) && value.ValueKind == JsonValueKind.String
            ? JsonSerializer.Deserialize<string[]>(value.GetString() ?? "[]") ?? [] : [];
    static string Current(NodeDocument node) => node.Parameters.TryGetValue("CurrentImagePath", out var value) ? value.GetString() ?? "" : "";
    public static NodeDocument Append(NodeDocument node, IEnumerable<string> added)
    {
        var paths = Paths(node).Concat(added).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var current = Current(node);
        // Import does not seed the first path: AutoPlay advances from the previous path on Run.
        return Replace(node, paths, paths.Contains(current, StringComparer.OrdinalIgnoreCase) ? current : "");
    }
    public static NodeDocument Remove(NodeDocument node, string path)
    {
        var paths = Paths(node).Where(p => !StringComparer.OrdinalIgnoreCase.Equals(p, path)).ToArray();
        return Replace(node, paths, StringComparer.OrdinalIgnoreCase.Equals(Current(node), path) ? "" : Current(node));
    }
    public static NodeDocument Clear(NodeDocument node) => Replace(node, [], "");
    static NodeDocument Replace(NodeDocument node, IReadOnlyList<string> paths, string current)
    {
        var values = node.Parameters.ToDictionary(p => p.Key, p => p.Value.Clone());
        values["ImagePaths"] = JsonSerializer.SerializeToElement(JsonSerializer.Serialize(paths));
        values["SelectedPaths"] = JsonSerializer.SerializeToElement(JsonSerializer.Serialize(Paths(node, "SelectedPaths").Where(p => paths.Contains(p, StringComparer.OrdinalIgnoreCase))));
        values["CurrentImagePath"] = JsonSerializer.SerializeToElement(current);
        return node with { Parameters = values };
    }
    public static NodeDocument ReturnToFirst(NodeDocument node)
    {
        IEnumerable<string> paths = Paths(node);
        if (node.Parameters.TryGetValue("ExecuteType", out var mode) && mode.GetInt32() == 2)
        {
            var selected = Paths(node, "SelectedPaths").ToHashSet(StringComparer.OrdinalIgnoreCase);
            paths = paths.Where(selected.Contains);
        }
        var ordered = paths.ToArray();
        if (ordered.Length == 0) return node;
        bool auto = node.Parameters.TryGetValue("AutoPlay", out var enabled) && enabled.GetBoolean();
        var values = node.Parameters.ToDictionary(p => p.Key, p => p.Value.Clone());
        values["CurrentImagePath"] = JsonSerializer.SerializeToElement(auto ? ordered[^1] : ordered[0]);
        return node with { Parameters = values };
    }
}
