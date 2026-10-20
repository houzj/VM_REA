using System.IO;
using System.Reflection;
using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPlugins.MapCalibration;
using VisionPluginContracts;
static class Program {
 static int checks;static void Check(bool ok,string text){if(!ok)throw new Exception(text);checks++;}
 static readonly string Dir=Directory.GetCurrentDirectory();
 static object State(MapCalibrationPlugin p,string node,ModuleExecutionScope? scope=null)=>typeof(MapCalibrationPlugin).GetMethod("CurrentImages",BindingFlags.NonPublic|BindingFlags.Instance)!.Invoke(p,[node,Dir,scope])!;
 static T Field<T>(object state,string name)=>(T)state.GetType().GetField(name)!.GetValue(state)!;
 static Dictionary<string,JsonElement> Parameters(MapCalibrationPlugin p,bool teach=true,int mode=1){var d=p.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());d["TeachEnable"]=JsonSerializer.SerializeToElement(teach);d["InputType"]=JsonSerializer.SerializeToElement(mode);return d;}
 static void Pending(object state)=>state.GetType().GetField("SourcePending")!.SetValue(state,true);
 static int Main(){try{Oracle();Graph();AliasGraph();Modes();Console.WriteLine($"PASS {checks}: Map exact native typed read gates/order plus actual Graph arrays/indices");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
 static void Oracle(){
  foreach(var line in File.ReadLines("analysis/map-calibration/typed-input-oracle/module-file-results.jsonl")){
   using var doc=JsonDocument.Parse(line);var row=doc.RootElement;if(!row.TryGetProperty("case",out var caseJson))continue;int index=caseJson.GetInt32();var p=new MapCalibrationPlugin();var state=State(p,"native"+index);Pending(state);var calls=new List<string>();
   ModuleTextInputReadResult? Text(string name,int at,int capacity){Check(at==0&&capacity==512,"Native text index/capacity");calls.Add(name);string value=index>=8?new string('Q',index==8?511:512):name=="TeachFlagInput"&&index==1?"B":name=="TeachFlagInput"&&index==6?"":"A";if(index==6&&name=="Trigger"||index==7&&name=="TeachFlagInput")return new(0xe0000123,1,"",0);if(index==9)return new(0xe0000012,1,"",512);return new(0,1,value,value.Length+1);}
   ModuleNumericInputReadResult? Float(string name,int at){Check(at==0,"Native float index0");calls.Add(name);if(index==2&&name=="PhysicalPointX"||index==3&&name=="PhysicalPointY"||index==4&&name=="WorldRotateAngle")return new(0xe0000123,0,0);if(index==5&&name=="PhysicalPointX")return new(0,0,0);return new(0,2,name=="PhysicalPointX"?12:name=="PhysicalPointY"?13:14);}
   var c=new ModuleExecutionContext("native"+index,Dir,new Dictionary<string,PortValue>(),Parameters(p,index!=0)){ReadNumericInput=Float,ReadTextInput=Text,ReadIntegerInput=(_,_)=>throw new Exception("Image must not read refresh")};var actual=p.CreateRuntime().ExecuteAsync(c,default).Result;
   Check(actual.NativeErrorCode==row.GetProperty("code").GetUInt32(),"Native oracle return "+index);var pose=Field<MapCalibrationPose>(state,"TeachPose");Check(new[]{pose.X,pose.Y,pose.Angle}.SequenceEqual(row.GetProperty("pose").EnumerateArray().Select(x=>x.GetSingle())),"Native partial pose/count0 "+index);Check(Field<bool>(state,"TeachTriggered")==row.GetProperty("teach").GetBoolean(),"Native teach flag "+index);
   var expected=row.GetProperty("calls").EnumerateArray().Where(x=>x.GetProperty("op").GetString() is "TextABI" or "GetFloat").Select(x=>x.GetProperty("name").GetString()!).ToArray();Check(calls.SequenceEqual(expected),"Native read order and early stop "+index);
  }
 }
 static void Graph(){
  foreach(var (mode,xIndex,textIndex,intIndex,expected) in new (int,string?,string?,string?,uint)[]{(1,null,null,null,0),(1,"1",null,null,0),(1,"9",null,null,0xe0000014),(1,"-1",null,null,0xe0000014),(1,null,"1",null,0xe0000314),(1,null,"9",null,0xe0000314),(1,null,"-1",null,0xe0000314),(2,null,null,null,0xe0000201),(2,null,null,"1",0x80100006),(2,null,null,"9",0x80100006),(2,null,null,"-1",0x80100006)}){
   var p=new MapCalibrationPlugin();var registry=new ModuleRegistry();registry.Register(p.Descriptor,p.CreateRuntime);registry.Register(Source.Definition,()=>new Source());var parameters=Parameters(p,true,mode);
   WorkflowSubscription Bind(string input,string output,string? selector)=>new("map",input,false,new(WorkflowSourceKind.NodeOutput,"source",output){ArrayIndex=selector is null?null:new(selector)});
   var flow=new ProcedureDocument("typed-map","typed-map",[new("source",Source.Id,"1","source",[]),new("map",MapCalibrationPlugin.Id,"1.0.0","map",parameters)],[],[new("source","map")]){Configuration=new(){Subscriptions=[Bind("PhysicalPointX","X",xIndex),Bind("PhysicalPointY","Y",null),Bind("WorldRotateAngle","A",null),Bind("Trigger","Trigger",textIndex),Bind("TeachFlagInput","Flag",null),Bind("RefreshSignal","Signal",intIndex)]}};
   Check(GraphValidator.Validate(flow,registry).IsValid,"Graph accepts original float/string/int array subscriptions");using var scope=new ModuleExecutionScope();var state=State(p,"map",scope.ForProcedure(flow.Id));Pending(state);var run=new GraphExecutor(registry).RunAsync(flow,Dir,scope:scope).Result;var node=run.Nodes.Single(n=>n.NodeId=="map");Check(node.NativeErrorCode==expected,"Actual Graph returns original branch error "+mode+"/"+xIndex+"/"+textIndex+"/"+intIndex);
   if(mode==1&&expected==0){Check(Field<MapCalibrationPose>(state,"TeachPose").X==(xIndex=="1"?91:12),"Graph selected numeric value, no eager scalar coercion");Check(node.Outputs["CalibStatus"].As<long>()==0,"Graph teach pending branch");}
  }
 }
 static void AliasGraph(){
  foreach(var selector in new[]{ConfigurationSubscriptionIndex.All,ConfigurationSubscriptionIndex.At(1),ConfigurationSubscriptionIndex.At(9),ConfigurationSubscriptionIndex.At(-1)}){
   var p=new MapCalibrationPlugin();var registry=new ModuleRegistry();registry.Register(p.Descriptor,p.CreateRuntime);registry.Register(Source.Definition,()=>new Source());
   WorkflowSubscription Bind(string input,string output)=>new("map",input,false,new(WorkflowSourceKind.NodeOutput,"source",output));
   var flow=new ProcedureDocument("alias-map","alias-map",[new("source",Source.Id,"1","source",[]),new("map",MapCalibrationPlugin.Id,"1.0.0","map",Parameters(p))],[],[new("source","map")]){Configuration=new(){Subscriptions=[Bind("WorldRotateAngle","A"),Bind("Trigger","Trigger"),Bind("TeachFlagInput","Flag")]}};
   void Apply(IReadOnlyList<ConfigurationSubscriptionDocumentChange> changes){var names=changes.Select(x=>x.Destination.Name).ToHashSet();var list=flow.Configuration.Subscriptions.Where(x=>!names.Contains(x.Target)).ToList();foreach(var change in changes)if(change.Source is {} source)list.Add(new("map",change.Destination.Name,false,source));flow=flow with{Configuration=flow.Configuration with{Subscriptions=list}};}
   var bridge=new WorkflowConfigurationSubscriptionService(()=>flow,registry,"map",(destination,reference)=>Apply([new(destination,reference)]),Apply);
   var alias=new PointAliasConfigurationSubscriptions(bridge,()=>flow,registry,"map",p.PointInputAliases);
   var bind=alias.Bind(new(ConfigurationSubscriptionDomain.Combination,"Physical Point","POINT"),new(new ConfigurationOutputReference(flow.Id,"source","Points","POINT"),selector));Check(bind.Status==ConfigurationSubscriptionStatus.Available,"Normal UI POINT array alias bind "+selector.LiteralText+"/"+bind.Status+"/"+bind.Diagnostic);
   Check(flow.Configuration.Subscriptions.Count(x=>x.Target is "PhysicalPointX" or "PhysicalPointY")==2&&!flow.Configuration.Subscriptions.Any(x=>x.Target=="Physical Point"),"UI alias persisted actual X/Y leaves");Check(GraphValidator.Validate(flow,registry).IsValid,"Alias array graph valid");
   using var scope=new ModuleExecutionScope();var state=State(p,"map",scope.ForProcedure(flow.Id));Pending(state);var result=new GraphExecutor(registry).RunAsync(flow,Dir,scope:scope).Result.Nodes.Single(x=>x.NodeId=="map");bool good=selector==ConfigurationSubscriptionIndex.All||selector.LiteralText=="1";Check(result.NativeErrorCode==(good?0u:0xe0000014u),"Actual alias Graph keeps first/indexed/error read");if(good){var pose=Field<MapCalibrationPose>(state,"TeachPose");Check(pose.X==(selector.LiteralText=="1"?91:12)&&pose.Y==(selector.LiteralText=="1"?92:13),"Actual alias pair uses same selected original index");}
  }
 }
 static void Modes(){
  using(var scope=new ModuleExecutionScope()){
   var scopedPlugin=new MapCalibrationPlugin();var scopedState=State(scopedPlugin,"scope",scope);Pending(scopedState);var reader=new ScopeReader();scope.RegisterService<IToolNumericInputReader>(reader);scope.RegisterService<IToolTextInputReader>(reader);
   var scoped=scopedPlugin.CreateRuntime().ExecuteAsync(new ModuleExecutionContext("scope",Dir,new Dictionary<string,PortValue>(),Parameters(scopedPlugin)){Scope=scope,ReadNumericInput=(_,_)=>throw new Exception("Scope numeric precedence"),ReadTextInput=(_,_,_)=>throw new Exception("Scope text precedence")},default).Result;
   Check(scoped.NativeErrorCode==0&&Field<MapCalibrationPose>(scopedState,"TeachPose")==new MapCalibrationPose(10,10,10),"Scope primitive services precede graph/materialized values");Check(reader.Calls.SequenceEqual(new[]{"Trigger:0:512","TeachFlagInput:0:512","PhysicalPointX:0","PhysicalPointY:0","WorldRotateAngle:0"}),"Scope exact branch read order/capacity");
  }
  var p=new MapCalibrationPlugin();var state=State(p,"frame");var order=new List<string>();var image=new GrayImage(2,2,new byte[4]);var c=new ModuleExecutionContext("frame",Dir,new Dictionary<string,PortValue>{{"InputImage1",PortValue.Image(image)}},Parameters(p,false)){ReadNumericInput=(name,i)=>{order.Add(name);return name=="PhysicalPointX"?new(0,2,23):new(0xe0000123,0,0);},ReadTextInput=(_,_,_)=>throw new Exception("Disabled teach reads text"),ReadIntegerInput=(_,_)=>throw new Exception("Image reads refresh")};var result=p.CreateRuntime().ExecuteAsync(c,default).Result;Check(result.NativeErrorCode==0&&Field<MapCalibrationPose>(state,"SourcePose")==new MapCalibrationPose(23,-999,-999),"Normal fresh frame ignores primitive failures and keeps initialized slots");Check(order.SequenceEqual(new[]{"PhysicalPointX","PhysicalPointY","WorldRotateAngle"}),"Fresh frame ordered reads despite previous failure");

 }
 sealed class ScopeReader:IToolNumericInputReader,IToolTextInputReader{
  public List<string> Calls=[];ToolNumericInputReadResult IToolNumericInputReader.Read(ModuleExecutionContext c,string name,int index){Calls.Add(name+":"+index);return new(0,2,10);}
  ToolTextInputReadResult IToolTextInputReader.Read(ModuleExecutionContext c,string name,int index,int capacity){Calls.Add(name+":"+index+":"+capacity);return new(0,2,"A",2);}
 }
 sealed class Source:IModule {
  public const string Id="test.map.typed";public static ModuleDescriptor Definition{get;}=new(Id,"1","source",[],[new("X",DataKind.NumberArray),new("Y",DataKind.NumberArray),new("A",DataKind.NumberArray),new("Trigger",DataKind.Text){Cardinality=ValueCardinality.Array},new("Flag",DataKind.Text){Cardinality=ValueCardinality.Array},new("Signal",DataKind.Integer){Cardinality=ValueCardinality.Array},new("Points",DataKind.Point){Cardinality=ValueCardinality.Array}],[]);public ModuleDescriptor Descriptor=>Definition;
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext c,CancellationToken t)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"X",PortValue.Numbers([12,91])},{"Y",PortValue.Numbers([13,92])},{"A",PortValue.Numbers([14,93])},{"Trigger",PortValue.TextArray(["A","B"])},{"Flag",PortValue.TextArray(["A","C"])},{"Signal",PortValue.IntegerArray([1,0])},{"Points",PortValue.PointArray([new Point2(12,13),new Point2(91,92)])}}));
 }
}
