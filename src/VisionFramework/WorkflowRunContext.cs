using System.Collections.ObjectModel;
namespace VisionFramework;

internal sealed class WorkflowRunContext
{
 readonly ProcedureDocument procedure; readonly int? groupLoopIndex;
 readonly Dictionary<string,PortValue> arguments;
 readonly Dictionary<string,PortValue> variables=new(StringComparer.Ordinal);
 readonly Dictionary<string,NodeRunRecord> records;
 public WorkflowRunContext(ProcedureDocument procedure,IReadOnlyDictionary<string,PortValue>? inputs,Dictionary<string,NodeRunRecord> records,int? groupLoopIndex=null)
 {
  this.procedure=procedure;this.records=records;this.groupLoopIndex=groupLoopIndex;arguments=inputs?.ToDictionary(p=>p.Key,p=>p.Value,StringComparer.Ordinal)??new(StringComparer.Ordinal);
  foreach(var pair in arguments){var spec=procedure.Configuration.Inputs.SingleOrDefault(i=>i.Name==pair.Key)??throw new InvalidDataException("Unknown workflow input: "+pair.Key);if(pair.Value is null||pair.Value.Kind!=spec.Kind||pair.Value.Cardinality!=spec.Cardinality)throw new InvalidDataException("Workflow input type mismatch: "+pair.Key);}
  foreach(var b in procedure.Configuration.Subscriptions.Where(b=>b.Source.Kind==WorkflowSourceKind.WorkflowInput)){var spec=procedure.Configuration.Inputs.Single(i=>i.Id==b.Source.Id);if(!arguments.ContainsKey(spec.Name)&&spec.NodeId is null)throw new InvalidDataException("Missing workflow input: "+spec.Name);}
  foreach(var v in procedure.Configuration.Variables)if(v.SourceNode is null)variables[v.Id]=WorkflowSubscriptions.Initial(v);
 }
 PortValue Node(string node,string port)=>records.TryGetValue(node,out var r)&&r.Status is NodeRunStatus.Success or NodeRunStatus.NoMatch&&r.Outputs.TryGetValue(port,out var value)?value:throw new InvalidDataException("Current-run source unavailable: "+node+"."+port);
 public PortValue Resolve(WorkflowValueReference source)=>source.Kind switch
 {
  WorkflowSourceKind.GroupLoopIndex=>groupLoopIndex is {} index?PortValue.Integer(index):throw new InvalidDataException("Group LoopIndex requires an owned invocation context."),
  WorkflowSourceKind.NodeOutput=>Node(source.Id,source.Port!),
  WorkflowSourceKind.LocalVariable=>variables.TryGetValue(source.Id,out var value)?value:throw new InvalidDataException("Current-run variable unavailable: "+source.Id),
  WorkflowSourceKind.WorkflowInput=>Input(source.Id),
  _=>throw new InvalidDataException("Unknown source kind.")
 };
 PortValue Input(string id){var spec=procedure.Configuration.Inputs.Single(i=>i.Id==id);return arguments.TryGetValue(spec.Name,out var value)?value:spec.NodeId is not null?Node(spec.NodeId,spec.Port!):throw new InvalidDataException("Missing workflow input: "+spec.Name);}
 public void Update(string node){foreach(var v in procedure.Configuration.Variables.Where(v=>v.SourceNode==node)){variables.Remove(v.Id);try{variables[v.Id]=Node(node,v.SourcePort!);}catch(InvalidDataException){}}}
 public ExecutionSessionResult Complete(ExecutionSessionResult session)
 {
  var output=new Dictionary<string,PortValue>(StringComparer.Ordinal);var errors=new List<string>();
  foreach(var spec in procedure.Configuration.Outputs){try{if(spec.NodeId is null)throw new InvalidDataException("Unbound workflow output: "+spec.Name);output.Add(spec.Name,Node(spec.NodeId,spec.Port!));}catch(InvalidDataException e){errors.Add(spec.Name+": "+e.Message);}}
  var displays=new Dictionary<string,IReadOnlyList<PortValue>>(StringComparer.Ordinal);var displayErrors=new List<string>();foreach(var spec in procedure.Configuration.Displays){try{if(spec.NodeId is null)throw new InvalidDataException("Unbound display: "+spec.Name);displays.Add(spec.Name,Array.AsReadOnly(new[]{Node(spec.NodeId,spec.Port!)}));}catch(InvalidDataException e){displayErrors.Add(e.Message);}}
  return session with{WorkflowDisplays=new ReadOnlyDictionary<string,IReadOnlyList<PortValue>>(displays),DisplayErrors=displayErrors.AsReadOnly(),Status=errors.Count>0&&session.Status==SessionStatus.Success?SessionStatus.Failed:session.Status,WorkflowInputs=new ReadOnlyDictionary<string,PortValue>(new Dictionary<string,PortValue>(arguments,StringComparer.Ordinal)),WorkflowOutputs=new ReadOnlyDictionary<string,PortValue>(output),LocalVariables=new ReadOnlyDictionary<string,PortValue>(variables.ToDictionary(v=>procedure.Configuration.Variables.Single(i=>i.Id==v.Key).Name,v=>v.Value,StringComparer.Ordinal)),OutputErrors=errors.AsReadOnly()};
 }
}
