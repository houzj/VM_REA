using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

namespace VisionPluginContracts;

public sealed record ToolDisplayFeatureEditResult(bool Accepted,string Value,string? Error=null);

/// <summary>Source Feature.ValueA user-data persistence. The original module key is
/// preserved; storage is managed presentation metadata, not algorithm CurValue or native ABI.</summary>
public sealed class ToolDisplayFeatureStore
{
 public const string ParameterName="ConfigurationUserData";
 readonly IReadOnlyList<ToolDisplayObjectDefinition> definitions;
 readonly Func<IReadOnlyDictionary<string,JsonElement>> read;
 readonly Action<Dictionary<string,JsonElement>> apply;
 public ToolDisplayFeatureStore(IReadOnlyList<ToolDisplayObjectDefinition> definitions,Func<IReadOnlyDictionary<string,JsonElement>> read,Action<Dictionary<string,JsonElement>> apply){this.definitions=definitions;this.read=read;this.apply=apply;}
 public static string Key(string objectName,string featureName)=>"[ParamRoot]_"+objectName+"."+featureName+".Value";
 public ToolDisplayObjectDefinition? Definition(string name){var matches=definitions.Where(d=>d.Name==name).ToArray();return matches.Length==1?matches[0]:null;}
 public string? Read(string objectName,string featureName)=>ReadValue(read(),definitions,objectName,featureName);
 public static string? ReadValue(IReadOnlyDictionary<string,JsonElement> parameters,IReadOnlyList<ToolDisplayObjectDefinition> definitions,string objectName,string featureName){
  var matches=definitions.Where(d=>d.Name==objectName).ToArray();if(matches.Length!=1||!matches[0].Features.TryGetValue(featureName,out var feature))return null;
  var data=ReadUserData(parameters);return data.TryGetValue(Key(objectName,featureName),out var saved)&&saved.ValueKind==JsonValueKind.String?saved.GetString():feature.Value;
 }
 public ToolDisplayFeatureEditResult Write(string objectName,string featureName,string value){
  var previous=Read(objectName,featureName);if(previous is null)return new(false,"","Unknown or ambiguous source display feature.");
  if(Encoding.UTF8.GetByteCount(value)>512)return new(false,previous,"Display feature content exceeds the source 512-byte limit.");
  try{
   var parameters=read();
   if(parameters.TryGetValue(ParameterName,out var container)){
    if(container.ValueKind!=JsonValueKind.String)return new(false,previous,"Display user-data container is not a string.");
    using var parsed=JsonDocument.Parse(container.GetString()??"{}");if(parsed.RootElement.ValueKind!=JsonValueKind.Object)return new(false,previous,"Display user-data container is not an object.");
   }
   var data=ReadUserData(parameters).ToDictionary(p=>p.Key,p=>p.Value.Clone(),StringComparer.Ordinal);data[Key(objectName,featureName)]=JsonSerializer.SerializeToElement(value);
   var next=parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());next[ParameterName]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(data));apply(next);
   var accepted=Read(objectName,featureName)??"";return accepted==value?new(true,accepted):new(false,accepted,"Display feature storage rejected the value.");
  }catch(Exception error){return new(false,Read(objectName,featureName)??previous,error.Message);}
 }
 static IReadOnlyDictionary<string,JsonElement> ReadUserData(IReadOnlyDictionary<string,JsonElement> parameters){
  if(parameters.TryGetValue(ParameterName,out var json)&&json.ValueKind==JsonValueKind.String){try{var data=JsonSerializer.Deserialize<Dictionary<string,JsonElement>>(json.GetString()??"{}");if(data is not null)return new ReadOnlyDictionary<string,JsonElement>(data);}catch(JsonException){}}
  return new ReadOnlyDictionary<string,JsonElement>(new Dictionary<string,JsonElement>());
 }
}
