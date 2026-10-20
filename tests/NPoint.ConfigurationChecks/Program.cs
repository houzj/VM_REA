using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
using Application=System.Windows.Application;
using Window=System.Windows.Window;
using Button=System.Windows.Controls.Button;

static class Program {
 static int checks;static void Check(bool condition,string message){if(!condition)throw new Exception(message);checks++;}
 [STAThread]static int Main(){try{Run();Console.WriteLine($"PASS {checks} N-point production configuration checks");return 0;}catch(Exception error){Console.Error.WriteLine(error);return 1;}}
 static void Run(){
  _=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};var output=Path.GetFullPath("analysis/full-audit/n-point-production-configuration");Directory.CreateDirectory(output);
  var plugin=new NPointPlugin();var registry=new ModuleRegistry();new ToolPluginRegistry().Register(plugin,registry);
  using var lifetime=new ModuleExecutionScope();var scope=lifetime.ForProcedure("npoint");
  var values=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());
  var node=new NodeDocument("npoint",NPointPlugin.Id,"1.0.0","N点标定",values);var flow=new ProcedureDocument("npoint","标定",[node],[],[]);
  var saves=new SaveService();var notices=new Notifications();bool reject=false;int runs=0;
  var subscriptions=new WorkflowConfigurationSubscriptionService(()=>flow,registry,node.Id,(destination,reference)=>{
   var remaining=flow.Configuration.Subscriptions.Where(s=>s.NodeId!=node.Id||s.Target!=destination.Name||s.IsParameter!=(destination.Domain==ConfigurationSubscriptionDomain.Parameter));
   flow=flow with{Configuration=flow.Configuration with{Subscriptions=(reference is null?remaining:remaining.Append(new WorkflowSubscription(node.Id,destination.Name,destination.Domain==ConfigurationSubscriptionDomain.Parameter,reference))).ToArray()}};
  });
  async Task<NodeRunRecord> Run(Dictionary<string,JsonElement> p){runs++;return(await new GraphExecutor(registry).RunAsync(flow with{Nodes=[node with{Parameters=p}]},output,scope:lifetime)).Nodes.Single();}
  IToolConfigurationSubscriptionService completeSubscriptions=new ToolConfigurationDisplaySubscriptions(subscriptions,()=>flow,registry,node.Id,()=>values,next=>{if(reject)throw new InvalidOperationException("Document rejected display transaction");values=next;flow=flow with{Nodes=[node with{Parameters=values}]};});
  var context=new ToolEditorContext(node,output,next=>{if(reject)throw new InvalidOperationException("Document rejected transaction");values=next;flow=flow with{Nodes=[node with{Parameters=values}]};},Run){Scope=scope,ConfigurationSubscriptions=completeSubscriptions,ConfigurationSaveFiles=saves,ConfigurationNotifications=notices,ConfigurationExecutionStatus=()=>new(false,false,true,false,true)};
  Check(plugin is IToolConfigurationProvider,"real plugin advertises public configuration model");
  using var session=plugin.CreateConfiguration(context);var state=session.State;
  Check(state.Definition.Tabs.Select(t=>t.Name).SequenceEqual(new[]{"Tab_Basic Params","Tab_Run Params","ResultShow"}),"original three tabs and ordering");
  Check(state.Definition.BottomCommands.Select(t=>t.Name).SequenceEqual(new[]{"ClearPoint","SaveFile","Execute"}),"original bottom commands have no invented continuous or confirm button");
  Check(state.Definition.LeftBottomCommands.Count==1,"original reset remains on left");
  var window=new ToolConfigurationModelWindow(context,session){ShowInTaskbar=false,ShowActivated=false,Left=-30000,Top=-30000,WindowStartupLocation=WindowStartupLocation.Manual};window.Show();window.UpdateLayout();
  Check(window.Width==420&&window.Height==500,"production host uses original default geometry rather than legacy650");
  Check(!window.ConfigurationView.Diagnostics.Any(d=>d.Code=="UnsupportedTemplate"),"all declared N-point leaves have production factories");
  File.WriteAllText(Path.Combine(output,"initial-diagnostics.json"),JsonSerializer.Serialize(state.Diagnostics,new JsonSerializerOptions{WriteIndented=true}));Check(state.UnsupportedOperations.Count==0,"original initializers use supported operation families");
  Check(!state.Diagnostics.Any(d=>d.Code=="MissingRuleNode"||d.Code=="MissingRuleTarget"),"physical visibility rule resolves original nodes");
  Check(state.Find("Tab_Phys Coord Params")!.Visible,"unsubscribed trigger mode shows original physical-coordinate section");
  state.CommitUser("CalibPointGet","1");Check(!state.Find("PicPoint")!.Visible&&!state.Find("TeachEnable")!.Visible&&!state.Find("Tab_Phys Coord Params")!.Visible,"manual acquisition XML hides source inputs and physical section");
  state.CommitUser("CalibPointTotalNum","4");state.CommitUser("RotPointTotalNum","0");Check(values["RotPointTotalNum"].GetInt32()==0&&!state.Find("BaseAngle")!.Visible,"inherited EditButton integer commits and original trigger runs");
  state.CommitUser("CameraMode","1");state.CommitUser("HomoFreedom","2");state.CommitUser("WeightFun","1");
  var points=new[]{(0d,0d),(100d,0d),(100d,100d),(0d,100d)};
  Exception? pointFailure=null;
  System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(()=>{
   Window? dialog=null;try{dialog=Application.Current.Windows.OfType<Window>().Single(w=>w.Content is NPointConfigurationPointEditor);var editor=(NPointConfigurationPointEditor)dialog.Content;
    Check(editor.Rows.Count==4,"source edit button opens real point list");
    for(int i=0;i<4;i++){var cell=new[]{points[i].Item1,points[i].Item2,2*points[i].Item1+3,2*points[i].Item2+4,0d};for(int axis=0;axis<5;axis++)Check(editor.TrySetCell(i,axis,cell[axis].ToString(System.Globalization.CultureInfo.CurrentCulture)),"point editor writes cell through production adapter");}
   }catch(Exception error){pointFailure=error;}finally{dialog?.Close();}
  }));
  Walk(window).OfType<Button>().Single(b=>b.Name=="NPointEditPoints").RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));if(pointFailure is not null)throw pointFailure;
  state.Invoke(state.Find("Execute")!);Pump();
  var frame=plugin.LastDisplay(node with{Parameters=values},output,scope);Check(runs>0&&frame is not null,"source bottom execute reaches real graph and display snapshot");
  File.WriteAllText(Path.Combine(output,"display-run.json"),JsonSerializer.Serialize(new{frame!.Text,frame.RecordText,frame.FontSize,values=values.ToDictionary(p=>p.Key,p=>p.Value.ToString())},new JsonSerializerOptions{WriteIndented=true}));Check(frame!.Text.Contains("CalibProgress:4/4"),"source display content uses actual completed point count");
  window.ConfigurationView.Tabs.SelectedIndex=2;window.ConfigurationView.Refresh();window.UpdateLayout();
  var size=Walk(window).OfType<ConfigurationNumericSpinner>().Single(n=>n.Minimum==6&&n.Maximum==72);int before=runs;size.Editor.Text="18";Check(size.CommitText()&&ToolDisplayService.Read(values,plugin.DisplayDefinition).FontSize==18,"production result spinner writes display object separately from algorithm values");
  Check(plugin.LastDisplay(node with{Parameters=values},output,scope)!.FontSize==18,"display change redraw reads stored object without rerunning algorithm");
  Check(runs==before,"object binding has no implicit algorithm dispatch");
  reject=true;size.Editor.Text="20";size.CommitText();Check(ToolDisplayService.Read(values,plugin.DisplayDefinition).FontSize==18,"rejected display commit retains accepted property");var old=state.Find("CalibPointTotalNum")!.Value;state.CommitUser("CalibPointTotalNum","5");Check(state.Find("CalibPointTotalNum")!.Value==old&&values["CalibPointTotalNum"].GetInt32()==4,"host rejection rolls model and runtime parameter back");reject=false;
  var target=Path.Combine(output,"source-save.xml");saves.Path=target;state.Invoke(state.Find("SaveFile")!);Check(File.Exists(target)&&values["CalibPathName"].GetString()==target,"source save command exports actual solved calibration file");Check(JsonSerializer.Deserialize<Dictionary<string,string>>(values["ConfigurationUserData"].GetString()!)!["SaveCalibPath"]==target,"SaveFileDialog records original user-string key");
  Check(saves.Remembered==target&&notices.Messages.Any(m=>m.Kind==ConfigurationNotificationKind.Success),"successful export records history and notification");
  int messages=notices.Messages.Count;saves.Cancel=true;state.Invoke(state.Find("SaveCalibPath")!);Check(notices.Messages.Count==messages,"hidden SaveFileDialog command supports cancel without export or notification");saves.Cancel=false;
  var previous=values["CalibPathName"].GetString();string invalid=Path.Combine(output,"wrong.txt");state.CommitUser("CalibPathName",invalid,true);Check(state.Find("CalibPathName")!.Value==invalid&&values["CalibPathName"].GetString()==previous,"source invalid typed path retains attempt while backend path remains accepted");
  string upper=Path.Combine(output,"rejected.XML");saves.Path=upper;int notifyCount=notices.Messages.Count;state.Invoke(state.Find("SaveFile")!);Check(!File.Exists(upper)&&values["CalibPathName"].GetString()==previous&&saves.Remembered==target,"source save rejects uppercase suffix before export and history");Check(notices.Messages.Count==notifyCount+1&&notices.Messages.Last().Kind==ConfigurationNotificationKind.Error,"source save suffix rejection reports once");
  window.ConfigurationView.Tabs.SelectedIndex=2;window.ConfigurationView.Refresh();window.UpdateLayout();Check(Walk(window).OfType<ConfigurationObjectTextBox>().Any(),"result tab uses actual object StringConfig control");Capture(window,Path.Combine(output,"result-source-derived.png"));
  window.ConfigurationView.Tabs.SelectedIndex=0;window.ConfigurationView.Refresh();window.UpdateLayout();Capture(window,Path.Combine(output,"manual-source-derived.png"));
  state.Invoke(state.Find("ClearPoint")!);Check(plugin.Snapshot(context with{Node=node with{Parameters=values}})[0,2]==0,"clear command really clears runtime points");
  bool acceptReset=false;int resets=0;
  var positionBinding=new ConfigurationSubscriptionBinding(new ConfigurationConstantReference("float","12.5"),ConfigurationSubscriptionIndex.All);
  Check(completeSubscriptions.Bind(new(ConfigurationSubscriptionDomain.Display,ToolDisplayService.PositionXInput,"float"),positionBinding).Status==ConfigurationSubscriptionStatus.Available,"production display binding is accepted before reset");
  string positionBeforeReset=values[ToolConfigurationDisplaySubscriptions.ParameterName].GetString()!;
  using(var resetSession=plugin.CreateConfiguration(context with{Node=node with{Parameters=values},ConfigurationConfirmations=new WpfConfigurationConfirmationService((_,_)=>acceptReset),ResetConfigurationParameters=next=>{resets++;values=next;}})){
   var reset=resetSession.State.Find("Button_ResetAlgoParams")!;
   var acceptedCount=values["CalibPointTotalNum"].GetInt32();resetSession.State.Invoke(reset);
   Check(resets==0&&values["CalibPointTotalNum"].GetInt32()==acceptedCount,"cancelled reset leaves accepted parameters untouched");
   acceptReset=true;resetSession.State.Invoke(reset);
   Check(resets==0&&values["CalibPointTotalNum"].GetInt32()==acceptedCount&&values["HomoFreedom"].GetInt32()==2&&values["WeightFun"].GetInt32()==3,"confirmed source-list reset is non-atomic and preserves point count");
   Check(ToolDisplayService.Read(values,plugin.DisplayDefinition).FontSize==18,"algorithm reset retains display object configuration");
   Check(values[ToolConfigurationDisplaySubscriptions.ParameterName].GetString()==positionBeforeReset,"algorithm reset retains display-only source bindings");
  }
  window.Close();
  using var reopened=plugin.CreateConfiguration(context with{Node=node with{Parameters=values}});var font=reopened.State.Nodes.Single(n=>ConfigurationObjectRoute.Parse(n)?.Property==ConfigurationObjectProperty.FontSize);Check(ConfigurationObjectRoute.Parse(font) is not null&&ToolDisplayService.Read(values,plugin.DisplayDefinition).FontSize==18,"reopened original result model preserves display object settings");
  var factory=plugin.CreateWpfEditor(context with{Node=node with{Parameters=values}});Check(factory is ToolConfigurationModelWindow,"compatibility WPF factory also uses shared host");factory.Close();
 }
 static JsonElement J<T>(T value)=>JsonSerializer.SerializeToElement(value);
 static void Pump(){for(int i=0;i<8;i++){var frame=new System.Windows.Threading.DispatcherFrame();System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle,new Action(()=>frame.Continue=false));System.Windows.Threading.Dispatcher.PushFrame(frame);}}
 static IEnumerable<DependencyObject> Walk(DependencyObject root){yield return root;for(int i=0;i<VisualTreeHelper.GetChildrenCount(root);i++)foreach(var item in Walk(VisualTreeHelper.GetChild(root,i)))yield return item;}
 static void Capture(Window window,string path){var image=new RenderTargetBitmap((int)window.Width,(int)window.Height,96,96,PixelFormats.Pbgra32);image.Render(window);var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(image));using var stream=File.Create(path);png.Save(stream);}
 sealed class SaveService:IToolConfigurationSaveFileService {public string Path="";public string? Remembered;public bool Cancel;public ConfigurationFileSelection SelectSave(Window? owner,ConfigurationSaveFileRequest request)=>Cancel?new(ConfigurationFileStatus.Cancelled,[]):new(ConfigurationFileStatus.Selected,[Path]);public ConfigurationFileHistoryResult Remember(string key,string selected){Remembered=selected;return new(true);}}
 sealed class Notifications:IToolConfigurationNotificationService {public List<ConfigurationNotification> Messages=[];public ConfigurationNotificationResult Notify(Window? owner,ConfigurationNotification notification){Messages.Add(notification);return new(ConfigurationNotificationStatus.Displayed);}}
}
