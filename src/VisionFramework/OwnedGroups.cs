namespace VisionFramework;
public static class OwnedGroups
{
 public static void Validate(SolutionDocument d)
 {
  var byId=d.Procedures.ToDictionary(p=>p.Id);
  foreach(var body in d.Procedures.Where(p=>p.GroupOwner is not null)){
   var owner=body.GroupOwner!;
   if(!byId.TryGetValue(owner.ProcedureId,out var parent)||parent.Nodes.SingleOrDefault(n=>n.Id==owner.NodeId) is not {} node||node.ModuleId!=WorkflowCalls.ModuleId||node.Call?.ProcedureId!=body.Id)throw new InvalidDataException("Group owner is missing or inconsistent.");
   if(d.Procedures.Sum(p=>p.Nodes.Count(n=>n.Call?.ProcedureId==body.Id))!=1)throw new InvalidDataException("Owned Group cannot be shared; copy its internal graph instead.");
   var seen=new HashSet<string>();var cursor=body;int depth=0;while(cursor.GroupOwner is {} edge){if(++depth>3||!seen.Add(cursor.Id)||!byId.TryGetValue(edge.ProcedureId,out cursor!))throw new InvalidDataException("Invalid Group ownership or nesting depth.");}
  }
 }
}
