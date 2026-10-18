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
    if(args.Contains("--calibration-pair"))document.AddNode(VisionPlugins.Calibration.NImagePlugin.Id,"1.0.0",50,50);
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
}
