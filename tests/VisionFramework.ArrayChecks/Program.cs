using System.Text.Json;
using VisionFramework;
using VisionEditor.Core;
var checks=new List<string>();
void Check(bool ok,string name){if(!ok)throw new Exception(name);checks.Add(name);}
void Reject(Action action,string name){bool rejected=false;try{action();}catch(Exception e)when(e is ArgumentException or InvalidDataException or InvalidOperationException){rejected=true;}Check(rejected,name);}
PortDescriptor Vector(string name,DataKind kind)=>new(name,kind){Cardinality=ValueCardinality.Array};
NodeDocument Node(string id,string module)=>new(id,module,"1",id,new());
var original=new long[]{2,4,6};var integers=PortValue.IntegerArray(original);original[0]=99;
Check(integers.Kind==DataKind.Integer&&integers.Cardinality==ValueCardinality.Array&&integers.As<IReadOnlyList<long>>().SequenceEqual(new long[]{2,4,6}),"integer array preserves base type and immutable snapshot");
Check(((ICollection<long>)integers.Value).IsReadOnly,"array payload cannot be edited through collection interface");
Reject(()=>integers.As<long>(),"array cannot be silently read as scalar");
Check(PortValue.IntegerArray([]).As<IReadOnlyList<long>>().Count==0,"empty array is present output rather than missing output");
Reject(()=>PortValue.NumberArray([double.NaN]),"NaN array rejected");Reject(()=>PortValue.NumberArray([double.PositiveInfinity]),"infinite array rejected");Reject(()=>PortValue.TextArray([null!]),"null text element rejected");
Check(PortValue.Integer(5).Cardinality==ValueCardinality.Scalar,"legacy scalar factory remains scalar");
var registry=new ModuleRegistry();WorkflowCalls.Register(registry);
var producer=new ModuleDescriptor("test.vector","1","vector",[],[Vector("values",DataKind.Integer)],[]);
registry.Register(producer,()=>new FunctionModule(producer,_=>new(ModuleStatus.Success,new Dictionary<string,PortValue>{{"values",integers}})));
var sink=new ModuleDescriptor("test.sum","1","sum",[Vector("values",DataKind.Integer)],[new("sum",DataKind.Integer)],[]);
registry.Register(sink,()=>new FunctionModule(sink,c=>new(ModuleStatus.Success,new Dictionary<string,PortValue>{{"sum",PortValue.Integer(c.Input<IReadOnlyList<long>>("values").Sum())}})));
var pipeline=new ProcedureDocument("main","array pipeline",[Node("p",producer.Id),Node("s",sink.Id)],[new("p","values","s","values")]);
var result=await new GraphExecutor(registry).RunAsync(pipeline,".");Check(result.Status==SessionStatus.Success&&result.Nodes.Single(n=>n.NodeId=="s").Outputs["sum"].As<long>()==12,"array values actually propagate through DAG into numeric consumer");
var scalarSink=sink with{Id="test.scalar",Inputs=[new("values",DataKind.Integer)]};registry.Register(scalarSink,()=>new FunctionModule(scalarSink,_=>throw new Exception("must not execute")));
var mismatched=pipeline with{Nodes=[pipeline.Nodes[0],Node("s",scalarSink.Id)]};Check(GraphValidator.Validate(mismatched,registry).Issues.Any(i=>i.Code=="PortType"),"same base kind scalar-array connection rejected before execution");
var wrong=producer with{Id="test.wrong",Outputs=[new("values",DataKind.Integer)]};registry.Register(wrong,()=>new FunctionModule(wrong,_=>new(ModuleStatus.Success,new Dictionary<string,PortValue>{{"values",integers}})));
var wrongResult=await new GraphExecutor(registry).RunAsync(new("wrong","wrong",[Node("wrong",wrong.Id)],[],[]),".");Check(wrongResult.Nodes.Single().ErrorCode=="InvalidOutput"&&wrongResult.Nodes.Single().Outputs.Count==0,"undeclared array output fails without exposing invalid data");
var subscribed=pipeline with{Connections=[],ExecutionLinks=[new("p","s")],Configuration=new(){Subscriptions=[new("s","values",false,new(WorkflowSourceKind.NodeOutput,"p","values"))]}};
var subResult=await new GraphExecutor(registry).RunAsync(subscribed,".");Check(subResult.Nodes.Single(n=>n.NodeId=="s").Outputs["sum"].As<long>()==12,"typed subscription propagates array into actual consumer");
var paramDescriptor=new ModuleDescriptor("test.parameter","1","parameter",[],[],[new("n",ParameterKind.Integer,true)]);registry.Register(paramDescriptor,()=>new FunctionModule(paramDescriptor,_=>new(ModuleStatus.Success,new Dictionary<string,PortValue>())));
var paramPipeline=new ProcedureDocument("param","param",[Node("p",producer.Id),Node("s",paramDescriptor.Id)],[],[new("p","s")]){Configuration=new(){Subscriptions=[new("s","n",true,new(WorkflowSourceKind.NodeOutput,"p","values"))]}};
Check(!GraphValidator.Validate(paramPipeline,registry).IsValid,"array source cannot subscribe to scalar parameter");Reject(()=>WorkflowSubscriptions.Parameter(paramDescriptor.Parameters[0],integers),"runtime parameter conversion also rejects array rather than selecting first element");
var echo=new ModuleDescriptor("test.echo","1","echo",[Vector("values",DataKind.Integer)],[Vector("values",DataKind.Integer)],[]);registry.Register(echo,()=>new FunctionModule(echo,c=>new(ModuleStatus.Success,new Dictionary<string,PortValue>{{"values",c.Inputs["values"]}})));
var body=new ProcedureDocument("body","Group",[Node("echo",echo.Id)],[],[]){GroupOwner=new("root","group"),Configuration=new(){Inputs=[new("in","items",DataKind.Integer){Cardinality=ValueCardinality.Array}],Outputs=[new("out","items",DataKind.Integer,"echo","values"){Cardinality=ValueCardinality.Array}],Subscriptions=[new("echo","values",false,new(WorkflowSourceKind.WorkflowInput,"in"))]}};
Reject(()=>new GraphExecutor(registry).RunAsync(body,".",inputs:new Dictionary<string,PortValue>{{"items",PortValue.Integer(1)}}).GetAwaiter().GetResult(),"workflow runtime rejects scalar argument for array input");
var emptyBody=await new GraphExecutor(registry).RunAsync(body,".",inputs:new Dictionary<string,PortValue>{{"items",PortValue.IntegerArray([])}});Check(emptyBody.Status==SessionStatus.Success&&emptyBody.WorkflowOutputs["items"].As<IReadOnlyList<long>>().Count==0,"workflow runtime preserves present empty input array");
var group=new NodeDocument("group",WorkflowCalls.ModuleId,WorkflowCalls.Version,"Group",new()){Call=WorkflowCalls.From(body)};
var root=new ProcedureDocument("root","root",[Node("p",producer.Id),group,Node("s",sink.Id)],[new("p","values","group","items"),new("group","items","s","values")]);
var document=new SolutionDocument(SolutionStore.CurrentVersion,"array-contract","array-contract",[root,body],[]);
var grouped=await new SolutionExecutor(registry).RunAsync(new(document,Environment.CurrentDirectory),"root");Check(grouped.Status==SessionStatus.Success&&grouped.Nodes.Single(n=>n.NodeId=="s").Outputs["sum"].As<long>()==12,"owned Group receives and returns array into downstream runtime");
Check(grouped.Calls["group"].WorkflowOutputs["items"].Cardinality==ValueCardinality.Array,"Group output preserves cardinality");
Reject(()=>new WorkflowConfiguration{Inputs=[new("bad","bad",DataKind.GrayImage){Cardinality=ValueCardinality.Array}]}.Validate(new("bad","bad",[],[],[])),"unsupported image array rejected explicitly");
Reject(()=>registry.Register(new("bad-array","1","bad",[],[Vector("v",DataKind.Circle)],[]),()=>throw new Exception()),"unsupported geometric array descriptor rejected");
Reject(()=>new WorkflowConfiguration{Groups=[new("g","g")],Variables=[new("v","g","v","int","0","","p","values")]}.Validate(pipeline,registry),"array output cannot bind to scalar local variable");
var output=Path.GetFullPath("analysis/full-audit/array-contract-runtime");Directory.CreateDirectory(output);var path=Path.Combine(output,"roundtrip.vmx.json");SolutionStore.Save(path,document);var loaded=SolutionStore.Load(path);
Check(loaded.Procedures.Single(p=>p.Id=="body").Configuration.Outputs.Single().Cardinality==ValueCardinality.Array,"array interface survives save and reopen");
var reopened=await new SolutionExecutor(registry).RunAsync(new(loaded,Environment.CurrentDirectory),"root");Check(reopened.Nodes.Single(n=>n.NodeId=="s").Outputs["sum"].As<long>()==12,"reopened owned Group executes array pipeline");
Check(WorkflowCalls.Describe(group).Inputs.Single().Cardinality==ValueCardinality.Array,"dynamic Group descriptor preserves array input shape");
var badShape=document with{Procedures=[root,body with{Configuration=body.Configuration with{Inputs=[body.Configuration.Inputs[0] with{Cardinality=ValueCardinality.Scalar}]}}]};Reject(()=>WorkflowCalls.Validate(badShape,"root"),"stale Group cardinality signature rejected");
foreach(var sample in new[]{PortValue.NumberArray([1.5,2.5]),PortValue.TextArray(["one","二"]),PortValue.IntegerArray([])}){
 var id="test.shape."+checks.Count;var descriptor=new ModuleDescriptor(id,"1",id,[],[Vector("v",sample.Kind)],[]);registry.Register(descriptor,()=>new FunctionModule(descriptor,_=>new(ModuleStatus.Success,new Dictionary<string,PortValue>{{"v",sample}})));
 var run=await new GraphExecutor(registry).RunAsync(new(id,id,[Node("v",id)],[],[]),".");Check(run.Status==SessionStatus.Success&&ReferenceEquals(run.Nodes[0].Outputs["v"],sample),"validated runtime output "+sample.Kind+" "+checks.Count);
}
File.WriteAllText(Path.Combine(output,"results.json"),JsonSerializer.Serialize(new{passed=true,count=checks.Count,checks,scope="independent runtime contracts, not native loop parity"},new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine($"PASS {checks.Count} array contract checks; owned Group array sum=12; runtime, subscriptions and roundtrip.");
sealed class FunctionModule(ModuleDescriptor descriptor,Func<ModuleExecutionContext,ModuleResult> run):IModule{public ModuleDescriptor Descriptor=>descriptor;public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token){token.ThrowIfCancellationRequested();return ValueTask.FromResult(run(context));}}
