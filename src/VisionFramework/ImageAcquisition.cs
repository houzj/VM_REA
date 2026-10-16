using VisionCore;
namespace VisionFramework;

public sealed record CameraInfo(int Id,string Name){public string? PersistentId{get;init;}}
public sealed record CameraRequest(int CameraId,double? ExposureTime,double? Gain,int SetParameterWaitTimeMs){public bool ConvertParametersToFloatBeforeFormatting{get;init;}=true;}
public sealed record AcquisitionLightRequest(int LightId,int Channel,int Brightness,int TriggerTimeMs);
public sealed record AcquisitionLightInfo(int Id,string Name);
public interface IAcquisitionLightService {IReadOnlyList<AcquisitionLightInfo> Lights{get;}ValueTask TriggerAsync(AcquisitionLightRequest request,CancellationToken cancellationToken);ValueTask CompleteAsync(AcquisitionLightRequest request,CancellationToken cancellationToken);}
public sealed record AcquisitionLightDevice(int Id,string Name,int ModuleId,int DeviceIndex,int DeviceType,string DeviceGuid);
// Device drivers own transport/connect/disconnect. This is the module binary-command boundary,
// not a serial/network wire protocol inferred from a controller's name.
public interface IAcquisitionLightModuleTransport {IReadOnlyList<AcquisitionLightDevice> Devices{get;}ValueTask SetBinaryDataAsync(int moduleId,string command,ReadOnlyMemory<byte> data,CancellationToken cancellationToken);}
public sealed record CameraFloatRange(double Value,double Minimum,double Maximum);
public sealed record AcquisitionCameraSettings(CameraFloatRange Exposure,CameraFloatRange Gain);
public interface IAcquisitionCameraSettingsService {AcquisitionCameraSettings ReadSettings(int cameraId);}
public sealed record CameraFrame(IImageData Image,long FrameNum=0,long LostFrameCount=0,long LostPacketCount=0);
public sealed class CameraAcquisitionException(uint nativeErrorCode,string message):IOException(message){public uint NativeErrorCode{get;}=nativeErrorCode;}
public interface IImageAcquisitionService {
 IReadOnlyList<CameraInfo> Cameras {get;}
 ValueTask<CameraFrame> AcquireAsync(CameraRequest request,CancellationToken cancellationToken);
}
