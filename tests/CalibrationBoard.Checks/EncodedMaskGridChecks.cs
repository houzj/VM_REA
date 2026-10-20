using System.IO;
using System.Text.Json;
using VisionPlugins.Calibration;
using VisionPlugins.CalibrationBoard;

static class EncodedMaskGridChecks
{
 public static void Run(Action<bool,string> check)
 {
  // Captured 873de0 return and 80e690's next 7ff3c0 entry, before decoding.
  foreach(string line in File.ReadLines("docs/plugins/calibration-board-encoded-mask-grid-fixtures.jsonl"))
  {
   using var document=JsonDocument.Parse(line);var row=document.RootElement;
   int id=row.GetProperty("index").GetInt32();
   var mask=CalibrationBoardRoiMask.Rectangle(row.GetProperty("imageWidth").GetInt32(),row.GetProperty("imageHeight").GetInt32(),row.GetProperty("cx").GetSingle(),row.GetProperty("cy").GetSingle(),row.GetProperty("width").GetSingle(),row.GetProperty("height").GetSingle(),row.GetProperty("angle").GetSingle());
   var local=row.GetProperty("localPoints").EnumerateArray().Select(p=>new NImageLatticePoint(p[0].GetInt32(),p[1].GetInt32(),p[2].GetSingle(),p[3].GetSingle(),0)).ToArray();
   var expected=row.GetProperty("originalFilteredPoints").EnumerateArray().ToArray();
   var actual=CalibrationBoardMaskedGrid.Project(local,mask);
   check(actual.Count==expected.Length,"native pre-decode masked grid count "+id);
   check(actual.Select((p,i)=>p.Column==expected[i][0].GetSingle()&&p.Row==expected[i][1].GetSingle()&&(float)p.X==expected[i][2].GetSingle()&&(float)p.Y==expected[i][3].GetSingle()).All(v=>v),"native pre-decode masked grid indices/order/float coordinates "+id);
  }
  foreach(int type in new[]{6,7,8,9})
  {
   var blank=new VisionCore.GrayImage(1000,1000,Enumerable.Repeat((byte)255,1000000).ToArray());
   var result=CalibrationBoardSolver.Recognize(blank,new(BoardType:type));
   check(result.Error==0x10000107u&&result.Points.Count==0,"original encoded board blank error "+type);
  }
 }
}
