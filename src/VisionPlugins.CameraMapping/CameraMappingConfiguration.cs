using System.IO;
using System.Text.Json;
using System.Windows;
using System.Xml.Linq;
using VisionPluginContracts;
namespace VisionPlugins.CameraMapping;
public sealed partial class CameraMappingPlugin {
 public ToolConfigurationSession CreateConfiguration(ToolEditorContext context){
  string Resource(string name){using var stream=typeof(CameraMappingPlugin).Assembly.GetManifestResourceStream("VisionPlugins.CameraMapping.Resources."+name)??throw new InvalidDataException(name);using var reader=new StreamReader(stream);return reader.ReadToEnd();}
  var definition=ToolConfigurationDefinition.Parse(Resource("CameraMappingAlgorithmTab.xml"),Resource("ConfigurationGroups.xml"));var common=ConfigurationLocalization.ForLanguage("Chinese");var localized=JsonSerializer.Deserialize<Dictionary<string,Dictionary<string,string>>>(Resource("CameraMappingLocalization.json"))!["Chinese"];string text(string key)=>localized.GetValueOrDefault(key)??common.Get(key);
  var values=Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());foreach(var p in context.Node.Parameters)values[p.Key]=p.Value.Clone();
  void Apply(Dictionary<string,JsonElement> next){context.Apply(next.ToDictionary(p=>p.Key,p=>p.Value.Clone()));values=next;}
  void SyncValues(){if(context.ReadEffectiveParameters?.Invoke() is {} effective)foreach(var p in effective)if(values.ContainsKey(p.Key))values[p.Key]=p.Value.Clone();}
  Dictionary<string,JsonElement> ReadValues(){SyncValues();return values;}
  ToolEditorContext Current()=>context with{Node=context.Node with{Parameters=ReadValues().ToDictionary(p=>p.Key,p=>p.Value.Clone())}};
  var files=context.ConfigurationFiles??new WpfConfigurationFileService();var save=context.ConfigurationSaveFiles??new WpfConfigurationSaveFileService();var notifications=context.ConfigurationNotifications??new WpfConfigurationNotificationService();
  ToolConfigurationState? state=null;ToolConfigurationSession? session=null;Func<Window?> owner=()=>null;
  void Notify(string message,bool error=false)=>notifications.Notify(owner(),new(error?ConfigurationNotificationKind.Error:ConfigurationNotificationKind.Success,message,"相机映射",context.Mode));
  ConfigurationExecutionHostStatus Host()=>context.ConfigurationExecutionStatus?.Invoke()??new(false,false,false,false,false);
  var execution=new ToolConfigurationExecution(()=>context.Run(new(values)),()=>Host().SolutionBusy,()=>Host().Ready&&Host().RunEnabled,()=>Host().Loading,()=>Host().ProcedureBusy);
  Dictionary<string,string> User()=>JsonSerializer.Deserialize<Dictionary<string,string>>(ReadValues()["ConfigurationUserData"].GetString()??"{}")??[];
  var adapter=new ToolConfigurationParameterAdapter(Descriptor,ReadValues,Apply,key=>User().GetValueOrDefault(key),(key,value)=>{var user=User();user[key]=value;var next=new Dictionary<string,JsonElement>(values){["ConfigurationUserData"]=J(JsonSerializer.Serialize(user))};Apply(next);});
  void Save(){var result=save.SelectSave(owner(),new(Id,values["CalibPathName"].GetString()??"",".iwcal|*.iwcal",text("Create Calibration File"),".iwcal"));if(result.Status==ConfigurationFileStatus.Cancelled)return;if(result.Status!=ConfigurationFileStatus.Selected){Notify(result.Diagnostic??"无法选择保存路径",true);return;}try{SyncValues();var path=result.Paths.Single();var raw=Get(context.Node.Id,context.BaseDirectory,context.Scope);lock(raw.Gate){if(!raw.CalibrationValid||raw.Matrix is null)throw new CameraMappingOperationException(0xe0000018,"尚无有效标定结果。");SaveCalibration(Current(),path);state!.RefreshParameter(state.Find("CalibPathName")!);}save.Remember(Id,result.Paths.Single());Notify(text("Lang_SaveSuccess"));}catch(Exception error)when(error is IOException or InvalidOperationException or UnauthorizedAccessException or ArgumentException){Notify(error.Message,true);}}
  ConfigurationOperationResult Reset(){
   var confirmation=context.ConfigurationConfirmations??new WpfConfigurationConfirmationService();
   if(!confirmation.Confirm(owner(),new(text("Lang_Sure"),text("Lang_ResetAlgoParamTips")+"?")))return new(true);
   // Original ResetRunParamBind visits only these ISubInfo controls, in resource order.
   foreach(string name in new[]{"CalibPathName","RefreshFileEnable","WeightFactor"}){
    var node=state!.Find(name)!;
    if(node.Get("IsSub")!="True"&&User().GetValueOrDefault("@"+name)!="True")continue;
    state.SetProgrammatic(node,"IsSub","False");
    var query=ConfigurationSubscriptionValidation.FromElement(node.Element,ConfigurationSubscriptionDomain.Parameter)!;
    var service=context.ConfigurationSubscriptions;
    try{
     var current=service?.Read(query.Destination);
     if(node.SubscriptionBinding is not null||!string.IsNullOrEmpty(node.Get("Subscription"))||current?.Binding is not null){
      var cleared=service?.Bind(query.Destination,null);
      if(cleared is not null&&state.ApplySubscription(node,cleared,true))state.RefreshParameter(node);
      else state.ReportSubscriptionDiagnostic(node,"ResetRunParamBind",cleared?.Diagnostic??"无法清除参数订阅。");
     }
    }catch(Exception error){state.ReportSubscriptionDiagnostic(node,"ResetRunParamBind",error.Message);}
    // Re-read effective parameters after unbinding, before persisting user data.
    state.PersistSubscriptionMode(node,false);
   }
   string? lastError=null;
   foreach(var item in XDocument.Parse(Resource("CameraMappingAlgorithm.xml")).Root!.Elements("ParamItem")){
    string name=item.Element("Name")!.Value,value=item.Element("DefaultValue")!.Value;
    if(name=="CalibPathName"&&value.Length==0){lastError="文件格式错误 (0xe0000204)";continue;}
    var result=adapter.Dispatch(new(ConfigurationOperationKind.WriteParameter,"ResetAlgorithmParameter",state!.Find(name)!,null,[],ConfigurationChangeKind.Command,value));
    if(!result.Handled||result.Error is not null)lastError=result.Error??("无法重置参数："+name);
   }
   // CameraMap's empty default path fails; original UI returns before full success refresh.
   if(lastError is not null){Notify(lastError,true);return new(true,Error:lastError);}
   foreach(var node in state!.Nodes)if(node.Name is {} name&&values.ContainsKey(name))state.RefreshParameter(node);
   Notify(text("Lang_ResetAlgoParamSuccess"));return new(true);
  }
  ConfigurationOperationResult Subscription(ConfigurationOperationRequest r){bool parameter=r.Kind is ConfigurationOperationKind.ReadParameterBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding;var query=ConfigurationSubscriptionValidation.FromElement(r.Source.Element,parameter?ConfigurationSubscriptionDomain.Parameter:null);if(query is null||context.ConfigurationSubscriptions is null)return new(true,Error:"Subscription service is unavailable.");var result=r.Kind is ConfigurationOperationKind.WriteInputBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding?context.ConfigurationSubscriptions.Bind(query.Destination,r.Kind==ConfigurationOperationKind.ClearParameterBinding?null:r.SubscriptionBinding):context.ConfigurationSubscriptions.Read(query.Destination);if(result.Status!=ConfigurationSubscriptionStatus.Available||!state!.ApplySubscription(r.Source,result,parameter))return new(true,Error:result.Diagnostic??"Subscription rejected.");return new(true,result.DisplayText??"");}
  var dispatch=new Dispatcher(r=>{
   if(r.Kind is ConfigurationOperationKind.ReadInputBinding or ConfigurationOperationKind.WriteInputBinding or ConfigurationOperationKind.ReadParameterBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding)return Subscription(r);
   if(r.Kind==ConfigurationOperationKind.ReadParameter&&!Descriptor.Parameters.Any(p=>p.Name==r.Source.Name))return new(true,r.Source.Value);
   if(r.Kind is ConfigurationOperationKind.SaveCalibration or ConfigurationOperationKind.SaveFile){Save();return new(true);}
   if(r.Kind==ConfigurationOperationKind.WriteParameter&&r.Source.Name=="CalibPathName"){
    var path=r.Value??"";try{if(path.Length==0||!Path.GetExtension(path).Equals(".iwcal",StringComparison.Ordinal)||!Directory.Exists(Path.GetDirectoryName(Path.GetFullPath(path,context.BaseDirectory))))return new(true,Error:".iwcal 输出路径的父目录必须存在。");var result=adapter.Dispatch(r);return result;}catch(Exception error)when(error is IOException or UnauthorizedAccessException or ArgumentException){return new(true,Error:error.Message);}
   }
   if(r.Kind==ConfigurationOperationKind.Run){if(Host().Loading||Host().SolutionBusy||!Host().Ready||!Host().RunEnabled)return new(true,Error:"Execution host is not ready.");_=execution.RunOnceAsync();return new(true);}
   if(r.Kind==ConfigurationOperationKind.RunContinuous){var rejected=execution.StartContinuous();return new(true,Error:rejected==ConfigurationRunRejection.None?null:rejected.ToString());}
   if(r.Kind==ConfigurationOperationKind.StopContinuous){_=execution.StopAsync();return new(true);}
   if(r.Kind==ConfigurationOperationKind.Close){session?.RequestClose();return new(true);}
   if(r.Kind==ConfigurationOperationKind.ResetParameters)return Reset();
   return adapter.Dispatch(r);
  });
  state=new(definition,dispatch){ExecutionEnabled=Host().RunEnabled};state.RegisterRule("SetCombinationForcePropertyOperation",new SourceIgnoredCombinationForceRule());var controls=new ToolConfigurationControlRegistry();ToolConfigurationStandardControls.Register(controls,text,()=>new(execution.IsContinuous,Host().ProcedureBusy,Host().RunEnabled),"Chinese",context.ConfigurationSubscriptions);ToolConfigurationSubscriptionControls.Register(controls,context.ConfigurationSubscriptions,text);
  var objects=new ToolDisplayConfigurationObjectService(()=>ToolDisplayService.Read(ReadValues(),DisplayDefinition),settings=>{SyncValues();var next=new Dictionary<string,JsonElement>(values){[ToolDisplayService.SettingsParameter]=ToolDisplayService.Serialize(settings)};Apply(next);return true;});
  ToolConfigurationObjectControls.Register(controls,objects,text,colorPicker:(initial,selected)=>ToolConfigurationObjectControls.CreateColorTabs(initial,color=>selected(color.Length==9?"#"+color[3..]:color),callback=>{var palette=new VisionUi.SourceColorPalette();palette.Selected+=callback;return palette;},(color,callback)=>{var picker=new VisionUi.SourceAdvancedColorPicker{SelectedCurrentColor=new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color))};picker.ColorChanged+=brush=>callback(brush.ToString());return picker;},new Uri("/VisionUi;component/SourceColorPickerStyles.xaml",UriKind.Relative),text));
  ToolConfigurationImageObjectControls.Register(controls,new ToolDisplayObjectStore(DisplayObjectDefinitions,ReadValues,Apply),text);
  CameraMappingPointPairControls.Register(controls,context.ConfigurationSubscriptions,text,c=>{owner=c.Owner;},(name,visible)=>{
   SyncValues();var next=new Dictionary<string,JsonElement>(values){[name]=J(name+(visible?"#1":"#0"))};try{Apply(next);state.SetProgrammatic(name,"CurValue",name+(visible?"#1":"#0"));state.SetProgrammatic(name,"CustomVisible",visible.ToString());return true;}catch(Exception error){Notify(error.Message,true);return false;}
  },Notify,message=>notifications.Notify(owner(),new(ConfigurationNotificationKind.Warning,message,"相机映射",context.Mode)));
  ToolConfigurationCalibrationPathControls.Register(controls,c=>{owner=c.Owner;var result=files.Select(owner(),new(Id,c.Node.Value,".iwcal|*.iwcal",text("Set Calibration File Path"),CheckFileExists:false));if(result.Status==ConfigurationFileStatus.Selected){state.CommitUser(c.Node,result.Paths.Single(),true);if(c.Node.Value==result.Paths.Single())files.Remember(Id,c.Node.Value);}},text,context.ConfigurationSubscriptions,"CameraMappingChooseCalibrationPath");
  controls.Register("SaveFileDialog",c=>CameraMappingPointPairControls.Command(c,text("Create Calibration File"),()=>{owner=c.Owner;Save();}));
  long revision=0;void Changed(){state.IsBusy=execution.IsRunning;state.SetProgrammatic("CalibPathName","ExecutionRevision",(++revision).ToString());}execution.Changed+=Changed;
  session=new(state,controls,"Chinese",text,()=>{execution.Changed-=Changed;_=execution.DisposeAsync();});state.Initialize();for(int i=1;i<=8;i++)state.SetProgrammatic("Data"+i,"CustomVisible",values["Data"+i].GetString()!.EndsWith("#1",StringComparison.Ordinal).ToString());return session;
 }
 // Installed parser has no such operation type. AlgorithmTabRoot loads XML with
 // IgnoreUnknown=true, and all sixteen source trigger setters keep Operation=null.
 sealed class SourceIgnoredCombinationForceRule:IToolConfigurationRule {public void Apply(ToolConfigurationState state,ConfigurationOperationRequest request){}public void Revert(ToolConfigurationState state,ConfigurationOperationRequest request){}}
 sealed class Dispatcher(Func<ConfigurationOperationRequest,ConfigurationOperationResult> run):IToolConfigurationDispatcher {public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest r)=>run(r);}
}
