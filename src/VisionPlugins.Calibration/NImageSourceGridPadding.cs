namespace VisionPlugins.Calibration;

/// <summary>8c00f0 one-sided -1 border insertion, preserving all three score components.</summary>
public static class NImageSourceGridPadding
{
 public static bool AddBorder(NImageSourceGridState state,int direction)
 {
  ArgumentNullException.ThrowIfNull(state);if(direction is <0 or >3)throw new ArgumentOutOfRangeException(nameof(direction));
  int width=state.Width,height=state.Height;if(width<1||height<1)return false;
  bool Occupied(int position)=>state.Indices[position]!=-2&&state.Indices[position]!=-1;
  bool needed=direction switch {
   0=>Enumerable.Range(0,width).Any(x=>Occupied(x)),
   1=>Enumerable.Range(0,height).Any(y=>Occupied(y*300)),
   2=>Enumerable.Range(0,width).Any(x=>Occupied((height-1)*300+x)),
   _=>Enumerable.Range(0,height).Any(y=>Occupied(y*300+width-1))};
  if(!needed)return false;
  // Managed capacity guard: native out-of-workspace writes are not reproduced.
  if(((direction&1)==0&&height>=300)||((direction&1)!=0&&width>=300))throw new InvalidOperationException("Grid workspace capacity exceeded");
  void Copy(int destination,int source)
  {
   state.Indices[destination]=state.Indices[source];
   for(int component=0;component<3;component++)state.Scores[destination*3+component]=state.Scores[source*3+component];
  }
  void Empty(int destination)
  {
   state.Indices[destination]=-1;for(int component=0;component<3;component++)state.Scores[destination*3+component]=float.MaxValue;
  }
  if(direction==0){for(int y=height;y>=0;y--)for(int x=0;x<width;x++){if(y>0)Copy(y*300+x,(y-1)*300+x);else Empty(x);}state.Height++;}
  else if(direction==1){for(int y=0;y<height;y++)for(int x=width;x>=0;x--){if(x>0)Copy(y*300+x,y*300+x-1);else Empty(y*300);}state.Width++;}
  else if(direction==2){for(int x=0;x<width;x++)Empty(height*300+x);state.Height++;}
  else{for(int y=0;y<height;y++)Empty(y*300+width);state.Width++;}
  return true;
 }
}
