using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using VisionCore;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
static class Program{
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;static int checks;
 static T Field<T>(object x,string n)=>(T)x.GetType().GetField(n,Private)!.GetValue(x)!;
 static object? Call(object x,string n,params object[] args)=>x.GetType().GetMethod(n,Private)!.Invoke(x,args);
 static void Check(bool c,string s){if(!c)throw new Exception(s);Console.WriteLine($"PASS {++checks}: {s}");}
 static IEnumerable<T> Visuals<T>(DependencyObject parent) where T:DependencyObject{for(int i=0;i<System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);i++){var child=System.Windows.Media.VisualTreeHelper.GetChild(parent,i);if(child is T item)yield return item;foreach(var nested in Visuals<T>(child))yield return nested;}}
 static void Pump()=>Application.Current.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);
 static void Wait(Task task){var frame=new DispatcherFrame();_=task.ContinueWith(_=>Application.Current.Dispatcher.BeginInvoke(()=>frame.Continue=false));if(!task.IsCompleted)Dispatcher.PushFrame(frame);task.GetAwaiter().GetResult();}
 static void Until(Func<bool> ready){var frame=new DispatcherFrame();var started=DateTime.UtcNow;var timer=new DispatcherTimer(DispatcherPriority.ApplicationIdle){Interval=TimeSpan.FromMilliseconds(20)};timer.Tick+=(_,_)=>{if(ready()||DateTime.UtcNow-started>TimeSpan.FromSeconds(40))frame.Continue=false;};timer.Start();if(!ready())Dispatcher.PushFrame(frame);timer.Stop();if(!ready())throw new TimeoutException("Configuration run did not complete");}
 [STAThread]static int Main(string[] args){try{
 if(args.Length!=1)throw new ArgumentException("Fresh output directory required");Directory.CreateDirectory(args[0]);
 var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(app.Dispatcher));
 var shell=new MainWindow{Left=-20000,Top=-20000,ShowActivated=false,ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual};shell.Show();try{
 Check(Field<ToolPluginRegistry>(shell,"plugins").Find(CoordinateModulePlugin.Id) is CoordinateModulePlugin,"real shell Coordinate registration without injection");
 var doc=Field<GraphEditorDocument>(shell,"doc");doc.NewSolution();var source=doc.AddNode("vision.image-source","1.0.0",0,0);var tool=doc.AddNode(CoordinateModulePlugin.Id,"1.0.0",200,0);doc.ConnectExecution(source,tool);doc.BindInput(source,"Image",tool,"InputImage");
 var image=Path.GetFullPath("analysis/distortion-calibration/trace-coded-boards/fixture.bmp");Check(File.Exists(image),"reuse original image fixture without new native probe");
 var sv=doc.CurrentProcedure.Nodes.Single(n=>n.Id==source).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());sv["ImagePaths"]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(new[]{image}));sv["CurrentImagePath"]=JsonSerializer.SerializeToElement(image);doc.SetParameters(source,sv);
 NodeDocument Node()=>doc.CurrentProcedure.Nodes.Single(n=>n.Id==tool);
 doc.SetProcedureToolConfigurationRoiGroup(doc.CurrentProcedureId,tool,"RoiType",Node().Parameters,[new("origin",RoiKind.Coordinate,200,180,AngleDegrees:0)]);
 void Run(){Wait((Task)Call(shell,"RunProcedure",doc.CurrentProcedureId)!);Call(shell,"RefreshAll");Pump();}
 NodeRunRecord Result()=>Field<Dictionary<string,NodeRunRecord>>(shell,"results")[tool];
 shell.GetType().GetField("selected",Private)!.SetValue(shell,tool);Run();Check(Result().Status==NodeRunStatus.Success&&Result().Outputs["CoordinateOriPointX"].As<double>()==200,"actual workflow publishes source ROI origin");
 Call(shell,"OpenToolConfiguration",tool);Pump();var window=(ToolConfigurationModelWindow)Field<ToolConfigurationWindowHost>(shell,"toolConfigurationWindows").Current!;
 Check(window.ConfigurationView.Tabs.Items.Count==1,"exact original one-page configuration");foreach(var diagnostic in window.ConfigurationView.Diagnostics)Console.WriteLine("CONFIG DIAGNOSTIC: "+diagnostic);Check(window.ConfigurationView.Diagnostics.Count==0,"all active original templates and operations resolved");
 var session=Field<ToolConfigurationSession>(window,"session");
 bool IsImage(System.Windows.Controls.ComboBoxItem item)=>item.Tag is ConfigurationSubscriptionTreeNode{Binding.Reference:ConfigurationOutputReference reference}&&reference.NodeId==source&&reference.PortId=="Image";
 var imageCombo=Visuals<System.Windows.Controls.ComboBox>(window).Single(c=>c.Items.OfType<System.Windows.Controls.ComboBoxItem>().Any(IsImage));
 Check(!imageCombo.IsDropDownOpen&&imageCombo.Items.OfType<System.Windows.Controls.ComboBoxItem>().Any(IsImage)&&session.State.Find("ImageSource")!.SubscriptionTree is not null,"actual pre-dropdown IMAGE options consume typed initialization tree");
 imageCombo.IsDropDownOpen=true;Pump();imageCombo.IsDropDownOpen=false;Pump();
 Check(imageCombo.Items.OfType<System.Windows.Controls.ComboBoxItem>().Any(IsImage),"actual dropdown refresh retains upstream image candidate identity");var lease=Field<IToolConfigurationRoiSession>(shell,"configurationRoiLease");lease.Refresh();Check(lease.Snapshot.Image?.Width==1000&&lease.Editor.Regions.Count==1,"actual input-image Coordinate ROI lease restored");Check(lease.Parts.Viewport.Parent is System.Windows.Controls.Grid,"actual viewport mounted in shell");
 void Move(double fromX,double fromY,double x,double y){var before=Result();lease.Editor.BeginPointer(new(fromX,fromY));lease.Editor.EndPointer(new(x,y));Until(()=>Field<int>(shell,"activeConfigurationRuns")==0&&!ReferenceEquals(before,Result()));lease.Refresh();}
 Move(200,180,220,190);Check(Node().Rois!.Single().CenterX==220&&Result().Outputs["CoordinateOriPointY"].As<double>()==190,"actual ROI pointer edit commits pixels and triggers run");Move(220,190,200,180);
 void Mode(string name,string value){session.State.CommitUser(name,value);Until(()=>!session.State.IsBusy&&Field<int>(shell,"activeConfigurationRuns")==0);Pump();}
 Mode("CordinateCreate","1");
 var inputRadios=Visuals<System.Windows.Controls.RadioButton>(window).GroupBy(r=>r.GroupName).Single(g=>g.Count()==3).ToArray();
 Check(inputRadios.All(r=>r.IsEnabled)&&inputRadios[0].IsChecked==true,"actual source RadioSelecter3 exposes all three defined input modes");
 inputRadios[2].IsChecked=true;Until(()=>!session.State.IsBusy&&Field<int>(shell,"activeConfigurationRuns")==0);Check(Node().Parameters["InputWay"].GetInt32()==2,"actual third radio commits Matrix input mode through configuration");
 inputRadios=Visuals<System.Windows.Controls.RadioButton>(window).GroupBy(r=>r.GroupName).Single(g=>g.Count()==3).ToArray();inputRadios[0].IsChecked=true;Until(()=>!session.State.IsBusy&&Field<int>(shell,"activeConfigurationRuns")==0);Check(Node().Parameters["InputWay"].GetInt32()==0,"actual first radio commits POINT input mode");
 inputRadios=Visuals<System.Windows.Controls.RadioButton>(window).GroupBy(r=>r.GroupName).Single(g=>g.Count()==3).ToArray();inputRadios[1].IsChecked=true;Until(()=>!session.State.IsBusy&&Field<int>(shell,"activeConfigurationRuns")==0);Check(Node().Parameters["InputWay"].GetInt32()==1,"actual second radio commits XY mode and preserves lifecycle");
 Check(window.ConfigurationView.Diagnostics.Count==0,"RoiDisplayOperation refresh is typed and handled during mode transition");
 Check(Result().Outputs["ModuStatus"].As<long>()==0,"subscribe with missing point source publishes source inner-failure status");
 Check(Result().Outputs["CoordinateOriPointX"].As<double>()==200&&Result().Outputs["CoordinateOriPointY"].As<double>()==180&&Result().Outputs.ContainsKey("CorCalibMatrix"),"actual configuration parameter changes preserve prior known pose/matrix across missing-source run");
 Mode("CordinateCreate","0");Run();Check(Result().Status==NodeRunStatus.Success&&Result().Outputs["ModuStatus"].As<long>()==1,"switch back Draw recovers without numeric binding prerequisites");
 var styles=new ToolDisplayObjectStore(CoordinateModulePlugin.DisplayObjects,()=>Node().Parameters,v=>doc.SetParameters(tool,v));var style=styles.Read("Coordinate System")!.Style with{OkColor="#123456",StrokeThickness=3};Check(styles.Write("Coordinate System",style).Accepted,"named coordinate style persists to actual document");Run();
 var plugin=(CoordinateModulePlugin)Field<ToolPluginRegistry>(shell,"plugins").Find(CoordinateModulePlugin.Id)!;
 var display=Field<ToolDisplayImagePresenter>(shell,"toolDisplayImage");Check(display.Frame?.Image?.Width==1000&&display.Frame.Image.Height==1000,"actual frame has source image background, not only ROI lease");var graphic=display.Frame!.Graphics.Single(g=>g.Name=="Coordinate System");Check(graphic.Color=="#123456","actual shell display consumes coordinate named style");Check(graphic.Points.Single().X==200&&graphic.Points.Single().Y==180&&graphic.XVector==new Point2(50,0)&&graphic.YVector==new Point2(0,50),"actual display maps all six source coordinate output leaves");Check(!display.Frame.TextVisible,"no invented ShowText overlay from Data Record object");
 Call(shell,"CloseToolConfiguration");Check(lease.Parts.Viewport.Parent is null&&lease.Editor.ImageOverlayRenderer is null,"close releases viewport and overlay callback");Check(Field<Dictionary<string,IToolConfigurationRoiSession>>(shell,"configurationRoiLeases").Count==0,"close releases actual session dictionary");
 var path=Path.Combine(Path.GetFullPath(args[0]),"coordinate.vmx.json");doc.Save(path);doc.Load(path);Run();Check(Result().Status==NodeRunStatus.Success&&Result().Outputs["CoordinateOriPointY"].As<double>()==180,"whole solution reopens ROI/bindings and runs");
 Check(new ToolDisplayObjectStore(CoordinateModulePlugin.DisplayObjects,()=>Node().Parameters,_=>throw new Exception()).Read("Coordinate System")!.Style==style,"whole solution preserves named style");
 Call(shell,"OpenToolConfiguration",tool);Pump();Check(Field<IToolConfigurationRoiSession>(shell,"configurationRoiLease").Snapshot.Image?.Width==1000,"reopened lease resolves actual source image");Call(shell,"CloseToolConfiguration");
 Console.WriteLine($"PASS {checks} actual Coordinate host checks; requires production registration and does not establish native bit parity");return 0;
 }finally{shell.Close();app.Shutdown();}
 }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
