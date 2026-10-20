namespace VisionPlugins.CalibrationBoard;

/// <summary>7582e0 mode 0: first/last nonzero pixels per row, Y/X order,
/// unique points and source-order Melkman deque. Empty native input reads stale
/// scratch storage; the independent API returns an explicit empty result.</summary>
public static class CalibrationBoardBinaryHull
{
 static int Cross((int X,int Y) a,(int X,int Y) b,(int X,int Y) c)=>unchecked((b.X-a.X)*(c.Y-a.Y)-(c.X-a.X)*(b.Y-a.Y));
 public static IReadOnlyList<(int X,int Y)> Find(ReadOnlySpan<byte> pixels,int width,int height,int stride)
 {
  if(width<1||height<1||stride<width||(long)(height-1)*stride+width>pixels.Length)throw new ArgumentException("二值图像的尺寸或步长无效。");
  var points=new List<(int X,int Y)>();
  for(int y=0;y<height;y++)
  {
   int first=-1,last=-1;for(int x=0;x<width;x++)if(pixels[y*stride+x]!=0){if(first<0)first=x;last=x;}
   if(first<0)continue;points.Add((first,y));if(last!=first)points.Add((last,y));
  }
  if(points.Count<=2)return points.AsReadOnly();
  int second=1,next=2;while(next<points.Count&&Cross(points[0],points[second],points[next])==0){second=next;next++;}
  if(next==points.Count)return Array.AsReadOnly(new[]{points[0],points[second]});
  var deque=new (int X,int Y)[points.Count*2+3];int bottom=points.Count,top=bottom+3;var p=points[next];
  deque[bottom]=p;deque[top]=p;
  if(Cross(points[0],points[second],p)>0){deque[bottom+1]=points[0];deque[bottom+2]=points[second];}
  else{deque[bottom+1]=points[second];deque[bottom+2]=points[0];}
  for(int i=next+1;i<points.Count;i++)
  {
   p=points[i];if(Cross(deque[bottom],deque[bottom+1],p)>0&&Cross(deque[top-1],deque[top],p)>0)continue;
   while(Cross(deque[bottom],deque[bottom+1],p)<=0)bottom++;
   deque[--bottom]=p;
   while(Cross(deque[top-1],deque[top],p)<=0)top--;
   deque[++top]=p;
  }
  return Array.AsReadOnly(deque[(bottom+1)..(top+1)]);
 }
}
