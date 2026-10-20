using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;

static class Program {
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 static int checks;
 static void Check(bool okay,string name){if(!okay)throw new Exception(name);Console.WriteLine($"PASS {++checks}: {name}");}
 static T Field<T>(object owner,string name)=>(T)owner.GetType().GetField(name,Private)!.GetValue(owner)!;
 static void Set(object owner,string name,object? value)=>owner.GetType().GetField(name,Private)!.SetValue(owner,value);
 static object? Call(object owner,string name,params object[] values)=>owner.GetType().GetMethod(name,Private)!.Invoke(owner,values);
 static void Wait(Task task){var frame=new DispatcherFrame();_=task.ContinueWith(_=>Application.Current.Dispatcher.BeginInvoke(()=>frame.Continue=false));if(!task.IsCompleted)Dispatcher.PushFrame(frame);task.GetAwaiter().GetResult();}
 static JsonElement J<T>(T value)=>JsonSerializer.SerializeToElement(value);
 [STAThread]static int Main(string[] args){try{
  if(args.Length!=1)throw new ArgumentException("Own output directory required");var output=Path.GetFullPath(args[0]);Directory.CreateDirectory(output);
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(app.Dispatcher));
  var shell=new MainWindow{Left=-20000,Top=-20000,WindowStartupLocation=WindowStartupLocation.Manual,ShowActivated=false,ShowInTaskbar=false};shell.Show();
  try{
   var doc=Field<GraphEditorDocument>(shell,"doc");var presenter=Field<ToolDisplayImagePresenter>(shell,"toolDisplayImage");var record=Field<ToolDisplayRecordPresenter>(shell,"toolDisplayRecord");var list=Field<ToolDisplayResultListPresenter>(shell,"toolDisplayList");var results=()=>Field<Dictionary<string,NodeRunRecord>>(shell,"results");
   void Select(string id){Set(shell,"selected",id);Call(shell,"RefreshRight");app.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
   void Run(){Wait((Task)Call(shell,"RunProcedure",doc.CurrentProcedureId)!);app.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
   doc.NewSolution();var npoint=doc.AddNode(NPointPlugin.Id,"1.0.0",180,0);var other=doc.AddNode("vision.image-source","1.0.0",0,150);
   var node=doc.CurrentProcedure.Nodes.Single(n=>n.Id==npoint);var values=node.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());values["CalibPointGet"]=J(1);values["CalibPointTotalNum"]=J(4);values["RotPointTotalNum"]=J(0);values["WeightFun"]=J(1);
   var pairs=new (double ix,double iy,double wx,double wy)[]{(0,0,0,0),(10,0,20,0),(0,10,0,20),(10,10,20,20)};
   for(int row=0;row<4;row++){values["ImagePointX"+row]=J(pairs[row].ix);values["ImagePointY"+row]=J(pairs[row].iy);values["WorldPointX"+row]=J(pairs[row].wx);values["WorldPointY"+row]=J(pairs[row].wy);}doc.SetParameters(npoint,values);
   Run();Console.WriteLine($"SUCCESS_STATUS {results()[npoint].Status}/{results()[npoint].Message}");Check(results()[npoint].Status==NodeRunStatus.Success&&results()[npoint].Outputs["CalibStatus"].As<long>()==1,"manual four-point solver succeeds");Select(npoint);var success=presenter.Frame;Check(success is not null&&success.Graphics.Any(g=>g.Name=="Image Point"),"success frame has image point graphics");Check(record.Text.Contains("CalibStatus")&&list.Columns.Count>0,"success frame reaches record and result list");Console.WriteLine($"SUCCESS_DISPLAY graphics={success!.Graphics.Count} record={record.Text.Length} list={list.Columns.Count}");
   values=doc.CurrentProcedure.Nodes.Single(n=>n.Id==npoint).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());for(int row=0;row<4;row++){values["ImagePointX"+row]=J(0d);values["ImagePointY"+row]=J(0d);}doc.SetParameters(npoint,values);
   Run();Console.WriteLine($"FAILED_STATUS {results()[npoint].Status}/{results()[npoint].Message}/0x{results()[npoint].NativeErrorCode:x8}");Check(results()[npoint].Status!=NodeRunStatus.Success,"degenerate manual point set fails solver");Select(npoint);var failed=presenter.Frame;Console.WriteLine($"FAILED_DISPLAY frame={(failed is not null)} graphics={failed?.Graphics.Count??-1} record={record.Text.Length} list={list.Columns.Count}");Check(failed is not null&&failed.RecordText.Contains("错误码:"),"failed round offers diagnostic record rather than previous success");
   Select(other);Check(presenter.Frame is null&&record.Text.Length==0&&list.Columns.Count==0,"switch to image source clears image/record/list presenters");
   File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{checks,passed=true,successGraphics=success.Graphics.Count,failedGraphics=failed.Graphics.Count,manualAccepted=false}));return 0;
  }finally{shell.Close();app.Shutdown();}
 }catch(Exception error){Console.Error.WriteLine(error);return 1;}}
}
