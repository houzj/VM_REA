using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;

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
 var run=(Task)typeof(MainWindow).GetMethod("Run",BindingFlags.NonPublic|BindingFlags.Instance)!.Invoke(window,null)!;
 var deadline=DateTime.UtcNow.AddSeconds(15);
 while(!run.IsCompleted&&DateTime.UtcNow<deadline){Pump();Thread.Sleep(5);}
 Check(run.IsCompletedSuccessfully,"Actual workflow run completes before opening timing view");
 var document=(GraphEditorDocument)typeof(MainWindow).GetField("doc",BindingFlags.NonPublic|BindingFlags.Instance)!.GetValue(window)!;
 var sessions=(Dictionary<string,ExecutionSessionResult>)typeof(MainWindow).GetField("workflowSessions",BindingFlags.NonPublic|BindingFlags.Instance)!.GetValue(window)!;
 Check(sessions.TryGetValue(document.CurrentProcedureId,out var session)&&session.Nodes.Count>0,"Timing view has real workflow execution records");
 var showTime=Find<Button>("ShowTimeButton");showTime.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent,showTime));Pump();
 var detail=window.OwnedWindows.OfType<ShellTimeDialog>().Single();
 detail.UpdateLayout();var dialogContent=(FrameworkElement)detail.Content;
 var bitmap=new RenderTargetBitmap((int)Math.Ceiling(dialogContent.ActualWidth),(int)Math.Ceiling(dialogContent.ActualHeight),96,96,PixelFormats.Pbgra32);
 bitmap.Render(dialogContent);var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(bitmap));
 using(var file=File.Create("analysis/full-audit/shell-time-dialog.png"))png.Save(file);
 var timingRows=(ListView)detail.FindName("TimingList");
 Check(timingRows.Items.Count==document.CurrentProcedure.Nodes.Count,"Timing detail shows the active procedure's actual nodes");
 var actual=session!.Nodes.ToDictionary(n=>n.NodeId);var first=document.CurrentProcedure.Nodes.First(n=>actual.ContainsKey(n.Id));
 var row=timingRows.Items.Cast<ShellTimeDialog.TimeRow>().First(r=>r.Name==first.Name);
 Check(row.ModuleTime==actual[first.Id].ElapsedMilliseconds.ToString("N2")&&row.AlgorithmTime=="—","Timing row uses measured module duration and marks unavailable algorithm duration");
 showTime.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent,showTime));Pump();
 Check(window.OwnedWindows.OfType<ShellTimeDialog>().Count()==1,"Repeated timing command replaces the prior detail window");
 document.AddProcedure("耗时检查流程");Pump();
 Check(!window.OwnedWindows.OfType<ShellTimeDialog>().Any(),"Changing the active procedure closes stale timing details");
 Console.WriteLine("Shell command and input checks passed.");
}
finally{window.Close();app.Shutdown();}
}
}
