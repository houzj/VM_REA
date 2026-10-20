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
using VisionPlugins.ImageSource;
using VisionPlugins.MapCalibration;

static class Program {
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 static int checks;
 static void Check(bool okay,string name){if(!okay)throw new Exception(name);Console.WriteLine($"PASS {++checks}: {name}");}
 static T Field<T>(object owner,string name)=>(T)owner.GetType().GetField(name,Private)!.GetValue(owner)!;
 static void Set(object owner,string name,object? value)=>owner.GetType().GetField(name,Private)!.SetValue(owner,value);
 static object? Call(object owner,string name,params object[] values)=>owner.GetType().GetMethod(name,Private)!.Invoke(owner,values);
 static void Wait(Task task){var frame=new DispatcherFrame();_=task.ContinueWith(_=>Application.Current.Dispatcher.BeginInvoke(()=>frame.Continue=false));if(!task.IsCompleted)Dispatcher.PushFrame(frame);task.GetAwaiter().GetResult();}
 static JsonElement J<T>(T value)=>JsonSerializer.SerializeToElement(value);
 static GrayImage Load(string path){var bitmap=BitmapDecoder.Create(new Uri(path),BitmapCreateOptions.PreservePixelFormat,BitmapCacheOption.OnLoad).Frames[0];var gray=new FormatConvertedBitmap(bitmap,PixelFormats.Gray8,null,0);var pixels=new byte[gray.PixelWidth*gray.PixelHeight];gray.CopyPixels(pixels,gray.PixelWidth,0);return new GrayImage(gray.PixelWidth,gray.PixelHeight,pixels);}
 [STAThread] static int Main(string[] args){try{
  if(args.Length!=1)throw new ArgumentException("Own output directory required");var output=Path.GetFullPath(args[0]);Directory.CreateDirectory(output);
  var fixture=Path.GetFullPath("worktrees/tool-plugins/analysis/full-audit/n-image-calibration/solver-fixture");
  var first=Path.Combine(fixture,"frame-0.bmp");var second=Path.Combine(fixture,"frame-4.bmp");
  Check(File.Exists(first)&&File.Exists(second),"two real calibration frames present");
  var a=Load(first);var b=Load(second);Console.WriteLine($"FRAME_SHA {Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(first)))} {Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(second)))}");
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(app.Dispatcher));
  var shell=new MainWindow{Left=-20000,Top=-20000,WindowStartupLocation=WindowStartupLocation.Manual,ShowActivated=false,ShowInTaskbar=false};shell.Show();
  try{
   var doc=Field<GraphEditorDocument>(shell,"doc");var presenter=Field<ToolDisplayImagePresenter>(shell,"toolDisplayImage");var record=Field<ToolDisplayRecordPresenter>(shell,"toolDisplayRecord");var list=Field<ToolDisplayResultListPresenter>(shell,"toolDisplayList");var results=()=>Field<Dictionary<string,NodeRunRecord>>(shell,"results");
   var image=(ImageSourcePlugin)Field<ToolPluginRegistry>(shell,"plugins").Find("vision.image-source")!;
   void Select(string id){Set(shell,"selected",id);Call(shell,"RefreshRight");app.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
   string Source(string path,int y){var id=doc.AddNode("vision.image-source","1.0.0",0,y);var node=doc.CurrentProcedure.Nodes.Single(n=>n.Id==id);var p=node.Parameters.ToDictionary(x=>x.Key,x=>x.Value.Clone());p["ImageSourceType"]=J(3);p["ImagePaths"]=J(JsonSerializer.Serialize(new[]{path}));p["CurrentImagePath"]=J(path);p["StitchEnable"]=J(false);p["OutMono8"]=J(true);doc.SetParameters(id,p);return id;}
   void Run(string s1,GrayImage f1,string s2,GrayImage f2){var scope=Field<ModuleExecutionScope>(shell,"executionScope").ForProcedure(doc.CurrentProcedureId);image.SubmitImage(scope,s1,f1);image.SubmitImage(scope,s2,f2);Wait((Task)Call(shell,"RunProcedure",doc.CurrentProcedureId)!);app.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
   doc.NewSolution();var s1=Source(first,0);var s2=Source(second,150);var map=doc.AddNode(MapCalibrationPlugin.Id,"1.0.0",220,75);doc.ConnectExecution(s1,s2);doc.Connect(s1,"Image",map,"InputImage1");doc.Connect(s2,"Image",map,"InputImage2");
   var node=doc.CurrentProcedure.Nodes.Single(n=>n.Id==map);var mp=node.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());mp["InputType"]=J(1);mp["WeightFun"]=J(1);mp["CalibPathName"]=J(Path.Combine(output,"mapped-image.xml"));mp["RefreshFileEnable"]=J(true);doc.SetParameters(map,mp);
   Run(s1,a,s2,b);Console.WriteLine($"SUCCESS_STATUS {results()[map].Status}/{results()[map].Message}");Check(results()[map].Status==NodeRunStatus.Success,"real two-frame Map graph succeeds");Select(map);var success=presenter.Frame;Check(success is not null&&success.ImageLayers.Any(l=>l.Name=="InImage"&&l.Image is not null),"Map frame has second input image layer");Check(success!.Graphics.Any(g=>g.Name=="Display Current Corners Point"),"Map frame has mapped corner graphics");Console.WriteLine($"SUCCESS_DISPLAY layers={success.ImageLayers.Count} graphics={success.Graphics.Count} record={record.Text.Length} list={list.Columns.Count}");
   var blank=new GrayImage(b.Width,b.Height,Enumerable.Repeat((byte)127,b.Width*b.Height).ToArray());Run(s1,a,s2,blank);Console.WriteLine($"FAILED_STATUS {results()[map].Status}/{results()[map].Message}");Check(results()[map].Status!=NodeRunStatus.Success,"blank target fails Map graph");Select(map);var failed=presenter.Frame;Console.WriteLine($"FAILED_DISPLAY frame={(failed is not null)} layers={failed?.ImageLayers.Count??-1} graphics={failed?.Graphics.Count??-1} record={record.Text.Length} list={list.Columns.Count}");
   Select(s1);Check(presenter.Frame is null&&record.Text.Length==0&&list.Columns.Count==0,"switch from failed Map clears image/record/list presenters");
   File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{checks,passed=true,successGraphics=success.Graphics.Count,failedFrame=failed is not null,failedGraphics=failed?.Graphics.Count,manualAccepted=false}));return 0;
  }finally{shell.Close();app.Shutdown();}
 }catch(Exception error){Console.Error.WriteLine(error);return 1;}}
}
