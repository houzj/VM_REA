using System.IO;
using System.Xml.Linq;
using VisionFramework;

namespace VisionPlugins.Calibration;

public sealed record ReadCalibFileExtra(string Name,string Type,bool IsArray) {
 public DataKind Kind=>Type=="string"?DataKind.Text:IsArray?DataKind.NumberArray:Type=="int"?DataKind.Integer:DataKind.Number;
 public static IReadOnlyList<ReadCalibFileExtra> Parse(string xml){if(string.IsNullOrEmpty(xml))return [];try{var document=XDocument.Parse(xml);var items=document.Root?.Element("Categorys")?.Elements().FirstOrDefault(c=>c.Attribute("Name")?.Value=="Output")?.Element("Items");if(items is null)return [];var values=new List<ReadCalibFileExtra>();foreach(var item in items.Elements()){string? name=item.Attribute("Name")?.Value,type=item.Attribute("ValueType")?.Value,others=item.Attribute("Others")?.Value;if(string.IsNullOrEmpty(name)||type is null||others is null||(!others.Contains("IsResultShow=t",StringComparison.Ordinal)&&!others.Contains("IsResultShow=T",StringComparison.Ordinal)))continue;type=type.ToLowerInvariant();if(type is not("int" or "float" or "string"))continue;values.Add(new(name,type,type!="string"&&string.Equals(item.Attribute("IsArray")?.Value,"true",StringComparison.OrdinalIgnoreCase)));}return values;}catch(System.Xml.XmlException e){throw new InvalidDataException("额外结果展示定义无法解析。",e);}}
 public void Copy(IReadOnlyDictionary<string,PortValue> inputs,IDictionary<string,PortValue> output){if(!inputs.TryGetValue(Name,out var input))return;if(Type=="string"){if(input.Kind==DataKind.Text)output[Name]=PortValue.Text(input.As<string>().Split('\0')[0]);return;}if(input.Kind==DataKind.NumberArray){var numbers=input.As<IReadOnlyList<double>>();if(numbers.Count==0)return;output[Name]=PortValue.Numbers(numbers.Select(v=>Type=="int"?(double)unchecked((int)v):(float)v));return;}if(Type=="int"){if(input.Kind==DataKind.Integer)output[Name]=PortValue.Integer(unchecked((int)input.As<long>()));return;}if(input.Kind==DataKind.Number)output[Name]=PortValue.Number((float)input.As<double>());else if(input.Kind==DataKind.Integer)output[Name]=PortValue.Number((float)input.As<long>());}
}
