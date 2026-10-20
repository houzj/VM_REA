namespace VisionCore;

/// <summary>Owned opaque image bytes. Pixel-format tags and payload length are preserved without decoding.</summary>
public sealed class RawImage:IImageData {
 readonly byte[] pixels;
 public int Width{get;}
 public int Height{get;}
 // Zero means that the transport supplied no packed-row stride (for example planar data).
 public int Stride{get;}
 public ImagePixelFormat Format{get;}
 public ReadOnlySpan<byte> Pixels=>pixels;
 public RawImage(int width,int height,ImagePixelFormat format,ReadOnlySpan<byte> data,int stride=0){
  if(width<=0||height<=0||stride<0||data.IsEmpty)throw new ArgumentOutOfRangeException(nameof(width));
  Width=width;Height=height;Format=format;Stride=stride;pixels=data.ToArray();
 }
}

public static class ImagePreview {
 // Preview conversion never changes the cached payload or its original format tag.
 public static IImageData? Packed(IImageData image){
  if(image is GrayImage or RgbImage)return image;
  long pixels=(long)image.Width*image.Height;int format=(int)image.Format;
  if(pixels>int.MaxValue)return null;
  if(format is 1 or 0x01000000 or 0x01080001&&image.Pixels.Length>=pixels)return new GrayImage(image.Width,image.Height,image.Pixels[..(int)pixels]);
  if(pixels>int.MaxValue/3||image.Pixels.Length<pixels*3)return null;
  if(format is 2 or 201 or 0x02180014)return new RgbImage(image.Width,image.Height,image.Pixels[..((int)pixels*3)]);
  if(format is not (3 or 200 or 0x02180021))return null;
  int plane=(int)pixels;byte[] packed=new byte[plane*3];var source=image.Pixels;for(int i=0;i<plane;i++){packed[3*i]=source[i];packed[3*i+1]=source[plane+i];packed[3*i+2]=source[plane*2+i];}return new RgbImage(image.Width,image.Height,packed);
 }
}
