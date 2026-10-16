using System.Windows;
using VisionFramework;
namespace VisionEditor;
public partial class MainWindow
{
 readonly HashSet<SubprogramCallWindow> groupConfigurationWindows=new();
 SubprogramCallWindow CreateSubprogramCallWindow(string id)
 {
  string flow=doc.CurrentProcedureId,solution=doc.SolutionId;
  ProcedureDocument OwnerFlow()=>ConfigurationProcedure(flow);
  NodeDocument Node()=>OwnerFlow().Nodes.Single(n=>n.Id==id);
  void Guard(){if(windowClosed||isLocked||activeRun is not null||doc.SolutionId!=solution||doc.CurrentProcedureId!=flow)throw new InvalidOperationException("当前Group不可编辑。");}
  IEnumerable<WorkflowSourceOption> Sources(DataKind kind){
   foreach(var s in WorkflowSources(flow,kind).Where(s=>s.NodeId!=id))yield return s with{Reference=new(WorkflowSourceKind.NodeOutput,s.NodeId,s.Port)};
   if(kind==DataKind.Integer&&OwnerFlow().GroupOwner is not null)yield return new("group:loop","LoopIndex",kind,"Group.LoopIndex"){Reference=new(WorkflowSourceKind.GroupLoopIndex,"LoopIndex")};
   foreach(var v in OwnerFlow().Configuration.Variables.Where(v=>WorkflowSubscriptions.VariableKind(v.Type)==kind))yield return new("variable:"+v.Id,v.Name,kind,"局部变量."+v.Name){Reference=new(WorkflowSourceKind.LocalVariable,v.Id)};
   foreach(var v in OwnerFlow().Configuration.Inputs.Where(v=>v.Kind==kind))yield return new("input:"+v.Id,v.Name,kind,"流程输入."+v.Name){Reference=new(WorkflowSourceKind.WorkflowInput,v.Id)};
  }
  ProcedureDocument Body()=>ConfigurationProcedure(Node().Call!.ProcedureId);
  IEnumerable<WorkflowSourceOption> BodySources(DataKind? kind)=>Body().Nodes.SelectMany(n=>modules.GetDescriptor(n).Outputs.Where(p=>!kind.HasValue||p.Kind==kind).Select(p=>new WorkflowSourceOption(n.Id,p.Name,p.Kind,n.Name+"."+p.Name){Cardinality=p.Cardinality}));
  IEnumerable<GroupComparisonOption> CompareSources(DataKind kind){
   foreach(var option in Sources(kind)){var reference=option.Reference!;yield return new("父级."+option.Label,new(false,reference),WorkflowSubscriptions.SourceCardinality(OwnerFlow(),modules,reference)==ValueCardinality.Array);}
   foreach(var output in Node().Call!.Outputs.Where(p=>p.Kind==kind))yield return new("Group."+output.Name,new(false,new(WorkflowSourceKind.NodeOutput,id,output.Name),Self:true),output.Cardinality==ValueCardinality.Array);
   foreach(var option in BodySources(kind))yield return new("内部."+option.Label,new(true,new(WorkflowSourceKind.NodeOutput,option.NodeId,option.Port)),option.Cardinality==ValueCardinality.Array);
   foreach(var v in Body().Configuration.Variables.Where(v=>WorkflowSubscriptions.VariableKind(v.Type)==kind))yield return new("内部.局部变量."+v.Name,new(true,new(WorkflowSourceKind.LocalVariable,v.Id)));
   foreach(var v in Body().Configuration.Inputs.Where(v=>v.Kind==kind))yield return new("内部.输入."+v.Name,new(true,new(WorkflowSourceKind.WorkflowInput,v.Id)),v.Cardinality==ValueCardinality.Array);
   if(kind==DataKind.Integer)yield return new("内部.Group.LoopIndex",new(true,new(WorkflowSourceKind.GroupLoopIndex,"LoopIndex")));
  }
  void Invalidate(){ClearWorkflowCallResults(flow);results.Clear();RefreshRight();}
  var window=new SubprogramCallWindow(OwnerFlow,Node,()=>doc.Document.Procedures,target=>{Guard();doc.ConfigureSubprogram(id,target);},(port,source)=>{Guard();doc.SetSubprogramInput(id,port,source);},Sources,async()=>{if(activeRun is not null)throw new InvalidOperationException("流程正在运行。");await RunProcedure(flow);if(!workflowSessions.TryGetValue(flow,out var session)||!session.Calls.ContainsKey(id))throw new InvalidOperationException(StatusText.Text);},()=>{if(doc.SolutionId==solution&&runningProcedureId==flow)activeRun?.Cancel();},()=>workflowSessions.GetValueOrDefault(flow)?.Calls.GetValueOrDefault(id),(key,value)=>{Guard();doc.SetParameter(id,key,value);},(key,source)=>{Guard();var config=OwnerFlow().Configuration;var remaining=config.Subscriptions.Where(b=>b.NodeId!=id||b.Target!=key||!b.IsParameter);doc.SetWorkflowConfiguration(flow,config with{Subscriptions=(source is null?remaining:remaining.Append(new(id,key,true,source))).ToArray()});},(enabled,type,comparison,bindings)=>{Guard();doc.SetGroupBreak(id,enabled,type,comparison,bindings);Invalidate();},CompareSources,configuration=>{Guard();doc.SetWorkflowConfiguration(Body().Id,configuration);Invalidate();},BodySources){Owner=this};groupConfigurationWindows.Add(window);window.Closed+=(_,_)=>groupConfigurationWindows.Remove(window);return window;
 }
 void OpenSubprogramCall(string id){if(isLocked||activeRun is not null||parameterEditorOpen)return;if(doc.CurrentProcedure.Nodes.Single(n=>n.Id==id).Call is null){Safe(()=>{var parent=doc.CurrentProcedureId;doc.EnterOwnedGroup(id);doc.SelectProcedure(parent);});if(doc.CurrentProcedure.Nodes.FirstOrDefault(n=>n.Id==id)?.Call is null)return;}ResetEditingGesture();var window=CreateSubprogramCallWindow(id);parameterEditorOpen=true;try{window.ShowDialog();}finally{parameterEditorOpen=false;}if(window.EnterProcedureId is {} target)SelectWorkflow(target);}
 void EnterSubprogram(string id){var groupNode=doc.CurrentProcedure.Nodes.Single(n=>n.Id==id);if(!groupNode.Enabled||isLocked||activeRun is not null)return;var call=groupNode.Call;if(call is null){Safe(()=>doc.EnterOwnedGroup(id));}else if(doc.Document.Procedures.FirstOrDefault(p=>p.Id==call.ProcedureId)?.GroupOwner is not null){Safe(()=>doc.EnterOwnedGroup(id));}else SelectWorkflow(call.ProcedureId);}
}
