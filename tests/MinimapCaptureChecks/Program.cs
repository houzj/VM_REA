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
                RoutedEvent=UIElement.MouseLeftButtonDownEvent,
                Source=map
            };
            typeof(MainWindow).GetMethod("MiniMapDown",BindingFlags.NonPublic|BindingFlags.Instance)!
                .Invoke(window,[map,down]);
            if(map.IsMouseCaptured)throw new Exception("Mouse-down captured the pointer before movement");
            if(!down.Handled)throw new Exception("Mini-map mouse-down was not handled");
            Console.WriteLine("PASS: Mini-map click records position without eager mouse capture");
            window.Close();
            app.Shutdown();
            return 0;
        }
        catch(Exception error){Console.Error.WriteLine(error);return 1;}
    }
}
