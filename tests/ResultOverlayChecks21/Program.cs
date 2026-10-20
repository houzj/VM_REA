using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;

static class Program
{
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 [STAThread] static int Main(string[] args)
 {
  AppContext.SetSwitch("VisionEditor.DisableAutomaticCameraDiscovery",true);
  string? output=null;
  try{
   if(args.Length<1)throw new ArgumentException("Supply an owned audit directory and optional observe-unfixed mode.");
   bool before=args.Skip(1).Contains("observe-unfixed");
   output=Path.Combine(Path.GetFullPath(args[0]),"result-overlay-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
   var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
   var checks=new List<string>();void Check(bool value,string label){if(!value)throw new Exception(label);checks.Add(label);Console.WriteLine("PASS "+label);}
   var window=new MainWindow();object? Field(string name)=>typeof(MainWindow).GetField(name,Private)!.GetValue(window);
   object? Call(string name,params object[] values)=>typeof(MainWindow).GetMethod(name,Private)!.Invoke(window,values);
   var doc=(GraphEditorDocument)Field("doc")!;var modules=(ModuleRegistry)Field("modules")!;
   double current=1;var descriptor=new ModuleDescriptor("test.result-overlay","1","result-overlay",[],[new("Value",DataKind.Number)],[]);
   modules.Register(descriptor,()=>new ValueModule(descriptor,()=>current));doc.NewSolution();string procedure=doc.CurrentProcedureId;string node=doc.AddNode(descriptor.Id,"1",100,100);
   var context=(ToolEditorContext)Call("CreateToolEditorContext",node)!;PumpUntil(context.Run(new()));
   var results=(Dictionary<string,NodeRunRecord>)Field("results")!;
   double debugVisible=results[node].Outputs["Value"].As<double>();
   Check(debugVisible==1,"Actual debug context publishes visible value 1");
   current=2;PumpUntil((Task)Call("StartRunCore",false,null!)!);
   var sessions=(Dictionary<string,ExecutionSessionResult>)Field("workflowSessions")!;
   double sessionValue=sessions[procedure].Nodes.Single(x=>x.NodeId==node).Outputs["Value"].As<double>();
   double visibleValue=results[node].Outputs["Value"].As<double>();
   var inputs=(IReadOnlyList<NodeRunRecord>)Call("ToolDebugInputs",procedure)!;
   double nextDebugInput=inputs.Single(x=>x.NodeId==node).Outputs["Value"].As<double>();
   int debugCount=((IDictionary)Field("toolDebugRecords")!).Count;
   var observation=new{before,debugVisible,sessionValue,visibleValue,nextDebugInput,debugCount};
   File.WriteAllText(Path.Combine(output,"observation.json"),JsonSerializer.Serialize(observation,new JsonSerializerOptions{WriteIndented=true}));
   Check(sessionValue==2,"Complete StartRun publishes actual session value 2");
   Check(visibleValue==(before?1:2),before?"Observed stale debug overlay hides complete workflow result":"Complete workflow result replaces stale visible debug overlay");
   Check(nextDebugInput==(before?1:2),before?"Observed next debug consumer reads stale value":"Next debug consumer reads latest complete workflow result");
   Check(debugCount==(before?1:0),before?"Observed stale debug record retained":"Complete workflow publication removes obsolete debug record");
   window.Close();app.Shutdown();
   File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=checks.Count,checks,observation},new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine(output);return 0;
  }catch(Exception error){if(output is not null)File.WriteAllText(Path.Combine(output,"failed.json"),JsonSerializer.Serialize(new{error=error.ToString()},new JsonSerializerOptions{WriteIndented=true}));Console.Error.WriteLine(error);return 1;}
 }
 static void PumpUntil(Task task){var watch=Stopwatch.StartNew();while(!task.IsCompleted&&watch.Elapsed<TimeSpan.FromSeconds(25)){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);Thread.Sleep(1);}if(!task.IsCompleted)throw new TimeoutException("Owned fixture deadline exceeded.");task.GetAwaiter().GetResult();}
}
sealed class ValueModule(ModuleDescriptor descriptor,Func<double> value):IModule
{
 public ModuleDescriptor Descriptor=>descriptor;
 public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"Value",PortValue.Number(value())}}));
}
