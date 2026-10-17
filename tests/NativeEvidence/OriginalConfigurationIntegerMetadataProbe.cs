// Original managed ParseInt only; no native algorithm, device or desktop window.
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;
public static class OriginalConfigurationIntegerMetadataProbe {
 public static int Main(string[] args) {
  string libraries=args[0];AppDomain.CurrentDomain.AssemblyResolve+=(s,a)=>{string p=Path.Combine(libraries,new AssemblyName(a.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
  var util=Assembly.LoadFrom(Path.Combine(libraries,"VM.Util.dll"));var method=util.GetType("Hik.Util.StringUtil",true).GetMethod("ParseInt",new[]{typeof(string)});
  string[] inputs={"0xffffff9c","0xfffe7960","0x186a0","0x1","0x7fffffff","0x80000000","0xffffffff","0x100000000","2147483648","-2147483649","0XFFFFFFFF","0x00000000ffffffff"};
  var rows=inputs.Select(x=>new {input=x,value=(int)method.Invoke(null,new object[]{x})}).ToArray();
  var parser=Assembly.LoadFrom(Path.Combine(libraries,"Apps.XmlParser.dll"));var integer=parser.GetType("Apps.XmlParser.ParamTab.Integer",true);
  var fields=new[]{"CurValue","DefaultValue","MinValue","MaxValue","IncValue"}.Select(x=>new{name=x,type=integer.GetField(x).FieldType.FullName}).ToArray();
  File.WriteAllText(args[1],new JavaScriptSerializer().Serialize(new{authority="original-managed-ParseInt/reflection-only",rows,fields,device=false,desktop=false}));return 0;
 }
}
