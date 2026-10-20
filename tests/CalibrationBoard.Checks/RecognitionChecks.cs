using System.IO;
using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPlugins.CalibrationBoard;

static class RecognitionChecks
{
 public static void Run(Action<bool,string> check)
 {
  byte[] pixels=Enumerable.Repeat((byte)255,480*400).ToArray();
  for(int row=0;row<8;row++)for(int col=0;col<10;col++)for(int dy=-9;dy<=9;dy++)for(int dx=-9;dx<=9;dx++)if(dx*dx+dy*dy<81)pixels[(80+row*30+dy)*480+90+col*30+dx]=0;
  using var fixture=JsonDocument.Parse(File.ReadAllText("docs/plugins/calibration-board-native-fixtures.jsonl").Split('\n',StringSplitOptions.RemoveEmptyEntries).First(line=>{using var d=JsonDocument.Parse(line);return d.RootElement.TryGetProperty("board",out var b)&&b.GetInt32()==2&&d.RootElement.TryGetProperty("mode",out var m)&&m.GetInt32()==1&&d.RootElement.TryGetProperty("points",out _);}));
  var expected=fixture.RootElement.GetProperty("points").EnumerateArray().ToArray();
  // Detection must remain usable without an unrelated world fit: invalid world
  // size/freedom/weight deliberately prevents the legacy calibration path.
  var result=CalibrationBoardSolver.Recognize(new GrayImage(480,400,pixels),new(BoardType:2,PhysicalSize:0,Freedom:99,Weight:99));
  check(result.Error==0&&result.Points.Count==expected.Length,"recognition independent of invalid world calibration parameters");
  check(result.Points.Select((p,i)=>p.X==expected[i][0].GetSingle()&&p.Y==expected[i][1].GetSingle()&&p.IndexX==expected[i][2].GetInt32()&&p.IndexY==expected[i][3].GetInt32()).All(v=>v),"recognition source SDK indices/order/pixels exact");
  check(result.EncodedGridPitch is null,"plain circle recognition does not invent encoded grid pitch");
  var sdkRows=File.ReadLines("docs/plugins/calibration-board-weight-mapping-fixtures.jsonl").Select(line=>JsonDocument.Parse(line).RootElement.Clone()).ToArray();
  foreach(int weight in new[]{3,4})
  {
   var sdk=sdkRows.Single(r=>r.TryGetProperty("mode",out var m)&&m.GetInt32()==weight&&r.TryGetProperty("matrix",out _));
   var metrics=sdkRows.Single(r=>r.TryGetProperty("mode",out var m)&&m.GetInt32()==weight&&r.TryGetProperty("phase",out var p)&&p.GetString()=="metrics");
   var solved=CalibrationBoardSolver.Solve(new GrayImage(480,400,pixels),new(BoardType:2,Weight:weight));
   check(solved.Error==0,"original SDK public weight "+weight+" succeeds");
   check(solved.Fit!.Matrix.Select((v,i)=>BitConverter.SingleToInt32Bits((float)v)==BitConverter.SingleToInt32Bits(sdk.GetProperty("matrix")[i].GetSingle())).All(v=>v),"original SDK public weight "+weight+" matrix float exact");
   double[] values=[solved.Fit.OffsetError,solved.Fit.Scale,solved.Fit.DeltaX,solved.Fit.DeltaY,solved.Fit.Theta,solved.Fit.Aspect,solved.Fit.Skew,solved.Fit.PixelPrecision];
   string[] names=["error","scale","x","y","rotate","aspect","skew","precision"];
   check(values.Select((v,i)=>BitConverter.SingleToInt32Bits((float)v)==BitConverter.SingleToInt32Bits(metrics.GetProperty(names[i]).GetSingle())).All(v=>v),"original SDK public weight "+weight+" all result metrics float exact");
   var plugin=new CalibrationBoardPlugin();var parameters=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());parameters["CalibBoardType"]=JsonSerializer.SerializeToElement(2);parameters["CalibBoardWeightFun"]=JsonSerializer.SerializeToElement(weight);
   var run=plugin.CreateRuntime().ExecuteAsync(new("sdk-axes",Environment.CurrentDirectory,new Dictionary<string,PortValue>{{"InputImage",PortValue.Image(new GrayImage(480,400,pixels))}},parameters),default).GetAwaiter().GetResult();
   check(run.Status==ModuleStatus.Success,"product runtime public weight "+weight);
   string[] axisNames=["CalibOriPointX","CalibOriPointY","CalibOriPointXVecX","CalibOriPointXVecY","CalibOriPointYVecX","CalibOriPointYVecY"];
   var sdkAxes=sdk.GetProperty("origin").EnumerateArray().Concat(sdk.GetProperty("vx").EnumerateArray()).Concat(sdk.GetProperty("vy").EnumerateArray()).Select(v=>v.GetSingle()).ToArray();
   check(axisNames.Select((name,i)=>BitConverter.SingleToInt32Bits((float)run.Outputs[name].As<double>())==BitConverter.SingleToInt32Bits(sdkAxes[i])).All(v=>v),"product original SDK six axes float exact weight "+weight);
  }
  var failed=new List<string>();
  foreach(string line in File.ReadLines("docs/plugins/calibration-board-fit-dispatch-fixtures.jsonl").Concat(File.ReadLines("docs/plugins/calibration-board-sparse-fit-fixtures.jsonl")))
  {
   using var row=JsonDocument.Parse(line);var r=row.RootElement;
   var points=r.GetProperty("input").EnumerateArray().Select(p=>new VisionPlugins.Calibration.NImageBoardPoint(p[2].GetInt32(),p[3].GetInt32(),p[0].GetSingle(),p[1].GetSingle())).ToArray();
   var options=new BoardCalibrationOptions(BoardType:2,OriginX:2,OriginY:-3,RotationAngle:(double)(.3f/.01745329238474369f),PhysicalSize:2,Freedom:r.GetProperty("freedom").GetInt32(),Weight:r.GetProperty("weight").GetInt32());
   var staged=CalibrationBoardFitting.Fit(points,options,referenceX:240,referenceY:200);
   var nativeInitial=r.GetProperty("model").EnumerateArray().Skip(1).Take(9).Select(v=>v.GetSingle()).ToArray();
   File.AppendAllText("analysis/calibration-board/fit-stages-differences.jsonl",JsonSerializer.Serialize(new{index=r.GetProperty("index").GetInt32(),staged.Error,nativeInitial,actualInitial=staged.NormalizedGridToImage,nativeFinal=r.GetProperty("matrix").EnumerateArray().Select(v=>v.GetSingle()).ToArray(),actualFinal=staged.ImageToWorld})+Environment.NewLine);
   uint nativeCode=r.GetProperty("code").GetUInt32();if(staged.Error!=nativeCode)failed.Add("code:"+r.GetProperty("index").GetInt32());
   if(nativeCode==0&&!(staged.Error==0&&staged.NormalizedGridToImage!.Select((v,i)=>BitConverter.SingleToInt32Bits(v)==BitConverter.SingleToInt32Bits(nativeInitial[i])).All(v=>v)))failed.Add("first:"+r.GetProperty("index").GetInt32());
   var nativeMatrix=r.GetProperty("matrix").EnumerateArray().Select(v=>v.GetSingle()).ToArray();
   if(nativeCode==0&&!(staged.Error==0&&staged.ImageToWorld!.Select((v,i)=>BitConverter.SingleToInt32Bits(v)==BitConverter.SingleToInt32Bits(nativeMatrix[i])).All(v=>v)))failed.Add("final:"+r.GetProperty("index").GetInt32());
   if(nativeCode==0&&staged.Error==0)
   {
    var analyzed=CalibrationBoardFitting.Analyze(points,options,staged);
    float[] values=[(float)analyzed.Scale,(float)analyzed.DeltaX,(float)analyzed.DeltaY,(float)analyzed.Theta,(float)analyzed.Aspect,(float)analyzed.Skew,(float)analyzed.PixelPrecision];
    var original=r.GetProperty("sdkParameters").EnumerateArray().Select(v=>v.GetSingle()).ToArray();
    check(values.Select((v,i)=>BitConverter.SingleToInt32Bits(v)==BitConverter.SingleToInt32Bits(original[i])).All(v=>v),"original SDK six parameters and pixel precision float exact "+r.GetProperty("index").GetInt32());
    check(BitConverter.SingleToInt32Bits((float)analyzed.OffsetError)==BitConverter.SingleToInt32Bits(r.GetProperty("error")[0].GetSingle()),"original fit mean error float exact "+r.GetProperty("index").GetInt32());
    var axes=CalibrationBoardAxes.FromMatrix(staged.ImageToWorld!);float[] axisValues=[axes.OriginX,axes.OriginY,axes.XVectorX,axes.XVectorY,axes.YVectorX,axes.YVectorY];
    check(axisValues.Select((v,i)=>BitConverter.SingleToInt32Bits(v)==BitConverter.SingleToInt32Bits(r.GetProperty("axes")[i].GetSingle())).All(v=>v),"original calibration origin and vectors float exact "+r.GetProperty("index").GetInt32());
   }
   var recoded=CalibrationBoardFitGeometry.Recode(points);var expectedIndices=r.GetProperty("recoded").EnumerateArray().ToArray();
   check(recoded.Select((p,i)=>p.X==expectedIndices[i][0].GetInt32()&&p.Y==expectedIndices[i][1].GetInt32()).All(v=>v),"source 7ed0c0 index recoding "+r.GetProperty("index").GetInt32());
  }
  check(failed.Count==0,"strict two-stage original errors/matrices: "+string.Join(",",failed));
 }
}
