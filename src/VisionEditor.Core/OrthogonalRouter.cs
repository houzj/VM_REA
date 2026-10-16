namespace VisionEditor.Core;

public readonly record struct RoutePoint(double X, double Y);
public readonly record struct RouteRect(double X, double Y, double Width, double Height)
{
    public double Right => X + Width;
    public double Bottom => Y + Height;
    public RouteRect Inflate(double margin) => new(X-margin,Y-margin,Width+2*margin,Height+2*margin);
}
public enum PortDirection { Left, Right, Up, Down }
public readonly record struct RoutePort(RoutePoint Anchor, PortDirection Direction, RouteRect Owner);
public enum RouteStatus { Success, Blocked, BudgetExceeded, InvalidInput }
public sealed record OrthogonalRoute(RouteStatus Status, IReadOnlyList<RoutePoint> Points, int ExpandedStates);

/// <summary>Independent rectilinear visibility-grid routing. Boundaries may be followed; rectangle interiors are forbidden.</summary>
public static class OrthogonalRouter
{
    public static OrthogonalRoute Route(RoutePort source, RoutePort target, IEnumerable<RouteRect> obstacles,
        double clearance = 5, int maxStates = 160000, double bendPenalty = 15)
    {
        var rects = obstacles.Append(source.Owner).Append(target.Owner).Distinct().ToArray();
        if (!double.IsFinite(clearance) || clearance < 0 || !double.IsFinite(bendPenalty) || bendPenalty < 0 || maxStates < 1 ||
            !Valid(source.Anchor) || !Valid(target.Anchor) || !Enum.IsDefined(source.Direction) || !Enum.IsDefined(target.Direction) || !OnSide(source) || !OnSide(target) ||
            rects.Any(r => !double.IsFinite(r.X) || !double.IsFinite(r.Y) || !double.IsFinite(r.Right) || !double.IsFinite(r.Bottom) || r.Width <= 0 || r.Height <= 0))
            return Fail(RouteStatus.InvalidInput);
        var walls = rects.Select(r => r.Inflate(clearance)).ToArray();
        if (walls.Any(r => !double.IsFinite(r.X) || !double.IsFinite(r.Y) || !double.IsFinite(r.Right) || !double.IsFinite(r.Bottom))) return Fail(RouteStatus.InvalidInput);
        var start = Exit(source, clearance); var end = Exit(target, clearance);
        if (!Valid(start) || !Valid(end) || !Clear(source.Anchor,start,rects.Where(r => r != source.Owner).Select(r => r.Inflate(clearance))) ||
            !Clear(target.Anchor,end,rects.Where(r => r != target.Owner).Select(r => r.Inflate(clearance))) ||
            walls.Any(r => Inside(start,r) || Inside(end,r))) return Fail(RouteStatus.Blocked);
        var xs = walls.SelectMany(r => new[]{r.X,r.Right}).Append(start.X).Append(end.X).Distinct().Order().ToArray();
        var ys = walls.SelectMany(r => new[]{r.Y,r.Bottom}).Append(start.Y).Append(end.Y).Distinct().Order().ToArray();
        if ((long)xs.Length*ys.Length*3 > maxStates) return Fail(RouteStatus.BudgetExceeded);
        int width=xs.Length, height=ys.Length;
        int startNode=Array.IndexOf(ys,start.Y)*width+Array.IndexOf(xs,start.X), endNode=Array.IndexOf(ys,end.Y)*width+Array.IndexOf(xs,end.X);
        RoutePoint Point(int node) => new(xs[node%width],ys[node/width]);
        int Axis(PortDirection d) => d is PortDirection.Left or PortDirection.Right ? 1 : 2;
        int initial=startNode*3+Axis(source.Direction);
        var costs=new Dictionary<int,double>{{initial,0}}; var previous=new Dictionary<int,int>();
        var queue=new PriorityQueue<int,double>(); queue.Enqueue(initial,0); int expanded=0, finish=-1; double best=double.PositiveInfinity;
        while(queue.TryDequeue(out int state,out double cost))
        {
            if(cost != costs[state]) continue;
            if(cost>best) break;
            if(++expanded>maxStates) return new(RouteStatus.BudgetExceeded,Array.Empty<RoutePoint>(),expanded);
            int node=state/3, axis=state%3;
            if(node==endNode) { double total=cost+(axis==Axis(target.Direction)?0:bendPenalty); if(total<best){best=total;finish=state;} continue; }
            int x=node%width,y=node/width;
            foreach(var (next,nextAxis) in new[]{(x>0?node-1:-1,1),(x+1<width?node+1:-1,1),(y>0?node-width:-1,2),(y+1<height?node+width:-1,2)})
            {
                if(next<0) continue;
                var a=Point(node);var b=Point(next);
                // Never backtrack into an anchor stub, even when the owner is omitted from the supplied obstacles.
                if(!Clear(a,b,walls)) continue;
                double nextCost=cost+Math.Abs(a.X-b.X)+Math.Abs(a.Y-b.Y)+(axis==nextAxis?0:bendPenalty);
                int nextState=next*3+nextAxis;
                if(!costs.TryGetValue(nextState,out double old)||nextCost<old){costs[nextState]=nextCost;previous[nextState]=state;queue.Enqueue(nextState,nextCost);}
            }
        }
        if(finish<0) return new(RouteStatus.Blocked,Array.Empty<RoutePoint>(),expanded);
        var points=new List<RoutePoint>(); for(int s=finish;;s=previous[s]) { points.Add(Point(s/3));if(s==initial)break; }
        points.Reverse();points.Insert(0,source.Anchor);points.Add(target.Anchor);
        var compact=new List<RoutePoint>();foreach(var p in points){if(compact.Count>0&&compact[^1]==p)continue;while(compact.Count>=2&&Collinear(compact[^2],compact[^1],p))compact.RemoveAt(compact.Count-1);compact.Add(p);}
        return new(RouteStatus.Success,compact,expanded);
    }
    private static OrthogonalRoute Fail(RouteStatus status)=>new(status,Array.Empty<RoutePoint>(),0);
    private static bool Valid(RoutePoint p)=>double.IsFinite(p.X)&&double.IsFinite(p.Y);
    private static bool Inside(RoutePoint p,RouteRect r)=>p.X>r.X&&p.X<r.Right&&p.Y>r.Y&&p.Y<r.Bottom;
    private static bool Collinear(RoutePoint a,RoutePoint b,RoutePoint c)=>a.X==b.X&&b.X==c.X||a.Y==b.Y&&b.Y==c.Y;
    private static bool OnSide(RoutePort p) => p.Direction switch { PortDirection.Left => p.Anchor.X == p.Owner.X && p.Anchor.Y >= p.Owner.Y && p.Anchor.Y <= p.Owner.Bottom, PortDirection.Right => p.Anchor.X == p.Owner.Right && p.Anchor.Y >= p.Owner.Y && p.Anchor.Y <= p.Owner.Bottom, PortDirection.Up => p.Anchor.Y == p.Owner.Y && p.Anchor.X >= p.Owner.X && p.Anchor.X <= p.Owner.Right, PortDirection.Down => p.Anchor.Y == p.Owner.Bottom && p.Anchor.X >= p.Owner.X && p.Anchor.X <= p.Owner.Right, _ => false };
    private static RoutePoint Exit(RoutePort p,double margin)=>p.Direction switch {
        PortDirection.Left=>new(p.Owner.X-margin,p.Anchor.Y),PortDirection.Right=>new(p.Owner.Right+margin,p.Anchor.Y),
        PortDirection.Up=>new(p.Anchor.X,p.Owner.Y-margin),_=>new(p.Anchor.X,p.Owner.Bottom+margin)};
    private static bool Clear(RoutePoint a,RoutePoint b,IEnumerable<RouteRect> walls)
    {
        if(a.X!=b.X&&a.Y!=b.Y)return false;
        foreach(var r in walls){if(a.X==b.X){if(a.X>r.X&&a.X<r.Right&&Math.Max(a.Y,b.Y)>r.Y&&Math.Min(a.Y,b.Y)<r.Bottom)return false;}
        else if(a.Y>r.Y&&a.Y<r.Bottom&&Math.Max(a.X,b.X)>r.X&&Math.Min(a.X,b.X)<r.Right)return false;}
        return true;
    }
}

