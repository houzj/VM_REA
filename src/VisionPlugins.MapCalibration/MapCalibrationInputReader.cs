using System.Text;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.MapCalibration;
// Explicit opt-in readers; no primitive input is pre-materialized before its source branch.
internal static class MapCalibrationInputReader {
 internal static ToolNumericInputReadResult Float(ModuleExecutionContext c,string name){
  if(c.Scope?.Service<IToolNumericInputReader>() is {} reader)return reader.Read(c,name,0);
  if(c.ReadNumericInput?.Invoke(name,0) is {} result)return new(result.ErrorCode,result.Count,result.Value);
  if(c.Inputs.TryGetValue(name,out var input)){
   var values=input.Kind==DataKind.NumberArray?input.As<IReadOnlyList<double>>():input.Kind==DataKind.Number?input.Elements().Select(v=>v.As<double>()).ToArray():Array.Empty<double>();
   if(values.Count>0)return new(0,values.Count,values[0]);
  }
  return new(0xe0000314,0,0);
 }
 internal static ToolIntegerInputReadResult Integer(ModuleExecutionContext c,string name){
  if(c.Scope?.Service<IToolIntegerInputReader>() is {} reader)return reader.Read(c,name,0);
  if(c.ReadIntegerInput?.Invoke(name,0) is {} result)return new(result.ErrorCode,result.Count,result.Value);
  if(c.Inputs.TryGetValue(name,out var input)&&input.Kind==DataKind.Integer){var values=input.Elements().ToArray();if(values.Length>0)return new(0,values.Length,unchecked((int)values[0].As<long>()));}
  return new(0xe0000314,0,0);
 }
 internal static ToolTextInputReadResult Text(ModuleExecutionContext c,string name){
  const int capacity=512;
  if(c.Scope?.Service<IToolTextInputReader>() is {} reader)return reader.Read(c,name,0,capacity);
  if(c.ReadTextInput?.Invoke(name,0,capacity) is {} result)return new(result.ErrorCode,result.Count,result.Value,result.RequiredBytes);
  if(c.Inputs.TryGetValue(name,out var input)&&input.Kind==DataKind.Text){var values=input.Elements().ToArray();if(values.Length>0){string value=values[0].As<string>().Split('\0')[0];int bytes=checked(Encoding.UTF8.GetByteCount(value)+1);return bytes<=capacity?new(0,values.Length,value,bytes):new(0xe0000012,0,"",bytes);}}
  return new(0xe0000314,0,"",0);
 }
 // A read without a value leaves the initialized destination untouched. Return status
 // is discarded by the caller; the primitive transport writes a destination only on a successful value read.
 internal static float Slot(ToolNumericInputReadResult read,float initialized)=>read.ErrorCode==0&&read.Count>0?(float)read.Value:initialized;
}
