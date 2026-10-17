using VisionCore;
namespace VisionPlugins.Calibration;

public sealed record NImageDetectedTag(NImageBoardIdentity Identity,IReadOnlyList<Point2> Opening,IReadOnlyList<Point2> Fiducials);
public readonly record struct NImageBoardPoint(int IndexX,int IndexY,double X,double Y);
public sealed record NImageDetectedBoard(float GridPitch,IReadOnlyList<NImageBoardPoint> Points,IReadOnlyList<NImageDetectedTag> Tags);

/// <summary>Independent upright HIK-I/II detection branch; arbitrary rotations and perspective remain separate work.</summary>
public static class NImageBoardRecognition
{
 public static NImageDetectedBoard? Find(GrayImage image,int boardType,CancellationToken cancellationToken=default,int filterStatus=1)=>
  FindUpright(image,boardType,cancellationToken,filterStatus)??FindProjective(image,boardType,cancellationToken,filterStatus)??NImageRotatedBoardRecognition.Find(image,boardType,cancellationToken,filterStatus);
 public static NImageDetectedBoard? FindOrthogonal(GrayImage image,int boardType,CancellationToken cancellationToken=default,int filterStatus=1){
  for(int turn=0;turn<4;turn++){
   cancellationToken.ThrowIfCancellationRequested();int width=(turn&1)==0?image.Width:image.Height,height=(turn&1)==0?image.Height:image.Width;var pixels=new byte[width*height];
   Point2 Original(double x,double y)=>turn switch{0=>new(x,y),1=>new(y,image.Height-1-x),2=>new(image.Width-1-x,image.Height-1-y),_=>new(image.Width-1-y,x)};
   for(int y=0;y<height;y++)for(int x=0;x<width;x++){var p=Original(x,y);pixels[y*width+x]=image[(int)p.X,(int)p.Y];}
   var board=FindUpright(new GrayImage(width,height,pixels),boardType,cancellationToken,filterStatus);if(board==null)continue;
   return new(board.GridPitch,Array.AsReadOnly(board.Points.Select(p=>{var q=Original(p.X,p.Y);return new NImageBoardPoint(p.IndexX,p.IndexY,q.X,q.Y);}).ToArray()),Array.AsReadOnly(board.Tags.Select(t=>new NImageDetectedTag(t.Identity,Array.AsReadOnly(t.Opening.Select(p=>Original(p.X,p.Y)).ToArray()),Array.AsReadOnly(t.Fiducials.Select(p=>Original(p.X,p.Y)).ToArray()))).ToArray()));
  }
  return null;
 }
 public static NImageDetectedBoard? FindUprightI(GrayImage image,int boardType=7,CancellationToken cancellationToken=default,int filterStatus=1){if(boardType is not (7 or 9))throw new ArgumentOutOfRangeException(nameof(boardType));return FindUpright(image,boardType,cancellationToken,filterStatus);}
 public static NImageDetectedBoard? FindUpright(GrayImage image,int boardType,CancellationToken cancellationToken=default,int filterStatus=1){
  if(boardType is not (6 or 7 or 8 or 9))throw new ArgumentOutOfRangeException(nameof(boardType));
  var lattice=NImageBoardDetector.FindLattice(image,cancellationToken,boardType is 6 or 8?1:2,filterStatus);return lattice==null?null:Recognize(image,boardType,lattice,cancellationToken);
 }
 public static NImageDetectedBoard? FindProjective(GrayImage image,int boardType,CancellationToken cancellationToken=default,int filterStatus=1){if(boardType is not (6 or 7 or 8 or 9))throw new ArgumentOutOfRangeException(nameof(boardType));var lattice=NImageProjectiveLattice.Find(image,boardType is 6 or 8?1:2,cancellationToken,filterStatus);return lattice==null?null:Recognize(image,boardType,lattice,cancellationToken);}
 static NImageDetectedBoard? Recognize(GrayImage image,int boardType,NImageLattice lattice,CancellationToken cancellationToken){var tags=new List<NImageDetectedTag>();
  foreach(var quad in lattice.CodeQuadrilaterals){cancellationToken.ThrowIfCancellationRequested();var rings=NImageFiducials.Find(image,quad);if(rings==null||!NImageCodeDecoder.TryDecode(image,rings,out var code)||code.Orientation!=0)continue;var identity=NImageBoardIdentity.FromDecodedBytes(code.Bytes.ToArray(),boardType);if(identity.GridPitch<=0||!float.IsFinite(identity.GridPitch))continue;tags.Add(new(identity,quad,rings));}
  if(tags.Count==0)return null;
  var gridFit=NPointSolver.Solve(lattice.Points.Select(p=>new NPointPair(p.X,p.Y,p.Column,p.Row)).ToArray(),[],new(1,1));if(gridFit.ErrorCode!=0)return null;
  (int Column,int Row) Nearest(Point2 point){var index=NPointSolver.Transform(gridFit.Matrix,point.X,point.Y);return ((int)Math.Round(index.X),(int)Math.Round(index.Y));}
  var first=tags[0];int anchorX=Nearest(first.Opening[0]).Column,anchorY=Nearest(first.Opening[0]).Row;
  foreach(var tag in tags){if(tag.Identity.GridPitch!=first.Identity.GridPitch)return null;var expected=first.Identity.WorldIndex(Nearest(tag.Opening[0]).Column,Nearest(tag.Opening[0]).Row,anchorX,anchorY,1);if(expected!=(tag.Identity.OriginX,tag.Identity.OriginY))return null;}
  var points=lattice.Points.Where(p=>!tags.Any(tag=>Inside(tag.Opening,p.X,p.Y,Math.Min(lattice.PitchX,lattice.PitchY)*.15))).Select(p=>{var index=first.Identity.WorldIndex(p.Column,p.Row,anchorX,anchorY,1);return new NImageBoardPoint(index.X,index.Y,p.X,p.Y);}).OrderBy(p=>p.IndexY).ThenBy(p=>p.IndexX).ToArray();
  return new(first.Identity.GridPitch,Array.AsReadOnly(points),tags.AsReadOnly());
 }
 static bool Inside(IReadOnlyList<Point2> quad,double x,double y,double margin){for(int i=0;i<4;i++){var a=quad[i];var b=quad[(i+1)%4];double dx=b.X-a.X,dy=b.Y-a.Y;if((dx*(y-a.Y)-dy*(x-a.X))/Math.Sqrt(dx*dx+dy*dy)<=margin)return false;}return true;}

}




