using VisionPlugins.GrayMatch;

int count = 0;
void Check(bool value, string name) { if (!value) throw new Exception(name); Console.WriteLine($"PASS {++count}: {name}"); }
var a = new GrayMatchFloatLimit(1, 1f, 1f);
Check(a.Accept(1f), "closed float endpoints");
Check(a.Accept(MathF.BitDecrement(1f)), "source epsilon includes one float step below one");
Check(a.Accept(MathF.BitIncrement(1f)), "source epsilon includes one float step above one");
Check(!a.Accept(MathF.BitIncrement(MathF.BitIncrement(1f))), "second positive step outside source epsilon");
Check(a.Accept(float.NaN), "unordered COMISS branch accepts NaN");
Check(new GrayMatchFloatLimit(1, float.NaN, float.NaN).Accept(12f), "unordered limits accept ordinary value");
Check(new GrayMatchFloatLimit(2, 0f, 1f).Accept(12f), "Enable exactly one only");
Check(!new GrayMatchIntegerLimit(1, -1, 3).Accept(4), "signed count upper gate");
Check(new GrayMatchIntegerLimit(1, -1, 3).Accept(-1), "signed count lower inclusive");
var off = new GrayMatchFloatLimit(0, 0, 0);
var limits = new GrayMatchLimits(new(1, 1, 1), off, new(1, .5f, 1f), off, off, off, off);
float[] good = new float[11]; good[10] = .75f;
float[] bad = new float[11]; bad[10] = .1f;
var result = limits.Apply(1, 2, new[] { good, bad });
Check(result.ModuleStatus == 0 && result.MatchStatus.SequenceEqual(new uint[] { 1, 0 }), "full aggregate count and per-record score combine before output truncation");
var zero = limits.Apply(1, 0, Array.Empty<float[]>());
Check(zero.ModuleStatus == 1 && zero.MatchStatus.Length == 0, "zero count preserves upstream OKWhenNumIsZero status");
var rejected = limits.Apply(0, 1, new[] { good });
Check(rejected.ModuleStatus == 0 && rejected.MatchStatus[0] == 1, "upstream failure remains while individual source check can pass");
bool mismatch = false;
try { limits.Apply(1, 2, new[] { good }); } catch (ArgumentException) { mismatch = true; }
Check(mismatch, "independent malformed-count safety rejection");
Console.WriteLine($"PASS {count}; source-formula candidate checks, not native execution parity or full-tool validation");
