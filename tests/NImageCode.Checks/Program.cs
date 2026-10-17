using System.IO;
using System.Text.Json;
using VisionPlugins.Calibration;
static class Program {
 [STAThread] static int Main(string[] args){try{Run(args);return 0;}catch(Exception error){Console.Error.WriteLine(error);return 1;}}
 static void Run(string[] args){
int checks=0;void Check(bool pass,string message){checks++;if(!pass)throw new Exception(message);}
using var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/n-image-code-evidence.json"));
foreach(var row in evidence.RootElement.GetProperty("cases").EnumerateArray()){
 var input=row.GetProperty("input").EnumerateArray().Select(v=>(byte)v.GetInt32()).ToArray();var expected=row.GetProperty("output").EnumerateArray().Select(v=>(byte)v.GetInt32()).ToArray();
 Check(NImageCodeReedSolomon.TryCorrect(input,out var output),"Original-correctable codeword");Check(output.SequenceEqual(expected),"Every corrected native symbol");
 var bits=Enumerable.Range(0,192).Select(i=>(byte)((input[i/8]>>(7-i%8))&1)).ToArray();Check(NImageCodeReedSolomon.TryDecodeBits(bits,out var bytes),"Original bit order");Check(bytes.SequenceEqual(expected.Reverse().Take(12)),"Original twelve exported byte order");
}
Check(!NImageCodeReedSolomon.TryCorrect(new byte[23],out var invalid)&&invalid.Length==0,"Reject malformed symbol count");Check(!NImageCodeReedSolomon.TryDecodeBits(new byte[191],out _),"Reject malformed bit count");var badBits=new byte[192];badBits[33]=2;Check(!NImageCodeReedSolomon.TryDecodeBits(badBits,out _),"Reject nonbinary sample");
Console.WriteLine($"NImageCode.Checks: {checks} checks passed; codeword layer only, no image detector parity claim");

if(args.Contains("--image")){
 var decoder=System.Windows.Media.Imaging.BitmapDecoder.Create(new Uri(Path.GetFullPath("tests/NImageCalibration.PluginChecks/Fixtures/hik-i-help-derived.png")),System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat,System.Windows.Media.Imaging.BitmapCacheOption.OnLoad);var image=new System.Windows.Media.Imaging.FormatConvertedBitmap(decoder.Frames[0],System.Windows.Media.PixelFormats.Gray8,null,0);var pixels=new byte[image.PixelWidth*image.PixelHeight];image.CopyPixels(pixels,image.PixelWidth,0);var gray=new VisionCore.GrayImage(image.PixelWidth,image.PixelHeight,pixels);
 Directory.CreateDirectory("analysis/full-audit/n-image-calibration/code-tag");File.WriteAllBytes("analysis/full-audit/n-image-calibration/code-tag/help-gray.raw",pixels);
 var quad=new[]{new VisionCore.Point2(246,253),new VisionCore.Point2(317,253),new VisionCore.Point2(294,324),new VisionCore.Point2(246,324)};Check(NImageCodeDecoder.TryDecode(gray,quad,out var code),"Real image with manual diagnostic quad");Check(code.Bytes.SequenceEqual(evidence.RootElement.GetProperty("realPixelTag").GetProperty("original").GetProperty("bytes").EnumerateArray().Select(v=>(byte)v.GetInt32())),"Same-quad original twelve bytes exactly");for(int rotation=0;rotation<4;rotation++)for(int mirror=0;mirror<2;mirror++){
 var changed=new byte[pixels.Length];VisionCore.Point2 Transform(VisionCore.Point2 p){double x=mirror==0?p.X:gray.Width-1-p.X,y=p.Y;for(int k=0;k<rotation;k++)(x,y)=(gray.Width-1-y,x);return new(x,y);}
 for(int y=0;y<gray.Height;y++)for(int x=0;x<gray.Width;x++){var p=Transform(new(x,y));changed[(int)p.Y*gray.Width+(int)p.X]=pixels[y*gray.Width+x];}
 Check(NImageCodeDecoder.TryDecode(new(gray.Width,gray.Height,changed),quad.Select(Transform).ToArray(),out var transformed),"Independent tag warp rotation/mirror property");Check(transformed.Bytes.SequenceEqual(code.Bytes),"Independent orientation keeps decoded byte sequence");
 }
 Console.WriteLine("Real-pixel tag checks: 18 passed; includes independent orientation properties; manual diagnostic quad, automatic location separately verified");
}

 }
}
