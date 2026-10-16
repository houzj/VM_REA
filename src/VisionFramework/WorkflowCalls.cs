namespace VisionFramework;

public sealed record WorkflowCallPort(string Id,string Name,DataKind Kind){public ValueCardinality Cardinality{get;init;}=ValueCardinality.Scalar;}
public sealed record WorkflowCallDocument(string ProcedureId,IReadOnlyList<WorkflowCallPort> Inputs,IReadOnlyList<WorkflowCallPort> Outputs){public WorkflowCallDocument Snapshot()=>this with{Inputs=Inputs?.ToArray()??throw new InvalidDataException("Missing call input signature."),Outputs=Outputs?.ToArray()??throw new InvalidDataException("Missing call output signature.")};}

public static class WorkflowCalls
{
 public const string ModuleId="workflow.subprogram";
 public const string Version="1.0.0";
 public static ModuleDescriptor Descriptor{get;}=new(ModuleId,Version,"Group",[],[],GroupLoop.Parameters);
 public static void Register(ModuleRegistry registry)=>registry.Register(Descriptor,()=>new CallModule());
 sealed class CallModule:IModule
 {
  public ModuleDescriptor Descriptor=>WorkflowCalls.Descriptor;
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>throw new InvalidOperationException("Subprogram requires a solution execution context.");
 }
 public static WorkflowCallDocument From(ProcedureDocument target)=>new(target.Id,target.Configuration.Inputs.Select(p=>new WorkflowCallPort(p.Id,p.Name,p.Kind){Cardinality=p.Cardinality}).ToArray(),target.Configuration.Outputs.Select(p=>new WorkflowCallPort(p.Id,p.Name,p.Kind){Cardinality=target.GroupOwner is not null&&p.Accumulate&&GroupBreak.Accumulatable(p.Kind)?ValueCardinality.Array:p.Cardinality}).ToArray());
 public static ModuleDescriptor Describe(NodeDocument node)
 {
  if(node.Call is null)return Descriptor;
  var call=node.Call;
  foreach(var ports in new[]{call.Inputs,call.Outputs}){if(ports is null||ports.Count>1024||ports.Any(p=>p is null||string.IsNullOrWhiteSpace(p.Id)||string.IsNullOrWhiteSpace(p.Name)||!PortTypes.Valid(p.Kind,p.Cardinality))||ports.Select(p=>p.Id).Distinct().Count()!=ports.Count||ports.Select(p=>p.Name).Distinct().Count()!=ports.Count)throw new InvalidDataException("Invalid subprogram port signature.");}
  return Descriptor with{Inputs=call.Inputs.Select(p=>new PortDescriptor(p.Name,p.Kind){Cardinality=p.Cardinality}).ToArray(),Outputs=call.Outputs.Select(p=>new PortDescriptor(p.Name,p.Kind){Cardinality=p.Cardinality}).ToArray()};
 }
 public static void Validate(SolutionDocument solution,string? entry=null,bool allowDrafts=false)
 {
  var procedures=solution.Procedures.ToDictionary(p=>p.Id,StringComparer.Ordinal);var active=new HashSet<string>();int visits=0;
  void Visit(string id,int depth){
   if(depth>32||++visits>10000)throw new InvalidDataException("Independent subprogram depth/expansion limit exceeded.");
   if(!procedures.TryGetValue(id,out var procedure))throw new InvalidDataException("Missing subprogram target: "+id);
   if(!active.Add(id))throw new InvalidDataException("Recursive subprogram call: "+id);
   foreach(var n in procedure.Nodes){
    if(n.ModuleId!=ModuleId){if(n.Call is not null)throw new InvalidDataException("Call metadata belongs to a subprogram node.");continue;}
    if(n.Call is null&&allowDrafts)continue;
    _=Describe(n);var call=n.Call??throw new InvalidDataException("Subprogram is not configured: "+n.Name);
    if(!procedures.TryGetValue(call.ProcedureId,out var target))throw new InvalidDataException("Missing subprogram target: "+call.ProcedureId);
    var current=From(target);if(!call.Inputs.SequenceEqual(current.Inputs)||!call.Outputs.SequenceEqual(current.Outputs))throw new InvalidDataException("Subprogram interface changed; reconfigure node: "+n.Name);
    if(n.Enabled)Visit(target.Id,depth+1);
   }
   active.Remove(id);
  }
  foreach(var id in entry is null?procedures.Keys.AsEnumerable():new[]{entry})Visit(id,0);
 }
}
