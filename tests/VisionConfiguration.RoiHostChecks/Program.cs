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
using VisionPlugins.CalibrationBoard;
using VisionUi;

static class Program {
 const BindingFlags Private=BindingFlags.NonPublic|BindingFlags.Instance;static int checks;
 static void Check(bool ok,string message){if(!ok)throw new Exception(message);checks++;Console.WriteLine("PASS "+checks+": "+message);}
 static T Field<T>(object target,string name)=>(T)target.GetType().GetField(name,Private)!.GetValue(target)!;
 static object? Call(object target,string name,params object[] args)=>target.GetType().GetMethod(name,Private)!.Invoke(target,args);
 static void Pump(){Application.Current.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
 static void Capture(FrameworkElement target,string name){target.UpdateLayout();var bitmap=new RenderTargetBitmap((int)Math.Ceiling(target.ActualWidth),(int)Math.Ceiling(target.ActualHeight),96,96,PixelFormats.Pbgra32);bitmap.Render(target);var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(bitmap));using(var output=File.Create("analysis/ordinary-roi/"+name+".png"))png.Save(output);}
 static IEnumerable<T> Visuals<T>(DependencyObject parent) where T:DependencyObject{if(parent is T item)yield return item;for(int i=0;i<VisualTreeHelper.GetChildrenCount(parent);i++)foreach(var child in Visuals<T>(VisualTreeHelper.GetChild(parent,i)))yield return child;}
 static void Wait(Task task){var frame=new DispatcherFrame();_=task.ContinueWith(_=>Application.Current.Dispatcher.BeginInvoke(()=>frame.Continue=false));if(!task.IsCompleted)Dispatcher.PushFrame(frame);task.GetAwaiter().GetResult();}
 static void Drain(MainWindow shell){var watch=System.Diagnostics.Stopwatch.StartNew();while(Field<int>(shell,"activeConfigurationRuns")>0&&watch.ElapsedMilliseconds<30000){Pump();Thread.Sleep(1);}Check(Field<int>(shell,"activeConfigurationRuns")==0,"ROI self-run completed");Pump();}
 static string Fixture(){int w=480,h=400;var pixels=Enumerable.Repeat((byte)255,w*h).ToArray();for(int y=50;y<350;y++)for(int x=60;x<420;x++)pixels[y*w+x]=(byte)((((x-60)/30+(y-50)/30)&1)==0?0:255);var bmp=new BmpBitmapEncoder();bmp.Frames.Add(BitmapFrame.Create(BitmapSource.Create(w,h,96,96,PixelFormats.Gray8,null,pixels,w)));Directory.CreateDirectory("analysis/ordinary-roi");string path=Path.GetFullPath("analysis/ordinary-roi/checker.bmp");using(var file=File.Create(path))bmp.Save(file);return path;}
 [STAThread] static int Main(){try{
  var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(app.Dispatcher));
  var shell=new MainWindow{Left=-20000,Top=-20000,WindowStartupLocation=WindowStartupLocation.Manual,ShowActivated=false,ShowInTaskbar=false};shell.Show();
  try{
   Check(Field<ToolPluginRegistry>(shell,"plugins").Find(CalibrationBoardPlugin.Id) is CalibrationBoardPlugin,"production shell registers board without test injection");
   using(var catalog=JsonDocument.Parse(File.ReadAllText("src/VisionEditor/tool-catalog.json")))Check(catalog.RootElement.GetProperty("Groups").EnumerateArray().SelectMany(g=>g.GetProperty("Items").EnumerateArray()).Single(i=>i.GetProperty("Id").GetString()=="IMVSCalibBoardCalibModu").GetProperty("ModuleId").GetString()==CalibrationBoardPlugin.Id,"source toolbox item maps to production board plugin");
   var doc=Field<GraphEditorDocument>(shell,"doc");doc.NewSolution();var source=doc.AddNode("vision.image-source","1.0.0",0,0);var board=doc.AddNode(CalibrationBoardPlugin.Id,"1.0.0",200,0);doc.ConnectExecution(source,board);doc.BindInput(source,"Image",board,"InputImage");
   var values=doc.CurrentProcedure.Nodes.Single(n=>n.Id==source).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());string path=Fixture();values["ImagePaths"]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(new[]{path}));values["CurrentImagePath"]=JsonSerializer.SerializeToElement(path);doc.SetParameters(source,values);
   shell.GetType().GetField("selected",Private)!.SetValue(shell,board);Call(shell,"RefreshAll");Wait((Task)Call(shell,"RunProcedure",doc.CurrentProcedureId)!);var records=Field<Dictionary<string,NodeRunRecord>>(shell,"results");Check(records[board].Status==NodeRunStatus.Success,"actual source to board graph runs: "+records[board].Message);
   Call(shell,"OpenToolConfiguration",board);Pump();var windows=Field<ToolConfigurationWindowHost>(shell,"toolConfigurationWindows");var window=(ToolConfigurationModelWindow)windows.Current!;
   Check(window.Width==420&&window.Height==500&&window.ConfigurationView.Tabs.Items.Count==3,"board uses ordinary source model window");
   Check(!window.ConfigurationView.Diagnostics.Any(d=>d.TemplateKey=="ROISelecter"),"ROI source template is registered");
   Check(window.ConfigurationView.Diagnostics.Count==0,"source board window has no unresolved operations: "+JsonSerializer.Serialize(window.ConfigurationView.Diagnostics));
   foreach(var tab in window.ConfigurationView.Tabs.Items){window.ConfigurationView.Tabs.SelectedItem=tab;Pump();Check(window.ConfigurationView.Diagnostics.Count==0,"each source tab materializes without unknown templates");}
   window.ConfigurationView.Tabs.SelectedIndex=1;Pump();
   var range=Visuals<ConfigurationNumericSpinner>(window).Where(x=>(x.Tag as ConfigurationNodeState)?.Name is "EdgeThreshLow" or "EdgeThreshHigh").ToArray();
   Check(range.Length==2,"source range owns two real Integer members");
   var low=range.Single(x=>((ConfigurationNodeState)x.Tag).Name=="EdgeThreshLow");var high=range.Single(x=>((ConfigurationNodeState)x.Tag).Name=="EdgeThreshHigh");
   Check(low.Maximum==double.Parse(((ConfigurationNodeState)high.Tag).Value)&&high.Minimum==double.Parse(((ConfigurationNodeState)low.Tag).Value),"range member limits prevent endpoint crossing");
   window.ConfigurationView.Tabs.SelectedIndex=0;Pump();
   var configuration=Field<ToolConfigurationSession>(window,"session");var imageState=configuration.State.Find("ImageSource")!;
   Check(configuration.Localize("Shape")=="形状"&&configuration.Localize("Calibration Point")=="标定点"&&configuration.Localize("Tab_Image Display")=="图像显示","board captions use shipped Chinese resource values");
   Check(imageState.Value.Contains(".Image[]")&&configuration.State.Diagnostics.Count==0,"source initialization resolves existing image binding and has no operation errors: "+JsonSerializer.Serialize(configuration.State.Diagnostics));
   window.UpdateLayout();var capture=new RenderTargetBitmap((int)window.ActualWidth,(int)window.ActualHeight,96,96,PixelFormats.Pbgra32);capture.Render(window);var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(capture));using(var file=File.Create("analysis/ordinary-roi/board-common-window.png"))encoder.Save(file);
   var lease=Field<IToolConfigurationRoiSession>(shell,"configurationRoiLease");Check(lease.Snapshot.Image?.Width==480&&lease.Snapshot.Image.Height==400,"external ROI reads actual subscribed image dimensions");
   Check(lease.Parts.Viewport.Parent is System.Windows.Controls.Grid,"ROI viewport is in actual shell image pane");
   Check(lease.Editor.Regions.Count==0,"source full image starts without a fabricated rectangle");
   var provider=(CalibrationBoardPlugin)Field<ToolPluginRegistry>(shell,"plugins").Find(CalibrationBoardPlugin.Id)!;
   ToolDisplayFrame Display()=>provider.LastDisplay(doc.CurrentProcedure.Nodes.Single(n=>n.Id==board),doc.BaseDirectory,Field<ModuleExecutionScope>(shell,"executionScope").ForProcedure(doc.CurrentProcedureId))!;
   var displayed=Display();Check(displayed.Image?.Width==480&&displayed.Graphics.Single(g=>g.Name=="Display Calibration Point").Points.Count==99,"production snapshot displays input image and 99 checker points");
   Check(displayed.Graphics.Any(g=>g.Name=="Display Original Calibration Point")&&displayed.Graphics.Any(g=>g.Name=="Coordinate System"),"source origin and coordinate graphics are present");
   Check(displayed.RecordText.Contains("CalibPointNum:99")&&displayed.RecordText.Contains("TransX:")&&displayed.RecordText.Split(Environment.NewLine).Length==2,"both source datarecord mappings reach actual display");
   Check(displayed.ResultList.Count==10&&displayed.ResultList.Single(c=>c.Column=="CalibPointNum").Value=="99","source result columns reach host frame");
   window.ConfigurationView.Tabs.SelectedIndex=2;Pump();var styles=Visuals<RoiDisplayStyleEditor>(window).ToArray();Check(styles.Length==2,"source result page creates two independent point object editors");
   styles[0].Eye.IsChecked=false;styles[0].Eye.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));Pump();Check(!Display().Graphics.Any(g=>g.Name=="Display Calibration Point")&&Display().Graphics.Any(g=>g.Name=="Display Original Calibration Point"),"real eye control hides calibration points independently of origin");
   styles[0].Eye.IsChecked=true;styles[0].Eye.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));Pump();Check(Display().Graphics.Any(g=>g.Name=="Display Calibration Point"),"eye control restores points without executing algorithm");styles[0].OpacityField.Step(-1);Pump();Check(Display().Graphics.Single(g=>g.Name=="Display Calibration Point").Opacity==.99&&Display().Graphics.Single(g=>g.Name=="Display Original Calibration Point").Opacity==1,"source opacity edit affects only its bound display object");Capture(window,"board-result-page");window.ConfigurationView.Tabs.SelectedIndex=0;Pump();Capture(shell,"board-production-shell");
   long before=doc.Revision;lease.Editor.SetDrawKind(RoiKind.Rectangle);lease.Editor.BeginPointer(new(20,20));lease.Editor.EndPointer(new(460,380));Drain(shell);
   var configured=doc.CurrentProcedure.Nodes.Single(n=>n.Id==board);var region=configured.Rois!.Single();Check(region.CenterX==240&&region.CenterY==200&&region.Width==440&&region.Height==360,"external draw persists image pixel coordinates");Check(configured.RoiGroups!["RoiType"].Single().Width==440,"named ROI source persists alongside legacy single-source ROI data");
   Check(doc.Revision==before+1,"ROI geometry and parameters share one undo edit");
   Check(Field<Dictionary<string,Dictionary<string,NodeRunRecord>>>(shell,"configurationResults")[doc.CurrentProcedureId][board].Status==NodeRunStatus.Success,"ROI commit executes real board runtime");
   doc.Undo();Pump();Check((doc.CurrentProcedure.Nodes.Single(n=>n.Id==board).Rois?.Count??0)==0&&lease.Editor.Regions.Count==0,"undo refreshes external geometry without another write");
   doc.Redo();Pump();Check(lease.Editor.Regions.Single().Width==440,"redo restores authoritative ROI");
   var boardNode=doc.CurrentProcedure.Nodes.Single(n=>n.Id==board);doc.SetProcedureToolConfigurationRoiGroup(doc.CurrentProcedureId,board,"RoiType_1",boardNode.Parameters,new[]{new RoiDocument("second",RoiKind.Rectangle,80,90,30,40)});var grouped=doc.CurrentProcedure.Nodes.Single(n=>n.Id==board);Check(grouped.RoiGroups!.Count==2&&grouped.RoiGroups["RoiType_1"].Single().Width==30&&grouped.Rois!.Single().Width==440,"second named ROI source persists without replacing first/legacy source");string groupedPath=Path.GetFullPath("analysis/ordinary-roi/grouped-solution.json");SolutionStore.Save(groupedPath,doc.Document);var reloaded=SolutionStore.Load(groupedPath).Procedures.Single().Nodes.Single(n=>n.Id==board);Check(reloaded.RoiGroups!.Count==2&&reloaded.RoiGroups["RoiType_1"].Single().Width==30&&reloaded.Rois!.Single().Width==440,"solution serialization round-trips named ROI groups and legacy ROI data");doc.Undo();Pump();Check(doc.CurrentProcedure.Nodes.Single(n=>n.Id==board).RoiGroups!.Count==1,"Undo removes only the second ROI source group");doc.Redo();Pump();
   FieldInfo locked=shell.GetType().GetField("isLocked",Private)!;locked.SetValue(shell,true);lease.Refresh();before=doc.Revision;lease.Editor.Select(0);lease.Editor.DeleteSelected();Pump();Check(doc.Revision==before&&lease.Editor.Regions.Count==1,"locked host rejects and restores attempted ROI delete");locked.SetValue(shell,false);lease.Refresh();
   lease.Editor.Select(0);lease.Editor.DeleteSelected();Drain(shell);Check(doc.CurrentProcedure.Nodes.Single(n=>n.Id==board).Rois?.Count==0,"single ROI deletion restores full image");
   before=doc.Revision;Call(shell,"CloseToolConfiguration");Check(lease.Parts.Viewport.Parent is null,"closing ordinary window detaches external drawing surface");lease.Editor.SetDrawKind(RoiKind.Rectangle);lease.Editor.BeginPointer(new(1,1));lease.Editor.EndPointer(new(40,40));Check(doc.Revision==before,"retained closed ROI callback cannot edit document");
   var persisted=doc.CurrentProcedure.Nodes.Single(n=>n.Id==board);doc.SetProcedureToolConfigurationRoiGroup(doc.CurrentProcedureId,board,"RoiType",persisted.Parameters,[new RoiDocument("rotated",RoiKind.Rectangle,240,200,420,340,AngleDegrees:5)]);
   string saved=Path.GetFullPath("analysis/ordinary-roi/board-host.vmx.json");doc.Save(saved);doc.Load(saved);Wait((Task)Call(shell,"RunProcedure",doc.CurrentProcedureId)!);Check(Field<Dictionary<string,NodeRunRecord>>(shell,"results")[board].Status==NodeRunStatus.Success,"save/reopen and actual workflow execute rotated checker ROI");
   Call(shell,"OpenToolConfiguration",board);Pump();var reopened=Field<IToolConfigurationRoiSession>(shell,"configurationRoiLease");Check(reopened.Editor.Regions.Single().AngleDegrees==5,"ordinary host restores saved rotated ROI");
   reopened.Editor.Select(0);reopened.Editor.DeleteSelected();Drain(shell);reopened.Editor.SetDrawKind(RoiKind.Rectangle);reopened.Editor.BeginPointer(new(1,1));reopened.Editor.EndPointer(new(70,70));Drain(shell);
   Check(Field<Dictionary<string,Dictionary<string,NodeRunRecord>>>(shell,"configurationResults")[doc.CurrentProcedureId][board].Status==NodeRunStatus.NoMatch,"actual ROI commit publishes failed recognition");Check(Display().Graphics.Single(g=>g.Name=="ROI").Color.Equals("#ff0000",StringComparison.OrdinalIgnoreCase)&&!Display().Graphics.Any(g=>g.Name=="Display Calibration Point"),"failed host snapshot replaces successful points with NG ROI");
   reopened.Editor.Select(0);reopened.Editor.DeleteSelected();Drain(shell);Check(Display().Graphics.Single(g=>g.Name=="Display Calibration Point").Points.Count==99,"full-image deletion recovers actual success after failure");Call(shell,"CloseToolConfiguration");
   string root=doc.CurrentProcedureId;string group=doc.AddOwnedGroup(500,100);doc.EnterOwnedGroup(group);string body=doc.CurrentProcedureId;var groupSource=doc.AddNode("vision.image-source","1.0.0",0,0);var groupBoard=doc.AddNode(CalibrationBoardPlugin.Id,"1.0.0",200,0);doc.ConnectExecution(groupSource,groupBoard);doc.BindInput(groupSource,"Image",groupBoard,"InputImage");var groupValues=doc.CurrentProcedure.Nodes.Single(n=>n.Id==groupSource).Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());groupValues["ImagePaths"]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(new[]{path}));groupValues["CurrentImagePath"]=JsonSerializer.SerializeToElement(path);doc.SetParameters(groupSource,groupValues);
   doc.SelectProcedure(root);Wait((Task)Call(shell,"RunProcedure",root)!);doc.SelectProcedure(body);Pump();Check(Field<Dictionary<string,NodeRunRecord>>(shell,"results")[groupBoard].Status==NodeRunStatus.Success,"owned Group executes real source to board plugin");Call(shell,"OpenToolConfiguration",groupBoard);Pump();Check(Field<IToolConfigurationRoiSession>(shell,"configurationRoiLease").Snapshot.Image?.Width==480,"Group board ordinary host resolves its own input image");Call(shell,"CloseToolConfiguration");
   Console.WriteLine($"PASS {checks} ordinary ROI host checks; native byte ABI and all shape modes remain separate.");
  }finally{Call(shell,"CloseToolConfiguration");shell.Close();app.Shutdown();}return 0;
 }catch(Exception error){Console.Error.WriteLine(error);return 1;}}
}
