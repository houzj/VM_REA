using System.Drawing.Drawing2D;
using VisionFramework;
using VisionEditor.Core;
using VisionEditor.Core.VmRouting;
namespace VisionEditor;

// Gesture-only view; the editor owns every document mutation.
public sealed class GraphCanvas : Control
{
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)] public Func<string,Image?>? ModuleIcon{get;set;}
    readonly ModuleRegistry registry;
    ProcedureDocument? procedure;
    readonly Dictionary<string, NodeRunStatus> statuses = new(StringComparer.Ordinal);
    float zoom = 1;
    float DpiScale=>DeviceDpi/96f; float ViewScale=>zoom*DpiScale;
    string breadcrumb=""; [System.ComponentModel.DefaultValue("")] public string Breadcrumb{get=>breadcrumb;set{breadcrumb=value;Invalidate();}}
    PointF pan = new(40, 45);
    string? selected, dragged, hoveredNode;
    ConnectionDocument? hoveredLine, selectedLine;
    public ConnectionDocument? SelectedConnection=>selectedLine;
    PointF dragStart, dragOriginal, dragPreview, lastMouse;
    PortHit? connecting;
    bool panning, readOnly,executionMode=true;
    const float NodeWidth = 110, HeaderHeight = 30, PortSpacing = 28;
    readonly ToolTip portTip=new();
    readonly Dictionary<ConnectionDocument,IReadOnlyList<RoutePoint>> routeCache=new();
    readonly HashSet<ConnectionDocument> obstructedRoutes=new();
    int unroutable;bool minimapExpanded=true,minimapDragging;RectangleF minimapBox,minimapWorld;float minimapScale;
    public readonly record struct Viewport(float Zoom,float PanX,float PanY);
    public Viewport GetViewport()=>new(zoom,pan.X,pan.Y);
    public void SetViewport(Viewport view){if(!float.IsFinite(view.Zoom)||!float.IsFinite(view.PanX)||!float.IsFinite(view.PanY))return;zoom=Math.Clamp(view.Zoom,.35f,2.5f);pan=new(view.PanX,view.PanY);Invalidate();}
    sealed record PortHit(NodeDocument Node, PortDescriptor Port, bool Output, PointF Position,int Index=-1);
    public GraphCanvas(ModuleRegistry registry)
    {
        this.registry = registry;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
        BackColor = Color.FromArgb(73,77,80); ForeColor = Color.FromArgb(65,65,65);
        Font = new Font("Microsoft YaHei UI",12,FontStyle.Regular,GraphicsUnit.Pixel); AllowDrop = true; TabStop = true;
    }
    [System.ComponentModel.DefaultValue(true)]
    public bool ExecutionMode {get=>executionMode;set{executionMode=value;routeCache.Clear();obstructedRoutes.Clear();CancelGesture();Invalidate();}}
    public string? SelectedNodeId => selected;
    public event Action<string?>? SelectionChanged;
    public event Action<string>? NodeActivated;
    public event Action<string,double,double>? NodeMoved;
    public event Action<ConnectionDocument>? ConnectionRequested;
    public event Action<ConnectionDocument>? DisconnectRequested;
    public event Action<string,double,double>? ModuleDropped;
    public event Action? DeleteRequested;
    [System.ComponentModel.DefaultValue(false)]
    public bool ReadOnlyMode { get => readOnly; set {readOnly=value; if(value)CancelGesture(); Invalidate();} }
    public void SetProcedure(ProcedureDocument procedure)
    {
        this.procedure=procedure; routeCache.Clear();obstructedRoutes.Clear();CancelGesture();
        if(!procedure.Nodes.Any(n=>n.Id==selected))selected=null;
        if(selectedLine is not null&&!VisibleConnections().Contains(selectedLine))selectedLine=null;hoveredLine=null;hoveredNode=null;
        foreach(var id in statuses.Keys.Where(id=>!procedure.Nodes.Any(n=>n.Id==id)).ToArray())statuses.Remove(id);
        Invalidate();
    }
    public void SetNodeStatus(string id,NodeRunStatus status){statuses[id]=status;Invalidate();}
    public void ClearStatuses(){statuses.Clear();Invalidate();}
    public void SelectNode(string? id){if(id is null || procedure?.Nodes.Any(n=>n.Id==id)==true)Select(id);}
    public RectangleF? GetNodeScreenBounds(string nodeId)
    {
        var node=procedure?.Nodes.FirstOrDefault(n=>n.Id==nodeId); if(node is null||!Drawable(node))return null;
        var r=BoundsOf(node);return new RectangleF(r.X*ViewScale+pan.X,r.Y*ViewScale+pan.Y,r.Width*ViewScale,r.Height*ViewScale);
    }
    public PointF? GetPortScreenPoint(string nodeId,string port,bool output)
    {
        var node=procedure?.Nodes.FirstOrDefault(n=>n.Id==nodeId);if(node is null||!Drawable(node))return null;
        var hit=Ports(node).FirstOrDefault(p=>p.Output==output&&p.Port.Name==port&&(!executionMode||p.Index==(output?3:1)));
        return hit is null?null:new PointF(hit.Position.X*ViewScale+pan.X,hit.Position.Y*ViewScale+pan.Y);
    }
    public void FitToContent()
    {
        routeCache.Clear();obstructedRoutes.Clear();
        var visible=procedure?.Nodes.Where(Drawable).ToArray()??[]; if(visible.Length==0){zoom=1;pan=new(40,45);Invalidate();return;}
        float left=visible.Min(n=>(float)n.X),top=visible.Min(n=>(float)n.Y);
        float right=visible.Max(n=>(float)n.X+NodeWidth),bottom=visible.Max(n=>(float)n.Y+HeightOf(n));
        zoom=Math.Clamp(Math.Min(Math.Max(100,ClientSize.Width/DpiScale-100)/(right-left),Math.Max(100,ClientSize.Height/DpiScale-100)/(bottom-top)),.35f,1.0f);
        pan=new((ClientSize.Width-(right-left)*ViewScale)/2-left*ViewScale,(ClientSize.Height-(bottom-top)*ViewScale)/2-top*ViewScale);Invalidate();
    }
    static bool Drawable(NodeDocument n)=>double.IsFinite(n.X)&&double.IsFinite(n.Y)&&Math.Abs(n.X)<=10000000&&Math.Abs(n.Y)<=10000000;
    ModuleDescriptor? Descriptor(NodeDocument n){try{return registry.GetDescriptor(n);}catch(KeyNotFoundException){return null;}}
    float HeightOf(NodeDocument n)=>HeaderHeight;
    PointF Position(NodeDocument n)=>new((float)n.X,(float)n.Y);
    RectangleF BoundsOf(NodeDocument n){var p=Position(n);return new(p.X,p.Y,NodeWidth,HeightOf(n));}
    PointF World(Point p)=>new((p.X-pan.X)/ViewScale,(p.Y-pan.Y)/ViewScale);
    PointF PortPosition(NodeDocument n,bool output,int index){var p=Position(n);var d=Descriptor(n);int count=output?d?.Outputs.Count??1:d?.Inputs.Count??1;return new(p.X+NodeWidth*(index+1)/(count+1),p.Y+(output?HeightOf(n):0));}
    static PortDirection Direction(int index)=>index switch{0=>PortDirection.Left,1=>PortDirection.Up,2=>PortDirection.Right,_=>PortDirection.Down};
    PointF SidePosition(NodeDocument n,int index){var r=BoundsOf(n);return index switch{0=>new(r.Left,r.Top+r.Height/2),1=>new(r.Left+r.Width/2,r.Top),2=>new(r.Right,r.Top+r.Height/2),_=>new(r.Left+r.Width/2,r.Bottom)};}
    IEnumerable<PortHit> Ports(NodeDocument n)
    {
        var d=Descriptor(n);if(!Drawable(n))yield break;
        if(executionMode){foreach(int index in new[]{3,1,0,2}){var position=SidePosition(n,index);yield return new(n,new PortDescriptor("__flow_out",DataKind.Boolean),true,position,index);yield return new(n,new PortDescriptor("__flow_in",DataKind.Boolean),false,position,index);}yield break;}

        if(d is null)yield break;
        for(int i=0;i<d.Inputs.Count;i++)yield return new(n,d.Inputs[i],false,PortPosition(n,false,i));
        for(int i=0;i<d.Outputs.Count;i++)yield return new(n,d.Outputs[i],true,PortPosition(n,true,i));
    }
    PortHit? HitPort(PointF p)=>procedure?.Nodes.Reverse().SelectMany(Ports).FirstOrDefault(h=>Distance(h.Position,p)<=7/zoom);
    NodeDocument? HitNode(PointF p)=>procedure?.Nodes.Reverse().FirstOrDefault(n=>Drawable(n)&&BoundsOf(n).Contains(p));
    static float Distance(PointF a,PointF b)=>MathF.Sqrt((a.X-b.X)*(a.X-b.X)+(a.Y-b.Y)*(a.Y-b.Y));
    IEnumerable<ConnectionDocument> VisibleConnections()=>procedure is null?[]:executionMode?ProcedureGraph.ExecutionEdges(procedure).Select(e=>new ConnectionDocument(e.FromNode,"__flow_out",e.ToNode,"__flow_in"){SourceIndex=e.SourceIndex,TargetIndex=e.TargetIndex}):procedure.Connections;
    bool Ends(ConnectionDocument c,out PointF a,out PointF b)
    {
        a=b=default;if(procedure is null)return false;
        var from=procedure.Nodes.FirstOrDefault(n=>n.Id==c.FromNode);var to=procedure.Nodes.FirstOrDefault(n=>n.Id==c.ToNode);
        if(from is null||to is null)return false;
        if(executionMode){a=SidePosition(from,c.SourceIndex);b=SidePosition(to,c.TargetIndex);return true;}
        var fp=Ports(from).FirstOrDefault(p=>p.Output&&p.Port.Name==c.FromPort);var tp=Ports(to).FirstOrDefault(p=>!p.Output&&p.Port.Name==c.ToPort);
        if(fp is null||tp is null)return false;a=fp.Position;b=tp.Position;return true;
    }
    static GraphicsPath Curve(PointF a,PointF b){var p=new GraphicsPath();float mid=b.Y>a.Y+30?(a.Y+b.Y)/2:a.Y+32;p.AddLines(new PointF[]{a,new(a.X,mid),new(b.X,mid),b});return p;}
    GraphicsPath ConnectionPath(ConnectionDocument c,PointF a,PointF b)
    {
        var path=new GraphicsPath();if(procedure is null)return path;
        if(!routeCache.TryGetValue(c,out var points))
        {
            var from=procedure.Nodes.First(n=>n.Id==c.FromNode);var to=procedure.Nodes.First(n=>n.Id==c.ToNode);
            var bounds=procedure.Nodes.Where(Drawable).Select(BoundsOf).ToArray();
            // The original page uses nonnegative coordinates. Translate the entire geometry by a grid multiple for our pan-able world.
            double dx=Math.Max(0,-Math.Floor(bounds.Min(r=>r.Left)/5)*5),dy=Math.Max(0,-Math.Floor(bounds.Min(r=>r.Top)/5)*5);
            RouteRect Rect(NodeDocument n){var r=BoundsOf(n);return new(r.X+dx,r.Y+dy,r.Width,r.Height);}
            var sourceRect=Rect(from);var targetRect=Rect(to);var walls=procedure.Nodes.Where(Drawable).Select(Rect).ToArray();
            double pageWidth=Math.Max(ClientSize.Width/ViewScale+dx,walls.Max(r=>r.Right)),pageHeight=Math.Max(ClientSize.Height/ViewScale+dy,walls.Max(r=>r.Bottom));
            var route=VmOrthogonalRouter.Route(new(new(a.X+dx,a.Y+dy),(executionMode?Direction(c.SourceIndex):PortDirection.Down),sourceRect),new(new(b.X+dx,b.Y+dy),(executionMode?Direction(c.TargetIndex):PortDirection.Up),targetRect),walls,pageWidth,pageHeight);
            points=route.Points.Select(p=>new RoutePoint(p.X-dx,p.Y-dy)).ToArray();
            if(route.Status==VmRouteStatus.OriginalFallback)obstructedRoutes.Add(c);
            routeCache[c]=points;
        }
        if(points.Count>=2)path.AddLines(points.Select(p=>new PointF((float)p.X,(float)p.Y)).ToArray());return path;
    }
    public IReadOnlyList<PointF> GetConnectionRoute(ConnectionDocument c){if(!Ends(c,out var a,out var b))return Array.Empty<PointF>();using var path=ConnectionPath(c,a,b);return path.PointCount==0?Array.Empty<PointF>():Array.AsReadOnly(path.PathPoints);}
    ConnectionDocument? HitLine(PointF p)
    {
        if(procedure is null)return null;using var pen=new Pen(Color.White,10/zoom);
        foreach(var c in VisibleConnections().Reverse()){if(!Ends(c,out var a,out var b))continue;using var path=ConnectionPath(c,a,b);if(path.PointCount>0&&path.IsOutlineVisible(p,pen))return c;}return null;
    }
    static Color KindColor(DataKind kind)=>kind switch
    {
        DataKind.GrayImage=>Color.FromArgb(68,189,228),DataKind.Number=>Color.FromArgb(244,188,75),DataKind.Integer=>Color.FromArgb(245,145,70),
        DataKind.Boolean=>Color.FromArgb(189,132,240),DataKind.Text=>Color.FromArgb(98,208,146),DataKind.Components=>Color.FromArgb(235,126,171),DataKind.Circle=>Color.FromArgb(99,155,240),_=>Color.Gray
    };
    static string StatusName(NodeRunStatus s)=>s switch
    {
        NodeRunStatus.Pending=>"待运行",NodeRunStatus.Running=>"运行中",NodeRunStatus.Success=>"完成",NodeRunStatus.NoMatch=>"未找到目标",NodeRunStatus.Failed=>"失败",NodeRunStatus.Skipped=>"已跳过",NodeRunStatus.Cancelled=>"已取消",NodeRunStatus.TimedOut=>"超时",_=>s.ToString()
    };
    static Color StatusColor(NodeRunStatus s)=>s switch
    {
        NodeRunStatus.Running=>Color.FromArgb(65,170,255),NodeRunStatus.Success=>Color.FromArgb(78,201,148),NodeRunStatus.Failed or NodeRunStatus.TimedOut=>Color.FromArgb(249,100,105),NodeRunStatus.NoMatch=>Color.FromArgb(241,184,76),_=>Color.FromArgb(145,155,172)
    };
    static string PortName(string n)=>n switch{"Image"=>"图像","Circle"=>"圆","Components"=>"区域","Count"=>"数量","Radius"=>"半径","CenterX"=>"圆心 X","CenterY"=>"圆心 Y","Passed"=>"判定","Value"=>"数值","Number"=>"数值","Found"=>"找到目标","__flow_out"=>"执行后继","__flow_in"=>"执行前驱",_=>n};
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;using(var texture=new TextureBrush(VmUiAssets.Get("grid_canvas.png")!,WrapMode.Tile)){using var matrix=new Matrix(ViewScale,0,0,ViewScale,pan.X,pan.Y);texture.Transform=matrix;g.FillRectangle(texture,ClientRectangle);}
        var saved=g.Save();g.TranslateTransform(pan.X,pan.Y);g.ScaleTransform(ViewScale,ViewScale);
        unroutable=0;
        if(procedure is not null)
        {
            foreach(var c in VisibleConnections()){if(!Ends(c,out var a,out var b))continue;using var pen=new Pen(c==selectedLine?Color.FromArgb(255,220,130):c==hoveredLine?Color.FromArgb(255,190,70):executionMode?Color.FromArgb(255,157,12):Color.FromArgb(68,189,228),c==selectedLine?3.5f:c==hoveredLine?3:2);pen.StartCap=LineCap.Flat;pen.EndCap=LineCap.Flat;pen.LineJoin=LineJoin.Round;using var path=ConnectionPath(c,a,b);if(path.PointCount>0){g.DrawPath(pen,path);using var arrowBrush=new SolidBrush(pen.Color);g.FillPolygon(arrowBrush,ArrowTriangle(b,executionMode?Direction(c.TargetIndex) switch{PortDirection.Up=>PortDirection.Down,PortDirection.Down=>PortDirection.Up,PortDirection.Left=>PortDirection.Right,_=>PortDirection.Left}:PortDirection.Down));}else unroutable++;}
            foreach(var n in procedure.Nodes.Where(Drawable))DrawNode(g,n);
            if(dragged is not null&&procedure.Nodes.FirstOrDefault(n=>n.Id==dragged)is {} moving&&Distance(dragOriginal,dragPreview)>1){var ghost=g.Save();g.TranslateTransform(dragPreview.X-(float)moving.X,dragPreview.Y-(float)moving.Y);DrawNode(g,moving,true);g.Restore(ghost);using var guide=new Pen(Color.FromArgb(143,207,139),1){DashStyle=DashStyle.Dash};g.DrawLine(guide,dragOriginal.X,dragPreview.Y,dragPreview.X+NodeWidth,dragPreview.Y);g.DrawLine(guide,dragOriginal.X,dragPreview.Y+HeaderHeight,dragPreview.X+NodeWidth,dragPreview.Y+HeaderHeight);}
            if(connecting is not null){using var pen=new Pen(KindColor(connecting.Port.Kind),2){DashStyle=DashStyle.Dash};using var path=new GraphicsPath();var r=BoundsOf(connecting.Node);var preview=VmOrthogonalRouter.Preview(new RoutePort(new(connecting.Position.X,connecting.Position.Y),executionMode?Direction(connecting.Index):PortDirection.Down,new(r.X,r.Y,r.Width,r.Height)),new(lastMouse.X,lastMouse.Y));if(preview.Count>=2)path.AddLines(preview.Select(p=>new PointF((float)p.X,(float)p.Y)).ToArray());if(path.PointCount>0)g.DrawPath(pen,path);else unroutable++;}
        }
        g.Restore(saved);DrawMinimap(g);using var footer=new SolidBrush(Color.FromArgb(195,195,195));
        using var crumbFont=new Font("Microsoft YaHei UI",14*DpiScale,FontStyle.Regular,GraphicsUnit.Pixel);g.DrawString(Breadcrumb,crumbFont,footer,5*DpiScale,5*DpiScale);
        if(obstructedRoutes.Count>0)g.DrawString($"{obstructedRoutes.Count} 条连接采用 VM 原始降级路径",Font,Brushes.Orange,12,32*DpiScale);
        if(unroutable>0)g.DrawString($"{unroutable} 条连线无可用避障路径",Font,Brushes.Orange,12,32*DpiScale);
    }
    internal static PointF[] ArrowTriangle(PointF tip,PortDirection direction){PointF d=direction switch{PortDirection.Up=>new(0,-1),PortDirection.Down=>new(0,1),PortDirection.Left=>new(-1,0),_=>new(1,0)};const float length=10,halfWidth=5;var center=new PointF(tip.X-d.X*length,tip.Y-d.Y*length);return new[]{tip,new PointF(center.X-d.Y*halfWidth,center.Y+d.X*halfWidth),new PointF(center.X+d.Y*halfWidth,center.Y-d.X*halfWidth)};}
    void DrawMinimap(Graphics g){if(!minimapExpanded||Width<250||Height<220||procedure is null){minimapBox=RectangleF.Empty;return;}var nodes=procedure.Nodes.Where(Drawable).ToArray();if(nodes.Length==0){minimapBox=RectangleF.Empty;return;}minimapBox=new(Width-198*DpiScale,Height-160*DpiScale,187*DpiScale,150*DpiScale);float left=Math.Min(-pan.X/ViewScale,nodes.Min(n=>BoundsOf(n).Left)),top=Math.Min(-pan.Y/ViewScale,nodes.Min(n=>BoundsOf(n).Top));float right=Math.Max((Width-pan.X)/ViewScale,nodes.Max(n=>BoundsOf(n).Right)),bottom=Math.Max((Height-pan.Y)/ViewScale,nodes.Max(n=>BoundsOf(n).Bottom));minimapWorld=new(left-20,top-20,right-left+40,bottom-top+40);minimapScale=Math.Min((minimapBox.Width-10)/minimapWorld.Width,(minimapBox.Height-10)/minimapWorld.Height);PointF Mini(float x,float y)=>new(minimapBox.X+5+(x-minimapWorld.X)*minimapScale,minimapBox.Y+5+(y-minimapWorld.Y)*minimapScale);using var bg=new SolidBrush(Color.FromArgb(70,73,75));g.FillRectangle(bg,minimapBox);using var frame=new Pen(Color.FromArgb(24,24,24));g.DrawRectangle(frame,minimapBox.X,minimapBox.Y,minimapBox.Width,minimapBox.Height);var state=g.Save();g.SetClip(minimapBox);foreach(var n in nodes){var r=BoundsOf(n);var p=Mini(r.X,r.Y);g.FillRectangle(n.Id==selected?Brushes.Orange:Brushes.Silver,p.X,p.Y,Math.Max(2,r.Width*minimapScale),Math.Max(2,r.Height*minimapScale));}var viewport=Mini(-pan.X/ViewScale,-pan.Y/ViewScale);using var view=new Pen(Color.FromArgb(255,145,0),1.5f);g.DrawRectangle(view,viewport.X,viewport.Y,Width/ViewScale*minimapScale,Height/ViewScale*minimapScale);g.Restore(state);}
    void NavigateMinimap(Point point){if(minimapBox.IsEmpty||minimapScale<=0)return;float x=(point.X-minimapBox.X-5)/minimapScale+minimapWorld.X,y=(point.Y-minimapBox.Y-5)/minimapScale+minimapWorld.Y;pan=new(Width/2-x*ViewScale,Height/2-y*ViewScale);Invalidate();}
    void DrawNode(Graphics g,NodeDocument n,bool ghost=false)
    {
        var r=BoundsOf(n);using var shape=new GraphicsPath();float radius=n.ModuleId=="vision.image-file"?r.Height/2:4;
        shape.AddArc(r.Left,r.Top,radius*2,radius*2,180,90);shape.AddArc(r.Right-radius*2,r.Top,radius*2,radius*2,270,90);shape.AddArc(r.Right-radius*2,r.Bottom-radius*2,radius*2,radius*2,0,90);shape.AddArc(r.Left,r.Bottom-radius*2,radius*2,radius*2,90,90);shape.CloseFigure();
        using(var shadow=new SolidBrush(Color.FromArgb(115,18,17,17))){var st=g.Save();g.TranslateTransform(1,3);g.FillPath(shadow,shape);g.Restore(st);}
        using(var body=new LinearGradientBrush(r,ghost?Color.FromArgb(205,205,195):n.Enabled?Color.FromArgb(250,247,245):Color.FromArgb(195,195,195),ghost?Color.FromArgb(185,185,180):n.Enabled?Color.FromArgb(235,230,226):Color.FromArgb(170,170,170),LinearGradientMode.Vertical))g.FillPath(body,shape);
        using var border=new Pen(selected==n.Id?Color.FromArgb(255,145,0):Color.FromArgb(180,180,176),selected==n.Id?2:1);if(ghost||dragged==n.Id)border.DashStyle=DashStyle.Dash;g.DrawPath(border,shape);

        var logo=ModuleIcon?.Invoke(n.ModuleId)??VmUiAssets.Get("tool-"+n.ModuleId.Replace("vision.","")+".png");if(logo is not null)g.DrawImage(logo,r.X+4,r.Y+7,16,16);else{
        using var icon=new Pen(Color.FromArgb(255,132,20),1.5f);g.DrawRectangle(icon,r.X+5,r.Y+8,14,14);g.DrawLine(icon,r.X+8,r.Y+12,r.X+16,r.Y+12);g.DrawLine(icon,r.X+8,r.Y+17,r.X+14,r.Y+17);
        }
        using var text=new SolidBrush(ForeColor);using var fmt=new StringFormat{Trimming=StringTrimming.EllipsisCharacter,FormatFlags=StringFormatFlags.NoWrap,LineAlignment=StringAlignment.Center};using var bold=new Font(Font,FontStyle.Bold);g.DrawString(n.Name,bold,text,new RectangleF(r.X+26,r.Y+2,r.Width-31,r.Height-4),fmt);
        if(!ghost&&(hoveredNode==n.Id||connecting is not null))foreach(var p in Ports(n)){using var brush=new SolidBrush(executionMode?Color.FromArgb(255,164,30):KindColor(p.Port.Kind));g.FillRectangle(brush,p.Position.X-2.5f,p.Position.Y-2.5f,5,5);using var outline=new Pen(Color.FromArgb(75,75,75),.7f);g.DrawRectangle(outline,p.Position.X-2.5f,p.Position.Y-2.5f,5,5);}

        if(statuses.TryGetValue(n.Id,out var state)){using var sb=new SolidBrush(StatusColor(state));g.FillEllipse(sb,r.Right-11,r.Top+5,6,6);}
        if(Descriptor(n) is null){using var warning=new Pen(Color.Red,2);g.DrawLine(warning,r.Right-18,r.Top+8,r.Right-8,r.Top+18);}
    }
    void Select(string? id){if(id is not null)selectedLine=null;if(selected==id)return;selected=id;Invalidate();SelectionChanged?.Invoke(id);}
    void CancelGesture(){dragged=null;connecting=null;panning=false;minimapDragging=false;Capture=false;Cursor=Cursors.Default;}
    protected override void OnResize(EventArgs e){base.OnResize(e);routeCache.Clear();obstructedRoutes.Clear();Invalidate();}
    protected override void Dispose(bool disposing){if(disposing)portTip.Dispose();base.Dispose(disposing);}
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);Focus();if(e.Button==MouseButtons.Left&&minimapBox.Contains(e.Location)){minimapDragging=true;Capture=true;NavigateMinimap(e.Location);return;}var p=World(e.Location);lastMouse=p;
        if(e.Button==MouseButtons.Right){if(readOnly)return;var connection=HitLine(p);selectedLine=connection;Invalidate();if(connection is not null){var menu=new ContextMenuStrip();menu.Items.Add("断开连接",null,(_,_)=>DisconnectRequested?.Invoke(connection));menu.Closed+=(_,_)=>menu.Dispose();menu.Show(this,e.Location);}return;}
        if(e.Button!=MouseButtons.Left&&e.Button!=MouseButtons.Middle)return;
        if(e.Button==MouseButtons.Left){var port=HitPort(p);if(!readOnly&&port?.Output==true){connecting=port;Capture=true;Invalidate();return;}var node=HitNode(p);if(node is not null){Select(node.Id);if(!readOnly){dragged=node.Id;dragStart=p;dragOriginal=new((float)node.X,(float)node.Y);dragPreview=dragOriginal;Capture=true;}return;}var line=HitLine(p);Select(null);selectedLine=line;if(line is not null){Invalidate();return;}}
        panning=true;dragStart=e.Location;Capture=true;Cursor=Cursors.Hand;
    }
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);if(minimapDragging){NavigateMinimap(e.Location);return;}lastMouse=World(e.Location);var hovered=HitPort(lastMouse);portTip.SetToolTip(this,hovered is null?null:$"{(hovered.Output?"输出":"输入")} · {PortName(hovered.Port.Name)} · {hovered.Port.Kind}");
        if(panning){pan=new(pan.X+e.X-dragStart.X,pan.Y+e.Y-dragStart.Y);dragStart=e.Location;Invalidate();}
        else if(dragged is not null){var p=World(e.Location);dragPreview=new(dragOriginal.X+p.X-dragStart.X,dragOriginal.Y+p.Y-dragStart.Y);Invalidate();}
        else if(connecting is not null)Invalidate();
        else {hoveredNode=hovered?.Node.Id??HitNode(lastMouse)?.Id;hoveredLine=hoveredNode is null?HitLine(lastMouse):null;Cursor=hovered is not null?Cursors.Cross:hoveredNode is not null||hoveredLine is not null?Cursors.Hand:Cursors.Default;Invalidate();}
    }
    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);if(minimapDragging){CancelGesture();Invalidate();return;}var source=connecting;var moved=dragged;var position=dragPreview;var original=dragOriginal;var target=source is null?null:HitPort(World(e.Location));CancelGesture();Invalidate();if(readOnly)return;
        if(source is not null&&target is not null&&(executionMode||!target.Output)&&target.Node.Id!=source.Node.Id&&target.Port.Kind==source.Port.Kind&&e.Button==MouseButtons.Left)ConnectionRequested?.Invoke(new ConnectionDocument(source.Node.Id,source.Port.Name,target.Node.Id,executionMode?"__flow_in":target.Port.Name){SourceIndex=source.Index<0?3:source.Index,TargetIndex=target.Index<0?1:target.Index});
        if(moved is not null&&e.Button==MouseButtons.Left&&Distance(position,original)>.1f)NodeMoved?.Invoke(moved,Math.Round(position.X/5)*5,Math.Round(position.Y/5)*5);
    }
    protected override void OnMouseDoubleClick(MouseEventArgs e){base.OnMouseDoubleClick(e);if(readOnly||e.Button!=MouseButtons.Left)return;var p=World(e.Location);if(HitNode(p)is {} node){Select(node.Id);NodeActivated?.Invoke(node.Id);}else if(HitLine(p)is {} c){selectedLine=c;Invalidate();}}
    protected override void OnMouseLeave(EventArgs e){base.OnMouseLeave(e);hoveredNode=null;hoveredLine=null;portTip.SetToolTip(this,null);Invalidate();}
    protected override void OnMouseCaptureChanged(EventArgs e){base.OnMouseCaptureChanged(e);if(!Capture){dragged=null;connecting=null;panning=false;Cursor=Cursors.Default;Invalidate();}}
    protected override void OnMouseWheel(MouseEventArgs e){base.OnMouseWheel(e);routeCache.Clear();obstructedRoutes.Clear();var p=World(e.Location);zoom=Math.Clamp(zoom*MathF.Pow(1.12f,e.Delta/120f),.35f,2.5f);pan=new(e.X-p.X*ViewScale,e.Y-p.Y*ViewScale);Invalidate();}
    protected override bool IsInputKey(Keys k)=>k is Keys.Delete or Keys.Escape||base.IsInputKey(k);
    protected override void OnKeyDown(KeyEventArgs e){base.OnKeyDown(e);if(e.KeyCode==Keys.Escape){CancelGesture();selectedLine=null;Invalidate();e.Handled=true;}else if(e.KeyCode==Keys.Delete&&!readOnly){if(selectedLine is {} line){selectedLine=null;DisconnectRequested?.Invoke(line);}else if(selected is not null)DeleteRequested?.Invoke();e.Handled=true;}}
    static string? DroppedModule(IDataObject? data){var text=data?.GetData(DataFormats.StringFormat)as string??data?.GetData(DataFormats.Text)as string;return text?.StartsWith("vision-module:",StringComparison.Ordinal)==true&&text.Length>14?text[14..]:null;}
    protected override void OnDragEnter(DragEventArgs e){base.OnDragEnter(e);e.Effect=!readOnly&&DroppedModule(e.Data)is not null?DragDropEffects.Copy:DragDropEffects.None;}
    protected override void OnDragOver(DragEventArgs e){base.OnDragOver(e);e.Effect=!readOnly&&DroppedModule(e.Data)is not null?DragDropEffects.Copy:DragDropEffects.None;}
    protected override void OnDragDrop(DragEventArgs e){base.OnDragDrop(e);if(readOnly||DroppedModule(e.Data)is not {} id)return;var p=World(PointToClient(new(e.X,e.Y)));ModuleDropped?.Invoke(id,Math.Round(p.X/5)*5,Math.Round(p.Y/5)*5);}
}





