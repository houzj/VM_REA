using System.IO;
using System.Text.Json;
using VisionPlugins.Calibration;
public static class NImageRotationCompletenessChecks{
 public static int Run(){int checks=0;void Check(bool pass,string message){checks++;if(!pass)throw new Exception(message);}
  using var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/n-image-numerical-evidence.json"));var pairs=evidence.RootElement.GetProperty("pairs").EnumerateArray().ToArray();var reports=new List<object>();
  foreach(var batch in evidence.RootElement.GetProperty("batches").EnumerateArray()){
   int count=batch.GetProperty("rot").GetInt32();var points=Enumerable.Range(0,count).Select(_=>new Dictionary<(int X,int Y),NImageBoardPoint>()).ToArray();
   for(int i=0;i<count-1;i++)foreach(var corner in pairs[i].GetProperty("corners").EnumerateArray())for(int side=0;side<2;side++){int x=corner.GetProperty("world")[0].GetInt32(),y=corner.GetProperty("world")[1].GetInt32();var pixel=corner.GetProperty("pixels")[side];points[i+side][(x,y)]=new(x,y,pixel[0].GetDouble(),pixel[1].GetDouble());}
   var frames=points.Select((p,i)=>new NImageSolveFrame(new(10,p.Values.OrderBy(v=>v.IndexY).ThenBy(v=>v.IndexX).ToArray(),[]),new(0,0,i*10))).ToArray();var result=NImageSolveRotation.Solve(frames);double centerDifference=Math.Max(Math.Abs(result.CenterX-batch.GetProperty("center")[0].GetDouble()),Math.Abs(result.CenterY-batch.GetProperty("center")[1].GetDouble()));double radialDifference=Math.Abs(result.AverageRadialError-batch.GetProperty("rotError")[0].GetDouble());double maximumDifference=Math.Abs(result.MaximumRadialError-batch.GetProperty("rotError")[1].GetDouble());double angleDifference=Math.Abs(result.AverageAngleError-batch.GetProperty("rotMotion")[0].GetDouble());reports.Add(new{count,pointCounts=points.Select(p=>p.Count).ToArray(),centerDifference,radialDifference,maximumDifference,angleDifference,result});
   Check(result.ErrorCode==0,"Complete adjacent native corners solve");Check(centerDifference<1e-4,"Native complete rotation center "+count+": "+centerDifference);Check(radialDifference<1e-5,"Native radial mean "+count+": "+radialDifference);Check(maximumDifference<1e-5,"Native radial maximum "+count+": "+maximumDifference);Check(angleDifference<1e-5,"Native float angle mean "+count+": "+angleDifference);
  }
  string directory="analysis/full-audit/n-image-calibration/numerical-probe";Directory.CreateDirectory(directory);File.WriteAllText(Path.Combine(directory,"complete-rotation-verification.json"),JsonSerializer.Serialize(new{checks,reports,scope="Original adjacent-pair corners isolate numerical stage; product detector not replaced by fixture points"},new JsonSerializerOptions{WriteIndented=true}));return checks;
 }
}
