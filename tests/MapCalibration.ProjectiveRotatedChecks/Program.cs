using System.Text.Json;

static string Root()
{
    for (var current = new DirectoryInfo(Environment.CurrentDirectory); current is not null; current = current.Parent)
        if (File.Exists(Path.Combine(current.FullName, "docs/plugins/MAP-CALIBRATION.md"))) return current.FullName;
    throw new DirectoryNotFoundException("Run from within the tool-plugins worktree.");
}

var root = Root();
string Read(string relative) => File.ReadAllText(Path.Combine(root, relative));
void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

var nativeRefinement = Read("analysis/map-calibration/ghidra/core/1807e4360.c");
var nativeCaller = Read("analysis/map-calibration/ghidra/core/1807e3f50.c");
var managedMap = Read("src/VisionPlugins.MapCalibration/MapCalibrationImageSolver.cs");
var sharedRecognition = Read("src/VisionPlugins.Calibration/NImageDetectedBoard.cs");
var projective = Read("src/VisionPlugins.Calibration/NImageProjectiveLattice.cs");
var rotated = Read("src/VisionPlugins.Calibration/NImageRotatedBoardRecognition.cs");
var runtimePath = Path.Combine(root, "analysis/map-calibration/image-detector-stage-trace");
using var nativeComparison = JsonDocument.Parse(File.ReadAllText(Path.Combine(runtimePath, "comparison.json")));
var comparison = nativeComparison.RootElement;
var stageLines = File.ReadLines(Path.Combine(runtimePath, "stage-trace.jsonl")).Select(line => JsonDocument.Parse(line)).ToArray();
try
{
    Require(nativeRefinement.Contains("if (3 < *param_8)", StringComparison.Ordinal), "Native candidate-count gate changed.");
    Require(nativeRefinement.Contains("(iVar14 == 1) && (auStack_108._4_4_ == 1)", StringComparison.Ordinal), "Native pre-fit success/flag gate changed.");
    Require(nativeRefinement.Contains("_DAT_181628c24", StringComparison.Ordinal) && nativeRefinement.Contains("switch(iStack_ec)", StringComparison.Ordinal), "Native residual-gated dispatch changed.");
    Require(nativeRefinement.Contains("param_11[2]", StringComparison.Ordinal), "Native dispatch selector source changed.");
    Require(nativeRefinement.Contains("case 5:", StringComparison.Ordinal) && nativeRefinement.Contains("FUN_180806ed0", StringComparison.Ordinal), "Native specialized case-5 call changed.");
    Require(nativeRefinement.Contains("iStack_118 - 2U < 299", StringComparison.Ordinal) && nativeRefinement.Contains("iStack_114 - 2U < 299", StringComparison.Ordinal), "Native output-dimension gate changed.");
    Require(nativeCaller.Contains("FUN_1807e4360", StringComparison.Ordinal), "The image caller no longer reaches the refinement symbol.");

    Require(managedMap.Contains("return source??NImageBoardRecognition.Find", StringComparison.Ordinal), "Map compatibility fallback condition changed.");
    Require(sharedRecognition.Contains("FindUpright(image,boardType,cancellationToken,filterStatus)??FindProjective(image,boardType,cancellationToken,filterStatus)??NImageRotatedBoardRecognition.Find", StringComparison.Ordinal), "Shared recognition order changed.");
    Require(projective.Contains("image.Width<64||image.Height<64", StringComparison.Ordinal) && projective.Contains("pitchX<8||pitchY<8", StringComparison.Ordinal) && projective.Contains("raw.Count<4", StringComparison.Ordinal) && projective.Contains("bestComponent.Count<16", StringComparison.Ordinal), "Independent projective gates changed.");
    Require(rotated.Contains("new[]{0,.25,-.25,.5,-.5}", StringComparison.Ordinal) && rotated.Contains("Math.Abs(angle)<.001", StringComparison.Ordinal), "Independent rotated attempts changed.");

    var rows = comparison.GetProperty("stages").EnumerateArray().ToArray();
    Require(comparison.GetProperty("refinement7e4360Called").GetBoolean() == false, "Existing SDK probe no longer reports the expected non-hit.");
    Require(rows.Length == 4 && rows.All(row => row.GetProperty("count").GetInt32() == 168 && row.GetProperty("fields").GetInt32() == row.GetProperty("equal").GetInt32()), "Existing stage-probe output summary changed.");
    Require(rows.Select(row => row.GetProperty("board").GetInt32()).Order().SequenceEqual(new[] { 7, 7, 9, 9 }), "Existing stage-probe board coverage changed.");
    Require(stageLines.Length == 4 && stageLines.All(doc => doc.RootElement.GetProperty("phase").GetString() == "return"), "Existing stage trace rows changed.");
    var runtimeKeys = stageLines[0].RootElement.EnumerateObject().Select(property => property.Name).ToHashSet(StringComparer.Ordinal);
    Require(!runtimeKeys.Contains("selector") && !runtimeKeys.Contains("param_11_2") && !runtimeKeys.Contains("dispatchTarget"), "The stage trace unexpectedly gained selector/dispatch evidence; review this audit.");

    Console.WriteLine("PASS: native residual-gated refinement and bounded output are distinct from the Map source-failure fallback; existing four SDK captures did not hit refinement and contain no dispatch selector.");
    return 0;
}
finally
{
    foreach (var line in stageLines) line.Dispose();
}
