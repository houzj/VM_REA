using System.Text.Json;
using VisionCore;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
static class GraphChecks{
 public static void Run(Action<bool,string> Check){
 DirectPoint(Check);
 var plugin=new CoordinateModulePlugin();var registry=new ModuleRegistry();new ToolPluginRegistry().Register(plugin,registry);var source=new Source();registry.Register(source.Descriptor,()=>source);
 var doc=new GraphEditorDocument(registry);var sid=doc.AddNode(source.Descriptor.Id,"1",0,0);var target=doc.AddNode(plugin.Manifest.Id,"1.0.0",200,0);doc.ConnectExecution(sid,target);doc.BindInput(sid,"Image",target,"InputImage");
 var values=doc.CurrentProcedure.Nodes.Single(n=>n.Id==target).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());values["CordinateCreate"]=JsonSerializer.SerializeToElement(1);doc.SetParameters(target,values);
 string procedure=doc.CurrentProcedureId;ProcedureDocument Flow()=>doc.Document.Procedures.Single(p=>p.Id==procedure);
 var basic=new WorkflowConfigurationSubscriptionService(Flow,registry,target,(d,r)=>doc.SetProcedureInputSubscription(procedure,target,d.Name,r),changes=>doc.SetProcedureInputSubscriptions(procedure,target,changes.ToDictionary(c=>c.Destination.Name,c=>c.Source)));
 var aliases=new PointAliasConfigurationSubscriptions(basic,Flow,registry,target,plugin.PointInputAliases);
 var combo=new ConfigurationSubscriptionDestination(ConfigurationSubscriptionDomain.Combination,"Src Input Point","POINT");var xd=new ConfigurationSubscriptionDestination(ConfigurationSubscriptionDomain.InputIo,"SrcPointX","float");var yd=new ConfigurationSubscriptionDestination(ConfigurationSubscriptionDomain.InputIo,"SrcPointY","float");var ad=new ConfigurationSubscriptionDestination(ConfigurationSubscriptionDomain.InputIo,"SrcAngle","float");
 ConfigurationSubscriptionBinding Ref(string name,string type,ConfigurationSubscriptionIndex? index=null)=>new(new ConfigurationOutputReference(procedure,sid,name,type),index??ConfigurationSubscriptionIndex.All);
 var traced=new RecordingSubscriptions(aliases);
 var editorContext=new ToolEditorContext(Flow().Nodes.Single(n=>n.Id==target),Environment.CurrentDirectory,v=>doc.SetParameters(target,v),_=>throw new Exception("No configuration run expected")){ConfigurationSubscriptions=traced,ReadEffectiveParameters=()=>Flow().Nodes.Single(n=>n.Id==target).Parameters,ConfigurationConfirmations=new Confirmations(),ConfigurationNotifications=new Notifications()};
 using var configuration=plugin.CreateConfiguration(editorContext);
 var dispatch=typeof(ToolConfigurationState).GetMethod("Dispatch",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)!;
 ConfigurationOperationResult Dispatch(ConfigurationOperationRequest r)=>(ConfigurationOperationResult)dispatch.Invoke(configuration.State,[r])!;
 var imageNode=configuration.State.Find("ImageSource")!;
 var imageQuery=ConfigurationSubscriptionValidation.FromElement(imageNode.Element)!;
 Check(traced.Queries.Any(q=>q.Destination.Name=="InputImage"&&q.NativeTypes.SequenceEqual(new[]{"IMAGE"})),"source IMAGE Initers invokes actual provider enumeration service");
 var enumerated=Dispatch(new(ConfigurationOperationKind.EnumerateInputs,"GetFrontParamItemsOperation",imageNode,null,["IMAGE"],ConfigurationChangeKind.Initialization));
 var imageTree=aliases.Enumerate(imageQuery);
 bool HasImage(IEnumerable<ConfigurationSubscriptionTreeNode> nodes)=>nodes.Any(n=>n.Binding?.Reference is ConfigurationOutputReference p&&p.NodeId==sid&&p.PortId=="Image"||HasImage(n.Children));
 Check(enumerated.Handled&&enumerated.Error is null&&imageQuery.Destination.Domain==ConfigurationSubscriptionDomain.Combination&&HasImage(imageTree.Roots),"actual IMAGE enumeration preserves combination destination and upstream image identity");
 Check(imageNode.SubscriptionTree is {Status:ConfigurationSubscriptionStatus.Available} initialTree&&HasImage(initialTree.Roots),"source Initers stores typed IMAGE tree before any dropdown opens");
 var pointNode=configuration.State.Find("SrcInputPoints")!;
 var written=Dispatch(new(ConfigurationOperationKind.WriteInputBinding,"SetCombinationSourceOperation",pointNode,null,["Src Input Point"],ConfigurationChangeKind.UserCommit){SubscriptionBinding=Ref("Points","POINT")});
 Check(written.Handled&&written.Error is null&&pointNode.SubscriptionBinding is not null,"provider actual operation validates/binds and applies authoritative POINT readback");
 var readback=Dispatch(new(ConfigurationOperationKind.ReadInputBinding,"GetSelectedCombinationOperation",pointNode,null,["Src Input Point"],ConfigurationChangeKind.Initialization));
 Check(readback.Handled&&readback.Error is null&&aliases.Read(combo).Binding is not null,"provider source GetSelected reads real alias service identity");
 Check(aliases.Bind(combo,Ref("Points","POINT")).Status==ConfigurationSubscriptionStatus.Available,"actual alias binds POINT array");Check(aliases.Bind(ad,Ref("Angles","float")).Status==ConfigurationSubscriptionStatus.Available,"actual float angle array binds");
 using var scope=new ModuleExecutionScope();NodeRunRecord Run()=>new GraphExecutor(registry).RunAsync(Flow(),Environment.CurrentDirectory,scope:scope).GetAwaiter().GetResult().Nodes.Single(n=>n.NodeId==target);
 var result=Run();Check(result.Status==NodeRunStatus.Success&&result.Outputs["CoordinateOriPointX"].As<double>()==20&&result.Outputs["CoordinateOriPointY"].As<double>()==30,"Graph All POINT reads first two projected leaves");
 Check(aliases.Bind(xd,Ref("Xs","float")).Status==ConfigurationSubscriptionStatus.Available,"X array override binds");result=Run();Check(result.Outputs["CoordinateOriPointX"].As<double>()==80&&result.Outputs["CoordinateOriPointY"].As<double>()==30,"X override preserves Y POINT source");
 var index=ConfigurationSubscriptionIndex.From(new ConfigurationOutputReference(procedure,sid,"Index","int"));aliases.Bind(combo,Ref("Points","POINT",index));result=Run();Check(result.Outputs["CoordinateOriPointX"].As<double>()==40,"actual dynamic selector reads current POINT index");
 source.Index=0;result=Run();Check(result.Outputs["CoordinateOriPointX"].As<double>()==20,"dynamic selector updates per execution");
 aliases.Bind(yd,Ref("Empty","float"));result=Run();Check(result.Status==NodeRunStatus.Success&&result.Outputs["ModuStatus"].As<long>()==0&&result.Outputs["CoordinateOriPointY"].As<double>()==30,"empty bound Y inner failure publishes previous pose with status0");
 aliases.Bind(combo,Ref("Points","POINT"));result=Run();Check(result.Outputs["ModuStatus"].As<long>()==1,"valid array binding recovers status");
 var restored=JsonSerializer.Deserialize<ProcedureDocument>(JsonSerializer.Serialize(Flow()))!;Check(restored.Configuration.Subscriptions.All(x=>x.Target!="Src Input Point"),"persisted POINT alias stores authoritative leaves");
 var matrixDestination=new ConfigurationSubscriptionDestination(ConfigurationSubscriptionDomain.InputIo,"CalibMatrix","float");Check(basic.Bind(matrixDestination,Ref("Matrix","float")).Status==ConfigurationSubscriptionStatus.Available,"actual matrix float-array All source binds");
 var matrixMode=Flow().Nodes.Single(n=>n.Id==target).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());matrixMode["InputWay"]=JsonSerializer.SerializeToElement(2);doc.SetParameters(target,matrixMode);result=Run();Check(result.Status==NodeRunStatus.Success&&result.Outputs["CoordinateOriPointX"].As<double>()==-8,"normal Graph stable-frame batch snapshot reaches source SDK matrix branch");
 source.Matrix=[2];result=Run();Check(result.Outputs["CorCalibMatrix"].As<IReadOnlyList<double>>()[2]==-8,"actual Graph matrix prefix preserves previously known tail");
 var beforeBindings=JsonSerializer.Serialize(Flow().Configuration.Subscriptions);var current=Flow().Nodes.Single(n=>n.Id==target).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());current["InputWay"]=JsonSerializer.SerializeToElement(2);current[ToolDisplayService.SettingsParameter]=JsonSerializer.SerializeToElement("preserve-independent-display");doc.SetParameters(target,current);
 var reset=Dispatch(new(ConfigurationOperationKind.ResetParameters,"Button_ResetAlgoParams",configuration.State.Find("Button_ResetAlgoParams")!,null,[],ConfigurationChangeKind.Command));
 var after=Flow().Nodes.Single(n=>n.Id==target).Parameters;
 Check(reset.Handled&&reset.Error is null&&after["CordinateCreate"].GetInt32()==0&&after["InputWay"].GetInt32()==0,"actual provider reset follows exact two Algorithm.xml defaults");
 Check(after[ToolDisplayService.SettingsParameter].GetString()=="preserve-independent-display"&&JsonSerializer.Serialize(Flow().Configuration.Subscriptions)==beforeBindings,"reset preserves non-list display and input source bindings");
 }
 static void DirectPoint(Action<bool,string> Check){
  var plugin=new CoordinateModulePlugin();var registry=new ModuleRegistry();new ToolPluginRegistry().Register(plugin,registry);var source=new Source();registry.Register(source.Descriptor,()=>source);
  var doc=new GraphEditorDocument(registry);var sid=doc.AddNode(source.Descriptor.Id,"1",0,0);var target=doc.AddNode(plugin.Manifest.Id,"1.0.0",200,0);doc.ConnectExecution(sid,target);doc.BindInput(sid,"Image",target,"InputImage");doc.BindInput(sid,"Points",target,"Src Input Point");doc.BindInput(sid,"Angles",target,"SrcAngle");
  var values=doc.CurrentProcedure.Nodes.Single(n=>n.Id==target).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());values["CordinateCreate"]=JsonSerializer.SerializeToElement(1);doc.SetParameters(target,values);
  using var scope=new ModuleExecutionScope();NodeRunRecord Run()=>new GraphExecutor(registry).RunAsync(doc.CurrentProcedure,Environment.CurrentDirectory,scope:scope).GetAwaiter().GetResult().Nodes.Single(n=>n.NodeId==target);
  var result=Run();Check(result.Status==NodeRunStatus.Success&&result.Outputs["ModuStatus"].As<long>()==1&&result.Outputs["CoordinateOriPointX"].As<double>()==20&&result.Outputs["CoordinateOriPointY"].As<double>()==30,"advertised POINT-array ordinary connection consumes first combined element without configuration alias injection");
  doc.BindInput(sid,"Xs",target,"SrcPointX");result=Run();Check(result.Outputs["CoordinateOriPointX"].As<double>()==80&&result.Outputs["CoordinateOriPointY"].As<double>()==30,"ordinary POINT parent plus explicit X uses leaf priority and parent Y only");
  doc.BindInput(sid,"Empty",target,"SrcPointY");result=Run();Check(result.Outputs["ModuStatus"].As<long>()==0&&result.Outputs["CoordinateOriPointX"].As<double>()==80,"explicit empty Y cannot be silently replaced with old parent payload");
 }
 sealed class RecordingSubscriptions(IToolConfigurationSubscriptionService inner):IToolConfigurationSubscriptionService{
  public List<ConfigurationSubscriptionQuery> Queries=[];
  public ConfigurationSubscriptionTreeResult Enumerate(ConfigurationSubscriptionQuery q){Queries.Add(q);return inner.Enumerate(q);}
  public ConfigurationSubscriptionBindingResult Read(ConfigurationSubscriptionDestination d)=>inner.Read(d);
  public ConfigurationSubscriptionBindingResult Bind(ConfigurationSubscriptionDestination d,ConfigurationSubscriptionBinding? b)=>inner.Bind(d,b);
 }
 sealed class Confirmations:IToolConfigurationConfirmationService{public bool Confirm(System.Windows.Window? owner,ConfigurationConfirmation request)=>true;}
 sealed class Notifications:IToolConfigurationNotificationService{public ConfigurationNotificationResult Notify(System.Windows.Window? owner,ConfigurationNotification notification)=>new(ConfigurationNotificationStatus.Suppressed);}
 sealed class Source:IModule{
  public int Index=1;
  public double[] Matrix=[1,0,-8,0,1,-9,0,0,1];
  public ModuleDescriptor Descriptor{get;}=new("coordinate-input-source","1","Coordinate source",[],[new("Image",DataKind.Image),new("Points",DataKind.Point){Cardinality=ValueCardinality.Array},new("Xs",DataKind.NumberArray),new("Angles",DataKind.NumberArray),new("Empty",DataKind.NumberArray),new("Matrix",DataKind.NumberArray),new("Index",DataKind.Integer)],[]);
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext c,CancellationToken t)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{["Image"]=PortValue.Image(new GrayImage(64,48,new byte[64*48])),["Points"]=PortValue.PointArray([new(20,30),new(40,50)]),["Xs"]=PortValue.Numbers([80,90]),["Angles"]=PortValue.Numbers([0,0]),["Empty"]=PortValue.Numbers([]),["Matrix"]=PortValue.Numbers(Matrix),["Index"]=PortValue.Integer(Index)}));
 }
}
