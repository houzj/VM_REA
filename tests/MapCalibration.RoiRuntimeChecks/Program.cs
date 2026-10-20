using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionCore;
using VisionFramework;
using VisionPlugins.MapCalibration;
try {
 int checks=0;void Check(bool ok,string message){if(!ok)throw new Exception(message);checks++;}
 GrayImage Load(string path){var decoder=BitmapDecoder.Create(new Uri(Path.GetFullPath(path)),BitmapCreateOptions.PreservePixelFormat,BitmapCacheOption.OnLoad);var bitmap=new FormatConvertedBitmap(decoder.Frames[0],PixelFormats.Gray8,null,0);var pixels=new byte[bitmap.PixelWidth*bitmap.PixelHeight];bitmap.CopyPixels(pixels,bitmap.PixelWidth,0);return new(bitmap.PixelWidth,bitmap.PixelHeight,pixels);}
 var a=Load("analysis/full-audit/n-image-calibration/solver-fixture/frame-0.bmp");var b=Load("analysis/full-audit/n-image-calibration/solver-fixture/frame-4.bmp");var plugin=new MapCalibrationPlugin();var runtime=plugin.CreateRuntime();var values=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());values["WeightFun"]=JsonSerializer.SerializeToElement(1);var inputs=new Dictionary<string,PortValue>{{"InputImage1",PortValue.Image(a)},{"InputImage2",PortValue.Image(b)}};var reports=new List<object>();
 foreach(string line in File.ReadLines("analysis/map-calibration/image-roi/image-results.jsonl")){
  using var document=JsonDocument.Parse(line);var native=document.RootElement;int index=native.GetProperty("roiCase").GetInt32();var box=native.GetProperty("roi").EnumerateArray().Select(v=>v.GetSingle()).ToArray();var groups=new Dictionary<string,IReadOnlyList<RoiDocument>>();
  for(int side=0;side<2;side++)if(index!=0&&!(index==7&&side==0)&&!(index==8&&side==1))groups["RoiType_"+side]=[new("roi"+side,RoiKind.Rectangle,(float)(box[0]+side*10+box[2]*.5f),(float)(box[1]+side*10+box[3]*.5f),box[2],box[3],AngleDegrees:box[4])];
  var result=await runtime.ExecuteAsync(new("roi-"+index,Directory.GetCurrentDirectory(),inputs,values,roiGroups:groups),CancellationToken.None);uint expected=native.TryGetProperty("exception",out var error)?error.GetUInt32():0;Check(result.NativeErrorCode==expected,"Original ROI case "+index+" code");
  if(expected==0){var counts=native.GetProperty("counts").EnumerateArray().Select(v=>v.GetInt32()).ToArray();var actual=new[]{"CurCornersPointNum","ObjCornersPointNum","ProCornersPointNum"}.Select(n=>result.Outputs[n].As<long>()).ToArray();for(int i=0;i<3;i++)Check(actual[i]==counts[i],"Original ROI point count case "+index+" component "+i);var session=typeof(MapCalibrationPlugin).GetMethod("CurrentImages",BindingFlags.NonPublic|BindingFlags.Instance)!.Invoke(plugin,["roi-"+index,Directory.GetCurrentDirectory(),null])!;var solved=(MapCalibrationImageResult)session.GetType().GetField("Result",BindingFlags.Public|BindingFlags.Instance)!.GetValue(session)!;
   foreach(var pair in solved.Matrix.Zip(native.GetProperty("matrix").EnumerateArray()))Check(BitConverter.SingleToInt32Bits(pair.First)==BitConverter.SingleToInt32Bits(pair.Second.GetSingle()),"ROI matrix bit parity "+index);
   foreach(var component in new[]{("source",solved.Source),("target",solved.Target),("projection",solved.Projection)}){var points=native.GetProperty(component.Item1);for(int k=0;k<component.Item2.Count;k++){var p=component.Item2[k];var q=points[k];Check(p.WorldX==q[2].GetInt32()&&p.WorldY==q[3].GetInt32(),"ROI point ID/order "+index);Check(BitConverter.SingleToInt32Bits(p.X)==BitConverter.SingleToInt32Bits(q[0].GetSingle())&&BitConverter.SingleToInt32Bits(p.Y)==BitConverter.SingleToInt32Bits(q[1].GetSingle()),"ROI XY bit parity "+index+"/"+component.Item1+"/"+k);}}
   Check(BitConverter.SingleToInt32Bits(solved.MapError)==BitConverter.SingleToInt32Bits(native.GetProperty("error").GetSingle()),"ROI error bit parity "+index);
   foreach(var pair in new[]{(solved.XRatio,"ratio",0),(solved.YRatio,"ratio",1),(solved.PixelScale1,"scale",0),(solved.PixelScale2,"scale",1)})Check(BitConverter.SingleToInt32Bits(pair.Item1)==BitConverter.SingleToInt32Bits(native.GetProperty(pair.Item2)[pair.Item3].GetSingle()),"ROI scalar bit parity "+index);
   reports.Add(new{index,counts=actual,mapError=solved.MapError,originalError=native.GetProperty("error").GetSingle(),pointCoordinateParity=true});}
  Console.WriteLine("ROI case "+index+" complete");
 }
 Directory.CreateDirectory("analysis/map-calibration/roi-runtime");File.WriteAllText("analysis/map-calibration/roi-runtime/checks.json",JsonSerializer.Serialize(new{checks,passed=true,publicGeometry=true,originalSdkCountCodeCompared=true,originalPointCoordinateParity=true,reports},new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine($"PASS {checks}: real plugin two-group ROI execution; SDK point/matrix/error bit parity");return 0;
}catch(Exception error){Console.Error.WriteLine(error);return 1;}

