using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Resources;
using System.Security.Cryptography;
using System.Text.Json;
internal static class PasswordResourceInventory
{
 public static void Export(string root){
  const string path="D:/Program Files/VisionMaster4.4.0/Applications/myLibs/PasswordWindowUI.dll";
  var bytes=File.ReadAllBytes(path);using var pe=new PEReader(new MemoryStream(bytes));var md=pe.GetMetadataReader();var resources=new List<object>();string Hash(byte[] b)=>Convert.ToHexString(SHA256.HashData(b)).ToLowerInvariant();
  foreach(var h in md.ManifestResources){var resource=md.GetManifestResource(h);if(!resource.Implementation.IsNil)continue;int rva=checked(pe.PEHeaders.CorHeader!.ResourcesDirectory.RelativeVirtualAddress+(int)resource.Offset);var block=pe.GetSectionData(rva);var prefix=block.GetContent(0,4).ToArray();int length=BitConverter.ToInt32(prefix);if(length<0||length>block.Length-4)throw new InvalidDataException("Resource size outside PE section");var data=block.GetContent(4,length).ToArray();var entries=new List<object>();using var reader=new ResourceReader(new MemoryStream(data));var enumerator=reader.GetEnumerator();while(enumerator.MoveNext()){string key=(string)enumerator.Key;reader.GetResourceData(key,out var type,out var raw);entries.Add(new{key,type,encodedValueBytes=raw.Length,encodedValueSha256=Hash(raw)});}resources.Add(new{name=md.GetString(resource.Name),resourceOffset=resource.Offset,byteLength=data.Length,sha256=Hash(data),entries});
  }
  var result=new{schemaVersion=1,artifact=path,sha256=Hash(bytes),mvid=md.GetGuid(md.GetModuleDefinition().Mvid),resources,authority="PEReader embedded managed resource bytes plus ResourceReader.GetResourceData; never reads resource Value or loads vendor code",limitations=new[]{"Resource keys and encoded value digests only; BAML was not decoded and connection-ID/event mapping is unconfirmed.","No ResourceReader.Value deserialization, XAML object construction, original UI or SDK execution."}};
  File.WriteAllText(Path.Combine(root,"docs/architecture-recovery/password-check-resource-map.json"),JsonSerializer.Serialize(result,new JsonSerializerOptions{WriteIndented=true})+Environment.NewLine);Console.WriteLine(JsonSerializer.Serialize(new{resources=resources.Count,vendorExecuted=false}));
 }
}
