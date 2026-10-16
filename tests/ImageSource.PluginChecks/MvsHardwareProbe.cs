using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using VisionFramework;
using VisionPlugins.ImageSource;

static class MvsHardwareProbe {
 public static void Restore(string managed,string native,float exposure,float gain){using var service=new MvsCameraAcquisition(managed,native);if(service.Cameras.Count!=1)throw new InvalidOperationException("Exactly one camera required");var flags=BindingFlags.Instance|BindingFlags.NonPublic;object handle=typeof(MvsCameraAcquisition).GetMethod("Open",flags)!.Invoke(service,[service.Cameras[0].Id])!;foreach(var pair in new[]{("ExposureTime",exposure),("Gain",gain)}){int code=(int)typeof(MvsCameraAcquisition).GetMethod("Invoke",flags)!.Invoke(service,[handle,"MV_CC_SetFloatValue_NET",new object[]{pair.Item1,pair.Item2}])!;if(code!=0)throw new Exception("Restore failed: "+code.ToString("x8"));}Console.WriteLine("Restored recorded baseline exposure="+exposure+", gain="+gain);}
 public static async Task Run(string managed,string native){
  string output=Path.GetFullPath("analysis/full-audit/image-source-camera-sdk/run-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);var records=new List<object>();
  using var service=new MvsCameraAcquisition(managed,native);
  try{
   records.Add(new{phase="artifacts",files=new[]{managed,native,typeof(MvsCameraAcquisition).Assembly.Location,typeof(MvsHardwareProbe).Assembly.Location}.Select(path=>new{path,sha256=Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant()}).ToArray()});
   records.Add(new{phase="enumeration",service.Cameras,service.SdkVersion});
   if(service.Cameras.Count!=1)throw new InvalidOperationException("Hardware probe requires exactly one selected camera.");int id=service.Cameras[0].Id;
   object Private(string name,params object?[] arguments)=>typeof(MvsCameraAcquisition).GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(service,arguments)!;
   object handle=Private("Open",id);
   object Query(string key,string structure,string method){object?[] arguments=[key,Private("Create",structure)];int result=(int)Private("Invoke",handle,method,arguments);if(result!=0)throw new InvalidOperationException(key+" read failed: "+result.ToString("x8"));return arguments[1]!;}
   uint EnumValue(string key){var value=Query(key,"MVCC_ENUMVALUE","MV_CC_GetEnumValue_NET");return Convert.ToUInt32(value.GetType().GetField("nCurValue")!.GetValue(value));}
   float FloatValue(string key){var value=Query(key,"MVCC_FLOATVALUE","MV_CC_GetFloatValue_NET");return (float)value.GetType().GetField("fCurValue")!.GetValue(value)!;}
   records.Add(new{phase="settings",triggerMode=EnumValue("TriggerMode"),triggerSource=EnumValue("TriggerSource"),exposure=FloatValue("ExposureTime"),gain=FloatValue("Gain")});
   for(int i=0;i<3;i++){
    var frame=await service.AcquireAsync(new(id,null,null,0),CancellationToken.None);byte[] pixels=frame.Image.Pixels.ToArray();File.WriteAllBytes(Path.Combine(output,"frame-"+i+".pixels"),pixels);
    records.Add(new{phase="frame-"+i,frame.Image.Width,frame.Image.Height,frame.Image.Stride,format=frame.Image.Format.ToString(),frame.FrameNum,frame.LostFrameCount,frame.LostPacketCount,pixelSha256=Convert.ToHexString(SHA256.HashData(pixels)).ToLowerInvariant()});
   }
   float initialExposure=FloatValue("ExposureTime"),initialGain=FloatValue("Gain");
   try{int index=0;foreach(var values in new[]{new[]{1000.1234,1.2345},new[]{5000.5678,3.4567},new[]{-1d,-1d}}){var frame=await service.AcquireAsync(new(id,values[0],values[1],10),CancellationToken.None);records.Add(new{phase="parameter-readback-"+index++,requestedExposure=values[0],requestedGain=values[1],exposure=FloatValue("ExposureTime"),gain=FloatValue("Gain"),frame.FrameNum,frame.LostFrameCount,frame.LostPacketCount});}}
   finally{int exposureResult=(int)Private("Invoke",handle,"MV_CC_SetFloatValue_NET",new object?[]{"ExposureTime",initialExposure});int gainResult=(int)Private("Invoke",handle,"MV_CC_SetFloatValue_NET",new object?[]{"Gain",initialGain});records.Add(new{phase="restored",exposureResult,gainResult,exposure=FloatValue("ExposureTime"),gain=FloatValue("Gain")});}
   using var scope=new ModuleExecutionScope();scope.RegisterService<IImageAcquisitionService>(service,owned:false);var plugin=new ImageSourcePlugin();var parameters=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());parameters["ImageSourceType"]=JsonSerializer.SerializeToElement(2);parameters["CameraID"]=JsonSerializer.SerializeToElement(id);parameters["CameraIdentity"]=JsonSerializer.SerializeToElement(service.Cameras.Single().PersistentId);parameters["InitialSN"]=JsonSerializer.SerializeToElement(7);parameters["OutMono8"]=JsonSerializer.SerializeToElement(true);
   async Task<ModuleResult> Execute(string phase){var result=await plugin.CreateRuntime().ExecuteAsync(new ModuleExecutionContext("hardware",Environment.CurrentDirectory,new Dictionary<string,PortValue>(),parameters){Scope=scope},CancellationToken.None);records.Add(new{phase,status=result.Status.ToString(),result.ErrorCode,result.NativeErrorCode,scalars=result.Outputs.Where(p=>p.Value.Value is not VisionCore.IImageData).ToDictionary(p=>p.Key,p=>p.Value.Value),images=result.Outputs.Where(p=>p.Value.Value is VisionCore.IImageData).ToDictionary(p=>p.Key,p=>{var image=(VisionCore.IImageData)p.Value.Value;return new{image.Width,image.Height,format=image.Format.ToString(),pixelSha256=Convert.ToHexString(SHA256.HashData(image.Pixels)).ToLowerInvariant()};})});return result;}
   var first=await Execute("plugin-frame");if(first.Status!=ModuleStatus.Success||first.Outputs["SNCode"].As<long>()!=8)throw new Exception("Real plugin first frame/SN mismatch");
   uint previousSource=EnumValue("TriggerSource");try{int code=(int)Private("Invoke",handle,"MV_CC_SetEnumValue_NET",new object?[]{"TriggerSource",0u});if(code!=0)throw new Exception("External-trigger setup failed");var failed=await Execute("plugin-no-trigger");if(failed.Status!=ModuleStatus.Failed||failed.Outputs["GetCameraFailCount"].As<long>()!=1||failed.NativeErrorCode!=0)throw new Exception("Real plugin timeout counter/diagnostic mismatch");}finally{int code=(int)Private("Invoke",handle,"MV_CC_SetEnumValue_NET",new object?[]{"TriggerSource",previousSource});if(code!=0)throw new Exception("Trigger source restore failed");}
   var recovered=await Execute("plugin-recovered");if(recovered.Status!=ModuleStatus.Success||recovered.Outputs["SNCode"].As<long>()!=9||recovered.Outputs["GetCameraFailCount"].As<long>()!=1)throw new Exception("Real plugin recovery counter/SN mismatch");
   parameters["StitchEnable"]=JsonSerializer.SerializeToElement(true);parameters["StitchHeight"]=JsonSerializer.SerializeToElement(1);parameters["StitchStartHeight"]=JsonSerializer.SerializeToElement(2048);var invalid=await Execute("plugin-invalid-stitch");if(invalid.NativeErrorCode!=0xe0000502||invalid.Outputs["SNCode"].As<long>()!=10||!invalid.Outputs.ContainsKey("FrameNum")||invalid.Outputs["GetCameraFailCount"].As<long>()!=1)throw new Exception("Camera stitch failure lost already-produced counters/SN");parameters["StitchEnable"]=JsonSerializer.SerializeToElement(false);var after=await Execute("plugin-after-invalid-stitch");if(after.Status!=ModuleStatus.Success||after.Outputs["SNCode"].As<long>()!=11)throw new Exception("Camera stitch recovery SN mismatch");
   records.Add(new{phase="final-settings",exposure=FloatValue("ExposureTime"),gain=FloatValue("Gain"),triggerMode=EnumValue("TriggerMode"),triggerSource=EnumValue("TriggerSource")});
  }catch(Exception ex){records.Add(new{phase="failure",error=ex.ToString()});throw;}
  finally{File.WriteAllText(Path.Combine(output,"probe.json"),JsonSerializer.Serialize(records,new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine(output);}
 }
}
