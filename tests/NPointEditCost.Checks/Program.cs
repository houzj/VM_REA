using System.Diagnostics;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using VisionFramework;
using VisionEditor.Core;
using VisionPluginContracts;
using VisionPlugins.Calibration;

static class Program {
 [STAThread] static int Main(){
  try{
   _=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
   foreach(int count in new[]{4,9,32})Run(count);
   return 0;
  }catch(Exception e){Console.Error.WriteLine(e);return 1;}
 }
 static void Run(int count){
  var plugin=new NPointPlugin();var values=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());
  values["CalibPointTotalNum"]=JsonSerializer.SerializeToElement(Math.Min(count,16));values["RotPointTotalNum"]=JsonSerializer.SerializeToElement(Math.Max(0,count-16));
  var registry=new ModuleRegistry();new ToolPluginRegistry().Register(plugin,registry);
  using var document=new GraphEditorDocument(registry);string nodeId=document.AddNode(NPointPlugin.Id,"1.0.0",0,0,values);
  var node=document.CurrentProcedure.Nodes.Single(n=>n.Id==nodeId);
  int applyCount=0;long applyTicks=0;Dictionary<string,JsonElement> accepted=values;
  var context=new ToolEditorContext(node,Environment.CurrentDirectory,next=>{
   var watch=Stopwatch.StartNew();document.SetProcedureNodeParameters("main",nodeId,next);accepted=next;applyCount++;watch.Stop();applyTicks+=watch.ElapsedTicks;
  },_=>throw new NotSupportedException());
  using var session=plugin.CreateConfiguration(context);
  var host=new ToolConfigurationModelWindow(context,session){ShowInTaskbar=false,ShowActivated=false,Left=-30000,Top=-30000,WindowStartupLocation=WindowStartupLocation.Manual};
  host.Show();host.UpdateLayout();
  var button=Walk(host).OfType<Button>().Single(b=>b.Name=="NPointEditPoints");
  Exception? failure=null;int openApply=0;double openMs=0,cellMs=0;long openTicks=0;
  Dispatcher.CurrentDispatcher.BeginInvoke(new Action(()=>{
   try{
    var popup=Application.Current.Windows.Cast<Window>().Single(w=>w.Content is NPointConfigurationPointEditor);
    var editor=(NPointConfigurationPointEditor)popup.Content;
    if(editor.Rows.Count!=count)throw new InvalidOperationException($"expected {count} rows; got {editor.Rows.Count}");
    openApply=applyCount;openTicks=applyTicks;
    var clock=Stopwatch.StartNew();if(!editor.TrySetCell(count-1,4,"1.25"))throw new InvalidOperationException("cell rejected");clock.Stop();cellMs=clock.Elapsed.TotalMilliseconds;
    if(applyCount!=openApply+1||accepted["DeltaAngle"+(count-1)].GetDouble()!=1.25)throw new InvalidOperationException("single-cell apply granularity mismatch");
    popup.Close();
   }catch(Exception e){failure=e;foreach(var w in Application.Current.Windows.Cast<Window>().Where(w=>w.Content is NPointConfigurationPointEditor).ToArray())w.Close();}
  }),DispatcherPriority.ApplicationIdle);
  var opened=Stopwatch.StartNew();button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));opened.Stop();openMs=opened.Elapsed.TotalMilliseconds;
  host.Close();if(failure is not null)throw failure;
  Console.WriteLine(JsonSerializer.Serialize(new{count,openMs,openApply,openAverageApplyMicroseconds=openApply==0?0:openTicks*1_000_000d/Stopwatch.Frequency/openApply,cellMs,cellApply=applyCount-openApply}));
 }
 static IEnumerable<DependencyObject> Walk(DependencyObject root){yield return root;for(int i=0;i<VisualTreeHelper.GetChildrenCount(root);i++)foreach(var item in Walk(VisualTreeHelper.GetChild(root,i)))yield return item;}
}
