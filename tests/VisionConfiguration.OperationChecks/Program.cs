using System.Text.Json;
using VisionFramework;
using VisionPluginContracts;
static class Program {
 static int Main(){try{Run();return 0;}catch(Exception error){Console.Error.WriteLine(error);return 1;}}
 static void Run(){
  int checks=0;void Check(bool ok,string message){checks++;if(!ok)throw new Exception(message);}
  var definition=ToolConfigurationDefinition.Parse("<AlgorithmTabRoot><Tabs><Tab Name='Tab_Basic Params'><Categorys><Category Name='group'><Items><Integer Name='count'><AccessMode>RW</AccessMode><MinValue>1</MinValue><MaxValue>10</MaxValue></Integer><RadioSelecter Name='mode'><AccessMode>O</AccessMode><DefaultValue>0</DefaultValue></RadioSelecter><Integer Name='item'><AccessMode>R</AccessMode><ComTailName>0</ComTailName></Integer></Items></Category></Categorys></Tab></Tabs></AlgorithmTabRoot>");
  var descriptor=new ModuleDescriptor("sample","1","sample",[],[],[new("count",ParameterKind.Integer,true,JsonSerializer.SerializeToElement(2),1,10),new("item0",ParameterKind.Integer,true,JsonSerializer.SerializeToElement(4))]);
  var values=descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());
  var userData=new Dictionary<string,string>{{"Param.mode","1"}};bool failWrite=false,noOpWrite=false;int writes=0;
  var adapter=new ToolConfigurationParameterAdapter(descriptor,()=>values,updated=>{if(failWrite)throw new InvalidOperationException("controlled failed write");if(noOpWrite)return;writes++;values=updated;},key=>userData.GetValueOrDefault(key),(key,value)=>userData[key]=value);
  var state=new ToolConfigurationState(definition,adapter);state.Initialize();
  Check(state.Find("count")!.Value=="2","Initialization reads backend parameter rather than UI default");
  Check(state.Find("mode")!.Value=="1"&&!values.ContainsKey("mode"),"Non-readable radio restores separate Param.* user data without algorithm parameter");
  Check(state.Find("item")!.Value=="4","Source ComName includes ComTailName");
  state.CommitUser("count","7");Check(values["count"].GetInt32()==7&&writes==1&&state.Find("count")!.Value=="7","Typed commit reaches backend and reads back committed value");
  state.CommitUser("mode","0");Check(userData["Param.mode"]=="0"&&writes==1&&!values.ContainsKey("Param.mode"),"UI selection persists separately without a module parameter write");
  failWrite=true;state.CommitUser("count","8");Check(values["count"].GetInt32()==7&&state.Find("count")!.Value=="7"&&state.Diagnostics.Any(d=>d.Code=="OperationError"),"Backend failure rolls back the view without corrupting accepted parameter");failWrite=false;
  state.CommitUser("count","11");Check(writes==1&&values["count"].GetInt32()==7,"Source range rejects invalid user value before backend");
  noOpWrite=true;state.CommitUser("count","6");Check(state.Find("count")!.Value=="7"&&values["count"].GetInt32()==7&&state.Diagnostics.Any(d=>d.Message.Contains("did not persist")),"No-op host apply is rejected instead of inventing a committed value");noOpWrite=false;
  var missingReadback=new ToolConfigurationParameterAdapter(descriptor,()=>new Dictionary<string,JsonElement>(),_=>{});
  var missingResult=missingReadback.Dispatch(new(ConfigurationOperationKind.WriteParameter,"WriteParameter",state.Find("count")!,null,[],ConfigurationChangeKind.UserCommit,"3"));
  Check(missingResult.Error?.Contains("readback") == true,"Missing persisted parameter cannot fall back to the requested value");
  var acceptedDefinition=ToolConfigurationDefinition.Parse("<AlgorithmTabRoot><Tabs><Tab Name='basic'><Categorys><Category Name='group'><Items><Float Name='accepted'><AccessMode>RW</AccessMode></Float></Items></Category></Categorys></Tab></Tabs></AlgorithmTabRoot>");
  var acceptedState=new ToolConfigurationState(acceptedDefinition,new ResultDispatcher(r=>r.Kind switch{ConfigurationOperationKind.ReadParameter=>new(true,"1"),ConfigurationOperationKind.ReadUserValue=>new(true),ConfigurationOperationKind.WriteParameter=>new(true,"1.5"),_=>new(false)}));acceptedState.Initialize();acceptedState.CommitUser("accepted","2");
  Check(acceptedState.Find("accepted")!.Value=="1.5","Model consumes an explicitly accepted adapter value rather than retaining the request");
  var router=new ToolConfigurationOperationRouter();bool invoked=false;router.Register(ConfigurationOperationKind.Close,_=>{invoked=true;return new(true);});
  var request=new ConfigurationOperationRequest(ConfigurationOperationKind.Close,"ExcuteMethodOperation",state.Find("count")!,null,["CloseParamsTabControl"],ConfigurationChangeKind.Command);
  Check(router.Dispatch(request).Handled&&invoked,"Only explicitly registered typed command dispatches");
  Check(!router.Dispatch(request with{Kind=ConfigurationOperationKind.Unknown,OriginalName="System.IO.File.Delete"}).Handled,"Unknown XML method cannot enter typed dispatch table");
  bool rejected=false;try{router.Register(ConfigurationOperationKind.Unknown,_=>new(true));}catch(ArgumentException){rejected=true;}Check(rejected,"Unknown operation registration is rejected");
  Check(ConfigurationLocalization.ForLanguage("Chinese").Get("Tab_Basic Params")=="基本参数"&&ConfigurationLocalization.ForLanguage("Chinese").Get("ResultShow")=="结果显示","Source Chinese captions are shared instead of per-plugin handwritten labels");
  Check(ConfigurationLocalization.ForLanguage("English").TryGet("Tab_Basic Params",out var english)&&english.Length>0,"Source English captions are available with language identity");
  Check(!ConfigurationLocalization.ForLanguage("Chinese").TryGet("not-a-native-key",out _)&&ConfigurationLocalization.ForLanguage("Chinese").Get("not-a-native-key")=="not-a-native-key","Unknown caption has an explicit independent fallback");
  Console.WriteLine($"{checks} configuration operation/store/localization checks passed; no devices.");
 }
 sealed class ResultDispatcher(Func<ConfigurationOperationRequest,ConfigurationOperationResult> dispatch):IToolConfigurationDispatcher {public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest request)=>dispatch(request);}
}
