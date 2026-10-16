using System.Globalization;
using System.Text;
namespace VisionFramework;
public sealed record WorkflowInterfaceItem(string Id,string Name,DataKind Kind,string? NodeId=null,string? Port=null,bool Display=false,bool Accumulate=false){public ValueCardinality Cardinality{get;init;}=ValueCardinality.Scalar;}
public sealed record WorkflowRunParameter(string Id,string NodeId,string Parameter,string Label,bool IsInput=false);
public sealed record WorkflowVariableGroup(string Id,string Name);
public sealed record WorkflowLocalVariable(string Id,string GroupId,string Name,string Type,string Value,string Remark="",string? SourceNode=null,string? SourcePort=null);
public sealed record WorkflowConfiguration
{
 public IReadOnlyList<WorkflowSubscription> Subscriptions{get;init;}=[];
 public IReadOnlyList<WorkflowInterfaceItem> Inputs{get;init;}=[];
 public IReadOnlyList<WorkflowInterfaceItem> Outputs{get;init;}=[];
 public IReadOnlyList<WorkflowInterfaceItem> Displays{get;init;}=[];
 public IReadOnlyList<WorkflowRunParameter> RunParameters{get;init;}=[];
 public IReadOnlyList<WorkflowVariableGroup> Groups{get;init;}=[];
 public IReadOnlyList<WorkflowLocalVariable> Variables{get;init;}=[];
 public WorkflowConfiguration Snapshot()=>this with{Subscriptions=Subscriptions.ToArray(),Inputs=Inputs.ToArray(),Outputs=Outputs.ToArray(),Displays=Displays.ToArray(),RunParameters=RunParameters.ToArray(),Groups=Groups.ToArray(),Variables=Variables.ToArray()};
 public WorkflowConfiguration RemapNodes(IReadOnlyDictionary<string,string> ids)=>this with{Subscriptions=Subscriptions.Select(b=>b with{NodeId=ids[b.NodeId],Source=b.Source.Kind==WorkflowSourceKind.NodeOutput?b.Source with{Id=ids[b.Source.Id]}:b.Source}).ToArray(),Inputs=Inputs.Select(item=>Map(item,ids)).ToArray(),Outputs=Outputs.Select(item=>Map(item,ids)).ToArray(),Displays=Displays.Select(item=>Map(item,ids)).ToArray(),RunParameters=RunParameters.Select(p=>p with{NodeId=ids[p.NodeId]}).ToArray(),Variables=Variables.Select(v=>v with{SourceNode=v.SourceNode is null?null:ids[v.SourceNode]}).ToArray(),Groups=Groups.ToArray()};
 WorkflowInterfaceItem Map(WorkflowInterfaceItem item,IReadOnlyDictionary<string,string> ids)=>item with{NodeId=item.NodeId is null?null:ids[item.NodeId]};
 public WorkflowConfiguration RemoveNodes(ISet<string> ids)=>this with{Subscriptions=Subscriptions.Where(b=>!ids.Contains(b.NodeId)&&!(b.Source.Kind==WorkflowSourceKind.NodeOutput&&ids.Contains(b.Source.Id))).ToArray(),Inputs=Inputs.Select(item=>Clear(item,ids)).ToArray(),Outputs=Outputs.Select(item=>Clear(item,ids)).ToArray(),Displays=Displays.Select(item=>Clear(item,ids)).ToArray(),RunParameters=RunParameters.Where(p=>!ids.Contains(p.NodeId)).ToArray(),Variables=Variables.Select(v=>v.SourceNode is not null&&ids.Contains(v.SourceNode)?v with{SourceNode=null,SourcePort=null}:v).ToArray()};
 static WorkflowInterfaceItem Clear(WorkflowInterfaceItem item,ISet<string> ids)=>item.NodeId is not null&&ids.Contains(item.NodeId)?item with{NodeId=null,Port=null}:item;
 public static void CheckName(string name)
 {
  if(string.IsNullOrWhiteSpace(name)||Encoding.UTF8.GetByteCount(name)+1>126||name.IndexOfAny("?;./\\`{}[]<>()".ToCharArray())>=0)throw new ArgumentException("名称不能为空、不能包含 ? ; . / \\ ` { } [ ] < > ( )，UTF-8 长度含结束符不能超过126字节。");
 }
 public static string NormalizeValue(string type,string value)=>type switch
 {
  "int" when int.TryParse(value,NumberStyles.Integer,CultureInfo.InvariantCulture,out var i)=>i.ToString(CultureInfo.InvariantCulture),
  "float" when float.TryParse(value,NumberStyles.Float,CultureInfo.InvariantCulture,out var f)&&float.IsFinite(f)=>f.ToString(CultureInfo.InvariantCulture),
  "string"=>value,
  "byte" when value.Length%2==0&&value.All(Uri.IsHexDigit)=>value.ToUpperInvariant(),
  _=>throw new ArgumentException("变量值与类型不符。")
 };
 public void Validate(ProcedureDocument procedure,ModuleRegistry? registry=null)
 {
  if(procedure.Nodes is null)throw new InvalidDataException("Missing workflow nodes.");
  if(Subscriptions is null||Inputs is null||Outputs is null||Displays is null||RunParameters is null||Groups is null||Variables is null||Inputs.Count>1024||Outputs.Count>1024||Groups.Count>10||Variables.Count>2048)throw new InvalidDataException("Invalid workflow configuration lists or count limits.");
  void Unique(IEnumerable<string> ids){var set=new HashSet<string>(StringComparer.Ordinal);foreach(var id in ids)if(string.IsNullOrWhiteSpace(id)||!set.Add(id))throw new InvalidDataException("Missing or duplicate workflow configuration identity.");}
  Unique(Groups.Select(g=>g.Id));Unique(Groups.Select(g=>g.Name));foreach(var g in Groups)CheckName(g.Name);
  Unique(Variables.Select(v=>v.Id));Unique(Variables.Select(v=>v.Name));
  foreach(var v in Variables){CheckName(v.Name.Trim('%'));if(!Groups.Any(g=>g.Id==v.GroupId))throw new InvalidDataException("Missing variable group.");_=NormalizeValue(v.Type,v.Value);CheckSource(v.SourceNode,v.SourcePort,v.Type=="int"?DataKind.Integer:v.Type=="float"?DataKind.Number:DataKind.Text);}
  foreach(var list in new[]{Inputs,Outputs,Displays}){Unique(list.Select(v=>v.Id));Unique(list.Select(v=>v.Name));foreach(var item in list){CheckName(item.Name);if(!PortTypes.Valid(item.Kind,item.Cardinality))throw new InvalidDataException("Unknown interface type.");CheckSource(item.NodeId,item.Port,item.Kind,item.Cardinality);}}
  Unique(RunParameters.Select(r=>r.Id));foreach(var item in RunParameters){var n=procedure.Nodes.FirstOrDefault(n=>n.Id==item.NodeId)??throw new InvalidDataException("Missing runtime parameter node.");if(registry is not null&&!(item.IsInput?registry.GetDescriptor(n).Inputs.Any(p=>p.Name==item.Parameter):registry.GetDescriptor(n).Parameters.Any(p=>p.Name==item.Parameter)))throw new InvalidDataException("Missing runtime parameter.");}
  if(Subscriptions.Any(b=>b is null||b.Source is null))throw new InvalidDataException("Null subscription.");
  if(Subscriptions.Select(b=>(b.NodeId,b.Target,b.IsParameter)).Distinct().Count()!=Subscriptions.Count)throw new InvalidDataException("Duplicate subscription target.");
  if(Subscriptions.Count>100000)throw new InvalidDataException("Too many subscriptions.");
  foreach(var b in Subscriptions){if(b is null||b.Source is null||string.IsNullOrWhiteSpace(b.Target)||!procedure.Nodes.Any(n=>n.Id==b.NodeId)||!Enum.IsDefined(b.Source.Kind)||string.IsNullOrWhiteSpace(b.Source.Id))throw new InvalidDataException("Invalid subscription identity.");if(b.Source.Kind==WorkflowSourceKind.LocalVariable&&!Variables.Any(v=>v.Id==b.Source.Id)||b.Source.Kind==WorkflowSourceKind.WorkflowInput&&!Inputs.Any(v=>v.Id==b.Source.Id)||b.Source.Kind==WorkflowSourceKind.NodeOutput&&(!procedure.Nodes.Any(n=>n.Id==b.Source.Id)||string.IsNullOrWhiteSpace(b.Source.Port)))throw new InvalidDataException("Missing subscription source.");}
  foreach(var b in Subscriptions.Where(b=>b.Source.Kind==WorkflowSourceKind.GroupLoopIndex))if(procedure.GroupOwner is null||b.Source.Id!="LoopIndex"||b.Source.Port is not null)throw new InvalidDataException("LoopIndex is private to an owned Group body.");
  if(registry is not null)WorkflowSubscriptions.Validate(procedure with{Configuration=this},registry);
  void CheckSource(string? nodeId,string? port,DataKind? kind,ValueCardinality cardinality=ValueCardinality.Scalar){if(nodeId is null&&port is null)return;if(nodeId is null||port is null)throw new InvalidDataException("Incomplete source reference.");var n=procedure.Nodes.FirstOrDefault(n=>n.Id==nodeId)??throw new InvalidDataException("Missing source node.");if(registry is not null){var output=registry.GetDescriptor(n).Outputs.FirstOrDefault(p=>p.Name==port)??throw new InvalidDataException("Missing source output.");if(kind.HasValue&&(output.Kind!=kind||output.Cardinality!=cardinality))throw new InvalidDataException("Source type mismatch.");}}
 }
}
