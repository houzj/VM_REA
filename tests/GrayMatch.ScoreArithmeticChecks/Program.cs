using System.Text.Json;
using VisionPlugins.GrayMatch;
if(args.Length!=1)throw new ArgumentException("Native scalar arithmetic oracle path required.");
using var doc=JsonDocument.Parse(File.ReadAllText(args[0]));int n=0;
double Read(JsonElement e,string key)=>BitConverter.UInt64BitsToDouble(Convert.ToUInt64(e.GetProperty(key).GetString(),16));
void Check(double value,string expected,string label){string bits=BitConverter.DoubleToUInt64Bits(value).ToString("x16");if(bits!=expected)throw new Exception($"{label}: {bits} != {expected}");Console.WriteLine($"PASS {++n}: {label} {bits}");}
Check(GrayMatchScoreArithmetic.SourceFloor,doc.RootElement.GetProperty("epsilonBits").GetString()!,"original PE constant");
foreach(var c in doc.RootElement.GetProperty("variance").EnumerateArray())Check(GrayMatchScoreArithmetic.Variance(Read(c,"sumBits"),Read(c,"squaresBits"),c.GetProperty("count").GetInt32()),c.GetProperty("resultBits").GetString()!,"native variance");
foreach(var c in doc.RootElement.GetProperty("score").EnumerateArray())Check(GrayMatchScoreArithmetic.PositiveSquaredScore(Read(c,"dotBits"),Read(c,"meanBits"),Read(c,"templateVarianceBits"),Read(c,"sumBits"),Read(c,"imageVarianceBits")),c.GetProperty("resultBits").GetString()!,"native scalar score");
Console.WriteLine($"PASS {n}; strict native scalar arithmetic only, not full matching parity");
