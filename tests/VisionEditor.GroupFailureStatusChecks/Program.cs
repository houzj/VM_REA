using System.IO;
using System.Reflection;
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
  int checks=0;void Check(bool ok,string name){if(!ok)throw new Exception(name);Console.WriteLine("PASS "+name);checks++;}
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};var main=new MainWindow{ShowActivated=false,ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(main.Dispatcher));main.Show();Pump();
  var type=typeof(MainWindow);var doc=(GraphEditorDocument)type.GetField("doc",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(main)!;
  doc.NewSolution();var flow=doc.CurrentProcedureId;var id=doc.AddNode(WorkflowCalls.ModuleId,WorkflowCalls.Version,100,100);doc.EnterOwnedGroup(id);var body=doc.CurrentProcedureId;
  doc.SetWorkflowConfiguration(body,doc.CurrentProcedure.Configuration with{Inputs=[new("in","in",DataKind.Integer)]});doc.SelectProcedure(flow);
  void Bind(bool enabled){var config=doc.CurrentProcedure.Configuration;doc.SetWorkflowConfiguration(flow,config with{Groups=[new("vars","分组0")],Variables=[new("value","vars","value","int","3")],Subscriptions=enabled?[new(id,"in",false,new(WorkflowSourceKind.LocalVariable,"value"))]:[]});}
  void BindConstant(){var config=doc.CurrentProcedure.Configuration;doc.SetWorkflowConfiguration(flow,config with{Subscriptions=[new(id,"in",false,new(WorkflowSourceKind.IntegerConstant,"3"))]});}
  BindConstant();Check(doc.CurrentProcedure.Nodes.Single(n=>n.Id==id).Call?.Inputs.Any(p=>p.Name=="in")==true,"Owned Group exposes a real required input");
  var create=type.GetMethod("CreateSubprogramCallWindow",BindingFlags.Instance|BindingFlags.NonPublic)!;var window=(SubprogramCallWindow)create.Invoke(main,[id])!;window.WindowStartupLocation=WindowStartupLocation.Manual;window.Left=window.Top=-20000;window.Show();Pump();
  var run=(Button)window.FindName("RunOnce");var status=(TextBlock)main.FindName("StatusText");var error=(TextBlock)window.FindName("Error");
  void Execute(){run.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));for(var i=0;i<500&&!run.IsEnabled;i++){Pump();Thread.Sleep(5);}Pump();if(!run.IsEnabled)throw new Exception("Group WPF run timeout");}
  var label=Children(window).OfType<Button>().Single(b=>Equals(b.Tag,"GroupInputSource:in"));Check(Equals(label.Content,"3"),"Actual Group WPF input label shows native-style integer constant value");
  Execute();var sessions=(Dictionary<string,ExecutionSessionResult>)type.GetField("workflowSessions",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(main)!;Check(sessions[flow].Calls[id].WorkflowInputs["in"].As<long>()==3,"Integer constant passes actual value 3 into owned Group run");
  var snapshotPath=Path.GetFullPath("artifacts/group-constant-input.vmx.json");Directory.CreateDirectory(Path.GetDirectoryName(snapshotPath)!);SolutionStore.Save(snapshotPath,doc.Document);var restored=SolutionStore.Load(snapshotPath);var restoredRun=Task.Run(()=>new GroupDebugExecutor((ModuleRegistry)type.GetField("modules",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(main)!).RunAsync(new(restored,Path.GetDirectoryName(snapshotPath)!),flow,id)).GetAwaiter().GetResult();Check(restoredRun.Calls[id].WorkflowInputs["in"].As<long>()==3,"Saved and reopened Group keeps integer constant subscription and actual runtime value");
  Bind(true);
  Execute();Check(status.Text.Contains("Success"),"Successful Group SelfRun updates main status");
  Bind(false);Execute();Check(status.Text.Contains("失败")&&status.Text.Contains("in"),"Missing first-run input replaces stale success status with failure detail");Check(error.Text.Contains("in"),"Group editor shows same missing-input error");
  Bind(true);Execute();Check(status.Text.Contains("Success")&&!status.Text.Contains("失败")&&error.Text.Length==0,"Restored input clears error and actual Group SelfRun succeeds");
  var registry=(ModuleRegistry)type.GetField("modules",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(main)!;var slow=new ModuleDescriptor("test.group.timeout-status","1","慢工具",[],[],[]);registry.Register(slow,()=>new SlowModule(slow));doc.EnterOwnedGroup(id);var slowId=doc.AddNode(slow.Id,"1",200,100);doc.SetTimeout(slowId,1);doc.SelectProcedure(flow);
  Execute();Check(status.Text.Contains("Failed")&&error.Text.Contains("TimedOut"),"Timed-out internal tool replaces prior successful Group status");
  doc.EnterOwnedGroup(id);doc.SetTimeout(slowId,1000);doc.SelectProcedure(flow);Execute();Check(status.Text.Contains("Success")&&error.Text.Length==0,"Group timeout recovery clears failure and runs actual body");
  window.Close();main.Close();app.Shutdown();Console.WriteLine($"PASS {checks} Group failure status checks");
 }
 static void Pump(){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);}
 static IEnumerable<FrameworkElement> Children(DependencyObject root){if(root is FrameworkElement e)yield return e;for(int i=0;i<System.Windows.Media.VisualTreeHelper.GetChildrenCount(root);i++)foreach(var child in Children(System.Windows.Media.VisualTreeHelper.GetChild(root,i)))yield return child;}
}
sealed class SlowModule(ModuleDescriptor descriptor):IModule{public ModuleDescriptor Descriptor=>descriptor;public async ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token){await Task.Delay(35,token);return new(ModuleStatus.Success,new Dictionary<string,PortValue>());}}
