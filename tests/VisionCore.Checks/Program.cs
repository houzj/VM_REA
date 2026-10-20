using VisionCore;
int checks = 0;
void Check(bool ok, string name) { if (!ok) throw new Exception("FAIL: " + name); checks++; Console.WriteLine("PASS: " + name); }
bool Near(double a,double b) => Math.Abs(a-b)<1e-8;
var raw = new byte[] {0,127,128,255};
var image = new GrayImage(4,1,raw); raw[0]=255;
Check(image[0,0]==0,"Image owns a copy of input");
Check(Algorithms.Threshold(image,128).Pixels.SequenceEqual(new byte[]{0,0,255,255}),"Threshold includes exact boundary");
Check(Algorithms.Threshold(image,0).Pixels.ToArray().All(x=>x==255),"Threshold zero includes black");
Check(Algorithms.Threshold(image,255).Pixels.SequenceEqual(new byte[]{0,0,0,255}),"Threshold 255 includes only maximum");
var mask = new GrayImage(5,3,new byte[]{255,0,0,0,255, 0,255,0,0,255, 0,0,0,0,0});
var components=Algorithms.ConnectedComponents8(mask);
Check(components.Count==2,"Two separate components");
Check(components[0].Area==2 && components[0].MaxX==1 && components[0].MaxY==1 && Near(components[0].CenterX,.5) && Near(components[0].CenterY,.5),"Diagonal adjacency and centroid");
Check(components[1].Area==2 && components[1].MinX==4 && components[1].MaxX==4 && Near(components[1].CenterY,.5),"Second component bounds");
Check(Algorithms.ConnectedComponents8(new GrayImage(2,2,new byte[4])).Count==0,"Empty foreground");
var points=Enumerable.Range(0,32).Select(i=> { double t=i*2*Math.PI/32; return new Point2(12+7*Math.Cos(t),-4+7*Math.Sin(t)); }).ToArray();
var fit=Algorithms.FitCircle(points);
Check(Near(fit.CenterX,12) && Near(fit.CenterY,-4) && Near(fit.Radius,7) && fit.RadialRmsError<1e-10,"Known circle center and radius");
var translated=points.Select(p=>new Point2(p.X+1000000,p.Y-1000000)).ToArray();
fit=Algorithms.FitCircle(translated);
Check(Near(fit.CenterX,1000012) && Near(fit.CenterY,-1000004) && Near(fit.Radius,7),"Translation stability");
bool rejected=false;
try { Algorithms.FitCircle(new[]{new Point2(0,0),new Point2(1,1),new Point2(2,2),new Point2(3,3)}); } catch(ArgumentException) { rejected=true; }
Check(rejected,"Collinear set rejected");
rejected=false;
try { Algorithms.FitCircle(new[]{new Point2(2,2),new Point2(2,2),new Point2(2,2)}); } catch(ArgumentException) { rejected=true; }
Check(rejected,"Coincident set rejected");
RadialChecks.Run(Check);
RectangleRoiMaskChecks.Run(Check);
Console.WriteLine($"All {checks} semantic checks passed. Rectangle ROI rasterization matches all 64 original mask fixtures; other VisionCore behaviors remain independent baselines.");

