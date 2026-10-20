using System.Text.Json;
using System.Windows;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.ContourMatch;

public sealed partial class ContourMatchPlugin
{
 public ToolConfigurationSession CreateConfiguration(ToolEditorContext context){
  const string language="Chinese";var text=ConfigurationLocalization.ForLanguage(language).Get;
  var definition=ToolConfigurationDefinition.Parse(Resource("IMVSContourMatchModuAlgorithmTab.xml"),Resource("ConfigurationGroups.xml"));
  var values=Descriptor.Parameters.Where(p=>p.DefaultValue.HasValue).ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());foreach(var p in context.Node.Parameters)values[p.Key]=p.Value.Clone();
  void Synchronize(){if(context.ReadEffectiveParameters?.Invoke() is {} effective)foreach(var p in effective)if(values.ContainsKey(p.Key))values[p.Key]=p.Value.Clone();}
  Dictionary<string,JsonElement> ReadValues(){Synchronize();return values;}
  void Apply(Dictionary<string,JsonElement> next){context.Apply(next.ToDictionary(p=>p.Key,p=>p.Value.Clone()));values=next;}
  Dictionary<string,string> User()=>JsonSerializer.Deserialize<Dictionary<string,string>>(ReadValues()["ConfigurationUserData"].GetString()??"{}")??[];
  var adapter=new ToolConfigurationParameterAdapter(Descriptor,ReadValues,Apply,k=>User().GetValueOrDefault(k),(k,v)=>{var u=User();u[k]=v;var next=new Dictionary<string,JsonElement>(values){["ConfigurationUserData"]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(u))};Apply(next);});
  ConfigurationExecutionHostStatus Host()=>context.ConfigurationExecutionStatus?.Invoke()??new(false,false,false,false,false);
  var execution=new ToolConfigurationExecution(()=>context.Run(ReadValues().ToDictionary(p=>p.Key,p=>p.Value.Clone())),()=>Host().SolutionBusy,()=>Host().Ready&&Host().RunEnabled,()=>Host().Loading,()=>Host().ProcedureBusy);
  ToolConfigurationState? state=null;ToolConfigurationSession? session=null;IToolConfigurationRoiRegistration? roi=null;
  var confirmations=context.ConfigurationConfirmations??new WpfConfigurationConfirmationService();
  var notifications=context.ConfigurationNotifications??new WpfConfigurationNotificationService();
  void Notify(string message,bool error)=>notifications.Notify(null,new(error?ConfigurationNotificationKind.Error:ConfigurationNotificationKind.Success,message,"轮廓匹配",context.Mode));
  ConfigurationOperationResult Subscription(ConfigurationOperationRequest request){
   bool parameter=request.Kind is ConfigurationOperationKind.ReadParameterBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding;
   var query=ConfigurationSubscriptionValidation.FromElement(request.Source.Element,parameter?ConfigurationSubscriptionDomain.Parameter:null);
   if(query is null||context.ConfigurationSubscriptions is null)return new(true,Error:"Source-declared subscription service is unavailable.");
   bool write=request.Kind is ConfigurationOperationKind.WriteInputBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding;
   var result=write?context.ConfigurationSubscriptions.Bind(query.Destination,request.Kind==ConfigurationOperationKind.ClearParameterBinding?null:request.SubscriptionBinding):context.ConfigurationSubscriptions.Read(query.Destination);
   if(result.Status!=ConfigurationSubscriptionStatus.Available)return new(true,Error:result.Diagnostic??result.Status.ToString());
   if(state is not null&&!state.ApplySubscription(request.Source,result,parameter))return new(true,Error:"Subscription identity/presentation was not confirmed.");
   return new(true,result.DisplayText??"");
  }
  var dispatch=new Dispatcher(r=>{
   if(r.Kind==ConfigurationOperationKind.EnumerateInputs){
    var query=ConfigurationSubscriptionValidation.FromElement(r.Source.Element);
    if(query is null||context.ConfigurationSubscriptions is null)return new(true,Error:"Source-declared subscription service is unavailable.");
    var tree=context.ConfigurationSubscriptions.Enumerate(query);
    return new(true,Error:tree.Status==ConfigurationSubscriptionStatus.Available?null:tree.Diagnostic??tree.Status.ToString()){SubscriptionTree=tree};
   }
   if(r.Kind is ConfigurationOperationKind.ReadInputBinding or ConfigurationOperationKind.WriteInputBinding or ConfigurationOperationKind.ReadParameterBinding or ConfigurationOperationKind.WriteParameterBinding or ConfigurationOperationKind.ClearParameterBinding)return Subscription(r);
   switch(r.Kind){
    case ConfigurationOperationKind.RefreshRoiVisibility:roi?.Refresh();return new(true);
    case ConfigurationOperationKind.ResetParameters:
     if(!confirmations.Confirm(null,new(text("Lang_Sure"),text("Lang_ResetAlgoParamTips")+"?")))return new(true);
     var reset=ToolConfigurationAlgorithmReset.Execute(Descriptor,state!,adapter,context.ConfigurationSubscriptions,Resource("IMVSContourMatchModuAlgorithm.xml"),Synchronize);
     Notify(reset.Error??text("Lang_ResetAlgoParamSuccess"),reset.Error is not null);return reset;
    case ConfigurationOperationKind.Run:
     if(Host().Loading||Host().SolutionBusy||!Host().Ready||!Host().RunEnabled)return new(true,Error:"Configuration execution is not ready.");
     _=execution.RunOnceAsync();return new(true);
    case ConfigurationOperationKind.RunContinuous:var rejection=execution.StartContinuous();return new(true,Error:rejection==ConfigurationRunRejection.None?null:rejection.ToString());
    case ConfigurationOperationKind.StopContinuous:_=execution.StopAsync();return new(true);
    case ConfigurationOperationKind.Close:session?.RequestClose();return new(true);
    default:return adapter.Dispatch(r);
   }
  });
  state=new(definition,dispatch){ExecutionEnabled=Host().RunEnabled};
  // Current original override returns GetType().BaseType.Name.
  state.RegisterDisplayTypeResolver("PyramidFeaturepatMatchConfigPanalData",_=>"FeaturepatMatchConfigPanalData");
  var controls=new ToolConfigurationControlRegistry();
  var models=ContourModelCollection.Load(ReadValues()[ContourModelCollection.ParameterName].GetString()??"");
  void PersistModels(){
   string json=models.Save();var next=new Dictionary<string,JsonElement>(ReadValues()){[ContourModelCollection.ParameterName]=JsonSerializer.SerializeToElement(json)};Apply(next);
   if(ReadValues()[ContourModelCollection.ParameterName].GetString()!=json)throw new InvalidOperationException("Host did not persist contour model state.");
  }
  ContourModelControls.Register(controls,models,()=>execution.IsRunning,PersistModels,
   message=>confirmations.Confirm(null,new(text("Lang_InfoTips"),message)),message=>Notify(message,true),
   ()=>throw new NotSupportedException("Contour training kernel and dedicated training dialog are not implemented."),
   ()=>throw new NotSupportedException("Contour model file import is not implemented; this operation does not accept raw model bytes."),
   _=>throw new NotSupportedException("Contour model re-training dialog is not implemented."));
  ToolConfigurationStandardControls.Register(controls,text,()=>new(execution.IsContinuous,Host().ProcedureBusy,Host().RunEnabled),language,context.ConfigurationSubscriptions);
  ToolConfigurationSubscriptionControls.Register(controls,context.ConfigurationSubscriptions,text,language);
  roi=ToolConfigurationRoiControls.RegisterRefreshable(controls,context,new([RoiKind.Rectangle,RoiKind.Polygon],Maximum:1,FullImage:true,Exclusions:false),_=>"InputImage",text);
  var objects=new ToolDisplayConfigurationObjectService(()=>ToolDisplayService.Read(ReadValues(),DisplayDefinition),settings=>{var next=new Dictionary<string,JsonElement>(values){[ToolDisplayService.SettingsParameter]=ToolDisplayService.Serialize(settings)};Apply(next);return true;},()=>roi.Refresh());
  ToolConfigurationObjectControls.Register(controls,objects,text);
  void Changed(){state.IsBusy=execution.IsRunning;roi.Refresh();}
  execution.Changed+=Changed;
  session=new(state,controls,language,text,()=>{execution.Changed-=Changed;roi.Dispose();_=execution.DisposeAsync();});
  state.Initialize();return session;
 }
 sealed class Dispatcher(Func<ConfigurationOperationRequest,ConfigurationOperationResult> run):IToolConfigurationDispatcher{public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest request)=>run(request);}
}


