using VisionFramework;
namespace VisionEditor.Core;
public sealed partial class GraphEditorDocument
{
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
 public void LeaveGroup(){var owner=CurrentProcedure.GroupOwner??throw new InvalidOperationException("当前不是 Group 内部画布。");SelectProcedure(owner.ProcedureId);}
 public string RootProcedureId(string id){var p=Procedure(id);var seen=new HashSet<string>();while(p.GroupOwner is {} owner){if(!seen.Add(p.Id))throw new InvalidDataException("Group ownership cycle.");p=Procedure(owner.ProcedureId);}return p.Id;}
 static SolutionDocument CloneOwnedCopies(SolutionDocument d)
 {
  var source=d.Procedures.ToDictionary(p=>p.Id);var appended=new List<ProcedureDocument>();
  ProcedureDocument Clone(ProcedureDocument body,GroupOwnership owner){
   var ids=body.Nodes.ToDictionary(n=>n.Id,_=>Guid.NewGuid().ToString("N"));var id=Guid.NewGuid().ToString("N");
   var nodes=body.Nodes.Select(n=>{var copied=n with{Id=ids[n.Id]};if(n.Call is {} call&&source.TryGetValue(call.ProcedureId,out var nested)&&nested.GroupOwner is not null){var child=Clone(nested,new(id,copied.Id));copied=copied with{Call=WorkflowCalls.From(child)};}return copied;}).ToArray();
   var result=body with{Id=id,GroupOwner=owner,Configuration=body.Configuration.RemapNodes(ids),Nodes=nodes,Connections=body.Connections.Select(c=>c with{FromNode=ids[c.FromNode],ToNode=ids[c.ToNode]}).ToArray(),ExecutionLinks=body.ExecutionLinks?.Select(e=>e with{FromNode=ids[e.FromNode],ToNode=ids[e.ToNode]}).ToArray()};appended.Add(result);return result;
  }
  var parents=d.Procedures.Select(parent=>parent with{Nodes=parent.Nodes.Select(n=>{
   if(n.Call is not {} call||!source.TryGetValue(call.ProcedureId,out var body)||body.GroupOwner is not {} owner||owner==new GroupOwnership(parent.Id,n.Id))return n;
   var clone=Clone(body,new(parent.Id,n.Id));return n with{Call=WorkflowCalls.From(clone)};
  }).ToArray()}).ToArray();return d with{Procedures=parents.Concat(appended).ToArray()};
 } static SolutionDocument PruneOwnedGroups(SolutionDocument d)
 {
  var list=d.Procedures.ToList();bool removed;
  do{removed=list.RemoveAll(p=>p.GroupOwner is {} owner&&!list.Any(parent=>parent.Id==owner.ProcedureId&&parent.Nodes.Any(n=>n.Id==owner.NodeId&&n.Call?.ProcedureId==p.Id)))>0;}while(removed);
  return d with{Procedures=list.ToArray()};
 }
}
