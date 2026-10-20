namespace VisionPlugins.GrayMatch;

// bf2610 + parent groupCount1 mask scan. Typed records, not a vendor model serializer.
public static class GrayMatchSmallTemplate
{
    public sealed record Span(int Row,int Start,int End,int Offset,int Sum,int Squares);
    public sealed record MaskSpan(int Row,int Start,int End);
    public sealed record Result(int Stride,int CropX,int CropY,int Width,int Height,
        float ShiftX,float ShiftY,byte[] Image,byte[] Mask,byte[] Selected,
        IReadOnlyList<Span> Spans,IReadOnlyList<MaskSpan> AuxiliaryMask,double Sum,double Squares);
    public static Result Sample(ReadOnlySpan<byte> image,int imageStride,ReadOnlySpan<byte> mask,int maskStride,
        int width,int height,int outputWidth,int outputHeight,float cosine,float sine)
    {
        if(width<=0||height<=0||(width>=64&&height>=64)||imageStride<width||maskStride<width||
            outputWidth<=0||outputHeight<=0||!float.IsFinite(cosine)||!float.IsFinite(sine)||
            MathF.Abs(cosine)>1||MathF.Abs(sine)>1||(long)(height-1)*imageStride+width>image.Length||
            (long)(height-1)*maskStride+width>mask.Length)throw new ArgumentException("Unsupported small-template caller input.");
        int size=checked(outputWidth*outputHeight);
        byte[] pixels=new byte[size],marks=new byte[size];var selected=new List<byte>();var spans=new List<Span>();
        float maxX=width-BitConverter.UInt32BitsToSingle(0x3f7d70a4),maxY=height-BitConverter.UInt32BitsToSingle(0x3f7d70a4);
        float lower=BitConverter.UInt32BitsToSingle(0xbc23d70a);
        float halfX=(outputWidth-1)*0.5f,halfY=(outputHeight-1)*0.5f;
        float originX=-(cosine*halfX)+-(sine*halfY)+0.5f*maxX;
        float originY=-(cosine*halfY)+halfX*sine+0.5f*maxY;
        int minX=outputWidth-1,lastX=0;double sum=0,squares=0;
        for(int y=0;y<outputHeight;y++)
        {
            float xx=originX,yy=originY;int start=-1,offset=0,runSum=0,runSquares=0,x=0;
            void Close(int end){if(start<0)return;spans.Add(new Span(y,start,end,offset,runSum,runSquares));sum+=(double)runSum;squares+=(double)runSquares;minX=Math.Min(minX,start);lastX=Math.Max(lastX,end);start=-1;}
            while(x<outputWidth&&(xx<lower||maxX<xx||yy<lower||maxY<yy)){xx+=cosine;yy-=sine;x++;}
            for(;x<outputWidth&&lower<=xx&&xx<=maxX&&lower<=yy&&yy<=maxY;x++)
            {
                byte mark=mask[(int)(0.5f+yy)*maskStride+(int)(0.5f+xx)];marks[y*outputWidth+x]=mark;
                if(mark==0)Close(x-1);
                else
                {
                    int ix=(int)xx,iy=(int)yy;float fx=xx-ix,fy=yy-iy;
                    int x0=Math.Max(ix-1,0),x1=ix,x2=Math.Min(ix+1,width-1),x3=Math.Min(ix+2,width-1);
                    int y0=Math.Max(iy-1,0),y1=iy,y2=Math.Min(iy+1,height-1),y3=Math.Min(iy+2,height-1);
                    float r0=Row(image[y0*imageStride+x0],image[y0*imageStride+x1],image[y0*imageStride+x2],image[y0*imageStride+x3],fx,false);
                    float r1=Row(image[y1*imageStride+x0],image[y1*imageStride+x1],image[y1*imageStride+x2],image[y1*imageStride+x3],fx,false);
                    float r2=Row(image[y2*imageStride+x0],image[y2*imageStride+x1],image[y2*imageStride+x2],image[y2*imageStride+x3],fx,false);
                    float r3=Row(image[y3*imageStride+x0],image[y3*imageStride+x1],image[y3*imageStride+x2],image[y3*imageStride+x3],fx,true);
                    float value=Row(r0,r1,r2,r3,fy,true)+0.5f;value=MathF.Min(MathF.Max(value,0),255);
                    byte b=(byte)(int)value;pixels[y*outputWidth+x]=b;
                    if(start<0){start=x;offset=selected.Count;runSum=0;runSquares=0;}
                    selected.Add(b);runSum=unchecked(runSum+b);runSquares=unchecked(runSquares+b*b);
                }
                xx+=cosine;yy-=sine;
            }
            Close(x-1);originX+=sine;originY+=cosine;
        }
        if(spans.Count==0)throw new NotSupportedException("Original empty-span output reads undefined record state.");
        int firstY=spans[0].Row,lastY=spans[^1].Row,cw=lastX-minX+1,ch=lastY-firstY+1;
        var rebased=spans.Select(s=>s with{Row=s.Row-firstY,Start=s.Start-minX,End=s.End-minX}).ToArray();
        var auxiliary=new List<MaskSpan>();
        // Parent groupCount1 scan: each output row walks an original mask column bottom-up.
        for(int row=0;row<cw;row++)
        {
            int start=-1;
            for(int x=0;x<ch;x++)
            {
                bool active=marks[(firstY+ch-1-x)*outputWidth+minX+row]!=0;
                if(active&&start<0)start=x;
                if(!active&&start>=0){auxiliary.Add(new MaskSpan(row,start,x-1));start=-1;}
            }
            if(start>=0)auxiliary.Add(new MaskSpan(row,start,ch-1));
        }
        return new Result(outputWidth,minX,firstY,cw,ch,(lastX+minX)*0.5f-halfX,(lastY+firstY)*0.5f-halfY,
            pixels,marks,selected.ToArray(),Array.AsReadOnly(rebased),auxiliary.AsReadOnly(),sum,squares);
    }
    private static float Row(float p0,float p1,float p2,float p3,float t,bool rightGrouped)
    {
        float a=(((p0+p0)-5f*p1)+4f*p2)-p3;
        float b=((p1-p2)*3f+p3)-p0;
        float halfT=0.5f*t;
        return rightGrouped?p1+halfT*((p2-p0)+t*(a+b*t)):p1+((p2-p0)+(a+b*t)*t)*halfT;
    }
}
