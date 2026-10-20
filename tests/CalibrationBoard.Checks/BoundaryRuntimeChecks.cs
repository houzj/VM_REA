using System.IO;
using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPlugins.CalibrationBoard;

static class BoundaryRuntimeChecks
{
 public static void Run(Action<bool,string> check)
 {
  var report=new List<object>();var failed=new List<int>();
  bool Bits(IEnumerable<float> a,IEnumerable<float> b)=>a.Select(BitConverter.SingleToInt32Bits).SequenceEqual(b.Select(BitConverter.SingleToInt32Bits));
  foreach(string line in File.ReadLines("docs/plugins/calibration-board-boundary-runtime-fixtures.jsonl"))
  {
   using var document=JsonDocument.Parse(line);var row=document.RootElement;
   int id=row.GetProperty("index").GetInt32(),start=row.GetProperty("start").GetInt32(),top=row.GetProperty("top").GetInt32(),shape=row.GetProperty("shape").GetInt32(),contrast=row.GetProperty("contrast").GetInt32();bool gradient=row.GetProperty("gradient").GetBoolean();
   var pixels=Enumerable.Repeat((byte)255,480*400).ToArray();
   for(int r=0;r<8;r++)for(int c=0;c<10;c++)for(int y=-9;y<=9;y++)for(int x=-9;x<=9;x++){bool inside=shape==0?x*x+y*y<81:shape==1?x*x/81f+y*y/25f<1:Math.Abs(x)<9&&Math.Abs(y)<9;int px=start+c*30+x,py=(top!=0?start:80)+r*30+y;if(inside&&px>=0&&px<480&&py>=0&&py<400)pixels[py*480+px]=(byte)(255-contrast+(gradient&&x*x+y*y<16?2:0));}
   var image=new GrayImage(480,400,pixels);var options=new BoardCalibrationOptions(BoardType:2,Circularity:20,EdgeLow:row.GetProperty("low").GetInt32(),EdgeHigh:row.GetProperty("high").GetInt32());
   var actual=CalibrationBoardSolver.Solve(image,options);var expected=row.GetProperty("points").EnumerateArray().ToArray();
   bool points=actual.Error==0&&actual.Points.Count==expected.Length&&actual.Points.Select((p,i)=>p.IndexX==expected[i][0].GetInt32()&&p.IndexY==expected[i][1].GetInt32()&&Bits([(float)p.X,(float)p.Y],[expected[i][2].GetSingle(),expected[i][3].GetSingle()])).All(v=>v);
   bool matrix=actual.Fit!=null&&Bits(actual.Fit.Matrix.Select(v=>(float)v),row.GetProperty("matrix").EnumerateArray().Select(v=>v.GetSingle()));
   string[] names=["error","scale","x","y","rotate","aspect","skew","precision"];
   float[] values=actual.Fit is { } fit?[(float)fit.OffsetError,(float)fit.Scale,(float)fit.DeltaX,(float)fit.DeltaY,(float)fit.Theta,(float)fit.Aspect,(float)fit.Skew,(float)fit.PixelPrecision]:[];
   bool metrics=Bits(values,names.Select(n=>row.GetProperty(n).GetSingle()));bool axes=false;
   if(actual.Fit!=null){var a=CalibrationBoardAxes.FromMatrix(actual.Fit.Matrix.Select(v=>(float)v).ToArray());axes=Bits([a.OriginX,a.OriginY,a.XVectorX,a.XVectorY,a.YVectorX,a.YVectorY],new[]{"origin","vx","vy"}.SelectMany(n=>row.GetProperty(n).EnumerateArray().Select(v=>v.GetSingle())));}
   if(!points||!matrix||!metrics||!axes)failed.Add(id);
   report.Add(new{index=id,actual.Error,pointCount=actual.Points.Count,nativeCount=expected.Length,pointsFloatBitsExact=points,matrixFloatBitsExact=matrix,metricsFloatBitsExact=metrics,axesFloatBitsExact=axes,actualMatrix=actual.Fit?.Matrix,actualMetrics=values});
   File.WriteAllText("analysis/calibration-board/boundary-runtime-differences.json",JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));
   if(id==120)
   {
    var plugin=new CalibrationBoardPlugin();var parameters=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());parameters["CalibBoardType"]=JsonSerializer.SerializeToElement(2);parameters["Circularity"]=JsonSerializer.SerializeToElement(20);parameters["EdgeThreshLow"]=JsonSerializer.SerializeToElement(options.EdgeLow);parameters["EdgeThreshHigh"]=JsonSerializer.SerializeToElement(options.EdgeHigh);
    var run=plugin.CreateRuntime().ExecuteAsync(new("boundary-general-fallback",Environment.CurrentDirectory,new Dictionary<string,PortValue>{{"InputImage",PortValue.Image(image)}},parameters),default).GetAwaiter().GetResult();
    check(run.Status==ModuleStatus.Success,"actual product module succeeds for previously unsupported non-frame fallback");
    check(Bits(run.Outputs["CalibData"].As<CalibrationData>().ImageToWorld.Select(v=>(float)v),row.GetProperty("matrix").EnumerateArray().Select(v=>v.GetSingle())),"actual non-frame fallback module H matches original SDK float bits");
   }
  }
  check(failed.Count==0,"actual original SDK boundary tool all points/H/metrics/axes: "+string.Join(",",failed));
 }
}
