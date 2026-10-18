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
[STAThread]
private static void Main()
{
int checks=0;void Check(bool value,string name){if(!value)throw new Exception(name);checks++;Console.WriteLine("PASS "+name);}
var registry=new ModuleRegistry();WorkflowCalls.Register(registry);
var array=new ModuleDescriptor("test.break.array","1","数组",[],[new("values",DataKind.Integer){Cardinality=ValueCardinality.Array}],[]);
var value=new ModuleDescriptor("test.break.value","1","输出",[],[new("value",DataKind.Integer)],[]);
registry.Register(array,()=>new ArrayModule(array));registry.Register(value,()=>new ValueModule(value));
var body=new ProcedureDocument("body","Group",[new("value",value.Id,"1","value",new())],[],[]){GroupOwner=new("parent","group"),Configuration=new(){Groups=[new("inner-g","分组0")],Variables=[new("inner","inner-g","%inner%","int","5")],Outputs=[new("o","out",DataKind.Integer,"value","value")]}};
var group=new NodeDocument("group",WorkflowCalls.ModuleId,WorkflowCalls.Version,"Group",new(){["EnableLoop"]=JsonSerializer.SerializeToElement(true),["StartIndex"]=JsonSerializer.SerializeToElement(0),["EndIndex"]=JsonSerializer.SerializeToElement(2),["EnableBreak"]=JsonSerializer.SerializeToElement(true),["DataType"]=JsonSerializer.SerializeToElement(1),["CmpType"]=JsonSerializer.SerializeToElement(1)}){
 Call=WorkflowCalls.From(body),GroupBreak=new(new(false,new(WorkflowSourceKind.NodeOutput,"array","values")){IndexSource=new(WorkflowSourceKind.LocalVariable,"idx")},new(false,new(WorkflowSourceKind.LocalVariable,"limit")))
};
var parent=new ProcedureDocument("parent","流程",[new("array",array.Id,"1","array",new()),group],[],[new("array","group")]){Configuration=new(){Groups=[new("g","分组0")],Variables=[new("idx","g","%idx%","int","0"),new("limit","g","%limit%","int","5")]}};
var seed=new SolutionDocument(SolutionStore.CurrentVersion,"break-cleanup","中断引用清理",[parent,body],[]);
var path=Path.GetFullPath("artifacts/group-variable-break.vmx.json");Directory.CreateDirectory(Path.GetDirectoryName(path)!);SolutionStore.Save(path,seed);
var editor=new GraphEditorDocument(registry);editor.Load(path);
NodeDocument Group()=>editor.Document.Procedures.Single(p=>p.Id=="parent").Nodes.Single(n=>n.Id=="group");
ProcedureDocument Parent()=>editor.Document.Procedures.Single(p=>p.Id=="parent");
ProcedureDocument Body()=>editor.Document.Procedures.Single(p=>p.Id=="body");
IEnumerable<WorkflowSourceOption> Sources(DataKind? kind)=>Parent().Nodes.SelectMany(n=>registry.GetDescriptor(n).Outputs.Where(o=>kind is null||o.Kind==kind.Value).Select(o=>new WorkflowSourceOption(n.Id,o.Name,o.Kind,n.Name+"."+o.Name){Cardinality=o.Cardinality}));
ExecutionSessionResult Run()=>new SolutionExecutor(registry).RunAsync(new(editor.Document,editor.BaseDirectory),"parent").GetAwaiter().GetResult();
Check(Run().Status==SessionStatus.Success,"Original Group break references run successfully");
var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
WorkflowLocalVariablesWindow Open(){var w=new WorkflowLocalVariablesWindow(Parent,c=>editor.SetWorkflowConfiguration("parent",c),Sources,(_,_)=>null,registry){ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};w.Show();Pump();return w;}
var window=Open();
Children(window).OfType<Button>().Where(b=>Equals(b.Content,"×")).First(b=>Equals(b.DataContext?.GetType().GetProperty("Id")?.GetValue(b.DataContext),"idx")).RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));Pump();
Check(Parent().Configuration.Variables.All(v=>v.Id!="idx"),"Actual WPF delete removes index variable");
Check(Group().GroupBreak?.Left?.IndexSource is null&&Group().GroupBreak?.Left?.Index==0,"Same transaction clears Group break dynamic index and retains fallback index");
Check(Run().Status==SessionStatus.Success,"Group runs with fixed index after source variable deletion");
editor.Undo();Check(Parent().Configuration.Variables.Any(v=>v.Id=="idx")&&Group().GroupBreak?.Left?.IndexSource?.Id=="idx","One undo restores variable and dynamic index together");
editor.Redo();Check(Parent().Configuration.Variables.All(v=>v.Id!="idx")&&Group().GroupBreak?.Left?.IndexSource is null,"One redo removes variable and index binding together");
window.Close();window=Open();
Children(window).OfType<ComboBox>().Single(c=>c.ItemsSource is string[] items&&items.Contains("float")).SelectedItem="float";Pump();Pump();
Check(Parent().Configuration.Variables.Single().Type=="float","Actual WPF type edit changes remaining variable");
Check(Group().GroupBreak?.Right is null,"Type change clears Group break operand referencing previous variable type");
editor.Undo();Check(Parent().Configuration.Variables.Single().Type=="int"&&Group().GroupBreak?.Right?.Source?.Id=="limit","One undo restores type and break reference together");
editor.Redo();Check(Parent().Configuration.Variables.Single().Type=="float"&&Group().GroupBreak?.Right is null,"One redo clears both again");
var unbound=Run();
Check(unbound.Status==SessionStatus.Success&&unbound.Calls["group"].GroupDiagnostics.Any(d=>d.Contains("unbound",StringComparison.OrdinalIgnoreCase)),"Unbound break operand reports diagnostic and does not read removed variable");
editor.SetGroupBreak("group",true,1,1,Group().GroupBreak! with{Right=new(Constant:JsonSerializer.SerializeToElement(5))});
Check(Run().Status==SessionStatus.Success,"Rebinding break operand restores Group execution");
editor.Save(path);editor.Load(path);
Check(Parent().Configuration.Variables.Single().Type=="float"&&Group().GroupBreak?.Left?.IndexSource is null&&Group().GroupBreak?.Right?.Constant?.GetInt32()==5,"Saved Group retains pruned and rebound break references");
Check(Run().Status==SessionStatus.Success,"Reopened Group executes with corrected break operands");
editor.SelectProcedure("parent");editor.SetGroupBreak("group",true,1,1,Group().GroupBreak! with{Right=new(true,new(WorkflowSourceKind.LocalVariable,"inner"))});
Check(Run().Status==SessionStatus.Success,"Group can read its own private local variable as an internal break operand");
window.Close();
window=new WorkflowLocalVariablesWindow(Body,c=>editor.SetWorkflowConfiguration("body",c),kind=>Body().Nodes.SelectMany(n=>registry.GetDescriptor(n).Outputs.Where(o=>kind is null||o.Kind==kind.Value).Select(o=>new WorkflowSourceOption(n.Id,o.Name,o.Kind,n.Name+"."+o.Name))),(_,_)=>null,registry){ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};window.Show();Pump();
Children(window).OfType<Button>().Where(b=>Equals(b.Content,"×")).First(b=>Equals(b.DataContext?.GetType().GetProperty("Id")?.GetValue(b.DataContext),"inner")).RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));Pump();
Check(Body().Configuration.Variables.Count==0&&Group().GroupBreak?.Right is null,"Deleting Group-private variable clears owning parent break reference in one transaction");
editor.Undo();Check(Body().Configuration.Variables.Single().Id=="inner"&&Group().GroupBreak?.Right?.Source?.Id=="inner","One undo restores private variable and parent break reference");
editor.Redo();Check(Body().Configuration.Variables.Count==0&&Group().GroupBreak?.Right is null,"One redo clears private variable and parent break reference");
window.Close();
editor.SelectProcedure("parent");editor.SetGroupBreak("group",true,1,1,Group().GroupBreak! with{Right=new(Constant:JsonSerializer.SerializeToElement(5))});editor.Save(path);editor.Load(path);
Check(Body().Configuration.Variables.Count==0&&Group().GroupBreak?.Right?.Constant?.GetInt32()==5&&Run().Status==SessionStatus.Success,"Private variable removal and corrected break survive save and reopen");
app.Shutdown();Console.WriteLine($"PASS {checks} Group variable break cleanup checks");
}
static void Pump(){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);}
static IEnumerable<FrameworkElement> Children(DependencyObject root){if(root is FrameworkElement e)yield return e;for(int i=0;i<System.Windows.Media.VisualTreeHelper.GetChildrenCount(root);i++)foreach(var child in Children(System.Windows.Media.VisualTreeHelper.GetChild(root,i)))yield return child;}
}
sealed class ArrayModule(ModuleDescriptor descriptor):IModule{public ModuleDescriptor Descriptor=>descriptor;public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"values",PortValue.IntegerArray([2,3])}}));}
sealed class ValueModule(ModuleDescriptor descriptor):IModule{public ModuleDescriptor Descriptor=>descriptor;public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"value",PortValue.Integer(7)}}));}
