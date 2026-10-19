using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;

// A map of declarations, not a native importer or an execution plan.
public static class SemanticMap
{
    public static object Build(XElement root, IEnumerable<string> uiEntryPaths)
    {
        if(root.Name!="Root")throw new InvalidDataException("Expected Root.");
        var diagnostics=new List<string>();
        var modules=(root.Element("ModulesInfo")?.Element("ModuleBase")?.Elements("Module")??[]).ToArray();
        var procedures=(root.Element("ProceduresInfo")?.Element("ProcedureBase")?.Elements("Procedure")??[]).ToArray();
        string? A(XElement e,string key)=>(string?)e.Attribute(key);
        string? Id(XElement e)=>A(e,"Index");
        var moduleIds=modules.Select(Id).Where(x=>x is not null).Cast<string>().ToHashSet(StringComparer.Ordinal);
        var procedureIds=procedures.Select(Id).Where(x=>x is not null).Cast<string>().ToHashSet(StringComparer.Ordinal);
        void CheckIds(XElement[] elements,string domain){var ids=elements.Select(Id).ToArray();if(ids.Any(string.IsNullOrWhiteSpace))diagnostics.Add(domain+": missing Index");foreach(var duplicate in ids.Where(x=>x is not null).GroupBy(x=>x,StringComparer.Ordinal).Where(g=>g.Count()>1))diagnostics.Add(domain+": duplicate Index "+duplicate.Key);}
        CheckIds(modules,"module");CheckIds(procedures,"procedure");
        var membership=(root.Element("ProceduresInfo")?.Element("ProcedureInsideModules")?.Elements("Procedure")??[])
            .SelectMany(p=>p.Elements("Inside").Select(m=>new{procedureId=Id(p),moduleId=Id(m),authority="explicit XML membership"})).ToArray();
        foreach(var relation in membership){if(relation.procedureId is null||!procedureIds.Contains(relation.procedureId))diagnostics.Add("membership: unknown procedure "+relation.procedureId);if(relation.moduleId is null||!moduleIds.Contains(relation.moduleId))diagnostics.Add("membership: unknown module "+relation.moduleId);}
        foreach(var occupied in membership.Where(x=>x.moduleId is not null).GroupBy(x=>x.moduleId).Where(g=>g.Select(x=>x.procedureId).Distinct().Count()>1))diagnostics.Add("membership: multiple declared owners for "+occupied.Key);
        var connectModules=(root.Element("ModulesInfo")?.Element("ModuleCommonConnect")?.Elements("Module")??[]).ToArray();
        var following=connectModules.SelectMany(m=>m.Element("FollowingModules")?.Elements("Following").Select(f=>(From:Id(m),To:Id(f)))??[]).ToArray();
        var front=connectModules.SelectMany(m=>m.Element("FrontModules")?.Elements("Front").Select(f=>(From:Id(f),To:Id(m)))??[]).ToArray();
        var edges=following.Concat(front).Distinct().Select(e=>new{from=e.From,to=e.To,followingDeclared=following.Contains(e),frontDeclared=front.Contains(e)}).ToArray();
        foreach(var e in edges){if(e.from is null||e.to is null||!moduleIds.Contains(e.from)||!moduleIds.Contains(e.to))diagnostics.Add("connection: dangling endpoint "+e.from+" -> "+e.to);if(!e.frontDeclared||!e.followingDeclared)diagnostics.Add("connection: asymmetric declarations "+e.from+" -> "+e.to);}
        var subscriptions=(root.Element("ModulesInfo")?.Element("ModuleSubscribe")?.Elements("Subscribe")??[]).Select(s=>new{raw=A(s,"Relation"),tokens=A(s,"Relation")?.Split(" . ",StringSplitOptions.None),status="unresolved-native-relation-semantics"}).ToArray();
        var ui=uiEntryPaths.Select(path=>{
            string normalized=path.Replace('\\','/'),name=normalized[(normalized.LastIndexOf('/')+1)..];
            string[] parts=name.StartsWith('_')?name[1..].Split('+',3):[];
            bool numeric=parts.Length==3&&uint.TryParse(parts[0],NumberStyles.None,CultureInfo.InvariantCulture,out _)&&uint.TryParse(parts[1],NumberStyles.None,CultureInfo.InvariantCulture,out _);
            string? id=numeric?parts[0]:null;
            // Filename coordinates are observed sample syntax. Never merge GUID domains.
            var candidates=id is null?[]:modules.Where(m=>Id(m)==id).Select(m=>"module:"+id).Concat(procedures.Where(p=>Id(p)==id).Select(p=>"procedure:"+id)).ToArray();
            return new{path,observedFileCoordinates=numeric?new{id,type=parts[1],name=parts[2]}:null,candidateDeclarations=candidates,status=!numeric?"unrecognized-filename":candidates.Length==1?"sample-coordinate-candidate":"unresolved-or-ambiguous",authority="sample filename correlation; no native reader proof"};
        }).ToArray();
        return new{
            schema="vm-compat.semantic-map.v1",authority="XML declarations plus source-confirmed key meanings; not native runtime reconstruction",
            modules=modules.Select(m=>new{id=Id(m),moduleName=A(m,"Name"),displayName=A(m,"DisplayName"),declaredType=A(m,"Type"),xmlGuid=A(m,"Guid"),owners=membership.Where(x=>x.moduleId==Id(m)).Select(x=>x.procedureId).ToArray()}).ToArray(),
            procedures=procedures.Select(p=>new{id=Id(p),displayName=A(p,"DisplayName"),xmlGuid=A(p,"Guid"),members=membership.Where(x=>x.procedureId==Id(p)).Select(x=>x.moduleId).ToArray()}).ToArray(),membership,connections=edges,subscriptions,uiEntries=ui,diagnostics,
            identityDomains=new[]{"XML Guid attribute is retained literally.","UI UserData GUID is a separately stored .NET Guid byte-array domain; equality with XML Guid is not established.","UI filename coordinates are candidates only; unmatched solution/global/implicit records are retained."},
            unknowns=new[]{"Native subscription Relation token semantics and writeback.","Implicit/global containers and native ownership transitions.","mdata internal parameter identities, resources and native round trip."}
        };
    }
}
