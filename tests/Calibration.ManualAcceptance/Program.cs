using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using VisionEditor;

// Manual acceptance launches the real shell and configuration host. Reflection
// stays in this test helper; the product has no test-only startup or capture path.
static class Program {
 [STAThread] static int Main(string[] args){
  try{
   bool framework=args.Contains("--framework");
   string id=framework?"vision.read-calib-file":args.FirstOrDefault()??"vision.n-point-calibration";
   var app=new Application{ShutdownMode=ShutdownMode.OnMainWindowClose};
   var window=new MainWindow{Title="插件线验收 · 公共配置模型 — "+id};
   var original=(UIElement)window.Content;window.Content=null;
   var host=new DockPanel();
   var banner=new TextBlock{Text="插件线验收  |  "+(framework?"公共配置框架：图像源 + 标定加载 · 公共配置模型":id)+"  |  tool-plugins 工作树",
    Background=new SolidColorBrush(Color.FromRgb(255,121,0)),Foreground=Brushes.Black,
    FontSize=15,Padding=new Thickness(10,5,10,5)};
   DockPanel.SetDock(banner,Dock.Top);host.Children.Add(banner);host.Children.Add(original);window.Content=host;
   app.DispatcherUnhandledException+=(_,e)=>{Console.Error.WriteLine(e.Exception);e.Handled=true;app.Shutdown(1);};
   window.Loaded+=(_,_)=>window.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,new Action(()=>{
    void Invoke(string name,params object[] values)=>typeof(MainWindow).GetMethod(name,BindingFlags.NonPublic|BindingFlags.Instance)!.Invoke(window,values);
    var document=(VisionEditor.Core.GraphEditorDocument)typeof(MainWindow).GetField("doc",BindingFlags.NonPublic|BindingFlags.Instance)!.GetValue(window)!;
    document.NewSolution();
    if(id=="vision.calibration-board"){
     var directory=System.IO.Path.GetFullPath(System.IO.Path.Combine("analysis/manual-acceptance/board-fixtures",Guid.NewGuid().ToString("N")));System.IO.Directory.CreateDirectory(directory);
     const int width=480,height=400;var pixels=Enumerable.Repeat((byte)255,width*height).ToArray();
     for(int y=50;y<350;y++)for(int x=60;x<420;x++)pixels[y*width+x]=(byte)((((x-60)/30+(y-50)/30)&1)==0?0:255);
     var encoder=new System.Windows.Media.Imaging.BmpBitmapEncoder();encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(System.Windows.Media.Imaging.BitmapSource.Create(width,height,96,96,PixelFormats.Gray8,null,pixels,width)));
     string imagePath=System.IO.Path.Combine(directory,"棋盘格-测试样本.bmp");using(var output=System.IO.File.Create(imagePath))encoder.Save(output);
     var source=document.AddNode("vision.image-source","1.0.0",50,70);var board=document.AddNode(id,"1.0.0",250,70);document.ConnectExecution(source,board);document.BindInput(source,"Image",board,"InputImage");
     var values=document.CurrentProcedure.Nodes.Single(n=>n.Id==source).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());values["ImagePaths"]=System.Text.Json.JsonSerializer.SerializeToElement(System.Text.Json.JsonSerializer.Serialize(new[]{imagePath}));values["CurrentImagePath"]=System.Text.Json.JsonSerializer.SerializeToElement(imagePath);document.SetParameters(source,values);
     var task=(Task)typeof(MainWindow).GetMethod("RunProcedure",BindingFlags.NonPublic|BindingFlags.Instance)!.Invoke(window,new object[]{document.CurrentProcedureId})!;
     var frame=new DispatcherFrame();_=task.ContinueWith(_=>window.Dispatcher.BeginInvoke(()=>frame.Continue=false));if(!task.IsCompleted)Dispatcher.PushFrame(frame);task.GetAwaiter().GetResult();
     var records=(Dictionary<string,VisionFramework.NodeRunRecord>)typeof(MainWindow).GetField("results",BindingFlags.NonPublic|BindingFlags.Instance)!.GetValue(window)!;
     if(records[board].Status!=VisionFramework.NodeRunStatus.Success)throw new InvalidOperationException("标定板验收样本运行失败："+records[board].Message);
     typeof(MainWindow).GetField("selected",BindingFlags.NonPublic|BindingFlags.Instance)!.SetValue(window,board);Invoke("RefreshAll");Invoke("OpenToolConfiguration",board);Console.WriteLine("MANUAL_ACCEPTANCE_READY "+id+" source -> board; synthetic checker fixture, no device tests; sample="+imagePath);return;
    }
    if(id=="vision.camera-mapping"){
     var registry=(VisionFramework.ModuleRegistry)typeof(MainWindow).GetField("modules",BindingFlags.NonPublic|BindingFlags.Instance)!.GetValue(window)!;
     registry.Register(new CameraPoints().Descriptor,()=>new CameraPoints());
     var source=document.AddNode("acceptance.camera-points","1",50,50);
     var camera=document.AddNode(id,"1.0.0",250,50);document.ConnectExecution(source,camera);
     document.BindInput(source,"Object.X",camera,"ObjectPointX1");document.BindInput(source,"Object.Y",camera,"ObjectPointY1");
     document.BindInput(source,"Target.X",camera,"TargetPointX1");document.BindInput(source,"Target.Y",camera,"TargetPointY1");
     var task=(Task)typeof(MainWindow).GetMethod("RunProcedure",BindingFlags.NonPublic|BindingFlags.Instance)!.Invoke(window,new object[]{document.CurrentProcedureId})!;
     var frame=new DispatcherFrame();_=task.ContinueWith(_=>window.Dispatcher.BeginInvoke(()=>frame.Continue=false));if(!task.IsCompleted)Dispatcher.PushFrame(frame);task.GetAwaiter().GetResult();
     Console.WriteLine("MANUAL_ACCEPTANCE_READY "+id+" synthetic point fixture; no device tests");Invoke("OpenToolConfiguration",camera);return;
    }
    if(args.Contains("--calibration-pair")){string paired=id switch{"vision.translation-calibration"=>"vision.rotate-calibration","vision.rotate-calibration"=>"vision.translation-calibration",_=>VisionPlugins.Calibration.NImagePlugin.Id};document.AddNode(paired,"1.0.0",50,50);}
    string? sample=args.Length>1&&!framework?args[1]:null;
    if(framework){
     Invoke("AddModule","vision.image-source");
     var image=document.CurrentProcedure.Nodes.Last();var imageValues=image.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());
     var imagePath=System.IO.Path.GetFullPath("examples/assets/circle-sample.bmp");
     imageValues["ImagePaths"]=System.Text.Json.JsonSerializer.SerializeToElement(System.Text.Json.JsonSerializer.Serialize(new[]{imagePath}));
     imageValues["CurrentImagePath"]=System.Text.Json.JsonSerializer.SerializeToElement(imagePath);document.SetParameters(image.Id,imageValues);
     using var evidence=System.Text.Json.JsonDocument.Parse(System.IO.File.ReadAllText("docs/plugins/read-calib-file-evidence.json"));
     var xml=evidence.RootElement.GetProperty("fixtures").EnumerateArray().Single(f=>f.GetProperty("name").GetString()=="rich.xml").GetProperty("xml").GetString()!;
     var directory=System.IO.Path.GetFullPath(System.IO.Path.Combine("analysis/manual-acceptance/fixtures",Guid.NewGuid().ToString("N")));System.IO.Directory.CreateDirectory(directory);
     sample=System.IO.Path.Combine(directory,"原厂受控标定样本.xml");System.IO.File.WriteAllText(sample,xml);
    }
    Invoke("AddModule",id);
    if(id=="vision.read-calib-file"&&sample is not null){
     var file=System.IO.Path.GetFullPath(sample);
     var loaded=VisionPlugins.Calibration.ReadCalibFileData.Read(file);
     if(loaded.ErrorCode!=0||loaded.Data is null)throw new InvalidOperationException("验收样本加载失败："+loaded.ErrorCode);
     var node=document.CurrentProcedure.Nodes.Last();var values=node.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());
     values["FilePath"]=System.Text.Json.JsonSerializer.SerializeToElement(file);
     values["OutputSchema"]=System.Text.Json.JsonSerializer.SerializeToElement(VisionPlugins.Calibration.ReadCalibFilePlugin.Schema(loaded.Data));
     document.SetParameters(node.Id,values);
    }
    Console.WriteLine("MANUAL_ACCEPTANCE_READY "+id);
    Invoke("OpenSelected");
   }));
   return app.Run(window);
  }catch(Exception e){Console.Error.WriteLine(e);return 1;}
 }
 sealed class CameraPoints:VisionFramework.IModule {
  public VisionFramework.ModuleDescriptor Descriptor=>new("acceptance.camera-points","1","验收点阵（测试夹具）",[],new[]{"Object.X","Object.Y","Target.X","Target.Y"}.Select(n=>new VisionFramework.PortDescriptor(n,VisionFramework.DataKind.NumberArray)).ToArray(),[]);
  public ValueTask<VisionFramework.ModuleResult> ExecuteAsync(VisionFramework.ModuleExecutionContext context,CancellationToken token)=>ValueTask.FromResult(new VisionFramework.ModuleResult(VisionFramework.ModuleStatus.Success,new Dictionary<string,VisionFramework.PortValue>{{"Object.X",VisionFramework.PortValue.Numbers(new double[]{0,10,0,10})},{"Object.Y",VisionFramework.PortValue.Numbers(new double[]{0,0,10,10})},{"Target.X",VisionFramework.PortValue.Numbers(new double[]{5,25,5,25})},{"Target.Y",VisionFramework.PortValue.Numbers(new double[]{-7,-7,23,23})}}));
 }
}
