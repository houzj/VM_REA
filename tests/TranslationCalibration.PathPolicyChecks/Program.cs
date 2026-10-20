using System.IO;
using System.Text.Json;
using System.Xml.Linq;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using W=System.Windows.Controls;
static class Program{
 static int checks;static void Check(bool b,string message){if(!b)throw new Exception(message);Console.WriteLine($"PASS {++checks}: {message}");}
 static string Xml(string tag="OpenFileForCalibDialog",string filter="%.xml|*.xml%"){
  var file=new XElement(tag,new XAttribute("Name","Path"),new XElement("AccessMode","RW"),new XElement("FileOption",new XElement("FilterName",filter)));
  var category=new XElement("Category",new XAttribute("Name","Files"),new XAttribute("CanTriggerRun","True"),new XElement("Items",file));
  var tab=new XElement("Tab",new XAttribute("Name","Basic"),new XElement("Categorys",category));
  return new XElement("AlgorithmTabRoot",new XElement("Tabs",tab)).ToString();
 }
 static (ToolConfigurationState State,Backend Backend,List<string> Directories,List<string> Events) Create(string tag="OpenFileForCalibDialog",string filter="%.xml|*.xml%"){
  var backend=new Backend();var directories=new List<string>();var events=new List<string>();var state=new ToolConfigurationState(ToolConfigurationDefinition.Parse(Xml(tag,filter)),backend){ExecutionEnabled=true};
  state.ParameterWriteValidator=request=>{events.Add("validate");var result=ToolConfigurationCalibrationPathPolicy.ValidateSourceWrite(request,key=>key,directory=>{directories.Add(directory);return directory==@"C:\exists";});if(result?.Error is not null)events.Add("notify:Error");return result;};state.Initialize();return(state,backend,directories,events);
 }
 [STAThread]static int Main(string[] args){try{
  var (state,backend,dirs,events)=Create();var node=state.Find("Path")!;Check(ToolConfigurationCalibrationPathPolicy.SourceFilter(node,key=>key)==".xml|*.xml","source percent-wrapped localized filter");
  foreach(string bad in new[]{@"C:\exists\bad.XML",@"C:\exists\bad.iwcal",@"C:\exists\bad.txt",""}){int writes=backend.Writes,runs=backend.Runs,queries=dirs.Count;state.CommitUser(node,bad);Check(node.Value==bad&&state.IsRejectedParameterWrite(node),"invalid suffix retains attempted value: "+bad);Check(backend.Writes==writes&&backend.Runs==runs&&dirs.Count==queries,"suffix gate stops before directory/backend/run");}
  int calls=events.Count;state.CommitUser(node,"");Check(events.Count==calls,"unchanged bad typed value does not repeat validator");state.CommitUser(node,backend.Accepted);Check(backend.Writes==1&&backend.Runs==1&&!state.IsRejectedParameterWrite(node),"return to old accepted value retries setter after rejected attempt");
  state.CommitUser(node,@"C:\missing\bad.xml");Check(dirs.Last()==@"C:\missing"&&backend.Writes==1&&node.Value==@"C:\missing\bad.xml","raw last backslash directory failure preserves attempted value");
  int dirCount=dirs.Count;state.CommitUser(node,"/root/share/a.xml");Check(dirs.Count==dirCount&&backend.Writes==1&&state.Diagnostics.Any(d=>d.Message.Contains("unsupported",StringComparison.OrdinalIgnoreCase)),"remote directory explicitly unsupported without local or remote lookup");
  events.Clear();void Pick(string path){state.CommitUser(node,path,true);events.Add("history");}Pick(@"C:\exists\bad.XML");Check(events.SequenceEqual(new[]{"validate","notify:Error","history"}),"failed source picker still remembers selected directory after failed validation");events.Clear();Pick(@"C:\exists\bad.XML");Check(events.SequenceEqual(new[]{"validate","notify:Error","history"})&&backend.Writes==1,"same failed picker forces retry null-to-path while typed same value did not");
  state.CommitUser(node,@"C:\exists\new.xml");Check(backend.Accepted==@"C:\exists\new.xml"&&backend.Runs==2&&!state.IsRejectedParameterWrite(node),"new valid path commits and clears rejected marker");
  backend.Reject=true;state.CommitUser(node,@"C:\exists\refused.xml");Check(node.Value==backend.Accepted&&!state.IsRejectedParameterWrite(node)&&backend.Runs==2,"independent host refusal retains ordinary rollback semantics");
  backend.NativeError=true;state.CommitUser(node,@"C:\exists\native-error.xml");Check(node.Value==@"C:\exists\native-error.xml"&&state.IsRejectedParameterWrite(node)&&backend.Runs==2,"explicit native error contract retains attempt separately from host refusal");
  var (multi,multiBackend,_,_)=Create(filter:".xml|*.xml;*.iwcal");multi.CommitUser("Path",@"C:\exists\new.iwcal");Check(multiBackend.Writes==1&&multiBackend.Runs==1,"source semicolon patterns permit exact iwcal suffix");
  var (board,boardBackend,_,_)=Create(filter:"%.iwcal|*.iwcal%");board.CommitUser("Path",@"C:\exists\new.xml");Check(boardBackend.Writes==0,"board filter does not inherit XML suffix");
  var (baseState,baseBackend,_,_)=Create("OpenFileDialog");baseState.CommitUser("Path",@"C:\missing\anything.txt");Check(baseBackend.Writes==1&&baseBackend.Runs==1,"base OpenFileDialog remains outside calibration subtype policy");
  int validationCount=events.Count;backend.NativeError=false;backend.Reject=false;var descriptor=new ModuleDescriptor("policy","1","policy",[],[],[new("Path",ParameterKind.Text,true,JsonSerializer.SerializeToElement(backend.Accepted))]);var reset=ToolConfigurationAlgorithmReset.Execute(descriptor,state,backend,null,"<AlgorithmParamList><ParamItem><Name>Path</Name><DefaultValue/></ParamItem></AlgorithmParamList>");Check(reset.Error is not null&&events.Count==validationCount,"algorithm reset bypasses UI validation and reaches native empty-path branch");
  string output=Path.GetFullPath(args.Length==1?args[0]:"analysis/translation/path-policy");Directory.CreateDirectory(output);Consumer(output);File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=true,checks,nativeExecution=false}));return 0;
 }catch(Exception error){Console.Error.WriteLine(error);return 1;}}
 sealed class Backend:IToolConfigurationDispatcher{
  public string Accepted=@"C:\exists\accepted.xml";public int Writes,Runs;public bool Reject,NativeError;
  public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest request){if(request.Kind==ConfigurationOperationKind.ReadParameter)return new(true,Accepted);if(request.Kind==ConfigurationOperationKind.ReadUserValue)return new(false);if(request.Kind==ConfigurationOperationKind.Run){Runs++;return new(true);}if(request.Kind==ConfigurationOperationKind.WriteParameter){Writes++;if(request.Value=="")return new(true,Error:"native E204"){KeepAttemptedValueOnError=true};if(Reject)return new(true,Error:NativeError?"native load error":"host apply refused"){KeepAttemptedValueOnError=NativeError};Accepted=request.Value!;return new(true,Accepted);}return new(false);}
 }
 static IEnumerable<DependencyObject> Walk(DependencyObject item){yield return item;for(int i=0;i<VisualTreeHelper.GetChildrenCount(item);i++)foreach(var child in Walk(VisualTreeHelper.GetChild(item,i)))yield return child;}
 static void Consumer(string output){
  var app=Application.Current??new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(app.Dispatcher));var plugin=new TranslationPlugin();var values=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());
  string folder=Path.Combine(output,"existing");Directory.CreateDirectory(folder);string accepted=Path.Combine(folder,"accepted.xml"),bad=Path.Combine(folder,"bad.XML");values["CalibPathName"]=JsonSerializer.SerializeToElement(accepted);var events=new List<string>();var files=new Files(events){Selected=bad};int writes=0,runs=0;
  var context=new ToolEditorContext(new("path-consumer",TranslationPlugin.Id,"1.0.0","Translation",values),output,next=>{writes++;events.Add("apply");values=next;},_=>{runs++;events.Add("run");return Task.FromResult(new NodeRunRecord("path-consumer",NodeRunStatus.Success,new Dictionary<string,PortValue>(),0));}){ConfigurationFiles=files,ConfigurationNotifications=new Notices(events),ConfigurationExecutionStatus=()=>new(false,false,true,false,true)};
  using var session=plugin.CreateConfiguration(context);var window=new ToolConfigurationModelWindow(context,session){Left=-20000,Top=-20000,ShowInTaskbar=false,ShowActivated=false};window.Show();try{
   window.UpdateLayout();var choose=Walk(window).OfType<W.Button>().Single(button=>button.Name=="ChooseCalibrationPath");events.Clear();choose.RaiseEvent(new RoutedEventArgs(W.Button.ClickEvent));
   Check(events.SequenceEqual(new[]{"select","notify:Error","history"})&&writes==0&&runs==0,"real commonprovider WPF failed picker orders notification then unconditional history without backend/run");Check(session.State.Find("CalibPathName")!.Value==bad&&values["CalibPathName"].GetString()==accepted,"real WPF invalid attempted path differs from committed backend path");
   events.Clear();choose.RaiseEvent(new RoutedEventArgs(W.Button.ClickEvent));Check(events.SequenceEqual(new[]{"select","notify:Error","history"})&&writes==0,"real WPF same failed picker forces a fresh validation and history");
   events.Clear();session.State.CommitUser("CalibPathName",bad);Check(events.Count==0,"unchanged typed bad value does not repeat picker side effects");
   events.Clear();string missing=Path.Combine(output,"missing-parent","new.xml");session.State.CommitUser("CalibPathName",missing);Check(events.SequenceEqual(new[]{"notify:Error"})&&writes==0&&runs==0,"actual commonprovider rejects missing parent before adapter and does not write picker history for typed text");
   events.Clear();string good=Path.Combine(folder,"new.xml");files.Selected=good;choose.RaiseEvent(new RoutedEventArgs(W.Button.ClickEvent));Check(values["CalibPathName"].GetString()==good&&writes==1&&runs==1,"actual commonprovider valid picker commits and pre-runs once");Check(events.SequenceEqual(new[]{"select","apply","notify:Success","run","history"}),"source successful picker notifies after SetParam and before PreRun then history");
   events.Clear();session.State.CommitUser("CalibPathName",bad);events.Clear();session.State.CommitUser("CalibPathName",good);Check(writes==2&&runs==2&&!session.State.IsRejectedParameterWrite(session.State.Find("CalibPathName")!),"actual consumer bad-to-old accepted path retries native setter and clears failure marker");
  }finally{window.Close();}
 }
 sealed class Files(List<string> events):IToolConfigurationFileService{
  public string Selected="";public ConfigurationFileSelection Select(Window? owner,ConfigurationFileRequest request){events.Add("select");return new(ConfigurationFileStatus.Selected,[Selected]);}
  public ConfigurationFileHistoryResult Remember(string moduleKey,string selectedFile){events.Add("history");return new(true,Path.GetDirectoryName(selectedFile));}
 }
 sealed class Notices(List<string> events):IToolConfigurationNotificationService{
  public ConfigurationNotificationResult Notify(Window? owner,ConfigurationNotification notification){events.Add("notify:"+notification.Kind);return new(ConfigurationNotificationStatus.Displayed);}
 }
}
