using VisionCore;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.Fixture;
internal static class FixtureInputReader {
 internal static FixtureRead Read(ModuleExecutionContext context,string name){
  if(context.Scope?.Service<IToolNumericInputReader>() is {} service){var value=service.Read(context,name,0);return new(value.ErrorCode,value.Count,(float)value.Value);}
  if(context.ReadNumericInput?.Invoke(name,0) is {} graph)return new(graph.ErrorCode,graph.Count,(float)graph.Value);
  if(context.Inputs.TryGetValue(name,out var raw)){
   if(raw.Kind==DataKind.NumberArray){var values=raw.As<IReadOnlyList<double>>();return values.Count==0?new(0xe0000314,0,0):new(0,values.Count,(float)values[0]);}
   if(raw.Kind==DataKind.Number){var values=raw.Elements().ToArray();return values.Length==0?new(0xe0000314,0,0):new(0,values.Length,(float)values[0].As<double>());}
   return new(0xe0000314,0,0);
  }
  if((name=="InPointX"||name=="InPointY")&&context.Inputs.TryGetValue("Origin",out var origin)){
   var points=origin.Elements().ToArray();if(origin.Kind!=DataKind.Point||points.Length==0)return new(0xe0000314,0,0);
   var point=points[0].As<Point2>();return new(0,points.Length,(float)(name=="InPointX"?point.X:point.Y));
  }
  return new(FixtureState.Missing,0,0);
 }
}
