using System.IO;
using System.Xml.Linq;
using VisionPluginContracts;

namespace VisionPlugins.ContourMatch;

/// <summary>Original resources and ordered reset list; no guessed parameter typing.</summary>
public static class ContourSourceConfiguration
{
    public sealed record DefaultEntry(string Name, string Value);
    public static string Resource(string name)
    {
        using var stream = typeof(ContourSourceConfiguration).Assembly.GetManifestResourceStream(
            "VisionPlugins.ContourMatch.Resources." + name) ?? throw new InvalidDataException(name);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public static IReadOnlyList<DefaultEntry> OrderedAlgorithmDefaults { get; } =
        Array.AsReadOnly(XDocument.Parse(Resource("IMVSContourMatchModuAlgorithm.xml"))
            .Root!.Elements("ParamItem").Select(item => new DefaultEntry(
                item.Element("Name")!.Value, item.Element("DefaultValue")!.Value)).ToArray());

    public static ToolConfigurationDefinition Definition() => ToolConfigurationDefinition.Parse(
        Resource("IMVSContourMatchModuAlgorithmTab.xml"), Resource("ConfigurationGroups.xml"));

    public static IReadOnlyList<ToolDisplayObjectDefinition> DisplayObjects { get; } =
        ToolDisplayObjectDefinition.Parse(Resource("IMVSContourMatchModuDisplay.xml"));
}
