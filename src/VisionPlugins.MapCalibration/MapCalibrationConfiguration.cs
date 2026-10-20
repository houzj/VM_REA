using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using VisionPluginContracts;
namespace VisionPlugins.MapCalibration;
public sealed partial class MapCalibrationPlugin {
 public ToolConfigurationSession CreateConfiguration(ToolEditorContext context){
  var values=Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());foreach(var p in context.Node.Parameters)values[p.Key]=p.Value.Clone();
  void Sync(){if(context.ReadEffectiveParameters?.Invoke() is {} current)foreach(var p in current)if(values.ContainsKey(p.Key))values[p.Key]=p.Value.Clone();}
  Dictionary<string,JsonElement> Read(){Sync();return values;}
  void Apply(Dictionary<string,JsonElement> next){context.Apply(next);values=next;}
  Dictionary<string,string> User()=>JsonSerializer.Deserialize<Dictionary<string,string>>(Read()["ConfigurationUserData"].GetString()??"{}")??[];
  var adapter=new ToolConfigurationParameterAdapter(Descriptor,Read,Apply,key=>User().GetValueOrDefault(key),(key,value)=>{var user=User();user[key]=value;Apply(new(values){["ConfigurationUserData"]=J(JsonSerializer.Serialize(user))});});
  var common=ConfigurationLocalization.ForLanguage("Chinese");var captions=JsonSerializer.Deserialize<Dictionary<string,Dictionary<string,string>>>(MapCalibrationMetadata.Resource("MapCalibrationLocalization.json"))!["Chinese"];string localize(string key)=>captions.GetValueOrDefault(key)??common.Get(key);ToolConfigurationState? state=null;ToolConfigurationSession? session=null;Func<Window?> owner=()=>null;
  var files=context.ConfigurationFiles??new WpfConfigurationFileService();var saves=context.ConfigurationSaveFiles??new WpfConfigurationSaveFileService();
  ConfigurationExecutionHostStatus Host()=>context.ConfigurationExecutionStatus?.Invoke()??new(false,false,false,false,false);
  var execution=new ToolConfigurationExecution(()=>context.Run(new(Read())),()=>Host().SolutionBusy,()=>Host().Ready&&Host().RunEnabled,()=>Host().Loading,()=>Host().ProcedureBusy);
  void NotifyConfiguration(string message,bool error)=>(context.ConfigurationNotifications??new WpfConfigurationNotificationService()).Notify(owner(),new(error?ConfigurationNotificationKind.Error:ConfigurationNotificationKind.Success,message,Manifest.Name,context.Mode));
  ConfigurationOperationResult Save(){Sync();var selected=saves.SelectSave(owner(),new(Id,values["CalibPathName"].GetString()??"",".xml|*.xml",localize("Create Calibration File"),".xml"));if(selected.Status==ConfigurationFileStatus.Cancelled)return new(true);if(selected.Status!=ConfigurationFileStatus.Selected)return new(true,Error:selected.Diagnostic??selected.Status.ToString());var path=selected.Paths.Single();if(ToolConfigurationCalibrationPathPolicy.ValidateSelectedSave(state!,path,localize) is {} rejected){NotifyConfiguration(rejected,true);return new(true,Error:rejected);}var raw=Current(context.Node.Id,context.BaseDirectory,context.Scope);lock(raw){MapCalibrationSave result;if(values["InputType"].GetInt32()==2)result=raw.Save(path);else{var image=CurrentImages(context.Node.Id,context.BaseDirectory,context.Scope);lock(image)result=MapCalibrationImageFiles.Save(Path.GetFullPath(path,context.BaseDirectory),image.Result,values["TeachEnable"].GetBoolean(),image.TeachTriggered,image.SourcePending,image.TargetPending,image.SourcePose,image.TargetPose,image.TeachPose);}if(result.ErrorCode!=0){var error="0x"+result.ErrorCode.ToString("x8");NotifyConfiguration(localize("Lang_FileSaveFail")+": "+error,true);return new(true,Error:error);}Apply(new(values){["CalibPathName"]=J(path)});if(state!.Find("CalibPathName") is {} node)state.RefreshParameter(node);saves.Remember(Id,path);NotifyConfiguration(localize("Lang_SaveSuccess"),false);return new(true);}}
  var dispatch=new Dispatcher(r=>{
   if(r.Kind==ConfigurationOperationKind.EnumerateInputs){
    var query=ConfigurationSubscriptionValidation.FromElement(r.Source.Element);
    if(query is null||context.ConfigurationSubscriptions is null)return new(true,Error:"Source-declared subscription service is unavailable.");
    var tree=context.ConfigurationSubscriptions.Enumerate(query);
    return new(true,Error:tree.Status==ConfigurationSubscriptionStatus.Available?null:tree.Diagnostic??tree.Status.ToString()){SubscriptionTree=tree};
   }
   if(r.Kind is ConfigurationOperationKind.ReadInputBinding or ConfigurationOperationKind.WriteInputBinding or ConfigurationOperationKind.ReadParameterBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding){bool parameter=r.Kind is ConfigurationOperationKind.ReadParameterBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding;var query=ConfigurationSubscriptionValidation.FromElement(r.Source.Element,parameter?ConfigurationSubscriptionDomain.Parameter:null);if(query is null||context.ConfigurationSubscriptions is null)return new(true,Error:"订阅服务不可用。");var result=r.Kind is ConfigurationOperationKind.WriteInputBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding?context.ConfigurationSubscriptions.Bind(query.Destination,r.Kind==ConfigurationOperationKind.ClearParameterBinding?null:r.SubscriptionBinding):context.ConfigurationSubscriptions.Read(query.Destination);if(result.Status!=ConfigurationSubscriptionStatus.Available||!state!.ApplySubscription(r.Source,result,parameter))return new(true,Error:result.Diagnostic??"订阅失败。");return new(true,result.DisplayText??"");}
   if(r.Kind==ConfigurationOperationKind.ReadParameter&&!Descriptor.Parameters.Any(p=>p.Name==r.Source.Name))return new(true,r.Source.Value);
   if(r.Kind==ConfigurationOperationKind.WriteParameter&&r.Source.Name=="CalibPathName"&&string.IsNullOrEmpty(r.Value))return new(true,Error:"0xe0000204");
   if(r.Kind==ConfigurationOperationKind.WriteParameter&&r.Source.Name is "CalibPath0" or "CalibPath1"){var raw=Current(context.Node.Id,context.BaseDirectory,context.Scope);lock(raw){uint code=raw.SetInputPath(r.Source.Name=="CalibPath0"?0:1,r.Value??"");if(code!=0){var error="0x"+code.ToString("x8");NotifyConfiguration(error,true);return new(true,Error:error){KeepAttemptedValueOnError=true};}}var committed=adapter.Dispatch(r);if(committed.Handled&&committed.Error is null)NotifyConfiguration(localize("Lang_SetSuc"),false);return committed;}
   if(r.Kind is ConfigurationOperationKind.SaveCalibration or ConfigurationOperationKind.SaveFile)return Save();
   if(r.Kind==ConfigurationOperationKind.Run){_=execution.RunOnceAsync();return new(true);}
   if(r.Kind==ConfigurationOperationKind.RunContinuous){var rejected=execution.StartContinuous();return new(true,Error:rejected==ConfigurationRunRejection.None?null:rejected.ToString());}
   if(r.Kind==ConfigurationOperationKind.StopContinuous){_=execution.StopAsync();return new(true);}
   if(r.Kind==ConfigurationOperationKind.Close){session?.RequestClose();return new(true);}
   if(r.Kind==ConfigurationOperationKind.ResetParameters){
    var confirmations=context.ConfigurationConfirmations??new WpfConfigurationConfirmationService();if(!confirmations.Confirm(owner(),new(localize("Lang_Sure"),localize("Lang_ResetAlgoParamTips")+"?")))return new(true);
    Sync();var result=ToolConfigurationAlgorithmReset.Execute(Descriptor,state!,adapter,context.ConfigurationSubscriptions,MapCalibrationMetadata.Resource("Algorithm.xml"),Sync);
    (context.ConfigurationNotifications??new WpfConfigurationNotificationService()).Notify(owner(),new(result.Error is null?ConfigurationNotificationKind.Success:ConfigurationNotificationKind.Error,result.Error??localize("Lang_ResetAlgoParamSuccess"),localize("Lang_Sure"),context.Mode));return result;
   }
   return adapter.Dispatch(r);
  });
  state=new(MapCalibrationMetadata.Definition(),dispatch){ExecutionEnabled=Host().RunEnabled};ToolConfigurationCalibrationPathPolicy.Attach(state,context,owner,localize,Manifest.Name);var controls=new ToolConfigurationControlRegistry();ToolConfigurationStandardControls.Register(controls,localize,()=>new(execution.IsContinuous,Host().ProcedureBusy,Host().RunEnabled),"Chinese",context.ConfigurationSubscriptions);ToolConfigurationSubscriptionControls.Register(controls,context.ConfigurationSubscriptions,localize);
  ToolConfigurationCalibrationPathControls.Register(controls,c=>{owner=c.Owner;var selected=files.Select(owner(),new(Id,c.Node.Value,".xml|*.xml",localize("Set Calibration File Path"),CheckFileExists:false,CheckPathExists:true));if(selected.Status==ConfigurationFileStatus.Selected){state.CommitUser(c.Node,selected.Paths.Single(),true);files.Remember(Id,selected.Paths.Single());}},localize,context.ConfigurationSubscriptions);
  ToolConfigurationOpenFilePathControls.Register(controls,c=>{owner=c.Owner;var selected=files.Select(owner(),new(Id,c.Node.Value,".xml;.iwcal|*.xml;*.iwcal",localize(c.Node.CaptionKey),Multiselect:false));if(selected.Status==ConfigurationFileStatus.Selected){var path=selected.Paths.Single();state.CommitUser(c.Node,path,true);files.Remember(Id,path);}},localize);
  controls.Register("SaveFileDialog",c=>{owner=c.Owner;if(!controls.TryCreate("Command",c,out var command))throw new InvalidOperationException("原厂保存叶需要公共Command模板。");return command!;});
  var display=DisplayDefinition;
  var objects=new ToolDisplayConfigurationObjectService(()=>ToolDisplayService.Read(Read(),display),settings=>{Sync();Apply(new(values){[ToolDisplayService.SettingsParameter]=ToolDisplayService.Serialize(settings)});return true;});ToolConfigurationObjectControls.Register(controls,objects,localize);ToolConfigurationImageObjectControls.Register(controls,new ToolDisplayObjectStore(MapCalibrationMetadata.DisplayObjects,Read,Apply),localize);
  var rois=ToolConfigurationRoiControls.Register(controls,context,Descriptor.RoiEditor!,node=>node.Name=="RoiType_0"?"InputImage1":"InputImage2",localize);
  void Changed(){state.IsBusy=execution.IsRunning;}execution.Changed+=Changed;session=new(state,controls,"Chinese",localize,()=>{rois.Dispose();execution.Changed-=Changed;_=execution.DisposeAsync();});state.Initialize();return session;
 }
 sealed class Dispatcher(Func<ConfigurationOperationRequest,ConfigurationOperationResult> dispatch):IToolConfigurationDispatcher {public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest request)=>dispatch(request);}
}








