using System.IO;
using System.Xml.Linq;

namespace VisionEditor;

/// <summary>
/// Source-guided startup preference snapshot, independent of plugin configuration.
/// VM.Utility.GetConfigValue reads a cfg document, not an appSettings value.
/// A fresh instance represents a new startup snapshot; there is no file watcher.
/// </summary>
public sealed class FrameworkExecutionPreferences
{
    private readonly string configPath;
    private bool? clearLastExecuteResult;

    public FrameworkExecutionPreferences(string configPath)
    {
        this.configPath = configPath;
    }

    // The caller supplies its application name and startup working directory.
    // This deliberately never discovers or reads another installed application's cfg.
    public static FrameworkExecutionPreferences FromWorkingDirectory(string applicationName, string workingDirectory)
        => new(Path.Combine(workingDirectory, applicationName + ".cfg"));

    public bool IsClearLastExecuteResult
    {
        get => clearLastExecuteResult ??= ReadClearLastExecuteResult();
        // Like SystemConfigManager, changing this property only changes memory.
        set => clearLastExecuteResult = value;
    }

    private bool ReadClearLastExecuteResult()
    {
        try
        {
            var document = XDocument.Load(configPath);
            var field = document.Root?.Elements().SingleOrDefault(e => e.Name == "IsClearLastExecuteResult");
            var value = field?.Elements().SingleOrDefault(e => e.Name == "CurValue");
            return value?.Value == "1";
        }
        catch (Exception)
        {
            // Original helper returns null on read/parse/duplicate errors;
            // SystemConfigManager's exact == "1" then caches false.
            return false;
        }
    }

    /// <summary>Original work-status callback Begin guard; it does not mean deleting an execution session.</summary>
    public bool ShouldClearAtBegin(uint executeCount, uint workStatus)
        => executeCount > 1 && workStatus != 0 && IsClearLastExecuteResult;
}
