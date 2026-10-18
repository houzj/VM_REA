using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;

internal static class Program
{
 [STAThread] static int Main(){try{Run();return 0;}catch(Exception ex){Console.Error.WriteLine(ex);return 1;}}
 static void Run()
 {
  var app=new Application {ShutdownMode=ShutdownMode.OnExplicitShutdown};
  var window=new MainWindow {ShowActivated=false,ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};
  SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(window.Dispatcher));
  object? Field(string name)=>typeof(MainWindow).GetField(name,BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window);
  object? Call(string name,params object[] args)=>typeof(MainWindow).GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(window,args);
  void Pump(){window.UpdateLayout();window.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
  void Check(bool value,string description){if(!value)throw new Exception(description);Console.WriteLine("PASS "+description);}
  var doc=(GraphEditorDocument)Field("doc")!;
  var selected=(HashSet<string>)Field("selectedNodes")!;
  try
  {
   window.Show();Pump();doc.NewSolution();
   var original=doc.AddNode("vision.image-source","1.0.0",100,100);selected.Clear();selected.Add(original);Call("CopySelection");Pump();
   long revision=doc.Revision;
   void PasteAt(Point point)
   {
    var menu=(ContextMenu)Call("CanvasSelectionMenu",point)!;
    var paste=(MenuItem)menu.Items[0];
    Check(paste.IsEnabled,"真实 WPF 画布右键菜单粘贴可用");
    paste.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));Pump();
   }
   PasteAt(new Point(0,0));
   Check(doc.Revision==revision+1&&doc.CurrentProcedure.Nodes.Count==2,"原点菜单粘贴产生单次文档事务");
   var pasted=doc.CurrentProcedure.Nodes.Single(n=>n.Id!=original);
   Check(pasted.X==200&&pasted.Y==200,"原点与键盘粘贴共用原生 +100 哨兵规则");
   doc.Undo();Pump();Check(doc.CurrentProcedure.Nodes.Count==1,"撤销恢复原节点");
   PasteAt(new Point(263,242));
   pasted=doc.CurrentProcedure.Nodes.Single(n=>n.Id!=original);
   Check(pasted.X==265&&pasted.Y==240,"普通菜单点按 5 像素格定位");
   doc.Undo();Pump();
   PasteAt(new Point(-20,-10));
   pasted=doc.CurrentProcedure.Nodes.Single(n=>n.Id!=original);
   Check(pasted.X==5&&pasted.Y==5,"负坐标菜单点最小落在 5 像素格");
   var dir=Path.Combine("analysis","full-audit");Directory.CreateDirectory(dir);
   var bitmap=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);bitmap.Render(window);
   var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(bitmap));using(var output=File.Create(Path.Combine(dir,"canvas-paste-origin-grid.png")))encoder.Save(output);
   var file=Path.Combine(Path.GetTempPath(),"canvas-paste-"+Guid.NewGuid().ToString("N")+".vmx.json");
   try{doc.Save(file);doc.Load(file);Pump();Check(doc.CurrentProcedure.Nodes.Any(n=>n.X==5&&n.Y==5),"保存重开保留最终粘贴位置");}
   finally{if(File.Exists(file))File.Delete(file);}
   var second=doc.AddNode("vision.image-source","1.0.0",300,150);
   selected.Clear();selected.Add(original);selected.Add(second);Call("CopySelection");Pump();
   revision=doc.Revision;PasteAt(new Point(4901,4923));
   var firstBoundary=doc.CurrentProcedure.Nodes.Single(n=>n.Id!=original&&n.Id!=second&&n.X==4900);
   var laterBoundary=doc.CurrentProcedure.Nodes.Single(n=>n.Id!=original&&n.Id!=second&&n.X==4850);
   Check(firstBoundary.Y==4925&&laterBoundary.Y==4850,"边界粘贴首节点不限制4850，后续节点独立限制");
   Check(doc.Revision==revision+1,"两节点边界粘贴仍是单次事务");
   var scroll=(ScrollViewer)window.FindName("CanvasScrollViewer")!;scroll.ScrollToHorizontalOffset(scroll.ScrollableWidth);scroll.ScrollToVerticalOffset(scroll.ScrollableHeight);Pump();
   bitmap=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);bitmap.Render(window);
   encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(bitmap));using(var output=File.Create(Path.Combine(dir,"canvas-paste-upper-boundary.png")))encoder.Save(output);
   doc.Undo();Pump();Check(doc.CurrentProcedure.Nodes.All(n=>n.X!=4900&&n.X!=4850),"撤销一次清除边界粘贴两节点");
   doc.Redo();Pump();Check(doc.CurrentProcedure.Nodes.Any(n=>n.X==4900&&n.Y==4925)&&doc.CurrentProcedure.Nodes.Any(n=>n.X==4850&&n.Y==4850),"重做恢复首项与后续项各自坐标");
   file=Path.Combine(Path.GetTempPath(),"canvas-paste-edge-"+Guid.NewGuid().ToString("N")+".vmx.json");
   try{doc.Save(file);doc.Load(file);Pump();Check(doc.CurrentProcedure.Nodes.Any(n=>n.X==4900&&n.Y==4925)&&doc.CurrentProcedure.Nodes.Any(n=>n.X==4850&&n.Y==4850),"保存重开保留两种边界行为");}
   finally{if(File.Exists(file))File.Delete(file);}
  }
  finally{window.Close();app.Shutdown();}
 }
}
