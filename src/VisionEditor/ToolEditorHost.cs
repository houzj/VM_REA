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
  EnsureToolDebugSolution();string flow=doc.CurrentProcedureId,solution=doc.SolutionId;var node=doc.CurrentProcedure.Nodes.Single(n=>n.Id==id);
  void Guard(){Dispatcher.VerifyAccess();if(windowClosed||isLocked||activeRun is not null||doc.SolutionId!=solution||doc.CurrentProcedureId!=flow||!ConfigurationProcedure(flow).Nodes.Any(n=>n.Id==id))throw new InvalidOperationException("当前模块调试会话已失效或流程正在运行。");}
  ToolDebugSession? session=null;
  session=new ToolDebugSession(async(values,token)=>{
   Guard();if(toolDebugSessions.Any(s=>!ReferenceEquals(s,session)&&s.IsBusy))throw new InvalidOperationException("其他模块正在调试。");
   var procedure=ConfigurationProcedure(flow);var cached=ToolDebugInputs(flow);var baseDirectory=doc.BaseDirectory;var invocationInputs=workflowSessions.GetValueOrDefault(flow)?.WorkflowInputs;var invocationIndex=workflowSessions.GetValueOrDefault(flow)?.GroupLoopIndex;InvalidateToolDebugResult(flow,id);NodeRunRecord record;
   try{record=await Task.Run(()=>new NodeDebugExecutor(modules).RunAsync(procedure,id,baseDirectory,values,cached,invocationInputs,cancellationToken:token,groupLoopIndex:invocationIndex));}catch(InvalidDataException e){record=new(id,NodeRunStatus.Failed,new Dictionary<string,PortValue>(),0,"DebugInputUnavailable",e.Message);}
   if(!windowClosed&&doc.SolutionId==solution)StoreToolDebugResult(flow,record);return record;
  });toolDebugSessions.Add(session);
  return new(node,doc.BaseDirectory,values=>{Guard();doc.SetProcedureNodeParameters(flow,id,values);},values=>session.RunOnceAsync(values),doc.CurrentProcedure.Nodes.ToList().FindIndex(n=>n.Id==id)){Session=session};
 }
 Window CreateToolEditorWindow(string id)
 {
  var node=doc.CurrentProcedure.Nodes.Single(n=>n.Id==id);if(plugins.Find(node.ModuleId)is not IWpfToolEditorFactory factory)throw new InvalidOperationException("该节点的独立调试窗口尚未接入。");
  var context=CreateToolEditorContext(id);Window window;
  try{window=factory.CreateWpfEditor(context);}catch{toolDebugSessions.Remove(context.Session!);_=context.Session!.DisposeAsync();throw;}
  BindToolEditorLifetime(window,context);return window;
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
