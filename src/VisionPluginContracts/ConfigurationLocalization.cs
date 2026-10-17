using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace VisionPluginContracts;

/// <summary>Source-backed captions, separate from configuration identity and binding references.</summary>
public sealed class ConfigurationLocalization {
 static readonly Lazy<IReadOnlyDictionary<string,ConfigurationLocalization>> builtIn=new(Load);
 readonly IReadOnlyDictionary<string,string> strings;
 ConfigurationLocalization(Dictionary<string,string> strings)=>this.strings=new ReadOnlyDictionary<string,string>(strings);
 public static ConfigurationLocalization ForLanguage(string language)=>builtIn.Value[language=="English"?"English":"Chinese"];
 public bool TryGet(string key,out string value)=>strings.TryGetValue(key,out value!);
 // An explicit independent fallback, not a claimed native runtime observation.
 public string Get(string key)=>strings.GetValueOrDefault(key,key);
 static IReadOnlyDictionary<string,ConfigurationLocalization> Load(){
  using var input=typeof(ConfigurationLocalization).Assembly.GetManifestResourceStream("VisionPluginContracts.Resources.ConfigurationLocalization.json")??throw new InvalidDataException("Missing configuration localization asset.");
  using var data=JsonDocument.Parse(input);
  return new ReadOnlyDictionary<string,ConfigurationLocalization>(data.RootElement.GetProperty("languages").EnumerateObject().ToDictionary(language=>language.Name,language=>new ConfigurationLocalization(language.Value.EnumerateObject().ToDictionary(value=>value.Name,value=>value.Value.GetString()!,StringComparer.Ordinal)),StringComparer.Ordinal));
 }
}
