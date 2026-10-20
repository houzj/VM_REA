using System;
namespace VisionPlugins.ContourMatch;

// Exact integer/float scalar slices from 6817d0/cec3f0/d25450/d285a0.
// These do not supply the edge/pyramid/model training kernel.
public static class ContourTrainingScalars
{
    public readonly record struct Rectangle(int X,int Y,int Width,int Height);
    static void Admit(ReadOnlySpan<byte> data,int stride,int width,int height)
    {
        // Independent bounds safety for managed spans. Original caller has
        // already checked positive dimensions/stride/non-null native pixels.
        if(width<1||height<1||stride<width||(height-1L)*stride+width>data.Length)
            throw new ArgumentException("Invalid accessible training plane extent.");
    }
    public static Rectangle? NonzeroMaskBounds(ReadOnlySpan<byte> mask,int stride,int width,int height)
    {
        Admit(mask,stride,width,height);int left=width,right=-1,top=height,bottom=-1;
        for(int y=0;y<height;y++)for(int x=0;x<width;x++)if(mask[y*stride+x]!=0){
            left=Math.Min(left,x);right=Math.Max(right,x);top=Math.Min(top,y);bottom=Math.Max(bottom,y);
        }
        // Native helper writes an invalid rectangle/status0; its actual
        // caller returns before reading that rectangle. Do not invent zero ROI.
        return bottom<0?null:new(left,top,right-left+1,bottom-top+1);
    }
    public static Rectangle ExpandMaskBounds(Rectangle roi,int width,int height)
    {
        if(roi.X<0||roi.Y<0||roi.Width<1||roi.Height<1||roi.X+(long)roi.Width>width||roi.Y+(long)roi.Height>height)
            throw new ArgumentException("ROI is not inside the training image.");
        float scaledX=roi.Width*0.025f,scaledY=roi.Height*0.025f;
        float shiftedX=scaledX+0.5f,shiftedY=scaledY+0.5f;
        int marginX=Math.Clamp((int)shiftedX,8,25),marginY=Math.Clamp((int)shiftedY,8,25);
        int left=Math.Max(0,roi.X-marginX),top=Math.Max(0,roi.Y-marginY);
        int right=(int)Math.Min(width-1L,roi.X+(long)roi.Width-1+marginX);
        int bottom=(int)Math.Min(height-1L,roi.Y+(long)roi.Height-1+marginY);
        return new(left,top,right-left+1,bottom-top+1);
    }
    public static bool HasMaskedContrast(ReadOnlySpan<byte> image,int imageStride,
        ReadOnlySpan<byte> mask,int maskStride,int width,int height,byte threshold=2)
    {
        Admit(image,imageStride,width,height);Admit(mask,maskStride,width,height);
        int maximum=0,minimum=255;
        // Original scalar tail and SIMD PCMPEQB/PMINUB/PMAXUB agree: every
        // nonzero mask byte selects the corresponding image byte.
        for(int y=0;y<height;y++)for(int x=0;x<width;x++)if(mask[y*maskStride+x]!=0){
            int value=image[y*imageStride+x];maximum=Math.Max(maximum,value);minimum=Math.Min(minimum,value);
        }
        return threshold<=maximum-minimum;
    }
    public static float ExplicitPyramidSupport(int width,int height)
    {
        if(width<1||height<1||(long)width*height>int.MaxValue)
            throw new NotSupportedException("Native signed-area overflow/nonfinite support is outside safe normal training admission.");
        int area=width*height;
        float first=MathF.Sqrt((float)area);float scaled=first*0.125f;
        return MathF.Sqrt(scaled); // Actual result is XMM0 float, not RAX integer.
    }
}
