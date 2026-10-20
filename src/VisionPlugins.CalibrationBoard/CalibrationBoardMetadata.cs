using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.CalibrationBoard;

/// <summary>Original XML remains the configuration specification; this projection only describes graph persistence.</summary>
public static class CalibrationBoardMetadata {
 public static string Resource(string name){using var s=typeof(CalibrationBoardMetadata).Assembly.GetManifestResourceStream("VisionPlugins.CalibrationBoard.Resources."+name)??throw new InvalidDataException(name);using var r=new StreamReader(s);return r.ReadToEnd();}
 public static ToolConfigurationDefinition Definition()=>ToolConfigurationDefinition.Parse(Resource("AlgorithmTab.xml"),Resource("Groups.xml"));
 public static IReadOnlyList<ParameterDescriptor> Parameters(){
  var defaults=XDocument.Parse(Resource("Algorithm.xml")).Descendants("ParamItem").ToDictionary(e=>e.Element("Name")!.Value,e=>e.Element("DefaultValue")!.Value);
  var result=new List<ParameterDescriptor>();
  foreach(var e in XDocument.Parse(Resource("AlgorithmTab.xml")).Descendants().Where(e=>e.Attribute("Name")!=null&&e.Element("CurValue")!=null&&e.Name.LocalName is "Integer" or "Float" or "Enumeration" or "Boolean" or "OpenFileForCalibDialog")){
   string name=e.Attribute("Name")!.Value,raw=defaults.GetValueOrDefault(name)??e.Element("CurValue")!.Value;
   var kind=e.Name.LocalName switch{"Float"=>ParameterKind.Number,"Boolean"=>ParameterKind.Boolean,"OpenFileForCalibDialog"=>ParameterKind.Text,_=>ParameterKind.Integer};
   object value=kind switch{ParameterKind.Number=>Number(raw),ParameterKind.Integer=>(int)Number(raw),ParameterKind.Boolean=>raw.Equals("true",StringComparison.OrdinalIgnoreCase)||raw=="1",_=>raw};
   double? Bound(string key)=>e.Element(key) is {} b?Number(b.Value):null;
   result.Add(new(name,kind,true,JsonSerializer.SerializeToElement(value),Bound("MinValue"),Bound("MaxValue")));
  }
  result.Add(new(ToolDisplayObjectStore.ParameterName,ParameterKind.Text,true,JsonSerializer.SerializeToElement("{}")));
  result.Add(new("DisplayInputBindings",ParameterKind.Text,true,JsonSerializer.SerializeToElement("{}")));
  result.Add(new("ConfigurationUserData",ParameterKind.Text,true,JsonSerializer.SerializeToElement("{}")));
  result.Add(new(ToolDisplayService.SettingsParameter,ParameterKind.Text,true,JsonSerializer.SerializeToElement("")));
  return result.AsReadOnly();
 }
 static double Number(string s)=>s.StartsWith("0x",StringComparison.OrdinalIgnoreCase)?unchecked((int)uint.Parse(s[2..],NumberStyles.HexNumber,CultureInfo.InvariantCulture)):double.Parse(s,CultureInfo.InvariantCulture);
}

