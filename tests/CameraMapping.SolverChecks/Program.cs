using System.Globalization;
using System.Text.Json;
using VisionPlugins.CameraMapping;
var source=args.Length>0?args[0]:"analysis/camera-mapping/native";var text=File.ReadAllLines(Path.Combine(source,"cases.txt"));
var native=File.ReadAllLines(Path.Combine(source,"results.jsonl")).Select(s=>JsonDocument.Parse(s).RootElement.Clone()).ToDictionary(x=>x.GetProperty("name").GetString()!);
var reports=new List<object>();int pass=0,fail=0;
for(int cursor=0;cursor<text.Length;){var h=text[cursor++].Split(' ');string name=h[0];int f=int.Parse(h[1]),w=int.Parse(h[2]),factor=int.Parse(h[3]),count=int.Parse(h[4]);var points=new List<CameraMappingPair>();for(int i=0;i<count;i++){var v=text[cursor++].Split(' ').Select(x=>double.Parse(x,CultureInfo.InvariantCulture)).ToArray();points.Add(new(v[0],v[1],v[2],v[3]));}
 var result=CameraMappingSolver.Solve(points,f,w,factor);var original=native[name];double? delta=null;bool match;
 if(original.TryGetProperty("exception",out var error))match=result.ErrorCode==error.GetUInt32();
 else if(result.Success){var matrix=original.GetProperty("matrix").EnumerateArray().Select(v=>v.GetDouble()).ToArray();delta=matrix.Zip(result.Matrix,(a,b)=>Math.Abs(a-b)).Max();var ratio=original.GetProperty("ratio").EnumerateArray().Select(v=>v.GetDouble()).ToArray();delta=Math.Max(delta.Value,Math.Max(Math.Abs(ratio[0]-result.RatioX),Math.Abs(ratio[1]-result.RatioY)));match=delta<0.003;}
 else match=false;
 if(match)pass++;else fail++;reports.Add(new{name,match,maxAbsoluteMatrixOrRatioDelta=delta,managedError=result.ErrorCode,managedMatrix=result.Matrix});
}
Directory.CreateDirectory("analysis/camera-mapping/managed");File.WriteAllText("analysis/camera-mapping/managed/comparison.json",JsonSerializer.Serialize(new{pass,fail,tolerance=.003,originalDesktopCompared=false,comparisons=reports},new JsonSerializerOptions{WriteIndented=true}));
Console.WriteLine($"Camera mapping SDK differential: {pass} matching, {fail} mismatching (0.003 absolute matrix/ratio tolerance). See comparison.json; numeric match is not UI parity.");
// Report differences without disguising them as passing assertions.
return fail==0?0:1;
