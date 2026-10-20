namespace VisionCore;

/// <summary>Rasterized rectangle ROI used by native PreproMask region inputs.</summary>
public sealed record RectangleRoiMask(int ImageWidth,int ImageHeight,byte[] Pixels,int Left,int Top,int Width,int Height,IReadOnlyList<(float X,float Y)> Vertices);

/// <summary>Float-order-compatible rectangle rasterization confirmed against native PreproMask inputs.</summary>
public static class RectangleRoiMaskRasterizer
{
 public static RectangleRoiMask Create(int imageWidth,int imageHeight,float cx,float cy,float width,float height,float angle)
 {
  if(imageWidth<1||imageHeight<1||!float.IsFinite(cx+cy+width+height+angle)||width<=0||height<=0)throw new ArgumentOutOfRangeException(nameof(width));
  byte[] pixels=new byte[checked(imageWidth*imageHeight)];var vertices=new List<(float X,float Y)>();
  if(Math.Abs((double)angle)<.0001)
  {
   float left=cx-width*.5f,top=cy-height*.5f,right=(float)(cx+(double)width*.5),bottom=(float)(cy+(double)height*.5);
   if(left<=imageWidth&&top<=imageHeight&&right>=0&&bottom>=0)
   {
    int x=Math.Max(0,(int)((double)left+.5)),y=Math.Max(0,(int)((double)top+.5));int endX=(int)Math.Min(imageWidth,(double)right+.5),endY=(int)Math.Min(imageHeight,(double)bottom+.5);
    for(int row=y;row<endY;row++)if(endX>x)Array.Fill(pixels,(byte)255,row*imageWidth+x,endX-x);
   }
  }
  else
  {
   float radians=(float)(((double)angle/180d)*Math.PI),cos=MathF.Cos(radians),sin=MathF.Sin(radians);
   float left=cx-width*.5f,top=cy-height*.5f,right=width*.5f+cx,bottom=height*.5f+cy;
   foreach(var point in new[]{(X:left,Y:top),(X:right,Y:top),(X:right,Y:bottom),(X:left,Y:bottom)})
   {
    float dx=point.X-cx,dy=point.Y-cy;vertices.Add(((dx*cos-dy*sin)+cx,(sin*dx+dy*cos)+cy));
   }
   var normalized=vertices.Select(p=>(X:p.X/imageWidth,Y:p.Y/imageHeight)).ToArray();
   float minX=1,maxX=0,minY=1,maxY=0;foreach(var p in normalized){minX=Math.Min(minX,p.X);maxX=Math.Max(maxX,p.X);minY=Math.Min(minY,p.Y);maxY=Math.Max(maxY,p.Y);}
   int firstX=Math.Max(0,(int)(imageWidth*minX+.5f)),lastX=Math.Min(imageWidth-1,(int)(imageWidth*maxX+.5f));
   int firstY=Math.Max(0,(int)(imageHeight*minY+.5f)),lastY=Math.Min(imageHeight-1,(int)(imageHeight*maxY+.5f));
   for(int y=firstY;y<=lastY;y++)
   {
    var intersections=new List<float>();
    for(int i=0;i<normalized.Length;i++)
    {
     var point=normalized[i];var previous=normalized[(i+normalized.Length-1)%normalized.Length];float ya=point.Y*imageHeight,yb=previous.Y*imageHeight;
     if(ya<y&&y<=yb||yb<y&&y<=ya)intersections.Add((((y-ya)/((previous.Y-point.Y)*imageHeight))*(previous.X-point.X))*imageWidth+point.X*imageWidth);
    }
    intersections.Sort();for(int i=0;i+1<intersections.Count;i+=2)
    {
     int x=(int)(intersections[i]+1f),endX=(int)intersections[i+1];if(x>lastX)break;if(endX<firstX)continue;x=Math.Max(firstX,x);endX=Math.Min(lastX,endX);if(endX>=x)Array.Fill(pixels,(byte)255,y*imageWidth+x,endX-x+1);
    }
   }
  }
  int boundLeft=imageWidth,boundTop=imageHeight,boundRight=-1,boundBottom=-1;
  for(int y=0;y<imageHeight;y++)for(int x=0;x<imageWidth;x++)if(pixels[y*imageWidth+x]!=0){boundLeft=Math.Min(boundLeft,x);boundRight=Math.Max(boundRight,x);boundTop=Math.Min(boundTop,y);boundBottom=Math.Max(boundBottom,y);}
  return new(imageWidth,imageHeight,pixels,boundLeft,boundTop,Math.Max(0,boundRight+1-boundLeft),Math.Max(0,boundBottom+1-boundTop),vertices);
 }
}
