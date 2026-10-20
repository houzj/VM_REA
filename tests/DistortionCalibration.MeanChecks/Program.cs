using System.Text.Json;
using VisionPlugins.Calibration;
try{
 int means=0,pointFields=0,parameterFields=0,failures=0,parameterDifferences=0;
 void Exact(float a,float b,string label){if(BitConverter.SingleToInt32Bits(a)!=BitConverter.SingleToInt32Bits(b))throw new Exception($"{label}: expected {b:R}, actual {a:R}");}
 using var errorEvidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/distortion-calibration-mean-evidence.json"));
 var errorRows=errorEvidence.RootElement.GetProperty("cases").EnumerateArray().ToDictionary(r=>r.GetProperty("sequence").GetInt32(),r=>r.Clone());
 using var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/distortion-calibration-fixed-trace-evidence.json"));
 foreach(var row in evidence.RootElement.GetProperty("cases").EnumerateArray()){
  if(row.GetProperty("returnRax").GetInt64()==0)continue;
  int seq=row.GetProperty("sequence").GetInt32(),count=row.GetProperty("count").GetInt32();
  byte[] bytes=Convert.FromBase64String(row.GetProperty("pointPairsBase64").GetString()!);
  var points=Enumerable.Range(0,count).Select(i=>(BitConverter.ToSingle(bytes,i*8),BitConverter.ToSingle(bytes,i*8+4),BitConverter.ToSingle(bytes,count*8+i*8),BitConverter.ToSingle(bytes,count*8+i*8+4))).ToArray();
  float[] h=row.GetProperty("matrix").EnumerateArray().Select(x=>x.GetSingle()).ToArray(),saved=h.ToArray();
  var original=errorRows[seq];var nativeErrors=Convert.FromBase64String(original.GetProperty("pointErrorsBase64").GetString()!);
  foreach(var actual in new[]{NPointSolver.AnalyzeNativeIrlsMatrix(points,h),NPointSolver.AnalyzeNativeLeastSquaresMatrix(points,h)}){
   if(actual.ErrorCode!=0||actual.PointErrors?.Length!=count)throw new Exception("Analysis rejected original H");
   Exact(actual.MeanInverseError,original.GetProperty("meanError").GetSingle(),$"mean {seq}");means++;
   for(int i=0;i<count;i++){Exact(actual.PointErrors![i],BitConverter.ToSingle(nativeErrors,i*4),$"error {seq}/{i}");pointFields++;}
  }
  for(int i=0;i<9;i++)Exact(h[i],saved[i],"Analysis must preserve supplied raw H");
 }
 using var parameters=JsonDocument.Parse(File.ReadAllText("docs/plugins/distortion-calibration-matrix-parameters-evidence.json"));
 foreach(var row in parameters.RootElement.GetProperty("cases").EnumerateArray()){
  var h=row.GetProperty("matrix").EnumerateArray().Select(x=>x.GetSingle()).ToArray();var saved=h.ToArray();
  var result=NPointSolver.MatrixToNativeParameters(h);bool success=row.GetProperty("coreCode").GetInt32()==1;
  if((result.ErrorCode==0)!=success||(result.Parameters is not null)!=success)throw new Exception("Decomposition failure boundary");
  if(!row.GetProperty("coreInputPreserved").GetBoolean()||(row.GetProperty("sdkError").GetUInt32()==0)!=success)throw new Exception("Original SDK/core boundaries differ");
  for(int i=0;i<9;i++)Exact(h[i],saved[i],"Decomposition must preserve supplied raw H");
  if(!success){failures++;continue;}var p=result.Parameters!.Value;float[] actual=[p.Scale,p.DeltaX,p.DeltaY,p.Theta,p.Aspect,p.Skew];
  foreach(string name in new[]{"coreValues","sdkValues"}){var expected=row.GetProperty(name).EnumerateArray().Select(x=>x.GetSingle()).ToArray();for(int i=0;i<6;i++){if(BitConverter.SingleToInt32Bits(actual[i])!=BitConverter.SingleToInt32Bits(expected[i])){Console.Error.WriteLine($"Parameter difference {row.GetProperty("sequence")}/{name}/{i}: {actual[i]:R} expected {expected[i]:R}");parameterDifferences++;}parameterFields++;}}
 }
 if(NPointSolver.AnalyzeNativeIrlsMatrix([],new float[9]).ErrorCode==0)throw new Exception("Invalid analysis accepted");
 if(parameterDifferences!=0)throw new Exception($"Parameter differences: {parameterDifferences}");Console.WriteLine($"Original-H analysis: {means}/164 means, {pointFields} point fields bit-exact; MatrixToParam {parameterFields} SDK/core fields exact, {failures} failure boundaries.");return 0;
}catch(Exception error){Console.Error.WriteLine(error);return 1;}
