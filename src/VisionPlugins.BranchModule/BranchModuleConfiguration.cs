using System.Text.Json;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.BranchModule;
internal static class BranchModuleConfiguration {
 public static ToolConfigurationSession Create(ToolEditorContext context,ModuleDescriptor descriptor){
  const string language="Chinese";
  var sourceText=JsonSerializer.Deserialize<Dictionary<string,string>>(BranchModulePlugin.Resource("BranchLocalization.json"))??[];
  string Text(string key)=>sourceText.GetValueOrDefault(key)??ConfigurationLocalization.ForLanguage(language).Get(key);
  var definition=ToolConfigurationDefinition.Parse(BranchModulePlugin.Resource("BranchModule_STDAlgorithmTab.xml"),BranchModulePlugin.Resource("BranchGroups.xml"));
  var values=descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());foreach(var p in context.Node.Parameters)values[p.Key]=p.Value.Clone();
  Dictionary<string,JsonElement> Read(){if(context.ReadEffectiveParameters?.Invoke() is {} effective)foreach(var p in effective)if(values.ContainsKey(p.Key))values[p.Key]=p.Value.Clone();return values;}
  void Apply(Dictionary<string,JsonElement> next){context.Apply(next.ToDictionary(p=>p.Key,p=>p.Value.Clone()));values=next;}
  var adapter=new ToolConfigurationParameterAdapter(descriptor,Read,Apply);
  ConfigurationExecutionHostStatus Host()=>context.ConfigurationExecutionStatus?.Invoke()??new(false,false,false,false,false);
  var execution=new ToolConfigurationExecution(()=>context.Run(Read().ToDictionary(p=>p.Key,p=>p.Value.Clone())),()=>Host().SolutionBusy,()=>Host().Ready&&Host().RunEnabled,()=>Host().Loading,()=>Host().ProcedureBusy);
  ToolConfigurationState? state=null;ToolConfigurationSession? session=null;
  var dispatcher=new Dispatcher(request=>{
   if(request.Kind==ConfigurationOperationKind.WriteParameter&&request.Source.Name=="ModuleInfoList"){var next=Read().ToDictionary(p=>p.Key,p=>p.Value.Clone());next["ModuleInfoList"]=JsonSerializer.SerializeToElement(BranchRouteTable.Serialize(BranchRouteTable.Parse(request.Value)));Apply(next);return new(true,request.Value??"");}
   if(request.Kind==ConfigurationOperationKind.EnumerateInputs){var query=ConfigurationSubscriptionValidation.FromElement(request.Source.Element);if(query is null||context.ConfigurationSubscriptions is null)return new(true,Error:"Source-declared integer subscription service is unavailable.");var tree=context.ConfigurationSubscriptions.Enumerate(query);return new(true,Error:tree.Status==ConfigurationSubscriptionStatus.Available?null:tree.Diagnostic??tree.Status.ToString()){SubscriptionTree=tree};}
   if(request.Kind is ConfigurationOperationKind.ReadInputBinding or ConfigurationOperationKind.WriteInputBinding){var query=ConfigurationSubscriptionValidation.FromElement(request.Source.Element);if(query is null||context.ConfigurationSubscriptions is null)return new(true,Error:"Source-declared integer subscription service is unavailable.");var result=request.Kind==ConfigurationOperationKind.WriteInputBinding?context.ConfigurationSubscriptions.Bind(query.Destination,request.SubscriptionBinding):context.ConfigurationSubscriptions.Read(query.Destination);if(result.Status!=ConfigurationSubscriptionStatus.Available)return new(true,Error:result.Diagnostic??result.Status.ToString());if(!state!.ApplySubscription(request.Source,result,false))return new(true,Error:"Subscription identity/presentation was not confirmed.");return new(true,result.DisplayText??"");}
   switch(request.Kind){
    case ConfigurationOperationKind.Run:_=execution.RunOnceAsync();return new(true);
    case ConfigurationOperationKind.RunContinuous:var rejected=execution.StartContinuous();return new(true,Error:rejected==ConfigurationRunRejection.None?null:rejected.ToString());
    case ConfigurationOperationKind.StopContinuous:_=execution.StopAsync();return new(true);
    case ConfigurationOperationKind.Close:session?.RequestClose();return new(true);
    default:return adapter.Dispatch(request);
   }
  });
  state=new(definition,dispatcher){ExecutionEnabled=Host().RunEnabled};
  var controls=new ToolConfigurationControlRegistry();ToolConfigurationStandardControls.Register(controls,Text,()=>new(execution.IsContinuous,Host().ProcedureBusy,Host().RunEnabled),language,context.ConfigurationSubscriptions);ToolConfigurationSubscriptionControls.Register(controls,context.ConfigurationSubscriptions,Text,language);BranchConfigurationControls.Register(controls,Text);
  void Changed()=>state.IsBusy=execution.IsRunning;
  execution.Changed+=Changed;session=new(state,controls,language,Text,()=>{execution.Changed-=Changed;_=execution.DisposeAsync();});state.Initialize();return session;
 }
 sealed class Dispatcher(Func<ConfigurationOperationRequest,ConfigurationOperationResult> invoke):IToolConfigurationDispatcher{public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest request)=>invoke(request);}
}

