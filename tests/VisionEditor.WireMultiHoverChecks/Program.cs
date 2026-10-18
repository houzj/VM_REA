using System.IO;
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
  var app=new Application {ShutdownMode=ShutdownMode.OnExplicitShutdown};
  var window=new MainWindow {ShowActivated=false,ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};
  SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(window.Dispatcher));
  object? Field(string name)=>typeof(MainWindow).GetField(name,BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window);
  object? Call(string name,params object[] args)=>typeof(MainWindow).GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(window,args);
  void Pump(){window.UpdateLayout();window.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
  void Check(bool condition,string label){if(!condition)throw new Exception(label);Console.WriteLine("PASS "+label);}
  var doc=(GraphEditorDocument)Field("doc")!;
  try
  {
   window.Show();Pump();doc.NewSolution();
   string source=doc.AddNode("vision.image-source","1.0.0",200,180);
   string target=doc.AddNode("vision.image-source","1.0.0",600,180);
   string first=doc.AddNode("vision.image-source","1.0.0",180,520);
   string second=doc.AddNode("vision.image-source","1.0.0",580,520);
   doc.ConnectExecution(source,target,3,1);Pump();
   Point? wirePoint=null;
   for(int y=180;y<=350&&wirePoint is null;y+=5)for(int x=360;x<=600;x+=5)
    if(Call("FindInsertionWire",new Point(x,y),Array.Empty<string>()) is not null){wirePoint=new Point(x,y);break;}
   Check(wirePoint.HasValue,"真实画布显示可命中的连接线");
   Call("SetBoxSelection",new Rect(160,500,610,85));Pump();
   var chosen=(HashSet<string>)Field("selectedNodes")!;
   Check(chosen.SetEquals([first,second]),"框选仅含两个待拖节点");
   Call("BeginNodeGesture",new Point(0,0),false);
   long revision=doc.Revision;
   Call("PreviewNodeMove",wirePoint!.Value);Pump();
   var views=(System.Collections.IDictionary)Field("wireViews")!;
   bool bold=views.Values.Cast<object>().Any(v=>((System.Windows.Shapes.Path)v.GetType().GetField("Item2")!.GetValue(v)!).StrokeThickness==10);
   Check(bold,"多节点拖过连线时按原生 DragEnter 加粗预览");
   Check(doc.Revision==revision&&doc.CurrentProcedure.ExecutionLinks!.Count==1,"预览不修改连接数据");
   Call("PreviewNodeMove",new Point(40,40));Pump();
   Check(views.Values.Cast<object>().All(v=>((System.Windows.Shapes.Path)v.GetType().GetField("Item2")!.GetValue(v)!).StrokeThickness==2),"节点拖离连线后恢复2像素可见线宽");
   Call("PreviewNodeMove",wirePoint.Value);Pump();
   Check(views.Values.Cast<object>().Any(v=>((System.Windows.Shapes.Path)v.GetType().GetField("Item2")!.GetValue(v)!).StrokeThickness==10),"重新拖入连线仍显示10像素插入预览");
   var dir=Path.Combine("analysis","full-audit");Directory.CreateDirectory(dir);
   var bitmap=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);bitmap.Render(window);
   var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(bitmap));using(var output=File.Create(Path.Combine(dir,"wire-multinode-hover.png")))png.Save(output);
   Call("CommitNodeMove");Pump();
   Check(doc.CurrentProcedure.ExecutionLinks!.Count==1&&doc.CurrentProcedure.ExecutionLinks[0].FromNode==source&&doc.CurrentProcedure.ExecutionLinks[0].ToNode==target,"多节点落在线上只移动，不插入拆线");
   Check(doc.Revision==revision+1,"多节点移动为单次事务");
   doc.Undo();Pump();Check(doc.CurrentProcedure.Nodes.Single(n=>n.Id==first).Y==520&&doc.CurrentProcedure.ExecutionLinks!.Count==1,"撤销恢复位置及原连线");
   doc.Redo();Pump();Check(doc.CurrentProcedure.ExecutionLinks!.Count==1,"重做仍保留原连线");
   var file=Path.Combine(Path.GetTempPath(),"wire-multinode-"+Guid.NewGuid().ToString("N")+".vmx.json");
   try{doc.Save(file);doc.Load(file);Pump();Check(doc.CurrentProcedure.ExecutionLinks!.Count==1,"保存重开不产生插入连线");}
   finally{if(File.Exists(file))File.Delete(file);}
  }
  finally{window.Close();app.Shutdown();}
 }
}
