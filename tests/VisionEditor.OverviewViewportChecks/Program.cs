using System.Reflection;
using System.IO;
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
 [STAThread] static int Main()
 {
  try {Run();return 0;} catch(Exception ex){Console.Error.WriteLine(ex);return 1;}
 }
 static void Run()
 {
  var app=new Application {ShutdownMode=ShutdownMode.OnExplicitShutdown};
  var window=new MainWindow {ShowActivated=false,ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};
  SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(window.Dispatcher));
  object? Field(string name)=>typeof(MainWindow).GetField(name,BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window);
  object? Call(string name,params object[] args)=>typeof(MainWindow).GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(window,args);
  void Pump(){window.UpdateLayout();window.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
  void Check(bool pass,string text){if(!pass)throw new Exception(text);Console.WriteLine("PASS "+text);}
  double Scale()=>(double)Field("scale")!;
  void Zoom(double value){Call("SetCanvasZoom",value,new Point(250,200));Pump();}
  var doc=(GraphEditorDocument)Field("doc")!;
  try
  {
   window.Show();Pump();doc.NewSolution();Pump();
   string first=doc.CurrentProcedureId,second=doc.AddProcedure("第二流程");Pump();
   doc.SelectProcedure(first);Pump();Zoom(1.4);
   doc.SelectProcedure(second);Pump();Zoom(1.7);
   doc.SelectProcedure(first);Pump();Check(Math.Abs(Scale()-1.4)<.001,$"第一流程视口恢复: {Scale()}");
   var overviewButton=(Button)window.FindName("WorkflowViewButton")!;
   overviewButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));Pump();
   Check((bool)Field("workflowOverview")!,"真实 WPF 按钮进入总览");Zoom(.6);
   var overviewCard=(Grid)Call("CreateWorkflowOverviewCard",doc.Document.Procedures.Single(p=>p.Id==second),false)!;
   var open=FindOpen(overviewCard);
   Check(open is not null,"第二流程卡片存在展开按钮");
   open!.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));Pump();
   Check(!(bool)Field("workflowOverview")!&&doc.CurrentProcedureId==second&&Math.Abs(Scale()-1.7)<.001,"卡片展开事件恢复第二流程自身视口");
   overviewButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));Pump();
   Check((bool)Field("workflowOverview")!&&Math.Abs(Scale()-.6)<.001,"重新进入总览恢复总览自身视口");
   overviewButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));Pump();
   doc.SelectProcedure(first);Pump();Check(Math.Abs(Scale()-1.4)<.001,"切回第一流程仍恢复原视口");
   string dir=Path.Combine("analysis","full-audit");Directory.CreateDirectory(dir);
   var bitmap=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);bitmap.Render(window);
   var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(bitmap));using(var stream=File.Create(Path.Combine(dir,"canvas-overview-viewport-return.png")))png.Save(stream);
   var file=Path.Combine(Path.GetTempPath(),"overview-viewport-"+Guid.NewGuid().ToString("N")+".vmx.json");
   try {doc.Save(file);doc.Load(file);Pump();Check(doc.Document.Procedures.Count==2&&doc.Document.Procedures.Any(p=>p.Id==second),"视口操作后保存重开保持流程数据");}
   finally {if(File.Exists(file))File.Delete(file);}
  }
  finally {window.Close();app.Shutdown();}
 }
 static Button? FindOpen(DependencyObject root)
 {
  if(root is Button button&&Equals(button.ToolTip,"展开流程"))return button;
  for(int i=0;i<VisualTreeHelper.GetChildrenCount(root);i++){var match=FindOpen(VisualTreeHelper.GetChild(root,i));if(match is not null)return match;}
  return null;
 }
}
