namespace VisionCore;
/// <summary>Source-confirmed extra MONO8 conversion: (76 R + 150 G + 30 B) >> 8.
/// This is distinct from file decoder grayscale paths. Coverage is recorded per consumer.</summary>
public static class ImageColorConversion {
 public static GrayImage ToMono8(IImageData image){
  ArgumentNullException.ThrowIfNull(image);if(image is GrayImage gray)return gray;
  if(image.Format is not (ImagePixelFormat.Mono8 or ImagePixelFormat.Rgb24))throw new NotSupportedException("Unsupported MONO8 conversion source format.");
  int rowBytes=checked(image.Width*(image.Format==ImagePixelFormat.Rgb24?3:1));
  if(image.Width<=0||image.Height<=0||image.Stride<rowBytes||image.Pixels.Length<checked((image.Height-1)*image.Stride+rowBytes))throw new ArgumentException("Invalid image dimensions, stride or data length.",nameof(image));
  var pixels=new byte[checked(image.Width*image.Height)];var source=image.Pixels;
  for(int y=0;y<image.Height;y++)for(int x=0;x<image.Width;x++){
   int at=checked(y*image.Stride+x*(image.Format==ImagePixelFormat.Rgb24?3:1));
   pixels[y*image.Width+x]=image.Format==ImagePixelFormat.Mono8?source[at]:(byte)((76*source[at]+150*source[at+1]+30*source[at+2])>>8);
  }
  return new(image.Width,image.Height,pixels);
 }
}
