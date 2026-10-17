using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
namespace VisionPlugins.Calibration;

/// <summary>Module XML and point-table text, reconstructed from WriteXmlData and NPointList.</summary>
public static class TranslationFile {
 static readonly CultureInfo Culture=CultureInfo.InvariantCulture;
 static string F(double value){string text=((float)value).ToString("G8",Culture);int e=text.IndexOf('E');if(e>=0)text=text[..e]+"e"+text[e+1]+text[(e+2)..].PadLeft(3,'0');return text;}
 public static string ExportPointText(float[,] points,int count){var text=new StringBuilder();for(int i=0;i<count;i++){for(int axis=0;axis<5;axis++){text.Append(points[i,axis].ToString("F3",Culture).PadRight(10));if(axis<4)text.Append(' ');}text.Append("\r\n");}return text.ToString();}
 public static string ExportPointRows(IEnumerable<IReadOnlyList<string>> rows){var text=new StringBuilder();foreach(var row in rows){if(row.Count!=5||row.Any(v=>!float.TryParse(v,NumberStyles.Float,Culture,out var number)||!float.IsFinite(number)))throw new InvalidDataException("标定点格式错误。");for(int axis=0;axis<5;axis++){text.Append(row[axis].PadRight(10));if(axis<4)text.Append(' ');}text.Append("\r\n");}return text.ToString();}
 public static float[][] ImportPointText(string text,int capacity,Action<int,float[]>? acceptRow=null){ArgumentNullException.ThrowIfNull(text);if(capacity is <0 or >32)throw new ArgumentOutOfRangeException(nameof(capacity));var result=new List<float[]>();foreach(string line in text.Split(["\r\n"],StringSplitOptions.RemoveEmptyEntries).Take(capacity)){var fields=line.Split(' ',StringSplitOptions.RemoveEmptyEntries);if(fields.Length<5)throw new InvalidDataException("标定点文件每行至少需要五列。");var row=new float[5];for(int k=0;k<fields.Length;k++)if(!IsPointNumber(fields[k])||!float.TryParse(fields[k],NumberStyles.Float,Culture,out float v)||!float.IsFinite(v))throw new InvalidDataException("标定点格式错误。");else if(k<5)row[k]=v;acceptRow?.Invoke(result.Count,row);result.Add(row);}return result.ToArray();}
 public static bool IsPointNumber(string value)=>!Regex.IsMatch(value,"[^0-9.-]")&&!Regex.IsMatch(value,"[0-9]*[.][0-9]*[.][0-9]*")&&!Regex.IsMatch(value,"[0-9]*[-][0-9]*[-][0-9]*")&&Regex.IsMatch(value,"(^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$)|(^([-]|[0-9])[0-9]*$)");
 public static void Write(string path,float[,] points,int count,IReadOnlyDictionary<string,JsonElement> values,NPointSolveResult result,bool hasTeach,IReadOnlyList<float> teach,int snapOrigin=0){
  if(result.ErrorCode!=0)throw new InvalidOperationException("尚无有效标定结果。");
  int trans=values["CalibPointTotalNum"].GetInt32(),rot=values["RotPointTotalNum"].GetInt32();if(count<trans+rot)throw new InvalidOperationException("标定点尚未采集完整。");bool manual=values["CalibPointGet"].GetInt32()==1;
  if(values["TeachEnable"].GetBoolean()&&!hasTeach)throw new InvalidOperationException("尚未获取示教点。");
  var input=new XElement("CalibInputParam");var output=new XElement("CalibOutputParam");var root=new XElement("CalibInfo",input,output);
  void Param(XElement parent,string name,string type,object value)=>parent.Add(new XElement("CalibParam",new XAttribute("ParamName",name),new XAttribute("DataType",type),new XElement("ParamValue",value)));
  Param(input,"CreateCalibTime","string",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss",Culture));Param(input,"CalibType","string","NPointCalib");Param(input,"CameraMode","int",values["CameraMode"].GetInt32()==3?"CameraMove":"CameraStatic");Param(input,"TransNum","int",trans);Param(input,"RotNum","int",rot);Param(input,"CalibErrStatus","int",result.Status);Param(input,"TransError","float",F(result.OffsetError));Param(input,"RotError","float",F(rot>0?result.RotateError:-999));Param(input,"TransWorldError","float",F(result.OffsetWorldError));Param(input,"RotWorldError","float",F(rot>0?result.RotateWorldError:-999));var h=result.Matrix;Param(input,"PixelPrecisionX","float",F(Math.Sqrt(h[0]*h[0]+h[3]*h[3])));Param(input,"PixelPrecisionY","float",F(Math.Sqrt(h[1]*h[1]+h[4]*h[4])));Param(input,"PixelPrecision","float",F(result.PixelPrecision));
  foreach(bool image in new[]{true,false}){var list=new XElement("CalibPointFListParam",new XAttribute("ParamName",image?"ImagePointLst":"WorldPointLst"),new XAttribute("DataType","CalibPointList"));for(int i=0;i<trans+rot;i++)list.Add(new XElement("PointF",new XElement("X",F(points[i,image?0:2])),new XElement("Y",F(points[i,image?1:3])),new XElement("R",F(points[i,image?5:4]))));input.Add(list);}
  Param(output,"RotDirectionState","int",result.Direction);Param(output,"IsRightCoorA","int",TranslationSolve.RightCoordinate(h));
  void Point(string name,string prefix,double x,double y,double angle)=>output.Add(new XElement("PointF",new XAttribute("ParamName",name),new XAttribute("DataType","CalibPointF"),new XElement(prefix+"X",F(x)),new XElement(prefix+"Y",F(y)),new XElement(name=="RotCenterImagePoint"?"RotCenterImageR":name=="RotCenterWorldPoint"?"RotCenterWorldR":name+"R",F(angle))));
  Point("RotCenterImagePoint","RotCenterImagePoint",result.CenterImageX,result.CenterImageY,-999);Point("RotCenterWorldPoint","RotCenterWorldPoint",result.CenterWorldX,result.CenterWorldY,-999);
  if(hasTeach){
   int origin=snapOrigin,worldOrigin=(trans-1)/2;bool enabled=values["TeachEnable"].GetBoolean(),snapMissing=!enabled||teach[0]<=-99998;
   void Pose(string name,double x,double y,double angle)=>output.Add(new XElement("PointF",new XAttribute("ParamName",name),new XAttribute("DataType","CalibPointF"),new XElement(name+"PointX",F(x)),new XElement(name+"PointY",F(y)),new XElement(name+"R",F(angle))));
   Pose("SnapImage",snapMissing?points[origin,0]:teach[0],snapMissing?points[origin,1]:teach[1],snapMissing?points[origin,5]:teach[2]);Pose("SnapWorld",enabled?points[worldOrigin,2]:-999,enabled?points[worldOrigin,3]:-999,enabled?points[worldOrigin,4]:-999);Pose("TeachWorld",enabled?teach[3]:-999,enabled?teach[4]:-999,enabled?teach[5]:-999);
  }
  output.Add(new XElement("CalibFloatListParam",new XAttribute("ParamName","CalibMatrix"),new XAttribute("DataType","FloatList"),h.Select(v=>new XElement("ParamValue",F(v)))));
  var settings=new XmlWriterSettings{Encoding=new UTF8Encoding(false),Indent=true,IndentChars="    ",NewLineChars="\r\n"};using var writer=XmlWriter.Create(path,settings);new XDocument(new XDeclaration("1.0","UTF-8",null),root).Save(writer);
 }
}
