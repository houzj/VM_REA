using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace VisionPluginContracts;

/// <summary>Source-preserving configuration metadata, separate from execution ports and parameters.</summary>
public sealed class ToolConfigurationDefinition
{
    public string SourceXml { get; }
    public string SourceSha256 { get; }
    public string? GroupsSourceSha256 { get; }
    public ConfigurationElement Declaration { get; }
    public ConfigurationElement Root { get; }
    public IReadOnlyList<ConfigurationElement> Tabs => Section("Tabs");
    public IReadOnlyList<ConfigurationElement> BottomCommands => Section("BottomExcuteButtons");
    public IReadOnlyList<ConfigurationElement> LeftBottomCommands => Section("BottomExcuteButtonsLeft");
    public IReadOnlyList<ConfigurationElement> ExternParams => Section("ExternParamsList");
    private ToolConfigurationDefinition(string xml, string? groups, ConfigurationElement declaration, ConfigurationElement root)
    {
        SourceXml = xml; SourceSha256 = Hash(xml); GroupsSourceSha256 = groups is null ? null : Hash(groups);
        Declaration = declaration; Root = root;
    }
    private IReadOnlyList<ConfigurationElement> Section(string name) => Root.Children.FirstOrDefault(c => c.Tag == name)?.Children ?? Array.Empty<ConfigurationElement>();
    public static ToolConfigurationDefinition Parse(string xml, string? groupsXml = null)
    {
        ArgumentNullException.ThrowIfNull(xml);
        var raw = Load(xml);
        if (raw.Name.LocalName != "AlgorithmTabRoot") throw new InvalidDataException("Expected AlgorithmTabRoot.");
        var groups = groupsXml is null ? new Dictionary<string, XElement>() : Load(groupsXml).Descendants("GroupInfoItem")
            .GroupBy(e => (string?)e.Attribute("Name") ?? throw new InvalidDataException("Group without Name."), StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);
        var declaration = Build(raw, null, "definition", Array.Empty<string>(), false, groups);
        var root = Build(raw, null, "definition", Array.Empty<string>(), true, groups);
        return new(xml, groupsXml, declaration, root);
    }
    private static XElement Load(string xml)
    {
        try
        {
            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null });
            return XDocument.Load(reader, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo).Root ?? throw new InvalidDataException("Missing XML root.");
        }
        catch (XmlException ex) { throw new InvalidDataException("Invalid configuration XML.", ex); }
    }
    private static ConfigurationElement Build(XElement xml, ConfigurationElement? parent, string source, IReadOnlyList<string> chain, bool resolve, Dictionary<string, XElement> groups)
    {
        var item = new ConfigurationElement(xml, parent, source, chain);
        var children = new List<ConfigurationElement>();
        foreach (var child in xml.Elements())
        {
            if (!resolve || child.Name.LocalName != "GroupLinkItem") { children.Add(Build(child, item, source, chain, resolve, groups)); continue; }
            var link = child.Element("LinkName")?.Value ?? "";
            if (chain.Contains(link, StringComparer.Ordinal)) throw new InvalidDataException("Cyclic GroupLinkItem: " + string.Join(" -> ", chain.Append(link)));
            if (!groups.TryGetValue(link, out var group)) throw new InvalidDataException("Unknown GroupLinkItem: " + link);
            var next = chain.Append(link).ToArray();
            // GroupLinkItem.AfterAddedToArray clones GroupInfoItem and reparents each Items child to the referencing parent.
            foreach (var linked in group.Element("Items")?.Elements() ?? Enumerable.Empty<XElement>())
            {
                // Use a temporary holder to resolve nested links without making the group a visible container.
                var holder = Build(new XElement("Items", new XElement(linked)), null, "group:" + link, next, true, groups);
                foreach (var expanded in holder.Children) { expanded.Parent = item; children.Add(expanded); }
            }
        }
        item.SetChildren(children); return item;
    }
    public ConfigurationGeometry ResolveGeometry(string language)
    {
        double Read(string name, double fallback = 0) => double.TryParse(Declaration.Children.FirstOrDefault(c => c.Tag == name)?.Text ?? Declaration.Attributes.GetValueOrDefault(name), NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : fallback;
        var height = Read("CustomTabHeight", 500);
        bool english = language == "English";
        var specificWidth = Read(english ? "ENCustomTabWidth" : "ZHCustomTabWidth");
        var width = specificWidth != 0 ? specificWidth : Read(english ? "CustomTabENWidth" : "CustomTabCHWidth", english ? 680 : 420);
        if (specificWidth != 0)
        {
            var specificHeight = Read(english ? "ENCustomTabHeight" : "ZHCustomTabHeight");
            if (specificHeight != 0) height = specificHeight;
        }
        return new(width, height);
    }
    private static string Hash(string text) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text))).ToLowerInvariant();
}

public sealed record ConfigurationGeometry(double Width, double Height);
public sealed class ConfigurationElement
{
    public string Tag { get; }
    public string? Name => Attributes.GetValueOrDefault("Name");
    public IReadOnlyDictionary<string, string> Attributes { get; }
    public string Text { get; }
    public string OriginalXml { get; }
    public string Source { get; }
    public IReadOnlyList<string> ReferenceChain { get; }
    public ConfigurationElement? Parent { get; internal set; }
    public IReadOnlyList<ConfigurationElement> Children { get; private set; } = Array.Empty<ConfigurationElement>();
    internal ConfigurationElement(XElement xml, ConfigurationElement? parent, string source, IReadOnlyList<string> chain)
    {
        Tag = xml.Name.LocalName; Attributes = new ReadOnlyDictionary<string,string>(xml.Attributes().ToDictionary(a => a.Name.ToString(), a => a.Value));
        Text = string.Concat(xml.Nodes().OfType<XText>().Select(t => t.Value)); OriginalXml = xml.ToString(SaveOptions.DisableFormatting);
        Parent = parent; Source = source; ReferenceChain = Array.AsReadOnly(chain.ToArray());
    }
    internal void SetChildren(List<ConfigurationElement> children) => Children = children.AsReadOnly();
}
