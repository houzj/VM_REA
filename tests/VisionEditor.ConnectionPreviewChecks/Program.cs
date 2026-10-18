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
using VisionFramework;

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
  void Check(bool ok,string label){if(!ok)throw new Exception(label);Console.WriteLine("PASS "+label);}
  var doc=(GraphEditorDocument)Field("doc")!;
  try
  {
   window.Show();Pump();doc.NewSolution();
   string source=doc.AddNode("vision.image-source","1.0.0",200,180);
   string target=doc.AddNode("vision.image-source","1.0.0",470,310);Pump();
   var ports=(Dictionary<(string Node,int Port),Border>)Field("executionPorts")!;
   Call("BeginConnection",source,3);
   Check(Field("pendingConnection") is not null,"起点端口手势状态已启动");
   Check(!((FrameworkElement)window.FindName("CanvasViewport")!).IsMouseCaptured,"起点按下阶段尚未捕获鼠标");
   var pointer=new Point(525,325);long revision=doc.Revision;
   var viewport=(FrameworkElement)window.FindName("CanvasViewport")!;
   viewport.RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice,Environment.TickCount){RoutedEvent=UIElement.MouseMoveEvent});Pump();
   Check(Field("pendingConnection") is null&&Field("connectionPreview") is null&&doc.Revision==revision,"左键未保持的首个 WPF 鼠标移动取消连线且不画预览");
   Call("BeginConnection",source,3);
   Check(Field("pendingConnection") is not null,"取消首帧后可再次启动连线");
   Call("FinishConnection",pointer);Pump();
   Check(doc.Revision==revision&&doc.CurrentProcedure.ExecutionLinks!.Count==0,"未经过目标预览的直接释放不凭空创建连线");
   Call("BeginConnection",source,3);
   Check(Field("pendingConnection") is not null,"直接释放后可再次由起点端口开始连接");
   Call("AdvanceConnectionPreview",pointer,MouseButtonState.Pressed);Pump();
   Check(((FrameworkElement)window.FindName("CanvasViewport")!).IsMouseCaptured,"左键保持的首个移动才捕获鼠标");
   var preview=(System.Windows.Shapes.Path)Field("connectionPreview")!;
   var figure=((PathGeometry)preview.Data).Figures.Single();
   var endpoint=((LineSegment)figure.Segments.Last()).Point;
   Check(endpoint==pointer,"目标节点上方仍以实际指针为预览终点，不提前吸附端口");
   Check(ports.Where(p=>p.Key.Node==target).Any(p=>p.Value.Visibility==Visibility.Visible),"目标节点最近端口可见并高亮");
   Check(doc.Revision==revision&&doc.CurrentProcedure.ExecutionLinks!.Count==0,"虚线预览不创建文档连接");
   var dir=Path.Combine("analysis","full-audit");Directory.CreateDirectory(dir);
   var bitmap=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);bitmap.Render(window);
   var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(bitmap));using(var output=File.Create(Path.Combine(dir,"connection-pointer-preview.png")))png.Save(output);
   viewport.ReleaseMouseCapture();Pump();
   Check(Field("pendingConnection") is null&&Field("connectionPreview") is null&&doc.Revision==revision,"首帧捕获丢失清理虚线与目标且不修改文档");
   Call("BeginConnection",source,3);
   Call("AdvanceConnectionPreview",pointer,MouseButtonState.Pressed);Pump();
   Check(Field("pendingConnection") is not null&&Field("connectionPreview") is not null,"失捕清理后可重新按下并预览目标");
   Call("FinishConnection",new Point(525,355));Pump();
   var link=ProcedureGraph.ExecutionEdges(doc.CurrentProcedure).Single();
   Check(link.FromNode==source&&link.ToNode==target&&link.SourceIndex==3&&link.TargetIndex==3,"已预览目标保持不变，松开位置决定最近目标端口");
   Check(doc.Revision==revision+1,"建连为单次撤销事务");
   doc.Undo();Pump();Check(ProcedureGraph.ExecutionEdges(doc.CurrentProcedure).Count==0,"撤销移除目标连接");
   doc.Redo();Pump();Check(ProcedureGraph.ExecutionEdges(doc.CurrentProcedure).Single().TargetIndex==3,"重做恢复目标端口索引");
   var file=Path.Combine(Path.GetTempPath(),"connection-preview-"+Guid.NewGuid().ToString("N")+".vmx.json");
   try{doc.Save(file);doc.Load(file);Pump();Check(ProcedureGraph.ExecutionEdges(doc.CurrentProcedure).Single().TargetIndex==3,"保存重开保留提交连接及目标端口");}
   finally{if(File.Exists(file))File.Delete(file);}
  }
  finally{window.Close();app.Shutdown();}
 }
}
