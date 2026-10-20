using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
static class Program
{
 [STAThread] static int Main(string[] args){
  AppContext.SetSwitch("VisionEditor.DisableAutomaticCameraDiscovery",true);string? output=null;
  try{
   if(args.Length!=1)throw new ArgumentException("Supply owned output parent.");output=Path.Combine(Path.GetFullPath(args[0]),"callback-before-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
   var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
   var window=new MainWindow();const BindingFlags flags=BindingFlags.NonPublic|BindingFlags.Instance;
   object? Field(string name)=>typeof(MainWindow).GetField(name,flags)?.GetValue(window);
   var doc=(GraphEditorDocument)Field("doc")!;var modules=(ModuleRegistry)Field("modules")!;
   var descriptor=new ModuleDescriptor("test.callback-before","1","callback-before",[],[new("Value",DataKind.Number)],[]);modules.Register(descriptor,()=>new ProbeModule(descriptor));doc.NewSolution();string node=doc.AddNode(descriptor.Id,"1",100,100);
   var run=(Task)typeof(MainWindow).GetMethod("StartRunCore",flags)!.Invoke(window,[false,null])!;var watch=Stopwatch.StartNew();while(!run.IsCompleted&&watch.Elapsed<TimeSpan.FromSeconds(25)){var f=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>f.Continue=false));Dispatcher.PushFrame(f);Thread.Sleep(1);}if(!run.IsCompleted)throw new TimeoutException();run.GetAwaiter().GetResult();
   var sessions=(Dictionary<string,ExecutionSessionResult>)Field("workflowSessions")!;double result=sessions[doc.CurrentProcedureId].Nodes.Single(x=>x.NodeId==node).Outputs["Value"].As<double>();
   bool queueAbsent=typeof(MainWindow).GetField("frameworkCallbacks",flags) is null;
   var observation=new{result,queueAbsent,note="Absence observation only; no overflow behavior is claimed to have executed in old product."};File.WriteAllText(Path.Combine(output,"observation.json"),JsonSerializer.Serialize(observation,new JsonSerializerOptions{WriteIndented=true}));
   if(result!=2||!queueAbsent)throw new InvalidOperationException("Expected actual old host output and absent callback integration seam.");window.Close();app.Shutdown();File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=2,observation}));Console.WriteLine(output);return 0;
  }catch(Exception error){if(output is not null)File.WriteAllText(Path.Combine(output,"failed.json"),JsonSerializer.Serialize(new{error=error.ToString()}));Console.Error.WriteLine(error);return 1;}
 }
}
sealed class ProbeModule(ModuleDescriptor descriptor):IModule{
 public ModuleDescriptor Descriptor=>descriptor;
 public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"Value",PortValue.Number(2)}}));
}
