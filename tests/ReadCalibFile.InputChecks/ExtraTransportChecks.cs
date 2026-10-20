using System.IO;
using System.Text;
using System.Text.Json;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;

// Prepared against the root-reviewed typed getter/consumer candidates; not compiled yet.
internal static class ExtraTransportChecks
{
 static readonly double[] Matrix=[1,0,0,0,1,0,0,0,1];
 const string Xml="<R><CalibInputParam/><CalibOutputParam><P ParamName='CalibMatrix' DataType='FloatList'><V>1</V><V>0</V><V>0</V><V>0</V><V>1</V><V>0</V><V>0</V><V>0</V><V>1</V></P><P ParamName='Dynamic' DataType='float'><V>7</V></P><P ParamName='DisplayText' DataType='string'><V>old</V></P></CalibOutputParam></R>";
 const string Extra="<ParamRoot><Categorys><Category Name='Output'><Items><Filter Name='CalibMatrix' ValueType='float' IsArray='false' Others='IsResultShow=true'/><Filter Name='Dynamic' ValueType='float' IsArray='false' Others='IsResultShow=true'/><Filter Name='DisplayInt' ValueType='int' IsArray='false' Others='IsResultShow=true'/><Filter Name='DisplayText' ValueType='string' Others='IsResultShow=true'/></Items></Category></Categorys></ParamRoot>";
 static Dictionary<string,JsonElement> Parameters(ReadCalibFilePlugin plugin){var p=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());p["CachedFileData"]=JsonSerializer.SerializeToElement(Xml);p["DynamicOutData"]=JsonSerializer.SerializeToElement(Extra);return p;}
 internal static void Run(Action<bool,string> check,string output)
 {
  var plugin=new ReadCalibFilePlugin();var parameters=Parameters(plugin);var tape=new Tape();var scope=new ModuleExecutionScope();scope.RegisterService<IToolNumericInputReader>(tape);scope.RegisterService<IToolIntegerInputReader>(tape);scope.RegisterService<IToolTextInputReader>(tape);
  ModuleResult Execute()=>plugin.CreateRuntime().ExecuteAsync(new("extra",output,new Dictionary<string,PortValue>(),parameters){Scope=scope},default).Result;
  tape.Float=(name,index)=>name=="CalibMatrix"?new(0,2,index==0?10:20):new(0xe0000314,0,0);
  tape.Integer=(name,index)=>name=="Dynamic"?new(0,2,index==0?7:8):name=="DisplayInt"?new(0,3,101+index):new(0xe0000314,0,0);
  string longText=new('界',200);tape.Text=(name,index,capacity)=>name!="DisplayText"?new(0xe0000314,0,"",0):capacity<601?new(0xe0000012,0,"",601):new(0,2,longText,601);
  var result=Execute();check(result.Status==ModuleStatus.Success&&result.Outputs["CalibMatrix"].As<IReadOnlyList<double>>().SequenceEqual(new double[]{10,20}.Concat(Matrix.Skip(2))),"short extra indexed writes preserve seven file matrix tail items");
  check(result.Outputs["Dynamic"].As<IReadOnlyList<double>>().SequenceEqual(new double[]{7,8}),"float extra uses explicit per-index integer fallback");
  check(tape.Calls.Where(c=>c.Contains(":Dynamic:",StringComparison.Ordinal)).SequenceEqual(new[]{"F:Dynamic:0","I:Dynamic:0","F:Dynamic:1","I:Dynamic:1"}),"exact extra GetFloat then failed-read GetInt phase order");
  check(result.Outputs["DisplayInt"].Kind==DataKind.Integer&&result.Outputs["DisplayInt"].Cardinality==ValueCardinality.Array&&result.Outputs["DisplayInt"].As<IReadOnlyList<long>>().SequenceEqual(new long[]{101,102,103}),"int extra runtime preserves integer array type despite IsArray false");
  check(result.UpdatedDescriptor!.Outputs.Single(p=>p.Name=="DisplayInt").Cardinality==ValueCardinality.Array&&result.UpdatedDescriptor.Outputs.Single(p=>p.Name=="Dynamic").Kind==DataKind.NumberArray,"actual output descriptor follows this round's extra writes");
  check(result.Outputs["DisplayText"].As<string>()==longText&&tape.Calls.Where(c=>c.StartsWith("S:DisplayText:",StringComparison.Ordinal)).SequenceEqual(new[]{"S:DisplayText:0:511","S:DisplayText:0:602"}),"text array first item retries exact required UTF8 buffer plus one");
  tape.Calls.Clear();tape.Float=(name,index)=>name is "CalibMatrix" or "Dynamic"?index==1?new(0xe000031b,3,0):new(0,3,index==0?11:33):new(0xe0000314,0,0);tape.Integer=(name,index)=>new(0xe000031b,3,0);tape.Text=(name,index,capacity)=>new(0xe0000314,0,"",0);
  var sparse=Execute();check(sparse.Outputs["CalibMatrix"].As<IReadOnlyList<double>>().SequenceEqual(new double[]{11,0,33}.Concat(Matrix.Skip(3))),"injected unchanged-count read failures preserve existing indexed file item");
  check(sparse.Outputs["Dynamic"].As<IReadOnlyList<double>>().SequenceEqual(new double[]{11,33}),"native setter appends one value when requested index exceeds current output count, without a fabricated hole");
  tape.Calls.Clear();tape.Float=(name,index)=>name=="Dynamic"?index==0?new(0,4,55):new(0xe000031b,4,0):new(0xe0000314,0,0);tape.Integer=(name,index)=>new(0xe0000314,0,0);
  var shrinking=Execute();check(shrinking.Outputs["Dynamic"].As<double>()==55&&tape.Calls.Where(c=>c.Contains(":Dynamic:",StringComparison.Ordinal)).SequenceEqual(new[]{"F:Dynamic:0","F:Dynamic:1","I:Dynamic:1"}),"later failure's updated zero count ends native extra read loop");
  tape.Calls.Clear();tape.Float=(name,index)=>new(0,0,777);var zero=Execute();check(zero.Outputs["Dynamic"].As<double>()==7&&!tape.Calls.Any(c=>c.StartsWith("I:Dynamic:",StringComparison.Ordinal)),"successful float Count0 neither writes nor tries integer phase");
  scope.Dispose();check(tape.Disposals==1,"three optional reader interfaces share one owned provider disposal");
  Graph(check,output);
 }
 static void Graph(Action<bool,string> check,string output)
 {
  var plugin=new ReadCalibFilePlugin();var parameters=Parameters(plugin);var registry=new ModuleRegistry();registry.Register(plugin.Descriptor,plugin.CreateRuntime,plugin.DescribeNode);registry.Register(new Source().Descriptor,()=>new Source());
  var names=new[]{"CalibMatrix","Dynamic","DisplayInt","DisplayText","RefreshSignal"};
  var flow=new ProcedureDocument("extras","extras",[new("source",Source.Id,"1","source",[]),new("read",ReadCalibFilePlugin.Id,"1.0.0","read",parameters)],[]){Configuration=new(){Subscriptions=names.Select(n=>new WorkflowSubscription("read",n,false,new(WorkflowSourceKind.NodeOutput,"source",n))).ToArray()}};
  check(GraphValidator.Validate(flow,registry).IsValid,"real Graph validates explicit int/float/text opt-in arrays and int RefreshSignal");
  using var scope=new ModuleExecutionScope();var run=new GraphExecutor(registry).RunAsync(flow,output,scope:scope).Result;var read=run.Nodes.Single(n=>n.NodeId=="read");
  check(read.Status==NodeRunStatus.Success&&read.Outputs["ModuStatus"].As<long>()==1&&read.Outputs["Dynamic"].As<IReadOnlyList<double>>().SequenceEqual(new double[]{7,8}),"actual loader Graph uses raw integer array fallback and RefreshSignal first zero");
  check(read.Outputs["DisplayText"].As<string>()==new string('界',200)&&read.Outputs["DisplayInt"].Cardinality==ValueCardinality.Array,"actual loader Graph publishes text first item and int array with refreshed descriptor");
  var cached=new[]{run.Nodes.Single(n=>n.NodeId=="source")};var debug=new NodeDebugExecutor(registry).RunAsync(flow,"read",output,parameters,cached).Result;
  check(debug.Status==NodeRunStatus.Success&&debug.Outputs["DisplayText"].As<string>()==new string('界',200)&&debug.Outputs["Dynamic"].As<IReadOnlyList<double>>().Count==2,"actual loader cached debug preserves independent text and int callbacks");
  var selected=flow with{Configuration=flow.Configuration with{Subscriptions=flow.Configuration.Subscriptions.Select(b=>b.Target is "Dynamic" or "DisplayText"?b with{Source=b.Source with{ArrayIndex=new("1")}}:b).ToArray()}};
  using(var selectedScope=new ModuleExecutionScope()){var one=new GraphExecutor(registry).RunAsync(selected,output,scope:selectedScope).Result.Nodes.Single(n=>n.NodeId=="read");check(one.Status==NodeRunStatus.Success&&one.Outputs["Dynamic"].As<double>()==8&&one.Outputs["DisplayText"].As<string>()=="second","actual loader indexed int fallback/text successful count1 stops after chosen item");}
  var refresh=flow with{Configuration=flow.Configuration with{Subscriptions=flow.Configuration.Subscriptions.Select(b=>b.Target=="RefreshSignal"?b with{Source=b.Source with{ArrayIndex=new("1")}}:b).ToArray()}};
  using(var refreshScope=new ModuleExecutionScope()){var refreshed=new GraphExecutor(registry).RunAsync(refresh,output,scope:refreshScope).Result.Nodes.Single(n=>n.NodeId=="read");check(refreshed.Status==NodeRunStatus.Success&&refreshed.Outputs["ModuStatus"].As<long>()==0&&refreshed.Outputs.ContainsKey("Dynamic"),"int array selected nonzero RefreshSignal clears cache-only file while extra outputs remain independent of file ModuStatus");}
 }
 sealed class Tape:IToolNumericInputReader,IToolIntegerInputReader,IToolTextInputReader,IDisposable
 {
  internal Func<string,int,ToolNumericInputReadResult> Float=(n,i)=>new(0xe0000314,0,0);
  internal Func<string,int,ToolIntegerInputReadResult> Integer=(n,i)=>new(0xe0000314,0,0);
  internal Func<string,int,int,ToolTextInputReadResult> Text=(n,i,c)=>new(0xe0000314,0,"",0);
  internal List<string> Calls=[];internal int Disposals;
  ToolNumericInputReadResult IToolNumericInputReader.Read(ModuleExecutionContext context,string name,int index){Calls.Add($"F:{name}:{index}");return Float(name,index);}
  ToolIntegerInputReadResult IToolIntegerInputReader.Read(ModuleExecutionContext context,string name,int index){Calls.Add($"I:{name}:{index}");return Integer(name,index);}
  ToolTextInputReadResult IToolTextInputReader.Read(ModuleExecutionContext context,string name,int index,int capacity){Calls.Add($"S:{name}:{index}:{capacity}");return Text(name,index,capacity);}
  public void Dispose()=>Disposals++;
 }
 sealed class Source:IModule
 {
  internal const string Id="readcalib.extra-source";
  public ModuleDescriptor Descriptor{get;}=new(Id,"1","source",[],[new("CalibMatrix",DataKind.NumberArray),new PortDescriptor("Dynamic",DataKind.Integer){Cardinality=ValueCardinality.Array},new PortDescriptor("DisplayInt",DataKind.Integer){Cardinality=ValueCardinality.Array},new PortDescriptor("DisplayText",DataKind.Text){Cardinality=ValueCardinality.Array},new PortDescriptor("RefreshSignal",DataKind.Integer){Cardinality=ValueCardinality.Array}],[]);
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"CalibMatrix",PortValue.Numbers([10,20])},{"Dynamic",PortValue.IntegerArray([7,8])},{"DisplayInt",PortValue.IntegerArray([101,102,103])},{"DisplayText",PortValue.TextArray([new string('界',200),"second"])},{"RefreshSignal",PortValue.IntegerArray([0,1])}}));
 }
}
