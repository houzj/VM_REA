using System.Text;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.Calibration;
public sealed partial class ReadCalibFilePlugin {
 static ToolNumericInputReadResult ReadPoseInput(ModuleExecutionContext context,string name)=>ReadFloatingInput(context,name,0);
 internal static ToolNumericInputReadResult ReadFloatingInput(ModuleExecutionContext context,string name,int index){
  if(context.Scope?.Service<IToolNumericInputReader>() is {} reader)return reader.Read(context,name,index);
  if(context.ReadNumericInput?.Invoke(name,index) is {} source)return new(source.ErrorCode,source.Count,source.Value);
  if(index<0)return new(0xe0000014,0,0);
  if(context.Inputs.TryGetValue(name,out var input)){
   IReadOnlyList<double> values=input.Kind==DataKind.NumberArray?input.As<IReadOnlyList<double>>():input.Kind==DataKind.Number?input.Elements().Select(v=>v.As<double>()).ToArray():Array.Empty<double>();
   if(values.Count>0)return index<values.Count?new(0,values.Count,values[index]):new(0xe0000014,0,0);
  }
  return new(0xe0000314,0,0);
 }
 internal static ToolIntegerInputReadResult ReadIntegerInput(ModuleExecutionContext context,string name,int index){
  if(context.Scope?.Service<IToolIntegerInputReader>() is {} reader)return reader.Read(context,name,index);
  if(context.ReadIntegerInput?.Invoke(name,index) is {} source)return new(source.ErrorCode,source.Count,source.Value);
  if(index<0)return new(0xe0000014,0,0);
  if(context.Inputs.TryGetValue(name,out var input)&&input.Kind==DataKind.Integer){var values=input.Elements().ToArray();if(values.Length>0)return index<values.Length?new(0,values.Length,unchecked((int)values[index].As<long>())):new(0xe0000014,0,0);}
  return new(0xe0000314,0,0);
 }
 internal static ToolTextInputReadResult ReadTextInput(ModuleExecutionContext context,string name,int index,int capacity){
  if(context.Scope?.Service<IToolTextInputReader>() is {} reader)return reader.Read(context,name,index,capacity);
  if(context.ReadTextInput?.Invoke(name,index,capacity) is {} source)return new(source.ErrorCode,source.Count,source.Value,source.RequiredBytes);
  if(index<0)return new(0xe0000014,0,"",0);
  if(!context.Inputs.TryGetValue(name,out var input)||input.Kind!=DataKind.Text)return new(0xe0000314,0,"",0);
  var values=input.Elements().ToArray();if(values.Length==0)return new(0xe0000314,0,"",0);if(index>=values.Length)return new(0xe0000014,0,"",0);
  string text=values[index].As<string>().Split('\0')[0];int required=checked(Encoding.UTF8.GetByteCount(text)+1);
  return capacity<required?new(0xe0000012,0,"",required):new(0,values.Length,text,required);
 }
}
