using System.Globalization;
using System.Xml.Linq;

namespace VisionFramework;

/// <summary>Reads the same application setting used by VM's Group run-parameter category.</summary>
public static class WorkflowRunParameterLimit
{
    static readonly Lazy<int> current = new(() => ReadApplicationSetting());

    public static int Current => current.Value;

    public static int ReadApplicationSetting(string? configurationPath = null)
    {
        int maximum = 32;
        try
        {
            configurationPath ??= Path.Combine(AppContext.BaseDirectory, AppDomain.CurrentDomain.FriendlyName + ".config");
            var document = XDocument.Load(configurationPath, LoadOptions.None);
            var value = document.Root?
                .Element("appSettings")?
                .Elements("add")
                .FirstOrDefault(element => (string?)element.Attribute("key") == "MaxRunParamsNum")?
                .Attribute("value")?
                .Value;
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
                maximum = parsed;
        }
        catch
        {
            // VM defaults to 32 and ignores configuration read/parse failures.
        }
        return maximum;
    }
}
