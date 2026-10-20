namespace VisionPlugins.Calibration;

/// <summary>One wrapper cell: two INT_MIN identity slots denote a missing cell.
/// Missing XY values are unknown, not a claim that native scratch memory was zero.</summary>
public readonly record struct NImageSourceGridCell(int Column,int Row,float? X,float? Y)
{
 public bool HasIdentity=>Column!=int.MinValue&&Row!=int.MinValue;
}

/// <summary>Immutable per-recognition context corresponding to the 80e690/80bcd0
/// wrapper's dense ID grid. This does not reconstruct native proposals or decoding.</summary>
public sealed class NImageSourceGridContext
{
 public int Width{get;}public int Height{get;}public int MinColumn{get;}public int MinRow{get;}
 public IReadOnlyList<NImageSourceGridCell> Cells{get;}
 public IReadOnlyList<NImageSourceGridPoint> OrderedPoints{get;}
 NImageSourceGridContext(int width,int height,int minColumn,int minRow,NImageSourceGridCell[] cells,NImageSourceGridPoint[] points){
  Width=width;Height=height;MinColumn=minColumn;MinRow=minRow;
  Cells=Array.AsReadOnly(cells);OrderedPoints=Array.AsReadOnly(points);
 }
 // 80e690/80bcd0 mask compaction precedes minima and dense allocation. Preserve
 // the detector-returned dimensions while recomputing minima over retained IDs.
 // XY must already be projected by the caller's source float-add order.
 public NImageSourceGridContext? RebuildForProjectedPoints(IReadOnlyList<NImageSourceGridPoint> points)
  =>FromResult(new(points,Width,Height,float.NaN));
 public static NImageSourceGridContext? FromResult(NImageSourceGridResult grid){
  ArgumentNullException.ThrowIfNull(grid);if(grid.Points.Count==0)return null;
  // The managed source-grid producer is bounded to 300x300. Reject malformed
  // external contexts instead of reproducing native unchecked pointer writes.
  if(grid.Width is <1 or >300||grid.Height is <1 or >300)throw new ArgumentOutOfRangeException(nameof(grid));
  var points=grid.Points.ToArray();int minColumn=points.Min(p=>p.Column),minRow=points.Min(p=>p.Row);
  var cells=new NImageSourceGridCell[checked(grid.Width*grid.Height)];
  Array.Fill(cells,new(int.MinValue,int.MinValue,null,null));
  foreach(var point in points){
   long x=(long)point.Column-minColumn,y=(long)point.Row-minRow;
   if(point.Column==int.MinValue||point.Row==int.MinValue||x<0||x>=grid.Width||y<0||y>=grid.Height)throw new ArgumentException("Point identity lies outside the source grid dimensions.",nameof(grid));
   // Original wrappers normalize the cell address by the minima, preserve the
   // original two IDs, and overwrite XY in input order for duplicate IDs.
   cells[checked((int)(y*grid.Width+x))]=new(point.Column,point.Row,point.X,point.Y);
  }
  return new(grid.Width,grid.Height,minColumn,minRow,cells,points);
 }
}
