using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;

internal static class Program
{
 [StructLayout(LayoutKind.Sequential)] struct NativePoint {public int X;public int Y;}
 [DllImport("user32.dll")] [return:MarshalAs(UnmanagedType.Bool)] static extern bool GetCursorPos(out NativePoint point);
 [DllImport("user32.dll")] [return:MarshalAs(UnmanagedType.Bool)] static extern bool SetCursorPos(int x,int y);
 [DllImport("user32.dll")] [return:MarshalAs(UnmanagedType.Bool)] static extern bool PostMessage(IntPtr hwnd,uint message,IntPtr wparam,IntPtr lparam);
 [STAThread] static int Main(string[] args){try{Run(args);return 0;}catch(Exception ex){Console.Error.WriteLine(ex);return 1;}}
 static void Run(string[] args)
 {
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
  var window=new MainWindow{Width=1200,Height=780,Left=110,Top=80,Topmost=true,WindowStartupLocation=WindowStartupLocation.Manual};
  SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(window.Dispatcher));
  object? Field(string name)=>typeof(MainWindow).GetField(name,BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window);
  void Pump(){window.UpdateLayout();window.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
  void Check(bool ok,string label){if(!ok)throw new Exception(label);Console.WriteLine("PASS "+label);}
  var previousCursor=GetCursorPos(out var cursor)?cursor:new NativePoint();
  try
  {
   window.Show();window.Activate();Pump();
   var doc=(GraphEditorDocument)Field("doc")!;doc.NewSolution();
   var first=doc.AddNode("vision.image-source","1.0.0",140,170);var second=doc.AddNode("vision.image-source","1.0.0",380,360);doc.ConnectExecution(first,second);Pump();
   var hits=(Dictionary<string,Path>)Field("wireHitPaths")!;
   var handles=(List<Rectangle>)Field("wireHandles")!;
   var hit=hits[first+">"+second];
   var displayed=graphWire();
   Path graphWire()=>((Canvas)window.FindName("GraphLayer")!).Children.OfType<Path>().Single(p=>!p.IsHitTestVisible&&p.StrokeThickness==2);
   hit.RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice,Environment.TickCount){RoutedEvent=UIElement.MouseEnterEvent});Pump();
   Check(displayed.StrokeThickness==2,"连线鼠标悬停保持原生2像素可见线宽");
   hit.RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice,Environment.TickCount){RoutedEvent=UIElement.MouseLeaveEvent});Pump();
   Check(displayed.StrokeThickness==2,"连线鼠标离开后仍保持2像素可见线宽");
   hit.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,Environment.TickCount,MouseButton.Left){RoutedEvent=Mouse.PreviewMouseDownEvent});Pump();
   Check(hit.IsKeyboardFocused&&handles.Count>0&&displayed.StrokeThickness==2,"实际连线点击取得焦点并显示中点柄，选中仍保持2像素");
   if(args.Contains("--hover-only"))return;
   var other=(Button)window.FindName("AddProcedureButton")!;other.Focus();Pump();
   Check(other.IsKeyboardFocused&&handles.Count==0,"转焦工具栏立即清除连线中点柄");
   hit.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,Environment.TickCount,MouseButton.Left){RoutedEvent=Mouse.PreviewMouseDownEvent});Pump();Check(hit.IsKeyboardFocused&&handles.Count>0,"再次点击连线重新生成中点柄");
   var output=System.IO.Path.Combine("analysis","full-audit","wire-midpoint-focus.png");System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(output)!);
   var bmp=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);bmp.Render(window);var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(bmp));using(var stream=System.IO.File.Create(output))png.Save(stream);
   var handle=handles[handles.Count/2];var graph=(Canvas)window.FindName("GraphLayer")!;var viewport=(FrameworkElement)window.FindName("CanvasViewport")!;
   var start=new Point(Canvas.GetLeft(handle)+3,Canvas.GetTop(handle)+3);
   Check(handle.IsLoaded&&graph.Children.Contains(handle),"中点柄实际位于当前画布");handle.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,Environment.TickCount,MouseButton.Left){RoutedEvent=UIElement.MouseLeftButtonDownEvent});Pump();Check(Field("wireDrag") is not null,"WPF 中点柄鼠标按下启动拖动");
   var destination=start+new Vector(25,35);var screen=graph.PointToScreen(destination);SetCursorPos((int)screen.X,(int)screen.Y);var client=graph.TranslatePoint(destination,window);PostMessage(new WindowInteropHelper(window).Handle,0x0200,IntPtr.Zero,(IntPtr)((int)client.X|((int)client.Y<<16)));Mouse.Synchronize();Pump();
   viewport.RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice,Environment.TickCount){RoutedEvent=UIElement.MouseMoveEvent});Pump();
   Check(Field("wireDrag") is not null&&new Rect(0,0,viewport.ActualWidth,viewport.ActualHeight).Contains(Mouse.GetPosition(viewport)),"鼠标移动时拖动保持有效且落点在视口内");
   viewport.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,Environment.TickCount,MouseButton.Left){RoutedEvent=Mouse.PreviewMouseUpEvent});Pump();Check(Field("wireDrag") is null,"WPF 预览鼠标释放结束拖动");
   var wire=ProcedureGraph.ExecutionEdges(doc.CurrentProcedure).Single();Check(wire.PathPoints is {Count:>1},"实际中点拖动提交路径坐标");
   var savedPoints=wire.PathPoints!.ToArray();doc.Undo();Pump();Check(ProcedureGraph.ExecutionEdges(doc.CurrentProcedure).Single().PathPoints is null,"连线路径一次撤销恢复自动计算");
   doc.Redo();Pump();Check(ProcedureGraph.ExecutionEdges(doc.CurrentProcedure).Single().PathPoints!.SequenceEqual(savedPoints),"中点拖动重做恢复所提交坐标");
   var file=System.IO.Path.Combine(System.IO.Path.GetTempPath(),"wire-focus-"+Guid.NewGuid().ToString("N")+".vmx.json");
   try{doc.Save(file);doc.Load(file);Pump();Check(ProcedureGraph.ExecutionEdges(doc.CurrentProcedure).Single().PathPoints!.SequenceEqual(savedPoints),"保存重开保留中点调整后的路径");}
   finally{if(System.IO.File.Exists(file))System.IO.File.Delete(file);}
   var edgeHit=hits[first+">"+second];edgeHit.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,Environment.TickCount,MouseButton.Left){RoutedEvent=Mouse.PreviewMouseDownEvent});Pump();
   var edgeHandle=handles[handles.Count/2];var edgeStart=new Point(Canvas.GetLeft(edgeHandle)+3,Canvas.GetTop(edgeHandle)+3);
   edgeHandle.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,Environment.TickCount,MouseButton.Left){RoutedEvent=UIElement.MouseLeftButtonDownEvent});Pump();
   Check(Field("wireDrag") is not null,"真实 WPF 中点柄再次启动边界拖动");
   var preview=typeof(MainWindow).GetMethod("PreviewWireDrag",BindingFlags.Instance|BindingFlags.NonPublic)!;
   preview.Invoke(window,[edgeStart+new Vector(35,45)]);Pump();
   var outside=viewport.PointToScreen(new Point(viewport.ActualWidth+20,Math.Max(20,viewport.ActualHeight/2)));
   SetCursorPos((int)outside.X,(int)outside.Y);Mouse.Synchronize();Pump();
   GetCursorPos(out var actualCursor);var actualViewportPoint=viewport.PointFromScreen(new Point(actualCursor.X,actualCursor.Y));
   Check(!new Rect(0,0,viewport.ActualWidth,viewport.ActualHeight).Contains(actualViewportPoint),"实际系统指针位于可见视口之外");
   long outerRevision=doc.Revision;
   viewport.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,Environment.TickCount,MouseButton.Left){RoutedEvent=Mouse.PreviewMouseUpEvent});Pump();
   var outerPoints=ProcedureGraph.ExecutionEdges(doc.CurrentProcedure).Single().PathPoints!.ToArray();
   Check(Field("wireDrag") is null&&doc.Revision==outerRevision+1&&!outerPoints.SequenceEqual(savedPoints),"视口外松开按原生 MouseLeftButtonUp 提交最后路径");
   var outerBmp=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);outerBmp.Render(window);
   var outerPng=new PngBitmapEncoder();outerPng.Frames.Add(BitmapFrame.Create(outerBmp));using(var stream=System.IO.File.Create(System.IO.Path.Combine("analysis","full-audit","wire-midpoint-outside-release.png")))outerPng.Save(stream);
   doc.Undo();Pump();Check(ProcedureGraph.ExecutionEdges(doc.CurrentProcedure).Single().PathPoints!.SequenceEqual(savedPoints),"视口外松开提交可一次撤销");
   doc.Redo();Pump();Check(ProcedureGraph.ExecutionEdges(doc.CurrentProcedure).Single().PathPoints!.SequenceEqual(outerPoints),"视口外松开提交可重做");
   file=System.IO.Path.Combine(System.IO.Path.GetTempPath(),"wire-outside-"+Guid.NewGuid().ToString("N")+".vmx.json");
   try{doc.Save(file);doc.Load(file);Pump();Check(ProcedureGraph.ExecutionEdges(doc.CurrentProcedure).Single().PathPoints!.SequenceEqual(outerPoints),"视口外松开路径保存重开不丢失");}
   finally{if(System.IO.File.Exists(file))System.IO.File.Delete(file);}
   var restored=hits[first+">"+second];
   var rightPoint=new Point((savedPoints[0].X+savedPoints[1].X)/2,(savedPoints[0].Y+savedPoints[1].Y)/2);
   var rightScreen=graph.PointToScreen(rightPoint);SetCursorPos((int)rightScreen.X,(int)rightScreen.Y);
   var rightClient=graph.TranslatePoint(rightPoint,window);var rightPacked=(IntPtr)((int)rightClient.X|((int)rightClient.Y<<16));var hwnd=new WindowInteropHelper(window).Handle;
   PostMessage(hwnd,0x0200,IntPtr.Zero,rightPacked);Mouse.Synchronize();Pump();
   restored.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,Environment.TickCount,MouseButton.Right){RoutedEvent=Mouse.PreviewMouseDownEvent});Pump();
   Check(restored.IsKeyboardFocused&&handles.Count>0&&Equals(Field("selectedWire"),first+">"+second),"真实右键按下选中连线并显示中点柄");
   var contextMenu=restored.ContextMenu!;
   Check(contextMenu.Items.Count==1&&Equals(((MenuItem)contextMenu.Items[0]).Header,"删除"),"原生样式对应的 WPF 连线删除菜单已绑定");
   // The routed right-down reaches the WPF control; synthetic right-up does not ask
   // the OS ContextMenuService to open its separate popup HWND in this host.
   contextMenu.IsOpen=true;Pump();Check(contextMenu.IsOpen,"WPF 连线菜单可打开且与右键选中状态共存");
   var menuImage=new RenderTargetBitmap(Math.Max(1,(int)Math.Ceiling(contextMenu.ActualWidth)),Math.Max(1,(int)Math.Ceiling(contextMenu.ActualHeight)),96,96,PixelFormats.Pbgra32);menuImage.Render(contextMenu);var menuPng=new PngBitmapEncoder();menuPng.Frames.Add(BitmapFrame.Create(menuImage));using(var stream=System.IO.File.Create(System.IO.Path.Combine("analysis","full-audit","wire-right-click-menu.png")))menuPng.Save(stream);
   var remove=(MenuItem)contextMenu.Items[0];remove.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));Pump();Check(ProcedureGraph.ExecutionEdges(doc.CurrentProcedure).Count==0,"连线菜单删除实际执行连线事务");
   doc.Undo();Pump();Check(ProcedureGraph.ExecutionEdges(doc.CurrentProcedure).Count==1,"菜单删除连线后一次撤销恢复");
   var final=System.IO.Path.Combine(System.IO.Path.GetTempPath(),"wire-right-menu-"+Guid.NewGuid().ToString("N")+".vmx.json");
   try{doc.Save(final);doc.Load(final);Pump();Check(ProcedureGraph.ExecutionEdges(doc.CurrentProcedure).Count==1,"菜单删除撤销后保存重开恢复连线");}
   finally{if(System.IO.File.Exists(final))System.IO.File.Delete(final);}
  }
  finally{SetCursorPos(previousCursor.X,previousCursor.Y);window.Close();app.Shutdown();}
 }
}
