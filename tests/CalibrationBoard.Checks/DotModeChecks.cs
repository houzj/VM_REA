using System.IO;
using System.Text.Json;
using VisionCore;
using VisionPlugins.CalibrationBoard;

static class DotModeChecks
{
 public static void Run(Action<bool,string> check)
 {
  foreach(string line in File.ReadLines("docs/plugins/calibration-board-dot-mode-fixtures.jsonl"))
  {
   using var doc=JsonDocument.Parse(line);var r=doc.RootElement;int mode=r.GetProperty("dotMode").GetInt32(),contrast=r.GetProperty("contrast").GetInt32();
   byte[] pixels=Enumerable.Repeat(mode==1?(byte)255:(byte)0,480*400).ToArray();for(int y=0;y<8;y++)for(int x=0;x<10;x++)for(int dy=-9;dy<=9;dy++)for(int dx=-9;dx<=9;dx++)if(dx*dx+dy*dy<81)pixels[(80+30*y+dy)*480+90+30*x+dx]=(byte)(mode==1?255-contrast:contrast);
   var solved=CalibrationBoardSolver.Solve(new GrayImage(480,400,pixels),new(BoardType:2,DotMode:mode,EdgeLow:r.GetProperty("low").GetInt32(),EdgeHigh:r.GetProperty("high").GetInt32()));
   check(solved.Error==r.GetProperty("code").GetUInt32(),"original SDK black/white dot recognition error "+r.GetProperty("index").GetInt32());
   if(solved.Error!=0)continue;
   var points=r.GetProperty("points").EnumerateArray().ToArray();
   check(solved.Points.Count==points.Length&&solved.Points.Select((p,i)=>(float)p.X==points[i][0].GetSingle()&&(float)p.Y==points[i][1].GetSingle()&&p.IndexX==points[i][2].GetInt32()&&p.IndexY==points[i][3].GetInt32()).All(v=>v),"original SDK black/white dot all indices/pixels exact");
   check(solved.Fit!.Matrix.Select((v,i)=>BitConverter.SingleToInt32Bits((float)v)==BitConverter.SingleToInt32Bits(r.GetProperty("matrix")[i].GetSingle())).All(v=>v),"original SDK black/white dot matrix float exact");
   check(BitConverter.SingleToInt32Bits((float)solved.Fit.OffsetError)==BitConverter.SingleToInt32Bits(r.GetProperty("error").GetSingle()),"original SDK black/white dot error float exact");
  }
 }
}
