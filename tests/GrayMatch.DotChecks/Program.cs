using System.Text.Json;
using VisionPlugins.GrayMatch;

if (args.Length != 1) throw new ArgumentException("Expected exact native oracle-final.json path.");
using var doc = JsonDocument.Parse(File.ReadAllText(args[0]));
if (!doc.RootElement.GetProperty("avx2OsAvailable").GetBoolean())
    throw new InvalidOperationException("This oracle did not execute the AVX backend.");
int n = 0;
void Check(bool p, string text) { if (!p) throw new Exception(text); Console.WriteLine($"PASS {++n}: {text}"); }
foreach (var item in doc.RootElement.GetProperty("cases").EnumerateArray())
{
    int width = item.GetProperty("width").GetInt32(), count = item.GetProperty("rows").GetInt32();
    var a = Enumerable.Repeat((byte)item.GetProperty("fill").GetInt32(), width + 128).ToArray();
    var rows = Enumerable.Repeat(new GrayMatchDotRow(0, 0, width), count).ToArray();
    foreach (var (backend, key) in new[] {
        (GrayMatchDotBackend.ScalarUnsignedTotal, "scalarBits"),
        (GrayMatchDotBackend.SseUnsignedTotal, "sseBits"),
        (GrayMatchDotBackend.AvxDoubleRows, "avxBits") })
    {
        double value = GrayMatchDotKernel.Compute(a, a, rows, backend);
        string bits = BitConverter.DoubleToUInt64Bits(value).ToString("x16");
        Check(bits == item.GetProperty(key).GetString(), $"native {key} width={width} rows={count} {bits}");
    }
}
Check(GrayMatchDotKernel.Select(11, 32 * 17, 17) == GrayMatchDotBackend.SseUnsignedTotal, "CPU11 ratio32 source boundary");
Check(GrayMatchDotKernel.Select(11, 33 * 17, 17) == GrayMatchDotBackend.AvxDoubleRows, "CPU11 ratio33 source boundary");
Check(GrayMatchDotKernel.Select(7, 1000, 17) == GrayMatchDotBackend.SseUnsignedTotal, "CPU7 does not choose class11 large backend");
Check(GrayMatchDotKernel.Select(10, 1000, 17) == GrayMatchDotBackend.ScalarUnsignedTotal, "other CPU class keeps scalar source branch");
bool invalid = false;
try { GrayMatchDotKernel.Compute([1], [1], [new(0, 0, 2)], GrayMatchDotBackend.ScalarUnsignedTotal); }
catch (ArgumentOutOfRangeException) { invalid = true; }
Check(invalid, "independent managed invalid-range rejection");
Console.WriteLine($"PASS {n}; exact same-hash native arithmetic oracle only, not template matching or full-tool parity");
