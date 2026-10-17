using System.Text.Json;
using VisionFramework;
namespace VisionEditor.Core;
public sealed partial class GraphEditorDocument {
 /// <summary>Plugin reset applies accepted parameters and clears their subscriptions in one undoable edit.
 /// Input/ROI/display bindings are separate and remain owned by their respective configuration paths.</summary>
 public void ResetProcedureNodeParameters(string procedureId,string nodeId,Dictionary<string,JsonElement> values){
  var p=Procedure(procedureId);var node=p.Nodes.Single(n=>n.Id==nodeId);var parameters=CheckedParameters(node,values);
  var next=p with{Nodes=p.Nodes.Select(n=>n.Id==nodeId?n with{Parameters=parameters}:n).ToArray(),Configuration=p.Configuration with{Subscriptions=p.Configuration.Subscriptions.Where(b=>b.NodeId!=nodeId||!b.IsParameter).ToArray()}};
  next.Configuration.Validate(next,registry);
  Edit(d=>d with{Procedures=d.Procedures.Select(flow=>flow.Id==procedureId?next:flow).ToArray()});
 }
}
