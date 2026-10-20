using System.Security.Cryptography;
using System.Text.Json;

static string Root()
{
    for (var current = new DirectoryInfo(Environment.CurrentDirectory); current is not null; current = current.Parent)
        if (File.Exists(Path.Combine(current.FullName, "docs/plugins/MAP-CALIBRATION.md"))) return current.FullName;
    throw new DirectoryNotFoundException("Run from within the tool-plugins worktree.");
}

var root = Root();
string Read(string relative) => File.ReadAllText(Path.Combine(root, relative));
var nativeSearch = Read("analysis/map-calibration/ghidra/core/180813ca0.c");
var nativeMapping = Read("analysis/map-calibration/ghidra/core/180813020.c");
var nativeCaller7 = Read("analysis/map-calibration/ghidra/core/18080e690.c");
var nativeCaller9 = Read("analysis/map-calibration/ghidra/core/18080bcd0.c");
var currentRecognizer = Read("src/VisionPlugins.Calibration/NImageDetectedBoard.cs");
var tracePath = Path.Combine(root, "analysis/map-calibration/image-tag-trace/tag-trace.jsonl");
var trace = File.ReadAllLines(tracePath).Select(line => JsonDocument.Parse(line)).ToArray();
try
{
    var selected = trace.Select(d => d.RootElement).Where(e => e.GetProperty("phase").GetString() == "selected-tag").ToArray();
    var attempts = trace.Select(d => d.RootElement).Where(e => e.GetProperty("phase").GetString() == "decode-attempt").ToArray();
    var nativeStopEvidence = nativeSearch.Contains("if (iVar8 == 1) break;", StringComparison.Ordinal)
        && nativeSearch.Contains("if (*(int *)(puVar12 + 0x1e) == 1)", StringComparison.Ordinal)
        && nativeCaller7.Contains("FUN_180813ca0", StringComparison.Ordinal)
        && nativeCaller7.Contains("FUN_180813020", StringComparison.Ordinal)
        && nativeCaller9.Contains("FUN_180813ca0", StringComparison.Ordinal)
        && nativeCaller9.Contains("FUN_180813020", StringComparison.Ordinal);
    var nativeMappingEvidence = nativeMapping.Contains("-0x80000000", StringComparison.Ordinal)
        && nativeMapping.Contains("param_10", StringComparison.Ordinal)
        && nativeMapping.Contains("param_11", StringComparison.Ordinal);
    var capturedSingleSuccess = selected.Length == 4 && attempts.Length == 4
        && selected.All(e => e.GetProperty("status").GetInt32() == 1 && e.GetProperty("attempts").GetInt32() == 1)
        && attempts.All(e => e.GetProperty("statusBefore").GetInt32() == 0);
    var implementationScansAll = currentRecognizer.Contains("foreach(var quad in lattice.CodeQuadrilaterals)", StringComparison.Ordinal)
        && currentRecognizer.Contains("tags.Add(new(identity,quad,rings))", StringComparison.Ordinal)
        && currentRecognizer.Contains("!tags.Any(tag=>Inside(tag.Opening", StringComparison.Ordinal);
    if (!nativeStopEvidence || !nativeMappingEvidence || !capturedSingleSuccess || !implementationScansAll)
    {
        Console.Error.WriteLine($"Audit evidence changed: nativeStop={nativeStopEvidence}, mapping={nativeMappingEvidence}, captured={capturedSingleSuccess}, implementation={implementationScansAll}");
        return 1;
    }

    string BinaryHash(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();
    var report = new
    {
        status = "source-confirmed-boundary; multi-success-output-effect-unverified",
        originalModule = new { path = "D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll", sha256 = "85d17ecc87d52e9a62d8bb86c8d5208ac999BCD31EC6D0806453F8165733F549".ToLowerInvariant() },
        decompiledSymbols = new[]
        {
            new { address = "0x180813CA0", role = "search/opening proposal and payload decode", file = "analysis/map-calibration/ghidra/core/180813ca0.c", sha256 = BinaryHash(Path.Combine(root, "analysis/map-calibration/ghidra/core/180813ca0.c")) },
            new { address = "0x18080A7F0", role = "candidate opening/fiducial solve called by 813ca0", file = "analysis/map-calibration/ghidra/core/18080a7f0.c", sha256 = BinaryHash(Path.Combine(root, "analysis/map-calibration/ghidra/core/18080a7f0.c")) },
            new { address = "0x180813020", role = "selected payload to lattice point mapping", file = "analysis/map-calibration/ghidra/core/180813020.c", sha256 = BinaryHash(Path.Combine(root, "analysis/map-calibration/ghidra/core/180813020.c")) }
        },
        sourceCallOrder = new[] { "0x1807D3E90", "0x18080E690 (HIK7) / 0x18080BCD0 (HIK9)", "0x180813CA0", "0x18080A7F0 per proposal", "first successful payload stops search", "0x180813020 maps selected payload to points" },
        runtimeTrace = new { path = "analysis/map-calibration/image-tag-trace/tag-trace.jsonl", sha256 = BinaryHash(tracePath), nativeCalls = selected.Length, decodeAttempts = attempts.Length, eachCallOneAttemptAndSuccess = capturedSingleSuccess },
        independentImplementation = new { source = "src/VisionPlugins.Calibration/NImageDetectedBoard.cs", scansEveryLatticeCodeQuadrilateral = true, masksPointsInsideEveryDecodedOpening = true, sharesTheSelectedFirstIdentity = true },
        discrepancy = "Native stops the proposal search after the first successful payload; the current shared independent recognizer continues decoding every supplied quadrilateral and excludes points inside every successfully decoded opening. This can alter outputs only when multiple valid openings exist; the captured four SDK calls each have one successful attempt, so that output-level effect is not established here.",
        nativeStopEvidence,
        nativeMappingEvidence,
        capturedSingleSuccess,
        implementationScansAll,
        conclusion = "Do not change the shared Calibration recognizer from the Map-only ownership slice. A multi-success native/managed differential fixture and shared-feature ownership handoff are required before a behavior fix."
    };
    var destination = Path.Combine(root, "analysis/map-calibration/anchor-audit");
    Directory.CreateDirectory(destination);
    var reportPath = Path.Combine(destination, "source-boundary.json");
    File.WriteAllText(reportPath, JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
    Console.WriteLine($"Native stop/source-vs-implementation boundary verified; captured calls={selected.Length}; output effect with multiple valid openings remains unverified.");
    Console.WriteLine(reportPath);
    return 0;
}
finally
{
    foreach (var document in trace) document.Dispose();
}
