using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using VisionFramework;
using VisionEditor;
using VisionEditor.Core;
using VisionPluginContracts;
using VisionPlugins.CameraMapping;

internal static class HostChecks {
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 static T Field<T>(object target,string name)=>(T)target.GetType().GetField(name,Private)!.GetValue(target)!;
 static object? Call(object target,string name,params object[] args)=>target.GetType().GetMethod(name,Private)!.Invoke(target,args);
 static IEnumerable<DependencyObject> Walk(DependencyObject root){yield return root;for(int i=0;i<VisualTreeHelper.GetChildrenCount(root);i++)foreach(var node in Walk(VisualTreeHelper.GetChild(root,i)))yield return node;}
 static void Pump()=>Dispatcher.CurrentDispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);
 static T Wait<T>(Task<T> task){var frame=new DispatcherFrame();_=task.ContinueWith(_=>Application.Current.Dispatcher.BeginInvoke(()=>frame.Continue=false));if(!task.IsCompleted)Dispatcher.PushFrame(frame);return task.GetAwaiter().GetResult();}
 internal static void Run(Action<bool,string> check,string output){
  var shell=new MainWindow{Left=-20000,Top=-20000,WindowStartupLocation=WindowStartupLocation.Manual,ShowActivated=false,ShowInTaskbar=false};shell.Show();
  try{
   var document=Field<GraphEditorDocument>(shell,"doc");var registry=Field<ModuleRegistry>(shell,"modules");document.NewSolution();var sourceModule=new Source();registry.Register(sourceModule.Descriptor,()=>sourceModule);string source=document.AddNode(sourceModule.Descriptor.Id,"1",0,0),target=document.AddNode(CameraMappingPlugin.Id,"1.0.0",200,0),procedure=document.CurrentProcedureId;document.ConnectExecution(source,target);
   var node=document.CurrentProcedure.Nodes.Single(n=>n.Id==target);var context=(ToolEditorContext)Call(shell,"ConnectToolEditorServices",new ToolEditorContext(node,output,next=>document.SetParameters(target,next),_=>throw new InvalidOperationException()),target)!;
   var dynamicIndex=ConfigurationSubscriptionIndex.From(new ConfigurationOutputReference(procedure,source,"Index","int"));
   foreach(string side in new[]{"Object","Target"})for(int group=1;group<=2;group++)check(context.ConfigurationSubscriptions!.Bind(new(ConfigurationSubscriptionDomain.Combination,CameraMappingPlugin.PointInputName(side,group),"POINT"),new(new ConfigurationOutputReference(procedure,source,side+group,"POINT"),group==1?ConfigurationSubscriptionIndex.At(0):dynamicIndex)).Status==ConfigurationSubscriptionStatus.Available,"MainWindow real bridge indexed POINT "+side+group);
   var cache=Field<Dictionary<string,Dictionary<string,NodeRunRecord>>>(shell,"configurationResults");
   void Seed(int? index){var values=new Dictionary<string,PortValue>{{"Object1",PortValue.PointArray([new(0,0),new(8,1)])},{"Object2",PortValue.PointArray([new(10,3),new(15,12)])},{"Target1",PortValue.PointArray([new(7,-2),new(23,0)])},{"Target2",PortValue.PointArray([new(27,4),new(48,29)])}};if(index is {} value)values["Index"]=PortValue.Integer(value);cache[procedure]=new(){{source,new(source,NodeRunStatus.Success,values,0)}};}
   Seed(1);var first=Wait(context.Run(node.Parameters));check(first.Status==NodeRunStatus.Success,"MainWindow context.Run preserves dynamic index");
   Seed(null);var fallback=Wait(context.Run(node.Parameters));check(fallback.Status==NodeRunStatus.Success&&first.Outputs["ScaleX"].As<double>()!=fallback.Outputs["ScaleX"].As<double>(),"MainWindow missing dynamic index falls back zero at read time");
   Seed(1);var recover=Wait(context.Run(node.Parameters));check(recover.Status==NodeRunStatus.Success&&recover.Outputs["ScaleX"].As<double>()==first.Outputs["ScaleX"].As<double>(),"MainWindow next self-run restores per-execution selector input");
   // Real common configuration Execute button invokes the host-connected context.Run.
   Call(shell,"OpenToolConfiguration",target);var host=Field<ToolConfigurationWindowHost>(shell,"toolConfigurationWindows");var window=(ToolConfigurationModelWindow)host.Current!;Pump();var before=cache[procedure].GetValueOrDefault(target);
   var controls=Field<IEnumerable<(ConfigurationNodeState Node,FrameworkElement Control,ToolConfigurationControlContext? Context)>>(window.ConfigurationView,"bindings");
   var execute=controls.Single(b=>b.Node.Name=="Execute").Control;var button=Walk(execute).OfType<Button>().Single();
   button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
   var deadline=DateTime.UtcNow.AddSeconds(15);while(ReferenceEquals(cache[procedure].GetValueOrDefault(target),before)&&DateTime.UtcNow<deadline){var frame=new DispatcherFrame();var timer=new DispatcherTimer(TimeSpan.FromMilliseconds(20),DispatcherPriority.Background,(_,_)=>frame.Continue=false,Dispatcher.CurrentDispatcher);timer.Start();Dispatcher.PushFrame(frame);timer.Stop();}
   check(!ReferenceEquals(cache[procedure].GetValueOrDefault(target),before)&&cache[procedure][target].Status==NodeRunStatus.Success,"actual MainWindow configuration Execute button completes indexed runtime");Call(shell,"CloseToolConfiguration");
   var cached=cache[procedure].Values.ToArray();File.WriteAllText(Path.Combine(output,"debug-cache.json"),System.Text.Json.JsonSerializer.Serialize(cached.Select(r=>new{r.NodeId,r.Status,outputs=r.Outputs.ToDictionary(p=>p.Key,p=>new{p.Value.Kind,p.Value.Cardinality,p.Value.Value})})));var debug=Wait(new NodeDebugExecutor(registry).RunAsync(document.CurrentProcedure,target,output,node.Parameters,cached));File.WriteAllText(Path.Combine(output,"debug-record.json"),System.Text.Json.JsonSerializer.Serialize(new{debug.Status,debug.ErrorCode,debug.NativeErrorCode,debug.Message,outputs=debug.Outputs.ToDictionary(p=>p.Key,p=>p.Value.Value),expected=first.Outputs["ScaleX"].As<double>()}));check(debug.Status==NodeRunStatus.Success&&debug.Outputs["ScaleX"].As<double>()==first.Outputs["ScaleX"].As<double>(),"NodeDebugExecutor isolated graph preserves deferred numeric selector: "+debug.Status+" / "+debug.ErrorCode+" / "+debug.NativeErrorCode+" / "+debug.Message);
   ConfigurationSubscriptionBinding Missing()=>new(new ConfigurationOutputReference(procedure,source,"OptionalX","float"),ConfigurationSubscriptionIndex.All);
   check(context.ConfigurationSubscriptions!.Bind(new(ConfigurationSubscriptionDomain.InputIo,"ObjectPointX3","float"),Missing()).Status==ConfigurationSubscriptionStatus.Available,"MainWindow inactive All missing optional binding");check(Wait(context.Run(node.Parameters)).Status==NodeRunStatus.Success,"MainWindow self-run ignores inactive group's unavailable All leaf");
   check(context.ConfigurationSubscriptions.Bind(new(ConfigurationSubscriptionDomain.InputIo,"TargetPointX1","float"),Missing()).Status==ConfigurationSubscriptionStatus.Available,"MainWindow active All missing optional binding");var missing=Wait(context.Run(node.Parameters));check(missing.NativeErrorCode==0xe0000001&&missing.ErrorCode!="SelfRunFailed","MainWindow missing All reaches module count0 mismatch rather than preemptive self-run failure");
   cache[procedure][source]=cache[procedure][source] with{Status=NodeRunStatus.Failed};var unavailable=Wait(context.Run(node.Parameters));check(unavailable.Status==NodeRunStatus.Failed&&unavailable.ErrorCode=="SelfRunFailed","MainWindow preserves failed cached source boundary");
  }finally{Call(shell,"CloseToolConfiguration");shell.Close();}
 }
 sealed class Source:IModule {
  public ModuleDescriptor Descriptor{get;}=new("camera-index-host-source","1","Host index source",[],new[]{"Object1","Object2","Target1","Target2"}.Select(n=>new PortDescriptor(n,DataKind.Point){Cardinality=ValueCardinality.Array}).Append(new("Index",DataKind.Integer,false)).Append(new("OptionalX",DataKind.NumberArray,false)).ToArray(),[]);
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>throw new InvalidOperationException("Host self-run must read only explicitly seeded cached records");
 }
}
