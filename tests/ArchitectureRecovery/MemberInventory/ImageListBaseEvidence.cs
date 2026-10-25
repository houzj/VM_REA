using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;

internal static class ImageListBaseEvidence
{
    public static void Export(string root)
    {
        var output=Path.Combine(root,"docs/architecture-recovery");
        var rea=JsonNode.Parse(File.ReadAllText(Path.Combine(output,"image-list-base-rea.json")))!;
        var childPath=rea["childArtifact"]!["subject"]!["local_path"]!.GetValue<string>();var basePath=rea["baseArtifact"]!["subject"]!["local_path"]!.GetValue<string>();
        byte[] Verified(string path,string expected){var bytes=File.ReadAllBytes(path);if(Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant()!=expected)throw new InvalidDataException("Original artifact hash changed.");return bytes;}
        var childBytes=Verified(childPath,rea["childArtifact"]!["subject"]!["digest"]!["sha256"]!.GetValue<string>());var baseBytes=Verified(basePath,rea["baseArtifact"]!["subject"]!["digest"]!["sha256"]!.GetValue<string>());
        using var childPe=new PEReader(new MemoryStream(childBytes));using var basePe=new PEReader(new MemoryStream(baseBytes));var childMd=childPe.GetMetadataReader();var baseMd=basePe.GetMetadataReader();
        string TypeName(MetadataReader md,TypeDefinitionHandle handle){var type=md.GetTypeDefinition(handle);return md.GetString(type.Namespace)+"."+md.GetString(type.Name);}
        var childTypeHandle=childMd.TypeDefinitions.Single(h=>TypeName(childMd,h)=="ImageSourceModuleControl.ImageListPanel");var childType=childMd.GetTypeDefinition(childTypeHandle);
        if(childType.BaseType.Kind!=HandleKind.TypeReference)throw new InvalidDataException("Expected external base TypeRef.");var baseRef=childMd.GetTypeReference((TypeReferenceHandle)childType.BaseType);
        if(baseRef.ResolutionScope.Kind!=HandleKind.AssemblyReference)throw new InvalidDataException("Expected assembly-scoped base reference.");
        var assemblyRef=childMd.GetAssemblyReference((AssemblyReferenceHandle)baseRef.ResolutionScope);var assemblyDef=baseMd.GetAssemblyDefinition();
        string PublicToken(byte[] publicKey)=>Convert.ToHexString(SHA1.HashData(publicKey).TakeLast(8).Reverse().ToArray()).ToLowerInvariant();
        var referenceToken=Convert.ToHexString(childMd.GetBlobBytes(assemblyRef.PublicKeyOrToken)).ToLowerInvariant();var definitionToken=PublicToken(baseMd.GetBlobBytes(assemblyDef.PublicKey));
        if(childMd.GetString(assemblyRef.Name)!=baseMd.GetString(assemblyDef.Name)||assemblyRef.Version!=assemblyDef.Version||referenceToken!=definitionToken||childMd.GetString(assemblyRef.Culture)!=baseMd.GetString(assemblyDef.Culture))throw new InvalidDataException("Base assembly reference identity mismatch.");
        string baseTypeName=childMd.GetString(baseRef.Namespace)+"."+childMd.GetString(baseRef.Name);if(baseTypeName!="VMControls.WPF.ImageListPanelBase")throw new InvalidDataException("Unexpected base type.");
        var baseTypeHandle=baseMd.TypeDefinitions.Single(h=>TypeName(baseMd,h)==baseTypeName);var baseType=baseMd.GetTypeDefinition(baseTypeHandle);
        var map=JsonNode.Parse(File.ReadAllText(Path.Combine(output,"xaml-entry-inventory.json")))!;
        var required=map["candidates"]!.AsArray().Where(e=>e!["rootClass"]?.GetValue<string>()=="ImageSourceModuleControl.ImageListPanel"&&e["kind"]!.GetValue<string>() is "input-event" or "ui-lifecycle-event" or "event-setter").Select(e=>e!["value"]!.GetValue<string>()).Distinct().OrderBy(n=>n,StringComparer.Ordinal).ToArray();
        var methods=new List<object>();foreach(var handle in baseType.GetMethods()){
            var method=baseMd.GetMethodDefinition(handle);var name=baseMd.GetString(method.Name);if(!required.Contains(name))continue;
            var il=method.RelativeVirtualAddress==0?null:basePe.GetMethodBody(method.RelativeVirtualAddress).GetILBytes();
            methods.Add(new{name,token=$"0x{MetadataTokens.GetToken(handle):x8}",attributes=method.Attributes.ToString(),rva=$"0x{method.RelativeVirtualAddress:x8}",signatureBytes=Convert.ToHexString(baseMd.GetBlobBytes(method.Signature)).ToLowerInvariant(),ilSize=il?.Length,ilSha256=il is null?null:Convert.ToHexString(SHA256.HashData(il)).ToLowerInvariant(),ilBytes=il is null?null:Convert.ToHexString(il).ToLowerInvariant()});
        }
        if(methods.Count!=required.Length)throw new InvalidDataException("Expected each requested base handler to be unique and present.");
        var sourceFiles=new[]{"analysis/decompiled/ImageSourceModuleControl/ImageSourceModuleControl/ImageListPanel.xaml.cs","analysis/decompiled/VMControls.WPF/VMControls.WPF/ImageListPanelBase.cs"}.Select(p=>new{path=p,sha256=Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Path.Combine(root,p)))).ToLowerInvariant()}).ToArray();
        var result=new{schemaVersion=1,complete=false,authority="Focused System.Reflection.Metadata/PEReader only; no vendor assembly loading, execution or semantic decompilation",child=new{path=childPath,sha256=rea["childArtifact"]!["subject"]!["digest"]!["sha256"]!.GetValue<string>(),mvid=childMd.GetGuid(childMd.GetModuleDefinition().Mvid),type="ImageSourceModuleControl.ImageListPanel",typeToken=$"0x{MetadataTokens.GetToken(childTypeHandle):x8}",baseReferenceToken=$"0x{MetadataTokens.GetToken(childType.BaseType):x8}"},parent=new{path=basePath,sha256=rea["baseArtifact"]!["subject"]!["digest"]!["sha256"]!.GetValue<string>(),mvid=baseMd.GetGuid(baseMd.GetModuleDefinition().Mvid),type=baseTypeName,typeToken=$"0x{MetadataTokens.GetToken(baseTypeHandle):x8}",assemblyName=baseMd.GetString(assemblyDef.Name),assemblyVersion=assemblyDef.Version.ToString(),publicKeyToken=definitionToken},assemblyReference=new{name=childMd.GetString(assemblyRef.Name),version=assemblyRef.Version.ToString(),publicKeyToken=referenceToken},sourceFiles,requiredHandlers=required,methods,link=new{derivedTree="analysis/decompiled/ImageSourceModuleControl",derivedType="ImageSourceModuleControl.ImageListPanel",baseTree="analysis/decompiled/VMControls.WPF",baseType=baseTypeName},limitations=new[]{"Original metadata confirms this declared base and assembly reference only; actual runtime assembly loading remains unverified.","Raw CIL bytes/hashes identify selected original bodies; they are not a C# equivalence proof for the local decompiled text.","Event delegate compatibility, generated connector dispatch and execution remain separate unknowns."}};
        File.WriteAllText(Path.Combine(output,"image-list-base-metadata.json"),JsonSerializer.Serialize(result,new JsonSerializerOptions{WriteIndented=true})+Environment.NewLine);
        Console.WriteLine(JsonSerializer.Serialize(new{baseType=baseTypeName,requiredHandlers=required.Length,recoveredMethodBodies=methods.Count,assemblyReferenceMatched=true,runtimeVerified=false}));
    }
}
