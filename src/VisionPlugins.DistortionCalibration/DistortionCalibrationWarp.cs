using VisionCore;
namespace VisionPlugins.DistortionCalibration;

/// <summary>Consumer of the original nineteen-float calibration payload. Mapping follows
/// 18083cef0 and fixed-point bilinear sampling follows 180858640; fit parity is separate.</summary>
public sealed class DistortionCalibrationWarp {
 readonly DistortionCalibrationFile file;
 readonly float[] h;
 public DistortionCalibrationWarp(DistortionCalibrationFile file){this.file=file;h=file.Values.ToArray();}
 public Point2 SourceAt(double x,double y){
  double qx=x,qy=y;
  if(file.Model!=2){double denominator=h[7]*x+h[8]*y+h[9];double reciprocal=Math.Abs(denominator)>2.220446049250313e-16?1/denominator:1;qx=(h[1]*x+h[2]*y+h[3])*reciprocal;qy=(h[4]*x+h[5]*y+h[6])*reciprocal;}
  if(file.Model!=1){double dx=qx-h[10],dy=qy-h[11],r2=dx*dx+h[14]*dy*dy,kx2=(double)h[15]*h[12],ky2=(double)h[15]*h[13];qx=qx+h[12]*dx*r2+dx*kx2*r2*r2;qy=qy+h[13]*dy*r2+dy*ky2*r2*r2;}
  return new((float)qx,(float)qy);
 }
 public IImageData Correct(IImageData input,CancellationToken token=default){
  if(input.Width!=file.Width||input.Height!=file.Height)throw new ArgumentException("标定图像尺寸与输入不一致。",nameof(input));int channels=input.Format==ImagePixelFormat.Mono8?1:input.Format==ImagePixelFormat.Rgb24?3:throw new NotSupportedException("Unsupported distortion correction image format.");
  var output=new byte[checked(input.Width*input.Height*channels)];var pixels=input.Pixels;int maximumIndex=input.Width*input.Height-input.Width-1;
  for(int y=0;y<input.Height;y++){token.ThrowIfCancellationRequested();for(int x=0;x<input.Width;x++){
   var p=SourceAt(x+1,y+1);if(!double.IsFinite(p.X)||!double.IsFinite(p.Y)||p.X<0||p.Y<0||p.X>=input.Width||p.Y>=input.Height)continue;int sx=(int)p.X,sy=(int)p.Y;if(sy*input.Width+sx>maximumIndex)continue;
   int tx=(int)(((float)p.X-sx)*128f),ty=(int)(((float)p.Y-sy)*128f);int a=(128-tx)*(128-ty),b=(128-ty)*tx,c=(128-tx)*ty,d=tx*ty;
   for(int channel=0;channel<channels;channel++){int at=sy*input.Stride+sx*channels+channel;output[(y*input.Width+x)*channels+channel]=(byte)((a*pixels[at]+b*pixels[at+channels]+c*pixels[at+input.Stride]+d*(at+input.Stride+channels<pixels.Length?pixels[at+input.Stride+channels]:0))>>14);}
  }}return channels==1?new GrayImage(input.Width,input.Height,output):new RgbImage(input.Width,input.Height,output);
 }
}


