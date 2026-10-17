using VisionFramework;
namespace VisionEditor.Core;
public sealed partial class GraphEditorDocument
{
 public string AddOwnedGroup(double x,double y,Dictionary<string,System.Text.Json.JsonElement>? parameters=null,string? executionPredecessor=null,ExecutionLinkDocument? insertInto=null)
 {
  string result="";
  EditProcedure(CurrentProcedureId,d=>{result=d.AddNode(WorkflowCalls.ModuleId,WorkflowCalls.Version,x,y,parameters,executionPredecessor,insertInto);d.EnterOwnedGroup(result);});
  return result;
 }
 string ProcedurePath(string id){var p=Procedure(id);var names=new List<string>();while(p.GroupOwner is {} owner){var parent=Procedure(owner.ProcedureId);names.Insert(0,parent.Nodes.Single(n=>n.Id==owner.NodeId).Name);p=parent;}names.Insert(0,p.Name);return string.Join(".",names);}
 static SolutionDocument SynchronizeGroupNames(SolutionDocument d){var parents=d.Procedures.ToDictionary(p=>p.Id);return d with{Procedures=d.Procedures.Select(p=>p.GroupOwner is {} owner?p with{Name=parents[owner.ProcedureId].Nodes.Single(n=>n.Id==owner.NodeId).Name}:p).ToArray()};}
 GraphFragment ResolveClipboardSources(GraphFragment fragment)
 {
  if(fragment.SourceSolutionId is null)return fragment; // Explicit independent fragments used by import/test callers.
  if(fragment.SourceSolutionId!=document.Id)throw new InvalidOperationException("剪贴板来源方案已改变。");
  var source=document.Procedures.SingleOrDefault(p=>p.Id==fragment.SourceProcedureId);
  if(source is null||ProcedurePath(source.Id)!=fragment.SourcePath)return new([],[],[]);
  var snapshot=Copy(document).Procedures.Single(p=>p.Id==source.Id);
  var nodes=fragment.Nodes.Select(old=>snapshot.Nodes.SingleOrDefault(n=>n.Id==old.Id&&n.Name==old.Name)).OfType<NodeDocument>().ToArray();
  var ids=nodes.Select(n=>n.Id).ToHashSet();bool sameParent=source.Id==CurrentProcedureId;
  return new(nodes,snapshot.Connections.Where(c=>ids.Contains(c.ToNode)&&(ids.Contains(c.FromNode)||sameParent)).ToArray(),
   ProcedureGraph.ExecutionEdges(snapshot).Where(c=>ids.Contains(c.ToNode)&&(ids.Contains(c.FromNode)||sameParent)).ToArray()){
   OwnedProcedures=CopyOwnedBodies(nodes),
   Subscriptions=snapshot.Configuration.Subscriptions.Where(s=>ids.Contains(s.NodeId)&&(sameParent||s.Source.Kind==WorkflowSourceKind.NodeOutput&&ids.Contains(s.Source.Id))).ToArray()};
 }
 public bool CanAddOwnedGroup {
  get{int depth=0;var p=CurrentProcedure;while(p.GroupOwner is {} owner){depth++;p=Procedure(owner.ProcedureId);}return depth<3;}
 }
 IReadOnlyList<ProcedureDocument> CopyOwnedBodies(IEnumerable<NodeDocument> nodes)
 {
  var all=Document.Procedures.ToDictionary(p=>p.Id);var seen=new HashSet<string>();var result=new List<ProcedureDocument>();
  void Include(NodeDocument node){if(node.Call is not {} call||!all.TryGetValue(call.ProcedureId,out var body)||body.GroupOwner is null||!seen.Add(body.Id))return;result.Add(body);foreach(var child in body.Nodes)Include(child);}
  foreach(var node in nodes)Include(node);return result.AsReadOnly();
 }
 (NodeDocument[] Nodes,IReadOnlyList<ProcedureDocument> Bodies) CloneClipboardGroups(GraphFragment fragment,NodeDocument[] nodes)
 {
  var source=fragment.OwnedProcedures.ToDictionary(p=>p.Id);var result=new List<ProcedureDocument>();var active=new HashSet<string>();
  NodeDocument CloneNode(NodeDocument node,string parent){
   if(node.Call is not {} call||!source.TryGetValue(call.ProcedureId,out var body))return node;
   if(body.GroupOwner is null||!active.Add(body.Id))throw new InvalidDataException("Invalid Group clipboard ownership.");
   var ids=body.Nodes.ToDictionary(n=>n.Id,_=>Guid.NewGuid().ToString("N"));string id=Guid.NewGuid().ToString("N");
   var children=body.Nodes.Select(n=>CloneNode(n with{Id=ids[n.Id],GroupBreak=n.GroupBreak?.Remap(ids,false)},id)).ToArray();
   var copy=body with{Id=id,GroupOwner=new(parent,node.Id),Configuration=body.Configuration.RemapNodes(ids),Nodes=children,
    Connections=body.Connections.Select(c=>c with{FromNode=ids[c.FromNode],ToNode=ids[c.ToNode]}).ToArray(),
    ExecutionLinks=body.ExecutionLinks?.Select(e=>e with{FromNode=ids[e.FromNode],ToNode=ids[e.ToNode]}).ToArray()};
   active.Remove(body.Id);result.Add(copy);return node with{Call=WorkflowCalls.From(copy),GroupBreak=node.GroupBreak?.Remap(ids,true)};
  }
  return(nodes.Select(n=>CloneNode(n,CurrentProcedureId)).ToArray(),result);
 }
 // VM: VmProcedureDecorator.OpenGroupCommandExecute sets DisplayModuleSet;
 // IMVSGroup is an owned VmModuleSet, not a top-level IVmProcedure.
 public string EnterOwnedGroup(string nodeId)
 {
  var node=Node(nodeId);if(node.ModuleId!=WorkflowCalls.ModuleId||!node.Enabled)throw new InvalidOperationException("Group 不可进入。");
  if(node.Call is {} existing){var target=Procedure(existing.ProcedureId);if(target.GroupOwner!=new GroupOwnership(CurrentProcedureId,nodeId))throw new InvalidOperationException("旧跨流程调用不属于内部 Group；请创建新的 Group。");SelectProcedure(target.Id);return target.Id;}
  int level=1;var ancestor=CurrentProcedure;while(ancestor.GroupOwner is {} parent){level++;ancestor=Procedure(parent.ProcedureId);}if(level>3)throw new InvalidOperationException("Group 最多嵌套三层。");
  var ownerId=CurrentProcedureId;var body=new ProcedureDocument(Guid.NewGuid().ToString("N"),node.Name,[],[],[]){GroupOwner=new(ownerId,nodeId)};
  Edit(d=>d with{Procedures=d.Procedures.Select(p=>p.Id==ownerId?p with{Nodes=p.Nodes.Select(n=>n.Id==nodeId?n with{Call=WorkflowCalls.From(body)}:n).ToArray()}:p).Append(body).ToArray()},body.Id);return body.Id;
 }
  public void SetGroupBreak(string nodeId,bool enabled,int dataType,int comparison,GroupBreakBindings? bindings){
  var node=Node(nodeId);var body=Procedure(node.Call?.ProcedureId??throw new InvalidOperationException("Group未配置。"));var values=node.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());values["EnableBreak"]=System.Text.Json.JsonSerializer.SerializeToElement(enabled);values["DataType"]=System.Text.Json.JsonSerializer.SerializeToElement(dataType);values["CmpType"]=System.Text.Json.JsonSerializer.SerializeToElement(comparison);var proposed=node with{Parameters=values,GroupBreak=bindings?.Snapshot()};GroupBreak.Validate(CurrentProcedure,proposed,body,registry);if(dataType is <1 or >3||comparison is <1 or >6)throw new ArgumentException("无效的中断条件。");var candidate=CurrentProcedure with{Nodes=CurrentProcedure.Nodes.Select(n=>n.Id==nodeId?proposed:n).ToArray()};var issue=GraphValidator.Validate(candidate,registry).Issues.FirstOrDefault(i=>i.Code is "GroupBreakType" or "GroupBreakSource" or "DataExecutionOrder" or "Cycle");if(issue is not null)throw new InvalidOperationException(issue.Message);ChangeNode(nodeId,_=>proposed);
 }
 public void ConfigureOwnedGroupInputs(string nodeId,IReadOnlyList<WorkflowInterfaceItem> inputs,IReadOnlyDictionary<string,WorkflowValueReference?> bindings){
  var node=Node(nodeId);var parent=CurrentProcedure;var body=Procedure(node.Call?.ProcedureId??throw new InvalidOperationException("Group未配置。"));if(body.GroupOwner!=new GroupOwnership(parent.Id,nodeId))throw new InvalidOperationException("不是拥有型Group。");
  var normalized=inputs.Select(i=>bindings.TryGetValue(i.Id,out var source)&&source is not null?i with{Cardinality=WorkflowSubscriptions.SourceCardinality(parent,registry,source)}:i).ToArray();foreach(var pair in bindings){var input=normalized.Single(i=>i.Id==pair.Key);if(pair.Value is not null&&WorkflowSubscriptions.SourceType(parent,registry,pair.Value)!=input.Kind)throw new InvalidOperationException("Group输入类型不匹配。");}
  var config=body.Configuration with{Inputs=normalized,Subscriptions=body.Configuration.Subscriptions.Where(b=>b.Source.Kind!=WorkflowSourceKind.WorkflowInput||normalized.Any(i=>i.Id==b.Source.Id)).ToArray()};config.Validate(body,registry);
  Edit(d=>{var reboundNames=bindings.Keys.Select(id=>body.Configuration.Inputs.FirstOrDefault(i=>i.Id==id)?.Name).Where(n=>n is not null).ToHashSet();var seed=parent with{Connections=parent.Connections.Where(e=>e.ToNode!=nodeId||!reboundNames.Contains(e.ToPort)).ToArray(),Configuration=parent.Configuration with{Subscriptions=parent.Configuration.Subscriptions.Where(b=>b.NodeId!=nodeId||b.IsParameter||!reboundNames.Contains(b.Target)).ToArray()}};d=d with{Procedures=d.Procedures.Select(p=>p.Id==seed.Id?seed:p).ToArray()};var next=PropagateGroupInterface(d,body with{Configuration=config});var updated=next.Procedures.Single(p=>p.Id==parent.Id);var connections=updated.Connections.ToList();var subscriptions=updated.Configuration.Subscriptions.ToList();foreach(var pair in bindings){var name=normalized.Single(i=>i.Id==pair.Key).Name;connections.RemoveAll(c=>c.ToNode==nodeId&&c.ToPort==name);subscriptions.RemoveAll(b=>b.NodeId==nodeId&&b.Target==name&&!b.IsParameter);if(pair.Value is {} source)subscriptions.Add(new(nodeId,name,false,source));}updated=updated with{Connections=connections.ToArray(),Configuration=updated.Configuration with{Subscriptions=subscriptions.ToArray()}};updated.Configuration.Validate(updated,registry);var problem=GraphValidator.Validate(updated,registry).Issues.FirstOrDefault(i=>i.Code is "PortType" or "WorkflowConfiguration" or "DataExecutionOrder" or "Cycle");if(problem is not null)throw new InvalidOperationException(problem.Message);return next with{Procedures=next.Procedures.Select(p=>p.Id==updated.Id?updated:p).ToArray()};});
 }
 public void SetGroupRunParameterValue(string procedureId,string proxyId,System.Text.Json.JsonElement value){
  var p=Procedure(procedureId);var proxy=p.Configuration.RunParameters.Single(r=>r.Id==proxyId);if(proxy.IsInput)throw new InvalidOperationException("输入代理通过订阅配置。");var replacements=new Dictionary<string,Dictionary<string,System.Text.Json.JsonElement>>();foreach(var t in proxy.Targets()){var node=p.Nodes.Single(n=>n.Id==t.NodeId);if(p.Configuration.Subscriptions.Any(b=>b.NodeId==t.NodeId&&b.Target==t.Parameter&&b.IsParameter))throw new InvalidOperationException("请先解除关联参数订阅。");if(!replacements.TryGetValue(node.Id,out var values))replacements[node.Id]=values=node.Parameters.ToDictionary(v=>v.Key,v=>v.Value.Clone());values[t.Parameter]=value.Clone();}foreach(var pair in replacements)_=CheckedParameters(p.Nodes.Single(n=>n.Id==pair.Key),pair.Value);Edit(d=>d with{Procedures=d.Procedures.Select(flow=>flow.Id==p.Id?flow with{Nodes=flow.Nodes.Select(n=>replacements.TryGetValue(n.Id,out var values)?n with{Parameters=values}:n).ToArray()}:flow).ToArray()});
 }
 public void SetGroupRunParameterSource(string procedureId,string proxyId,WorkflowValueReference? source){var p=Procedure(procedureId);var proxy=p.Configuration.RunParameters.Single(r=>r.Id==proxyId);var targets=proxy.Targets().ToArray();var config=p.Configuration with{Subscriptions=p.Configuration.Subscriptions.Where(b=>!targets.Any(t=>b.NodeId==t.NodeId&&b.Target==t.Parameter&&b.IsParameter!=t.IsInput)).Concat(source is null?[]:targets.Select(t=>new WorkflowSubscription(t.NodeId,t.Parameter,!t.IsInput,source))).ToArray()};var candidate=p with{Connections=p.Connections.Where(e=>!targets.Any(t=>t.IsInput&&e.ToNode==t.NodeId&&e.ToPort==t.Parameter)).ToArray(),Configuration=config};config.Validate(candidate,registry);var issue=GraphValidator.Validate(candidate,registry).Issues.FirstOrDefault(i=>i.Code is "WorkflowConfiguration" or "PortType" or "Cycle" or "DataExecutionOrder");if(issue is not null)throw new InvalidOperationException(issue.Message);Edit(d=>d with{Procedures=d.Procedures.Select(flow=>flow.Id==p.Id?candidate:flow).ToArray()});}
 public void SetGroupRunComplexValue(string procedureId,string proxyId,IReadOnlyDictionary<string,System.Text.Json.JsonElement> fields,IReadOnlyList<RoiDocument>? rois=null){var p=Procedure(procedureId);var proxy=p.Configuration.RunParameters.Single(r=>r.Id==proxyId);if(proxy.IsInput)throw new InvalidOperationException("Input has no parameter editor.");var nodes=p.Nodes.ToDictionary(n=>n.Id);foreach(var target in proxy.Targets()){var n=nodes[target.NodeId];var spec=registry.GetDescriptor(n).Parameters.Single(v=>v.Name==target.Parameter);var editor=spec.Editor??throw new InvalidOperationException("没有专用编辑器元数据。");var allowed=editor.Fields??new[]{target.Parameter};if(fields.Keys.Any(k=>!allowed.Contains(k)))throw new InvalidOperationException("Unknown complex parameter field.");if(allowed.Any(f=>p.Configuration.Subscriptions.Any(b=>b.NodeId==n.Id&&b.Target==f&&b.IsParameter)))throw new InvalidOperationException("请先解除复杂参数订阅。");var values=n.Parameters.ToDictionary(v=>v.Key,v=>v.Value.Clone());foreach(var pair in fields)values[pair.Key]=pair.Value.Clone();_=CheckedParameters(n,values);if(editor.Kind==ParameterEditorKind.Enumeration&&fields.TryGetValue(target.Parameter,out var choice)&&!editor.Choices!.Any(c=>c.Value.GetRawText()==choice.GetRawText()))throw new InvalidOperationException("Invalid enumeration choice.");if(editor.Kind==ParameterEditorKind.Range){var lower=values.GetValueOrDefault(allowed[0],registry.GetDescriptor(n).Parameters.Single(v=>v.Name==allowed[0]).DefaultValue??default).GetDouble();var upper=values.GetValueOrDefault(allowed[1],registry.GetDescriptor(n).Parameters.Single(v=>v.Name==allowed[1]).DefaultValue??default).GetDouble();if(lower>upper)throw new InvalidOperationException("范围下限不能大于上限。");}if(editor.Kind==ParameterEditorKind.Roi){if(rois is null)throw new InvalidOperationException("缺少ROI数据。");RoiGeometry.ValidateForEditor(rois,editor.Roi);}else if(rois is not null)throw new InvalidOperationException("Editor does not accept ROI.");nodes[n.Id]=n with{Parameters=values,Rois=editor.Kind==ParameterEditorKind.Roi?rois!.Select(r=>r with{Vertices=r.Vertices?.ToArray()}).ToArray():n.Rois};}Edit(d=>d with{Procedures=d.Procedures.Select(flow=>flow.Id==p.Id?flow with{Nodes=flow.Nodes.Select(n=>nodes[n.Id]).ToArray()}:flow).ToArray()});}
 public void LeaveGroup(){var owner=CurrentProcedure.GroupOwner??throw new InvalidOperationException("当前不是 Group 内部画布。");SelectProcedure(owner.ProcedureId);}
 public string RootProcedureId(string id){var p=Procedure(id);var seen=new HashSet<string>();while(p.GroupOwner is {} owner){if(!seen.Add(p.Id))throw new InvalidDataException("Group ownership cycle.");p=Procedure(owner.ProcedureId);}return p.Id;}
 static SolutionDocument CloneOwnedCopies(SolutionDocument d)
 {
  var source=d.Procedures.ToDictionary(p=>p.Id);var appended=new List<ProcedureDocument>();
  ProcedureDocument Clone(ProcedureDocument body,GroupOwnership owner){
   var ids=body.Nodes.ToDictionary(n=>n.Id,_=>Guid.NewGuid().ToString("N"));var id=Guid.NewGuid().ToString("N");
   var nodes=body.Nodes.Select(n=>{var copied=n with{Id=ids[n.Id],GroupBreak=n.GroupBreak?.Remap(ids,false)};if(n.Call is {} call&&source.TryGetValue(call.ProcedureId,out var nested)&&nested.GroupOwner is not null){var child=Clone(nested,new(id,copied.Id));var childIds=nested.Nodes.Zip(child.Nodes).ToDictionary(pair=>pair.First.Id,pair=>pair.Second.Id);copied=copied with{Call=WorkflowCalls.From(child),GroupBreak=copied.GroupBreak?.Remap(childIds,true)};}return copied;}).ToArray();
   var result=body with{Id=id,GroupOwner=owner,Configuration=body.Configuration.RemapNodes(ids),Nodes=nodes,Connections=body.Connections.Select(c=>c with{FromNode=ids[c.FromNode],ToNode=ids[c.ToNode]}).ToArray(),ExecutionLinks=body.ExecutionLinks?.Select(e=>e with{FromNode=ids[e.FromNode],ToNode=ids[e.ToNode]}).ToArray()};appended.Add(result);return result;
  }
  var parents=d.Procedures.Select(parent=>parent with{Nodes=parent.Nodes.Select(n=>{
   if(n.Call is not {} call||!source.TryGetValue(call.ProcedureId,out var body)||body.GroupOwner is not {} owner||owner==new GroupOwnership(parent.Id,n.Id))return n;
   var clone=Clone(body,new(parent.Id,n.Id));var childIds=body.Nodes.Zip(clone.Nodes).ToDictionary(pair=>pair.First.Id,pair=>pair.Second.Id);return n with{Call=WorkflowCalls.From(clone),GroupBreak=n.GroupBreak?.Remap(childIds,true)};
  }).ToArray()}).ToArray();return d with{Procedures=parents.Concat(appended).ToArray()};
 } SolutionDocument PruneOwnedGroups(SolutionDocument d)
 {
  var list=d.Procedures.ToList();bool removed;
  do{removed=list.RemoveAll(p=>p.GroupOwner is {} owner&&!list.Any(parent=>parent.Id==owner.ProcedureId&&parent.Nodes.Any(n=>n.Id==owner.NodeId&&n.Call?.ProcedureId==p.Id)))>0;}while(removed);
    var byId=list.ToDictionary(p=>p.Id);list=list.Select(parent=>parent with{Nodes=parent.Nodes.Select(node=>{
   if(node.GroupBreak is null||node.Call is null||!byId.TryGetValue(node.Call.ProcedureId,out var body)||body.GroupOwner is null)return node;
   GroupOperand? Clean(GroupOperand? operand){if(operand?.Source is not {} source)return operand;try{var scope=operand.Inside?body:parent;_=WorkflowSubscriptions.SourceType(scope,registry,source);if(operand.IndexSource is {} index&&(WorkflowSubscriptions.SourceType(parent,registry,index)!=DataKind.Integer||WorkflowSubscriptions.SourceCardinality(parent,registry,index)!=ValueCardinality.Scalar))return null;if((operand.Index!=0||operand.IndexSource is not null)&&WorkflowSubscriptions.SourceCardinality(scope,registry,source)==ValueCardinality.Scalar)return null;return operand;}catch(InvalidDataException){return null;}catch(KeyNotFoundException){return operand;}}
   return node with{GroupBreak=new(Clean(node.GroupBreak.Left),Clean(node.GroupBreak.Right))};
  }).ToArray()}).ToList();
  return d with{Procedures=list.ToArray()};
 }
}
