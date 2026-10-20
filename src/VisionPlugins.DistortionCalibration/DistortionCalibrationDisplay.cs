using System.Text.Json;
using VisionFramework;
using VisionPluginContracts;

namespace VisionPlugins.DistortionCalibration;

/// <summary>Source display-object projection for captured results. No algorithm run,
/// image redetection or result generation. Complete runtime/layer selection is separate.</summary>
public static class DistortionCalibrationDisplay
{
 public static IReadOnlyList<ToolDisplayGraphic> BuildGraphics(IReadOnlyDictionary<string,JsonElement> parameters,IReadOnlyDictionary<string,PortValue> inputs,IReadOnlyDictionary<string,PortValue> outputs)=>
  ToolDisplayGraphics.Build(DistortionCalibrationMetadata.DisplayObjects,parameters,inputs,outputs)
   .Concat(ToolDisplayGraphics.BuildNamedTexts(DistortionCalibrationMetadata.DisplayObjects,parameters,inputs,outputs)).ToArray();
}
