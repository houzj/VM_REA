using System.IO;
using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPlugins.CalibrationBoard;

static class RoiChecks
{
 public static async Task Run(Action<bool,string> check)
 {
  using var fixture=JsonDocument.Parse(File.ReadAllText("docs/plugins/calibration-board-roi-fixtures.json"));
  foreach(var row in fixture.RootElement.GetProperty("rows").EnumerateArray().Where(r=>r.GetProperty("phase").GetString()=="shape"))
  {
   float cx=row.GetProperty("cx").GetSingle(),cy=row.GetProperty("cy").GetSingle(),w=row.GetProperty("width").GetSingle(),h=row.GetProperty("height").GetSingle();
   check((float)(cx-w*.5f)==row.GetProperty("left").GetSingle()&&(float)(cy-h*.5f)==row.GetProperty("top").GetSingle(),"native RectangleF left/top float construction");
  }
  var plugin=new CalibrationBoardPlugin();var parameters=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());parameters["CalibBoardType"]=JsonSerializer.SerializeToElement(2);
  var pixels=Enumerable.Repeat((byte)255,480*400).ToArray();for(int row=0;row<8;row++)for(int col=0;col<10;col++)for(int dy=-9;dy<=9;dy++)for(int dx=-9;dx<=9;dx++)if(dx*dx+dy*dy<81)pixels[(80+row*30+dy)*480+90+col*30+dx]=0;
  var image=new GrayImage(480,400,pixels);var differences=new List<object>();
  foreach(var row in fixture.RootElement.GetProperty("rows").EnumerateArray().Where(r=>r.GetProperty("phase").GetString()=="roi"))
  {
   float left=row.GetProperty("left").GetSingle(),top=row.GetProperty("top").GetSingle(),w=row.GetProperty("width").GetSingle(),h=row.GetProperty("height").GetSingle();
   var roi=new RoiDocument("native",RoiKind.Rectangle,(float)(left+w*.5f),(float)(top+h*.5f),w,h);
   var result=await plugin.CreateRuntime().ExecuteAsync(new("roi-boundary",Environment.CurrentDirectory,new Dictionary<string,PortValue>{{"InputImage",PortValue.Image(image)}},parameters,[roi]),default);
   uint expected=row.GetProperty("code").GetUInt32(),actual=result.Status==ModuleStatus.Success?0:result.NativeErrorCode??uint.MaxValue;int index=row.GetProperty("index").GetInt32();
   check(actual==expected,"native ROI disjoint/zero rounded extent error "+index);
   double? maximum=null;if(expected==0&&actual==0){var xs=result.Outputs["CalibPointX"].As<IReadOnlyList<double>>();var ys=result.Outputs["CalibPointY"].As<IReadOnlyList<double>>();var points=row.GetProperty("points").EnumerateArray().ToArray();check(xs.Count==points.Length,"native ROI successful point count");maximum=Enumerable.Range(0,points.Length).Max(i=>Math.Max(Math.Abs(xs[i]-points[i][0].GetSingle()),Math.Abs(ys[i]-points[i][1].GetSingle())));check(maximum==0,"native ROI source-coordinate points float exact");}
   differences.Add(new{index,originalError=expected,managedError=actual,maximumPointDifference=maximum});
  }
  File.WriteAllText("analysis/calibration-board/roi-differences.json",JsonSerializer.Serialize(differences,new JsonSerializerOptions{WriteIndented=true}));
 }
}
