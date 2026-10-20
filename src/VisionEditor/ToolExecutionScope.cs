using System.IO;
using VisionFramework;
using VisionEditor.Core;
using VisionPluginContracts;
using VisionPlugins.ImageSource;
namespace VisionEditor;

public partial class MainWindow {
 ModuleExecutionScope executionScope=new();
 readonly Dictionary<(string Procedure,string Module,string Node),PointAliasPresentationState> pointAliasPresentation=new();
 PointAliasPresentationState PointAliasState(string procedure,string module,string node){var key=(procedure,module,node);if(!pointAliasPresentation.TryGetValue(key,out var state))pointAliasPresentation.Add(key,state=new());return state;}
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
  doc.PrepareLoad=plugins.PrepareLoad;
  doc.PreparePasteSubscriptions=PrepareToolPasteSubscriptions;
  doc.PrepareExecutionTopologyChange=PrepareToolExecutionTopologyChange;
  DetachExecutionScope();
  if(!AppContext.TryGetSwitch("VisionEditor.DisableAutomaticCameraDiscovery",out bool disabled)||!disabled)
   RefreshCameraProvider(false);
  var owner=executionScope;
  scopeRefreshHandler=request=>Dispatcher.BeginInvoke(()=>{if(!windowClosed&&ReferenceEquals(executionScope,owner)&&request.ScopeId==owner.Id&&request.ProcedureId==doc.CurrentProcedureId)RefreshRight();});
  scopeRunHandler=request=>Dispatcher.BeginInvoke(()=>{if(windowClosed||activeRun is not null||!ReferenceEquals(executionScope,owner)||request.ScopeId!=owner.Id)return;var procedure=doc.Document.Procedures.FirstOrDefault(p=>p.Id==request.ProcedureId&&p.Nodes.Any(n=>n.Id==request.NodeId));if(procedure is not null)_=RunProcedure(procedure.Id);});
  subscribedExecutionScope=owner;owner.ProcedureRefreshRequested+=scopeRefreshHandler;owner.ProcedureRunRequested+=scopeRunHandler;
 }
 ProcedureDocument PrepareToolExecutionTopologyChange(ProcedureDocument before,ProcedureDocument after){
  var prior=before.Nodes.ToDictionary(n=>n.Id,StringComparer.Ordinal);
  var oldEdges=ProcedureGraph.ExecutionEdges(before);
  var newEdges=ProcedureGraph.ExecutionEdges(after);
  return after with{Nodes=after.Nodes.Select(node=>{
   if(!prior.ContainsKey(node.Id)||plugins.Find(node.ModuleId) is not IToolExecutionTopologyProvider provider)return node;
   var previous=oldEdges.Where(e=>e.FromNode==node.Id).Select(e=>new ExecutionSuccessor(e.ToNode)).ToArray();
   var current=newEdges.Where(e=>e.FromNode==node.Id).Select(e=>new ExecutionSuccessor(e.ToNode)).ToArray();
   return previous.SequenceEqual(current)?node:provider.ApplyExecutionTopologyChange(node,previous,current);
  }).ToArray()};
 }
 void ReplaceExecutionScope(){executionOwnerEpoch.Invalidate();DetachExecutionScope();activeConfigurationRuns=0;configurationRunsByProcedure.Clear();debugSolution=null;toolDebugRecords.Clear();toolDebugInvalid.Clear();CloseToolConfiguration();activeRun?.Cancel();configurationResults.Clear();pointAliasPresentation.Clear();workflowSessions.Clear();results.Clear();executionScope.Dispose();executionScope=new();AttachExecutionScope();}
 void RetainExecutionNodes(){var nodes=doc.Document.Procedures.SelectMany(p=>p.Nodes.Select(n=>(p.Id,n.ModuleId,n.Id))).ToHashSet();executionScope.RetainProcedureNodes(nodes);foreach(var key in pointAliasPresentation.Keys.Where(k=>!nodes.Contains(k)).ToArray())pointAliasPresentation.Remove(key);}

 ToolEditorContext ConnectToolEditorServices(ToolEditorContext context,string node){
  string procedure=doc.CurrentProcedureId,solution=doc.SolutionId;
  var contextOwner=CaptureExecutionOwner();
  bool Alive()=>IsCurrentExecutionOwner(contextOwner)&&(context.ConfigurationActive?.Invoke()??true);
  void RequireAlive(){if(!Alive())throw new InvalidOperationException("配置会话已失效。");}
  ProcedureDocument Flow(){RequireAlive();return doc.Document.Procedures.Single(p=>p.Id==procedure);}
  string Label(string target,bool parameter){var flow=Flow();var binding=flow.Configuration.Subscriptions.FirstOrDefault(b=>b.NodeId==node&&b.Target==target&&b.IsParameter==parameter);if(binding is null){var edge=parameter?null:flow.Connections.FirstOrDefault(e=>e.ToNode==node&&e.ToPort==target);return edge is null?"":flow.Nodes.First(n=>n.Id==edge.FromNode).Name+"."+edge.FromPort;}var source=binding.Source;return source.Kind switch{WorkflowSourceKind.IntegerConstant or WorkflowSourceKind.NumberConstant or WorkflowSourceKind.NumberArrayConstant=>source.Id,WorkflowSourceKind.TextConstant=>WorkflowSubscriptions.ReadTextConstant(source.Id),WorkflowSourceKind.LocalVariable=>"局部变量."+flow.Configuration.Variables.First(v=>v.Id==source.Id).Name,WorkflowSourceKind.WorkflowInput=>"流程输入."+flow.Configuration.Inputs.First(v=>v.Id==source.Id).Name,_=>flow.Nodes.First(n=>n.Id==source.Id).Name+"."+source.Port};}
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
    var target=Flow().Nodes.Single(n=>n.Id==node);var effective=executionScope.ForProcedure(procedure).ReadEffectiveParameters(target.ModuleId,node,target.Parameters);System.Text.Json.JsonElement? retained=null;if(reference is null&&Flow().Configuration.Subscriptions.Any(b=>b.NodeId==node&&b.Target==destination.Name&&b.IsParameter)&&effective is not null&&effective.TryGetValue(destination.Name,out var current))retained=current;
    doc.SetProcedureParameterSubscription(procedure,node,destination.Name,reference,retained);
   }else doc.SetProcedureInputSubscription(procedure,node,destination.Name,reference);
  },changes=>{
   if(!Alive()||windowClosed||isLocked||activeRun is not null||activeConfigurationRuns>0||doc.SolutionId!=solution)throw new InvalidOperationException("当前模块不可编辑。");
   doc.SetProcedureInputSubscriptions(procedure,node,changes.ToDictionary(change=>change.Destination.Name,change=>change.Source));
  });
  IToolConfigurationSubscriptionService inputSubscriptions=plugins.Find(context.Node.ModuleId) is IToolPointInputAliasProvider aliases?new PointAliasConfigurationSubscriptions(configurationSubscriptions,Flow,modules,node,aliases.PointInputAliases,PointAliasState(procedure,context.Node.ModuleId,node)):configurationSubscriptions;
  IToolConfigurationSubscriptionService completeSubscriptions=new ToolConfigurationDisplaySubscriptions(inputSubscriptions,Flow,modules,node,()=>Flow().Nodes.Single(n=>n.Id==node).Parameters,next=>context.Apply(next));
  var presentation=new ConfigurationPresentationBridge(request=>{
   if(!Alive()||windowClosed||doc.SolutionId!=solution)return new(ConfigurationPresentationStatus.Failed,"Configuration solution is no longer active.");
   if(request.NodeId!=node||request.Kind!=ConfigurationPresentationKind.ImageSourceMode||!Enum.IsDefined(request.SourceMode))return new(ConfigurationPresentationStatus.Unsupported,"Unsupported presentation request or owner identity.");
   var source=Flow().Nodes.FirstOrDefault(n=>n.Id==node);
   if(source is null||plugins.Find(source.ModuleId)is not IToolImageListProvider)return new(ConfigurationPresentationStatus.Unsupported,"Module does not expose an image list.");
   if(doc.CurrentProcedureId==procedure)RefreshImageSourcePanel(SelectedImageSource());
   return new(ConfigurationPresentationStatus.Supported);
  });
  return context with{ReadEffectiveParameters=()=>{if(!Alive()||windowClosed||doc.SolutionId!=solution)return null;var target=Flow().Nodes.Single(n=>n.Id==node);return executionScope.ForProcedure(procedure).ReadEffectiveParameters(target.ModuleId,node,target.Parameters);},ConfigurationFolders=new WpfConfigurationFolderService(),ConfigurationPresentation=presentation,ConfigurationSubscriptions=completeSubscriptions,ConfigurationFiles=new WpfConfigurationFileService(),ConfigurationSaveFiles=new WpfConfigurationSaveFileService(),ConfigurationConfirmations=new WpfConfigurationConfirmationService(),ResetConfigurationParameters=next=>{if(!Alive()||windowClosed||isLocked||activeRun is not null||activeConfigurationRuns>0||doc.SolutionId!=solution)throw new InvalidOperationException("当前模块不可编辑。");var target=Flow().Nodes.Single(n=>n.Id==node);doc.ResetProcedureNodeParameters(procedure,node,next);executionScope.ForProcedure(procedure).RememberConfigurationParameters(target.ModuleId,node,target.Parameters,next,true);},ConfigurationNotifications=new WpfConfigurationNotificationService(),ConfigurationExecutionStatus=ConfigurationStatus,Resources=()=>{if(!Alive()||windowClosed||doc.SolutionId!=solution)throw new InvalidOperationException("配置资源所属方案已失效。");return new ResourceCatalog(doc.BaseDirectory,doc.Document.Resources);},Scope=executionScope.ForProcedure(procedure),Run=values=>{if(!Alive())throw new InvalidOperationException("配置会话已关闭。");return RunConfiguredTool(procedure,node,values,Alive);},Cameras=()=>{RequireAlive();return executionScope.Service<IImageAcquisitionService>()?.Cameras??[];},ConfigureCameras=()=>{RequireAlive();RefreshCameraProvider(true);},SubscriptionLabel=Label,
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
 GraphPastePreparation PrepareToolPasteSubscriptions(GraphPasteContext paste){
  // Native PasteIO reads live source parents at paste time. Only reconstruct a
  // parent whose complete X/Y source exists in this typed root-flow boundary.
  var source=doc.Document.Procedures.SingleOrDefault(p=>p.Id==paste.SourceProcedureId);
  if(source?.GroupOwner is not null)return new(paste.Subscriptions);
  var subscriptions=paste.Subscriptions.ToList();var committed=new List<Action>();
  var copiedNodes=paste.Nodes.Select(copy=>{
   var oldId=paste.NodeIds.Single(pair=>pair.Value==copy.Id).Key;
   var original=paste.SourceNodes.SingleOrDefault(n=>n.Id==oldId);
   if(original is null||plugins.Find(copy.ModuleId) is not IToolNodeCopyProvider provider)return copy;
   var successors=paste.ExecutionLinks.Where(edge=>edge.FromNode==copy.Id).Select(edge=>new ExecutionSuccessor(edge.ToNode)).ToArray();
   // A clipboard fragment may outlive the source node. Only expose live source
   // state when that exact source identity still exists in the current procedure.
   if(provider is IToolScopedNodeCopyProvider scoped){
    bool live=source?.Nodes.Any(node=>node.Id==oldId&&node.ModuleId==original.ModuleId)==true;
    return scoped.PrepareCopiedNode(original,copy,paste.NodeIds,successors,live?executionScope.ForProcedure(source!.Id):null);
   }
   return provider.PrepareCopiedNode(original,copy,paste.NodeIds,successors);
  }).ToArray();
  if(source is not null)foreach(var pair in paste.NodeIds){
   var old=source.Nodes.SingleOrDefault(n=>n.Id==pair.Key);if(old is null||plugins.Find(old.ModuleId) is not IToolPointInputAliasProvider aliases)continue;
   var basic=new WorkflowConfigurationSubscriptionService(()=>source,modules,old.Id,(_,_)=>throw new InvalidOperationException("Clone source is read-only."));
   var view=new PointAliasConfigurationSubscriptions(basic,()=>source,modules,old.Id,aliases.PointInputAliases,PointAliasState(source.Id,old.ModuleId,old.Id));
   foreach(var alias in aliases.PointInputAliases){
    var selected=view.Read(new(ConfigurationSubscriptionDomain.Combination,alias.Combination,"POINT"));
    if(selected.Status!=ConfigurationSubscriptionStatus.Available||selected.ReadOnly||selected.Binding?.Reference is not ConfigurationOutputReference publisher)continue;
    var x=subscriptions.SingleOrDefault(b=>b.NodeId==pair.Value&&!b.IsParameter&&b.Target==alias.X);var y=subscriptions.SingleOrDefault(b=>b.NodeId==pair.Value&&!b.IsParameter&&b.Target==alias.Y);
    if(x?.Source.Kind!=WorkflowSourceKind.NodeOutput||y?.Source.Kind!=WorkflowSourceKind.NodeOutput)continue;
    string publisherId=paste.NodeIds.GetValueOrDefault(publisher.NodeId,publisher.NodeId);
    if(x.Source.Id!=publisherId||y.Source.Id!=publisherId||!((x.Source.Port==publisher.PortId+".X"&&y.Source.Port==publisher.PortId+".Y")||(x.Source.Port==publisher.PortId+".Y"&&y.Source.Port==publisher.PortId+".X")))continue;
    var index=selected.Binding.Index;
    if(index.Reference is ConfigurationOutputReference dynamic){
     if(paste.NodeIds.TryGetValue(publisher.NodeId,out var mappedPublisher)){
      // Exact PasteIO mapped-publisher branch calls new publisher.GetIO(old index.Name).
      // Do not silently remap the index's original owner instead.
      var publisherNode=paste.Nodes.Single(n=>n.Id==mappedPublisher);
      index=modules.GetDescriptor(publisherNode).Outputs.Any(p=>p.Name==dynamic.PortId&&p.Kind==DataKind.Integer)?ConfigurationSubscriptionIndex.From(dynamic with{ProcedureId=paste.TargetProcedureId,NodeId=mappedPublisher}):ConfigurationSubscriptionIndex.All;
     }else if(paste.SourceProcedureId!=paste.TargetProcedureId)continue;
    }
    WorkflowArrayIndex? selector=index.Kind switch{ConfigurationSubscriptionIndexKind.All=>null,ConfigurationSubscriptionIndexKind.Literal=>new(index.LiteralText??index.Literal!.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)),ConfigurationSubscriptionIndexKind.Reference when index.Reference is ConfigurationOutputReference reference=>new(IndexSource:new(WorkflowSourceKind.NodeOutput,reference.NodeId,reference.PortId)),_=>throw new InvalidDataException("Unsupported cloned parent index.")};
    subscriptions[subscriptions.IndexOf(x)]=x with{Source=x.Source with{Port=publisher.PortId+".X",ArrayIndex=selector}};subscriptions[subscriptions.IndexOf(y)]=y with{Source=y.Source with{Port=publisher.PortId+".Y",ArrayIndex=selector}};
    var captured=index;committed.Add(()=>PointAliasState(paste.TargetProcedureId,old.ModuleId,pair.Value).Record(alias.Combination,captured));
   }
  }
  return new(subscriptions,()=>{foreach(var commit in committed)commit();}){Nodes=copiedNodes};
 }
 sealed class ConfigurationPresentationBridge(Func<ConfigurationPresentationRequest,ConfigurationPresentationResult> apply):IToolConfigurationPresentationService {
  public ConfigurationPresentationResult Apply(ConfigurationPresentationRequest request)=>apply(request);
 }
 async Task<NodeRunRecord> RunConfiguredTool(string procedureId,string nodeId,Dictionary<string,System.Text.Json.JsonElement> values,Func<bool>? configurationAlive=null){
  var runOwner=CaptureExecutionOwner();
  bool CanPublish()=>IsCurrentExecutionOwner(runOwner)&&(configurationAlive?.Invoke()??true);
  if(!CanPublish())throw new InvalidOperationException("配置会话已失效。");
  var procedure=doc.Document.Procedures.Single(p=>p.Id==procedureId);var node=procedure.Nodes.Single(n=>n.Id==nodeId);var descriptor=modules.GetDescriptor(node);var inputs=new Dictionary<string,PortValue>();var parameters=values.ToDictionary(p=>p.Key,p=>p.Value.Clone());
  // Capture this self-run's upstream records before dispatch; dynamic selectors read this
  // invocation's snapshot, never a mutable latest-result dictionary from another round.
  var available=(workflowSessions.TryGetValue(procedureId,out var priorSession)?priorSession.Nodes:[]).ToDictionary(r=>r.NodeId,StringComparer.Ordinal);
  if(configurationResults.TryGetValue(procedureId,out var configured))foreach(var cached in configured)available[cached.Key]=cached.Value;
  PortValue Output(string id,string port){
   NodeRunRecord? record=available.GetValueOrDefault(id);
   return record is not null&&record.Status is NodeRunStatus.Success or NodeRunStatus.NoMatch&&CompoundOutputFields.Expand(record.Outputs).TryGetValue(port,out var value)?value:throw new InvalidDataException("订阅图像或数据尚无运行结果。");
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
   var numericIndices=new Dictionary<string,WorkflowArrayIndex?>(StringComparer.Ordinal);
   foreach(var binding in procedure.Configuration.Subscriptions.Where(b=>b.NodeId==nodeId)){PortValue value;if(!binding.IsParameter&&descriptor.Inputs.Single(port=>port.Name==binding.Target) is {} deferred&&(deferred.SupportsDeferredNumericRead||deferred.SupportsDeferredIntegerRead||deferred.SupportsDeferredTextRead)){numericIndices.Add(binding.Target,binding.Source.ArrayIndex);try{value=Resolve(binding.Source with{ArrayIndex=null});}catch(InvalidDataException)when(binding.Source.Kind==WorkflowSourceKind.NodeOutput&&available.TryGetValue(binding.Source.Id,out var producer)&&producer.Status is NodeRunStatus.Success or NodeRunStatus.NoMatch){continue;}}else value=Resolve(binding.Source);if(binding.IsParameter){if(!boundParameters.Add(binding.Target))throw new InvalidDataException("配置参数存在重复绑定。");parameters[binding.Target]=WorkflowSubscriptions.Parameter(descriptor.Parameters.Single(p=>p.Name==binding.Target),value);}else if(!inputs.TryAdd(binding.Target,value))throw new InvalidDataException("配置输入存在重复绑定。");}
   foreach(var spec in descriptor.Parameters)if(!parameters.ContainsKey(spec.Name)&&spec.DefaultValue is {} def)parameters[spec.Name]=def.Clone();
   foreach(var port in descriptor.Inputs)if(inputs.TryGetValue(port.Name,out var supplied))inputs[port.Name]=DataKinds.Coerce(supplied,port);
   // Reuse the graph runtime's parameter/input/output/status/timeout checks for
   // an isolated node. Configuration dispatch must not bypass those contracts.
   var inputSpecs=inputs.Select(p=>new WorkflowInterfaceItem(p.Key,p.Key,p.Value.Kind){Cardinality=p.Value.Cardinality}).ToArray();
   var numericReader=numericIndices.Count==0?null:new WorkflowNumericInputRead(inputs,numericIndices,Resolve);
   var isolated=new ProcedureDocument(procedureId,procedure.Name,[node with{Parameters=parameters}],[],[]){Configuration=new(){Inputs=inputSpecs,Subscriptions=inputSpecs.Select(p=>new WorkflowSubscription(nodeId,p.Name,false,new(WorkflowSourceKind.WorkflowInput,p.Id))).ToArray()}};
   var baseDirectory=doc.BaseDirectory;var resources=new ResourceCatalog(baseDirectory,doc.Document.Resources);
   var originalSuccessors=Array.AsReadOnly(ProcedureGraph.ExecutionEdges(procedure).Where(edge=>edge.FromNode==nodeId).Select(edge=>new ExecutionSuccessor(edge.ToNode)).ToArray());
   var run=await Task.Run(()=>new GraphExecutor(modules).RunAsync(isolated,baseDirectory,cancellationToken:ownerScope.Stopping,inputs:inputs,scope:ownerScope,control:new ModuleRunControl{Mode=ExecutionMode.SelfRun,ParameterSeed=id=>id==nodeId?node.Parameters:null},resources:resources,numericInputRead:(id,name,index)=>id==nodeId?numericReader?.Read(name,index):null,integerInputRead:(id,name,index)=>id==nodeId?numericReader?.ReadInteger(name,index):null,textInputRead:(id,name,index,capacity)=>id==nodeId?numericReader?.ReadText(name,index,capacity):null,executionSuccessorRead:id=>id==nodeId?originalSuccessors:null));
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
