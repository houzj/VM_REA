using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using VisionEditor;

static class MiniMapWheelParityChecks
{
 [DllImport("user32.dll")] [return:MarshalAs(UnmanagedType.Bool)] static extern bool GetKeyboardState(byte[] state);
 [DllImport("user32.dll")] [return:MarshalAs(UnmanagedType.Bool)] static extern bool SetKeyboardState(byte[] state);
 public static void Run(MainWindow host,Action<bool,string> check)
 {
  var saved=new byte[256];if(!GetKeyboardState(saved))throw new Exception("Cannot snapshot test thread keyboard state");
  var scroll=(ScrollViewer)host.FindName("CanvasScrollViewer");var map=(Canvas)host.FindName("MiniMapCanvas");
  var method=typeof(MainWindow).GetMethod("MiniMapWheel",BindingFlags.Instance|BindingFlags.NonPublic)!;
  var scale=typeof(MainWindow).GetField("scale",BindingFlags.Instance|BindingFlags.NonPublic)!;
  var oldX=scroll.HorizontalOffset;var oldY=scroll.VerticalOffset;var oldScale=(double)scale.GetValue(host)!;
  void Pump(){host.UpdateLayout();host.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
  void Keys(bool shift,bool ctrl){var state=(byte[])saved.Clone();foreach(var k in new[]{0x10,0xa0,0xa1,0x11,0xa2,0xa3})state[k]=0;if(shift)state[0x10]=state[0xa0]=0x80;if(ctrl)state[0x11]=state[0xa2]=0x80;if(!SetKeyboardState(state))throw new Exception("Cannot set test thread modifier state");}
  MouseWheelEventArgs Wheel(int delta){var e=new MouseWheelEventArgs(Mouse.PrimaryDevice,Environment.TickCount,delta){RoutedEvent=Mouse.PreviewMouseWheelEvent};map.RaiseEvent(e);Pump();return e;}
  try
  {
   Keys(true,false);check((Keyboard.Modifiers&ModifierKeys.Shift)!=0,"Actual WPF keyboard device observes isolated test-thread Shift state");
   scroll.ScrollToHorizontalOffset(200);Pump();double start=scroll.HorizontalOffset;var right=Wheel(-120);check(right.Handled&&scroll.HorizontalOffset>start,"Actual minimap Shift wheel calls ScrollViewer line-right and advances horizontal offset");
   start=scroll.HorizontalOffset;var left=Wheel(120);check(left.Handled&&scroll.HorizontalOffset<start,"Actual minimap reverse Shift wheel calls ScrollViewer line-left");
   Keys(false,false);var plain=Wheel(120);check(!plain.Handled,"Unmodified minimap wheel remains unhandled by the recovered handler");
   Keys(false,true);var ctrl=Wheel(120);check(!ctrl.Handled,"Ctrl-only minimap zoom preserves native unhandled routing state");
   Keys(true,true);double before=(double)scale.GetValue(host)!;Wheel(before<2?120:-120);check((double)scale.GetValue(host)!=before,"Actual minimap Ctrl+Shift wheel retains zoom after horizontal scroll branch");
   before=(double)scale.GetValue(host)!;Wheel(0);check((double)scale.GetValue(host)==before,"Zero wheel delta does not change minimap zoom tick");
  }
  finally
  {
   SetKeyboardState(saved);
   typeof(MainWindow).GetMethod("SetCanvasZoom",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(host,new object[]{oldScale,new Point(0,0)});
   scroll.ScrollToHorizontalOffset(oldX);scroll.ScrollToVerticalOffset(oldY);Pump();
  }
 }
}
