using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VisionCore;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.CalibrationBoard;
using VisionPlugins.DistortionCalibration;
using VisionPlugins.ImageSource;

static class Program {
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 static int checks;
 static void Check(bool value,string label){if(!value)throw new Exception(label);Console.WriteLine($"PASS {++checks}: {label}");}
 static T Field<T>(object owner,string name)=>(T)owner.GetType().GetField(name,Private)!.GetValue(owner)!;
 static void Set(object owner,string name,object? value)=>owner.GetType().GetField(name,Private)!.SetValue(owner,value);
 static object? Call(object owner,string name,params object[] args)=>owner.GetType().GetMethod(name,Private)!.Invoke(owner,args);
 static void Wait(Task task){var frame=new DispatcherFrame();_=task.ContinueWith(_=>Application.Current.Dispatcher.BeginInvoke(()=>frame.Continue=false));if(!task.IsCompleted)Dispatcher.PushFrame(frame);task.GetAwaiter().GetResult();}
 static JsonElement J<T>(T value)=>JsonSerializer.SerializeToElement(value);
 static string Sha(string path)=>Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));
 static void SaveGray(string path,int width,int height,byte[] bytes){var bitmap=BitmapSource.Create(width,height,96,96,PixelFormats.Gray8,null,bytes,width);var encoder=new BmpBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(bitmap));using var stream=File.Create(path);encoder.Save(stream);}
 [STAThread]static int Main(string[] args){try{
  if(args.Length!=1)throw new ArgumentException("Own output directory required");var output=Path.GetFullPath(args[0]);Directory.CreateDirectory(output);
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(app.Dispatcher));
  var shell=new MainWindow{Left=-20000,Top=-20000,WindowStartupLocation=WindowStartupLocation.Manual,ShowActivated=false,ShowInTaskbar=false};shell.Show();
  try{
   var doc=Field<GraphEditorDocument>(shell,"doc");var presenter=Field<ToolDisplayImagePresenter>(shell,"toolDisplayImage");var record=Field<ToolDisplayRecordPresenter>(shell,"toolDisplayRecord");var list=Field<ToolDisplayResultListPresenter>(shell,"toolDisplayList");var results=()=>Field<Dictionary<string,NodeRunRecord>>(shell,"results");
   void Select(string id){Set(shell,"selected",id);Call(shell,"RefreshRight");app.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
   var imagePlugin=(ImageSourcePlugin)Field<ToolPluginRegistry>(shell,"plugins").Find("vision.image-source")!;
   void Run(string sourceId,GrayImage frame){imagePlugin.SubmitImage(Field<ModuleExecutionScope>(shell,"executionScope").ForProcedure(doc.CurrentProcedureId),sourceId,frame);Wait((Task)Call(shell,"RunProcedure",doc.CurrentProcedureId)!);app.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
   string Source(string image){var id=doc.AddNode("vision.image-source","1.0.0",0,0);var node=doc.CurrentProcedure.Nodes.Single(n=>n.Id==id);var values=node.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());values["ImageSourceType"]=J(3);values["ImagePaths"]=J(JsonSerializer.Serialize(new[]{image}));values["CurrentImagePath"]=J(image);values["StitchEnable"]=J(false);values["OutMono8"]=J(true);doc.SetParameters(id,values);return id;}
   var pixels=Enumerable.Repeat((byte)255,480*400).ToArray();for(int y=50;y<350;y++)for(int x=60;x<420;x++)pixels[y*480+x]=(byte)((((x-60)/30+(y-50)/30)&1)==0?0:255);
   var boardImage=Path.Combine(output,"board-valid.bmp");var blankImage=Path.Combine(output,"board-blank.bmp");SaveGray(boardImage,480,400,pixels);SaveGray(blankImage,480,400,Enumerable.Repeat((byte)127,480*400).ToArray());
   doc.NewSolution();var source=Source(boardImage);var board=doc.AddNode(CalibrationBoardPlugin.Id,"1.0.0",200,0);doc.Connect(source,"Image",board,"InputImage");
   Select(board);Run(source,new GrayImage(480,400,pixels));Console.WriteLine($"BOARD_RUN source={results()[source].Status}/{results()[source].Message} board={results()[board].Status}/{results()[board].Message}/0x{results()[board].NativeErrorCode:x8}");Check(results()[board].Status==NodeRunStatus.Success,"board graph success");Select(board);var boardSuccess=presenter.Frame;Check(boardSuccess is not null&&boardSuccess.Graphics.Count>0,"board selected success has display graphics");Console.WriteLine($"BOARD_SUCCESS image={(boardSuccess.Image is not null)} graphics={boardSuccess.Graphics.Count} record={record.Text.Length} list={list.Columns.Count}");
   Run(source,new GrayImage(480,400,Enumerable.Repeat((byte)127,480*400).ToArray()));Check(results()[board].Status==NodeRunStatus.NoMatch,"board blank image yields NoMatch");Select(board);var boardFailed=presenter.Frame;Console.WriteLine($"BOARD_FAILED frame={(boardFailed is not null)} image={(boardFailed?.Image is not null)} graphics={boardFailed?.Graphics.Count??-1} record={record.Text.Length} list={list.Columns.Count}");Check(boardFailed is not null&&boardFailed.Graphics.All(g=>g.Name!="Display Calibration Point"),"board failure has no stale calibration point graphics");Select(source);Check(presenter.Frame is null&&record.Text.Length==0&&list.Columns.Count==0,"switch from failed board clears selected tool frame/record/list");
   doc.NewSolution();var fixture=Path.GetFullPath("worktrees/tool-plugins/analysis/distortion-calibration/trace-coded-boards/fixture.bmp");Check(File.Exists(fixture),"distortion SDK fixture present");var ds=Source(fixture);var distortion=doc.AddNode(DistortionCalibrationPlugin.Id,"1.0.0",200,0);doc.Connect(ds,"Image",distortion,"InputImage");var target=doc.CurrentProcedure.Nodes.Single(n=>n.Id==distortion);var dp=target.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());dp["CalibBoardType"]=J(7);dp["DistortionEvaluationEnable"]=J(true);dp["OutputPostCorrectionEnable"]=J(true);dp["CalibPathName"]=J(Path.Combine(output,"distortion.iccal"));dp["RefreshFileEnable"]=J(true);doc.SetParameters(distortion,dp);
   var decoder=BitmapDecoder.Create(new Uri(fixture),BitmapCreateOptions.PreservePixelFormat,BitmapCacheOption.OnLoad);var bitmap=new FormatConvertedBitmap(decoder.Frames[0],PixelFormats.Gray8,null,0);var dbytes=new byte[bitmap.PixelWidth*bitmap.PixelHeight];bitmap.CopyPixels(dbytes,bitmap.PixelWidth,0);var dframe=new GrayImage(bitmap.PixelWidth,bitmap.PixelHeight,dbytes);
   Select(distortion);Run(ds,dframe);Check(results()[distortion].Status==NodeRunStatus.Success,"distortion graph success");Select(distortion);var distSuccess=presenter.Frame;Check(distSuccess is not null&&distSuccess.ImageLayers.Any(l=>l.Name=="InImage")&&distSuccess.ImageLayers.Any(l=>l.Name=="Display Post-Correction Image"),"distortion selected success has source/corrected image layers");Console.WriteLine($"DIST_SUCCESS layers={string.Join(',',distSuccess.ImageLayers.Select(l=>l.Name+":"+(l.Image is not null)))} graphics={distSuccess.Graphics.Count} record={record.Text.Length} list={list.Columns.Count}");
   dp=doc.CurrentProcedure.Nodes.Single(n=>n.Id==distortion).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());dp["CalibPathName"]=J(Path.Combine(output,"distortion.bad"));doc.SetParameters(distortion,dp);Run(ds,dframe);Check(results()[distortion].Status==NodeRunStatus.Failed,"distortion invalid export path fails graph round");Select(distortion);var distFailed=presenter.Frame;Console.WriteLine($"DIST_FAILED frame={(distFailed is not null)} layers={string.Join(',',distFailed?.ImageLayers.Select(l=>l.Name+":"+(l.Image is not null))??[])} graphics={distFailed?.Graphics.Count??-1} record={record.Text.Length} list={list.Columns.Count}");Check(distFailed is not null,"distortion failed round retains a display frame for status inspection");Select(ds);Check(presenter.Frame is null&&record.Text.Length==0&&list.Columns.Count==0,"switch from failed distortion clears selected tool frame/record/list");
   File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{checks,passed=true,boardSuccessImage=boardSuccess.Image is not null,boardSuccessGraphics=boardSuccess.Graphics.Count,boardFailedGraphics=boardFailed.Graphics.Count,distSuccessLayers=distSuccess.ImageLayers.Select(l=>new{l.Name,hasImage=l.Image is not null}).ToArray(),distFailedLayers=distFailed.ImageLayers.Select(l=>new{l.Name,hasImage=l.Image is not null}).ToArray(),manualAccepted=false}));return 0;
  }finally{shell.Close();app.Shutdown();}
 }catch(Exception error){Console.Error.WriteLine(error);return 1;}}
}
