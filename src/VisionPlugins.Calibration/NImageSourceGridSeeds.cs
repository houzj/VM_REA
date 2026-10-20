namespace VisionPlugins.Calibration;

/// <summary>Algorithm-local grid workspace, not the host ROI/configuration state.</summary>
public sealed class NImageSourceGridState
{
 public const int Stride=300,Capacity=Stride*Stride;
 public int Count {get;set;}
 public int Width {get;set;}
 public int Height {get;set;}
 public int[] Indices {get;}=new int[Capacity];
 public float[] Scores {get;}=new float[Capacity*3];
 // Raw alignment words between the native 90000 flags and the index matrix.
 // Explicit workspace context; this is not a host/persistence parameter.
 public int[] FlagAlignmentPadding {get;}=new int[16];
}

/// <summary>Native grid seed phases. Full directional expansion/selection is separate.</summary>
public static class NImageSourceGridSeeds
{
 public static bool Create3x3(IReadOnlyList<NImageSourceCandidate> candidates,int reference,int[] used,NImageSourceGridState state)
 {
  ArgumentNullException.ThrowIfNull(candidates);ArgumentNullException.ThrowIfNull(used);ArgumentNullException.ThrowIfNull(state);
  if((uint)reference>=(uint)candidates.Count)throw new ArgumentOutOfRangeException(nameof(reference));
  if(used.Length<NImageSourceGridState.Capacity)throw new ArgumentException("Native grid flag capacity is 90000",nameof(used));
  Array.Fill(state.Indices,-2);Array.Clear(used,0,90000);if(candidates.Count<9)return false;
  var costs=new float[candidates.Count];var distances=new float[8];var origin=candidates[reference];
  state.Indices[301]=reference;used[reference]=1;
  int Neighbor(int from,float dx,float dy,out float distance)=>NImageSourceGridNeighbor.Select(candidates,from,used,dx,dy,costs,out distance);
  int left=Neighbor(reference,origin.DirectionAX,origin.DirectionAY,out distances[0]);state.Indices[300]=left;used[left]=1;
  int right=Neighbor(reference,-origin.DirectionAX,-origin.DirectionAY,out distances[1]);state.Indices[302]=right;used[right]=1;
  int top=Neighbor(reference,-origin.DirectionBX,-origin.DirectionBY,out distances[2]);state.Indices[1]=top;used[top]=1;
  int bottom=Neighbor(reference,origin.DirectionBX,origin.DirectionBY,out distances[3]);state.Indices[601]=bottom;used[bottom]=1;
  float Balance(int index,int first,int second)
  {
   var point=candidates[index];var a=candidates[first];var b=candidates[second];
   float dx=point.X-a.X,dy=point.Y-a.Y,ex=point.X-b.X,ey=point.Y-b.Y;
   return MathF.Abs(MathF.Sqrt(dx*dx+dy*dy)-MathF.Sqrt(ex*ex+ey*ey));
  }
  int Corner(int first,float dx,float dy,int second,float ex,float ey,out float distance)
  {
   int a=Neighbor(first,dx,dy,out float costA),b=Neighbor(second,ex,ey,out float costB);
   if(a!=b&&Balance(b,first,second)<Balance(a,first,second)){distance=costB;return b;}
   distance=costA;return a;
  }
  int tl=Corner(left,-origin.DirectionBX,-origin.DirectionBY,top,origin.DirectionAX,origin.DirectionAY,out distances[4]);state.Indices[0]=tl;used[tl]=1;
  int tr=Corner(right,-origin.DirectionBX,-origin.DirectionBY,top,-origin.DirectionAX,-origin.DirectionAY,out distances[5]);state.Indices[2]=tr;used[tr]=1;
  int bl=Corner(left,origin.DirectionBX,origin.DirectionBY,bottom,origin.DirectionAX,origin.DirectionAY,out distances[6]);state.Indices[600]=bl;used[bl]=1;
  int br=Corner(right,origin.DirectionBX,origin.DirectionBY,bottom,-origin.DirectionAX,-origin.DirectionAY,out distances[7]);state.Indices[602]=br;used[br]=1;
  for(int i=0;i<8;i++)if(MathF.Abs(distances[i]-10000000000f)<1f)
  {
   foreach(int index in new[]{tl,top,tr,left,reference,right,bl,bottom,br})used[index]=0;
   Array.Clear(state.Indices);state.Width=state.Height=0;return false;
  }
  state.Count=9;for(int y=0;y<3;y++)for(int x=0;x<3;x++)state.Scores[(y*300+x)*3]=float.MaxValue;
  state.Width=state.Height=3;return true;
 }
 public static bool Create2x2(IReadOnlyList<NImageSourceCandidate> candidates,int reference,int[] used,NImageSourceGridState state)
 {
  ArgumentNullException.ThrowIfNull(candidates);ArgumentNullException.ThrowIfNull(used);ArgumentNullException.ThrowIfNull(state);
  if((uint)reference>=(uint)candidates.Count)throw new ArgumentOutOfRangeException(nameof(reference));
  if(used.Length<NImageSourceGridState.Capacity)throw new ArgumentException("Native grid flag capacity is 90000",nameof(used));
  Array.Fill(state.Indices,-2);var costs=new float[candidates.Count];var distances=new float[3];var origin=candidates[reference];
  state.Indices[0]=reference;used[reference]=1;
  int right=NImageSourceGridNeighbor.Select(candidates,reference,used,-origin.DirectionAX,-origin.DirectionAY,costs,out distances[0]);
  state.Indices[1]=right;used[right]=1;
  int bottom=NImageSourceGridNeighbor.Select(candidates,reference,used,origin.DirectionBX,origin.DirectionBY,costs,out distances[1]);
  state.Indices[300]=bottom;used[bottom]=1;
  var a=candidates[right];var b=candidates[bottom];float best=float.MaxValue;int diagonal=0;
  for(int i=0;i<candidates.Count;i++)
  {
   float dx=((a.X-origin.X)+b.X)-candidates[i].X,dy=((a.Y-origin.Y)+b.Y)-candidates[i].Y;
   float distance=MathF.Sqrt(dx*dx+dy*dy);if(distance<best){diagonal=i;best=distance;}
  }
  distances[2]=best;state.Indices[301]=diagonal;used[diagonal]=1;
  for(int i=0;i<3;i++)if(MathF.Abs(distances[i]-10000000000f)<1f)
  {
   foreach(int index in new[]{reference,right,bottom,diagonal})used[index]=0;
   Array.Clear(state.Indices);state.Width=state.Height=0;
   // Native failure preserves the previous Count and unrelated score cells.
   return false;
  }
  state.Count=4;for(int y=0;y<2;y++)for(int x=0;x<2;x++)state.Scores[(y*300+x)*3]=float.MaxValue;
  state.Width=state.Height=2;return true;
 }
}
