using System.Text.Json;
using VisionFramework;
namespace VisionEditor.Core;

public sealed partial class GraphEditorDocument {
 public void SetProcedureToolConfigurationRoiGroup(string flow,string id,string sourceName,Dictionary<string,JsonElement> parameters,IReadOnlyList<RoiDocument> rois){
  if(string.IsNullOrWhiteSpace(sourceName))throw new ArgumentException("ROI source name is required.",nameof(sourceName));
  var p=Procedure(flow);var n=p.Nodes.Single(n=>n.Id==id);var descriptor=registry.GetDescriptor(n);var capability=descriptor.RoiEditor??descriptor.Parameters.Select(item=>item.Editor).FirstOrDefault(editor=>editor?.Kind==ParameterEditorKind.Roi)?.Roi??throw new InvalidOperationException("工具未声明ROI能力。");
  RoiGeometry.ValidateForEditor(rois,capability);var values=CheckedParameters(n,parameters);var groups=(n.RoiGroups??new Dictionary<string,IReadOnlyList<RoiDocument>>(StringComparer.Ordinal)).ToDictionary(pair=>pair.Key,pair=>(IReadOnlyList<RoiDocument>)pair.Value.Select(RoiData.Clone).ToArray(),StringComparer.Ordinal);groups[sourceName]=rois.Select(RoiData.Clone).ToArray();
  var next=p with{Nodes=p.Nodes.Select(node=>node.Id==id?node with{Parameters=values,RoiGroups=groups,Rois=sourceName is "RoiType" or "RoiType_0"?rois.Select(RoiData.Clone).ToArray():node.Rois}:node).ToArray()};next.Configuration.Validate(next,registry);
  Edit(document=>document with{Procedures=document.Procedures.Select(proc=>proc.Id==flow?next:proc).ToArray()});
 }
}
