using System.IO;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionCore;
using VisionPlugins.Calibration;
using VisionPlugins.CalibrationBoard;

static class Program
{
 [STAThread]static void Main(string[] args)
 {
  int type=int.Parse(args[0]),index=int.Parse(args[1]);
  using var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/calibration-board-encoded-roi-rotated-evidence.json"));
  var source=evidence.RootElement.GetProperty("sources").EnumerateArray().Single(s=>s.GetProperty("boardType").GetInt32()==type);
  var gray=new FormatConvertedBitmap(BitmapFrame.Create(new Uri(Path.GetFullPath(source.GetProperty("image").GetString()!))),PixelFormats.Gray8,null,0);
  var bytes=new byte[gray.PixelWidth*gray.PixelHeight];gray.CopyPixels(bytes,gray.PixelWidth,0);var image=new GrayImage(gray.PixelWidth,gray.PixelHeight,bytes);
  var row=File.ReadLines("docs/plugins/calibration-board-encoded-roi-rotated-fixtures.jsonl").Select(l=>JsonDocument.Parse(l).RootElement.Clone()).Single(r=>r.GetProperty("boardType").GetInt32()==type&&r.GetProperty("index").GetInt32()==index);
  var mask=CalibrationBoardRoiMask.Rectangle(image.Width,image.Height,row.GetProperty("cx").GetSingle(),row.GetProperty("cy").GetSingle(),row.GetProperty("width").GetSingle(),row.GetProperty("height").GetSingle(),row.GetProperty("angle").GetSingle());
  var crop=new byte[mask.Width*mask.Height];for(int y=0;y<mask.Height;y++)image.Pixels.Slice((mask.Top+y)*image.Width+mask.Left,mask.Width).CopyTo(crop.AsSpan(y*mask.Width,mask.Width));
  var lattice=NImageBoardDetector.FindLattice(new(mask.Width,mask.Height,crop),default,type is 6 or 8?1:2);
  if(lattice is null){Console.WriteLine("No lattice");return;}
  var points=CalibrationBoardMaskedGrid.Project(lattice.Points,mask);
  var quads=lattice.CodeQuadrilaterals.Select(q=>(IReadOnlyList<Point2>)q.Select(p=>new Point2((float)p.X+mask.Left,(float)p.Y+mask.Top)).ToArray()).ToArray();
  var decoded=NImageBoardRecognition.RecognizeLattice(image,type,new(lattice.PitchX,lattice.PitchY,points,quads));
  var original=row.GetProperty("points").EnumerateArray().Select(p=>new NImageBoardPoint(p[0].GetInt32(),p[1].GetInt32(),p[2].GetSingle(),p[3].GetSingle())).ToArray();
  var actual=decoded?.Points??[];
  var unmatchedActual=actual.Where(p=>!original.Any(q=>Math.Abs(q.X-p.X)<.02&&Math.Abs(q.Y-p.Y)<.02)).ToArray();
  var unmatchedOriginal=original.Where(p=>!actual.Any(q=>Math.Abs(q.X-p.X)<.02&&Math.Abs(q.Y-p.Y)<.02)).ToArray();
  var indexMismatches=actual.Select(p=>new{actual=p,original=original.FirstOrDefault(q=>Math.Abs(q.X-p.X)<.02&&Math.Abs(q.Y-p.Y)<.02)}).Where(p=>p.original!=default&&(p.actual.IndexX!=p.original.IndexX||p.actual.IndexY!=p.original.IndexY)).ToArray();
  var result=new{type,index,bounds=new[]{mask.Left,mask.Top,mask.Width,mask.Height},lattice.PitchX,lattice.PitchY,raw=lattice.Points,projected=points,quads,original,actual,tags=decoded?.Tags,unmatchedActual,unmatchedOriginal,indexMismatches};
  File.WriteAllText(args[2],JsonSerializer.Serialize(result,new JsonSerializerOptions{WriteIndented=true}));
  Console.WriteLine($"type={type} case={index}: original={original.Length} actual={actual.Count} extra={unmatchedActual.Length} missing={unmatchedOriginal.Length} mismatchedIds={indexMismatches.Length}");
 }
}
