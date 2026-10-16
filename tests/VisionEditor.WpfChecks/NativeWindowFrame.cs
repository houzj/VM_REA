using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Interop;
internal static class NativeWindowFrame
{
 [StructLayout(LayoutKind.Sequential)] struct Rect {public int Left,Top,Right,Bottom;}
 [StructLayout(LayoutKind.Sequential)] struct Point {public int X,Y;}
 [DllImport("user32.dll")] static extern bool GetWindowRect(nint window,out Rect rect);
 [DllImport("user32.dll")] static extern bool ClientToScreen(nint window,ref Point point);
 [DllImport("user32.dll")] static extern bool GetClientRect(nint window,out Rect rect);
 [DllImport("user32.dll")] static extern nint MonitorFromWindow(nint window,uint flags);
 [StructLayout(LayoutKind.Sequential)] struct Monitor {public int Size;public Rect Bounds,Work;public uint Flags;}
 [DllImport("user32.dll",CharSet=CharSet.Auto)] static extern bool GetMonitorInfo(nint monitor,ref Monitor info);
 internal static bool FitsWorkArea(Window window,FrameworkElement content,string path)
 {
  nint hwnd=new WindowInteropHelper(window).Handle;var info=new Monitor{Size=Marshal.SizeOf<Monitor>()};var origin=new Point();
  if(!GetMonitorInfo(MonitorFromWindow(hwnd,2),ref info)||!GetClientRect(hwnd,out var client)||!ClientToScreen(hwnd,ref origin)||!GetWindowRect(hwnd,out var bounds))throw new InvalidOperationException("Cannot measure maximized work area.");
  var top=content.PointToScreen(new System.Windows.Point());var bottom=content.PointToScreen(new System.Windows.Point(content.ActualWidth,content.ActualHeight));
  bool fits=bounds.Left>=info.Work.Left&&bounds.Top>=info.Work.Top&&bounds.Right<=info.Work.Right&&bounds.Bottom<=info.Work.Bottom&&top.X>=origin.X&&top.Y>=origin.Y&&bottom.X<=origin.X+client.Right+1&&bottom.Y<=origin.Y+client.Bottom+1;
  File.WriteAllText(path,JsonSerializer.Serialize(new {fits,work=new[]{info.Work.Left,info.Work.Top,info.Work.Right,info.Work.Bottom},window=new[]{bounds.Left,bounds.Top,bounds.Right,bounds.Bottom},content=new[]{top.X,top.Y,bottom.X,bottom.Y},method="GetMonitorInfo + native window/client bounds + WPF PointToScreen",pixelCaptureCompared=false}));return fits;
 }
 internal static int MeasureTopInset(Window window,string path)
 {
  nint handle=new WindowInteropHelper(window).Handle;var origin=new Point();
  if(!GetWindowRect(handle,out var rect)||!ClientToScreen(handle,ref origin))throw new InvalidOperationException("Cannot measure native frame.");
  int inset=origin.Y-rect.Top;
  File.WriteAllText(path,JsonSerializer.Serialize(new {topInset=inset,windowTop=rect.Top,clientTop=origin.Y,method="GetWindowRect + ClientToScreen",pixelCaptureCompared=false}));return inset;
 }
}
