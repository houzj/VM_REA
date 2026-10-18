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
  int checks=0;void Check(bool ok,string label){if(!ok)throw new Exception(label);Console.WriteLine("PASS "+label);checks++;}
  var registry=new ModuleRegistry();WorkflowCalls.Register(registry);
  foreach(var version in new[]{"1","2"}){var d=new ModuleDescriptor("test.same-tool",version,"同类工具",[],[new("out",DataKind.Integer)],[new("gain",ParameterKind.Integer,false,JsonSerializer.SerializeToElement(1))]);registry.Register(d,()=>new GainModule(d));}
  var body=new ProcedureDocument("body","Group",[new("first","test.same-tool","1","工具v1",new(){["gain"]=JsonSerializer.SerializeToElement(7)}),new("second","test.same-tool","2","工具v2",new(){["gain"]=JsonSerializer.SerializeToElement(1)})],[],[]){GroupOwner=new("main","group"),Configuration=new(){RunCategories=[new("category","分组0")],RunParameters=[new("proxy","first","gain","gain",false){CategoryId="category"}],Outputs=[new("output","out",DataKind.Integer,"second","out")]}};
  var group=new NodeDocument("group",WorkflowCalls.ModuleId,WorkflowCalls.Version,"Group",new()){Call=WorkflowCalls.From(body)};
  var path=Path.GetFullPath("artifacts/group-cross-version-relation.vmx.json");Directory.CreateDirectory(Path.GetDirectoryName(path)!);SolutionStore.Save(path,new(SolutionStore.CurrentVersion,"relation","relation",[new("main","main",[group],[],[]),body],[]));
  var editor=new GraphEditorDocument(registry);editor.Load(path);ProcedureDocument Body()=>editor.Document.Procedures.Single(p=>p.Id=="body");NodeDocument Group()=>editor.Document.Procedures.Single(p=>p.Id=="main").Nodes.Single();
  long Run()=>Task.Run(()=>new SolutionExecutor(registry).RunAsync(new(editor.Document,editor.BaseDirectory),"main")).GetAwaiter().GetResult().Calls["group"].WorkflowOutputs["out"].As<long>();
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
  var window=new SubprogramCallWindow(()=>editor.Document.Procedures.Single(p=>p.Id=="main"),Group,()=>editor.Document.Procedures,_=>{},(_,_)=>{},_=>[],()=>Task.CompletedTask,()=>{},()=>null,
    saveGroupConfiguration:c=>editor.SetWorkflowConfiguration("body",c),groupRegistry:registry,setGroupProxyValue:(id,value)=>editor.SetGroupRunParameterValue("body",id,value))
   {ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};window.Show();Pump();
  var config=window.CreateRunConfigurationWindow();config.Show();Pump();
  Exception? pickerError=null;window.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,new Action(()=>{WorkflowSourcePicker? picker=null;try{picker=Application.Current.Windows.OfType<WorkflowSourcePicker>().Single();var choice=Children(picker).OfType<CheckBox>().Single(c=>Equals(c.Content,"gain"));choice.IsChecked=true;Children(picker).OfType<Button>().Single(b=>Equals(b.Content,"确定")).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));}catch(Exception e){pickerError=e;picker?.Close();}}));
  Children(config).OfType<Button>().Single(b=>Equals(b.Tag,"GroupProxyRelations:proxy")).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));Pump();if(pickerError is not null)throw new Exception("Picker: "+pickerError);
  Check(Body().Configuration.RunParameters.Single().Relations?.Single().NodeId=="second","Actual Group relation picker accepts same tool type across module versions");
  Check(Run()==1,"Before sync actual Group output still comes from version 2 target value 1");
  Children(config).OfType<Button>().Single(b=>Equals(b.Tag,"GroupProxySync:proxy")).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));Pump();
  Check(Body().Nodes.Single(n=>n.Id=="second").Parameters["gain"].GetInt32()==7&&Run()==7,"WPF sync writes version 1 value into compatible version 2 target and real Group output");
  editor.Undo();Check(Body().Nodes.Single(n=>n.Id=="second").Parameters["gain"].GetInt32()==1&&Run()==1,"Undo restores version 2 target and actual data");editor.Redo();Check(Run()==7,"Redo restores synced value and actual Group data");
  config.Close();window.Close();editor.Save(path);editor.Load(path);
  Check(Body().Configuration.RunParameters.Single().Relations?.Single().NodeId=="second"&&Run()==7,"Save and reopen retains cross-version relation and synced Group output");
  app.Shutdown();Console.WriteLine($"PASS {checks} Group cross-version relation checks");
 }
 static void Pump(){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);}
 static IEnumerable<FrameworkElement> Children(DependencyObject root){if(root is FrameworkElement e)yield return e;for(int i=0;i<System.Windows.Media.VisualTreeHelper.GetChildrenCount(root);i++)foreach(var child in Children(System.Windows.Media.VisualTreeHelper.GetChild(root,i)))yield return child;}
}
sealed class GainModule(ModuleDescriptor descriptor):IModule{public ModuleDescriptor Descriptor=>descriptor;public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token){var value=context.Parameters["gain"].GetInt32();return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"out",PortValue.Integer(value)}}));}}
