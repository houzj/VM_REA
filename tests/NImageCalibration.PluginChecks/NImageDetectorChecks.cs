using System.IO;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionCore;
using VisionPlugins.Calibration;
public static class NImageDetectorChecks
{
 public static int Run(){int checks=0;void Check(bool c,string name){checks++;if(!c)throw new Exception(name);}var path=Path.GetFullPath("tests/NImageCalibration.PluginChecks/Fixtures/hik-i-help-derived.png");var decoder=BitmapDecoder.Create(new Uri(path),BitmapCreateOptions.PreservePixelFormat,BitmapCacheOption.OnLoad);var gray=new FormatConvertedBitmap(decoder.Frames[0],PixelFormats.Gray8,null,0);var pixels=new byte[gray.PixelWidth*gray.PixelHeight];gray.CopyPixels(pixels,gray.PixelWidth,0);var image=new GrayImage(gray.PixelWidth,gray.PixelHeight,pixels);using var doc=JsonDocument.Parse(File.ReadAllText("docs/plugins/n-image-calibration-evidence.json"));var native=doc.RootElement.GetProperty("fixtureRuntime").EnumerateArray().First(e=>e.TryGetProperty("board",out var b)&&b.GetInt32()==7&&e.GetProperty("count").GetInt32()==9&&e.GetProperty("code").GetInt32()==0).GetProperty("corners").EnumerateArray().ToArray();double maximum=0;for(int frame=0;frame<9;frame++){int dx=frame%3*10,dy=frame/3*10;var shifted=Enumerable.Repeat((byte)255,pixels.Length).ToArray();for(int y=0;y<image.Height-dy;y++)pixels.AsSpan(y*image.Width,image.Width-dx).CopyTo(shifted.AsSpan((y+dy)*image.Width+dx,image.Width-dx));var found=NImageBoardDetector.FindLattice(new GrayImage(image.Width,image.Height,shifted))??throw new Exception("No checker lattice detected in original-help fixture");Check(found.Points.Count>=native.Length&&found.Points.Count<=180,"bounded raw lattice candidates, before code filtering");Check(found.CodeQuadrilaterals.Count>=4,"code region proposals");foreach(var row in native){var p=row.GetProperty("pixels")[frame];double x=p[0].GetDouble(),y=p[1].GetDouble();double error=found.Points.Min(q=>Math.Sqrt((q.X-x)*(q.X-x)+(q.Y-y)*(q.Y-y)));maximum=Math.Max(maximum,error);Check(error<1,"original subpixel corner covered within 1 pixel");}var recognized=NImageBoardRecognition.FindUprightI(new GrayImage(image.Width,image.Height,shifted))??throw new Exception("Automatic HIK-I recognition failed");Check(recognized.Tags.Count==4,"all four real HIK tags automatically decoded");Check(recognized.GridPitch==10,"decoded physical grid pitch");Check(recognized.Points.Count==native.Length,"decoded board corner count matches original");foreach(var point in recognized.Points){var expected=native.Single(row=>row.GetProperty("world")[0].GetDouble()==point.IndexX&&row.GetProperty("world")[1].GetDouble()==point.IndexY);var pixel=expected.GetProperty("pixels")[frame];Check(Math.Abs(pixel[0].GetDouble()-point.X)<1&&Math.Abs(pixel[1].GetDouble()-point.Y)<1,"automatic original world identifiers and pixels");}}
  Check(NImageBoardDetector.FindLattice(new GrayImage(64,64,new byte[4096]))==null,"blank image has no coded-board lattice");Check(NImageBoardDetector.FindLattice(new GrayImage(32,32,new byte[1024]))==null,"core minimum image size");using var cancelled=new CancellationTokenSource();cancelled.Cancel();try{NImageBoardDetector.FindLattice(image,cancelled.Token);throw new Exception("Cancellation ignored");}catch(OperationCanceledException){checks++;}Console.WriteLine($"NImage raw lattice: all {native.Length} original corners across 9 frames covered; maximum nearest distance {maximum:F6} px. All four tags and world identifiers independently recovered in every frame.");return checks;
 }
}








