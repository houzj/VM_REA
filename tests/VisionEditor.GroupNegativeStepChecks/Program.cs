using System.IO;
using System.Text.Json;
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
  JsonElement J<T>(T value)=>JsonSerializer.SerializeToElement(value);
  var registry=new ModuleRegistry();WorkflowCalls.Register(registry);var calls=new List<long>();
  var descriptor=new ModuleDescriptor("test.negative-step","1","轮次",[new("index",DataKind.Integer)],[new("value",DataKind.Integer)],[]);registry.Register(descriptor,()=>new RoundModule(descriptor,calls));
  var body=new ProcedureDocument("body","Group",[new("round",descriptor.Id,"1","轮次",new())],[],[]){GroupOwner=new("main","group"),Configuration=new(){Subscriptions=[new("round","index",false,new(WorkflowSourceKind.GroupLoopIndex,"LoopIndex"))],Outputs=[new("o","rounds",DataKind.Integer,"round","value",Accumulate:true)]}};
  var group=new NodeDocument("group",WorkflowCalls.ModuleId,WorkflowCalls.Version,"Group",new(){["EnableLoop"]=J(false),["StartIndex"]=J(0),["EndIndex"]=J(3),["LoopStep"]=J(int.MinValue)}){Call=WorkflowCalls.From(body)};
  var parent=new ProcedureDocument("main","流程",[group],[],[]);var path=Path.GetFullPath("artifacts/group-negative-step.vmx.json");Directory.CreateDirectory(Path.GetDirectoryName(path)!);SolutionStore.Save(path,new(SolutionStore.CurrentVersion,"negative-step","negative-step",[parent,body],[]));
  var editor=new GraphEditorDocument(registry);editor.Load(path);ExecutionSessionResult? last=null;
  NodeDocument Group()=>editor.Document.Procedures.Single(p=>p.Id=="main").Nodes.Single();
  async Task Run(){calls.Clear();last=await Task.Run(()=>new SolutionExecutor(registry).RunAsync(new(editor.Document,editor.BaseDirectory),"main")).ConfigureAwait(false);}
  Run().GetAwaiter().GetResult();Check(last!.Status==SessionStatus.Success&&calls.SequenceEqual(new long[]{0}),"Disabled loop runs one body round despite hidden negative step");
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
  SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
  var window=new SubprogramCallWindow(()=>editor.Document.Procedures.Single(p=>p.Id=="main"),Group,()=>editor.Document.Procedures,_=>{},(_,_)=>{},_=>[],Run,()=>{},()=>last,
   setLoopParameter:(key,value)=>editor.SetParameter("group",key,value),setBreak:(enabled,type,comparison,bindings)=>editor.SetGroupBreak("group",enabled,type,comparison,bindings))
  {ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};window.Show();Pump();
  var tabs=Children(window).OfType<TabControl>().First(t=>t.Items.Count==5);tabs.SelectedIndex=4;Pump();
  var enable=Children(window).OfType<CheckBox>().Single(c=>Equals(c.Content,"循环使能"));enable.IsChecked=true;enable.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));Pump();
  Check(GroupLoop.Read(Group()).Enabled,"Actual WPF Group switch enables the loop");
  var runButton=(Button)window.FindName("RunOnce");runButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));while(window.IsExecuting)Pump();
  Check(last!.Status==SessionStatus.Failed&&calls.SequenceEqual(new long[]{0,int.MinValue}),"Actual WPF run stops at independent int32 range guard after two rounds");
  editor.Undo();Run().GetAwaiter().GetResult();Check(!GroupLoop.Read(Group()).Enabled&&last!.Status==SessionStatus.Success&&calls.SequenceEqual(new long[]{0}),"Undo restores disabled one-shot execution");
  editor.Redo();Run().GetAwaiter().GetResult();Check(last!.Status==SessionStatus.Failed&&calls.SequenceEqual(new long[]{0,int.MinValue}),"Redo restores guarded negative-step runtime");
  window.Close();editor.Save(path);editor.Load(path);Run().GetAwaiter().GetResult();Check(Group().Parameters["LoopStep"].GetInt32()==int.MinValue&&last!.Status==SessionStatus.Failed&&calls.SequenceEqual(new long[]{0,int.MinValue}),"Save and reopen retains negative step and guarded runtime");
  app.Shutdown();Console.WriteLine($"PASS {checks} Group negative step checks");
 }
 static void Pump(){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);}
 static IEnumerable<FrameworkElement> Children(DependencyObject root){if(root is FrameworkElement e)yield return e;for(int i=0;i<System.Windows.Media.VisualTreeHelper.GetChildrenCount(root);i++)foreach(var child in Children(System.Windows.Media.VisualTreeHelper.GetChild(root,i)))yield return child;}
}
sealed class RoundModule(ModuleDescriptor descriptor,List<long> calls):IModule{public ModuleDescriptor Descriptor=>descriptor;public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token){var index=context.Inputs["index"].As<long>();calls.Add(index);return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"value",PortValue.Integer(index)}}));}}
