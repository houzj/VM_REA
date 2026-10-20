using VisionCore;
namespace VisionPlugins.Calibration;

/// <summary>Source-order 8b47c0 candidate phase; grid arrangement and board identity are separate.
/// Current parity evidence covers original 1000/500/250-pixel source/target HIK7/9 inputs.</summary>
public static class NImageSourceCandidateDetector
{
 public static IReadOnlyList<NImageSourceCandidate> FindCandidates(GrayImage source,int filterStatus=1,CancellationToken cancellationToken=default)
  =>FindCandidates(source,30f,135f,filterStatus,cancellationToken);
 public static IReadOnlyList<NImageSourceCandidate> FindCandidates(GrayImage source,float minimumAngle,float maximumAngle,int filterStatus=1,CancellationToken cancellationToken=default)
 {
  ArgumentNullException.ThrowIfNull(source);if(filterStatus is not (1 or 2))throw new ArgumentOutOfRangeException(nameof(filterStatus));
  cancellationToken.ThrowIfCancellationRequested();int width=source.Width,height=source.Height;if(width<32||height<32)return [];
  var classificationImage=filterStatus==1?NImageMedian.Apply(source,cancellationToken):source;
  var image=Gaussian(source,cancellationToken);var response=new float[width*height];float minimum=255;
  for(int y=1;y<height-1;y++){if((y&31)==0)cancellationToken.ThrowIfCancellationRequested();for(int x=1;x<width-1;x++){
   int i=y*width+x;float ul=image[i-width-1],uc=image[i-width],ur=image[i-width+1],left=image[i-1],center=image[i],right=image[i+1],bl=image[i+width-1],bc=image[i+width],br=image[i+width+1];
   float mixed=(((ur-ul)+bl)-br)*.25f;float determinant=((left-(center+center))+right)*((uc-(center+center))+bc)-mixed*mixed;minimum=MathF.Min(minimum,determinant);response[i]=MathF.Abs(determinant);
  }}
  float threshold=MathF.Abs(minimum*.005f);var points=new List<NImageSourceCandidate>();
  foreach(int radius in new[]{4,5,7})for(int y=2*radius;y<height-2*radius;y+=radius+1){cancellationToken.ThrowIfCancellationRequested();for(int x=2*radius;x<width-2*radius;x+=radius+1){
   int px=x,py=y;float maximum=response[y*width+x];for(int iy=y;iy<=y+radius;iy++)for(int ix=x;ix<=x+radius;ix++)if(maximum<response[iy*width+ix]){maximum=response[iy*width+ix];px=ix;py=iy;}
   bool peak=true;for(int iy=py-radius;iy<Math.Min(py+radius,height-radius)&&peak;iy++)for(int ix=px-radius;ix<Math.Min(px+radius,width-radius);ix++)if(maximum<response[iy*width+ix]){peak=false;break;}
   if(points.Count>=90000)break;if(peak&&threshold<maximum)points.Add(new(px,py,radius));
  }}
  float F(uint x)=>BitConverter.Int32BitsToSingle((int)x);float[] cos=[1,F(0x3f6eb680),F(0x3f3d2fb0),.44573835f,F(0x3dbcf733),F(0xbe8c1d91),F(0xbf1a4644),F(0xbf59a7d7),F(0xbf7ba420),F(0xbf7ba420),F(0xbf59a7d3),F(0xbf1a463f),F(0xbe8c1d8b),F(0x3dbcf75a),F(0x3ee437e1),F(0x3f3d2fb3),F(0x3f6eb680),1];float[] sin=[0,F(0x3eb8f4ab),F(0x3f2c7751),.8951633f,F(0x3f7ee86f),F(0x3f763a34),F(0x3f4c4adb),F(0x3f06c43e),F(0x3e3c28d6),F(0xbe3c28d2),F(0xbf06c444),F(0xbf4c4adf),F(0xbf763a35),F(0xbf7ee86f),F(0xbf652968),F(0xbf2c774e),F(0xbeb8f4ac),F(0x343bbd2e)];

  bool Classify(NImageSourceCandidate p){float radius=p.Radius*1.6f;if(p.X-radius<0||p.X+radius>=width-1||p.Y-radius<0||p.Y+radius>=height-1)return false;var values=new int[18];for(int i=0;i<18;i++){int x=(int)((double)(cos[i]*radius+p.X)+.5),y=(int)((double)(sin[i]*radius+p.Y)+.5);values[i]=classificationImage[x,y];}int min=values.Min(),cut=(int)((values.Max()-min)*.5f);var signs=values.Select(v=>v-min-cut>0).ToArray();int start=0;for(int i=1;i<18;i++)if(signs[i]!=signs[i-1])start=i;if(signs[0]!=signs[17])start=0;int run=signs[(start+1)%18]!=signs[(start+2)%18]?2:3,segments=0;for(int j=2;j<18;j++){if(signs[(start+j)%18]!=signs[(start+j+1)%18]){if(run>2)segments++;run=1;}run++;}return segments==4;}
  var classified=points.Where(Classify);var selected=new List<NImageSourceCandidate>();var occupied=new bool[width*height];
  foreach(var p in classified){int x=(int)p.X,y=(int)p.Y;bool found=false;for(int iy=y-1;iy<=y+1&&!found;iy++)for(int ix=x-1;ix<=x+1;ix++)if(occupied[iy*width+ix]){found=true;break;}if(found)continue;selected.Add(p);occupied[y*width+x]=true;}
  var refined=new List<NImageSourceCandidate>();foreach(var p in selected){cancellationToken.ThrowIfCancellationRequested();if(NImageQuadraticCornerRefiner.TryRefine(source,p.X,p.Y,p.Radius,90000,selected.Count,out var result))refined.Add(new((float)result.X,(float)result.Y,p.Radius));}
  var described=new List<NImageSourceCandidate>();foreach(var p in refined){cancellationToken.ThrowIfCancellationRequested();if(NImageSourceCandidateDescriptor.Describe(source,p,refined.Count,3,minimumAngle,maximumAngle) is { } result)described.Add(result);}
  return described.AsReadOnly();
 }
 static float[] Gaussian(GrayImage source,CancellationToken cancellationToken)
 {
  int width=source.Width,height=source.Height;var kernel=new float[13];float sum=0;for(int i=0;i<13;i++){float d=MathF.Abs(i-6f);kernel[i]=MathF.Exp((d*d)/((-2f*3f)*3f));sum=sum+kernel[i];}for(int i=0;i<13;i++)kernel[i]=kernel[i]/sum;
  var horizontal=new float[width*height];for(int y=0;y<height;y++){if((y&31)==0)cancellationToken.ThrowIfCancellationRequested();for(int x=0;x<width;x++){float value=0;for(int k=0;k<13;k++)value=value+source[Math.Clamp(x+k-6,0,width-1),y]*kernel[k];horizontal[y*width+x]=value;}}
  var output=new float[width*height];for(int y=0;y<height;y++){if((y&31)==0)cancellationToken.ThrowIfCancellationRequested();for(int x=0;x<width;x++){float value=0;for(int k=0;k<13;k++)value=value+kernel[k]*horizontal[Math.Clamp(y+k-6,0,height-1)*width+x];output[y*width+x]=value;}}return output;
 }
}
