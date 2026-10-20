using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;
using VisionFramework;
using VisionPluginContracts;
using VisionCore;
namespace VisionPlugins.DistortionCalibration;

/// <summary>Source configuration resources and backend initialization defaults; not an alternate UI.</summary>
public static class DistortionCalibrationMetadata {
 public const string ModuleId="vision.distortion-calibration";
 static readonly Lazy<IReadOnlyDictionary<string,IReadOnlyDictionary<string,string>>> captions=new(()=>XDocument.Parse(Resource("DistortionLocalization.xml")).Root!.Elements("Language").ToDictionary(language=>language.Attribute("Name")!.Value,language=>(IReadOnlyDictionary<string,string>)language.Elements("String").ToDictionary(text=>text.Attribute("Key")!.Value,text=>text.Value,StringComparer.Ordinal),StringComparer.Ordinal));
 public static string Localize(string key,string language="Chinese")=>captions.Value.GetValueOrDefault(language)?.GetValueOrDefault(key)??ConfigurationLocalization.ForLanguage(language).Get(key);
 public static string Resource(string name){using var stream=typeof(DistortionCalibrationMetadata).Assembly.GetManifestResourceStream("VisionPlugins.DistortionCalibration.Resources."+name)??throw new InvalidDataException(name);using var reader=new StreamReader(stream);return reader.ReadToEnd();}
 public static ToolConfigurationDefinition Definition()=>ToolConfigurationDefinition.Parse(Resource("DistortionAlgorithmTab.xml"),Resource("Groups.xml"));
 public static IReadOnlyList<ToolDisplayObjectDefinition> DisplayObjects{get;}=ToolDisplayObjectDefinition.Parse(Resource("DistortionDisplay.xml"));
 public static ToolDisplayDefinition DisplayDefinition{get;}=new("ModuleStatus:{0}EstimationError:{1}",["ModuStatus","EstimationError"],["EstimationError"],"InImage");
 public static IReadOnlyList<ParameterDescriptor> Parameters{get;}=CreateParameters();
 // Module XML declares IMAGE generically; Process rejects non-Mono8 with e0000503.
 // CalibPointError's XML IsArray=false conflicts with the confirmed native vector output.
 public static ModuleDescriptor Descriptor{get;}=new(ModuleId,"1.0.0","畸变标定",
  [new("InputImage",DataKind.Image),new("CorrectCenterPoint",DataKind.Point,false),new("CorrectCenterPointX",DataKind.NumberArray,false,AcceptInteger:true){SupportsDeferredNumericRead=true},new("CorrectCenterPointY",DataKind.NumberArray,false,AcceptInteger:true){SupportsDeferredNumericRead=true}],
  new[]{"ModuRunTime","EstimationError","EvaluationError","DetectCenterX","DetectCenterY","DetectWidth","DetectHeight","DetectAngle"}.Select(name=>new PortDescriptor(name,DataKind.Number,name=="ModuRunTime"))
   .Concat(new[]{"CalibPointX","CalibPointY","CalibPointStatus","CalibPointError"}.Select(name=>new PortDescriptor(name,DataKind.NumberArray,false)))
   .Concat(new[]{new PortDescriptor("ModuStatus",DataKind.Integer,false),new("CalibPointNum",DataKind.Integer,false),new("Calibration Point",DataKind.Point,false){Cardinality=ValueCardinality.Array},new("ROI",DataKind.Region,false),new("PostCorrectionImage",DataKind.Image,false)}).ToArray(),Parameters){RoiEditor=new([RoiKind.Rectangle],Maximum:1,FullImage:true)};
 public static Dictionary<string,JsonElement> Defaults()=>Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());
 static IReadOnlyList<ParameterDescriptor> CreateParameters(){
  // Module Init -> vtable+0x60 -> ResetDefaultParam -> GetDefaultConfigByFile
  // uses <module>Algorithm.xml, then dispatches each backend SetParam.
  var backend=XDocument.Parse(Resource("DistortionAlgorithm.xml")).Descendants("ParamItem").ToDictionary(e=>e.Element("Name")!.Value,e=>e.Element("DefaultValue")!.Value);
  var result=new List<ParameterDescriptor>();
  foreach(var e in XDocument.Parse(Resource("DistortionAlgorithmTab.xml")).Descendants().Where(e=>e.Attribute("Name") is not null&&e.Element("DefaultValue") is not null&&e.Name.LocalName is "Integer" or "Float" or "Enumeration" or "Boolean" or "OpenFileForCalibDialog")){
   string name=e.Attribute("Name")!.Value,raw=backend.GetValueOrDefault(name)??e.Element("DefaultValue")!.Value;var kind=e.Name.LocalName switch{"Float"=>ParameterKind.Number,"Boolean"=>ParameterKind.Boolean,"OpenFileForCalibDialog"=>ParameterKind.Text,_=>ParameterKind.Integer};
   object value=kind switch{ParameterKind.Number=>Number(raw),ParameterKind.Integer=>(int)Number(raw),ParameterKind.Boolean=>raw.Equals("True",StringComparison.OrdinalIgnoreCase)||raw=="1",_=>raw};double? Bound(string key)=>e.Element(key) is {} b?Number(b.Value):null;
   result.Add(new(name,kind,true,JsonSerializer.SerializeToElement(value),Bound("MinValue"),Bound("MaxValue")));
  }
  foreach(var item in new[]{("ConfigurationUserData","{}"),(ToolDisplayObjectStore.ParameterName,"{}"),(ToolDisplayService.SettingsParameter,""),("DisplayInputBindings","{}")})result.Add(new(item.Item1,ParameterKind.Text,true,JsonSerializer.SerializeToElement(item.Item2)));
  return result.AsReadOnly();
 }
 static double Number(string text)=>text.StartsWith("0x",StringComparison.OrdinalIgnoreCase)?unchecked((int)uint.Parse(text[2..],NumberStyles.HexNumber,CultureInfo.InvariantCulture)):double.Parse(text,CultureInfo.InvariantCulture);
}


