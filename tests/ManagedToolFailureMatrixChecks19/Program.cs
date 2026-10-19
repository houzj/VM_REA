using System.Collections;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using Application=System.Windows.Application;
using Button=System.Windows.Controls.Button;

static class Program
{
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 [STAThread]static int Main(string[] args)
 {
  AppContext.SetSwitch("VisionEditor.DisableAutomaticCameraDiscovery",true);
  string root=Path.Combine(Path.GetFullPath(args.Single()),"failure-matrix-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);int checks=0;var failures=new List<object>();
  try
  {
   var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};var window=new MainWindow();
   object? Field(string name)=>typeof(MainWindow).GetField(name,Private)!.GetValue(window);
   void Set(string name,object value)=>typeof(MainWindow).GetField(name,Private)!.SetValue(window,value);
   object? Call(string name,params object[] values)=>typeof(MainWindow).GetMethod(name,Private)!.Invoke(window,values);
   void Check(bool ok,string label){if(!ok)throw new Exception(label);checks++;Console.WriteLine("PASS "+label);}
   var modules=(ModuleRegistry)Field("modules")!;var plugins=(ToolPluginRegistry)Field("plugins")!;var doc=(GraphEditorDocument)Field("doc")!;
   var session=(ManagedToolCatalogLoadSession)Field("managedToolLoads")!;
   string assemblyPath=Path.Combine(AppContext.BaseDirectory,"fixture","ManagedToolFailureFixture19.dll");string hash=Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(assemblyPath)));
   ManagedToolPluginReference Ref(string type,string id,string version="1.0.0")=>new(assemblyPath,"ManagedToolFailureFixture19."+type,hash,id,version);
   var good=Ref("PrefixPlugin","fixture.prefix19");string catalog=Path.Combine(root,"matrix-catalog.json");
   var cases=new Dictionary<string,ManagedToolPluginReference>{
    ["MissingFile"]=good with{AssemblyPath=assemblyPath+".missing"},["MissingType"]=good with{TypeFullName="No.Such.Type"},["WrongHash"]=good with{Sha256=new string('0',64)},["WrongVersion"]=good with{Version="99.0.0"},
    ["Constructor"]=Ref("ConstructorFailurePlugin","fixture.constructor19"),["Dependency"]=Ref("DependencyPlugin","fixture.dependency19"),["PrivateConstructor"]=Ref("NoPublicConstructorPlugin","fixture.private19"),["NotContract"]=Ref("NotAPlugin","fixture.not-contract19"),
    ["Duplicate"]=Ref("AliasPlugin","fixture.prefix19"),["VersionConflict"]=Ref("VersionTwoPlugin","fixture.prefix19","2.0.0"),["BuiltinConflict"]=Ref("BuiltinConflictPlugin","vision.threshold")};
   object Tool(string id,ManagedToolPluginReference reference)=>new{Id=id,Name=id,Priority=1d,DisplayType="IMAGE",Icon=(string?)null,ModuleId=reference.ModuleId,Implementation=reference};
   void Write(bool repair)
   {
    var groups=new List<object>{new{Id="Success",Name="Success",Priority=0d,Icon=(string?)null,HoverIcon=(string?)null,Items=new[]{Tool("success",good)}}};
    foreach(var pair in cases)groups.Add(new{Id=pair.Key,Name=pair.Key,Priority=1d,Icon=(string?)null,HoverIcon=(string?)null,Items=new[]{Tool(pair.Key,pair.Value)}});
    groups.Add(new{Id="PrefixConflict",Name="PrefixConflict",Priority=2d,Icon=(string?)null,HoverIcon=(string?)null,Items=new[]{Tool("prefix",good),Tool("conflict",cases["Duplicate"])}});
    groups.Add(new{Id="Repair",Name="Repair",Priority=3d,Icon=(string?)null,HoverIcon=(string?)null,Items=new[]{Tool("repair",repair?good:good with{AssemblyPath=assemblyPath+".repair-missing"})}});
    File.WriteAllText(catalog,JsonSerializer.Serialize(new{Schema=1,Groups=groups}));
   }
   Expander Category(string id)=>((StackPanel)window.FindName("Rail")).Children.OfType<Expander>().Single(e=>Equals(e.Header,id));
   object CategoryState(string id)=>((IDictionary)Field("toolCategoryLoads")!)[id]!;
   int Attempts(string id)=>(int)CategoryState(id).GetType().GetProperty("Attempts")!.GetValue(CategoryState(id))!;
   string RegistryState()=>JsonSerializer.Serialize(new{modules=modules.Descriptors.Select(d=>new{d.Id,d.Version}).ToArray(),plugins=plugins.Plugins.Select(p=>new{p.Manifest.Id,p.Manifest.Version}).OrderBy(p=>p.Id).ToArray()});
   string RegistryBefore(string id)=>RegistryState();
   doc.NewSolution();Set("toolboxExpanded",true);Set("toolCatalogPath",catalog);Write(false);Call("RefreshToolCatalog");
   Check(!Directory.EnumerateFiles(AppContext.BaseDirectory,"ManagedToolDependencyFixture19.dll",SearchOption.AllDirectories).Any(),"Missing-dependency case has no deployed dependency assembly");
   Category("Success").IsExpanded=true;var successful=plugins.Find(good.ModuleId);Check(successful is not null,"Baseline explicit dynamic reference registers before independent negative cases");
   foreach(var pair in cases)
   {
    string before=RegistryBefore(pair.Key);Category(pair.Key).IsExpanded=true;
    var observation=session.Observations.Single(o=>o.Reference.TypeFullName==pair.Value.TypeFullName&&o.Reference.AssemblyPath==pair.Value.AssemblyPath&&o.Reference.Sha256.Equals(pair.Value.Sha256,StringComparison.OrdinalIgnoreCase)&&o.Reference.Version==pair.Value.Version);
    Check(observation.Failure is ManagedToolCatalogLoadException,"Failure matrix retains precise session failure: "+pair.Key);
    Check(RegistryState()==before&&Category(pair.Key).Content is null,"Failure leaves exact module/plugin registries unchanged: "+pair.Key);
    failures.Add(new{caseName=pair.Key,observation.Stage,exception=observation.Failure!.ToString()});
   }
   Check(!AppDomain.CurrentDomain.GetAssemblies().Any(a=>a.GetName().Name=="ManagedToolDependencyFixture19"),"Dependency failure never silently imports an unrelated DLL");
   Category("PrefixConflict").IsExpanded=true;Check(Category("PrefixConflict").Content is null,"Conflicting later item aborts first category presentation");
   Category("PrefixConflict").IsExpanded=false;Category("PrefixConflict").IsExpanded=true;
   Check(Category("PrefixConflict").Content is System.Windows.Controls.Primitives.UniformGrid {Children.Count:1}&&Attempts("PrefixConflict")==1,"Successful category prefix survives conflict and later entry without retry");
   Category("Repair").IsExpanded=true;Write(true);Category("Repair").IsExpanded=false;Category("Repair").IsExpanded=true;
   Check(Attempts("Repair")==1&&Category("Repair").Content is System.Windows.Controls.Primitives.UniformGrid {Children.Count:0},"Repair is sticky until explicit category refresh");
   Call("RefreshToolCatalog");Category("Repair").IsExpanded=true;Category("Success").IsExpanded=true;
   Check(ReferenceEquals(plugins.Find(good.ModuleId),successful)&&modules.Descriptors.Count(d=>d.Id==good.ModuleId)==1,"Explicit refresh repairs failure and retains successful registration exactly once");
   Check(((System.Windows.Controls.Primitives.UniformGrid)Category("Repair").Content).Children.OfType<Button>().Single().IsEnabled,"Repaired category receives the existing successful plugin identity");
   // Registration atomics: mutate only a self-owned plugin after static loader preflight.
   var volatileLoaded=ManagedToolPluginLoader.Load(Ref("VolatileDescriptorPlugin","fixture.volatile19"));var volatileType=volatileLoaded.Plugin.GetType();
   int reads=(int)volatileType.GetField("DescriptorReads")!.GetValue(null)!;volatileType.GetField("FailDescriptor")!.SetValue(null,true);
   var atomicModules=new ModuleRegistry();var atomicPlugins=new ToolPluginRegistry();bool invalidRejected=false;
   try{atomicPlugins.Register(volatileLoaded.Plugin,atomicModules);}catch(ArgumentException error){invalidRejected=true;failures.Add(new{caseName="invalid-descriptor-at-registration",exception=error.ToString()});}
   Check(invalidRejected&&!atomicPlugins.Plugins.Any()&&!atomicModules.Descriptors.Any(),"Descriptor becoming invalid after loader validation cannot leave plugin-only registration");
   Check((int)volatileType.GetField("DescriptorReads")!.GetValue(null)! == reads+1,"Registration captures volatile Descriptor exactly once");volatileType.GetField("FailDescriptor")!.SetValue(null,false);
   var collisionModules=new ModuleRegistry();collisionModules.Register(volatileLoaded.Plugin.Descriptor,volatileLoaded.Plugin.CreateRuntime);var collisionPlugins=new ToolPluginRegistry();bool collided=false;
   try{collisionPlugins.Register(volatileLoaded.Plugin,collisionModules);}catch(InvalidOperationException error){collided=true;failures.Add(new{caseName="preexisting-module-collision",exception=error.ToString()});}
   Check(collided&&!collisionPlugins.Plugins.Any()&&collisionModules.Descriptors.Count==1,"Preexisting module collision cannot publish a plugin-only entry");
   var fixtureAssembly=volatileType.Assembly;var flipping=(IVisionToolPlugin)Activator.CreateInstance(fixtureAssembly.GetType("ManagedToolFailureFixture19.FlippingManifestPlugin",true)!)!;
   var captureModules=new ModuleRegistry();var capturePlugins=new ToolPluginRegistry();capturePlugins.Register(flipping,captureModules);
   Check(ReferenceEquals(capturePlugins.Find("fixture.capture19"),flipping)&&capturePlugins.Find("fixture.drift19") is null&&captureModules.Descriptors.Single().Id=="fixture.capture19","Registration key uses the single captured Manifest rather than a later getter value");
   Check((int)flipping.GetType().GetField("reads",Private)!.GetValue(flipping)! == 1,"Registration reads Manifest exactly once");
   var identity=ManagedToolPluginLoader.Load(Ref("ChangingIdentityPlugin","fixture.identity19"));identity.Plugin.GetType().GetField("CurrentId")!.SetValue(null,"fixture.changed19");
   var expectedModules=new ModuleRegistry();var expectedPlugins=new ToolPluginRegistry();bool driftRejected=false;
   try{expectedPlugins.Register(identity.Plugin,expectedModules,"fixture.identity19","1.0.0");}catch(ArgumentException error){driftRejected=true;failures.Add(new{caseName="identity-drift-after-load",exception=error.ToString()});}
   Check(driftRejected&&!expectedPlugins.Plugins.Any()&&!expectedModules.Descriptors.Any(),"Expected reference identity is rechecked inside the same registration capture before either registry changes");
   // Existing-document consumption must preserve unavailable modules, not invent a runtime.
   var support=fixtureAssembly.GetType("ManagedToolFailureFixture19.PluginSupport",true)!;
   var runtimeCreates=support.GetField("RuntimeCreates")!;
   foreach(var unavailable in new[]{("unknown","fixture.never-registered19","1.0.0"),("wrong-version",good.ModuleId,"99.0.0")})
   {
    string phase=unavailable.Item1;var opaque=JsonSerializer.SerializeToElement(new{nested=new object[]{1,"中文",new{enabled=true}},unknownType="vendor-private",bytes="AAECAw=="});
    var node=new NodeDocument("unavailable-node",unavailable.Item2,unavailable.Item3,"unavailable preserved",new(){["OpaqueUnknown"]=opaque,["Scalar"]=JsonSerializer.SerializeToElement(17)},11,13);
    var solution=new SolutionDocument(SolutionStore.CurrentVersion,"unavailable-"+phase,"unavailable",[new("unavailable-procedure","unavailable",[node],[])],[]);
    string source=Path.Combine(root,phase+"-original.vmx.json");SolutionStore.Save(source,solution);string sourceHash=Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(source)));
    doc.Load(source);Set("selected",node.Id);Call("RefreshRight");
    var preserved=doc.CurrentProcedure.Nodes.Single();
    Check(preserved.ModuleId==node.ModuleId&&preserved.ModuleVersion==node.ModuleVersion&&JsonSerializer.Serialize(preserved.Parameters)==JsonSerializer.Serialize(node.Parameters),phase+": actual host load/selection preserves identity and opaque parameters without crashing");
    var validation=doc.Validate();Check(!validation.IsValid&&validation.Issues.Any(i=>i.Code=="Module"&&i.NodeId==node.Id),phase+": missing exact module/version is a Module validation error");
    int createsBefore=(int)runtimeCreates.GetValue(null)!;int sessionsBefore=((IDictionary)Field("workflowSessions")!).Count;
    ((Task)Call("StartRun")!).GetAwaiter().GetResult();
    Check(Field("activeRun") is null&&((IDictionary)Field("workflowSessions")!).Count==sessionsBefore&&(int)runtimeCreates.GetValue(null)! == createsBefore,phase+": real StartRun rejects unavailable module before runtime/session creation");
    string resaved=Path.Combine(root,phase+"-resaved.vmx.json");doc.Save(resaved);doc.Load(resaved);Set("selected",node.Id);Call("RefreshRight");preserved=doc.CurrentProcedure.Nodes.Single();
    Check(preserved.ModuleId==node.ModuleId&&preserved.ModuleVersion==node.ModuleVersion&&JsonSerializer.Serialize(preserved.Parameters)==JsonSerializer.Serialize(node.Parameters),phase+": save/reload retains unavailable node and opaque payload information");
    Check(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(source)))==sourceHash,phase+": original source file bytes are not rewritten by failed run or SaveAs");
   }
   Check(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(assemblyPath)))==hash,"Self-owned failure fixture bytes remain unchanged");
   window.Close();app.Shutdown();File.WriteAllText(Path.Combine(root,"summary.json"),JsonSerializer.Serialize(new{checks,hash,failures,scope="Actual MainWindow failure categories plus isolated registration atomics; self-owned fixtures only; no original SDK, no camera discovery"},new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine($"PASS {checks}/{checks}; {root}");return 0;
  }
  catch(Exception error){File.WriteAllText(Path.Combine(root,"failed.json"),JsonSerializer.Serialize(new{checks,failures,error=error.ToString()},new JsonSerializerOptions{WriteIndented=true}));Console.Error.WriteLine(error);return 1;}
 }
}
