using VisionCore;
namespace VisionPlugins.Calibration;

public readonly record struct NImageLatticePoint(int Column,int Row,double X,double Y,double Contrast);
public sealed record NImageLattice(double PitchX,double PitchY,IReadOnlyList<NImageLatticePoint> Points,IReadOnlyList<IReadOnlyList<Point2>> CodeQuadrilaterals){public NImageSourceGridContext? SourceGrid{get;init;}}

/// <summary>Independent checker-lattice candidate phase. Decoded HIK identifiers are a separate required phase.</summary>
public static class NImageBoardDetector
{
 public static NImageLattice? FindLattice(GrayImage image,CancellationToken cancellationToken=default,int codeCellSize=2,int filterStatus=1){
  ArgumentNullException.ThrowIfNull(image);if(codeCellSize is not (1 or 2))throw new ArgumentOutOfRangeException(nameof(codeCellSize));if(filterStatus is not (1 or 2))throw new ArgumentOutOfRangeException(nameof(filterStatus));cancellationToken.ThrowIfCancellationRequested();if(image.Width<64||image.Height<64)return null;
  int width=image.Width,height=image.Height;double pitchX=EstimatePitch(image,true),pitchY=EstimatePitch(image,false);if(pitchX<8||pitchY<8)return null;
  var candidates=DetectCandidates(image,pitchX,pitchY,cancellationToken,filterStatus==1);
  if(candidates.Count<4)return null;
  // Coordinate clustering identifies lattice rows/columns without assigning the encoded board's physical origin.
  var columns=Clusters(candidates.Select(p=>p.X),pitchX*.20);var rows=Clusters(candidates.Select(p=>p.Y),pitchY*.20);columns=Regular(columns,pitchX);rows=Regular(rows,pitchY);if(columns.Count<2||rows.Count<2)return null;
  var points=new List<NImageLatticePoint>();foreach(var p in candidates){int col=Nearest(columns,p.X),row=Nearest(rows,p.Y);if(Math.Abs(columns[col]-p.X)<=pitchX*.2&&Math.Abs(rows[row]-p.Y)<=pitchY*.2)points.Add(new(col,row,p.X,p.Y,p.Response));}
  // A sparse I-type code occupies a 2x2 cell opening. Proposals are geometric only; a valid RS code must confirm them.
  var occupied=points.Select(p=>(p.Column,p.Row)).ToHashSet();var quads=new List<IReadOnlyList<Point2>>();for(int y=0;y+codeCellSize<rows.Count;y++)for(int x=0;x+codeCellSize<columns.Count;x++){if(codeCellSize==2&&points.Any(p=>p.Column==x+1&&p.Row==y+1&&p.Contrast>180))continue;int present=0;foreach(var p in new[]{(x,y),(x+codeCellSize,y),(x+codeCellSize,y+codeCellSize),(x,y+codeCellSize)})if(occupied.Contains(p))present++;if(present>=2)quads.Add(Array.AsReadOnly(new[]{new Point2(columns[x],rows[y]),new Point2(columns[x+codeCellSize],rows[y]),new Point2(columns[x+codeCellSize],rows[y+codeCellSize]),new Point2(columns[x],rows[y+codeCellSize])}));}
  return new(pitchX,pitchY,Array.AsReadOnly(points.OrderBy(p=>p.Row).ThenBy(p=>p.Column).ToArray()),Array.AsReadOnly(quads.ToArray()));
 }
 internal static List<(double X,double Y,double Response)> DetectCandidates(GrayImage image,double pitchX,double pitchY,CancellationToken cancellationToken,bool medianFilter=true){var working=medianFilter?NImageMedian.Apply(image,cancellationToken):image;int width=image.Width,height=image.Height;
  int radius=Math.Clamp((int)(Math.Min(pitchX,pitchY)*.14),2,12),stride=width+1;var integral=new long[checked(stride*(height+1))];
  for(int y=0;y<height;y++){long row=0;for(int x=0;x<width;x++){row+=working[x,y];integral[(y+1)*stride+x+1]=integral[y*stride+x+1]+row;}}
  double Average(int x0,int y0,int x1,int y1)=>(double)(integral[y1*stride+x1]-integral[y0*stride+x1]-integral[y1*stride+x0]+integral[y0*stride+x0])/((x1-x0)*(y1-y0));
  var response=new float[checked(width*height)];for(int y=radius+1;y<height-radius-1;y++){if((y&31)==0)cancellationToken.ThrowIfCancellationRequested();for(int x=radius+1;x<width-radius-1;x++){double a=Average(x-radius,y-radius,x,y),b=Average(x+1,y-radius,x+radius+1,y),c=Average(x-radius,y+1,x,y+radius+1),d=Average(x+1,y+1,x+radius+1,y+radius+1);double contrast=Math.Abs(a+d-b-c)*.5,imbalance=Math.Abs(a-d)+Math.Abs(b-c);if(contrast>=60&&imbalance<contrast*.6)response[y*width+x]=(float)(contrast-imbalance);}}
  int suppress=Math.Max(2,radius/2);var candidates=new List<(double X,double Y,double Response)>();for(int y=radius+suppress+1;y<height-radius-suppress-1;y++)for(int x=radius+suppress+1;x<width-radius-suppress-1;x++){float score=response[y*width+x];if(score<80)continue;bool peak=true;for(int dy=-suppress;dy<=suppress&&peak;dy++)for(int dx=-suppress;dx<=suppress;dx++){float other=response[(y+dy)*width+x+dx];if(other>score||other==score&&(dy<0||dy==0&&dx<0)){peak=false;break;}}if(!peak)continue;var refined=Refine(image,x,y,radius);candidates.Add((refined.X,refined.Y,score));}
  return candidates;
 }
 internal static double EstimatePitch(GrayImage image,bool horizontal){int length=horizontal?image.Width:image.Height,other=horizontal?image.Height:image.Width,max=Math.Min(length/3,500);var weights=new double[max+1];for(int line=3;line<other-3;line+=Math.Max(3,other/70)){int previous=0;bool dark=(horizontal?image[0,line]:image[line,0])<128;for(int i=1;i<length;i++){bool next=(horizontal?image[i,line]:image[line,i])<128;if(next==dark)continue;int run=i-previous;previous=i;dark=next;if(run>=8&&run<=max)weights[run]+=run*run;}}
  int best=0;double score=0;for(int i=8;i<=max;i++){double sum=weights[i]+(i>8?weights[i-1]:0)+(i<max?weights[i+1]:0);if(sum>score){score=sum;best=i;}}if(best==0)return 0;double w=0,p=0;for(int i=Math.Max(8,best-1);i<=Math.Min(max,best+1);i++){w+=weights[i];p+=i*weights[i];}return p/w;
 }
 static List<double> Clusters(IEnumerable<double> source,double tolerance){var groups=new List<List<double>>();foreach(double v in source.Order()){if(groups.Count==0||v-groups[^1].Average()>tolerance)groups.Add([v]);else groups[^1].Add(v);}int minimum=Math.Max(2,groups.Max(g=>g.Count)/4);return groups.Where(g=>g.Count>=minimum).Select(g=>g.Average()).ToList();}
 static List<double> Regular(List<double> values,double pitch){if(values.Count==0)return values;double phase=values.OrderByDescending(v=>values.Count(q=>Math.Abs((q-v)/pitch-Math.Round((q-v)/pitch))<.18)).First();return values.Where(q=>Math.Abs((q-phase)/pitch-Math.Round((q-phase)/pitch))<.18).ToList();}
 static int Nearest(IReadOnlyList<double> values,double value){int best=0;for(int i=1;i<values.Count;i++)if(Math.Abs(values[i]-value)<Math.Abs(values[best]-value))best=i;return best;}
 static Point2 Refine(GrayImage image,int x,int y,int radius){double xx=0,xy=0,yy=0,bx=0,by=0;for(int j=y-radius;j<=y+radius;j++)for(int i=x-radius;i<=x+radius;i++){double gx=image[i+1,j]-image[i-1,j],gy=image[i,j+1]-image[i,j-1],weight=Math.Exp(-((i-x)*(i-x)+(j-y)*(j-y))/(double)(radius*radius));xx+=weight*gx*gx;xy+=weight*gx*gy;yy+=weight*gy*gy;bx+=weight*(gx*gx*i+gx*gy*j);by+=weight*(gx*gy*i+gy*gy*j);}double determinant=xx*yy-xy*xy;if(determinant<=1e-12)return new(x,y);double rx=(yy*bx-xy*by)/determinant,ry=(xx*by-xy*bx)/determinant;return Math.Abs(rx-x)<=radius&&Math.Abs(ry-y)<=radius?new(rx,ry):new(x,y);}
}
