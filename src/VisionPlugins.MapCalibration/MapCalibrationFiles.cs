using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
namespace VisionPlugins.MapCalibration;

public sealed record MapCalibrationSave(uint ErrorCode,bool Written);
public sealed record MapCalibrationFileRead(uint ErrorCode,float[]? Replacement);

/// <summary>Source-confirmed module XML/binary matrix input. Replacement may exist on a failed short XML read.</summary>
public static class MapCalibrationFiles
{
 public static string NativeFloat(float value)=>Regex.Replace(value.ToString("G8",CultureInfo.InvariantCulture).Replace('E','e'),@"e([+-])(\d+)$",m=>"e"+m.Groups[1].Value+m.Groups[2].Value.PadLeft(3,'0'));
 public static MapCalibrationFileRead Read(string path){
  try{
   if(!path.Contains(".xml",StringComparison.Ordinal)){
    byte[] data=File.ReadAllBytes(path);
    // Native target buffer is fixed at 37 bytes; unsafe short/oversized payloads are not replayed.
    if(data.Length!=36)return new(0xe0000018,null);
    return new(0,Enumerable.Range(0,9).Select(i=>BitConverter.ToSingle(data,i*4)).ToArray());
   }
   var document=XDocument.Load(path);var container=document.Root?.Element("CalibOutputParam")?.Element("CalibFloatListParam");
   if(container is null)return new(0xe0000018,null);
   var values=container.Elements().ToArray();var matrix=new float[9];float previous=0;
   for(int i=0;i<Math.Min(9,values.Length);i++){
    var token=Regex.Match(values[i].Value,@"^\s*[+-]?(?:\d+(?:\.\d*)?|\.\d+)(?:[eE][+-]?\d+)?").Value;
    if(float.TryParse(token,NumberStyles.Float,CultureInfo.InvariantCulture,out float parsed))previous=parsed;
    matrix[i]=previous;
   }
   return new(values.Length==9?0u:0xe0000018u,matrix);
  }catch(Exception ex)when(ex is IOException or UnauthorizedAccessException or XmlException or ArgumentException or NotSupportedException){return new(0xe0000201,null);}
 }
 public static string FileModeXml(IReadOnlyList<float> matrix){
  if(matrix.Count!=9)throw new ArgumentException("Calibration matrices contain nine floats.");
  var s=new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<CalibInfo>\r\n    <CalibOutputParam>\r\n        <CalibFloatListParam ParamName=\"CalibMatrix\" DataType=\"FloatList\">\r\n");
  foreach(float value in matrix)s.Append("            <ParamValue>").Append(NativeFloat(value)).Append("</ParamValue>\r\n");
  return s.Append("        </CalibFloatListParam>\r\n    </CalibOutputParam>\r\n</CalibInfo>\r\n").ToString();
 }
 public static MapCalibrationSave SaveFileModeOperation(string path,IReadOnlyList<float> matrix){if(!path.EndsWith(".xml",StringComparison.Ordinal))return new(0xe0000204,false);return new(0,SaveFileMode(path,matrix));}
 public static bool SaveFileMode(string path,IReadOnlyList<float> matrix){try{File.WriteAllText(path,FileModeXml(matrix),new UTF8Encoding(false));return true;}catch(Exception error)when(error is IOException or UnauthorizedAccessException){return false;}}
}


