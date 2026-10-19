using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using VisionEditor;

internal static class Program
{
    [STAThread]
    private static int Main()
    {
        try
        {
            var app=new Application {ShutdownMode=ShutdownMode.OnExplicitShutdown};
            var window=new MainWindow {ShowActivated=false,ShowInTaskbar=false,Left=-20000,Top=-20000};
            window.Show();
            window.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);
            var map=(Canvas)window.FindName("MiniMapCanvas");
            if(map is null||!map.IsVisible)throw new Exception("Mini-map was not rendered");
            Mouse.Capture(null);
            var down=new MouseButtonEventArgs(Mouse.PrimaryDevice,Environment.TickCount,MouseButton.Left)
            {
                RoutedEvent=Mouse.MouseDownEvent,
                Source=map
            };
            map.RaiseEvent(down);
            if(map.IsMouseCaptured)throw new Exception("Mouse-down captured the pointer before movement");
            if(down.Handled)throw new Exception("Native GridEagleEye_MouseDown does not consume the routed mouse event");
            if(!(bool)typeof(MainWindow).GetField("eyeDragging",BindingFlags.NonPublic|BindingFlags.Instance)!.GetValue(window)!)
                throw new Exception("The actual WPF MouseDown route did not start the mini-map gesture");
            var move=new MouseEventArgs(Mouse.PrimaryDevice,Environment.TickCount)
            {
                RoutedEvent=UIElement.MouseMoveEvent,
                Source=map
            };
            map.RaiseEvent(move);
            if(move.Handled)throw new Exception("Native GridEagleEye_MouseMove does not consume the routed mouse event");
            var up=new MouseButtonEventArgs(Mouse.PrimaryDevice,Environment.TickCount,MouseButton.Left)
            {
                RoutedEvent=Mouse.MouseUpEvent,
                Source=map
            };
            map.RaiseEvent(up);
            if(up.Handled)throw new Exception("Native GridEagleEye_MouseUp does not consume the routed mouse event");
            if((bool)typeof(MainWindow).GetField("eyeDragging",BindingFlags.NonPublic|BindingFlags.Instance)!.GetValue(window)!)
                throw new Exception("The actual WPF MouseUp route did not end the mini-map gesture");
            Console.WriteLine("PASS: Mini-map click records position without eager capture and preserves native mouse routing");
            window.Close();
            app.Shutdown();
            return 0;
        }
        catch(Exception error){Console.Error.WriteLine(error);return 1;}
    }
}
