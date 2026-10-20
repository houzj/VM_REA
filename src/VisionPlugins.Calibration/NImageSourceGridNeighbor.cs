namespace VisionPlugins.Calibration;

/// <summary>Source-order 8c0fa0 directional neighbor cost and ordered minimum.
/// Input candidates retain native direction fields; used flag 1 excludes an item.</summary>
public static class NImageSourceGridNeighbor
{
 public static int Select(IReadOnlyList<NImageSourceCandidate> candidates,int reference,ReadOnlySpan<int> used,
  float directionX,float directionY,Span<float> costs,out float score)
 {
  ArgumentNullException.ThrowIfNull(candidates);
  if((uint)reference>=(uint)candidates.Count)throw new ArgumentOutOfRangeException(nameof(reference));
  if(used.Length<candidates.Count||costs.Length<candidates.Count)throw new ArgumentException("Candidate buffers are too short");
  costs[..candidates.Count].Fill(10000000000f);
  var origin=candidates[reference];float slope=-directionY/directionX,squaredSlope=slope*slope+1f;
  for(int i=0;i<candidates.Count;i++)
  {
   if(used[i]==1)continue;var point=candidates[i];
   float dax=point.DirectionAX-origin.DirectionAX,day=point.DirectionAY-origin.DirectionAY;
   float dbx=point.DirectionBX-origin.DirectionBX,dby=point.DirectionBY-origin.DirectionBY;
   float dx=point.X-origin.X,dy=point.Y-origin.Y;
   // Preserve conditional-negation behavior rather than rewriting it as an angular metric.
   float adax=dax>=0?dax:-dax,aday=day>=0?day:-day,adbx=dbx>=0?dbx:-dbx,adby=dby>=0?dby:-dby;
   float distance=MathF.Sqrt(dx*dx+dy*dy);
   float lateral=directionX==0?MathF.Abs(dx):MathF.Abs(dx*slope+dy)/MathF.Sqrt(squaredSlope);
   if(dx*directionX+dy*directionY>=0)
    costs[i]=distance+lateral*6f+(adax+aday+adbx+adby)*distance;
  }
  // Native scans adjacent pairs with asymmetric <= / < ties. Keep that order for NaN as well.
  int selected=0;score=float.MaxValue;
  int pairs=candidates.Count/2;
  for(int i=0;i<pairs;i++)
  {
   int index=i*2;float value=costs[index];
   if(score<=value){value=score;index=selected;}
   selected=index;score=value;
   if(costs[i*2+1]<value){score=costs[i*2+1];selected=i*2+1;}
  }
  if((candidates.Count&1)!=0)
  {
   int index=candidates.Count-1;float value=costs[index];
   if(score<=value){value=score;index=selected;}
   selected=index;score=value;
  }
  return selected;
 }
}
