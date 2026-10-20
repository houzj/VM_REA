using System.IO.Compression;
using System.Text.Json;
using VisionCore;
using VisionPlugins.DistortionCalibration;
try{
 int assertions=0;
 void Check(bool condition,string name){if(!condition)throw new Exception(name);assertions++;}
 var small=DistortionCalibrationRecognition.Recognize(new GrayImage(63,64,new byte[63*64]));
 Check(small.ErrorCode==0x10103054u&&small.Points.Count==0,"Original image dimension guard");
 var invalid=DistortionCalibrationRecognition.Recognize(new GrayImage(64,64,new byte[64*64]),boardType:5);
 Check(invalid.ErrorCode==0xe000030eu&&invalid.Points.Count==0,"Invalid board is not silently remapped");
 var blank=DistortionCalibrationRecognition.Recognize(new GrayImage(480,400,new byte[480*400]));
 Check(blank.ErrorCode==0x10103000u&&blank.Points.Count==0,"Blank checker is not a fabricated result");
 bool cancelled=false;try{DistortionCalibrationRecognition.Recognize(new GrayImage(64,64,new byte[4096]),token:new CancellationToken(true));}catch(OperationCanceledException){cancelled=true;}
 Check(cancelled,"Recognition cancellation");
 byte[] Inflate(string text){using var input=new MemoryStream(Convert.FromBase64String(text));using var zlib=new ZLibStream(input,CompressionMode.Decompress);using var output=new MemoryStream();zlib.CopyTo(output);return output.ToArray();}
 using var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/distortion-calibration-pixel-evidence.json"));
 var reports=new List<object>();int successful=0;
 foreach(var row in evidence.RootElement.GetProperty("cases").EnumerateArray()){
  var native=row.GetProperty("native");var values=native.GetProperty("matrix").EnumerateArray().Select(x=>x.GetSingle()).ToArray();
  int width=(int)values[16],height=(int)values[17],board=native.GetProperty("board").GetInt32();
  var found=DistortionCalibrationRecognition.Recognize(new GrayImage(width,height,Inflate(row.GetProperty("inputZlibBase64").GetString()!)),board);
  Check(found.GridPitch==1f,"Distortion grid pitch stays unit, not board physical metadata");
  if(found.ErrorCode==0)successful++;
  var expected=native.GetProperty("points").EnumerateArray().Select(p=>p.EnumerateArray().Select(v=>v.GetDouble()).ToArray()).ToArray();
  var expectedById=expected.ToDictionary(p=>((int)p[2],(int)p[3]));
  int common=0,bits=0;double maximum=0;
  foreach(var point in found.Points)if(expectedById.TryGetValue((point.GridX,point.GridY),out var p)){
   common++;bits+=BitConverter.SingleToInt32Bits(point.X)==BitConverter.SingleToInt32Bits((float)p[0])?1:0;
   bits+=BitConverter.SingleToInt32Bits(point.Y)==BitConverter.SingleToInt32Bits((float)p[1])?1:0;
   maximum=Math.Max(maximum,Math.Max(Math.Abs(point.X-p[0]),Math.Abs(point.Y-p[1])));
  }
  reports.Add(new{board,model=native.GetProperty("model").GetInt32(),error=found.ErrorCode,expectedCount=expected.Length,actualCount=found.Points.Count,common,bits,maximum});
 }
 Check(successful>0,"At least one real original image is recognized");
 Directory.CreateDirectory("analysis/distortion-calibration/recognition-verification");
 File.WriteAllText("analysis/distortion-calibration/recognition-verification/report.json",JsonSerializer.Serialize(new{scope="Real image recognition differential diagnostic; no full native parity claim",assertions,successful,reports},new JsonSerializerOptions{WriteIndented=true}));
 Console.WriteLine($"Recognition adapter {assertions} assertions; {successful}/{reports.Count} original inputs recognized. Pixel/index differences recorded separately.");return 0;
}catch(Exception error){Console.Error.WriteLine(error);return 1;}
