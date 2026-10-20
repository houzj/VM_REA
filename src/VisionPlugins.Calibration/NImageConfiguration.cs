using System.IO;
using System.Text.Json;
using System.Windows;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.Calibration;

/// <summary>Production source-XML configuration adapter. The owner supplies real clear/export operations;
/// this adapter does not make an incomplete algorithm available in the toolbox.</summary>
public sealed class NImageConfigurationProvider(Action<ToolEditorContext> clear,Action<ToolEditorContext,string> export):IToolConfigurationProvider
{
 public const string ModuleId="vision.n-image-calibration";
 public static ToolDisplayDefinition DisplayDefinition{get;}=new("Calib Status:{0},RunParam_err_trans:{1},RunParam_err_trans_world:{2},RunParam_err_rot:{3},RunParam_err_rot_world:{4},RotateCenterPointX:{5},RotateCenterPointY:{6},Calibration Progress:{7}/{8}",["CalibStatus","TransError","TransWorldError","RotError","RotWorldError","RotateCenterPointX","RotateCenterPointY","CurNum","TotalNum"],["ModuStatus","CalibStatus","TransError","TransWorldError","RotError","RotWorldError","RotateCenterPointX","RotateCenterPointY"],"InImage");
 public static ModuleDescriptor ConfigurationDescriptor{get;}=new(ModuleId,"1.0.0","N图像标定",[],[],NImageParameters.Definitions);
 public ToolConfigurationSession CreateConfiguration(ToolEditorContext context){
  string Resource(string name){using var stream=typeof(NImageConfigurationProvider).Assembly.GetManifestResourceStream("VisionPlugins.Calibration.Resources."+name)??throw new InvalidDataException("Missing original NImage resource: "+name);using var reader=new StreamReader(stream);return reader.ReadToEnd();}
  var definition=ToolConfigurationDefinition.Parse(Resource("NImageAlgorithmTab.xml"),Resource("ConfigurationGroups.xml"));
  var values=NImageParameters.Defaults();foreach(var pair in context.Node.Parameters)values[pair.Key]=pair.Value.Clone();
  IReadOnlyDictionary<string,JsonElement> ReadValues()=>context.ReadEffectiveParameters?.Invoke()??values;
  void SyncValues()=>values=ReadValues().ToDictionary(p=>p.Key,p=>p.Value.Clone());
  ToolEditorContext Current()=>context with{Node=context.Node with{Parameters=ReadValues().ToDictionary(p=>p.Key,p=>p.Value.Clone())}};
  void Apply(Dictionary<string,JsonElement> next){context.Apply(next.ToDictionary(p=>p.Key,p=>p.Value.Clone()));values=next;}
  Dictionary<string,string> User()=>JsonSerializer.Deserialize<Dictionary<string,string>>(ReadValues()["ConfigurationUserData"].GetString()??"{}")??[];
  var adapter=new ToolConfigurationParameterAdapter(ConfigurationDescriptor,ReadValues,Apply,key=>User().GetValueOrDefault(key),(key,value)=>{SyncValues();var user=User();user[key]=value;var next=new Dictionary<string,JsonElement>(values){["ConfigurationUserData"]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(user))};Apply(next);});
  var text=ConfigurationLocalization.ForLanguage("Chinese").Get;ToolConfigurationState? state=null;ToolConfigurationSession? session=null;Func<Window?> owner=()=>null;
  ConfigurationExecutionHostStatus Host()=>context.ConfigurationExecutionStatus?.Invoke()??new(false,false,false,false,false);
  var execution=new ToolConfigurationExecution(()=>context.Run(values.ToDictionary(p=>p.Key,p=>p.Value.Clone())),()=>Host().SolutionBusy,()=>Host().Ready&&Host().RunEnabled,()=>Host().Loading,()=>Host().ProcedureBusy);
  var files=context.ConfigurationFiles??new WpfConfigurationFileService();var saves=context.ConfigurationSaveFiles??new WpfConfigurationSaveFileService();var notifications=context.ConfigurationNotifications??new WpfConfigurationNotificationService();
  void Notify(string message,bool error=false)=>notifications.Notify(owner(),new(error?ConfigurationNotificationKind.Error:ConfigurationNotificationKind.Success,message,"N图像标定",context.Mode));
  ConfigurationOperationResult Save(){var chosen=saves.SelectSave(owner(),new(ModuleId,values["CalibPathName"].GetString()??"",".xml|*.xml",text("Create Calibration File"),".xml"));if(chosen.Status==ConfigurationFileStatus.Cancelled)return new(true);if(chosen.Status!=ConfigurationFileStatus.Selected)return new(true,Error:chosen.Diagnostic??chosen.Status.ToString());try{var path=chosen.Paths.Single();if(ToolConfigurationCalibrationPathPolicy.ValidateSelectedSave(state!,path,text) is {} saveError)throw new InvalidDataException(saveError);export(Current(),path);var user=User();user["SaveCalibPath"]=path;Apply(new(values){["CalibPathName"]=JsonSerializer.SerializeToElement(path),["ConfigurationUserData"]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(user))});if(state?.Find("CalibPathName") is {} node)state.RefreshParameter(node);saves.Remember(ModuleId,path);Notify(text("Lang_SaveSuccess"));return new(true);}catch(Exception error)when(error is InvalidDataException or IOException or ArgumentException or InvalidOperationException or UnauthorizedAccessException){Notify(error.Message,true);return new(true,Error:error.Message);}}
  var dispatcher=new Dispatcher(request=>{
   if(request.Kind==ConfigurationOperationKind.EnumerateInputs){
    var query=ConfigurationSubscriptionValidation.FromElement(request.Source.Element);
    if(query is null||context.ConfigurationSubscriptions is null)return new(true,Error:"Source-declared subscription service unavailable.");
    var tree=context.ConfigurationSubscriptions.Enumerate(query);
    return new(true,Error:tree.Status==ConfigurationSubscriptionStatus.Available?null:tree.Diagnostic??tree.Status.ToString()){SubscriptionTree=tree};
   }
   if(request.Kind is ConfigurationOperationKind.ReadInputBinding or ConfigurationOperationKind.WriteInputBinding or ConfigurationOperationKind.ReadParameterBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding){bool parameter=request.Kind is ConfigurationOperationKind.ReadParameterBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding;var query=ConfigurationSubscriptionValidation.FromElement(request.Source.Element,parameter?ConfigurationSubscriptionDomain.Parameter:null);if(query is null||context.ConfigurationSubscriptions is null)return new(true,Error:"Source-declared subscription service unavailable.");bool write=request.Kind is ConfigurationOperationKind.WriteInputBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding;var result=write?context.ConfigurationSubscriptions.Bind(query.Destination,request.Kind==ConfigurationOperationKind.ClearParameterBinding?null:request.SubscriptionBinding):context.ConfigurationSubscriptions.Read(query.Destination);if(result.Status!=ConfigurationSubscriptionStatus.Available)return new(true,Error:result.Diagnostic??result.Status.ToString());if(!state!.ApplySubscription(request.Source,result,parameter))return new(true,Error:"Subscription readback was not authoritative.");return new(true,result.DisplayText??"");}
   if(request.Kind is ConfigurationOperationKind.SaveFile or ConfigurationOperationKind.SaveCalibration)return Save();
   // Original SetParam(CalibPathName) rejects an empty payload (0xe0000204).
   // Unlike SaveCalibPath, this setter does not require a .xml suffix or load the file.
   if(request.Kind==ConfigurationOperationKind.WriteParameter&&request.Source.Name=="CalibPathName"&&string.IsNullOrEmpty(request.Value))return new(true,Error:"SetParam(CalibPathName): 0xe0000204 empty path.");
   if(request.Kind==ConfigurationOperationKind.Clear){try{Apply(new(values){["ClearRevision"]=JsonSerializer.SerializeToElement(checked(values["ClearRevision"].GetInt32()+1))});clear(Current());return new(true);}catch(Exception error){return new(true,Error:error.Message);}}
   if(request.Kind==ConfigurationOperationKind.Close){session?.RequestClose();return new(true);}
   if(request.Kind==ConfigurationOperationKind.Run){if(!Host().Ready||!Host().RunEnabled||Host().Loading||Host().SolutionBusy)return new(true,Error:"Configuration host is not ready for the source-triggered run.");_=execution.RunOnceAsync();return new(true);}
   if(request.Kind==ConfigurationOperationKind.ResetParameters){var confirmation=context.ConfigurationConfirmations??new WpfConfigurationConfirmationService();if(!confirmation.Confirm(owner(),new(text("Lang_Sure"),text("Lang_ResetAlgoParamTips")+"?")))return new(true);SyncValues();var result=ToolConfigurationAlgorithmReset.Execute(ConfigurationDescriptor,state!,adapter,context.ConfigurationSubscriptions,Resource("NImageAlgorithm.xml"),SyncValues);Notify(result.Error??text("Lang_ResetAlgoParamSuccess"),result.Error is not null);return result;}
   if(request.Kind==ConfigurationOperationKind.ReadParameter&&!ConfigurationDescriptor.Parameters.Any(p=>p.Name==request.Source.Name))return new(true,request.Source.Value);
   return adapter.Dispatch(request);
  });
  state=new(definition,dispatcher){ExecutionEnabled=Host().RunEnabled};ToolConfigurationCalibrationPathPolicy.Attach(state,context,owner,text,"N图像标定");state.RegisterRule("HidePhysicInfoOperation",new NImagePhysicalVisibilityRule());state.RegisterRule("SetMutualExclusion",new CountMinimumRule());
  var controls=new ToolConfigurationControlRegistry();ToolConfigurationStandardControls.Register(controls,text,language:"Chinese",subscriptions:context.ConfigurationSubscriptions);ToolConfigurationSubscriptionControls.Register(controls,context.ConfigurationSubscriptions,text,"Chinese");
  // Same shipped OpenFileForCalibDialog type and source template as the N-point module.
  ToolConfigurationCalibrationPathControls.Register(controls,control=>{owner=control.Owner;var chosen=files.Select(owner(),new(ModuleId,control.Node.Value,".xml|*.xml",text("Set Calibration File Path"),CheckFileExists:false));if(chosen.Status==ConfigurationFileStatus.Selected){state.CommitUser(control.Node,chosen.Paths.Single(),true);files.Remember(ModuleId,chosen.Paths.Single());}},text,context.ConfigurationSubscriptions);
  var objects=new ToolDisplayConfigurationObjectService(()=>ToolDisplayService.Read(values,DisplayDefinition),settings=>{Apply(new(values){[ToolDisplayService.SettingsParameter]=ToolDisplayService.Serialize(settings)});return true;});
  ToolConfigurationImageObjectControls.Register(controls,new ToolDisplayObjectStore(NImagePlugin.DisplayObjectDefinitions,()=>values,Apply),text);
  ToolConfigurationObjectControls.Register(controls,objects,text,colorPicker:(initial,selected)=>ToolConfigurationObjectControls.CreateColorTabs(initial,value=>selected(value.Length==9?"#"+value[3..]:value),callback=>{var palette=new VisionUi.SourceColorPalette();palette.Selected+=callback;return palette;},(color,callback)=>{var advanced=new VisionUi.SourceAdvancedColorPicker{SelectedCurrentColor=new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color))};advanced.ColorChanged+=brush=>callback(brush.ToString());return advanced;},new Uri("/VisionUi;component/SourceColorPickerStyles.xaml",UriKind.Relative),text));
  void Changed()=>state.IsBusy=execution.IsRunning;execution.Changed+=Changed;
  session=new(state,controls,"Chinese",text,()=>{execution.Changed-=Changed;_=execution.DisposeAsync();});state.Initialize();return session;
 }
 sealed class Dispatcher(Func<ConfigurationOperationRequest,ConfigurationOperationResult> dispatch):IToolConfigurationDispatcher{public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest request)=>dispatch(request);}
 sealed class CountMinimumRule:IToolConfigurationRule{
  public void Apply(ToolConfigurationState state,ConfigurationOperationRequest request){if(request.Target is {} target)state.SetProgrammatic(target,"MinValue",request.Source.Value=="0"?"1":"0");}
  public void Revert(ToolConfigurationState state,ConfigurationOperationRequest request){if(request.Target is {} target)state.SetProgrammatic(target,"MinValue","0");}
 }
}


