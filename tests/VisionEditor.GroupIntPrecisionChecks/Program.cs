using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
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
  var registry=new ModuleRegistry();WorkflowCalls.Register(registry);
  var descriptor=new ModuleDescriptor("test.int32.round","1","轮次",[new("index",DataKind.Integer)],[new("value",DataKind.Integer)],[]);
  registry.Register(descriptor,()=>new RoundModule(descriptor));
  var body=new ProcedureDocument("body","Group",[new("round",descriptor.Id,"1","轮次",new())],[],[]){GroupOwner=new("main","group"),Configuration=new(){Subscriptions=[new("round","index",false,new(WorkflowSourceKind.GroupLoopIndex,"LoopIndex"))],Outputs=[new("o","rounds",DataKind.Integer,"round","value",Accumulate:true)]}};
  var group=new NodeDocument("group",WorkflowCalls.ModuleId,WorkflowCalls.Version,"Group",new(){["EnableLoop"]=J(true),["StartIndex"]=J(0),["EndIndex"]=J(2),["EnableBreak"]=J(true),["DataType"]=J(1),["CmpType"]=J(2)}){Call=WorkflowCalls.From(body),GroupBreak=new(new(Constant:J(16777216)),new(Constant:J(16777217)))};
  var parent=new ProcedureDocument("main","流程",[group],[],[]);var path=Path.GetFullPath("artifacts/group-int32-roundtrip.vmx.json");Directory.CreateDirectory(Path.GetDirectoryName(path)!);SolutionStore.Save(path,new(SolutionStore.CurrentVersion,"int32","int32",[parent,body],[]));
  var editor=new GraphEditorDocument(registry);editor.Load(path);
  NodeDocument Group()=>editor.Document.Procedures.Single(p=>p.Id=="main").Nodes.Single();
  ExecutionSessionResult Run()=>new SolutionExecutor(registry).RunAsync(new(editor.Document,editor.BaseDirectory),"main").GetAwaiter().GetResult();
  Check(Run().Calls["group"].CompletedGroupIterations==0,"Less-than comparison breaks before first round");
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
  var window=new SubprogramCallWindow(()=>editor.Document.Procedures.Single(p=>p.Id=="main"),Group,()=>editor.Document.Procedures,_=>{},(_,_)=>{},_=>[],()=>Task.CompletedTask,()=>{},()=>null,
   setLoopParameter:(key,value)=>editor.SetParameter("group",key,value),
   setBreak:(enabled,type,comparison,bindings)=>editor.SetGroupBreak("group",enabled,type,comparison,bindings))
  {ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};
  window.Show();Pump();var tabs=Children(window).OfType<TabControl>().First(t=>t.Items.Count==5);tabs.SelectedIndex=4;Pump();
  var comparisonBox=Children(window).OfType<ComboBox>().Single(c=>Equals(c.Tag,"GroupBreakOperator"));comparisonBox.SelectedIndex=2;Pump();
  Check(GroupBreak.Integer(Group(),"CmpType",0)==3,"Actual WPF Group operator changes to equality");
  var run=Run();Check(run.Status==SessionStatus.Success&&run.Calls["group"].CompletedGroupIterations==2&&!run.Calls["group"].GroupBroke&&run.Calls["group"].WorkflowOutputs["rounds"].As<IReadOnlyList<long>>().SequenceEqual(new long[]{0,1}),"Distinct adjacent int32 operands execute two real rounds and accumulate output");
  editor.Undo();Check(GroupBreak.Integer(Group(),"CmpType",0)==2&&Run().Calls["group"].CompletedGroupIterations==0,"Undo restores prior comparison and run behavior");
  editor.Redo();Check(GroupBreak.Integer(Group(),"CmpType",0)==3&&Run().Calls["group"].CompletedGroupIterations==2,"Redo restores equality and two-round output");
  window.Close();editor.Save(path);editor.Load(path);run=Run();Check(run.Status==SessionStatus.Success&&run.Calls["group"].CompletedGroupIterations==2&&Group().GroupBreak!.Right!.Constant!.Value.GetInt32()==16777217,"Save and reopen retains exact int32 literal and execution behavior");
  app.Shutdown();Console.WriteLine($"PASS {checks} Group int32 precision checks");
 }
 static void Pump(){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);}
 static IEnumerable<FrameworkElement> Children(DependencyObject root){if(root is FrameworkElement e)yield return e;for(int i=0;i<System.Windows.Media.VisualTreeHelper.GetChildrenCount(root);i++)foreach(var child in Children(System.Windows.Media.VisualTreeHelper.GetChild(root,i)))yield return child;}
}
sealed class RoundModule(ModuleDescriptor descriptor):IModule{public ModuleDescriptor Descriptor=>descriptor;public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"value",context.Inputs["index"]}}));}
