using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
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
 [DllImport("user32.dll")] [return:MarshalAs(UnmanagedType.Bool)] static extern bool GetKeyboardState(byte[] state);
 [DllImport("user32.dll")] [return:MarshalAs(UnmanagedType.Bool)] static extern bool SetKeyboardState(byte[] state);
 [STAThread] static int Main(){try{Run();return 0;}catch(Exception ex){Console.Error.WriteLine(ex);return 1;}}
 static void Run()
 {
  var app=new Application {ShutdownMode=ShutdownMode.OnExplicitShutdown};
  var window=new MainWindow {ShowActivated=false,ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};
  SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(window.Dispatcher));
  object? Field(string name)=>typeof(MainWindow).GetField(name,BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window);
  void Pump(){window.UpdateLayout();window.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
  void Check(bool condition,string label){if(!condition)throw new Exception(label);Console.WriteLine("PASS "+label);}
  var saved=new byte[256];if(!GetKeyboardState(saved))throw new Exception("Cannot read thread keyboard state");
  var doc=(GraphEditorDocument)Field("doc")!;
  try
  {
   window.Show();Pump();doc.NewSolution();Pump();
   var scroll=(ScrollViewer)window.FindName("CanvasScrollViewer")!;
   var canvas=(Canvas)window.FindName("GraphLayer")!;
   scroll.ScrollToHorizontalOffset(300);scroll.ScrollToVerticalOffset(300);Pump();
   long revision=doc.Revision;
   bool? scrollPreviewHandled=null;int scrollPreviewCount=0;
   MouseWheelEventHandler observer=(_,e)=>{scrollPreviewCount++;scrollPreviewHandled=e.Handled;};
   scroll.AddHandler(Mouse.PreviewMouseWheelEvent,observer,true);
   void Keys(bool shift,bool ctrl){var state=(byte[])saved.Clone();foreach(var k in new[]{0x10,0xa0,0xa1,0x11,0xa2,0xa3})state[k]=0;if(shift)state[0x10]=state[0xa0]=0x80;if(ctrl)state[0x11]=state[0xa2]=0x80;if(!SetKeyboardState(state))throw new Exception("Cannot set thread modifier state");}
   MouseWheelEventArgs Wheel(int delta){scrollPreviewHandled=null;scrollPreviewCount=0;var e=new MouseWheelEventArgs(Mouse.PrimaryDevice,Environment.TickCount,delta){RoutedEvent=Mouse.PreviewMouseWheelEvent};canvas.RaiseEvent(e);Pump();return e;}
   Keys(false,true);Check((Keyboard.Modifiers&ModifierKeys.Control)!=0,"WPF 键盘设备识别测试线程 Ctrl");
   double beforeScale=(double)Field("scale")!;double beforeX=scroll.HorizontalOffset;
   Wheel(120);
   Check(scrollPreviewCount==1&&scrollPreviewHandled==false,"Ctrl 滚轮缩放后 ScrollViewer 继续收到未消费的预览事件");
   Check(Math.Abs((double)Field("scale")!-beforeScale-.1)<.001,"Ctrl 滚轮只推进一个缩放刻度");
   Console.WriteLine($"Ctrl zoom anchor horizontal offset: {beforeX:F2} -> {scroll.HorizontalOffset:F2}");
   Keys(true,false);beforeScale=(double)Field("scale")!;beforeX=scroll.HorizontalOffset;
   Wheel(-120);
   Check(scrollPreviewCount==1&&scrollPreviewHandled==true&&scroll.HorizontalOffset>beforeX,"Shift 横滚后向下游传递已消费事件");
   Check(Math.Abs((double)Field("scale")!-beforeScale)<.001,"Shift 单独滚轮不缩放");
   Keys(true,true);beforeScale=(double)Field("scale")!;beforeX=scroll.HorizontalOffset;
   Wheel(-120);
   Check(scrollPreviewCount==1&&scrollPreviewHandled==true&&Math.Abs(scroll.HorizontalOffset-beforeX)>1,"Ctrl+Shift 先执行横向滚动并保持已消费状态");
   Check(Math.Abs((double)Field("scale")!-beforeScale+.1)<.001,"Ctrl+Shift 仍只后退一个缩放刻度");
   Keys(false,true);beforeScale=(double)Field("scale")!;Wheel(0);Check(Math.Abs((double)Field("scale")!-beforeScale)<.001,"零滚轮增量不改变缩放刻度");
   Check(doc.Revision==revision,"滚轮操作不改变流程撤销历史");
   var dir=Path.Combine("analysis","full-audit");Directory.CreateDirectory(dir);
   var bitmap=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);bitmap.Render(window);
   var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(bitmap));using(var output=File.Create(Path.Combine(dir,"canvas-wheel-native-routing.png")))png.Save(output);
   var file=Path.Combine(Path.GetTempPath(),"canvas-wheel-"+Guid.NewGuid().ToString("N")+".vmx.json");
   var beforeReloadScale=(double)Field("scale")!;
   var beforeReloadX=scroll.HorizontalOffset;
   var beforeReloadY=scroll.VerticalOffset;
   try{doc.Save(file);doc.Load(file);Pump();Check(doc.CurrentProcedure.Nodes.Count==0,"滚轮后保存重开流程结构不变");Check(Math.Abs((double)Field("scale")!-beforeReloadScale)<.001&&Math.Abs(scroll.HorizontalOffset-beforeReloadX)<1&&Math.Abs(scroll.VerticalOffset-beforeReloadY)<1,"保存重开保持当前画布视口");}
   finally{if(File.Exists(file))File.Delete(file);}
   scroll.RemoveHandler(Mouse.PreviewMouseWheelEvent,observer);
  }
  finally{SetKeyboardState(saved);window.Close();app.Shutdown();}
 }
}
