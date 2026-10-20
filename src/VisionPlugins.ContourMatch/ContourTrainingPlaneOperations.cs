using System;
namespace VisionPlugins.ContourMatch;
// Additional necessary deterministic helpers, not a full pyramid estimator.
public static class ContourTrainingPlaneOperations
{
    public static int EstimateLevelBound(int width,int height,int minimumArea=32)
    {
        // Independent safe admission. Native IMUL32 has no overflow protection.
        if(width<1||height<1||width==int.MaxValue||height==int.MaxValue||(long)width*height>int.MaxValue)
            throw new NotSupportedException("Unsafe native training dimension arithmetic.");
        int levels=1,w=(width+1)>>1,h=(height+1)>>1;
        while(minimumArea<=w*h&&h>6&&w>6){w=(w+1)>>1;h=(h+1)>>1;levels++;}
        return levels; // bdf300; d2aeb0 separately caps this result at8.
    }
    public static byte[] CopyRegion(ReadOnlySpan<byte> image,int stride,int width,int height,
        ContourTrainingScalars.Rectangle roi)
    {
        if(width<1||height<1||stride<width||(height-1L)*stride+width>image.Length||
           roi.X<0||roi.Y<0||roi.Width<1||roi.Height<1||
           roi.X+(long)roi.Width>width||roi.Y+(long)roi.Height>height)
            throw new ArgumentException("Invalid accessible training ROI.");
        var result=new byte[checked(roi.Width*roi.Height)];
        for(int y=0;y<roi.Height;y++)image.Slice((roi.Y+y)*stride+roi.X,roi.Width)
            .CopyTo(result.AsSpan(y*roi.Width,roi.Width));
        return result; // bdf580: tight output rows, no filtering/downsample.
    }
}
