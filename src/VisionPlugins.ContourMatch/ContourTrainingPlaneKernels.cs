using System;
namespace VisionPlugins.ContourMatch;
// Source-derived baseline4f8/500 plane primitives. Independent scalar CPU
// adaptation; no claim of all selected SIMD-branch bit parity or full trainer.
public static class ContourTrainingPlaneKernels
{
    public sealed record Planes(short[] Horizontal,short[] Vertical,float[] Magnitude,int Width,int Height);
    public static Planes Compute(ReadOnlySpan<byte> image,int stride,int width,int height)
    {
        if(width<2||height<2||stride<width||(height-1L)*stride+width>image.Length)
            throw new ArgumentException("Training plane cannot supply original two-neighbor boundary domain.");
        int size=checked(width*height);var dx=new short[size];var dy=new short[size];var magnitude=new float[size];
        for(int y=0;y<height;y++)for(int x=0;x<width;x++){
            // Algebraic folding of c03e10's interior, each edge and four
            // explicit corners; all sums remain in signed16 range for bytes.
            int x0=Math.Max(x-1,0),x1=Math.Min(x+1,width-1);
            int y0=Math.Max(y-1,0),y1=Math.Min(y+1,height-1);
            int horizontal=(image[y0*stride+x1]-image[y0*stride+x0])+
                2*(image[y*stride+x1]-image[y*stride+x0])+
                (image[y1*stride+x1]-image[y1*stride+x0]);
            int vertical=(image[y1*stride+x0]-image[y0*stride+x0])+
                2*(image[y1*stride+x]-image[y0*stride+x])+
                (image[y1*stride+x1]-image[y0*stride+x1]);
            int at=y*width+x;dx[at]=(short)horizontal;dy[at]=(short)vertical;
            // c04af0 signed shorts -> int squares/sum -> float -> SQRTSS/PS.
            int sum=horizontal*horizontal+vertical*vertical;
            magnitude[at]=MathF.Sqrt((float)sum);
        }
        return new(dx,dy,magnitude,width,height);
    }
    public static float Maximum(ReadOnlySpan<float> values,ReadOnlySpan<byte> mask,bool masked)
    {
        if(masked&&mask.Length!=values.Length)throw new ArgumentException("Mask extent mismatch.");
        float result=BitConverter.Int32BitsToSingle(unchecked((int)0xff7fffff));
        for(int i=0;i<values.Length;i++){
            // Finite support only: actual output of the byte-gradient stage.
            // Arbitrary NaN reduction order is not hidden as native parity.
            if(!float.IsFinite(values[i]))throw new NotSupportedException("Nonfinite arbitrary max input outside finite gradient domain.");
            if((!masked||mask[i]!=0)&&result<values[i])result=values[i];
        }
        return result;
    }
}
