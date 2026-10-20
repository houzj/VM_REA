using System.IO;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionCore;
using VisionPlugins.Calibration;
var reports=new List<object>();bool failed=false;var options=new JsonSerializerOptions{WriteIndented=true};Directory.CreateDirectory("analysis/map-calibration/grid-roi");
foreach(int type in new[]{7,9,6,8}){
 string fixture=type is 7 or 9?"analysis/full-audit/n-image-calibration/solver-fixture/frame-0.bmp":$"tests/NImageCalibration.PluginChecks/Fixtures/hik-generated-{(type==6?2:4)}-large.png";using var stream=File.OpenRead(fixture);var decoder=BitmapDecoder.Create(stream,BitmapCreateOptions.PreservePixelFormat,BitmapCacheOption.OnLoad);var bitmap=new FormatConvertedBitmap(decoder.Frames[0],PixelFormats.Gray8,null,0);int sw=bitmap.PixelWidth,sh=bitmap.PixelHeight;var raw=new byte[sw*sh];bitmap.CopyPixels(raw,sw,0);
 string folder=$"analysis/calibration-board/encoded-mask-grid-{type}-native";var runtime=File.ReadAllLines(folder+"/runtime.jsonl").Select(s=>JsonDocument.Parse(s)).ToDictionary(d=>d.RootElement.GetProperty("index").GetInt32());
 foreach(var line in File.ReadLines(folder+"/grid-stage.jsonl")){using var doc=JsonDocument.Parse(line);var row=doc.RootElement;int index=row.GetProperty("index").GetInt32();var bounds=runtime[index].RootElement.GetProperty("bounds");int left=bounds[0].GetInt32(),top=bounds[1].GetInt32(),width=bounds[2].GetInt32(),height=bounds[3].GetInt32();var bytes=new byte[width*height];for(int y=0;y<height;y++)Array.Copy(raw,(y+top)*sw+left,bytes,y*width,width);var actual=NImageSourceGridPyramid.Find(new GrayImage(width,height,bytes));var expected=row.GetProperty("points");int exact=0;var differences=new List<object>();for(int i=0;i<Math.Min(actual.Points.Count,expected.GetArrayLength());i++){var a=actual.Points[i];var e=expected[i];bool same=a.Column==e[0].GetInt32()&&a.Row==e[1].GetInt32()&&BitConverter.SingleToInt32Bits(a.X)==BitConverter.SingleToInt32Bits(e[2].GetSingle())&&BitConverter.SingleToInt32Bits(a.Y)==BitConverter.SingleToInt32Bits(e[3].GetSingle());if(same)exact++;else if(differences.Count<12)differences.Add(new{i,a,expected=e.Clone()});}bool success=actual.Points.Count==expected.GetArrayLength()&&exact==actual.Points.Count;failed|=!success;reports.Add(new{type,index,width,height,count=actual.Points.Count,expected=expected.GetArrayLength(),exact,success,differences});File.WriteAllText("analysis/map-calibration/grid-roi/comparison.json",JsonSerializer.Serialize(new{cases=reports.Count,failed,reports},options));Console.WriteLine($"ROI grid type{type} case{index}: {exact}/{actual.Points.Count}/{expected.GetArrayLength()}, success={success}");}
 foreach(var d in runtime.Values)d.Dispose();
}
return failed||reports.Count!=84?1:0;
