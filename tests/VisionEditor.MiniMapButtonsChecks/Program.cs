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
   window.Show();Pump();doc.NewSolution();Pump();
   var map=(Canvas)window.FindName("MiniMapCanvas")!;
   var scroll=(ScrollViewer)window.FindName("CanvasScrollViewer")!;
   long revision=doc.Revision;
   void DispatchMouse(MouseButton button,RoutedEvent routedEvent){map.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,button){RoutedEvent=routedEvent});Pump();}
   DispatchMouse(MouseButton.Right,Mouse.MouseDownEvent);
   Check((bool)Field("eyeDragging")!,"右键 MouseDown 到达鹰眼原生事件处理器");
   DispatchMouse(MouseButton.Right,Mouse.MouseUpEvent);
   Check(!(bool)Field("eyeDragging")!,"右键 MouseUp 结束鹰眼手势");
   DispatchMouse(MouseButton.Left,Mouse.MouseDownEvent);
   Check((bool)Field("eyeDragging")!,"左键仍进入鹰眼手势");
   DispatchMouse(MouseButton.Left,Mouse.MouseUpEvent);
   Call("BeginMiniMapDrag",new Point(145,110));Pump();
   Check(scroll.HorizontalOffset>0&&scroll.VerticalOffset>0,"鹰眼矩形外点按跳转实际滚动偏移");
   Call("MoveMiniMapDrag",new Point(130,100));Pump();
   Check(scroll.HorizontalOffset>0,"拖动后的视口仍能更新");
   DispatchMouse(MouseButton.Left,Mouse.MouseUpEvent);
   Check(doc.Revision==revision,"鹰眼手势不写入流程撤销历史");
   var dir=Path.Combine("analysis","full-audit");Directory.CreateDirectory(dir);
   var bitmap=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);bitmap.Render(window);
   var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(bitmap));using(var output=File.Create(Path.Combine(dir,"canvas-minimap-all-buttons.png")))png.Save(output);
   var file=Path.Combine(Path.GetTempPath(),"minimap-buttons-"+Guid.NewGuid().ToString("N")+".vmx.json");
   try{doc.Save(file);doc.Load(file);Pump();Check(doc.CurrentProcedure.Nodes.Count==0,"鹰眼手势后保存重开流程结构不变");}
   finally{if(File.Exists(file))File.Delete(file);}
  }
  finally{window.Close();app.Shutdown();}
 }
}
