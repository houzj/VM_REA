using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows;
using VisionFramework;
using VisionPluginContracts;

namespace VisionPlugins.Calibration;

public sealed partial class NPointPlugin:IToolConfigurationProvider,IToolDisplayProvider,IToolPointInputAliasProvider {
 public IReadOnlyList<ToolPointInputAlias> PointInputAliases {get;}=new[]{new ToolPointInputAlias("Image Point","ImagePointX","ImagePointY"),new("Physical Point","PhysicalPointX","PhysicalPointY")};
 public static IReadOnlyList<ToolDisplayObjectDefinition> DisplayObjectDefinitions {get;}=LoadDisplayObjects();
 static IReadOnlyList<ToolDisplayObjectDefinition> LoadDisplayObjects(){using var stream=typeof(NPointPlugin).Assembly.GetManifestResourceStream("VisionPlugins.Calibration.Resources.NPointDisplay.xml")??throw new InvalidDataException("Missing N-point display model.");using var reader=new StreamReader(stream);return ToolDisplayObjectDefinition.Parse(reader.ReadToEnd());}
 public ToolDisplayDefinition DisplayDefinition {get;}=new(
  "ModuStatus:{0},CalibStatus{1},CalibErrStatus:{2},CalibProgress:{3}/{4}",
  ["ModuStatus","CalibStatus","CalibErrStatus","CalibCurIndex","CalibTotalNum"],
  ["ModuStatus","CalibStatus","CalibErrStatus"]);
 public ToolDisplayFrame? LastDisplay(NodeDocument node,string baseDirectory,ModuleExecutionScope? scope){
  var raw=GetState(node.Id,baseDirectory,scope);lock(raw.Gate)return raw.DisplayOutputs is null?null:ToolDisplayService.Build(DisplayDefinition,ToolDisplayService.Read(node.Parameters,DisplayDefinition),raw.DisplayInputs??[],raw.DisplayOutputs,diagnostic:raw.DisplayDiagnostic) with{Graphics=ToolDisplayGraphics.Build(DisplayObjectDefinitions,node.Parameters,raw.DisplayInputs??[],raw.DisplayOutputs)};
 }
 public ToolConfigurationSession CreateConfiguration(ToolEditorContext context){
  const string language="Chinese";var text=ConfigurationLocalization.ForLanguage(language).Get;
  string Resource(string name){using var stream=typeof(NPointPlugin).Assembly.GetManifestResourceStream("VisionPlugins.Calibration.Resources."+name)??throw new InvalidDataException("Missing N-point model: "+name);using var reader=new StreamReader(stream);return reader.ReadToEnd();}
  var definition=ToolConfigurationDefinition.Parse(Resource("NPointAlgorithmTab.xml"),Resource("ConfigurationGroups.xml"));
  var values=Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());foreach(var pair in context.Node.Parameters)values[pair.Key]=pair.Value.Clone();
  ToolEditorContext Current()=>context with{Node=context.Node with{Parameters=values}};
  void Apply(Dictionary<string,JsonElement> next){context.Apply(next.ToDictionary(p=>p.Key,p=>p.Value.Clone()));values=next;UpdateConfiguration(Current(),values);}
  UpdateConfiguration(Current(),values);
  ToolConfigurationState? state=null;ToolConfigurationSession? session=null;Func<Window?> owner=()=>null;
  var files=context.ConfigurationFiles??new WpfConfigurationFileService();var saveFiles=context.ConfigurationSaveFiles??new WpfConfigurationSaveFileService();var notifications=context.ConfigurationNotifications??new WpfConfigurationNotificationService();
  void Notify(string message,bool error=false){var result=notifications.Notify(owner(),new(error?ConfigurationNotificationKind.Error:ConfigurationNotificationKind.Success,message,"N点标定",context.Mode));if(result.Status==ConfigurationNotificationStatus.Failed)state?.ReportDiagnostic(new("NotificationService","Notify",null,result.Diagnostic??"Notification failed."));}
  ConfigurationExecutionHostStatus Host()=>context.ConfigurationExecutionStatus?.Invoke()??new(false,false,false,false,false);
  var confirmations=context.ConfigurationConfirmations??new WpfConfigurationConfirmationService();
  var execution=new ToolConfigurationExecution(()=>context.Run(values.ToDictionary(p=>p.Key,p=>p.Value.Clone())),()=>Host().SolutionBusy,()=>Host().Ready&&Host().RunEnabled,()=>Host().Loading,()=>Host().ProcedureBusy);
  Dictionary<string,string> User()=>JsonSerializer.Deserialize<Dictionary<string,string>>(values["ConfigurationUserData"].GetString()??"{}")??[];
  var adapter=new ToolConfigurationParameterAdapter(Descriptor,()=>values,Apply,key=>User().GetValueOrDefault(key),(key,value)=>{var user=User();user[key]=value;var next=new Dictionary<string,JsonElement>(values){["ConfigurationUserData"]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(user))};Apply(next);});
  void Refresh(){if(state?.Find("CalibPathName") is {} path)state.RefreshParameter(path);}
  void SaveCalibration(){
   var result=saveFiles.SelectSave(owner(),new(Id,values["CalibPathName"].GetString()??"",".xml|*.xml",text("Create Calibration File"),".xml"));
   if(result.Status==ConfigurationFileStatus.Cancelled)return;
   if(result.Status!=ConfigurationFileStatus.Selected){Notify(result.Diagnostic??result.Status.ToString(),true);return;}
   try{
    string path=result.Paths.Single();if(path.Length>=260)throw new InvalidDataException(text("Lang_FilePathTooLong"));
    // IUserStringData[SaveCalibPath] invokes module export; it is not a UI CurValue write.
    Export(Current(),path);var user=User();user["SaveCalibPath"]=path;
    var next=new Dictionary<string,JsonElement>(values){["CalibPathName"]=JsonSerializer.SerializeToElement(path),["ConfigurationUserData"]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(user))};Apply(next);Refresh();
    var remembered=saveFiles.Remember(Id,path);if(!remembered.Success)state?.ReportDiagnostic(new("FileHistory","Remember",null,remembered.Diagnostic??"History failed."));Notify(text("Lang_SaveSuccess"));
   }catch(Exception error)when(error is IOException or InvalidOperationException or ArgumentException or UnauthorizedAccessException){Notify(error.Message,true);}
  }
  bool ClearPoints(){try{
   var next=new Dictionary<string,JsonElement>(values);for(int i=0;i<32;i++)foreach(var key in PointKeys)next[key+i]=JsonSerializer.SerializeToElement(0d);next["ClearRevision"]=JsonSerializer.SerializeToElement(checked(values["ClearRevision"].GetInt32()+1));Apply(next);Clear(Current());Notify(text("Lang_ClearCalibPointSuc"));return true;
  }catch(Exception error){Notify(error.Message,true);return false;}}
  ConfigurationOperationResult Subscription(ConfigurationOperationRequest request){
   var parameter=request.Kind is ConfigurationOperationKind.ReadParameterBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding;
   var query=ConfigurationSubscriptionValidation.FromElement(request.Source.Element,parameter?ConfigurationSubscriptionDomain.Parameter:null);
   if(query is null||context.ConfigurationSubscriptions is null)return new(true,Error:"Source-declared subscription service is unavailable.");
   bool write=request.Kind is ConfigurationOperationKind.WriteInputBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding;
   var result=write?context.ConfigurationSubscriptions.Bind(query.Destination,request.Kind==ConfigurationOperationKind.ClearParameterBinding?null:request.SubscriptionBinding):context.ConfigurationSubscriptions.Read(query.Destination);
   if(result.Status!=ConfigurationSubscriptionStatus.Available)return new(true,Error:result.Diagnostic??result.Status.ToString());
   if(state is not null&&!state.ApplySubscription(request.Source,result,parameter))return new(true,Error:"Subscription identity/presentation was not confirmed.");return new(true,result.DisplayText??"");
  }
  var dispatch=new NPointConfigurationDispatcher(request=>{
   if(request.Kind is ConfigurationOperationKind.ReadInputBinding or ConfigurationOperationKind.WriteInputBinding or ConfigurationOperationKind.ReadParameterBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding)return Subscription(request);
   if(request.Kind==ConfigurationOperationKind.ReadParameter){
    var name=(request.Source.Name??"")+(request.Source.Get("ComTailName")??"");if(!Descriptor.Parameters.Any(p=>p.Name==name))return new(true,request.Source.Value);
   }
   if(request.Kind==ConfigurationOperationKind.WriteParameter&&request.Source.Name=="CalibPathName"){
    var path=request.Value??"";if(path.Length>0&&(!Path.GetExtension(path).Equals(".xml",StringComparison.OrdinalIgnoreCase)||!Directory.Exists(Path.GetDirectoryName(Path.GetFullPath(path,context.BaseDirectory)))))return new(true,Error:"Calibration path must have .xml extension and an existing parent directory.");
    var result=adapter.Dispatch(request);if(result.Error is null)Notify(text("Lang_SetSuc"));return result;
   }
   if(request.Kind is ConfigurationOperationKind.SaveCalibration or ConfigurationOperationKind.SaveFile){SaveCalibration();return new(true);}
   if(request.Kind==ConfigurationOperationKind.ClearCalibrationPoints)return new(true,Error:ClearPoints()?null:"Clear points was not accepted.");
   if(request.Kind==ConfigurationOperationKind.Run){if(Host().Loading||Host().SolutionBusy||!Host().Ready||!Host().RunEnabled)return new(true,Error:"Configuration execution is not ready.");_=execution.RunOnceAsync();return new(true);}
   if(request.Kind==ConfigurationOperationKind.RunContinuous){var rejection=execution.StartContinuous();return new(true,Error:rejection==ConfigurationRunRejection.None?null:rejection.ToString());}
   if(request.Kind==ConfigurationOperationKind.StopContinuous){_=execution.StopAsync();return new(true);}
   if(request.Kind==ConfigurationOperationKind.Close){session?.RequestClose();return new(true);}
   if(request.Kind==ConfigurationOperationKind.ResetParameters){
    if(!confirmations.Confirm(owner(),new(text("Lang_Sure"),text("Lang_ResetAlgoParamTips")+"?")))return new(true);
    if(context.ResetConfigurationParameters is null)return new(true,Error:"Atomic parameter reset service is unavailable.");
    var next=new Dictionary<string,JsonElement>(values);foreach(var p in Descriptor.Parameters.Where(p=>p.Name!="ConfigurationUserData"&&p.Name!=ToolDisplayService.SettingsParameter&&p.Name!=ToolConfigurationDisplaySubscriptions.ParameterName&&p.Name!=ToolDisplayObjectStore.ParameterName&&p.Name!="ClearRevision"))next[p.Name]=p.DefaultValue!.Value.Clone();context.ResetConfigurationParameters(next.ToDictionary(p=>p.Key,p=>p.Value.Clone()));values=next;UpdateConfiguration(Current(),values);foreach(var node in state!.Nodes)if(node.Name is {} name&&next.TryGetValue(name,out var value))state.RefreshParameter(node);Notify(text("Lang_ResetAlgoParamSuccess"));return new(true);
   }
   return adapter.Dispatch(request);
  });
  state=new(definition,dispatch){ExecutionEnabled=Host().RunEnabled};state.RegisterRule("HidePhysicInfoOperation",new NPointPhysicalVisibilityRule());
  var controls=new ToolConfigurationControlRegistry();ToolConfigurationStandardControls.Register(controls,text,()=>new(execution.IsContinuous,Host().ProcedureBusy,Host().RunEnabled),language,context.ConfigurationSubscriptions);ToolConfigurationSubscriptionControls.Register(controls,context.ConfigurationSubscriptions,text,language);
  var objects=new ToolDisplayConfigurationObjectService(()=>ToolDisplayService.Read(values,DisplayDefinition),settings=>{var next=new Dictionary<string,JsonElement>(values){[ToolDisplayService.SettingsParameter]=ToolDisplayService.Serialize(settings)};Apply(next);return true;},Refresh);
  ToolConfigurationObjectControls.Register(controls,objects,text,colorPicker:(initial,selected)=>{
   void Select(string value)=>selected(value.Length==9?"#"+value[3..]:value);
   return ToolConfigurationObjectControls.CreateColorTabs(initial,Select,callback=>{var palette=new VisionUi.SourceColorPalette();palette.Selected+=callback;return palette;},(color,callback)=>{var advanced=new VisionUi.SourceAdvancedColorPicker{SelectedCurrentColor=new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color))};advanced.ColorChanged+=brush=>callback(brush.ToString());return advanced;},new Uri("/VisionUi;component/SourceColorPickerStyles.xaml",UriKind.Relative),text);
  });
  NPointConfigurationControls.Register(controls,control=>{
   owner=control.Owner;
   var editor=new NPointConfigurationPointEditor(Snapshot(Current()),values["CalibPointTotalNum"].GetInt32()+values["RotPointTotalNum"].GetInt32(),(row,axis,value)=>{
    if(!float.TryParse(value,NumberStyles.Float,CultureInfo.CurrentCulture,out var number)||!float.IsFinite(number))return false;
    var next=PointValues();next[PointKeys[axis]+row]=JsonSerializer.SerializeToElement((double)number);Apply(next);return true;
   },index=>{
    int trans=values["CalibPointTotalNum"].GetInt32(),rot=values["RotPointTotalNum"].GetInt32(),count=trans+rot;if(count<=4)return false;
    var next=PointValues();if(index<trans){if(trans<=4)return false;next["CalibPointTotalNum"]=JsonSerializer.SerializeToElement(trans-1);}else next["RotPointTotalNum"]=JsonSerializer.SerializeToElement(rot-1);
    for(int i=index;i<count-1;i++)foreach(var key in PointKeys)next[key+i]=next[key+(i+1)].Clone();Apply(next);state.SetProgrammatic("CalibPointTotalNum","CurValue",next["CalibPointTotalNum"].ToString());state.SetProgrammatic("RotPointTotalNum","CurValue",next["RotPointTotalNum"].ToString());return true;
   },ClearPoints,()=>{var selected=files.Select(owner(),new(Id,"","txt|*.txt",text("Lang_Import")));return selected.Status==ConfigurationFileStatus.Selected?selected.Paths.Single():null;},()=>{var selected=saveFiles.SelectSave(owner(),new(Id,"","txt|*.txt",text("Lang_Export"),".txt"));return selected.Status==ConfigurationFileStatus.Selected?selected.Paths.Single():null;},Notify);
   editor.CreateWindow(owner()).ShowDialog();
  },control=>{
   owner=control.Owner;var selected=files.Select(owner(),new(Id,control.Node.Value,".xml|*.xml",text("Set Calibration File Path"),CheckFileExists:false));
   if(selected.Status==ConfigurationFileStatus.Selected){state.CommitUser(control.Node,selected.Paths.Single(),true);if(state.Find("CalibPathName")!.Value==selected.Paths.Single())files.Remember(Id,selected.Paths.Single());}
   else if(selected.Status!=ConfigurationFileStatus.Cancelled)Notify(selected.Diagnostic??selected.Status.ToString(),true);
  },text,context.ConfigurationSubscriptions);
  long revision=0;void Changed(){state.IsBusy=execution.IsRunning;state.SetProgrammatic("CalibPathName","ExecutionRevision",(++revision).ToString(CultureInfo.InvariantCulture));}
  execution.Changed+=Changed;session=new(state,controls,language,text,()=>{execution.Changed-=Changed;_=execution.DisposeAsync();});state.Initialize();return session;
  Dictionary<string,JsonElement> PointValues(){var next=new Dictionary<string,JsonElement>(values);var points=Snapshot(Current());for(int i=0;i<32;i++)for(int axis=0;axis<5;axis++)next[PointKeys[axis]+i]=JsonSerializer.SerializeToElement((double)points[i,axis]);return next;}
 }
 static readonly string[] PointKeys=["ImagePointX","ImagePointY","WorldPointX","WorldPointY","DeltaAngle"];
 sealed class NPointConfigurationDispatcher(Func<ConfigurationOperationRequest,ConfigurationOperationResult> run):IToolConfigurationDispatcher {public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest request)=>run(request);}
}
