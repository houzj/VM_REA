using System.IO;
using System.Globalization;
using System.Text;
using System.Xml;
using FrontendDesignMigration;

CultureInfo.CurrentCulture=CultureInfo.InvariantCulture;
try
{
 var output=Path.GetFullPath(args.FirstOrDefault()??"analysis/full-audit/batch13-20261019/A/migration-checks");Directory.CreateDirectory(output);
 // Explicit synthetic templates exercise the known structure only. They are not vendor resources.
 var design=Path.Combine(output,"synthetic-default.xml");var sub=Path.Combine(output,"synthetic-sub.xml");
 File.WriteAllText(design,"<Design><Header/><Body><Tabs><FrontendTabItem><SubRoot/></FrontendTabItem></Tabs></Body></Design>");File.WriteAllText(sub,"<SubRoot/>");
 var converter=new DesignConverter(design,sub,Encoding.UTF8);int checks=0;
 void Check(bool ok,string label){if(!ok)throw new Exception(label);checks++;Console.WriteLine("PASS "+label);}
 XmlDocument Parse(string xml){var doc=new XmlDocument();doc.LoadXml(xml);return doc;}
 XmlElement Pick(XmlDocument doc,string xpath)=>(XmlElement)(doc.SelectSingleNode(xpath)??throw new Exception(xpath));
 string embedded=Convert.ToBase64String(Encoding.UTF8.GetBytes("<OldRoot><PicBox Size='13,14' Background='{x:Null}'/></OldRoot>"));
 var input=$"<Root Size='100,200' Background='#123456' Title='{{Lang:LangResource Lang_FrontDefaultTitle}}'><GroupBox Size='20,30'><Content><ImageControl Size='3,4' Width='99' Height='98' BorderBrush='{{x:Null}}'/></Content></GroupBox><Tab><TabItem><Content><TableControl Size='5,6'/></Content></TabItem></Tab><SubWindow Document='{embedded}' Size='7,8'/><MultiImageControl/><Wrapper><ImageControl Size='40,50'/></Wrapper></Root>";
 var result=converter.ConvertV40ToV44(input);File.WriteAllText(Path.Combine(output,"actual.xml"),result);var xml=Parse(result);
 Check(xml.DocumentElement!.GetAttribute("VersionInfo")=="V4.4.0","V4.0 conversion writes requested V4.4.0 branch");
 Check(xml.DocumentElement.GetAttribute("Width")=="100"&&xml.DocumentElement.GetAttribute("Height")=="260","Root Size becomes template Width and Height plus 60");
 Check(xml.DocumentElement.GetAttribute("Background")=="#123456","Root background propagates to template");
 var group=Pick(xml,"//GroupBox");Check(group.GetAttribute("AutoStrechA")=="false"&&group.GetAttribute("Width")=="20"&&!group.HasAttribute("Size"),"GroupBox gains stretch flag and size conversion");
 var image=Pick(xml,"//GroupBox/Content/ImageControl");Check(image.GetAttribute("AutoStrechA")=="false"&&image.GetAttribute("Width")=="3"&&image.GetAttribute("Height")=="4","GroupBox first content child traversal overwrites old dimensions");
 Check(image.GetAttribute("BorderBrush")=="#00000000","Null border brush normalizes to transparent");
 var tab=Pick(xml,"//Tab");var table=Pick(xml,"//Tab/TabItem/Content/TableControl");Check(tab.GetAttribute("AutoStrechA")=="false"&&table.GetAttribute("Width")=="5"&&table.GetAttribute("AutoStrechA")=="false","Tab visits each item's first content subtree");
 var window=Pick(xml,"//SubWindow");var subXml=Parse(Encoding.UTF8.GetString(Convert.FromBase64String(window.GetAttribute("Document"))));var pic=Pick(subXml,"/SubRoot/PicBox");
 Check(pic.GetAttribute("Width")=="13"&&pic.GetAttribute("Height")=="14"&&pic.GetAttribute("AutoStrechA")=="false","SubWindow explicit UTF8 base64 is converted through supplied subtemplate");
 Check(pic.GetAttribute("Background")=="#00000000","Embedded null background normalizes transparently");
 Check(window.GetAttribute("Width")=="7"&&window.GetAttribute("Height")=="8","SubWindow own dimensions converted after nested document");
 Check(Pick(xml,"//MultiImageControl").GetAttribute("AutoStrechA")=="false","MultiImageControl receives source stretch flag");
 Check(Pick(xml,"//Wrapper/ImageControl").HasAttribute("Size"),"Unknown wrappers do not invent recursive traversal");
 Check(converter.String2Base64("中文")==Convert.ToBase64String(Encoding.UTF8.GetBytes("中文")),"Base64 uses explicit encoding rather than guessing original DefaultEncode");
 Check(converter.Base64StringToString("%%%")=="","Invalid base64 follows original empty-string catch");
 Check(converter.Bytes2Base64(null!)==""&&converter.String2Base64("")=="","Null bytes and empty string preserve helper behavior");
 bool declarationRejected=false;try{converter.ConvertV40ToV44("<?xml version='1.0'?><Root><PicBox/></Root>");}catch(InvalidDataException){declarationRejected=true;}Check(declarationRejected,"Original FirstChild dispatch does not skip XML declaration");
 bool dtdRejected=false;try{converter.ConvertV40ToV44("<!DOCTYPE Root [<!ENTITY e 'expanded'>]><Root><PicBox/></Root>");}catch(XmlException){dtdRejected=true;}Check(dtdRejected,"Independent hardening prohibits DTD without running XAML");
 bool unsupported=false;try{converter.ConvertV40ToV44("<Design VersionInfo='V4.4.0'/>");}catch(InvalidDataException){unsupported=true;}Check(unsupported,"Unsupported source branch is explicitly rejected");
 Console.WriteLine($"PASS {checks}/{checks}; synthetic XML templates, no XAML instantiation or SDK runtime");return 0;
}
catch(Exception ex){Console.Error.WriteLine(ex);return 1;}
