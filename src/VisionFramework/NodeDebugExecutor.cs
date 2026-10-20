using System.Text.Json;
namespace VisionFramework;

// Independent single-module execution using explicitly supplied cached input records.
// Native SelfRun's upstream preparation is not reconstructed by this class.
public sealed class NodeDebugExecutor(ModuleRegistry registry)
{
 // Preserve exact scalar/array checks while accepting owned image formats for a generic IMAGE input.
 static bool MatchesInput(PortDescriptor port,PortValue value)=>PortTypes.Matches(port,value)||PortTypes.DeferredReadAccepts(port,value.Kind)||port.Kind==DataKind.Image&&DataKinds.IsImage(value.Kind)&&port.Cardinality==value.Cardinality;
 // Read a particular tool input from its actual binding. Never execute producers or choose another image.
 public PortValue ResolveInput(ProcedureDocument procedure,string nodeId,string inputName,IReadOnlyList<NodeRunRecord>? cachedResults=null,IReadOnlyDictionary<string,PortValue>? workflowInputs=null,int? groupLoopIndex=null)
 {
  var node=procedure.Nodes.Single(n=>n.Id==nodeId);var spec=registry.GetDescriptor(node).Inputs.Single(p=>p.Name==inputName);var records=(cachedResults??[]).Where(r=>r.NodeId!=nodeId).Select(r=>r with{Outputs=CompoundOutputFields.Expand(r.Outputs)}).ToDictionary(r=>r.NodeId);var scoped=procedure with{Configuration=procedure.Configuration with{Subscriptions=procedure.Configuration.Subscriptions.Where(b=>b.NodeId==nodeId).ToArray()}};var context=new WorkflowRunContext(scoped,workflowInputs,records,groupLoopIndex);foreach(var id in records.Keys)context.Update(id);
  var sources=procedure.Connections.Where(e=>e.ToNode==nodeId&&e.ToPort==inputName).Select(e=>new WorkflowValueReference(WorkflowSourceKind.NodeOutput,e.FromNode,e.FromPort)).Concat(procedure.Configuration.Subscriptions.Where(b=>b.NodeId==nodeId&&!b.IsParameter&&b.Target==inputName).Select(b=>b.Source)).ToArray();
  if(sources.Length!=1)throw new InvalidDataException("Tool preview requires exactly one input binding: "+inputName);var value=context.Resolve(sources[0]);if(!MatchesInput(spec,value))throw new InvalidDataException("Tool preview input type/cardinality mismatch: "+inputName);return value;
 }
 public async Task<NodeRunRecord> RunAsync(ProcedureDocument procedure,string nodeId,string baseDirectory,IReadOnlyDictionary<string,JsonElement> values,IReadOnlyList<NodeRunRecord>? cachedResults=null,IReadOnlyDictionary<string,PortValue>? workflowInputs=null,CancellationToken cancellationToken=default,int? groupLoopIndex=null,ModuleExecutionScope? scope=null)
 {
  var node=procedure.Nodes.SingleOrDefault(n=>n.Id==nodeId)??throw new InvalidDataException("Debug node no longer exists.");
  if(node.ModuleId==WorkflowCalls.ModuleId)throw new InvalidOperationException("Group debugging requires its module-set execution service.");
  if(!node.Enabled)throw new InvalidOperationException("Disabled node cannot self-run.");
  var descriptor=registry.GetDescriptor(node);var records=(cachedResults??[]).Where(r=>r.NodeId!=nodeId).Select(r=>r with{Outputs=CompoundOutputFields.Expand(r.Outputs)}).ToDictionary(r=>r.NodeId);
  var scoped=procedure with{Configuration=procedure.Configuration with{Subscriptions=procedure.Configuration.Subscriptions.Where(b=>b.NodeId==nodeId).ToArray()}};var context=new WorkflowRunContext(scoped,workflowInputs,records,groupLoopIndex);
  foreach(var source in records.Keys)context.Update(source);
  var inputs=new Dictionary<string,PortValue>(StringComparer.Ordinal);
  foreach(var edge in procedure.Connections.Where(c=>c.ToNode==nodeId))inputs.Add(edge.ToPort,context.Resolve(new(WorkflowSourceKind.NodeOutput,edge.FromNode,edge.FromPort)));
  var parameters=values.ToDictionary(p=>p.Key,p=>p.Value.Clone(),StringComparer.Ordinal);
  var numericIndices=new Dictionary<string,WorkflowArrayIndex?>(StringComparer.Ordinal);
  foreach(var binding in procedure.Configuration.Subscriptions.Where(b=>b.NodeId==nodeId)){
   PortValue value;if(!binding.IsParameter&&descriptor.Inputs.Single(p=>p.Name==binding.Target) is {} deferred&&(deferred.SupportsDeferredNumericRead||deferred.SupportsDeferredIntegerRead||deferred.SupportsDeferredTextRead)){numericIndices.Add(binding.Target,binding.Source.ArrayIndex);try{value=context.Resolve(binding.Source with{ArrayIndex=null});}catch(InvalidDataException)when(context.CanDeferNumericRead(binding.Source)){continue;}}else value=context.Resolve(binding.Source);
   if(binding.IsParameter)parameters[binding.Target]=WorkflowSubscriptions.Parameter(descriptor.Parameters.Single(p=>p.Name==binding.Target),value);else if(!inputs.TryAdd(binding.Target,value))throw new InvalidDataException("Duplicate debug input binding.");
  }
  foreach(var input in inputs.ToArray()){var port=descriptor.Inputs.SingleOrDefault(p=>p.Name==input.Key)??throw new InvalidDataException("Unknown debug input.");if(numericIndices.ContainsKey(input.Key))inputs[input.Key]=DataKinds.Coerce(input.Value,port);if(!MatchesInput(port,inputs[input.Key]))throw new InvalidDataException("Cached debug input type/cardinality mismatch.");}
  var inputSpecs=inputs.Select(p=>new WorkflowInterfaceItem(p.Key,p.Key,p.Value.Kind){Cardinality=p.Value.Cardinality}).ToArray();
  var isolated=new ProcedureDocument(procedure.Id,procedure.Name,[node with{Parameters=parameters}],[],[]){Configuration=new(){Inputs=inputSpecs,Subscriptions=inputSpecs.Select(p=>new WorkflowSubscription(nodeId,p.Name,false,new(WorkflowSourceKind.WorkflowInput,p.Id))).ToArray()}};
  var originalSuccessors=Array.AsReadOnly(ProcedureGraph.ExecutionEdges(procedure).Where(edge=>edge.FromNode==nodeId).Select(edge=>new ExecutionSuccessor(edge.ToNode)).ToArray());
  // Read selectors during this execution, not while materializing cached input containers.
  var numericReader=numericIndices.Count==0?null:new WorkflowNumericInputRead(inputs,numericIndices,context.Resolve);
  var run=await new GraphExecutor(registry).RunAsync(isolated,baseDirectory,cancellationToken:cancellationToken,inputs:inputs,scope:scope,numericInputRead:(id,name,index)=>id==nodeId?numericReader?.Read(name,index):null,integerInputRead:(id,name,index)=>id==nodeId?numericReader?.ReadInteger(name,index):null,textInputRead:(id,name,index,capacity)=>id==nodeId?numericReader?.ReadText(name,index,capacity):null,executionSuccessorRead:id=>id==nodeId?originalSuccessors:null);
  return run.Nodes.Single();
 }
}
