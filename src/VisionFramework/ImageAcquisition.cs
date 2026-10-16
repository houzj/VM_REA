using VisionCore;
namespace VisionFramework;

public sealed record CameraInfo(int Id,string Name){public string? PersistentId{get;init;}}
public sealed record CameraRequest(int CameraId,double? ExposureTime,double? Gain,int SetParameterWaitTimeMs);
public sealed record CameraFrame(IImageData Image,long FrameNum=0,long LostFrameCount=0,long LostPacketCount=0);
public sealed class CameraAcquisitionException(uint nativeErrorCode,string message):IOException(message){public uint NativeErrorCode{get;}=nativeErrorCode;}
public interface IImageAcquisitionService {
 IReadOnlyList<CameraInfo> Cameras {get;}
 ValueTask<CameraFrame> AcquireAsync(CameraRequest request,CancellationToken cancellationToken);
}
