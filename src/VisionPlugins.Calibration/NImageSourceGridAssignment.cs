namespace VisionPlugins.Calibration;

public readonly record struct NImageSourcePredictionTriple(int First,int Second,int Third);

/// <summary>8bf960 linear/curved extrapolation and source-ordered greedy one-to-one assignment.</summary>
public static class NImageSourceGridAssignment
{
 public static int Assign(IReadOnlyList<NImageSourceCandidate> candidates,ReadOnlySpan<NImageSourcePredictionTriple> triples,
  Span<int> used,Span<int> assigned,Span<float> predictedXY)
 {
  ArgumentNullException.ThrowIfNull(candidates);
  if(used.Length<candidates.Count||assigned.Length<triples.Length||predictedXY.Length<triples.Length*2)throw new ArgumentException("Prediction buffers are too short");
  var spacing=new float[triples.Length];
  for(int i=0;i<triples.Length;i++)
  {
   var triple=triples[i];if((uint)triple.Second>=(uint)candidates.Count||(uint)triple.Third>=(uint)candidates.Count||triple.First>=candidates.Count)throw new ArgumentException("Prediction requires two valid trailing indices");
   var middle=candidates[triple.Second];var last=candidates[triple.Third];float dx=last.X-middle.X,dy=last.Y-middle.Y;
   spacing[i]=(dx>=0?dx:-dx)+(dy>=0?dy:-dy);
   if(triple.First<0){predictedXY[i*2]=last.X*2f-middle.X;predictedXY[i*2+1]=last.Y*2f-middle.Y;}
   else
   {
    var first=candidates[triple.First];float ex=middle.X-first.X,ey=middle.Y-first.Y;
    float before=MathF.Atan2(ey,ex),after=MathF.Atan2(dy,dx),angle=(after+after)-before;
    float length=MathF.Sqrt(dx*dx+dy*dy),extrapolated=(length+length)-MathF.Sqrt(ex*ex+ey*ey);
    float cosine=MathF.Cos(angle);predictedXY[i*2]=cosine*extrapolated+last.X;
    float sine=MathF.Sin(angle);predictedXY[i*2+1]=extrapolated*sine+last.Y;
   }
  }
  var costs=new float[checked(triples.Length*candidates.Count)];
  for(int row=0;row<triples.Length;row++)for(int column=0;column<candidates.Count;column++)
  {
   float value=float.MaxValue;
   if(used[column]!=1)
   {
    float radius=12.5f;if(spacing[row]*.2f<=12.5f)radius=spacing[row]*.2f;
    var point=candidates[column];float dx=point.X-predictedXY[row*2],dy=point.Y-predictedXY[row*2+1];
    float distance=MathF.Sqrt(dx*dx+dy*dy);if(!(radius<distance))value=distance;
   }
   costs[row*candidates.Count+column]=value;
  }
  int changed=0;
  for(int iteration=0;iteration<triples.Length;iteration++)
  {
   float best=float.MaxValue;int selectedRow=0,selectedColumn=0;
   for(int row=0;row<triples.Length;row++)
   {
    float rowBest=float.MaxValue;int rowColumn=0;
    for(int column=0;column<candidates.Count;column++)if(costs[row*candidates.Count+column]<rowBest){rowBest=costs[row*candidates.Count+column];rowColumn=column;}
    if(rowBest<best){best=rowBest;selectedRow=row;selectedColumn=rowColumn;}
   }
   if(float.MaxValue-best<1f)break;
   for(int column=0;column<candidates.Count;column++)costs[selectedRow*candidates.Count+column]=float.MaxValue;
   for(int row=0;row<triples.Length;row++)costs[row*candidates.Count+selectedColumn]=float.MaxValue;
   assigned[selectedRow]=selectedColumn;used[selectedColumn]=1;changed++;
  }
  return changed;
 }
}
