using System.IO;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionCore;
using VisionFramework;
using VisionPlugins.CalibrationBoard;

static class EncodedFullProductChecks
{
 public static void Run(Action<bool,string> check)
 {
  using var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/calibration-board-encoded-roi-rotated-evidence.json"));
  var report=new List<object>();
  foreach(string line in File.ReadLines("docs/plugins/calibration-board-encoded-full-output-fixtures.jsonl"))
  {
   using var doc=JsonDocument.Parse(line);var row=doc.RootElement;int type=row.GetProperty("boardType").GetInt32();
   string path=evidence.RootElement.GetProperty("sources").EnumerateArray().Single(r=>r.GetProperty("boardType").GetInt32()==type).GetProperty("image").GetString()!;
   var gray=new FormatConvertedBitmap(BitmapFrame.Create(new Uri(Path.GetFullPath(path))),PixelFormats.Gray8,null,0);var pixels=new byte[gray.PixelWidth*gray.PixelHeight];gray.CopyPixels(pixels,gray.PixelWidth,0);var image=new GrayImage(gray.PixelWidth,gray.PixelHeight,pixels);
   var plugin=new CalibrationBoardPlugin();var parameters=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());parameters["CalibBoardType"]=JsonSerializer.SerializeToElement(type);
   var run=plugin.CreateRuntime().ExecuteAsync(new("encoded-full-"+type,Environment.CurrentDirectory,new Dictionary<string,PortValue>{{"InputImage",PortValue.Image(image)}},parameters),default).GetAwaiter().GetResult();
   check(run.Status==ModuleStatus.Success,"encoded actual product runtime succeeds "+type);
   check(plugin.Descriptor.Outputs.All(p=>run.Outputs.ContainsKey(p.Name)),"encoded actual product declares every output "+type);
   var expected=row.GetProperty("points").EnumerateArray().ToArray();var xs=run.Outputs["CalibPointX"].As<IReadOnlyList<double>>();var ys=run.Outputs["CalibPointY"].As<IReadOnlyList<double>>();
   bool pointBits=xs.Count==expected.Length&&xs.Select((x,i)=>BitConverter.SingleToInt32Bits((float)x)==BitConverter.SingleToInt32Bits(expected[i][2].GetSingle())&&BitConverter.SingleToInt32Bits((float)ys[i])==BitConverter.SingleToInt32Bits(expected[i][3].GetSingle())).All(v=>v);
   string[] ports=["EstimationError","Scale","TranslateX","TranslateY","Rotate","Aspect","Skew","PixelPrecision","CalibOriPointX","CalibOriPointY","CalibOriPointXVecX","CalibOriPointXVecY","CalibOriPointYVecX","CalibOriPointYVecY"];
   float[] native=[row.GetProperty("error").GetSingle(),row.GetProperty("scale").GetSingle(),row.GetProperty("x").GetSingle(),row.GetProperty("y").GetSingle(),row.GetProperty("rotate").GetSingle(),row.GetProperty("aspect").GetSingle(),row.GetProperty("skew").GetSingle(),row.GetProperty("precision").GetSingle(),..new[]{"origin","vx","vy"}.SelectMany(n=>row.GetProperty(n).EnumerateArray().Select(v=>v.GetSingle()))];
   var fields=ports.Select((name,i)=>new{name,original=native[i],managed=(float)run.Outputs[name].As<double>(),floatBitsExact=BitConverter.SingleToInt32Bits(native[i])==BitConverter.SingleToInt32Bits((float)run.Outputs[name].As<double>())}).ToArray();
   var actualH=run.Outputs["CalibData"].As<CalibrationData>().ImageToWorld;var originalH=row.GetProperty("matrix").EnumerateArray().Select(v=>v.GetSingle()).ToArray();bool matrixBits=actualH.Select(v=>BitConverter.SingleToInt32Bits((float)v)).SequenceEqual(originalH.Select(BitConverter.SingleToInt32Bits));
   check(pointBits&&matrixBits&&fields.All(f=>f.floatBitsExact),"original encoded type "+type+" full product points/H/all scalar outputs float exact");
   report.Add(new{boardType=type,matrixFloatBitsExact=matrixBits,actualMatrix=actualH,originalMatrix=originalH,originalCount=expected.Length,managedCount=xs.Count,pointsFloatBitsExact=pointBits,fields,scaleRatio=run.Outputs["Scale"].As<double>()/row.GetProperty("scale").GetDouble(),precisionRatio=run.Outputs["PixelPrecision"].As<double>()/row.GetProperty("precision").GetDouble()});
   File.WriteAllText("analysis/calibration-board/encoded-full-product-differences.json",JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));
  }
 }
}

