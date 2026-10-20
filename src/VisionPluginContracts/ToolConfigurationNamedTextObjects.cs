using System.Globalization;

namespace VisionPluginContracts;

/// <summary>Additive named-text feature bridge. The existing Show Text service remains
/// authoritative for all of its routes and its DisplaySettings snapshot.</summary>
public sealed class ToolDisplayNamedTextConfigurationObjectService(ToolDisplayFeatureStore features,IToolConfigurationObjectService legacy):IToolConfigurationObjectService
{
 readonly Dictionary<ConfigurationObjectRoute,IToolConfigurationObjectBinding> bindings=[];
 public IToolConfigurationObjectBinding? Resolve(ConfigurationObjectRoute route){
  var original=legacy.Resolve(route);if(original is not null)return original;
  if(route.ObjectName=="Show Text"||route.Property!=ConfigurationObjectProperty.FontSize||route.FeatureName!="FontSize")return null;
  var definition=features.Definition(route.ObjectName);
  if(definition?.Type!="text"||!definition.Features.TryGetValue("FontSize",out var font)||font.Mapping is not ("NULL" or ""))return null;
  if(!bindings.TryGetValue(route,out var binding))bindings[route]=binding=new Binding(features,route.ObjectName);
  return binding;
 }
 sealed class Binding(ToolDisplayFeatureStore features,string objectName):IToolConfigurationObjectBinding{
  public string Read()=>features.Read(objectName,"FontSize")??"";
  public ConfigurationObjectEditResult Write(string value){
   if(!double.TryParse(value,NumberStyles.Float,CultureInfo.InvariantCulture,out var number)||!double.IsFinite(number)||number<=0)return new(false,Read(),"Invalid finite font size.");
   var result=features.Write(objectName,"FontSize",value);return new(result.Accepted,result.Value,result.Error);
  }
 }
}
