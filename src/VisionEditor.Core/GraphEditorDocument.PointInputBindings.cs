using System.Text.Json;
using VisionFramework;
namespace VisionEditor.Core;
public sealed partial class GraphEditorDocument {
 /// <summary>Independent managed atomic edit for a set of leaf input bindings.</summary>
 public void SetProcedureInputSubscriptions(string procedureId,string nodeId,IReadOnlyDictionary<string,WorkflowValueReference?> changes){
  if(changes.Count==0)return;var p=Procedure(procedureId);var node=p.Nodes.Single(n=>n.Id==nodeId);var descriptor=registry.GetDescriptor(node);
  if(changes.Keys.Any(name=>!descriptor.Inputs.Any(port=>port.Name==name)))throw new InvalidOperationException("Unknown batch input destination.");
  var subscriptions=p.Configuration.Subscriptions.Where(s=>s.NodeId!=nodeId||s.IsParameter||!changes.ContainsKey(s.Target)).ToList();foreach(var pair in changes)if(pair.Value is {} source)subscriptions.Add(new(nodeId,pair.Key,false,source));
  var next=p with{Connections=p.Connections.Where(c=>c.ToNode!=nodeId||!changes.ContainsKey(c.ToPort)).ToArray(),Configuration=p.Configuration with{Subscriptions=subscriptions.ToArray()}};
  next.Configuration.Validate(next,registry);var invalid=GraphValidator.Validate(next,registry).Issues.FirstOrDefault(i=>i.Code is "DataExecutionOrder" or "Cycle" or "WorkflowConfiguration");if(invalid is not null)throw new InvalidOperationException(invalid.Message);
  Edit(d=>d with{Procedures=d.Procedures.Select(flow=>flow.Id==procedureId?next:flow).ToArray()});
 }
}
