using System.Text.Json;
using System.IO;
namespace VisionPlugins.ImageSource;

public sealed record LightUiField(string Name,string Kind,string Label,bool Visible,int Minimum,int Maximum);
public static class LightUiRules {
 static readonly JsonDocument definitions=JsonDocument.Parse(typeof(LightUiRules).Assembly.GetManifestResourceStream("VisionPlugins.ImageSource.Resources.LightDefinition.json")!);
 public static IReadOnlyList<LightUiField> Fields(int deviceType){var fields=definitions.RootElement.GetProperty("items").EnumerateArray().Select(item=>new LightUiField(item.GetProperty("name").GetString()!,item.GetProperty("kind").GetString()!,item.GetProperty("label").GetString()!,item.GetProperty("visible").GetBoolean(),int.TryParse(item.GetProperty("minimum").GetString(),out int min)?min:0,int.TryParse(item.GetProperty("maximum").GetString(),out int max)?max:0)).ToDictionary(f=>f.Name);
  if(definitions.RootElement.GetProperty("rules").TryGetProperty(LightPlugin.NormalizeDeviceType(deviceType).ToString(System.Globalization.CultureInfo.InvariantCulture),out var rules))foreach(var rule in rules.EnumerateArray()){string target=rule.GetProperty("target").GetString()!,operation=rule.GetProperty("operation").GetString()!;var current=fields[target];fields[target]=operation switch{"VisibleOperation"=>current with{Visible=true},"HiddenOperation"=>current with{Visible=false},"SetBrightnessRangeOperation"=>current with{Maximum=rule.GetProperty("argument").GetString()=="1"?100:255},_=>throw new InvalidDataException("Unknown light UI operation: "+operation)};}
  return fields.Values.Where(f=>f.Name!="LightDeviceParamGet").ToArray();
 }
}
