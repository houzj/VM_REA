using System.IO;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;

namespace VisionPlugins.CameraMapping;

internal static class CameraMappingInputReader {
 // Original module 84b0 probes count/status; 8580 then reads each value.
 // The optional scope reader can preserve changing native read outcomes. The
 // fallback is an independent adapter over already-materialized host values.
 internal static uint Read(ModuleExecutionContext context,ref CameraMappingPair[]? cache,out CameraMappingPair[] points){
  points=[];
  var reader=context.Scope?.Service<IToolNumericInputReader>();
  var materialized=new Dictionary<string,IReadOnlyList<double>>();
  var counts=new int[8];int total=0;bool active=false;
  for(int group=1;group<=8;group++){
   if(!context.Parameters["Data"+group].GetString()!.EndsWith("#1",StringComparison.Ordinal))continue;
   active=true;
   var tx=Value("Target","X",group,0);var ty=Value("Target","Y",group,0);
   if(tx.Count!=ty.Count)return 0xe0000001;
   var ox=Value("Object","X",group,0);var oy=Value("Object","Y",group,0);
   if(ox.Count!=oy.Count||ox.Count!=tx.Count||oy.Count!=ty.Count)return 0xe0000001;
   if(tx.ErrorCode==0&&ty.ErrorCode==0&&ox.ErrorCode==0&&oy.ErrorCode==0){counts[group-1]=tx.Count;total=checked(total+tx.Count);}
  }
  if(!active)return 0xe00000ff;
  if(cache is null||cache.Length<total)cache=new CameraMappingPair[total];
  int offset=0;
  for(int group=1;group<=8;group++)for(int index=0;index<counts[group-1];index++){
   var value=Value("Target","X",group,index);if(value.ErrorCode!=0)return value.ErrorCode;
   cache[offset]=cache[offset] with{TargetX=(float)value.Value};
   value=Value("Target","Y",group,index);if(value.ErrorCode!=0)return value.ErrorCode;
   cache[offset]=cache[offset] with{TargetY=(float)value.Value};
   value=Value("Object","X",group,index);if(value.ErrorCode!=0)return value.ErrorCode;
   cache[offset]=cache[offset] with{ObjectX=(float)value.Value};
   value=Value("Object","Y",group,index);if(value.ErrorCode!=0)return value.ErrorCode;
   cache[offset]=cache[offset] with{ObjectY=(float)value.Value};
   offset++;
  }
  points=cache.Take(total).ToArray();
  return total==0?0xe0000309:0;

  ToolNumericInputReadResult Value(string side,string axis,int group,int index){
   string name=side+"Point"+axis+group;
   if(reader is not null){var read=reader.Read(context,name,index);if(read.Count<0)throw new InvalidDataException("点坐标数量不能为负数。");return read;}
   if(context.ReadNumericInput?.Invoke(name,index) is {} graph){if(graph.Count<0)throw new InvalidDataException("点坐标数量不能为负数。");return new(graph.ErrorCode,graph.Count,graph.Value);}
   if(!materialized.TryGetValue(name,out var values)){
    if(context.Inputs.TryGetValue(name,out var input))values=input.Kind==DataKind.NumberArray?input.As<IReadOnlyList<double>>():input.Kind==DataKind.Number?[input.As<double>()]:throw new InvalidDataException("点坐标类型错误。");
    else if(context.Inputs.TryGetValue(CameraMappingPlugin.PointInputName(side,group),out var combined))values=combined.Elements().Select(p=>axis=="X"?p.As<Point2>().X:p.As<Point2>().Y).ToArray();
    else values=Array.Empty<double>();
    materialized.Add(name,values);
   }
   return index<values.Count?new(0,values.Count,values[index]):new(values.Count==0?0xe0000314u:0xe0000014u,values.Count,0);
  }
 }
}
