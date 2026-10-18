using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
  void Check(bool condition,string label){if(!condition)throw new Exception(label);Console.WriteLine("PASS "+label);}
  var doc=(GraphEditorDocument)Field("doc")!;
  try
  {
   window.Show();Pump();doc.NewSolution();
   string first=doc.AddNode("vision.image-source","1.0.0",4550,150);
   string edge=doc.AddNode("vision.image-source","1.0.0",4800,150);Pump();
   var scroll=(ScrollViewer)window.FindName("CanvasScrollViewer")!;
   scroll.ScrollToHorizontalOffset(4470);Pump();
   Call("SetBoxSelection",new Rect(4530,125,390,80));Pump();
   var frame=(UIElement)Field("groupSelectionFrame")!;
   Check(frame is not null,"框选两节点后显示可拖动的组边框");
   frame.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left){RoutedEvent=UIElement.MouseLeftButtonDownEvent});Pump();
   Check(Field("movingNodes") is not null,"真实 WPF 组边框鼠标事件启动多节点拖动");
   var origin=(Point)Field("nodePointerStart")!;
   long revision=doc.Revision;
   Call("PreviewNodeMove",new Point(origin.X+200,origin.Y));Pump();
   var ghosts=(Dictionary<string,Canvas>)Field("movingGhosts")!;
   Check(ghosts.Count==2,"拖动预览同时显示两个临时节点");
   Check(Canvas.GetLeft(ghosts[first])+10==4750&&Canvas.GetLeft(ghosts[edge])+10==4890,"右边节点独立限幅，左边节点仍可移动 200 像素");
   var dir=Path.Combine("analysis","full-audit");Directory.CreateDirectory(dir);
   var bitmap=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);bitmap.Render(window);
   var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(bitmap));using(var output=File.Create(Path.Combine(dir,"canvas-multimove-right-edge.png")))png.Save(output);
   Call("CommitNodeMove");Pump();
   Check(doc.Revision==revision+1&&doc.CurrentProcedure.Nodes.Single(n=>n.Id==first).X==4750&&doc.CurrentProcedure.Nodes.Single(n=>n.Id==edge).X==4890,"提交为一次事务且沿用预览逐节点位置");
   doc.Undo();Pump();Check(doc.CurrentProcedure.Nodes.Single(n=>n.Id==first).X==4550&&doc.CurrentProcedure.Nodes.Single(n=>n.Id==edge).X==4800,"撤销恢复两节点原坐标");
   doc.Redo();Pump();Check(doc.CurrentProcedure.Nodes.Single(n=>n.Id==first).X==4750&&doc.CurrentProcedure.Nodes.Single(n=>n.Id==edge).X==4890,"重做恢复逐节点限幅结果");
   var file=Path.Combine(Path.GetTempPath(),"multimove-"+Guid.NewGuid().ToString("N")+".vmx.json");
   try{doc.Save(file);doc.Load(file);Pump();Check(doc.CurrentProcedure.Nodes.Single(n=>n.Id==first).X==4750&&doc.CurrentProcedure.Nodes.Single(n=>n.Id==edge).X==4890,"保存重开保留边缘拖动结果");}
   finally{if(File.Exists(file))File.Delete(file);}
  }
  finally{window.Close();app.Shutdown();}
 }
}
