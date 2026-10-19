using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
if (args.Length != 1) { Console.Error.WriteLine("Usage: SolutionInspector <path-to-sol>"); return 2; }
try {
    string path = Path.GetFullPath(args[0]);
    using var file = File.OpenRead(path);
    string digest = Convert.ToHexString(SHA256.HashData(file)).ToLowerInvariant();
    file.Position = 0;
    using var archive = new ZipArchive(file, ZipArchiveMode.Read, leaveOpen: true);
    if (archive.Entries.Count > 100000) throw new InvalidDataException("Entry count exceeds limit.");
    var configs = archive.Entries.Where(e => e.FullName.Replace('\\', '/') == "SolutionFile/VmServer.xml").ToArray();
    if (configs.Length != 1) throw new InvalidDataException("Expected exactly one SolutionFile/VmServer.xml.");
    var config = configs[0];
    if (config.Length > 8388608) throw new InvalidDataException("XML exceeds inspection limit.");
    using var stream = config.Open();
    using var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null, MaxCharactersInDocument = 8388608 });
    var root = XDocument.Load(reader).Root ?? throw new InvalidDataException("Missing XML root.");
    if (root.Name != "Root") throw new InvalidDataException("Unrecognized root.");
    Dictionary<string,string> Attrs(XElement e) => e.Attributes().ToDictionary(a => a.Name.ToString(), a => a.Value);
    var modules = root.Element("ModulesInfo")?.Element("ModuleBase")?.Elements("Module").Select(Attrs).ToArray() ?? [];
    var procedures = root.Element("ProceduresInfo")?.Element("ProcedureBase")?.Elements("Procedure").Select(Attrs).ToArray() ?? [];
    var membership = root.Element("ProceduresInfo")?.Element("ProcedureInsideModules")?.Elements("Procedure").Select(p => new { procedure = (string?)p.Attribute("Index"), modules = p.Elements("Inside").Select(m => (string?)m.Attribute("Index")).ToArray() }).ToArray();
    var connections = root.Element("ModulesInfo")?.Element("ModuleCommonConnect")?.Elements("Module").SelectMany(m => m.Element("FollowingModules")?.Elements("Following").Select(f => new { from = (string?)m.Attribute("Index"), to = (string?)f.Attribute("Index") }) ?? []).ToArray();
    var subscriptions = root.Element("ModulesInfo")?.Element("ModuleSubscribe")?.Elements("Subscribe").Select(s => (string?)s.Attribute("Relation")).ToArray();
    Console.WriteLine(JsonSerializer.Serialize(new {
        schema = "vm-compat.solution-inspection.v1", path, sha256 = digest, bytes = file.Length,
        declaredVersion = (string?)root.Element("SoftVersion")?.Element("Version")?.Attribute("CurrentVersion"),
        solutionName = (string?)root.Element("SolutionInfo")?.Element("Solution")?.Attribute("SolutionName"),
        modules, procedures, membership, connections, subscriptions,
        semanticMap = SemanticMap.Build(root, archive.Entries.Where(e => e.FullName.Replace('\\', '/')
            .StartsWith("SolutionFile/UiParamData/", StringComparison.Ordinal)).Select(e => e.FullName)),
        moduleFrames = archive.Entries.Where(e => e.FullName.Replace((char)92, (char)47) == "SolutionFile/MoudleFrame").Select(ModuleFrame.Inspect).ToArray(),
        uiParameters = archive.Entries.Where(e => e.FullName.Replace((char)92, (char)47).StartsWith("SolutionFile/UiParamData/")).Select(UiParameters.Inspect).ToArray(),
        entries = archive.Entries.Select(e => new { path = e.FullName, bytes = e.Length, compressedBytes = e.CompressedLength }).ToArray(),
        limitations = new[] { "Read-only structural inspection; no module execution, SDK loading or extraction.", "VmServer.xml, observed UiParamData v1 and ModuleFrame v5 fixed tables are decoded; remaining payloads are opaque.", "Declared graph does not prove runtime ordering or parameter semantics.", "ZIP payload integrity outside selected XML is not verified.", "Serialization and round-trip compatibility are not implemented." }
    }, new JsonSerializerOptions { WriteIndented = true }));
    return 0;
} catch (Exception ex) when (ex is IOException or InvalidDataException or XmlException or UnauthorizedAccessException) {
    Console.Error.WriteLine("Inspection failed: " + ex.Message); return 1;
}


