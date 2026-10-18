using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VisionFramework;
using VisionEditor.Core;
using VisionEditor.Core.VmRouting;
using Point=System.Windows.Point;
using Rectangle=System.Windows.Shapes.Rectangle;
using Panel=System.Windows.Controls.Panel;
using Brushes=System.Windows.Media.Brushes;
using Color=System.Windows.Media.Color;
using Brush=System.Windows.Media.Brush;
namespace VisionEditor;
public partial class MainWindow
{
 readonly Dictionary<string,(ExecutionLinkDocument Link,System.Windows.Shapes.Path View,IReadOnlyList<RoutePoint> Points)> wireViews=new();
 readonly Dictionary<string,System.Windows.Shapes.Polygon> wireArrows=new();
 readonly Dictionary<string,System.Windows.Shapes.Path> wireHitPaths=new();
 readonly List<Rectangle> wireHandles=new();
 RecoveredConnectionDrag? wireDrag;
 ExecutionLinkDocument? draggedWire,insertWire;
 Point[]? wireDragOriginalPoints;
 long wireDragRevision;
 static readonly Brush wireNormalBrush=new SolidColorBrush(Color.FromRgb(255,167,15)),wireSelectedBrush=new SolidColorBrush(Color.FromRgb(231,105,0));
 Brush WireStroke(ExecutionLinkDocument link)=>selectedWire==link.FromNode+">"+link.ToNode||(!boxSelectionActive&&selectedNodes.Count==1&&(selectedNodes.Contains(link.FromNode)||selectedNodes.Contains(link.ToNode)))?wireSelectedBrush:wireNormalBrush;
 static PointCollection OriginalWireArrow(IReadOnlyList<RoutePoint> points)
 {
  var end=new Point(points[^1].X,points[^1].Y);var prior=points.Take(points.Count-1).LastOrDefault(p=>p!=points[^1]);var vector=new System.Windows.Vector(prior.X-end.X,prior.Y-end.Y);if(vector.Length==0)return new();vector.Normalize();
  double angle=Math.Atan2(8,5)*180/Math.PI,length=Math.Sqrt(89);var center=end+6*vector;var matrix=new Matrix();matrix.Rotate(90);var first=center+5*vector*matrix;matrix.Rotate(180-angle);var second=first+length*vector*matrix;matrix.Rotate(2*angle);return new(){first,second,second+length*vector*matrix};
 }
 static IReadOnlyList<RoutePoint> CleanWirePoints(IReadOnlyList<RoutePoint> points)
 {
  // Connection.GetCleanLinePoint reconstructs orthogonal corners from paired chamfer points.
  if(points.Count<2)return points;var clean=new List<RoutePoint>{points[0]};var last=points[0];
  for(int i=1;i+1<points.Count;i+=2){var a=points[i];var b=points[i+1];if(a==b){clean.Add(a);clean.Add(b);continue;}last=last.X==a.X?new(last.X,b.Y):new(b.X,last.Y);clean.Add(last);}clean.Add(points[^1]);return clean;
 }
 void DrawWireHandles()
 {
  foreach(var handle in wireHandles)GraphLayer.Children.Remove(handle);wireHandles.Clear();
  if(!EditableCanvas||selectedWire is null||!wireViews.TryGetValue(selectedWire,out var entry))return;
  // Connection.OnIsSelectedChanged uses IsFocused; LostFocus clears MidPoints.
  if(wireDrag is null&&(!wireHitPaths.TryGetValue(selectedWire,out var hit)||!hit.IsKeyboardFocused))return;
  var points=wireDrag?.PathPoints.Select(p=>new RoutePoint(p.X,p.Y)).ToArray()??entry.Points;
  for(int i=0;i<points.Count-1;i++)
  {
   var a=points[i];var b=points[i+1];if(Math.Sqrt(Math.Pow(a.X-b.X,2)+Math.Pow(a.Y-b.Y,2))<=5)continue;
   bool horizontal=a.Y==b.Y;int index=i;
   var handle=new Rectangle {Width=6,Height=6,Fill=new SolidColorBrush(Color.FromRgb(231,105,0)),Cursor=horizontal?System.Windows.Input.Cursors.SizeNS:System.Windows.Input.Cursors.SizeWE,Tag="wire-midpoint"};
   Canvas.SetLeft(handle,(a.X+b.X)/2-3);Canvas.SetTop(handle,(a.Y+b.Y)/2-3);Panel.SetZIndex(handle,70);
   handle.MouseLeftButtonDown+=(_,e)=>{BeginWireDrag(entry.Link,index);e.Handled=true;};wireHandles.Add(handle);GraphLayer.Children.Add(handle);
  }
 }
 void BeginWireDrag(ExecutionLinkDocument link,int index)
 {
  if(!EditableCanvas||!wireViews.TryGetValue(link.FromNode+">"+link.ToNode,out var view)||index<0||index>=view.Points.Count-1)return;
  var nodes=doc.CurrentProcedure.Nodes;var source=nodes.Single(n=>n.Id==link.FromNode);var target=nodes.Single(n=>n.Id==link.ToNode);
  selectedWire=link.FromNode+">"+link.ToNode;selected=null;selectedNodes.Clear();boxSelectionActive=false;draggedWire=link;
  RefreshNodeSelection();
  canvasGestureRevision=doc.Revision;wireDragRevision=doc.Revision;wireDragOriginalPoints=view.Points.Select(p=>new Point(p.X,p.Y)).ToArray();wireDrag=new(wireDragOriginalPoints,index,NodeBounds(source),NodeBounds(target));GraphLayer.Focus();CanvasViewport.CaptureMouse();
 }
 void PreviewWireDrag(Point point)
 {
  if(wireDrag is null||draggedWire is null)return;
  Safe(()=>wireDrag.Move(point));
  var key=draggedWire.FromNode+">"+draggedWire.ToNode;
  var entry=wireViews[key];var displayed=VmOrthogonalRouter.Passivate(wireDrag.PathPoints.Select(p=>new RoutePoint(p.X,p.Y)).ToArray());
  entry.View.Data=LineGeometry(displayed);
  // Native Connection.SetPath redraws its polyline and arrow from the same PathPoints change.
  wireArrows[key].Points=OriginalWireArrow(displayed);
  wireHitPaths[key].Data=entry.View.Data;
  DrawWireHandles();
 }
 void FinishWireDrag(bool commit)
 {
  var link=draggedWire;bool changed=wireDrag is not null&&wireDragOriginalPoints is not null&&!wireDrag.PathPoints.SequenceEqual(wireDragOriginalPoints);var points=wireDrag?.PathPoints.Select(p=>new WirePointDocument(p.X,p.Y)).ToArray();
  wireDrag=null;draggedWire=null;wireDragOriginalPoints=null;CanvasViewport.ReleaseMouseCapture();
  if(commit&&changed&&EditableCanvas&&link is not null&&points is not null)Safe(()=>doc.SetExecutionPath(link,points));RenderGraph();
 }
 ExecutionLinkDocument? FindInsertionWire(Point point,IEnumerable<string> excluded)
 {
  var ids=excluded.ToHashSet();
  if(doc.CurrentProcedure.Nodes.Any(n=>!ids.Contains(n.Id)&&NodeBounds(n).Contains(point)))return null;
  // A real WPF geometry hit test on the same path used for display, including source chamfers.
  foreach(var entry in wireViews.Values.Reverse())if(!ids.Contains(entry.Link.FromNode)&&!ids.Contains(entry.Link.ToNode)&&entry.View.Data.StrokeContains(new System.Windows.Media.Pen(Brushes.Black,10),point))return entry.Link;
  return null;
 }
 void UpdateInsertionWire(Point point,IEnumerable<string> excluded)
 {
  insertWire=FindInsertionWire(point,excluded);
  foreach(var entry in wireViews.Values){bool active=insertWire is {} hit&&hit.FromNode==entry.Link.FromNode&&hit.ToNode==entry.Link.ToNode;entry.View.StrokeThickness=active?10:2;entry.View.Stroke=active?wireNormalBrush:WireStroke(entry.Link);}
 }
 void ClearInsertionWire(){insertWire=null;foreach(var entry in wireViews.Values){entry.View.StrokeThickness=2;entry.View.Stroke=WireStroke(entry.Link);}}
 void DropToolModule(string id,double x,double y,ExecutionLinkDocument? insertion,string? predecessor)
 {
  if(!EditableCanvas)return;var descriptor=modules.Descriptors.First(d=>d.Id==id);
  string added=id==WorkflowCalls.ModuleId?doc.AddOwnedGroup(x,y,Defaults(descriptor),insertion is null?predecessor:null,insertion):doc.AddNode(id,descriptor.Version,x,y,Defaults(descriptor),insertion is null?predecessor:null,insertion);
  selected=added;selectedNodes.Clear();selectedNodes.Add(added);boxSelectionActive=false;selectedWire=null;RenderGraph();RefreshRight();
 }
}
