using System.Text.Json;
using System.Windows;
using VisionFramework;
using VisionPluginContracts;
using Button=System.Windows.Controls.Button;
using Grid=System.Windows.Controls.Grid;
using HorizontalAlignment=System.Windows.HorizontalAlignment;
namespace VisionPlugins.Fixture;
public sealed partial class FixturePlugin {
 public ToolConfigurationSession CreateConfiguration(ToolEditorContext context){
  const string language="Chinese";var words=JsonSerializer.Deserialize<Dictionary<string,string>>(Resource("FixtureLocalization.json"))??[];string Text(string key)=>words.GetValueOrDefault(key)??ConfigurationLocalization.ForLanguage(language).Get(key);
  var values=Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());foreach(var pair in context.Node.Parameters)values[pair.Key]=pair.Value.Clone();
  var memory=Get(context.Node.Id,context.BaseDirectory,context.Scope);
  Dictionary<string,JsonElement> Read(){lock(memory.Gate){if(context.ReadEffectiveParameters?.Invoke() is {} effective)foreach(var pair in effective)values[pair.Key]=pair.Value.Clone();Sync(memory,values);return Snapshot(memory,values);}}
  void Apply(Dictionary<string,JsonElement> next){context.Apply(next.ToDictionary(p=>p.Key,p=>p.Value.Clone()));lock(memory.Gate){values=next;foreach(string name in Fields){Set(memory.Core,name,next[name].GetDouble());memory.Seed[name]=next[name].Clone();}}}
  Dictionary<string,string> User()=>JsonSerializer.Deserialize<Dictionary<string,string>>(Read()["ConfigurationUserData"].GetString()??"{}")??[];
  var adapter=new ToolConfigurationParameterAdapter(Descriptor,Read,Apply,key=>User().GetValueOrDefault(key),(key,value)=>{var user=User();user[key]=value;var next=Read();next["ConfigurationUserData"]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(user));Apply(next);});
  var definition=ToolConfigurationDefinition.Parse(Resource("IMVSFixtureModuAlgorithmTab.xml"),Resource("ConfigurationGroups.xml"));
  ToolConfigurationState? state=null;ToolConfigurationSession? session=null;Func<Window?> owner=()=>null;
  var notifications=context.ConfigurationNotifications??new WpfConfigurationNotificationService();
  void Notify(string message,bool error=false)=>notifications.Notify(owner(),new(error?ConfigurationNotificationKind.Error:ConfigurationNotificationKind.Success,message,"位置修正",context.Mode));
  ConfigurationExecutionHostStatus Host()=>context.ConfigurationExecutionStatus?.Invoke()??new(false,false,false,false,false);
  var execution=new ToolConfigurationExecution(()=>context.Run(Read()),()=>Host().SolutionBusy,()=>Host().Ready&&Host().RunEnabled,()=>Host().Loading,()=>Host().ProcedureBusy);
  void Teach(){var confirmations=context.ConfigurationConfirmations??new WpfConfigurationConfirmationService();if(state!.Find("FixtureInit")!.Value=="1"&&!confirmations.Confirm(owner(),new(Text("Lang_InfoTips"),Text("Lang_FdPExitedIfNew"))))return;
   if(!Host().Ready){Notify(Text("基准点创建失败")+" , "+Text("Tab_Params")+Text("未配置")+" !",true);state.SetProgrammatic("FixtureInit","CurValue","0");return;}
   uint result;FixturePoseFields oldReference;int oldInit;lock(memory.Gate){Read();oldReference=memory.Core.Reference;oldInit=memory.Core.InitFlag;result=memory.Core.CreateReference();}
   if(result==0){var next=Read();try{Apply(next);}catch(Exception error){lock(memory.Gate){memory.Core.Reference=oldReference;memory.Core.InitFlag=oldInit;}Notify(error.Message,true);return;}Notify(Text("Lang_FdPCreateSuc"));state.SetProgrammatic("FixtureInit","CurValue","1");}
   else{if(result==0xa0000002)Notify(Text("Lang_FdPCreateErr"),true);state.SetProgrammatic("FixtureInit","CurValue","0");}
  }
  var dispatch=new FixtureDispatcher(r=>{
   if(r.Kind==ConfigurationOperationKind.ReadParameter&&r.Source.Name=="FixtureInit"){lock(memory.Gate){Read();return new(true,memory.Core.InitFlag.ToString(System.Globalization.CultureInfo.InvariantCulture));}}
   if(r.Kind==ConfigurationOperationKind.ReadParameter&&!Descriptor.Parameters.Any(p=>p.Name==r.Source.Name))return new(true,r.Source.Value);
   if(r.Kind==ConfigurationOperationKind.EnumerateInputs){var query=ConfigurationSubscriptionValidation.FromElement(r.Source.Element);if(query is null||context.ConfigurationSubscriptions is null)return new(true,Error:"Input subscription service is unavailable.");var tree=context.ConfigurationSubscriptions.Enumerate(query);return new(true,Error:tree.Status==ConfigurationSubscriptionStatus.Available?null:tree.Diagnostic??tree.Status.ToString()){SubscriptionTree=tree};}
   if(r.Kind is ConfigurationOperationKind.ReadInputBinding or ConfigurationOperationKind.WriteInputBinding){var query=ConfigurationSubscriptionValidation.FromElement(r.Source.Element);if(query is null||context.ConfigurationSubscriptions is null)return new(true,Error:"Input subscription service is unavailable.");var result=r.Kind==ConfigurationOperationKind.ReadInputBinding?context.ConfigurationSubscriptions.Read(query.Destination):context.ConfigurationSubscriptions.Bind(query.Destination,r.SubscriptionBinding);if(result.Status!=ConfigurationSubscriptionStatus.Available||!state!.ApplySubscription(r.Source,result,false))return new(true,Error:result.Diagnostic??"Input source was rejected.");return new(true,result.DisplayText??"");}
   if(r.Kind==ConfigurationOperationKind.Run){_=execution.RunOnceAsync();return new(true);}
   if(r.Kind==ConfigurationOperationKind.RunContinuous){var rejected=execution.StartContinuous();return new(true,Error:rejected==ConfigurationRunRejection.None?null:rejected.ToString());}
   if(r.Kind==ConfigurationOperationKind.StopContinuous){_=execution.StopAsync();return new(true);}
   if(r.Kind==ConfigurationOperationKind.Close){session?.RequestClose();return new(true);}
   return adapter.Dispatch(r);
  });
  state=new(definition,dispatch){ExecutionEnabled=Host().RunEnabled};var controls=new ToolConfigurationControlRegistry();ToolConfigurationStandardControls.Register(controls,Text,()=>new(execution.IsContinuous,Host().ProcedureBusy,Host().RunEnabled),language,context.ConfigurationSubscriptions);ToolConfigurationSubscriptionControls.Register(controls,context.ConfigurationSubscriptions,Text,language);
  controls.Register("Command_FixtureModule",c=>{owner=c.Owner;var panel=new Grid{Height=32};panel.ColumnDefinitions.Add(new(){Width=new GridLength(89)});panel.ColumnDefinitions.Add(new());var button=new Button{Content=Text(c.Node.Get("DisplayName")??"create Reference"),HorizontalAlignment=HorizontalAlignment.Left};Grid.SetColumn(button,1);panel.Children.Add(button);button.Click+=(_,_)=>Teach();return panel;});
  ToolConfigurationImageObjectControls.Register(controls,new ToolDisplayObjectStore(DisplayObjects,Read,Apply),Text);
  long revision=0;void Changed(){state.IsBusy=execution.IsRunning;state.SetProgrammatic("FixtureInit","ExecutionRevision",(++revision).ToString());state.RefreshParameter(state.Find("FixtureInit")!);}execution.Changed+=Changed;
  session=new(state,controls,language,Text,()=>{execution.Changed-=Changed;_=execution.DisposeAsync();});state.Initialize();return session;
 }
 sealed class FixtureDispatcher(Func<ConfigurationOperationRequest,ConfigurationOperationResult> run):IToolConfigurationDispatcher{public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest request)=>run(request);}
}



