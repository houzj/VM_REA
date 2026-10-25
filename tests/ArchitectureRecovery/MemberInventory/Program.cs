using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Buffers.Binary;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// Parses text with the SDK's Roslyn. Does not load or execute vendor assemblies.
if(args.Length==4&&args[0]=="--manifest-evidence"){
    if(args[2]!=Path.GetFileName(args[2])||args[3]!=Path.GetFileName(args[3])||!args[2].EndsWith(".json",StringComparison.Ordinal)||!args[3].EndsWith(".json",StringComparison.Ordinal)||string.Equals(args[2],args[3],StringComparison.OrdinalIgnoreCase))throw new ArgumentException("Distinct evidence JSON basenames required.");
    ConnectorEvidence.Export(Path.GetFullPath(args[1]),args[2],args[3]);return;
}
if(args.Length==2&&args[0]=="--xaml-handlers"){XamlHandlerInventory.Export(Path.GetFullPath(args[1]));return;}
if(args.Length==2&&args[0]=="--xaml-remainder"){XamlRemainderInventory.Export(Path.GetFullPath(args[1]));return;}
if(args.Length==2&&args[0]=="--resource-protection-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"resource-protection-artifact-identities.json","resource-protection-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--zip-stream-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"zip-stream-artifact-identities.json","zip-stream-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--password-ui-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"password-ui-artifact-identities.json","password-ui-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--base-window-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"base-window-artifact-identities.json","base-window-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--global-tool-resource"){GlobalToolResource.Export(Path.GetFullPath(args[1]));return;}
if(args.Length==2&&args[0]=="--global-toolbar-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"global-toolbar-artifact-identities.json","global-toolbar-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--toolbar-effects-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"toolbar-effects-artifact-identities.json","toolbar-effects-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--password-check-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"password-check-artifact-identities.json","password-check-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--password-check-resources"){PasswordResourceInventory.Export(Path.GetFullPath(args[1]));return;}
if(args.Length==2&&args[0]=="--password-edit-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"password-edit-artifact-identities.json","password-edit-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--group-password-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"group-password-artifact-identities.json","group-password-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--group-file-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"group-file-artifact-identities.json","group-file-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--parameter-xml-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"parameter-xml-artifact-identities.json","parameter-xml-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--solution-lifecycle-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"solution-lifecycle-artifact-identities.json","solution-lifecycle-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--procedure-transfer-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"procedure-transfer-artifact-identities.json","procedure-transfer-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--external-tools-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"external-tools-artifact-identities.json","external-tools-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--settings-lifecycle-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"settings-lifecycle-artifact-identities.json","settings-lifecycle-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--main-menu-save-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"main-menu-save-artifact-identities.json","main-menu-save-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--configuration-hosts-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"configuration-hosts-artifact-identities.json","configuration-hosts-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--frontend-design-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"frontend-design-artifact-identities.json","frontend-design-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--run-exit-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"run-exit-artifact-identities.json","run-exit-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--menu-object-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"menu-object-artifact-identities.json","menu-object-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--menu-factory-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"menu-factory-artifact-identities.json","menu-factory-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--sync-wait-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"sync-wait-artifact-identities.json","sync-wait-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--menu-startup-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"menu-startup-artifact-identities.json","menu-startup-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--frontend-registry-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"frontend-registry-artifact-identities.json","frontend-registry-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--mq-entry-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"mq-entry-artifact-identities.json","mq-entry-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--startup-order-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"startup-order-artifact-identities.json","startup-order-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--undo-service-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"undo-service-artifact-identities.json","undo-service-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--parameter-candidates-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"parameter-candidates-artifact-identities.json","parameter-candidates-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--parameter-consumers-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"parameter-consumers-artifact-identities.json","parameter-consumers-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--group-candidate-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"group-candidate-artifact-identities.json","group-candidate-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--group-relation-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"group-relation-artifact-identities.json","group-relation-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--relation-tree-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"relation-tree-artifact-identities.json","relation-tree-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--module-scope-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"module-scope-artifact-identities.json","module-scope-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--selection-command-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"selection-command-artifact-identities.json","selection-command-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--delegate-command-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"delegate-command-artifact-identities.json","delegate-command-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--configuration-entry-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"configuration-entry-artifact-identities.json","configuration-entry-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--config-check-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"config-check-artifact-identities.json","config-check-cil-evidence.json");return;}
if(args.Length==3&&args[0]=="--config-check-shape"){ConfigCheckShape.Export(args[1],args[2]);return;}
if(args.Length==2&&args[0]=="--attached-event-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"attached-event-artifact-identities.json","attached-event-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--baml-carriers"){BamlCarrierInventory.Export(Path.GetFullPath(args[1]));return;}
if(args.Length==2&&args[0]=="--baml-load-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"baml-load-artifact-identities.json","baml-load-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--diagram-template-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"diagram-template-artifact-identities.json","diagram-template-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--guide-mode-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"guide-mode-artifact-identities.json","guide-mode-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--guide-storage-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"guide-storage-artifact-identities.json","guide-storage-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--diagram-container-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"diagram-container-artifact-identities.json","diagram-container-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--display-type-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"display-type-artifact-identities.json","display-type-cil-evidence.json");return;}
if(args.Length==4&&args[0]=="--enum-values"){EnumValueInventory.Export(args[1],args[2],args[3]);return;}
if(args.Length==2&&args[0]=="--current-path-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"current-path-artifact-identities.json","current-path-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--password-baml-declarations"){PasswordBamlDeclarations.Export(Path.GetFullPath(args[1]));return;}
if(args.Length==2&&args[0]=="--group-parameter-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"group-parameter-artifact-identities.json","group-parameter-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--password-parameter-resource"){PasswordParameterResource.Export(Path.GetFullPath(args[1]));return;}
if(args.Length==2&&args[0]=="--image-base-evidence"){ImageListBaseEvidence.Export(Path.GetFullPath(args[1]));return;}
if(args.Length==2&&args[0]=="--connectors"){ConnectorEvidence.Export(Path.GetFullPath(args[1]));return;}
if(args.Length==2&&args[0]=="--autoload-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"autoload-artifact-identities.json","autoload-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--role-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"role-artifact-identities.json","role-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--module-access-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"module-access-artifact-identities.json","module-access-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--password-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"password-artifact-identities.json","password-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--compatibility-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"compatibility-artifact-identities.json","compatibility-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--variable-migration-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"variable-migration-artifact-identities.json","variable-migration-cil-evidence.json");return;}
if(args.Length==2&&args[0]=="--variable-manager-evidence"){ConnectorEvidence.Export(Path.GetFullPath(args[1]),"variable-manager-artifact-identities.json","variable-manager-cil-evidence.json");return;}
if(args.Length==4&&args[0]=="--pe-export"){
    var artifactPath=Path.GetFullPath(args[1]);var artifactBytes=File.ReadAllBytes(artifactPath);
    using var imageReader=new PEReader(new MemoryStream(artifactBytes));var headers=imageReader.PEHeaders;var peHeader=headers.PEHeader??throw new InvalidDataException("Not PE.");
    int Offset(int rva,int length){foreach(var section in headers.SectionHeaders){long delta=(long)rva-section.VirtualAddress;if(delta>=0&&delta+length<=section.SizeOfRawData){long at=(long)section.PointerToRawData+delta;if(at+length>artifactBytes.Length)break;return checked((int)at);}}throw new InvalidDataException("RVA not file-backed.");}
    uint U32(int rva)=>BinaryPrimitives.ReadUInt32LittleEndian(artifactBytes.AsSpan(Offset(rva,4),4));
    ushort U16(int rva)=>BinaryPrimitives.ReadUInt16LittleEndian(artifactBytes.AsSpan(Offset(rva,2),2));
    string CString(int rva){var chars=new List<byte>();for(int i=0;i<4096;i++){var b=artifactBytes[Offset(checked(rva+i),1)];if(b==0)return System.Text.Encoding.ASCII.GetString(chars.ToArray());chars.Add(b);}throw new InvalidDataException("Export string too long.");}
    var directory=peHeader.ExportTableDirectory;if(directory.RelativeVirtualAddress==0)throw new InvalidDataException("No export table.");
    int table=directory.RelativeVirtualAddress;uint functions=U32(table+20),names=U32(table+24),ordinalBase=U32(table+16);
    if(functions>1000000||names>1000000)throw new InvalidDataException("Unreasonable export count.");
    int functionTable=checked((int)U32(table+28)),nameTable=checked((int)U32(table+32)),ordinalTable=checked((int)U32(table+36));var matches=new List<object>();
    for(int i=0;i<names;i++){var name=CString(checked((int)U32(checked(nameTable+4*i))));if(name!=args[2])continue;var ordinalIndex=U16(checked(ordinalTable+2*i));if(ordinalIndex>=functions)throw new InvalidDataException("Export ordinal outside table.");int functionRva=checked((int)U32(checked(functionTable+ordinalIndex*4)));bool forwarded=functionRva>=table&&(long)functionRva<table+(long)directory.Size;
        matches.Add(new{name,ordinal=ordinalBase+ordinalIndex,rva=$"0x{functionRva:x8}",virtualAddress=$"0x{peHeader.ImageBase+(uint)functionRva:x}",forwarded,forwarder=forwarded?CString(functionRva):null,fileOffset=forwarded?(int?)null:Offset(functionRva,32),first32Bytes=forwarded?null:Convert.ToHexString(artifactBytes.AsSpan(Offset(functionRva,32),32)).ToLowerInvariant()});}
    if(matches.Count!=1)throw new InvalidDataException("Expected one exact export.");
    var exportResult=new{artifactPath,sha256=Convert.ToHexString(SHA256.HashData(artifactBytes)).ToLowerInvariant(),machine=headers.CoffHeader.Machine.ToString(),imageBase=$"0x{peHeader.ImageBase:x}",exportDirectoryRva=$"0x{table:x8}",exportedNames=names,matches,meaning="Read-only PE export table and file-backed bytes. No disassembly, ABI/data flow, target loading or execution."};
    File.WriteAllText(Path.GetFullPath(args[3]),JsonSerializer.Serialize(exportResult,new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine(JsonSerializer.Serialize(exportResult));return;
}
if(args.Length==4&&args[0]=="--base-type"){
    var artifact=Path.GetFullPath(args[1]);var bytes=File.ReadAllBytes(artifact);
    using var pe=new PEReader(new MemoryStream(bytes));var md=pe.GetMetadataReader();
    var owner=md.TypeDefinitions.Single(h=>{var t=md.GetTypeDefinition(h);return md.GetString(t.Namespace)+"."+md.GetString(t.Name)==args[2];});var definition=md.GetTypeDefinition(owner);var target=definition.BaseType;
    string baseType;EntityHandle scope=default;object? assembly=null;
    if(target.Kind==HandleKind.TypeReference){var t=md.GetTypeReference((TypeReferenceHandle)target);baseType=md.GetString(t.Namespace)+"."+md.GetString(t.Name);scope=t.ResolutionScope;if(scope.Kind==HandleKind.AssemblyReference){var a=md.GetAssemblyReference((AssemblyReferenceHandle)scope);assembly=new{name=md.GetString(a.Name),version=a.Version.ToString(),publicKeyOrToken=Convert.ToHexString(md.GetBlobBytes(a.PublicKeyOrToken)).ToLowerInvariant()};}}
    else if(target.Kind==HandleKind.TypeDefinition){var t=md.GetTypeDefinition((TypeDefinitionHandle)target);baseType=md.GetString(t.Namespace)+"."+md.GetString(t.Name);}
    else throw new InvalidDataException("Base type is neither TypeRef nor TypeDef.");
    var baseTypeResult=new{artifact,sha256=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(),mvid=md.GetGuid(md.GetModuleDefinition().Mvid),type=args[2],ownerToken=$"0x{MetadataTokens.GetToken(owner):x8}",baseToken=$"0x{MetadataTokens.GetToken(target):x8}",baseKind=target.Kind.ToString(),baseType,resolutionScopeToken=scope.IsNil?null:$"0x{MetadataTokens.GetToken(scope):x8}",assembly,meaning="Exact TypeDef.Extends metadata only; runtime binding and construction unverified."};
    File.WriteAllText(Path.GetFullPath(args[3]),JsonSerializer.Serialize(baseTypeResult,new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine(JsonSerializer.Serialize(baseTypeResult));return;
}
if(args.Length==4&&args[0]=="--type-fields"){TypeShapeInventory.Export(args[1],args[2],args[3]);return;}
if(args.Length==4&&args[0]=="--type-interfaces"){InterfaceInventory.Export(args[1],args[2],args[3]);return;}
if(args.Length==5&&args[0]=="--field-rva"){FieldRvaInventory.Export(args[1],args[2],int.Parse(args[3]),args[4]);return;}
if(args.Length==4&&(args[0]=="--type-ref"||args[0]=="--type-ref-name")){
    var artifact=Path.GetFullPath(args[1]);var bytes=File.ReadAllBytes(artifact);
    using var pe=new PEReader(new MemoryStream(bytes));var metadata=pe.GetMetadataReader();
    var token=args[0]=="--type-ref-name"?MetadataTokens.GetToken(metadata.TypeReferences.Single(h=>{var t=metadata.GetTypeReference(h);return metadata.GetString(t.Namespace)+"."+metadata.GetString(t.Name)==args[2];})):Convert.ToInt32(args[2].Replace("0x","",StringComparison.OrdinalIgnoreCase),16);
    var handle=MetadataTokens.EntityHandle(token);if(handle.Kind!=HandleKind.TypeReference)throw new InvalidDataException("Expected a TypeRef token.");
    var reference=metadata.GetTypeReference((TypeReferenceHandle)handle);var scope=reference.ResolutionScope;
    object? assembly=null;if(scope.Kind==HandleKind.AssemblyReference){var a=metadata.GetAssemblyReference((AssemblyReferenceHandle)scope);assembly=new{name=metadata.GetString(a.Name),version=a.Version.ToString(),publicKeyOrToken=Convert.ToHexString(metadata.GetBlobBytes(a.PublicKeyOrToken)).ToLowerInvariant()};}
    var typeReferenceResult=new{artifact,sha256=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(),mvid=metadata.GetGuid(metadata.GetModuleDefinition().Mvid),token=$"0x{token:x8}",type=metadata.GetString(reference.Namespace)+"."+metadata.GetString(reference.Name),resolutionScopeToken=$"0x{MetadataTokens.GetToken(scope):x8}",assembly,meaning="Metadata reference identity only; runtime binding and callee behavior not established."};
    File.WriteAllText(Path.GetFullPath(args[3]),JsonSerializer.Serialize(typeReferenceResult,new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine(JsonSerializer.Serialize(typeReferenceResult));return;
}
var root=Path.GetFullPath(args.Single());
var output=Path.Combine(root,"docs/architecture-recovery");
var resources=JsonNode.Parse(File.ReadAllText(Path.Combine(output,"menu-variants.json")))!;
var members=new List<Member>();var sourceFiles=new List<object>();
foreach(var relative in new[]{"analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/TitleMenueItemData.cs","analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/ExternTitleMenueItemData.cs"}){
    var bytes=File.ReadAllBytes(Path.Combine(root,relative));var hash=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
    var text=System.Text.Encoding.UTF8.GetString(bytes);
    var tree=CSharpSyntaxTree.ParseText(text,new CSharpParseOptions(LanguageVersion.Preview),relative);
    var errors=tree.GetDiagnostics().Where(d=>d.Severity==DiagnosticSeverity.Error).ToArray();
    sourceFiles.Add(new{path=relative,sha256=hash,syntaxErrors=errors.Select(d=>new{message=d.ToString(),start=d.Location.SourceSpan.Start}).ToArray()});
    foreach(var type in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Where(c=>c.Identifier.Text is "TitleMenueItemData" or "ExternTitleMenueItemData")){
        foreach(var method in type.Members.OfType<MethodDeclarationSyntax>()){
            var span=method.GetLocation().GetLineSpan();var body=method.ToString();
            string At(SyntaxNode n)=>$"{n.GetLocation().GetLineSpan().StartLinePosition.Line+1}: {n}";
            members.Add(new Member(
                $"{type.Identifier.Text}.{method.Identifier.Text}@{span.StartLinePosition.Line+1}",type.Identifier.Text,method.Identifier.Text,
                relative,hash,span.StartLinePosition.Line+1,span.EndLinePosition.Line+1,
                method.Modifiers.Any(SyntaxKind.PublicKeyword),method.Modifiers.Any(SyntaxKind.StaticKeyword),method.ParameterList.Parameters.Count,
                method.ReturnType+" "+method.Identifier+method.ParameterList,body,method.Span.Start,method.Span.Length,
                method.DescendantNodes().OfType<InvocationExpressionSyntax>().Select(n=>At(n.Expression)).Distinct().ToArray(),
                method.DescendantNodes().OfType<IfStatementSyntax>().Select(n=>At(n.Condition)).ToArray(),
                method.DescendantNodes().OfType<AssignmentExpressionSyntax>().Select(n=>At(n.Left)+" "+n.OperatorToken.Text).Distinct().ToArray(),
                method.DescendantNodes().OfType<ThrowStatementSyntax>().Select(At).ToArray(),
                method.DescendantNodes().OfType<CatchClauseSyntax>().Select(n=>new{type=n.Declaration?.ToString(),filter=n.Filter?.ToString()}).ToArray(),
                body.Contains("Unknown result type")||body.Contains("Invalid comparison between Unknown")||body.Contains("Expected Obj, but got Unknown"),
                errors.Count(d=>d.Location.SourceSpan.IntersectsWith(method.Span))));
        }
    }
}
var entries=new List<object>();var referenced=new HashSet<string>();var statuses=new Dictionary<string,int>();
foreach(var variant in resources["variants"]!.AsArray())foreach(var entry in variant!["entries"]!.AsArray()){
    var declaredType=entry!["sourceType"]!.GetValue<string>();
    var chain=declaredType=="ExternTitleMenueItemData"?new[]{"ExternTitleMenueItemData","TitleMenueItemData"}:new[]{declaredType};
    var resolvers=new List<object>();
    foreach(var resolver in entry["resolvers"]!.AsObject()){
        var name=resolver.Value!.GetValue<string>();
        var found=chain.SelectMany(type=>members.Where(m=>m.Type==type&&m.Name==name&&m.Public)).ToArray();
        var status=found.Length switch{0=>"missing-public-member",>1=>"ambiguous-public-name",_=>found[0].Parameters==0?"public-zero-argument-declaration":"invocation-argument-gap"};
        statuses[status]=statuses.GetValueOrDefault(status)+1;
        foreach(var member in found)referenced.Add(member.Id);
        resolvers.Add(new{role=resolver.Key,name,status,members=found.Select(m=>m.Id).ToArray(),runtimeTypeObserved=false,fullBehaviorClosed=false});
    }
    entries.Add(new{resource=variant["name"]!.GetValue<string>(),id=entry["id"]!.GetValue<string>(),name=entry["name"]!.GetValue<string>(),sourceType=declaredType,hierarchy=entry["hierarchy"]!.DeepClone(),hidden=entry["hidden"]!.GetValue<bool>(),fields=entry["fields"]!.DeepClone(),resolvers,fullBehaviorClosed=false});
}
var selected=members.Where(m=>referenced.Contains(m.Id)).ToArray();
var result=new{
    schemaVersion=1,generatedUtc=DateTime.UtcNow.ToString("O"),evidenceType="Roslyn syntax parse of existing decompiled text; no vendor assembly loading or execution",
    scope="All resolver fields in four shipped menu XML resources; declared XML types TitleMenueItemData/ExternTitleMenueItemData and known source inheritance only",
    complete=false,sourceFiles,counts=new{declarations=entries.Count,resolverReferences=statuses.Values.Sum(),resolverStatuses=statuses,referencedBodies=selected.Length,bodiesWithDecompilerILWarnings=selected.Count(m=>m.HasDecompilerWarnings),syntaxErrors=sourceFiles.Count==0?0:selected.Sum(m=>m.SyntaxErrors),closedBehaviorChains=0},
    entries,members=selected,
    limitations=new[]{"A public declaration match is not runtime reflection binding or operation verification.","XML type construction/TypeTable and visibility/permissions require separate evidence.","Calls, conditions and assignment targets are syntax facts; symbol resolution, aliasing, field/data-flow and downstream effects remain unproven.","Decompiler IL warnings remain open; no CIL or ABI validation was performed by this parser.","Dynamic child factories can produce additional commands not written in resource XML; these are not a closed entry universe.","Native SDK, external programs, callbacks, passwords and licensing were not executed."}
};
File.WriteAllText(Path.Combine(output,"menu-member-map.json"),JsonSerializer.Serialize(result,new JsonSerializerOptions{WriteIndented=true})+Environment.NewLine);
Console.WriteLine(JsonSerializer.Serialize(result.counts));

record Member(string Id,string Type,string Name,string Path,string Sha256,int Start,int End,bool Public,bool Static,int Parameters,string Signature,string Body,int SpanStart,int SpanLength,string[] Calls,string[] Conditions,string[] Assignments,string[] Throws,object[] Catches,bool HasDecompilerWarnings,int SyntaxErrors);
