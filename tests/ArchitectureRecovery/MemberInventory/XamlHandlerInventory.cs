using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static class XamlHandlerInventory
{
    public static void Export(string root)
    {
        var output=Path.Combine(root,"docs/architecture-recovery");
        var xml=JsonNode.Parse(File.ReadAllText(Path.Combine(output,"xaml-entry-inventory.json")))!;
        var entries=xml["candidates"]!.AsArray().Where(c=>c!["kind"]!.GetValue<string>() is "input-event" or "ui-lifecycle-event" or "event-setter").Select(c=>c!).ToArray();
        string Tree(string relative)=>string.Join('/',relative.Split('/').Take(3));
        string ClassKey(string tree,string fullName)=>tree+"|"+fullName;
        var metadataBridge=JsonNode.Parse(File.ReadAllText(Path.Combine(output,"image-list-base-metadata.json")))!;
        foreach(var artifactKey in new[]{"child","parent"}){var artifact=metadataBridge[artifactKey]!;if(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(artifact["path"]!.GetValue<string>()))).ToLowerInvariant()!=artifact["sha256"]!.GetValue<string>())throw new InvalidDataException("Cross-artifact original identity changed.");}
        foreach(var file in metadataBridge["sourceFiles"]!.AsArray())if(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Path.Combine(root,file!["path"]!.GetValue<string>())))).ToLowerInvariant()!=file["sha256"]!.GetValue<string>())throw new InvalidDataException("Cross-artifact decompiled source changed.");
        var explicitBase=metadataBridge["link"]!;var explicitDerivedKey=ClassKey(explicitBase["derivedTree"]!.GetValue<string>(),explicitBase["derivedType"]!.GetValue<string>());var explicitBaseKey=ClassKey(explicitBase["baseTree"]!.GetValue<string>(),explicitBase["baseType"]!.GetValue<string>());
        var connectorPath=Path.Combine(output,"xaml-connector-review.json");var connectorReview=File.Exists(connectorPath)?JsonNode.Parse(File.ReadAllText(connectorPath)):null;
        if(connectorReview is not null)foreach(var key in new[]{"overload","privateClosing"}){var artifact=connectorReview[key]!["artifact"]!;if(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(artifact["path"]!.GetValue<string>()))).ToLowerInvariant()!=artifact["sha256"]!.GetValue<string>())throw new InvalidDataException("Connector original identity changed.");}
        var handlerNames=entries.Select(e=>e["value"]!.GetValue<string>()).ToHashSet(StringComparer.Ordinal);
        var methods=new Dictionary<string,List<HandlerMethod>>(StringComparer.Ordinal);
        var classes=new Dictionary<string,List<ClassPart>>(StringComparer.Ordinal);
        var files=new List<object>();
        foreach(var full in Directory.EnumerateFiles(Path.Combine(root,"analysis/decompiled"),"*.cs",SearchOption.AllDirectories).Where(f=>!f.Replace('\\','/').Split('/').Any(p=>p is "obj" or "bin")).OrderBy(f=>f,StringComparer.Ordinal))
        {
            var relative=Path.GetRelativePath(root,full).Replace('\\','/');var bytes=File.ReadAllBytes(full);var hash=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
            var tree=CSharpSyntaxTree.ParseText(System.Text.Encoding.UTF8.GetString(bytes),new CSharpParseOptions(LanguageVersion.Preview),relative);
            var errors=tree.GetDiagnostics().Count(d=>d.Severity==DiagnosticSeverity.Error);files.Add(new{path=relative,sha256=hash,syntaxErrors=errors});
            foreach(var type in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                var parts=type.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().Reverse().Select(n=>n.Name.ToString()).Concat(type.Ancestors().OfType<TypeDeclarationSyntax>().Reverse().Select(t=>t.Identifier.ValueText)).Append(type.Identifier.ValueText);
                var fullName=string.Join('.',parts);var key=ClassKey(Tree(relative),fullName);
                if(!classes.TryGetValue(key,out var declarations))classes[key]=declarations=new();
                declarations.Add(new ClassPart(relative,hash,fullName,fullName.Contains('.')?fullName[..fullName.LastIndexOf('.')]:"",type.BaseList?.Types.Select(t=>t.Type.ToString()).ToArray()??[],tree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>().Where(u=>u.Alias is not null).Select(u=>u.Alias!.Name.Identifier.ValueText).ToArray(),errors));
                if(!methods.TryGetValue(key,out var candidates))methods[key]=candidates=new();
                foreach(var method in type.Members.OfType<MethodDeclarationSyntax>().Where(m=>handlerNames.Contains(m.Identifier.ValueText)))
                {
                    var span=method.GetLocation().GetLineSpan();var body=method.ToString();
                    string At(SyntaxNode node)=>$"{node.GetLocation().GetLineSpan().StartLinePosition.Line+1}: {node}";
                    candidates.Add(new HandlerMethod($"{relative}:{method.Span.Start}",fullName,method.Identifier.ValueText,relative,hash,span.StartLinePosition.Line+1,span.EndLinePosition.Line+1,method.Span.Start,method.Span.Length,body,method.ReturnType+" "+method.Identifier+method.ParameterList,method.ParameterList.Parameters.Select(p=>p.Type?.ToString()??"?").ToArray(),method.Modifiers.Select(m=>m.Text).ToArray(),method.Body is not null||method.ExpressionBody is not null,method.Body is {Statements.Count:0},errors,Regex.IsMatch(body,@"//\s*IL_.*(?:Unknown|Invalid|Expected)"),method.DescendantNodes().OfType<InvocationExpressionSyntax>().Select(n=>At(n.Expression)).Distinct().ToArray()));
                }
            }
        }
        var selected=new Dictionary<string,HandlerMethod>(StringComparer.Ordinal);var rows=new List<object>();var counts=new Dictionary<string,int>(StringComparer.Ordinal);
        foreach(var entry in entries)
        {
            var sourcePath=entry["path"]!.GetValue<string>();var className=entry["rootClass"]?.GetValue<string>();var handler=entry["value"]!.GetValue<string>();var key=className is null?null:ClassKey(Tree(sourcePath),className);
            var literal=Regex.IsMatch(handler,@"^[A-Za-z_][A-Za-z0-9_]*$");var found=key is not null&&methods.TryGetValue(key,out var list)&&literal?list.Where(m=>m.Name==handler).ToArray():[];
            var lookupTrace=new List<object>();int inheritedDepth=0;var visited=new HashSet<string>(StringComparer.Ordinal);var lookupKey=key;string? crossArtifactEvidence=null;
            while(literal&&lookupKey is not null&&classes.TryGetValue(lookupKey,out var currentParts)&&visited.Add(lookupKey)){
                lookupTrace.Add(new{type=currentParts[0].type,parts=currentParts,crossArtifactEvidence});
                if(found.Length>0)break;
                var baseKeys=new HashSet<string>(StringComparer.Ordinal);
                foreach(var part in currentParts)foreach(var baseText in part.bases){
                    if(baseText.Contains('<')||part.aliases.Contains(baseText.Split('.')[0]))continue;
                    string candidate=baseText.StartsWith("global::",StringComparison.Ordinal)?baseText[8..]:baseText.Contains('.')?baseText:(part.@namespace.Length==0?baseText:part.@namespace+"."+baseText);
                    var candidateKey=ClassKey(Tree(part.path),candidate);if(classes.ContainsKey(candidateKey))baseKeys.Add(candidateKey);
                }
                crossArtifactEvidence=null;
                if(baseKeys.Count==0&&lookupKey==explicitDerivedKey&&classes.ContainsKey(explicitBaseKey)&&currentParts.Any(p=>p.bases.Contains(explicitBase["baseType"]!.GetValue<string>().Split('.').Last())||p.bases.Contains(explicitBase["baseType"]!.GetValue<string>()))){baseKeys.Add(explicitBaseKey);crossArtifactEvidence="image-list-base-metadata.json";}
                if(baseKeys.Count!=1)break;lookupKey=baseKeys.Single();inheritedDepth++;
                found=methods.TryGetValue(lookupKey,out var inherited)?inherited.Where(m=>m.Name==handler).ToArray():[];
            }
            var methodCandidates=found.ToArray();JsonNode? connectorEvidence=null;bool originalOverload=false;
            if(found.Length>1&&connectorReview?["overload"] is {} overload&&overload["type"]!.GetValue<string>()==className&&overload["handler"]!.GetValue<string>()==handler){var original=found.Where(m=>m.Id==overload["selectedSourceMethodId"]!.GetValue<string>()&&m.Sha256==overload["selectedSourceSha256"]!.GetValue<string>()&&m.ParameterTypes.Length==overload["selectedParameterCount"]!.GetValue<int>()).ToArray();if(original.Length!=1)throw new InvalidDataException("Connector/source overload mapping stale.");found=original;connectorEvidence=overload.DeepClone();originalOverload=true;}
            if(connectorReview?["privateClosing"] is {} privateClosing&&privateClosing["derivedType"]!.GetValue<string>()==className&&handler=="BaseWindow_Closing")connectorEvidence=privateClosing.DeepClone();
            var status=className is null?"no-root-xclass":!literal?"nonliteral-handler-value":!classes.ContainsKey(key!)?"declaring-type-not-found":found.Length==0?"method-not-located-in-local-base-chain":found.Length>1?"multiple-declarations":!found[0].HasBody?"declaration-without-body":found[0].FileSyntaxErrors>0?"body-located-file-syntax-errors":originalOverload?"original-connector-overload-located":inheritedDepth>0?(found[0].Modifiers.Contains("private")&&!found[0].Modifiers.Contains("protected")?"inherited-private-body-location-only":"inherited-body-located"):"body-located";
            counts[status]=counts.GetValueOrDefault(status)+1;foreach(var method in methodCandidates)selected[method.Id]=method;
            rows.Add(new{declarationId=entry["id"]!.GetValue<string>(),path=sourcePath,sha256=entry["sha256"]!.GetValue<string>(),line=entry["line"]!.GetValue<int>(),kind=entry["kind"]!.GetValue<string>(),@event=entry["target"]!.GetValue<string>(),rootClass=className,handler,status,methods=found.Select(m=>m.Id).ToArray(),methodCandidates=methodCandidates.Select(m=>m.Id).ToArray(),connectorEvidence,declaringTypeParts=key is not null&&classes.TryGetValue(key,out var declared)?declared:[],lookupTrace,inheritedDepth,bindingConfirmed=false,fullBehaviorClosed=false});
        }
        var result=new{schemaVersion=4,generatedUtc=DateTime.UtcNow.ToString("O"),complete=false,authority="Roslyn syntax location in local decompiled text; no vendor assembly loading or execution",scope="All 3 event candidate kinds from current structural XAML inventory. Exact x:Class/name, uniquely declared same-tree bases, plus the single metadata-verified ImageListPanel external base; no general using/import or cross-artifact fallback.",crossArtifactBaseEvidence="image-list-base-metadata.json",counts=new{eventDeclarations=rows.Count,sourceFiles=files.Count,statuses=counts,locatedMethodBodies=selected.Values.Count(m=>m.HasBody),methodsWithDecompilerWarnings=selected.Values.Count(m=>m.HasDecompilerWarnings),confirmedBindings=0,closedBehaviorChains=0},files,entries=rows,methods=selected.Values.OrderBy(m=>m.Id,StringComparer.Ordinal).ToArray(),limitations=new[]{"Root class/base member location is not semantic delegate binding, generated IStyleConnector proof or runtime activation.","No same-name fallback across artifact trees; only the explicit original-metadata-verified ImageListPanel base link is admitted.","Generic or alias bases, multiple class candidates, private base accessibility, overload/delegate compatibility and external dictionaries remain unproven.","File syntax errors and decompiler IL warnings stay visible; no CIL/ABI inference is made.","The 3 event kinds are a bounded candidate scope, not all WPF/native/dynamic entry registrations."}};
        File.WriteAllText(Path.Combine(output,"xaml-handler-map.json"),JsonSerializer.Serialize(result,new JsonSerializerOptions{WriteIndented=true})+Environment.NewLine);Console.WriteLine(JsonSerializer.Serialize(result.counts));
    }
    sealed record HandlerMethod(string Id,string Type,string Name,string Path,string Sha256,int Start,int End,int SpanStart,int SpanLength,string Body,string Signature,string[] ParameterTypes,string[] Modifiers,bool HasBody,bool EmptyBlock,int FileSyntaxErrors,bool HasDecompilerWarnings,string[] Calls);
    sealed record ClassPart(string path,string sha256,string type,string @namespace,string[] bases,string[] aliases,int syntaxErrors);
}
