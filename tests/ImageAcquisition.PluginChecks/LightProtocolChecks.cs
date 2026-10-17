using System.Buffers.Binary;
using System.IO;
using System.Text.Json;
using VisionFramework;
using VisionPlugins.ImageSource;
static partial class Program {
 sealed class Transport:IAcquisitionLightModuleTransport {
  public IReadOnlyList<AcquisitionLightDevice> Devices=>[new(17,"Offline controller resource",300,2,3,"guid-1")];
  public List<(int Module,string Command,byte[] Data)> Calls=[];
  public ValueTask SetBinaryDataAsync(int moduleId,string command,ReadOnlyMemory<byte> data,CancellationToken token){token.ThrowIfCancellationRequested();Calls.Add((moduleId,command,data.ToArray()));return ValueTask.CompletedTask;}
 }
 static async Task Protocol(){
  var transport=new Transport();var service=new AcquisitionLightProtocolService(transport);var request=new AcquisitionLightRequest(17,3,99,500);
  await service.TriggerAsync(request,default);await service.CompleteAsync(request,default);
  Check(transport.Calls.Count==1&&transport.Calls[0].Command=="LightDeviceParamSetTrigger"&&transport.Calls[0].Module==300,"native light command boundary and no invented physical OFF");
  var bytes=transport.Calls[0].Data;int I(int offset)=>BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(offset,4));
  Check(bytes.Length==0x2ec&&I(0)==2&&I(4)==3&&I(24)==500,"native 748-byte header includes trigger time");
  Check(System.Text.Encoding.ASCII.GetString(bytes,8,6)=="guid-1"&&bytes[14]==0,"fixed device identifier");
  for(int channel=1;channel<=4;channel++){int offset=28+(channel-1)*44;Check(I(offset)==channel&&I(offset+4)==(channel==3?1:0),"source channel offsets and selection");Check(I(offset+8)==(channel==3?99:0)&&I(offset+12)==(channel==3?2:1)&&I(offset+16)==1,"source brightness, state and edge defaults");}
  Check(bytes.Skip(28+4*44).All(b=>b==0),"native unused channel structures and reserve remain zero");
  bytes=AcquisitionLightProtocolService.Encode(transport.Devices[0],request with{Channel=6});Check(Enumerable.Range(0,4).All(i=>I(28+i*44+4)==0),"native UI channels 5/6 do not enable a supported channel");
  bytes=AcquisitionLightProtocolService.Encode(transport.Devices[0] with{DeviceType=101},request);Check(I(28+2*44+12)==1,"other device type state");
  using var scope=new ModuleExecutionScope();scope.RegisterService<IImageAcquisitionService>(new Camera());scope.RegisterService<IAcquisitionLightModuleTransport>(transport);var plugin=new ImageAcquisitionPlugin();var p=plugin.Descriptor.Parameters.ToDictionary(d=>d.Name,d=>d.DefaultValue!.Value.Clone());Set(p,"CameraIdentity","serial");Set(p,"Interval",0);Set(p,"DelayAfterTriggerLight",0);Set(p,"ImageConfigList",JsonSerializer.Serialize(new[]{new AcquisitionImageConfig(LightId:17,Channel:3,Brightness:99,TriggerTime:500)}));var result=await plugin.CreateRuntime().ExecuteAsync(new("protocol",Environment.CurrentDirectory,new Dictionary<string,PortValue>(),p){Scope=scope},default);
  Check(result.Status==ModuleStatus.Success&&transport.Calls.Count==2&&transport.Calls.Last().Data.SequenceEqual(AcquisitionLightProtocolService.Encode(transport.Devices[0],request)),"runtime uses real encoder through module transport contract");
  using var noLight=new ModuleExecutionScope();noLight.RegisterService<IImageAcquisitionService>(new Camera());var missing=await plugin.CreateRuntime().ExecuteAsync(new("missing-light",Environment.CurrentDirectory,new Dictionary<string,PortValue>(),p){Scope=noLight},default);Check(missing.Status==ModuleStatus.Success&&missing.Message is not null&&missing.Outputs.ContainsKey("OutImage0Image"),"source continues camera after unavailable light");
  using var partial=new ModuleExecutionScope();partial.RegisterService<IImageAcquisitionService>(new Camera{FailRequestIndex=1});Set(p,"ImageConfigList",JsonSerializer.Serialize(new[]{new AcquisitionImageConfig(),new AcquisitionImageConfig()}));var recovered=await plugin.CreateRuntime().ExecuteAsync(new("partial",Environment.CurrentDirectory,new Dictionary<string,PortValue>(),p){Scope=partial},default);Check(recovered.Status==ModuleStatus.Success&&!recovered.Outputs.ContainsKey("OutImage0Image")&&recovered.Outputs.ContainsKey("OutImage1Image")&&recovered.Outputs["GetCameraFailCount"].As<long>()==1,"native failure does not abort later configured slots");
  using var wrap=new ModuleExecutionScope();wrap.RegisterService<IImageAcquisitionService>(new Camera{FrameBase=int.MaxValue});Set(p,"ImageConfigList",JsonSerializer.Serialize(new[]{new AcquisitionImageConfig()}));var wrapped=await plugin.CreateRuntime().ExecuteAsync(new("wrap",Environment.CurrentDirectory,new Dictionary<string,PortValue>(),p){Scope=wrap},default);Check(wrapped.Outputs["FrameNum"].As<long>()==int.MinValue,"native signed 32-bit frame counter representation");
 }
}
