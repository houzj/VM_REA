using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using VisionFramework;
using VisionCore;
namespace VisionPluginContracts;

public sealed record ToolDisplayPoint(double X,double Y,string Tips,int Index=0);
public sealed record ToolDisplayGraphic(string Name,string Type,string Color,double Opacity,double StrokeThickness,IReadOnlyList<ToolDisplayPoint> Points){
 public double Width{get;init;}public double Height{get;init;}public double Angle{get;init;}
 public Point2 XVector{get;init;}public Point2 YVector{get;init;}
}
/// <summary>Source Display.xml scalar/array projection. This is not the native dynamic display object ABI.</summary>
public static class ToolDisplayGraphics {
 public static IReadOnlyList<ToolDisplayGraphic> Build(IReadOnlyList<ToolDisplayObjectDefinition> definitions,IReadOnlyDictionary<string,JsonElement> parameters,IReadOnlyDictionary<string,PortValue> inputs,IReadOnlyDictionary<string,PortValue> outputs){
  var styles=ToolDisplayObjectStore.ReadStyles(parameters,definitions);var result=new List<ToolDisplayGraphic>();
  foreach(var definition in definitions){
   if(definition.Type is not ("point" or "path" or "rect" or "coordinate"))continue;var style=styles.GetValueOrDefault(definition.Name)??definition.DefaultStyle;if(!style.Visible)continue;
   ToolDisplayFeature? Feature(string key)=>definition.Features.GetValueOrDefault(key);
   PortValue? Value(string name)=>outputs.GetValueOrDefault(name)??inputs.GetValueOrDefault(name);
   double[]? Values(string key){var f=Feature(key);if(f is null)return null;if(f.Mapping is "NULL" or "")return double.TryParse(f.Value,NumberStyles.Float,CultureInfo.InvariantCulture,out var v)?[v]:null;var value=Value(f.Mapping);return value?.Kind switch{DataKind.Number=>[value.As<double>()],DataKind.Integer=>[value.As<long>()],DataKind.NumberArray=>value.As<IReadOnlyList<double>>().ToArray(),_=>null};}
   double Scalar(string key,double fallback=0)=>Values(key) is {Length:>0} array?array[0]:fallback;
   var x=Values("CenterX");var y=Values("CenterY");if(x is null||y is null)continue;
   // Object.GetValueList derives shape count from a mapped feature's value count.
   // Num's scalar value is not that count (some shipped XML even maps it to "1").
   int count=Math.Min(100000,Math.Min(x.Length,y.Length));
   // Source path renderer returns no geometry unless GetArrowPath has at least two points.
   if(definition.Type=="path"&&count<2)continue;
   var points=new List<ToolDisplayPoint>();
   string Tips(int index){var feature=Feature("Tips");if(feature is null)return "";var mappings=feature.Mapping.Split(',');return Regex.Replace(feature.Value,@"\{(\d+)\}",m=>{int slot=int.Parse(m.Groups[1].Value,CultureInfo.InvariantCulture);if(slot>=mappings.Length)return "";var port=Value(mappings[slot]);double? number=port?.Kind switch{DataKind.Number=>port.As<double>(),DataKind.Integer=>port.As<long>(),DataKind.NumberArray when index<port.As<IReadOnlyList<double>>().Count=>port.As<IReadOnlyList<double>>()[index],_=>null};return number is null?"":((float)number.Value).ToString("G7",CultureInfo.CurrentCulture);});}
   for(int i=0;i<count;i++)if(double.IsFinite(x[i])&&double.IsFinite(y[i]))points.Add(new(x[i],y[i],Tips(i)));
   if(points.Count==0)continue;
   result.Add(new(definition.Name,definition.Type,Scalar("Status")==0?style.NgColor:style.OkColor,style.Opacity,style.StrokeThickness,points.AsReadOnly()){
    Width=Scalar("Width"),Height=Scalar("Height"),Angle=Scalar("Angle"),XVector=new(Scalar("CoorDXVecX"),Scalar("CoorDXVecY")),YVector=new(Scalar("CoorDYVecX"),Scalar("CoorDYVecY"))});
  }
  return result.AsReadOnly();
 }
}
