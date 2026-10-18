using System.IO;
using System.Windows;
using VisionEditor.Core;
using VisionEditor.Core.VmRouting;
using VMControls.WPF.Base;
using VMControls.WPF.Base.Astar;
int checks=0;
void Check(bool b,string m){checks++;if(!b)throw new Exception(m);}
Point Offset(RoutePort p)=>p.Direction switch{PortDirection.Left=>new(p.Anchor.X-5,p.Anchor.Y),PortDirection.Right=>new(p.Anchor.X+5,p.Anchor.Y),PortDirection.Up=>new(p.Anchor.X,p.Anchor.Y-5),_=>new(p.Anchor.X,p.Anchor.Y+5)};
Rect R(RouteRect r)=>new(r.X-5,r.Y-5,r.Width+10,r.Height+10);
ConnectorOrientation O(PortDirection d)=>d switch{PortDirection.Left=>ConnectorOrientation.Left,PortDirection.Right=>ConnectorOrientation.Right,PortDirection.Up=>ConnectorOrientation.Top,_=>ConnectorOrientation.Bottom};
List<Point> Oracle(RoutePort s,RoutePort t,RouteRect[] obstacles,int width,int height)
{
    var pair=new List<Rect>{R(s.Owner),R(t.Owner)};var all=obstacles.Append(s.Owner).Append(t.Owner).Distinct().Select(R).ToList();
    var a=Offset(s);var b=Offset(t);var map=new APoint[width/5+32,height/5+32];for(int x=0;x<map.GetLength(0);x++)for(int y=0;y<map.GetLength(1);y++)map[x,y]=new(x,y);
    void Mark(List<Rect> rects){APoint.CurTimeEnableFlag=Guid.NewGuid().ToString();foreach(var r in rects)for(int k=1;k<r.Width/5;k++)for(int l=1;l<r.Height/5;l++){int x=k+(int)r.X/5+1,y=l+(int)r.Y/5+1;if(x<map.GetLength(0)&&y<map.GetLength(1))map[x,y].IsWall=true;}}
    Mark(all);var path=AStar.FindPath(new((int)a.X/5+1,(int)a.Y/5+1),new((int)b.X/5+1,(int)b.Y/5+1),map);
    if(path.Count<3){Mark(pair);all=pair;path=AStar.FindPath(new((int)a.X/5+1,(int)a.Y/5+1),new((int)b.X/5+1,(int)b.Y/5+1),map);}
    var p=new List<Point>();if(path.Count<3){if(a.X!=b.X&&a.Y!=b.Y)p.Add(new(a.X,b.Y));p.Insert(0,a);p.Add(b);}else p=path.Select(v=>new Point((v.X-1)*5,(v.Y-1)*5)).ToList();
    p=PathFinder.OptimizeLinePoints(p,pair,all,O(s.Direction),O(t.Direction));p.Insert(0,new(s.Anchor.X,s.Anchor.Y));p.Add(new(t.Anchor.X,t.Anchor.Y));var result=PathFinder.GetTurnPoints(p);PathFinder.RechargeLinePoints(result,all);return result;
}
var cases=new[]{
(new RoutePort(new(100,80),PortDirection.Down,new(50,40,100,40)),new RoutePort(new(100,240),PortDirection.Up,new(50,240,100,40)),Array.Empty<RouteRect>()),
(new RoutePort(new(100,80),PortDirection.Down,new(50,40,100,40)),new RoutePort(new(300,240),PortDirection.Up,new(250,240,100,40)),new[]{new RouteRect(80,130,160,50)}),
(new RoutePort(new(150,60),PortDirection.Right,new(50,40,100,40)),new RoutePort(new(300,260),PortDirection.Left,new(300,240,100,40)),new[]{new RouteRect(180,70,80,130)})};
foreach(var(s,t,w) in cases){var result=VmOrthogonalRouter.Route(s,t,w,600,400);var original=Oracle(s,t,w,600,400);Check(result.Points.SequenceEqual(original.Select(p=>new RoutePoint(p.X,p.Y))),"matches untouched original WPF geometry pipeline");Check(result.Points[0]==s.Anchor&&result.Points[^1]==t.Anchor,"anchors retained");}
Check(VmOrthogonalRouter.Route(cases[0].Item1,cases[0].Item2,Array.Empty<RouteRect>(),600,400,2).Status==VmRouteStatus.BudgetExceeded,"cell cap");
Check(VmOrthogonalRouter.Route(cases[0].Item1,cases[0].Item2,new[]{new RouteRect(-10,50,20,20)},600,400).Status==VmRouteStatus.InvalidInput,"negative original map rejected explicitly");
var random=new Random(440);
for(int n=0;n<40;n++){
 double sx=45+random.Next(30),tx=260+random.Next(50);
 var source=new RoutePort(new(sx+50,80),PortDirection.Down,new(sx,40,100,40));
 var target=new RoutePort(new(tx+50,300),PortDirection.Up,new(tx,300,100,40));
 var walls=new[]{new RouteRect(100+random.Next(80),120+random.Next(40),70+random.Next(50),50+random.Next(40))};
 var actual=VmOrthogonalRouter.Route(source,target,walls,600,450);var expected=Oracle(source,target,walls,600,450);
 Check(actual.Points.SequenceEqual(expected.Select(p=>new RoutePoint(p.X,p.Y))),"seeded original differential");
}
RoutePort Port(RouteRect r,PortDirection d)=>new(d switch {PortDirection.Left=>new(r.X,r.Y+r.Height/2),PortDirection.Right=>new(r.Right,r.Y+r.Height/2),PortDirection.Up=>new(r.X+r.Width/2,r.Y),_=>new(r.X+r.Width/2,r.Bottom)},d,r);
foreach(var (source,target,walls) in new[]{
    (Port(new(400,320,110,30),PortDirection.Down),Port(new(140,120,110,30),PortDirection.Up),new[]{new RouteRect(300,200,110,30)}),
    (Port(new(180,150,110,30),PortDirection.Down),Port(new(220,160,110,30),PortDirection.Up),new[]{new RouteRect(210,145,110,30)}),
    (Port(new(180,150,110,30),PortDirection.Right),Port(new(180,150,110,30),PortDirection.Left),Array.Empty<RouteRect>())}){
 var actual=VmOrthogonalRouter.Route(source,target,walls,600,450);
 var expected=Oracle(source,target,walls,600,450);
 Check(actual.Points.SequenceEqual(expected.Select(p=>new RoutePoint(p.X,p.Y))),"reverse or overlapping endpoints match original AStar and PathFinder");
}
foreach(var sd in Enum.GetValues<PortDirection>())foreach(var td in Enum.GetValues<PortDirection>()){
 var source=Port(new(50,50,100,40),sd);var target=Port(new(350,300,100,40),td);var walls=new[]{new RouteRect(180,150,100,50)};
 var actual=VmOrthogonalRouter.Route(source,target,walls,600,450);var expected=Oracle(source,target,walls,600,450);
 Check(actual.Points.SequenceEqual(expected.Select(p=>new RoutePoint(p.X,p.Y))),"all 16 endpoint direction combinations");
}
using(var fs=File.OpenRead(@"D:\Program Files\VisionMaster4.4.0\Applications\myLibs\VMControls.WPF.dll"))
using(var pe=new System.Reflection.PortableExecutable.PEReader(fs)){
 var md=System.Reflection.Metadata.PEReaderExtensions.GetMetadataReader(pe);
 foreach(var h in md.TypeDefinitions){var d=md.GetTypeDefinition(h);string name=md.GetString(d.Name);if(name is "AStar" or "APoint" or "POINT" or "PathFinder" or "DiagramPage")Console.Error.WriteLine($"{md.GetString(d.Namespace)}.{name} typeToken=0x{System.Reflection.Metadata.Ecma335.MetadataTokens.GetToken(h):X8}");}
}
Console.WriteLine($"{checks} VM port checks passed");



