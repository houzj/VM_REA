using System.Reflection;
using System.Windows;
using System.Windows.Controls;
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
  var window=new MainWindow{Width=1200,Height=770,Left=-20000,Top=-20000,ShowInTaskbar=false,ShowActivated=false,WindowStartupLocation=WindowStartupLocation.Manual};
  SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(window.Dispatcher));
  object? Field(string name)=>typeof(MainWindow).GetField(name,BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window);
  void Pump(){window.UpdateLayout();window.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
  void Check(bool ok,string label){if(!ok)throw new Exception(label);Console.WriteLine("PASS "+label);}
  window.Show();Pump();
  try
  {
   var doc=(GraphEditorDocument)Field("doc")!;doc.NewSolution();
   var first=doc.AddNode("vision.image-source","1.0.0",120,103);
   var second=doc.AddNode("vision.image-source","1.0.0",250,241);
   var group=doc.AddOwnedGroup(390,241);Pump();
   typeof(MainWindow).GetMethod("SetBoxSelection",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(window,[new Rect(100,90,450,210)]);Pump();
   var selected=(HashSet<string>)Field("selectedNodes")!;Check(selected.SetEquals([first,second,group]),"实际框选入口选中两个普通节点和40px Group");
   var frame=(System.Windows.Shapes.Rectangle)Field("groupSelectionFrame")!;
   var menu=frame.ContextMenu!;var align=(MenuItem)menu.Items[4];var center=(MenuItem)align.Items[4];
   Check(Equals(center.Header,"垂直居中"),"多选 WPF 菜单提供垂直居中入口");
   var before=doc.CurrentProcedure.Nodes.Where(n=>selected.Contains(n.Id)).ToDictionary(n=>n.Id,n=>n.Y);long revision=doc.Revision;
   center.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));Pump();
   var positions=doc.CurrentProcedure.Nodes.Where(n=>selected.Contains(n.Id)).ToDictionary(n=>n.Id,n=>n.Y);
   Check(Math.Abs(positions[first]-178)<.001&&Math.Abs(positions[second]-178)<.001&&Math.Abs(positions[group]-173)<.001,"原生相对选区顶部取整：普通节点178，40px Group173");
   Check(doc.Revision==revision+1,"一次菜单动作只提交一项多节点事务");
   var output=System.IO.Path.Combine("analysis","full-audit","canvas-align-native-relative-y.png");System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(output)!);
   var bmp=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);bmp.Render(window);var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(bmp));using(var stream=System.IO.File.Create(output))png.Save(stream);
   doc.Undo();Pump();Check(doc.CurrentProcedure.Nodes.Where(n=>before.ContainsKey(n.Id)).All(n=>Math.Abs(n.Y-before[n.Id])<.001),"一次撤销恢复三节点原始非5网格高度");
   doc.Redo();Pump();Check(doc.CurrentProcedure.Nodes.Where(n=>positions.ContainsKey(n.Id)).All(n=>Math.Abs(n.Y-positions[n.Id])<.001),"重做恢复垂直居中结果");
   var file=System.IO.Path.Combine(System.IO.Path.GetTempPath(),"canvas-align-"+Guid.NewGuid().ToString("N")+".vmx.json");
   try{doc.Save(file);doc.Load(file);Pump();Check(doc.CurrentProcedure.Nodes.Where(n=>positions.ContainsKey(n.Id)).All(n=>Math.Abs(n.Y-positions[n.Id])<.001),"保存重开保留三节点垂直居中位置");}
   finally{if(System.IO.File.Exists(file))System.IO.File.Delete(file);}
  }
  finally{window.Close();app.Shutdown();}
 }
}
