using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Xml.Linq;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
static class Program {
 static int checks;static void Check(bool ok,string message){if(!ok)throw new Exception(message);checks++;}static JsonElement J<T>(T x)=>JsonSerializer.SerializeToElement(x);
 [STAThread]static int Main(){try{
  _=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};var dir=Path.GetFullPath("analysis/rotate-calibration/reset-checks");Directory.CreateDirectory(dir);string file=Path.Combine(dir,"matrix.iwcal");File.WriteAllBytes(file,new float[]{2,0,10,0,2,-5,0,0,1}.SelectMany(BitConverter.GetBytes).ToArray());
  var plugin=new RotateCalibrationPlugin();var backing=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());backing["InputWay"]=J(1);backing["RevOutputFlag"]=J(true);backing["FilePath"]=J(file);backing["ConfigurationUserData"]=J(JsonSerializer.Serialize(new Dictionary<string,string>{{"@RevOutputFlag","True"},{"@FilePath","True"},{"Param.InputWay","1"},{"Keep","keep"}}));
  var inputs=new Dictionary<string,PortValue>{{"ImagePointX",PortValue.Number(50)},{"ImagePointY",PortValue.Number(40)},{"WorldRotateAngle",PortValue.Number(17)},{"RotNum",PortValue.Integer(3)}};var before=plugin.CreateRuntime().ExecuteAsync(new("rotate",dir,inputs,backing),CancellationToken.None).Result;Check(before.NativeErrorCode==0,"Normal pre-reset sample");
  var get=typeof(RotateCalibrationPlugin).GetMethod("Get",BindingFlags.NonPublic|BindingFlags.Instance)!;object State()=>get.Invoke(plugin,["rotate",dir,null])!;var state=State();var native=(RotateCalibrationSession)state.GetType().GetField("Session")!.GetValue(state)!;var matrix=native.Matrix.ToArray();Check(native.Current==1&&native.Loaded,"Pre-reset cache loaded with sample");
  var confirms=new Confirms();var notifications=new Notifications();var subscriptions=new Subscriptions();int atomic=0,runs=0,applies=0;bool rejectReverse=false;
  var context=new ToolEditorContext(new("rotate",RotateCalibrationPlugin.Id,"1","rotate",backing),dir,next=>{if(rejectReverse&&!next["RevOutputFlag"].GetBoolean())throw new InvalidOperationException("host rejected reverse");applies++;backing=next;},_=>{runs++;throw new Exception("Reset must not run");}){ReadEffectiveParameters=()=>backing,ConfigurationConfirmations=confirms,ConfigurationNotifications=notifications,ConfigurationSubscriptions=subscriptions,ResetConfigurationParameters=_=>{atomic++;throw new Exception("Atomic reset forbidden");}};
  using var editor=plugin.CreateConfiguration(context);rejectReverse=true;editor.State.CommitUser("RevOutputFlag","False",true);Check(backing["RevOutputFlag"].GetBoolean(),"Host rejection preserves backing reverse value");
  rejectReverse=false;editor.State.CommitUser("InputWay","1",true);Check(backing["RevOutputFlag"].GetBoolean(),"Subsequent user-data write does not publish rejected local parameter");
  int initial=applies;editor.State.Invoke(editor.State.Find("Button_ResetAlgoParams")!);Check(applies==initial&&subscriptions.Clears.Count==0&&notifications.Items.Count==0,"Cancel leaves bindings/defaults/cache unchanged");
  confirms.Allow=true;editor.State.Invoke(editor.State.Find("Button_ResetAlgoParams")!);
  Check(subscriptions.Clears.SequenceEqual(new[]{"RevOutputFlag","FilePath"}),"Only actual two ISubInfo modes clear in source order");Check(atomic==0&&runs==0,"No whole dictionary reset or run");
  Check(backing["InputWay"].GetInt32()==1&&backing["RevOutputFlag"].GetBoolean(),"Non-list input mode and reverse output survive");Check(backing["FilePath"].GetString()==file,"Original length-zero rejected default does not replace path");
  var user=JsonSerializer.Deserialize<Dictionary<string,string>>(backing["ConfigurationUserData"].GetString()!)!;Check(user["@RevOutputFlag"]=="False"&&user["@FilePath"]=="False","Unbind modes persist despite later default failure");Check(user["Param.InputWay"]=="1"&&user["Keep"]=="keep","Unrelated user values survive");
  Check(notifications.Items.Count==1&&notifications.Items[0].Kind==ConfigurationNotificationKind.Error&&notifications.Items[0].Message=="0xe0000001","Original sole default rejection surfaces error, no success notification");
  Check(ReferenceEquals(state,State())&&native.Loaded&&native.Current==1&&native.Matrix.SequenceEqual(matrix)&&native.FilePath==file,"Reset retains native matrix/path/sample cache");editor.Dispose();Check(ReferenceEquals(state,State())&&native.Current==1,"Configuration close retains sample cache");
  using var resource=typeof(RotateCalibrationPlugin).Assembly.GetManifestResourceStream("VisionPlugins.Calibration.Resources.RotateCalibModuAlgorithm.xml")!;var defaults=XDocument.Load(resource).Root!.Elements("ParamItem").ToArray();Check(defaults.Length==1&&defaults[0].Element("Name")!.Value=="FilePath"&&defaults[0].Element("DefaultValue")!.Value=="","Original effective defaults list has only empty FilePath");
  plugin.Reset("rotate");Check(!ReferenceEquals(state,State()),"Explicit lifecycle Reset still removes session");Console.WriteLine($"PASS {checks}: Rotate original empty-default rejection/two-mode unbind/cache lifecycle");return 0;
 }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
 sealed class Confirms:IToolConfigurationConfirmationService{public bool Allow;public bool Confirm(Window? owner,ConfigurationConfirmation request)=>Allow;}
 sealed class Notifications:IToolConfigurationNotificationService{public List<ConfigurationNotification> Items=[];public ConfigurationNotificationResult Notify(Window? owner,ConfigurationNotification value){Items.Add(value);return new(ConfigurationNotificationStatus.Displayed);}}
 sealed class Subscriptions:IToolConfigurationSubscriptionService{
  readonly HashSet<string> bound=new(["RevOutputFlag","FilePath"]);public List<string> Clears=[];ConfigurationSubscriptionBinding Binding=>new(new ConfigurationConstantReference("int","1"),ConfigurationSubscriptionIndex.All);
  public ConfigurationSubscriptionTreeResult Enumerate(ConfigurationSubscriptionQuery q)=>new(ConfigurationSubscriptionStatus.Available,[]);
  public ConfigurationSubscriptionBindingResult Read(ConfigurationSubscriptionDestination d)=>new(ConfigurationSubscriptionStatus.Available,bound.Contains(d.Name)?Binding:null);
  public ConfigurationSubscriptionBindingResult Bind(ConfigurationSubscriptionDestination d,ConfigurationSubscriptionBinding? b){if(b is null&&bound.Remove(d.Name))Clears.Add(d.Name);return Read(d);}
 }
}
