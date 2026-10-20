using System.IO;
using VisionFramework;
using VisionEditor.Core;
using VisionPluginContracts;
using VisionPlugins.CameraMapping;

internal static class AllTransportChecks {
 internal static void Run(Action<bool,string> check,string output){
  var plugin=new CameraMappingPlugin();var registry=new ModuleRegistry();new ToolPluginRegistry().Register(plugin,registry);var producer=new Source();registry.Register(producer.Descriptor,()=>producer);var doc=new GraphEditorDocument(registry);string source=doc.AddNode(producer.Descriptor.Id,"1",0,0),target=doc.AddNode(CameraMappingPlugin.Id,"1.0.0",200,0),procedure=doc.CurrentProcedureId;doc.ConnectExecution(source,target);ProcedureDocument Flow()=>doc.CurrentProcedure;
  var bridge=new WorkflowConfigurationSubscriptionService(Flow,registry,target,(d,r)=>doc.SetProcedureInputSubscription(procedure,target,d.Name,r),changes=>doc.SetProcedureInputSubscriptions(procedure,target,changes.ToDictionary(c=>c.Destination.Name,c=>c.Source)));var aliases=new PointAliasConfigurationSubscriptions(bridge,Flow,registry,target,plugin.PointInputAliases);
  foreach(string side in new[]{"Object","Target"})for(int group=1;group<=2;group++)check(aliases.Bind(new(ConfigurationSubscriptionDomain.Combination,CameraMappingPlugin.PointInputName(side,group),"POINT"),new(new ConfigurationOutputReference(procedure,source,side+group,"POINT"),ConfigurationSubscriptionIndex.All)).Status==ConfigurationSubscriptionStatus.Available,"All transport binds "+side+group);
  NodeRunRecord Run(ProcedureDocument? flow=null){using var scope=new ModuleExecutionScope();return new GraphExecutor(registry).RunAsync(flow??Flow(),output,scope:scope).GetAwaiter().GetResult().Nodes.Single(n=>n.NodeId==target);}
  void Missing(string name)=>check(aliases.Bind(new(ConfigurationSubscriptionDomain.InputIo,name,"float"),new(new ConfigurationOutputReference(procedure,source,"OptionalX","float"),ConfigurationSubscriptionIndex.All)).Status==ConfigurationSubscriptionStatus.Available,"bind missing optional All "+name);
  Missing("ObjectPointX3");check(Run().Status==NodeRunStatus.Success,"inactive Data3 missing All leaf is never read and does not preempt runtime");
  var cached=new[]{new NodeRunRecord(source,NodeRunStatus.Success,producer.Values(),0)};var debug=new NodeDebugExecutor(registry).RunAsync(Flow(),target,output,Flow().Nodes.Single(n=>n.Id==target).Parameters,cached).GetAwaiter().GetResult();check(debug.Status==NodeRunStatus.Success,"debug inactive group All missing optional leaf reaches actual runtime");
  Missing("TargetPointX1");var failed=Run();check(failed.NativeErrorCode==0xe0000001&&failed.ErrorCode!="SubscriptionUnavailable","active missing All leaf yields original Count0 versus sibling2 module mismatch");
  debug=new NodeDebugExecutor(registry).RunAsync(Flow(),target,output,Flow().Nodes.Single(n=>n.Id==target).Parameters,cached).GetAwaiter().GetResult();check(debug.NativeErrorCode==0xe0000001,"debug active All missing leaf returns module count mismatch");
  foreach(string leaf in new[]{"TargetPointY1","ObjectPointX1","ObjectPointY1"})Missing(leaf);var excluded=Run();check(excluded.Status==NodeRunStatus.Success,"four missing All group1 leaves exclude group1 while group2 remains usable");
  producer.Fail=true;var skipped=Run();check(skipped.Status==NodeRunStatus.Skipped&&skipped.ErrorCode=="ExecutionPredecessorUnavailable","All transport preserves explicit failed predecessor propagation");var implicitFailed=Run(Flow() with{ExecutionLinks=null});check(implicitFailed.Status==NodeRunStatus.Failed&&implicitFailed.ErrorCode=="SubscriptionUnavailable","All transport preserves implicit DAG failed source exception rather than swallowing into E314");
  var input=new Dictionary<string,PortValue>{{"X",PortValue.Numbers([2,4])}};var selections=new Dictionary<string,WorkflowArrayIndex?>{{"X",null},{"Missing",null}};var reader=new WorkflowNumericInputRead(input,selections,_=>throw new InvalidDataException());check(reader.Read("X",1)==new ModuleNumericInputReadResult(0,2,4),"All primitive uses requested index and original count");check(reader.Read("Missing",0)==new ModuleNumericInputReadResult(0xe0000314,0,0),"All primitive missing value carries E314/count0");check(reader.Read("Unbound",0) is null,"unbound numeric input remains unhandled fallback");
 }
 sealed class Source:IModule {
  public bool Fail;
  public ModuleDescriptor Descriptor{get;}=new("camera-all-source","1","All source",[],new[]{"Object1","Object2","Target1","Target2"}.Select(n=>new PortDescriptor(n,DataKind.Point){Cardinality=ValueCardinality.Array}).Append(new("OptionalX",DataKind.NumberArray,false)).ToArray(),[]);
  public IReadOnlyDictionary<string,PortValue> Values()=>new Dictionary<string,PortValue>{{"Object1",PortValue.PointArray([new(0,0),new(10,0)])},{"Object2",PortValue.PointArray([new(0,10),new(10,10)])},{"Target1",PortValue.PointArray([new(7,-2),new(27,-2)])},{"Target2",PortValue.PointArray([new(7,18),new(27,18)])}};
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>ValueTask.FromResult(Fail?new ModuleResult(ModuleStatus.Failed,new Dictionary<string,PortValue>(),"ControlledProducerFailure"):new ModuleResult(ModuleStatus.Success,Values()));
 }
}
