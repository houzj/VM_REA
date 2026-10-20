using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
using Application=System.Windows.Application;
using Button=System.Windows.Controls.Button;
using ButtonBase=System.Windows.Controls.Primitives.ButtonBase;

internal static class HostChecks
{
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 static T Field<T>(object target,string name)=>(T)target.GetType().GetField(name,Private)!.GetValue(target)!;
 static object? Call(object target,string name,params object[] args)=>target.GetType().GetMethod(name,Private)!.Invoke(target,args);
 static IEnumerable<DependencyObject> Walk(DependencyObject value){yield return value;for(int i=0;i<VisualTreeHelper.GetChildrenCount(value);i++)foreach(var child in Walk(VisualTreeHelper.GetChild(value,i)))yield return child;}
 static T Wait<T>(Task<T> task){var frame=new DispatcherFrame();_=task.ContinueWith(_=>Application.Current.Dispatcher.BeginInvoke(()=>frame.Continue=false));if(!task.IsCompleted)Dispatcher.PushFrame(frame);return task.GetAwaiter().GetResult();}
 internal static void Run(Action<bool,string> check,string output)
 {
  var shell=new MainWindow{Left=-20000,Top=-20000,ShowActivated=false,ShowInTaskbar=false};shell.Show();
  try
  {
   var document=Field<GraphEditorDocument>(shell,"doc");var registry=Field<ModuleRegistry>(shell,"modules");document.NewSolution();var producer=new Source();registry.Register(producer.Descriptor,()=>producer);
   string source=document.AddNode(producer.Descriptor.Id,"1",0,0),target=document.AddNode(ReadCalibFilePlugin.Id,"1.0.0",200,0),procedure=document.CurrentProcedureId;document.ConnectExecution(source,target);
   var parameters=document.CurrentProcedure.Nodes.Single(n=>n.Id==target).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());parameters["CachedFileData"]=JsonSerializer.SerializeToElement("<R><CalibInputParam/><CalibOutputParam><P ParamName='SnapImagePointX' DataType='float'><V>12</V></P></CalibOutputParam></R>");document.SetParameters(target,parameters);
   var node=document.CurrentProcedure.Nodes.Single(n=>n.Id==target);var context=(ToolEditorContext)Call(shell,"ConnectToolEditorServices",new ToolEditorContext(node,output,next=>document.SetParameters(target,next),_=>throw new InvalidOperationException()),target)!;
   var destination=new ConfigurationSubscriptionDestination(ConfigurationSubscriptionDomain.InputIo,"SnapImagePointX1","float");
   var binding=new ConfigurationSubscriptionBinding(new ConfigurationOutputReference(procedure,source,"Values","float"),ConfigurationSubscriptionIndex.From(new ConfigurationOutputReference(procedure,source,"Index","int")));
   check(context.ConfigurationSubscriptions!.Bind(destination,binding).Status==ConfigurationSubscriptionStatus.Available,"actual MainWindow bridge accepts float array pose source with dynamic selector");
   var cache=Field<Dictionary<string,Dictionary<string,NodeRunRecord>>>(shell,"configurationResults");
   void Seed(int? index,bool missing=false){var values=new Dictionary<string,PortValue>();if(!missing)values["Values"]=PortValue.Numbers([54,98]);if(index is {} i)values["Index"]=PortValue.Integer(i);cache[procedure]=new(){{source,new(source,NodeRunStatus.Success,values,0)}};}
   Seed(1);var selected=Wait(context.Run(parameters));check(selected.Status==NodeRunStatus.Success&&selected.Outputs["SnapImagePointX"].As<double>()==98,"actual MainWindow self-run preserves pose array selector");
   Seed(null);check(Wait(context.Run(parameters)).Outputs["SnapImagePointX"].As<double>()==54,"actual MainWindow dynamic index read failure uses zero");
   Seed(1,true);check(Wait(context.Run(parameters)).Outputs["SnapImagePointX"].As<double>()==12,"actual MainWindow absent optional pose source preserves file value");
   Seed(1);var debug=Wait(new NodeDebugExecutor(registry).RunAsync(document.CurrentProcedure,target,output,parameters,cache[procedure].Values.ToArray()));check(debug.Status==NodeRunStatus.Success&&debug.Outputs["SnapImagePointX"].As<double>()==98,"actual cached debug preserves indexed pose read");
   Call(shell,"OpenToolConfiguration",target);var host=Field<ToolConfigurationWindowHost>(shell,"toolConfigurationWindows");var window=(ToolConfigurationModelWindow)host.Current!;Dispatcher.CurrentDispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);
   var controls=Field<IEnumerable<(ConfigurationNodeState Node,FrameworkElement Control,ToolConfigurationControlContext? Context)>>(window.ConfigurationView,"bindings");var execute=Walk(controls.Single(b=>b.Node.Name=="Execute").Control).OfType<Button>().Single();var before=cache[procedure].GetValueOrDefault(target);execute.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
   var deadline=DateTime.UtcNow.AddSeconds(15);while(ReferenceEquals(before,cache[procedure].GetValueOrDefault(target))&&DateTime.UtcNow<deadline){var frame=new DispatcherFrame();var timer=new DispatcherTimer(TimeSpan.FromMilliseconds(20),DispatcherPriority.Background,(_,_)=>frame.Continue=false,Dispatcher.CurrentDispatcher);timer.Start();Dispatcher.PushFrame(frame);timer.Stop();}
   check(!ReferenceEquals(before,cache[procedure].GetValueOrDefault(target))&&cache[procedure][target].Outputs["SnapImagePointX"].As<double>()==98,"real configuration Execute button reaches pose array runtime");
   check(new ReadCalibFilePlugin().LastDisplay(node,output,context.Scope) is not null,"actual self-run publishes display snapshot in execution scope");
   Call(shell,"CloseToolConfiguration");
  }
  finally{Call(shell,"CloseToolConfiguration");shell.Close();}
 }
 sealed class Source:IModule
 {
  public ModuleDescriptor Descriptor{get;}=new("readcalib.host-source","1","source",[],[new("Values",DataKind.NumberArray,false),new("Index",DataKind.Integer,false)],[]);
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>throw new InvalidOperationException("Self-run/debug must only use the explicit cache.");
 }
}
