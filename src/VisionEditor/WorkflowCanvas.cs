using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using VisionFramework;
using VisionEditor.Core;
using VisionEditor.Core.VmRouting;
using Color=System.Windows.Media.Color;
using Brushes=System.Windows.Media.Brushes;
using Image=System.Windows.Controls.Image;
using Rectangle=System.Windows.Shapes.Rectangle;
using Point=System.Windows.Point;
using Size=System.Windows.Size;
using Cursors=System.Windows.Input.Cursors;
using MouseEventArgs=System.Windows.Input.MouseEventArgs;
using MenuItem=System.Windows.Controls.MenuItem;
using ContextMenu=System.Windows.Controls.ContextMenu;
using KeyEventArgs=System.Windows.Input.KeyEventArgs;
using TextBox=System.Windows.Controls.TextBox;
using ComboBox=System.Windows.Controls.ComboBox;
using HorizontalAlignment=System.Windows.HorizontalAlignment;
using Panel=System.Windows.Controls.Panel;

namespace VisionEditor;
public partial class MainWindow
{
    const double NodeWidth=110,NodeHeight=30;
    readonly HashSet<string> selectedNodes=new(StringComparer.Ordinal);
    readonly Dictionary<string,Canvas> nodeCards=new();
    readonly Dictionary<string,(double Scale,double X,double Y)> canvasViews=new();
    readonly Dictionary<(string Procedure,string From,string To),(RoutePort Source,RoutePort Target,IReadOnlyList<RoutePoint> Points)> automaticRoutes=new();
    string? canvasProcedure,selectedWire;
    bool syncingViewport,eyeExpanded=true,eyeDragging,panning,boxSelecting;
    Point pointerStart,eyePointerStart,eyeRegionStart,nodePointerStart;
    const double DiagramPageSize=5000; // DiagramPage.DiagramPageWidth and constructor.
    double worldWidth=DiagramPageSize,worldHeight=DiagramPageSize;
    Rectangle? selectionBox;
    Dictionary<string,Point>? movingNodes;
    readonly Dictionary<string,Canvas> movingGhosts=new();
    (string Node,int Port)? pendingConnection;
    System.Windows.Shapes.Path? connectionPreview;

    void InitializeCanvasInput()
    {
        GraphLayer.Focusable=true;
        CanvasViewport.PreviewMouseDown+=CanvasPointerDown;
        CanvasViewport.MouseMove+=CanvasPointerMove;
        CanvasViewport.PreviewMouseUp+=CanvasPointerUp;
        CanvasViewport.PreviewMouseWheel+=CanvasWheel;
        CanvasViewport.LostMouseCapture+=(_,_)=> {if(nodeOleActive)return;if(wireDrag is not null)FinishWireDrag(false);else if(movingNodes is not null||pendingConnection is not null||boxSelecting||panning)CancelCanvasGesture();};
        InitializeInputBoundaries();
        GraphLayer.MouseRightButtonDown+=(_,e)=>{if(!IsCanvasBackground(e.OriginalSource as DependencyObject))return;GraphLayer.ContextMenu=CanvasSelectionMenu(e.GetPosition(GraphLayer));GraphLayer.ContextMenu.IsOpen=true;e.Handled=true;};
        PreviewKeyDown+=CanvasKeyDown;
        ZoomSlider.ValueChanged+=(_,e)=> {if(!syncingViewport&&doc is not null)SetCanvasZoom(e.NewValue,new Point(CanvasViewport.ActualWidth/2,CanvasViewport.ActualHeight/2));};
        MiniMapToggle.Content=new Image {Source=Asset("ic_retract_normal.png"),Stretch=Stretch.None};
        MiniMapToggle.MouseEnter+=(_,_)=>SetEyeIcon(true);
        MiniMapToggle.MouseLeave+=(_,_)=>SetEyeIcon(false);
        MiniMapPreview.Fill=new VisualBrush {Visual=GraphLayer}; // ZoomArea.RectEagleEye.Fill = new VisualBrush(cp).
        MiniMapPanel.Effect=new System.Windows.Media.Effects.DropShadowEffect {Opacity=.45,ShadowDepth=2,BlurRadius=8,Color=Colors.Black};
        GraphLayer.Background=OriginalCanvasGrid();
        doc.SolutionReplaced+=()=>automaticRoutes.Clear();
        CanvasScrollViewer.ScrollChanged+=(_,_)=> {if(!syncingViewport){panX=-CanvasScrollViewer.HorizontalOffset;panY=-CanvasScrollViewer.VerticalOffset;UpdateMiniMap();}};
    }

    Point World(Point p)=>new((p.X-panX)/scale,(p.Y-panY)/scale);
    Rect ViewRect()=>new(CanvasScrollViewer.HorizontalOffset/scale,CanvasScrollViewer.VerticalOffset/scale,Math.Max(1,CanvasScrollViewer.ViewportWidth)/scale,Math.Max(1,CanvasScrollViewer.ViewportHeight)/scale);
    static double Snap(double value)=>Math.Round(value/5)*5;
    static double NodeVisualHeight(NodeDocument n)=>n.ModuleId==WorkflowCalls.ModuleId?40:NodeHeight; Rect NodeBounds(NodeDocument n)=>new(n.X,n.Y,NodeWidth,NodeVisualHeight(n));

    void RenderGraph()
    {
        if(!IsInitialized||doc is null)return;
        CheckGestureBoundary();
        if(wireDrag is not null&&(!EditableCanvas||wireDragRevision!=doc.Revision)){wireDrag=null;draggedWire=null;wireDragOriginalPoints=null;CanvasViewport.ReleaseMouseCapture();}
        if(workflowOverview){RenderWorkflowOverview();return;}
        if(canvasProcedure!=doc.CurrentProcedureId)
        {
            wireDrag=null;draggedWire=null;wireDragOriginalPoints=null;
            pendingConnection=null;connectionTarget=null;boxSelecting=false;boxSelectionActive=false;panning=false;CancelNodeMove();
            if(canvasProcedure is not null)canvasViews[canvasProcedure]=(scale,panX,panY);
            canvasProcedure=doc.CurrentProcedureId;
            var saved=canvasViews.GetValueOrDefault(canvasProcedure,(1d,0d,0d));scale=saved.Item1;panX=saved.Item2;panY=saved.Item3;
            selectedNodes.Clear();selectedWire=null;pendingConnection=null;
        }
        if(HasEditingGesture){ApplyCanvasTransform();return;}
        var procedure=doc.CurrentProcedure;
        selectedNodes.RemoveWhere(id=>procedure.Nodes.All(n=>n.Id!=id));
        if(selected is not null&&procedure.Nodes.Any(n=>n.Id==selected))selectedNodes.Add(selected);
        GraphLayer.Children.Clear();nodeCards.Clear();executionPorts.Clear();wireViews.Clear();wireArrows.Clear();wireHitPaths.Clear();wireHandles.Clear();groupSelectionFrame=null;
        worldWidth=DiagramPageSize;
        worldHeight=DiagramPageSize;
        GraphLayer.Width=worldWidth;GraphLayer.Height=worldHeight;
        var boxes=procedure.Nodes.ToDictionary(n=>n.Id,n=>new RouteRect(n.X,n.Y,NodeWidth,NodeVisualHeight(n)));
        var wires=ProcedureGraph.ExecutionEdges(procedure).ToArray();
        var activeWires=wires.Select(w=>(w.FromNode,w.ToNode)).ToHashSet();
        var liveProcedures=doc.Document.Procedures.Select(p=>p.Id).ToHashSet();
        foreach(var old in automaticRoutes.Keys.Where(k=>!liveProcedures.Contains(k.Procedure)||(k.Procedure==procedure.Id&&!activeWires.Contains((k.From,k.To)))).ToArray())automaticRoutes.Remove(old);
        foreach(var wire in wires)
        {
            if(!boxes.TryGetValue(wire.FromNode,out var sourceBox)||!boxes.TryGetValue(wire.ToNode,out var targetBox))continue;
            var cacheKey=(procedure.Id,wire.FromNode,wire.ToNode);
            IReadOnlyList<RoutePoint> points;
            if(wire.PathPoints is {Count:>1})
            {
                automaticRoutes.Remove(cacheKey);
                points=wire.PathPoints.Select(p=>new RoutePoint(p.X,p.Y)).ToArray();
            }
            else
            {
                var source=Port(sourceBox,wire.SourceIndex);var target=Port(targetBox,wire.TargetIndex);
                if(automaticRoutes.TryGetValue(cacheKey,out var previous)&&previous.Source==source&&previous.Target==target)points=previous.Points;
                else
                {
                    double routeWidth=Math.Min(DiagramPageSize,Math.Max(200,boxes.Values.Max(b=>b.Right)+100)),routeHeight=Math.Min(DiagramPageSize,Math.Max(200,boxes.Values.Max(b=>b.Bottom)+100));
                    // Connection.OnPositionChanged invalidates only its own endpoint/port changes.
                    // DiagramPage.GetPathMap is therefore rerun for those changes, not for an unrelated obstacle move.
                    points=VmOrthogonalRouter.Route(source,target,boxes.Where(p=>p.Key!=wire.FromNode&&p.Key!=wire.ToNode).Select(p=>p.Value),routeWidth,routeHeight,1_100_000).Points;
                    if(points.Count>=2)automaticRoutes[cacheKey]=(source,target,points);else automaticRoutes.Remove(cacheKey);
                }
            }
            if(points.Count<2) { StatusText.Text="有连线无法在当前画布范围内完成布线";continue; }
            string key=wire.FromNode+">"+wire.ToNode;
            var displayedPoints=wire.PathPoints is {Count:>1}?VmOrthogonalRouter.Passivate(points):points;
            var path=new System.Windows.Shapes.Path {Data=LineGeometry(displayedPoints),Stroke=WireStroke(wire),StrokeThickness=2,IsHitTestVisible=false};
            wireViews[key]=(wire,path,wire.PathPoints is {Count:>1}?points:CleanWirePoints(points));
            GraphLayer.Children.Add(path);
            var arrow=new Polygon {Points=OriginalWireArrow(displayedPoints),IsHitTestVisible=false};arrow.SetBinding(System.Windows.Shapes.Shape.FillProperty,new System.Windows.Data.Binding("Stroke"){Source=path});wireArrows[key]=arrow;GraphLayer.Children.Add(arrow);
            var hit=new System.Windows.Shapes.Path {Data=path.Data,Stroke=Brushes.Transparent,StrokeThickness=12,Focusable=true};
            // Connection.RequestBringIntoView suppresses focus-driven scroll jumps.
            hit.RequestBringIntoView+=(_,e)=>e.Handled=true;
            hit.LostKeyboardFocus+=(_,_)=>DrawWireHandles();
            hit.GotKeyboardFocus+=(_,_)=>DrawWireHandles();
            hit.MouseEnter+=(_,_)=>path.StrokeThickness=3;hit.MouseLeave+=(_,_)=>path.StrokeThickness=selectedWire==key?3:2;
            // Connection.OnPreviewMouseDown selects for every mouse button, including right-click.
            hit.PreviewMouseDown+=(_,e)=> {selectedWire=key;selected=null;selectedNodes.Clear();boxSelectionActive=false;hit.Focus();RefreshNodeSelection();RefreshRight();e.Handled=true;};
            var menu=new ContextMenu();var delete=new MenuItem {Header="删除",IsEnabled=!isLocked&&activeRun is null};delete.Click+=(_,_)=>Safe(()=> {doc.DisconnectExecution(wire);selectedWire=null;});menu.Items.Add(delete);hit.ContextMenu=menu;
            wireHitPaths[key]=hit;GraphLayer.Children.Add(hit);
        }
        for(int i=0;i<procedure.Nodes.Count;i++)
        {
            var node=procedure.Nodes[i];var card=CreateNodeCard(node,i,selectedNodes.Contains(node.Id));
            nodeCards[node.Id]=card;Canvas.SetLeft(card,node.X-10);Canvas.SetTop(card,node.Y-10);GraphLayer.Children.Add(card);
            card.MouseLeftButtonDown+=NodeDown;
            var ports=Enumerable.Range(0,4).Select(index=>CreateExecutionPort(node,index)).ToArray();
            foreach(var port in ports)GraphLayer.Children.Add(port);
            card.MouseEnter+=(_,_)=> {if(!isLocked&&movingNodes is null)foreach(var port in ports)port.Visibility=Visibility.Visible;};
            card.MouseLeave+=(_,_)=>Dispatcher.BeginInvoke(new Action(()=> {if(pendingConnection is null&&!card.IsMouseOver&&!ports.Any(p=>p.IsMouseOver))foreach(var port in ports)port.Visibility=Visibility.Collapsed;}));
            var menu=new ContextMenu();var open=new MenuItem {Header="参数设置"};open.Click+=(_,_)=>OpenNode(node.Id);menu.Items.Add(open);if(node.ModuleId==WorkflowCalls.ModuleId){var enter=new MenuItem {Header="进入 Group",IsEnabled=node.Call is not null};enter.Click+=(_,_)=>EnterSubprogram(node.Id);menu.Items.Add(enter);}
            var copy=new MenuItem {Header="复制",InputGestureText="Ctrl+C",IsEnabled=EditableCanvas};copy.Click+=(_,_)=>CopySelection();menu.Items.Add(copy);
            var remove=new MenuItem {Header="删除",IsEnabled=!isLocked&&activeRun is null};remove.Click+=(_,_)=> {selectedNodes.Add(node.Id);DeleteSelection();};menu.Items.Add(remove);card.ContextMenu=menu;
            card.MouseRightButtonDown+=(_,_)=> {if(!selectedNodes.Contains(node.Id)){selectedNodes.Clear();boxSelectionActive=false;}selectedNodes.Add(node.Id);selected=node.Id;RefreshNodeSelection();RefreshRight();};
        }
        DrawSelectionGroup();DrawWireHandles();ApplyCanvasTransform();
    }

    Canvas CreateNodeCard(NodeDocument node,int index,bool isSelected)
    {
        var card=new Canvas {Width=130,Height=NodeVisualHeight(node)+20,Background=Brushes.Transparent,Tag=node.Id}; if(node.ModuleId==WorkflowCalls.ModuleId)return CreateGroupCard(node,isSelected,card);
        var body=new Border {Width=NodeWidth,Height=NodeHeight,CornerRadius=new CornerRadius(node.ModuleId=="vision.image-source"?15:4),Background=new LinearGradientBrush(Color.FromRgb(250,247,245),Color.FromRgb(235,230,226),90),BorderBrush=isSelected?orange:Brushes.Gray,BorderThickness=new Thickness(isSelected?2:.7),Effect=new System.Windows.Media.Effects.DropShadowEffect {BlurRadius=3,ShadowDepth=3,Opacity=.45}};
        if(!node.Enabled)body.Background=new LinearGradientBrush(Color.FromArgb(211,211,211,245),Color.FromArgb(211,211,211,225),90);
        var content=new Grid();content.ColumnDefinitions.Add(new ColumnDefinition {Width=new GridLength(24)});content.ColumnDefinitions.Add(new ColumnDefinition());
        var icon=new Image {Source=ToolAsset(node.ModuleId),Width=16,Height=16,Margin=new Thickness(4,0,0,0),VerticalAlignment=VerticalAlignment.Center};content.Children.Add(icon);
        var text=new TextBlock {Text=index+node.Name,FontSize=12,FontWeight=FontWeights.Bold,Foreground=new SolidColorBrush(Color.FromRgb(55,58,60)),VerticalAlignment=VerticalAlignment.Center,HorizontalAlignment=HorizontalAlignment.Center,TextTrimming=TextTrimming.CharacterEllipsis};Grid.SetColumn(text,1);content.Children.Add(text);body.Child=content;
        Canvas.SetLeft(body,10);Canvas.SetTop(body,10);card.Children.Add(body);return card;
    }

    static RoutePort Port(RouteRect box,int index)=>index switch
    {
        0=>new(new(box.X,box.Y+box.Height/2),PortDirection.Left,box),1=>new(new(box.X+55,box.Y),PortDirection.Up,box),2=>new(new(box.Right,box.Y+box.Height/2),PortDirection.Right,box),_=>new(new(box.X+55,box.Bottom),PortDirection.Down,box)
    };
    static PathGeometry LineGeometry(IReadOnlyList<RoutePoint> points)
    {
        var geometry=new PathGeometry();if(points.Count==0)return geometry;
        var figure=new PathFigure {StartPoint=new Point(points[0].X,points[0].Y)};foreach(var point in points.Skip(1))figure.Segments.Add(new LineSegment(new Point(point.X,point.Y),true));geometry.Figures.Add(figure);return geometry;
    }

    Border CreateExecutionPort(NodeDocument node,int index)
    {
        var area=PortHitRect(node,index);
        var handle=new Border {Background=new SolidColorBrush(Color.FromRgb(255,167,25)),BorderBrush=new SolidColorBrush(Color.FromRgb(68,68,68))};
        var layer=new Canvas {Width=area.Width,Height=area.Height,Background=Brushes.Transparent};layer.Children.Add(handle);
        var hit=new Border {Width=area.Width,Height=area.Height,Background=Brushes.Transparent,Child=layer,Visibility=Visibility.Collapsed,Cursor=Cursors.Cross,Tag=(node.Id,index)};
        SetPortHit(hit,false);Canvas.SetLeft(hit,area.X);Canvas.SetTop(hit,area.Y);Panel.SetZIndex(hit,20);executionPorts[(node.Id,index)]=hit;
        hit.MouseEnter+=(_,_)=> {hit.Visibility=isLocked?Visibility.Collapsed:Visibility.Visible;SetPortHit(hit,true);};
        hit.MouseLeave+=(_,_)=> {SetPortHit(hit,connectionTarget==(node.Id,index));if(pendingConnection is null&&!nodeCards[node.Id].IsMouseOver)hit.Visibility=Visibility.Collapsed;};
        hit.MouseLeftButtonDown+=(_,e)=> {BeginConnection(node.Id,index);e.Handled=true;};
        return hit;
    }

    void NodeDown(object sender,MouseButtonEventArgs e)
    {
        if(sender is not Canvas {Tag:string id})return;
        GraphLayer.Focus();selectedWire=null;
        if(e.ClickCount==2){if(doc.CurrentProcedure.Nodes.Single(n=>n.Id==id).ModuleId==WorkflowCalls.ModuleId)EnterSubprogram(id);else OpenNode(id);e.Handled=true;return;}
        if(!selectedNodes.Contains(id)){selectedNodes.Clear();selectedNodes.Add(id);boxSelectionActive=false;}
        selected=id;RefreshNodeSelection();RefreshRight();
        if(!isLocked&&activeRun is null)BeginNodeMove(e.GetPosition(GraphLayer));
        e.Handled=true;
    }
    void RefreshNodeSelection()
    {
        foreach(var pair in nodeCards)if(pair.Value.Children[0] is Border body){body.BorderBrush=selectedNodes.Contains(pair.Key)?orange:Brushes.Gray;body.BorderThickness=new Thickness(selectedNodes.Contains(pair.Key)?2:.7);var groupNode=doc.CurrentProcedure.Nodes.Single(n=>n.Id==pair.Key);if(groupNode.ModuleId==WorkflowCalls.ModuleId){body.BorderThickness=new Thickness(0);body.Background=GroupBackground(groupNode,selectedNodes.Contains(pair.Key));}}
        foreach(var entry in wireViews.Values)entry.View.Stroke=WireStroke(entry.Link);
        DrawSelectionGroup();
        DrawWireHandles();
    }
    void BeginNodeMove(Point point)
    {
        BeginNodeGesture(point,false);nodeOleArmed=movingNodes is {Count:>0};
    }
    void PreviewNodeMove(Point point)
    {
        if(movingNodes is null||movingNodes.Count==0)return;Vector delta=point-nodePointerStart;
        if(Math.Abs(delta.X)<=3&&Math.Abs(delta.Y)<=3&&movingGhosts.Count==0)return;
        double left=movingNodes.Values.Min(p=>p.X),top=movingNodes.Values.Min(p=>p.Y);
        double dx=Snap(left+delta.X)-left,dy=Snap(top+delta.Y)-top;
        // ItemsPage.GetDropItems clamps each selected DiagramItem separately.
        // A group-wide limit would freeze all items when just one reaches an edge.
        if(movingNodes.Count==1)
        {
            dx=Math.Clamp(dx,-left,Math.Max(-left,DiagramPageSize-NodeWidth-movingNodes.Values.Max(p=>p.X)));
            double bottom=movingNodes.Max(entry=>entry.Value.Y+NodeVisualHeight(doc.CurrentProcedure.Nodes.Single(n=>n.Id==entry.Key)));
            dy=Math.Clamp(dy,-top,Math.Max(-top,DiagramPageSize-bottom));
        }
        foreach(var entry in movingNodes)
        {
            if(!movingGhosts.TryGetValue(entry.Key,out var ghost))
            {
                var node=doc.CurrentProcedure.Nodes.First(n=>n.Id==entry.Key);ghost=CreateNodeCard(node,doc.CurrentProcedure.Nodes.ToList().FindIndex(n=>n.Id==entry.Key),true);ghost.Opacity=.7;ghost.IsHitTestVisible=false;Panel.SetZIndex(ghost,60);movingGhosts[entry.Key]=ghost;GraphLayer.Children.Add(ghost);
            }
            var height=NodeVisualHeight(doc.CurrentProcedure.Nodes.Single(n=>n.Id==entry.Key));
            Canvas.SetLeft(ghost,Math.Clamp(entry.Value.X+dx,0,DiagramPageSize-NodeWidth)-10);
            Canvas.SetTop(ghost,Math.Clamp(entry.Value.Y+dy,0,DiagramPageSize-height)-10);
        }
        if(boxSelectionActive)
        {
            if(groupMoveFrame is not null)GraphLayer.Children.Remove(groupMoveFrame);
            var leftEdge=movingGhosts.Values.Min(g=>Canvas.GetLeft(g)+10);
            var topEdge=movingGhosts.Values.Min(g=>Canvas.GetTop(g)+10);
            var rightEdge=movingGhosts.Values.Max(g=>Canvas.GetLeft(g)+10+NodeWidth);
            var bottomEdge=movingGhosts.Max(g=>Canvas.GetTop(g.Value)+10+NodeVisualHeight(doc.CurrentProcedure.Nodes.Single(n=>n.Id==g.Key)));
            var area=new Rect(new Point(leftEdge,topEdge),new Point(rightEdge,bottomEdge));area.Inflate(10,10);
            groupMoveFrame=GroupRectangle(area,false);groupMoveFrame.Opacity=.7;GraphLayer.Children.Add(groupMoveFrame);
        }
        DrawAlignmentGuides(left+dx,top+dy);
        if(movingNodes.Count==1)UpdateNodeHotRegion(left+dx,top+dy,movingNodes.Keys);
        if(movingNodes.Count==1&&!nodeCopyDrag)UpdateInsertionWire(point,movingNodes.Keys);
    }
    void CommitNodeMove()
    {
        if(!EditableCanvas||canvasGestureRevision!=doc.Revision){CancelCanvasGesture();return;}
        var positions=movingGhosts.ToDictionary(p=>p.Key,p=>(Canvas.GetLeft(p.Value)+10,Canvas.GetTop(p.Value)+10));
        if(positions.Count==1&&ActiveNodeHotPoint is {} hot)positions[positions.Keys.Single()]=(hot.X,hot.Y);
        var fragment=nodeCopyDrag&&movingNodes is not null&&positions.Count>0?doc.CopyNodes(movingNodes.Keys):null;
        var insertion=positions.Count==1&&!nodeCopyDrag?insertWire:null;
        CancelNodeMove();Safe(()=>{if(fragment is not null){graphClipboard=fragment;graphClipboardSolution=doc.Document.Id;SelectPasted(doc.PasteNodes(fragment,positions.Values.Min(p=>p.Item1),positions.Values.Min(p=>p.Item2)));}
        else if(insertion is not null){var position=positions.Single();doc.InsertExecutionNode(insertion,position.Key,position.Value.Item1,position.Value.Item2);}
        else if(positions.Count>0)doc.MoveNodes(positions);else RenderGraph();});
    }
    void CancelNodeMove(){nodeOleArmed=false;HideNodePreview();movingNodes=null;nodeCopyDrag=false;CanvasViewport.ReleaseMouseCapture();}

    void CanvasPointerDown(object sender,MouseButtonEventArgs e)
    {
        if(e.ChangedButton!=MouseButton.Left)return;
        bool background=IsCanvasBackground(e.OriginalSource as DependencyObject);
        if((Keyboard.Modifiers&ModifierKeys.Control)!=0&&background)
        {
            panning=true;pointerStart=e.GetPosition(CanvasViewport);CanvasViewport.CaptureMouse();e.Handled=true;return;
        }
        if(!background||workflowOverview)return;
        GraphLayer.Focus();selected=null;selectedWire=null;selectedNodes.Clear();boxSelectionActive=false;DrawSelectionGroup();DrawWireHandles();RefreshRight();
        if(isLocked||activeRun is not null){RenderGraph();return;}
        canvasGestureRevision=doc.Revision;pointerStart=e.GetPosition(GraphLayer);boxSelecting=true;
        // ItemsPage.ToolItemGrid_PreviewMouseMove uses an unfilled LightSlateGray
        // adorner path with a {2,1} dash pattern during marquee selection.
        selectionBox=new Rectangle {Stroke=Brushes.LightSlateGray,StrokeThickness=1/scale,StrokeDashArray=new DoubleCollection {2,1},Fill=Brushes.Transparent,IsHitTestVisible=false};GraphLayer.Children.Add(selectionBox);CanvasViewport.CaptureMouse();e.Handled=true;
    }
    void CanvasPointerMove(object sender,MouseEventArgs e)
    {
        if(wireDrag is not null){PreviewWireDrag(e.GetPosition(GraphLayer));return;}
        if(panning)
        {
            if((Keyboard.Modifiers&ModifierKeys.Control)==0)return;
            MoveCanvasPan(e.GetPosition(CanvasViewport));return;
        }
        if(movingNodes is not null){if(nodeOleArmed)StartNodeOleDrag(e);else PreviewNodeMove(e.GetPosition(GraphLayer));return;}
        if(boxSelecting&&selectionBox is not null)
        {
            var rect=new Rect(pointerStart,e.GetPosition(GraphLayer));Canvas.SetLeft(selectionBox,rect.X);Canvas.SetTop(selectionBox,rect.Y);selectionBox.Width=rect.Width;selectionBox.Height=rect.Height;return;
        }
        if(pendingConnection is {} from)
        {
            UpdateConnectionPreview(e.GetPosition(GraphLayer));
        }
    }
    void MoveCanvasPan(Point position)
    {
        var delta=position-pointerStart;
        pointerStart=position; // ZoomArea.ScrollViewer_MouseMove advances lastDragPoint on every move.
        panX+=delta.X;panY+=delta.Y;
        ApplyCanvasTransform();
    }
    void CanvasPointerUp(object sender,MouseButtonEventArgs e)
    {
        if(e.ChangedButton!=MouseButton.Left)return;
        if(wireDrag is not null){FinishWireDrag(IsCanvasDropPoint(e.GetPosition(CanvasViewport)));e.Handled=true;return;}
        if(panning){panning=false;CanvasViewport.ReleaseMouseCapture();UpdateMiniMap();e.Handled=true;return;}
        if(movingNodes is not null){CompleteNodeDrop(e.GetPosition(CanvasViewport));e.Handled=true;return;}
        if(boxSelecting)
        {
            var rect=new Rect(pointerStart,e.GetPosition(GraphLayer));boxSelecting=false;CanvasViewport.ReleaseMouseCapture();
            selectionBox=null;SetBoxSelection(rect);e.Handled=true;return;
        }
        if(pendingConnection is {} from)
        {
            if(IsCanvasDropPoint(e.GetPosition(CanvasViewport)))FinishConnection(e.GetPosition(GraphLayer));else CancelCanvasGesture();e.Handled=true;
        }
    }

    void CanvasKeyDown(object sender,KeyEventArgs e)
    {
        if(Keyboard.FocusedElement is TextBox||Keyboard.FocusedElement is ComboBox)return;
        if(e.Key==Key.Escape){selectedWire=null;CancelCanvasGesture();e.Handled=true;}
        // Native DiagramItem/RectDragPanal delete handlers are keyboard-focus scoped;
        // hovering the canvas while a toolbar or another pane owns focus is insufficient.
        if(e.Key==Key.Delete&&GraphLayer.IsKeyboardFocusWithin){DeleteSelection();e.Handled=true;}
        if((Keyboard.Modifiers&ModifierKeys.Control)!=0&&GraphLayer.IsKeyboardFocusWithin)
        {
            if(e.Key==Key.C){CopySelection();e.Handled=true;}
            if(e.Key==Key.V){Safe(()=>PasteSelection(null));e.Handled=true;}
            if(e.Key==Key.A&&boxSelectionActive){SetSelectionEnabled(true);e.Handled=true;}
            if(e.Key==Key.F&&boxSelectionActive){SetSelectionEnabled(false);e.Handled=true;}
            if(e.Key==Key.Z&&!isLocked&&activeRun is null){doc.Undo();e.Handled=true;}
            if(e.Key==Key.Y&&!isLocked&&activeRun is null){doc.Redo();e.Handled=true;}
        }
    }
    void DeleteSelection()
    {
        if(!EditableCanvas)return;
        if(wireDrag is not null)FinishWireDrag(false);
        if(selectedWire is not null){var wire=ProcedureGraph.ExecutionEdges(doc.CurrentProcedure).FirstOrDefault(w=>w.FromNode+">"+w.ToNode==selectedWire);selectedWire=null;if(wire is not null)doc.DisconnectExecution(wire);return;}
        var ids=selectedNodes.ToArray();selected=null;selectedNodes.Clear();doc.DeleteNodes(ids);
    }

    void CanvasWheel(object sender,MouseWheelEventArgs e)
    {
        // ZoomArea.OnPreviewMouseWheel calls OnHoriScroll, then OnChangeZoom.
        // Unmodified wheel is left to WPF ScrollViewer's native wheel policy.
        if((Keyboard.Modifiers&ModifierKeys.Shift)!=0){if(e.Delta<0)CanvasScrollViewer.LineRight();else if(e.Delta>0)CanvasScrollViewer.LineLeft();e.Handled=true;}
        if((Keyboard.Modifiers&ModifierKeys.Control)!=0)
        {
            // ZoomArea.OnChangeZoom changes DiagramZoomIndex only for a nonzero delta.
            MoveCanvasZoomByWheel(e.Delta,e.GetPosition(CanvasViewport));
            // ZoomArea.OnPreviewMouseWheel does not consume Ctrl zoom; only
            // OnHoriScroll's Shift branch marks the routed event handled.
        }
    }
    void MoveCanvasZoomByWheel(int delta,Point anchor)
    {
        if(delta!=0)SetCanvasZoom(scale+(delta>0?.1:-.1),anchor);
    }
    void SetCanvasZoom(double value,Point anchor)
    {
        Point world=World(anchor);scale=Math.Clamp(Math.Round(value,1),.1,2);panX=anchor.X-world.X*scale;panY=anchor.Y-world.Y*scale;ApplyCanvasTransform();
    }
    void ApplyCanvasTransform()
    {
        syncingViewport=true;
        GraphSurface.LayoutTransform=new ScaleTransform(scale,scale);
        CanvasScrollViewer.UpdateLayout();
        CanvasScrollViewer.ScrollToHorizontalOffset(Math.Max(0,-panX));CanvasScrollViewer.ScrollToVerticalOffset(Math.Max(0,-panY));
        CanvasScrollViewer.UpdateLayout();
        panX=-CanvasScrollViewer.HorizontalOffset;panY=-CanvasScrollViewer.VerticalOffset;
        ZoomSlider.Value=scale;ZoomValue.Text=$"{scale*100:0}%";syncingViewport=false;
        UpdateMiniMap();
    }
    static DrawingBrush OriginalCanvasGrid()
    {
        // DiagramPage.xaml Control.Background; colorstyle1 Col_EditProcess_DiagramPage_Fill.
        var geometry=new GeometryGroup();geometry.Children.Add(new LineGeometry(new Point(0,10),new Point(20,10)));geometry.Children.Add(new LineGeometry(new Point(10,0),new Point(10,20)));
        return new DrawingBrush {Drawing=new GeometryDrawing(null,new System.Windows.Media.Pen(new SolidColorBrush(Color.FromRgb(64,64,66)),5),geometry),TileMode=TileMode.Tile,ViewportUnits=BrushMappingMode.Absolute,Viewport=new Rect(0,0,6,6)};
    }
    void Fit()
    {
        var nodes=doc.CurrentProcedure.Nodes;if(nodes.Count==0){scale=1;panX=panY=0;ApplyCanvasTransform();return;}
        double left=nodes.Min(n=>n.X)-20,top=nodes.Min(n=>n.Y)-20,right=nodes.Max(n=>n.X+NodeWidth)+20,bottom=nodes.Max(n=>n.Y+NodeHeight)+20;
        scale=Math.Clamp(Math.Floor(Math.Min(CanvasViewport.ActualWidth/(right-left),CanvasViewport.ActualHeight/(bottom-top))*10)/10,.1,2);
        panX=(CanvasViewport.ActualWidth-(right-left)*scale)/2-left*scale;panY=(CanvasViewport.ActualHeight-(bottom-top)*scale)/2-top*scale;ApplyCanvasTransform();
    }

    void UpdateMiniMap()
    {
        if(doc is null||CanvasScrollViewer.ExtentWidth<=0||CanvasScrollViewer.ExtentHeight<=0)return;
        // Direct ZoomArea.SelectRegion* formulas, using ActualSize (not node bounds).
        MiniMapViewport.Width=Math.Min(CanvasScrollViewer.ActualWidth/CanvasScrollViewer.ExtentWidth*187,187);
        MiniMapViewport.Height=Math.Min(CanvasScrollViewer.ActualHeight/CanvasScrollViewer.ExtentHeight*150,150);
        Canvas.SetLeft(MiniMapViewport,CanvasScrollViewer.HorizontalOffset/CanvasScrollViewer.ExtentWidth*187);
        Canvas.SetTop(MiniMapViewport,CanvasScrollViewer.VerticalOffset/CanvasScrollViewer.ExtentHeight*150);
    }
    void NavigateMiniMap(Point regionTopLeft)
    {
        CanvasScrollViewer.ScrollToHorizontalOffset(regionTopLeft.X/187*CanvasScrollViewer.ExtentWidth);
        CanvasScrollViewer.ScrollToVerticalOffset(regionTopLeft.Y/150*CanvasScrollViewer.ExtentHeight);
        CanvasScrollViewer.UpdateLayout();panX=-CanvasScrollViewer.HorizontalOffset;panY=-CanvasScrollViewer.VerticalOffset;UpdateMiniMap();
    }
    void MiniMapDown(object sender,MouseButtonEventArgs e)
    {
        BeginMiniMapDrag(e.GetPosition(MiniMapCanvas));e.Handled=true;
    }
    void BeginMiniMapDrag(Point point)
    {
        var rect=new Rect(Canvas.GetLeft(MiniMapViewport),Canvas.GetTop(MiniMapViewport),MiniMapViewport.Width,MiniMapViewport.Height);
        eyeDragging=true;
        // ZoomArea.GridEagleEye_MouseDown records the requested rect.Location, even
        // when ScrollViewer clamps the viewport at the canvas boundary.
        var requested=rect.Contains(point)?rect.Location:new Point(point.X-rect.Width/2,point.Y-rect.Height/2);
        if(!rect.Contains(point))NavigateMiniMap(requested);
        eyePointerStart=point;eyeRegionStart=requested;
    }
    void MiniMapMove(object sender,MouseEventArgs e){if(!eyeDragging||e.LeftButton!=MouseButtonState.Pressed)return;var point=e.GetPosition(MiniMapCanvas);if(point==eyePointerStart)return;if(!MiniMapCanvas.IsMouseCaptured)MiniMapCanvas.CaptureMouse();MoveMiniMapDrag(point);e.Handled=true;}
    void MoveMiniMapDrag(Point point){if(eyeDragging)NavigateMiniMap(eyeRegionStart+(point-eyePointerStart));}
    void MiniMapUp(object sender,MouseButtonEventArgs e){eyeDragging=false;MiniMapCanvas.ReleaseMouseCapture();UpdateMiniMap();e.Handled=true;}
    void MiniMapWheel(object sender,MouseWheelEventArgs e)
    {
        // ZoomArea.GridEagleEye_PreviewMouseWheel uses the same Shift policy as the main canvas,
        // before its Ctrl zoom branch (both branches run for Ctrl+Shift).
        if((Keyboard.Modifiers&ModifierKeys.Shift)!=0){if(e.Delta<0)CanvasScrollViewer.LineRight();else if(e.Delta>0)CanvasScrollViewer.LineLeft();e.Handled=true;}
        if((Keyboard.Modifiers&ModifierKeys.Control)!=0){var point=e.GetPosition(MiniMapCanvas);var world=new Point(point.X/187*DiagramPageSize,point.Y/150*DiagramPageSize);SetCanvasZoom(scale+(e.Delta>0?.1:e.Delta<0?-.1:0),new Point(world.X*scale+panX,world.Y*scale+panY));}
    }
    void MiniMapToggleClick(object sender,RoutedEventArgs e){eyeExpanded=!eyeExpanded;MiniMapPanel.Visibility=eyeExpanded?Visibility.Visible:Visibility.Collapsed;SetEyeIcon(false);}
    void SetEyeIcon(bool hover)=>((Image)MiniMapToggle.Content).Source=Asset((eyeExpanded?"ic_retract_":"ic_expand_")+(hover?"click.png":"normal.png"));
}
