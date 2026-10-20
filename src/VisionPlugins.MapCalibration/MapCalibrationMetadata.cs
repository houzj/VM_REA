using System.IO;
using VisionPluginContracts;
namespace VisionPlugins.MapCalibration;

/// <summary>Original declarations only; algorithm and session wiring are separately verified.</summary>
public static class MapCalibrationMetadata
{
 public static string Resource(string name){using var s=typeof(MapCalibrationMetadata).Assembly.GetManifestResourceStream("VisionPlugins.MapCalibration.Resources."+name)??throw new InvalidDataException(name);using var r=new StreamReader(s);return r.ReadToEnd();}
 public static ToolConfigurationDefinition Definition()=>ToolConfigurationDefinition.Parse(Resource("MapCalibrationAlgorithmTab.xml"),Resource("ConfigurationGroups.xml"));
 public static IReadOnlyList<ToolDisplayObjectDefinition> DisplayObjects=>ToolDisplayObjectDefinition.Parse(Resource("MapCalibrationDisplay.xml"));
}
