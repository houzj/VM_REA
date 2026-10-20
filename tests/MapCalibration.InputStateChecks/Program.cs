using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using VisionCore;
using VisionFramework;
using VisionPlugins.MapCalibration;

try {
 int checks=0;
 void Check(bool condition,string message){if(!condition)throw new Exception(message);checks++;}
 var plugin=new MapCalibrationPlugin();var runtime=plugin.CreateRuntime();
 var values=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());
 var image=new GrayImage(2,2,new byte[]{255,255,255,255});
 string directory=Directory.GetCurrentDirectory();
 object State(string node)=>typeof(MapCalibrationPlugin).GetMethod("CurrentImages",BindingFlags.NonPublic|BindingFlags.Instance)!.Invoke(plugin,new object[]{node,directory,null})!;
 T Read<T>(object state,string field)=>(T)state.GetType().GetField(field,BindingFlags.Public|BindingFlags.Instance)!.GetValue(state)!;
 async Task<ModuleResult> Frame(string node,int side,Dictionary<string,PortValue> optional){
  var inputs=new Dictionary<string,PortValue>(optional){["InputImage"+(side+1)]=PortValue.Image(image)};
  var result=await runtime.ExecuteAsync(new(node,directory,inputs,values),CancellationToken.None);
  Check(result.NativeErrorCode==0&&result.Status==ModuleStatus.Success,"Single received frame waits without running detection");
  Check(result.Outputs["ModuStatus"].As<long>()==1&&result.Outputs["CalibStatus"].As<long>()==0,"Original pending status");
  Check(result.Outputs.Count==3&&result.Outputs.ContainsKey("ModuRunTime"),"Wait outputs statuses and forced timing only");
  return result;
 }
 for(int side=0;side<2;side++){
  string node="pose-side-"+side,field=side==0?"SourcePose":"TargetPose";
  await Frame(node,side,new(){{"PhysicalPointX",PortValue.Number(12.25)},{"PhysicalPointY",PortValue.Number(-4.5)},{"WorldRotateAngle",PortValue.Number(17.125)}});
  var state=State(node);Check(Read<MapCalibrationPose>(state,field)==new MapCalibrationPose(12.25f,-4.5f,17.125f),"Explicit pose is stored on its received side");
  await Frame(node,side,new());
  var absent=Read<MapCalibrationPose>(state,field);
  Check(absent==new MapCalibrationPose(-999999f,-999f,-999f),"Each newly received frame resets all missing pose fields");
  Check(BitConverter.SingleToInt32Bits(absent.Angle)==unchecked((int)0xc479c000),"Original angle sentinel float bits");
  await Frame(node,side,new(){{"PhysicalPointX",PortValue.Number(3)},{"WorldRotateAngle",PortValue.Number(-45.5)}});
  Check(Read<MapCalibrationPose>(state,field)==new MapCalibrationPose(3f,-999f,-45.5f),"Partial pose uses this frame's values and defaults");
  await Frame(node,side,new(){{"PhysicalPointX",PortValue.Number(4)},{"PhysicalPointY",PortValue.Number(5)}});
  var reset=Read<MapCalibrationPose>(state,field);Check(reset==new MapCalibrationPose(4f,5f,-999f),"Omitted angle does not retain preceding explicit angle");
  var noFrame=await runtime.ExecuteAsync(new(node,directory,new Dictionary<string,PortValue>(),values),CancellationToken.None);
  Check(noFrame.NativeErrorCode==0xe0000314&&noFrame.Outputs.Keys.SequenceEqual(new[]{"ModuRunTime"}),"Absent frame exits before status output");
  Check(Read<MapCalibrationPose>(state,field)==reset,"No received frame preserves cached pose");
  Check(Read<bool>(state,side==0?"SourcePending":"TargetPending")&&!Read<bool>(state,side==0?"TargetPending":"SourcePending"),"Each side has an independent pending flag");
  values["InputType"]=JsonSerializer.SerializeToElement(2);
  var fileFailure=await runtime.ExecuteAsync(new(node,directory,new Dictionary<string,PortValue>(),values),CancellationToken.None);
  Check(fileFailure.NativeErrorCode==0x80100006,"File mode without matrices fails independently");
  values["InputType"]=JsonSerializer.SerializeToElement(1);
  Check(Read<MapCalibrationPose>(state,field)==reset&&Read<bool>(state,side==0?"SourcePending":"TargetPending"),"File failure preserves pending image pose");
  Check(plugin.LastDisplay(new(node,MapCalibrationPlugin.Id,"1.0.0",node,values),directory,null) is null,"A waiting session has no synthetic result display");
  var solved=new MapCalibrationImageResult(0,new float[]{1,0,0,0,1,0,0,0,1},1,1,1,1,0,Array.Empty<MapCalibrationImagePoint>(),Array.Empty<MapCalibrationImagePoint>(),Array.Empty<MapCalibrationImagePoint>());
  var xml=XDocument.Parse(MapCalibrationImageFiles.Xml(solved,new DateTime(2026,10,19),reset,new(-999999,-999,-999)));
  Check(xml.Descendants("SnapWorldCurR").Single().Value=="-999","Cached sentinel is serialized instead of the preceding frame angle");
 }
 Console.WriteLine($"PASS {checks}: Map input pose/pending/file-mode wait states; no detector execution or native replay");return 0;
}catch(Exception error){Console.Error.WriteLine(error);return 1;}
