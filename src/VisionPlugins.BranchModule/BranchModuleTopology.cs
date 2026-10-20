using System.Text.Json;
using VisionFramework;
namespace VisionPlugins.BranchModule;
public sealed partial class BranchModulePlugin {
 // Called by the host transaction only for actual execution link edits. Opening
 // an editor, restoring a document and Undo/Redo must not synthesize this event.
 public NodeDocument ApplyExecutionTopologyChange(NodeDocument node,IReadOnlyList<ExecutionSuccessor> previous,IReadOnlyList<ExecutionSuccessor> current){
  if(node.ModuleId!=Id)throw new ArgumentException("Branch topology provider received another module.",nameof(node));
  if(current.Count>32&&current.Count>previous.Count)throw new InvalidOperationException("Branch module supports at most 32 outgoing execution connections.");
  var rows=BranchRouteTable.Parse(node.Parameters.TryGetValue("ModuleInfoList",out var stored)?stored.GetString():"");
  var update=BranchRouteTable.ApplyTopologyChange(rows,previous.Select(x=>new BranchConnectedNode(x.NodeId,x.NumericId)).ToArray(),current.Select(x=>new BranchConnectedNode(x.NodeId,x.NumericId)).ToArray());
  if(previous.Select(x=>x.NodeId).SequenceEqual(current.Select(x=>x.NodeId)))return node;
  return SetRows(node,BranchRouteTable.Serialize(update.Rows));
 }
 // Source Paste first issues ModuleOperate on the copied instance, then applies
 // remapped rows only when at least one target survives. Copy successors are
 // supplied after the host has constructed its complete copied link map.
 public NodeDocument PrepareCopiedNode(NodeDocument source,NodeDocument copy,IReadOnlyDictionary<string,string> nodeMap,IReadOnlyList<ExecutionSuccessor> copiedSuccessors){
  if(source.ModuleId!=Id||copy.ModuleId!=Id)throw new ArgumentException("Branch copy provider received another module.");
  var seeded=BranchRouteTable.ApplyTopologyChange([],[],copiedSuccessors.Select(x=>new BranchConnectedNode(x.NodeId,x.NumericId)).ToArray());
  copy=SetRows(copy,BranchRouteTable.Serialize(seeded.Rows));
  string? remapped=BranchCopyRows.Remap(source.Parameters.TryGetValue("ModuleInfoList",out var stored)?stored.GetString():"",nodeMap);
  return remapped is null?copy:SetRows(copy,remapped);
 }
 static NodeDocument SetRows(NodeDocument node,string rows){var parameters=node.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());parameters["ModuleInfoList"]=JsonSerializer.SerializeToElement(rows);return node with{Parameters=parameters};}
}

