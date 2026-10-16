using VisionEditor.Core;
int checks=0;
void Check(bool b,string message){checks++;if(!b)throw new Exception(message);}
var s=new RoutePort(new(10,20),PortDirection.Down,new(0,0,20,20));
var t=new RoutePort(new(10,160),PortDirection.Up,new(0,160,20,20));
var wall=new RouteRect(-20,60,60,40);
var route=OrthogonalRouter.Route(s,t,new[]{wall});
Check(route.Status==RouteStatus.Success,"detour exists");Check(route.Points.Count>=6,"detour bends");
Check(route.Points[1].X==s.Anchor.X&&route.Points[1].Y>s.Anchor.Y,"source exits down");
Check(route.Points[^2].X==t.Anchor.X&&route.Points[^2].Y<t.Anchor.Y,"target enters down");
for(int i=1;i<route.Points.Count;i++){var a=route.Points[i-1];var b=route.Points[i];Check(a.X==b.X||a.Y==b.Y,"orthogonal");var r=wall.Inflate(5);Check(!(a.X==b.X?a.X>r.X&&a.X<r.Right&&Math.Max(a.Y,b.Y)>r.Y&&Math.Min(a.Y,b.Y)<r.Bottom:a.Y>r.Y&&a.Y<r.Bottom&&Math.Max(a.X,b.X)>r.X&&Math.Min(a.X,b.X)<r.Right),"no obstacle crossing");}
var negative=OrthogonalRouter.Route(new(new(-100,-100),PortDirection.Left,new(-100,-110,20,20)),new(new(-200,-100),PortDirection.Right,new(-220,-110,20,20)),Array.Empty<RouteRect>());
Check(negative.Status==RouteStatus.Success,"negative coordinates");
Check(OrthogonalRouter.Route(s,t,new[]{new RouteRect(-10,20,40,30)}).Status==RouteStatus.Blocked,"blocked stub");
Check(OrthogonalRouter.Route(s,t,new[]{wall},maxStates:2).Status==RouteStatus.BudgetExceeded,"bounded budget");
Check(OrthogonalRouter.Route(s,t,Array.Empty<RouteRect>(),clearance:double.NaN).Status==RouteStatus.InvalidInput,"invalid margin");
Check(OrthogonalRouter.Route(s,t,Array.Empty<RouteRect>()).Points.Count==2,"direct path simplified");
Check(OrthogonalRouter.Route(s with { Direction=PortDirection.Left },t,Array.Empty<RouteRect>()).Status==RouteStatus.InvalidInput,"anchor direction matches owner side");
Console.WriteLine($"{checks} routing checks passed");

