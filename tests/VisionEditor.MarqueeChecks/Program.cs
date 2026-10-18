using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Rectangle=System.Windows.Shapes.Rectangle;
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
  var selected=(HashSet<string>)Field("selectedNodes")!;
  try
  {
   window.Show();Pump();doc.NewSolution();
   var first=doc.AddNode("vision.image-source","1.0.0",150,150);
   var second=doc.AddNode("vision.image-source","1.0.0",330,150);
   var outside=doc.AddNode("vision.image-source","1.0.0",550,150);Pump();
   long revision=doc.Revision;
   var canvas=(Canvas)window.FindName("GraphLayer")!;
   canvas.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left){RoutedEvent=Mouse.PreviewMouseDownEvent});Pump();
   var marquee=(Rectangle?)Field("selectionBox");
   Check(marquee is not null&&(bool)Field("boxSelecting")!,"真实 WPF 鼠标按下进入框选预览");
   marquee=marquee!;
   Check(marquee.Stroke is SolidColorBrush brush&&brush.Color==Colors.LightSlateGray,"预览边框采用原始 LightSlateGray");
   Check(marquee.StrokeDashArray is {Count:2} dash&&dash[0]==2&&dash[1]==1&&marquee.Fill==Brushes.Transparent,"预览为 2:1 虚线且无可见填充");
   Canvas.SetLeft(marquee,125);Canvas.SetTop(marquee,125);marquee.Width=345;marquee.Height=60;Pump();
   var dir=Path.Combine("analysis","full-audit");Directory.CreateDirectory(dir);
   var bitmap=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);bitmap.Render(window);
   var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(bitmap));using(var stream=File.Create(Path.Combine(dir,"canvas-marquee-native-dash.png")))png.Save(stream);
   Call("CancelCanvasGesture");Pump();
   Check(!(bool)Field("boxSelecting")!&&Field("selectionBox") is null,"取消框选撤掉预览且不改流程");
   Call("SetBoxSelection",new Rect(125,125,345,60));Pump();
   Check(selected.SetEquals([first,second])&&!selected.Contains(outside),"框选只包含完全落入矩形的两个节点");
   Check(doc.Revision==revision,"框选操作不创建文档撤销事务");
   Call("AlignSelection","left");Pump();
   Check(doc.Revision==revision+1&&doc.CurrentProcedure.Nodes.Where(n=>selected.Contains(n.Id)).All(n=>n.X==150),"框选后左对齐写入一次文档事务");
   doc.Undo();Pump();Check(doc.CurrentProcedure.Nodes.Single(n=>n.Id==second).X==330,"撤销恢复第二节点位置");
   var file=Path.Combine(Path.GetTempPath(),"marquee-"+Guid.NewGuid().ToString("N")+".vmx.json");
   try{doc.Save(file);doc.Load(file);Pump();Check(doc.CurrentProcedure.Nodes.Count==3&&doc.CurrentProcedure.Nodes.Single(n=>n.Id==second).X==330,"保存重开保持撤销后的三节点数据");}
   finally{if(File.Exists(file))File.Delete(file);}
  }
  finally{window.Close();app.Shutdown();}
 }
}
