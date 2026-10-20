using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.DistortionCalibration;
using VisionUi;

static class Program {
 const BindingFlags Private=BindingFlags.NonPublic|BindingFlags.Instance;
 static int checks;
 static void Check(bool ok,string message){if(!ok)throw new Exception(message);Console.WriteLine($"PASS {++checks}: {message}");}
 static T Field<T>(object value,string name)=>(T)value.GetType().GetField(name,Private)!.GetValue(value)!;
 static object? Call(object value,string name,params object[] args)=>value.GetType().GetMethod(name,Private)!.Invoke(value,args);
 static void Pump()=>Application.Current.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);
 static void Wait(Task task){var frame=new DispatcherFrame();_=task.ContinueWith(_=>Application.Current.Dispatcher.BeginInvoke(()=>frame.Continue=false));if(!task.IsCompleted)Dispatcher.PushFrame(frame);task.GetAwaiter().GetResult();}
 static void Until(Func<bool> ready){var frame=new DispatcherFrame();var started=DateTime.UtcNow;var timer=new DispatcherTimer(DispatcherPriority.ApplicationIdle){Interval=TimeSpan.FromMilliseconds(30)};timer.Tick+=(_,_)=>{if(ready()||DateTime.UtcNow-started>TimeSpan.FromMinutes(2))frame.Continue=false;};timer.Start();if(!ready())Dispatcher.PushFrame(frame);timer.Stop();if(!ready())throw new TimeoutException("ROI-triggered run did not complete");}
 static IEnumerable<Drawing> Drawings(Drawing drawing)=>new[]{drawing}.Concat(drawing is DrawingGroup group?group.Children.SelectMany(Drawings):[]);
 static void CheckOverlay(IToolConfigurationRoiSession lease){
  var original=lease.Snapshot;var frame=original.Display??throw new Exception("Actual ROI display missing");
  var test=frame with{TextVisible=false,Graphics=[new("fixture","path","#0000ff",1,2,[new(10,20,"",0){Color="#00ff00"},new(30,40,"",1){Color="#ff0000"}]),new("named error","text","#123456",1,1,[new(15,25,""){Text="123"}]){FontSize=23}]};
  lease.GetType().GetProperty("Snapshot")!.SetValue(lease,original with{Display=test});
  try{var drawing=new DrawingGroup();using(var dc=drawing.Open())lease.GetType().GetMethod("DrawOverlay",Private)!.Invoke(lease,[dc,Matrix.Identity,1d]);var items=Drawings(drawing).ToArray();var colors=items.OfType<GeometryDrawing>().Select(d=>d.Pen?.Brush.ToString().ToLowerInvariant()).ToArray();Check(colors.Contains("#ff0000ff")&&colors.Contains("#ff00ff00")&&colors.Contains("#ffff0000"),"actual ROI overlay draws path line plus separate OK/NG marker pens");Check(items.OfType<GlyphRunDrawing>().Any(d=>d.GlyphRun.FontRenderingEmSize==23&&d.ForegroundBrush.ToString().Equals("#FF123456",StringComparison.OrdinalIgnoreCase)),"actual ROI overlay draws named text glyphs with configured font size");}
  finally{lease.GetType().GetProperty("Snapshot")!.SetValue(lease,original);}
 }
 [STAThread] static int Main(string[] args){try{
  if(args.Length!=1)throw new ArgumentException("Fresh output directory required");Directory.CreateDirectory(args[0]);
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(app.Dispatcher));
  var shell=new MainWindow{Left=-20000,Top=-20000,WindowStartupLocation=WindowStartupLocation.Manual,ShowActivated=false,ShowInTaskbar=false};shell.Show();
  try{
   Check(Field<ToolPluginRegistry>(shell,"plugins").Find(DistortionCalibrationPlugin.Id) is DistortionCalibrationPlugin,"actual shell registers distortion without injection");
   var doc=Field<GraphEditorDocument>(shell,"doc");doc.NewSolution();var source=doc.AddNode("vision.image-source","1.0.0",0,0);var tool=doc.AddNode(DistortionCalibrationPlugin.Id,"1.0.0",200,0);doc.ConnectExecution(source,tool);doc.BindInput(source,"Image",tool,"InputImage");
   var values=doc.CurrentProcedure.Nodes.Single(n=>n.Id==source).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());var image=Path.GetFullPath("analysis/distortion-calibration/trace-coded-boards/fixture.bmp");Check(File.Exists(image),"existing original-board fixture available");values["ImagePaths"]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(new[]{image}));values["CurrentImagePath"]=JsonSerializer.SerializeToElement(image);doc.SetParameters(source,values);
   var parameters=doc.CurrentProcedure.Nodes.Single(n=>n.Id==tool).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());parameters["CalibBoardType"]=JsonSerializer.SerializeToElement(7);parameters["DistortionEvaluationEnable"]=JsonSerializer.SerializeToElement(true);parameters["OutputPostCorrectionEnable"]=JsonSerializer.SerializeToElement(true);doc.SetParameters(tool,parameters);
   shell.GetType().GetField("selected",Private)!.SetValue(shell,tool);Call(shell,"RefreshAll");Wait((Task)Call(shell,"RunProcedure",doc.CurrentProcedureId)!);
   Check(Field<Dictionary<string,NodeRunRecord>>(shell,"results")[tool].Status==NodeRunStatus.Success,"actual source to distortion workflow succeeds");
   Call(shell,"OpenToolConfiguration",tool);Pump();var window=(ToolConfigurationModelWindow)Field<ToolConfigurationWindowHost>(shell,"toolConfigurationWindows").Current!;
   Check(window.Width==420&&window.Height==500&&window.ConfigurationView.Tabs.Items.Count==3,"source model window and three tabs");
   foreach(var tab in window.ConfigurationView.Tabs.Items){window.ConfigurationView.Tabs.SelectedItem=tab;Pump();Check(window.ConfigurationView.Diagnostics.Count==0,"source tab has no unresolved templates");}
   window.ConfigurationView.Tabs.SelectedIndex=0;Pump();var lease=Field<IToolConfigurationRoiSession>(shell,"configurationRoiLease");lease.Refresh();if(lease.Snapshot.Image is null){Console.WriteLine("ROI bindings: "+JsonSerializer.Serialize(doc.CurrentProcedure));var sessions=Field<Dictionary<string,ExecutionSessionResult>>(shell,"workflowSessions");Console.WriteLine("sessions: "+string.Join(",",sessions.Keys));if(sessions.TryGetValue(doc.CurrentProcedureId,out var completed)){Console.WriteLine("cached: "+string.Join(",",completed.Nodes.Select(n=>n.NodeId+":"+n.Status)));var resolved=new NodeDebugExecutor(Field<ModuleRegistry>(shell,"modules")).ResolveInput(doc.CurrentProcedure,tool,"InputImage",completed.Nodes);Console.WriteLine("resolved "+resolved.Kind+" "+resolved.Value?.GetType());}}Check(lease.Snapshot.Image?.Width==1000&&lease.Snapshot.Image.Height==1000,"actual ROI host resolves subscribed image: "+(lease.Snapshot.Image is {} actualImage?$"{actualImage.Width}x{actualImage.Height}":"null"));Check(lease.Parts.Viewport.Parent is System.Windows.Controls.Grid,"ROI viewport attaches to actual shell");
   CheckOverlay(lease);
   void Edit(Action action){var before=Field<Dictionary<string,NodeRunRecord>>(shell,"results")[tool];action();Until(()=>Field<int>(shell,"activeConfigurationRuns")==0&&!ReferenceEquals(before,Field<Dictionary<string,NodeRunRecord>>(shell,"results")[tool]));lease.Refresh();}
   Edit(()=>{lease.Editor.SetDrawKind(RoiKind.Rectangle);lease.Editor.BeginPointer(new(0,0));lease.Editor.EndPointer(new(80,80));});
   Check(doc.CurrentProcedure.Nodes.Single(n=>n.Id==tool).Rois?.Count==1,"actual ROI drawing commits geometry into workflow node");
   var failed=Field<Dictionary<string,NodeRunRecord>>(shell,"results")[tool];Check(failed.Status!=NodeRunStatus.Success&&failed.Outputs["ModuStatus"].As<long>()==0,"corner ROI triggers real node failure and current status zero");
   Edit(()=>lease.Editor.DeleteSelected());Check(Field<Dictionary<string,NodeRunRecord>>(shell,"results")[tool].Status==NodeRunStatus.Success,"removing ROI restores full-image successful run");
   var styles=new ToolDisplayObjectStore(DistortionCalibrationMetadata.DisplayObjects,()=>doc.CurrentProcedure.Nodes.Single(n=>n.Id==tool).Parameters,next=>doc.SetParameters(tool,next));
   var pointStyle=styles.Read("Display Calibration Point")!.Style with{OkColor="#123456",StrokeThickness=3};Check(styles.Write("Display Calibration Point",pointStyle).Accepted,"actual workflow node accepts display point style");
   Call(shell,"CloseToolConfiguration");Check(lease.Parts.Viewport.Parent is null&&lease.Editor.ImageOverlayRenderer is null,"configuration close detaches viewport and clears overlay delegate");
   Check(Field<Dictionary<string,IToolConfigurationRoiSession>>(shell,"configurationRoiLeases").Count==0,"configuration close releases all actual ROI leases");
   var saved=Path.GetFullPath(Path.Combine(args[0],"host.vmx.json"));doc.Save(saved);doc.Load(saved);Wait((Task)Call(shell,"RunProcedure",doc.CurrentProcedureId)!);Check(Field<Dictionary<string,NodeRunRecord>>(shell,"results")[tool].Status==NodeRunStatus.Success,"saved workflow reopens and reruns");
   Call(shell,"OpenToolConfiguration",tool);Pump();var reopened=Field<IToolConfigurationRoiSession>(shell,"configurationRoiLease");Check(reopened.Snapshot.Image?.Width==1000,"reopened configuration restores real image subscription");
   var restored=new ToolDisplayObjectStore(DistortionCalibrationMetadata.DisplayObjects,()=>doc.CurrentProcedure.Nodes.Single(n=>n.Id==tool).Parameters,_=>throw new InvalidOperationException());Check(restored.Read("Display Calibration Point")!.Style==pointStyle,"whole solution save-load preserves point style");
   Check(reopened.Snapshot.Display!.Graphics.Where(g=>g.Name=="Display Calibration Point").SelectMany(g=>g.Points).Any(p=>p.Color?.Equals("#123456",StringComparison.OrdinalIgnoreCase)==true),"reopened actual display consumes persisted point color");
   File.WriteAllText(Path.Combine(args[0],"summary.json"),JsonSerializer.Serialize(new{checks,passed=true,manualAcceptance=false}));return 0;
  }finally{Call(shell,"CloseToolConfiguration");shell.Close();app.Shutdown();}
 }catch(Exception error){Console.Error.WriteLine(error);return 1;}}
}
