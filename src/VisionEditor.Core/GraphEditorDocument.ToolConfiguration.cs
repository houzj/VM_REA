using System.Text.Json;
using VisionFramework;
namespace VisionEditor.Core;
public sealed partial class GraphEditorDocument {
 public void SetProcedureToolConfiguration(string flow,string id,Dictionary<string,JsonElement> parameters,IReadOnlyList<RoiDocument> rois){var p=Procedure(flow);var n=p.Nodes.Single(n=>n.Id==id);var d=registry.GetDescriptor(n);var capability=d.Parameters.Select(p=>p.Editor).FirstOrDefault(e=>e?.Kind==ParameterEditorKind.Roi)?.Roi??throw new InvalidOperationException("工具未声明ROI能力。");RoiGeometry.ValidateForEditor(rois,capability);var values=CheckedParameters(n,parameters);Edit(doc=>doc with{Procedures=doc.Procedures.Select(proc=>proc.Id==flow?proc with{Nodes=proc.Nodes.Select(node=>node.Id==id?node with{Parameters=values,Rois=rois.Select(r=>r with{Vertices=r.Vertices?.ToArray()}).ToArray()}:node).ToArray()}:proc).ToArray()});}
}
