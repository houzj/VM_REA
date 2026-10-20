using VisionFramework;
namespace VisionPlugins.MapCalibration;

// Independent managed interoperability rule; not original ROIBOX subscription precedence.
internal static class MapCalibrationRegionInput {
 internal static IReadOnlyList<RoiDocument> Resolve(IReadOnlyList<RoiDocument> configured,RoiRegionValue? input){
  Validate(configured);
  if(input is null)return configured;
  var supplied=input.Regions;Validate(supplied);
  bool full=IsFull(configured);
  if(!full&&!Same(configured,supplied))throw new ArgumentException("RegionConfigurationConflict: 输入Region与配置ROI不一致。");
  return supplied;
 }
 static bool IsFull(IReadOnlyList<RoiDocument> values)=>values.Count==0||values[0].Kind==RoiKind.FullImage;
 static void Validate(IReadOnlyList<RoiDocument> values){
  if(values.Count>1||values.Any(r=>r.Exclusion||r.CoordinateSpace!=RoiCoordinateSpace.ImagePixels||r.Kind is not (RoiKind.Rectangle or RoiKind.FullImage)))throw new ArgumentException("UnsupportedRegion: 映射标定仅接受原图像素坐标的单个包含矩形或全图。");
  foreach(var roi in values)RoiValidator.Validate(roi);
 }
 static bool Same(IReadOnlyList<RoiDocument> a,IReadOnlyList<RoiDocument> b){
  if(IsFull(a)||IsFull(b))return IsFull(a)&&IsFull(b);
  var x=a[0];var y=b[0];
  return x.Kind==y.Kind&&x.CoordinateSpace==y.CoordinateSpace&&x.Exclusion==y.Exclusion&&x.CenterX==y.CenterX&&x.CenterY==y.CenterY&&x.Width==y.Width&&x.Height==y.Height&&x.AngleDegrees==y.AngleDegrees;
 }
}
