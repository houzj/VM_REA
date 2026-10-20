using System.Text.Json;
using VisionFramework;
using VisionPluginContracts;
int checks=0;void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
var descriptor=new ModuleDescriptor("sample","1","Sample",[],[new("N",DataKind.Number),new("I",DataKind.Integer),new("A",DataKind.NumberArray),new PortDescriptor("Batch",DataKind.Number){Cardinality=ValueCardinality.Array},new PortDescriptor("IBatch",DataKind.Integer){Cardinality=ValueCardinality.Array},new("Text",DataKind.Text)],[]);
var registry=new ModuleRegistry();registry.Register(descriptor,()=>new Fake(descriptor));
NodeDocument Node(string id)=>new(id,"sample","1",id,new());
var current=new ProcedureDocument("p","P",[Node("before"),Node("target"),Node("after"),Node("unrelated")],[],[new("before","target"),new("target","after")]);
IReadOnlyDictionary<string,JsonElement> Read()=>current.Nodes.Single(n=>n.Id=="target").Parameters;
bool reject=false;void Apply(Dictionary<string,JsonElement> values){if(reject)return;current=current with{Nodes=current.Nodes.Select(n=>n.Id=="target"?n with{Parameters=values}:n).ToArray()};}
var service=new ToolConfigurationDisplaySubscriptions(new Fallback(),()=>current,registry,"target",Read,Apply);
var destination=new ConfigurationSubscriptionDestination(ConfigurationSubscriptionDomain.Display,ToolDisplayService.PositionXInput,"int;float");
ConfigurationSubscriptionBinding Output(string node="target",string port="N",string native="float",string procedure="p")=>new(new ConfigurationOutputReference(procedure,node,port,native),ConfigurationSubscriptionIndex.All);
ConfigurationSubscriptionBinding Constant(string value,string native="float")=>new(new ConfigurationConstantReference(native,value),ConfigurationSubscriptionIndex.All);
IEnumerable<ConfigurationSubscriptionTreeNode> Leaves(IEnumerable<ConfigurationSubscriptionTreeNode> rows)=>rows.SelectMany(r=>r.Children.Count==0?[r]:Leaves(r.Children));
var tree=service.Enumerate(new(destination,["int","float"],IncludeSelf:true));
Check(tree.Status==ConfigurationSubscriptionStatus.Available,"display tree");
var leaves=Leaves(tree.Roots).Where(n=>n.Binding is not null).ToArray();Check(leaves.Length==10,"scalar and array numeric self/predecessor tree");
Check(leaves.Select(n=>((ConfigurationOutputReference)n.Binding!.Reference).NodeId).ToHashSet().SetEquals(["target","before"]),"explicit ancestors");
Check(service.Bind(destination,Output()).Status==ConfigurationSubscriptionStatus.Available,"self output bind");
Check(current.Configuration.Subscriptions.Count==0&&current.Connections.Count==0,"no algorithm self cycle");
Check(service.Read(destination).Binding==Output(),"typed read roundtrip");
var persisted=JsonSerializer.Serialize(current);current=JsonSerializer.Deserialize<ProcedureDocument>(persisted)!;Check(service.Read(destination).Binding==Output(),"document JSON persistence");
Check(service.Bind(destination,Output("after")).Status!=ConfigurationSubscriptionStatus.Available,"reject descendant");
Check(service.Bind(destination,Output("unrelated")).Status!=ConfigurationSubscriptionStatus.Available,"reject unrelated");
Check(service.Bind(destination,Output(procedure:"other")).Status!=ConfigurationSubscriptionStatus.Available,"reject other flow");
Check(service.Bind(destination,Output(port:"A")).Status==ConfigurationSubscriptionStatus.Available,"accept legacy NumberArray");
Check(service.Bind(destination,Output(port:"Batch")).Status==ConfigurationSubscriptionStatus.Available,"accept typed number array");
Check(service.Bind(destination,new(Output().Reference,ConfigurationSubscriptionIndex.At(0))).Status==ConfigurationSubscriptionStatus.Unsupported,"reject index");
Check(service.Bind(destination,new(new ConfigurationGlobalReference("g","float"),ConfigurationSubscriptionIndex.All)).Status==ConfigurationSubscriptionStatus.Unsupported,"reject global");
Check(service.Bind(destination,Constant("NaN")).Status!=ConfigurationSubscriptionStatus.Available,"reject nonfinite");
Check(service.Bind(destination,Constant("12.5")).Status==ConfigurationSubscriptionStatus.Available,"constant bind");
var frame=new ToolDisplayFrame(null,"", "#ffffff",10,1,91,92,"",[]);
foreach(var sample in new[]{("A","float",PortValue.Numbers([12.5,91])),("Batch","float",PortValue.NumberArray([14.5,92])),("IBatch","int",PortValue.IntegerArray([17,93]))}){
 Check(service.Bind(destination,Output(port:sample.Item1,native:sample.Item2)).Status==ConfigurationSubscriptionStatus.Available,"array source bind "+sample.Item1);
 var document=JsonSerializer.Serialize(current);current=JsonSerializer.Deserialize<ProcedureDocument>(document)!;
 Check(service.Read(destination).Binding==Output(port:sample.Item1,native:sample.Item2),"exact array source identity persists "+sample.Item1);
 var shown=ToolConfigurationDisplaySubscriptions.ApplyPositions(frame,current.Nodes.Single(n=>n.Id=="target"),"p",(_,_)=>sample.Item3);
 double expected=sample.Item1=="A"?12.5:sample.Item1=="Batch"?14.5:17;
 Check(shown.PositionX==expected&&shown.PositionY==92,"array index0 consumed and unbound Y retained "+sample.Item1);
 PortValue empty=sample.Item1=="A"?PortValue.Numbers([]):sample.Item1=="Batch"?PortValue.NumberArray([]):PortValue.IntegerArray([]);
 int errors=0;shown=ToolConfigurationDisplaySubscriptions.ApplyPositions(frame,current.Nodes.Single(n=>n.Id=="target"),"p",(_,_)=>empty,_=>errors++);
 Check(shown.PositionX==0&&errors==1,"empty bound array falls back0 with diagnostic "+sample.Item1);
 errors=0;shown=ToolConfigurationDisplaySubscriptions.ApplyPositions(frame,current.Nodes.Single(n=>n.Id=="target"),"p",(_,_)=>null,_=>errors++);
 Check(shown.PositionX==0&&errors==1,"missing array falls back0 with diagnostic "+sample.Item1);
 Check(current.Configuration.Subscriptions.Count==0,"display array does not add algorithm edge "+sample.Item1);
}
Check(service.Bind(destination,Constant("12.5")).Status==ConfigurationSubscriptionStatus.Available,"restore scalar constant compatibility");

var resolved=ToolConfigurationDisplaySubscriptions.ApplyPositions(frame,current.Nodes.Single(n=>n.Id=="target"),"p",(_,_)=>null);
Check(resolved.PositionX==12.5&&resolved.PositionY==92,"constant display and unbound default");
reject=true;Check(service.Bind(destination,Constant("6")).Status==ConfigurationSubscriptionStatus.Failed,"readonly apply cannot fake success");reject=false;
Check(service.Read(destination).Binding==Constant("12.5"),"failed write retained");
Check(service.Bind(destination,Output()).Status==ConfigurationSubscriptionStatus.Available,"replace self output");
resolved=ToolConfigurationDisplaySubscriptions.ApplyPositions(frame,current.Nodes.Single(n=>n.Id=="target"),"p",(n,p)=>n=="target"&&p=="N"?PortValue.Number(17):null);Check(resolved.PositionX==17,"real self output");
int diagnostics=0;resolved=ToolConfigurationDisplaySubscriptions.ApplyPositions(frame,current.Nodes.Single(n=>n.Id=="target"),"p",(_,_)=>null,_=>diagnostics++);Check(resolved.PositionX==0&&diagnostics==1,"missing cache resets stale position");
resolved=ToolConfigurationDisplaySubscriptions.ApplyPositions(frame,current.Nodes.Single(n=>n.Id=="target"),"other",(_,_)=>PortValue.Number(99));Check(resolved.PositionX==0,"no cross procedure cache");
Check(service.Bind(destination,null).Status==ConfigurationSubscriptionStatus.Available&&service.Read(destination).Binding is null,"clear binding");
var old=current;current=current with{Id="other"};Check(service.Read(destination).Status!=ConfigurationSubscriptionStatus.Available,"closed procedure scoped service");current=old with{Nodes=old.Nodes.Where(n=>n.Id!="target").ToArray()};Check(service.Bind(destination,Constant("1")).Status!=ConfigurationSubscriptionStatus.Available,"removed node blocks write");
Check(service.Read(new(ConfigurationSubscriptionDomain.Parameter,"P","float")).Status==ConfigurationSubscriptionStatus.Unsupported,"delegates other domains");
Console.WriteLine($"PASS {checks} display array subscription checks");
sealed class Fake(ModuleDescriptor descriptor):IModule{public ModuleDescriptor Descriptor=>descriptor;public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext c,CancellationToken t)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>()));}
sealed class Fallback:IToolConfigurationSubscriptionService{public ConfigurationSubscriptionTreeResult Enumerate(ConfigurationSubscriptionQuery q)=>new(ConfigurationSubscriptionStatus.Unsupported,[]);public ConfigurationSubscriptionBindingResult Read(ConfigurationSubscriptionDestination d)=>new(ConfigurationSubscriptionStatus.Unsupported,null);public ConfigurationSubscriptionBindingResult Bind(ConfigurationSubscriptionDestination d,ConfigurationSubscriptionBinding? b)=>new(ConfigurationSubscriptionStatus.Unsupported,null);}
