using System.Text.Json;
using VisionPluginContracts;
try {
int checks=0;void Check(bool ok,string name){checks++;if(!ok)throw new InvalidOperationException(name);}
using var document=JsonDocument.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(),"docs/plugins/configuration-template-catalog-evidence.json")));
var evidence=document.RootElement;
Check(evidence.GetProperty("scope").GetProperty("primaryToolCount").GetInt32()==8,"Eight primary tools");
Check(evidence.GetProperty("scope").GetProperty("supplementalToolCount").GetInt32()==1,"Supplemental highest-priority NImage");
int instances=0;
foreach(var module in evidence.GetProperty("coverage").EnumerateArray())foreach(var node in module.GetProperty("controls").EnumerateArray()){
 var expected=node.GetProperty("resolution");var actual=ConfigurationTemplateCatalog.Resolve(node.GetProperty("nodeType").GetString()!);
 Check(actual.Support.ToString()==expected.GetProperty("support").GetString(),"Expanded XML template support");
 Check(actual.TemplateKey==expected.GetProperty("templateKey").GetString(),"Expanded XML original template key");
 Check(!actual.IndependentRendererVerified,"Pure metadata is not renderer parity");instances++;
}
Check(instances==267,"All expanded module/linked-group instances accounted for");
foreach(var type in evidence.GetProperty("typeRules").EnumerateObject()){
 var expected=type.Value;var actual=ConfigurationTemplateCatalog.Resolve(type.Name);string? key=expected.GetProperty("defaultTemplateKey").ValueKind==JsonValueKind.Null?null:expected.GetProperty("defaultTemplateKey").GetString();
 Check(actual.EffectiveDisplayType==key,"Default/derived source property rule "+type.Name);
 if(expected.GetProperty("kind").GetString()=="runtime")Check(actual.Support==ConfigurationTemplateSupport.RequiresRuntimeDisplayType,"Runtime-derived type must not be guessed");
}
foreach(string key in ConfigurationTemplateCatalog.Templates.Keys){var actual=ConfigurationTemplateCatalog.Resolve("unknown-node",key);Check(actual.IsRecognized&&actual.TemplateKey==key,"Explicit evaluated/ExternDisplayType original key");Check(actual.ResourceUris.Count>0,"Original resource provider carried to host");}
foreach(string name in new[]{"UnknownToolField","Number","NumberArray","GrayImage","System.Double","integer"}){var actual=ConfigurationTemplateCatalog.Resolve(name);Check(actual.Support==ConfigurationTemplateSupport.Unsupported&&actual.TemplateKey is null,"Unknown/data-kind names never select generic TextBox");Check(actual.NativeFallbackKey=="Default","Original fallback preserved as evidence only");}
Check(ConfigurationTemplateCatalog.Resolve("SaveFileDialog").TemplateKey=="Command","Source-confirmed SaveFileDialog alias");
Check(ConfigurationTemplateCatalog.Resolve("OpenFolderDialog").TemplateKey=="OpenFileDialog","Source-confirmed folder alias");
Check(ConfigurationTemplateCatalog.Resolve("EnumerationLightForLight").TemplateKey=="EnumerationLight","Source-confirmed light alias");
Check(ConfigurationTemplateCatalog.Resolve("BooleanTips").ResourceUris.Any(p=>p.Contains("BooleanTipsDataTemplate.xaml",StringComparison.OrdinalIgnoreCase)),"Relative merged extension dictionary traced");
Check(ConfigurationTemplateCatalog.Resolve("Integer","").Support==ConfigurationTemplateSupport.Unsupported,"Empty explicit override does not use node default");
Check(ConfigurationTemplateCatalog.Resolve("Integer","integer").Support==ConfigurationTemplateSupport.Unsupported,"Original key lookup is case-sensitive");
var nonTemplate=ConfigurationTemplateCatalog.Resolve("Integer","TextLeft");Check(nonTemplate.Support==ConfigurationTemplateSupport.Unsupported&&nonTemplate.TemplateKey is null&&nonTemplate.NativeFallbackKey is null,"Existing non-DataTemplate resource casts to null, not native Default");
Check(ConfigurationTemplateCatalog.Resolve("unknown-node","Integer").TemplateKey=="Integer","Explicit extern display override is independent of node value type");
Check(ConfigurationTemplateCatalog.NativeBindingProperty=="DisplayType","Installed original binding property name");
Directory.CreateDirectory("analysis/full-audit/configuration-templates");File.WriteAllText("analysis/full-audit/configuration-templates/checks.json",JsonSerializer.Serialize(new{checks,expandedXmlInstances=instances,templates=ConfigurationTemplateCatalog.Templates.Count,unexpectedFailures=0,scope="pure metadata; no vendor execution, renderer, windows or devices"},new JsonSerializerOptions{WriteIndented=true}));
Console.WriteLine($"Configuration template metadata: {checks} checks passed; {instances} XML/group instances, {ConfigurationTemplateCatalog.Templates.Count} original DataTemplates. No rendering or native parity claim.");return 0;
}catch(Exception exception){Console.Error.WriteLine(exception);return 1;}
