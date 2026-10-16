using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace VisionEditor;

public partial class MainWindow
{
    // VM.Controls.BaseWindow.WmGetMinMaxInfo also uses monitor work-area bounds.
    // Our WindowChrome host uses monitor-relative coordinates, including secondary monitors.
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        HwndSource.FromHwnd(new WindowInteropHelper(this).Handle)?.AddHook(WorkAreaHook);
    }

    static nint WorkAreaHook(nint hwnd, int message, nint wParam, nint lParam, ref bool handled)
    {
        if (message != 0x0024) return 0; // WM_GETMINMAXINFO: physical pixels, no DIP conversion.
        var monitor = MonitorFromWindow(hwnd, 2);
        var info = new MonitorArea { Size = Marshal.SizeOf<MonitorArea>() };
        if (monitor == 0 || !GetMonitorInfo(monitor, ref info)) return 0;
        var limits = Marshal.PtrToStructure<WindowLimits>(lParam);
        limits.MaxPosition = new NativePoint(info.Work.Left - info.Monitor.Left, info.Work.Top - info.Monitor.Top);
        limits.MaxSize = new NativePoint(info.Work.Right - info.Work.Left, info.Work.Bottom - info.Work.Top);
        Marshal.StructureToPtr(limits, lParam, false);
        handled = true;
        return 0;
    }

    [StructLayout(LayoutKind.Sequential)] struct NativePoint
    {
        public int X, Y;
        public NativePoint(int x, int y) { X = x; Y = y; }
    }
    [StructLayout(LayoutKind.Sequential)] struct NativeRect { public int Left, Top, Right, Bottom; }
    [StructLayout(LayoutKind.Sequential)] struct WindowLimits
    {
        public NativePoint Reserved, MaxSize, MaxPosition, MinTrackSize, MaxTrackSize;
    }
    [StructLayout(LayoutKind.Sequential)] struct MonitorArea
    {
        public int Size;
        public NativeRect Monitor, Work;
        public uint Flags;
    }
    [DllImport("user32.dll")] static extern nint MonitorFromWindow(nint hwnd, uint flags);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)] static extern bool GetMonitorInfo(nint monitor, ref MonitorArea info);
}
