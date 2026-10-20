using System.IO;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionCore;
using VisionPlugins.CalibrationBoard;
static class EncodedAxisRuntimeChecks {
 public static void Run(Action<bool,string> check){
 using var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/calibration-board-encoded-roi-rotated-evidence.json"));var report=new List<object>();var failed=new List<int>();
 bool Bits(IEnumerable<float>a,IEnumerable<float>b)=>a.Select(BitConverter.SingleToInt32Bits).SequenceEqual(b.Select(BitConverter.SingleToInt32Bits));
 foreach(var line in File.ReadLines("docs/plugins/calibration-board-encoded-axis-output-fixtures.jsonl")){
 using var d=JsonDocument.Parse(line);var r=d.RootElement;int type=r.GetProperty("boardType").GetInt32();
 string path=evidence.RootElement.GetProperty("sources").EnumerateArray().Single(s=>s.GetProperty("boardType").GetInt32()==type).GetProperty("image").GetString()!;var gray=new FormatConvertedBitmap(BitmapFrame.Create(new Uri(Path.GetFullPath(path))),PixelFormats.Gray8,null,0);var pixels=new byte[gray.PixelWidth*gray.PixelHeight];gray.CopyPixels(pixels,gray.PixelWidth,0);var image=new GrayImage(gray.PixelWidth,gray.PixelHeight,pixels);
 int left=(int)(r.GetProperty("cx").GetSingle()-r.GetProperty("width").GetSingle()*.5f+.5f),top=(int)(r.GetProperty("cy").GetSingle()-r.GetProperty("height").GetSingle()*.5f+.5f),width=(int)(r.GetProperty("width").GetSingle()+.5f),height=(int)(r.GetProperty("height").GetSingle()+.5f);var bytes=new byte[width*height];for(int y=0;y<height;y++)image.Pixels.Slice((top+y)*image.Width+left,width).CopyTo(bytes.AsSpan(y*width,width));var result=CalibrationBoardSolver.SolveCropped(image,new GrayImage(width,height,bytes),new(BoardType:type),left,top);var expected=r.GetProperty("points").EnumerateArray().ToArray();var fit=result.Fit;
 bool points=result.Points.Count==expected.Length&&result.Points.Select((p,i)=>p.IndexX==expected[i][0].GetInt32()&&p.IndexY==expected[i][1].GetInt32()&&Bits([(float)p.X,(float)p.Y],[expected[i][2].GetSingle(),expected[i][3].GetSingle()])).All(v=>v);
 bool h=fit!=null&&Bits(fit.Matrix.Select(v=>(float)v),r.GetProperty("matrix").EnumerateArray().Select(v=>v.GetSingle()));bool scalar=false;
 if(fit!=null){var a=CalibrationBoardAxes.FromMatrix(fit.Matrix.Select(v=>(float)v).ToArray());float[] values=[(float)fit.OffsetError,(float)fit.Scale,(float)fit.DeltaX,(float)fit.DeltaY,(float)fit.Theta,(float)fit.Aspect,(float)fit.Skew,(float)fit.PixelPrecision,a.OriginX,a.OriginY,a.XVectorX,a.XVectorY,a.YVectorX,a.YVectorY];var native=new[]{"error","scale","x","y","rotate","aspect","skew","precision"}.Select(n=>r.GetProperty(n).GetSingle()).Concat(new[]{"origin","vx","vy"}.SelectMany(n=>r.GetProperty(n).EnumerateArray().Select(v=>v.GetSingle())));scalar=Bits(values,native);}
 report.Add(new{boardType=type,index=r.GetProperty("index").GetInt32(),result.Error,pointsFloatBitsExact=points,matrixFloatBitsExact=h,scalarsFloatBitsExact=scalar,actual=fit});if(result.Error!=0||!points||!h||!scalar)failed.Add(type);File.WriteAllText("analysis/calibration-board/encoded-axis-runtime-differences.json",JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));
 }
 check(failed.Count==0,"actual encoded axis-crop full tool all points/H/14scalar outputs "+string.Join(",",failed));
 }
}
