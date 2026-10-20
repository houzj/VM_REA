using System.IO;
using System.Text.Json;
using System.Windows;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;

internal static class FileFailureChecks
{
 internal static void Run(Action<bool,string> check,string output)
 {
  var plugin=new ReadCalibFilePlugin();var parameters=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());
  string Xml(int value)=>$"<R><CalibInputParam/><CalibOutputParam><P ParamName='Dynamic' DataType='float'><V>{value}</V></P></CalibOutputParam></R>";
  string good=Path.Combine(output,"retained-source.xml"),bad=Path.Combine(output,"bad-source.xml");File.WriteAllText(good,Xml(12));parameters["FilePath"]=JsonSerializer.SerializeToElement(good);
  using var scope=new ModuleExecutionScope();var node=new NodeDocument("file-failure",ReadCalibFilePlugin.Id,"1.0.0","read",parameters);int runs=0;
  var context=new ToolEditorContext(node,output,next=>parameters=next,_=>{runs++;throw new InvalidOperationException("Failed picker must not schedule execution.");}){Scope=scope,ConfigurationNotifications=new Notices()};
  using var configuration=plugin.CreateConfiguration(context);
  ModuleResult Execute(bool refresh=false)=>plugin.CreateRuntime().ExecuteAsync(new(node.Id,output,refresh?new Dictionary<string,PortValue>{{"RefreshSignal",PortValue.Integer(1)}}:new Dictionary<string,PortValue>(),parameters){Scope=scope},default).Result;
  check(Execute().Outputs["Dynamic"].As<double>()==12,"initial accepted file cached");
  File.WriteAllText(good,Xml(99));File.WriteAllText(bad,"broken");configuration.State.CommitUser("FilePath",bad);
  check(parameters["FilePath"].GetString()==good&&runs==0,"failed new path retains accepted document parameter and schedules no success run");
  check(configuration.State.Find("FilePath")!.Value==bad,"native error retains attempted visible value");
  check(Execute().Outputs["Dynamic"].As<double>()==12,"malformed selection does not implicitly re-read modified old path");
  File.WriteAllText(bad,Xml(22));configuration.State.CommitUser("FilePath",bad);
  check(Execute().Outputs["Dynamic"].As<double>()==12,"same failed visible value does not repeat native setter");
  configuration.State.CommitUser("FilePath",good);
  check(Execute().Outputs["Dynamic"].As<double>()==99,"returning to old accepted value after error retries setter");
  File.WriteAllText(good,Xml(100));configuration.State.CommitUser("FilePath",good);
  check(Execute().Outputs["Dynamic"].As<double>()==99,"successful retry clears retained-failure marker");
  string empty=Path.Combine(output,"empty-output.xml");File.WriteAllText(empty,"<R><CalibInputParam><P ParamName='NewInput' DataType='float'><V>7</V></P></CalibInputParam></R>");configuration.State.CommitUser("FilePath",empty);
  var partial=Execute();check(partial.Outputs["ModuStatus"].As<long>()==0&&!partial.Outputs.ContainsKey("Dynamic"),"wellformed missing output preserves original clear-before-E17 partial state");
  var editor=context with{Node=node with{Parameters=parameters}};var snapshot=plugin.Snapshot(editor);
  check(snapshot.Error==0xe0000017&&snapshot.Data.InputDescription().Contains("NewInput",StringComparison.Ordinal),"snapshot retains failed selection new input/cleared outputs without implicit reload");
  configuration.Dispose();using(var reopened=plugin.CreateConfiguration(context with{Node=node with{Parameters=parameters}})){
   check(reopened.State.Find("FilePath")!.Value==good,"same-scope reopen reads native committed path rather than failed presentation");
   check(Execute().Outputs["ModuStatus"].As<long>()==0,"same-scope reopen preserves partial cleared outputs");
  }
  check(Execute(true).Outputs["Dynamic"].As<double>()==100,"explicit refresh reads retained successful native file path");
  string fourth=Path.Combine(output,"fourth-pose.xml");File.WriteAllText(fourth,"<R><CalibInputParam/><CalibOutputParam><P ParamName='P' DataType='CalibPointF'><X>1</X><Y>2</Y><R>3</R><Fourth>4</Fourth></P></CalibOutputParam></R>");
  using(var next=plugin.CreateConfiguration(context with{Node=node with{Parameters=parameters}})){
   next.State.CommitUser("FilePath",fourth);var point=Execute();
   check(parameters["FilePath"].GetString()==fourth&&next.State.Find("FilePath")!.Value==fourth,"E207 commits native path and retains attempted visible path");
   check(point.Outputs["X"].As<double>()==1&&point.Outputs["Y"].As<double>()==2&&point.Outputs["R"].As<double>()==3,"E207 retains three prior coordinates");
   check(plugin.Snapshot(context with{Node=node with{Parameters=parameters}}).Error==0xe0000207,"E207 survives manual execution without implicit reload");
   var saved=plugin.PrepareSave(node with{Parameters=parameters},output,new ResourceCatalog(output,[]),scope);
   using var cold=new ModuleExecutionScope();var restored=plugin.Snapshot(context with{Node=saved.Node,Scope=cold});
   check(restored.Error==0xe0000207&&restored.Data.Values()["R"].As<double>()==3,"saved E207 cached XML restores source partial data in new scope");
   File.WriteAllText(bad,Xml(22));next.State.CommitUser("FilePath",bad);check(Execute().Outputs["Dynamic"].As<double>()==22,"new valid path succeeds after partial-load error");
   File.WriteAllText(bad,Xml(33));next.State.CommitUser("FilePath",bad);check(Execute().Outputs["Dynamic"].As<double>()==22,"new success clears failed marker and suppresses unchanged setter");
   next.State.CommitUser("FilePath",bad,forceWrite:true);check(Execute().Outputs["Dynamic"].As<double>()==33,"picker SetCurValue null then same path forces retry unlike unchanged text binding");
  }
  File.WriteAllText(bad,Xml(44));scope.RemoveNode(ReadCalibFilePlugin.Id,node.Id);
  check(Execute().Outputs["Dynamic"].As<double>()==44,"RemoveNode drops loader cache and reconstructs from current source");
  scope.Dispose();bool rejected=false;try{Execute();}catch(ObjectDisposedException){rejected=true;}
  check(rejected,"closed solution scope rejects later loader state acquisition");
 }
 sealed class Notices:IToolConfigurationNotificationService
 {
  public ConfigurationNotificationResult Notify(Window? owner,ConfigurationNotification notification)=>new(ConfigurationNotificationStatus.Displayed);
 }
}
