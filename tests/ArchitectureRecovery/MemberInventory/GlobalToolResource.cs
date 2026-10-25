using System.Security.Cryptography;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
internal static class GlobalToolResource
{
 public static void Export(string root){
  const string file="D:/Program Files/VisionMaster4.4.0/Applications/myLibs/GlobalToolResource/GlobalToolItems.xml";
  var bytes=File.ReadAllBytes(file);using var stream=new MemoryStream(bytes);using var reader=XmlReader.Create(stream,new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null});var doc=XDocument.Load(reader,LoadOptions.SetLineInfo);var rows=new List<object>();int named=0,external=0;
  void Visit(XElement node,string id,string[] parents,bool hidden){
   var fields=node.Elements().Where(e=>e.Name.LocalName is not ("items" or "toolGroupItemInfo")).ToDictionary(e=>e.Name.LocalName,e=>e.Value.Trim());string? name=fields.GetValueOrDefault("name");hidden=hidden||fields.GetValueOrDefault("isDisplayInMenu")=="false";var hierarchy=parents.Append(name??("[unnamed:"+id+"]")).ToArray();var group=node.Element("toolGroupItemInfo")?.Elements().ToDictionary(e=>e.Name.LocalName,e=>e.Value.Trim())??new Dictionary<string,string>();var line=(IXmlLineInfo)node;if(name is not null)named++;if(fields.GetValueOrDefault("isShowExternal")=="true")external++;
   rows.Add(new{id,name,type=node.Name.LocalName,line=line.LineNumber,column=line.LinePosition,hierarchy,inheritedMenuHidden=hidden,fields,group,registrationConfirmed=false,fullBehaviorClosed=false});int index=0;foreach(var child in node.Element("items")?.Elements()??[])Visit(child,id+"."+(index++),hierarchy,hidden);
  }
  Visit(doc.Root!,"root",[],false);
  var result=new{schemaVersion=1,complete=false,authority="Shipped XML; DTD prohibited, XmlResolver null, line information retained; no vendor execution",resource=new{path=file,sha256=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant()},counts=new{allNodes=rows.Count,namedDeclarations=named,externalDeclared=external},entries=rows,limitations=new[]{"Separate resource and receiver type from all four main-menu variants.","XML declared external visibility is not proof of runtime visibility or callback reachability.","No current-path resolver, original BAML, installed host or SDK execution."}};
  File.WriteAllText(Path.Combine(root,"docs/architecture-recovery/global-tool-resource.json"),JsonSerializer.Serialize(result,new JsonSerializerOptions{WriteIndented=true})+Environment.NewLine);Console.WriteLine(JsonSerializer.Serialize(result.counts));
 }
}
