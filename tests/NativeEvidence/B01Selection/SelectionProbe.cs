// Runs shipped selection code. Synthetic cached nodes and uninitialized ID-only shell are NOT a real VmModule.
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
public static class SelectionProbe {
 static List<object> rows=new List<object>(); static Dictionary<string,bool> checks=new Dictionary<string,bool>();
 static object Get(object x,string n){var f=x.GetType().GetField(n);return f!=null?f.GetValue(x):x.GetType().GetProperty(n).GetValue(x,null);}
 static void Set(object x,string n,object v){var f=x.GetType().GetField(n);if(f!=null)f.SetValue(x,v);else x.GetType().GetProperty(n).SetValue(x,v,null);}
 static object Call(object x,string n,params object[] a){return x.GetType().GetMethods().Single(m=>m.Name==n && m.GetParameters().Length==a.Length).Invoke(x,a);}
 static object[] Array(object x){return x==null?new object[0]:((IEnumerable)x).Cast<object>().ToArray();}
 static string Hash(string p){using(var s=File.OpenRead(p))using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(s)).Replace("-","").ToLowerInvariant();}
 static void Check(string n,bool v){checks.Add(n,v);}
 [STAThread] public static int Main(string[] args){string app=args[0],output=args[1];int code=0;Type select=null,node=null,vm=null,holder=null;
 AppDomain.CurrentDomain.AssemblyResolve+=(s,a)=>{string f=new AssemblyName(a.Name).Name+".dll";foreach(string d in new[]{"myLibs","PublicFile/x64","3rdLib/MVVM",".CommonControl"}){string p=Path.Combine(app,d,f);if(File.Exists(p))return Assembly.LoadFrom(p);}return null;};
 try {
 var ui=Assembly.LoadFrom(Path.Combine(app,"myLibs/Apps.XmlUI.dll"));select=ui.GetType("Apps.XmlUI.ParamTab.RunParamsSelectData",true);node=ui.GetType("Apps.XmlUI.ParamTab.RunParamsBaseNode",true);
 var parser=Assembly.LoadFrom(Path.Combine(app,"myLibs/Apps.XmlParser.dll"));holder=parser.GetType("Apps.XmlParser.ParamTab.Holder",true);vm=Assembly.LoadFrom(Path.Combine(app,"myLibs/VM.Core.dll")).GetType("VM.Core.VmModule",true);
 object data=Activator.CreateInstance(select);Check("default-limit-32",(int)Get(data,"MaxCheckedCount")==32);Set(data,"IsFromVarBind",true);Check("bind-limit-intmax",(int)Get(data,"MaxCheckedCount")==int.MaxValue);Set(data,"IsFromVarBind",false);
 Check("all-type-null",Call(data,"GetValueType",0)==null);Check("unknown-index-all",Call(data,"GetValueType",99)==null);
 Check("integer-types",((string[])Call(data,"GetValueType",1)).SequenceEqual(new[]{"integer","integerbettween"}));Check("float-types",((string[])Call(data,"GetValueType",2)).SequenceEqual(new[]{"float","floatbettween"}));Check("string-types",((string[])Call(data,"GetValueType",3)).SequenceEqual(new[]{"string","stringcompare","stringex"}));
 // Precondition: exact ID getter must load only moduleInfo then nNodeID. No constructor, Params or SDK access.
 var idGetter=vm.GetProperty("ID").GetGetMethod();var il=idGetter.GetMethodBody().GetILAsByteArray();var info=vm.GetField("moduleInfo",BindingFlags.Instance|BindingFlags.NonPublic);var nodeId=info.FieldType.GetField("nNodeID");
 // Shipped debug-style getter: nop;ldarg.0;ldflda info;ldfld nNodeID;stloc.0;br.s +0;ldloc.0;ret.
 if(!info.FieldType.IsValueType || nodeId.FieldType!=typeof(uint) || il.Length!=17 || il[0]!=0x00 || il[1]!=0x02 || il[2]!=0x7c || il[7]!=0x7b || il[13]!=0x2b || il[14]!=0 || il[15]!=0x06 || il[16]!=0x2a || il[12]!=0x0a || BitConverter.ToInt32(il,3)!=info.MetadataToken || vm.Module.ResolveField(BitConverter.ToInt32(il,8)).Name!="nNodeID" || vm.Module.ResolveField(BitConverter.ToInt32(il,8)).DeclaringType!=info.FieldType)throw new InvalidOperationException("ID getter differs from audited field-only CIL");
 if(vm.GetMethod("Finalize",BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.DeclaredOnly)!=null)throw new InvalidOperationException("Unexpected module finalizer");
 object shell=FormatterServices.GetUninitializedObject(vm);GC.SuppressFinalize(shell);Check("id-only-shell-zero",(uint)Get(shell,"ID")==0);
 object parent=Activator.CreateInstance(holder);Set(parent,"HolderTarget",shell);
 var listType=typeof(List<>).MakeGenericType(node);IList cached=(IList)Activator.CreateInstance(listType);
 foreach(string type in new[]{"Integer","Float","String"}){object n=Activator.CreateInstance(node),target=Activator.CreateInstance(parser.GetType("Apps.XmlParser.ParamTab."+type,true));Set(target,"NameA",type+"Param");Set(n,"TargetObject",target);cached.Add(n);}
 var cache=(IDictionary)Get(data,"AllModuleRunParams");cache.Add(0,cached);cache.Add(99,null);
 var getSub=select.GetMethod("GetSubRunParamsTree");object[] subArgs={parent,Call(data,"GetValueType",1),cache,null,null,"",false,""};getSub.Invoke(data,subArgs);
 var filtered=Array(Get(parent,"EnumEntrysA"));Check("cached-integer-only",filtered.Length==1 && object.ReferenceEquals(filtered[0],cached[0]));Check("cached-reparent",object.ReferenceEquals(Get(cached[0],"ParentTree"),parent));Check("cache-retains-all-types",cached.Count==3);
 object parent2=Activator.CreateInstance(holder);Set(parent2,"HolderTarget",shell);subArgs[0]=parent2;subArgs[1]=null;subArgs[6]=true;getSub.Invoke(data,subArgs);Check("all-type-restores-cached-identities",Array(Get(parent2,"EnumEntrysA")).SequenceEqual(cached.Cast<object>()));Check("all-type-reparents-all",cached.Cast<object>().All(n=>object.ReferenceEquals(Get(n,"ParentTree"),parent2)));
 Check("flatten-skips-null",Array(Call(data,"GetRunParams")).SequenceEqual(cached.Cast<object>()));
 Set(cached[0],"OutputEnabled",true);Set(cached[2],"OutputEnabled",true);IList old=(IList)Activator.CreateInstance(listType);old.Add(Activator.CreateInstance(node));Set(data,"SelectRunParams",old);
 Check("confirm-success",(bool)Call(data,"GetSelectedRunParams"));Check("confirm-clears-old-list-alias",old.Count==0);Check("confirm-new-list",!object.ReferenceEquals(old,Get(data,"SelectRunParams")));Check("confirm-keeps-node-identities",Array(Get(data,"SelectRunParams")).SequenceEqual(new[]{cached[0],cached[2]}));
 // Var-bind success with null TargetObject: intentionally proves only !IsSupportSub bypass, not native params.
 object bind=Activator.CreateInstance(select),plain=Activator.CreateInstance(node);Set(plain,"OutputEnabled",true);IList bindList=(IList)Activator.CreateInstance(listType);bindList.Add(plain);((IDictionary)Get(bind,"AllModuleRunParams")).Add(3,bindList);Set(bind,"IsFromVarBind",true);Set(bind,"SelectedVarType","unmatched");Check("non-subscribable-bind-accepted",(bool)Call(bind,"GetSelectedRunParams"));
 object invalidParent=Activator.CreateInstance(holder);Set(invalidParent,"HolderTarget",new object());subArgs[0]=invalidParent;getSub.Invoke(data,subArgs);Check("non-module-holder-ignored",Array(Get(invalidParent,"EnumEntrysA")).Length==0);
 rows.Add(new{phase="assertions",passed=checks.Values.All(x=>x),checks,scope="Original selector methods; synthetic cache and ID-only uninitialized shell. No real module/SDK/window; initial cache creation and rejection dialogs not executed."});if(!checks.Values.All(x=>x))code=2;GC.KeepAlive(shell);
 }catch(Exception e){rows.Add(new{phase="fatal",error=e.ToString()});code=1;}
 finally {
 var types=new[]{select,node,vm,holder,select==null?null:select.Assembly.GetType("Apps.XmlUI.ParamTab.RunParamsSelectWindow")}.Where(t=>t!=null).Concat(select==null?new Type[0]:select.GetNestedTypes(BindingFlags.Public|BindingFlags.NonPublic)).ToArray();rows.Add(new{phase="cil",methods=types.SelectMany(t=>t.GetMethods(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.Static|BindingFlags.DeclaredOnly).Where(m=>t==select || t==node || t.DeclaringType==select || t.Name=="RunParamsSelectWindow" || (t==vm&&m.Name=="get_ID") || (t==holder&&new[]{"get_HolderTarget","set_HolderTarget","get_EnumEntrysA","set_EnumEntrysA","OnPropertyChangedParentTree"}.Contains(m.Name))).Select(m=>new{type=t.FullName,name=m.ToString(),token=m.MetadataToken,cil=m.GetMethodBody()==null?null:BitConverter.ToString(m.GetMethodBody().GetILAsByteArray()).Replace("-","")})).ToArray()});
 rows.Add(new{phase="assemblies",items=AppDomain.CurrentDomain.GetAssemblies().Where(a=>!a.IsDynamic).Select(a=>new{name=a.FullName,path=a.Location,sha256=Hash(a.Location),mvid=a.ManifestModule.ModuleVersionId}).ToArray()});rows.Add(new{phase="native-modules",paths=System.Diagnostics.Process.GetCurrentProcess().Modules.Cast<System.Diagnostics.ProcessModule>().Select(m=>m.FileName).ToArray()});File.WriteAllText(Path.Combine(output,"probe.json"),new JavaScriptSerializer{MaxJsonLength=int.MaxValue}.Serialize(rows));}return code;
 }
}
