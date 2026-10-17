using System.Text;
using System.Text.Json;
namespace VisionFramework;
public sealed record GroupOperand(bool Inside=false,WorkflowValueReference? Source=null,JsonElement? Constant=null,int Index=0,bool Self=false){
 public WorkflowValueReference? IndexSource {get;init;}
 [System.Text.Json.Serialization.JsonIgnore] public string? PreparationError {get;init;}
 public GroupOperand Remap(IReadOnlyDictionary<string,string> ids,bool inside){
  WorkflowValueReference? Map(WorkflowValueReference? reference)=>reference is {Kind:WorkflowSourceKind.NodeOutput} r&&ids.TryGetValue(r.Id,out var id)?r with{Id=id}:reference;
  return this with{Source=Inside==inside?Map(Source):Source,IndexSource=!inside?Map(IndexSource):IndexSource};
 }
}
public sealed record GroupBreakBindings(GroupOperand? Left=null,GroupOperand? Right=null){public GroupBreakBindings Snapshot()=>new(Left is null?null:Left with{Constant=Left.Constant?.Clone()},Right is null?null:Right with{Constant=Right.Constant?.Clone()});public GroupBreakBindings Remap(IReadOnlyDictionary<string,string> ids,bool inside)=>new(Left?.Remap(ids,inside),Right?.Remap(ids,inside));}
public static class GroupBreak {
 public static IEnumerable<(WorkflowValueReference Source,DataKind Kind)> ParentRequirements(NodeDocument node){foreach(var operand in new[]{node.GroupBreak?.Left,node.GroupBreak?.Right}){if(operand is null)continue;if(operand is {Inside:false,Self:false,Source:{} value})yield return(value,Kind(node));if(operand.IndexSource is {} index)yield return(index,DataKind.Integer);}}
 public static IEnumerable<WorkflowValueReference> ParentReferences(NodeDocument node)=>ParentRequirements(node).Select(p=>p.Source);
 public static DataKind Kind(NodeDocument node)=>Integer(node,"DataType",1) switch{1=>DataKind.Integer,2=>DataKind.Number,3=>DataKind.Text,_=>throw new InvalidDataException("Unsupported Group break data type.")};
 public static int Integer(NodeDocument node,string key,int fallback)=>node.Parameters.TryGetValue(key,out var value)?value.GetInt32():fallback;
 public static bool Enabled(NodeDocument node)=>node.Parameters.TryGetValue("EnableBreak",out var value)&&value.GetBoolean();
 public static bool Accumulatable(DataKind kind)=>kind is DataKind.Integer or DataKind.Number or DataKind.Text or DataKind.Circle or DataKind.Point or DataKind.Fixture;
 public static void Validate(ProcedureDocument parent,NodeDocument node,ProcedureDocument body,ModuleRegistry registry){
  if(node.GroupBreak is null)return;if(body.GroupOwner!=new GroupOwnership(parent.Id,node.Id))throw new InvalidDataException("Break bindings require an owned Group.");var kind=Kind(node);
  foreach(var operand in new[]{node.GroupBreak.Left,node.GroupBreak.Right}){if(operand is null)continue;if(operand.IndexSource is {} indexSource){if(operand.Source is null||WorkflowSubscriptions.SourceType(parent,registry,indexSource)!=DataKind.Integer||WorkflowSubscriptions.SourceCardinality(parent,registry,indexSource)!=ValueCardinality.Scalar||WorkflowSubscriptions.Producer(parent,indexSource)==node.Id)throw new InvalidDataException("Group array index requires a preceding scalar integer source.");}
   if(operand.Index<0||operand.Source is not null&&operand.Constant is not null)throw new InvalidDataException("Invalid Group break operand.");
   if(operand.Self&&(operand.Source is not {Kind:WorkflowSourceKind.NodeOutput} own||own.Id!=node.Id||operand.Inside))throw new InvalidDataException("Invalid Group self-output source.");if(operand.Source is {} source){var scope=operand.Inside?body:parent;if(WorkflowSubscriptions.SourceCardinality(scope,registry,source)==ValueCardinality.Scalar&&(operand.Index!=0||operand.IndexSource is not null))throw new InvalidDataException("Scalar break source index must be zero.");if(WorkflowSubscriptions.SourceType(scope,registry,source)!=kind)throw new InvalidDataException("Group break operand type mismatch.");if(!operand.Inside&&!operand.Self&&WorkflowSubscriptions.Producer(parent,source)==node.Id)throw new InvalidDataException("Group cannot subscribe to its own result.");}
   else if(operand.Constant is {} literal){_=Literal(kind,literal);}
  }
 }
 public static PortValue Literal(DataKind kind,JsonElement value){try{return kind switch{DataKind.Integer=>PortValue.Integer(value.GetInt32()),DataKind.Number=>PortValue.Number(value.GetSingle()),DataKind.Text=>PortValue.Text(value.GetString()??throw new InvalidDataException("Missing string operand.")),_=>throw new InvalidDataException("Invalid Group comparison kind.")};}catch(Exception e)when(e is InvalidOperationException or FormatException or OverflowException or ArgumentOutOfRangeException){throw new InvalidDataException("Invalid Group comparison literal.",e);}}
 internal static GroupBreakBindings? Capture(NodeDocument node,Func<WorkflowValueReference,PortValue> resolve){
  GroupOperand? CaptureOperand(GroupOperand? operand){
   if(operand is null)return null;
   try{
    if(operand.IndexSource is {} indexSource){var value=resolve(indexSource);if(value.Kind!=DataKind.Integer||value.Cardinality!=ValueCardinality.Scalar||value.As<long>() is <0 or >int.MaxValue)throw new InvalidDataException("Group array index must be a nonnegative int32 scalar.");operand=operand with{Index=(int)value.As<long>(),IndexSource=null};}
    if(operand is {Inside:false,Self:false,Source:{} source}){var value=Element(resolve(source),operand.Index);return new(Constant:JsonSerializer.SerializeToElement(value.Value));}
    return operand;
   }catch(InvalidDataException error){return new(){PreparationError=error.Message};}
  }
  return node.GroupBreak is {} bindings?new(CaptureOperand(bindings.Left),CaptureOperand(bindings.Right)):null;
 }
 internal static PortValue Element(PortValue value,int index){if(value.Cardinality==ValueCardinality.Scalar){if(index!=0)throw new InvalidDataException("Scalar index must be zero.");return value;}try{return value.Kind switch{DataKind.Integer=>PortValue.Integer(value.As<IReadOnlyList<long>>()[index]),DataKind.Number=>PortValue.Number(value.As<IReadOnlyList<double>>()[index]),DataKind.Text=>PortValue.Text(value.As<IReadOnlyList<string>>()[index]),_=>throw new InvalidDataException("Unsupported comparison array.")};}catch(Exception e)when(e is ArgumentOutOfRangeException or IndexOutOfRangeException){throw new InvalidDataException("Break comparison array index is unavailable.");}}
 static PortValue Resolve(ProcedureDocument body,ExecutionSessionResult? previous,int index,GroupOperand operand,DataKind kind,IReadOnlyDictionary<string,PortValue> inputs,Func<string,int,PortValue> selfOutput){
  if(operand.PreparationError is {} error)throw new InvalidDataException(error);
  if(operand.Self&&operand.Source is {} own)return selfOutput(own.Port!,operand.Index);if(operand.Constant is {} literal)return Literal(kind,literal);if(!operand.Inside||operand.Source is not {} source)throw new InvalidDataException("Break operand is unbound.");if(source.Kind==WorkflowSourceKind.GroupLoopIndex)return PortValue.Integer(index);if(source.Kind==WorkflowSourceKind.WorkflowInput){var spec=body.Configuration.Inputs.Single(v=>v.Id==source.Id);if(inputs.TryGetValue(spec.Name,out var input))return Element(input,operand.Index);}if(source.Kind==WorkflowSourceKind.LocalVariable){var spec=body.Configuration.Variables.Single(v=>v.Id==source.Id);if(spec.SourceNode is null)return WorkflowSubscriptions.Initial(spec);}if(previous is null)throw new InvalidDataException("Internal comparison requires a completed iteration.");
  var value=source.Kind switch{WorkflowSourceKind.NodeOutput=>previous.Nodes.SingleOrDefault(n=>n.NodeId==source.Id&&n.Status is NodeRunStatus.Success or NodeRunStatus.NoMatch)?.Outputs.GetValueOrDefault(source.Port!)??throw new InvalidDataException("Internal output unavailable."),WorkflowSourceKind.LocalVariable=>previous.LocalVariables.GetValueOrDefault(body.Configuration.Variables.Single(v=>v.Id==source.Id).Name)??throw new InvalidDataException("Internal variable unavailable."),WorkflowSourceKind.WorkflowInput=>previous.WorkflowInputs.GetValueOrDefault(body.Configuration.Inputs.Single(v=>v.Id==source.Id).Name)??(body.Configuration.Inputs.Single(v=>v.Id==source.Id) is {NodeId:{} producer,Port:{} port}?previous.Nodes.SingleOrDefault(n=>n.NodeId==producer)?.Outputs.GetValueOrDefault(port):null)??throw new InvalidDataException("Internal input unavailable."),_=>throw new InvalidDataException("Unsupported comparison source.")};return Element(value,operand.Index);
 }
 internal static bool Evaluate(NodeDocument node,ProcedureDocument body,ExecutionSessionResult? previous,int index,List<string> diagnostics,IReadOnlyDictionary<string,PortValue> inputs,Func<string,int,PortValue> selfOutput){
  if(!Enabled(node))return false;var bindings=node.GroupBreak;
  // Native first-round guard excludes values produced by the internal graph.
  if(previous is null&&new[]{bindings?.Left,bindings?.Right}.Any(o=>o is {Self:true}||(o is {Inside:true,Source:{} source}&&WorkflowSubscriptions.Producer(body,source) is not null)))return false;
  try{var kind=Kind(node);return Compare(kind,Integer(node,"CmpType",1),Resolve(body,previous,index,bindings?.Left??new(),kind,inputs,selfOutput),Resolve(body,previous,index,bindings?.Right??new(),kind,inputs,selfOutput));}catch(InvalidDataException e){if(diagnostics.Count<32&&!diagnostics.Contains(e.Message))diagnostics.Add(e.Message);return false;}
 }
 public static bool Compare(DataKind kind,int operation,PortValue left,PortValue right){
  if(left.Kind!=kind||right.Kind!=kind||left.Cardinality!=ValueCardinality.Scalar||right.Cardinality!=ValueCardinality.Scalar)throw new InvalidDataException("Break comparison type mismatch.");int order;bool equal;
  if(kind==DataKind.Text){var a=Encoding.UTF8.GetBytes(left.As<string>());var b=Encoding.UTF8.GetBytes(right.As<string>());order=a.AsSpan().SequenceCompareTo(b);equal=order==0;}
  else if(kind==DataKind.Number){float a=(float)left.As<double>(),b=(float)right.As<double>();if(!float.IsFinite(a)||!float.IsFinite(b))throw new InvalidDataException("Break float operand exceeds native range.");order=a.CompareTo(b);equal=MathF.Abs(a-b)<1.1920929E-07f;}
  else{if(left.As<long>() is <int.MinValue or >int.MaxValue||right.As<long>() is <int.MinValue or >int.MaxValue)throw new InvalidDataException("Break integer operand exceeds native int32 range.");var a=(int)left.As<long>();var b=(int)right.As<long>();order=a.CompareTo(b);equal=a==b;}
  return operation switch{1=>order>0,2=>order<0,3=>equal,4=>kind==DataKind.Number?(float)left.As<double>()!=(float)right.As<double>():!equal,5=>order>=0,6=>order<=0,_=>throw new InvalidDataException("Unknown Group comparison operator.")};
 }
}
