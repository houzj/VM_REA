using System.IO;
using System.Text.Json;
using VisionPlugins.Calibration;
public static class NImageBoardFitChecks{
 public static int Run(){using var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/n-image-numerical-evidence.json"));var native=evidence.RootElement.GetProperty("officialNPointBoardFits").EnumerateArray().ToArray();var original=evidence.RootElement.GetProperty("translationCases").EnumerateArray().Single(r=>r.GetProperty("rot").GetInt32()==0);var reports=new List<object>();int exact=0,checks=0;
  for(int i=0;i<9;i++){var pairs=original.GetProperty("corners").EnumerateArray().Select(c=>new NPointPair(c.GetProperty("pixels")[i][0].GetDouble(),c.GetProperty("pixels")[i][1].GetDouble(),c.GetProperty("world")[0].GetDouble()*10,c.GetProperty("world")[1].GetDouble()*10)).ToArray();var fit=NPointSolver.Solve(pairs,[],new(2,1));var matrix=native[i].GetProperty("matrix").EnumerateArray().Select(v=>v.GetDouble()).ToArray();var differences=fit.Matrix.Select((v,j)=>Math.Abs(v-matrix[j])).ToArray();exact+=fit.Matrix.Select((v,j)=>(float)v==(float)matrix[j]?1:0).Sum();checks++;if(fit.ErrorCode!=0||fit.Matrix.Where((v,j)=>(float)v!=(float)matrix[j]).Any())throw new Exception("Original board matrix float-bit mismatch at frame "+i);reports.Add(new{frame=i,maximumDifference=differences.Max(),differences,native=matrix,independent=fit.Matrix});}
  Directory.CreateDirectory("analysis/full-audit/n-image-calibration/numerical-probe");File.WriteAllText("analysis/full-audit/n-image-calibration/numerical-probe/board-fit-verification.json",JsonSerializer.Serialize(new{checks,exactFloatComponents=exact,totalComponents=81,reports},new JsonSerializerOptions{WriteIndented=true}));return checks;
 }
}
