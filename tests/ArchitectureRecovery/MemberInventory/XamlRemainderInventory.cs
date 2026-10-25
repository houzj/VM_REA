using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static class XamlRemainderInventory
{
    public static void Export(string root)
    {
        string Rel(string p)=>Path.GetRelativePath(root,p).Replace('\\','/');
        string Artifact(string p)=>p.Split('/')[2];
        string Hash(byte[] b)=>Convert.ToHexString(SHA256.HashData(b)).ToLowerInvariant();
        var output=Path.Combine(root,"docs/architecture-recovery");
        var xml=JsonNode.Parse(File.ReadAllText(Path.Combine(output,"xaml-entry-inventory.json")))!;
        var classified=xml["candidates"]!.AsArray().Where(n=>n!["evidence"]!.GetValue<string>()=="xml-attribute-declaration").Select(n=>n!["id"]!.GetValue<string>()).ToHashSet(StringComparer.Ordinal);
        var elementDeclarations=xml["candidates"]!.AsArray().Where(n=>n!["evidence"]!.GetValue<string>()=="xml-element-declaration").ToArray();
        var methods=new Dictionary<string,List<object>>(StringComparer.Ordinal);var sourceFiles=new List<object>();
        foreach(var full in Directory.EnumerateFiles(Path.Combine(root,"analysis/decompiled"),"*.cs",SearchOption.AllDirectories).Where(f=>!Rel(f).Split('/').Any(s=>s is "bin" or "obj" or ".git")).OrderBy(f=>f,StringComparer.Ordinal)){
            var relative=Rel(full);var bytes=File.ReadAllBytes(full);var text=System.Text.Encoding.UTF8.GetString(bytes);var hash=Hash(bytes);
            var tree=CSharpSyntaxTree.ParseText(text,new CSharpParseOptions(LanguageVersion.Preview),relative);int errors=tree.GetDiagnostics().Count(d=>d.Severity==DiagnosticSeverity.Error);sourceFiles.Add(new{path=relative,sha256=hash,syntaxErrors=errors});
            foreach(var type in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()){
                var name=string.Join('.',type.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().Reverse().Select(n=>n.Name.ToString()).Concat(type.Ancestors().OfType<TypeDeclarationSyntax>().Reverse().Select(t=>t.Identifier.ValueText)).Append(type.Identifier.ValueText));
                foreach(var m in type.Members.OfType<MethodDeclarationSyntax>()){
                    var key=Artifact(relative)+"\n"+name+"\n"+m.Identifier.ValueText;if(!methods.TryGetValue(key,out var found))methods[key]=found=new();var span=m.GetLocation().GetLineSpan();
                    found.Add(new{Id=relative+":"+m.SpanStart,Path=relative,Sha256=hash,Type=name,Name=m.Identifier.ValueText,Start=span.StartLinePosition.Line+1,End=span.EndLinePosition.Line+1,m.SpanStart,m.Span.Length,Body=text.Substring(m.SpanStart,m.Span.Length),Parameters=m.ParameterList.Parameters.Select(p=>p.Type?.ToString()??"").ToArray(),Modifiers=m.Modifiers.ToString(),FileSyntaxErrors=errors});
                }
            }
        }
        var names=new Dictionary<string,int>(StringComparer.Ordinal);var rows=new List<object>();int total=0,identifierCount=0,coveredCount=0;var xamlFiles=new List<object>();
        foreach(var f in xml["files"]!.AsArray()){
            var relative=f!["path"]!.GetValue<string>();var bytes=File.ReadAllBytes(Path.Combine(root,relative));var hash=Hash(bytes);if(hash!=f["sha256"]!.GetValue<string>()||f["status"]!.GetValue<string>()!="parsed")throw new InvalidDataException("Structural XAML snapshot changed/unparsed.");
            using var stream=new MemoryStream(bytes);using var reader=XmlReader.Create(stream,new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null});var doc=XDocument.Load(reader,LoadOptions.SetLineInfo);
            string? rootClass=doc.Root?.Attribute(XName.Get("Class","http://schemas.microsoft.com/winfx/2006/xaml"))?.Value;int ignored=0,matched=0;
            foreach(var element in doc.Descendants())foreach(var a in element.Attributes().Where(a=>!a.IsNamespaceDeclaration)){
                var line=(IXmlLineInfo)a;var id=$"xaml:{relative}:{line.LineNumber}:{line.LinePosition}";if(classified.Contains(id))continue;
                total++;ignored++;var name=a.Name.ToString();names[name]=names.GetValueOrDefault(name)+1;if(!SyntaxFacts.IsValidIdentifier(a.Value))continue;identifierCount++;
                var key=Artifact(relative)+"\n"+rootClass+"\n"+a.Value;if(rootClass is null||!methods.TryGetValue(key,out var found))continue;
                var elementLine=(IXmlLineInfo)element;var covered=elementDeclarations.Where(n=>n!["path"]!.GetValue<string>()==relative&&n["line"]!.GetValue<int>()==elementLine.LineNumber&&n["column"]!.GetValue<int>()==elementLine.LinePosition&&n["kind"]!.GetValue<string>()=="event-setter").Select(n=>n!["id"]!.GetValue<string>()).ToArray();if(covered.Length>0)coveredCount++;
                matched++;rows.Add(new{id="xaml-remainder:"+id[5..],path=relative,sha256=hash,line=line.LineNumber,column=line.LinePosition,rootClass,element=element.Name.ToString(),elementAttributes=element.Attributes().Where(a=>!a.IsNamespaceDeclaration).ToDictionary(a=>a.Name.ToString(),a=>a.Value),target=a.Name.LocalName,attributeNamespace=a.Name.NamespaceName,value=a.Value,kind="handler-valued-attribute-candidate",status="same-tree-root-method-name-located",coveredElementDeclarations=covered,methods=found,bindingConfirmed=false,fullBehaviorClosed=false});
            }
            xamlFiles.Add(new{path=relative,sha256=hash,unclassifiedAttributes=ignored,matchedHandlerValuedCandidates=matched});
        }
        var result=new{schemaVersion=1,complete=false,authority="Secure XML remainder plus same-tree exact root-class Roslyn method location; no XAML object construction or vendor execution",counts=new{xamlFiles=xamlFiles.Count,sourceFiles=sourceFiles.Count,unclassifiedAttributes=total,identifierValuedRemainder=identifierCount,handlerValuedCandidates=rows.Count,alreadyCoveredByEventSetterElements=coveredCount,supplementalCandidates=rows.Count-coveredCount,confirmedBindings=0,closedChains=0},unclassifiedAttributeNames=names.OrderByDescending(p=>p.Value).Select(p=>new{name=p.Key,count=p.Value}),files=xamlFiles,sourceFiles,entries=rows,limitations=new[]{"An identifier attribute matching a root-class method is a candidate, not proof of an event or delegate property assignment.","Direct class declarations only: no inheritance, aliases/import fallback, cross-artifact lookup, original BAML or semantic delegate conversion.","Unmatched and non-identifier remainder attributes remain counted by name; they are not proved non-entries.","All rows remain unconfirmed; custom delegate dependency properties must not be relabeled as CLR events."}};
        File.WriteAllText(Path.Combine(output,"xaml-remainder-map.json"),JsonSerializer.Serialize(result,new JsonSerializerOptions{WriteIndented=true})+Environment.NewLine);Console.WriteLine(JsonSerializer.Serialize(result.counts));
    }
}
