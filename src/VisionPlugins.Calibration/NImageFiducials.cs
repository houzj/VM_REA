using VisionCore;
namespace VisionPlugins.Calibration;

/// <summary>Independent ring localization inside a source-confirmed 20 by 20 HIK-I opening.</summary>
public static class NImageFiducials
{
 static readonly double[] Cosines=Enumerable.Range(0,24).Select(i=>Math.Cos(i*Math.PI*2/24)).ToArray(),Sines=Enumerable.Range(0,24).Select(i=>Math.Sin(i*Math.PI*2/24)).ToArray();
 public static IReadOnlyList<Point2>? Find(GrayImage image,IReadOnlyList<Point2> opening)
 {
  ArgumentNullException.ThrowIfNull(image);if(opening.Count!=4)throw new ArgumentException("Four opening corners required",nameof(opening));
  double width=Distance(opening[0],opening[1]),height=Distance(opening[0],opening[3]),radius=Math.Min(width,height)/16;
  if(radius<2)return null;
  // Reference positions follow original 80a7f0; unlike the decoder, this input is the outer opening.
  var references=new[]{(.2,.2),(.8,.2),(.6,.8),(.2,.8)};var result=new List<Point2>();
  foreach(var (u,v) in references){var seed=Interpolate(opening,u,v);double best=double.NegativeInfinity;Point2 selected=seed;int reach=(int)Math.Ceiling(radius*1.25);
   for(int y=(int)Math.Round(seed.Y)-reach;y<=(int)Math.Round(seed.Y)+reach;y++)for(int x=(int)Math.Round(seed.X)-reach;x<=(int)Math.Round(seed.X)+reach;x++)for(int scale=8;scale<=12;scale++){
    double score=Score(image,x,y,radius*scale/10);if(score>best){best=score;selected=new(x,y);}}
   if(best<12)return null;result.Add(selected);
  }
  return result.AsReadOnly();
 }
 static double Score(GrayImage image,double x,double y,double radius){double inner=0,ring=0,outer=0,square=0;const int samples=24;
  for(int i=0;i<samples;i++){double c=Cosines[i],s=Sines[i];double r=Sample(image,x+c*radius,y+s*radius),b=Sample(image,x+c*radius*1.65,y+s*radius*1.65),n=Sample(image,x+c*radius*.25,y+s*radius*.25);if(r<0||b<0||n<0)return double.NegativeInfinity;ring+=r;square+=r*r;outer+=b;inner+=n;}
  ring/=samples;inner/=samples;outer/=samples;double deviation=Math.Sqrt(Math.Max(0,square/samples-ring*ring));return Math.Min(inner-ring,outer-ring)-deviation*.7;
 }
 static double Sample(GrayImage image,double x,double y){int ix=(int)Math.Floor(x),iy=(int)Math.Floor(y);if(ix<0||iy<0||ix+1>=image.Width||iy+1>=image.Height)return -1;double a=x-ix,b=y-iy;return image[ix,iy]*(1-a)*(1-b)+image[ix+1,iy]*a*(1-b)+image[ix,iy+1]*(1-a)*b+image[ix+1,iy+1]*a*b;}
 static Point2 Interpolate(IReadOnlyList<Point2> q,double u,double v)=>new(q[0].X*(1-u)*(1-v)+q[1].X*u*(1-v)+q[2].X*u*v+q[3].X*(1-u)*v,q[0].Y*(1-u)*(1-v)+q[1].Y*u*(1-v)+q[2].Y*u*v+q[3].Y*(1-u)*v);
 static double Distance(Point2 a,Point2 b)=>Math.Sqrt((a.X-b.X)*(a.X-b.X)+(a.Y-b.Y)*(a.Y-b.Y));
}

