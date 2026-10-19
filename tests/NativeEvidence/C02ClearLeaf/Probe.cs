using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Script.Serialization;
class Probe {
 static int hits;
 public static void Hit(){hits++;}
 static BindingFlags F=BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly;
 static string Hash(byte[] b){using(var s=SHA256.Create())return BitConverter.ToString(s.ComputeHash(b)).Replace("-","").ToLowerInvariant();}
 static int Main(string[] a){var log=new List<object>();int code=1;AppDomain.CurrentDomain.AssemblyResolve+=(s,e)=>{foreach(var d in new[]{"myLibs","PublicFile/x64","3rdLib/MVVM"}){var p=Path.Combine(a[0],d,new AssemblyName(e.Name).Name+".dll");if(File.Exists(p))return Assembly.LoadFrom(p);}return null;};try{
 var t=Assembly.LoadFrom(Path.Combine(a[0],"myLibs/VM.Core.dll")).GetType("VM.Core.VmIO",true);
 var fs=new List<FieldInfo>();for(var b=t;b!=null;b=b.BaseType){log.Add(new{phase="preflight",type=b.FullName,cctor=b.TypeInitializer==null?null:b.TypeInitializer.ToString(),constructors=b.GetConstructors(F).Select(c=>new{signature=c.ToString(),il=Convert.ToBase64String(c.GetMethodBody().GetILAsByteArray())}).ToArray()});if(b.TypeInitializer!=null)throw new Exception("Blocked: static constructor "+b.FullName);fs.AddRange(b.GetFields(F));}
 var m=t.GetMethod("SetNoneValueWithoutRaiseEvent",F);string h=Hash(m.GetMethodBody().GetILAsByteArray());if(h!="361e8712c1b44dc1d1c1e18d659f5e09e6ef8ea60eae4ba954a9fb8bf75062cc")throw new Exception("Blocked: original leaf IL mismatch");
 int assertions=0;for(int i=0;i<4;i++){
 object o=FormatterServices.GetUninitializedObject(t);var vf=fs.Single(f=>f.Name=="_value");var lf=fs.Single(f=>f.Name=="_valueList");var ec=fs.Single(f=>f.Name=="_ExecuteCount");var nf=fs.Single(f=>f.Name=="isNull");var rf=fs.Single(f=>f.Name=="RaiseValueChanged");var merge=fs.Single(f=>f.Name=="needMergeIOLoopValue");
 object value=i%2==0?(object)new[]{11,22}:null;var list=new List<object>{"retained",42};vf.SetValue(o,value);lf.SetValue(o,list);ec.SetValue(o,(uint)37);nf.SetValue(o,i>=2);rf.SetValue(o,i<2);merge.SetValue(o,true);
 foreach(var f in fs.Where(f=>typeof(Delegate).IsAssignableFrom(f.FieldType))){var sig=f.FieldType.GetMethod("Invoke");var ps=sig.GetParameters().Select(p=>Expression.Parameter(p.ParameterType,p.Name)).ToArray();f.SetValue(o,Expression.Lambda(f.FieldType,Expression.Call(typeof(Probe).GetMethod("Hit")),ps).Compile());}
 var before=fs.ToDictionary(f=>f,f=>f.GetValue(o));int oldhits=hits;m.Invoke(o,null);var checks=new Dictionary<string,bool>{
 {"value-null",vf.GetValue(o)==null},{"list-identity",Object.ReferenceEquals(list,lf.GetValue(o))},{"list-content",list.Count==2&&(string)list[0]=="retained"&&(int)list[1]==42},{"count",(uint)ec.GetValue(o)==37},{"null-flag",(bool)nf.GetValue(o)==(i>=2)},{"raise-flag",(bool)rf.GetValue(o)==(i<2)},{"events-silent",hits==oldhits},{"all-other-fields-unchanged",fs.Where(f=>f!=vf).All(f=>Object.Equals(before[f],f.GetValue(o)))}};
 assertions+=checks.Count;log.Add(new{phase="case",index=i,beforeValue=value,afterValue=vf.GetValue(o),executeCount=ec.GetValue(o),isNull=nf.GetValue(o),raiseValueChanged=rf.GetValue(o),merge=merge.GetValue(o),fieldsCompared=fs.Count,checks});if(checks.Values.Any(x=>!x))throw new Exception("Assertion failed case "+i);
 }
 log.Add(new{phase="result",passed=true,assertions=assertions,cases=4,methodToken=m.MetadataToken,ilSHA256=h,authority="original leaf on synthetic uninitialized object; not real VM"});code=0;
 }catch(Exception e){log.Add(new{phase="failure",error=e.ToString()});}finally{log.Add(new{phase="assemblies",items=AppDomain.CurrentDomain.GetAssemblies().Where(x=>!x.IsDynamic).Select(x=>new{identity=x.FullName,path=x.Location,sha256=File.Exists(x.Location)?Hash(File.ReadAllBytes(x.Location)):null}).ToArray()});File.WriteAllText(Path.Combine(a[1],"probe.json"),new JavaScriptSerializer{MaxJsonLength=int.MaxValue}.Serialize(log));}return code;}
}
