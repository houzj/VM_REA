using System.IO;
using System.Text.Json;
using VisionPlugins.CalibrationBoard;

static class WindowChecks
{
 public static void Run(Action<bool,string> check)
 {
  foreach(string line in File.ReadLines("docs/plugins/calibration-board-circle-window-fixtures.jsonl"))
  {
   using var row=JsonDocument.Parse(line);var r=row.RootElement;int index=r.GetProperty("index").GetInt32();
   var input=r.GetProperty("inputs").EnumerateArray().Select(p=>new CalibrationBoardCircleGridPoint(p[0].GetInt32(),p[1].GetInt32(),p[2].GetInt32(),0,0,8.5f)).ToArray();
   var result=CalibrationBoardCircleGrid.SelectWindow(input,r.GetProperty("limit")[0].GetInt32(),r.GetProperty("limit")[1].GetInt32());
   var expected=r.GetProperty("outputs").EnumerateArray().ToArray();
   check(result.Width==r.GetProperty("dimensions")[0].GetInt32()&&result.Height==r.GetProperty("dimensions")[1].GetInt32(),"source maximum density window dimensions "+index);
   check(result.Points.Count==expected.Length&&result.Points.Select((p,i)=>p.CandidateId==expected[i][0].GetInt32()&&p.IndexX==expected[i][1].GetInt32()&&p.IndexY==expected[i][2].GetInt32()).All(v=>v),"source maximum density window ids/order/indices "+index);
  }
 }
}
