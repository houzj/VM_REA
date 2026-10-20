using System.IO;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionCore;
using VisionPlugins.Calibration;
using VisionPlugins.CalibrationBoard;

// Diagnostic only: source-confirmed mask lifecycle around an independently
// reconstructed shared decoder. Differences are recorded, never hidden by a
// passing bbox assertion or treated as production acceptance.
static class EncodedMaskedRecognitionChecks
{
 public static void Run(Action<bool,string> check)
 {
  using var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/calibration-board-encoded-roi-rotated-evidence.json"));
  var images=new Dictionary<int,GrayImage>();
  foreach(var source in evidence.RootElement.GetProperty("sources").EnumerateArray())
  {
   int type=source.GetProperty("boardType").GetInt32();
   var frame=BitmapFrame.Create(new Uri(Path.GetFullPath(source.GetProperty("image").GetString()!)));
   var gray=new FormatConvertedBitmap(frame,PixelFormats.Gray8,null,0);var pixels=new byte[gray.PixelWidth*gray.PixelHeight];gray.CopyPixels(pixels,gray.PixelWidth,0);images[type]=new(gray.PixelWidth,gray.PixelHeight,pixels);
  }
  int selectedType=int.TryParse(Environment.GetCommandLineArgs().FirstOrDefault(a=>a.StartsWith("--encoded-mask-type="))?.Split('=')[1],out int typeArgument)?typeArgument:-1;
  int startIndex=int.TryParse(Environment.GetCommandLineArgs().FirstOrDefault(a=>a.StartsWith("--encoded-mask-start="))?.Split('=')[1],out int startArgument)?startArgument:0;
  bool strict=Environment.GetCommandLineArgs().Contains("--encoded-mask-strict");
  var report=new List<object>();
  foreach(string line in File.ReadLines("docs/plugins/calibration-board-encoded-roi-rotated-fixtures.jsonl"))
  {
   using var document=JsonDocument.Parse(line);var row=document.RootElement;int type=row.GetProperty("boardType").GetInt32(),index=row.GetProperty("index").GetInt32();if(index<startIndex||(selectedType>=0&&type!=selectedType))continue;var image=images[type];
   var mask=CalibrationBoardRoiMask.Rectangle(image.Width,image.Height,row.GetProperty("cx").GetSingle(),row.GetProperty("cy").GetSingle(),row.GetProperty("width").GetSingle(),row.GetProperty("height").GetSingle(),row.GetProperty("angle").GetSingle());
   var crop=new byte[mask.Width*mask.Height];for(int y=0;y<mask.Height;y++)image.Pixels.Slice((mask.Top+y)*image.Width+mask.Left,mask.Width).CopyTo(crop.AsSpan(y*mask.Width,mask.Width));
   var result=CalibrationBoardSolver.RecognizeMasked(image,new(BoardType:type),mask);
   var actual=result.Points;
   var original=row.GetProperty("points").EnumerateArray().ToArray();
   bool ids=actual.Count==original.Length&&actual.Select((p,i)=>p.IndexX==original[i][0].GetInt32()&&p.IndexY==original[i][1].GetInt32()).All(v=>v);
   bool floatBits=ids&&actual.Select((p,i)=>BitConverter.SingleToInt32Bits((float)p.X)==BitConverter.SingleToInt32Bits(original[i][2].GetSingle())&&BitConverter.SingleToInt32Bits((float)p.Y)==BitConverter.SingleToInt32Bits(original[i][3].GetSingle())).All(v=>v);
   if(strict)check(floatBits,"strict original masked decode points/IDs/order/float bits "+type+"/"+index);
   double? maximum=ids?actual.Select((p,i)=>Math.Max(Math.Abs(p.X-original[i][2].GetSingle()),Math.Abs(p.Y-original[i][3].GetSingle()))).Max():null;
   report.Add(new{boardType=type,index,originalCount=original.Length,managedCount=actual.Count,idsAndOrderExact=ids,coordinatesFloatBitsExact=floatBits,maximumCoordinateDifference=maximum});
   check(mask.Left==row.GetProperty("bounds")[0].GetInt32()&&mask.Top==row.GetProperty("bounds")[1].GetInt32()&&mask.Width==row.GetProperty("bounds")[2].GetInt32()&&mask.Height==row.GetProperty("bounds")[3].GetInt32(),"original encoded rotated-mask bbox "+type+"/"+index);
   File.WriteAllText(selectedType<0?"analysis/calibration-board/encoded-mask-recognition-differences.json":$"analysis/calibration-board/encoded-mask-recognition-type-{selectedType}.json",JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));
  }
 }
}

