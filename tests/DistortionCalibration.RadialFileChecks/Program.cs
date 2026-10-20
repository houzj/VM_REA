using System.Text.Json;
using VisionCore;
using VisionPlugins.DistortionCalibration;
try{
 using var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/distortion-calibration-pixel-evidence.json"));var reports=new List<object>();
 foreach(var row in evidence.RootElement.GetProperty("cases").EnumerateArray()){
  var native=row.GetProperty("native");var expected=native.GetProperty("matrix").EnumerateArray().Select(v=>v.GetSingle()).ToArray();
  var points=native.GetProperty("points").EnumerateArray().Select(p=>p.EnumerateArray().Select(v=>v.GetDouble()).ToArray()).Select(p=>new DistortionCalibrationPoint((float)p[0],(float)p[1],(int)p[2],(int)p[3])).ToArray();
  int board=native.GetProperty("board").GetInt32(),model=(int)expected[18],warp=native.GetProperty("warp").GetInt32();
  var actual=DistortionCalibrationSolver.SolveFromPointsNativeDetailed(points,(int)expected[16],(int)expected[17],model,1,2,20,new Point2(240,200));
  if(actual.ErrorCode!=0||actual.File is null)throw new Exception($"Source point-set fit failed {board}/{model}/{warp}: {actual.ErrorCode:X8}");
  var values=actual.File.Values.ToArray();int bits=values.Zip(expected).Count(p=>BitConverter.SingleToInt32Bits(p.First)==BitConverter.SingleToInt32Bits(p.Second));double max=values.Zip(expected).Max(p=>Math.Abs((double)p.First-p.Second));
  bool priorException=board==2&&warp==0&&model is 2 or 3;
  if(!priorException&&bits!=19)throw new Exception($"Previously exact source fixture changed {board}/{model}/{warp}: {bits}/19 max={max:R}");
  if(!(board==2&&warp==0&&model==3)&&actual.EstimationError!=native.GetProperty("error").GetSingle())throw new Exception("Previously exact source mean changed");
  reports.Add(new{board,model,warp,bits,max,nativeMean=native.GetProperty("error").GetSingle(),actualMean=actual.EstimationError,expected,actual=values});
  Console.WriteLine($"Source files {board}/{model}/{warp}: {bits}/19; max={max:R}; mean={actual.EstimationError:R}");
 }
 Directory.CreateDirectory("analysis/distortion-calibration/radial-source-order");File.WriteAllText("analysis/distortion-calibration/radial-source-order/file-replay.json",JsonSerializer.Serialize(new{reports,nativeExecution=false,scope="Existing native point-set replay; retains two pre-existing differences explicitly"},new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine($"Radial source-order file replay passed: {reports.Count} original point sets.");return 0;
}catch(Exception e){Console.Error.WriteLine(e);return 1;}
