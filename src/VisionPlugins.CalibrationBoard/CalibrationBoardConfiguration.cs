using System.Text.Json;
using System.IO;
using System.Windows;
using VisionPlugins.Calibration;
using VisionPluginContracts;
namespace VisionPlugins.CalibrationBoard;

public sealed partial class CalibrationBoardPlugin {
 public ToolConfigurationSession CreateConfiguration(ToolEditorContext context){
  var values=Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());foreach(var p in context.Node.Parameters)values[p.Key]=p.Value.Clone();
  IReadOnlyDictionary<string,JsonElement> ReadValues()=>context.ReadEffectiveParameters?.Invoke()??values;
  void SyncValues()=>values=ReadValues().ToDictionary(p=>p.Key,p=>p.Value.Clone());
  void Apply(Dictionary<string,JsonElement> next){context.Apply(next.ToDictionary(p=>p.Key,p=>p.Value.Clone()));values=next;}
  var common=ConfigurationLocalization.ForLanguage("Chinese");var captions=JsonSerializer.Deserialize<Dictionary<string,Dictionary<string,string>>>(CalibrationBoardMetadata.Resource("Localization.json"))!["Chinese"];string localize(string key)=>captions.GetValueOrDefault(key)??common.Get(key);
    Dictionary<string,string> User(){try{return JsonSerializer.Deserialize<Dictionary<string,string>>(ReadValues()["ConfigurationUserData"].GetString()??"{}")??new();}catch(JsonException){return new();}}
  var adapter=new ToolConfigurationParameterAdapter(Descriptor,ReadValues,Apply,key=>User().GetValueOrDefault(key),(key,value)=>{SyncValues();var user=User();user[key]=value;var next=values.ToDictionary(p=>p.Key,p=>p.Value.Clone());next["ConfigurationUserData"]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(user));Apply(next);});
  ToolConfigurationSession? session=null;var host=context.ConfigurationExecutionStatus;
  ConfigurationExecutionHostStatus Host()=>host?.Invoke()??new(false,false,false,false,false);
  var execution=new ToolConfigurationExecution(()=>context.Run(values.ToDictionary(p=>p.Key,p=>p.Value.Clone())),()=>Host().SolutionBusy,()=>Host().Ready&&Host().RunEnabled,()=>Host().Loading,()=>Host().ProcedureBusy);
    ToolConfigurationState? state=null;Func<Window?> owner=()=>null;var files=context.ConfigurationFiles??new WpfConfigurationFileService();var saves=context.ConfigurationSaveFiles??new WpfConfigurationSaveFileService();
  void NotifySave(string message,bool error=false)=>(context.ConfigurationNotifications??new WpfConfigurationNotificationService()).Notify(owner(),new(error?ConfigurationNotificationKind.Error:ConfigurationNotificationKind.Success,message,"标定板",context.Mode));
  var dispatcher=new Dispatcher(request=>{
   if(request.Kind==ConfigurationOperationKind.ReadParameter&&!Descriptor.Parameters.Any(p=>p.Name==request.Source.Name))return new(true,request.Source.Value);
   if(request.Kind==ConfigurationOperationKind.EnumerateInputs){
    var query=ConfigurationSubscriptionValidation.FromElement(request.Source.Element);
    if(query is null||context.ConfigurationSubscriptions is null)return new(true,Error:"源声明的订阅服务不可用。");
    var tree=context.ConfigurationSubscriptions.Enumerate(query);
    return new(true,Error:tree.Status==ConfigurationSubscriptionStatus.Available?null:tree.Diagnostic??tree.Status.ToString()){SubscriptionTree=tree};
   }
   if(request.Kind==ConfigurationOperationKind.WriteParameter&&request.Source.Name=="CalibPathName"&&!string.IsNullOrEmpty(request.Value)&&!Path.GetExtension(request.Value).Equals(".iwcal",StringComparison.OrdinalIgnoreCase))return new(true,Error:"标定文件必须使用.iwcal扩展名。");
   if(request.Kind is ConfigurationOperationKind.ReadInputBinding or ConfigurationOperationKind.WriteInputBinding or ConfigurationOperationKind.ReadParameterBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding){
    var query=ConfigurationSubscriptionValidation.FromElement(request.Source.Element,request.Kind is ConfigurationOperationKind.ReadInputBinding or ConfigurationOperationKind.WriteInputBinding?null:ConfigurationSubscriptionDomain.Parameter);
    if(query is null||context.ConfigurationSubscriptions is null)return new(true,Error:"源声明的订阅服务不可用。");
    var result=request.Kind is ConfigurationOperationKind.WriteInputBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding?context.ConfigurationSubscriptions.Bind(query.Destination,request.Kind==ConfigurationOperationKind.ClearParameterBinding?null:request.SubscriptionBinding):context.ConfigurationSubscriptions.Read(query.Destination);
    if(result.Status!=ConfigurationSubscriptionStatus.Available)return new(true,Error:result.Diagnostic??result.Status.ToString());if(state is not null&&!state.ApplySubscription(request.Source,result,query.Destination.Domain==ConfigurationSubscriptionDomain.Parameter))return new(true,Error:"订阅呈现未确认。");return new(true,result.DisplayText??"");
   }
      if(request.Kind is ConfigurationOperationKind.SaveFile or ConfigurationOperationKind.SaveCalibration){
    var selected=saves.SelectSave(owner(),new(Id,values["CalibPathName"].GetString()??"",".iwcal|*.iwcal",localize("Create Calibration File"),".iwcal"));if(selected.Status==ConfigurationFileStatus.Cancelled)return new(true);
    if(selected.Status!=ConfigurationFileStatus.Selected)return new(true,Error:selected.Diagnostic??selected.Status.ToString());
    try{var selectedPath=selected.Paths.Single();if(ToolConfigurationCalibrationPathPolicy.ValidateSelectedSave(state!,selectedPath,localize) is {} saveError)throw new InvalidDataException(saveError);Export(context,selectedPath);var next=values.ToDictionary(p=>p.Key,p=>p.Value.Clone());next["CalibPathName"]=JsonSerializer.SerializeToElement(selectedPath);Apply(next);if(state?.Find("CalibPathName") is {} pathNode)state.RefreshParameter(pathNode);var remembered=saves.Remember(Id,selected.Paths.Single());if(!remembered.Success)state?.ReportDiagnostic(new("FileHistory","Remember",null,remembered.Diagnostic??"History failed."));NotifySave(localize("Lang_SaveSuccess"));return new(true);}catch(Exception error)when(error is InvalidDataException or IOException or InvalidOperationException or UnauthorizedAccessException){NotifySave(error.Message,true);return new(true,Error:error.Message);}
   }
      if(request.Kind==ConfigurationOperationKind.ResetParameters){
    var confirmations=context.ConfigurationConfirmations??new WpfConfigurationConfirmationService();if(!confirmations.Confirm(owner(),new(localize("Lang_Sure"),localize("Lang_ResetAlgoParamTips")+"?")))return new(true);
    SyncValues();var result=ToolConfigurationAlgorithmReset.Execute(Descriptor,state!,adapter,context.ConfigurationSubscriptions,CalibrationBoardMetadata.Resource("Algorithm.xml"),SyncValues);
    (context.ConfigurationNotifications??new WpfConfigurationNotificationService()).Notify(owner(),new(result.Error is null?ConfigurationNotificationKind.Success:ConfigurationNotificationKind.Error,result.Error??localize("Lang_ResetAlgoParamSuccess"),localize("Lang_Sure"),context.Mode));return result;
   }
   if(request.Kind==ConfigurationOperationKind.Run){_=execution.RunOnceAsync();return new(true);}
   if(request.Kind==ConfigurationOperationKind.RunContinuous)return new(true,Error:execution.StartContinuous()==ConfigurationRunRejection.None?null:"执行尚未就绪。");
   if(request.Kind==ConfigurationOperationKind.StopContinuous){_=execution.StopAsync();return new(true);}
   if(request.Kind==ConfigurationOperationKind.Close){session?.RequestClose();return new(true);}
   return adapter.Dispatch(request);
  });
  state=new ToolConfigurationState(CalibrationBoardMetadata.Definition(),dispatcher){ExecutionEnabled=Host().RunEnabled};ToolConfigurationCalibrationPathPolicy.Attach(state,context,owner,localize,"标定板");
  var controls=new ToolConfigurationControlRegistry();ToolConfigurationStandardControls.Register(controls,localize,()=>new(execution.IsContinuous,Host().ProcedureBusy,Host().RunEnabled),"Chinese",context.ConfigurationSubscriptions);ToolConfigurationSubscriptionControls.Register(controls,context.ConfigurationSubscriptions,localize,"Chinese");
    ToolConfigurationCalibrationPathControls.Register(controls,c=>{
   owner=c.Owner;var selected=files.Select(owner(),new(Id,c.Node.Value,".iwcal|*.iwcal",localize("Set Calibration File Path"),CheckFileExists:false,CheckPathExists:false));
   if(selected.Status==ConfigurationFileStatus.Selected){state.CommitUser(c.Node,selected.Paths.Single(),true);files.Remember(Id,selected.Paths.Single());}
  },localize,context.ConfigurationSubscriptions);
  var displayObjects=new ToolDisplayConfigurationObjectService(()=>ToolDisplayService.Read(values,DisplayDefinition),settings=>{var next=values.ToDictionary(p=>p.Key,p=>p.Value.Clone());next[ToolDisplayService.SettingsParameter]=ToolDisplayService.Serialize(settings);Apply(next);return true;});
  ToolConfigurationObjectControls.Register(controls,displayObjects,localize);
  var imageObjects=new ToolDisplayObjectStore(ToolDisplayObjectDefinition.Parse(CalibrationBoardMetadata.Resource("Display.xml")),()=>values,Apply);ToolConfigurationImageObjectControls.Register(controls,imageObjects,localize);
  var roiControls=ToolConfigurationRoiControls.Register(controls,context,Descriptor.RoiEditor!,"InputImage",localize);
  void RefreshExecution(){state.IsBusy=execution.IsRunning;}execution.Changed+=RefreshExecution;session=new(state,controls,"Chinese",localize,()=>{roiControls.Dispose();execution.Changed-=RefreshExecution;_=execution.DisposeAsync();});state.Initialize();return session;
 }
 sealed class Dispatcher(Func<ConfigurationOperationRequest,ConfigurationOperationResult> dispatch):IToolConfigurationDispatcher{public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest request)=>dispatch(request);}
}










