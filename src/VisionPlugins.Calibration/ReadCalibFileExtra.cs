using System.IO;
using System.Xml.Linq;
using VisionFramework;
using VisionPluginContracts;

namespace VisionPlugins.Calibration;

public sealed record ReadCalibFileExtra(string Name,string Type,bool IsArray) {
 public DataKind Kind=>Type=="string"?DataKind.Text:Type=="int"?DataKind.Integer:IsArray?DataKind.NumberArray:DataKind.Number;
 public ValueCardinality Cardinality=>Type=="int"&&IsArray?ValueCardinality.Array:ValueCardinality.Scalar;
 public PortDescriptor InputDescriptor()=>Type switch{
  "int"=>new(Name,DataKind.Integer,false){SupportsDeferredIntegerRead=true},
  "float"=>new(Name,DataKind.NumberArray,false,true){SupportsDeferredNumericRead=true,SupportsDeferredIntegerRead=true},
  _=>new(Name,DataKind.Text,false){SupportsDeferredTextRead=true}};
 public PortDescriptor OutputDescriptor()=>new(Name,Kind,false){Cardinality=Cardinality};
 public bool SameChannel(DataKind kind)=>Type=="string"?kind==DataKind.Text:Type=="int"?kind==DataKind.Integer:kind is DataKind.Number or DataKind.NumberArray;
 public static IReadOnlyList<ReadCalibFileExtra> Parse(string xml){if(string.IsNullOrEmpty(xml))return [];try{var document=XDocument.Parse(xml);var items=document.Root?.Element("Categorys")?.Elements().FirstOrDefault(c=>c.Attribute("Name")?.Value=="Output")?.Element("Items");if(items is null)return [];var values=new List<ReadCalibFileExtra>();foreach(var item in items.Elements()){string? name=item.Attribute("Name")?.Value,type=item.Attribute("ValueType")?.Value,others=item.Attribute("Others")?.Value;if(string.IsNullOrEmpty(name)||type is null||others is null||(!others.Contains("IsResultShow=t",StringComparison.Ordinal)&&!others.Contains("IsResultShow=T",StringComparison.Ordinal)))continue;type=type.ToLowerInvariant();if(type is not("int" or "float" or "string"))continue;values.Add(new(name,type,type!="string"&&string.Equals(item.Attribute("IsArray")?.Value,"true",StringComparison.OrdinalIgnoreCase)));}return values;}catch(System.Xml.XmlException e){throw new InvalidDataException("额外结果展示定义无法解析。",e);}}
 public void Copy(IReadOnlyDictionary<string,PortValue> inputs,IDictionary<string,PortValue> output)=>Copy(new ModuleExecutionContext("read-calib-extra",Environment.CurrentDirectory,inputs,new Dictionary<string,System.Text.Json.JsonElement>()),output);
 public void Copy(ModuleExecutionContext context,IDictionary<string,PortValue> output){
  if(Type=="string"){
   var text=ReadCalibFilePlugin.ReadTextInput(context,Name,0,0x1ff);
   if(text.ErrorCode==0xe0000012){if(text.RequiredBytes<0||text.RequiredBytes==int.MaxValue)throw new InvalidDataException("Invalid required text buffer size.");text=ReadCalibFilePlugin.ReadTextInput(context,Name,0,text.RequiredBytes+1);}
   if(text.ErrorCode==0&&text.Count>0)output[Name]=PortValue.Text(text.Value.Split('\0')[0]);return;
  }
  bool integer=Type=="int";
  ToolNumericInputReadResult Read(int index){
   if(!integer){var floating=ReadCalibFilePlugin.ReadFloatingInput(context,Name,index);if(floating.ErrorCode==0)return floating;}
   var integral=ReadCalibFilePlugin.ReadIntegerInput(context,Name,index);return new(integral.ErrorCode,integral.Count,integral.Value);
  }
  var read=Read(0);if(read.ErrorCode!=0||read.Count<=0)return;
  var values=new List<double>();bool forceArray=IsArray;
  if(output.TryGetValue(Name,out var existing)){
   forceArray|=existing.Kind==DataKind.NumberArray||existing.Cardinality==ValueCardinality.Array;
   if(existing.Kind==DataKind.NumberArray)values.AddRange(existing.As<IReadOnlyList<double>>());
   else if(existing.Kind==DataKind.Number)values.AddRange(existing.Elements().Select(v=>v.As<double>()));
   else if(existing.Kind==DataKind.Integer)values.AddRange(existing.Elements().Select(v=>(double)v.As<long>()));
  }
  void Write(int index,double value){value=integer?unchecked((int)value):(float)value;if(index<values.Count)values[index]=value;else values.Add(value);}
  Write(0,read.Value);
  for(int index=1;index<read.Count;index++){read=Read(index);if(read.ErrorCode==0)Write(index,read.Value);}
  bool array=forceArray||values.Count>1;
  output[Name]=integer?(array?PortValue.IntegerArray(values.Select(v=>(long)unchecked((int)v))):PortValue.Integer(unchecked((int)values[0]))):(array?PortValue.Numbers(values):PortValue.Number(values[0]));
 }
}
