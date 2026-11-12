using System.IO;
using System.Xml.Linq;

namespace VisionEditor;

/// <summary>
/// 启动配置读取层。原厂使用 app.config appSettings（见
/// docs/reverse/C01-startup-chain-behavior-contract-20261111.md 第 1 节）。
/// 本实现用同一文件格式读取，缺键时回退到原厂默认值。
/// </summary>
internal static class StartupSettings
{
    static readonly object Gate = new();
    static readonly Dictionary<string, string> Values = new(StringComparer.OrdinalIgnoreCase);
    static bool loaded;

    internal static string ConfigPath { get; set; } = Path.Combine(AppContext.BaseDirectory, "VisionEditor.exe.config");

    static void EnsureLoaded()
    {
        if (loaded) return;
        lock (Gate)
        {
            if (loaded) return;
            Load();
            loaded = true;
        }
    }

    static void Load()
    {
        try
        {
            if (!File.Exists(ConfigPath)) return;
            var document = XDocument.Load(ConfigPath);
            foreach (var entry in document.Descendants("appSettings").Elements("add"))
            {
                var key = entry.Attribute("key")?.Value;
                if (string.IsNullOrWhiteSpace(key)) continue;
                Values[key] = entry.Attribute("value")?.Value ?? string.Empty;
            }
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or System.Xml.XmlException)
        {
            // 配置不可读时按原厂默认值继续启动。
        }
    }

    internal static string Read(string key, string fallback)
    {
        EnsureLoaded();
        lock (Gate) return Values.TryGetValue(key, out var value) ? value : fallback;
    }

    internal static bool ReadBool(string key, bool fallback)
    {
        var value = Read(key, fallback ? "True" : "False");
        return bool.TryParse(value, out var parsed) ? parsed : fallback;
    }

    internal static int ReadInt(string key, int fallback)
    {
        var value = Read(key, fallback.ToString(System.Globalization.CultureInfo.InvariantCulture));
        return int.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var parsed) ? parsed : fallback;
    }

    internal static double ReadDouble(string key, double fallback)
    {
        var value = Read(key, fallback.ToString(System.Globalization.CultureInfo.InvariantCulture));
        return double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsed) ? parsed : fallback;
    }

    internal static void Write(string key, string value)
    {
        EnsureLoaded();
        lock (Gate)
        {
            Values[key] = value;
            try
            {
                XDocument document;
                if (File.Exists(ConfigPath)) document = XDocument.Load(ConfigPath);
                else document = new XDocument(new XElement("configuration", new XElement("appSettings")));
                var settings = document.Root?.Element("appSettings");
                if (settings is null)
                {
                    settings = new XElement("appSettings");
                    document.Root?.Add(settings);
                }
                var entry = settings.Elements("add").FirstOrDefault(item => string.Equals(item.Attribute("key")?.Value, key, StringComparison.OrdinalIgnoreCase));
                if (entry is null) settings.Add(new XElement("add", new XAttribute("key", key), new XAttribute("value", value)));
                else entry.SetAttributeValue("value", value);
                document.Save(ConfigPath);
            }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException or System.Xml.XmlException)
            {
                // 写失败不影响当前会话内的会话值。
            }
        }
    }

    // 原厂 app.config 基线（契约第 1 节）。
    internal static bool ShowEnable => ReadBool("ShowEnable", true);
    internal static string ApplicationName
    {
        get
        {
            var name = Read("ApplicationName", "VisionMaster");
            return string.IsNullOrWhiteSpace(name) ? "VisionMaster" : name;
        }
    }
    internal static bool IsMultiApp => Read("IsMultiApp", "0") == "1";
    internal static bool FrontendFirst => ReadBool("FrontendFirst", false);
    internal static bool IsRunBackground => Read("IsRunBackground", "0") == "1";
    internal static int RunningMode => ReadInt("RunningMode", 0);
    internal static double ZoomIndex => ReadDouble("ZoomIndex", 1.0);
    internal static bool IsMaximizedForVM => ReadBool("IsMaximizedForVM", false);
    internal static bool IsFullScreenForVM => ReadBool("IsFullScreenForVM", false);
    internal static bool IsStartTimerGC => ReadBool("IsStartTimerGC", false);
    internal static string Demo => Read("Demo", "\\Samples\\");
    internal static string ToolBoxType => Read("ToolBoxType", "General");

    internal static void SetShowEnable(bool enabled) => Write("ShowEnable", enabled ? "True" : "False");
    internal static void SetToolBoxType(string type) => Write("ToolBoxType", type);
}
