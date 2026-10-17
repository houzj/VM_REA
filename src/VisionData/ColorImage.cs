namespace VisionCore;

public enum ImagePixelFormat { Mono8=0x01080001, Rgb24=0x02180014 }
public interface IImageData {
 int Width {get;}
 int Height {get;}
 int Stride {get;}
 ImagePixelFormat Format {get;}
 ReadOnlySpan<byte> Pixels {get;}
}
public sealed class RgbImage:IImageData {
 readonly byte[] pixels;
 public int Width {get;}
 public int Height {get;}
 public int Stride=>checked(Width*3);
 public ImagePixelFormat Format=>ImagePixelFormat.Rgb24;
 public ReadOnlySpan<byte> Pixels=>pixels;
 public RgbImage(int width,int height,ReadOnlySpan<byte> data){
  if(width<=0||height<=0)throw new ArgumentOutOfRangeException(nameof(width));
  if(data.Length!=checked(width*height*3))throw new ArgumentException("RGB24 data must contain width * height * 3 bytes.",nameof(data));
  Width=width;Height=height;pixels=data.ToArray();
 }
}
