using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Resources;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;

internal static class BamlCarrierInventory
{
 public static void Export(string root)
 {
  var dir=Path.Combine(root,"docs/architecture-recovery");var manifest=JsonNode.Parse(File.ReadAllText(Path.Combine(dir,"baml-carrier-artifacts.json")))!;var artifacts=new List<object>();string Hash(byte[] b)=>Convert.ToHexString(SHA256.HashData(b)).ToLowerInvariant();
  foreach(var a in manifest["artifacts"]!.AsArray()){
   var path=a!["path"]!.GetValue<string>();var bytes=File.ReadAllBytes(path);using var pe=new PEReader(new MemoryStream(bytes));var md=pe.GetMetadataReader();var mvid=md.GetGuid(md.GetModuleDefinition().Mvid).ToString();if(Hash(bytes)!=a["sha256"]!.GetValue<string>()||mvid!=a["mvid"]!.GetValue<string>())throw new InvalidDataException("BAML carrier artifact changed.");
   var resources=new List<object>();foreach(var h in md.ManifestResources){var r=md.GetManifestResource(h);var name=md.GetString(r.Name);if(!name.EndsWith(".g.resources",StringComparison.Ordinal)||!r.Implementation.IsNil)continue;
    var expected=a["resources"]!.AsArray().Single(v=>v!["name"]!.GetValue<string>()==name)!;var rd=pe.PEHeaders.CorHeader!.ResourcesDirectory;var relative=checked((int)r.Offset);if(relative<0||relative>rd.Size-4)throw new InvalidDataException("Managed resource prefix outside directory.");var rva=checked(rd.RelativeVirtualAddress+relative);var block=pe.GetSectionData(rva);var length=BitConverter.ToInt32(block.GetContent(0,4).ToArray());if(length<0||length>rd.Size-relative-4||length>block.Length-4)throw new InvalidDataException("Managed resource data outside directory or section.");var section=pe.PEHeaders.SectionHeaders.Single(s=>rva>=s.VirtualAddress&&(long)rva+4+length<=(long)s.VirtualAddress+s.SizeOfRawData);var offset=checked(section.PointerToRawData+rva-section.VirtualAddress+4);if(offset<0||(long)offset+length>bytes.Length)throw new InvalidDataException("Managed resource outside file.");var data=block.GetContent(4,length).ToArray();if(!data.SequenceEqual(bytes.AsSpan(offset,length).ToArray())||Hash(data)!=expected["data_sha256"]!.GetValue<string>()||length!=expected["data_length"]!.GetValue<int>())throw new InvalidDataException("REA resource size/digest or RVA/file bytes disagree.");
    using var reader=new ResourceReader(new MemoryStream(data));var entries=new List<object>();var all=0;var it=reader.GetEnumerator();while(it.MoveNext()){all++;var key=(string)it.Key;if(!key.EndsWith(".baml",StringComparison.OrdinalIgnoreCase))continue;reader.GetResourceData(key,out var type,out var raw);if(type!="ResourceTypeCode.Stream"&&type!="ResourceTypeCode.ByteArray")throw new InvalidDataException("Unsupported BAML encoded value type.");if(raw.Length<4)throw new InvalidDataException("Missing encoded resource length.");var size=BitConverter.ToInt32(raw,0);if(size<0||size!=raw.Length-4)throw new InvalidDataException("Encoded BAML value length mismatch.");var payload=raw.AsSpan(4,size).ToArray();entries.Add(new{key,encodedType=type,encodedByteLength=raw.Length,encodedSha256=Hash(raw),payloadByteLength=size,payloadSha256=Hash(payload),payloadPrefixHex=Convert.ToHexString(payload.AsSpan(0,Math.Min(size,32))).ToLowerInvariant(),semanticAnalysisIncluded=false});}
    resources.Add(new{name,rva,fileOffset=offset,resourceOffset=r.Offset,byteLength=length,sha256=Hash(data),entryCount=all,bamlCount=entries.Count,bamlEntries=entries});
   }
   if(resources.Count!=a["resources"]!.AsArray().Count)throw new InvalidDataException("Resource manifest scope mismatch.");artifacts.Add(new{key=a["key"]!.GetValue<string>(),path,sha256=Hash(bytes),mvid,resources});
  }
  var result=new{schemaVersion=1,complete=false,artifacts,authority="Original managed resource bytes, bounded RVA/direct-file equality, REA manifest resource digests and ResourceReader.GetResourceData only; resource Value never read.",limitations=new[]{"This raw byte inventory includes no BAML semantic analysis; separate selected-decode evidence may establish offline output mappings.","Key naming does not prove equality to local extracted XAML or actual runtime activation.","No XAML object creation, vendor assembly loading or target/SDK execution."}};File.WriteAllText(Path.Combine(dir,"baml-carriers.json"),JsonSerializer.Serialize(result,new JsonSerializerOptions{WriteIndented=true})+Environment.NewLine);Console.WriteLine(JsonSerializer.Serialize(new{artifacts=artifacts.Count,runtimeExecuted=false}));
 }
}
