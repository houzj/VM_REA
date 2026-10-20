using VisionFramework;
using VisionPluginContracts;
using VisionCore;
namespace VisionPlugins.Calibration;
public sealed partial class NPointPlugin{
 static ToolNumericInputReadResult ReadCoordinate(ModuleExecutionContext c,string name,int index){
  ToolNumericInputReadResult result;
  if(c.Scope?.Service<IToolNumericInputReader>() is {} reader)result=reader.Read(c,name,index);
  else if(c.ReadNumericInput?.Invoke(name,index) is {} read)result=new(read.ErrorCode,read.Count,read.Value);
  else if(c.Inputs.TryGetValue(name,out var value)){
   var items=value.Kind==DataKind.NumberArray?value.As<IReadOnlyList<double>>():value.Kind==DataKind.Number?value.Elements().Select(v=>v.As<double>()).ToArray():Array.Empty<double>();
   result=items.Count==0?new(0xe0000314,0,0):index<0||index>=items.Count?new(0xe0000014,0,0):new(0,items.Count,items[index]);
  }else{
   // Independent compatibility for old direct POINT contexts; normal UI aliases
   // transport the authoritative leaves, which always take precedence above.
   string? alias=name is "ImagePointX" or "ImagePointY"?"Image Point":name is "PhysicalPointX" or "PhysicalPointY"?"Physical Point":null;
   if(alias is not null&&c.Inputs.TryGetValue(alias,out var point)&&point.Kind==DataKind.Point){var points=point.Elements().Select(p=>p.As<Point2>()).ToArray();result=points.Length==0?new(0xe0000314,0,0):index<0||index>=points.Length?new(0xe0000014,0,0):new(0,points.Length,name.EndsWith('X')?points[index].X:points[index].Y);}
   else result=new(0xe000031c,0,0);
  }
  return result.ErrorCode==0?result:new(result.ErrorCode,0,0);
 }
 static (uint Error,float[] Values) ReadCoordinatePrefix(ModuleExecutionContext c,string name,int count){
  var first=ReadCoordinate(c,name,0);var values=new float[count];
  if(first.ErrorCode!=0)return(first.ErrorCode,values);
  if(first.Count<count)return(0xe0000001,values);
  if(count==1){values[0]=(float)first.Value;return(0,values);}
  // Managed batch transport over an immutable graph snapshot. The per-index
  // reader is not a second native BatchGetFloat ABI or atomic-write promise.
  for(int i=0;i<count;i++){var item=ReadCoordinate(c,name,i);if(item.ErrorCode!=0)return(item.ErrorCode,values);values[i]=(float)item.Value;}
  return(0,values);
 }
 static uint ReadCaptureInputs(ModuleExecutionContext c,int rotation,out float[] ix,out float[] iy,out float[] wx,out float[] wy,out float[] ia,out float[] wa){
  ix=iy=wx=wy=ia=wa=[];
  var imageX=ReadCoordinate(c,"ImagePointX",0);var imageY=ReadCoordinate(c,"ImagePointY",0);
  var worldX=ReadCoordinate(c,"PhysicalPointX",0);var worldY=ReadCoordinate(c,"PhysicalPointY",0);
  var worldAngle=rotation>0?ReadCoordinate(c,"WorldRotateAngle",0):default;
  var imageAngle=rotation>0?ReadCoordinate(c,"ImageRotateAngle",0):default;
  if(imageX.Count!=imageY.Count||worldX.Count!=worldY.Count)return 0xe0000001;
  int Cap(int count)=>Math.Clamp(count,0,32);
  var x=ReadCoordinatePrefix(c,"ImagePointX",Cap(imageX.Count));if(x.Error!=0)return x.Error;ix=x.Values;
  var y=ReadCoordinatePrefix(c,"ImagePointY",Cap(imageY.Count));if(y.Error!=0)return y.Error;iy=y.Values;
  wx=ReadCoordinatePrefix(c,"PhysicalPointX",Cap(worldX.Count)).Values;
  wy=ReadCoordinatePrefix(c,"PhysicalPointY",Cap(worldY.Count)).Values;
  wa=ReadCoordinatePrefix(c,"WorldRotateAngle",Cap(worldAngle.Count)).Values;
  ia=ReadCoordinatePrefix(c,"ImageRotateAngle",Cap(imageAngle.Count)).Values;
  return 0;
 }
 static uint ReadTeachingInputs(ModuleExecutionContext c,State state){
  state.SnapX=-99999;state.SnapY=state.SnapAngle=0;
  var x=ReadCoordinate(c,"PhysicalPointX",0);if(x.ErrorCode!=0)return x.ErrorCode;if(x.Count>0)state.TeachX=(float)x.Value;
  var y=ReadCoordinate(c,"PhysicalPointY",0);if(y.ErrorCode!=0)return y.ErrorCode;if(y.Count>0)state.TeachY=(float)y.Value;
  var angle=ReadCoordinate(c,"WorldRotateAngle",0);if(angle.ErrorCode!=0)return angle.ErrorCode;if(angle.Count>0)state.TeachAngle=(float)angle.Value;
  var imageX=ReadCoordinate(c,"ImagePointX",0);if(imageX.ErrorCode==0&&imageX.Count>0)state.SnapX=(float)imageX.Value;
  var imageY=ReadCoordinate(c,"ImagePointY",0);if(imageY.ErrorCode==0&&imageY.Count>0)state.SnapY=(float)imageY.Value;
  var imageAngle=ReadCoordinate(c,"ImageRotateAngle",0);if(imageAngle.ErrorCode==0&&imageAngle.Count>0)state.SnapAngle=(float)imageAngle.Value;
  state.HasTeach=true;return 0;
 }
}
