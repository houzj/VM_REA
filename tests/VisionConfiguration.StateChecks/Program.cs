using System.Text.Json;
using VisionPluginContracts;
int checks=0;void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
var fixture=Path.Combine(AppContext.BaseDirectory,"Fixtures");
var definition=ToolConfigurationDefinition.Parse(File.ReadAllText(Path.Combine(fixture,"IMVSNImageCalibModuAlgorithmTab.xml")),File.ReadAllText(Path.Combine(fixture,"Group.xml")));
var state=new ToolConfigurationState(definition);state.RegisterRule("HidePhysicInfoOperation",new NImagePhysicalVisibility());state.Initialize();
using var snapshots=JsonDocument.Parse(File.ReadAllText(Path.Combine(fixture,"NImageOriginalStates.json")));
foreach(var snapshot in snapshots.RootElement.EnumerateArray()) {
 string phase=snapshot.GetProperty("phase").GetString()!;
 switch(phase) {
 case "motion-input-1":state.SetProgrammatic("CalibPointInput","CurValue","1");break;
 case "motion-input-0":state.SetProgrammatic("CalibPointInput","CurValue","0");break;
 case "fixture-True":state.SetProgrammatic("HomoFixEnable","CurValue","True");break;
 case "fixture-False":state.SetProgrammatic("HomoFixEnable","CurValue","False");break;
 case "fixture-input-1":state.SetProgrammatic("HomoFixEnable","CurValue","True");state.SetProgrammatic("InputMode","CurValue","1");break;
 case "fixture-input-0":state.SetProgrammatic("InputMode","CurValue","0");break;
 default:
 if(phase.StartsWith("rotation-count-"))state.SetProgrammatic("RotPointTotalNum","CurValue",phase[15..]);
 if(phase.StartsWith("board-"))state.SetProgrammatic("CalibBoardType","CurValue",phase[6..]);break;
 }
 foreach(var item in snapshot.GetProperty("items").EnumerateArray()) {
  if(!item.TryGetProperty("NameA",out var name)||string.IsNullOrEmpty(name.GetString()))continue;
  var node=state.Find(name.GetString()!);Check(node!=null,phase+" missing "+name.GetString());
  if(item.TryGetProperty("CurValueA",out var actualValue)) {
   var expected=actualValue.GetString()??"";
   bool valueMatches=node!.Value==expected || double.TryParse(node.Value,out var a)&&double.TryParse(expected,out var b)&&a==b;
   Check(valueMatches,phase+" value "+name.GetString()+" expected '"+expected+"' actual '"+node.Value+"'");
  }
  if(item.TryGetProperty("CustomVisibleA",out var visible))Check(node!.CustomVisible==bool.Parse(visible.GetString()!),phase+" visibility "+name.GetString()+" expected "+visible.GetString()+" actual "+node.CustomVisible);
 }
 foreach(var category in snapshot.GetProperty("categories").EnumerateArray()) {
  string name=category.GetProperty("name").GetString()!;Check(state.Find(name)!.CustomVisible==(category.GetProperty("visible").ValueKind==JsonValueKind.True),phase+" category "+name);
 }
}
Check(state.UnsupportedOperations.Contains("GetSelectedFilterOperation"),"unhandled service reported");
// Behavioral checks use a typed recorder, never native methods or arbitrary XML reflection.
var calls=new Recorder();
var simple=ToolConfigurationDefinition.Parse("""
<AlgorithmTabRoot><Tabs><Tab Name="Main"><Categorys><Category Name="C" CanTriggerRun="True" SubscribeVisible="False"><Items>
<Float Name="X"><AccessMode>RW</AccessMode><CurValue>1</CurValue><MinValue>0</MinValue><MaxValue>10</MaxValue><DecimalDigits>2</DecimalDigits></Float>
<Integer Name="Y"><AccessMode>RW</AccessMode><CurValue>2</CurValue><Enable>False</Enable></Integer>
<RadioSelecter Name="Mode"><AccessMode>O</AccessMode><CurValue>0</CurValue><Triggers>
<Trigger><Property>CurValue</Property><Value>0</Value><Setters><Setter><TargetName>T</TargetName><OperationName>VisibleOperation</OperationName></Setter></Setters></Trigger>
<Trigger><Property>CurValue</Property><Value>1</Value><Setters><Setter><TargetName>T</TargetName><OperationName>HiddenOperation</OperationName></Setter></Setters></Trigger>
</Triggers></RadioSelecter><Integer Name="T"><CustomVisible>False</CustomVisible></Integer>
<Integer Name="Unsupported"><Triggers><Trigger><Property>CurValue</Property><Setters><Setter><OperationName>LaunchAnythingOperation</OperationName></Setter></Setters></Trigger></Triggers></Integer>
</Items></Category></Categorys></Tab></Tabs><BottomExcuteButtons>
<BottomCommand Name="Close"><CommandSetter><OperationName>ExcuteMethodOperation</OperationName><OperationParams>CloseParamsTabControl</OperationParams></CommandSetter></BottomCommand>
<BottomCommand Name="Bad"><CommandSetter><OperationName>ExcuteMethodOperation</OperationName><OperationParams>GetType</OperationParams></CommandSetter></BottomCommand>
</BottomExcuteButtons></AlgorithmTabRoot>
""");
var engine=new ToolConfigurationState(simple,calls){ExecutionEnabled=true};int events=0;int thread=Environment.CurrentManagedThreadId;
engine.Changed+=()=>{Check(Environment.CurrentManagedThreadId==thread,"caller-thread event");events++;};engine.Initialize();
Check(!calls.Calls.Any(x=>x.Kind is ConfigurationOperationKind.WriteParameter or ConfigurationOperationKind.Run),"initialize never commits/runs");
Check(engine.Find("T")!.CustomVisible,"initial visible operation");Check(!engine.Find("X")!.SubscribeVisible,"category inherited subscription visibility");Check(engine.Find("X")!.CanTriggerRun,"inherited execution flag");
calls.Calls.Clear();engine.SetProgrammatic("X","CurValue","3.125");Check(engine.Find("X")!.Value=="3.12","bankers decimal rounding");Check(calls.Calls.Count==0,"programmatic change no commit/run");
engine.CommitUser("X","3.125");Check(calls.Calls.Select(x=>x.Kind).SequenceEqual(new[]{ConfigurationOperationKind.WriteParameter,ConfigurationOperationKind.Run}),"one commit then one run");
int before=calls.Calls.Count;engine.CommitUser("X","3.12");Check(calls.Calls.Count==before,"unchanged commit no rerun");
engine.IsBusy=true;engine.CommitUser("X","4");Check(calls.Calls.Last().Kind==ConfigurationOperationKind.WriteParameter,"busy blocks run");
engine.IsBusy=false;engine.ExecutionEnabled=false;engine.CommitUser("X","5");Check(calls.Calls.Last().Kind==ConfigurationOperationKind.WriteParameter,"global switch blocks run");
calls.FailWrite=true;engine.CommitUser("X","6");Check(engine.Find("X")!.Value=="5","failed write rolls back");Check(engine.Diagnostics.Any(x=>x.Code=="OperationError"),"service failure diagnosed");calls.FailWrite=false;
before=calls.Calls.Count;engine.CommitUser("Y","7");engine.CommitUser("X","NaN");engine.CommitUser("X","11");Check(calls.Calls.Count==before,"disabled/nonfinite/range failures do not dispatch");
engine.SetProgrammatic("Mode","CurValue","1");Check(!engine.Find("T")!.CustomVisible,"hidden operation");engine.SetProgrammatic("Mode","CurValue","0");Check(engine.Find("T")!.CustomVisible,"reset all operations before applying matching trigger");
engine.SetProgrammatic("Unsupported","CurValue","1");Check(engine.UnsupportedOperations.Contains("LaunchAnythingOperation"),"unknown operation diagnosed");Check(!calls.Calls.Any(x=>x.Kind==ConfigurationOperationKind.Unknown),"unknown never reaches dispatcher");
engine.Invoke(engine.Find("Close")!);Check(calls.Calls.Last().Kind==ConfigurationOperationKind.Close,"explicit close command");before=calls.Calls.Count;engine.Invoke(engine.Find("Bad")!);Check(calls.Calls.Count==before&&engine.UnsupportedOperations.Contains("ExcuteMethodOperation"),"arbitrary method blocked");
var duplicate=ToolConfigurationDefinition.Parse("<AlgorithmTabRoot><Tabs><Tab Name='A'><Categorys><Category Name='CA'><Items><Integer Name='Same'/></Items></Category></Categorys></Tab><Tab Name='B'><Categorys><Category Name='CB'><Items><Integer Name='Same'/></Items></Category></Categorys></Tab></Tabs></AlgorithmTabRoot>");
var duplicates=new ToolConfigurationState(duplicate);var same=duplicates.Nodes.Where(x=>x.Name=="Same").ToArray();Check(same.Length==2&&!ReferenceEquals(same[0],same[1]),"duplicate name distinct state");Check(ReferenceEquals(duplicates.GetNode(same[1].Element),same[1]),"element lookup avoids duplicate-name ambiguity");
var subCalls=new Recorder();var subState=new ToolConfigurationState(duplicate,subCalls);var subNode=subState.Find("Same")!;
subState.SetSubscription(subNode,new(new ConfigurationOutputReference("P","N","Float","float"),ConfigurationSubscriptionIndex.All),"Prev.Float");Check(subNode.Get("IsSub")=="True"&&subNode.Get("Subscription")=="Prev.Float","typed parameter subscription");
subState.SetProgrammatic(subNode,"CustomVisible","False");Check(subNode.Get("Subscription")==""&&subNode.Get("IsSub")=="True","hide clears binding but preserves subscription mode");Check(subCalls.Calls.Select(x=>x.Kind).SequenceEqual(new[]{ConfigurationOperationKind.WriteParameterBinding,ConfigurationOperationKind.ClearParameterBinding,ConfigurationOperationKind.ReadParameter}),"unsubscribe then refresh parameter");
subState.SetProgrammatic(subNode,"IsDeleteSubscribe","False");subState.SetSubscription(subNode,new(new ConfigurationOutputReference("P","N","Float","float"),ConfigurationSubscriptionIndex.All),"Prev.Float");subState.SetProgrammatic(subNode,"CustomVisible","True");subState.SetProgrammatic(subNode,"CustomVisible","False");Check(subNode.Get("Subscription")=="Prev.Float","delete-subscription flag respected");
var trace=new List<string>();var rulesXml="""
<AlgorithmTabRoot><Tabs><Tab Name='A'><Categorys><Category Name='C'><Items><Integer Name='Mode'><Triggers>
<Trigger><Property>CurValue</Property><Value>0</Value><Setters><Setter><OperationName>RegisteredRule</OperationName><OperationParams>zero</OperationParams></Setter></Setters></Trigger>
<Trigger><Property>CurValue</Property><Value>1</Value><Setters><Setter><OperationName>RegisteredRule</OperationName><OperationParams>one</OperationParams></Setter></Setters></Trigger>
</Triggers></Integer></Items></Category></Categorys></Tab></Tabs></AlgorithmTabRoot>
""";
var traced=new ToolConfigurationState(ToolConfigurationDefinition.Parse(rulesXml));traced.RegisterRule("RegisteredRule",new TraceRule(trace));traced.Initialize();trace.Clear();traced.SetProgrammatic("Mode","CurValue","1");Check(trace.SequenceEqual(new[]{"revert:zero","apply:one"}),"ReOperate phase precedes OnTrigger phase");
var templates=ToolConfigurationDefinition.Parse("<AlgorithmTabRoot><Tabs><Tab Name='A'><Categorys><Category Name='C'><Items><BooleanBx Name='Alias'><DisplayType>RadioButtons</DisplayType></BooleanBx><SubModuleInfoItem Name='Dynamic'/><RadioButtons Name='Unknown'/></Items></Category></Categorys></Tab></Tabs></AlgorithmTabRoot>");
var templateState=new ToolConfigurationState(templates);Check(templateState.Find("Alias")!.DisplayType=="Boolean","literal override uses catalog; raw XML DisplayType ignored");
Check(templateState.Find("Dynamic")!.Template.Support==ConfigurationTemplateSupport.RequiresRuntimeDisplayType,"runtime type needs evaluation");
templateState.RegisterDisplayTypeResolver("SubModuleInfoItem",_=>"Integer");Check(templateState.Find("Dynamic")!.DisplayType=="Integer","explicit evaluated display type");
Check(templateState.Find("Unknown")!.Template.Support==ConfigurationTemplateSupport.Unsupported&&templateState.Diagnostics.Any(x=>x.Code=="UnsupportedTemplate"),"unknown template diagnosed");
Check(!templateState.Find("Alias")!.Template.IndependentRendererVerified,"metadata does not assert renderer parity");
var quantized=ToolConfigurationDefinition.Parse("<AlgorithmTabRoot><Tabs><Tab Name='A'><Categorys><Category Name='C'><Items><Integer Name='I'><MinValue>0</MinValue><MaxValue>30</MaxValue><IncValue>3</IncValue><CurValue>5</CurValue></Integer></Items></Category></Categorys></Tab></Tabs></AlgorithmTabRoot>");
var quantizedState=new ToolConfigurationState(quantized);quantizedState.SetProgrammatic("I","CurValue","5");Check(quantizedState.Find("I")!.Value=="5","same current integer bypasses setter quantization");quantizedState.SetProgrammatic("I","CurValue","8");Check(quantizedState.Find("I")!.Value=="6","original integer min-relative step quantization");quantizedState.SetProgrammatic("I","CurValue","32");Check(quantizedState.Find("I")!.Value=="32","programmatic out-of-range retained by native setter");
var noHead=ToolConfigurationDefinition.Parse("<AlgorithmTabRoot><Tabs><Tab Name='A'><Categorys><CategoryNoHead Name='NoHead' CanTriggerRun='True'><Items><StaticParam><DisplayName>CaptionOnly</DisplayName></StaticParam><Integer><Name>FieldName</Name></Integer></Items></CategoryNoHead></Categorys></Tab></Tabs></AlgorithmTabRoot>");var noHeadState=new ToolConfigurationState(noHead);Check(noHeadState.Nodes.Any(x=>x.Element.Tag=="StaticParam"&&x.Name==null),"unnamed source row retained by element identity");Check(noHeadState.Find("FieldName")!.CanTriggerRun&&noHeadState.Find("NoHead")!.IsCategory,"field-form name and CategoryNoHead inheritance");
Check(state.Find("MoveFirstType")!.Choices.Any(x=>x.Value=="1")&&!state.Find("MoveFirstType")!.Choices.Any(x=>x.Value.StartsWith("0x")),"EnumEntry.Value has native Int32 identity");
var expandDefinition=ToolConfigurationDefinition.Parse("<AlgorithmTabRoot><Tabs><Tab Name='A'><Categorys><Category Name='C' CanTriggerRun='True'><Items><Expand><CurValue>False</CurValue><Triggers><Trigger><Property>CurValue</Property><Value>True</Value><Setters><Setter><TargetName>Advanced</TargetName><OperationName>VisibleOperation</OperationName></Setter></Setters></Trigger></Triggers></Expand><Float Name='Advanced'><AccessMode>RW</AccessMode><CustomVisible>False</CustomVisible></Float><String Name='Text'><AccessMode>RW</AccessMode><MaxLen>4</MaxLen><CurValue>x</CurValue></String></Items></Category></Categorys></Tab></Tabs></AlgorithmTabRoot>");
var extraCalls=new Recorder();var extra=new ToolConfigurationState(expandDefinition,extraCalls){ExecutionEnabled=true};extra.Initialize();extraCalls.Calls.Clear();var expand=extra.Nodes.Single(x=>x.Element.Tag=="Expand");extra.CommitUser(expand,"True");Check(extra.Find("Advanced")!.CustomVisible&&extra.Find("Advanced")!.Get("IsDeleteSubscribe")=="True","Expand enables advanced subscription deletion before showing");extra.CommitUser(expand,"False");Check(!extra.Find("Advanced")!.CustomVisible&&extra.Find("Advanced")!.Get("IsDeleteSubscribe")=="False"&&extraCalls.Calls.Count==0,"Expand is model-only without write, persistence or execution");
extra.CommitUser("Text","中文");Check(extra.Find("Text")!.Value=="x"&&extra.Diagnostics.Any(x=>x.Operation=="String.CurValueA"),"String MaxLen counts UTF8 bytes");extra.SetProgrammatic("Text","IsSub","True");extra.CommitUser("Text","中文");Check(extra.Find("Text")!.Value=="中文","String subscribed path bypasses byte limit");
before=calls.Calls.Count;engine.CommitUser("X","5",forceWrite:true);Check(calls.Calls.Count==before+1&&calls.Calls.Last().Kind==ConfigurationOperationKind.WriteParameter,"file-style explicit recommit bypasses only deduplication");
var hideDefinition=ToolConfigurationDefinition.Parse("<AlgorithmTabRoot><Tabs><Tab Name='A'><Categorys><Category Name='C'><Items><Float Name='Amount'><AccessMode>RW</AccessMode><MinValue>0</MinValue><MaxValue>100</MaxValue><CurValue>3</CurValue></Float></Items></Category></Categorys></Tab></Tabs></AlgorithmTabRoot>");
var hideDispatcher=new HideReadback();var hideState=new ToolConfigurationState(hideDefinition,hideDispatcher);hideState.Initialize();var amount=hideState.Find("Amount")!;
hideState.SetSubscription(amount,new(new ConfigurationOutputReference("P","N","Value","float"),ConfigurationSubscriptionIndex.All),"Source.Value[]");Check(hideDispatcher.HasBinding&&amount.Get("IsSub")=="True","hide setup has real controlled dispatcher binding and mode");hideDispatcher.Calls.Clear();
hideState.SetProgrammatic(amount,"CustomVisible","False");Check(!hideDispatcher.HasBinding&&amount.SubscriptionBinding==null&&amount.Get("Subscription")=="","hiding clears dispatcher binding and presentation");Check(amount.Value=="12.5","hiding reads actual unbound numeric parameter back");Check(amount.Get("IsSub")=="True"&&hideDispatcher.UserMode=="True"&&!hideDispatcher.Calls.Any(x=>x.Kind==ConfigurationOperationKind.PersistUserValue),"hiding preserves local and @ user mode without writes");
hideState.SetProgrammatic(amount,"CustomVisible","True");Check(amount.Visible&&amount.Get("IsSub")=="True"&&amount.SubscriptionBinding==null&&amount.Value=="12.5","show again remains subscribed editor with empty binding and restored numeric value");
int readbackWrites=hideDispatcher.Calls.Count(x=>x.Kind==ConfigurationOperationKind.WriteParameter);hideState.CommitUser(amount,"12.5");Check(hideDispatcher.Calls.Count(x=>x.Kind==ConfigurationOperationKind.WriteParameter)==readbackWrites,"restored native value also updates committed baseline");
var hostile=new HostExceptionDispatcher();var isolatedState=new ToolConfigurationState(simple,hostile){ExecutionEnabled=true};isolatedState.Initialize();Check(isolatedState.Find("X")!.Value=="1"&&isolatedState.Diagnostics.Any(x=>x.Code=="OperationError"&&x.Operation=="ReadParameter"),"initial host read exception becomes typed error and preserves default");isolatedState.CommitUser("X","2");Check(isolatedState.Find("X")!.Value=="1"&&hostile.RunCalls==0,"write exception is unhandled failure, rolls model back, never runs");isolatedState.Invoke(isolatedState.Find("Close")!);Check(isolatedState.Diagnostics.Any(x=>x.Code=="OperationError"&&x.Operation=="ExcuteMethodOperation"),"command callback exception isolated from WPF event path");Check(!isolatedState.Diagnostics.Any(x=>x.Code=="UnsupportedOperation"&&x.Operation=="ExcuteMethodOperation"),"registered throwing operation is error rather than unknown operation");
foreach(bool throws in new[]{false,true}) {
 var rejectedClear=new HideReadback{ReturnClearError=!throws,ThrowClearError=throws};var rejectedState=new ToolConfigurationState(hideDefinition,rejectedClear);rejectedState.Initialize();var rejectedNode=rejectedState.Find("Amount")!;var rejectedBinding=new ConfigurationSubscriptionBinding(new ConfigurationOutputReference("P","N","Value","float"),ConfigurationSubscriptionIndex.All);rejectedState.SetSubscription(rejectedNode,rejectedBinding,"Source.Value[]");rejectedClear.Calls.Clear();
 rejectedState.SetProgrammatic(rejectedNode,"CustomVisible","False");Check(!rejectedNode.CustomVisible&&rejectedClear.HasBinding&&rejectedNode.SubscriptionBinding==rejectedBinding&&rejectedNode.Get("Subscription")=="Source.Value[]","failed clear keeps server/UI identity while allowing requested hide; throws="+throws);
 Check(rejectedNode.Get("IsSub")=="True"&&rejectedClear.UserMode=="True"&&!rejectedClear.Calls.Any(x=>x.Kind is ConfigurationOperationKind.ReadParameter or ConfigurationOperationKind.PersistUserValue),"failed clear does not read a fake unbound value or rewrite mode; throws="+throws);
 Check(rejectedState.Diagnostics.Any(x=>x.Code=="SubscriptionClearFailed")&&rejectedState.Diagnostics.Any(x=>x.Code=="OperationError"),"failed/throwing clear is diagnosed; throws="+throws);
 rejectedState.SetProgrammatic(rejectedNode,"CustomVisible","True");Check(rejectedNode.Visible&&rejectedNode.SubscriptionBinding==rejectedBinding&&rejectedNode.Value=="3","reshow after rejected clear retains binding and prior value; throws="+throws);
}
Check(events>0,"events raised");Console.WriteLine($"VisionConfiguration.StateChecks: {checks} checks passed");

sealed class NImagePhysicalVisibility:IToolConfigurationRule {
 public void Apply(ToolConfigurationState s,ConfigurationOperationRequest r) {
  bool point=s.Find("CalibPointInput")?.Value!="1";
  bool visible=string.IsNullOrEmpty(s.Find("RotateAngle")?.Value)&&(point?string.IsNullOrEmpty(s.Find("PhyPoint")?.Value):string.IsNullOrEmpty(s.Find("MovePhysicalPointX")?.Value)&&string.IsNullOrEmpty(s.Find("MovePhysicalPointY")?.Value));
  if(r.Target!=null)s.SetProgrammatic(r.Target,"CustomVisible",visible.ToString());
 }
 public void Revert(ToolConfigurationState s,ConfigurationOperationRequest r){if(r.Target!=null)s.SetProgrammatic(r.Target,"CustomVisible","True");}
}

sealed class Recorder:IToolConfigurationDispatcher {
 public List<ConfigurationOperationRequest> Calls=new();public bool FailWrite;
 public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest r){Calls.Add(r);return new(true,Error:FailWrite&&r.Kind==ConfigurationOperationKind.WriteParameter?"write failed":null);}
}


sealed class TraceRule(List<string> trace):IToolConfigurationRule {
 public void Apply(ToolConfigurationState s,ConfigurationOperationRequest r)=>trace.Add("apply:"+r.Parameters[0]);
 public void Revert(ToolConfigurationState s,ConfigurationOperationRequest r)=>trace.Add("revert:"+r.Parameters[0]);
}

sealed class HideReadback:IToolConfigurationDispatcher {
 public bool HasBinding;public bool ReturnClearError,ThrowClearError;public string UserMode="True";public List<ConfigurationOperationRequest> Calls=[];
 public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest r) {
  Calls.Add(r);
  if(r.Kind==ConfigurationOperationKind.ReadUserValue)return new(true,r.Parameters[0].StartsWith("@")?UserMode:null);
  if(r.Kind==ConfigurationOperationKind.WriteParameterBinding){HasBinding=true;return new(true);}
  if(r.Kind==ConfigurationOperationKind.ClearParameterBinding){if(ThrowClearError)throw new System.IO.IOException("controlled clear failure");if(ReturnClearError)return new(true,Error:"controlled clear refusal");HasBinding=false;return new(true);}
  if(r.Kind==ConfigurationOperationKind.ReadParameter)return new(true,Calls.Any(x=>x.Kind==ConfigurationOperationKind.ClearParameterBinding)?"12.5":"3");
  if(r.Kind==ConfigurationOperationKind.PersistUserValue)UserMode=r.Value!;
  return new(true);
 }
}

sealed class HostExceptionDispatcher:IToolConfigurationDispatcher {
 public int RunCalls;
 public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest r){if(r.Kind==ConfigurationOperationKind.Run){RunCalls++;return new(true);}throw new System.IO.IOException("controlled host callback failure");}
}
