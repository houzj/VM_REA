using System.IO;
using System.IO.Compression;
using System.Text.Json;
using VisionPlugins.CalibrationBoard;
static class RoiMaskChecks
{
 public static void Run(Action<bool,string> check)
 {
  using var fixture=JsonDocument.Parse(File.ReadAllText("docs/plugins/calibration-board-roi-mask-fixtures.json"));var differences=new List<object>();
  foreach(var row in fixture.RootElement.GetProperty("rows").EnumerateArray())
  {
   var mask=CalibrationBoardRoiMask.Rectangle(64,48,row.GetProperty("cx").GetSingle(),row.GetProperty("cy").GetSingle(),row.GetProperty("width").GetSingle(),row.GetProperty("height").GetSingle(),row.GetProperty("angle").GetSingle());
   using var input=new MemoryStream(Convert.FromBase64String(row.GetProperty("zlibBase64").GetString()!));using var stream=new ZLibStream(input,CompressionMode.Decompress);using var output=new MemoryStream();stream.CopyTo(output);var native=output.ToArray();
   int diff=mask.Pixels.Zip(native).Count(p=>p.First!=p.Second);differences.Add(new{index=row.GetProperty("index").GetInt32(),pixelDifferences=diff});check(diff==0,"native rotated rectangle mask pixels "+row.GetProperty("index").GetInt32());
   check(new[]{mask.Left,mask.Top,mask.Width,mask.Height}.SequenceEqual(row.GetProperty("bounds").EnumerateArray().Select(e=>e.GetInt32())),"native nonzero mask bbox");
   if(mask.Vertices.Count>0)foreach(var (point,nativePoint) in mask.Vertices.Zip(row.GetProperty("vertices").EnumerateArray()))check(point.X==nativePoint[0].GetSingle()&&point.Y==nativePoint[1].GetSingle(),"native source float rotated polygon vertex");
  }
  File.WriteAllText("analysis/calibration-board/roi-mask-native/managed-differences.json",JsonSerializer.Serialize(differences,new JsonSerializerOptions{WriteIndented=true}));
 }
}
