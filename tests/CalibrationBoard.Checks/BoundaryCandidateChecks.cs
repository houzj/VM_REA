using System.IO;
using System.Text.Json;
using VisionCore;
using VisionPlugins.CalibrationBoard;

static class BoundaryCandidateChecks
{
 public static void Run(Action<bool,string> check)
 {
  var report=new List<object>();var failed=new List<int>();
  foreach(string line in File.ReadLines("docs/plugins/calibration-board-boundary-candidate-fixtures.jsonl"))
  {
   using var document=JsonDocument.Parse(line);var row=document.RootElement;
   bool gradient=row.TryGetProperty("gradient",out var gradientValue)&&gradientValue.GetBoolean();
   int id=row.GetProperty("index").GetInt32()+(gradient?120:0),start=row.GetProperty("start").GetInt32(),top=row.GetProperty("top").GetInt32(),shape=row.GetProperty("shape").GetInt32(),contrast=row.GetProperty("contrast").GetInt32();
   var pixels=Enumerable.Repeat((byte)255,480*400).ToArray();
   for(int r=0;r<8;r++)for(int c=0;c<10;c++)for(int y=-9;y<=9;y++)for(int x=-9;x<=9;x++)
   {
    bool inside=shape==0?x*x+y*y<81:shape==1?x*x/81f+y*y/25f<1:Math.Abs(x)<9&&Math.Abs(y)<9;
    int px=start+c*30+x,py=(top!=0?start:80)+r*30+y;if(inside&&px>=0&&px<480&&py>=0&&py<400)pixels[py*480+px]=(byte)(255-contrast+(gradient&&x*x+y*y<16?2:0));
   }
   IReadOnlyList<CalibrationBoardCircleCandidate>? actual=null;string? unsupported=null;
   try{actual=CalibrationBoardCircleCandidates.Find(new GrayImage(480,400,pixels),20,row.GetProperty("low").GetInt32(),row.GetProperty("high").GetInt32());}catch(NotSupportedException error){unsupported=error.Message;}
   var expected=row.GetProperty("records").EnumerateArray().ToArray();bool same=actual!=null&&actual.Count==expected.Length;float? maximum=actual?.Count==expected.Length?0:null;
   if(actual!=null&&actual.Count==expected.Length)for(int i=0;i<actual.Count;i++){same&=actual[i].X==expected[i][0].GetSingle()&&actual[i].Y==expected[i][1].GetSingle()&&actual[i].EdgeFit==(expected[i][2].GetSingle()!=0)&&actual[i].Size==expected[i][6].GetSingle();maximum=Math.Max(maximum!.Value,Math.Max(Math.Abs(actual[i].X-expected[i][0].GetSingle()),Math.Abs(actual[i].Y-expected[i][1].GetSingle())));}
   if(!same)failed.Add(id);report.Add(new{index=id,gradient,start,top,shape,contrast,same,unsupported,actualCount=actual?.Count,nativeCount=expected.Length,maximum,actual});
  }
  File.WriteAllText("analysis/calibration-board/boundary-candidate-differences.json",JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));
  check(failed.Count==0,"actual native boundary candidates every coordinate/flag/size: "+string.Join(",",failed));
 }
}
