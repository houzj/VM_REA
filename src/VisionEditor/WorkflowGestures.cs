using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using VisionEditor.Core;
using VisionFramework;
using Point=System.Windows.Point;
using Rectangle=System.Windows.Shapes.Rectangle;
using MenuItem=System.Windows.Controls.MenuItem;
using ContextMenu=System.Windows.Controls.ContextMenu;
using Brushes=System.Windows.Media.Brushes;
using Color=System.Windows.Media.Color;
using Panel=System.Windows.Controls.Panel;
namespace VisionEditor;
public partial class MainWindow
{
 bool boxSelectionActive,nodeCopyDrag;
 Rectangle? groupSelectionFrame,groupMoveFrame;
 readonly List<Line> alignmentGuides=new();
 Rectangle? nodeHotRegion;
 Point? nodeHotPoint;
 string? nodeHotPredecessor;
 double MovingNodeHeight=>toolDragGhost?.Height??(movingNodes?.Count==1?NodeVisualHeight(doc.CurrentProcedure.Nodes.Single(n=>n.Id==movingNodes.Keys.Single())):NodeHeight);
 void ClearNodeHotRegion(){if(nodeHotRegion is not null)GraphLayer.Children.Remove(nodeHotRegion);nodeHotRegion=null;nodeHotPoint=null;nodeHotPredecessor=null;}
 void UpdateNodeHotRegion(double x,double y,IEnumerable<string> excluded)
 {
  var ids=excluded.ToHashSet();
  var nearest=doc.CurrentProcedure.Nodes.Where(n=>!ids.Contains(n.Id)&&n.X==x&&n.Y<y).OrderBy(n=>y-n.Y).FirstOrDefault();
  // DiagramItem.GetSpanByDiagramItem: ordinary module spacing is ModuleSpanCount (40) + 30.
  bool visible=false;
  if(nearest is not null&&nearest.Y+70-NodeVisualHeight(nearest)<y&&y<nearest.Y+70+NodeVisualHeight(nearest)){nodeHotPoint=new Point(nearest.X,nearest.Y+70-MovingNodeHeight/2+NodeVisualHeight(nearest)/2);nodeHotPredecessor=nearest.Id;visible=true;}
  else if(nodeHotPoint is {} retained&&retained.X-20<x&&x<retained.X+20&&retained.Y-NodeHeight<y&&y<retained.Y+NodeHeight)visible=true;
  if(nodeHotRegion is null){nodeHotRegion=new Rectangle {Width=66,Height=22,Fill=Brushes.Orange,IsHitTestVisible=false};Panel.SetZIndex(nodeHotRegion,56);GraphLayer.Children.Add(nodeHotRegion);}
  nodeHotRegion.Visibility=visible?Visibility.Visible:Visibility.Collapsed;
  if(nodeHotPoint is {} point){Canvas.SetLeft(nodeHotRegion,point.X+NodeWidth/2-33);Canvas.SetTop(nodeHotRegion,point.Y+NodeHeight/2-11);}
 }
 Point? ActiveNodeHotPoint=>nodeHotRegion?.Visibility==Visibility.Visible?nodeHotPoint:null;
 string? AvailableHotPredecessor()=>ActiveNodeHotPoint is not null&&nodeHotPredecessor is {} id&&!ProcedureGraph.ExecutionEdges(doc.CurrentProcedure).Any(e=>(e.FromNode==id&&e.SourceIndex==3)||(e.ToNode==id&&e.TargetIndex==3))?id:null;
 readonly Dictionary<(string Node,int Port),Border> executionPorts=new();
 (string Node,int Port)? connectionTarget;
 static GraphFragment? graphClipboard;
 static string? graphClipboardSolution;
 bool EditableCanvas=>!windowClosed&&!parameterEditorOpen&&!isLocked&&activeRun is null&&!workflowOverview&&doc.HasCurrentProcedure;
 Rect SelectedBounds()
 {
  var nodes=doc.CurrentProcedure.Nodes.Where(n=>selectedNodes.Contains(n.Id)).ToArray();
  if(nodes.Length==0)return Rect.Empty;
  var area=new Rect(new Point(nodes.Min(n=>n.X),nodes.Min(n=>n.Y)),new Point(nodes.Max(n=>n.X+NodeWidth),nodes.Max(n=>n.Y+NodeVisualHeight(n))));area.Inflate(10,10);return area;
 }
 Rectangle GroupRectangle(Rect area,bool interactive)
 {
  var result=new Rectangle {Width=area.Width,Height=area.Height,RadiusX=3,RadiusY=3,Stroke=new SolidColorBrush(Color.FromArgb(127,218,120,15)),StrokeThickness=2,StrokeDashArray=new DoubleCollection {5,2},Fill=new LinearGradientBrush(Color.FromArgb(8,198,87,12),Color.FromArgb(8,253,129,18),90),IsHitTestVisible=interactive,Cursor=System.Windows.Input.Cursors.SizeAll};
  Canvas.SetLeft(result,area.X);Canvas.SetTop(result,area.Y);Panel.SetZIndex(result,50);
  return result;
 }
 void DrawSelectionGroup()
 {
  if(groupSelectionFrame is not null)GraphLayer.Children.Remove(groupSelectionFrame);groupSelectionFrame=null;
  if(!boxSelectionActive||selectedNodes.Count==0||!EditableCanvas)return;
  var area=SelectedBounds();if(area.IsEmpty)return;
  groupSelectionFrame=GroupRectangle(area,true);
  groupSelectionFrame.MouseLeftButtonDown+=(_,e)=>{GraphLayer.Focus();BeginNodeMove(e.GetPosition(GraphLayer));e.Handled=true;};
  groupSelectionFrame.ContextMenu=GroupSelectionMenu();
  GraphLayer.Children.Add(groupSelectionFrame);
 }
 void SetBoxSelection(Rect rect)
 {
  selectedNodes.Clear();foreach(var n in doc.CurrentProcedure.Nodes)if(rect.Contains(NodeBounds(n)))selectedNodes.Add(n.Id);
  boxSelectionActive=selectedNodes.Count>0;selected=selectedNodes.FirstOrDefault();selectedWire=null;RenderGraph();DrawSelectionGroup();RefreshRight();
 }
 void CopySelection()
 {
  if(!EditableCanvas||selectedNodes.Count==0)return;
  graphClipboard=doc.CopyNodes(selectedNodes);graphClipboardSolution=doc.Document.Id;
 }
 void PasteSelection(Point? position)
 {
  if(!EditableCanvas||graphClipboard is not {Nodes.Count:>0} snapshot||graphClipboardSolution!=doc.Document.Id)return;
  var first=new Point(snapshot.Nodes.Min(n=>n.X),snapshot.Nodes.Min(n=>n.Y));
  var target=position??new Point(Math.Min(first.X+100,4850),Math.Min(first.Y+100,4850));
  SelectPasted(doc.PasteNodes(snapshot,target.X,target.Y));
 }
 void SelectPasted(IReadOnlyList<string> ids)
 {
  selectedNodes.Clear();foreach(var id in ids)selectedNodes.Add(id);selected=ids.FirstOrDefault();boxSelectionActive=ids.Count>1;RenderGraph();RefreshRight();
 }
 ContextMenu CanvasSelectionMenu(Point location)
 {
  var menu=new ContextMenu();var paste=new MenuItem {Header="粘贴",InputGestureText="Ctrl+V",IsEnabled=EditableCanvas&&graphClipboard is not null&&graphClipboardSolution==doc.Document.Id};paste.Click+=(_,_)=>Safe(()=>PasteSelection(location));menu.Items.Add(paste);return menu;
 }
 ContextMenu GroupSelectionMenu()
 {
  var menu=new ContextMenu();
  void Add(string label,Action action){var item=new MenuItem {Header=label,IsEnabled=EditableCanvas};item.Click+=(_,_)=>Safe(action);menu.Items.Add(item);}
  Add("删除",DeleteSelection);Add("复制",CopySelection);Add("禁用",()=>SetSelectionEnabled(false));Add("启用",()=>SetSelectionEnabled(true));
  var align=new MenuItem {Header="对齐"};foreach(var entry in new[]{("左对齐","left"),("水平居中","centerX"),("右对齐","right"),("顶部对齐","top"),("垂直居中","centerY"),("底部对齐","bottom")}){var item=new MenuItem {Header=entry.Item1};item.Click+=(_,_)=>AlignSelection(entry.Item2);align.Items.Add(item);}menu.Items.Add(align);
  var layout=new MenuItem {Header="分布"};foreach(var horizontal in new[]{true,false}){var item=new MenuItem {Header=horizontal?"水平分布":"垂直分布"};item.Click+=(_,_)=>DistributeSelection(horizontal);layout.Items.Add(item);}menu.Items.Add(layout);return menu;
 }
 void SetSelectionEnabled(bool enabled){if(EditableCanvas)doc.SetNodesEnabled(selectedNodes,enabled);}
 void AlignSelection(string direction)
 {
  if(!EditableCanvas||selectedNodes.Count==0)return;var area=SelectedBounds();
  var positions=doc.CurrentProcedure.Nodes.Where(n=>selectedNodes.Contains(n.Id)).ToDictionary(n=>n.Id,n=>direction switch {
   "left"=>(area.Left+10,n.Y),"right"=>(area.Right-NodeWidth-10,n.Y),"centerX"=>(area.Left+Snap((area.Width-NodeWidth)/2),n.Y),
   "top"=>(n.X,area.Top+10),"bottom"=>(n.X,area.Bottom-NodeVisualHeight(n)-10),"centerY"=>(n.X,Snap(area.Top+area.Height/2-NodeVisualHeight(n)/2)),_=>(n.X,n.Y)});
  doc.MoveNodes(positions);
 }
 void DistributeSelection(bool horizontal)
 {
  if(!EditableCanvas||selectedNodes.Count<2)return;var area=SelectedBounds();var nodes=doc.CurrentProcedure.Nodes.Where(n=>selectedNodes.Contains(n.Id)).OrderBy(n=>horizontal?n.X:n.Y).ToArray();
  // Native DiagramItem alignment and ItemsPage selection bounds use each item's ActualHeight.
  // A Group is taller than an ordinary tool, so vertical spacing cannot use a fixed module height.
  double required=nodes.Sum(n=>(horizontal?NodeWidth:NodeVisualHeight(n))+5)+15,extent=Math.Max(horizontal?area.Width:area.Height,required),gap=Math.Max(0,(extent-required)/(nodes.Length-1));
  double cursor=Math.Max(Snap((horizontal?area.Left:area.Top)+10),10);
  var positions=new Dictionary<string,(double X,double Y)>();foreach(var n in nodes){positions[n.Id]=horizontal?(cursor,Math.Max(Snap(area.Top+area.Height/2-NodeVisualHeight(n)/2),5)):(Math.Max(Snap(area.Left+area.Width/2-NodeWidth/2),5),cursor);cursor+=Snap((horizontal?NodeWidth:NodeVisualHeight(n))+5+gap);}doc.MoveNodes(positions);
 }
 bool IsCanvasBackground(DependencyObject? hit)
 {
  for(var p=hit;p is not null;p=p is Visual||p is System.Windows.Media.Media3D.Visual3D?VisualTreeHelper.GetParent(p):LogicalTreeHelper.GetParent(p))
  {
   if(p==GraphLayer||p==CanvasViewport)return true;
   if(p is Canvas {Tag:string}||p is Border {Tag:ValueTuple<string,int>}||p==groupSelectionFrame||p is System.Windows.Shapes.Path||p is Rectangle {Tag:"wire-midpoint"})return false;
  }
  return false;
 }
 void BeginNodeGesture(Point point,bool copy)
 {
  if(!EditableCanvas)return;
  canvasGestureRevision=doc.Revision;
  ClearNodeHotRegion();
  movingNodes=doc.CurrentProcedure.Nodes.Where(n=>selectedNodes.Contains(n.Id)).ToDictionary(n=>n.Id,n=>new Point(n.X,n.Y));nodePointerStart=point;nodeCopyDrag=copy;CanvasViewport.CaptureMouse();
 }
 bool IsCanvasDropPoint(Point point)=>new Rect(0,0,Math.Max(0,CanvasScrollViewer.ViewportWidth),Math.Max(0,CanvasScrollViewer.ViewportHeight)).Contains(point);
 void CompleteNodeDrop(Point viewportPoint){if(IsCanvasDropPoint(viewportPoint))CommitNodeMove();else CancelCanvasGesture();}
 void ClearAlignmentGuides(){foreach(var line in alignmentGuides)GraphLayer.Children.Remove(line);alignmentGuides.Clear();}
 void DrawAlignmentGuides(double x,double y)
 {
  ClearAlignmentGuides();if(movingNodes?.Count!=1&&toolDragGhost is null)return;
  var others=doc.CurrentProcedure.Nodes.Where(n=>movingNodes is null||!movingNodes.ContainsKey(n.Id)).ToArray();double height=MovingNodeHeight;
  foreach(bool vertical in new[]{true,false})foreach(double fraction in new[]{0d,1d,.5})
  {
   double axis=(vertical?x:y)+(vertical?NodeWidth:height)*fraction;
   var neighbor=others.Where(n=>(vertical?n.X+NodeWidth*fraction:n.Y+NodeVisualHeight(n)*fraction)==axis).OrderBy(n=>Math.Abs(vertical?n.Y-y:n.X-x)).FirstOrDefault();if(neighbor is null)continue;
   double start=Math.Min(vertical?y: x,vertical?neighbor.Y:neighbor.X)-5/scale,end=Math.Max(vertical?y+height:x+NodeWidth,vertical?neighbor.Y+NodeVisualHeight(neighbor):neighbor.X+NodeWidth)+5/scale;
   var line=new Line {X1=vertical?axis:start,Y1=vertical?start:axis,X2=vertical?axis:end,Y2=vertical?end:axis,Stroke=Brushes.LightGreen,StrokeThickness=1/scale,StrokeDashArray=new DoubleCollection {5,2.5},StrokeLineJoin=PenLineJoin.Round,IsHitTestVisible=false};Panel.SetZIndex(line,55);alignmentGuides.Add(line);GraphLayer.Children.Add(line);
  }
 }
 (string Node,int Port)? FindConnectionTarget(Point point)
 {
  if(pendingConnection is not {} source)return null;
  foreach(var node in doc.CurrentProcedure.Nodes.Reverse())
  {
   if(node.Id==source.Node)continue;var bounds=NodeBounds(node);bounds.Inflate(10,10);if(!bounds.Contains(point)&&!Enumerable.Range(0,4).Any(i=>PortHitRect(node,i).Contains(point)))continue;
   int index=Enumerable.Range(0,4).OrderBy(i=>{var p=Port(new(node.X,node.Y,NodeWidth,NodeVisualHeight(node)),i).Anchor;return Math.Pow(p.X-point.X,2)+Math.Pow(p.Y-point.Y,2);}).First();return (node.Id,index);
  }
  return null;
 }
 void BeginConnection(string nodeId,int port)
 {
  if(!EditableCanvas||port is <0 or >3||!doc.CurrentProcedure.Nodes.Any(n=>n.Id==nodeId))return;
  canvasGestureRevision=doc.Revision;selectedWire=null;pendingConnection=(nodeId,port);connectionTarget=null;GraphLayer.Focus();DrawWireHandles();CanvasViewport.CaptureMouse();
 }
 void SetPortHit(Border port,bool highlighted)
 {
  if(port.Child is not Canvas layer||layer.Children.OfType<Border>().FirstOrDefault() is not {} handle||port.Tag is not ValueTuple<string,int> key)return;
  bool vertical=key.Item2%2==0;
  handle.Width=vertical?(highlighted?8.5:5):(highlighted?12:8);handle.Height=vertical?(highlighted?12:8):(highlighted?8.5:5);
  double thickness=highlighted?2:1;handle.BorderThickness=key.Item2 switch {0=>new Thickness(0,thickness,thickness,thickness),1=>new Thickness(thickness,0,thickness,thickness),2=>new Thickness(thickness,thickness,0,thickness),_=>new Thickness(thickness,thickness,thickness,0)};
  Canvas.SetLeft(handle,key.Item2 switch {0=>20,2=>-handle.Width,_=>(30-handle.Width)/2});Canvas.SetTop(handle,key.Item2 switch {1=>20,3=>-handle.Height,_=>(30-handle.Height)/2});
 }
 Rect PortHitRect(NodeDocument node,int index)=>index switch {0=>new(node.X-20,node.Y+(NodeVisualHeight(node)-30)/2,20,30),2=>new(node.X+NodeWidth,node.Y+(NodeVisualHeight(node)-30)/2,20,30),1=>new(node.X+NodeWidth/2-15,node.Y-20,30,20),_=>new(node.X+NodeWidth/2-15,node.Y+NodeVisualHeight(node),30,20)};
 void UpdateConnectionPreview(Point point)
 {
  if(pendingConnection is not {} source)return;
  connectionTarget=FindConnectionTarget(point);
  foreach(var pair in executionPorts){bool target=connectionTarget is {} hit&&pair.Key.Node==hit.Node;pair.Value.Visibility=pair.Key.Node==source.Node||target?Visibility.Visible:Visibility.Collapsed;SetPortHit(pair.Value,connectionTarget==pair.Key);}
  var node=doc.CurrentProcedure.Nodes.Single(n=>n.Id==source.Node);
  IReadOnlyList<RoutePoint> route;
  if(connectionTarget is {} destination){var target=doc.CurrentProcedure.Nodes.Single(n=>n.Id==destination.Node);var boxes=doc.CurrentProcedure.Nodes.Select(n=>new {n.Id,Box=new RouteRect(n.X,n.Y,NodeWidth,NodeVisualHeight(n))}).ToArray();route=VisionEditor.Core.VmRouting.VmOrthogonalRouter.Route(Port(new(node.X,node.Y,NodeWidth,NodeVisualHeight(node)),source.Port),Port(new(target.X,target.Y,NodeWidth,NodeVisualHeight(target)),destination.Port),boxes.Where(n=>n.Id!=node.Id&&n.Id!=target.Id).Select(n=>n.Box),Math.Max(200,boxes.Max(n=>n.Box.Right)+100),Math.Max(200,boxes.Max(n=>n.Box.Bottom)+100)).Points;}
  else route=VisionEditor.Core.VmRouting.VmOrthogonalRouter.Preview(Port(new(node.X,node.Y,NodeWidth,NodeVisualHeight(node)),source.Port),new(point.X,point.Y));
  if(connectionPreview is not null)GraphLayer.Children.Remove(connectionPreview);
  connectionPreview=new System.Windows.Shapes.Path {Data=LineGeometry(route),Stroke=new SolidColorBrush(Color.FromRgb(255,167,15)),StrokeThickness=2,StrokeDashArray=new DoubleCollection {5,2.5},StrokeLineJoin=PenLineJoin.Round,IsHitTestVisible=false};Panel.SetZIndex(connectionPreview,60);GraphLayer.Children.Add(connectionPreview);
 }
 void FinishConnection(Point point)
 {
  var source=pendingConnection;var target=FindConnectionTarget(point);pendingConnection=null;connectionTarget=null;
  CanvasViewport.ReleaseMouseCapture();
  if(source is {} from&&target is {} to&&EditableCanvas)Safe(()=>doc.ConnectExecution(from.Node,to.Node,from.Port,to.Port));
  RenderGraph();
 }
 void CancelCanvasGesture()
 {
  ResetEditingGesture();if(!windowClosed)RenderGraph();
 }
}

