using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
static class Program {
 [STAThread]static int Main(){try{
  _=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};int checks=0;
  void Check(bool ok,string message){if(!ok)throw new Exception(message);checks++;}
  var dir=Path.GetFullPath("analysis/rotate-calibration/lifecycle-checks");Directory.CreateDirectory(dir);var file=Path.Combine(dir,"matrix.iwcal");File.WriteAllBytes(file,new float[]{2,0,10,0,2,-5,0,0,1}.SelectMany(BitConverter.GetBytes).ToArray());
  var plugin=new RotateCalibrationPlugin();var parameters=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());parameters["FilePath"]=JsonSerializer.SerializeToElement(file);parameters["RevOutputFlag"]=JsonSerializer.SerializeToElement(true);
  var get=typeof(RotateCalibrationPlugin).GetMethod("Get",BindingFlags.NonPublic|BindingFlags.Instance)!;
  object State(ModuleExecutionScope? scope)=>get.Invoke(plugin,["life",dir,scope])!;
  RotateCalibrationSession Session(object state)=>(RotateCalibrationSession)state.GetType().GetField("Session")!.GetValue(state)!;
  ModuleResult Run(ModuleExecutionScope? scope)=>plugin.CreateRuntime().ExecuteAsync(new ModuleExecutionContext("life",dir,new Dictionary<string,PortValue>{{"ImagePointX",PortValue.Number(50)},{"ImagePointY",PortValue.Number(40)},{"WorldRotateAngle",PortValue.Number(17)},{"RotNum",PortValue.Integer(3)}},parameters){Scope=scope},CancellationToken.None).AsTask().GetAwaiter().GetResult();
  var result=Run(null);var original=State(null);Check(result.NativeErrorCode==0&&Session(original).Current==1,"Normal first sample stored");
  using(var editor=plugin.CreateConfiguration(new(new("life",RotateCalibrationPlugin.Id,"1","life",parameters),dir,_=>{},_=>throw new Exception("Lifecycle test must not run config")))){Check(ReferenceEquals(original,State(null)),"Opening editor reuses runtime cache");}
  Check(ReferenceEquals(original,State(null))&&Session(original).Loaded&&Session(original).Current==1,"Closing editor preserves matrix and in-progress sample");
  plugin.Reset("life");Check(!ReferenceEquals(original,State(null))&&!Session(State(null)).Loaded&&Session(State(null)).Current==0,"Explicit tool Reset removes standalone session");
  using var a=new ModuleExecutionScope();using var b=new ModuleExecutionScope();Run(a);var scoped=State(a);Check(Session(scoped).Current==1&&!Session(State(b)).Loaded,"Execution scopes have independent sessions");
  a.RemoveNode(RotateCalibrationPlugin.Id,"life");Check(!ReferenceEquals(scoped,State(a))&&!Session(State(a)).Loaded,"Removing scope node releases session identity");
  Console.WriteLine($"PASS {checks}: rotation production lifecycle/cache scope boundaries");return 0;
 }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
