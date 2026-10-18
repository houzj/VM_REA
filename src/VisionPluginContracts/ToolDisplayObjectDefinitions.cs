using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using VisionFramework;
namespace VisionPluginContracts;

public sealed record ToolDisplayFeature(string Name,string Mapping,string Value,bool ArrayMatch);
public sealed record ToolDisplayObjectDefinition(string Name,string Type,RoiDisplayStyle DefaultStyle,IReadOnlyDictionary<string,ToolDisplayFeature> Features){
 public bool StrokeThicknessConfigEnabled=>Type is "circle" or "point" or "rect" or "line" or "LineAngle" or "coordinate" or "polygon" or "annularsector" or "ellipse" or "polyline" or "point3d" or "line3d" or "PointXYZ";
 public static IReadOnlyList<ToolDisplayObjectDefinition> Parse(string xml){
  using var reader=XmlReader.Create(new StringReader(xml),new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null});var root=XDocument.Load(reader).Root??throw new InvalidDataException("Missing display root.");
  var result=new List<ToolDisplayObjectDefinition>();
  foreach(var node in root.Descendants("Object")){
   string A(string key,string fallback="")=>(string?)node.Attribute(key)??fallback;
   double Number(string key,double fallback)=>double.TryParse(A(key),NumberStyles.Float,CultureInfo.InvariantCulture,out var n)&&double.IsFinite(n)?n:fallback;
   var style=new RoiDisplayStyle(!bool.TryParse(A("IsDisplay"),out var visible)||visible,A("okcolor","#66ff00"),A("ngcolor","#ff0000"),Number("opacity",1),Number("linewidth",1));style.Validate();
   var features=node.Element("Features")?.Elements("Feature").Select(f=>new ToolDisplayFeature((string?)f.Attribute("Name")??throw new InvalidDataException("Unnamed display feature."),(string?)f.Attribute("Mapping")??"NULL",(string?)f.Attribute("Value")??"",(string?)f.Attribute("ArrayMatch")=="true")).ToDictionary(f=>f.Name,StringComparer.Ordinal)??[];
   var name=A("Name");if(name.Length==0)throw new InvalidDataException("Invalid display object identity.");
   result.Add(new(name,A("Type"),style,new ReadOnlyDictionary<string,ToolDisplayFeature>(features)));
  }
  return result.AsReadOnly();
 }
}
public sealed record ToolDisplayObjectState(ToolDisplayObjectDefinition Definition,RoiDisplayStyle Style);
public sealed record ToolDisplayObjectEditResult(bool Accepted,ToolDisplayObjectState? State,string? Error=null);
/// <summary>Managed presentation storage only. It does not emulate the native object ABI or write algorithm CurValue.</summary>
public sealed class ToolDisplayObjectStore {
 public const string ParameterName="DisplayObjectSettings";
 readonly IReadOnlyList<ToolDisplayObjectDefinition> definitions;
 readonly Func<IReadOnlyDictionary<string,JsonElement>> read;
 readonly Action<Dictionary<string,JsonElement>> apply;
 public ToolDisplayObjectStore(IReadOnlyList<ToolDisplayObjectDefinition> definitions,Func<IReadOnlyDictionary<string,JsonElement>> read,Action<Dictionary<string,JsonElement>> apply){this.definitions=definitions;this.read=read;this.apply=apply;}
 public ToolDisplayObjectState? Read(string name){var matches=definitions.Where(d=>d.Name==name).ToArray();if(matches.Length!=1)return null;var definition=matches[0];return new(definition,ReadStyles(read(),definitions)[name]);}
 public ToolDisplayObjectEditResult Write(string name,RoiDisplayStyle style){
  var old=Read(name);if(old is null)return new(false,null,"Unknown source display object.");
  try{style.Validate();var values=read();var styles=ReadStyles(values,definitions).ToDictionary(p=>p.Key,p=>p.Value);styles[name]=style;var next=values.ToDictionary(p=>p.Key,p=>p.Value.Clone());next[ParameterName]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(styles));apply(next);var accepted=Read(name);return accepted?.Style==style?new(true,accepted):new(false,accepted,"Display object storage rejected the value.");}
  catch(Exception error){return new(false,Read(name),error.Message);}
 }
 public static IReadOnlyDictionary<string,RoiDisplayStyle> ReadStyles(IReadOnlyDictionary<string,JsonElement> values,IReadOnlyList<ToolDisplayObjectDefinition> definitions){
  // Original Display.xml may contain repeated Data Record names. Keep definition order;
  // only uniquely addressed objects participate in a name-based configuration bridge.
  var result=definitions.GroupBy(d=>d.Name,StringComparer.Ordinal).Where(g=>g.Count()==1).ToDictionary(g=>g.Key,g=>g.First().DefaultStyle,StringComparer.Ordinal);
  if(values.TryGetValue(ParameterName,out var json)&&json.ValueKind==JsonValueKind.String){
   try{var saved=JsonSerializer.Deserialize<Dictionary<string,RoiDisplayStyle>>(json.GetString()??"{}");if(saved is not null)foreach(var pair in saved){if(!result.ContainsKey(pair.Key)||pair.Value is null)continue;pair.Value.Validate();result[pair.Key]=pair.Value;}}
   catch(Exception error)when(error is JsonException or ArgumentException){}
  }
  return new ReadOnlyDictionary<string,RoiDisplayStyle>(result);
 }
}
