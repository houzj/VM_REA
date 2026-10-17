using System.Globalization;
using System.Text.Json;
namespace VisionFramework;

// Typed identities deliberately separate a subscription from a literal parameter.
public enum WorkflowSourceKind { LocalVariable, WorkflowInput, NodeOutput, IntegerConstant, TextConstant }
public sealed record WorkflowValueReference(WorkflowSourceKind Kind,string Id,string? Port=null);
public sealed record WorkflowSubscription(string NodeId,string Target,bool IsParameter,WorkflowValueReference Source);
public static class WorkflowSubscriptions
{
 public static DataKind VariableKind(string type)=>type switch{"int"=>DataKind.Integer,"float"=>DataKind.Number,"string" or "byte"=>DataKind.Text,_=>throw new InvalidDataException("Unsupported variable type.")};
 public static DataKind ParameterType(ParameterKind kind)=>kind switch{ParameterKind.Integer=>DataKind.Integer,ParameterKind.Number=>DataKind.Number,ParameterKind.Boolean=>DataKind.Boolean,_=>DataKind.Text};
 public static DataKind SourceType(ProcedureDocument p,ModuleRegistry registry,WorkflowValueReference source)=>source.Kind switch
 {
  WorkflowSourceKind.TextConstant=>ReadTextConstant(source.Id) is not null?DataKind.Text:throw new InvalidDataException("Invalid text constant."),
  WorkflowSourceKind.IntegerConstant=>int.TryParse(source.Id,NumberStyles.Integer,CultureInfo.InvariantCulture,out _)?DataKind.Integer:throw new InvalidDataException("Invalid integer constant."),
  WorkflowSourceKind.LocalVariable=>VariableKind(p.Configuration.Variables.SingleOrDefault(v=>v.Id==source.Id)?.Type??throw new InvalidDataException("Missing local variable.")),
  WorkflowSourceKind.WorkflowInput=>p.Configuration.Inputs.SingleOrDefault(v=>v.Id==source.Id)?.Kind??throw new InvalidDataException("Missing workflow input."),
  WorkflowSourceKind.NodeOutput=>OutputType(p,registry,source.Id,source.Port),
  _=>throw new InvalidDataException("Unknown subscription source.")
 };
 public static string ReadTextConstant(string json){try{return JsonSerializer.Deserialize<string>(json)??throw new InvalidDataException("Null text constant.");}catch(JsonException ex){throw new InvalidDataException("Invalid text constant.",ex);}}
 static DataKind OutputType(ProcedureDocument p,ModuleRegistry registry,string id,string? port){var n=p.Nodes.SingleOrDefault(n=>n.Id==id)??throw new InvalidDataException("Missing source node.");return registry.GetDescriptor(n).Outputs.SingleOrDefault(o=>o.Name==port)?.Kind??throw new InvalidDataException("Missing source output.");}
 public static string? Producer(ProcedureDocument p,WorkflowValueReference source)=>source.Kind switch
 {
  WorkflowSourceKind.NodeOutput=>source.Id,
  WorkflowSourceKind.LocalVariable=>p.Configuration.Variables.SingleOrDefault(v=>v.Id==source.Id)?.SourceNode,
  WorkflowSourceKind.WorkflowInput=>p.Configuration.Inputs.SingleOrDefault(v=>v.Id==source.Id)?.NodeId,
  _=>null
 };
 public static void Validate(ProcedureDocument p,ModuleRegistry registry)
 {
  var targets=new HashSet<(string,string,bool)>();
  foreach(var b in p.Configuration.Subscriptions){if(!targets.Add((b.NodeId,b.Target,b.IsParameter)))throw new InvalidDataException("Duplicate subscription target.");var n=p.Nodes.SingleOrDefault(n=>n.Id==b.NodeId)??throw new InvalidDataException("Missing consumer node.");var d=registry.GetDescriptor(n);var kind=b.IsParameter?ParameterType(d.Parameters.SingleOrDefault(v=>v.Name==b.Target)?.Kind??throw new InvalidDataException("Missing target parameter.")):d.Inputs.SingleOrDefault(v=>v.Name==b.Target)?.Kind??throw new InvalidDataException("Missing target input.");if(!DataKinds.CanFlow(SourceType(p,registry,b.Source),kind))throw new InvalidDataException("Subscription type mismatch.");if(!b.IsParameter&&p.Connections.Any(c=>c.ToNode==b.NodeId&&c.ToPort==b.Target))throw new InvalidDataException("Input already has a subscription.");if(b.Source.Kind!=WorkflowSourceKind.NodeOutput&&b.Source.Port is not null)throw new InvalidDataException("Unexpected subscription source port.");}
 }
 public static PortValue Initial(WorkflowLocalVariable v)=>v.Type switch{"int"=>PortValue.Integer(int.Parse(v.Value,CultureInfo.InvariantCulture)),"float"=>PortValue.Number(float.Parse(v.Value,CultureInfo.InvariantCulture)),_=>PortValue.Text(v.Value)};
 public static JsonElement Parameter(ParameterDescriptor spec,PortValue value)
 {
  if(ParameterType(spec.Kind)!=value.Kind)throw new InvalidDataException("Parameter subscription type mismatch.");
  JsonElement json=spec.Kind==ParameterKind.Integer?JsonSerializer.SerializeToElement(checked((int)value.As<long>())):JsonSerializer.SerializeToElement(value.Value);
  if(!GraphValidator.ValidParameter(spec,json))throw new InvalidDataException("Subscribed parameter outside declared range: "+spec.Name);return json;
 }
}
