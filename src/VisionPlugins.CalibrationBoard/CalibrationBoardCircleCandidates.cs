using VisionCore;

namespace VisionPlugins.CalibrationBoard;

public sealed record CalibrationBoardCircleCandidate(float X,float Y,bool EdgeFit,float Size);

/// <summary>Source-guided 812930 / 877230 candidate stage; grid matching is a separate stage.</summary>
public static class CalibrationBoardCircleCandidates
{
    sealed record Region(List<(int X,int Y)> Pixels,int MinX,int MinY,int MaxX,int MaxY,float CenterX,float CenterY)
    {public int Area=>Pixels.Count;}

    public static IReadOnlyList<CalibrationBoardCircleCandidate> Find(GrayImage image,int circularity=50,int low=20,int high=40,int dotMode=1,int factor=5,CancellationToken token=default)
    {
        if(factor<1||image.Width/factor<1||image.Height/factor<1)return [];
        var resized=CalibrationBoardPreprocess.Resize(image,image.Width/factor,image.Height/factor,token);
        if(dotMode==2)resized=new(resized.Width,resized.Height,resized.Pixels.ToArray().Select(value=>(byte)~value).ToArray());
        int threshold=CalibrationBoardPreprocess.Otsu(resized);var regions=Regions(resized,threshold,token);
        var substantial=regions.Where(region=>region.Area>4).ToArray();if(substantial.Length==0)return [];
        int maximum=(substantial.Sum(region=>region.Area)/substantial.Length)*8;
        regions=regions.Where(region=>region.Area>=5&&region.Area<maximum).ToList();if(regions.Count==0)return [];
        int minimum=(regions.Sum(region=>region.Area)/regions.Count)>>2;
        regions=regions.Where(region=>region.Area>minimum).ToList();if(regions.Count>2500)return [];
        regions=regions.Where(region=>Circularity(region)>=circularity*.01f).ToList();
        var candidates=new List<CalibrationBoardCircleCandidate>();
        // 877230 initializes 877b20's output once. A positive contour result with
        // a singular 7075e0 solve returns 1 without overwriting this local output.
        (float X,float Y) previousFit=default;
        foreach(var region in regions)
        {
            token.ThrowIfCancellationRequested();int x=Math.Clamp(region.MinX*factor-6,0,image.Width),y=Math.Clamp(region.MinY*factor-6,0,image.Height);
            int width=Math.Min((region.MaxX-region.MinX+1)*factor+12,image.Width-x),height=Math.Min((region.MaxY-region.MinY+1)*factor+12,image.Height-y);
            var pixels=new byte[width*height];for(int row=0;row<height;row++)for(int col=0;col<width;col++)pixels[row*width+col]=image[x+col,y+row];
            var crop=new GrayImage(width,height,pixels);var contours=CalibrationBoardCircleEdges.Contours(crop,low,high,token);var selected=contours.OrderByDescending(contour=>contour.Length).FirstOrDefault();
            (float X,float Y) center=default;bool fit=selected is not null&&selected.Length>0;
            if(fit)
            {
                if(CalibrationBoardCircleEdges.FitCircleContour(selected!.Points,out var fitted))previousFit=fitted;
                center=previousFit;
            }
            if(!fit)
            {
                // 8620c0 -> 7582e0 mode0 ordered binary hull -> 8a9f50/8aa010.
                // Keep the source's float, support ordering and iteration fallback.
                int cropThreshold=CalibrationBoardPreprocess.Otsu(crop);
                var mask=new GrayImage(width,height,pixels.Select(value=>value<cropThreshold?(byte)0:(byte)255).ToArray());
                if(!TryFrameFallback(mask,out center))
                {
                    var hull=CalibrationBoardBinaryHull.Find(mask.Pixels,width,height,width);
                    if(hull.Count==0)throw new NotSupportedException("原厂空二值凸包依赖旧工作区内容，不能作为有效回退结果。");
                    var circle=CalibrationBoardEnclosingCircle.Find(hull);center=(circle.X,circle.Y);
                }
            }
            candidates.Add(new(x+center.X,y+center.Y,fit,(width-20+height)*.25f));
        }
        return candidates;
    }

    public static bool TryFrameFallback(GrayImage binary,out (float X,float Y) center)
    {
        center=default;
        for(int x=0;x<binary.Width;x++)if(binary[x,0]==0||binary[x,binary.Height-1]==0)return false;
        for(int y=0;y<binary.Height;y++)if(binary[0,y]==0||binary[binary.Width-1,y]==0)return false;
        center=((binary.Width-1)*.5f,(binary.Height-1)*.5f);return true;
    }

    static float Circularity(Region region)
    {
        if(region.Area==1)return 1;float radius=0;
        foreach(var point in region.Pixels){float dx=((float)(point.X-region.MinX)-region.CenterX)+region.MinX,dy=((float)(point.Y-region.MinY)-region.CenterY)+region.MinY;radius=Math.Max(radius,dy*dy+dx*dx);}
        return radius==0?0:Math.Min(1,region.Area/(radius*3.1415927410125732f));
    }

    static List<Region> Regions(GrayImage image,int threshold,CancellationToken token)
    {
        int width=image.Width,height=image.Height;var seen=new bool[width*height];var regions=new List<Region>();var queue=new Queue<int>();
        for(int seed=0;seed<seen.Length;seed++)
        {
            if(seen[seed]||image.Pixels[seed]>=threshold)continue;seen[seed]=true;queue.Enqueue(seed);var pixels=new List<(int X,int Y)>();
            int minX=width,minY=height,maxX=0,maxY=0;long sumX=0,sumY=0;
            while(queue.TryDequeue(out int index))
            {
                token.ThrowIfCancellationRequested();int x=index%width,y=index/width;pixels.Add((x,y));minX=Math.Min(minX,x);minY=Math.Min(minY,y);maxX=Math.Max(maxX,x);maxY=Math.Max(maxY,y);sumX+=x;sumY+=y;
                for(int dy=-1;dy<=1;dy++)for(int dx=-1;dx<=1;dx++){int xx=x+dx,yy=y+dy;if(xx<0||yy<0||xx>=width||yy>=height)continue;int neighbor=yy*width+xx;if(seen[neighbor]||image.Pixels[neighbor]>=threshold)continue;seen[neighbor]=true;queue.Enqueue(neighbor);}
            }
            regions.Add(new(pixels,minX,minY,maxX,maxY,(float)((double)sumX/pixels.Count),(float)((double)sumY/pixels.Count)));
        }
        return regions;
    }
}
