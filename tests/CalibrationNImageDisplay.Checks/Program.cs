using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VisionCore;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
using VisionPlugins.ImageSource;

static class Program {
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 static int checks;
 static void Check(bool yes,string name){if(!yes)throw new Exception(name);Console.WriteLine($"PASS {++checks}: {name}");}
 static T Field<T>(object owner,string name)=>(T)owner.GetType().GetField(name,Private)!.GetValue(owner)!;
 static void Set(object owner,string name,object? value)=>owner.GetType().GetField(name,Private)!.SetValue(owner,value);
 static object? Call(object owner,string name,params object[] values)=>owner.GetType().GetMethod(name,Private)!.Invoke(owner,values);
 static void Wait(Task task){var frame=new DispatcherFrame();_=task.ContinueWith(_=>Application.Current.Dispatcher.BeginInvoke(()=>frame.Continue=false));if(!task.IsCompleted)Dispatcher.PushFrame(frame);task.GetAwaiter().GetResult();}
 static JsonElement J<T>(T value)=>JsonSerializer.SerializeToElement(value);
 [STAThread]static int Main(string[] args){try{
  if(args.Length!=1)throw new ArgumentException("Own output directory required");var output=Path.GetFullPath(args[0]);Directory.CreateDirectory(output);
  var fixture=Path.GetFullPath("tests/NImageCalibration.PluginChecks/Fixtures/hik-i-help-derived.png");Check(File.Exists(fixture),"coded-board source fixture exists");Console.WriteLine($"FIXTURE_SHA {Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(fixture)))}");
  var decoder=BitmapDecoder.Create(new Uri(fixture),BitmapCreateOptions.PreservePixelFormat,BitmapCacheOption.OnLoad);var bmp=new FormatConvertedBitmap(decoder.Frames[0],PixelFormats.Gray8,null,0);int width=bmp.PixelWidth,height=bmp.PixelHeight;var bytes=new byte[width*height];bmp.CopyPixels(bytes,width,0);
  GrayImage Frame(int i){int dx=i%3*10,dy=i/3*10;var shifted=Enumerable.Repeat((byte)255,bytes.Length).ToArray();for(int y=0;y<height-dy;y++)bytes.AsSpan(y*width,width-dx).CopyTo(shifted.AsSpan((y+dy)*width+dx,width-dx));return new(width,height,shifted);}
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(app.Dispatcher));
  var shell=new MainWindow{Left=-20000,Top=-20000,WindowStartupLocation=WindowStartupLocation.Manual,ShowActivated=false,ShowInTaskbar=false};shell.Show();
  try{
   var doc=Field<GraphEditorDocument>(shell,"doc");var presenter=Field<ToolDisplayImagePresenter>(shell,"toolDisplayImage");var record=Field<ToolDisplayRecordPresenter>(shell,"toolDisplayRecord");var list=Field<ToolDisplayResultListPresenter>(shell,"toolDisplayList");var results=()=>Field<Dictionary<string,NodeRunRecord>>(shell,"results");
   var image=(ImageSourcePlugin)Field<ToolPluginRegistry>(shell,"plugins").Find("vision.image-source")!;
   void Select(string id){Set(shell,"selected",id);Call(shell,"RefreshRight");app.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
   doc.NewSolution();var source=doc.AddNode("vision.image-source","1.0.0",0,0);var target=doc.AddNode(NImagePlugin.Id,"1.0.0",200,0);doc.Connect(source,"Image",target,"InputImage");
   var sn=doc.CurrentProcedure.Nodes.Single(n=>n.Id==source);var ip=sn.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());ip["ImageSourceType"]=J(3);ip["ImagePaths"]=J(JsonSerializer.Serialize(new[]{fixture}));ip["CurrentImagePath"]=J(fixture);ip["StitchEnable"]=J(false);ip["OutMono8"]=J(true);doc.SetParameters(source,ip);
   var tn=doc.CurrentProcedure.Nodes.Single(n=>n.Id==target);var np=tn.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());np["CalibPointTotalNum"]=J(9);np["RotPointTotalNum"]=J(0);np["CalibPathName"]=J(Path.Combine(output,"recovered.xml"));np["RefreshFileEnable"]=J(true);doc.SetParameters(target,np);
   void Run(int i,bool blank){var subscriptions=new[]{("MovePhysicalPointX",(double)(i%3)),("MovePhysicalPointY",(double)(i/3)),("RotateAngle",0d)}.Select(p=>new WorkflowSubscription(target,p.Item1,false,new(WorkflowSourceKind.NumberArrayConstant,JsonSerializer.Serialize(new[]{p.Item2})))).ToArray();doc.SetWorkflowConfiguration(doc.CurrentProcedureId,new(){Subscriptions=subscriptions});var frame=blank?new GrayImage(width,height,Enumerable.Repeat((byte)127,bytes.Length).ToArray()):Frame(i);image.SubmitImage(Field<ModuleExecutionScope>(shell,"executionScope").ForProcedure(doc.CurrentProcedureId),source,frame);Wait((Task)Call(shell,"RunProcedure",doc.CurrentProcedureId)!);app.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);Console.WriteLine($"RUN i={i} blank={blank} status={results()[target].Status} cur={(results()[target].Outputs.TryGetValue("CurNum",out var v)?v.As<long>().ToString():"none")}");}
   for(int i=0;i<8;i++)Run(i,false);Check(results()[target].Status==NodeRunStatus.Success&&results()[target].Outputs["CurNum"].As<long>()==8,"eight valid images retained as pending batch");Select(target);var pending=presenter.Frame;Check(pending is not null&&pending.ImageLayers.Any(l=>l.Name=="InImage"&&l.Image is not null),"pending display keeps source image layer");Console.WriteLine($"PENDING layers={pending!.ImageLayers.Count} graphics={pending.Graphics.Count} record={record.Text.Length} list={list.Columns.Count}");
   Run(8,true);Check(results()[target].Status==NodeRunStatus.Failed&&results()[target].NativeErrorCode==0x10103000,"blank ninth frame fails recognition");Select(target);var failed=presenter.Frame;Console.WriteLine($"FAILED frame={(failed is not null)} layers={failed?.ImageLayers.Count??-1} graphics={failed?.Graphics.Count??-1} record={record.Text.Length} list={list.Columns.Count}");Check(failed is not null&&failed.RecordText.Contains("错误码:"),"failed selected node displays current error record");
   Run(0,false);Check(results()[target].Status==NodeRunStatus.Success&&results()[target].Outputs["CurNum"].As<long>()==1,"next valid frame restarts batch after failure");Select(target);var restarted=presenter.Frame;Check(restarted is not null&&restarted.ImageLayers.Any(l=>l.Image is not null),"restarted frame displays new source image");
   for(int i=1;i<9;i++)Run(i,false);Check(results()[target].Status==NodeRunStatus.Success&&results()[target].Outputs["CalibStatus"].As<long>()==1,"recovery batch completes calibration");Select(target);var recovered=presenter.Frame;Check(recovered is not null&&recovered.Graphics.Any(g=>g.Name=="Image Point"),"recovered display has current calibration points");Console.WriteLine($"RECOVERED layers={recovered!.ImageLayers.Count} graphics={recovered.Graphics.Count} record={record.Text.Length} list={list.Columns.Count}");
   Select(source);Check(presenter.Frame is null&&record.Text.Length==0&&list.Columns.Count==0,"switch to source clears image/record/list presenters");
   File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{checks,passed=true,pendingGraphics=pending.Graphics.Count,failedGraphics=failed.Graphics.Count,recoveredGraphics=recovered.Graphics.Count,manualAccepted=false}));return 0;
  }finally{shell.Close();app.Shutdown();}
 }catch(Exception error){Console.Error.WriteLine(error);return 1;}}
}
