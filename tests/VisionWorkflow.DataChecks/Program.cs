using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionModules;
using VisionEditor.Core;

var registry=new ModuleRegistry();BuiltinModules.RegisterAll(registry);
var checks=new List<object>();void Check(bool condition,string name,object? observed=null){if(!condition)throw new Exception(name);checks.Add(new{name,observed});}
async Task Reject(Func<Task> action,string name){try{await action();}catch(InvalidDataException){Check(true,name);return;}throw new Exception("Expected rejection: "+name);}
NodeDocument Node(string id,string module,params (string,object)[] values)=>new(id,module,BuiltinModules.Version,id,values.ToDictionary(v=>v.Item1,v=>JsonSerializer.SerializeToElement(v.Item2)));
var image=new GrayImage(7,3,new byte[]{0,0,0,0,0,0,0,0,100,0,0,0,100,0,0,0,0,0,0,0,0});
var config=new WorkflowConfiguration {
 Inputs=[new("image-in","image",DataKind.GrayImage)],Outputs=[new("count-out","count",DataKind.Integer,"components","Count"),new("image-out","image",DataKind.GrayImage,"threshold","Image")],
 Groups=[new("g","分组0")],Variables=[new("threshold-var","g","%threshold%","int","50")],
 Subscriptions=[new("threshold","Image",false,new(WorkflowSourceKind.WorkflowInput,"image-in")),new("threshold","Threshold",true,new(WorkflowSourceKind.LocalVariable,"threshold-var"))]
};
var procedure=new ProcedureDocument("a","流程A",[Node("threshold","vision.threshold",("Threshold",1)),Node("components","vision.components")],[new("threshold","Image","components","Image")],[new("threshold","components")]){Configuration=config};
var solution=new SolutionDocument(SolutionStore.CurrentVersion,"test","独立真实数据链",[procedure,procedure with{Id="b",Name="流程B"}],[]);
var workspace=new SolutionWorkspace(solution,Environment.CurrentDirectory);var executor=new SolutionExecutor(registry);
var first=await executor.RunAsync(workspace,"a",inputs:new Dictionary<string,PortValue>{{"image",PortValue.Image(image)}});
Check(first.Status==SessionStatus.Success&&first.WorkflowOutputs["count"].As<long>()==2,"external image -> subscribed threshold -> real components -> exported count",first.WorkflowOutputs["count"].Value);
Check(first.LocalVariables["%threshold%"].As<long>()==50,"local variable overrides stored literal 1",first.LocalVariables["%threshold%"].Value);
var callee=await executor.CallAsync(workspace,"b",first,new Dictionary<string,string>{{"image","image"}});
Check(callee.WorkflowOutputs["count"].As<long>()==2,"caller named image export injects callee named input",callee.WorkflowOutputs["count"].Value);
var changed=procedure with{Configuration=config with{Variables=[config.Variables[0] with{Value="0"}]}};
var second=await new GraphExecutor(registry).RunAsync(changed,".",inputs:new Dictionary<string,PortValue>{{"image",PortValue.Image(image)}});
Check(second.WorkflowOutputs["count"].As<long>()==1,"changing local initial value changes actual downstream count 2 -> 1",second.WorkflowOutputs["count"].Value);
var zero=await executor.RunAsync(workspace,"a",inputs:new Dictionary<string,PortValue>{{"image",PortValue.Image(new GrayImage(7,3,new byte[21]))}});
Check(zero.WorkflowOutputs["count"].As<long>()==0,"second invocation receives fresh image and no stale output",zero.WorkflowOutputs["count"].Value);
Check(first.WorkflowOutputs["count"].As<long>()==2,"completed caller result remains isolated");
await Reject(()=>executor.RunAsync(workspace,"a"),"missing required workflow argument rejected before modules");
await Reject(()=>executor.RunAsync(workspace,"a",inputs:new Dictionary<string,PortValue>{{"image",PortValue.Integer(3)}}),"workflow argument type mismatch rejected");
await Reject(()=>executor.RunAsync(workspace,"a",inputs:new Dictionary<string,PortValue>{{"oops",PortValue.Integer(3)}}),"unknown workflow argument rejected");
await Reject(()=>executor.CallAsync(workspace,"b",first,new Dictionary<string,string>{{"image","absent"}}),"unknown caller export rejected");
var invalid=procedure with{Configuration=config with{Variables=[config.Variables[0] with{Value="300"}]}};
var range=await new GraphExecutor(registry).RunAsync(invalid,".",inputs:new Dictionary<string,PortValue>{{"image",PortValue.Image(image)}});
Check(range.Status==SessionStatus.Failed&&range.Nodes[0].ErrorCode=="SubscriptionUnavailable"&&range.WorkflowOutputs.Count==0,"runtime subscribed parameter range enforced; failed run exports no stale values");
await Reject(()=>executor.CallAsync(workspace,"b",range,new Dictionary<string,string>{{"image","image"}}),"failed caller cannot invoke callee");
var bound=procedure with{Nodes=[..procedure.Nodes,Node("last","vision.threshold",("Threshold",0))],ExecutionLinks=[new("threshold","components"),new("components","last")],Configuration=config with{Variables=[..config.Variables,new("count-var","g","%count%","int","99",SourceNode:"components",SourcePort:"Count")],Subscriptions=[..config.Subscriptions,new("last","Threshold",true,new(WorkflowSourceKind.LocalVariable,"count-var")),new("last","Image",false,new(WorkflowSourceKind.WorkflowInput,"image-in"))],Outputs=[new("last-image","last",DataKind.GrayImage,"last","Image")]}};
var boundResult=await new GraphExecutor(registry).RunAsync(bound,".",inputs:new Dictionary<string,PortValue>{{"image",PortValue.Image(image)}});
Check(boundResult.Status==SessionStatus.Success&&boundResult.LocalVariables["%count%"].As<long>()==2,"module result writes local variable during same execution",boundResult.LocalVariables["%count%"].Value);
Check(boundResult.WorkflowOutputs["last"].As<GrayImage>().Pixels.SequenceEqual(Algorithms.Threshold(image,2).Pixels),"downstream parameter consumes updated count=2, not initial 99");
await Reject(()=>new GraphExecutor(registry).RunAsync(bound with{ExecutionLinks=[new("threshold","last"),new("last","components")]},".",inputs:new Dictionary<string,PortValue>{{"image",PortValue.Image(image)}}),"consumer preceding variable producer rejected");
var legacy=await new GraphExecutor(registry).RunAsync(bound with{ExecutionLinks=null},".",inputs:new Dictionary<string,PortValue>{{"image",PortValue.Image(image)}});
Check(legacy.Status==SessionStatus.Success&&legacy.Nodes.Select(n=>n.NodeId).SequenceEqual(new[]{"threshold","components","last"}),"legacy DAG includes variable producer dependency");
var wrong=procedure with{Configuration=config with{Subscriptions=[new("threshold","Image",false,new(WorkflowSourceKind.LocalVariable,"threshold-var"))]}};
Check(!GraphValidator.Validate(wrong,registry).IsValid,"scalar/image subscription incompatible");
var duplicate=procedure with{Configuration=config with{Subscriptions=[..config.Subscriptions,config.Subscriptions[0]]}};
Check(!GraphValidator.Validate(duplicate,registry).IsValid,"duplicate target subscription rejected");
await Reject(()=>executor.CallAsync(workspace,"b",first,new Dictionary<string,string>{{"image","count"}}),"caller scalar export cannot be injected as image");
var failedBound=await new GraphExecutor(registry).RunAsync(bound with{Configuration=bound.Configuration with{Variables=bound.Configuration.Variables.Select(v=>v.Id=="threshold-var"?v with{Value="300"}:v).ToArray()}},".",inputs:new Dictionary<string,PortValue>{{"image",PortValue.Image(image)}});
Check(failedBound.Status==SessionStatus.Failed&&!failedBound.LocalVariables.ContainsKey("%count%")&&failedBound.WorkflowOutputs.Count==0,"failed producer does not restore bound variable initial99 or previous value2");
var namedVariable=procedure with{Configuration=config with{Variables=[config.Variables[0] with{Name="%renamed%"}]}};
var renamed=await new GraphExecutor(registry).RunAsync(namedVariable,".",inputs:new Dictionary<string,PortValue>{{"image",PortValue.Image(image)}});
Check(renamed.WorkflowOutputs["count"].As<long>()==2&&renamed.LocalVariables.ContainsKey("%renamed%"),"rename keeps typed reference stable by identity");
var badIdentity=procedure with{Configuration=config with{Variables=[]}};
Check(!GraphValidator.Validate(badIdentity,registry).IsValid,"dangling variable references rejected before execution");

// A genuinely bound workflow input may use a current-run module result as its default source.
var fallback=new ProcedureDocument("fallback","fallback",[Node("source","vision.threshold",("Threshold",50)),Node("target","vision.components")],[],[new("source","target")]){Configuration=new(){Inputs=[new("raw","raw",DataKind.GrayImage),new("mapped","mapped",DataKind.GrayImage,"source","Image")],Outputs=[new("count","count",DataKind.Integer,"target","Count")],Subscriptions=[new("source","Image",false,new(WorkflowSourceKind.WorkflowInput,"raw")),new("target","Image",false,new(WorkflowSourceKind.WorkflowInput,"mapped"))]}};
var defaultMapped=await new GraphExecutor(registry).RunAsync(fallback,".",inputs:new Dictionary<string,PortValue>{{"raw",PortValue.Image(image)}});
Check(defaultMapped.WorkflowOutputs["count"].As<long>()==2,"workflow input default subscription reads current-run source output");
var explicitOverride=await new GraphExecutor(registry).RunAsync(fallback,".",inputs:new Dictionary<string,PortValue>{{"raw",PortValue.Image(image)},{"mapped",PortValue.Image(new GrayImage(7,3,new byte[21]))}});
Check(explicitOverride.WorkflowOutputs["count"].As<long>()==0,"explicit call argument overrides default workflow input subscription");
string output=Path.GetFullPath("analysis/full-audit/workflow-data-runtime");Directory.CreateDirectory(output);string file=Path.Combine(output,"operated.vmx.json");SolutionStore.Save(file,solution);
var loaded=SolutionStore.Load(file);var persisted=await executor.RunAsync(new(loaded,"."),"a",inputs:new Dictionary<string,PortValue>{{"image",PortValue.Image(image)}});
Check(persisted.WorkflowOutputs["count"].As<long>()==2,"saved subscription executes identically after reload");
var editor=new GraphEditorDocument(registry);editor.Load(file);var copied=editor.DuplicateProcedure("a");var cloned=await executor.RunAsync(new(editor.Document,"."),copied,inputs:new Dictionary<string,PortValue>{{"image",PortValue.Image(image)}});
Check(cloned.WorkflowOutputs["count"].As<long>()==2,"copied workflow remaps target nodes and preserves scoped references");
editor.DeleteNodes([editor.CurrentProcedure.Nodes.First().Id]);Check(editor.CurrentProcedure.Configuration.Subscriptions.Count==0,"deleted consumer clears subscriptions atomically");editor.Undo();
Check(editor.CurrentProcedure.Configuration.Subscriptions.Count==2,"undo restores data subscriptions");
var interactive=procedure with{Id="image-analysis",Name="图像分析",Nodes=[Node("source","vision.image-file",("Path","../analysis/images/circle-find-sample.bmp")) with{X=160,Y=90},procedure.Nodes[0] with{X=160,Y=210},procedure.Nodes[1] with{X=160,Y=330}],Connections=[new("source","Image","threshold","Image"),..procedure.Connections],ExecutionLinks=[new("source","threshold"),new("threshold","components")],Configuration=config with{Inputs=[],Variables=[..config.Variables,new("observed-count","g","%count%","int","0",SourceNode:"components",SourcePort:"Count")],Subscriptions=config.Subscriptions.Where(b=>b.IsParameter).ToArray(),RunParameters=[new("threshold-editor","threshold","Threshold","阈值") ]}};
var demonstration=solution with{Id="workflow-data-demo",Name="真实局部变量与流程调用",Procedures=[interactive,procedure with{Id="call-target",Name="外部图像调用",Configuration=config with{RunParameters=[new("input-editor","threshold","Image","输入图像",true),new("threshold-editor","threshold","Threshold","阈值")]}}]};
string demo=Path.GetFullPath("examples/workflow-data-bindings.vmx.json");SolutionStore.Save(demo,demonstration);var realFileResult=await executor.RunAsync(SolutionStore.OpenWorkspace(demo),"image-analysis");
Check(realFileResult.Status==SessionStatus.Success&&realFileResult.WorkflowOutputs["image"].Kind==DataKind.GrayImage,"interactive sample loads real BMP and exports its processed image",realFileResult.WorkflowOutputs["count"].Value);
var realChanged=await new GraphExecutor(registry).RunAsync(interactive with{Configuration=interactive.Configuration with{Variables=interactive.Configuration.Variables.Select(v=>v.Id=="threshold-var"?v with{Value="0"}:v).ToArray()}},Path.GetDirectoryName(demo)!);
Check(realChanged.WorkflowOutputs["count"].As<long>()==1,"real BMP sample local threshold 50 -> 0 changes exported count 365 -> 1",realChanged.WorkflowOutputs["count"].Value);
var realCall=await executor.CallAsync(SolutionStore.OpenWorkspace(demo),"call-target",realFileResult,new Dictionary<string,string>{{"image","image"}});
Check(realCall.Status==SessionStatus.Success,"interactive sample output invokes second real workflow",realCall.WorkflowOutputs["count"].Value);
File.WriteAllText(Path.Combine(output,"results.json"),JsonSerializer.Serialize(checks,new JsonSerializerOptions{WriteIndented=true}));
Console.WriteLine($"PASS {checks.Count} real workflow data checks; counts: first=2, changed=1, fresh=0; same-run bound variable=2.");
