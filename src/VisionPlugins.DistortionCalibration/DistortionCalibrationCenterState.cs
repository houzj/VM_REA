using VisionCore;
namespace VisionPlugins.DistortionCalibration;
/// <summary>Original Process20550 keeps SDK center state when Y is unbound. SDK Run2a70
/// uses integer-truncated floats for the perspective anchor; radial fit center is separate.</summary>
public sealed class DistortionCalibrationCenterState {
 float x,y;
 public Point2 Stored=>new(x,y);
 public Point2 Effective=>new(unchecked((int)x),unchecked((int)y));
 public Point2 Update(double? inputX,double? inputY){if(inputY.HasValue){x=(float)(inputX??0);y=(float)inputY.Value;}return Effective;}
}
