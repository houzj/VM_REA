using System.Text.Json;
using VisionPlugins.GrayMatch;
if (args.Length != 2) throw new ArgumentException("Exact pyramid-native and pyramid-chain-native oracle.json paths required.");
using var doc = JsonDocument.Parse(File.ReadAllText(args[0]));
var cases = doc.RootElement.GetProperty("cases").EnumerateArray().ToArray();
int count = 0;
void Check(bool value, string name) { if (!value) throw new Exception(name); Console.WriteLine($"PASS {++count}: {name}"); }
foreach (var c in cases)
{
    int w = c.GetProperty("w").GetInt32(), h = c.GetProperty("h").GetInt32(), s = c.GetProperty("stride").GetInt32();
    var input = c.GetProperty("input").EnumerateArray().Select(x => x.GetByte()).ToArray();
    // c97090 copies tight before sampling: compare to tight original leaf layout,
    // never to padded leaf output when odd width differs.
    var tight = cases.Single(x => x.GetProperty("w").GetInt32() == w && x.GetProperty("h").GetInt32() == h && x.GetProperty("stride").GetInt32() == w);
    foreach (bool mask in new[] { false, true })
    {
        var p = GrayMatchTrainingPyramid.Build(input, w, h, s, 2, mask);
        if (w == 3) { Check(p.Count == 1, "caller min-area32 prevents small-image lower level"); continue; }
        Check(p.Count == 2, "caller permits requested two levels");
        var expected = tight.GetProperty("outputs")[mask ? 2 : 0].GetProperty("bytes").EnumerateArray().Select(x => x.GetByte()).ToArray();
        Check(p[1].Pixels.SequenceEqual(expected), $"original tight leaf bytes after full caller copy mask={mask} {w}x{h} stride{s}");
        Check(p[0].SourceRelativeOffset == 0x80 && p[1].SourceRelativeOffset == 0x80 + ((w * h + 127) & ~127), "source1390 aligned relative payload offsets");
    }
}
bool rejectedRawZero = false;
try { GrayMatchTrainingPyramid.Build(new byte[65 * 65], 65, 65, 65, 0, false); }
catch (ArgumentException) { rejectedRawZero = true; }
Check(rejectedRawZero, "normalized-only API rejects raw0; original training wrapper must map0 to8");
using var chains = JsonDocument.Parse(File.ReadAllText(args[1]));
foreach (var chain in chains.RootElement.GetProperty("chains").EnumerateArray())
{
    int kernel = chain.GetProperty("kernel").GetInt32();
    var original = chain.GetProperty("levels").EnumerateArray().ToArray();
    var input = original[0].GetProperty("bytes").EnumerateArray().Select(x => x.GetByte()).ToArray();
    var actual = GrayMatchTrainingPyramid.Build(input, 65, 65, 65, 8, kernel == 2);
    Check(actual.Count == original.Length && actual.Count == 4, "original65 tight chain depth capped at9x9");
    for (int i = 0; i < original.Length; i++)
    {
        Check(actual[i].Width == original[i].GetProperty("w").GetInt32() && actual[i].Height == original[i].GetProperty("h").GetInt32(), "original multi-level dimensions");
        Check(actual[i].Pixels.SequenceEqual(original[i].GetProperty("bytes").EnumerateArray().Select(x => x.GetByte())), $"strict original multi-level bytes kernel{kernel} level{i}");
    }
}
Console.WriteLine($"PASS {count}; candidate caller pyramid only, not complete training/search or vendor model bytes");
