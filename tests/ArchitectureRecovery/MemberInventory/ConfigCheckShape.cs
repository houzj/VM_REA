using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text.Json;

internal static class ConfigCheckShape
{
 public static void Export(string artifact,string output)
 {
  var bytes=File.ReadAllBytes(artifact);using var pe=new PEReader(new MemoryStream(bytes));var md=pe.GetMetadataReader();
  string TypeName(EntityHandle h){if(h.Kind==HandleKind.TypeDefinition){var t=md.GetTypeDefinition((TypeDefinitionHandle)h);return md.GetString(t.Namespace)+"."+md.GetString(t.Name);}if(h.Kind==HandleKind.TypeReference){var t=md.GetTypeReference((TypeReferenceHandle)h);return md.GetString(t.Namespace)+"."+md.GetString(t.Name);}return h.Kind.ToString();}
  string MemberName(EntityHandle h)=>h.Kind==HandleKind.MethodDefinition?md.GetString(md.GetMethodDefinition((MethodDefinitionHandle)h).Name):h.Kind==HandleKind.MemberReference?md.GetString(md.GetMemberReference((MemberReferenceHandle)h).Name):h.Kind.ToString();
  var names=new HashSet<string>{"VM.Core.IMVSGroup","VM.Core.VmModuleSet","VM.Core.VmModule","VM.Core.VmProxyModule"};
  var types=md.TypeDefinitions.Where(h=>names.Contains(TypeName(h))).Select(h=>{var t=md.GetTypeDefinition(h);return new{type=TypeName(h),token=$"0x{MetadataTokens.GetToken(h):x8}",baseType=TypeName(t.BaseType),baseKind=t.BaseType.Kind.ToString(),declaredConfigCheck=t.GetMethods().Where(m=>md.GetString(md.GetMethodDefinition(m).Name).EndsWith("get_ConfigCheck",StringComparison.Ordinal)).Select(m=>{var v=md.GetMethodDefinition(m);return new{token=$"0x{MetadataTokens.GetToken(m):x8}",name=md.GetString(v.Name),attributes=v.Attributes.ToString(),signatureBytes=Convert.ToHexString(md.GetBlobBytes(v.Signature)).ToLowerInvariant()};}).ToArray(),methodImpls=t.GetMethodImplementations().Select(ih=>{var i=md.GetMethodImplementation(ih);return new{bodyToken=$"0x{MetadataTokens.GetToken(i.MethodBody):x8}",bodyName=MemberName(i.MethodBody),declarationToken=$"0x{MetadataTokens.GetToken(i.MethodDeclaration):x8}",declarationName=MemberName(i.MethodDeclaration)};}).ToArray()};}).ToArray();
  if(types.Length!=4)throw new InvalidDataException("Expected four core types.");
  File.WriteAllText(output,JsonSerializer.Serialize(new{schemaVersion=1,artifact=Path.GetFullPath(artifact),sha256=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(),mvid=md.GetGuid(md.GetModuleDefinition().Mvid),types,meaning="Selected TypeDef inheritance, complete declared ConfigCheck-name getters and complete MethodImpl rows; no runtime receiver or plugin-loaded override proof."},new JsonSerializerOptions{WriteIndented=true})+Environment.NewLine);
  Console.WriteLine(JsonSerializer.Serialize(new{types=types.Length,runtimeExecuted=false}));
 }
}
