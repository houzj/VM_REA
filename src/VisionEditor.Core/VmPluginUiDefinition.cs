using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Linq;
namespace VisionEditor.Core;

// Declarative evidence, not a generic parameter-grid description. No target DLL is loaded.
public sealed record XmlUiItem(string Kind,string Name,IReadOnlyDictionary<string,string> Properties,string OriginalXml);
public sealed record XmlUiCategory(string Name,IReadOnlyList<XmlUiItem> Items);
public sealed record XmlUiTab(string Name,IReadOnlyList<XmlUiCategory> Categories,string OriginalXml);
public sealed record VmPluginUiDefinition(float? ChineseWidth,float? ChineseHeight,IReadOnlyList<XmlUiTab> Tabs,IReadOnlyList<XmlUiItem> ExternItems,IReadOnlyList<XmlUiItem> BottomButtons,string OriginalXml) {
 public static VmPluginUiDefinition Load(string path){
  using var reader=XmlReader.Create(path,new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null,MaxCharactersInDocument=16*1024*1024});
  var root=XDocument.Load(reader,LoadOptions.PreserveWhitespace).Root??throw new InvalidDataException("Missing plugin UI root.");
  if(root.Name.LocalName!="AlgorithmTabRoot")throw new InvalidDataException("Expected original AlgorithmTabRoot declaration.");
  float? Size(string name){var e=root.Element(name);if(e is null)return null;if(!float.TryParse(e.Value,System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture,out var value)||!float.IsFinite(value)||value<=0)throw new InvalidDataException("Invalid declared UI dimension.");return value;}
  XmlUiItem Item(XElement element){string kind=element.Name.LocalName,name=element.Attribute("Name")?.Value??"";var properties=new Dictionary<string,string>(StringComparer.Ordinal);foreach(var e in element.Elements().Where(e=>!e.HasElements)){if(!properties.TryAdd(e.Name.LocalName,e.Value))throw new InvalidDataException("Duplicate UI property.");}return new(kind,name,new ReadOnlyDictionary<string,string>(properties),element.ToString(SaveOptions.DisableFormatting));}
  var tabs=root.Element("Tabs")?.Elements("Tab").Select(tab=>new XmlUiTab(tab.Attribute("Name")?.Value??"",Array.AsReadOnly((tab.Element("Categorys")?.Elements("Category")??[]).Select(category=>new XmlUiCategory(category.Attribute("Name")?.Value??"",Array.AsReadOnly((category.Element("Items")?.Elements()??[]).Select(Item).ToArray()))).ToArray()),tab.ToString(SaveOptions.DisableFormatting))).ToArray()??[];
  IReadOnlyList<XmlUiItem> Items(string section)=>Array.AsReadOnly((root.Element(section)?.Elements()??[]).Select(Item).ToArray());
  return new(Size("ZHCustomTabWidth"),Size("ZHCustomTabHeight"),Array.AsReadOnly(tabs),Items("ExternParamsList"),Items("BottomExcuteButtons"),root.ToString(SaveOptions.DisableFormatting));
 }
}
