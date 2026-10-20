using System.IO;
using System.Text.Json;
using VisionFramework;
using VisionPlugins.CameraMapping;

var directory = Path.GetFullPath("analysis/camera-mapping/reachable-matrix-20261019");
using var casesDocument = JsonDocument.Parse(File.ReadAllText(Path.Combine(directory, "cases.json")));
var nativeFrames = File.ReadLines(Path.Combine(directory, "module-results.jsonl"))
    .Select(line => JsonDocument.Parse(line).RootElement.Clone())
    .Where(frame => frame.TryGetProperty("phase", out var phase) && phase.GetString()!.StartsWith("seed-", StringComparison.Ordinal))
    .ToDictionary(frame => frame.GetProperty("phase").GetString()!);
var reports = new List<object>();
int success = 0, mismatch = 0, fields = 0, fourPoint = 0, sixteenPoint = 0;
foreach (var fixture in casesDocument.RootElement.EnumerateArray()) {
    string name = fixture.GetProperty("case").GetString()!;
    var points = fixture.GetProperty("points").EnumerateArray().Select(row => row.EnumerateArray().Select(x => x.GetDouble()).ToArray()).ToArray();
    var plugin = new CameraMappingPlugin();
    var values = plugin.Descriptor.Parameters.ToDictionary(x => x.Name, x => x.DefaultValue!.Value.Clone());
    values["Data1"] = JsonSerializer.SerializeToElement("Data1#1");
    for (int group = 3; group <= 8; group++) values["Data" + group] = JsonSerializer.SerializeToElement("Data" + group + "#0");
    var input = new Dictionary<string, PortValue>();
    for (int group = 1; group <= 2; group++) {
        var half = points.Skip((group - 1) * (points.Length / 2)).Take(points.Length / 2).ToArray();
        input["ObjectPointX" + group] = PortValue.Numbers(half.Select(p => p[0]).ToArray());
        input["ObjectPointY" + group] = PortValue.Numbers(half.Select(p => p[1]).ToArray());
        input["TargetPointX" + group] = PortValue.Numbers(half.Select(p => p[2]).ToArray());
        input["TargetPointY" + group] = PortValue.Numbers(half.Select(p => p[3]).ToArray());
    }
    using var scope = new ModuleExecutionScope();
    var result = plugin.CreateRuntime().ExecuteAsync(new(name, directory, input, values) { Scope = scope }, default).GetAwaiter().GetResult();
    var deltas = new Dictionary<string, double>();
    var outputs = new Dictionary<string, double>();
    string? firstMismatch = null;
    if (result.Status == ModuleStatus.Success) {
        success++;
        if (points.Length == 4) fourPoint++; else if (points.Length == 16) sixteenPoint++;
        foreach (var item in nativeFrames[name].GetProperty("writes").EnumerateArray()) {
            string field = item.GetProperty("name").GetString()!;
            double expected = item.GetProperty("values")[0].GetDouble();
            double actual = result.Outputs[field].Kind == DataKind.Integer ? result.Outputs[field].As<long>() : result.Outputs[field].As<double>();
            fields++;
            double delta = Math.Abs(actual - expected);
            outputs[field] = actual;
            deltas[field] = delta;
            if (!(delta < 0.003) && firstMismatch is null) firstMismatch = field;
        }
    }
    bool compared = result.Status == ModuleStatus.Success && firstMismatch is null;
    if (!compared) mismatch++;
    reports.Add(new { name, pointCount = points.Length, groupCount = 2, status = result.Status.ToString(), result.NativeErrorCode, outputs, absoluteDelta = deltas, firstMismatch, originalComparisonPassed = compared });
}
var summary = new { scope = "Actual CameraMappingPlugin.CreateRuntime().ExecuteAsync, current project reference, two fixed default active groups of float arrays; no WPF window or platform/device", cases = reports.Count, success, fourPoint, sixteenPoint, comparedFields = fields, mismatch, tolerance = 0.003, passed = success == reports.Count && mismatch == 0 };
var options = new JsonSerializerOptions { WriteIndented = true };
var reportPath=args.Length>0?Path.GetFullPath(args[0]):Path.Combine(directory, "actual-plugin-comparison.json");
File.WriteAllText(reportPath, JsonSerializer.Serialize(new { summary, reports }, options));
Console.WriteLine(JsonSerializer.Serialize(summary, options));
return summary.passed ? 0 : 1;
