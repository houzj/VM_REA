using VisionEditor;

var directory = Path.Combine(Path.GetTempPath(), "vm-tool-catalog-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(directory);
try
{
    static void Put(string root, string relative, string content)
    {
        var path = Path.Combine(root, relative);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
    }
    static string Xml(string name, string priority, string extra = "") =>
        $"<ToolBoxItemData><name>{name}</name><priority>{priority}</priority>{extra}</ToolBoxItemData>";
    static void Require(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    Put(directory, "Collection/ToolGroup.xml", Xml("Collection", "2"));
    Put(directory, "Collection/ImageSourceModule/ToolItemInfo.xml", Xml("ImageSourceModule", "0", "<displayType>IMAGE</displayType>"));
    Put(directory, "Collection/SaveImage/ToolItemInfo.xml", Xml("SaveImage", "2"));
    Put(directory, "Collection/Invalid/ToolItemInfo.xml", "<wrong/>");
    Put(directory, "Collection/Entity/ToolItemInfo.xml", "<!DOCTYPE ToolBoxItemData [<!ENTITY value 'bad'>]><ToolBoxItemData><name>&value;</name></ToolBoxItemData>");
    Put(directory, "Calibration/ToolGroup.xml", Xml("Calibration", "3", "<priorityStrategyInfo>General:0.5;Other:40;General:1.5</priorityStrategyInfo>"));
    Put(directory, "UserTools/ToolGroup.xml", Xml("UserTools", "4"));
    Put(directory, "UserTools/MyGroup/ToolItemInfo.xml", Xml("MyGroup", "1"));
    Put(directory, "Global/ProcedureModule/ToolItemInfo.xml", Xml("ProcedureModule", "1"));
    Put(directory, "Global/GlobalCameraModule/ToolItemInfo.xml", Xml("GlobalCameraModule", "2"));
    Put(directory, "Global/OtherModule/ToolItemInfo.xml", Xml("OtherModule", "0"));

    var initial = ToolDirectoryCatalog.Discover(directory, "General");
    Require(initial.Diagnostics.Count == 0, "Deferred categories should not parse their tools before expansion.");
    Require(initial.Groups.Where(g => !g.IsGlobal).First().DirectoryName == "Calibration", "Strategy priority must affect category ordering.");
    Require(initial.Groups.Single(g => g.DirectoryName == "Calibration").Priority == 1.5,
        "Repeated matching strategy entries must use the last value, as ToolBoxData.ChangePriorityByInfo does.");
    Require(ToolDirectoryCatalog.Discover(directory, "Missing").Groups.Single(g => g.DirectoryName == "Calibration").Priority == 3,
        "An unmatched strategy must preserve the metadata priority.");
    Require(initial.Groups.Single(g => g.DirectoryName == "Collection").IsDeferred, "Ordinary category should defer tools.");
    Require(initial.Groups.Single(g => g.DirectoryName == "UserTools").Tools.Single().DirectoryName == "MyGroup", "User tools should load immediately.");
    Require(initial.Groups.Single(g => g.IsGlobal).Tools.Count == 2, "Global tools must use the source allowlist.");

    var expanded = ToolDirectoryCatalog.Expand(initial, "Collection");
    var collection = expanded.Groups.Single(g => g.DirectoryName == "Collection");
    Require(!collection.IsDeferred && collection.Tools.Select(t => t.DirectoryName).SequenceEqual(["ImageSourceModule", "SaveImage"]), "Expansion must read and sort available tools.");
    Require(collection.Tools[0].DisplayType == "IMAGE", "Display metadata should survive discovery.");
    Require(expanded.Diagnostics.Count == 2 && expanded.Diagnostics.Any(d => d.Path.EndsWith("Invalid\\ToolItemInfo.xml", StringComparison.OrdinalIgnoreCase)), "Malformed XML should be reported without hiding valid tools.");
    Require(ToolDirectoryCatalog.Expand(expanded, "Collection").Diagnostics.Count == 2, "Repeated expansion must not duplicate diagnostics.");
    Require(ToolDirectoryCatalog.Discover(Path.Combine(directory, "missing")).Diagnostics.Count == 1, "Missing module directory should produce a diagnostic.");

    Console.WriteLine("Tool directory catalog: 11 behavior checks passed.");
    if (args.Length > 0)
    {
        var installed = ToolDirectoryCatalog.Discover(args[0], "General");
        Console.WriteLine("Installed groups: " + string.Join(",", installed.Groups.Select(g => g.DirectoryName)));
        foreach (var diagnostic in installed.Diagnostics.Take(5)) Console.WriteLine($"Metadata diagnostic: {diagnostic.Path}: {diagnostic.Reason}");
        var collectionGroup = ToolDirectoryCatalog.Expand(installed, "Collection").Groups.Single(g => g.DirectoryName == "Collection");
        Require(collectionGroup.Tools.Any(t => t.DirectoryName == "ImageSourceModule" && t.DisplayType == "IMAGE"), "Installed image-source metadata differs from the expected source artifact.");
        Console.WriteLine($"Installed VM metadata: {installed.Groups.Count} groups; Collection {collectionGroup.Tools.Count} tools; {installed.Diagnostics.Count} initial diagnostics.");
    }
}
finally
{
    Directory.Delete(directory, recursive: true);
}
