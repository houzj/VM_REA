using VisionFramework;
using VisionPluginContracts;
using VisionCore;
namespace VisionPlugins.Calibration;
internal static class RotateCalibrationInputReader {
 internal static ToolNumericInputReadResult Float(ModuleExecutionContext c,string name){
  if(c.Scope?.Service<IToolNumericInputReader>() is {} reader)return reader.Read(c,name,0);
  if(c.ReadNumericInput?.Invoke(name,0) is {} source)return new(source.ErrorCode,source.Count,source.Value);
  if(c.Inputs.TryGetValue(name,out var input)){
   var values=input.Kind==DataKind.NumberArray?input.As<IReadOnlyList<double>>():input.Kind==DataKind.Number?input.Elements().Select(v=>v.As<double>()).ToArray():Array.Empty<double>();
   return values.Count>0?new(0,values.Count,values[0]):new(0xe0000314,0,0);
  }
  if(name is "ImagePointX" or "ImagePointY" && c.Inputs.TryGetValue("Image Point",out var points)){
   var values=points.Elements().Select(p=>p.As<Point2>()).ToArray();if(values.Length>0)return new(0,values.Length,name.EndsWith('X')?values[0].X:values[0].Y);
  }
  return new(0xe0000314,0,0);
 }
 internal static ToolIntegerInputReadResult Integer(ModuleExecutionContext c,string name){
  if(c.Scope?.Service<IToolIntegerInputReader>() is {} reader)return reader.Read(c,name,0);
  if(c.ReadIntegerInput?.Invoke(name,0) is {} source)return new(source.ErrorCode,source.Count,source.Value);
  if(c.Inputs.TryGetValue(name,out var input)&&input.Kind==DataKind.Integer){var values=input.Elements().ToArray();if(values.Length>0)return new(0,values.Length,unchecked((int)values[0].As<long>()));}
  return new(0xe0000314,0,0);
 }
}
