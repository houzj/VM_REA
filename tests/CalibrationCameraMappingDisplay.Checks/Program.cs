using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.CameraMapping;

static class Program {
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 static int checks;
 static void Check(bool okay,string name){if(!okay)throw new Exception(name);Console.WriteLine($"PASS {++checks}: {name}");}
 static T Field<T>(object owner,string name)=>(T)owner.GetType().GetField(name,Private)!.GetValue(owner)!;
 static void Set(object owner,string name,object? value)=>owner.GetType().GetField(name,Private)!.SetValue(owner,value);
 static object? Call(object owner,string name,params object[] values)=>owner.GetType().GetMethod(name,Private)!.Invoke(owner,values);
 static void Wait(Task task){var frame=new DispatcherFrame();_=task.ContinueWith(_=>Application.Current.Dispatcher.BeginInvoke(()=>frame.Continue=false));if(!task.IsCompleted)Dispatcher.PushFrame(frame);task.GetAwaiter().GetResult();}
 [STAThread] static int Main(string[] args){try{
  if(args.Length!=1)throw new ArgumentException("Own output directory required");var output=Path.GetFullPath(args[0]);Directory.CreateDirectory(output);
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(app.Dispatcher));
  var shell=new MainWindow{Left=-20000,Top=-20000,WindowStartupLocation=WindowStartupLocation.Manual,ShowActivated=false,ShowInTaskbar=false};shell.Show();
  try{
   var doc=Field<GraphEditorDocument>(shell,"doc");var image=Field<ToolDisplayImagePresenter>(shell,"toolDisplayImage");var record=Field<ToolDisplayRecordPresenter>(shell,"toolDisplayRecord");var list=Field<ToolDisplayResultListPresenter>(shell,"toolDisplayList");var results=()=>Field<Dictionary<string,NodeRunRecord>>(shell,"results");
   void Select(string id){Set(shell,"selected",id);Call(shell,"RefreshRight");app.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
   void Run(){Wait((Task)Call(shell,"RunProcedure",doc.CurrentProcedureId)!);app.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
   doc.NewSolution();var map=doc.AddNode(CameraMappingPlugin.Id,"1.0.0",150,0);var other=doc.AddNode("vision.image-source","1.0.0",0,150);
   var inputs=new Dictionary<string,string>{["ObjectPointX1"]="[0,10]",["ObjectPointY1"]="[0,0]",["TargetPointX1"]="[10,30]",["TargetPointY1"]="[-5,-5]",["ObjectPointX2"]="[0,10]",["ObjectPointY2"]="[10,10]",["TargetPointX2"]="[10,30]",["TargetPointY2"]="[15,15]"};
   void Configure(){doc.SetWorkflowConfiguration(doc.CurrentProcedureId,new(){Subscriptions=inputs.Select(p=>new WorkflowSubscription(map,p.Key,false,new(WorkflowSourceKind.NumberArrayConstant,p.Value))).ToList()});}
   Configure();Run();Console.WriteLine($"SUCCESS_STATUS {results()[map].Status}/{results()[map].Message}");Check(results()[map].Status==NodeRunStatus.Success&&results()[map].Outputs["CalibStatus"].As<long>()==1,"four real point pairs solve through main workflow");Select(map);var success=image.Frame;Check(success is not null&&record.Text.Contains("CalibStatus")&&list.Columns.Count==9,"source-declared record and nine result columns reach main presenters");Check(success!.Image is null&&success.Graphics.Count==0,"CameraMapping text-only display adds no invented image or graphics");
   inputs["TargetPointY1"]="[-5]";Configure();Run();Console.WriteLine($"FAILED_STATUS {results()[map].Status}/{results()[map].Message}");Check(results()[map].Status!=NodeRunStatus.Success,"mismatched target array fails CameraMapping");Select(map);Console.WriteLine($"FAILED_DISPLAY frame={(image.Frame is not null)} record={record.Text.Length} list={list.Columns.Count}");Check(image.Frame is null&&record.Text.Length==0&&list.Columns.Count==0,"failed round clears prior text/list snapshot");
   inputs["TargetPointY1"]="[-5,-5]";Configure();Run();Check(results()[map].Status==NodeRunStatus.Success,"same node recovers after correcting source array");Select(other);Check(image.Frame is null&&record.Text.Length==0&&list.Columns.Count==0,"switch to source node leaves no CameraMapping presenter state");
   File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{checks,passed=true,manualAccepted=false}));return 0;
  }finally{shell.Close();app.Shutdown();}
 }catch(Exception error){Console.Error.WriteLine(error);return 1;}}
}
