using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using VisionCore;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;

internal static class Program
{
 [STAThread] static void Main()
 {
  int checks=0;void Check(bool ok,string name){if(!ok)throw new Exception(name);checks++;Console.WriteLine("PASS "+name);}
  JsonElement J<T>(T value)=>JsonSerializer.SerializeToElement(value);
  var registry=new ModuleRegistry();WorkflowCalls.Register(registry);
  var descriptor=new ModuleDescriptor("test.group.point","1","点",[new("index",DataKind.Integer)],[new("point",DataKind.Point)],[]);registry.Register(descriptor,()=>new PointModule(descriptor));
  var body=new ProcedureDocument("body","Group",[new("point",descriptor.Id,"1","点",new())],[],[]){GroupOwner=new("main","group"),Configuration=new(){Subscriptions=[new("point","index",false,new(WorkflowSourceKind.GroupLoopIndex,"LoopIndex"))]}};
  var group=new NodeDocument("group",WorkflowCalls.ModuleId,WorkflowCalls.Version,"Group",new(){["EnableLoop"]=J(true),["StartIndex"]=J(0),["EndIndex"]=J(2)}){Call=WorkflowCalls.From(body)};
  var parent=new ProcedureDocument("main","流程",[group],[],[]);var path=Path.GetFullPath("artifacts/group-point-output.vmx.json");Directory.CreateDirectory(Path.GetDirectoryName(path)!);SolutionStore.Save(path,new(SolutionStore.CurrentVersion,"point","point",[parent,body],[]));
  var editor=new GraphEditorDocument(registry);editor.Load(path);
  NodeDocument Group()=>editor.Document.Procedures.Single(p=>p.Id=="main").Nodes.Single();
  ProcedureDocument Body()=>editor.Document.Procedures.Single(p=>p.Id=="body");
  ExecutionSessionResult Run()=>Task.Run(()=>new SolutionExecutor(registry).RunAsync(new(editor.Document,editor.BaseDirectory),"main")).GetAwaiter().GetResult();
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
  var window=new SubprogramCallWindow(()=>editor.Document.Procedures.Single(p=>p.Id=="main"),Group,()=>editor.Document.Procedures,_=>{},(_,_)=>{},_=>[],()=>Task.CompletedTask,()=>{},()=>null,
   saveGroupConfiguration:c=>editor.SetWorkflowConfiguration("body",c),groupSources:kind=>kind==DataKind.Point?[new WorkflowSourceOption("point","point",DataKind.Point,"点.point")]:[],groupRegistry:registry)
  {ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};window.Show();Pump();
  var tabs=Children(window).OfType<TabControl>().First(t=>t.Items.Count==5);tabs.SelectedIndex=1;Pump();
  Children(window).OfType<Button>().Single(b=>Equals(b.Tag,"GroupOutputAddPoint")).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));Pump();
  var output=Body().Configuration.Outputs.Single();Check(output.Kind==DataKind.Point&&output.Name=="out0","Actual WPF dedicated point-output button adds Point interface");
  editor.SetWorkflowConfiguration("body",Body().Configuration with{Outputs=[output with{NodeId="point",Port="point"}]});
  tabs.SelectedIndex=0;Pump();tabs.SelectedIndex=1;Pump();
  Children(window).OfType<CheckBox>().Single(c=>Equals(c.Tag,"GroupOutputAccumulate:"+output.Id)).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));Pump();
  // RaiseEvent does not flip IsChecked when no input device is involved.
  if(!Body().Configuration.Outputs.Single().Accumulate){var checkbox=Children(window).OfType<CheckBox>().Single(c=>Equals(c.Tag,"GroupOutputAccumulate:"+output.Id));checkbox.IsChecked=true;checkbox.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));Pump();}
  Check(Body().Configuration.Outputs.Single().Accumulate&&Group().Call!.Outputs.Single().Cardinality==ValueCardinality.Array,"WPF accumulation switch updates public Point array signature");
  var run=Run();var points=run.Calls["group"].WorkflowOutputs["out0"].As<IReadOnlyList<Point2>>();Check(run.Status==SessionStatus.Success&&points.Count==2&&points[0].X==0&&points[1].X==1,"Two actual Group rounds export ordered Point values");
  editor.Undo();Check(!Body().Configuration.Outputs.Single().Accumulate&&Group().Call!.Outputs.Single().Cardinality==ValueCardinality.Scalar,"Undo restores scalar output signature");
  editor.Redo();Check(Body().Configuration.Outputs.Single().Accumulate&&Run().Calls["group"].WorkflowOutputs["out0"].ElementCount==2,"Redo restores Point accumulation and runtime data");
  window.Close();editor.Save(path);editor.Load(path);Check(Body().Configuration.Outputs.Single().Kind==DataKind.Point&&Run().Calls["group"].WorkflowOutputs["out0"].ElementCount==2,"Save and reopen retains Point output and two-round data");
  app.Shutdown();Console.WriteLine($"PASS {checks} Group point output checks");
 }
 static void Pump(){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);}
 static IEnumerable<FrameworkElement> Children(DependencyObject root){if(root is FrameworkElement e)yield return e;for(int i=0;i<System.Windows.Media.VisualTreeHelper.GetChildrenCount(root);i++)foreach(var child in Children(System.Windows.Media.VisualTreeHelper.GetChild(root,i)))yield return child;}
}
sealed class PointModule(ModuleDescriptor descriptor):IModule{public ModuleDescriptor Descriptor=>descriptor;public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token){var i=context.Inputs["index"].As<long>();return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"point",PortValue.Point(new(i,i+1))}}));}}
