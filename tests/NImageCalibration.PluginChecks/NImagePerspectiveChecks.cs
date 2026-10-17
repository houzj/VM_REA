using System.IO;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionCore;
using VisionPlugins.Calibration;
public static class NImagePerspectiveChecks
{
 public static int Run(GrayImage image,NImageDetectedBoard original,bool export=false){
  Point2 Project(double x,double y){double d=1+.00015*x+.00008*y;return new((x+.08*y+180)/d,(.04*x+y+150)/d);}
  var corners=new[]{new Point2(0,0),new Point2(image.Width-1,0),new Point2(image.Width-1,image.Height-1),new Point2(0,image.Height-1)};
  var inverse=NPointSolver.Solve(corners.Select(p=>{var q=Project(p.X,p.Y);return new NPointPair(q.X,q.Y,p.X,p.Y);}).ToArray(),[],new(1,1));if(inverse.ErrorCode!=0)throw new Exception("Synthetic projective inverse failed");
  const int size=1400;var pixels=Enumerable.Repeat((byte)255,size*size).ToArray();for(int y=0;y<size;y++)for(int x=0;x<size;x++){var p=NPointSolver.Transform(inverse.Matrix,x,y);int ix=(int)Math.Floor(p.X),iy=(int)Math.Floor(p.Y);if(ix<0||iy<0||ix+1>=image.Width||iy+1>=image.Height)continue;double a=p.X-ix,b=p.Y-iy;pixels[y*size+x]=(byte)Math.Round(image[ix,iy]*(1-a)*(1-b)+image[ix+1,iy]*a*(1-b)+image[ix,iy+1]*(1-a)*b+image[ix+1,iy+1]*a*b);}
  if(export){string folder=Path.GetFullPath("analysis/full-audit/n-image-calibration/perspective-generator");Directory.CreateDirectory(folder);for(int frame=0;frame<9;frame++){int dx=frame%3*10,dy=frame/3*10;var shifted=Enumerable.Repeat((byte)255,pixels.Length).ToArray();for(int y=0;y<size-dy;y++)pixels.AsSpan(y*size,size-dx).CopyTo(shifted.AsSpan((y+dy)*size+dx,size-dx));var bitmap=BitmapSource.Create(size,size,96,96,PixelFormats.Gray8,null,shifted,size);var encoder=new BmpBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(bitmap));using var output=File.Create(Path.Combine(folder,$"perspective-{frame}.bmp"));encoder.Save(output);}}
  var warped=new GrayImage(size,size,pixels);var lattice=NImageProjectiveLattice.Find(warped,2);Console.WriteLine($"Perspective graph {lattice?.Points.Count} points, {lattice?.CodeQuadrilaterals.Count} proposals");var board=NImageBoardRecognition.FindProjective(warped,7)??throw new Exception("Perspective board not decoded");Console.WriteLine($"Perspective recognized {board.Points.Count} points, {board.Tags.Count} tags");int checks=1;double maximum=0;foreach(var point in board.Points){var source=original.Points.Single(p=>p.IndexX==point.IndexX&&p.IndexY==point.IndexY);var expected=Project(source.X,source.Y);double error=Math.Sqrt(Math.Pow(expected.X-point.X,2)+Math.Pow(expected.Y-point.Y,2));maximum=Math.Max(maximum,error);if(error>1)throw new Exception($"Projective corner differs {error}");checks++;}Console.WriteLine($"Perspective known transform maximum pixel error {maximum:F6}");using(var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/n-image-calibration-generated-evidence.json"))){var native=evidence.RootElement.GetProperty("perspective").GetProperty("native");if(board.Points.Count!=native.GetProperty("pairs").GetInt32())throw new Exception("Native perspective count differs");checks++;maximum=0;foreach(var point in board.Points){var expected=native.GetProperty("corners").EnumerateArray().Single(r=>r.GetProperty("world")[0].GetInt32()==point.IndexX&&r.GetProperty("world")[1].GetInt32()==point.IndexY).GetProperty("pixels")[0];double error=Math.Sqrt(Math.Pow(point.X-expected[0].GetDouble(),2)+Math.Pow(point.Y-expected[1].GetDouble(),2));maximum=Math.Max(maximum,error);if(error>1)throw new Exception("Native perspective pixel differs");checks++;}Console.WriteLine($"Perspective original maximum pixel error {maximum:F6}");}return checks;
 }
}



