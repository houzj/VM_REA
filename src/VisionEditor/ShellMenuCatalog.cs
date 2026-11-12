using System.IO;
using System.Xml.Linq;

namespace VisionEditor;

/// <summary>
/// 原厂菜单节点。字段与 menuitems.xml 的 <c>TitleMenueItemData</c> /
/// <c>ExternTitleMenueItemData</c> 元素一一对应。
/// </summary>
internal sealed record ShellMenuNode(
    string Key,
    string? MethodName,
    string? Info,
    string? InitMethodName,
    string? GroupName,
    int GroupIndex,
    int Priority,
    bool IsShowSeparator,
    bool IsDisplayInMenu,
    bool IsVisible,
    bool IsEnableByRun,
    bool IsGroupData,
    string? DynamicItems,
    string? ImageEnable,
    string? ImageDisable,
    IReadOnlyList<ShellMenuNode> Children)
{
    internal bool IsToolbarItem => GroupName is not null;

    internal bool IsExternalTool => Info is not null;

    /// <summary>原厂 <c>%中文%</c> 这类占位名表示按字面显示，去掉百分号标记。</summary>
    internal string CaptionKey => Key.StartsWith('%') && Key.EndsWith('%') && Key.Length > 2 ? Key[1..^1] : Key;
}

/// <summary>
/// menuitems.xml 解析结果。文件为原厂原件逐字节副本
/// （src/VisionEditor/Resources/shell-menuitems.xml，SHA-256 D9E9A1DA…6E7959）。
/// 结构：根 TitleMenueItemData → items → 5 个一级菜单 + 1 个 isDisplayInMenu=false 的工具栏命令组。
/// </summary>
internal sealed class ShellMenuCatalog
{
    internal const int ExpectedNodeElements = 50;
    internal const int ExpectedExternalTools = 5;
    /// <summary>39 个内部命令 + 3 个外部启动命令（OpenExe / OpenExeByLang / OpenModuleParamTool）。</summary>
    internal const int ExpectedMethods = 42;
    internal const int ExpectedToolbarItems = 15;
    internal const int ExpectedMenuRoots = 5;
    internal const int ExpectedToolbarGroups = 4;

    ShellMenuCatalog(
        int nodeElements,
        IReadOnlyList<ShellMenuNode> menus,
        IReadOnlyList<ShellMenuNode> toolbarItems,
        IReadOnlyList<string> methods,
        IReadOnlyList<ShellMenuNode> allNodes)
    {
        NodeElements = nodeElements;
        Menus = menus;
        ToolbarItems = toolbarItems;
        Methods = methods;
        AllNodes = allNodes;
    }

    internal int NodeElements { get; }

    internal IReadOnlyList<ShellMenuNode> Menus { get; }

    internal IReadOnlyList<ShellMenuNode> ToolbarItems { get; }

    internal IReadOnlyList<string> Methods { get; }

    internal IReadOnlyList<ShellMenuNode> AllNodes { get; }

    /// <summary>输出根目录优先，其次 Resources 子目录（兼容未加 Link 的构建）。</summary>
    internal static string DefaultPath
    {
        get
        {
            var root = Path.Combine(AppContext.BaseDirectory, "shell-menuitems.xml");
            if (File.Exists(root)) return root;
            var nested = Path.Combine(AppContext.BaseDirectory, "Resources", "shell-menuitems.xml");
            return File.Exists(nested) ? nested : root;
        }
    }

    internal static ShellMenuCatalog LoadDefault()
    {
        var path = DefaultPath;
        if (!File.Exists(path)) throw new FileNotFoundException("原厂菜单数据缺失：shell-menuitems.xml", path);
        return Load(path);
    }

    internal static ShellMenuCatalog Load(string path)
    {
        var document = XDocument.Load(path);
        var root = document.Root ?? throw new InvalidDataException("menuitems.xml 缺少根节点");
        var roots = Children(root);
        var all = new List<ShellMenuNode>();
        foreach (var element in roots) Collect(element, all);

        var menus = roots
            .Where(element => element.Name.LocalName == "TitleMenueItemData")
            .Where(element => element.Element("name") is not null && !IsFalse(element, "isDisplayInMenu"))
            .Select(element => Map(element))
            .ToArray();

        var toolbarItems = all
            .Where(node => node.IsToolbarItem)
            .OrderBy(node => node.GroupIndex)
            .ThenBy(node => node.Priority)
            .ToArray();

        var methods = all
            .Select(node => node.MethodName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        return new ShellMenuCatalog(CountElements(root), menus, toolbarItems, methods, all);
    }

    /// <summary>完整性自检：节点数、命令数、一级菜单数、工具栏项与分组数必须与原厂一致。</summary>
    internal void Verify()
    {
        var problems = new List<string>();
        if (NodeElements != ExpectedNodeElements) problems.Add($"节点元素 {NodeElements} ≠ {ExpectedNodeElements}");
        if (Methods.Count != ExpectedMethods) problems.Add($"命令数 {Methods.Count} ≠ {ExpectedMethods}");
        if (ToolbarItems.Count != ExpectedToolbarItems) problems.Add($"工具栏项 {ToolbarItems.Count} ≠ {ExpectedToolbarItems}");
        if (Menus.Count != ExpectedMenuRoots) problems.Add($"一级菜单 {Menus.Count} ≠ {ExpectedMenuRoots}");
        var groups = ToolbarItems.Select(item => item.GroupIndex).Distinct().Count();
        if (groups != ExpectedToolbarGroups) problems.Add($"工具栏分组 {groups} ≠ {ExpectedToolbarGroups}");
        if (ExternalToolCount != ExpectedExternalTools) problems.Add($"外部工具 {ExternalToolCount} ≠ {ExpectedExternalTools}");
        if (problems.Count > 0) throw new InvalidDataException("menuitems.xml 完整性校验失败：" + string.Join("；", problems));
    }

    static int CountElements(XElement root) => root.DescendantsAndSelf().Count(element => element.Name.LocalName == "TitleMenueItemData");

    internal int ExternalToolCount => AllNodes.Count(node => node.IsExternalTool);

    static void Collect(XElement element, List<ShellMenuNode> sink)
    {
        sink.Add(Map(element));
        foreach (var child in Children(element)) Collect(child, sink);
    }

    static IEnumerable<XElement> Children(XElement element) =>
        element.Element("items")?.Elements()
            .Where(child => child.Name.LocalName is "TitleMenueItemData" or "ExternTitleMenueItemData") ?? [];

    static ShellMenuNode Map(XElement element)
    {
        var groupName = element.Element("toolGroupItemInfo")?.Element("groupName")?.Value;
        return new ShellMenuNode(
            Key: element.Element("name")?.Value ?? string.Empty,
            MethodName: element.Element("methodName")?.Value,
            Info: element.Element("info")?.Value,
            InitMethodName: element.Element("initMethodName")?.Value,
            GroupName: groupName,
            GroupIndex: ParseGroupIndex(groupName),
            Priority: ParseInt(element.Element("toolGroupItemInfo")?.Element("priority")?.Value),
            IsShowSeparator: IsTrue(element, "isShowSeparator"),
            IsDisplayInMenu: !IsFalse(element, "isDisplayInMenu"),
            IsVisible: !IsFalse(element, "isVisible"),
            IsEnableByRun: IsTrue(element, "isEnableByRun"),
            IsGroupData: IsTrue(element, "isGroupData"),
            DynamicItems: element.Element("dynamicGetItemsMethodName")?.Value,
            ImageEnable: element.Element("imageEnablePath")?.Value,
            ImageDisable: element.Element("imageDisablePath")?.Value,
            Children: Children(element).Select(Map).ToArray());
    }

    static int ParseGroupIndex(string? groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName)) return int.MaxValue;
        var digits = new string(groupName.Where(char.IsDigit).ToArray());
        return int.TryParse(digits, out var index) ? index : int.MaxValue;
    }

    static int ParseInt(string? value) => int.TryParse(value, out var parsed) ? parsed : int.MaxValue;

    static bool IsTrue(XElement element, string name) => string.Equals(element.Element(name)?.Value?.Trim(), "true", StringComparison.OrdinalIgnoreCase);

    static bool IsFalse(XElement element, string name) => string.Equals(element.Element(name)?.Value?.Trim(), "false", StringComparison.OrdinalIgnoreCase);
}
