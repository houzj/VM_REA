using System.Windows;
using VisionFramework;
namespace VisionEditor;
public partial class MainWindow
{
 SubprogramCallWindow CreateSubprogramCallWindow(string id)
 {
  string flow=doc.CurrentProcedureId,solution=doc.SolutionId;
  ProcedureDocument OwnerFlow()=>ConfigurationProcedure(flow);
  NodeDocument Node()=>OwnerFlow().Nodes.Single(n=>n.Id==id);
  void Guard(){if(windowClosed||isLocked||activeRun is not null||doc.SolutionId!=solution||doc.CurrentProcedureId!=flow)throw new InvalidOperationException("当前Group不可编辑。");}
  IEnumerable<WorkflowSourceOption> Sources(DataKind kind){
   foreach(var s in WorkflowSources(flow,kind).Where(s=>s.NodeId!=id))yield return s with{Reference=new(WorkflowSourceKind.NodeOutput,s.NodeId,s.Port)};
   foreach(var v in OwnerFlow().Configuration.Variables.Where(v=>WorkflowSubscriptions.VariableKind(v.Type)==kind))yield return new("variable:"+v.Id,v.Name,kind,"局部变量."+v.Name){Reference=new(WorkflowSourceKind.LocalVariable,v.Id)};
   foreach(var v in OwnerFlow().Configuration.Inputs.Where(v=>v.Kind==kind))yield return new("input:"+v.Id,v.Name,kind,"流程输入."+v.Name){Reference=new(WorkflowSourceKind.WorkflowInput,v.Id)};
  }
  return new(OwnerFlow,Node,()=>doc.Document.Procedures,target=>{Guard();doc.ConfigureSubprogram(id,target);},(port,source)=>{Guard();doc.SetSubprogramInput(id,port,source);},Sources,async()=>{if(activeRun is not null)throw new InvalidOperationException("流程正在运行。");await RunProcedure(flow);if(!workflowSessions.TryGetValue(flow,out var session)||!session.Calls.ContainsKey(id))throw new InvalidOperationException(StatusText.Text);},()=>activeRun?.Cancel(),()=>workflowSessions.GetValueOrDefault(flow)?.Calls.GetValueOrDefault(id)){Owner=this};
 }
 void OpenSubprogramCall(string id){if(isLocked||activeRun is not null||parameterEditorOpen)return;if(doc.CurrentProcedure.Nodes.Single(n=>n.Id==id).Call is null){Safe(()=>{var parent=doc.CurrentProcedureId;doc.EnterOwnedGroup(id);doc.SelectProcedure(parent);});if(doc.CurrentProcedure.Nodes.FirstOrDefault(n=>n.Id==id)?.Call is null)return;}ResetEditingGesture();var window=CreateSubprogramCallWindow(id);parameterEditorOpen=true;try{window.ShowDialog();}finally{parameterEditorOpen=false;}if(window.EnterProcedureId is {} target)SelectWorkflow(target);}
 void EnterSubprogram(string id){var groupNode=doc.CurrentProcedure.Nodes.Single(n=>n.Id==id);if(!groupNode.Enabled||isLocked||activeRun is not null)return;var call=groupNode.Call;if(call is null){Safe(()=>doc.EnterOwnedGroup(id));}else if(doc.Document.Procedures.FirstOrDefault(p=>p.Id==call.ProcedureId)?.GroupOwner is not null){Safe(()=>doc.EnterOwnedGroup(id));}else SelectWorkflow(call.ProcedureId);}
}
