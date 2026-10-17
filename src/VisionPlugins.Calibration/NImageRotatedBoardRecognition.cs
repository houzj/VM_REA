using VisionCore;
namespace VisionPlugins.Calibration;

/// <summary>Independent rotation normalization; perspective deformation is not covered by this branch.</summary>
public static class NImageRotatedBoardRecognition
{
 public static NImageDetectedBoard? Find(GrayImage image,int boardType,CancellationToken cancellationToken=default,int filterStatus=1){
  double angle=EstimateAngle(image,cancellationToken);foreach(double offset in new[]{0,.25,-.25,.5,-.5}){var board=FindAtAngle(image,boardType,angle+offset*Math.PI/180,cancellationToken,filterStatus);if(board!=null)return board;}return null;
 }
 public static NImageDetectedBoard? FindAtAngle(GrayImage image,int boardType,double angle,CancellationToken cancellationToken=default,int filterStatus=1){
  ArgumentNullException.ThrowIfNull(image);if(!double.IsFinite(angle))throw new ArgumentOutOfRangeException(nameof(angle));double c=Math.Cos(angle),s=Math.Sin(angle);
  if(Math.Abs(angle)<.001)return NImageBoardRecognition.FindOrthogonal(image,boardType,cancellationToken,filterStatus);
  int width=(int)Math.Ceiling(image.Width*Math.Abs(c)+image.Height*Math.Abs(s)),height=(int)Math.Ceiling(image.Width*Math.Abs(s)+image.Height*Math.Abs(c));
  double cx=(width-1)*.5,cy=(height-1)*.5,ox=(image.Width-1)*.5,oy=(image.Height-1)*.5;
  Point2 Original(double x,double y)=>new(c*(x-cx)-s*(y-cy)+ox,s*(x-cx)+c*(y-cy)+oy);
  var pixels=new byte[checked(width*height)];for(int y=0;y<height;y++){if((y&31)==0)cancellationToken.ThrowIfCancellationRequested();for(int x=0;x<width;x++){var p=Original(x,y);int ix=(int)Math.Floor(p.X),iy=(int)Math.Floor(p.Y);if(ix<0||iy<0||ix+1>=image.Width||iy+1>=image.Height){pixels[y*width+x]=255;continue;}double a=p.X-ix,b=p.Y-iy;pixels[y*width+x]=(byte)Math.Clamp((int)Math.Round(image[ix,iy]*(1-a)*(1-b)+image[ix+1,iy]*a*(1-b)+image[ix,iy+1]*(1-a)*b+image[ix+1,iy+1]*a*b),0,255);}}
  var board=NImageBoardRecognition.FindOrthogonal(new GrayImage(width,height,pixels),boardType,cancellationToken,filterStatus);if(board==null)return null;
  return new(board.GridPitch,Array.AsReadOnly(board.Points.Select(p=>{var q=Original(p.X,p.Y);return new NImageBoardPoint(p.IndexX,p.IndexY,q.X,q.Y);}).ToArray()),Array.AsReadOnly(board.Tags.Select(t=>new NImageDetectedTag(t.Identity,Array.AsReadOnly(t.Opening.Select(p=>Original(p.X,p.Y)).ToArray()),Array.AsReadOnly(t.Fiducials.Select(p=>Original(p.X,p.Y)).ToArray()))).ToArray()));
 }
 public static double EstimateAngle(GrayImage image,CancellationToken cancellationToken=default,int filterStatus=1){
  const int bins=360;var weights=new double[bins];double[] sums=new double[bins];
  var gaussian=Enumerable.Range(-3,7).Select(i=>Math.Exp(-i*i/4.5)).ToArray();
  for(int y=3;y<image.Height-3;y+=4){if((y&63)==3)cancellationToken.ThrowIfCancellationRequested();for(int x=3;x<image.Width-3;x+=4){double gx=0,gy=0;for(int j=-3;j<=3;j++)for(int k=-3;k<=3;k++){double value=image[x+k,y+j]*gaussian[k+3]*gaussian[j+3];gx+=k*value;gy+=j*value;}double weight=gx*gx+gy*gy;if(weight<250000)continue;double a=Math.Atan2(gy,gx);while(a< -Math.PI/4)a+=Math.PI/2;while(a>=Math.PI/4)a-=Math.PI/2;int bin=Math.Clamp((int)((a+Math.PI/4)*bins/(Math.PI/2)),0,bins-1);weights[bin]+=weight;sums[bin]+=weight*a;}}  int peak=Enumerable.Range(0,bins).MaxBy(i=>weights[i]+weights[(i+1)%bins]+weights[(i+bins-1)%bins]);double total=0,angle=0;for(int i=Math.Max(0,peak-16);i<=Math.Min(bins-1,peak+16);i++){total+=weights[i];angle+=sums[i];}return total>0?angle/total:0;
 }
}



