using VisionCore;
namespace VisionPlugins.Calibration;

/// <summary>Independent 3x3 median with clipped windows and rounded even medians at edges; original FilterStatus=1 preprocessing.</summary>
public static class NImageMedian
{
 public static GrayImage Apply(GrayImage image,CancellationToken cancellationToken=default){ArgumentNullException.ThrowIfNull(image);var output=new byte[checked(image.Width*image.Height)];Span<byte> window=stackalloc byte[9];for(int y=0;y<image.Height;y++){if((y&31)==0)cancellationToken.ThrowIfCancellationRequested();for(int x=0;x<image.Width;x++){int k=0;for(int dy=-1;dy<=1;dy++)for(int dx=-1;dx<=1;dx++)if(x+dx>=0&&x+dx<image.Width&&y+dy>=0&&y+dy<image.Height)window[k++]=image[x+dx,y+dy];var values=window[..k];values.Sort();output[y*image.Width+x]=k%2==1?values[k/2]:(byte)((values[k/2-1]+values[k/2]+1)/2);}}return new(image.Width,image.Height,output);}
}

