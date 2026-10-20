using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
static class Program{
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;static int checks;
 static T Field<T>(object x,string n)=>(T)x.GetType().GetField(n,Private)!.GetValue(x)!;
 static object? Call(object x,string n,params object[] a)=>x.GetType().GetMethod(n,Private)!.Invoke(x,a);
 static void Check(bool c,string m){if(!c)throw new Exception(m);Console.WriteLine($"PASS {++checks}: {m}");}
 static JsonElement J<T>(T v)=>JsonSerializer.SerializeToElement(v);
 static void Pump()=>Application.Current.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);
 static void Wait(Task t){var f=new DispatcherFrame();_=t.ContinueWith(_=>Application.Current.Dispatcher.BeginInvoke(()=>f.Continue=false));if(!t.IsCompleted)Dispatcher.PushFrame(f);t.GetAwaiter().GetResult();}
 [STAThread]static int Main(string[] args){try{
 if(args.Length!=1)throw new ArgumentException("Fresh output directory required");string output=Path.GetFullPath(args[0]);Directory.CreateDirectory(output);
 var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(app.Dispatcher));
 var shell=new MainWindow{Left=-20000,Top=-20000,ShowInTaskbar=false,ShowActivated=false,WindowStartupLocation=WindowStartupLocation.Manual};shell.Show();try{
 var doc=Field<GraphEditorDocument>(shell,"doc");var plugin=(NImagePlugin)Field<ToolPluginRegistry>(shell,"plugins").Find(NImagePlugin.Id)!;Check(plugin is not null,"actual MainWindow registers NImage plugin");doc.NewSolution();string source=doc.AddNode("vision.image-source","1.0.0",0,0),target=doc.AddNode(NImagePlugin.Id,"1.0.0",200,0);doc.ConnectExecution(source,target);doc.BindInput(source,"Image",target,"InputImage");
 NodeDocument Node(string id)=>doc.CurrentProcedure.Nodes.Single(n=>n.Id==id);var parameters=Node(target).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());parameters["CalibPointTotalNum"]=J(9);parameters["RotPointTotalNum"]=J(0);doc.SetParameters(target,parameters);
 var decoder=BitmapDecoder.Create(new Uri(Path.GetFullPath("tests/NImageCalibration.PluginChecks/Fixtures/hik-i-help-derived.png")),BitmapCreateOptions.PreservePixelFormat,BitmapCacheOption.OnLoad);var bitmap=new FormatConvertedBitmap(decoder.Frames[0],PixelFormats.Gray8,null,0);int width=bitmap.PixelWidth,height=bitmap.PixelHeight;var bytes=new byte[width*height];bitmap.CopyPixels(bytes,width,0);var paths=new List<string>();for(int i=0;i<9;i++){int dx=i%3*10,dy=i/3*10;var data=Enumerable.Repeat((byte)255,bytes.Length).ToArray();for(int y=0;y<height-dy;y++)bytes.AsSpan(y*width,width-dx).CopyTo(data.AsSpan((y+dy)*width+dx,width-dx));var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(BitmapSource.Create(width,height,96,96,PixelFormats.Gray8,null,data,width)));string path=Path.Combine(output,$"frame-{i}.png");using(var stream=File.Create(path))png.Save(stream);paths.Add(path);}
 var sourceValues=Node(source).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());sourceValues["ImagePaths"]=J(JsonSerializer.Serialize(paths));sourceValues["CurrentImagePath"]=J(paths[0]);sourceValues["AutoPlay"]=J(true);doc.SetParameters(source,sourceValues);
 var context=(ToolEditorContext)Call(shell,"ConnectToolEditorServices",new ToolEditorContext(Node(target),doc.BaseDirectory,p=>doc.SetParameters(target,p),_=>throw new NotSupportedException()),target)!;
 void Constant(string name,double value)=>Check(context.ConfigurationSubscriptions!.Bind(new(ConfigurationSubscriptionDomain.InputIo,name,"float"),new(new ConfigurationConstantReference("float",value.ToString(System.Globalization.CultureInfo.InvariantCulture)),ConfigurationSubscriptionIndex.All)).Status==ConfigurationSubscriptionStatus.Available,"actual host binds "+name);
 shell.GetType().GetField("selected",Private)!.SetValue(shell,target);NodeRunRecord Result()=>Field<Dictionary<string,NodeRunRecord>>(shell,"results")[target];
 for(int i=0;i<9;i++){Constant("MovePhysicalPointX",i%3);Constant("MovePhysicalPointY",i/3);Constant("RotateAngle",0);Wait((Task)Call(shell,"RunProcedure",doc.CurrentProcedureId)!);Call(shell,"RefreshAll");Pump();var r=Result();Check(r.Status==NodeRunStatus.Success&&r.Outputs["CurNum"].As<long>()==i+1,"actual image-source workflow frame "+i+": "+r.Message);}
 Check(Result().Outputs.ContainsKey("CalibData")&&Result().Outputs["CalibStatus"].As<long>()==1,"actual workflow produces complete calibration");var presenter=Field<ToolDisplayImagePresenter>(shell,"toolDisplayImage");Check(presenter.Frame!.Graphics.Any(g=>g.Type=="path"&&g.Points.Count==9),"actual shell renders source nine-frame trajectory");Check(presenter.Frame.ImageLayers.Any(l=>l.Name=="InImage"&&l.Image is not null),"actual image layer consumer");
 Call(shell,"OpenToolConfiguration",target);var host=Field<ToolConfigurationWindowHost>(shell,"toolConfigurationWindows");var window=(ToolConfigurationModelWindow)host.Current!;Check(window.Width==420&&window.Height==500&&window.ConfigurationView.Tabs.Items.Count==3,"original three tabs and common host geometry");foreach(var tab in window.ConfigurationView.Tabs.Items){window.ConfigurationView.Tabs.SelectedItem=tab;Pump();Check(window.ConfigurationView.Diagnostics.Count==0,"source templates resolve");}var session=Field<ToolConfigurationSession>(window,"session");Call(shell,"CloseToolConfiguration");Check(host.Current is null&&Field<bool>(session,"disposed"),"actual close releases configuration session");
 var scope=Field<ModuleExecutionScope>(shell,"executionScope").ForProcedure(doc.CurrentProcedureId);var editor=new ToolEditorContext(Node(target),doc.BaseDirectory,_=>{},_=>throw new NotSupportedException()){Scope=scope};string exported=Path.Combine(output,"host-result.xml");plugin!.Export(editor,exported);Check(NImageFile.Read(new StringReader(File.ReadAllText(exported))).ImagePoints.Count==9,"actual shell-owned SDK state exports");string solution=Path.Combine(output,"nimage.vmx.json");doc.Save(solution);doc.Load(solution);Check(Node(target).Parameters["CalibPointTotalNum"].GetInt32()==9,"solution reload preserves tool configuration");
 var freshScope=Field<ModuleExecutionScope>(shell,"executionScope").ForProcedure(doc.CurrentProcedureId);Check(plugin.Inspect(editor with{Node=Node(target),Scope=freshScope}).Current==0,"solution reload releases nonpersistent image batch");Call(shell,"OpenToolConfiguration",target);Check(host.Current is ToolConfigurationModelWindow,"saved tool configuration reopens");Call(shell,"CloseToolConfiguration");
 File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{checks,passed=true,manualAcceptance=false,originalPlatformExecution=false}));return 0;
 }finally{Call(shell,"CloseToolConfiguration");shell.Close();app.Shutdown();}
 }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}