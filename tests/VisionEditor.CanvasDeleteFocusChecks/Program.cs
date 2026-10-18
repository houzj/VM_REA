using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;

internal static class Program
{
 [StructLayout(LayoutKind.Sequential)] struct NativePoint {public int X;public int Y;}
 [DllImport("user32.dll")] [return:MarshalAs(UnmanagedType.Bool)] static extern bool GetCursorPos(out NativePoint point);
 [DllImport("user32.dll")] [return:MarshalAs(UnmanagedType.Bool)] static extern bool SetCursorPos(int x,int y);
 [DllImport("user32.dll")] [return:MarshalAs(UnmanagedType.Bool)] static extern bool PostMessage(IntPtr hwnd,uint message,IntPtr wparam,IntPtr lparam);
 [STAThread] static int Main(){try{Run();return 0;}catch(Exception ex){Console.Error.WriteLine(ex);return 1;}}
 static void Run()
 {
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
  var window=new MainWindow{Width=1050,Height=750,Left=120,Top=90,Topmost=true,WindowStartupLocation=WindowStartupLocation.Manual};
  SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(window.Dispatcher));
  object? Field(string name)=>typeof(MainWindow).GetField(name,BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window);
  void Pump(){window.UpdateLayout();window.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
  void Check(bool ok,string label){if(!ok)throw new Exception(label);Console.WriteLine("PASS "+label);}
  var previousCursor=GetCursorPos(out var cursor)?cursor:new NativePoint();
  try
  {
   window.Show();window.Activate();Pump();
   var doc=(GraphEditorDocument)Field("doc")!;doc.NewSolution();
   string first=doc.AddNode("vision.image-source","1.0.0",220,160),second=doc.AddNode("vision.image-source","1.0.0",330,280);Pump();
   var layer=(Canvas)window.FindName("GraphLayer")!;
   var viewport=(FrameworkElement)window.FindName("CanvasViewport")!;
   var other=(Button)window.FindName("AddProcedureButton")!;
   typeof(MainWindow).GetMethod("SetBoxSelection",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(window,[new Rect(200,130,260,200)]);Pump();
   var selected=(HashSet<string>)Field("selectedNodes")!;Check(selected.SetEquals([first,second]),"真实框选入口选中两个节点");
   var hover=viewport.PointToScreen(new Point(300,300));var moved=SetCursorPos((int)hover.X+1,(int)hover.Y);SetCursorPos((int)hover.X,(int)hover.Y);var client=viewport.TranslatePoint(new Point(300,300),window);var packed=(int)client.X|((int)client.Y<<16);PostMessage(new WindowInteropHelper(window).Handle,0x0200,IntPtr.Zero,(IntPtr)packed);Mouse.Synchronize();Pump();GetCursorPos(out var current);Console.WriteLine($"cursor moved={moved}, requested={hover}, actual={current.X},{current.Y}, viewport={viewport.ActualWidth:F0}x{viewport.ActualHeight:F0}, visible={viewport.IsVisible}, over={viewport.IsMouseOver}");
   Check(viewport.IsMouseOver,"真实指针悬在工作流画布上");
   KeyEventArgs Delete(UIElement source){var e=new KeyEventArgs(Keyboard.PrimaryDevice,PresentationSource.FromVisual(source)!,Environment.TickCount,Key.Delete){RoutedEvent=Keyboard.PreviewKeyDownEvent};source.RaiseEvent(e);Pump();return e;}
   other.Focus();Pump();Check(other.IsKeyboardFocusWithin&&!layer.IsKeyboardFocusWithin,"画布悬停但其它按钮持有键盘焦点");
   var output=System.IO.Path.Combine("analysis","full-audit","canvas-delete-focus.png");System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(output)!);
   var bmp=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);bmp.Render(window);var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(bmp));using(var stream=System.IO.File.Create(output))png.Save(stream);
   long revision=doc.Revision;Delete(other);Check(doc.Revision==revision&&doc.CurrentProcedure.Nodes.Count==2,"非画布焦点 Delete 不删除多选节点");
   var input=new TextBox{Text="AB",Width=100,Height=25};layer.Children.Add(input);Canvas.SetLeft(input,20);Canvas.SetTop(input,20);Pump();
   input.Focus();input.Select(1,1);Pump();var key=Delete(input);Check(!key.Handled&&doc.CurrentProcedure.Nodes.Count==2,"输入框 Delete 未被画布截获且不删除节点");
   layer.Children.Remove(input);layer.Focus();Pump();Check(layer.IsKeyboardFocusWithin,"真实画布取得键盘焦点");
   Delete(layer);Check(doc.CurrentProcedure.Nodes.Count==0,"画布焦点 Delete 原子删除两个选中节点");
   doc.Undo();Pump();Check(doc.CurrentProcedure.Nodes.Count==2,"多选删除一次撤销恢复两节点");
   doc.Redo();Pump();Check(doc.CurrentProcedure.Nodes.Count==0,"多选删除重做后节点再次移除");
   doc.Undo();Pump();
   var file=System.IO.Path.Combine(System.IO.Path.GetTempPath(),"canvas-delete-focus-"+Guid.NewGuid().ToString("N")+".vmx.json");
   try{doc.Save(file);doc.Load(file);Pump();Check(doc.CurrentProcedure.Nodes.Count==2,"多选删除撤销后保存重开保留两节点");}
   finally{if(System.IO.File.Exists(file))System.IO.File.Delete(file);}
  }
  finally{SetCursorPos(previousCursor.X,previousCursor.Y);window.Close();app.Shutdown();}
 }
}
