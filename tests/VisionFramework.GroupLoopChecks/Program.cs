using System.Text.Json;
using VisionFramework;
using VisionEditor.Core;
var checks=0;void Check(bool value,string name){if(!value)throw new Exception(name);Console.WriteLine("PASS "+name);checks++;}
var registry=new ModuleRegistry();WorkflowCalls.Register(registry);var indices=new List<long>();
var descriptor=new ModuleDescriptor("test.index","1","index",[new("index",DataKind.Integer)],[new("value",DataKind.Integer)],[]);
registry.Register(descriptor,()=>new IndexModule(descriptor,indices));
var internalNode=new NodeDocument("probe",descriptor.Id,descriptor.Version,"probe",new());
var body=new ProcedureDocument("body","Group",[internalNode],[],[]){GroupOwner=new("main","group"),Configuration=new(){Subscriptions=[new("probe","index",false,new(WorkflowSourceKind.GroupLoopIndex,"LoopIndex"))],Outputs=[new("out","out",DataKind.Integer,"probe","value")]}};
NodeDocument Group(int start,int end,bool enabled=true,int gap=0)=>new("group",WorkflowCalls.ModuleId,WorkflowCalls.Version,"Group",new(){["EnableLoop"]=JsonSerializer.SerializeToElement(enabled),["StartIndex"]=JsonSerializer.SerializeToElement(start),["EndIndex"]=JsonSerializer.SerializeToElement(end),["LoopTimeGap"]=JsonSerializer.SerializeToElement(gap)}){Call=WorkflowCalls.From(body)};
SolutionWorkspace Workspace(NodeDocument group,ProcedureDocument? replacement=null)=>new(new(SolutionStore.CurrentVersion,"group-test","group-test",[new("main","main",[group],[],[]),replacement??body],[]),Environment.CurrentDirectory);
var executor=new SolutionExecutor(registry);
var run=await executor.RunAsync(Workspace(Group(2,5)),"main");
Check(run.Status==SessionStatus.Success&&indices.SequenceEqual(new long[]{2,3,4}),"owned Group range [2,5) runs actual modules three times");
Check(run.Calls["group"].CompletedGroupIterations==3&&run.Calls["group"].GroupLoopIndex==4&&run.Calls["group"].WorkflowOutputs["out"].As<long>()==4,"last result and invocation index reflect final iteration");
Check(!WorkflowCalls.From(body).Outputs.Any(p=>p.Name=="LoopIndex"),"private LoopIndex is not a public output");
var debug=await new NodeDebugExecutor(registry).RunAsync(body,"probe",".",new Dictionary<string,JsonElement>(),groupLoopIndex:4);
Check(debug.Status==NodeRunStatus.Success&&debug.Outputs["value"].As<long>()==4,"isolated debug uses explicit owned invocation index snapshot");
indices.Clear();run=await executor.RunAsync(Workspace(Group(2,5,false)),"main");Check(indices.SequenceEqual(new long[]{0})&&run.Calls["group"].CompletedGroupIterations==1,"disabled loop uses default one-shot index zero");
indices.Clear();run=await executor.RunAsync(Workspace(Group(2,2)),"main");Check(indices.Count==0&&run.Status==SessionStatus.Failed,"empty range does not execute or fabricate required output");
indices.Clear();run=await executor.RunAsync(Workspace(Group(5,2)),"main");Check(indices.Count==0,"nonascending range does not invent descending iterations");
indices.Clear();run=await executor.RunAsync(Workspace(Group(-2,1)),"main");Check(indices.SequenceEqual(new long[]{-2,-1,0}),"signed loop range is preserved");
var invalid=GraphValidator.Validate(body with{GroupOwner=null},registry);Check(!invalid.IsValid,"private source rejected outside owned Group");
invalid=GraphValidator.Validate(body with{Configuration=body.Configuration with{Subscriptions=[new("probe","index",false,new(WorkflowSourceKind.GroupLoopIndex,"Other"))]}},registry);Check(!invalid.IsValid,"private source identity validated");
var direct=await new GraphExecutor(registry).RunAsync(body,".");Check(direct.Status==SessionStatus.Failed&&direct.Nodes[0].ErrorCode=="SubscriptionUnavailable","direct body execution cannot forge LoopIndex context");
var parent=Workspace(Group(0,1)).Document.Procedures[0] with{Configuration=new(){Groups=[new("g","g")],Variables=[new("start","g","start","int","3"),new("end","g","end","int","6")],Subscriptions=[new("group","StartIndex",true,new(WorkflowSourceKind.LocalVariable,"start")),new("group","EndIndex",true,new(WorkflowSourceKind.LocalVariable,"end"))]}};
indices.Clear();run=await executor.RunAsync(new(Workspace(Group(0,1)).Document with{Procedures=[parent,body]},Environment.CurrentDirectory),"main");Check(indices.SequenceEqual(new long[]{3,4,5}),"parent local-variable subscriptions control actual Group range");
indices.Clear();run=await executor.RunAsync(Workspace(Group(0,5,gap:100) with{TimeoutMs=30}),"main");Check(run.Status==SessionStatus.TimedOut&&indices.Count==1,"parent timeout cancels interval before another iteration");
indices.Clear();using(var cancel=new CancellationTokenSource(30)){run=await executor.RunAsync(Workspace(Group(0,5,gap:100)),"main",cancellationToken:cancel.Token);Check(run.Status==SessionStatus.Cancelled&&indices.Count==1,"parent cancellation stops interval without another iteration");}
var path=Path.GetFullPath("artifacts/group-loop-roundtrip.vmx.json");SolutionStore.Save(path,Workspace(Group(2,5)).Document);indices.Clear();run=await executor.RunAsync(SolutionStore.OpenWorkspace(path),"main");Check(indices.SequenceEqual(new long[]{2,3,4}),"save and reopen preserves range and private subscription");
var editor=new GraphEditorDocument(registry);editor.Load(path);editor.SetParameter("group","EndIndex",JsonSerializer.SerializeToElement(7));editor.Undo();Check(editor.CurrentProcedure.Nodes.Single().Parameters["EndIndex"].GetInt32()==5,"loop parameter edit participates in undo");
var nestedBody=body with{Id="nested-body",GroupOwner=new("body","inner")};
var inner=Group(0,2) with{Id="inner",Call=WorkflowCalls.From(nestedBody)};
var outerBody=body with{Nodes=[inner],Configuration=new(){Outputs=[new("out","out",DataKind.Integer,"inner","out")]}};
var nestedWorkspace=Workspace(Group(0,3),outerBody);nestedWorkspace=new(nestedWorkspace.Document with{Procedures=[..nestedWorkspace.Document.Procedures,nestedBody]},Environment.CurrentDirectory);
indices.Clear();run=await executor.RunAsync(nestedWorkspace,"main");Check(run.Status==SessionStatus.Success&&indices.SequenceEqual(new long[]{0,1,0,1,0,1}),"nested owned Groups reset private index independently on each invocation");
Console.WriteLine($"PASS {checks} Group loop checks");
sealed class IndexModule(ModuleDescriptor descriptor,List<long> indices):IModule{public ModuleDescriptor Descriptor=>descriptor;public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token){var index=context.Inputs["index"].As<long>();indices.Add(index);return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"value",PortValue.Integer(index)}}));}}

