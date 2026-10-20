using System.IO;
using System.Text.Json;
using VisionPlugins.Calibration;
using VisionPlugins.CalibrationBoard;
var report=new List<object>();
foreach(var line in File.ReadLines("docs/plugins/calibration-board-boundary-fit-stage-fixtures.jsonl")){
 using var d=JsonDocument.Parse(line);var r=d.RootElement;int caller=r.GetProperty("caller").GetInt32();if(caller!=0x7edfce&&caller!=0x7ee6ca)continue;
 var pairs=r.GetProperty("points").EnumerateArray().Select(p=>(p[0].GetSingle(),p[1].GetSingle(),p[2].GetSingle(),p[3].GetSingle())).ToArray();
 var fit=NPointSolver.FitNativePoints(pairs,1,2,2f,20,1e-4f);var candidate=BoundaryFitCandidate.NPointSolver.FitNativePoints(pairs,1,2,2f,20,1e-4f);var expected=r.GetProperty("matrix").EnumerateArray().Select(p=>p.GetSingle()).ToArray();
 bool exact=fit.ErrorCode==0&&fit.Matrix!=null&&fit.Matrix.Select(BitConverter.SingleToInt32Bits).SequenceEqual(expected.Select(BitConverter.SingleToInt32Bits));
 report.Add(new{index=r.GetProperty("index").GetInt32(),sequence=r.GetProperty("sequence").GetInt32(),caller,fit.ErrorCode,exact,candidateExact=candidate.Matrix!=null&&candidate.Matrix.Select(BitConverter.SingleToInt32Bits).SequenceEqual(expected.Select(BitConverter.SingleToInt32Bits)),candidateMatrix=candidate.Matrix,ti=BoundaryFitCandidate.NPointSolver.LastTi,tw=BoundaryFitCandidate.NPointSolver.LastTw,normalizedFinal=BoundaryFitCandidate.NPointSolver.LastNormalizedFinal,iterations=BoundaryFitCandidate.NPointSolver.Iterations,initial=BoundaryFitCandidate.NPointSolver.LastInitial,normalized=BoundaryFitCandidate.NPointSolver.LastNormalized,actual=fit.Matrix,expected});
}
File.WriteAllText("analysis/calibration-board/boundary-fit-direct-replay.json",JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));
Console.WriteLine(JsonSerializer.Serialize(report));



var complete=new List<object>();
foreach(var line in File.ReadLines("docs/plugins/calibration-board-boundary-runtime-fixtures.jsonl")){
 using var d=JsonDocument.Parse(line);var r=d.RootElement;var points=r.GetProperty("points").EnumerateArray().Select(p=>new NImageBoardPoint(p[0].GetInt32(),p[1].GetInt32(),p[2].GetSingle(),p[3].GetSingle())).ToArray();
 var o=new BoardCalibrationOptions(BoardType:2);var stages=ExperimentalBoardFitting.Fit(points,o);var fit=ExperimentalBoardFitting.Analyze(points,o,stages);
 bool Bits(IEnumerable<float>a,IEnumerable<float>b)=>a.Select(BitConverter.SingleToInt32Bits).SequenceEqual(b.Select(BitConverter.SingleToInt32Bits));
 var axes=CalibrationBoardAxes.FromMatrix(fit.Matrix.Select(v=>(float)v).ToArray());
 bool h=Bits(fit.Matrix.Select(v=>(float)v),r.GetProperty("matrix").EnumerateArray().Select(v=>v.GetSingle()));
 float[] actual=[(float)fit.OffsetError,(float)fit.Scale,(float)fit.DeltaX,(float)fit.DeltaY,(float)fit.Theta,(float)fit.Aspect,(float)fit.Skew,(float)fit.PixelPrecision,axes.OriginX,axes.OriginY,axes.XVectorX,axes.XVectorY,axes.YVectorX,axes.YVectorY];
 var expected=new[]{"error","scale","x","y","rotate","aspect","skew","precision"}.Select(n=>r.GetProperty(n).GetSingle()).Concat(new[]{"origin","vx","vy"}.SelectMany(n=>r.GetProperty(n).EnumerateArray().Select(v=>v.GetSingle())));
 complete.Add(new{index=r.GetProperty("index").GetInt32(),matrixExact=h,scalarsExact=Bits(actual,expected),fit.ErrorCode});
}
File.WriteAllText("analysis/calibration-board/boundary-experimental-complete-output.json",JsonSerializer.Serialize(complete,new JsonSerializerOptions{WriteIndented=true}));
Console.WriteLine("Experimental complete outputs "+JsonSerializer.Serialize(complete));

if(complete.Count!=33||complete.Any(v=>{var r=JsonSerializer.SerializeToElement(v);return !r.GetProperty("matrixExact").GetBoolean()||!r.GetProperty("scalarsExact").GetBoolean()||r.GetProperty("ErrorCode").GetUInt32()!=0;}))throw new Exception("Independent source-hoist experiment does not reproduce all33 native complete final outputs.");
