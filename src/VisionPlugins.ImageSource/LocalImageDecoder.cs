using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionCore;
using VisionIO;

namespace VisionPlugins.ImageSource;

/// <summary>Independent WIC decoding with source-correlated, corpus-verified MONO8 conversion.</summary>
internal static class LocalImageDecoder
{
 public static GrayImage Read(string path)
 {
  using var stream=File.OpenRead(path);
  // Keep the existing strict BMP integrity/resource policy. Native acceptance of
  // a truncated BMP does not establish defined values for its missing pixels.
  int first=stream.ReadByte(),second=stream.ReadByte();stream.Position=0;
  if(first==0xff&&second==0xd8) {
   var decoded=LocalJpegDecoder.DecodeRgb(stream);byte[] mono=new byte[decoded.Width*decoded.Height];
   for(int i=0;i<mono.Length;i++)mono[i]=(byte)((76*decoded.Rgb[i*3]+150*decoded.Rgb[i*3+1]+30*decoded.Rgb[i*3+2])>>8);
   return new GrayImage(decoded.Width,decoded.Height,mono);
  }
  if(first=='B'&&second=='M') {
   _=BmpReader.Read(stream);stream.Position=0;
  }
  try {
   BitmapDecoder decoder;
   try {decoder=BitmapDecoder.Create(stream,BitmapCreateOptions.PreservePixelFormat,BitmapCacheOption.OnLoad);}
   catch(NotSupportedException ex){throw new InvalidDataException("无法识别或解码图像文件。",ex);}
   if(decoder is not (BmpBitmapDecoder or PngBitmapDecoder or TiffBitmapDecoder))throw new NotSupportedException("此图像容器尚未完成原厂对照。");
   var frame=decoder.Frames[0];
   if((long)frame.PixelWidth*frame.PixelHeight>BmpReader.MaxPixels)throw new InvalidDataException("Image exceeds pixel resource limit.");
   var color=new FormatConvertedBitmap(frame,PixelFormats.Bgr24,null,0);
   byte[] pixels=new byte[checked(frame.PixelWidth*frame.PixelHeight)],bgr=new byte[checked(pixels.Length*3)];
   color.CopyPixels(bgr,frame.PixelWidth*3,0);
   // Native corpus: pure R/G/B -> 75/149/29. Existing rounded BT.601 differs.
   // This is an independently implemented integer rule, not recovered vendor source.
   for(int i=0;i<pixels.Length;i++)pixels[i]=(byte)((76*bgr[i*3+2]+150*bgr[i*3+1]+30*bgr[i*3])>>8);
   return new GrayImage(frame.PixelWidth,frame.PixelHeight,pixels);
  } catch(FileFormatException ex) {
   throw new InvalidDataException("Unsupported or invalid image.",ex);
  }
 }
}
