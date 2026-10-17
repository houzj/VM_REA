using System.Globalization;
using System.Text.Json;
namespace VisionFramework;

// Typed identities deliberately separate a subscription from a literal parameter.
public enum WorkflowSourceKind { LocalVariable, WorkflowInput, NodeOutput, GroupLoopIndex, IntegerConstant, TextConstant, NumberConstant, NumberArrayConstant }

public sealed record WorkflowValueReference(WorkflowSourceKind Kind,string Id,string? Port=null);
public sealed record WorkflowSubscription(string NodeId,string Target,bool IsParameter,WorkflowValueReference Source);
public static class WorkflowSubscriptions
{
 public static DataKind VariableKind(string type)=>type switch{"int"=>DataKind.Integer,"float"=>DataKind.Number,"string" or "byte"=>DataKind.Text,_=>throw new InvalidDataException("Unsupported variable type.")};
 public static DataKind ParameterType(ParameterKind kind)=>kind switch{ParameterKind.Integer=>DataKind.Integer,ParameterKind.Number=>DataKind.Number,ParameterKind.Boolean=>DataKind.Boolean,_=>DataKind.Text};
 public static DataKind SourceType(ProcedureDocument p,ModuleRegistry registry,WorkflowValueReference source)=>source.Kind switch
 {
  WorkflowSourceKind.GroupLoopIndex=>PrivateLoopIndex(p,source),
  WorkflowSourceKind.IntegerConstant=>int.TryParse(source.Id,NumberStyles.Integer,CultureInfo.InvariantCulture,out _)?DataKind.Integer:throw new InvalidDataException("Invalid integer constant."),
  WorkflowSourceKind.NumberConstant=>double.IsFinite(ReadNumberConstant(source.Id))?DataKind.Number:throw new InvalidDataException("Invalid number constant."),
  WorkflowSourceKind.NumberArrayConstant=>ReadNumberArrayConstant(source.Id) is not null?DataKind.NumberArray:throw new InvalidDataException("Invalid number array constant."),
  WorkflowSourceKind.TextConstant=>ReadTextConstant(source.Id) is not null?DataKind.Text:throw new InvalidDataException("Invalid text constant."),

  WorkflowSourceKind.LocalVariable=>VariableKind(p.Configuration.Variables.SingleOrDefault(v=>v.Id==source.Id)?.Type??throw new InvalidDataException("Missing local variable.")),
  WorkflowSourceKind.WorkflowInput=>p.Configuration.Inputs.SingleOrDefault(v=>v.Id==source.Id)?.Kind??throw new InvalidDataException("Missing workflow input."),
  WorkflowSourceKind.NodeOutput=>OutputType(p,registry,source.Id,source.Port),
  _=>throw new InvalidDataException("Unknown subscription source.")
 };
 public static ValueCardinality SourceCardinality(ProcedureDocument p,ModuleRegistry registry,WorkflowValueReference source)=>source.Kind switch{
  WorkflowSourceKind.GroupLoopIndex=>PrivateLoopIndex(p,source)==DataKind.Integer?ValueCardinality.Scalar:throw new InvalidDataException(),
  WorkflowSourceKind.IntegerConstant or WorkflowSourceKind.NumberConstant or WorkflowSourceKind.NumberArrayConstant or WorkflowSourceKind.TextConstant=>ValueCardinality.Scalar,
  WorkflowSourceKind.LocalVariable=>ValueCardinality.Scalar,
  WorkflowSourceKind.WorkflowInput=>p.Configuration.Inputs.SingleOrDefault(v=>v.Id==source.Id)?.Cardinality??throw new InvalidDataException("Missing workflow input."),
  WorkflowSourceKind.NodeOutput=>registry.GetDescriptor(p.Nodes.SingleOrDefault(n=>n.Id==source.Id)??throw new InvalidDataException("Missing source node.")).Outputs.SingleOrDefault(o=>o.Name==source.Port)?.Cardinality??throw new InvalidDataException("Missing source output."),
  _=>throw new InvalidDataException("Unknown subscription source.")};
 static DataKind PrivateLoopIndex(ProcedureDocument p,WorkflowValueReference source){if(p.GroupOwner is null||source.Id!="LoopIndex"||source.Port is not null)throw new InvalidDataException("LoopIndex is private to an owned Group body.");return DataKind.Integer;}

 public static double ReadNumberConstant(string json){try{double value=JsonSerializer.Deserialize<double>(json);return double.IsFinite(value)?value:throw new InvalidDataException("Non-finite number constant.");}catch(JsonException e){throw new InvalidDataException("Invalid number constant.",e);}}
 public static IReadOnlyList<double> ReadNumberArrayConstant(string json){try{var values=JsonSerializer.Deserialize<double[]>(json)??throw new InvalidDataException("Null numeric array.");if(values.Length>1000000||values.Any(v=>!double.IsFinite(v)))throw new InvalidDataException("Invalid numeric array.");return Array.AsReadOnly(values);}catch(JsonException e){throw new InvalidDataException("Invalid numeric array.",e);}}
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
  foreach(var b in p.Configuration.Subscriptions){if(!targets.Add((b.NodeId,b.Target,b.IsParameter)))throw new InvalidDataException("Duplicate subscription target.");var n=p.Nodes.SingleOrDefault(n=>n.Id==b.NodeId)??throw new InvalidDataException("Missing consumer node.");var d=registry.GetDescriptor(n);var kind=b.IsParameter?ParameterType(d.Parameters.SingleOrDefault(v=>v.Name==b.Target)?.Kind??throw new InvalidDataException("Missing target parameter.")):d.Inputs.SingleOrDefault(v=>v.Name==b.Target)?.Kind??throw new InvalidDataException("Missing target input.");var cardinality=b.IsParameter?ValueCardinality.Scalar:d.Inputs.Single(v=>v.Name==b.Target).Cardinality;var sourceKind=SourceType(p,registry,b.Source);var sourceCardinality=SourceCardinality(p,registry,b.Source);var destination=b.IsParameter?null:d.Inputs.Single(v=>v.Name==b.Target);if(sourceCardinality!=cardinality||!(b.IsParameter?sourceKind==kind:DataKinds.CanFlow(sourceKind,destination!)))throw new InvalidDataException("Subscription type mismatch.");if(!b.IsParameter&&p.Connections.Any(c=>c.ToNode==b.NodeId&&c.ToPort==b.Target))throw new InvalidDataException("Input already has a subscription.");if(b.Source.Kind!=WorkflowSourceKind.NodeOutput&&b.Source.Port is not null)throw new InvalidDataException("Unexpected subscription source port.");}

 }
 public static PortValue Initial(WorkflowLocalVariable v)=>v.Type switch{"int"=>PortValue.Integer(int.Parse(v.Value,CultureInfo.InvariantCulture)),"float"=>PortValue.Number(float.Parse(v.Value,CultureInfo.InvariantCulture)),_=>PortValue.Text(v.Value)};
 public static JsonElement Parameter(ParameterDescriptor spec,PortValue value)
 {
  if(ParameterType(spec.Kind)!=value.Kind||value.Cardinality!=ValueCardinality.Scalar)throw new InvalidDataException("Parameter subscription type mismatch.");
  JsonElement json=spec.Kind==ParameterKind.Integer?JsonSerializer.SerializeToElement(checked((int)value.As<long>())):JsonSerializer.SerializeToElement(value.Value);
  if(!GraphValidator.ValidParameter(spec,json))throw new InvalidDataException("Subscribed parameter outside declared range: "+spec.Name);return json;
 }
}
