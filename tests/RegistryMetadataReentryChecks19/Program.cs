using System.IO;
using System.Text.Json;
using VisionFramework;
using VisionPluginContracts;

string? output=null;
try{
 if(args.Length!=1)throw new ArgumentException("Supply owned audit parent.");
 output=Path.Combine(Path.GetFullPath(args[0]),"registry-reentry-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
 var checks=new List<string>();void Check(bool value,string label){if(!value)throw new Exception(label);checks.Add(label);Console.WriteLine("PASS "+label);}
 var modules=new ModuleRegistry();var registry=new ToolPluginRegistry();var plugin=new MetadataPlugin("test.metadata-reentry");bool innerRejected=false;
 plugin.DuringManifest=()=>{try{registry.Register(plugin,modules);}catch(InvalidOperationException){innerRejected=true;}};
 registry.Register(plugin,modules);
 Check(innerRejected,"Manifest getter reentry is rejected before reading nested metadata");
 Check(ReferenceEquals(registry.Find(plugin.Id),plugin)&&modules.Descriptors.Count==1,"Outer registration owns the only real plugin and module commit");
 var bad=new MetadataPlugin("test.metadata-failure"){ThrowDescriptor=true};bool failed=false;
 try{registry.Register(bad,modules);}catch(InvalidDataException){failed=true;}
 Check(failed&&registry.Find(bad.Id) is null&&modules.Descriptors.All(d=>d.Id!=bad.Id),"Metadata getter failure leaves neither registry partially mutated");
 bad.ThrowDescriptor=false;registry.Register(bad,modules);
 Check(registry.Find(bad.Id)==bad&&modules.Descriptors.Count==2,"Metadata failure resets admission so a corrected plugin registers normally");
 var drift=new MetadataPlugin("test.expected-retry");bool denied=false;
 try{registry.Register(drift,modules,"test.wrong-expected","1");}catch(ArgumentException){denied=true;}
 registry.Register(drift,modules,drift.Id,"1");
 Check(denied&&registry.Find(drift.Id)==drift&&modules.Descriptors.Count==3,"Expected-identity rejection also resets metadata admission without a ghost entry");
 File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=checks.Count,checks,scope="existing main public registry metadata callbacks, own managed fixtures; no GUI, SDK, execution or concurrency claim"},new JsonSerializerOptions{WriteIndented=true}));
 Console.WriteLine("PASS "+checks.Count+" registry callback checks; audit="+output);return 0;
}catch(Exception error){if(output is not null)File.WriteAllText(Path.Combine(output,"failed.json"),error.ToString());Console.Error.WriteLine(error);return 1;}
sealed class MetadataPlugin(string id):IVisionToolPlugin{
 public string Id=>id;public Action? DuringManifest;bool attempted;public bool ThrowDescriptor;
 public ToolManifest Manifest{get{if(!attempted){attempted=true;DuringManifest?.Invoke();}return new(id,"1",id,"test","");}}
 public ModuleDescriptor Descriptor=>ThrowDescriptor?throw new InvalidDataException("owned metadata failure"):new(id,"1",id,[],[],[]);
 public IModule CreateRuntime()=>throw new NotSupportedException("Never execute fixture");
 public System.Windows.Forms.Form CreateEditor(ToolEditorContext context)=>throw new NotSupportedException("Never open fixture");
 public void Reset(string nodeId){}
}
