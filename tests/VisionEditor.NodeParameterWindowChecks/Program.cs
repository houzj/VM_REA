using System.Reflection;
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
 [STAThread] static int Main(){try{Run();return 0;}catch(Exception ex){Console.Error.WriteLine(ex);return 1;}}
 static void Run()
 {
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
  var window=new MainWindow{Width=1200,Height=780,Left=-20000,Top=-20000,ShowInTaskbar=false,ShowActivated=false,WindowStartupLocation=WindowStartupLocation.Manual};
  SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(window.Dispatcher));
  object? Field(string name)=>typeof(MainWindow).GetField(name,BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window);
  object? Call(string name,params object[] args)=>typeof(MainWindow).GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(window,args);
  void Pump(){window.UpdateLayout();window.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
  void Check(bool ok,string label){if(!ok)throw new Exception(label);Console.WriteLine("PASS "+label);}
  void Click(Canvas card,MouseButton button){card.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,Environment.TickCount,button){RoutedEvent=button==MouseButton.Left?UIElement.MouseLeftButtonDownEvent:UIElement.MouseRightButtonDownEvent});Pump();}
  void Capture(string name,FrameworkElement element){var dir=System.IO.Path.Combine("analysis","full-audit");System.IO.Directory.CreateDirectory(dir);var bmp=new RenderTargetBitmap(Math.Max(1,(int)Math.Ceiling(element.ActualWidth)),Math.Max(1,(int)Math.Ceiling(element.ActualHeight)),96,96,PixelFormats.Pbgra32);bmp.Render(element);var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(bmp));using var stream=System.IO.File.Create(System.IO.Path.Combine(dir,name));png.Save(stream);}
  try
  {
   window.Show();Pump();var doc=(GraphEditorDocument)Field("doc")!;doc.NewSolution();
   var first=doc.AddNode("vision.image-source","1.0.0",200,180);Pump();long revision=doc.Revision;
   Call("OpenWorkflowConfiguration",doc.CurrentProcedureId);Pump();
   var configuration=(Window)Field("workflowConfigurationWindow")!;
   Check(configuration.IsVisible&&(bool)Field("parameterEditorOpen")!,"实际非模态流程配置窗口打开并传播参数编辑状态");
   var cards=(Dictionary<string,Canvas>)Field("nodeCards")!;var card=cards[first];
   Check(card.ContextMenu is null,"参数窗口打开时节点右键菜单按原生模板移除");
   var selected=(HashSet<string>)Field("selectedNodes")!;selected.Clear();
   Click(card,MouseButton.Left);
   Check(selected.Count==0&&Field("movingNodes") is null&&doc.Revision==revision,"参数窗口打开时单击节点被消费，不选中或武装拖动");
   Click(card,MouseButton.Right);
   Check(card.ContextMenu is null&&doc.Revision==revision,"参数窗口打开时右键无节点菜单且不改文档");
   Capture("node-parameter-window-open.png",window);configuration.Close();Pump();
   card=cards[first];Check(!(bool)Field("parameterEditorOpen")!&&card.ContextMenu is not null,"关闭流程配置后节点菜单恢复");
   Click(card,MouseButton.Left);Check(selected.Contains(first)&&doc.Revision==revision,"关闭流程配置后单击恢复正常选中且不产生事务");
   Call("CancelCanvasGesture");Pump();
   var group=doc.AddOwnedGroup(400,200);doc.EnterOwnedGroup(group);var child=doc.AddNode("vision.image-source","1.0.0",220,170);Pump();revision=doc.Revision;
   Call("OpenDisplayedConfiguration");Pump();
   var groupWindow=(Window)Field("displayedGroupConfiguration")!;
   Check(groupWindow.IsVisible&&(bool)Field("parameterEditorOpen")!,"实际非模态 Group 配置窗口打开并传播参数编辑状态");
   card=cards[child];selected.Clear();Check(card.ContextMenu is null,"Group 配置打开时内部节点菜单移除");
   Click(card,MouseButton.Left);Check(selected.Count==0&&Field("movingNodes") is null&&doc.Revision==revision,"Group 配置打开时单击内部节点不选中或移动");
   Capture("node-group-parameter-window-open.png",window);groupWindow.Close();Pump();
   Check(!(bool)Field("parameterEditorOpen")!&&cards[child].ContextMenu is not null,"关闭 Group 配置后内部节点菜单恢复");
   var file=System.IO.Path.Combine(System.IO.Path.GetTempPath(),"node-param-window-"+Guid.NewGuid().ToString("N")+".vmx.json");
   try{doc.Save(file);doc.Load(file);doc.EnterOwnedGroup(group);Pump();Check(doc.CurrentProcedure.Nodes.Any(n=>n.Id==child),"参数窗口期间的节点交互不破坏保存重开结构");}
   finally{if(System.IO.File.Exists(file))System.IO.File.Delete(file);}
  }
  finally{window.Close();app.Shutdown();}
 }
}
