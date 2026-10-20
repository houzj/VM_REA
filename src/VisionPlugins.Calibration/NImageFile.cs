using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
namespace VisionPlugins.Calibration;

public readonly record struct NImagePose(double X,double Y,double R);
public sealed record NImageCalibrationFileData(string Created,int TranslationCount,int RotationCount,int ErrorStatus,double TranslationError,double RotationError,double TranslationWorldError,double RotationWorldError,double PrecisionX,double PrecisionY,double Precision,IReadOnlyList<NImagePose> ImagePoints,IReadOnlyList<NImagePose> WorldPoints,int RotationDirection,int Handedness,NImagePose RotationImageCenter,IReadOnlyList<double> Matrix);

/// <summary>Independent serialization of the module's CalibInfo XML, distinct from SDK configuration export.</summary>
public static class NImageFile
{
 /// <summary>WriteXmlData only computes world/camera rotation relation for exactly three
 /// rotation images. CareraMove flips that relation and its derived handedness;
 /// other rotation counts keep the -999 relation sentinel. The homography is unchanged.</summary>
 public static NImageCalibrationFileData ForCameraMovement(NImageCalibrationFileData data,bool cameraMoves)=>
  cameraMoves&&data.RotationCount==3&&data.RotationDirection is 1 or -1?data with{RotationDirection=-data.RotationDirection,Handedness=data.Handedness is 1 or -1?-data.Handedness:data.Handedness}:data;
 static readonly CultureInfo Culture=CultureInfo.InvariantCulture;
 static string F(double v){if(!double.IsFinite(v)||!float.IsFinite((float)v))throw new InvalidDataException("标定文件包含非有限数。");return ((float)v).ToString("G8",Culture);}
 public static XDocument Document(NImageCalibrationFileData d)=>Document(d,false);
 static XDocument Document(NImageCalibrationFileData d,bool retainedModuleResult){
  ArgumentNullException.ThrowIfNull(d);int total=checked(d.TranslationCount+d.RotationCount);if(d.TranslationCount<0||d.RotationCount<0||total<1||(!retainedModuleResult&&(d.ImagePoints.Count<total||d.WorldPoints.Count<total))||d.Matrix.Count!=9)throw new InvalidDataException("标定数据未完成或矩阵不是9项。");
  var input=new XElement("CalibInputParam");var output=new XElement("CalibOutputParam");
  void Param(XElement parent,string name,string type,object value)=>parent.Add(new XElement("CalibParam",new XAttribute("ParamName",name),new XAttribute("DataType",type),new XElement("ParamValue",value)));
  Param(input,"CreateCalibTime","string",d.Created);Param(input,"CalibType","string","NImageCalib");Param(input,"TransNum","int",d.TranslationCount);Param(input,"RotNum","int",d.RotationCount);Param(input,"CalibErrStatus","int",d.ErrorStatus);
  foreach(var p in new[]{("TransError",d.TranslationError),("RotError",d.RotationError),("TransWorldError",d.TranslationWorldError),("RotWorldError",d.RotationWorldError),("PixelPrecisionX",d.PrecisionX),("PixelPrecisionY",d.PrecisionY),("PixelPrecision",d.Precision)})Param(input,p.Item1,"float",F(p.Item2));
  foreach(var item in new[]{("ImagePointLst",d.ImagePoints),("WorldPointLst",d.WorldPoints)})input.Add(new XElement("CalibPointFListParam",new XAttribute("ParamName",item.Item1),new XAttribute("DataType","CalibPointList"),(retainedModuleResult?item.Item2:item.Item2.Take(total)).Select(p=>new XElement("PointF",new XElement("X",F(p.X)),new XElement("Y",F(p.Y)),new XElement("R",F(p.R))))));
  Param(output,"RotDirectionState","int",d.RotationDirection);Param(output,"IsRightCoorA","int",d.Handedness);output.Add(new XElement("PointF",new XAttribute("ParamName","RotCenterImagePoint"),new XAttribute("DataType","CalibPointF"),new XElement("RotCenterImagePointX",F(d.RotationImageCenter.X)),new XElement("RotCenterImagePointY",F(d.RotationImageCenter.Y)),new XElement("RotCenterImagePointR",F(d.RotationImageCenter.R))));output.Add(new XElement("CalibFloatListParam",new XAttribute("ParamName","CalibMatrix"),new XAttribute("DataType","FloatList"),d.Matrix.Select(v=>new XElement("ParamValue",F(v)))));
  return new(new XDeclaration("1.0","UTF-8",null),new XElement("CalibInfo",input,output));
 }
 public static void Write(string path,NImageCalibrationFileData data){if(!Path.GetExtension(path).Equals(".xml",StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("标定文件扩展名必须为.xml。");var document=Document(data);var settings=new XmlWriterSettings{Encoding=new UTF8Encoding(false),Indent=true,IndentChars="    ",NewLineChars="\r\n"};using var writer=XmlWriter.Create(path,settings);document.Save(writer);}
 // Native WriteXmlData serializes the two lists independently, including a retained SDK list.
 internal static void WriteModule(string path,NImageCalibrationFileData data){var document=Document(data,true);var settings=new XmlWriterSettings{Encoding=new UTF8Encoding(false),Indent=true,IndentChars="    ",NewLineChars="\r\n"};using var writer=XmlWriter.Create(path,settings);document.Save(writer);}
 // Reading is a typed format validator for tests/interchange; the original NImage module's path field does not load calibration.
 public static NImageCalibrationFileData Read(TextReader text){using var reader=XmlReader.Create(text,new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null});return Read(XDocument.Load(reader));}
 public static NImageCalibrationFileData Read(XDocument document){
  if(document.Root?.Name!="CalibInfo")throw new InvalidDataException("标定文件根节点错误。");var input=document.Root.Element("CalibInputParam")??throw new InvalidDataException("缺少标定输入信息。");var output=document.Root.Element("CalibOutputParam")??throw new InvalidDataException("缺少标定输出信息。");
  XElement Find(XElement p,string name){var matches=p.Elements().Where(e=>(string?)e.Attribute("ParamName")==name).Take(2).ToArray();return matches.Length==1?matches[0]:throw new InvalidDataException("缺少或重复字段 "+name);}
  string Text(XElement p,string n)=>Find(p,n).Element("ParamValue")?.Value??throw new InvalidDataException("字段格式错误 "+n);
  double Parse(string s){if(!double.TryParse(s,NumberStyles.Float,Culture,out double v)||!double.IsFinite(v))throw new InvalidDataException("标定数值格式错误。");return v;}
  double Number(XElement p,string n)=>Parse(Text(p,n));int Integer(XElement p,string n)=>int.Parse(Text(p,n),NumberStyles.Integer,Culture);
  NImagePose[] Points(string n)=>Find(input,n).Elements("PointF").Select(p=>new NImagePose(Parse(p.Element("X")?.Value??""),Parse(p.Element("Y")?.Value??""),Parse(p.Element("R")?.Value??""))).ToArray();
  if(Text(input,"CalibType")!="NImageCalib")throw new InvalidDataException("不是N图像标定文件。");var center=Find(output,"RotCenterImagePoint");var result=new NImageCalibrationFileData(Text(input,"CreateCalibTime"),Integer(input,"TransNum"),Integer(input,"RotNum"),Integer(input,"CalibErrStatus"),Number(input,"TransError"),Number(input,"RotError"),Number(input,"TransWorldError"),Number(input,"RotWorldError"),Number(input,"PixelPrecisionX"),Number(input,"PixelPrecisionY"),Number(input,"PixelPrecision"),Array.AsReadOnly(Points("ImagePointLst")),Array.AsReadOnly(Points("WorldPointLst")),Integer(output,"RotDirectionState"),Integer(output,"IsRightCoorA"),new(Parse(center.Element("RotCenterImagePointX")?.Value??""),Parse(center.Element("RotCenterImagePointY")?.Value??""),Parse(center.Element("RotCenterImagePointR")?.Value??"")),Array.AsReadOnly(Find(output,"CalibMatrix").Elements("ParamValue").Select(e=>Parse(e.Value)).ToArray()));_ = Document(result);return result;
 }
}
