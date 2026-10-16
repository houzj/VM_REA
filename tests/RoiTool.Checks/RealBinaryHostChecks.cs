using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPlugins.ImageBinary;
static class RealBinaryHostChecks {
 static IEnumerable<DependencyObject> Children(DependencyObject root){yield return root;for(int i=0;i<VisualTreeHelper.GetChildrenCount(root);i++)foreach(var c in Children(VisualTreeHelper.GetChild(root,i)))yield return c;}
 static object Invoke(object o,string name,params object[] args)=>o.GetType().GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(o,args)!;
 static void Pump(Window window){window.UpdateLayout();var frame=new DispatcherFrame();var timer=new DispatcherTimer(DispatcherPriority.Send,window.Dispatcher){Interval=TimeSpan.FromMilliseconds(5)};timer.Tick+=(_,_)=>{timer.Stop();frame.Continue=false;};timer.Start();Dispatcher.PushFrame(frame);timer.Stop();}
 static void Wait(Task task,Window host){var watch=System.Diagnostics.Stopwatch.StartNew();while(!task.IsCompleted&&watch.Elapsed.TotalSeconds<30)Pump(host);if(!task.IsCompleted)throw new TimeoutException("Real host did not finish.");task.GetAwaiter().GetResult();}
 public static void Run(Action<bool,string> check){var host=new MainWindow{ShowInTaskbar=false,ShowActivated=false,Left=-20000,Top=-20000,WindowStartupLocation=WindowStartupLocation.Manual};host.Show();Pump(host);var doc=(GraphEditorDocument)typeof(MainWindow).GetField("doc",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(host)!;doc.NewSolution();var source=doc.AddNode("vision.image-file","1.0.0",100,100,new(){["Path"]=JsonSerializer.SerializeToElement(Path.GetFullPath("analysis/images/circle-find-sample.bmp"))});var binary=doc.AddNode(ImageBinaryPlugin.Id,"1.0.0",100,220);doc.Connect(source,"Image",binary,"Image");Wait((Task)Invoke(host,"RunProcedure","main"),host);var window=(ImageBinaryWindow)Invoke(host,"CreateToolEditorWindow",binary);window.Left=-20000;window.Top=-20000;window.WindowStartupLocation=WindowStartupLocation.Manual;window.Show();Pump(window);
 window.Roi.SetDrawKind(RoiKind.Rectangle);window.Roi.BeginPointer(new(400,300));window.Roi.UpdatePointer(new(800,700));window.Roi.EndPointer(new(800,700));check(doc.CurrentProcedure.Nodes.Single(n=>n.Id==binary).Rois?.Single().Width==400,"real shell ROI event writes node configuration immediately");
 var inputButton=Children(window).OfType<Button>().Single(b=>Equals(b.Tag,"ImageBinaryInput:Image"));window.Dispatcher.BeginInvoke(new Action(()=>{var picker=Application.Current.Windows.OfType<WorkflowSourcePicker>().Single();picker.SetInitialSelection(o=>o.NodeId==source);Children(picker).OfType<Button>().Single(b=>Equals(b.Content,"确定")).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));}));inputButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));check(doc.CurrentProcedure.Configuration.Subscriptions.Any(s=>s.NodeId==binary&&s.Target=="Image"&&s.Source.Id==source),"real debug-window subscription picker commits actual image binding");Wait(window.RunAsync(),window);check(window.LastResult?.Status==NodeRunStatus.Success,"real shell debug session executes subscribed source and binary tool");check(window.LastResult!.Outputs["ProcessedPixels"].As<long>()==160000,"real image ROI processed-pixel output matches region");
 var root=(FrameworkElement)window.Content;var bitmap=new RenderTargetBitmap((int)root.ActualWidth,(int)root.ActualHeight,96,96,PixelFormats.Pbgra32);bitmap.Render(root);var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(bitmap));using(var file=File.Create("analysis/full-audit/roi-binary-real-host-window.png"))png.Save(file);
 window.Close();for(int i=0;i<100&&window.IsVisible;i++)Pump(host);check(!window.IsVisible,"real tool close disposes host debug session and window");host.Close();for(int i=0;i<100&&host.IsVisible;i++)Pump(host);check(!host.IsVisible,"real shell closes without retained tool session");}
}
