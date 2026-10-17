using System.ComponentModel;
using System.Text.Json;
using System.Windows;
using VisionFramework;
using VisionPluginContracts;
namespace VisionEditor;
public partial class MainWindow
{
 readonly HashSet<IToolDebugSession> toolDebugSessions=new();
 readonly Dictionary<string,Dictionary<string,NodeRunRecord>> toolDebugRecords=new();
 readonly Dictionary<string,HashSet<string>> toolDebugInvalid=new();
 string? debugSolution;bool toolClosePending,toolCloseReady;
 void EnsureToolDebugSolution(){if(debugSolution==doc.SolutionId)return;debugSolution=doc.SolutionId;toolDebugRecords.Clear();toolDebugInvalid.Clear();workflowSessions.Clear();results.Clear();}
 IReadOnlyList<NodeRunRecord> ToolDebugInputs(string flow){EnsureToolDebugSolution();var merged=new Dictionary<string,NodeRunRecord>();if(workflowSessions.TryGetValue(flow,out var previous))foreach(var record in previous.Nodes)if(!toolDebugInvalid.GetValueOrDefault(flow)?.Contains(record.NodeId)??true)merged[record.NodeId]=record;if(toolDebugRecords.TryGetValue(flow,out var current))foreach(var pair in current)merged[pair.Key]=pair.Value;return merged.Values.ToArray();}
 void RestoreToolDebugResults(string flow){EnsureToolDebugSolution();if(toolDebugInvalid.TryGetValue(flow,out var invalid))foreach(var id in invalid)results.Remove(id);if(toolDebugRecords.TryGetValue(flow,out var records))foreach(var pair in records)results[pair.Key]=pair.Value;}
 void ClearToolDebugResults(string flow){toolDebugRecords.Remove(flow);toolDebugInvalid.Remove(flow);}
 void InvalidateToolDebugResult(string flow,string nodeId)
 {
  EnsureToolDebugSolution();var procedure=ConfigurationProcedure(flow);var affected=new HashSet<string>{nodeId};
  var edges=ProcedureGraph.ExecutionEdges(procedure).Select(e=>(From:e.FromNode,To:e.ToNode)).Concat(procedure.Connections.Select(c=>(From:c.FromNode,To:c.ToNode))).Concat(procedure.Configuration.Subscriptions.Select(b=>(From:WorkflowSubscriptions.Producer(procedure,b.Source),To:b.NodeId)).Where(e=>e.From is not null).Select(e=>(From:e.From!,To:e.To))).ToArray();
  bool changed;do{changed=false;foreach(var edge in edges)if(affected.Contains(edge.From))changed|=affected.Add(edge.To);}while(changed);
  if(!toolDebugInvalid.TryGetValue(flow,out var invalid))toolDebugInvalid[flow]=invalid=new();invalid.UnionWith(affected);
  if(!toolDebugRecords.TryGetValue(flow,out var records))toolDebugRecords[flow]=records=new();foreach(var id in affected)records.Remove(id);if(doc.CurrentProcedureId==flow){RestoreWorkflowResults();RefreshRight();}
 }
 void StoreToolDebugResult(string flow,NodeRunRecord record){InvalidateToolDebugResult(flow,record.NodeId);toolDebugRecords[flow][record.NodeId]=record;
  if(doc.CurrentProcedureId==flow){RestoreWorkflowResults();RefreshRight();StatusText.Text="模块调试："+record.Status+(record.Message is null?"":" · "+record.Message);}
 }
 ToolEditorContext CreateToolEditorContext(string id)
 {
  var flow=doc.CurrentProcedureId;return CreateScopedToolEditorContext(flow,id,()=>flow);
 }
 ToolEditorContext CreateScopedToolEditorContext(string flow,string id,Func<string> activeScope)
 {
  EnsureToolDebugSolution();string solution=doc.SolutionId;var node=ConfigurationProcedure(flow).Nodes.Single(n=>n.Id==id);
  void Guard(){Dispatcher.VerifyAccess();if(windowClosed||isLocked||activeRun is not null||doc.SolutionId!=solution||activeScope()!=doc.CurrentProcedureId||!ConfigurationProcedure(flow).Nodes.Any(n=>n.Id==id))throw new InvalidOperationException("当前模块调试会话已失效或流程正在运行。");}
  ToolDebugSession? session=null;
  session=new ToolDebugSession(async(values,token)=>{
   Guard();if(toolDebugSessions.Any(s=>!ReferenceEquals(s,session)&&s.IsBusy))throw new InvalidOperationException("其他模块正在调试。");
   var procedure=ConfigurationProcedure(flow);var cached=ToolDebugInputs(flow);var baseDirectory=doc.BaseDirectory;var invocationInputs=workflowSessions.GetValueOrDefault(flow)?.WorkflowInputs;var invocationIndex=workflowSessions.GetValueOrDefault(flow)?.GroupLoopIndex;InvalidateToolDebugResult(flow,id);NodeRunRecord record;
   try{record=await Task.Run(()=>new NodeDebugExecutor(modules).RunAsync(procedure,id,baseDirectory,values,cached,invocationInputs,cancellationToken:token,groupLoopIndex:invocationIndex));}catch(InvalidDataException e){record=new(id,NodeRunStatus.Failed,new Dictionary<string,PortValue>(),0,"DebugInputUnavailable",e.Message);}
   if(!windowClosed&&doc.SolutionId==solution)StoreToolDebugResult(flow,record);return record;
  });toolDebugSessions.Add(session);
  return new(node,doc.BaseDirectory,values=>{Guard();doc.SetProcedureNodeParameters(flow,id,values);InvalidateToolDebugResult(flow,id);},values=>session.RunOnceAsync(values),ConfigurationProcedure(flow).Nodes.ToList().FindIndex(n=>n.Id==id)){Session=session,ApplyConfigurationChange=(values,rois,clear)=>{Guard();doc.SetProcedureToolConfiguration(flow,id,values,rois,clear);InvalidateToolDebugResult(flow,id);},ClearInput=port=>{Guard();doc.SetProcedureInputSubscription(flow,id,port,null);InvalidateToolDebugResult(flow,id);},PreviewInput=port=>ResolveToolInput(flow,id,port),ApplyDisplayStyles=styles=>{Guard();doc.SetProcedureDisplayStyles(flow,id,styles);},InputLabel=port=>ToolInputLabel(flow,id,port),EditInput=(port,owner)=>{Guard();EditToolInput(flow,id,port,owner);},PreviewImage=()=>ResolveGroupToolImage(flow,id),ApplyConfiguration=(values,rois)=>{Guard();doc.SetProcedureToolConfiguration(flow,id,values,rois);InvalidateToolDebugResult(flow,id);}};
 }
 string ToolInputLabel(string flow,string id,string port){var p=ConfigurationProcedure(flow);var edge=p.Connections.FirstOrDefault(e=>e.ToNode==id&&e.ToPort==port);if(edge is not null)return p.Nodes.Single(n=>n.Id==edge.FromNode).Name+"."+edge.FromPort;var subscription=p.Configuration.Subscriptions.FirstOrDefault(b=>b.NodeId==id&&!b.IsParameter&&b.Target==port);if(subscription is null)return "------";var source=subscription.Source;return source.Kind==WorkflowSourceKind.NodeOutput?p.Nodes.Single(n=>n.Id==source.Id).Name+"."+source.Port:source.Kind+"."+source.Id;}
 void EditToolInput(string flow,string id,string port,Window owner){var p=ConfigurationProcedure(flow);var node=p.Nodes.Single(n=>n.Id==id);var input=modules.GetDescriptor(node).Inputs.Single(i=>i.Name==port);var options=WorkflowSources(flow,input.Kind).Where(o=>o.NodeId!=id&&o.Cardinality==input.Cardinality).Select(o=>o with{Reference=new(WorkflowSourceKind.NodeOutput,o.NodeId,o.Port)}).Concat(p.Configuration.Inputs.Where(i=>i.Kind==input.Kind&&i.Cardinality==input.Cardinality).Select(i=>new WorkflowSourceOption("workflow-input",i.Name,i.Kind,"流程输入."+i.Name){Reference=new(WorkflowSourceKind.WorkflowInput,i.Id)})).Concat(p.Configuration.Variables.Where(v=>input.Cardinality==ValueCardinality.Scalar&&WorkflowSubscriptions.VariableKind(v.Type)==input.Kind).Select(v=>new WorkflowSourceOption("local-variable",v.Name,input.Kind,"局部变量."+v.Name){Reference=new(WorkflowSourceKind.LocalVariable,v.Id)}));var picker=new WorkflowSourcePicker(new[]{new WorkflowSourceOption("","",input.Kind,"解除订阅")}.Concat(options)){Owner=owner};if(picker.ShowDialog()==true&&picker.Selection is {} choice){doc.SetProcedureInputSubscription(flow,id,port,choice.Reference);InvalidateToolDebugResult(flow,id);}}
 Window CreateToolEditorWindow(string id)
 {
  var node=doc.CurrentProcedure.Nodes.Single(n=>n.Id==id);if(plugins.Find(node.ModuleId)is not IWpfToolEditorFactory factory)throw new InvalidOperationException("该节点的独立调试窗口尚未接入。");
  var context=CreateToolEditorContext(id);Window window;
  try{window=factory.CreateWpfEditor(context);}catch{toolDebugSessions.Remove(context.Session!);_=context.Session!.DisposeAsync();throw;}
  BindToolEditorLifetime(window,context);return window;
 }
 PortValue? ResolveToolInput(string flow,string id,string port){try{var previous=workflowSessions.GetValueOrDefault(flow);return new NodeDebugExecutor(modules).ResolveInput(ConfigurationProcedure(flow),id,port,ToolDebugInputs(flow),previous?.WorkflowInputs,previous?.GroupLoopIndex);}catch(InvalidDataException){return null;}}
 VisionCore.GrayImage? ResolveGroupToolImage(string flow,string nodeId)
 {
  var procedure=ConfigurationProcedure(flow);var node=procedure.Nodes.Single(n=>n.Id==nodeId);var descriptor=modules.GetDescriptor(node);var records=ToolDebugInputs(flow);var previous=workflowSessions.GetValueOrDefault(flow);var inputs=descriptor.Inputs.Where(p=>p.Kind==DataKind.GrayImage&&p.Cardinality==ValueCardinality.Scalar).ToArray();
  if(inputs.Length==1){try{return new NodeDebugExecutor(modules).ResolveInput(procedure,nodeId,inputs[0].Name,records,previous?.WorkflowInputs,previous?.GroupLoopIndex).Value as VisionCore.GrayImage;}catch(InvalidDataException){return null;}}
  if(inputs.Length>1)return null;var images=records.SingleOrDefault(r=>r.NodeId==nodeId)?.Outputs.Values.Select(v=>v.Value).OfType<VisionCore.GrayImage>().ToArray();return images?.Length==1?images[0]:null;
 }
 void OpenOwnedGroupToolEditor(string parentFlow,string groupId,string bodyFlow,string nodeId,Window owner)
 {
  var node=ConfigurationProcedure(bodyFlow).Nodes.Single(n=>n.Id==nodeId);
  if(plugins.Find(node.ModuleId)is not IWpfToolEditorFactory factory)throw new InvalidOperationException("此工具尚未提供专用WPF编辑器，不能用通用窗口替代。");
  var context=CreateScopedToolEditorContext(bodyFlow,nodeId,()=>doc.CurrentProcedureId==bodyFlow?bodyFlow:parentFlow);Window editor;
  try{editor=factory.CreateWpfEditor(context);}catch{toolDebugSessions.Remove(context.Session!);_=context.Session!.DisposeAsync();throw;}
  BindToolEditorLifetime(editor,context);editor.Owner=owner;
  try{editor.ShowDialog();}finally{InvalidateToolDebugResult(parentFlow,groupId);}
 }
 void BindToolEditorLifetime(Window window,ToolEditorContext context){window.Owner=this;bool ready=false,pending=false;
  window.Closing+=async(_,e)=>{if(ready||e.Cancel)return;e.Cancel=true;if(pending)return;pending=true;try{await context.Session!.DisposeAsync();}catch(Exception error){if(!windowClosed)StatusText.Text="调试关闭："+error.Message;}finally{toolDebugSessions.Remove(context.Session!);ready=true;_ = window.Dispatcher.BeginInvoke(new Action(window.Close));}};
  window.Closed+=(_,_)=>toolDebugSessions.Remove(context.Session!);
 }
 protected override async void OnClosing(CancelEventArgs e)
 {
  base.OnClosing(e);if(e.Cancel||toolCloseReady||(toolDebugSessions.Count==0&&groupConfigurationWindows.Count==0))return;e.Cancel=true;if(toolClosePending)return;toolClosePending=true;activeRun?.Cancel();
  try{foreach(var groupWindow in groupConfigurationWindows.ToArray())await groupWindow.StopAndCloseAsync();foreach(var session in toolDebugSessions.ToArray()){try{await session.DisposeAsync();}catch(Exception error){StatusText.Text="调试关闭："+error.Message;}}}finally{toolDebugSessions.Clear();toolCloseReady=true;_ = Dispatcher.BeginInvoke(new Action(Close));}
 }
}
