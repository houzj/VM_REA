using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using VisionEditor;
using VisionFramework;

internal static class Program
{
[STAThread]
private static void Main()
{
int checks=0;
void Check(bool ok,string name){if(!ok)throw new Exception(name);checks++;Console.WriteLine("PASS "+name);}
var registry=new ModuleRegistry();WorkflowCalls.Register(registry);
var producer=new ModuleDescriptor("test.variable.producer","1","源",[],[new("source",DataKind.Integer)],[]);
var consumer=new ModuleDescriptor("test.variable.consumer","1","消费",[new("value",DataKind.Integer)],[new("result",DataKind.Integer)],[]);
registry.Register(producer,()=>new Producer(producer));registry.Register(consumer,()=>new Consumer(consumer));
var body=new ProcedureDocument("body","Group",[new("producer",producer.Id,"1","producer",new()),new("consumer",consumer.Id,"1","consumer",new())],[],[new("producer","consumer")]){
 GroupOwner=new("parent","group"),Configuration=new(){Groups=[new("g","分组0")],Variables=[new("v","g","%v%","int","2")],Subscriptions=[new("consumer","value",false,new(WorkflowSourceKind.LocalVariable,"v"))],Outputs=[new("o","result",DataKind.Integer,"consumer","result")]}
};
var parent=new ProcedureDocument("parent","流程",[new("group",WorkflowCalls.ModuleId,WorkflowCalls.Version,"Group",new()){Call=WorkflowCalls.From(body)}],[],[]);
var solution=new SolutionDocument(SolutionStore.CurrentVersion,"variable-binding","变量绑定",[parent,body],[]);
ProcedureDocument Body()=>solution.Procedures.Single(p=>p.Id=="body");
void Save(WorkflowConfiguration c)=>solution=solution with{Procedures=solution.Procedures.Select(p=>p.Id=="body"?p with{Configuration=c}:p).ToArray()};
IEnumerable<WorkflowSourceOption> Sources(DataKind? kind)=>Body().Nodes.SelectMany(n=>registry.GetDescriptor(n).Outputs.Where(o=>kind is null||o.Kind==kind.Value).Select(o=>new WorkflowSourceOption(n.Id,o.Name,o.Kind,n.Name+"."+o.Name){Cardinality=o.Cardinality}));
long Run(){var r=new SolutionExecutor(registry).RunAsync(new(solution,Environment.CurrentDirectory),"parent").GetAwaiter().GetResult();Check(r.Status==SessionStatus.Success,"Parent and Group execute");return r.Calls["group"].WorkflowOutputs["result"].As<long>();}
Check(Run()==2,"Group consumes local initial value before binding");
var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
var window=new WorkflowLocalVariablesWindow(Body,Save,Sources,(_,_)=>null,registry){ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};window.Show();Pump();
Exception? pickerError=null;
_ =window.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,new Action(()=>{WorkflowSourcePicker? picker=null;try{
 picker=Application.Current.Windows.OfType<WorkflowSourcePicker>().Single();picker.UpdateLayout();var labels=Children(picker).OfType<CheckBox>().Select(c=>c.Content?.ToString()).ToArray();Console.WriteLine("Picker: "+string.Join(" | ",labels));
 Check(labels.Contains("source"),"Source picker shows preceding producer");
 Check(!labels.Contains("result"),"Source picker excludes consumer output that would create a cycle");
 Children(picker).OfType<CheckBox>().Single(c=>Equals(c.Content,"source")).IsChecked=true;
 Children(picker).OfType<Button>().Single(b=>Equals(b.Content,"确定")).RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
}catch(Exception e){pickerError=e;picker?.Close();}}));
Children(window).OfType<Button>().Single(b=>Equals(b.Content,"------")).RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));Pump();
if(pickerError is not null)throw pickerError;
Check(Body().Configuration.Variables.Single().SourceNode=="producer"&&Body().Configuration.Variables.Single().SourcePort=="source","Actual WPF picker commits Group local variable source");
Check(Run()==7,"Bound producer value participates in actual Group execution");
var bad=Body() with{Configuration=Body().Configuration with{Variables=[Body().Configuration.Variables.Single() with{SourceNode="consumer",SourcePort="result"}]}};
var badIssues=GraphValidator.Validate(bad,registry).Issues;Console.WriteLine("Invalid source: "+string.Join(" | ",badIssues.Select(i=>i.Code+":"+i.Message)));
Check(badIssues.Any(i=>i.Code is "Cycle" or "DataExecutionOrder"),"Runtime graph validation rejects a manually introduced variable cycle");
var path=Path.GetFullPath("artifacts/group-variable-binding.vmx.json");Directory.CreateDirectory(Path.GetDirectoryName(path)!);SolutionStore.Save(path,solution);solution=SolutionStore.Load(path);
Check(Body().Configuration.Variables.Single().SourceNode=="producer","Source binding survives save and reopen");
Check(Run()==7,"Reopened Group executes bound source");
window.Close();app.Shutdown();Console.WriteLine($"PASS {checks} Group variable binding checks");
}
static void Pump(){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);}
static IEnumerable<FrameworkElement> Children(DependencyObject root){if(root is FrameworkElement e)yield return e;for(int i=0;i<System.Windows.Media.VisualTreeHelper.GetChildrenCount(root);i++)foreach(var child in Children(System.Windows.Media.VisualTreeHelper.GetChild(root,i)))yield return child;}
}
sealed class Producer(ModuleDescriptor descriptor):IModule{public ModuleDescriptor Descriptor=>descriptor;public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"source",PortValue.Integer(7)}}));}
sealed class Consumer(ModuleDescriptor descriptor):IModule{public ModuleDescriptor Descriptor=>descriptor;public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"result",PortValue.Integer(context.Input<long>("value"))}}));}
