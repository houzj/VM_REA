using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.ImageSource;
namespace VisionEditor;

public partial class MainWindow {
 ModuleExecutionScope executionScope=new();
 void AttachExecutionScope(){
  RefreshCameraProvider(false);
  var owner=executionScope;
  executionScope.RefreshRequested+=node=>Dispatcher.BeginInvoke(()=>{if(!windowClosed&&ReferenceEquals(executionScope,owner))RefreshRight();});
  executionScope.RunRequested+=node=>Dispatcher.BeginInvoke(()=>{if(windowClosed||activeRun is not null||!ReferenceEquals(executionScope,owner))return;var procedure=doc.Document.Procedures.FirstOrDefault(p=>p.Nodes.Any(n=>n.Id==node));if(procedure is not null)_=RunProcedure(procedure.Id);});
 }
 void ReplaceExecutionScope(){activeRun?.Cancel();executionScope.Dispose();executionScope=new();AttachExecutionScope();}
 void RetainExecutionNodes()=>executionScope.RetainNodes(doc.Document.Procedures.SelectMany(p=>p.Nodes).Select(n=>(n.ModuleId,n.Id)));
 ToolEditorContext ConnectToolEditorServices(ToolEditorContext context,string node){
  string procedure=doc.CurrentProcedureId,solution=doc.SolutionId;
  ProcedureDocument Flow()=>doc.Document.Procedures.Single(p=>p.Id==procedure);
  string Label(string target,bool parameter){var binding=Flow().Configuration.Subscriptions.FirstOrDefault(b=>b.NodeId==node&&b.Target==target&&b.IsParameter==parameter);if(binding is null)return "";var source=binding.Source;return source.Kind switch{WorkflowSourceKind.LocalVariable=>"局部变量."+Flow().Configuration.Variables.First(v=>v.Id==source.Id).Name,WorkflowSourceKind.WorkflowInput=>"流程输入."+Flow().Configuration.Inputs.First(v=>v.Id==source.Id).Name,_=>Flow().Nodes.First(n=>n.Id==source.Id).Name+"."+source.Port};}
  return context with{Scope=executionScope,Cameras=()=>executionScope.Service<IImageAcquisitionService>()?.Cameras??[],ConfigureCameras=()=>RefreshCameraProvider(true),SubscriptionLabel=Label,PickSubscription=(owner,target,parameter,kind)=>{
   if(windowClosed||isLocked||activeRun is not null||doc.SolutionId!=solution)throw new InvalidOperationException("当前模块不可编辑。");
   var options=new List<WorkflowSourceOption>{new("none","",kind,"不订阅")};
   options.AddRange(Flow().Configuration.Variables.Where(v=>WorkflowSubscriptions.VariableKind(v.Type)==kind).Select(v=>new WorkflowSourceOption("variable:"+v.Id,v.Name,kind,"局部变量."+v.Name){Reference=new(WorkflowSourceKind.LocalVariable,v.Id)}));
   options.AddRange(Flow().Configuration.Inputs.Where(v=>DataKinds.CanFlow(v.Kind,kind)).Select(v=>new WorkflowSourceOption("input:"+v.Id,v.Name,v.Kind,"流程输入."+v.Name){Reference=new(WorkflowSourceKind.WorkflowInput,v.Id)}));
   options.AddRange(WorkflowSources(procedure,kind).Where(s=>s.NodeId!=node).Select(s=>s with{Reference=new(WorkflowSourceKind.NodeOutput,s.NodeId,s.Port)}));
   var picker=new WorkflowSourcePicker(options){Owner=owner};if(picker.ShowDialog()!=true)return Label(target,parameter);var source=picker.Selection?.Reference;
   if(parameter){var config=Flow().Configuration;var bindings=config.Subscriptions.Where(b=>b.NodeId!=node||b.Target!=target||!b.IsParameter);doc.SetWorkflowConfiguration(procedure,config with{Subscriptions=(source is null?bindings:bindings.Append(new(node,target,true,source))).ToArray()});}
   else doc.SetProcedureInputSubscription(procedure,node,target,source);
   return Label(target,parameter);
  }};
 }
 void RefreshCameraProvider(bool report){
  if(activeRun is not null)return;
  try{var provider=MvsCameraAcquisition.TryCreateInstalled();if(provider is not null)executionScope.RegisterService<IImageAcquisitionService>(provider);if(report)System.Windows.MessageBox.Show(this,provider is null?"未找到已安装的相机运行库。":"发现相机 "+provider.Cameras.Count+" 台。"+(provider.Cameras.Count==0?"请检查设备连接。":"\n"+string.Join("\n",provider.Cameras.Select(c=>c.Name))),"相机列表");}
  catch(Exception ex){StatusText.Text="相机运行库不可用："+ex.Message;if(report)System.Windows.MessageBox.Show(this,ex.Message,"相机运行库不可用");}
 }
}
