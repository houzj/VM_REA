using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;

internal static class Program
{
 [STAThread] static void Main()
 {
  int checks=0;void Check(bool ok,string name){if(!ok)throw new Exception(name);checks++;Console.WriteLine("PASS "+name);}
  var registry=new ModuleRegistry();WorkflowCalls.Register(registry);
  var source=new ModuleDescriptor("test.input.source","1","源",[],[new("value",DataKind.Integer)],[]);
  var sink=new ModuleDescriptor("test.input.sink","1","接收",[new("value",DataKind.Integer)],[new("value",DataKind.Integer)],[]);
  registry.Register(source,()=>new SourceModule(source));registry.Register(sink,()=>new SinkModule(sink));
  var body=new ProcedureDocument("body","Group",[new("sink",sink.Id,"1","接收",new())],[],[]){GroupOwner=new("main","group"),Configuration=new(){Outputs=[new("o","result",DataKind.Integer,"sink","value")]}};
  var group=new NodeDocument("group",WorkflowCalls.ModuleId,WorkflowCalls.Version,"Group",new()){Call=WorkflowCalls.From(body)};
  var parent=new ProcedureDocument("main","流程",[new("source",source.Id,"1","源",new()),group],[],[new("source","group")]);
  var path=Path.GetFullPath("artifacts/group-input-names.vmx.json");Directory.CreateDirectory(Path.GetDirectoryName(path)!);SolutionStore.Save(path,new(SolutionStore.CurrentVersion,"input-names","input-names",[parent,body],[]));
  var editor=new GraphEditorDocument(registry);editor.Load(path);
  ProcedureDocument Parent()=>editor.Document.Procedures.Single(p=>p.Id=="main");ProcedureDocument Body()=>editor.Document.Procedures.Single(p=>p.Id=="body");NodeDocument Group()=>Parent().Nodes.Single(n=>n.Id=="group");
  ExecutionSessionResult Run()=>Task.Run(()=>new SolutionExecutor(registry).RunAsync(new(editor.Document,editor.BaseDirectory),"main")).GetAwaiter().GetResult();
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
  var window=new SubprogramCallWindow(Parent,Group,()=>editor.Document.Procedures,_=>{},(_,_)=>{},kind=>kind==DataKind.Integer?[new WorkflowSourceOption("source","value",DataKind.Integer,"源.value")]:[],()=>Task.CompletedTask,()=>{},()=>null,
   saveGroupInputs:(items,bindings)=>editor.ConfigureOwnedGroupInputs("group",items,bindings),saveGroupConfiguration:c=>editor.SetWorkflowConfiguration("body",c),groupRegistry:registry)
  {ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};window.Show();Pump();
  void Add(){Children(window).OfType<ComboBox>().Single(c=>Equals(c.Tag,"GroupInputAddType")).SelectedIndex=1;Pump();Children(window).OfType<Button>().Single(b=>Equals(b.Tag,"GroupInputAdd")).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));Pump();}
  Add();var input=Body().Configuration.Inputs.Single();Check(input.Name=="in"&&Group().Call!.Inputs.Single().Name=="in","Actual WPF simple input starts with native in name");
  Add();var extra=Body().Configuration.Inputs.Single(i=>i.Id!=input.Id);Check(extra.Name=="in0","Second WPF input uses native in0 collision suffix");
  Children(window).OfType<Button>().Single(b=>Equals(b.Tag,"GroupInputDelete:"+extra.Id)).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));Pump();Check(Body().Configuration.Inputs.Count==1,"WPF delete removes duplicate candidate");
  editor.ConfigureOwnedGroupInputs("group",Body().Configuration.Inputs,new Dictionary<string,WorkflowValueReference?>{{input.Id,new(WorkflowSourceKind.NodeOutput,"source","value")}});
  editor.SetWorkflowConfiguration("body",Body().Configuration with{Subscriptions=[new("sink","value",false,new(WorkflowSourceKind.WorkflowInput,input.Id))]});
  var run=Run();Check(run.Status==SessionStatus.Success&&run.Calls["group"].WorkflowInputs["in"].As<long>()==7&&run.Calls["group"].WorkflowOutputs["result"].As<long>()==7,"Named input carries actual parent value into internal module and result");
  editor.Undo();Check(Body().Configuration.Subscriptions.Count==0,"Undo clears internal input consumer atomically");editor.Redo();Check(Run().Calls["group"].WorkflowOutputs["result"].As<long>()==7,"Redo restores Group data flow");
  window.Close();editor.Save(path);editor.Load(path);Check(Body().Configuration.Inputs.Single().Name=="in"&&Run().Calls["group"].WorkflowOutputs["result"].As<long>()==7,"Save and reopen retain native input name and actual execution");
  app.Shutdown();Console.WriteLine($"PASS {checks} Group input name checks");
 }
 static void Pump(){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);}
 static IEnumerable<FrameworkElement> Children(DependencyObject root){if(root is FrameworkElement e)yield return e;for(int i=0;i<System.Windows.Media.VisualTreeHelper.GetChildrenCount(root);i++)foreach(var child in Children(System.Windows.Media.VisualTreeHelper.GetChild(root,i)))yield return child;}
}
sealed class SourceModule(ModuleDescriptor descriptor):IModule{public ModuleDescriptor Descriptor=>descriptor;public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"value",PortValue.Integer(7)}}));}
sealed class SinkModule(ModuleDescriptor descriptor):IModule{public ModuleDescriptor Descriptor=>descriptor;public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"value",context.Inputs["value"]}}));}
