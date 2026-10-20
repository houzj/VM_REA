using System.IO;
using System.Text.Json;
using VisionCore;
using VisionPlugins.CalibrationBoard;
static class MaskedRuntimeChecks
{
 public static void Run(Action<bool,string> check)
 {
  using var fixture=JsonDocument.Parse(File.ReadAllText("docs/plugins/calibration-board-roi-rotated-fixtures.json"));var differences=new List<object>();
  byte[] pixels=Enumerable.Repeat((byte)255,480*400).ToArray();for(int row=0;row<8;row++)for(int col=0;col<10;col++)for(int dy=-9;dy<=9;dy++)for(int dx=-9;dx<=9;dx++)if(dx*dx+dy*dy<81)pixels[(80+row*30+dy)*480+90+col*30+dx]=0;var image=new GrayImage(480,400,pixels);
  foreach(var row in fixture.RootElement.GetProperty("rows").EnumerateArray())
  {
   int index=row.GetProperty("index").GetInt32();var mask=CalibrationBoardRoiMask.Rectangle(480,400,row.GetProperty("cx").GetSingle(),row.GetProperty("cy").GetSingle(),row.GetProperty("width").GetSingle(),row.GetProperty("height").GetSingle(),row.GetProperty("angle").GetSingle());
   check(new[]{mask.Left,mask.Top,mask.Width,mask.Height}.SequenceEqual(row.GetProperty("bounds").EnumerateArray().Select(e=>e.GetInt32())),"original full-size rotated mask bbox "+index);
   var result=CalibrationBoardSolver.SolveMasked(image,new(BoardType:2),mask);var native=row.GetProperty("points").EnumerateArray().ToArray();check(result.Error==0,"original rotated board runtime success "+index);check(result.Points.Count==native.Length,"original mask-retained count "+index);
   double max=0;int indexDifferences=0;for(int i=0;i<Math.Min(native.Length,result.Points.Count);i++){var actual=result.Points[i];var expected=native[i];if(actual.IndexX!=expected[0].GetInt32()||actual.IndexY!=expected[1].GetInt32())indexDifferences++;max=Math.Max(max,Math.Max(Math.Abs(actual.X-expected[2].GetSingle()),Math.Abs(actual.Y-expected[3].GetSingle())));}
   differences.Add(new{index,managedCount=result.Points.Count,originalCount=native.Length,indexDifferences,maximumPixelDifference=max});
  }
  File.WriteAllText("analysis/calibration-board/roi-rotated-native/managed-differences.json",JsonSerializer.Serialize(differences,new JsonSerializerOptions{WriteIndented=true}));
  foreach(var item in differences){using var parsed=JsonDocument.Parse(JsonSerializer.Serialize(item));check(parsed.RootElement.GetProperty("indexDifferences").GetInt32()==0,"original mask-retained object indices");check(parsed.RootElement.GetProperty("maximumPixelDifference").GetDouble()==0,"original mask-retained source image points exact float");}
 }
}
