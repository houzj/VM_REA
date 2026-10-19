using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Script.Serialization;
using System.Windows.Data;

// Executes only the installed, unmodified ToolBoxData.UpdateLoad.
// Initial group and flattened collection are seeded by this probe; LoadData/Reload are not executed.
public static class LoaderStateProbe
{
    sealed class ProbeGroup : CollectionViewGroup
    {
        public ProbeGroup(object name) : base(name) { }
        public override bool IsBottomLevel { get { return true; } }
    }
    static Type loaderType, itemType;
    static string libraryRoot;
    static int checks;
    static List<object> events = new List<object>();
    const BindingFlags Any = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    static object Get(object o, string name) { var t=o.GetType(); var f=t.GetField(name,Any); return f!=null?f.GetValue(o):t.GetProperty(name,Any).GetValue(o,null); }
    static void Set(object o,string name,object value) { var t=o.GetType(); var f=t.GetField(name,Any); if(f!=null)f.SetValue(o,value);else t.GetProperty(name,Any).SetValue(o,value,null); }
    static void Check(bool ok, string name) { checks++; if(!ok)throw new Exception("Assertion failed: "+name); }
    static object Item(string name) { var o=Activator.CreateInstance(itemType); Set(o,"name",name);return o; }
    sealed class State { public object Loader, Group, Placeholder; public IList Flat, Items; }
    static State Seed(string path, string name, bool placeholder)
    {
        var s=new State {Loader=Activator.CreateInstance(loaderType),Group=Item(name),Placeholder=Item("probe-placeholder")};
        Set(s.Group,"DirPath",path);s.Items=(IList)Get(s.Group,"items");
        s.Flat=(IList)Activator.CreateInstance(typeof(System.Collections.ObjectModel.ObservableCollection<>).MakeGenericType(itemType));
        if(placeholder){s.Items.Add(s.Placeholder);s.Flat.Add(s.Placeholder);}
        loaderType.GetField("toolSource",Any).SetValue(s.Loader,s.Flat);
        return s;
    }
    static string Invoke(State s, object group)
    {
        try { loaderType.GetMethod("UpdateLoad").Invoke(s.Loader,new object[]{group});return null; }
        catch(TargetInvocationException e){return e.InnerException.GetType().FullName;}
    }
    static string Load(State s) {return Invoke(s,new ProbeGroup(s.Group));}
    static void Snapshot(string operation,State s,string exception)
    {
        events.Add(new {operation,groupIdentity=RuntimeHelpers.GetHashCode(s.Group),init=(bool)Get(s.Group,"IsInit"),
            children=s.Items.Cast<object>().Select(x=>(string)Get(x,"name")).ToArray(),flat=s.Flat.Cast<object>().Select(x=>(string)Get(x,"name")).ToArray(),
            placeholderRetained=s.Flat.Contains(s.Placeholder),exception});
    }
    static string Tool(string root,string dir,string xml)
    {
        string p=Path.Combine(root,dir);Directory.CreateDirectory(p);p=Path.Combine(p,"ToolItemInfo.xml");File.WriteAllText(p,xml);return p;
    }
    static string Xml(string name,int priority){return "<ToolBoxItemData><name>"+name+"</name><priority>"+priority+"</priority></ToolBoxItemData>";}
    [STAThread] public static int Main(string[] args)
    {
        if(args.Length!=3)throw new ArgumentException("libraryRoot fixtureRoot outputJson");
        libraryRoot=Path.GetFullPath(args[0]);string root=Path.GetFullPath(args[1]);Directory.CreateDirectory(root);
        AppDomain.CurrentDomain.AssemblyResolve+=(s,e)=>{
            string n=new AssemblyName(e.Name).Name+".dll";
            string[] dirs={libraryRoot,Path.GetFullPath(Path.Combine(libraryRoot,"../3rdLib/MVVM"))};
            foreach(string d in dirs){string p=Path.Combine(d,n);if(File.Exists(p))return Assembly.LoadFrom(p);}return null;};
        var assembly=Assembly.LoadFrom(Path.Combine(libraryRoot,"VMControls.Data.dll"));
        loaderType=assembly.GetType("VMControls.Data.ToolBoxData",true);itemType=assembly.GetType("VMControls.Data.ToolBoxItemData",true);
        string normal=Path.Combine(root,"normal");Tool(normal,"first",Xml("First",2));Tool(normal,"second",Xml("Second",1));
        var a=Seed(normal,"Normal",true);Snapshot("seed",a,null);string ex=Load(a);Snapshot("first-load",a,ex);
        Check(ex==null,"first load success");Check((bool)Get(a.Group,"IsInit"),"initialized");Check(a.Items.Count==2&&a.Flat.Count==2,"2 children and flattened entries");
        Check(!a.Flat.Contains(a.Placeholder),"placeholder removed");Check((string)Get(a.Items[0],"name")=="Second","ascending priorities");
        Check(Object.ReferenceEquals(Get(a.Items[0],"GroupName"),a.Group),"parent identity");
        Tool(normal,"third",Xml("Third",0));ex=Load(a);Snapshot("second-load-after-new-file",a,ex);Check(ex==null&&a.Items.Count==2,"same initialized group does not scan new file");
        string bad=Path.Combine(root,"bad");Tool(bad,"first",Xml("First",1));string damaged=Tool(bad,"second","<ToolBoxItemData><name>Broken");
        string[] ordered=Directory.GetDirectories(bad);events.Add(new{operation="bad-enumeration-order",directories=ordered});
        // Deliberately corrupt the last enumerated entry to capture the prior partial mutation.
        foreach(string d in ordered)File.WriteAllText(Path.Combine(d,"ToolItemInfo.xml"),Xml(Path.GetFileName(d),1));
        damaged=Path.Combine(ordered.Last(),"ToolItemInfo.xml");File.WriteAllText(damaged,"<ToolBoxItemData><name>Broken");
        var b=Seed(bad,"Bad",true);ex=Load(b);Snapshot("malformed-last-xml",b,ex);
        Check(ex=="System.Xml.XmlException","malformed XML exception");Check((bool)Get(b.Group,"IsInit"),"failed group initialized");
        Check(b.Items.Count==1&&b.Flat.Count==1&&b.Flat.Contains(b.Placeholder),"partial children but original flat placeholder");
        File.WriteAllText(damaged,Xml("Repaired",2));ex=Load(b);Snapshot("repair-same-object",b,ex);Check(ex==null&&b.Items.Count==1&&b.Flat.Contains(b.Placeholder),"repair does not retry same object");
        var fresh=Seed(bad,"Bad",true);ex=Load(fresh);Snapshot("probe-created-new-group-after-repair",fresh,ex);Check(ex==null&&fresh.Items.Count==2&&!fresh.Flat.Contains(fresh.Placeholder),"new seeded group succeeds");
        var missing=Seed(Path.Combine(root,"missing"),"Missing",true);ex=Load(missing);Snapshot("missing-directory",missing,ex);
        Check(ex=="System.IO.DirectoryNotFoundException"&&(bool)Get(missing.Group,"IsInit")&&missing.Items.Count==1,"directory failure precedes clear");
        string emptyPath=Path.Combine(root,"empty-seed");Tool(emptyPath,"present",Xml("Present",1));var empty=Seed(emptyPath,"Empty",false);ex=Load(empty);Snapshot("empty-initial-children",empty,ex);
        Check(ex==null&&(bool)Get(empty.Group,"IsInit")&&empty.Items.Count==0&&empty.Flat.Count==0,"empty seed never populates despite present file");
        var global=Seed(Path.Combine(root,"missing-global"),"Lang_Tools",true);ex=Load(global);Snapshot("global-bypass",global,ex);Check(ex==null&&!(bool)Get(global.Group,"IsInit"),"global bypass before directory and init");
        Check(Invoke(global,null)==null,"null no-op");Check(Invoke(global,new object())=="System.NullReferenceException","non CollectionViewGroup is not accepted");
        File.WriteAllText(args[2],new JavaScriptSerializer{MaxJsonLength=Int32.MaxValue}.Serialize(new{scope="Original UpdateLoad with probe-seeded objects; no LoadData, Reload, UI or SDK",checks,events,
            originalAssembly=assembly.Location,mvid=assembly.ManifestModule.ModuleVersionId,loadedAssemblies=AppDomain.CurrentDomain.GetAssemblies().Select(x=>new{name=x.FullName,path=x.IsDynamic?null:x.Location}).ToArray()}));
        Console.WriteLine("PASS "+checks+" assertions; "+events.Count+" observations");return 0;
    }
}
