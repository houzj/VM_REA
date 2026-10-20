using VisionCore;
namespace VisionPlugins.Calibration;

/// <summary>Combines source-validated grid points with the existing independent code-opening geometry.
/// This adapter does not claim the geometry proposal or payload decoder is source-exact.</summary>
public static class NImageSourceLatticeAdapter
{
 public static NImageLattice? Find(GrayImage image,CancellationToken cancellationToken=default,int codeCellSize=2,int filterStatus=1)
  =>Find(image,30f,135f,cancellationToken,codeCellSize,filterStatus);
 public static NImageLattice? Find(GrayImage image,float minimumAngle,float maximumAngle,CancellationToken cancellationToken=default,int codeCellSize=2,int filterStatus=1)
 {
  ArgumentNullException.ThrowIfNull(image);if(codeCellSize is not (1 or 2))throw new ArgumentOutOfRangeException(nameof(codeCellSize));
  var geometry=NImageBoardDetector.FindLattice(image,cancellationToken,codeCellSize,filterStatus);if(geometry is null)return null;
  var grid=NImageSourceGridPyramid.Find(image,minimumAngle,maximumAngle,filterStatus,cancellationToken:cancellationToken);if(grid.Points.Count==0)return null;
  // Contrast belongs to the legacy geometry detector. No Hessian/direction score is
  // substituted into that field; recognition of an existing lattice does not read it.
  return geometry with {Points=grid.Points.Select(p=>new NImageLatticePoint(p.Column,p.Row,p.X,p.Y,double.NaN)).ToArray(),SourceGrid=NImageSourceGridContext.FromResult(grid)};
 }
 public static NImageLattice? FindSource(GrayImage image,CancellationToken token=default,int codeCellSize=2,int filterStatus=1)=>FindSource(image,30f,135f,token,codeCellSize,filterStatus);
 public static NImageLattice? FindSource(GrayImage image,float minimumAngle,float maximumAngle,CancellationToken token=default,int codeCellSize=2,int filterStatus=1){
  ArgumentNullException.ThrowIfNull(image);if(codeCellSize is not (1 or 2))throw new ArgumentOutOfRangeException(nameof(codeCellSize));
  var grid=NImageSourceGridPyramid.Find(image,minimumAngle,maximumAngle,filterStatus,cancellationToken:token);
  var context=NImageSourceGridContext.FromResult(grid);if(context is null)return null;
  return new(double.NaN,double.NaN,grid.Points.Select(p=>new NImageLatticePoint(p.Column,p.Row,p.X,p.Y,double.NaN)).ToArray(),Array.Empty<IReadOnlyList<Point2>>()){SourceGrid=context};
 }
}
