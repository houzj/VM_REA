using System.Text.Json;
using VisionPlugins.GrayMatch;
if (args.Length != 1) throw new ArgumentException("Exact angle-native oracle.json required.");
using var doc = JsonDocument.Parse(File.ReadAllText(args[0]));
int checks = 0;
void Check(bool ok, string name) { if (!ok) throw new Exception(name); Console.WriteLine($"PASS {++checks}: {name}"); }
foreach (var c in doc.RootElement.GetProperty("cases").EnumerateArray())
{
    var angles = c.GetProperty("angleBits").EnumerateArray().Select(x => BitConverter.UInt32BitsToSingle(Convert.ToUInt32(x.GetString(),16))).ToArray();
    var expected = c.GetProperty("groups").EnumerateArray().Select(x => x.EnumerateArray().Select(y => y.GetInt32()).ToArray()).ToArray();
    var actual = GrayMatchTrainingAngles.Group(angles);
    Check(actual.Count == expected.Length, "original group count");
    for (int i = 0; i < expected.Length; i++) Check(actual[i].SequenceEqual(expected[i]), $"strict original ordered angle indices group{i}");
}
foreach (var bad in new[] { Array.Empty<float>(), new[] { float.NaN }, new[] { 1f, 0f } })
{
    bool rejected = false;
    try { GrayMatchTrainingAngles.Group(bad); } catch (ArgumentException) { rejected = true; }
    Check(rejected, "independent bounded API safety rejection, not native bad-table parity");
}
Console.WriteLine($"PASS {checks}; scalar training angle grouping only, not trained model or complete matching");
