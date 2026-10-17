using System.IO;
using System.Windows;
using System.Windows.Controls;
using VisionPluginContracts;
static class Program {
 static int checks;static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
 [STAThread] static void Main(){
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
  var fixtures=Path.Combine(AppContext.BaseDirectory,"Fixtures");
  var definition=ToolConfigurationDefinition.Parse(File.ReadAllText(Path.Combine(fixtures,"IMVSNImageCalibModuAlgorithmTab.xml")),File.ReadAllText(Path.Combine(fixtures,"Group.xml")));
  var state=new ToolConfigurationState(definition);var source=state.Find("MovePhysicalPointX")!;
  var query=ConfigurationSubscriptionValidation.FromElement(source.Element)!;
  Check(query.Destination.Domain==ConfigurationSubscriptionDomain.InputIo&&query.Destination.Name=="MovePhysicalPointX","input destination from operation params");
  Check(query.NativeTypes.SequenceEqual(new[]{"float"})&&!query.IncludeSelf&&!query.IncludeInside&&query.IncludeEmpty,"original source filter flags");
  var point=ConfigurationSubscriptionValidation.FromElement(state.Find("PhyPoint")!.Element)!;Check(point.Destination.Domain==ConfigurationSubscriptionDomain.Combination&&point.NativeTypes.SequenceEqual(new[]{"POINT"}),"combination distinction");
  var scalar=state.Find("BaseX")??state.Nodes.First(x=>x.Element.Tag=="Float");var parameter=ConfigurationSubscriptionValidation.FromElement(scalar.Element,ConfigurationSubscriptionDomain.Parameter)!;
  Check(parameter.Destination.Domain==ConfigurationSubscriptionDomain.Parameter&&parameter.Destination.Name==scalar.Name,"parameter destination");
  var a=new ConfigurationOutputReference("procedure-a","node-identity-1","port-x","float");
  var b=new ConfigurationOutputReference("procedure-a","node-identity-2","port-x","float");
  var ba=new ConfigurationSubscriptionBinding(a,ConfigurationSubscriptionIndex.All);var bb=new ConfigurationSubscriptionBinding(b,ConfigurationSubscriptionIndex.All);
  var leafA=new ConfigurationSubscriptionTreeNode("leaf-a","same.caption",binding:ba);var leafB=new ConfigurationSubscriptionTreeNode("leaf-b","same.caption",binding:bb);
  var empty=new ConfigurationSubscriptionTreeNode("none","null",isUnbind:true);var group=new ConfigurationSubscriptionTreeNode("procedure","procedure.caption",new[]{new ConfigurationSubscriptionTreeNode("module","module.caption",new[]{leafA,leafB}),empty});
  Check(!group.IsSelectable&&leafA.IsSelectable&&empty.IsSelectable,"groups versus selectable leaves");
  var service=new Service(new[]{group});var selector=new ToolConfigurationSubscriptionSelector(state,source,query,service,key=>"translated "+key);
  Check(service.Reads==1&&source.SubscriptionBinding==null,"authoritative initial unbound read");
  selector.Open();Check(selector.Tree.Items.Count==1&&((TreeViewItem)selector.Tree.Items[0]).Items.Count==2,"hierarchy retained");
  Check(((TreeViewItem)((TreeViewItem)selector.Tree.Items[0]).Items[0]).Items.Count==2,"nested module leaves retained");
  Check(selector.Select(leafA)&&source.SubscriptionBinding!.Reference==a,"select typed identity A");
  Check(selector.Select(leafB)&&source.SubscriptionBinding!.Reference==b,"identical captions preserve distinct identity B");
  Check(service.Last?.Reference==b,"binding uses identity not localized label");
  Check(!selector.Select(group),"group not bindable");
  Check(selector.SetIndex(ConfigurationSubscriptionIndex.At(3))&&source.SubscriptionBinding!.Index.Literal==3,"literal index");
  var index=new ConfigurationOutputReference("procedure-a","counter","index","int");Check(selector.SetIndex(ConfigurationSubscriptionIndex.From(index))&&source.SubscriptionBinding!.Index.Reference==index,"reference index distinct");
  Check(selector.CommitConstant("service display[5]")&&source.SubscriptionBinding!.Reference==b&&source.SubscriptionBinding.Index.Literal==5,"index text reuses typed source identity");
  Check(!selector.CommitConstant("different caption[5]")&&source.SubscriptionBinding!.Reference==b,"caption edit cannot change identity");
  selector.OpenIndex();Check(service.LastQuery!.IndexOnly&&service.LastQuery.NativeTypes.SequenceEqual(new[]{"int"}),"index popup requests int tree");selector.Close();
  bool wrongIndex=false;try{ConfigurationSubscriptionIndex.From(a);}catch(ArgumentException){wrongIndex=true;}Check(wrongIndex,"float cannot serve as index");
  Check(!selector.CommitConstant("same.caption[0]"),"display text is never parsed as source identity");
  Check(selector.CommitConstant("2.5")&&service.Last!.Reference is ConfigurationConstantReference{Literal:"2.5",NativeType:"float"},"typed constant input");
  Check(!selector.CommitConstant("NaN"),"NaN constant rejected");
  Check(selector.Select(empty)&&source.SubscriptionBinding==null,"explicit unbind leaf");
  service.Fail=true;Check(!selector.Select(leafA)&&source.SubscriptionBinding==null,"failed bind cannot fake selected value");service.Fail=false;
  var unavailable=new ToolConfigurationSubscriptionSelector(state,source,query,null);unavailable.Open();Check(unavailable.Diagnostic!=null&&unavailable.Tree.Items.Count==0,"missing service diagnosed no fabricated tree");unavailable.Close();
  Check(ConfigurationSubscriptionValidation.Check(query,new(new ConfigurationOutputReference("p","n","x","int"),ConfigurationSubscriptionIndex.All))!=null,"native type filter enforced");
  Check(ConfigurationSubscriptionValidation.CheckTree(new(ConfigurationSubscriptionStatus.Available,new[]{leafA,leafA}))!=null,"duplicate tree identity diagnosed");
  Check(ConfigurationSubscriptionValidation.Check(query,new(new ConfigurationOutputReference("","n","x","float"),ConfigurationSubscriptionIndex.All))!=null,"incomplete source identity rejected");
  var ps=new ToolConfigurationSubscriptionSelector(state,scalar,parameter,service);string original=scalar.Value;
  Check(ps.Bind(ba)&&scalar.Value==original&&scalar.Get("IsSub")=="True","parameter subscription does not overwrite numeric value");
  Check(ps.SetMode(false)&&scalar.Get("IsSub")=="False"&&scalar.Value==original,"disable subscription preserves scalar value");
  Check(ps.SetMode(true)&&scalar.Get("IsSub")=="True"&&scalar.SubscriptionBinding==null,"empty binding preserves explicitly chosen subscribe mode");
  var metadata=new ConfigurationSubscriptionPortMetadata(Visible:true,CanSubscribe:true,IsPrivate:false,OwnerContainerId:"group",NativeFieldPath:new[]{"Point","X"});
  var detailed=new ConfigurationSubscriptionTreeNode("detailed","caption",binding:ba,metadata:metadata);Check(detailed.Metadata!.NativeFieldPath!.SequenceEqual(new[]{"Point","X"}),"native field metadata remains separate from caption");
  var typedRecorder=new Recorder();var typedState=new ToolConfigurationState(definition,typedRecorder);var typedNode=typedState.Find("MovePhysicalPointX")!;
  var applied=typedState.ApplySubscription(typedNode,new(ConfigurationSubscriptionStatus.Available,ba,"service-owned caption"));Check(applied&&typedNode.SubscriptionBinding==ba&&!typedRecorder.Calls.Any(x=>x.Kind==ConfigurationOperationKind.WriteInputBinding),"authoritative bind result does not rebind by presentation trigger");
  Check(!typedState.ApplySubscription(typedNode,new(ConfigurationSubscriptionStatus.Available,bb))&&typedNode.SubscriptionBinding==ba,"missing presentation diagnosed without inventing a fake bound value");
  var indexQuery=query with {NativeTypes=new[]{"int"},IndexOnly=true};Check(ConfigurationSubscriptionValidation.Check(indexQuery,ba)!=null,"index query restricted to int");
  var dispatch=new Recorder();var persisted=ToolConfigurationDefinition.Parse("<AlgorithmTabRoot><Tabs><Tab Name='A'><Categorys><Category Name='C'><Items><RadioSelecter Name='Mode'><AccessMode>O</AccessMode><DefaultValue>0</DefaultValue><CurValue>0</CurValue></RadioSelecter></Items></Category></Categorys></Tab></Tabs></AlgorithmTabRoot>");
  var userState=new ToolConfigurationState(persisted,dispatch);userState.Initialize();Check(userState.Find("Mode")!.Value=="1","O-only radio read user value");Check(dispatch.Calls.Any(x=>x.Kind==ConfigurationOperationKind.ReadUserValue&&x.Parameters.Single()=="Param.Mode"),"user state key separated from native parameter");
  userState.CommitUser("Mode","0");Check(dispatch.Calls.Last().Kind==ConfigurationOperationKind.PersistUserValue&&dispatch.Calls.Last().Parameters.Single()=="Param.Mode","O-only write uses user value store");
  var commands=ToolConfigurationDefinition.Parse("<AlgorithmTabRoot><BottomExcuteButtons>"+string.Concat(new[]{"ClearCalibPoint","ContinuousSelfExcuteModule","StopSelfExcuteModule"}.Select((x,i)=>"<BottomCommand Name='C"+i+"'><CommandSetter><OperationName>ExcuteMethodOperation</OperationName><OperationParams>"+x+"</OperationParams></CommandSetter></BottomCommand>"))+"</BottomExcuteButtons></AlgorithmTabRoot>");
  var cmdState=new ToolConfigurationState(commands,dispatch);foreach(var node in cmdState.Nodes)cmdState.Invoke(node);
  Check(dispatch.Calls.TakeLast(3).Select(x=>x.Kind).SequenceEqual(new[]{ConfigurationOperationKind.ClearCalibrationPoints,ConfigurationOperationKind.RunContinuous,ConfigurationOperationKind.StopContinuous}),"source-confirmed bottom method kinds");
  selector.Close();ps.Close();app.Shutdown();Console.WriteLine($"VisionConfiguration.SubscriptionChecks: {checks} checks passed");
 }
 sealed class Service(IReadOnlyList<ConfigurationSubscriptionTreeNode> roots):IToolConfigurationSubscriptionService {
  public int Reads;public ConfigurationSubscriptionQuery? LastQuery;public bool Fail;public ConfigurationSubscriptionBinding? Last;
  public ConfigurationSubscriptionTreeResult Enumerate(ConfigurationSubscriptionQuery q){LastQuery=q;return new(ConfigurationSubscriptionStatus.Available,roots);}
  public ConfigurationSubscriptionBindingResult Read(ConfigurationSubscriptionDestination d){Reads++;return new(ConfigurationSubscriptionStatus.Available,Last,Last==null?null:"service display");}
  public ConfigurationSubscriptionBindingResult Bind(ConfigurationSubscriptionDestination d,ConfigurationSubscriptionBinding? b){if(Fail)return new(ConfigurationSubscriptionStatus.Failed,Last,Diagnostic:"rejected by service");Last=b;return new(ConfigurationSubscriptionStatus.Available,b,b==null?null:"service display");}
 }
 sealed class Recorder:IToolConfigurationDispatcher {
  public List<ConfigurationOperationRequest> Calls=[];
  public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest r){Calls.Add(r);return new(true,Value:r.Kind==ConfigurationOperationKind.ReadUserValue?"1":null);}
 }
}
