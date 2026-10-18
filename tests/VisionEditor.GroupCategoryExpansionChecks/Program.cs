using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using VisionEditor;
using VisionFramework;

internal static class Program
{
 [STAThread] static void Main()
 {
  int checks=0;void Check(bool ok,string label){if(!ok)throw new Exception(label);Console.WriteLine("PASS "+label);checks++;}
  var registry=new ModuleRegistry();WorkflowCalls.Register(registry);var descriptor=new ModuleDescriptor("test.group.category-state","1","参数工具",[],[new("value",DataKind.Integer)],[new("gain",ParameterKind.Integer,true,JsonSerializer.SerializeToElement(1))]);registry.Register(descriptor,()=>new GainModule(descriptor));
  var body=new ProcedureDocument("body","Group",[new("tool",descriptor.Id,"1","参数工具",new(){["gain"]=JsonSerializer.SerializeToElement(7)})],[],[]){GroupOwner=new("main","group"),Configuration=new(){RunCategories=[new("category","分组0")],RunParameters=[new("proxy","tool","gain","增益"){CategoryId="category"}],Outputs=[new("output","out",DataKind.Integer,"tool","value")]}};
  var group=new NodeDocument("group",WorkflowCalls.ModuleId,WorkflowCalls.Version,"Group",new()){Call=WorkflowCalls.From(body)};var path=Path.GetFullPath("artifacts/group-category-expansion.vmx.json");Directory.CreateDirectory(Path.GetDirectoryName(path)!);var workspace=new SolutionDocument(SolutionStore.CurrentVersion,"category","category",[new("main","流程",[group],[],[]),body],[]);SolutionStore.Save(path,workspace);
  ProcedureDocument Body()=>workspace.Procedures.Single(p=>p.Id=="body");void Save(WorkflowConfiguration configuration){var current=Body();workspace=workspace with{Procedures=workspace.Procedures.Select(p=>p.Id=="body"?current with{Configuration=configuration}:p).ToArray()};}
  long Run()=>Task.Run(()=>new GroupDebugExecutor(registry).RunAsync(new(workspace,Path.GetDirectoryName(path)!),"main","group")).GetAwaiter().GetResult().Calls["group"].WorkflowOutputs["out"].As<long>();
  Check(Run()==7,"Real Group internal parameter produces data before category edit");
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
  var owner=new SubprogramCallWindow(()=>workspace.Procedures.Single(p=>p.Id=="main"),()=>workspace.Procedures.Single(p=>p.Id=="main").Nodes.Single(),()=>workspace.Procedures,_=>{},(_,_)=>{},_=>[],()=>Task.CompletedTask,()=>{},()=>null,saveGroupConfiguration:Save,groupSources:_=>[],groupRegistry:registry){ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};owner.Show();Pump();((TabControl)owner.FindName("ConfigurationTabs")).SelectedIndex=2;Pump();Check(Children(owner).OfType<Expander>().Single(e=>Equals(e.Header,"分组0")).IsExpanded,"Outer run-parameter tree starts expanded independently of config dialog");
  var config=owner.CreateRunConfigurationWindow();config.Show();Pump();
  Expander Category()=>Children(config).OfType<Expander>().Single(e=>Equals(e.Tag,"GroupCategorySelect:category"));
  Check(!Category().IsExpanded,"Native-bound config category starts collapsed");Category().IsExpanded=true;Pump();Check(Category().IsExpanded,"Actual WPF category can expand");Category().IsExpanded=false;Pump();Check(!Category().IsExpanded,"Actual WPF collapse changes category visual state");
  var name=Children(config).OfType<TextBox>().Single(t=>Equals(t.Tag,"GroupCategoryName:category"));name.Text="新类别";name.RaiseEvent(new KeyboardFocusChangedEventArgs(System.Windows.Input.Keyboard.PrimaryDevice,0,name,null){RoutedEvent=UIElement.LostKeyboardFocusEvent});Pump();Check(Body().Configuration.RunCategories!.Single().Name=="新类别"&&!Category().IsExpanded,"Category edit rebuild preserves collapsed state and writes data");
  var add=Children(config).OfType<Button>().Single(b=>Equals(b.Tag,"GroupCategoryAdd"));add.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));Pump();var added=Body().Configuration.RunCategories!.Single(c=>c.Id!="category");Check(Body().Configuration.RunCategories!.Count==2&&!Category().IsExpanded&&!Children(config).OfType<Expander>().Single(e=>Equals(e.Tag,"GroupCategorySelect:"+added.Id)).IsExpanded,"New config category starts collapsed while previous state survives");
  Save(Body().Configuration with{RunCategories=Body().Configuration.RunCategories!.Where(c=>c.Id=="category").ToArray()});config.Rebuild();Pump();Check(Body().Configuration.RunCategories!.Count==1&&!Category().IsExpanded,"Removing another category preserves the surviving category state");
  config.Close();config=owner.CreateRunConfigurationWindow();config.Show();Pump();Check(!Category().IsExpanded,"Reopening config in the same Group window retains collapsed category");Category().IsExpanded=true;Pump();config.Close();config=owner.CreateRunConfigurationWindow();config.Show();Pump();Check(Category().IsExpanded,"Reopening config also retains expanded category");config.Close();owner.Close();Check(Run()==7,"Category edit retains actual Group execution result");SolutionStore.Save(path,workspace);workspace=SolutionStore.Load(path);Check(Body().Configuration.RunCategories!.Single().Name=="新类别"&&Run()==7,"Saved and reopened category and Group run data remain intact");app.Shutdown();Console.WriteLine($"PASS {checks} Group category expansion checks");
 }
 static void Pump(){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);}
 static IEnumerable<FrameworkElement> Children(DependencyObject root){if(root is FrameworkElement e)yield return e;for(int i=0;i<System.Windows.Media.VisualTreeHelper.GetChildrenCount(root);i++)foreach(var child in Children(System.Windows.Media.VisualTreeHelper.GetChild(root,i)))yield return child;}
}
sealed class GainModule(ModuleDescriptor descriptor):IModule{public ModuleDescriptor Descriptor=>descriptor;public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"value",PortValue.Integer(context.Parameters["gain"].GetInt32())}}));}
