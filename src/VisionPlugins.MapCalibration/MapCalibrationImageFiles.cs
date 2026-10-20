using System.Globalization;
using System.IO;
using System.Text;
namespace VisionPlugins.MapCalibration;
public readonly record struct MapCalibrationPose(float X,float Y,float Angle);
public static class MapCalibrationImageFiles {
 public static string Xml(MapCalibrationImageResult result,DateTime created,MapCalibrationPose current=default,MapCalibrationPose target=default,MapCalibrationPose? teach=null){
  if(result.Matrix.Length!=9)throw new ArgumentException("Nine matrix coefficients required.");
  var s=new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<CalibInfo>\r\n    <CalibInputParam>\r\n");
  void Param(string name,string type,string value)=>s.Append("        <CalibParam ParamName=\"").Append(name).Append("\" DataType=\"").Append(type).Append("\">\r\n            <ParamValue>").Append(value).Append("</ParamValue>\r\n        </CalibParam>\r\n");
  Param("CreateCalibTime","string",created.ToString("yyyy-MM-dd HH:mm:ss",CultureInfo.InvariantCulture));Param("CalibType","string","MapCalib");Param("MapError","int",MapCalibrationFiles.NativeFloat(result.MapError));Param("CalibStatus1","float","1");Param("CalibStatus2","float","1");Param("PixelScale1","float",MapCalibrationFiles.NativeFloat(result.PixelScale1));Param("PixelScale2","float",MapCalibrationFiles.NativeFloat(result.PixelScale2));
  var h=result.Matrix;float precision=h[0]==0?0:(float)((Math.Sqrt((double)h[3]*h[3]+(double)h[0]*h[0])+Math.Sqrt((double)h[1]*h[1]+(double)h[4]*h[4]))*.5);Param("PixelPrecision","float",MapCalibrationFiles.NativeFloat(precision));
  s.Append("    </CalibInputParam>\r\n    <CalibOutputParam>\r\n");
  void Pose(string name,MapCalibrationPose pose){s.Append("        <PointF ParamName=\"").Append(name).Append("\" DataType=\"CalibPointF\">\r\n");foreach(var item in new[]{("PointX",pose.X),("PointY",pose.Y),("R",pose.Angle)})s.Append("            <").Append(name).Append(item.Item1).Append('>').Append(MapCalibrationFiles.NativeFloat(item.Item2)).Append("</").Append(name).Append(item.Item1).Append(">\r\n");s.Append("        </PointF>\r\n");}
  if(current.X> -999990f)Pose("SnapWorldCur",current);if(target.X> -999990f)Pose("SnapWorldObj",target);if(teach is {} t)Pose("TeachWorld",t);
  s.Append("        <CalibFloatListParam ParamName=\"CalibMatrix\" DataType=\"FloatList\">\r\n");foreach(float value in h)s.Append("            <ParamValue>").Append(MapCalibrationFiles.NativeFloat(value)).Append("</ParamValue>\r\n");return s.Append("        </CalibFloatListParam>\r\n    </CalibOutputParam>\r\n</CalibInfo>\r\n").ToString();
 }
 public static MapCalibrationSave Save(string path,MapCalibrationImageResult? result,bool teachEnabled,bool teachTriggered,bool sourcePending,bool targetPending,MapCalibrationPose current=default,MapCalibrationPose target=default,MapCalibrationPose? teach=null,DateTime? created=null,bool validateExtension=true){
  if(validateExtension&&!path.EndsWith(".xml",StringComparison.Ordinal))return new(0xe0000204,false);
  if(teachEnabled&&!teachTriggered)return new(0xe0000516,false);if(sourcePending||targetPending)return new(0xe0000517,false);if(result is null)return new(0x80100203,false);
  try{File.WriteAllText(path,Xml(result,created??DateTime.Now,current,target,teachTriggered?teach:null),new UTF8Encoding(false));return new(0,true);}catch(Exception error)when(error is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException){return new(0,false);}
 }
}

