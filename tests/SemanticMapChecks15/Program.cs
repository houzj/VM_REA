using System.Text.Json;
using System.Xml.Linq;
int count=0;void Check(bool value,string message){if(!value)throw new Exception(message);count++;}
var root=XElement.Parse("""
<Root><ModulesInfo><ModuleBase><Module Index="0" Name="Source" Guid="XML-A"/><Module Index="1" Name="Target"/></ModuleBase><ModuleCommonConnect><Module Index="0"><FollowingModules><Following Index="1"/></FollowingModules></Module><Module Index="1"><FrontModules><Front Index="0"/></FrontModules></Module></ModuleCommonConnect><ModuleSubscribe><Subscribe Relation="1 . Input . 0 . Output . 0 . 0 . All . 1"/></ModuleSubscribe></ModulesInfo><ProceduresInfo><ProcedureBase><Procedure Index="10000" Guid="XML-P"/></ProcedureBase><ProcedureInsideModules><Procedure Index="10000"><Inside Index="0"/><Inside Index="1"/></Procedure></ProcedureInsideModules></ProceduresInfo></Root>
""");
JsonElement Map(XElement xml)=>JsonSerializer.SerializeToElement(SemanticMap.Build(xml,new[]{"SolutionFile\\UiParamData\\_0+0+Source","SolutionFile/UiParamData/_70000+100+solution","bad"}));
var map=Map(root);
Check(map.GetProperty("modules")[0].GetProperty("xmlGuid").GetString()=="XML-A","XML GUID preserved");
Check(map.GetProperty("modules")[0].GetProperty("owners")[0].GetString()=="10000","explicit owner");
Check(map.GetProperty("procedures")[0].GetProperty("members").GetArrayLength()==2,"membership exact");
Check(map.GetProperty("connections").GetArrayLength()==1,"front/following merge");
Check(map.GetProperty("connections")[0].GetProperty("frontDeclared").GetBoolean(),"front provenance");
Check(map.GetProperty("connections")[0].GetProperty("followingDeclared").GetBoolean(),"following provenance");
Check(map.GetProperty("subscriptions")[0].GetProperty("tokens").GetArrayLength()==8,"tokens retained");
Check(map.GetProperty("subscriptions")[0].GetProperty("status").GetString()=="unresolved-native-relation-semantics","unknown not inferred");
Check(map.GetProperty("uiEntries")[0].GetProperty("candidateDeclarations")[0].GetString()=="module:0","sample correlation");
Check(map.GetProperty("uiEntries")[1].GetProperty("status").GetString()=="unresolved-or-ambiguous","implicit record retained");
Check(map.GetProperty("uiEntries")[2].GetProperty("status").GetString()=="unrecognized-filename","unknown filename retained");
root.Element("ModulesInfo")!.Element("ModuleBase")!.Add(new XElement("Module",new XAttribute("Index","0")));
Check(Map(root).GetProperty("diagnostics").EnumerateArray().Any(x=>x.GetString()!.Contains("duplicate Index")),"duplicate identity diagnosed");
root.Element("ModulesInfo")!.Element("ModuleCommonConnect")!.Elements("Module").Last().Remove();
Check(Map(root).GetProperty("diagnostics").EnumerateArray().Any(x=>x.GetString()!.Contains("asymmetric")),"asymmetric declared edge diagnosed");
root.Element("ProceduresInfo")!.Element("ProcedureInsideModules")!.Add(new XElement("Procedure",new XAttribute("Index","404"),new XElement("Inside",new XAttribute("Index","999"))));
Check(Map(root).GetProperty("diagnostics").EnumerateArray().Count(x=>x.GetString()!.Contains("membership: unknown"))==2,"dangling membership diagnosed");
Check(map.GetProperty("identityDomains").GetArrayLength()==3,"identity domains separate");
Console.WriteLine($"PASS {count} semantic mapping checks; declaration inspector only.");
