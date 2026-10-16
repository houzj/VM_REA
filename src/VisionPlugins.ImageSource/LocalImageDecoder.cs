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
   var decoded=LocalJpegDecoder.DecodeRgb(stream);
   // Original color-to-MONO command 4 (0x18000c4d0) requires both dimensions >=4.
   // Grayscale JPEG is decoded directly to gray by the original wrapper and bypasses it.
   if(!decoded.IsGrayscale&&(decoded.Width<4||decoded.Height<4))throw new InvalidDataException("彩色图像转 MONO8 要求宽高均不小于4（原厂 0x801000ff）。");
   byte[] mono=new byte[decoded.Width*decoded.Height];
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
   if(decoder is PngBitmapDecoder&&frame.Palette is not null&&frame.Format.BitsPerPixel<8)
    throw new NotSupportedException("低位深调色板 PNG 的原厂行读取存在未定义边界，尚不提供兼容输出。");
   if(frame.Format.BitsPerPixel<8)throw new InvalidDataException("原厂不支持此图像位深（0x80100001）。");
   if(frame.Format==PixelFormats.Gray16) {
    byte[] source=new byte[checked(frame.PixelWidth*frame.PixelHeight*2)],gray=new byte[frame.PixelWidth*frame.PixelHeight];frame.CopyPixels(source,frame.PixelWidth*2,0);
    // Native unsigned 16-bit grayscale conversion takes the high byte, without WIC gamma conversion.
    for(int i=0;i<gray.Length;i++)gray[i]=source[i*2+1];return new GrayImage(frame.PixelWidth,frame.PixelHeight,gray);
   }
   if(decoder is TiffBitmapDecoder&&frame.Format==PixelFormats.Indexed8) {
    byte[] indices=new byte[frame.PixelWidth*frame.PixelHeight];frame.CopyPixels(indices,frame.PixelWidth,0);return new GrayImage(frame.PixelWidth,frame.PixelHeight,indices);
   }
   bool grayscale=frame.Format==PixelFormats.Gray8||decoder is BmpBitmapDecoder&&frame.Format.BitsPerPixel==8;
   if(!grayscale&&(frame.PixelWidth<4||frame.PixelHeight<4))throw new InvalidDataException("彩色图像转 MONO8 要求宽高均不小于4（原厂 0x801000ff）。");
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
