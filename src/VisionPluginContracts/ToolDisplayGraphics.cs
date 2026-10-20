using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using VisionFramework;
using VisionCore;
namespace VisionPluginContracts;

public sealed record ToolDisplayPoint(double X,double Y,string Tips,int Index=0){public string? Text{get;init;}public string? Color{get;init;}}
public sealed record ToolDisplayGraphic(string Name,string Type,string Color,double Opacity,double StrokeThickness,IReadOnlyList<ToolDisplayPoint> Points){
 public double Width{get;init;}public double Height{get;init;}public double Angle{get;init;}
 public Point2 XVector{get;init;}public Point2 YVector{get;init;}
 public int FontSize{get;init;}=10;
}
/// <summary>Source Display.xml scalar/array projection. This is not the native dynamic display object ABI.</summary>
public static class ToolDisplayGraphics {
 /// <summary>Explicit opt-in for source named text objects. Existing Build and the
 /// legacy Show Text overlay are unchanged. Unequal/nonfinite position arrays are bounded safely.</summary>
 public static IReadOnlyList<ToolDisplayGraphic> BuildNamedTexts(IReadOnlyList<ToolDisplayObjectDefinition> definitions,IReadOnlyDictionary<string,JsonElement> parameters,IReadOnlyDictionary<string,PortValue> inputs,IReadOnlyDictionary<string,PortValue> outputs){
  var result=new List<ToolDisplayGraphic>();var styles=ToolDisplayObjectStore.ReadStyles(parameters,definitions);
  foreach(var definition in definitions.Where(d=>d.Type=="text"&&d.Name!="Show Text")){
   var style=styles.GetValueOrDefault(definition.Name)??definition.DefaultStyle;if(!style.Visible)continue;
   PortValue? Port(string name)=>outputs.GetValueOrDefault(name)??inputs.GetValueOrDefault(name);
   string FeatureValue(string key)=>ToolDisplayFeatureStore.ReadValue(parameters,definitions,definition.Name,key)??definition.Features.GetValueOrDefault(key)?.Value??"";
   double[]? Values(string key){var feature=definition.Features.GetValueOrDefault(key);if(feature is null)return null;if(feature.Mapping is "NULL" or "")return double.TryParse(FeatureValue(key),NumberStyles.Float,CultureInfo.CurrentCulture,out var n)?[n]:null;var value=Port(feature.Mapping);return value?.Kind switch{DataKind.Number=>[value.As<double>()],DataKind.Integer=>[value.As<long>()],DataKind.NumberArray=>value.As<IReadOnlyList<double>>().ToArray(),_=>null};}
   var x=Values("PositionX");var y=Values("PositionY");if(x is null||y is null)continue;
   var status=Values("Status");var content=definition.Features.GetValueOrDefault("Content");if(content is null)continue;
   // ObjectBase.ConvertValue requests int via Convert.ToInt32(string), with zero
   // fallback on invalid/fractional strings. Preserve that payload instead of rounding.
   int.TryParse(FeatureValue("FontSize"),NumberStyles.Integer,CultureInfo.CurrentCulture,out int fontSize);
   var mappings=content.Mapping.Split(',');int count=Math.Min(100000,Math.Min(x.Length,y.Length));
   for(int i=0;i<count;i++){
    if(!double.IsFinite(x[i])||!double.IsFinite(y[i]))continue;
    string text=content.Mapping is "NULL" or ""?FeatureValue("Content"):Regex.Replace(FeatureValue("Content"),@"\{(\d+)\}",match=>{int slot=int.Parse(match.Groups[1].Value,CultureInfo.InvariantCulture);if(slot>=mappings.Length)return "";var port=Port(mappings[slot]);double? number=port?.Kind switch{DataKind.Number=>port.As<double>(),DataKind.Integer=>port.As<long>(),DataKind.NumberArray when i<port.As<IReadOnlyList<double>>().Count=>port.As<IReadOnlyList<double>>()[i],_=>null};return number is null?"":((float)number.Value).ToString("G7",CultureInfo.CurrentCulture);});
    double state=status is null||status.Length==0?0:status.Length==1?status[0]:i<status.Length?status[i]:0;bool ng=state==0;
    result.Add(new(definition.Name,"text",ng?style.NgColor:style.OkColor,style.Opacity,style.StrokeThickness,[new(x[i],y[i],"",i){Text=text}]){FontSize=fontSize});
   }
  }
  return result.AsReadOnly();
 }
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
   var statuses=definition.Type=="point"?Values("Status"):null;
   // Object.SetBaseProperty consumes Status at each shape index. Both cached
   // Object and direct VmIO reads use element zero for a nonempty short array.
   string PointColor(int index){double status=statuses is {Length:>0}?statuses[index<statuses.Length?index:0]:0;return status==0?style.NgColor:style.OkColor;}
   for(int i=0;i<count;i++)if(double.IsFinite(x[i])&&double.IsFinite(y[i]))points.Add(new(x[i],y[i],Tips(i),i){Color=definition.Type=="point"?PointColor(i):null});
   if(points.Count==0)continue;
   result.Add(new(definition.Name,definition.Type,Scalar("Status")==0?style.NgColor:style.OkColor,style.Opacity,style.StrokeThickness,points.AsReadOnly()){
    Width=Scalar("Width"),Height=Scalar("Height"),Angle=Scalar("Angle"),XVector=new(Scalar("CoorDXVecX"),Scalar("CoorDXVecY")),YVector=new(Scalar("CoorDYVecX"),Scalar("CoorDYVecY"))});
  }
  return result.AsReadOnly();
 }
}
