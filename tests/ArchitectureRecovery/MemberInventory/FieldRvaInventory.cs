using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text.Json;

internal static class FieldRvaInventory
{
 public static void Export(string path,string tokenText,int expectedSize,string output)
 {
  if(expectedSize<=0||expectedSize>4096)throw new InvalidDataException("Only a bounded explicitly sized field is supported.");
  var artifact=Path.GetFullPath(path);var bytes=File.ReadAllBytes(artifact);using var pe=new PEReader(new MemoryStream(bytes));var md=pe.GetMetadataReader();
  int token=Convert.ToInt32(tokenText.Replace("0x","",StringComparison.OrdinalIgnoreCase),16);var handle=MetadataTokens.EntityHandle(token);
  if(handle.Kind!=HandleKind.FieldDefinition)throw new InvalidDataException("Expected a FieldDef token.");var fh=(FieldDefinitionHandle)handle;var field=md.GetFieldDefinition(fh);
  if((field.Attributes&(FieldAttributes.Static|FieldAttributes.HasFieldRVA))!=(FieldAttributes.Static|FieldAttributes.HasFieldRVA))throw new InvalidDataException("Field must be static and have RVA data.");
  var signature=md.GetBlobReader(field.Signature);if(signature.ReadByte()!=0x06||signature.ReadByte()!=0x11)throw new InvalidDataException("Expected a value-type field signature.");
  var fieldType=signature.ReadTypeHandle();if(fieldType.Kind!=HandleKind.TypeDefinition||signature.RemainingBytes!=0)throw new InvalidDataException("Expected one exact TypeDef signature.");
  var type=md.GetTypeDefinition((TypeDefinitionHandle)fieldType);var layout=type.GetLayout();if(layout.Size!=expectedSize)throw new InvalidDataException("Declared field type size differs from requested bound.");
  int rva=field.GetRelativeVirtualAddress();if(rva<=0)throw new InvalidDataException("Missing field RVA.");
  var section=pe.PEHeaders.SectionHeaders.Single(s=>rva>=s.VirtualAddress&&(long)rva+expectedSize<=(long)s.VirtualAddress+s.SizeOfRawData);
  int offset=checked(section.PointerToRawData+rva-section.VirtualAddress);if(offset<0||(long)offset+expectedSize>bytes.Length)throw new InvalidDataException("Field data extends outside file.");
  var payload=pe.GetSectionData(rva).GetContent(0,expectedSize).ToArray();if(!payload.SequenceEqual(bytes.AsSpan(offset,expectedSize).ToArray()))throw new InvalidDataException("RVA/file-offset bytes disagree.");
  string Hash(byte[] data)=>Convert.ToHexString(SHA256.HashData(data)).ToLowerInvariant();
  string Name(TypeDefinition t)=>md.GetString(t.Namespace)+"."+md.GetString(t.Name);
  var data=new{schemaVersion=1,artifact,sha256=Hash(bytes),mvid=md.GetGuid(md.GetModuleDefinition().Mvid),fieldToken=$"0x{token:x8}",declaringType=Name(md.GetTypeDefinition(field.GetDeclaringType())),name=md.GetString(field.Name),attributes=field.Attributes.ToString(),signatureHex=Convert.ToHexString(md.GetBlobBytes(field.Signature)).ToLowerInvariant(),signatureSha256=Hash(md.GetBlobBytes(field.Signature)),fieldTypeToken=$"0x{MetadataTokens.GetToken(fieldType):x8}",fieldType=Name(type),layoutSize=layout.Size,packingSize=layout.PackingSize,rva,fileOffset=offset,section=section.Name,byteLength=payload.Length,dataHex=Convert.ToHexString(payload).ToLowerInvariant(),dataSha256=Hash(payload),runtimeExecuted=false,meaning="Bounded original static field RVA bytes only; no assembly loading, provider execution or resource decryption."};
  File.WriteAllText(Path.GetFullPath(output),JsonSerializer.Serialize(data,new JsonSerializerOptions{WriteIndented=true})+"\n");Console.WriteLine(JsonSerializer.Serialize(new{fieldToken=data.fieldToken,byteLength=data.byteLength,runtimeExecuted=false}));
 }
}
