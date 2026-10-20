using System.Text.Json;
using VisionPlugins.Calibration;
try{
 using var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/distortion-calibration-fixed-native-map-evidence.json"));int cases=0,fields=0;
 foreach(var row in evidence.RootElement.GetProperty("cases").EnumerateArray()){
  int count=row.GetProperty("count").GetInt32();byte[] bytes=Convert.FromBase64String(row.GetProperty("pointPairsBase64").GetString()!);if(bytes.Length!=count*16)throw new Exception("Point storage length differs.");
  var pairs=Enumerable.Range(0,count).Select(i=>(BitConverter.ToSingle(bytes,i*8),BitConverter.ToSingle(bytes,i*8+4),BitConverter.ToSingle(bytes,count*8+i*8),BitConverter.ToSingle(bytes,count*8+i*8+4))).ToArray();
  var options=row.GetProperty("options").EnumerateArray().Select(v=>v.GetSingle()).ToArray();var actual=NPointSolver.FitNativePoints(pairs,(int)options[1],(int)options[2],options[5]);
  if(options[3]!=50||options[4]!=1e-4f)throw new Exception("Fixture does not establish the API defaults.");
  if((actual.ErrorCode==0)!=(row.GetProperty("returnRax").GetInt32()==1)||actual.Matrix is null)throw new Exception("Native fit success differs.");
  var expected=row.GetProperty("matrix").EnumerateArray().Select(v=>v.GetSingle()).ToArray();
  for(int i=0;i<9;i++){if(BitConverter.SingleToInt32Bits(expected[i])!=BitConverter.SingleToInt32Bits(actual.Matrix[i]))throw new Exception($"Native field differs: {row.GetProperty("source")}/{row.GetProperty("sequence")} H[{i}] {actual.Matrix[i]:R}/{expected[i]:R}.");fields++;}cases++;
 }
 if(cases!=36||fields!=324)throw new Exception("Native primary fixture coverage changed.");
 using var zero=JsonDocument.Parse(File.ReadAllText("docs/plugins/distortion-calibration-zero-residual-evidence.json"));int boundaries=0;
 foreach(var row in zero.RootElement.GetProperty("cases").EnumerateArray()){
  var options=row.GetProperty("optionsInt32Bits").EnumerateArray().Select(x=>x.GetInt32()).ToArray();
  var points=row.GetProperty("points").EnumerateArray().Select(x=>x.EnumerateArray().Select(v=>v.GetSingle()).ToArray()).Select(p=>(p[0],p[1],p[2],p[3])).ToArray();
  var actual=NPointSolver.FitNativePoints(points,options[1],options[2],BitConverter.Int32BitsToSingle(options[5]),options[3],BitConverter.Int32BitsToSingle(options[4]));
  bool success=row.GetProperty("nativeCode").GetInt32()==1;
  if((actual.ErrorCode==0)!=success||(actual.Matrix is not null)!=success)throw new Exception($"Native zero-residual failure boundary {row.GetProperty("call")} differs.");boundaries++;
 }
 if(boundaries!=34)throw new Exception("Board native boundary fixture coverage changed.");
 Console.WriteLine($"Source Board7ed0c0 fixed-fit failure boundaries {boundaries}/34, including zero residual singular IRLS.");
 Console.WriteLine($"Fixed 7ff3c0: {cases} real primary calls, {fields}/{fields} float fields bit-exact; WeightFun1/2/3 and all three freedoms, defaults 50 / 1e-4.");return 0;
}catch(Exception error){Console.Error.WriteLine(error);return 1;}
