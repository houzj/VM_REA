using VisionCore;

namespace VisionPlugins.CalibrationBoard;

public sealed record CalibrationBoardGradients(short[] X,short[] Y);
public sealed record CalibrationBoardLinks(int[] Previous,int[] Next);
public sealed record CalibrationBoardEdgePoints(IReadOnlyList<(float X,float Y)> Points,int[] PixelIndex,float HighThreshold);
public sealed record CalibrationBoardContour(int Kind,float Length,float Strength,IReadOnlyList<(float X,float Y)> Points);

/// <summary>Source-derived circle refinement stages. Gradient parity does not establish contour parity.</summary>
public static class CalibrationBoardCircleEdges
{
    public static IReadOnlyList<CalibrationBoardContour> Contours(GrayImage image,int low,int high,CancellationToken token=default)
    {
        var state=NonMaximumState(image,low,high,token);var points=state.Points;var links=LinkPixels(image,points,state.PixelIndex);
        var previous=links.Previous;var next=links.Next;int width=image.Width;
        int Pixel((float X,float Y) point)=>(int)(point.Y+.5f)*width+(int)(point.X+.5f);
        for(int i=0;i<points.Count;i++)
        {
            int pixel=Pixel(points[i]),a=previous[i],b=next[i];
            if(a!=-1&&pixel!=next[state.PixelIndex[a]])previous[i]=-1;
            if(b!=-1&&pixel!=previous[state.PixelIndex[b]])next[i]=-1;
        }
        var gradients=Gradients(image,token);var magnitude=new float[gradients.X.Length];
        for(int i=0;i<magnitude.Length;i++)magnitude[i]=MathF.Sqrt((float)gradients.X[i]*gradients.X[i]+(float)gradients.Y[i]*gradients.Y[i])*.22360679507255554f;
        var contours=new List<CalibrationBoardContour>();
        for(int pass=0;pass<2;pass++)for(int start=0;start<points.Count;start++)
        {
            if(next[start]==-1||(pass==0?previous[start]!=-1:previous[start]==-1))continue;
            var path=new List<(float X,float Y)>{points[start]};int current=start;float length=0,strength=magnitude[Pixel(points[start])]+0f;
            while(next[current]!=-1)
            {
                token.ThrowIfCancellationRequested();int pixel=next[current],index=state.PixelIndex[pixel];var a=points[current];var b=points[index];
                path.Add(b);strength+=magnitude[pixel];float dx=a.X-b.X,dy=a.Y-b.Y;length+=MathF.Sqrt(dx*dx+dy*dy);
                next[current]=-1;previous[index]=-1;current=index;
                if(path.Count>points.Count+2)throw new InvalidOperationException("标定板轮廓连接超过候选点范围。");
            }
            if(strength>=state.HighThreshold*.22360679507255554f)contours.Add(new(pass==0?2:1,length,strength,path));
        }
        return contours;
    }

    public static CalibrationBoardLinks LinkPixels(GrayImage image,IReadOnlyList<(float X,float Y)> points,int[]? pixelIndex=null)
    {
        // 8b1b10 chooses neighbours in gradient-sector-specific priority order.
        int width=image.Width;var gradients=Gradients(image);var grid=Enumerable.Repeat(-1,width*image.Height).ToArray();
        if(pixelIndex is not null)grid=pixelIndex;
        else for(int i=0;i<points.Count;i++)grid[(int)(points[i].Y+.5f)*width+(int)(points[i].X+.5f)]=i;
        var previous=new int[points.Count];var next=new int[points.Count];
        int n=-width,s=width,w=-1,e=1,nw=n-1,ne=n+1,sw=s-1,se=s+1;
        for(int i=0;i<points.Count;i++)
        {
            int pixel=(int)(points[i].Y+.5f)*width+(int)(points[i].X+.5f),gx=gradients.X[pixel],gy=gradients.Y[pixel];
            int[] a,b;
            if(gy<=0&&gx>0){if(gx > -gy){a=[n,w,nw,ne];b=[s,e,se,sw];}else{a=[w,n,nw,sw];b=[e,s,se,ne];}}
            else if(gy>=0&&gx<0){if(gx < -gy){a=[s,e,se,sw];b=[n,w,nw,ne];}else{a=[e,s,se,ne];b=[w,n,nw,sw];}}
            else if(gy<0){if(gx<=gy){a=[s,w,sw,se];b=[n,e,ne,nw];}else{a=[w,s,sw,nw];b=[e,n,ne,se];}}
            else{if(gx>=gy){a=[n,e,ne,nw];b=[s,w,sw,se];}else{a=[e,n,ne,se];b=[w,s,sw,nw];}}
            int Choose(int[] offsets){foreach(int offset in offsets){int candidate=pixel+offset;if(candidate>=0&&candidate<grid.Length&&grid[candidate]>=0)return candidate;}return -1;}
            previous[i]=Choose(a);next[i]=Choose(b);
        }
        return new(previous,next);
    }

    public static IReadOnlyList<(float X,float Y)> NonMaximumPoints(GrayImage image,int low,int high,CancellationToken token=default)=>NonMaximumState(image,low,high,token).Points;

    public static CalibrationBoardEdgePoints NonMaximumState(GrayImage image,int low,int high,CancellationToken token=default)
    {
        var gradients=Gradients(image,token);int width=image.Width,height=image.Height;var magnitude=new float[gradients.X.Length];
        for(int i=0;i<magnitude.Length;i++)magnitude[i]=MathF.Sqrt((float)gradients.X[i]*gradients.X[i]+(float)gradients.Y[i]*gradients.Y[i]);
        var flags=Enumerable.Repeat(-1,magnitude.Length).ToArray();var points=new List<(float X,float Y)>();
        float lower=low*4.4721360206604f,upper=high*4.4721360206604f;
        if(low==high)
        {
            float maximum=magnitude.Max();if(maximum==0)return new([],flags,0);
            var histogram=new int[64];float scale=63/maximum;
            foreach(float value in magnitude)histogram[(int)(value*scale+.5f)]++;
            int accumulated=0,bin=0,cut=(int)(magnitude.Length*.699999988079071f);
            for(;bin<64;bin++){accumulated+=histogram[bin];if(cut<accumulated)break;}
            upper=(bin+1)*maximum*.015625f;lower=upper*.4000000059604645f;
        }
        for(int y=1;y<height-1;y++)for(int x=1;x<width-1;x++)
        {
            token.ThrowIfCancellationRequested();int index=y*width+x,gx=gradients.X[index],gy=gradients.Y[index];if(gx==0&&gy==0)continue;
            float ratio,forward,backward,angle;
            float At(int dx,int dy)=>magnitude[index+dy*width+dx];
            if(gx>0&&gy>=0)
            {
                if(gx>=gy){ratio=(float)gy/gx;forward=At(1,0)*(1-ratio)+At(1,1)*ratio;backward=At(-1,0)*(1-ratio)+At(-1,-1)*ratio;angle=MathF.Atan(ratio);}
                else{ratio=(float)gx/gy;forward=At(0,1)*(1-ratio)+At(1,1)*ratio;backward=At(0,-1)*(1-ratio)+At(-1,-1)*ratio;angle=1.5707963705062866f-MathF.Atan(ratio);}
            }
            else if(gx<=0&&gy>0)
            {
                if(gx < -gy){ratio=-(float)gy/gx;forward=At(-1,0)*(1-ratio)+At(-1,1)*ratio;backward=At(1,0)*(1-ratio)+At(1,-1)*ratio;angle=3.1415927410125732f-MathF.Atan(ratio);}
                else{ratio=-(float)gx/gy;forward=At(0,1)*(1-ratio)+At(-1,1)*ratio;backward=At(0,-1)*(1-ratio)+At(1,-1)*ratio;angle=MathF.Atan(ratio)+1.5707963705062866f;}
            }
            else if(gx<0&&gy<=0)
            {
                if(gx<=gy){ratio=(float)gy/gx;forward=At(-1,0)*(1-ratio)+At(-1,-1)*ratio;backward=At(1,0)*(1-ratio)+At(1,1)*ratio;angle=MathF.Atan(ratio)-3.1415927410125732f;}
                else{ratio=(float)gx/gy;forward=At(0,-1)*(1-ratio)+At(-1,-1)*ratio;backward=At(0,1)*(1-ratio)+At(1,1)*ratio;angle=-1.5707963705062866f-MathF.Atan(ratio);}
            }
            else
            {
                if(gx > -gy){ratio=-(float)gy/gx;forward=At(1,0)*(1-ratio)+At(1,-1)*ratio;backward=At(-1,0)*(1-ratio)+At(-1,1)*ratio;angle=-MathF.Atan(ratio);}
                else{ratio=-(float)gx/gy;forward=At(0,-1)*(1-ratio)+At(1,-1)*ratio;backward=At(0,1)*(1-ratio)+At(-1,1)*ratio;angle=MathF.Atan(ratio)-1.5707963705062866f;}
            }
            float value=magnitude[index];if(!(forward<value&&backward<=value&&lower<=value))continue;
            float offset=((forward-backward)*.5f*-.5f)/((forward+backward)*.5f-value);
            points.Add((x+MathF.Cos(angle)*offset,offset*MathF.Sin(angle)+y));flags[index]=value>=upper?2:1;
        }
        var queue=new Queue<int>();for(int i=0;i<flags.Length;i++)if(flags[i]==2)queue.Enqueue(i);
        while(queue.TryDequeue(out var index))for(int dy=-1;dy<=1;dy++)for(int dx=-1;dx<=1;dx++)
        {int neighbor=index+dy*width+dx;if(neighbor>=0&&neighbor<flags.Length&&flags[neighbor]==1){flags[neighbor]=2;queue.Enqueue(neighbor);}}
        // Source removes residual weak points through rounded subpixel coordinates,
        // rather than the original NMS pixel. Half-pixel axis edges can therefore
        // survive even when no strong seed exists; preserve that observed branch.
        var retained=new List<(float X,float Y)>();
        foreach(var point in points)
        {
            int index=(int)(point.Y+.5f)*width+(int)(point.X+.5f);
            if(flags[index]==1){flags[index]=-1;continue;}retained.Add(point);
        }
        for(int i=0;i<retained.Count;i++)flags[(int)(retained[i].Y+.5f)*width+(int)(retained[i].X+.5f)]=i;
        return new(retained,flags,upper);
    }

    public static bool FitCircleContour(IReadOnlyList<(float X,float Y)> points,out (float X,float Y) center)
    {
        // 877b20 includes the closing duplicate. 7075e0 forms A' A and A' b;
        // 730be0 performs left-associated float products, then 730e90 full pivoting.
        center=default;if(points.Count<3)return false;
        var normal=new float[3,3];var right=new float[3];
        foreach(var point in points)
        {
            float[] row=[point.X,point.Y,1];float target=-(point.X*point.X)-point.Y*point.Y;
            for(int i=0;i<3;i++){right[i]+=row[i]*target;for(int j=0;j<3;j++)normal[i,j]+=row[i]*row[j];}
        }
        try{CalibrationBoardCoordinates.Invert(normal);}catch(InvalidOperationException){return false;}
        float x=0,y=0;for(int j=0;j<3;j++){x+=normal[0,j]*right[j];y+=normal[1,j]*right[j];}
        center=(-.5f*x,-.5f*y);return true;
    }

    public static CalibrationBoardGradients Gradients(GrayImage image,CancellationToken token=default)
    {
        // Original 0x1806a7570 / 0x1806a6c50: signed Sobel, zero image border.
        var gx=new short[checked(image.Width*image.Height)];var gy=new short[gx.Length];
        for(int y=1;y<image.Height-1;y++)
        {
            token.ThrowIfCancellationRequested();
            for(int x=1;x<image.Width-1;x++)
            {
                gx[y*image.Width+x]=(short)(image[x+1,y-1]-image[x-1,y-1]+2*(image[x+1,y]-image[x-1,y])+image[x+1,y+1]-image[x-1,y+1]);
                gy[y*image.Width+x]=(short)(image[x-1,y+1]-image[x-1,y-1]+2*(image[x,y+1]-image[x,y-1])+image[x+1,y+1]-image[x+1,y-1]);
            }
        }
        return new(gx,gy);
    }
}
