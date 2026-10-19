// Original managed parser, synthetic owner/parameters; no SDK initialization, UI or devices.
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using VMControls.BaseInterface;
public sealed class B01Params : IParamsConfig {
 public string XmlPath {get;set;} public uint TimeOut {get;set;}
 public readonly Dictionary<string,string> Values=new Dictionary<string,string>();
 string IUserStringData.this[string key] {get{string v;return Values.TryGetValue(key,out v)?v:"";}set{Values[key]=value;}}
 byte[] IUserBytesData.this[string key] {get{return new byte[0];}set{}}
}
public static class TabAssemblyProbe {
 static string output; static Type rootType; static List<object> rows=new List<object>();
 static object Get(object o,string name){var f=o.GetType().GetField(name);return f!=null?f.GetValue(o):o.GetType().GetProperty(name).GetValue(o,null);}
 static object Call(object o,string name,params object[] args){return o.GetType().GetMethods().Single(m=>m.Name==name&&m.GetParameters().Length==args.Length).Invoke(o,args);}
 static object[] Array(object o){return o==null?new object[0]:((IEnumerable)o).Cast<object>().ToArray();}
 static object Item(object o){return new {type=o.GetType().FullName,name=Convert.ToString(Get(o,"Name")),display=Convert.ToString(Get(o,"DisplayNameA")),visible=Convert.ToString(Get(o,"CustomVisible")),id=System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(o)};}
 static void Capture(string phase,object model,bool candidates){var tabs=Array(Get(model,"Tabs"));rows.Add(new {phase,tabs=tabs.Select(t=>new{name=Convert.ToString(Get(t,"Name")),collection=Array(Get(t,"Collection")).Select(i=>i.GetType().Name=="LinkItem"?(object)new{type=i.GetType().FullName,link=Convert.ToString(Get(i,"LinkName"))}:Item(i)).ToArray()}).ToArray(),tree=candidates?Array(Call(model,"GetRunParamsTree")).Select(h=>Item(Get(h,"HolderTarget"))).ToArray():null});}
 static string Hash(string p){using(var s=File.OpenRead(p))using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(s)).Replace("-","").ToLowerInvariant();}
 static object Construct(string xml,bool notify){rootType.GetField("NeedNotifyLoaded").SetValue(null,notify);return Activator.CreateInstance(rootType,new object[]{xml,new object(),new B01Params{XmlPath=xml}});}
 [STAThread] public static int Main(string[] args){string app=args[0];output=args[1];Directory.CreateDirectory(output);int code=0;
 AppDomain.CurrentDomain.AssemblyResolve+=(s,a)=>{string f=new AssemblyName(a.Name).Name+".dll";foreach(string d in new[]{"myLibs","PublicFile/x64","3rdLib/MVVM",".CommonControl"}){string p=Path.Combine(app,d,f);if(File.Exists(p))return Assembly.LoadFrom(p);}return null;};
 try{var parser=Assembly.LoadFrom(Path.Combine(app,"myLibs/Apps.XmlParser.dll"));rootType=parser.GetType("Apps.XmlParser.ParamTab.AlgorithmTabRoot",true);
 var util=Assembly.LoadFrom(Path.Combine(app,"myLibs/VM.Util.dll")).GetType("Hik.Util.ActivatorWrap",true);util.GetMethod("LoadAssembly",new[]{typeof(string),typeof(bool)}).Invoke(null,new object[]{parser.Location,true});
 string original=Path.Combine(app,"Module(sp)/x64/Collection/ImageSourceModule/ImageSourceModuleAlgorithmTab.xml");string before=Hash(original),local=Path.Combine(output,"ImageSource-copy.xml");File.Copy(original,local,false);File.SetAttributes(local,FileAttributes.ReadOnly);string localAttributesBefore=File.GetAttributes(local).ToString();
 var image=Construct(local,false);Capture("image-copy-notify-disabled",image,true);
 rows.Add(new{phase="original-integrity",before,after=Hash(original),same=before==Hash(original),localAfter=Hash(local),localAttributesBefore,attributes=File.GetAttributes(local).ToString()});
 // Minimal XML isolates normal callback, LinkItem replacement and candidate predicates.
 string synthetic=Path.Combine(output,"synthetic.xml");File.WriteAllText(synthetic,"<AlgorithmTabRoot><Tabs><Tab Name=\"Tab_Run Params\"><Categorys><Category Name=\"C\"><Items><Integer Name=\"Hidden\"><DisplayName>%Hidden%</DisplayName><CustomVisible>False</CustomVisible><SubscribeVisible>False</SubscribeVisible></Integer><Integer Name=\"Blank\"><DisplayName></DisplayName></Integer><Integer Name=\"Visible\"><DisplayName>%Visible%</DisplayName></Integer><LinkItem LinkName=\"Visible\" CustomVisible=\"False\" /></Items></Category></Categorys></Tab></Tabs></AlgorithmTabRoot>");
 var unresolved=Construct(synthetic,false);Capture("synthetic-notify-disabled",unresolved,false);
 var resolved=Construct(synthetic,true);Capture("synthetic-notify-enabled",resolved,true);
 var tab=Array(Get(resolved,"Tabs")).Single();var list=Array(Get(tab,"Collection"));var tree=Array(Call(resolved,"GetRunParamsTree")).Select(h=>Get(h,"HolderTarget")).ToArray();
 var checks=new Dictionary<string,bool>{{"single-image-tab",Array(Get(image,"Tabs")).Length==1},{"basic-name-retained",Convert.ToString(Get(Array(Get(image,"Tabs"))[0],"Name"))=="Tab_Basic Params"},{"image-tree-empty",Array(Call(image,"GetRunParamsTree")).Length==0},{"link-count-retained",list.Length==4},{"link-replaced",list.All(x=>x.GetType().Name!="LinkItem")},{"three-candidates",tree.Length==3},{"hidden-in-tree",tree.Any(x=>Convert.ToString(Get(x,"Name"))=="Hidden")},{"blank-excluded",!tree.Any(x=>Convert.ToString(Get(x,"Name"))=="Blank")},{"clone-distinct",!object.ReferenceEquals(list[2],list[3])},{"clone-hidden",!(bool)Get(list[3],"CustomVisible")},{"not-subscribable-in-tree",tree.Any(x=>Convert.ToString(Get(x,"Name"))=="Hidden" && !(bool)Get(x,"SubscribeVisible"))},{"original-unchanged",before==Hash(original)},{"copy-attribute-reset",File.GetAttributes(local)==FileAttributes.Normal}};
 bool ok=checks.Values.All(v=>v);rows.Add(new{phase="assertions",passed=ok,checks,scope="synthetic-owner != real VM Params.Data; image callbacks disabled; synthetic callbacks enabled"});if(!ok)code=2;
 }catch(Exception e){rows.Add(new{phase="fatal",error=e.ToString()});code=1;}
 finally{if(rootType!=null){var types=new[]{rootType,rootType.BaseType,rootType.Assembly.GetType("Apps.XmlParser.ParamTab.LinkItem"),rootType.Assembly.GetType("Apps.XmlParser.ParamTab.Setter")};rows.Add(new{phase="cil",items=types.SelectMany(t=>t.GetMethods(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.DeclaredOnly).Where(m=>new[]{"Init","GetRunParamsTree","AddSubChildToTree","root_XmlLoaded","Operate"}.Contains(m.Name)).Select(m=>new{type=t.FullName,method=m.ToString(),token=m.MetadataToken,cil=BitConverter.ToString(m.GetMethodBody().GetILAsByteArray()).Replace("-","")})).ToArray()});}
 rows.Add(new{phase="assemblies",items=AppDomain.CurrentDomain.GetAssemblies().Where(a=>!a.IsDynamic).Select(a=>new{name=a.FullName,path=a.Location,sha256=Hash(a.Location),mvid=a.ManifestModule.ModuleVersionId.ToString()}).ToArray()});rows.Add(new{phase="native-module-inventory",paths=System.Diagnostics.Process.GetCurrentProcess().Modules.Cast<System.Diagnostics.ProcessModule>().Select(m=>m.FileName).ToArray()});File.WriteAllText(Path.Combine(output,"probe.json"),new JavaScriptSerializer{MaxJsonLength=int.MaxValue}.Serialize(rows));}return code;
 }
}
