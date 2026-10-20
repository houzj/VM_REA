using VisionCore;
namespace VisionPlugins.Calibration;

// REVIEW CANDIDATE ONLY. No image decoder fallback is hidden behind this API.
// 813ca0 + 80e690/80bcd0 + 813020, core SHA85d17ecc...549.
internal static class NImageSourceRecognitionBridge {
 internal readonly record struct FloatPoint(float X,float Y);
 internal sealed record Proposal(int Radius,int CenterColumn,int CenterRow,int Direction,
  IReadOnlyList<FloatPoint> Opening);
 // True is exactly record+f0==1, not a geometry confidence threshold.
 internal sealed record DecodeResult(bool StatusOne,byte[] Payload,string? UnsupportedReason=null);
 internal sealed record Selection(Proposal Proposal,NImageBoardIdentity Identity,int AnchorColumn,int AnchorRow);
 internal sealed record Result(Selection? Selected,IReadOnlyList<NImageBoardPoint> Points,string? UnsupportedReason);

 // project maps ORIGINAL integer IDs to source-image coordinates: the wrapper's
 // 7ff3c0 homography + 800670 float projection must supply this operation.
 // decode must be 80a7f0, including its internal opening fallback/refinement and
 // 86ff00 preprocessing. Passing independent TryDecode here does not close R04.
 // prefix is explicit: radius2/column1 reads column-1. Do not fabricate empty IDs.
 internal static Result Recognize(NImageSourceGridContext grid,int boardType,
  Func<float,float,FloatPoint> project,Func<Proposal,DecodeResult> decode,
  Func<int,int,NImageSourceGridCell?>? prefix=null,CancellationToken cancel=default){
  for(int radius=1;radius<3;radius++){
   float scaled=(2*radius+1)*(2*radius+1)*0.6665999889373779f;
   int threshold=(int)(scaled+(scaled>=0?.5f:-.5f));
   // Original row starts radius, COLUMN starts ONE in both radii.
   for(int row=radius;row<grid.Height-radius;row++)for(int col=1;col<grid.Width-radius;col++){
    cancel.ThrowIfCancellationRequested();int missing=0,unknown=0;
    for(int y=row-radius;y<=row+radius;y++)for(int x=col-radius;x<=col+radius;x++){
     if(x==col&&y==row)continue;
     // Native rowPtr[y] is B+y*width*8, with no per-row padding.
     // A negative column in a later row addresses the preceding row's last cell.
     int linear=y*grid.Width+x;
     NImageSourceGridCell? cell=linear>=0&&linear<grid.Cells.Count?grid.Cells[linear]:prefix?.Invoke(x,y);
     if(cell is null){unknown++;continue;}
     if(cell.Value.Column==int.MinValue||cell.Value.Row==int.MinValue)missing++;
    }
    // Each unknown pair contributes exactly zero or one to the native OR-of-two
    // INT_MIN predicate. Decide only if every possible prefix has the same branch.
    if(missing<threshold){
     if(missing+unknown<threshold)continue;
     return new(null,[],$"813ca0 missing-count interval straddles threshold: radius={radius}, row={row}, col={col}, known={missing}, unknown={unknown}, threshold={threshold}");
    }
    // param5=minRow,param6=minColumn in the caller's stack ABI.
    int left=grid.MinColumn+col-radius,top=grid.MinRow+row-radius;
    int right=grid.MinColumn+col+radius,bottom=grid.MinRow+row+radius;
    FloatPoint[] corners=[project(left,top),project(right,top),project(right,bottom),project(left,bottom)];
    for(int direction=1;direction<=8;direction++){
     int start=(direction-1)&3,step=direction<=4?1:-1;
     var opening=Enumerable.Range(0,4).Select(i=>corners[(start+step*i+8)&3]).ToArray();
     var proposal=new Proposal(radius,col,row,direction,Array.AsReadOnly(opening));
     var decoded=decode(proposal);if(decoded.UnsupportedReason is not null)return new(null,[],decoded.UnsupportedReason);if(!decoded.StatusOne)continue;
     if(decoded.Payload.Length<6)return new(null,[],"Decoder status1 lacks source-required six payload bytes");
     var identity=NImageBoardIdentity.FromDecodedBytes(decoded.Payload,boardType);
     // 80e690/80bcd0 select the correct corner; NOT nearest opening geometry.
     int centerX=grid.MinColumn+col+radius,centerY=grid.MinRow+row+radius;
     int anchorX=direction is 1 or 4 or 5 or 8?centerX-2*radius:centerX;
     int anchorY=direction is 1 or 2 or 5 or 6?centerY-2*radius:centerY;
     var selection=new Selection(proposal,identity,anchorX,anchorY);
     return new(selection,Map(grid,selection),null);
    }
   }
  }
  return new(null,[],null);
 }
 // 80f9a0 /80cfc0 inline search: adjacent occupied cells, not813ca0 missing windows.
 internal static Result RecognizeCells(NImageSourceGridContext grid,int boardType,Func<Proposal,DecodeResult> decode,CancellationToken cancel=default){
  for(int row=0;row<grid.Height-1;row++)for(int col=0;col<grid.Width-1;col++){
   cancel.ThrowIfCancellationRequested();
   NImageSourceGridCell[] cells=[grid.Cells[row*grid.Width+col],grid.Cells[row*grid.Width+col+1],grid.Cells[(row+1)*grid.Width+col+1],grid.Cells[(row+1)*grid.Width+col]];
   // Both actual wrappers test FIRST identity only for these four corners.
   if(cells.Any(p=>p.Column==int.MinValue))continue;
   if(cells.Any(p=>p.X is null||p.Y is null))return new(null,[],"Occupied adjacent-cell proposal lacks original float XY");
   for(int direction=1;direction<=8;direction++){
    int start=(direction-1)&3,step=direction<=4?1:-1;
    var opening=Enumerable.Range(0,4).Select(i=>{var p=cells[(start+step*i+8)&3];return new FloatPoint(p.X!.Value,p.Y!.Value);}).ToArray();
    var proposal=new Proposal(0,col,row,direction,Array.AsReadOnly(opening));var decoded=decode(proposal);
    if(decoded.UnsupportedReason is not null)return new(null,[],decoded.UnsupportedReason);if(!decoded.StatusOne)continue;
    if(decoded.Payload.Length<6)return new(null,[],"Decoder status1 lacks source-required six payload bytes");
    var identity=NImageBoardIdentity.FromDecodedBytes(decoded.Payload,boardType);
    // C loops increment row/col before the success switch. Re-express its
    // (min-1)+(candidate+1) without applying an extra decrement to the anchor.
    int anchorX=grid.MinColumn+col+(direction is 2 or 3 or 6 or 7?1:0);
    int anchorY=grid.MinRow+row+(direction is 3 or 4 or 7 or 8?1:0);
    var selected=new Selection(proposal,identity,anchorX,anchorY);return new(selected,Map(grid,selected),null);
   }
  }
  return new(null,[],null);
 }
 static IReadOnlyList<NImageBoardPoint> Map(NImageSourceGridContext grid,Selection selected){
  var result=new List<NImageBoardPoint>();int d=selected.Proposal.Direction;
  void Add(int x,int y){var cell=grid.Cells[y*grid.Width+x];
   // 813020 tests the FIRST identity only, unlike proposal missing count.
   if(cell.Column==int.MinValue)return;
   if(cell.X is not float px||cell.Y is not float py)throw new InvalidOperationException("Occupied source cell lacks XY");
   var id=selected.Identity.WorldIndex(cell.Column,cell.Row,selected.AnchorColumn,selected.AnchorRow,d);
   result.Add(new(id.X,id.Y,px,py));
  }
  // 813020 traversal pairs are shared by 1/5,2/6,3/7,4/8.
  // Preserve cell traversal; do not sort by transformed world identity.
  if(d is 1 or 5)for(int y=0;y<grid.Height;y++)for(int x=0;x<grid.Width;x++)Add(x,y);
  else if(d is 2 or 6)for(int x=grid.Width-1;x>=0;x--)for(int y=0;y<grid.Height;y++)Add(x,y);
  else if(d is 3 or 7)for(int y=grid.Height-1;y>=0;y--)for(int x=grid.Width-1;x>=0;x--)Add(x,y);
  else for(int x=0;x<grid.Width;x++)for(int y=grid.Height-1;y>=0;y--)Add(x,y);
  return result.AsReadOnly();
 }
}

