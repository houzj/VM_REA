using System.IO;
using System.Reflection;
using System.Text.Json;
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
Directory.CreateDirectory("analysis/full-audit");
var lockPreferencePath=Path.GetFullPath("analysis/full-audit/shell-lock-preferences.json");
if(File.Exists(lockPreferencePath))File.Delete(lockPreferencePath);
typeof(MainWindow).Assembly.GetType("VisionEditor.ShellLockPreference")!.GetProperty("FilePath",BindingFlags.Static|BindingFlags.NonPublic)!.SetValue(null,lockPreferencePath);
var recentPath=Path.GetFullPath("analysis/full-audit/shell-recent-solutions.json");
if(File.Exists(recentPath))File.Delete(recentPath);
var recentStore=typeof(MainWindow).Assembly.GetType("VisionEditor.RecentSolutionStore")!;
recentStore.GetProperty("FilePath",BindingFlags.Static|BindingFlags.NonPublic)!.SetValue(null,recentPath);
var sampleDirectoryPath=Path.GetFullPath("analysis/full-audit/shell-sample-directory.txt");
if(File.Exists(sampleDirectoryPath))File.Delete(sampleDirectoryPath);
typeof(MainWindow).Assembly.GetType("VisionEditor.SampleDirectoryPreference")!.GetProperty("FilePath",BindingFlags.Static|BindingFlags.NonPublic)!.SetValue(null,sampleDirectoryPath);
var openDirectoryPath=Path.GetFullPath("analysis/full-audit/shell-open-directory.txt");
if(File.Exists(openDirectoryPath))File.Delete(openDirectoryPath);
typeof(MainWindow).Assembly.GetType("VisionEditor.OpenSolutionDirectoryPreference")!.GetProperty("FilePath",BindingFlags.Static|BindingFlags.NonPublic)!.SetValue(null,openDirectoryPath);
var saveDirectoryPath=Path.GetFullPath("analysis/full-audit/shell-save-directory.txt");
if(File.Exists(saveDirectoryPath))File.Delete(saveDirectoryPath);
typeof(MainWindow).Assembly.GetType("VisionEditor.SaveSolutionDirectoryPreference")!.GetProperty("FilePath",BindingFlags.Static|BindingFlags.NonPublic)!.SetValue(null,saveDirectoryPath);
var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
var samplePicker=new FixedSamplePicker();
var solutionPicker=new FixedSolutionPicker();
var savePicker=new FixedSavePicker();
var window=new MainWindow(samplePicker,solutionPicker,savePicker){ShowActivated=false,ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};
SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(window.Dispatcher));
void Pump(){window.UpdateLayout();window.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
void Check(bool value,string description){if(!value)throw new Exception(description);Console.WriteLine("PASS: "+description);}
T Find<T>(string name) where T:FrameworkElement=>window.FindName(name) as T??throw new Exception("Missing "+name);
try
{
 window.Show();Pump();
 var header=Find<Grid>("ShellHeader");var toolbar=Find<StackPanel>("ToolBar");var toolbarScroll=Find<ScrollViewer>("ToolBarScroll");
 var menu=Find<Menu>("MenuHost");
 Check(window.WindowState==WindowState.Normal,"Window starts normal");
 var procedureTabs=Find<TabControl>("ProcedureTabs");
 var fileMenu=(MenuItem)menu.Items[0];fileMenu.IsSubmenuOpen=true;Pump();
 Check(ReferenceEquals(FocusManager.GetFocusedElement(procedureTabs),fileMenu),"Opening a main menu records the menu as the procedure focus element");
 fileMenu.IsSubmenuOpen=false;Pump();
 var stopTool=toolbar.Children.OfType<Button>().Single(b=>Equals(b.ToolTip,"停止"));
 stopTool.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent,stopTool));Pump();
 Check(ReferenceEquals(FocusManager.GetFocusedElement(procedureTabs),stopTool),"Clicking a toolbar command records the button as the procedure focus element");
 var maximize=Find<Button>("MaximizeButton");
 Check(Equals(maximize.ToolTip,"最大化"),"Normal titlebar advertises maximize");
 maximize.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent,maximize));Pump();
 Check(window.WindowState==WindowState.Maximized,"Titlebar maximize button changes window state");
 Check(Equals(maximize.ToolTip,"向下还原")&&Find<Image>("MaximizeIcon").Source.ToString()!.EndsWith("windowshift_ic_normal.png",StringComparison.OrdinalIgnoreCase),"Maximized titlebar switches to native restore icon and tooltip");
 var content=Find<DockPanel>("WindowContent");
 Directory.CreateDirectory("analysis/full-audit");
 Check(NativeWindowFrame.FitsWorkArea(window,content,Path.GetFullPath("analysis/full-audit/shell-window-work-area.json")),"Maximized shell remains in work area");
 maximize.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent,maximize));Pump();
 Check(window.WindowState==WindowState.Normal,"Titlebar maximize button restores normal state");
 Check(Equals(maximize.ToolTip,"最大化")&&Find<Image>("MaximizeIcon").Source.ToString()!.EndsWith("windowshift_ic_maximize.png",StringComparison.OrdinalIgnoreCase),"Restored titlebar switches to native maximize icon and tooltip");
 var headerHandler=typeof(MainWindow).GetMethod("HeaderMouseDown",BindingFlags.NonPublic|BindingFlags.Instance)!;
 var onMenu=new MouseButtonEventArgs(Mouse.PrimaryDevice,Environment.TickCount,MouseButton.Left){RoutedEvent=UIElement.MouseLeftButtonDownEvent,Source=menu.Items[0]};
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
 window.Width=800;Pump();
 Check(toolbarScroll.ScrollableWidth>0,"Narrow shell exposes overflowing toolbar commands through horizontal scrolling");
 var wheelDown=new MouseWheelEventArgs(Mouse.PrimaryDevice,Environment.TickCount,-120){RoutedEvent=UIElement.PreviewMouseWheelEvent,Source=toolbarScroll};
 toolbarScroll.RaiseEvent(wheelDown);Pump();
 Check(wheelDown.Handled&&Math.Abs(toolbarScroll.HorizontalOffset-60)<1,"Wheel down scrolls toolbar right by half the wheel delta");
 var wheelUp=new MouseWheelEventArgs(Mouse.PrimaryDevice,Environment.TickCount,120){RoutedEvent=UIElement.PreviewMouseWheelEvent,Source=toolbarScroll};
 toolbarScroll.RaiseEvent(wheelUp);Pump();
 Check(wheelUp.Handled&&toolbarScroll.HorizontalOffset==0,"Wheel up restores hidden toolbar commands");
 var onToolbarScroll=new MouseButtonEventArgs(Mouse.PrimaryDevice,Environment.TickCount,MouseButton.Left){RoutedEvent=UIElement.MouseLeftButtonDownEvent,Source=toolbarScroll};
 headerHandler.Invoke(window,[header,onToolbarScroll]);
 Check(!onToolbarScroll.Handled&&window.WindowState==WindowState.Normal,"Toolbar scroll area is not treated as a titlebar drag gesture");
 window.Width=1366;Pump();
 var zoomSlider=Find<Slider>("ZoomSlider");var zoomLabel=Find<TextBlock>("ZoomValue");
 var resetZoom=Find<Button>("ResetCanvasZoomButton");var graphSurface=Find<Grid>("GraphSurface");
 var zoomDoc=(GraphEditorDocument)typeof(MainWindow).GetField("doc",BindingFlags.NonPublic|BindingFlags.Instance)!.GetValue(window)!;
 long revision=zoomDoc.Revision;
 zoomSlider.Value=1.5;Pump();
 Check(Math.Abs(((ScaleTransform)graphSurface.LayoutTransform).ScaleX-1.5)<.001&&zoomLabel.Text=="150%","Status slider changes the actual workflow canvas to 150%");
 resetZoom.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent,resetZoom));Pump();
 Check(Math.Abs(zoomSlider.Value-1)<.001&&Math.Abs(((ScaleTransform)graphSurface.LayoutTransform).ScaleX-1)<.001&&zoomLabel.Text=="100%","Status zoom command restores the actual canvas and displayed percent to 100%");
 Check(zoomDoc.Revision==revision,"Viewport zoom reset does not modify workflow content or undo revision");
 window.UpdateLayout();
 var statusImage=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);
 statusImage.Render(window);var statusPng=new PngBitmapEncoder();statusPng.Frames.Add(BitmapFrame.Create(statusImage));
 Directory.CreateDirectory("analysis/full-audit");using(var file=File.Create("analysis/full-audit/shell-zoom-reset.png"))statusPng.Save(file);
 var lockButton=toolbar.Children.OfType<Button>().Single(b=>Equals(b.ToolTip,"锁定工作区"));
 bool firstPrompt=false;
 window.Dispatcher.BeginInvoke(new Action(()=>
 {
  var dialog=window.OwnedWindows.OfType<ShellLockPrompt>().Single();firstPrompt=dialog.Owner==window;
  dialog.UpdateLayout();var promptContent=(FrameworkElement)dialog.Content;
  var promptImage=new RenderTargetBitmap((int)Math.Ceiling(promptContent.ActualWidth),(int)Math.Ceiling(promptContent.ActualHeight),96,96,PixelFormats.Pbgra32);
  promptImage.Render(promptContent);var promptPng=new PngBitmapEncoder();promptPng.Frames.Add(BitmapFrame.Create(promptImage));
  using(var output=File.Create("analysis/full-audit/shell-lock-confirmation.png"))promptPng.Save(output);
  ((Button)dialog.FindName("CancelLock")).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
 }),DispatcherPriority.Background);
 lockButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent,lockButton));Pump();
 Check(firstPrompt&&lockButton.ToolTip?.ToString()=="锁定工作区","First toolbar lock shows native-style confirmation and No keeps work area editable");
 bool secondPrompt=false;
 window.Dispatcher.BeginInvoke(new Action(()=>
 {
  var dialog=window.OwnedWindows.OfType<ShellLockPrompt>().Single();secondPrompt=true;
  ((CheckBox)dialog.FindName("SkipPrompt")).IsChecked=true;
  ((Button)dialog.FindName("ConfirmLock")).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
 }),DispatcherPriority.Background);
 lockButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent,lockButton));Pump();
 Check(secondPrompt&&lockButton.ToolTip?.ToString()=="解锁工作区"&&!Find<Button>("AddProcedureButton").IsEnabled,"Yes locks actual editor commands and updates toolbar state");
 Check(File.ReadAllText(lockPreferencePath).Contains("\"IsNotifyLock\":true"),"Do-not-show preference is persisted separately from the solution");
 lockButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent,lockButton));Pump();
 Check(lockButton.ToolTip?.ToString()=="锁定工作区"&&Find<Button>("AddProcedureButton").IsEnabled,"Unlock is immediate and restores actual editor commands");
 lockButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent,lockButton));Pump();
 Check(lockButton.ToolTip?.ToString()=="解锁工作区"&&window.OwnedWindows.OfType<ShellLockPrompt>().Count()==0,"Saved do-not-show preference bypasses the next confirmation");
 lockButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent,lockButton));Pump();
 var run=(Task)typeof(MainWindow).GetMethod("Run",BindingFlags.NonPublic|BindingFlags.Instance)!.Invoke(window,null)!;
 var deadline=DateTime.UtcNow.AddSeconds(15);
 while(!run.IsCompleted&&DateTime.UtcNow<deadline){Pump();Thread.Sleep(5);}
 Check(run.IsCompletedSuccessfully,"Actual workflow run completes before opening timing view");
 var document=(GraphEditorDocument)typeof(MainWindow).GetField("doc",BindingFlags.NonPublic|BindingFlags.Instance)!.GetValue(window)!;
 var statusModuleId=document.CurrentProcedure.Nodes[0].ModuleId;
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
 var recentFirst=Path.GetFullPath("analysis/full-audit/shell-recent-first.vmx.json");
 var recentSecond=Path.GetFullPath("analysis/full-audit/shell-recent-second.vmx.json");
 document.Save(recentFirst);
 var saveMenu=((MenuItem)menu.Items[0]).Items.OfType<MenuItem>().Single(item=>Equals(item.Header,"保存"));
 saveMenu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent,saveMenu));Pump();
 document.Save(recentSecond);
 saveMenu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent,saveMenu));Pump();
 var fileRoot=(MenuItem)menu.Items[0];var recentMenu=fileRoot.Items.OfType<MenuItem>().Single(item=>Equals(item.Header,"最近打开"));
 fileRoot.IsSubmenuOpen=true;Pump();recentMenu.IsSubmenuOpen=true;Pump();
 var recentMenuCaptured=false;
 if(recentMenu.Template.FindName("PART_Popup",recentMenu) is Popup recentPopup&&recentPopup.Child is FrameworkElement recentContent&&recentContent.ActualWidth>0&&recentContent.ActualHeight>0)
 {
  var recentBitmap=new RenderTargetBitmap((int)Math.Ceiling(recentContent.ActualWidth),(int)Math.Ceiling(recentContent.ActualHeight),96,96,PixelFormats.Pbgra32);
  recentBitmap.Render(recentContent);var recentPng=new PngBitmapEncoder();recentPng.Frames.Add(BitmapFrame.Create(recentBitmap));
  using var recentFile=File.Create("analysis/full-audit/shell-recent-menu.png");recentPng.Save(recentFile);
  recentMenuCaptured=true;
 }
 Check(recentMenuCaptured,"Recent submenu renders a reviewable test-only bitmap");
 Check(recentMenu.Items.Count==2&&Equals(((MenuItem)recentMenu.Items[0]).Tag,recentSecond)&&Equals(((MenuItem)recentMenu.Items[1]).Tag,recentFirst),"Save menu persists and orders recent solution paths");
 var earlier=(MenuItem)recentMenu.Items[1];
 Check(Equals(earlier.Header,Path.GetFileNameWithoutExtension(recentFirst))&&Equals(earlier.ToolTip,recentFirst),"Recent menu uses the source filename-without-extension while retaining the full path tooltip");
 var recentRunField=typeof(MainWindow).GetField("activeRun",BindingFlags.Instance|BindingFlags.NonPublic)!;
 var recentContinuousField=typeof(MainWindow).GetField("continuousRun",BindingFlags.Instance|BindingFlags.NonPublic)!;
 using(var recentRound=new CancellationTokenSource())
 {
  recentRunField.SetValue(window,recentRound);recentContinuousField.SetValue(window,true);
  earlier.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent,earlier));Pump();
  Check(!(bool)recentContinuousField.GetValue(window)!&&!recentRound.IsCancellationRequested&&document.FilePath==recentSecond,"Recent command stops continuous mode without cancelling an active single round or replacing its document");
  recentRunField.SetValue(window,null);
 }
 earlier.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent,earlier));Pump();
 Check(document.FilePath==recentFirst&&Equals(((MenuItem)recentMenu.Items[0]).Tag,recentFirst),"Recent menu reopens an actual saved solution and moves it to the top");
 using(var recentData=JsonDocument.Parse(File.ReadAllText(recentPath)))
  Check(recentData.RootElement.EnumerateArray().Any(entry=>entry.GetProperty("Path").GetString()==recentFirst),"Recent solutions survive in the independent shell preference file");
 var sampleMenu=fileRoot.Items.OfType<MenuItem>().Single(item=>Equals(item.Header,"打开示例"));
 var sampleRunField=typeof(MainWindow).GetField("activeRun",BindingFlags.Instance|BindingFlags.NonPublic)!;
 var sampleContinuousField=typeof(MainWindow).GetField("continuousRun",BindingFlags.Instance|BindingFlags.NonPublic)!;
 using(var sampleRound=new CancellationTokenSource())
 {
  sampleRunField.SetValue(window,sampleRound);sampleContinuousField.SetValue(window,true);
  sampleMenu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent,sampleMenu));Pump();
  Check(!(bool)sampleContinuousField.GetValue(window)!&&!sampleRound.IsCancellationRequested&&document.FilePath==recentFirst&&samplePicker.LastInitialDirectory==AppContext.BaseDirectory,"Sample command stops continuous mode after selection and keeps the active single round and document");
  sampleRunField.SetValue(window,null);
 }
 sampleMenu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent,sampleMenu));Pump();
 var demoPath=Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,"demo.vmx.json"));
 Check(document.FilePath==demoPath&&Equals(((MenuItem)recentMenu.Items[0]).Tag,demoPath),"Opening the shipped example records the loaded sample as the newest recent solution");
 Check(samplePicker.LastInitialDirectory==Path.GetDirectoryName(demoPath)&&File.ReadAllText(sampleDirectoryPath)==Path.GetDirectoryName(demoPath),"Next sample picker starts in the remembered example directory");
 samplePicker.NextPath=null;
 sampleMenu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent,sampleMenu));Pump();
 Check(samplePicker.LastInitialDirectory==Path.GetDirectoryName(demoPath)&&document.FilePath==demoPath,"Cancelling the sample picker keeps the loaded solution unchanged");
 samplePicker.NextPath=Path.Combine(Path.GetDirectoryName(demoPath)!,"missing-sample.vmx.json");
 sampleContinuousField.SetValue(window,true);
 sampleMenu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent,sampleMenu));Pump();
 Check(document.FilePath==demoPath&&Equals(((MenuItem)recentMenu.Items[0]).Tag,demoPath)&&Find<TextBlock>("StatusText").Text=="示例方案文件不存在"&&(bool)sampleContinuousField.GetValue(window)!,"Missing sample path does not stop continuous mode, replace the solution or enter recent history");
 var openMenu=(MenuItem)fileRoot.Items[1];
 var activeRunField=typeof(MainWindow).GetField("activeRun",BindingFlags.Instance|BindingFlags.NonPublic)!;
 var continuousField=typeof(MainWindow).GetField("continuousRun",BindingFlags.Instance|BindingFlags.NonPublic)!;
 using(var pendingRun=new CancellationTokenSource())
 {
  activeRunField.SetValue(window,pendingRun);continuousField.SetValue(window,true);
  solutionPicker.OnPick=()=>Check(!(bool)continuousField.GetValue(window)!&&!pendingRun.IsCancellationRequested,"Open command stops continuous mode before showing the picker without cancelling an active round");
  openMenu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent,openMenu));Pump();
  Check(document.FilePath==demoPath&&!File.Exists(openDirectoryPath),"Cancelling Open leaves the solution and remembered folder unchanged");
  solutionPicker.NextPath=recentSecond;
  openMenu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent,openMenu));Pump();
  Check(document.FilePath==demoPath&&!pendingRun.IsCancellationRequested&&File.ReadAllText(openDirectoryPath)==Path.GetDirectoryName(recentSecond),"An active one-shot run prevents document replacement while retaining the chosen directory");
  activeRunField.SetValue(window,null);
 }
 solutionPicker.OnPick=null;solutionPicker.NextPath=recentSecond;
 openMenu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent,openMenu));Pump();
 Check(document.FilePath==recentSecond&&Equals(((MenuItem)recentMenu.Items[0]).Tag,recentSecond),"Open picker loads a saved solution and updates recent solutions");
 Check(solutionPicker.LastInitialDirectory==Path.GetDirectoryName(recentSecond)&&File.ReadAllText(openDirectoryPath)==Path.GetDirectoryName(recentSecond),"Open picker remembers its selected directory");
 solutionPicker.NextPath=null;
 openMenu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent,openMenu));Pump();
 Check(solutionPicker.LastInitialDirectory==Path.GetDirectoryName(recentSecond)&&document.FilePath==recentSecond,"Next Open starts in the remembered directory; cancelling retains the solution");
 var saveAsMenu=fileRoot.Items.OfType<MenuItem>().Single(item=>Equals(item.Header,"另存为"));
 var recentCount=recentMenu.Items.Count;
 saveAsMenu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent,saveAsMenu));Pump();
 Check(document.FilePath==recentSecond&&!File.Exists(saveDirectoryPath)&&recentMenu.Items.Count==recentCount&&savePicker.LastInitialDirectory==string.Empty,"Cancelling Save As retains document, directory preference and recent history");
 var saveAsPath=Path.GetFullPath("analysis/full-audit/save-as-target/shell-save-as.vmx.json");
 Directory.CreateDirectory(Path.GetDirectoryName(saveAsPath)!);
 if(File.Exists(saveAsPath))File.Delete(saveAsPath);
 savePicker.NextPath=saveAsPath;
 saveAsMenu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent,saveAsMenu));Pump();
 Check(document.FilePath==saveAsPath&&File.Exists(saveAsPath)&&Equals(((MenuItem)recentMenu.Items[0]).Tag,saveAsPath),"Save As menu writes a real solution and refreshes recent history");
 Check(File.ReadAllText(saveDirectoryPath)==Path.GetDirectoryName(saveAsPath)&&File.ReadAllText(openDirectoryPath)==Path.GetDirectoryName(recentSecond),"Successful Save As persists a directory separate from Open");
 savePicker.NextPath=null;
 saveAsMenu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent,saveAsMenu));Pump();
 Check(savePicker.LastInitialDirectory==Path.GetDirectoryName(saveAsPath)&&document.FilePath==saveAsPath,"Next Save As starts in its remembered directory; cancellation keeps the active path");
 savePicker.NextPath="bad\0name.vmx.json";
 saveAsMenu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent,saveAsMenu));Pump();
 Check(document.FilePath==saveAsPath&&File.ReadAllText(saveDirectoryPath)==Path.GetDirectoryName(saveAsPath)&&Equals(((MenuItem)recentMenu.Items[0]).Tag,saveAsPath),"Failed Save As cannot mutate the document path, saved directory or recent history");
 document.AddProcedure("工具栏保存复验");Pump();
 var pickerCalls=savePicker.Calls;
 var saveTool=toolbar.Children.OfType<Button>().Single(b=>Equals(b.ToolTip,"保存"));
 saveTool.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent,saveTool));Pump();
 using(var savedJson=JsonDocument.Parse(File.ReadAllText(saveAsPath)))
  Check(savePicker.Calls==pickerCalls&&!document.IsDirty&&savedJson.RootElement.GetProperty("Procedures").EnumerateArray().Any(p=>p.GetProperty("Name").GetString()=="工具栏保存复验"),"Toolbar Save writes changes to the existing path without reopening Save As");
 var exitMenu=fileRoot.Items.OfType<MenuItem>().Single(item=>Equals(item.Header,"退出"));
 Check(string.Join(",",fileRoot.Items.OfType<MenuItem>().Select(item=>item.Header))=="新建方案,打开方案,最近打开,打开示例,保存,另存为,退出","File menu follows the source order for implemented native commands");
 Check(fileRoot.Items[fileRoot.Items.IndexOf(sampleMenu)-1] is Separator&&fileRoot.Items[fileRoot.Items.IndexOf(saveMenu)-1] is Separator,"Source separator flags appear before sample and save commands");
 Check(fileRoot.Items[fileRoot.Items.IndexOf(exitMenu)-1] is Separator,"File menu separates its native Exit command from solution commands");
 fileRoot.IsSubmenuOpen=true;Pump();
 Check(exitMenu.IsVisible&&exitMenu.ActualWidth>0&&exitMenu.ActualHeight>0,"File Exit appears as a laid-out WPF menu item in the opened menu");
 var filePopup=fileRoot.Template.FindName("PART_Popup",fileRoot) as Popup;
 if(filePopup?.Child is FrameworkElement popupContent&&popupContent.ActualWidth>0&&popupContent.ActualHeight>0)
 {
  var menuBitmap=new RenderTargetBitmap((int)Math.Ceiling(popupContent.ActualWidth),(int)Math.Ceiling(popupContent.ActualHeight),96,96,PixelFormats.Pbgra32);
  menuBitmap.Render(popupContent);var menuPng=new PngBitmapEncoder();menuPng.Frames.Add(BitmapFrame.Create(menuBitmap));
  using var menuFile=File.Create("analysis/full-audit/shell-file-menu.png");menuPng.Save(menuFile);
  Check(menuBitmap.PixelWidth>0&&menuBitmap.PixelHeight>0,"File submenu renders a reviewable test-only bitmap");
 }
 var newMenu=fileRoot.Items.OfType<MenuItem>().Single(item=>Equals(item.Header,"新建方案"));
 newMenu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent,newMenu));Pump();
 Check(!document.HasCurrentProcedure&&Find<Image>("StatusIcon").Source.ToString()!.EndsWith("bottombar_config.png",StringComparison.OrdinalIgnoreCase)&&Find<TextBlock>("StatusText").Text=="等待添加组件",
  "File New displays the source NOItem icon and waiting text on an empty canvas");
 var emptyStatus=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);
 emptyStatus.Render(window);var emptyPng=new PngBitmapEncoder();emptyPng.Frames.Add(BitmapFrame.Create(emptyStatus));
 using(var file=File.Create("analysis/full-audit/shell-status-no-item.png"))emptyPng.Save(file);
 Find<Button>("EmptyAddWorkflowButton").RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));Pump();
 typeof(MainWindow).GetMethod("AddModuleAt",BindingFlags.NonPublic|BindingFlags.Instance)!.Invoke(window,[statusModuleId,120.0,100.0,null]);Pump();
 Check(document.CurrentProcedure.Nodes.Count==1&&!Find<Image>("StatusIcon").Source.ToString()!.EndsWith("bottombar_config.png",StringComparison.OrdinalIgnoreCase)&&Find<TextBlock>("StatusText").Text!="等待添加组件",
  "Adding a node leaves NOItem and updates the actual status row");
 var populatedStatus=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);
 populatedStatus.Render(window);var populatedPng=new PngBitmapEncoder();populatedPng.Frames.Add(BitmapFrame.Create(populatedStatus));
 using(var file=File.Create("analysis/full-audit/shell-status-with-node.png"))populatedPng.Save(file);
 window.Width=900;window.Height=700;Pump();
 var workarea=Find<Grid>("WorkareaGrid");var splitter=Find<GridSplitter>("WorkareaSplitter");
 var rightColumn=(ColumnDefinition)window.FindName("RightWorkareaColumn");
 splitter.RaiseEvent(new DragStartedEventArgs(0,0){RoutedEvent=Thumb.DragStartedEvent});Pump();
 Check(Math.Abs(rightColumn.MaxWidth-Math.Max(rightColumn.MinWidth,workarea.ActualWidth-65))<1,
  "Source splitter drag start caps the right result column at workarea width minus 65 pixels");
 splitter.RaiseEvent(new DragDeltaEventArgs(-1000,0){RoutedEvent=Thumb.DragDeltaEvent});Pump();
 var leftColumn=(ColumnDefinition)window.FindName("LeftWorkareaColumn");
 Check(leftColumn.ActualWidth>=59&&rightColumn.ActualWidth>=400,
  "Narrow resized shell retains a visible workflow side at the splitter limit");
 var atLimit=rightColumn.ActualWidth;
 var splitterImage=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);
 splitterImage.Render(window);var splitterPng=new PngBitmapEncoder();splitterPng.Frames.Add(BitmapFrame.Create(splitterImage));
 using(var file=File.Create("analysis/full-audit/shell-splitter-limit.png"))splitterPng.Save(file);
 splitter.RaiseEvent(new DragDeltaEventArgs(80,0){RoutedEvent=Thumb.DragDeltaEvent});Pump();
 Check(rightColumn.ActualWidth<atLimit&&leftColumn.ActualWidth>59,
  "Splitter can be dragged back after reaching the source width cap");
 window.Width=800;Find<Button>("ToggleToolboxButton").RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));Pump();
 splitter.RaiseEvent(new DragStartedEventArgs(0,0){RoutedEvent=Thumb.DragStartedEvent});
 splitter.RaiseEvent(new DragDeltaEventArgs(-1000,0){RoutedEvent=Thumb.DragDeltaEvent});Pump();
 Check(workarea.ActualWidth>465&&rightColumn.MaxWidth>=400&&leftColumn.ActualWidth>=59,
  "Expanded toolbox at minimum window width recalculates a nonnegative splitter cap");
 var closed=false;window.Closed+=(_,_)=>closed=true;
 exitMenu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent,exitMenu));Pump();
 Check(closed&&!window.IsVisible,"File Exit invokes the same real WPF window close path as the titlebar");
 Console.WriteLine("Shell command and input checks passed.");
}

finally{window.Close();app.Shutdown();}
}
}

sealed class FixedSamplePicker : ISampleSolutionPicker
{
 public string? NextPath {get;set;}=Path.Combine(AppContext.BaseDirectory,"demo.vmx.json");
 public string? LastInitialDirectory {get;private set;}
 public string? Pick(MainWindow owner,string initialDirectory){LastInitialDirectory=initialDirectory;return NextPath;}
}

sealed class FixedSolutionPicker : ISolutionFilePicker
{
 public string? NextPath {get;set;}
 public string? LastInitialDirectory {get;private set;}
 public Action? OnPick {get;set;}
 public string? Pick(MainWindow owner,string initialDirectory){LastInitialDirectory=initialDirectory;OnPick?.Invoke();return NextPath;}
}

sealed class FixedSavePicker : ISolutionSavePicker
{
 public string? NextPath {get;set;}
 public string? LastInitialDirectory {get;private set;}
 public int Calls {get;private set;}
 public string? Pick(MainWindow owner,string initialDirectory){Calls++;LastInitialDirectory=initialDirectory;return NextPath;}
}
