using System.Security.Cryptography;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

internal static class PasswordParameterResource
{
 public static void Export(string root)
 {
  const string original="D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Collection/SaveImage/SaveImageAlgorithmTab.xml";
  byte[] bytes=File.ReadAllBytes(original);
  using var reader=XmlReader.Create(new MemoryStream(bytes),new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null});
  var doc=XDocument.Load(reader,LoadOptions.SetLineInfo);
  var nodes=doc.Descendants().Where(e=>e.Name.LocalName=="PasswordString").Select(e=>new{
   element=e.Name.ToString(),name=(string?)e.Attribute("Name"),line=((IXmlLineInfo)e).LineNumber,
   hierarchy=e.AncestorsAndSelf().Reverse().Select(a=>new{element=a.Name.ToString(),name=(string?)a.Attribute("Name"),line=((IXmlLineInfo)a).LineNumber}).ToArray(),
   fields=e.Elements().Where(c=>new[]{"Description","DisplayName","AccessMode","CustomVisible"}.Contains(c.Name.LocalName)).Select(c=>new{name=c.Name.LocalName,value=c.Value,line=((IXmlLineInfo)c).LineNumber}).ToArray(),
   valueState=e.Elements().Where(c=>new[]{"CurValue","DefaultValue"}.Contains(c.Name.LocalName)).Select(c=>new{name=c.Name.LocalName,characterCount=c.Value.Length,empty=c.Value.Length==0,line=((IXmlLineInfo)c).LineNumber}).ToArray()
  }).ToArray();
  if(nodes.Length!=1||nodes[0].name!="Password")throw new InvalidDataException("Password resource scope changed.");
  var result=new{schemaVersion=1,complete=false,artifact=new{path=original,sha256=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(),byteLength=bytes.Length},nodes,runtimeExecuted=false,limitations=new[]{"Exact selected shipped XML declaration only; no runtime loading, visibility or SDK password effect proven.","Password/default values are not exported; only length and empty state recorded.","No installed file was changed."}};
  File.WriteAllText(Path.Combine(root,"docs/architecture-recovery/password-parameter-resource.json"),JsonSerializer.Serialize(result,new JsonSerializerOptions{WriteIndented=true})+Environment.NewLine);
  Console.WriteLine(JsonSerializer.Serialize(new{nodes=nodes.Length,hierarchy=nodes[0].hierarchy,fields=nodes[0].fields}));
 }
}
