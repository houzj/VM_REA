using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionEditor;
using VisionFramework;

static class NativeSubscriptionPopupChecks
{
 public static void Run()
 {
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
  var chosen=new List<string>();
  var entries=new[]{new WorkflowSourceOption("","",DataKind.Components,"解除订阅"),new WorkflowSourceOption("node-1","Components.Area",DataKind.Components,"连通域.Components.Area"){NodeName="连通域"},new WorkflowSourceOption("node-1","CentroidPoint.CentroidX",DataKind.Number,"连通域.CentroidPoint.CentroidX"){NodeName="连通域"}};
  var selector=new NativeCompoundSourceSelector("------",()=>entries,o=>chosen.Add(o.Label));
  var owner=new Window{Width=340,Height=100,WindowStartupLocation=WindowStartupLocation.Manual,Left=-10000,Top=-10000,Content=selector,ShowInTaskbar=false};owner.Show();Pump(owner);
  selector.LinkButton.IsChecked=true;Pump(owner);
  Check(selector.SubscriptionPopup.IsOpen,"Relation link opens anchored source popup");
  Check(selector.SubscriptionTree.Width==180&&selector.SubscriptionTree.Height==180,"Popup tree matches recovered 180x180 dimensions");
  var module=selector.SubscriptionTree.Items.OfType<TreeViewItem>().Single(i=>Equals(i.Header,"连通域"));
  Check(module.Items.OfType<TreeViewItem>().Any(i=>Equals(i.Header,"Components")&&i.Items.OfType<TreeViewItem>().Any(c=>Equals(c.Header,"Area"))),"Source tree groups Components.Area under the native module");
  Check(module.Items.OfType<TreeViewItem>().Any(i=>Equals(i.Header,"CentroidPoint")&&i.Items.OfType<TreeViewItem>().Any(c=>Equals(c.Header,"CentroidX"))),"Source tree preserves nested CentroidPoint.CentroidX path");
  Capture(selector.SubscriptionPopup.Child!,"analysis/full-audit/group-subscription-popup.png");
  var area=module.Items.OfType<TreeViewItem>().Single(i=>Equals(i.Header,"Components")).Items.OfType<TreeViewItem>().Single(i=>Equals(i.Header,"Area"));
  area.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left){RoutedEvent=Control.MouseDoubleClickEvent});Pump(owner);
  Check(chosen.Single()=="连通域.Components.Area"&&!selector.SubscriptionPopup.IsOpen,"Leaf double-click commits complete source path and closes popup");
  selector.LinkButton.IsChecked=true;Pump(owner);
  var clear=selector.SubscriptionTree.Items.OfType<TreeViewItem>().Single(i=>Equals(i.Header,"解除订阅"));
  clear.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left){RoutedEvent=Control.MouseDoubleClickEvent});Pump(owner);
  Check(chosen.Last()=="解除订阅","Unsubscribe leaf invokes the selection callback");
  owner.Close();app.Shutdown();Console.WriteLine("Native subscription popup checks passed.");
 }
 static void Pump(Window owner){owner.UpdateLayout();owner.Dispatcher.Invoke(()=>{},System.Windows.Threading.DispatcherPriority.ApplicationIdle);}
 static void Check(bool condition,string message){if(!condition)throw new InvalidOperationException(message);Console.WriteLine("PASS "+message);}
 static void Capture(UIElement element,string path){var size=element.RenderSize;if(size.Width<=0||size.Height<=0)throw new InvalidOperationException("Popup visual has no layout size.");var bitmap=new RenderTargetBitmap((int)Math.Ceiling(size.Width),(int)Math.Ceiling(size.Height),96,96,PixelFormats.Pbgra32);bitmap.Render(element);var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(bitmap));Directory.CreateDirectory(Path.GetDirectoryName(path)!);using var stream=File.Create(path);encoder.Save(stream);}
}
