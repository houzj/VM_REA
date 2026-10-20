using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.CalibrationBoard;

public sealed partial class CalibrationBoardPlugin:IToolDisplayProvider {
 public IReadOnlyList<ToolDisplayObjectDefinition> DisplayObjects{get;}=ToolDisplayObjectDefinition.Parse(CalibrationBoardMetadata.Resource("Display.xml"));
 public ToolDisplayDefinition DisplayDefinition{get;}=new("EstimationError{0},PixcelAccuracy:{1}",["EstimationError","PixelPrecision"],["ModuStatus","EstimationError","Scale","PixelPrecision","CalibPointNum","TranslateX","TranslateY","Rotate","Skew","Aspect"]);
 public ToolDisplayFrame? LastDisplay(NodeDocument node,string baseDirectory,ModuleExecutionScope? scope){
  var snapshot=Current(node.Id,baseDirectory,scope);lock(snapshot){
   if(snapshot.Outputs is null)return null;
   var inputs=snapshot.Inputs??new Dictionary<string,PortValue>();
   var settings=ToolDisplayService.Read(node.Parameters,DisplayDefinition);
   var frame=ToolDisplayService.Build(DisplayDefinition,settings,inputs,snapshot.Outputs);
   // Display.xml contains two distinct datarecord objects with the same name.
   // Preserve declaration order rather than resolving them through a unique-name store.
   var records=DisplayObjects.Where(d=>d.Type=="datarecord"&&d.DefaultStyle.Visible)
    .Select(d=>d.Features.GetValueOrDefault("Content")).Where(f=>f is not null)
    .Select(f=>ToolDisplayService.Build(DisplayDefinition with{ContentTemplate=f!.Value,ContentMapping=f.Mapping.Split(',')},settings with{Content=f!.Value},inputs,snapshot.Outputs).Text);
   return frame with{RecordText=string.Join(Environment.NewLine,records),Graphics=ToolDisplayGraphics.Build(DisplayObjects,node.Parameters,inputs,snapshot.Outputs)};
  }
 }
}

