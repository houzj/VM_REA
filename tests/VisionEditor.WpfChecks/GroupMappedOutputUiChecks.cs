using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
static class GroupMappedOutputUiChecks
{
 static IEnumerable<DependencyObject> Children(DependencyObject root){for(int i=0;i<VisualTreeHelper.GetChildrenCount(root);i++){var child=VisualTreeHelper.GetChild(root,i);yield return child;foreach(var nested in Children(child))yield return nested;}}
 public static void Run(MainWindow host,Action<bool,string> check){
  T Field<T>(string name)=>(T)typeof(MainWindow).GetField(name,BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(host)!;
  void Pump(){host.UpdateLayout();host.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
  var doc=Field<GraphEditorDocument>("doc");doc.NewSolution();var root=doc.CurrentProcedureId;var group=doc.AddOwnedGroup(100,100);var body=doc.EnterOwnedGroup(group);var source=doc.AddNode("vision.image-source","1.0.0",100,100,new(){["ImagePaths"]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(new[]{System.IO.Path.GetFullPath("analysis/images/circle-find-sample.bmp")}))});var components=doc.AddNode("vision.components","1.0.0",100,200);doc.Connect(source,"Image",components,"Image");doc.SetWorkflowConfiguration(body,new(){Outputs=[new("blobs","blobs",DataKind.Components)]});Pump();
  typeof(MainWindow).GetMethod("OpenDisplayedConfiguration",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(host,null);Pump();var config=Application.Current.Windows.OfType<SubprogramCallWindow>().Single();
  try{var tabs=(TabControl)config.FindName("ConfigurationTabs");tabs.SelectedIndex=1;Pump();var selector=Children(config).OfType<NativeCompoundSourceSelector>().Single(c=>Equals(c.Tag,"GroupOutputSource:blobs"));selector.LinkButton.IsChecked=true;Pump();
   IEnumerable<TreeViewItem> Items(ItemsControl p){foreach(var item in p.Items.OfType<TreeViewItem>()){yield return item;foreach(var nested in Items(item))yield return nested;}}
   var leaf=Items(selector.SubscriptionTree).Single(i=>i.Tag is WorkflowSourceOption o&&o.NodeId==components&&o.Port=="Components");leaf.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left){RoutedEvent=Control.MouseDoubleClickEvent});Pump();check(doc.Document.Procedures.Single(p=>p.Id==body).Configuration.Outputs.Single().NodeId==components,"Actual Group compound link popup writes selected real algorithm source into executable interface");
   var run=(Button)config.FindName("RunOnce");run.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));var watch=System.Diagnostics.Stopwatch.StartNew();while(!run.IsEnabled&&watch.ElapsedMilliseconds<15000){Pump();Thread.Sleep(1);}Pump();var result=Field<Dictionary<string,ExecutionSessionResult>>("workflowSessions")[root].Calls[group];check(run.IsEnabled&&result.Status==SessionStatus.Success&&result.WorkflowOutputs["blobs"].As<IReadOnlyList<VisionCore.Component>>().Count>0,"Actual Group run after popup subscription returns real BMP components rather than mock data");
  }finally{config.Close();Pump();doc.Load(System.IO.Path.Combine(AppContext.BaseDirectory,"demo.vmx.json"));Pump();}
 }
}
