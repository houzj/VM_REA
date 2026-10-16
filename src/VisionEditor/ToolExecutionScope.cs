using System.IO;
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
  string Label(string target,bool parameter){var binding=Flow().Configuration.Subscriptions.FirstOrDefault(b=>b.NodeId==node&&b.Target==target&&b.IsParameter==parameter);if(binding is null)return "";var source=binding.Source;return source.Kind switch{WorkflowSourceKind.IntegerConstant=>source.Id,WorkflowSourceKind.LocalVariable=>"局部变量."+Flow().Configuration.Variables.First(v=>v.Id==source.Id).Name,WorkflowSourceKind.WorkflowInput=>"流程输入."+Flow().Configuration.Inputs.First(v=>v.Id==source.Id).Name,_=>Flow().Nodes.First(n=>n.Id==source.Id).Name+"."+source.Port};}
  return context with{Scope=executionScope,Run=values=>RunConfiguredTool(procedure,node,values),Cameras=()=>executionScope.Service<IImageAcquisitionService>()?.Cameras??[],ConfigureCameras=()=>RefreshCameraProvider(true),SubscriptionLabel=Label,
   InputInteger=target=>Flow().Configuration.Subscriptions.FirstOrDefault(b=>b.NodeId==node&&b.Target==target&&!b.IsParameter)?.Source is {Kind:WorkflowSourceKind.IntegerConstant} source?int.Parse(source.Id,System.Globalization.CultureInfo.InvariantCulture):null,
   SetInputInteger=(target,value)=>{if(windowClosed||isLocked||activeRun is not null||doc.SolutionId!=solution)throw new InvalidOperationException("当前模块不可编辑。");doc.SetProcedureInputSubscription(procedure,node,target,value.HasValue?new(WorkflowSourceKind.IntegerConstant,value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)):null);},
   PickSubscription=(owner,target,parameter,kind)=>{
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
 async Task<NodeRunRecord> RunConfiguredTool(string procedureId,string nodeId,Dictionary<string,System.Text.Json.JsonElement> values){
  var procedure=doc.Document.Procedures.Single(p=>p.Id==procedureId);var node=procedure.Nodes.Single(n=>n.Id==nodeId);var descriptor=modules.GetDescriptor(node);var inputs=new Dictionary<string,PortValue>();var parameters=values.ToDictionary(p=>p.Key,p=>p.Value.Clone());
  PortValue Output(string id,string port)=>results.TryGetValue(id,out var record)&&record.Status is NodeRunStatus.Success or NodeRunStatus.NoMatch&&record.Outputs.TryGetValue(port,out var value)?value:throw new InvalidDataException("订阅图像或数据尚无运行结果。");
  PortValue Resolve(WorkflowValueReference source)=>source.Kind switch{
   WorkflowSourceKind.IntegerConstant=>PortValue.Integer(int.Parse(source.Id,System.Globalization.CultureInfo.InvariantCulture)),
   WorkflowSourceKind.NodeOutput=>Output(source.Id,source.Port!),
   WorkflowSourceKind.LocalVariable=>procedure.Configuration.Variables.Single(v=>v.Id==source.Id) is var variable&&variable.SourceNode is {} producer?Output(producer,variable.SourcePort!):WorkflowSubscriptions.Initial(variable),
   WorkflowSourceKind.WorkflowInput=>procedure.Configuration.Inputs.Single(i=>i.Id==source.Id) is {NodeId:{} producer,Port:{} port}?Output(producer,port):throw new InvalidDataException("流程输入尚无运行结果。"),
   _=>throw new InvalidDataException("无效订阅来源。")};
  var clock=System.Diagnostics.Stopwatch.StartNew();NodeRunRecord record;
  try{
   using var lease=executionScope.Enter();foreach(var connection in procedure.Connections.Where(c=>c.ToNode==nodeId))inputs[connection.ToPort]=Output(connection.FromNode,connection.FromPort);
   foreach(var binding in procedure.Configuration.Subscriptions.Where(b=>b.NodeId==nodeId)){var value=Resolve(binding.Source);if(binding.IsParameter)parameters[binding.Target]=WorkflowSubscriptions.Parameter(descriptor.Parameters.Single(p=>p.Name==binding.Target),value);else inputs[binding.Target]=value;}
   foreach(var spec in descriptor.Parameters)if(!parameters.ContainsKey(spec.Name)&&spec.DefaultValue is {} def)parameters[spec.Name]=def.Clone();
   var runtime=modules.Create(node.ModuleId,node.ModuleVersion);ModuleResult result;
   try{result=await runtime.ExecuteAsync(new(nodeId,doc.BaseDirectory,inputs,parameters,node.Rois){Scope=executionScope,ProcedureId=procedureId,Control=new ModuleRunControl{Mode=ExecutionMode.SelfRun},Resources=new ResourceCatalog(doc.BaseDirectory,doc.Document.Resources)},executionScope.Stopping);}
   finally{if(runtime is IAsyncDisposable asyncDisposable)await asyncDisposable.DisposeAsync();else if(runtime is IDisposable disposable)disposable.Dispose();}
   record=new(nodeId,result.Status==ModuleStatus.Success?NodeRunStatus.Success:result.Status==ModuleStatus.NoMatch?NodeRunStatus.NoMatch:NodeRunStatus.Failed,result.Outputs,clock.Elapsed.TotalMilliseconds,result.ErrorCode,result.Message){NativeErrorCode=result.NativeErrorCode};
  }catch(Exception ex){record=new(nodeId,NodeRunStatus.Failed,new Dictionary<string,PortValue>(),clock.Elapsed.TotalMilliseconds,"SelfRunFailed",ex.Message);}
  results[nodeId]=record;RefreshRight();return record;
 }
 void RefreshCameraProvider(bool report){
  if(activeRun is not null)return;
  try{var provider=MvsCameraAcquisition.TryCreateInstalled();if(provider is not null)executionScope.RegisterService<IImageAcquisitionService>(provider);if(report)System.Windows.MessageBox.Show(this,provider is null?"未找到已安装的相机运行库。":"发现相机 "+provider.Cameras.Count+" 台。"+(provider.Cameras.Count==0?"请检查设备连接。":"\n"+string.Join("\n",provider.Cameras.Select(c=>c.Name))),"相机列表");}
  catch(Exception ex){StatusText.Text="相机运行库不可用："+ex.Message;if(report)System.Windows.MessageBox.Show(this,ex.Message,"相机运行库不可用");}
 }
}
