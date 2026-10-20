namespace VisionPlugins.Calibration;

public readonly record struct NImageSourceGridScore(int Column,int Row,int Axis,float Value);

/// <summary>8bdb40 collinear-triple curvature costs and source-ordered worst triple.</summary>
public static class NImageSourceGridScoring
{
 public static NImageSourceGridScore FindWorst(IReadOnlyList<NImageSourceCandidate> candidates,NImageSourceGridState state)
 {
  ArgumentNullException.ThrowIfNull(candidates);ArgumentNullException.ThrowIfNull(state);
  if(state.Width<0||state.Height<0||state.Width>300||state.Height>300)throw new ArgumentOutOfRangeException(nameof(state));
  float negativeCount=-(float)state.Count,best=BitConverter.Int32BitsToSingle(0x00800000);int column=0,row=0,axis=0;
  void Visit(int x,int y,int step,int component)
  {
   int position=y*300+x,a=state.Indices[position],b=state.Indices[position+step],c=state.Indices[position+step*2];
   if(a<0||b<0||c<0)return;
   var first=candidates[a];var middle=candidates[b];var last=candidates[c];
   float ex=first.X-last.X,ey=first.Y-last.Y;
   float dx=(first.X+last.X)-(middle.X+middle.X),dy=(first.Y+last.Y)-(middle.Y+middle.Y);
   float curvature=MathF.Sqrt(dx*dx+dy*dy)/MathF.Sqrt(ex*ex+ey*ey),previous=best;
   best=curvature;if(curvature<=previous)best=previous;
   state.Scores[position*3+component]=(1f-(curvature+curvature))*negativeCount;
   if(previous<curvature){column=x;row=y;axis=component;}
  }
  for(int y=0;y<state.Height;y++)for(int x=0;x<state.Width-2;x++)Visit(x,y,1,0);
  for(int y=0;y<state.Height-2;y++)for(int x=0;x<state.Width;x++)Visit(x,y,300,2);
  return new(column,row,axis,state.Scores[(row*300+column)*3+axis]);
 }
}
