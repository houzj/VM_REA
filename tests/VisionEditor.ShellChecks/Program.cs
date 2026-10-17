using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using VisionEditor;

static class Program
{
[STAThread]
static void Main()
{
var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
var window=new MainWindow{ShowActivated=false,ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};
SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(window.Dispatcher));
void Pump(){window.UpdateLayout();window.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
void Check(bool value,string description){if(!value)throw new Exception(description);Console.WriteLine("PASS: "+description);}
T Find<T>(string name) where T:FrameworkElement=>window.FindName(name) as T??throw new Exception("Missing "+name);
try
{
 window.Show();Pump();
 var header=Find<Grid>("ShellHeader");var toolbar=Find<StackPanel>("ToolBar");
 var menu=Find<StackPanel>("MenuHost");
 Check(window.WindowState==WindowState.Normal,"Window starts normal");
 var maximize=header.Children.OfType<Grid>().SelectMany(g=>g.Children.OfType<Grid>()).SelectMany(g=>g.Children.OfType<StackPanel>()).SelectMany(g=>g.Children.OfType<Button>()).First(b=>Equals(b.Content,"□"));
 maximize.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent,maximize));Pump();
 Check(window.WindowState==WindowState.Maximized,"Titlebar maximize button changes window state");
 var content=Find<DockPanel>("WindowContent");
 Directory.CreateDirectory("analysis/full-audit");
 Check(NativeWindowFrame.FitsWorkArea(window,content,Path.GetFullPath("analysis/full-audit/shell-window-work-area.json")),"Maximized shell remains in work area");
 maximize.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent,maximize));Pump();
 Check(window.WindowState==WindowState.Normal,"Titlebar maximize button restores normal state");
 var headerHandler=typeof(MainWindow).GetMethod("HeaderMouseDown",BindingFlags.NonPublic|BindingFlags.Instance)!;
 var onMenu=new MouseButtonEventArgs(Mouse.PrimaryDevice,Environment.TickCount,MouseButton.Left){RoutedEvent=UIElement.MouseLeftButtonDownEvent,Source=menu.Children[0]};
 headerHandler.Invoke(window,[header,onMenu]);
 Check(window.WindowState==WindowState.Normal&&!onMenu.Handled,"Menu input is not consumed as titlebar gesture");
 var onTool=new MouseButtonEventArgs(Mouse.PrimaryDevice,Environment.TickCount,MouseButton.Left){RoutedEvent=UIElement.MouseLeftButtonDownEvent,Source=toolbar.Children.OfType<Button>().First()};
 headerHandler.Invoke(window,[header,onTool]);
 Check(window.WindowState==WindowState.Normal&&!onTool.Handled,"Toolbar input is not consumed as titlebar gesture");
 var onBlank=new MouseButtonEventArgs(Mouse.PrimaryDevice,Environment.TickCount,MouseButton.Left){RoutedEvent=UIElement.MouseLeftButtonDownEvent,Source=header};
 typeof(MouseButtonEventArgs).GetProperty(nameof(MouseButtonEventArgs.ClickCount))!.SetValue(onBlank,2);
 headerHandler.Invoke(window,[header,onBlank]);Pump();
 Check(window.WindowState==WindowState.Maximized&&onBlank.Handled,"Blank header double-click maximizes");
 headerHandler.Invoke(window,[header,onBlank]);Pump();
 Check(window.WindowState==WindowState.Normal,"Blank header double-click restores");
 Console.WriteLine("Shell command and input checks passed.");
}
finally{window.Close();app.Shutdown();}
}
}
