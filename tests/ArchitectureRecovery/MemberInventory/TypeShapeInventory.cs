using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text.Json;

internal sealed class TypeShapeInventory : ISignatureTypeProvider<string,object?>
{
 public static void Export(string path,string typeName,string output)
 {
  var bytes=File.ReadAllBytes(path);using var pe=new PEReader(new MemoryStream(bytes));var md=pe.GetMetadataReader();var provider=new TypeShapeInventory();
  var handle=md.TypeDefinitions.Single(h=>{var t=md.GetTypeDefinition(h);return md.GetString(t.Namespace)+"."+md.GetString(t.Name)==typeName;});var type=md.GetTypeDefinition(handle);
  var fields=type.GetFields().Select(h=>{var f=md.GetFieldDefinition(h);var raw=md.GetBlobBytes(f.Signature);return new{token=$"0x{MetadataTokens.GetToken(h):x8}",name=md.GetString(f.Name),attributes=f.Attributes.ToString(),signatureBytes=Convert.ToHexString(raw).ToLowerInvariant(),signatureSha256=Convert.ToHexString(SHA256.HashData(raw)).ToLowerInvariant(),decodedType=f.DecodeSignature(provider,null)};}).ToArray();
  var result=new{artifact=Path.GetFullPath(path),sha256=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(),mvid=md.GetGuid(md.GetModuleDefinition().Mvid),type=typeName,typeToken=$"0x{MetadataTokens.GetToken(handle):x8}",fields,meaning="Metadata field signature decode only; assembly scopes are metadata identities, not runtime binding. No instance field values read and no target assembly loaded."};
  File.WriteAllText(output,JsonSerializer.Serialize(result,new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine(JsonSerializer.Serialize(result));
 }
 public string GetArrayType(string e,ArrayShape s)=>e+"["+new string(',',s.Rank-1)+"]";
 public string GetByReferenceType(string e)=>e+"&";
 public string GetFunctionPointerType(MethodSignature<string> s)=>"methodptr("+string.Join(",",s.ParameterTypes)+")->"+s.ReturnType;
 public string GetGenericInstantiation(string t,ImmutableArray<string> a)=>t+"<"+string.Join(",",a)+">";
 public string GetGenericMethodParameter(object? c,int i)=>"!!"+i;
 public string GetGenericTypeParameter(object? c,int i)=>"!"+i;
 public string GetModifiedType(string m,string t,bool required)=>t+(required?" modreq(":" modopt(")+m+")";
 public string GetPinnedType(string t)=>t+" pinned";
 public string GetPointerType(string e)=>e+"*";
 public string GetPrimitiveType(PrimitiveTypeCode c)=>"System."+c;
 public string GetSZArrayType(string e)=>e+"[]";
 public string GetTypeFromDefinition(MetadataReader r,TypeDefinitionHandle h,byte raw){var t=r.GetTypeDefinition(h);return t.GetDeclaringType().IsNil?r.GetString(t.Namespace)+"."+r.GetString(t.Name):GetTypeFromDefinition(r,t.GetDeclaringType(),raw)+"+"+r.GetString(t.Name);}
 public string GetTypeFromReference(MetadataReader r,TypeReferenceHandle h,byte raw){var t=r.GetTypeReference(h);string scope=t.ResolutionScope.Kind==HandleKind.AssemblyReference?r.GetString(r.GetAssemblyReference((AssemblyReferenceHandle)t.ResolutionScope).Name)+"::":"";return scope+r.GetString(t.Namespace)+"."+r.GetString(t.Name);}
 public string GetTypeFromSpecification(MetadataReader r,object? c,TypeSpecificationHandle h,byte raw)=>r.GetTypeSpecification(h).DecodeSignature(this,c);
}
