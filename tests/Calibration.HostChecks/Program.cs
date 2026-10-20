using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using VisionCore;
using VisionFramework;
using VisionEditor;
using VisionEditor.Core;
using VisionPluginContracts;
using VisionPlugins.Calibration;

static class Program {
 const BindingFlags Private=BindingFlags.NonPublic|BindingFlags.Instance;
 static int checks;
 static void Check(bool ok,string message){if(!ok)throw new Exception(message);checks++;}
 static T Field<T>(object target,string name)=>(T)target.GetType().GetField(name,Private)!.GetValue(target)!;
 static object? Call(object target,string name,params object[] args)=>target.GetType().GetMethod(name,Private)!.Invoke(target,args);
 static void Capture(Window window,string name){window.UpdateLayout();var image=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);image.Render(window);var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(image));Directory.CreateDirectory("analysis/full-audit/calibration-host");using var output=File.Create("analysis/full-audit/calibration-host/"+name+".png");png.Save(output);}
 static T Wait<T>(Task<T> task){var frame=new DispatcherFrame();_=task.ContinueWith(_=>Application.Current.Dispatcher.BeginInvoke(()=>frame.Continue=false));if(!task.IsCompleted)Dispatcher.PushFrame(frame);return task.GetAwaiter().GetResult();}
 [STAThread] static int Main(){try{
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
  SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(app.Dispatcher));
  var shell=new MainWindow{Left=-20000,Top=-20000,ShowActivated=false,ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual};shell.Show();
  try{
   var doc=Field<GraphEditorDocument>(shell,"doc");var registry=Field<ModuleRegistry>(shell,"modules");var plugins=Field<ToolPluginRegistry>(shell,"plugins");doc.NewSolution();
   Check(plugins.Find(NPointPlugin.Id) is IToolConfigurationProvider,"actual shell registers N-point common provider");
   Check(plugins.Find(NImagePlugin.Id) is IToolConfigurationProvider,"actual shell registers N-image common provider");
   registry.Register(new Source().Descriptor,()=>new Source());var source=doc.AddNode("test.calibration-points","1",0,0);
   var target=doc.AddNode(NPointPlugin.Id,"1.0.0",200,0);doc.ConnectExecution(source,target);
   var values=doc.CurrentProcedure.Nodes.Single(n=>n.Id==target).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());
   values["CalibPointTotalNum"]=JsonSerializer.SerializeToElement(4);values["RotPointTotalNum"]=JsonSerializer.SerializeToElement(0);doc.SetParameters(target,values);
   var node=doc.CurrentProcedure.Nodes.Single(n=>n.Id==target);
   var context=(ToolEditorContext)Call(shell,"ConnectToolEditorServices",new ToolEditorContext(node,doc.BaseDirectory,p=>doc.SetParameters(target,p),_=>throw new NotSupportedException()),target)!;
   var destination=new ConfigurationSubscriptionDestination(ConfigurationSubscriptionDomain.Combination,"Image Point","POINT");
   var binding=new ConfigurationSubscriptionBinding(new ConfigurationOutputReference(doc.CurrentProcedureId,source,"Points","POINT"),ConfigurationSubscriptionIndex.All);
   long revision=doc.Revision;Check(context.ConfigurationSubscriptions!.Bind(destination,binding).Status==ConfigurationSubscriptionStatus.Available,"actual host binds POINT combination");
   Check(doc.Revision==revision+1&&doc.CurrentProcedure.Configuration.Subscriptions.Count==2,"host persists two authoritative leaves in one edit");
   // Populate actual shell result cache with the unexpanded runtime result, as configuration self-run does.
   var cache=Field<Dictionary<string,Dictionary<string,NodeRunRecord>>>(shell,"configurationResults");cache[doc.CurrentProcedureId]=new(){{source,new(source,NodeRunStatus.Success,new Dictionary<string,PortValue>{{"Points",PortValue.PointArray(Source.Points)}},0)}};
   var record=Wait(context.Run(values));Check(record.Status==NodeRunStatus.Success,"configuration self-run resolves POINT leaves from actual unexpanded result cache: "+record.Message);
   Check(record.Outputs.ContainsKey("CalibData"),"self-run reaches real N-point algorithm outputs");
   Call(shell,"OpenToolConfiguration",target);var host=Field<ToolConfigurationWindowHost>(shell,"toolConfigurationWindows");var window=host.Current as ToolConfigurationModelWindow;
   Check(window is not null&&window.ConfigurationView.Tabs.Items.Count==3,"N-point opens original three tabs through actual shell");
   Capture(window!,"npoint-common-window");
   Call(shell,"CloseToolConfiguration");
   var image=doc.AddNode(NImagePlugin.Id,"1.0.0",400,0);Call(shell,"OpenToolConfiguration",image);window=host.Current as ToolConfigurationModelWindow;
   Check(window is not null&&window.ConfigurationView.Tabs.Items.Count==3,"N-image opens original three tabs through actual shell");
   Check(window!.Width==420&&window.Height==500,"N-image uses ordinary common configuration geometry");
   Capture(window,"nimage-common-window");
   Call(shell,"CloseToolConfiguration");Check(host.Current is null,"actual host releases calibration window");
   Console.WriteLine($"PASS {checks} actual calibration host checks; native desktop parity remains separate.");
  }finally{shell.Close();app.Shutdown();}return 0;
 }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
 sealed class Source:IModule {
  public static readonly Point2[] Points=[new(10,20),new(110,20),new(210,20),new(210,120)];
  public ModuleDescriptor Descriptor=>new("test.calibration-points","1","points",[],[new PortDescriptor("Points",DataKind.Point){Cardinality=ValueCardinality.Array}],[]);
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"Points",PortValue.PointArray(Points)}}));
 }
}
