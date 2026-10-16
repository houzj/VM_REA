using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using VisionCore;
using VisionIO;
namespace VisionWorkflow;
public sealed record OfflineCirclePlan(string Schema, string ImagePath, RadialCircleOptions Options);
public sealed record OfflineCircleRunResult(GrayImage Image, CircleDetectionResult Detection, string ImageSha256, double ElapsedMilliseconds);
public static class OfflinePlanStore
{
 public const string Schema = "vision-independent.circle-plan.v1";
 private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true, UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow };
 public static OfflineCirclePlan Load(string path)
 {
  string fullPath = Path.GetFullPath(path);
  using var document = JsonDocument.Parse(File.ReadAllText(fullPath));
  var root = document.RootElement;
  Require(root, "Schema", "ImagePath", "Options");
  Require(root.GetProperty("Options"), "CenterX", "CenterY", "InitialRadius", "SearchHalfWidth", "SampleCount", "GradientThreshold", "Polarity", "RadialStep", "GradientHalfSpan", "MinimumEdges");
  var plan = root.Deserialize<OfflineCirclePlan>(JsonOptions) ?? throw new InvalidDataException("Missing plan.");
  Validate(plan);
  return plan with { ImagePath = Path.GetFullPath(plan.ImagePath, Path.GetDirectoryName(fullPath)!) };
 }
 public static void Save(string path, OfflineCirclePlan plan)
 {
  Validate(plan);
  File.WriteAllText(path, JsonSerializer.Serialize(plan, JsonOptions));
 }
 internal static void Validate(OfflineCirclePlan plan)
 {
  ArgumentNullException.ThrowIfNull(plan);
  if (plan.Schema != Schema) throw new InvalidDataException("Unsupported independent plan schema.");
  if (string.IsNullOrWhiteSpace(plan.ImagePath)) throw new InvalidDataException("ImagePath is required.");
  if (plan.Options is null) throw new InvalidDataException("Explicit circle options are required.");
  RadialCircleDetector.ValidateOptions(plan.Options);
 }
 private static void Require(JsonElement element, params string[] names)
 {
  if (element.ValueKind != JsonValueKind.Object) throw new InvalidDataException("Expected JSON object.");
  var seen = new HashSet<string>(StringComparer.Ordinal);
  foreach (var p in element.EnumerateObject()) if (!seen.Add(p.Name)) throw new InvalidDataException("Duplicate property: " + p.Name);
  foreach (string name in names) if (!seen.Contains(name) || element.GetProperty(name).ValueKind == JsonValueKind.Null) throw new InvalidDataException("Missing explicit parameter: " + name);
 }
}
public static class OfflineCircleExecutor
{
 public static OfflineCircleRunResult Run(OfflineCirclePlan plan)
 {
  OfflinePlanStore.Validate(plan);
  var timer = Stopwatch.StartNew();
  using var stream = File.OpenRead(plan.ImagePath);
  string hash = Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
  stream.Position = 0;
  var image = BmpReader.Read(stream);
  var detection = RadialCircleDetector.Detect(image, plan.Options);
  timer.Stop();
  return new(image, detection, hash, timer.Elapsed.TotalMilliseconds);
 }
}

