using System.IO;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.ImageSource;
namespace VisionEditor;

public partial class MainWindow {
 ModuleExecutionScope executionScope=new();
 int activeConfigurationRuns;
 readonly Dictionary<string,int> configurationRunsByProcedure=new();
 readonly Dictionary<string,Dictionary<string,NodeRunRecord>> configurationResults=new(StringComparer.Ordinal);
 ModuleExecutionScope? subscribedExecutionScope;
 Action<ModuleNodeRequest>? scopeRefreshHandler,scopeRunHandler;
 void DetachExecutionScope(){
  if(subscribedExecutionScope is {} old){if(scopeRefreshHandler is not null)old.ProcedureRefreshRequested-=scopeRefreshHandler;if(scopeRunHandler is not null)old.ProcedureRunRequested-=scopeRunHandler;}
  subscribedExecutionScope=null;scopeRefreshHandler=null;scopeRunHandler=null;
 }
 void AttachExecutionScope(){
  DetachExecutionScope();
  if(!AppContext.TryGetSwitch("VisionEditor.DisableAutomaticCameraDiscovery",out bool disabled)||!disabled)
   RefreshCameraProvider(false);
  var owner=executionScope;
  scopeRefreshHandler=request=>Dispatcher.BeginInvoke(()=>{if(!windowClosed&&ReferenceEquals(executionScope,owner)&&request.ScopeId==owner.Id&&request.ProcedureId==doc.CurrentProcedureId)RefreshRight();});
  scopeRunHandler=request=>Dispatcher.BeginInvoke(()=>{if(windowClosed||activeRun is not null||!ReferenceEquals(executionScope,owner)||request.ScopeId!=owner.Id)return;var procedure=doc.Document.Procedures.FirstOrDefault(p=>p.Id==request.ProcedureId&&p.Nodes.Any(n=>n.Id==request.NodeId));if(procedure is not null)_=RunProcedure(procedure.Id);});
  subscribedExecutionScope=owner;owner.ProcedureRefreshRequested+=scopeRefreshHandler;owner.ProcedureRunRequested+=scopeRunHandler;
 }
 void ReplaceExecutionScope(){executionOwnerEpoch.Invalidate();DetachExecutionScope();activeConfigurationRuns=0;configurationRunsByProcedure.Clear();debugSolution=null;toolDebugRecords.Clear();toolDebugInvalid.Clear();CloseToolConfiguration();activeRun?.Cancel();configurationResults.Clear();workflowSessions.Clear();results.Clear();executionScope.Dispose();executionScope=new();AttachExecutionScope();}
 void RetainExecutionNodes()=>executionScope.RetainProcedureNodes(doc.Document.Procedures.SelectMany(p=>p.Nodes.Select(n=>(p.Id,n.ModuleId,n.Id))));
 ToolEditorContext ConnectToolEditorServices(ToolEditorContext context,string node){
  string procedure=doc.CurrentProcedureId,solution=doc.SolutionId;
  var contextOwner=CaptureExecutionOwner();
  bool Alive()=>IsCurrentExecutionOwner(contextOwner)&&(context.ConfigurationActive?.Invoke()??true);
  void RequireAlive(){if(!Alive())throw new InvalidOperationException("配置会话已失效。");}
  ProcedureDocument Flow(){RequireAlive();return doc.Document.Procedures.Single(p=>p.Id==procedure);}
  string Label(string target,bool parameter){var binding=Flow().Configuration.Subscriptions.FirstOrDefault(b=>b.NodeId==node&&b.Target==target&&b.IsParameter==parameter);if(binding is null)return "";var source=binding.Source;return source.Kind switch{WorkflowSourceKind.IntegerConstant or WorkflowSourceKind.NumberConstant or WorkflowSourceKind.NumberArrayConstant=>source.Id,WorkflowSourceKind.TextConstant=>WorkflowSubscriptions.ReadTextConstant(source.Id),WorkflowSourceKind.LocalVariable=>"局部变量."+Flow().Configuration.Variables.First(v=>v.Id==source.Id).Name,WorkflowSourceKind.WorkflowInput=>"流程输入."+Flow().Configuration.Inputs.First(v=>v.Id==source.Id).Name,_=>Flow().Nodes.First(n=>n.Id==source.Id).Name+"."+source.Port};}
  ConfigurationExecutionHostStatus ConfigurationStatus(){
   bool loading=!Alive()||windowClosed||doc.SolutionId!=solution;
   var flow=loading?null:doc.Document.Procedures.FirstOrDefault(p=>p.Id==procedure);
   var target=flow?.Nodes.FirstOrDefault(n=>n.Id==node);
   bool ready=target is not null&&!GraphValidator.Validate(flow!,modules).Issues.Any(i=>i.NodeId is null||i.NodeId==node);
   return new(activeRun is not null||activeConfigurationRuns>0,activeRun is not null&&runningProcedureId==procedure||configurationRunsByProcedure.GetValueOrDefault(procedure)>0,ready,loading,!isLocked&&target?.Enabled==true);
  }
  var configurationSubscriptions=new WorkflowConfigurationSubscriptionService(Flow,modules,node,(destination,reference)=>{
   if(!Alive()||windowClosed||isLocked||activeRun is not null||activeConfigurationRuns>0||doc.SolutionId!=solution)throw new InvalidOperationException("当前模块不可编辑。");
   if(destination.Domain==ConfigurationSubscriptionDomain.Parameter){
    var config=Flow().Configuration;var bindings=config.Subscriptions.Where(b=>b.NodeId!=node||b.Target!=destination.Name||!b.IsParameter);
    doc.SetWorkflowConfiguration(procedure,config with{Subscriptions=(reference is null?bindings:bindings.Append(new(node,destination.Name,true,reference))).ToArray()});
   }else doc.SetProcedureInputSubscription(procedure,node,destination.Name,reference);
  });
  var presentation=new ConfigurationPresentationBridge(request=>{
   if(!Alive()||windowClosed||doc.SolutionId!=solution)return new(ConfigurationPresentationStatus.Failed,"Configuration solution is no longer active.");
   if(request.NodeId!=node||request.Kind!=ConfigurationPresentationKind.ImageSourceMode||!Enum.IsDefined(request.SourceMode))return new(ConfigurationPresentationStatus.Unsupported,"Unsupported presentation request or owner identity.");
   var source=Flow().Nodes.FirstOrDefault(n=>n.Id==node);
   if(source is null||plugins.Find(source.ModuleId)is not IToolImageListProvider)return new(ConfigurationPresentationStatus.Unsupported,"Module does not expose an image list.");
   if(doc.CurrentProcedureId==procedure)RefreshImageSourcePanel(SelectedImageSource());
   return new(ConfigurationPresentationStatus.Supported);
  });
  return context with{ConfigurationFolders=new WpfConfigurationFolderService(),ConfigurationPresentation=presentation,ConfigurationSubscriptions=configurationSubscriptions,ConfigurationFiles=new WpfConfigurationFileService(),ConfigurationNotifications=new WpfConfigurationNotificationService(),ConfigurationExecutionStatus=ConfigurationStatus,Resources=()=>{if(!Alive()||windowClosed||doc.SolutionId!=solution)throw new InvalidOperationException("配置资源所属方案已失效。");return new ResourceCatalog(doc.BaseDirectory,doc.Document.Resources);},Scope=executionScope.ForProcedure(procedure),Run=values=>{if(!Alive())throw new InvalidOperationException("配置会话已关闭。");return RunConfiguredTool(procedure,node,values,Alive);},Cameras=()=>{RequireAlive();return executionScope.Service<IImageAcquisitionService>()?.Cameras??[];},ConfigureCameras=()=>{RequireAlive();RefreshCameraProvider(true);},SubscriptionLabel=Label,
   ConfigureLights=executionScope.Service<ILightResourceConfiguration>() is {} lightConfiguration?()=>{RequireAlive();lightConfiguration.Configure();}:null,
   InputNumber=target=>Flow().Configuration.Subscriptions.FirstOrDefault(b=>b.NodeId==node&&b.Target==target&&!b.IsParameter)?.Source is {Kind:WorkflowSourceKind.NumberConstant} source?WorkflowSubscriptions.ReadNumberConstant(source.Id):null,
   SetInputNumber=(target,value)=>{if(!Alive()||windowClosed||isLocked||activeRun is not null||doc.SolutionId!=solution)throw new InvalidOperationException("当前模块不可编辑。");if(value.HasValue&&!double.IsFinite(value.Value))throw new ArgumentOutOfRangeException(nameof(value));doc.SetProcedureInputSubscription(procedure,node,target,value.HasValue?new(WorkflowSourceKind.NumberConstant,System.Text.Json.JsonSerializer.Serialize(value.Value)):null);},
   InputNumbers=target=>Flow().Configuration.Subscriptions.FirstOrDefault(b=>b.NodeId==node&&b.Target==target&&!b.IsParameter)?.Source is {} source?source.Kind==WorkflowSourceKind.NumberArrayConstant?WorkflowSubscriptions.ReadNumberArrayConstant(source.Id):source.Kind==WorkflowSourceKind.NumberConstant?new[]{WorkflowSubscriptions.ReadNumberConstant(source.Id)}:null:null,
   SetInputNumbers=(target,value)=>{if(!Alive()||windowClosed||isLocked||activeRun is not null||doc.SolutionId!=solution)throw new InvalidOperationException("当前模块不可编辑。");if(value is not null)_=PortValue.Numbers(value);doc.SetProcedureInputSubscription(procedure,node,target,value is not null?new(WorkflowSourceKind.NumberArrayConstant,System.Text.Json.JsonSerializer.Serialize(value)):null);},
   InputText=target=>Flow().Configuration.Subscriptions.FirstOrDefault(b=>b.NodeId==node&&b.Target==target&&!b.IsParameter)?.Source is {Kind:WorkflowSourceKind.TextConstant} source?WorkflowSubscriptions.ReadTextConstant(source.Id):null,
   SetInputText=(target,value)=>{if(!Alive()||windowClosed||isLocked||activeRun is not null||doc.SolutionId!=solution)throw new InvalidOperationException("当前模块不可编辑。");doc.SetProcedureInputSubscription(procedure,node,target,value is null?null:new(WorkflowSourceKind.TextConstant,System.Text.Json.JsonSerializer.Serialize(value)));},
   InputInteger=target=>Flow().Configuration.Subscriptions.FirstOrDefault(b=>b.NodeId==node&&b.Target==target&&!b.IsParameter)?.Source is {Kind:WorkflowSourceKind.IntegerConstant} source?int.Parse(source.Id,System.Globalization.CultureInfo.InvariantCulture):null,
   SetInputInteger=(target,value)=>{if(!Alive()||windowClosed||isLocked||activeRun is not null||doc.SolutionId!=solution)throw new InvalidOperationException("当前模块不可编辑。");doc.SetProcedureInputSubscription(procedure,node,target,value.HasValue?new(WorkflowSourceKind.IntegerConstant,value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)):null);},
   PickSubscription=(owner,target,parameter,kind)=>{
   if(!Alive()||windowClosed||isLocked||activeRun is not null||doc.SolutionId!=solution)throw new InvalidOperationException("当前模块不可编辑。");
   var targetPort=parameter?null:modules.GetDescriptor(Flow().Nodes.Single(n=>n.Id==node)).Inputs.Single(p=>p.Name==target);
   bool Accept(DataKind source)=>targetPort is null?DataKinds.CanFlow(source,kind):DataKinds.CanFlow(source,targetPort);
   var options=new List<WorkflowSourceOption>{new("none","",kind,"不订阅")};
   options.AddRange(Flow().Configuration.Variables.Where(v=>Accept(WorkflowSubscriptions.VariableKind(v.Type))).Select(v=>new WorkflowSourceOption("variable:"+v.Id,v.Name,kind,"局部变量."+v.Name){Reference=new(WorkflowSourceKind.LocalVariable,v.Id)}));
   options.AddRange(Flow().Configuration.Inputs.Where(v=>Accept(v.Kind)).Select(v=>new WorkflowSourceOption("input:"+v.Id,v.Name,v.Kind,"流程输入."+v.Name){Reference=new(WorkflowSourceKind.WorkflowInput,v.Id)}));
   options.AddRange(WorkflowSources(procedure,null).Where(s=>s.NodeId!=node&&Accept(s.Kind)).Select(s=>s with{Reference=new(WorkflowSourceKind.NodeOutput,s.NodeId,s.Port)}));
   var picker=new WorkflowSourcePicker(options){Owner=owner};if(picker.ShowDialog()!=true)return Label(target,parameter);RequireAlive();var source=picker.Selection?.Reference;
   if(parameter){var config=Flow().Configuration;var bindings=config.Subscriptions.Where(b=>b.NodeId!=node||b.Target!=target||!b.IsParameter);doc.SetWorkflowConfiguration(procedure,config with{Subscriptions=(source is null?bindings:bindings.Append(new(node,target,true,source))).ToArray()});}
   else doc.SetProcedureInputSubscription(procedure,node,target,source);
   return Label(target,parameter);
  }};
 }
 sealed class ConfigurationPresentationBridge(Func<ConfigurationPresentationRequest,ConfigurationPresentationResult> apply):IToolConfigurationPresentationService {
  public ConfigurationPresentationResult Apply(ConfigurationPresentationRequest request)=>apply(request);
 }
 async Task<NodeRunRecord> RunConfiguredTool(string procedureId,string nodeId,Dictionary<string,System.Text.Json.JsonElement> values,Func<bool>? configurationAlive=null){
  var runOwner=CaptureExecutionOwner();
  bool CanPublish()=>IsCurrentExecutionOwner(runOwner)&&(configurationAlive?.Invoke()??true);
  if(!CanPublish())throw new InvalidOperationException("配置会话已失效。");
  var procedure=doc.Document.Procedures.Single(p=>p.Id==procedureId);var node=procedure.Nodes.Single(n=>n.Id==nodeId);var descriptor=modules.GetDescriptor(node);var inputs=new Dictionary<string,PortValue>();var parameters=values.ToDictionary(p=>p.Key,p=>p.Value.Clone());
  PortValue Output(string id,string port){
   NodeRunRecord? record=configurationResults.GetValueOrDefault(procedureId)?.GetValueOrDefault(id)
    ??(workflowSessions.TryGetValue(procedureId,out var session)?session.Nodes.FirstOrDefault(n=>n.NodeId==id):null);
   return record is not null&&record.Status is NodeRunStatus.Success or NodeRunStatus.NoMatch&&record.Outputs.TryGetValue(port,out var value)?value:throw new InvalidDataException("订阅图像或数据尚无运行结果。");
  }
  PortValue Resolve(WorkflowValueReference source)=>source.Kind switch{
   WorkflowSourceKind.NumberConstant=>PortValue.Number(WorkflowSubscriptions.ReadNumberConstant(source.Id)),
   WorkflowSourceKind.NumberArrayConstant=>PortValue.Numbers(WorkflowSubscriptions.ReadNumberArrayConstant(source.Id)),
   WorkflowSourceKind.TextConstant=>PortValue.Text(WorkflowSubscriptions.ReadTextConstant(source.Id)),
   WorkflowSourceKind.IntegerConstant=>PortValue.Integer(int.Parse(source.Id,System.Globalization.CultureInfo.InvariantCulture)),
   WorkflowSourceKind.NodeOutput=>Output(source.Id,source.Port!),
   WorkflowSourceKind.LocalVariable=>procedure.Configuration.Variables.Single(v=>v.Id==source.Id) is var variable&&variable.SourceNode is {} producer?Output(producer,variable.SourcePort!):WorkflowSubscriptions.Initial(variable),
   WorkflowSourceKind.WorkflowInput=>procedure.Configuration.Inputs.Single(i=>i.Id==source.Id) is {NodeId:{} producer,Port:{} port}?Output(producer,port):throw new InvalidDataException("流程输入尚无运行结果。"),
   _=>throw new InvalidDataException("无效订阅来源。")};
  var clock=System.Diagnostics.Stopwatch.StartNew();NodeRunRecord record;
  activeConfigurationRuns++;configurationRunsByProcedure[procedureId]=configurationRunsByProcedure.GetValueOrDefault(procedureId)+1;
  var ownerScope=executionScope;var ownerSolution=doc.SolutionId;
  try{
   using var lease=ownerScope.Enter();foreach(var connection in procedure.Connections.Where(c=>c.ToNode==nodeId))if(!inputs.TryAdd(connection.ToPort,Output(connection.FromNode,connection.FromPort)))throw new InvalidDataException("配置输入存在重复绑定。");
   var boundParameters=new HashSet<string>(StringComparer.Ordinal);
   foreach(var binding in procedure.Configuration.Subscriptions.Where(b=>b.NodeId==nodeId)){var value=Resolve(binding.Source);if(binding.IsParameter){if(!boundParameters.Add(binding.Target))throw new InvalidDataException("配置参数存在重复绑定。");parameters[binding.Target]=WorkflowSubscriptions.Parameter(descriptor.Parameters.Single(p=>p.Name==binding.Target),value);}else if(!inputs.TryAdd(binding.Target,value))throw new InvalidDataException("配置输入存在重复绑定。");}
   foreach(var spec in descriptor.Parameters)if(!parameters.ContainsKey(spec.Name)&&spec.DefaultValue is {} def)parameters[spec.Name]=def.Clone();
   foreach(var port in descriptor.Inputs)if(inputs.TryGetValue(port.Name,out var supplied))inputs[port.Name]=DataKinds.Coerce(supplied,port);
   // Reuse the graph runtime's parameter/input/output/status/timeout checks for
   // an isolated node. Configuration dispatch must not bypass those contracts.
   var inputSpecs=inputs.Select(p=>new WorkflowInterfaceItem(p.Key,p.Key,p.Value.Kind)).ToArray();
   var isolated=new ProcedureDocument(procedureId,procedure.Name,[node with{Parameters=parameters}],[],[]){Configuration=new(){Inputs=inputSpecs,Subscriptions=inputSpecs.Select(p=>new WorkflowSubscription(nodeId,p.Name,false,new(WorkflowSourceKind.WorkflowInput,p.Id))).ToArray()}};
   var baseDirectory=doc.BaseDirectory;var resources=new ResourceCatalog(baseDirectory,doc.Document.Resources);
   var run=await Task.Run(()=>new GraphExecutor(modules).RunAsync(isolated,baseDirectory,cancellationToken:ownerScope.Stopping,inputs:inputs,scope:ownerScope,control:new ModuleRunControl{Mode=ExecutionMode.SelfRun},resources:resources));
   record=run.Nodes.Single();
  }catch(Exception ex){record=new(nodeId,NodeRunStatus.Failed,new Dictionary<string,PortValue>(),clock.Elapsed.TotalMilliseconds,"SelfRunFailed",ex.Message);}
  finally{if(IsCurrentExecutionOwner(runOwner)){activeConfigurationRuns--;if(configurationRunsByProcedure.TryGetValue(procedureId,out int count)){if(count<=1)configurationRunsByProcedure.Remove(procedureId);else configurationRunsByProcedure[procedureId]=count-1;}}}
  if(CanPublish()&&doc.SolutionId==ownerSolution&&ReferenceEquals(executionScope,ownerScope)&&doc.Document.Procedures.Any(p=>p.Id==procedureId&&p.Nodes.Any(n=>n.Id==nodeId&&n.ModuleId==node.ModuleId&&n.ModuleVersion==node.ModuleVersion))){
   if(!configurationResults.TryGetValue(procedureId,out var cached))configurationResults[procedureId]=cached=new(StringComparer.Ordinal);
   cached[nodeId]=record;
   if(doc.CurrentProcedureId==procedureId){
    results[nodeId]=record;RefreshRight();RenderGraph();
    StatusText.Text="模块执行："+node.Name+" · "+record.Status+(record.Message is null?"":" · "+record.Message);
    StatusIcon.Source=Asset(record.Status is NodeRunStatus.Success or NodeRunStatus.NoMatch or NodeRunStatus.Skipped?"bottombar_run.png":"bottombar_abnormal.png");
   }
  }
  return record;
 }
 void RefreshCameraProvider(bool report){
  if(activeRun is not null)return;
  try{var provider=MvsCameraAcquisition.TryCreateInstalled();if(provider is not null)executionScope.RegisterService<IImageAcquisitionService>(provider);if(report)System.Windows.MessageBox.Show(this,provider is null?"未找到已安装的相机运行库。":"发现相机 "+provider.Cameras.Count+" 台。"+(provider.Cameras.Count==0?"请检查设备连接。":"\n"+string.Join("\n",provider.Cameras.Select(c=>c.Name))),"相机列表");}
  catch(Exception ex){StatusText.Text="相机运行库不可用："+ex.Message;if(report)System.Windows.MessageBox.Show(this,ex.Message,"相机运行库不可用");}
 }
}
