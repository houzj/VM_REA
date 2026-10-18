using VisionPluginContracts;
namespace VisionPlugins.Calibration;

/// <summary>IMVSNImageCalibModuControl HidePhysicInfoOperation, distinct from the N-point acquisition-mode rule.</summary>
public sealed class NImagePhysicalVisibilityRule : IToolConfigurationRule
{
 public void Apply(ToolConfigurationState state,ConfigurationOperationRequest request){
  var target=request.Target??(request.TargetName is {} name?state.Find(name):null);
  var input=state.Find("CalibPointInput");var angle=state.Find("RotateAngle");var point=state.Find("PhyPoint");var x=state.Find("MovePhysicalPointX");var y=state.Find("MovePhysicalPointY");
  if(target?.IsCategory!=true||input is null||angle is null||point is null||x is null||y is null){state.ReportDiagnostic(new("NImageRuleModel","HidePhysicInfoOperation",request.Source.Name,"Original category/input/angle/point model is incomplete."));return;}
  bool Bound(ConfigurationNodeState node)=>node.SubscriptionBinding is not null||!string.IsNullOrEmpty(node.Value);
  state.SetProgrammatic(target,"CustomVisible",(!Bound(angle)&&(input.Value=="0"?!Bound(point):!Bound(x)&&!Bound(y))).ToString());
 }
 public void Revert(ToolConfigurationState state,ConfigurationOperationRequest request){if((request.Target??(request.TargetName is {} name?state.Find(name):null)) is {} target)state.SetProgrammatic(target,"CustomVisible","True");}
}
