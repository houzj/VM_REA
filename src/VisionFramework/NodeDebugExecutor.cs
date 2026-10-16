using System.Text.Json;
namespace VisionFramework;

// Independent single-module execution using explicitly supplied cached input records.
// Native SelfRun's upstream preparation is not reconstructed by this class.
public sealed class NodeDebugExecutor(ModuleRegistry registry)
{
 // Read a particular tool input from its actual binding. Never execute producers or choose another image.
 public PortValue ResolveInput(ProcedureDocument procedure,string nodeId,string inputName,IReadOnlyList<NodeRunRecord>? cachedResults=null,IReadOnlyDictionary<string,PortValue>? workflowInputs=null,int? groupLoopIndex=null)
 {
  var node=procedure.Nodes.Single(n=>n.Id==nodeId);var spec=registry.GetDescriptor(node).Inputs.Single(p=>p.Name==inputName);var records=(cachedResults??[]).Where(r=>r.NodeId!=nodeId).ToDictionary(r=>r.NodeId);var scoped=procedure with{Configuration=procedure.Configuration with{Subscriptions=procedure.Configuration.Subscriptions.Where(b=>b.NodeId==nodeId).ToArray()}};var context=new WorkflowRunContext(scoped,workflowInputs,records,groupLoopIndex);foreach(var id in records.Keys)context.Update(id);
  var sources=procedure.Connections.Where(e=>e.ToNode==nodeId&&e.ToPort==inputName).Select(e=>new WorkflowValueReference(WorkflowSourceKind.NodeOutput,e.FromNode,e.FromPort)).Concat(procedure.Configuration.Subscriptions.Where(b=>b.NodeId==nodeId&&!b.IsParameter&&b.Target==inputName).Select(b=>b.Source)).ToArray();
  if(sources.Length!=1)throw new InvalidDataException("Tool preview requires exactly one input binding: "+inputName);var value=context.Resolve(sources[0]);if(!PortTypes.Matches(spec,value))throw new InvalidDataException("Tool preview input type/cardinality mismatch: "+inputName);return value;
 }
 public async Task<NodeRunRecord> RunAsync(ProcedureDocument procedure,string nodeId,string baseDirectory,IReadOnlyDictionary<string,JsonElement> values,IReadOnlyList<NodeRunRecord>? cachedResults=null,IReadOnlyDictionary<string,PortValue>? workflowInputs=null,CancellationToken cancellationToken=default,int? groupLoopIndex=null)
 {
  var node=procedure.Nodes.SingleOrDefault(n=>n.Id==nodeId)??throw new InvalidDataException("Debug node no longer exists.");
  if(node.ModuleId==WorkflowCalls.ModuleId)throw new InvalidOperationException("Group debugging requires its module-set execution service.");
  if(!node.Enabled)throw new InvalidOperationException("Disabled node cannot self-run.");
  var descriptor=registry.GetDescriptor(node);var records=(cachedResults??[]).Where(r=>r.NodeId!=nodeId).ToDictionary(r=>r.NodeId);
  var scoped=procedure with{Configuration=procedure.Configuration with{Subscriptions=procedure.Configuration.Subscriptions.Where(b=>b.NodeId==nodeId).ToArray()}};var context=new WorkflowRunContext(scoped,workflowInputs,records,groupLoopIndex);
  foreach(var source in records.Keys)context.Update(source);
  var inputs=new Dictionary<string,PortValue>(StringComparer.Ordinal);
  foreach(var edge in procedure.Connections.Where(c=>c.ToNode==nodeId))inputs.Add(edge.ToPort,context.Resolve(new(WorkflowSourceKind.NodeOutput,edge.FromNode,edge.FromPort)));
  var parameters=values.ToDictionary(p=>p.Key,p=>p.Value.Clone(),StringComparer.Ordinal);
  foreach(var binding in procedure.Configuration.Subscriptions.Where(b=>b.NodeId==nodeId)){
   var value=context.Resolve(binding.Source);
   if(binding.IsParameter)parameters[binding.Target]=WorkflowSubscriptions.Parameter(descriptor.Parameters.Single(p=>p.Name==binding.Target),value);else if(!inputs.TryAdd(binding.Target,value))throw new InvalidDataException("Duplicate debug input binding.");
  }
  foreach(var input in inputs){var port=descriptor.Inputs.SingleOrDefault(p=>p.Name==input.Key)??throw new InvalidDataException("Unknown debug input.");if(!PortTypes.Matches(port,input.Value))throw new InvalidDataException("Cached debug input type/cardinality mismatch.");}
  var inputSpecs=inputs.Select(p=>new WorkflowInterfaceItem(p.Key,p.Key,p.Value.Kind){Cardinality=p.Value.Cardinality}).ToArray();
  var isolated=new ProcedureDocument(procedure.Id,procedure.Name,[node with{Parameters=parameters}],[],[]){Configuration=new(){Inputs=inputSpecs,Subscriptions=inputSpecs.Select(p=>new WorkflowSubscription(nodeId,p.Name,false,new(WorkflowSourceKind.WorkflowInput,p.Id))).ToArray()}};
  var run=await new GraphExecutor(registry).RunAsync(isolated,baseDirectory,cancellationToken:cancellationToken,inputs:inputs);
  return run.Nodes.Single();
 }
}
