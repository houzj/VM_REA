using System.Text.Json;
using System.Windows;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;

static class Program {
 static int checks;
 static JsonElement J<T>(T value)=>JsonSerializer.SerializeToElement(value);
 static void Check(bool value,string message){if(!value)throw new Exception(message);checks++;}
 [STAThread]static int Main(){try{_=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};Run(false);Run(true);Console.WriteLine($"PASS {checks} NImage source-list reset checks");return 0;}catch(Exception error){Console.Error.WriteLine(error);return 1;}}
 static void Run(bool rejectFilter){
  var values=NImageParameters.Defaults();values["CalibBoardType"]=J(6);values["FilterStatus"]=J(2);values["CareraMove"]=J(true);
  values["CalibPointTotalNum"]=J(12);values["BasePointX"]=J(17d);values["ClearRevision"]=J(4);values["CalibPathName"]=J("preserved.xml");values["RefreshFileEnable"]=J(true);
  values["ConfigurationUserData"]=J("{\"Keep\":\"value\"}");
  var before=values.ToDictionary(p=>p.Key,p=>p.Value.Clone());
  var confirmations=new Confirmations();var notifications=new Notifications();var writes=new List<string>();bool record=false;int runs=0,atomic=0,clears=0,exports=0;
  var provider=new NImageConfigurationProvider(_=>clears++,(_,_)=>exports++);
  var context=new ToolEditorContext(new("image",NImagePlugin.Id,"1.0.0","NImage",values),Environment.CurrentDirectory,next=>{
   if(record){var changed=next.Keys.Where(k=>values[k].GetRawText()!=next[k].GetRawText()).ToArray();writes.AddRange(changed);if(rejectFilter&&changed.Contains("FilterStatus"))throw new InvalidOperationException("reject FilterStatus");}values=next;
  },_=>{runs++;throw new Exception("reset cannot run");}){ReadEffectiveParameters=()=>values,ConfigurationConfirmations=confirmations,ConfigurationNotifications=notifications,ResetConfigurationParameters=_=>{atomic++;throw new Exception("no wholesale reset");}};
  using var session=provider.CreateConfiguration(context);record=true;session.State.Invoke(session.State.Find("Button_ResetAlgoParams")!);
  Check(writes.Count==0,"decline makes no writes");confirmations.Allow=true;session.State.Invoke(session.State.Find("Button_ResetAlgoParams")!);record=false;
  Check(writes.SequenceEqual(new[]{"CalibBoardType","FilterStatus","CareraMove"}),"only actual three XML defaults in source order");
  Check(values["CalibBoardType"].GetInt32()==7&&values["FilterStatus"].GetInt32()==(rejectFilter?2:1)&&!values["CareraMove"].GetBoolean(),"intermediate rejection does not roll back or stop later defaults");
  foreach(var p in before.Where(p=>p.Key is not ("CalibBoardType" or "FilterStatus" or "CareraMove")))Check(values[p.Key].GetRawText()==p.Value.GetRawText(),"preserve non-list "+p.Key);
  Check(atomic==0&&runs==0&&clears==0&&exports==0,"reset does not invoke atomic run clear or export");
  Check(notifications.Items.Last().Kind==(rejectFilter?ConfigurationNotificationKind.Error:ConfigurationNotificationKind.Success),"notification follows final result");
 }
 sealed class Confirmations:IToolConfigurationConfirmationService{public bool Allow;public bool Confirm(Window? owner,ConfigurationConfirmation request)=>Allow;}
 sealed class Notifications:IToolConfigurationNotificationService{public List<ConfigurationNotification> Items=[];public ConfigurationNotificationResult Notify(Window? owner,ConfigurationNotification value){Items.Add(value);return new(ConfigurationNotificationStatus.Displayed);}}
}
