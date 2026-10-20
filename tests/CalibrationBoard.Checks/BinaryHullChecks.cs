using System.IO;
using System.Text.Json;
using VisionPlugins.CalibrationBoard;

static class BinaryHullChecks
{
 public static void Run(Action<bool,string> check)
 {
  var report=new List<object>();var failures=new List<int>();
  foreach(string line in File.ReadLines("docs/plugins/calibration-board-binary-hull-fixtures.jsonl"))
  {
   using var document=JsonDocument.Parse(line);var row=document.RootElement;int width=row.GetProperty("width").GetInt32(),height=row.GetProperty("height").GetInt32(),index=row.GetProperty("index").GetInt32();
   var pixels=new byte[width*height];foreach(var p in row.GetProperty("foreground").EnumerateArray())pixels[p.GetInt32()]=255;
   var hull=CalibrationBoardBinaryHull.Find(pixels,width,height,width);
   if(!pixels.Any(p=>p!=0)){check(hull.Count==0,"empty binary API is explicit; native stale-scratch result is not reproduced");continue;}
   bool points=hull.SequenceEqual(row.GetProperty("hull").EnumerateArray().Select(p=>(p[0].GetInt32(),p[1].GetInt32())));
   var circle=CalibrationBoardEnclosingCircle.Find(hull);uint[] bits=[BitConverter.SingleToUInt32Bits(circle.X),BitConverter.SingleToUInt32Bits(circle.Y),BitConverter.SingleToUInt32Bits(circle.Radius)];
   bool same=bits.SequenceEqual(row.GetProperty("fallbackBits").EnumerateArray().Select(p=>p.GetUInt32()));
   if(!points||!same)failures.Add(index);report.Add(new{index,hullExact=points,fallbackBitsExact=same,hull=hull.Select(p=>new[]{p.X,p.Y}),circle,bits});
  }
  File.WriteAllText("analysis/calibration-board/binary-hull-differences.json",JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));
  check(failures.Count==0,"source binary hull/fallback order and float bits: "+string.Join(",",failures));
 }
}
