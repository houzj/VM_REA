using System.IO;
using System.Text.Json;
using VisionPlugins.CalibrationBoard;

static class EnclosingCircleChecks
{
 public static void Run(Action<bool,string> check)
 {
  var failures=new List<int>();var report=new List<object>();
  foreach(string line in File.ReadLines("docs/plugins/calibration-board-enclosing-circle-fixtures.jsonl"))
  {
   using var document=JsonDocument.Parse(line);var row=document.RootElement;
   var points=row.GetProperty("points").EnumerateArray().Select(p=>(p[0].GetInt32(),p[1].GetInt32())).ToArray();
   var actual=CalibrationBoardEnclosingCircle.Find(points);uint[] bits=[BitConverter.SingleToUInt32Bits(actual.X),BitConverter.SingleToUInt32Bits(actual.Y),BitConverter.SingleToUInt32Bits(actual.Radius)];
   bool same=bits.SequenceEqual(row.GetProperty("resultBits").EnumerateArray().Select(v=>v.GetUInt32()));int id=row.GetProperty("index").GetInt32();if(!same)failures.Add(id);
   report.Add(new{index=id,same,actual,bits});
  }
  File.WriteAllText("analysis/calibration-board/enclosing-circle-differences.json",JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));
  check(failures.Count==0,"native integer contour enclosing circle float bits: "+string.Join(",",failures));
 }
}
