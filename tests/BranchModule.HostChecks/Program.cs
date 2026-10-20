using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.BranchModule;
static class Program {
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;static int checks;
 static T Field<T>(object value,string name)=>(T)value.GetType().GetField(name,Private)!.GetValue(value)!;
 static object? Call(object value,string name,params object[] args)=>value.GetType().GetMethod(name,Private)!.Invoke(value,args);
 static void Check(bool condition,string message){if(!condition)throw new Exception(message);Console.WriteLine($"PASS {++checks}: {message}");}
 static void Pump()=>Application.Current.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);
 static void Idle(MainWindow shell){Pump();var deadline=DateTime.UtcNow.AddSeconds(30);while(Field<int>(shell,"activeConfigurationRuns")>0){if(DateTime.UtcNow>deadline)throw new TimeoutException("Branch configuration run");var frame=new DispatcherFrame();var timer=new DispatcherTimer{Interval=TimeSpan.FromMilliseconds(20)};timer.Tick+=(_,_)=>{frame.Continue=false;timer.Stop();};timer.Start();Dispatcher.PushFrame(frame);}Pump();}
 [STAThread]static int Main(){MainWindow? shell=null;try{var app=new Application();shell=new MainWindow{ShowInTaskbar=false,ShowActivated=false};shell.Show();Pump();Run(shell);Console.WriteLine($"PASS {checks} actual Branch MainWindow host checks");return 0;}catch(Exception error){Console.Error.WriteLine(error);return 1;}finally{shell?.Close();Application.Current?.Shutdown();}}
 static void Run(MainWindow shell){
  var plugins=Field<ToolPluginRegistry>(shell,"plugins");var plugin=plugins.Find(BranchModulePlugin.Id) as BranchModulePlugin;Check(plugin is not null,"actual host registers Branch plugin");var doc=Field<GraphEditorDocument>(shell,"doc");doc.NewSolution();var registry=Field<ModuleRegistry>(shell,"modules");var sinkDescriptor=new ModuleDescriptor("test.branch-host-sink","1","Sink",[],[],[]);registry.Register(sinkDescriptor,()=>new Sink(sinkDescriptor));
  string branch=doc.AddNode(BranchModulePlugin.Id,"1.0.0",0,0),left=doc.AddNode(sinkDescriptor.Id,"1",150,0),right=doc.AddNode(sinkDescriptor.Id,"1",150,100);NodeDocument Node()=>doc.CurrentProcedure.Nodes.Single(n=>n.Id==branch);string Table()=>Node().Parameters["ModuleInfoList"].GetString()!;
  Check(Table()=="","new node table starts empty");doc.ConnectExecution(branch,left);doc.ConnectExecution(branch,right);Check(Table()==left+"#0$"+right+"#0$","real graph connection transaction updates Branch table in connection order");
  Call(shell,"OpenToolConfiguration",branch);var window=(ToolConfigurationModelWindow)Field<ToolConfigurationWindowHost>(shell,"toolConfigurationWindows").Current!;var session=Field<ToolConfigurationSession>(window,"session");Check(window.ConfigurationView.Diagnostics.Count==0,"real configuration window has all original leaf templates");
  session.State.CommitUser("ModuleInfoList",right+"#9$"+left+"#7$");Check(Table()==right+"#9$"+left+"#7$","table edit persists document values without route rebuild");
  var context= (ToolEditorContext)Call(shell,"ConnectToolEditorServices",new ToolEditorContext(Node(),doc.BaseDirectory,_=>{},_=>throw new InvalidOperationException()),branch)!;
  var bound=context.ConfigurationSubscriptions!.Bind(new(ConfigurationSubscriptionDomain.InputIo,"inputint","int"),new(new ConfigurationConstantReference("int","7"),ConfigurationSubscriptionIndex.All));Check(bound.Status==ConfigurationSubscriptionStatus.Available,"real host binds source int constant");
  session.State.Invoke(session.State.Find("Execute")!);Idle(shell);var display=plugin!.LastDisplay(Node(),doc.BaseDirectory,Field<ModuleExecutionScope>(shell,"executionScope").ForProcedure(doc.CurrentProcedureId));Check(display is not null,"actual config execute updates Branch display scope");var record=Field<Dictionary<string,Dictionary<string,NodeRunRecord>>>(shell,"configurationResults")[doc.CurrentProcedureId][branch];Check(record.Status==NodeRunStatus.Success&&record.Outputs["BranchMatch"].As<string>()=="NodeId:"+left+";","real configuration self-run retains original outgoing adjacency and routes matching row");
  window.Close();Pump();var both=doc.CopyNodes([branch,left]);var copiedIds=doc.PasteNodes(both,300,200);var copiedBranch=doc.CurrentProcedure.Nodes.Single(n=>copiedIds.Contains(n.Id)&&n.ModuleId==BranchModulePlugin.Id);var copiedSink=doc.CurrentProcedure.Nodes.Single(n=>copiedIds.Contains(n.Id)&&n.ModuleId==sinkDescriptor.Id);Check(copiedBranch.Parameters["ModuleInfoList"].GetString()==copiedSink.Id+"#7$","actual MainWindow Paste hook remaps copied target only and preserves value");var singleIds=doc.PasteNodes(doc.CopyNodes([branch]),600,200);Check(doc.CurrentProcedure.Nodes.Single(n=>n.Id==singleIds.Single()).Parameters["ModuleInfoList"].GetString()=="","actual single-node Paste clears stale target through ModuleOperate seed");doc.Undo();doc.Redo();Check(doc.CurrentProcedure.Nodes.Single(n=>n.Id==singleIds.Single()).Parameters["ModuleInfoList"].GetString()=="","document Undo/Redo replays prepared copied table without rebuilding neighbors");
  doc.DisconnectExecution(new(branch,right));Check(Table()==left+"#7$","real disconnect preserves surviving configured value");var save=System.IO.Path.GetFullPath("analysis/b-branch/host-solution/branch.vsolution");System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(save)!);doc.Save(save);doc.Load(save);Check(Table()==left+"#7$","real solution save/reopen preserves Branch table without neighbor rebuild");
  string capped=doc.AddNode(BranchModulePlugin.Id,"1.0.0",0,400);for(int i=0;i<32;i++){string target=doc.AddNode(sinkDescriptor.Id,"1",200+i*5,400);doc.ConnectExecution(capped,target);}Check(ProcedureGraph.ExecutionEdges(doc.CurrentProcedure).Count(e=>e.FromNode==capped)==32,"real topology hook permits exactly32 successors");string extra=doc.AddNode(sinkDescriptor.Id,"1",400,400);bool rejected=false;try{doc.ConnectExecution(capped,extra);}catch(InvalidOperationException){rejected=true;}Check(rejected&&ProcedureGraph.ExecutionEdges(doc.CurrentProcedure).Count(e=>e.FromNode==capped)==32,"real 33rd connection rejected atomically without table or link mutation");
 }
 sealed class Sink(ModuleDescriptor descriptor):IModule {public ModuleDescriptor Descriptor=>descriptor;public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken cancellationToken)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>()));}
}





