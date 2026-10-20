using VisionFramework;

namespace VisionEditor;

public partial class MainWindow
{
 readonly FrameworkExecutionPreferences frameworkExecutionPreferences=ReadFrameworkExecutionPreferences();
 readonly Dictionary<string,uint> workflowExecuteCounts=new(StringComparer.Ordinal);
 readonly HashSet<string> clearedWorkflowResultViews=new(StringComparer.Ordinal);

 static FrameworkExecutionPreferences ReadFrameworkExecutionPreferences()
 {
  var preferences=FrameworkExecutionPreferences.FromWorkingDirectory("VisionEditor",Environment.CurrentDirectory);
  _=preferences.IsClearLastExecuteResult; // Program reads the original setting at startup.
  return preferences;
 }
 void InitializeFrameworkExecutionPolicy()
 {
  doc.SolutionReplaced+=()=>{workflowExecuteCounts.Clear();clearedWorkflowResultViews.Clear();};
  doc.Changed+=()=>
  {
   var live=doc.Document.Procedures.Select(p=>p.Id).ToHashSet(StringComparer.Ordinal);
   foreach(var id in workflowExecuteCounts.Keys.Where(id=>!live.Contains(id)).ToArray())workflowExecuteCounts.Remove(id);
   clearedWorkflowResultViews.RemoveWhere(id=>!live.Contains(id));
  };
 }
 // Independent UI adapter: keep completed sessions intact; hide their old view
 // until replacement results arrive. This is not native VmIO leaf mutation.
 void BeginWorkflowResults(string procedureId)
 {
  uint previous=workflowExecuteCounts.GetValueOrDefault(procedureId);
  uint count=previous==uint.MaxValue?uint.MaxValue:previous+1;
  workflowExecuteCounts[procedureId]=count;
  if(!frameworkExecutionPreferences.ShouldClearAtBegin(count,1))return;
  ClearLiveDynamicDescriptors(procedureId);
  void Mark(string id)
  {
   if(!clearedWorkflowResultViews.Add(id))return;
   var procedure=doc.Document.Procedures.FirstOrDefault(p=>p.Id==id);
   if(procedure is not null)foreach(var node in procedure.Nodes.Where(n=>n.Call is not null))Mark(node.Call!.ProcedureId);
  }
  Mark(procedureId);
  if(doc.HasCurrentProcedure&&doc.RootProcedureId(doc.CurrentProcedureId)==procedureId){results.Clear();RefreshRight();RenderGraph();}
 }
}
