using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Point=System.Windows.Point;
using FlowDirection=System.Windows.FlowDirection;
using FontFamily=System.Windows.Media.FontFamily;
using Brushes=System.Windows.Media.Brushes;
namespace VisionPluginContracts;

public static class ToolDisplayGraphicGeometry {
 // Image coordinates, with point markers kept at source 3 * thickness screen pixels.
 public static Geometry Create(ToolDisplayGraphic graphic,double imageWidth,double screenScale=1,bool includeMarkers=true){
  var geometry=new GeometryGroup();double scale=double.IsFinite(screenScale)&&screenScale>0?screenScale:1;
  void Line(Point a,Point b)=>geometry.Children.Add(new LineGeometry(a,b));
  void Arrow(Point a,Point b){Line(a,b);double angle=Math.Atan2(a.Y-b.Y,a.X-b.X);foreach(double delta in new[]{Math.PI/4,-Math.PI/4})Line(b,new(b.X+10*Math.Cos(angle+delta),b.Y+10*Math.Sin(angle+delta)));}
  if(graphic.Type is "point" or "path"){
   if(graphic.Type=="path")for(int i=1;i<graphic.Points.Count;i++){var a=graphic.Points[i-1];var b=graphic.Points[i];const float fraction=.8f;var start=new Point(a.X+(1f-fraction)*(b.X-a.X),a.Y+(1f-fraction)*(b.Y-a.Y));var end=new Point(a.X+fraction*(b.X-a.X),a.Y+fraction*(b.Y-a.Y));Arrow(start,end);}
   if(includeMarkers)foreach(var point in graphic.Points){double arm=3*graphic.StrokeThickness/scale;Line(new(point.X-arm,point.Y),new(point.X+arm,point.Y));Line(new(point.X,point.Y-arm),new(point.X,point.Y+arm));}
  }else if(graphic.Type=="rect"){
   foreach(var point in graphic.Points)if(graphic.Width>0&&graphic.Height>0&&double.IsFinite(graphic.Width)&&double.IsFinite(graphic.Height)&&double.IsFinite(graphic.Angle))geometry.Children.Add(new RectangleGeometry(new Rect(point.X-graphic.Width/2,point.Y-graphic.Height/2,graphic.Width,graphic.Height)){Transform=new RotateTransform(graphic.Angle,point.X,point.Y)});
  }else if(graphic.Type=="coordinate"){
   var x=new Vector(graphic.XVector.X,graphic.XVector.Y);var y=new Vector(graphic.YVector.X,graphic.YVector.Y);
   if(x.Length>0&&y.Length>0){x.Normalize();y.Normalize();foreach(var point in graphic.Points){var center=new Point(point.X,point.Y);var px=center+x*(.05*imageWidth);var py=center+y*(.05*imageWidth);Arrow(center,px);Arrow(center,py);
    foreach(var axis in new[]{("X",px),("Y",py)}){var text=new FormattedText(axis.Item1,CultureInfo.GetCultureInfo("en-us"),FlowDirection.LeftToRight,new Typeface(new FontFamily("Times New Roman"),FontStyles.Normal,FontWeights.Bold,FontStretches.Normal),12,Brushes.Red,1);geometry.Children.Add(text.BuildGeometry(axis.Item2));}
   }}
  }
  geometry.Freeze();return geometry;
 }
}
