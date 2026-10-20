using System.IO;
using System.Text.Json;
using VisionCore;
using VisionPlugins.CalibrationBoard;
static class CheckerRuntimeChecks {
 public static void Run(Action<bool,string> check){
 var records=File.ReadLines("docs/plugins/calibration-board-native-fixtures.jsonl").Select(l=>JsonDocument.Parse(l).RootElement.Clone()).Where(r=>r.TryGetProperty("board",out var b)&&b.GetInt32()==1).ToArray();
 var pixels=Enumerable.Repeat((byte)255,480*400).ToArray();for(int y=50;y<350;y++)for(int x=60;x<420;x++)pixels[y*480+x]=(byte)((((x-60)/30+(y-50)/30)&1)==0?0:255);
 bool Bits(IEnumerable<float>a,IEnumerable<float>b)=>a.Select(BitConverter.SingleToInt32Bits).SequenceEqual(b.Select(BitConverter.SingleToInt32Bits));
 var report=new List<object>();var failed=new List<int>();
 for(int mode=1;mode<=5;mode++){
 var r=records.Single(r=>r.GetProperty("mode").GetInt32()==mode&&r.TryGetProperty("matrix",out _));var metrics=records.Single(r=>r.GetProperty("mode").GetInt32()==mode&&r.TryGetProperty("phase",out _));
 var result=CalibrationBoardSolver.Solve(new GrayImage(480,400,pixels),new(BoardType:1,CoordinateMode:mode is 2 or 5?2:1,PhysicalSize:mode>=3?2:1,OriginX:mode>=3?2:0,OriginY:mode>=3?-3:0,RotationAngle:mode is 3 or 5?90:0));var p=r.GetProperty("points").EnumerateArray().ToArray();
 bool points=result.Points.Count==p.Length&&result.Points.Select((q,i)=>q.IndexX==p[i][2].GetInt32()&&q.IndexY==p[i][3].GetInt32()&&Bits([(float)q.X,(float)q.Y],[p[i][0].GetSingle(),p[i][1].GetSingle()])).All(v=>v);
 var fit=result.Fit;bool matrix=fit!=null&&Bits(fit.Matrix.Select(v=>(float)v),r.GetProperty("matrix").EnumerateArray().Select(v=>v.GetSingle()));
 bool scalar=false;if(fit!=null){var a=CalibrationBoardAxes.FromMatrix(fit.Matrix.Select(v=>(float)v).ToArray());float[] values=[(float)fit.OffsetError,(float)fit.Scale,(float)fit.DeltaX,(float)fit.DeltaY,(float)fit.Theta,(float)fit.Aspect,(float)fit.Skew,(float)fit.PixelPrecision,a.OriginX,a.OriginY,a.XVectorX,a.XVectorY,a.YVectorX,a.YVectorY];var native=new[]{"error","scale","x","y","rotate","aspect","skew","precision"}.Select(n=>metrics.GetProperty(n).GetSingle()).Concat(new[]{"origin","vx","vy"}.SelectMany(n=>r.GetProperty(n).EnumerateArray().Select(v=>v.GetSingle())));scalar=Bits(values,native);}
 report.Add(new{mode,result.Error,points,matrix,scalar,actual=fit});if(!points||!matrix||!scalar)failed.Add(mode);
 }
 File.WriteAllText("analysis/calibration-board/checker-runtime-differences.json",JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));check(failed.Count==0,"source checker SDK whole tool outputs modes "+string.Join(",",failed));
 }
}
