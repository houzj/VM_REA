using System.Text.Json;
using System.Windows;
using VisionFramework;
namespace VisionEditor;
public partial class MainWindow
{
 WorkflowConfigurationWindow? workflowConfigurationWindow;
 string? configurationProcedureId;
 ProcedureDocument ConfigurationProcedure(string id)=>doc.Document.Procedures.FirstOrDefault(p=>p.Id==id)??throw new InvalidOperationException("流程已不存在。");
 void WriteWorkflowConfiguration(string solution,string id,WorkflowConfiguration value)
 {
  if(windowClosed||isLocked||activeRun is not null||doc.SolutionId!=solution||!doc.Document.Procedures.Any(p=>p.Id==id))throw new InvalidOperationException("当前流程配置不可编辑。");
  doc.SetWorkflowConfiguration(id,value);
 }
 IEnumerable<WorkflowSourceOption> WorkflowSources(string id,DataKind? kind)=>ConfigurationProcedure(id).Nodes.SelectMany(n=>modules.GetDescriptor(n).Outputs.Where(p=>p.CanSubscribe&&(!kind.HasValue||DataKinds.CanFlow(p.Kind,kind.Value))).Select(p=>new WorkflowSourceOption(n.Id,p.Name,p.Kind,n.Name+"."+WorkflowPresentation.Output(n.ModuleId,p.Name)){Cardinality=p.Cardinality}));
 // VmModule.GetFrontParamItems follows PreModules transitively. Keep this scope on
 // consumer input pickers; procedure output and variable pickers need all nodes.
 IEnumerable<WorkflowSourceOption> WorkflowFrontSources(string procedureId,string targetNodeId,DataKind? kind)
 {
  var p=ConfigurationProcedure(procedureId);
  if(p.ExecutionLinks is null)throw new InvalidOperationException("当前流程缺少显式执行连线，无法确定前置模块。");
  var previous=new Dictionary<string,List<string>>(StringComparer.Ordinal);
  foreach(var link in p.ExecutionLinks){if(!previous.TryGetValue(link.ToNode,out var list))previous[link.ToNode]=list=new();list.Add(link.FromNode);}
  var front=new HashSet<string>(StringComparer.Ordinal);var pending=new Stack<string>();pending.Push(targetNodeId);
  while(pending.TryPop(out var id))if(previous.TryGetValue(id,out var sources))foreach(var source in sources)if(front.Add(source))pending.Push(source);
  front.Remove(targetNodeId);
  return WorkflowSources(procedureId,kind).Where(source=>front.Contains(source.NodeId)&&p.Nodes.Any(node=>node.Id==source.NodeId&&node.Enabled)).ToArray();
 }
 PortValue? WorkflowOutput(string id,string node,string port)=>workflowSessions.TryGetValue(id,out var session)?session.Nodes.FirstOrDefault(n=>n.NodeId==node)?.Outputs.GetValueOrDefault(port):null;
 WorkflowConfigurationWindow CreateWorkflowConfigurationWindow(string id)
 {
  string solution=doc.SolutionId;
  return new WorkflowConfigurationWindow(()=>ConfigurationProcedure(id),value=>WriteWorkflowConfiguration(solution,id,value),kind=>WorkflowSources(id,kind),modules,(node,key,value)=>
  {
   if(windowClosed||isLocked||activeRun is not null||doc.SolutionId!=solution)throw new InvalidOperationException("当前流程参数不可编辑。");
   var n=ConfigurationProcedure(id).Nodes.Single(p=>p.Id==node);var parameters=n.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());parameters[key]=value;doc.SetProcedureNodeParameters(id,node,parameters);
  },()=>RunProcedure(id),(node,port)=>WorkflowOutput(id,node,port),(node,port,source)=>{if(windowClosed||isLocked||activeRun is not null||doc.SolutionId!=solution)throw new InvalidOperationException("当前流程订阅不可编辑。");doc.SetProcedureInputSubscription(id,node,port,source);}){Owner=this};
 }
 void OpenWorkflowConfiguration(string id)
 {
  if(workflowConfigurationWindow is {IsVisible:true}){if(configurationProcedureId==id){workflowConfigurationWindow.Activate();return;}workflowConfigurationWindow.Close();}
  if(!CanEditProcedure(id))return;
  SelectWorkflow(id);ResetEditingGesture();var window=CreateWorkflowConfigurationWindow(id);workflowConfigurationWindow=window;configurationProcedureId=id;parameterEditorOpen=true;
  window.RepeatRequested+=()=>StartProcedureLoop(id);window.StopRequested+=()=>{if(runningProcedureId==id)activeRun?.Cancel();};
  window.Closed+=(_,_)=>{if(ReferenceEquals(workflowConfigurationWindow,window)){workflowConfigurationWindow=null;configurationProcedureId=null;parameterEditorOpen=false;if(!windowClosed)RefreshAll();}};
  try{window.Show();RefreshAll();}catch{workflowConfigurationWindow=null;configurationProcedureId=null;parameterEditorOpen=false;throw;}
 }
 WorkflowLocalVariablesWindow CreateWorkflowLocalVariablesWindow(string id)
 {
  string solution=doc.SolutionId;
  return new WorkflowLocalVariablesWindow(()=>ConfigurationProcedure(id),value=>WriteWorkflowConfiguration(solution,id,value),kind=>WorkflowSources(id,kind),(node,port)=>WorkflowOutput(id,node,port)?.Value.ToString(),modules){Owner=this};
 }
 void OpenWorkflowLocalVariables(string id)
 {
  if(!CanEditProcedure(id))return;ResetEditingGesture();var window=CreateWorkflowLocalVariablesWindow(id);parameterEditorOpen=true;try{window.ShowDialog();}finally{parameterEditorOpen=false;if(!windowClosed)RefreshAll();}
 }
}
