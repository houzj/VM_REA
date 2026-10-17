using System.IO;
using System.Text.Json;
using VisionCore;
using VisionPlugins.Calibration;
public static class NImageFilterChecks
{
 public static int Run(){int checks=0;using var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/n-image-calibration-filter-evidence.json"));foreach(var row in evidence.RootElement.GetProperty("records").EnumerateArray()){int w=row.GetProperty("width").GetInt32(),h=row.GetProperty("height").GetInt32();var result=NImageMedian.Apply(new GrayImage(w,h,row.GetProperty("input").EnumerateArray().Select(v=>v.GetByte()).ToArray()));var expected=row.GetProperty("output").EnumerateArray().Select(v=>v.GetByte()).ToArray();for(int i=0;i<expected.Length;i++){checks++;if(result.Pixels[i]!=expected[i])throw new Exception("Original median pixel mismatch");}}
  var tiny=NImageMedian.Apply(new GrayImage(1,1,new byte[]{123}));checks++;if(tiny[0,0]!=123)throw new Exception("One-pixel clipped median");return checks;
 }
}
