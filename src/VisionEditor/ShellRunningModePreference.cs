using System.Text.Json;

namespace VisionEditor;

/// <summary>
/// Independent persistence for the decompiled RunningMode key. The original
/// ConfigurationHelper storage backend is external to the available source.
/// </summary>
internal static class ShellRunningModePreference
{
    internal static string FilePath { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "VisionReplica", "running-mode.json");

    internal static int Read()
    {
        try
        {
            if (!File.Exists(FilePath)) return 0;
            using var doc = JsonDocument.Parse(File.ReadAllText(FilePath));
            if (doc.RootElement.TryGetProperty("RunningMode", out var value) &&
                value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var index) &&
                index is 0 or 1) return index;
        }
        catch (IOException) { }
        catch (JsonException) { }
        catch (UnauthorizedAccessException) { }
        return 0;
    }

    internal static void Write(int index)
    {
        if (index is not (0 or 1)) throw new ArgumentOutOfRangeException(nameof(index));
        var directory = Path.GetDirectoryName(FilePath) ?? throw new InvalidOperationException("运行模式配置路径无效");
        Directory.CreateDirectory(directory);
        var temporary = Path.Combine(directory, Path.GetRandomFileName());
        try
        {
            File.WriteAllText(temporary, JsonSerializer.Serialize(new { RunningMode = index }));
            File.Move(temporary, FilePath, true);
        }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
    }
}
