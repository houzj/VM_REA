using System.Text.Json;
using VisionFramework;
var directory=Path.Combine(Path.GetTempPath(),"vm-model-checks-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(directory);int checks=0;
void Check(bool yes,string label){if(!yes)throw new Exception(label);checks++;Console.WriteLine("PASS "+label);}
var node=new NodeDocument("source","vision.image-file","1.0.0","源",new(){{"Path",JsonSerializer.SerializeToElement("assets/image.bmp")}},17.5,33.2,1500);
var procedure=new ProcedureDocument("p","流程",new[]{node},Array.Empty<ConnectionDocument>());
var original=new SolutionDocument(1,"demo","多流程方案",new[]{procedure,procedure with{Id="p2",Name="第二流程"}},new[]{new ResourceDocument("image","assets/image.bmp",new string('a',64))});
string path=Path.Combine(directory,"solution.vmx.json");SolutionStore.Save(path,original);var loaded=SolutionStore.OpenWorkspace(path);
Check(loaded.Document.Procedures.Count==2,"multiple procedures survive roundtrip");Check(loaded.Document.Procedures[0].Nodes[0].Parameters["Path"].GetString()=="assets/image.bmp","relative module path retained");Check(loaded.BaseDirectory==directory,"workspace base directory is document directory");Check(loaded.Document.Procedures[0].Nodes[0].TimeoutMs==1500&&loaded.Document.Procedures[0].Nodes[0].X==17.5,"timeout and layout retained");Check(loaded.Document.Resources[0].Sha256==new string('a',64),"resource identity retained");
void Reject(SolutionDocument bad,string name){string before=File.ReadAllText(path);try{SolutionStore.Save(path,bad);throw new Exception("Accepted "+name);}catch(InvalidDataException){}Check(File.ReadAllText(path)==before,name+" rejected before replacing saved solution");}
Reject(original with{FormatVersion=2},"future schema");Reject(original with{Procedures=new[]{procedure,procedure}},"duplicate procedures");Reject(original with{Procedures=new[]{procedure with{Nodes=new[]{node,node}}}},"duplicate nodes");Reject(original with{Procedures=new[]{procedure with{Connections=new[]{new ConnectionDocument("source","Image","missing","Image")}}}},"dangling connection");Reject(original with{Resources=new[]{new ResourceDocument("i","x","bad")}},"bad resource hash");Reject(original with{Procedures=new[]{procedure with{Nodes=new[]{node with{X=double.NaN}}}}},"nonfinite node position");Reject(original with{Procedures=new[]{procedure with{Nodes=new[]{node with{TimeoutMs=-1}}}}},"negative timeout");
string malformed=Path.Combine(directory,"bad.json");File.WriteAllText(malformed,"{\"FormatVersion\":1,\"FormatVersion\":1}");try{SolutionStore.Load(malformed);throw new Exception("Accepted duplicate JSON");}catch(InvalidDataException){checks++;Console.WriteLine("PASS duplicate JSON rejected");}
var json=File.ReadAllText(path);File.WriteAllText(malformed,json.Replace("\"FormatVersion\": 1","\"Unknown\": 5, \"FormatVersion\": 1"));try{SolutionStore.Load(malformed);throw new Exception("Accepted unknown property");}catch(JsonException){checks++;Console.WriteLine("PASS unknown document field rejected");}
var rectangle=new RoiDocument("roi1",RoiKind.Rectangle,30,40,Width:20,Height:10,AngleDegrees:15);
var geometryDoc=original with{Procedures=new[]{procedure with{Nodes=new[]{node with{Rois=new[]{rectangle}}}}}};
string geometryPath=Path.Combine(directory,"geometry.json");SolutionStore.Save(geometryPath,geometryDoc);
Check(SolutionStore.Load(geometryPath).Procedures[0].Nodes[0].Rois![0]==rectangle,"ROI geometry roundtrip");
Reject(original with{Procedures=new[]{procedure with{Nodes=new[]{node with{Rois=new[]{rectangle with{Width=-1}}}}}}},"invalid rectangle");
Reject(original with{Procedures=new[]{procedure with{Nodes=new[]{node with{Rois=new[]{rectangle,rectangle}}}}}},"duplicate ROI identity");
string resourcePath=Path.Combine(directory,"resource.bin");File.WriteAllBytes(resourcePath,new byte[]{1,2,3,4});string digest=Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(resourcePath)));
var catalog=new ResourceCatalog(directory,new[]{new ResourceDocument("verified","resource.bin",digest)});using(var verified=catalog.OpenVerified("verified")){Check(verified.Position==0&&verified.ReadByte()==1,"verified relative resource opens at start");}
File.WriteAllBytes(resourcePath,new byte[]{4,3,2,1});try{using var stale=catalog.OpenVerified("verified");throw new Exception("Accepted modified resource");}catch(InvalidDataException){checks++;Console.WriteLine("PASS changed resource digest rejected");}
try{catalog.Resolve("missing");throw new Exception("Accepted missing resource");}catch(KeyNotFoundException){checks++;Console.WriteLine("PASS missing resource id rejected");}
var explicitDoc=original with{Procedures=original.Procedures.Select(p=>ProcedureGraph.Separate(p)).ToArray()};SolutionStore.Save(path,explicitDoc);var loadedExplicit=SolutionStore.Load(path);Check(loadedExplicit.Procedures[0].ExecutionLinks is not null,"explicit empty execution list roundtrip differs from legacy null");
Check(SolutionStore.Load(geometryPath).Procedures[0].ExecutionLinks is null,"legacy null execution retained");
Reject(original with{Procedures=[procedure with{ExecutionLinks=[new("missing",node.Id)]}]},"dangling execution relation");
Console.WriteLine($"{checks} persistence checks passed.");if(!Path.GetFullPath(directory).StartsWith(Path.GetFullPath(Path.GetTempPath()),StringComparison.OrdinalIgnoreCase)||!Path.GetFileName(directory).StartsWith("vm-model-checks-",StringComparison.Ordinal))throw new Exception("Unexpected cleanup path.");Directory.Delete(directory,true);


