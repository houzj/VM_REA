using VisionCore;

namespace VisionPlugins.CalibrationBoard;

public sealed record CalibrationBoardPreprocessResult(GrayImage Image, GrayImage Binary, int Threshold);

/// <summary>Source-derived circle downsampling and automatic threshold stage; contour refinement is separate.</summary>
public static class CalibrationBoardPreprocess
{
    public static CalibrationBoardPreprocessResult Run(GrayImage image, bool invert = false, CancellationToken token = default)
    {
        var half = Resize(image, Math.Max(1,image.Width/2), Math.Max(1,image.Height/2), token);
        if(invert){var inverted=half.Pixels.ToArray();for(int i=0;i<inverted.Length;i++)inverted[i]=(byte)~inverted[i];half=new(half.Width,half.Height,inverted);}
        int threshold=Otsu(half);
        var binary=half.Pixels.ToArray();for(int i=0;i<binary.Length;i++)binary[i]=binary[i]<threshold?(byte)0:(byte)255;
        return new(half,new(half.Width,half.Height,binary),threshold);
    }

    public static GrayImage Resize(GrayImage image, int width, int height, CancellationToken token=default)
    {
        if(width<1||height<1)throw new ArgumentOutOfRangeException(nameof(width));
        if(width==image.Width&&height==image.Height)return image;
        var vertical=Weights(image.Height,height);var horizontal=Weights(image.Width,width);
        // CPU11 0x18075e570 uses four-wide horizontal SIMD except its final 4+remainder columns.
        // Widths <=32 fall back entirely to scalar 0x180726fc0.
        int vectorColumns=image.Width>32&&width>32?((width>>2)-1)*4:0;
        var intermediate=new float[checked(image.Width*height)];var result=new byte[checked(width*height)];
        for(int y=0;y<height;y++)
        {
            token.ThrowIfCancellationRequested();var taps=vertical[y];
            for(int x=0;x<image.Width;x++){float sum=0;foreach(var tap in taps)sum+=image[x,tap.Index]*tap.Weight;intermediate[y*image.Width+x]=sum;}
        }
        for(int y=0;y<height;y++)for(int x=0;x<width;x++)
        {
            float sum=0;foreach(var tap in horizontal[x])sum+=intermediate[y*image.Width+tap.Index]*tap.Weight;
            int rounded=x<vectorColumns?(int)MathF.Round(sum,MidpointRounding.ToEven):(int)((double)sum+.5);
            result[y*width+x]=(byte)Math.Clamp(rounded,0,255);
        }
        return new(width,height,result);
    }

    static (int Index,float Weight)[][] Weights(int input,int output)
    {
        // 0x180727470: antialias-scaled triangle, half-pixel centers, clamped sample indices.
        float scale=(float)output/input,support=output<input?1/scale:1,kernelWidth=output<input?2/scale:2;
        int count=(int)MathF.Ceiling(kernelWidth)+2;var rows=new (int Index,float Weight)[output][];
        for(int i=0;i<output;i++)
        {
            float center=(i+.5f)/scale-.5f;int left=(int)MathF.Floor(center-kernelWidth*.5f);
            var taps=new (int Index,float Weight)[count];float sum=0;
            for(int j=0;j<count;j++){float distance=(center-(left+j))/support;float weight=Math.Max(0,1-MathF.Abs(distance));taps[j]=(Math.Clamp(left+j,0,input-1),weight);sum+=weight;}
            float inverse=1/sum;for(int j=0;j<count;j++)taps[j].Weight*=inverse;rows[i]=taps;
        }
        return rows;
    }

    public static int Otsu(GrayImage image)
    {
        // 0x180724460/0x180724880: float histogram and variance, retaining native order and epsilon.
        var histogram=new float[256];foreach(byte pixel in image.Pixels)histogram[pixel]++;
        float inverse=1f/(image.Width*image.Height);for(int i=0;i<256;i++)histogram[i]*=inverse;
        float mean=0;for(int i=1;i<255;i+=2)mean=mean+i*histogram[i]+(i+1)*histogram[i+1];
        float weight=0,partial=0,maximum=0;int best=0;
        for(int i=0;i<256;i++)
        {
            weight+=histogram[i];if(weight>=1.0000001192092896f)break;
            partial+=i*histogram[i];float difference=(mean+255*histogram[255])*weight-partial;
            float variance=difference*difference/((weight-weight*weight)+1.1920928955078125e-7f);
            if(maximum<variance){maximum=variance;best=i;}
        }
        return best;
    }
}
