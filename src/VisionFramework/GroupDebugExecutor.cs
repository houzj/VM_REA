using System.Collections.ObjectModel;
namespace VisionFramework;
public sealed record GroupPreparedInvocation(NodeDocument Node,IReadOnlyDictionary<string,PortValue> Inputs);
public sealed class GroupDebugExecutor(ModuleRegistry registry){
 // ModuleFrame JustRunModuleSelf updates topology/parameter bindings before node execution.
 // Independent typed preparation uses explicit caller snapshots; native cache ABI is not reproduced.
 public GroupPreparedInvocation Prepare(SolutionWorkspace workspace,string procedureId,string nodeId,IReadOnlyList<NodeRunRecord>? cachedResults=null,IReadOnlyDictionary<string,PortValue>? workflowInputs=null,int? groupLoopIndex=null){
  var parent=workspace.Document.Procedures.Single(p=>p.Id==procedureId);var node=parent.Nodes.Single(n=>n.Id==nodeId);if(node.ModuleId!=WorkflowCalls.ModuleId||node.Call is null||!node.Enabled)throw new InvalidDataException("Group不可独立运行。");
  var records=(cachedResults??[]).Where(r=>r.NodeId!=nodeId).ToDictionary(r=>r.NodeId);var scoped=parent with{Configuration=parent.Configuration with{Subscriptions=parent.Configuration.Subscriptions.Where(b=>b.NodeId==nodeId).ToArray()}};var context=new WorkflowRunContext(scoped,workflowInputs,records,groupLoopIndex);foreach(var id in records.Keys)context.Update(id);
  var inputs=new Dictionary<string,PortValue>();var errors=new List<string>();var parameters=node.Parameters.ToDictionary(v=>v.Key,v=>v.Value.Clone());var descriptor=registry.GetDescriptor(node);foreach(var parameter in descriptor.Parameters)if(!parameters.ContainsKey(parameter.Name)&&parameter.DefaultValue is {} value)parameters[parameter.Name]=value.Clone();
  void Resolve(string target,Action assign){try{assign();}catch(Exception e)when(e is InvalidDataException or OverflowException or InvalidOperationException or KeyNotFoundException){errors.Add(target+": "+e.Message);}}
  foreach(var edge in parent.Connections.Where(e=>e.ToNode==nodeId))Resolve(edge.ToPort,()=>inputs.Add(edge.ToPort,context.Resolve(new(WorkflowSourceKind.NodeOutput,edge.FromNode,edge.FromPort))));
  foreach(var binding in scoped.Configuration.Subscriptions)Resolve(binding.Target,()=>{var value=context.Resolve(binding.Source);if(binding.IsParameter)parameters[binding.Target]=WorkflowSubscriptions.Parameter(descriptor.Parameters.Single(p=>p.Name==binding.Target),value);else if(!inputs.TryAdd(binding.Target,value))throw new InvalidDataException("Duplicate Group input.");});
  foreach(var port in descriptor.Inputs){if(!inputs.TryGetValue(port.Name,out var value)){if(port.Required&&!errors.Any(e=>e.StartsWith(port.Name+":",StringComparison.Ordinal)))errors.Add(port.Name+": 输入未订阅或来源尚无结果。");}else if(!PortTypes.Matches(port,value))errors.Add(port.Name+": 输入类型或数组基数不匹配。");}
  if(errors.Count>0)throw new InvalidDataException("SelfRun数据准备失败："+string.Join("；",errors.Distinct()));
  return new(node with{Parameters=parameters,GroupBreak=GroupBreak.Capture(node,context.Resolve)},new ReadOnlyDictionary<string,PortValue>(inputs));
 }
 public async Task<ExecutionSessionResult> RunAsync(SolutionWorkspace workspace,string procedureId,string nodeId,IReadOnlyList<NodeRunRecord>? cachedResults=null,IReadOnlyDictionary<string,PortValue>? workflowInputs=null,CancellationToken cancellationToken=default,int? groupLoopIndex=null){
  var preparation=Prepare(workspace,procedureId,nodeId,cachedResults,workflowInputs,groupLoopIndex);var prepared=preparation.Node;var inputs=preparation.Inputs;var parent=workspace.Document.Procedures.Single(p=>p.Id==procedureId);var specs=inputs.Select(p=>new WorkflowInterfaceItem(p.Key,p.Key,p.Value.Kind){Cardinality=p.Value.Cardinality}).ToArray();
  var isolated=parent with{GroupOwner=null,Nodes=[prepared],Connections=[],ExecutionLinks=[],Configuration=new(){Inputs=specs,Subscriptions=specs.Select(p=>new WorkflowSubscription(nodeId,p.Name,false,new(WorkflowSourceKind.WorkflowInput,p.Id))).ToArray()}};
  var needed=new HashSet<string>{procedureId};void Include(string id){if(!needed.Add(id))return;foreach(var n in workspace.Document.Procedures.Single(p=>p.Id==id).Nodes)if(n.Call is {} call)Include(call.ProcedureId);}Include(prepared.Call!.ProcedureId);
  var document=workspace.Document with{Procedures=workspace.Document.Procedures.Where(p=>needed.Contains(p.Id)).Select(p=>p.Id==procedureId?isolated:p).ToArray()};
  var run=await new SolutionExecutor(registry).RunAsync(new(document,workspace.BaseDirectory),procedureId,cancellationToken:cancellationToken,inputs:inputs);return run with{WorkflowInputs=workflowInputs?.ToDictionary(p=>p.Key,p=>p.Value)??new Dictionary<string,PortValue>()};
 }
}