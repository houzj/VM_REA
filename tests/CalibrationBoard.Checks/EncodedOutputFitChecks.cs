using System.IO;
using System.Text.Json;
using VisionPlugins.Calibration;
using VisionPlugins.CalibrationBoard;

static class EncodedOutputFitChecks
{
 public static void Run(Action<bool,string> check)
 {
  var report=new List<object>();var failures=new List<string>();
  bool Equal(IEnumerable<float> actual,IEnumerable<float> expected)=>actual.Select(BitConverter.SingleToInt32Bits).SequenceEqual(expected.Select(BitConverter.SingleToInt32Bits));
  foreach(string line in File.ReadLines("docs/plugins/calibration-board-encoded-roi-output-fixtures.jsonl").Concat(File.ReadLines("docs/plugins/calibration-board-encoded-physical-pitch-fixtures.jsonl")))
  {
   using var document=JsonDocument.Parse(line);var row=document.RootElement;int type=row.GetProperty("boardType").GetInt32(),id=row.GetProperty("index").GetInt32();
   var points=row.GetProperty("points").EnumerateArray().Select(p=>new NImageBoardPoint(p[0].GetInt32(),p[1].GetInt32(),p[2].GetSingle(),p[3].GetSingle())).ToArray();
   int physicalSize=row.TryGetProperty("physicalSize",out var physical)?physical.GetInt32():1;
   var options=new BoardCalibrationOptions(BoardType:type,PhysicalSize:physicalSize);
   var stages=CalibrationBoardFitting.Fit(points,options);
   var result=CalibrationBoardFitting.Analyze(points,options,stages);
   bool matrix=result.ErrorCode==0&&Equal(result.Matrix.Select(v=>(float)v),row.GetProperty("matrix").EnumerateArray().Select(v=>v.GetSingle()));
   bool mean=result.ErrorCode==0&&BitConverter.SingleToInt32Bits((float)result.OffsetError)==BitConverter.SingleToInt32Bits(row.GetProperty("error").GetSingle());
   float[] values=[(float)result.Scale,(float)result.DeltaX,(float)result.DeltaY,(float)result.Theta,(float)result.Aspect,(float)result.Skew,(float)result.PixelPrecision];
   string[] names=["scale","x","y","rotate","aspect","skew","precision"];
   bool parameters=result.ErrorCode==0&&Equal(values,names.Select(n=>row.GetProperty(n).GetSingle()));
   bool axes=false;if(stages.ImageToWorld is not null){var a=CalibrationBoardAxes.FromMatrix(stages.ImageToWorld);axes=Equal([a.OriginX,a.OriginY,a.XVectorX,a.XVectorY,a.YVectorX,a.YVectorY],new[]{"origin","vx","vy"}.SelectMany(n=>row.GetProperty(n).EnumerateArray().Select(v=>v.GetSingle())));}
   if(!matrix||!mean||!parameters||!axes)failures.Add(type+"/"+id);
   double? legacyOverrideRatio=null;if(type is 7 or 9&&id==0&&physicalSize==1){var legacy=CalibrationBoardFitting.Fit(points,options,type==7?10:1000);if(legacy.ImageToWorld is not null)legacyOverrideRatio=legacy.ImageToWorld[0]/row.GetProperty("matrix")[0].GetDouble();}
   report.Add(new{boardType=type,index=id,physicalSize,legacyOverrideRatio,result.ErrorCode,matrixFloatBitsExact=matrix,meanFloatBitsExact=mean,parametersFloatBitsExact=parameters,axesFloatBitsExact=axes,actualMatrix=result.Matrix,actualMean=result.OffsetError,actualParameters=values});
  }
  File.WriteAllText("analysis/calibration-board/encoded-original-point-fit-differences.json",JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));
  check(failures.Count==0,"actual encoded board original points and public PhysicalSize fit: "+string.Join(",",failures));
 }
}
