using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionEditor.Core;
using VisionPluginContracts;
using VisionPlugins.DistortionCalibration;
static class Program {
 [STAThread] static int Main(){try{Run();return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
 static void Run(){
 int checks=0;void Check(bool b,string text){if(!b)throw new Exception(text);checks++;}
 var plugin=new DistortionCalibrationPlugin();var modules=new ModuleRegistry();new ToolPluginRegistry().Register(plugin,modules);var source=new Source();modules.Register(source.Descriptor,()=>new Source());
 var document=new GraphEditorDocument(modules);var sourceId=document.AddNode("center-source","1",0,0);var target=document.AddNode(plugin.Manifest.Id,"1.0.0",200,0);document.ConnectExecution(sourceId,target);var procedure=document.CurrentProcedureId;ProcedureDocument Flow()=>document.Document.Procedures.Single(p=>p.Id==procedure);
 var basic=new WorkflowConfigurationSubscriptionService(Flow,modules,target,(d,r)=>document.SetProcedureInputSubscription(procedure,target,d.Name,r),changes=>document.SetProcedureInputSubscriptions(procedure,target,changes.ToDictionary(c=>c.Destination.Name,c=>c.Source)));
 var service=new PointAliasConfigurationSubscriptions(basic,Flow,modules,target,plugin.PointInputAliases);
 var combo=new ConfigurationSubscriptionDestination(ConfigurationSubscriptionDomain.Combination,"CorrectCenterPoint","POINT");var x=new ConfigurationSubscriptionDestination(ConfigurationSubscriptionDomain.InputIo,"CorrectCenterPointX","float");var y=new ConfigurationSubscriptionDestination(ConfigurationSubscriptionDomain.InputIo,"CorrectCenterPointY","float");
 ConfigurationSubscriptionBinding Ref(string name,string type)=>new(new ConfigurationOutputReference(procedure,sourceId,name,type),ConfigurationSubscriptionIndex.All);
 Check(service.Bind(combo,Ref("Point","POINT")).Status==ConfigurationSubscriptionStatus.Available,"Combination binding accepted");
 Check(service.Read(x).Binding==Ref("Point.X","float")&&service.Read(y).Binding==Ref("Point.Y","float"),"Original combination projects into same two leaves");
 Check(Flow().Configuration.Subscriptions.Count==2&&Flow().Configuration.Subscriptions.All(s=>s.Target!="CorrectCenterPoint"),"No stale combined subscription remains");
 var context=new ToolEditorContext(Flow().Nodes.Single(n=>n.Id==target),Environment.CurrentDirectory,next=>document.SetProcedureNodeParameters(procedure,target,next),_=>throw new InvalidOperationException("No run requested")){ConfigurationSubscriptions=service};
 using var session=plugin.CreateConfiguration(context);session.State.CommitUser("CorrectCenterPointInput","1");
 Check(service.Bind(x,Ref("OverrideX","float")).Status==ConfigurationSubscriptionStatus.Available,"X override accepted");
 Check(service.Read(y).Binding==Ref("Point.Y","float"),"X override retains original Y");
 session.State.CommitUser("CorrectCenterPointInput","0");session.State.CommitUser("CorrectCenterPointInput","1");
 Check(service.Read(x).Binding==Ref("OverrideX","float")&&service.Read(y).Binding==Ref("Point.Y","float"),"Both mode switches preserve current leaf sources");
 Check(service.Read(combo).Binding?.Reference is ConfigurationPointTupleReference,"Mixed source combination is reconstructed tuple, not old POINT");
 IEnumerable<ConfigurationSubscriptionTreeNode> Walk(IEnumerable<ConfigurationSubscriptionTreeNode> nodes)=>nodes.SelectMany(n=>new[]{n}.Concat(Walk(n.Children)));
 var arrayTree=service.Enumerate(new(combo,new[]{"POINT"}));Check(arrayTree.Status==ConfigurationSubscriptionStatus.Available&&Walk(arrayTree.Roots).Any(n=>n.Binding?.Reference is ConfigurationOutputReference r&&r.PortId=="PointArray"),"Actual POINT tree includes array source without IndexOpenSetter");
 var numericTree=service.Enumerate(new(x,new[]{"float"}));Check(numericTree.Status==ConfigurationSubscriptionStatus.Available&&Walk(numericTree.Roots).Any(n=>n.Binding?.Reference is ConfigurationOutputReference r&&r.PortId=="OverrideXs"),"Actual float tree includes numeric array source");
 Check(service.Bind(combo,Ref("PointArray","POINT")).Status==ConfigurationSubscriptionStatus.Available,"POINT array combination binding accepted");Check(service.Read(x).Binding==Ref("PointArray.X","float")&&service.Read(y).Binding==Ref("PointArray.Y","float"),"POINT array is stored as same two numeric leaves");
 Check(service.Bind(x,Ref("OverrideXs","float")).Status==ConfigurationSubscriptionStatus.Available,"Numeric array X override accepted");session.State.CommitUser("CorrectCenterPointInput","0");session.State.CommitUser("CorrectCenterPointInput","1");Check(service.Read(y).Binding==Ref("PointArray.Y","float"),"Numeric array X override and mode changes preserve POINT array Y");
 var stored=JsonSerializer.Serialize(Flow());var restored=JsonSerializer.Deserialize<ProcedureDocument>(stored)!;
 Check(restored.Configuration.Subscriptions.Count==2&&restored.Configuration.Subscriptions.All(s=>s.Target!="CorrectCenterPoint"),"Persisted configuration keeps only authoritative leaves");
 Console.WriteLine($"Distortion point alias checks passed: {checks}; production shared alias service and real document/provider, no native execution.");
 }
 sealed class Source:IModule {
 public ModuleDescriptor Descriptor{get;}=new("center-source","1","Center",[],[new("Point",DataKind.Point),new("OverrideX",DataKind.Number),new("PointArray",DataKind.Point){Cardinality=ValueCardinality.Array},new("OverrideXs",DataKind.NumberArray)],[]);
 public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext c,CancellationToken t)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"Point",PortValue.Point(new Point2(200,180))},{"OverrideX",PortValue.Number(30)},{"PointArray",PortValue.PointArray([new Point2(100,200),new Point2(300,400)])},{"OverrideXs",PortValue.Numbers([50,70])}}));
 }
}


