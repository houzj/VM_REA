using VisionPluginContracts;
using VisionPlugins.CalibrationBoard;

static class AbandonedParameterChecks {
 public static void Run(Action<bool,string> check) {
  var plugin=new CalibrationBoardPlugin();
  var values=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());
  var state=new ToolConfigurationState(CalibrationBoardMetadata.Definition());
  state.Initialize();
  foreach(var (name,expected) in new[]{("GrayContrast","15"),("SubpixelWinAuto","1"),("SubPixelWindowSize","30")}) {
   var control=state.Find(name);
   check(control is not null,$"source-declared abandoned parameter is represented in configuration state: {name}");
   check(control!.Get("IsAbandon")=="True",$"source marks {name} abandoned");
   check(control.Get("AccessMode")=="O",$"source marks {name} read-only");
   check(!control.Visible,$"source-hidden {name} stays hidden in the evaluated configuration state");
   check(values[name].GetInt32()==int.Parse(expected),$"module descriptor retains compatibility default for {name}");
  }
 }
}
