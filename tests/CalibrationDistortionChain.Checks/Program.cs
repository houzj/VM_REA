using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionCore;
using VisionPluginContracts;
using VisionPlugins.DistortionCalibration;
using VisionPlugins.Calibration;

static class Program {
 const BindingFlags Private=BindingFlags.NonPublic|BindingFlags.Instance;
 static int checks;
 static void Check(bool value,string name){if(!value)throw new Exception(name);Console.WriteLine($"PASS {++checks}: {name}");}
 static T Field<T>(object obj,string name)=>(T)obj.GetType().GetField(name,Private)!.GetValue(obj)!;
 static object? Call(object obj,string name,params object[] arguments)=>obj.GetType().GetMethod(name,Private)!.Invoke(obj,arguments);
 static void Wait(Task task){var frame=new DispatcherFrame();_=task.ContinueWith(_=>Application.Current.Dispatcher.BeginInvoke(()=>frame.Continue=false));if(!task.IsCompleted)Dispatcher.PushFrame(frame);task.GetAwaiter().GetResult();}
 static JsonElement J<T>(T value)=>JsonSerializer.SerializeToElement(value);
 static string Sha(string path)=>Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));
 [STAThread] static int Main(string[] args){try{
  if(args.Length!=1)throw new ArgumentException("Own output directory required");var output=Path.GetFullPath(args[0]);Directory.CreateDirectory(output);
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(app.Dispatcher));
  var shell=new MainWindow{Left=-20000,Top=-20000,WindowStartupLocation=WindowStartupLocation.Manual,ShowActivated=false,ShowInTaskbar=false};shell.Show();
  try{
   var doc=Field<GraphEditorDocument>(shell,"doc");doc.NewSolution();
   var source=doc.AddNode("vision.image-source","1.0.0",0,0);var distortion=doc.AddNode(DistortionCalibrationPlugin.Id,"1.0.0",200,0);var downstream=doc.AddNode(NImagePlugin.Id,"1.0.0",400,0);
   doc.ConnectExecution(source,distortion);doc.ConnectExecution(distortion,downstream);doc.BindInput(source,"Image",distortion,"InputImage");doc.BindInput(distortion,"PostCorrectionImage",downstream,"InputImage");
   var node=(string id)=>doc.CurrentProcedure.Nodes.Single(n=>n.Id==id);
   var image=Path.GetFullPath("worktrees/tool-plugins/analysis/distortion-calibration/trace-coded-boards/fixture.bmp");Check(File.Exists(image),"original SDK board image fixture exists");
   var src=node(source).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());src["ImagePaths"]=J(JsonSerializer.Serialize(new[]{image}));src["CurrentImagePath"]=J(image);doc.SetParameters(source,src);
   var path=Path.Combine(output,"distortion.iccal");var invalid=Path.Combine(output,"distortion.bad");
   void Configure(string file){var pars=node(distortion).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());pars["CalibBoardType"]=J(7);pars["DistortionEvaluationEnable"]=J(true);pars["OutputPostCorrectionEnable"]=J(true);pars["CalibPathName"]=J(file);pars["RefreshFileEnable"]=J(true);doc.SetParameters(distortion,pars);}
   Configure(path);
   var next=node(downstream).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());next["CalibPointTotalNum"]=J(9);next["RotPointTotalNum"]=J(0);doc.SetParameters(downstream,next);
   void Run(){Wait((Task)Call(shell,"RunProcedure",doc.CurrentProcedureId)!);}
   var results=()=>Field<Dictionary<string,NodeRunRecord>>(shell,"results");
   Run();var d=results()[distortion];Check(d.Status==NodeRunStatus.Success,"actual image-source to distortion execution succeeds: "+d.Message);
   Check(d.Outputs.TryGetValue("PostCorrectionImage",out var corrected)&&corrected.Value is IImageData,"distortion publishes typed corrected image");
   var pixels=(IImageData)corrected!.Value!;Check(pixels.Width==1000&&pixels.Height==1000,"corrected image retains source dimensions");
   Check(File.Exists(path)&&new FileInfo(path).Length==76,"automatic .iccal export writes 19 floats");var fileHash=Sha(path);
   var n=results()[downstream];Console.WriteLine($"DOWNSTREAM status={n.Status}; message={n.Message}; outputs={string.Join(',',n.Outputs.Keys)}");Check(n.Status==NodeRunStatus.Success&&n.Outputs.TryGetValue("CurNum",out var count)&&count.As<long>()==1,"actual NImage consumer processes distortion corrected Image as first calibration frame");
   var saved=Path.Combine(output,"chain.vmx.json");doc.Save(saved);doc.Load(saved);Check(doc.CurrentProcedure.Connections.Any(c=>c.FromNode==distortion&&c.ToNode==downstream&&c.FromPort=="PostCorrectionImage"),"solution reload retains typed distortion to NImage binding");
   Configure(invalid);Run();d=results()[distortion];Console.WriteLine($"INVALID status={d.Status}; message={d.Message}; outputs={string.Join(',',d.Outputs.Keys)}");Check(d.Status==NodeRunStatus.Failed&&d.Message?.Contains("0x8010000e")==true,"invalid export extension fails current distortion round with exact file code");Check(results()[downstream].Status==NodeRunStatus.Skipped,"failed distortion execution gates downstream calibration consumer");Check(!File.Exists(invalid)&&Sha(path)==fileHash,"failed export leaves previous valid file unchanged");
   Configure(path);Run();d=results()[distortion];Check(d.Status==NodeRunStatus.Success&&File.Exists(path),"valid path recovers successful distortion run after export failure");
   File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{checks,passed=true,sourceImage=image,sourceHash=Sha(image),calibrationFile=path,calibrationHash=Sha(path),correctedWidth=pixels.Width,correctedHeight=pixels.Height,downstreamStatus=n.Status.ToString(),manualAccepted=false}));return 0;
  }finally{shell.Close();app.Shutdown();}
 }catch(Exception error){Console.Error.WriteLine(error);return 1;}}
}
