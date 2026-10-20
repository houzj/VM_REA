using System.IO;

namespace VisionPluginContracts;

/// <summary>The concrete OpenFileForCalibDialog.ValueChangedCmdExecute gate.
/// This is not the base OpenFileDialog or a native SetParam validation policy.</summary>
public static class ToolConfigurationCalibrationPathPolicy {
 public static string? ValidateSelectedSave(ToolConfigurationState state,string path,Func<string,string>? localize=null){
  var nodes=state.Nodes.Where(n=>n.Element.Tag=="SaveFileDialog").ToArray();
  if(nodes.Length!=1)return (localize??ConfigurationLocalization.ForLanguage("Chinese").Get)("Lang_FileSaveFail");
  var fields=SourceFilter(nodes[0],localize).Split('|');
  return fields.Length==2&&path.EndsWith(fields[0],StringComparison.CurrentCulture)?null:(localize??ConfigurationLocalization.ForLanguage("Chinese").Get)("Lang_FileSaveFail");
 }
 public static void Attach(ToolConfigurationState state,ToolEditorContext context,Func<System.Windows.Window?> owner,
  Func<string,string> localize,string caption,Action<string>? presentError=null){
  bool validationRejected=false;
  void Notify(string message,bool error){if(error&&presentError is not null)presentError(message);else (context.ConfigurationNotifications??new WpfConfigurationNotificationService()).Notify(owner(),new(error?ConfigurationNotificationKind.Error:ConfigurationNotificationKind.Success,message,caption,context.Mode));}
  state.ParameterWriteValidator=request=>{
   validationRejected=false;
   var result=ValidateSourceWrite(request,localize);
   if(result?.Error is {} error){
    validationRejected=true;Notify(error,true);
   }
   return result;
  };
  state.ParameterWriteCompleted=(request,result)=>{
   if(request.Source.Element.Tag!="OpenFileForCalibDialog"||validationRejected)return;
   if(result.Handled)Notify(localize(result.Error is null?"Lang_SetSuc":"Lang_FileNameSetFail"),result.Error is not null);
  };
 }
 public static string SourceFilter(ConfigurationNodeState node,Func<string,string>? localize=null){
  string raw=node.Element.Children.FirstOrDefault(c=>c.Tag=="FileOption")?.Children.FirstOrDefault(c=>c.Tag=="FilterName")?.Text??"";
  string filter=(localize??ConfigurationLocalization.ForLanguage("Chinese").Get)(raw);
  return filter.Length>=2&&filter[0]=='%'&&filter[^1]=='%'?filter[1..^1]:filter;
 }
 public static ConfigurationOperationResult? ValidateSourceWrite(ConfigurationOperationRequest request,
  Func<string,string>? localize=null,Func<string,bool>? directoryExists=null){
  if(request.Kind!=ConfigurationOperationKind.WriteParameter||request.Source.Element.Tag!="OpenFileForCalibDialog")return null;
  var fields=SourceFilter(request.Source,localize).Split('|');
  ConfigurationOperationResult Reject(string error)=>new(true,Error:error){KeepAttemptedValueOnError=true};
  if(fields.Length<=1)return Reject("Unsupported calibration file filter declaration.");
  string value=request.Value??"";
  var patterns=fields[1].Split(';');bool matches=false;
  foreach(string pattern in patterns){
   if(pattern.Length==0)return Reject("Unsupported empty calibration file filter pattern.");
   if(value.EndsWith(pattern.Substring(1),StringComparison.CurrentCulture)){matches=true;break;}
  }
  if(!matches)return Reject((localize??ConfigurationLocalization.ForLanguage("Chinese").Get)("Lang_FileOpenFail"));
  // The original branch calls FtpCheckFileName for /root. No remote provider is
  // fabricated by this local policy, and no local directory check is substituted.
  if(value.StartsWith("/root",StringComparison.CurrentCulture))return Reject("Remote calibration directory validation is unsupported.");
  string directory=value;int slash=value.LastIndexOf('\\');
  if(slash>=0&&slash<value.Length)directory=value.Substring(0,slash);
  if(!(directoryExists??Directory.Exists)(directory))return Reject((localize??ConfigurationLocalization.ForLanguage("Chinese").Get)("Lang_FileOpenFail"));
  return null;
 }
}
