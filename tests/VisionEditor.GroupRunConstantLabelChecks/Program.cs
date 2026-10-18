using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using VisionEditor;
using VisionFramework;

internal static class Program
{
 [STAThread] static void Main()
 {
  int checks=0;void Check(bool ok,string label){if(!ok)throw new Exception(label);Console.WriteLine("PASS "+label);checks++;}
  var registry=new ModuleRegistry();WorkflowCalls.Register(registry);var descriptor=new ModuleDescriptor("test.group.param-constant","1","参数工具",[],[new("value",DataKind.Integer)],[new("gain",ParameterKind.Integer,true,JsonSerializer.SerializeToElement(1))]);registry.Register(descriptor,()=>new GainModule(descriptor));
  var body=new ProcedureDocument("body","Group",[new("tool",descriptor.Id,"1","参数工具",new(){["gain"]=JsonSerializer.SerializeToElement(1)})],[],[]){GroupOwner=new("main","group"),Configuration=new(){RunCategories=[new("category","分组0")],RunParameters=[new("proxy","tool","gain","增益"){CategoryId="category"}],Subscriptions=[new("tool","gain",true,new(WorkflowSourceKind.IntegerConstant,"7"))],Outputs=[new("output","out",DataKind.Integer,"tool","value")]}};
  var group=new NodeDocument("group",WorkflowCalls.ModuleId,WorkflowCalls.Version,"Group",new()){Call=WorkflowCalls.From(body)};var path=Path.GetFullPath("artifacts/group-run-constant.vmx.json");Directory.CreateDirectory(Path.GetDirectoryName(path)!);SolutionStore.Save(path,new(SolutionStore.CurrentVersion,"constant","constant",[new("main","流程",[group],[],[]),body],[]));
  var workspace=SolutionStore.Load(path);NodeDocument Group()=>workspace.Procedures.Single(p=>p.Id=="main").Nodes.Single();
  long Run()=>Task.Run(()=>new GroupDebugExecutor(registry).RunAsync(new(workspace,Path.GetDirectoryName(path)!),"main","group")).GetAwaiter().GetResult().Calls["group"].WorkflowOutputs["out"].As<long>();
  Check(Run()==7,"Stored integer constant supplies real Group internal parameter and output");
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
  SubprogramCallWindow Open(){var w=new SubprogramCallWindow(()=>workspace.Procedures.Single(p=>p.Id=="main"),Group,()=>workspace.Procedures,_=>{},(_,_)=>{},_=>[],()=>Task.CompletedTask,()=>{},()=>null,groupSources:_=>[],groupRegistry:registry){ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};w.Show();Pump();((TabControl)w.FindName("ConfigurationTabs")).SelectedIndex=2;Pump();return w;}
  void Verify(SubprogramCallWindow w){var value=Children(w).OfType<TextBox>().Single(t=>Equals(t.Tag,"GroupRunValue:proxy"));Check(value.Text=="7"&&value.IsReadOnly,"Actual WPF Group run parameter shows constant Name instead of empty subscription");}
  var window=Open();Verify(window);window.Close();workspace=SolutionStore.Load(path);Check(Run()==7,"Saved and reopened Group retains constant parameter runtime value");window=Open();Verify(window);window.Close();app.Shutdown();Console.WriteLine($"PASS {checks} Group run constant label checks");
 }
 static void Pump(){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);}
 static IEnumerable<FrameworkElement> Children(DependencyObject root){if(root is FrameworkElement e)yield return e;for(int i=0;i<System.Windows.Media.VisualTreeHelper.GetChildrenCount(root);i++)foreach(var child in Children(System.Windows.Media.VisualTreeHelper.GetChild(root,i)))yield return child;}
}
sealed class GainModule(ModuleDescriptor descriptor):IModule{public ModuleDescriptor Descriptor=>descriptor;public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"value",PortValue.Integer(context.Parameters["gain"].GetInt32())}}));}
