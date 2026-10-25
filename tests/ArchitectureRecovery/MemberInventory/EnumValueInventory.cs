using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text.Json;

internal static class EnumValueInventory
{
 public static void Export(string path,string typeName,string output)
 {
  var bytes=File.ReadAllBytes(path);using var pe=new PEReader(new MemoryStream(bytes));var md=pe.GetMetadataReader();var handle=md.TypeDefinitions.Single(h=>{var t=md.GetTypeDefinition(h);return md.GetString(t.Namespace)+"."+md.GetString(t.Name)==typeName;});var type=md.GetTypeDefinition(handle);if(type.BaseType.Kind!=HandleKind.TypeReference)throw new InvalidDataException("Expected Enum base TypeRef.");var b=md.GetTypeReference((TypeReferenceHandle)type.BaseType);if(md.GetString(b.Namespace)+"."+md.GetString(b.Name)!="System.Enum")throw new InvalidDataException("Not an enum.");
  var values=type.GetFields().Where(h=>!md.GetFieldDefinition(h).GetDefaultValue().IsNil).Select(h=>{var f=md.GetFieldDefinition(h);var c=md.GetConstant(f.GetDefaultValue());var raw=md.GetBlobBytes(c.Value);if(c.TypeCode!=ConstantTypeCode.Int32||raw.Length!=4)throw new InvalidDataException("Expected bounded Int32 enum literal.");return new{name=md.GetString(f.Name),token=$"0x{MetadataTokens.GetToken(h):x8}",value=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(raw),constantType=c.TypeCode.ToString(),constantHex=Convert.ToHexString(raw).ToLowerInvariant(),attributes=f.Attributes.ToString()};}).ToArray();
  File.WriteAllText(output,JsonSerializer.Serialize(new{schemaVersion=1,artifact=Path.GetFullPath(path),sha256=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(),mvid=md.GetGuid(md.GetModuleDefinition().Mvid),type=typeName,typeToken=$"0x{MetadataTokens.GetToken(handle):x8}",values,meaning="Original metadata Enum literal constants; no vendor assembly loading or UI execution."},new JsonSerializerOptions{WriteIndented=true})+Environment.NewLine);Console.WriteLine(JsonSerializer.Serialize(new{type=typeName,values=values.Length}));
 }
}
