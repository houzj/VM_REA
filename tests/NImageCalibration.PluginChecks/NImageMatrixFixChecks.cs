using System.IO;
using System.Text.Json;
using VisionPlugins.Calibration;
public static class NImageMatrixFixChecks{
 public static int Run(){int checks=0;using var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/n-image-matrix-fix-evidence.json"));foreach(var row in evidence.RootElement.GetProperty("records").EnumerateArray()){
  var h=row.GetProperty("input").EnumerateArray().Select(v=>v.GetDouble()).ToArray();var result=NImageMatrixFix.Apply(h,10,20,100,200);var expected=row.GetProperty("matrix").EnumerateArray().Select(v=>v.GetDouble()).ToArray();for(int i=0;i<9;i++){checks++;if(Math.Abs(result[i]-expected[i])>1e-6)throw new Exception("Original MatrixFix mismatch "+row.GetProperty("case")+" component "+i);}
  checks++;if(h[2]!=row.GetProperty("input")[2].GetDouble())throw new Exception("Caller matrix mutated");
 }
 return checks;}
}
