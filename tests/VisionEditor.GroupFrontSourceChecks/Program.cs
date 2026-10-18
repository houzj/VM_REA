using System.IO;
using System.Reflection;
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
  int checks=0;void Check(bool ok,string label){if(!ok)throw new Exception(label);Console.WriteLine("PASS "+label);checks++;}
  var registry=new ModuleRegistry();WorkflowCalls.Register(registry);var seen=new List<long>();
  var source=new ModuleDescriptor("test.group.front-source","1","来源",[],[new("value",DataKind.Integer)],[]);registry.Register(source,()=>new FixedModule(source));
  var consumer=new ModuleDescriptor("test.group.front-index","1","索引",[new("index",DataKind.Integer)],[new("value",DataKind.Integer)],[]);registry.Register(consumer,()=>new IndexModule(consumer,seen));
  var body=new ProcedureDocument("body","Group",[new("probe",consumer.Id,"1","索引",new())],[],[]){GroupOwner=new("main","group"),Configuration=new(){Subscriptions=[new("probe","index",false,new(WorkflowSourceKind.GroupLoopIndex,"LoopIndex"))],Outputs=[new("out","out",DataKind.Integer,"probe","value")]}};
  var group=new NodeDocument("group",WorkflowCalls.ModuleId,WorkflowCalls.Version,"Group",new(){["EnableLoop"]=JsonSerializer.SerializeToElement(true),["StartIndex"]=JsonSerializer.SerializeToElement(0),["EndIndex"]=JsonSerializer.SerializeToElement(1)}){Call=WorkflowCalls.From(body)};
  var parent=new ProcedureDocument("main","流程",[new("up",source.Id,"1","前置",new()),group,new("down",source.Id,"1","后置",new()),new("other",source.Id,"1","无关",new())],[],[new("up","group"),new("group","down")]);
  var path=Path.GetFullPath("artifacts/group-front-sources.vmx.json");Directory.CreateDirectory(Path.GetDirectoryName(path)!);SolutionStore.Save(path,new(SolutionStore.CurrentVersion,"front","front",[parent,body],[]));
  var editor=new GraphEditorDocument(registry);editor.Load(path);ProcedureDocument Parent()=>editor.Document.Procedures.Single(p=>p.Id=="main");NodeDocument Group()=>Parent().Nodes.Single(n=>n.Id=="group");
  HashSet<string> Front()=>((HashSet<string>)typeof(MainWindow).GetMethod("GroupFrontNodes",BindingFlags.NonPublic|BindingFlags.Static)!.Invoke(null,[Parent(),"group"])!);
  Check(Front().SetEquals(["up"]),"Native front-tree scope keeps only reachable previous node, excluding downstream and unrelated nodes");
  IEnumerable<WorkflowSourceOption> Sources(DataKind kind)=>Parent().Nodes.Where(n=>Front().Contains(n.Id)).SelectMany(n=>registry.GetDescriptor(n).Outputs.Where(p=>p.Kind==kind).Select(p=>new WorkflowSourceOption(n.Id,p.Name,p.Kind,n.Name+"."+p.Name){Reference=new(WorkflowSourceKind.NodeOutput,n.Id,p.Name)}));
  void Bind(string target,WorkflowValueReference? sourceRef){var c=Parent().Configuration;editor.SetWorkflowConfiguration("main",c with{Subscriptions=c.Subscriptions.Where(b=>b.NodeId!="group"||b.Target!=target||!b.IsParameter).Concat(sourceRef is null?[]:[new WorkflowSubscription("group",target,true,sourceRef)]).ToArray()});}
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
  var window=new SubprogramCallWindow(Parent,Group,()=>editor.Document.Procedures,_=>{},(_,_)=>{},Sources,()=>Task.CompletedTask,()=>{},()=>null,(key,value)=>editor.SetParameter("group",key,value),Bind){ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};window.Show();Pump();
  Children(window).OfType<TabControl>().Single(t=>t.Items.Count==5).SelectedIndex=4;Pump();
  Exception? pickerError=null;window.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,new Action(()=>{WorkflowSourcePicker? picker=null;try{picker=Application.Current.Windows.OfType<WorkflowSourcePicker>().Single();var labels=Children(picker).OfType<CheckBox>().Select(c=>Convert.ToString(c.Content)).ToArray();var groups=Children(picker).OfType<TreeViewItem>().Select(i=>Convert.ToString(i.Header)).ToArray();Check(groups.Contains("前置")&&labels.Contains("value")&&!groups.Contains("后置")&&!groups.Contains("无关"),"Actual WPF Group boundary picker excludes downstream and unrelated node outputs");Children(picker).OfType<CheckBox>().Single(c=>Equals(c.Content,"value")).IsChecked=true;Children(picker).OfType<Button>().Single(b=>Equals(b.Content,"确定")).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));}catch(Exception e){pickerError=e;picker?.Close();}}));
  Children(window).OfType<Button>().Single(b=>Equals(b.Content,"1")).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));Pump();if(pickerError is not null)throw pickerError;
  Check(Parent().Configuration.Subscriptions.Single().Source.Id=="up","WPF Group loop end binds reachable upstream value");
  int Iterations(){seen.Clear();var run=Task.Run(()=>new SolutionExecutor(registry).RunAsync(new(editor.Document,editor.BaseDirectory),"main")).GetAwaiter().GetResult();Check(run.Status==SessionStatus.Success,"Actual Group run succeeds");return seen.Count;}
  Check(Iterations()==2&&seen.SequenceEqual(new long[]{0,1}),"Upstream value 2 controls two actual Group loop iterations");
  editor.Undo();Check(Iterations()==1,"Undo removes boundary source and restores one iteration");editor.Redo();Check(Iterations()==2,"Redo restores upstream-bound Group run");
  window.Close();editor.Save(path);editor.Load(path);Check(Iterations()==2&&Parent().Configuration.Subscriptions.Single().Source.Id=="up","Save and reopen retains reachable-source binding and data result");
  app.Shutdown();Console.WriteLine($"PASS {checks} Group front source checks");
 }
 static void Pump(){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);}
 static IEnumerable<FrameworkElement> Children(DependencyObject root){if(root is FrameworkElement e)yield return e;for(int i=0;i<System.Windows.Media.VisualTreeHelper.GetChildrenCount(root);i++)foreach(var child in Children(System.Windows.Media.VisualTreeHelper.GetChild(root,i)))yield return child;}
}
sealed class FixedModule(ModuleDescriptor descriptor):IModule{public ModuleDescriptor Descriptor=>descriptor;public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"value",PortValue.Integer(2)}}));}
sealed class IndexModule(ModuleDescriptor descriptor,List<long> seen):IModule{public ModuleDescriptor Descriptor=>descriptor;public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token){var i=context.Inputs["index"].As<long>();seen.Add(i);return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"value",PortValue.Integer(i)}}));}}
