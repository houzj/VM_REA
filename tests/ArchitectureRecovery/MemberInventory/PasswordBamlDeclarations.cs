using System.Xml;
using System.Xml.Linq;
using System.Text.Json;
using System.Security.Cryptography;

internal static class PasswordBamlDeclarations
{
 public static void Export(string root)
 {
  var dir=Path.Combine(root,"docs/architecture-recovery/baml-decoded/passwordwindow");var events=new HashSet<string>{"Click","PreviewTextInput","PreviewKeyDown","TextChanged","CanExecute"};var files=new List<object>();
  foreach(var file in new[]{"passwordcheckwindow.xaml","passwordsetwindow.xaml","passwordmodifywindow.xaml","resourcedictionary.xaml"}){
   var path=Path.Combine(dir,file);var bytes=File.ReadAllBytes(path);using var xr=XmlReader.Create(new MemoryStream(bytes),new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null});var doc=XDocument.Load(xr,LoadOptions.SetLineInfo);string? Name(XElement e)=>e.Attributes().FirstOrDefault(a=>a.Name.LocalName=="Name")?.Value;int Line(XObject e)=>(e as IXmlLineInfo)?.LineNumber??0;
   var registrations=doc.Descendants().SelectMany(e=>e.Attributes().Where(a=>events.Contains(a.Name.LocalName)).Select(a=>new{control=e.Name.LocalName,controlName=Name(e),nearestNamedParent=e.Ancestors().Select(Name).FirstOrDefault(n=>n!=null),@event=a.Name.LocalName,handler=a.Value,line=Line(a),runtimeBindingConfirmed=false,connectionIdMappingConfirmed=false})).ToArray();
   var controls=doc.Descendants().Where(e=>new[]{"Button","ToggleButton","PasswordBox","TextBox"}.Contains(e.Name.LocalName)).Select(e=>new{type=e.Name.LocalName,name=Name(e),line=Line(e),attributes=e.Attributes().Where(a=>!a.IsNamespaceDeclaration).ToDictionary(a=>a.Name.ToString(),a=>a.Value)}).ToArray();
   var setters=doc.Descendants().Where(e=>e.Name.LocalName=="Setter"&&new[]{"PasswordBox.MaxLength","TextBox.MaxLength","PasswordBox.PasswordChar","IsCancel","IsDefault"}.Contains((string?)e.Attribute("Property"))).Select(e=>new{property=(string?)e.Attribute("Property"),value=(string?)e.Attribute("Value"),styleKey=e.Ancestors().FirstOrDefault(p=>p.Name.LocalName=="Style")?.Attributes().FirstOrDefault(a=>a.Name.LocalName=="Key")?.Value,line=Line(e)}).ToArray();
   files.Add(new{path=Path.GetRelativePath(root,path).Replace('\\','/'),sha256=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(),rootType=doc.Root!.Name.LocalName,rootAttributes=doc.Root.Attributes().Where(a=>!a.IsNamespaceDeclaration).ToDictionary(a=>a.Name.ToString(),a=>a.Value),events=registrations,controls,setters});
  }
  File.WriteAllText(Path.Combine(root,"docs/architecture-recovery/password-baml-declarations.json"),JsonSerializer.Serialize(new{schemaVersion=1,complete=false,files,authority="XDocument reads only offline decompiler XML with DTD prohibited and null resolver; no XAML objects or vendor code created.",runtimeExecuted=false},new JsonSerializerOptions{WriteIndented=true})+Environment.NewLine);Console.WriteLine(JsonSerializer.Serialize(new{files=files.Count,runtimeExecuted=false}));
 }
}
