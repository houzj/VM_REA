using System.IO;
using System.Text.Json;
using System.Windows;
using VisionFramework;
using VisionCore;
using VisionPluginContracts;
namespace VisionPlugins.DistortionCalibration;

/// <summary>Production source-XML configuration adapter. The owner supplies real clear/export operations;
/// backend ownership remains with the production tool and common WPF configuration host.</summary>
public sealed class DistortionCalibrationConfigurationProvider(Action<ToolEditorContext,string> export):IToolConfigurationProvider
{
 public const string ModuleId="vision.distortion-calibration";
 public static ToolDisplayDefinition DisplayDefinition=>DistortionCalibrationMetadata.DisplayDefinition;
 public static ModuleDescriptor ConfigurationDescriptor=>DistortionCalibrationMetadata.Descriptor;
 public ToolConfigurationSession CreateConfiguration(ToolEditorContext context){
  string Resource(string name){using var stream=typeof(DistortionCalibrationConfigurationProvider).Assembly.GetManifestResourceStream("VisionPlugins.DistortionCalibration.Resources."+name)??throw new InvalidDataException("Missing original distortion resource: "+name);using var reader=new StreamReader(stream);return reader.ReadToEnd();}
  var definition=ToolConfigurationDefinition.Parse(Resource("DistortionAlgorithmTab.xml"),Resource("Groups.xml"));
  var values=DistortionCalibrationMetadata.Defaults();foreach(var pair in context.Node.Parameters)values[pair.Key]=pair.Value.Clone();
  ToolEditorContext Current()=>context with{Node=context.Node with{Parameters=values}};
  void Apply(Dictionary<string,JsonElement> next){context.Apply(next.ToDictionary(p=>p.Key,p=>p.Value.Clone()));values=next;}
  void SyncValues(){if(context.ReadEffectiveParameters?.Invoke() is {} effective)foreach(var pair in effective)if(values.ContainsKey(pair.Key))values[pair.Key]=pair.Value.Clone();}
  Dictionary<string,JsonElement> ReadValues(){SyncValues();return values;}
  Dictionary<string,string> User()=>JsonSerializer.Deserialize<Dictionary<string,string>>(ReadValues()["ConfigurationUserData"].GetString()??"{}")??[];
  var adapter=new ToolConfigurationParameterAdapter(ConfigurationDescriptor,ReadValues,Apply,key=>User().GetValueOrDefault(key),(key,value)=>{var user=User();user[key]=value;var next=new Dictionary<string,JsonElement>(values){["ConfigurationUserData"]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(user))};Apply(next);});
  Func<string,string> text=key=>DistortionCalibrationMetadata.Localize(key);ToolConfigurationState? state=null;ToolConfigurationSession? session=null;Func<Window?> owner=()=>null;
  ConfigurationExecutionHostStatus Host()=>context.ConfigurationExecutionStatus?.Invoke()??new(false,false,false,false,false);
  var execution=new ToolConfigurationExecution(()=>context.Run(values.ToDictionary(p=>p.Key,p=>p.Value.Clone())),()=>Host().SolutionBusy,()=>Host().Ready&&Host().RunEnabled,()=>Host().Loading,()=>Host().ProcedureBusy);
  var files=context.ConfigurationFiles??new WpfConfigurationFileService();var saves=context.ConfigurationSaveFiles??new WpfConfigurationSaveFileService();var notifications=context.ConfigurationNotifications??new WpfConfigurationNotificationService();
  void Notify(string message,bool error=false)=>notifications.Notify(owner(),new(error?ConfigurationNotificationKind.Error:ConfigurationNotificationKind.Success,message,"畸变标定",context.Mode));
  ConfigurationOperationResult Save(){var chosen=saves.SelectSave(owner(),new(ModuleId,values["CalibPathName"].GetString()??"",".iccal|*.iccal",text("Create Calibration File"),".iccal"));if(chosen.Status==ConfigurationFileStatus.Cancelled)return new(true);if(chosen.Status!=ConfigurationFileStatus.Selected)return new(true,Error:chosen.Diagnostic??chosen.Status.ToString());try{var path=chosen.Paths.Single();if(ToolConfigurationCalibrationPathPolicy.ValidateSelectedSave(state!,path,text) is {} saveError)throw new InvalidDataException(saveError);DistortionCalibrationFileLifecycle.ValidateExplicitPath(path);export(Current(),path);var user=User();user["SaveCalibPath"]=path;Apply(new(values){["CalibPathName"]=JsonSerializer.SerializeToElement(path),["ConfigurationUserData"]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(user))});if(state?.Find("CalibPathName") is {} node)state.RefreshParameter(node);saves.Remember(ModuleId,path);Notify(text("Lang_SaveSuccess"));return new(true);}catch(Exception error)when(error is InvalidDataException or IOException or ArgumentException or InvalidOperationException or UnauthorizedAccessException){Notify(error.Message,true);return new(true,Error:error.Message);}}
  var dispatcher=new Dispatcher(request=>{
   if(request.Kind==ConfigurationOperationKind.EnumerateInputs){
    var query=ConfigurationSubscriptionValidation.FromElement(request.Source.Element);
    if(query is null||context.ConfigurationSubscriptions is null)return new(true,Error:"Source-declared subscription service unavailable.");
    var tree=context.ConfigurationSubscriptions.Enumerate(query);
    return new(true,Error:tree.Status==ConfigurationSubscriptionStatus.Available?null:tree.Diagnostic??tree.Status.ToString()){SubscriptionTree=tree};
   }
   if(request.Kind is ConfigurationOperationKind.ReadInputBinding or ConfigurationOperationKind.WriteInputBinding or ConfigurationOperationKind.ReadParameterBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding){bool parameter=request.Kind is ConfigurationOperationKind.ReadParameterBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding;var query=ConfigurationSubscriptionValidation.FromElement(request.Source.Element,parameter?ConfigurationSubscriptionDomain.Parameter:null);if(query is null||context.ConfigurationSubscriptions is null)return new(true,Error:"Source-declared subscription service unavailable.");bool write=request.Kind is ConfigurationOperationKind.WriteInputBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding;var result=write?context.ConfigurationSubscriptions.Bind(query.Destination,request.Kind==ConfigurationOperationKind.ClearParameterBinding?null:request.SubscriptionBinding):context.ConfigurationSubscriptions.Read(query.Destination);if(result.Status!=ConfigurationSubscriptionStatus.Available)return new(true,Error:result.Diagnostic??result.Status.ToString());if(!state!.ApplySubscription(request.Source,result,parameter))return new(true,Error:"Subscription readback was not authoritative.");return new(true,result.DisplayText??"");}
   if(request.Kind is ConfigurationOperationKind.SaveFile or ConfigurationOperationKind.SaveCalibration)return Save();
   if(request.Kind==ConfigurationOperationKind.WriteParameter&&request.Source.Name=="CalibPathName"){try{DistortionCalibrationPath.Prepare(request.Value??"");}catch(InvalidDataException error){return new(true,Error:error.Message);}}
   if(request.Kind==ConfigurationOperationKind.RunContinuous)return new(true,Error:execution.StartContinuous()==ConfigurationRunRejection.None?null:"执行尚未就绪。");
   if(request.Kind==ConfigurationOperationKind.StopContinuous){_=execution.StopAsync();return new(true);}
   if(request.Kind==ConfigurationOperationKind.Close){session?.RequestClose();return new(true);}
   if(request.Kind==ConfigurationOperationKind.Run){if(!Host().Ready||!Host().RunEnabled||Host().Loading||Host().SolutionBusy)return new(true,Error:"Configuration host is not ready for the source-triggered run.");_=execution.RunOnceAsync();return new(true);}
   if(request.Kind==ConfigurationOperationKind.ResetParameters){
    var confirmation=context.ConfigurationConfirmations??new WpfConfigurationConfirmationService();
    if(!confirmation.Confirm(owner(),new(text("Lang_Sure"),text("Lang_ResetAlgoParamTips")+"?")))return new(true);
    var result=ToolConfigurationAlgorithmReset.Execute(ConfigurationDescriptor,state!,adapter,context.ConfigurationSubscriptions,Resource("DistortionAlgorithm.xml"),SyncValues);
    Notify(result.Error??text("Lang_ResetAlgoParamSuccess"),result.Error is not null);
    return result;
   }
   if(request.Kind==ConfigurationOperationKind.ReadParameter&&!ConfigurationDescriptor.Parameters.Any(p=>p.Name==request.Source.Name))return new(true,request.Source.Value);
   return adapter.Dispatch(request);
  });
  state=new(definition,dispatcher){ExecutionEnabled=Host().RunEnabled};ToolConfigurationCalibrationPathPolicy.Attach(state,context,owner,text,"畸变标定");
  var controls=new ToolConfigurationControlRegistry();ToolConfigurationStandardControls.Register(controls,text,()=>new(execution.IsContinuous,Host().ProcedureBusy,Host().RunEnabled),language:"Chinese",subscriptions:context.ConfigurationSubscriptions);ToolConfigurationSubscriptionControls.Register(controls,context.ConfigurationSubscriptions,text,"Chinese");
  var roiControls=ToolConfigurationRoiControls.Register(controls,context,ConfigurationDescriptor.RoiEditor!,"InputImage",text);
  // Source OpenFileForCalibDialog uses the common template.
  ToolConfigurationCalibrationPathControls.Register(controls,control=>{owner=control.Owner;var chosen=files.Select(owner(),new(ModuleId,control.Node.Value,".iccal|*.iccal",text("Set Calibration File Path"),CheckFileExists:false));if(chosen.Status==ConfigurationFileStatus.Selected){state.CommitUser(control.Node,chosen.Paths.Single(),true);files.Remember(ModuleId,chosen.Paths.Single());}},text,context.ConfigurationSubscriptions);
  var legacyObjects=new ToolDisplayConfigurationObjectService(()=>ToolDisplayService.Read(values,DisplayDefinition),settings=>{Apply(new(values){[ToolDisplayService.SettingsParameter]=ToolDisplayService.Serialize(settings)});return true;});
  var displayObjects=new ToolDisplayObjectStore(DistortionCalibrationMetadata.DisplayObjects,()=>values,Apply);
  var featureStore=new ToolDisplayFeatureStore(DistortionCalibrationMetadata.DisplayObjects,()=>values,Apply);
  var objects=new ToolDisplayNamedTextConfigurationObjectService(featureStore,legacyObjects);
  ToolConfigurationImageObjectControls.Register(controls,displayObjects,text);
  ToolConfigurationObjectControls.Register(controls,objects,text,colorPicker:(initial,selected)=>ToolConfigurationObjectControls.CreateColorTabs(initial,value=>selected(value.Length==9?"#"+value[3..]:value),callback=>{var palette=new VisionUi.SourceColorPalette();palette.Selected+=callback;return palette;},(color,callback)=>{var advanced=new VisionUi.SourceAdvancedColorPicker{SelectedCurrentColor=new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color))};advanced.ColorChanged+=brush=>callback(brush.ToString());return advanced;},new Uri("/VisionUi;component/SourceColorPickerStyles.xaml",UriKind.Relative),text));
  void Changed()=>state.IsBusy=execution.IsRunning;execution.Changed+=Changed;
  session=new(state,controls,"Chinese",text,()=>{execution.Changed-=Changed;roiControls.Dispose();_=execution.DisposeAsync();});state.Initialize();return session;
 }
 sealed class Dispatcher(Func<ConfigurationOperationRequest,ConfigurationOperationResult> dispatch):IToolConfigurationDispatcher{public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest request)=>dispatch(request);}
}




