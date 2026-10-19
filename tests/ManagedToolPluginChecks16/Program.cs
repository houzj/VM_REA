using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;

static class Program
{
 [STAThread]static int Main(string[] args)
 {
  string output=Path.Combine(Path.GetFullPath(args.Single()),"managed-loader-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);int checks=0;var failures=new List<object>();
  try
  {
   string fixture=Path.Combine(AppContext.BaseDirectory,"fixture","ManagedToolFixture16.dll");string hash=Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(fixture)));
   var reference=new ManagedToolPluginReference(fixture,"ManagedToolFixture16.FixturePlugin",hash,"fixture.dynamic","1.0.0");
   void Check(bool ok,string label){if(!ok)throw new Exception(label);checks++;Console.WriteLine("PASS "+label);}
   bool IsLoaded()=>AppDomain.CurrentDomain.GetAssemblies().Any(a=>a.GetName().Name=="ManagedToolFixture16");
   void Reject(ManagedToolPluginReference candidate,string label){bool rejected=false;try{ManagedToolPluginLoader.Load(candidate);}catch(Exception error)when(error is InvalidDataException or InvalidOperationException or ArgumentException or TargetInvocationException){rejected=true;failures.Add(new{label,type=error.GetType().FullName,error.Message,inner=error.InnerException?.Message});}Check(rejected,label);}
   Check(!IsLoaded(),"Fixture is not statically referenced or loaded before metadata preflight");
   Reject(reference with{Sha256=new string('0',64)},"Wrong hash rejected before code load");Check(!IsLoaded(),"Hash rejection does not execute fixture");
   Reject(reference with{TypeFullName="ManagedToolFixture16.NotAPlugin"},"Non-contract type rejected from raw PE metadata");Check(!IsLoaded(),"Incompatible type rejection does not load assembly");
   Reject(reference with{TypeFullName="Missing.Type"},"Exact missing type rejected rather than choosing first convenient type");
   var loaded=ManagedToolPluginLoader.Load(reference);Check(loaded.AssemblySha256==hash&&loaded.ModuleVersionId.Length>0,"Loaded plugin preserves authenticated assembly identity and MVID");
   Check(loaded.Plugin.GetType().Assembly.Location==fixture&&typeof(IVisionToolPlugin).IsAssignableFrom(loaded.Plugin.GetType()),"Runtime plugin uses exact host contract and selected assembly");
   var modules=new ModuleRegistry();var plugins=new ToolPluginRegistry();plugins.Register(loaded.Plugin,modules);
   Check(ReferenceEquals(plugins.Find(reference.ModuleId),loaded.Plugin)&&modules.Descriptors.Count==1,"Existing ToolPluginRegistry and ModuleRegistry consume real loaded plugin");
   var document=new GraphEditorDocument(modules);string node=document.AddNode(reference.ModuleId,reference.Version,10,20,new(){["Value"]=JsonSerializer.SerializeToElement(42)});
   var result=new GraphExecutor(modules).RunAsync(document.CurrentProcedure,output).GetAwaiter().GetResult();
   Check(result.Status==SessionStatus.Success&&result.Nodes.Single().Outputs["Value"].As<long>()==42,"Real graph executes dynamically registered fixture node and returns its value");
   string saved=Path.Combine(output,"dynamic.vmx.json");document.Save(saved);document.Load(saved);Check(document.CurrentProcedure.Nodes.Single().ModuleId==reference.ModuleId,"Dynamic module identity persists through ordinary host document storage");
   using(var editor=loaded.Plugin.CreateEditor(new ToolEditorContext(document.CurrentProcedure.Nodes.Single(),output,_=>{},_=>throw new NotSupportedException())))
    Check(editor.Text=="Synthetic fixture editor","Existing public editor factory remains usable without another configuration framework");
   Reject(reference with{ModuleId="different.module"},"Manifest and descriptor identity mismatch is explicitly rejected");
   Reject(reference with{TypeFullName="ManagedToolFixture16.ThrowingPlugin"},"Constructor failure is retained without fallback or retry");
   Check(modules.Descriptors.Count==1&&plugins.Plugins.Count()==1,"Loader failures cannot mutate real host registries");
   var again=ManagedToolPluginLoader.Load(reference);Check(again.Plugin.GetType().Assembly==loaded.Plugin.GetType().Assembly,"Repeated explicit load reuses identical assembly identity");
   Check(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(fixture)))==hash,"Fixture source assembly remains unchanged after loading and execution");
   File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{checks,reference,loaded.AssemblySha256,loaded.ModuleVersionId,failures,scope="Owned synthetic plugin; current public contracts only; no vendor DLL or startup integration"},new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine($"PASS {checks}/{checks}; {output}");return 0;
  }
  catch(Exception error){File.WriteAllText(Path.Combine(output,"failed.json"),JsonSerializer.Serialize(new{checks,failures,error=error.ToString()}));Console.Error.WriteLine(error);return 1;}
 }
}
