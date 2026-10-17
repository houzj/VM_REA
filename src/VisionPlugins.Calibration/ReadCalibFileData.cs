using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using VisionFramework;

namespace VisionPlugins.Calibration;

public sealed record ReadCalibFileParameter(string Name,string Type,IReadOnlyList<string> Names,IReadOnlyList<float> Values,string? Text=null,IReadOnlyList<float[]>? Points=null);
public sealed record ReadCalibFileLoad(uint ErrorCode,ReadCalibFileData? Data,bool ReplaceData,bool CommitPath);
public sealed record ReadCalibFilePort(string Name,DataKind Kind);

/// <summary>Independent reader of the original module's typed XML, not an algorithm SDK file reader.</summary>
public sealed class ReadCalibFileData {
 public IReadOnlyList<ReadCalibFileParameter> Inputs{get;init;}=[];
 public IReadOnlyList<ReadCalibFileParameter> Outputs{get;init;}=[];
 public string Xml{get;init;}="";
 static readonly Regex FloatPrefix=new(@"^\s*([+-]?(?:(?:\d+(?:\.\d*)?|\.\d+)(?:[eE][+-]?\d+)?|inf(?:inity)?|nan))",RegexOptions.IgnoreCase|RegexOptions.CultureInvariant);
 static readonly Regex IntPrefix=new(@"^\s*([+-]?\d+)",RegexOptions.CultureInvariant);
 static bool Float(string text,out float value){var match=FloatPrefix.Match(text);return float.TryParse(match.Success?match.Groups[1].Value:"",NumberStyles.Float,CultureInfo.InvariantCulture,out value);}
 static bool Integer(string text,out int value){var match=IntPrefix.Match(text);return int.TryParse(match.Success?match.Groups[1].Value:"",NumberStyles.Integer,CultureInfo.InvariantCulture,out value);}
 public static ReadCalibFileLoad Parse(string xml){
  XDocument document;try{document=XDocument.Parse(xml);}catch(System.Xml.XmlException){return new(0xe0000201,null,false,false);}
  var input=new List<ReadCalibFileParameter>();var output=new List<ReadCalibFileParameter>();uint error=0;float lastFloat=0;
  ReadCalibFileData Result()=>new(){Inputs=input.ToArray(),Outputs=output.ToArray(),Xml=xml};
  bool Section(XElement section,List<ReadCalibFileParameter> target,bool isInput){
   foreach(var item in section.Elements()){
    string? name=item.Attribute("ParamName")?.Value,type=item.Attribute("DataType")?.Value;var children=item.Elements().ToArray();
    // The original dereferences malformed attributes. The independent reader reports a controlled error.
    if(name is null||type is null||children.Length==0){error=0xe000000a;return false;}
    if(type=="int"){if(Integer(children[0].Value,out int integer))target.Add(new(name,type,[],[(float)integer],integer.ToString(CultureInfo.InvariantCulture)));}
    else if(type=="float"){if(Float(children[0].Value,out float number)){lastFloat=number;target.Add(new(name,type,[],[number]));}}
    else if(type=="string")target.Add(new(name,type,[],[],children[0].Value));
    else if(isInput&&type=="CalibPointList"){
     var points=new List<float[]>();foreach(var point in children){var fields=point.Elements().ToArray();if(fields.Length!=3){error=0xe0000207;break;}var coordinates=new float[3];for(int i=0;i<3;i++){if(Float(fields[i].Value,out float number))lastFloat=number;coordinates[i]=lastFloat;}points.Add(coordinates);}target.Add(new(name,type,[],[],Points:points));
    }else if(!isInput&&type=="FloatList"){
     var values=new List<float>();foreach(var child in children){if(Float(child.Value,out float number))lastFloat=number;values.Add(lastFloat);}target.Add(new(name,type,[],values));
    }else if(!isInput&&type=="CalibPointF"){
     var names=new List<string>();var values=new List<float>();for(int i=0;i<children.Length;i++){if(i==3){error=0xe0000207;break;}if(Float(children[i].Value,out float number))lastFloat=number;if(i!=2||lastFloat>=-990){names.Add(children[i].Name.LocalName);values.Add(lastFloat);}}target.Add(new(name,type,names,values));
    }
   }return true;
  }
  if(document.Root is {} root){var ins=root.Element("CalibInputParam");if(ins is not null&&!Section(ins,input,true))return new(error,Result(),true,false);var outs=root.Element("CalibOutputParam");if(outs is null)return new(0xe0000017,Result(),true,false);if(!Section(outs,output,false))return new(error,Result(),true,false);}
  return new(error,Result(),true,true);
 }
 public static ReadCalibFileLoad Read(string path){try{return Parse(File.ReadAllText(path));}catch(Exception e)when(e is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException){return new(0xe0000201,null,false,false);}}
 public IReadOnlyDictionary<string,PortValue> Values(){var result=new Dictionary<string,PortValue>();foreach(string type in new[]{"int","float","string","FloatList"})foreach(var item in Outputs.Where(p=>p.Type==type||(type=="float"&&p.Type=="CalibPointF"))){
   if(type=="int")result[item.Name]=PortValue.Integer(long.Parse(item.Text!,CultureInfo.InvariantCulture));
   else if(item.Type=="float")result[item.Name]=PortValue.Number(item.Values[0]);
   else if(type=="string")result[item.Name]=PortValue.Text(item.Text??"");
   else if(type=="FloatList"){if(item.Values.Count>0)result[item.Name]=PortValue.Numbers(item.Values.Select(v=>(double)v));}
   else for(int i=0;i<item.Names.Count;i++)result[item.Names[i]]=PortValue.Number(item.Values[i]);
  }return result;
 }
 public IReadOnlyList<ReadCalibFilePort> Ports()=>Values().Select(p=>new ReadCalibFilePort(p.Key,p.Value.Kind)).ToArray();
 public string InputDescription(){var result=new StringBuilder();foreach(string type in new[]{"int","float","string","CalibPointList"}){var items=Inputs.Where(p=>p.Type==type).ToArray();if(items.Length==0)continue;result.Append(type).Append('#');foreach(var item in items){result.Append(item.Name).Append(',');if(type=="int"||type=="string")result.Append(item.Text);else if(item.Type=="float")result.Append(item.Values[0].ToString("F6",CultureInfo.InvariantCulture));else result.AppendJoin(',',item.Points!.Select(p=>string.Join('~',p.Select(v=>v.ToString("F6",CultureInfo.InvariantCulture)))));result.Append('$');}result.Append(';');}return result.ToString();}
}
