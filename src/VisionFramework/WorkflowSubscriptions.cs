using System.Globalization;
using System.Text.Json;
namespace VisionFramework;

// Typed identities deliberately separate a subscription from a literal parameter.
public enum WorkflowSourceKind { LocalVariable, WorkflowInput, NodeOutput, GroupLoopIndex, IntegerConstant, TextConstant, NumberConstant, NumberArrayConstant }

public sealed record WorkflowValueReference(WorkflowSourceKind Kind,string Id,string? Port=null){
 [System.Text.Json.Serialization.JsonIgnore(Condition=System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
 public WorkflowArrayIndex? ArrayIndex{get;init;}
}
// Null on WorkflowValueReference is legacy All. Literal text is retained, not caption-derived.
public sealed record WorkflowArrayIndex(string? LiteralText=null,WorkflowValueReference? IndexSource=null){
 public static int ParseLiteral(string text){
  // Exact MathHelper.IsNumber four-regex conjunction, including standalone "-".
  // Normal GetBindIndex first treats any dot as an IO path: numeric recognition
  // alone does not prove a decimal literal is reachable from that UI operation.
  bool Match(string pattern)=>System.Text.RegularExpressions.Regex.IsMatch(text,pattern);
  if(text is null||Match("[^0-9.-]")||Match("[0-9]*[.][0-9]*[.][0-9]*")||Match("[0-9]*[-][0-9]*[-][0-9]*")||!Match("(^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$)|(^([-]|[0-9])[0-9]*$)"))throw new FormatException("Invalid numeric index literal.");
  int dot=text.IndexOf('.');string prefix=dot<0?text:text[..dot];
  if(prefix is "" or "-")return 0;
  return int.Parse(prefix,NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture);
 }
 public void Validate(){if((LiteralText is null)==(IndexSource is null))throw new InvalidDataException("Index requires exactly one literal or source.");try{if(LiteralText is not null)_=ParseLiteral(LiteralText);}catch(Exception error)when(error is FormatException or OverflowException){throw new InvalidDataException("Invalid index literal or unsupported int32 overflow.",error);}if(IndexSource?.ArrayIndex is not null)throw new InvalidDataException("Nested index references are unsupported.");}
}
public sealed record WorkflowSubscription(string NodeId,string Target,bool IsParameter,WorkflowValueReference Source);
public static class WorkflowSubscriptions
{
 public static IEnumerable<WorkflowValueReference> References(WorkflowValueReference source){yield return source;if(source.ArrayIndex?.IndexSource is {} index)yield return index;}
 public static WorkflowValueReference RemapNodes(WorkflowValueReference source,IReadOnlyDictionary<string,string> ids)=>source with{Id=source.Kind==WorkflowSourceKind.NodeOutput&&ids.TryGetValue(source.Id,out var id)?id:source.Id,ArrayIndex=source.ArrayIndex is {} index?index with{IndexSource=index.IndexSource is {} nested?RemapNodes(nested,ids):null}:null};
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
  foreach(var b in p.Configuration.Subscriptions){if(!targets.Add((b.NodeId,b.Target,b.IsParameter)))throw new InvalidDataException("Duplicate subscription target.");var n=p.Nodes.SingleOrDefault(n=>n.Id==b.NodeId)??throw new InvalidDataException("Missing consumer node.");var d=registry.GetDescriptor(n);var kind=b.IsParameter?ParameterType(d.Parameters.SingleOrDefault(v=>v.Name==b.Target)?.Kind??throw new InvalidDataException("Missing target parameter.")):d.Inputs.SingleOrDefault(v=>v.Name==b.Target)?.Kind??throw new InvalidDataException("Missing target input.");var cardinality=b.IsParameter?ValueCardinality.Scalar:d.Inputs.Single(v=>v.Name==b.Target).Cardinality;var sourceKind=SourceType(p,registry,b.Source);var sourceCardinality=SourceCardinality(p,registry,b.Source);var destination=b.IsParameter?null:d.Inputs.Single(v=>v.Name==b.Target);if(b.Source.ArrayIndex is {} index){index.Validate();if(b.IsParameter||destination is null||!(destination.SupportsDeferredNumericRead||destination.SupportsDeferredIntegerRead||destination.SupportsDeferredTextRead)||destination.Kind is not(DataKind.Integer or DataKind.Number or DataKind.NumberArray or DataKind.Text)||b.Source.Kind!=WorkflowSourceKind.NodeOutput)throw new InvalidDataException("Array index requires an opted-in numeric input/output binding.");if(index.IndexSource is {} indexSource&&(indexSource.Kind!=WorkflowSourceKind.NodeOutput||SourceType(p,registry,indexSource)!=DataKind.Integer))throw new InvalidDataException("Index source must be a typed integer output.");}if(b.IsParameter?sourceCardinality!=cardinality||sourceKind!=kind:!PortTypes.AcceptsInput(destination!,sourceKind,sourceCardinality))throw new InvalidDataException("Subscription type mismatch.");if(!b.IsParameter&&p.Connections.Any(c=>c.ToNode==b.NodeId&&c.ToPort==b.Target))throw new InvalidDataException("Input already has a subscription.");if(b.Source.Kind!=WorkflowSourceKind.NodeOutput&&b.Source.Port is not null)throw new InvalidDataException("Unexpected subscription source port.");}

 }
 public static PortValue Initial(WorkflowLocalVariable v)=>v.Type switch{"int"=>PortValue.Integer(int.Parse(v.Value,CultureInfo.InvariantCulture)),"float"=>PortValue.Number(float.Parse(v.Value,CultureInfo.InvariantCulture)),_=>PortValue.Text(v.Value)};
 public static JsonElement Parameter(ParameterDescriptor spec,PortValue value)
 {
  if(ParameterType(spec.Kind)!=value.Kind||value.Cardinality!=ValueCardinality.Scalar)throw new InvalidDataException("Parameter subscription type mismatch.");
  JsonElement json=spec.Kind==ParameterKind.Integer?JsonSerializer.SerializeToElement(checked((int)value.As<long>())):JsonSerializer.SerializeToElement(value.Value);
  if(!GraphValidator.ValidParameter(spec,json))throw new InvalidDataException("Subscribed parameter outside declared range: "+spec.Name);return json;
 }
}
