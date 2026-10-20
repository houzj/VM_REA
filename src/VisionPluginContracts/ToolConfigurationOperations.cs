using System.Globalization;
using System.Text.Json;
using VisionFramework;

namespace VisionPluginContracts;

/// <summary>Typed, explicit dispatch table. XML strings cannot invoke CLR methods.</summary>
public sealed class ToolConfigurationOperationRouter : IToolConfigurationDispatcher {
 readonly Dictionary<ConfigurationOperationKind,Func<ConfigurationOperationRequest,ConfigurationOperationResult>> handlers=[];
 public void Register(ConfigurationOperationKind kind,Func<ConfigurationOperationRequest,ConfigurationOperationResult> handler){
  if(kind==ConfigurationOperationKind.Unknown||!Enum.IsDefined(kind))throw new ArgumentException("Cannot register unknown XML operations.",nameof(kind));
  ArgumentNullException.ThrowIfNull(handler);if(!handlers.TryAdd(kind,handler))throw new InvalidOperationException("Duplicate configuration operation: "+kind);
 }
 public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest request)=>handlers.TryGetValue(request.Kind,out var handle)?handle(request):new(false);
}

/// <summary>Host-neutral adapter to execution parameters and a separate configuration
/// user-data store. Neither the model nor renderer invents storage from a control type.</summary>
public sealed class ToolConfigurationParameterAdapter : IToolConfigurationDispatcher {
 readonly ModuleDescriptor descriptor;
 readonly Func<IReadOnlyDictionary<string,JsonElement>> read;
 readonly Action<Dictionary<string,JsonElement>> apply;
 readonly Func<string,string?>? readUser;
 readonly Action<string,string>? writeUser;
 public ToolConfigurationParameterAdapter(ModuleDescriptor descriptor,Func<IReadOnlyDictionary<string,JsonElement>> read,Action<Dictionary<string,JsonElement>> apply,Func<string,string?>? readUser=null,Action<string,string>? writeUser=null){
  this.descriptor=descriptor;this.read=read;this.apply=apply;this.readUser=readUser;this.writeUser=writeUser;
 }
 public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest request){
  string name=(request.Source.Name??"")+(request.Source.Get("ComTailName")??"");
  string userKey=request.Parameters.FirstOrDefault()??("Param."+name);
  try{
   if(request.Kind==ConfigurationOperationKind.ReadUserValue)return readUser is null?new(false):new(true,readUser(userKey));
   if(request.Kind==ConfigurationOperationKind.PersistUserValue){if(writeUser is null)return new(false);writeUser(userKey,request.Value??"");return new(true,request.Value);}
   if(request.Kind is not (ConfigurationOperationKind.ReadParameter or ConfigurationOperationKind.WriteParameter))return new(false);
   var spec=descriptor.Parameters.SingleOrDefault(p=>p.Name==name);if(spec is null)return new(false);
   if(request.Kind==ConfigurationOperationKind.ReadParameter){var values=read();var value=values.TryGetValue(name,out var current)?current:spec.DefaultValue;return value is {} found?new(true,Text(found)):new(true,Error:"Missing parameter: "+name);}
   return WriteParameter(name,request.Value??"");
  }catch(Exception error)when(error is ArgumentException or InvalidOperationException or System.IO.IOException or UnauthorizedAccessException or OverflowException){return new(true,Error:error.Message);}
 }
 // Backend Algorithm.xml defaults can include parameters with no editor node.
 public ConfigurationOperationResult WriteParameter(string name,string value){
  try{
   var spec=descriptor.Parameters.SingleOrDefault(p=>p.Name==name);if(spec is null)return new(false);
   var values=read();
   if(!TryValue(spec.Kind,value,out var parsed)||!GraphValidator.ValidParameter(spec,parsed))return new(true,Error:"Parameter is outside its declared type/range: "+name);
   var updated=values.ToDictionary(p=>p.Key,p=>p.Value.Clone());updated[name]=parsed;apply(updated);
   var after=read();
   if(!after.TryGetValue(name,out var applied)||!GraphValidator.ValidParameter(spec,applied))return new(true,Error:"Parameter readback is missing or invalid: "+name);
   bool persisted=spec.Kind switch{
    ParameterKind.Text=>applied.GetString()==parsed.GetString(),
    ParameterKind.Boolean=>applied.GetBoolean()==parsed.GetBoolean(),
    ParameterKind.Integer=>applied.GetInt32()==parsed.GetInt32(),
    _=>applied.GetDouble()==parsed.GetDouble()};
   return persisted?new(true,Text(applied)):new(true,Error:"Host apply did not persist the requested parameter: "+name);
  }catch(Exception error)when(error is ArgumentException or InvalidOperationException or System.IO.IOException or UnauthorizedAccessException or OverflowException){return new(true,Error:error.Message);}
 }
 static string Text(JsonElement value)=>value.ValueKind==JsonValueKind.String?value.GetString()??"":value.ToString();
 static bool TryValue(ParameterKind kind,string value,out JsonElement result){
  result=default;
  switch(kind){
   case ParameterKind.Text:result=JsonSerializer.SerializeToElement(value);return true;
   case ParameterKind.Integer:if(int.TryParse(value,NumberStyles.Integer,CultureInfo.InvariantCulture,out var integer)){result=JsonSerializer.SerializeToElement(integer);return true;}return false;
   case ParameterKind.Number:if(double.TryParse(value,NumberStyles.Float,CultureInfo.InvariantCulture,out var number)&&double.IsFinite(number)){result=JsonSerializer.SerializeToElement(number);return true;}return false;
   case ParameterKind.Boolean:if(bool.TryParse(value,out var boolean)){result=JsonSerializer.SerializeToElement(boolean);return true;}if(value is "0" or "1"){result=JsonSerializer.SerializeToElement(value=="1");return true;}return false;
   default:return false;
  }
 }
}
