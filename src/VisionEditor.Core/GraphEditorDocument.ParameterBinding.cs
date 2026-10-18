using System.Text.Json;
using VisionFramework;
namespace VisionEditor.Core;
public sealed partial class GraphEditorDocument {
 public void SetProcedureParameterSubscription(string flow,string id,string parameter,WorkflowValueReference? source,JsonElement? retain=null){
  var p=Procedure(flow);var node=p.Nodes.Single(n=>n.Id==id);var descriptor=registry.GetDescriptor(node);var spec=descriptor.Parameters.Single(s=>s.Name==parameter);
  var values=node.Parameters.ToDictionary(v=>v.Key,v=>v.Value.Clone());if(retain is {} value){if(source is not null||!GraphValidator.ValidParameter(spec,value))throw new InvalidOperationException("Invalid retained parameter value.");values[parameter]=value.Clone();}
  var bindings=p.Configuration.Subscriptions.Where(b=>b.NodeId!=id||b.Target!=parameter||!b.IsParameter).ToList();if(source is not null)bindings.Add(new(id,parameter,true,source));
  var next=p with{Nodes=p.Nodes.Select(n=>n.Id==id?n with{Parameters=CheckedParameters(node,values)}:n).ToArray(),Configuration=p.Configuration with{Subscriptions=bindings.ToArray()}};next.Configuration.Validate(next,registry);
  Edit(document=>document with{Procedures=document.Procedures.Select(item=>item.Id==flow?next:item).ToArray()});
 }
}
