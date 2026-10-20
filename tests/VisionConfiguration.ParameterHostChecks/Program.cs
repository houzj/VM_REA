using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using VisionCore;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.CameraMapping;

static class Program {
 const BindingFlags Private=BindingFlags.NonPublic|BindingFlags.Instance;static int checks;
 static void Check(bool ok,string message){if(!ok)throw new Exception(message);checks++;Console.WriteLine("PASS "+checks+": "+message);}
 static T Field<T>(object owner,string name)=>(T)owner.GetType().GetField(name,Private)!.GetValue(owner)!;
 static object? Call(object owner,string name,params object[] args)=>owner.GetType().GetMethod(name,Private)!.Invoke(owner,args);
 static void Wait(Task task){var frame=new DispatcherFrame();_=task.ContinueWith(_=>Application.Current.Dispatcher.BeginInvoke(()=>frame.Continue=false));if(!task.IsCompleted)Dispatcher.PushFrame(frame);task.GetAwaiter().GetResult();}
 [STAThread] static int Main(){try{
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(app.Dispatcher));var shell=new MainWindow{Left=-20000,Top=-20000,ShowActivated=false,ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual};shell.Show();
  try{
   var doc=Field<GraphEditorDocument>(shell,"doc");var registry=Field<ModuleRegistry>(shell,"modules");registry.Register(new Source().Descriptor,()=>new Source());doc.NewSolution();var source=doc.AddNode("test.effective-source","1",0,0);var target=doc.AddNode(CameraMappingPlugin.Id,"1.0.0",200,0);doc.ConnectExecution(source,target);
   ToolEditorContext Context(){var node=doc.CurrentProcedure.Nodes.Single(n=>n.Id==target);return (ToolEditorContext)Call(shell,"ConnectToolEditorServices",new ToolEditorContext(node,doc.BaseDirectory,values=>{var before=doc.CurrentProcedure.Nodes.Single(n=>n.Id==target).Parameters;doc.SetParameters(target,values);Field<ModuleExecutionScope>(shell,"executionScope").ForProcedure(doc.CurrentProcedureId).RememberConfigurationParameters(node.ModuleId,target,before,values);},_=>throw new NotSupportedException()),target)!;}
   var context=Context();var service=context.ConfigurationSubscriptions!;Check(context.ReadEffectiveParameters?.Invoke() is null,"unexecuted module has no invented effective snapshot");
   ConfigurationSubscriptionBinding Binding(string port,string type)=>new(new ConfigurationOutputReference(doc.CurrentProcedureId,source,port,type),ConfigurationSubscriptionIndex.All);
   Check(service.Bind(new(ConfigurationSubscriptionDomain.Combination,"ObjectPointInput1","POINT"),Binding("Object","POINT")).Status==ConfigurationSubscriptionStatus.Available,"real object point array bound");
   Check(service.Bind(new(ConfigurationSubscriptionDomain.Combination,"TargetPointInput1","POINT"),Binding("Target","POINT")).Status==ConfigurationSubscriptionStatus.Available,"real target point array bound");
   var weight=new ConfigurationSubscriptionDestination(ConfigurationSubscriptionDomain.Parameter,"WeightFactor","int");Check(service.Bind(weight,Binding("Weight","int")).Status==ConfigurationSubscriptionStatus.Available,"parameter binding accepted");
   Wait((Task)Call(shell,"RunProcedure",doc.CurrentProcedureId)!);Check(Field<Dictionary<string,NodeRunRecord>>(shell,"results")[target].Status==NodeRunStatus.Success,"actual camera mapping graph executes");
   Check(doc.CurrentProcedure.Nodes.Single(n=>n.Id==target).Parameters["WeightFactor"].GetInt32()==20,"bound execution preserves document seed");Check(context.ReadEffectiveParameters!()!["WeightFactor"].GetInt32()==50,"host reads actual resolved runtime parameter");
   long before=doc.Revision;Check(service.Bind(weight,null).Status==ConfigurationSubscriptionStatus.Available,"parameter unbind accepted");Check(doc.Revision==before+1&&doc.CurrentProcedure.Nodes.Single(n=>n.Id==target).Parameters["WeightFactor"].GetInt32()==50,"unbind retains last effective value in one edit");
   doc.Undo();Check(doc.CurrentProcedure.Nodes.Single(n=>n.Id==target).Parameters["WeightFactor"].GetInt32()==20&&service.Read(weight).Binding is not null,"undo restores parameter binding and seed together");doc.Redo();Check(context.ReadEffectiveParameters!()!["WeightFactor"].GetInt32()==50,"redo retains effective value");
   var changed=doc.CurrentProcedure.Nodes.Single(n=>n.Id==target).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());changed["WeightFactor"]=JsonSerializer.SerializeToElement(30);context.Apply(changed);Check(context.ReadEffectiveParameters!()!["WeightFactor"].GetInt32()==30,"explicit parameter write supersedes last runtime value");
   Check(service.Bind(weight,Binding("Weight","int")).Status==ConfigurationSubscriptionStatus.Available,"rebind for configuration self-run");Wait(context.Run(doc.CurrentProcedure.Nodes.Single(n=>n.Id==target).Parameters));Check(context.ReadEffectiveParameters!()!["WeightFactor"].GetInt32()==50,"isolated self-run preserves original document seed when recording effective values");
   Source.Weight=5000;Wait((Task)Call(shell,"RunProcedure",doc.CurrentProcedureId)!);Check(context.ReadEffectiveParameters!()!["WeightFactor"].GetInt32()==50,"failed binding validation cannot overwrite effective state");Source.Weight=50;
   var defaults=registry.GetDescriptor(doc.CurrentProcedure.Nodes.Single(n=>n.Id==target)).Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());context.ResetConfigurationParameters!(defaults);Check(context.ReadEffectiveParameters!()!["WeightFactor"].GetInt32()==20,"reset overrides effective parameter state");Check(doc.CurrentProcedure.Configuration.Subscriptions.Count==4,"reset preserves POINT input leaves");
   var scope=Field<ModuleExecutionScope>(shell,"executionScope");scope.ForProcedure("other").RememberExecutionParameters(CameraMappingPlugin.Id,target,defaults,new Dictionary<string,JsonElement>(defaults){["WeightFactor"]=JsonSerializer.SerializeToElement(75)});Check(context.ReadEffectiveParameters!()!["WeightFactor"].GetInt32()==20,"same node ID in another procedure cannot change current parameters");
   Call(shell,"OpenToolConfiguration",target);Check(Field<ToolConfigurationWindowHost>(shell,"toolConfigurationWindows").Current is ToolConfigurationModelWindow,"actual camera mapping opens common production window");
   var window=(ToolConfigurationModelWindow)Field<ToolConfigurationWindowHost>(shell,"toolConfigurationWindows").Current!;var session=Field<ToolConfigurationSession>(window,"session");
   foreach(var tab in window.ConfigurationView.Tabs.Items){window.ConfigurationView.Tabs.SelectedItem=tab;app.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);Check(window.ConfigurationView.Diagnostics.Count==0,"camera source tab has no unresolved templates");}
   Check(session.State.Diagnostics.Count==0,"camera host source operations complete: "+JsonSerializer.Serialize(session.State.Diagnostics));
   Call(shell,"CloseToolConfiguration");
   Console.WriteLine($"PASS {checks} effective parameter host checks; managed module entry is not native setter acknowledgement.");
  }finally{Call(shell,"CloseToolConfiguration");shell.Close();app.Shutdown();}return 0;
 }catch(Exception error){Console.Error.WriteLine(error);return 1;}}
 sealed class Source:IModule {
  public static int Weight=50;
  public ModuleDescriptor Descriptor=>new("test.effective-source","1","参数与点测试夹具",[],[new PortDescriptor("Object",DataKind.Point){Cardinality=ValueCardinality.Array},new PortDescriptor("Target",DataKind.Point){Cardinality=ValueCardinality.Array},new("Weight",DataKind.Integer)],[]);
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"Object",PortValue.PointArray(new[]{new Point2(0,0),new Point2(10,0),new Point2(0,10),new Point2(10,10)})},{"Target",PortValue.PointArray(new[]{new Point2(5,-7),new Point2(25,-7),new Point2(5,23),new Point2(25,23)})},{"Weight",PortValue.Integer(Weight)}}));
 }
}
