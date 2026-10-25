using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text.Json;

internal static class InterfaceInventory
{
 public static void Export(string path,string typeName,string output)
 {
  var artifact=Path.GetFullPath(path);var bytes=File.ReadAllBytes(artifact);using var pe=new PEReader(new MemoryStream(bytes));var md=pe.GetMetadataReader();
  string Name(TypeDefinition t)=>md.GetString(t.Namespace)+"."+md.GetString(t.Name);
  var owner=md.TypeDefinitions.Single(h=>Name(md.GetTypeDefinition(h))==typeName);var type=md.GetTypeDefinition(owner);
  object Describe(EntityHandle h){
   if(h.Kind==HandleKind.TypeReference){var t=md.GetTypeReference((TypeReferenceHandle)h);object? assembly=null;if(t.ResolutionScope.Kind==HandleKind.AssemblyReference){var a=md.GetAssemblyReference((AssemblyReferenceHandle)t.ResolutionScope);assembly=new{name=md.GetString(a.Name),version=a.Version.ToString(),publicKeyOrToken=Convert.ToHexString(md.GetBlobBytes(a.PublicKeyOrToken)).ToLowerInvariant()};}return new{token=$"0x{MetadataTokens.GetToken(h):x8}",kind=h.Kind.ToString(),name=md.GetString(t.Namespace)+"."+md.GetString(t.Name),scope=$"0x{MetadataTokens.GetToken(t.ResolutionScope):x8}",assembly};}
   if(h.Kind==HandleKind.TypeDefinition)return new{token=$"0x{MetadataTokens.GetToken(h):x8}",kind=h.Kind.ToString(),name=Name(md.GetTypeDefinition((TypeDefinitionHandle)h))};
   if(h.Kind==HandleKind.TypeSpecification)return new{token=$"0x{MetadataTokens.GetToken(h):x8}",kind=h.Kind.ToString(),rawSignature=Convert.ToHexString(md.GetBlobBytes(md.GetTypeSpecification((TypeSpecificationHandle)h).Signature)).ToLowerInvariant()};
   throw new InvalidDataException("Unsupported interface handle.");
  }
  var interfaces=type.GetInterfaceImplementations().Select(h=>{var i=md.GetInterfaceImplementation(h);return new{implementationToken=$"0x{MetadataTokens.GetToken(h):x8}",target=Describe(i.Interface)};}).ToArray();
  var data=new{schemaVersion=1,artifact,sha256=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(),mvid=md.GetGuid(md.GetModuleDefinition().Mvid),type=typeName,typeToken=$"0x{MetadataTokens.GetToken(owner):x8}",baseType=type.BaseType.IsNil?null:Describe(type.BaseType),interfaces,meaning="Exact direct InterfaceImpl metadata; inherited interfaces, runtime binding and activation remain separate. No vendor assembly loaded."};
  File.WriteAllText(Path.GetFullPath(output),JsonSerializer.Serialize(data,new JsonSerializerOptions{WriteIndented=true})+"\n");Console.WriteLine(JsonSerializer.Serialize(new{type=typeName,directInterfaces=interfaces.Length,runtimeExecuted=false}));
 }
}
