using System.Windows;
using VisionFramework;
namespace VisionEditor;
public partial class MainWindow
{
 string? runningProcedureId;
 bool windowClosed;
 readonly Dictionary<string,ExecutionSessionResult> workflowSessions=new(StringComparer.Ordinal);
 void ClearInvalidWorkflowResults(){if(doc.HasCurrentProcedure)ClearWorkflowCallResults(doc.CurrentProcedureId);results.Clear();RefreshRight();if(configurationProcedureId==doc.CurrentProcedureId)workflowConfigurationWindow?.RefreshResults();}
 bool CanStartWorkflow(bool allowGroup=false,string? procedureId=null)
 {
  if(toolDebugSessions.Any(s=>s.IsBusy)||(!allowGroup&&groupConfigurationWindows.Any(w=>w.IsExecuting))){StatusText.Text="请先停止模块调试。";return false;}
  var procedure=doc.HasCurrentProcedure?ConfigurationProcedure(procedureId??doc.RootProcedureId(doc.CurrentProcedureId)):null;
  if(procedure is null||procedure.Nodes.Count==0){ClearInvalidWorkflowResults();StatusText.Text="请先添加流程和模块";return false;}
  var validation=GraphValidator.Validate(procedure,modules);if(!validation.IsValid){ClearInvalidWorkflowResults();StatusText.Text="流程有连接或参数错误："+string.Join("；",validation.Issues.Select(i=>i.Message));return false;}return true;

 }
 async Task Run()=>await StartRun();
 Task StartRun()=>StartRunCore(false);
 async Task StartRunCore(bool allowGroup,string? procedureId=null)
 {
  if(activeRun is not null||!CanStartWorkflow(allowGroup,procedureId))return;
  continuousRun=false;runningProcedureId=procedureId??doc.RootProcedureId(doc.CurrentProcedureId);var owner=new CancellationTokenSource();activeRun=owner;RefreshAll();
  try{await RunCore(runningProcedureId,owner.Token);}finally{FinishWorkflowRun(owner);}
 }
 async Task<bool> RunCore(string procedureId,CancellationToken token)
 {
  EnsureToolDebugSolution();
  try
  {
   ClearWorkflowCallResults(procedureId);ClearLiveDynamicDescriptors(procedureId);if(doc.HasCurrentProcedure&&doc.RootProcedureId(doc.CurrentProcedureId)==procedureId){results.Clear();RefreshRight();RenderGraph();}if(configurationProcedureId==procedureId)workflowConfigurationWindow?.RefreshResults();
   var control=new ModuleRunControl{Mode=continuousRun?ExecutionMode.Continuous:ExecutionMode.SelfRun};var session=await new SolutionExecutor(modules).RunAsync(new SolutionWorkspace(doc.Document,doc.BaseDirectory),procedureId,progress:CreateExecutionProgress(procedureId),cancellationToken:token,scope:executionScope,control:control);if(control.StopRequested)continuousRun=false;

   StoreWorkflowCallResults(procedureId,session);var elapsed=session.Nodes.Sum(r=>r.ElapsedMilliseconds);RecordWorkflowRun(procedureId,elapsed);
   if(windowClosed)return false;
   var name=doc.Document.Procedures.FirstOrDefault(p=>p.Id==procedureId)?.Name??procedureId;
   history.Add(new{Seq=history.Count+1,Time=DateTime.Now.ToString("HH:mm:ss"),Data=name+" · "+session.Status});
   if(doc.HasCurrentProcedure&&doc.RootProcedureId(doc.CurrentProcedureId)==procedureId){RestoreWorkflowResults();ProcessTimeText.Text=elapsed.ToString("F2");StatusText.Text="流程完成："+session.Status+(session.OutputErrors.Count>0?" · "+string.Join("；",session.OutputErrors):"");StatusIcon.Source=Asset(session.Nodes.All(r=>r.Status is NodeRunStatus.Success or NodeRunStatus.NoMatch or NodeRunStatus.Skipped)?"bottombar_run.png":"bottombar_abnormal.png");RefreshRight();}
   if(workflowOverview)RenderWorkflowOverview();
   if(configurationProcedureId==procedureId)workflowConfigurationWindow?.RefreshResults();
   return session.Status==SessionStatus.Success&&session.Nodes.All(r=>r.Status is NodeRunStatus.Success or NodeRunStatus.Skipped);
  }
  catch(OperationCanceledException){if(!windowClosed)StatusText.Text="运行已停止";}
  catch(Exception e){if(!windowClosed)StatusText.Text="运行失败："+e.Message;}
  return false;
 }
 void ClearWorkflowCallResults(string procedure){var seen=new HashSet<string>();void Clear(string id){if(!seen.Add(id))return;workflowSessions.Remove(id);ClearToolDebugResults(id);var p=doc.Document.Procedures.FirstOrDefault(p=>p.Id==id);if(p is not null)foreach(var n in p.Nodes.Where(n=>n.Call is not null))Clear(n.Call!.ProcedureId);}Clear(procedure);}
 void StoreWorkflowCallResults(string procedure,ExecutionSessionResult session){workflowSessions[procedure]=session;var p=doc.Document.Procedures.Single(x=>x.Id==procedure);foreach(var pair in session.Calls){var target=p.Nodes.Single(n=>n.Id==pair.Key).Call!.ProcedureId;StoreWorkflowCallResults(target,pair.Value);}}
 void RestoreWorkflowResults()
 {
  results.Clear();if(workflowSessions.TryGetValue(doc.CurrentProcedureId,out var session))foreach(var result in session.Nodes)results[result.NodeId]=result;RestoreToolDebugResults(doc.CurrentProcedureId);

 }
 async Task RunProcedure(string id){if(!doc.HasCurrentProcedure||doc.RootProcedureId(doc.CurrentProcedureId)!=id)SelectWorkflow(id);await StartRunCore(false,id);}
 async Task RunLegacyGroupProcedure(string id){SelectWorkflow(id);await StartRunCore(true);}
 void StartContinuous()
 {
  if(activeRun is not null){activeRun.Cancel();return;}
  if(!CanStartWorkflow())return;
  continuousRun=true;runningProcedureId=doc.RootProcedureId(doc.CurrentProcedureId);var owner=new CancellationTokenSource();activeRun=owner;RefreshAll();_=ContinuousRun(runningProcedureId,owner);
 }
 void StartProcedureLoop(string id){if(activeRun is not null){activeRun.Cancel();return;}if(!doc.HasCurrentProcedure||doc.RootProcedureId(doc.CurrentProcedureId)!=id)SelectWorkflow(id);StartContinuous();}
 async Task ContinuousRun(string procedureId,CancellationTokenSource owner)
 {
  try{while(!owner.IsCancellationRequested&&continuousRun){bool ok=await RunCore(procedureId,owner.Token);var p=doc.Document.Procedures.FirstOrDefault(p=>p.Id==procedureId);if(p is null)break;if(!ok&&p.StopOnNg&&!owner.IsCancellationRequested){if(!windowClosed)StatusText.Text="流程 NG，已停止连续运行";break;}if(!owner.IsCancellationRequested){if(p.ContinuousRunIntervalMs>0)await Task.Delay(p.ContinuousRunIntervalMs,owner.Token);else await System.Windows.Threading.Dispatcher.Yield(System.Windows.Threading.DispatcherPriority.Background);}}}
  catch(OperationCanceledException){if(!windowClosed)StatusText.Text="连续运行已停止";}
  catch(Exception error){if(!windowClosed)StatusText.Text="连续运行失败："+error.Message;}
  finally{FinishWorkflowRun(owner);}
 }
 void FinishWorkflowRun(CancellationTokenSource owner)
 {
  if(ReferenceEquals(activeRun,owner)){activeRun=null;runningProcedureId=null;continuousRun=false;owner.Dispose();if(!windowClosed)RefreshAll();}
 }
 protected override void OnClosed(EventArgs e)
 {
  windowClosed=true;
  CloseToolConfiguration();
  executionScope.Dispose();ResetEditingGesture();canvasDragData.Clear();ReleaseToolDragData();activeRun?.Cancel();if(workflowListPopup is not null)workflowListPopup.IsOpen=false;if(toolPopup is not null)toolPopup.IsOpen=false;base.OnClosed(e);
 }
}

