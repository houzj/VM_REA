using System.IO;
using System.Text.Json;
using VisionCore;
using VisionPlugins.Calibration;
using VisionPlugins.CalibrationBoard;

// Diagnostic source-driven replay; deliberately separate from product acceptance
// while the checker corner kernel still has known numerical differences.
static class CheckerMaskedChecks
{
 public static void Run(Action<bool,string> check)
 {
  byte[] pixels=Enumerable.Repeat((byte)255,480*400).ToArray();for(int y=50;y<350;y++)for(int x=60;x<420;x++)pixels[y*480+x]=(byte)((((x-60)/30+(y-50)/30)&1)==0?0:255);
  var differences=new List<object>();var failures=new List<int>();
  foreach(string line in File.ReadLines("docs/plugins/calibration-board-checker-roi-rotated-fixtures.jsonl"))
  {
   using var row=JsonDocument.Parse(line);var r=row.RootElement;int index=r.GetProperty("index").GetInt32();
   var mask=CalibrationBoardRoiMask.Rectangle(480,400,r.GetProperty("cx").GetSingle(),r.GetProperty("cy").GetSingle(),r.GetProperty("width").GetSingle(),r.GetProperty("height").GetSingle(),r.GetProperty("angle").GetSingle());
   var crop=new byte[mask.Width*mask.Height];for(int y=0;y<mask.Height;y++)Buffer.BlockCopy(pixels,(mask.Top+y)*480+mask.Left,crop,y*mask.Width,mask.Width);
   var actual=CalibrationBoardSolver.RecognizeMasked(new GrayImage(480,400,pixels),new(BoardType:1),mask).Points.ToArray();
   var original=r.GetProperty("points").EnumerateArray().ToArray();bool ids=actual.Length==original.Length&&actual.Select((p,i)=>p.IndexX==original[i][0].GetInt32()&&p.IndexY==original[i][1].GetInt32()).All(v=>v);
   double? maximum=ids?actual.Select((p,i)=>Math.Max(Math.Abs(p.X-original[i][2].GetSingle()),Math.Abs(p.Y-original[i][3].GetSingle()))).Max():null;
   bool floatBits=ids&&actual.Select((p,i)=>BitConverter.SingleToInt32Bits((float)p.X)==BitConverter.SingleToInt32Bits(original[i][2].GetSingle())&&BitConverter.SingleToInt32Bits((float)p.Y)==BitConverter.SingleToInt32Bits(original[i][3].GetSingle())).All(v=>v);check(floatBits,"source checker masked points/ids/order/float bits "+index);
   if(!ids)failures.Add(index);differences.Add(new{index,originalCount=original.Length,managedCount=actual.Length,idsAndOrderExact=ids,coordinatesFloatBitsExact=floatBits,maximumCoordinateDifference=maximum});
   check(mask.Left==r.GetProperty("bounds")[0].GetInt32()&&mask.Top==r.GetProperty("bounds")[1].GetInt32()&&mask.Width==r.GetProperty("bounds")[2].GetInt32()&&mask.Height==r.GetProperty("bounds")[3].GetInt32(),"original checker rotated-mask bbox "+index);
  }
  File.WriteAllText("analysis/calibration-board/checker-mask-differences.json",JsonSerializer.Serialize(differences,new JsonSerializerOptions{WriteIndented=true}));
  check(failures.Count==0,"checker mask source ids/order: "+string.Join(",",failures));
 }
}
