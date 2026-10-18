using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;

internal static class Program
{
 [STAThread] static void Main()
 {
  var checks=0;void Check(bool value,string label){if(!value)throw new Exception(label);Console.WriteLine("PASS "+label);checks++;}
  var registry=new ModuleRegistry();WorkflowCalls.Register(registry);var indices=new List<long>();
  var descriptor=new ModuleDescriptor("test.group.gap","1","索引",[new("index",DataKind.Integer)],[new("out",DataKind.Integer)],[]);
  registry.Register(descriptor,()=>new IndexModule(descriptor,indices));
  var body=new ProcedureDocument("body","Group",[new("probe",descriptor.Id,"1","probe",new())],[],[]){GroupOwner=new("main","group"),Configuration=new(){Subscriptions=[new("probe","index",false,new(WorkflowSourceKind.GroupLoopIndex,"LoopIndex"))],Outputs=[new("out","out",DataKind.Integer,"probe","out")]}};
  var group=new NodeDocument("group",WorkflowCalls.ModuleId,WorkflowCalls.Version,"Group",new(){["EnableLoop"]=JsonSerializer.SerializeToElement(true),["StartIndex"]=JsonSerializer.SerializeToElement(0),["EndIndex"]=JsonSerializer.SerializeToElement(3),["LoopTimeGap"]=JsonSerializer.SerializeToElement(0)}){Call=WorkflowCalls.From(body)};
  var path=Path.GetFullPath("artifacts/group-loop-gap.vmx.json");Directory.CreateDirectory(Path.GetDirectoryName(path)!);SolutionStore.Save(path,new(SolutionStore.CurrentVersion,"gap","gap",[new("main","main",[group],[],[]),body],[]));
  var editor=new GraphEditorDocument(registry);editor.Load(path);NodeDocument Group()=>editor.Document.Procedures[0].Nodes.Single();
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
  var window=new SubprogramCallWindow(()=>editor.Document.Procedures[0],Group,()=>editor.Document.Procedures,_=>{},(_,_)=>{},_=>[],()=>Task.CompletedTask,()=>{},()=>null,(key,value)=>editor.SetParameter("group",key,value)){ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};window.Show();Pump();
  var tabs=Children(window).OfType<TabControl>().Single(t=>t.Items.Count==5);tabs.SelectedIndex=4;Pump();
  var slider=Children(window).OfType<Slider>().Single(s=>Equals(s.Tag,"GroupLoopTimeGapSlider"));var input=Children(window).OfType<TextBox>().Single(t=>Equals(t.Tag,"GroupLoopTimeGapValue"));
  Check(slider.Minimum==0&&slider.Maximum==10000&&slider.TickFrequency==1&&slider.IsSnapToTickEnabled,"WPF Group interval exposes XML Integer slider bounds and unit ticks");
  slider.Value=500;Pump();Check(input.Text=="500","Slider value updates visible numeric value during drag");
  slider.RaiseEvent(new DragCompletedEventArgs(0,0,false){RoutedEvent=Thumb.DragCompletedEvent});
  slider.RaiseEvent(new KeyboardFocusChangedEventArgs(Keyboard.PrimaryDevice,0,slider,null){RoutedEvent=Keyboard.LostKeyboardFocusEvent});Pump();
  Check(Group().Parameters["LoopTimeGap"].GetInt32()==500,"Actual WPF slider commits Group interval");
  editor.Undo();Check(Group().Parameters["LoopTimeGap"].GetInt32()==0,"Undo restores prior interval");editor.Redo();Check(Group().Parameters["LoopTimeGap"].GetInt32()==500,"Redo restores slider interval");
  window.Close();editor.Save(path);editor.Load(path);Check(Group().Parameters["LoopTimeGap"].GetInt32()==500,"Saved solution reopens with slider interval");
  var timed=Group() with{TimeoutMs=150};var flow=editor.Document.Procedures[0] with{Nodes=[timed]};var solution=editor.Document with{Procedures=[flow,editor.Document.Procedures[1]]};
  var result=Task.Run(()=>new SolutionExecutor(registry).RunAsync(new(solution,editor.BaseDirectory),"main")).GetAwaiter().GetResult();
  Console.WriteLine($"Runtime status={result.Status}; indices={string.Join(',',indices)}");Check(result.Status==SessionStatus.TimedOut&&indices.SequenceEqual(new long[]{0}),"Actual Group data run waits at interval and times out before next index");
  app.Shutdown();Console.WriteLine($"PASS {checks} Group loop gap checks");
 }
 static void Pump(){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);}
 static IEnumerable<FrameworkElement> Children(DependencyObject root){if(root is FrameworkElement e)yield return e;for(int i=0;i<System.Windows.Media.VisualTreeHelper.GetChildrenCount(root);i++)foreach(var child in Children(System.Windows.Media.VisualTreeHelper.GetChild(root,i)))yield return child;}
}
sealed class IndexModule(ModuleDescriptor descriptor,List<long> indices):IModule{public ModuleDescriptor Descriptor=>descriptor;public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token){var index=context.Inputs["index"].As<long>();indices.Add(index);return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"out",PortValue.Integer(index)}}));}}
