using System.Buffers.Binary;
using System.IO;
using System.Text;
using VisionFramework;
namespace VisionPlugins.ImageSource;

/// <summary>Independent ImageAcquisition-to-global-light command encoder.</summary>
public sealed class AcquisitionLightProtocolService(IAcquisitionLightModuleTransport transport):IAcquisitionLightService {
 public IReadOnlyList<AcquisitionLightInfo> Lights=>transport.Devices.Select(d=>new AcquisitionLightInfo(d.Id,d.Name)).ToArray();
 public ValueTask TriggerAsync(AcquisitionLightRequest request,CancellationToken token){token.ThrowIfCancellationRequested();var device=transport.Devices.FirstOrDefault(d=>d.Id==request.LightId)??throw new IOException("光源设备不存在。");return transport.SetBinaryDataAsync(device.ModuleId,"LightDeviceParamSetTrigger",Encode(device,request),token);}
 // Native 180012940 ->180012580 only resets the local staged channel structure.
 // It does not send another device command or force brightness to zero. Each Encode
 // below owns a fresh reset structure, so completion must not invent a physical OFF.
 public ValueTask CompleteAsync(AcquisitionLightRequest request,CancellationToken token)=>ValueTask.CompletedTask;
 public static byte[] Encode(AcquisitionLightDevice device,AcquisitionLightRequest request){
  if(request.Channel is <1 or >6||request.Brightness is <0 or >255||request.TriggerTimeMs is <0 or >65535)throw new ArgumentOutOfRangeException(nameof(request));
  if(device.DeviceGuid.Any(c=>c>127)||device.DeviceGuid.Length>15)throw new ArgumentException("Device GUID must fit the original 16-byte identifier.",nameof(device));
  // Native buffer is 0x2ec, with TriggerTime at +0x18 and channel1 at +0x1c.
  // The older managed ST_LIGHT_CONFIG_ID lacks this extra trigger-time field;
  // marshaling that older 0x2e8 structure would silently shift every channel.
  byte[] data=new byte[0x2ec];void I(int offset,int value)=>BinaryPrimitives.WriteInt32LittleEndian(data.AsSpan(offset,4),value);
  I(0,device.DeviceIndex);I(4,device.DeviceType);Encoding.ASCII.GetBytes(device.DeviceGuid).CopyTo(data,8);I(24,request.TriggerTimeMs);
  for(int channel=1;channel<=4;channel++){int offset=28+(channel-1)*44;I(offset,channel);I(offset+12,1);I(offset+16,1);}
  // UI accepts 1..6; native GetLightChannel supports only 1..4. For 5/6 it logs
  // an invalid channel and leaves the four reset entries disabled.
  if(request.Channel<=4){int offset=28+(request.Channel-1)*44;I(offset+4,1);I(offset+8,request.Brightness);I(offset+12,device.DeviceType is 3 or 4?2:1);}
  return data;
 }
}
