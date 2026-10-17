using System.Xml.Linq;
using VisionPluginContracts;
int checks=0;
void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
void Bad(string xml,string? groups,string contains){try{ToolConfigurationDefinition.Parse(xml,groups);throw new Exception("No diagnostic");}catch(System.IO.InvalidDataException e){Check(e.Message.Contains(contains),e.Message);}}
var dir=Path.Combine(AppContext.BaseDirectory,"Fixtures");var groups=File.ReadAllText(Path.Combine(dir,"Group.xml"));
foreach(var (name,count) in new[]{("IMVSNPointCalibModu",3),("IMVSNImageCalibModu",3),("TranslationCalibModu",2),("RotateCalibModu",2),("ReadCalibFileModu",1)}){
 var xml=File.ReadAllText(Path.Combine(dir,name+"AlgorithmTab.xml"));var d=ToolConfigurationDefinition.Parse(xml,groups);
 Check(d.Tabs.Count==count,name+" tab count");
 var declared=XDocument.Parse(xml).Root!.Element("Tabs")!.Elements().Select(e=>(string?)e.Attribute("Name"));
 Check(d.Tabs.Select(t=>t.Name).SequenceEqual(declared),name+" ordered tabs");
 var groupDoc=XDocument.Parse(groups);
 var original=XDocument.Parse(xml).Root!.Element("BottomExcuteButtons")!.Elements().SelectMany(e=>e.Name.LocalName=="GroupLinkItem" ? groupDoc.Descendants("GroupInfoItem").First(g=>(string?)g.Attribute("Name")==e.Element("LinkName")!.Value).Element("Items")!.Elements() : new[]{e}).Select(e=>(string?)e.Attribute("Name"));
 Check(d.BottomCommands.Select(t=>t.Name).SequenceEqual(original),name+" bottom order");
 var left=XDocument.Parse(xml).Root!.Element("BottomExcuteButtonsLeft");
 if(left is not null)Check(d.LeftBottomCommands.Select(c=>c.Name).SequenceEqual(left.Elements().Select(e=>(string?)e.Attribute("Name"))),name+" left command order");
 Check(d.SourceSha256.Length==64 && d.GroupsSourceSha256?.Length==64,"review hashes");
 Check(d.Declaration.Children.Any(c=>c.Tag=="Tabs"),"declaration retained");
 foreach(var command in d.BottomCommands){Check(command.Parent?.Tag=="BottomExcuteButtons","parent source");Check(command.Source=="definition" || command.ReferenceChain.Count>0,"reference source");}
 Check(d.ResolveGeometry("English")==new ConfigurationGeometry(680,500),"English defaults");Check(d.ResolveGeometry("Chinese")==new ConfigurationGeometry(420,500),"Chinese defaults");
}
var g="<GroupsRoot><Items><GroupInfoItem Name='G'><Items><Integer Name='X' Custom='yes'><DefaultValue>7</DefaultValue></Integer></Items></GroupInfoItem></Items></GroupsRoot>";
var reuse=ToolConfigurationDefinition.Parse("<AlgorithmTabRoot><Tabs><Tab Name='A'><GroupLinkItem><LinkName>G</LinkName></GroupLinkItem></Tab><Tab Name='B'><GroupLinkItem><LinkName>G</LinkName></GroupLinkItem></Tab></Tabs><ExternParamsList><Unknown Name='Extension' Custom='x'/></ExternParamsList></AlgorithmTabRoot>",g);
Check(!ReferenceEquals(reuse.Tabs[0].Children[0],reuse.Tabs[1].Children[0]),"group cloned");Check(reuse.Tabs[0].Children[0].Parent==reuse.Tabs[0],"first parent");Check(reuse.Tabs[1].Children[0].Parent==reuse.Tabs[1],"second parent");Check(reuse.Tabs[0].Children[0].Attributes["Custom"]=="yes","all attrs");Check(reuse.ExternParams[0].Tag=="Unknown","unknown leaf preserved");
foreach(var (body,lang,w,h) in new[]{("<CustomTabHeight>550</CustomTabHeight><ENCustomTabHeight>900</ENCustomTabHeight>","English",680d,550d),("<ENCustomTabWidth>700</ENCustomTabWidth><ENCustomTabHeight>900</ENCustomTabHeight>","English",700d,900d),("<ZHCustomTabWidth>450</ZHCustomTabWidth><ZHCustomTabHeight>600</ZHCustomTabHeight>","Chinese",450d,600d),("<CustomTabENWidth>710</CustomTabENWidth><CustomTabCHWidth>430</CustomTabCHWidth>","English",710d,500d)})Check(ToolConfigurationDefinition.Parse("<AlgorithmTabRoot>"+body+"</AlgorithmTabRoot>").ResolveGeometry(lang)==new ConfigurationGeometry(w,h),"override rule");
Bad("<AlgorithmTabRoot><GroupLinkItem><LinkName>Missing</LinkName></GroupLinkItem></AlgorithmTabRoot>",null,"Unknown");
Bad("<AlgorithmTabRoot><GroupLinkItem><LinkName>G</LinkName></GroupLinkItem></AlgorithmTabRoot>","<GroupsRoot><GroupInfoItem Name='G'><Items><GroupLinkItem><LinkName>G</LinkName></GroupLinkItem></Items></GroupInfoItem></GroupsRoot>","Cyclic");
Bad("<AlgorithmTabRoot>",null,"Invalid");
Bad("<!DOCTYPE x [<!ENTITY e SYSTEM 'file:///private'>]><AlgorithmTabRoot>&e;</AlgorithmTabRoot>",null,"Invalid");
var nested=ToolConfigurationDefinition.Parse("<AlgorithmTabRoot><Tabs><GroupLinkItem><LinkName>A</LinkName></GroupLinkItem></Tabs></AlgorithmTabRoot>","<GroupsRoot><GroupInfoItem Name='A'><Items><GroupLinkItem><LinkName>B</LinkName></GroupLinkItem></Items></GroupInfoItem><GroupInfoItem Name='B'><Items><Tab Name='Nested'/></Items></GroupInfoItem></GroupsRoot>");
Check(nested.Tabs[0].Name=="Nested" && nested.Tabs[0].ReferenceChain.SequenceEqual(new[]{"A","B"}),"nested reference chain");
Check(nested.Tabs[0].Parent==nested.Root.Children[0],"nested referencing parent");
Console.WriteLine($"VisionConfiguration.ModelChecks: {checks} checks passed");
