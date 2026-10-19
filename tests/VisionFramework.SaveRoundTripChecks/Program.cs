using System.Text.Json;
using VisionFramework;

if(args.Length!=1)throw new ArgumentException("Supply an owned audit directory.");
string root=Path.Combine(Path.GetFullPath(args[0]),"save-roundtrip-"+Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(root);var checks=new List<string>();
void Check(bool ok,string label){if(!ok)throw new Exception(label);checks.Add(label);Console.WriteLine("PASS "+label);}
SolutionDocument Document(JsonElement payload)=>new(SolutionStore.CurrentVersion,"save-roundtrip","save-roundtrip",[new("p","p",[new("n","independent.opaque","1","n",new(){["Payload"]=payload},0,0)],[])],[]);
using var valid=JsonDocument.Parse("{\"left\":{\"value\":1},\"right\":{\"value\":2}}");
string file=Path.Combine(root,"valid.vmx.json");SolutionStore.Save(file,Document(valid.RootElement.Clone()));
byte[] before=File.ReadAllBytes(file);
Check(SolutionStore.Load(file).Procedures[0].Nodes[0].Parameters["Payload"].GetProperty("right").GetProperty("value").GetInt32()==2,"same names in distinct objects round-trip");
foreach(string text in new[]{"{\"value\":1,\"value\":2}","{\"items\":[{\"value\":1,\"value\":2}]}"}){
 using var duplicate=JsonDocument.Parse(text); bool rejected=false;
 try{SolutionStore.Save(file,Document(duplicate.RootElement.Clone()));}catch(InvalidDataException){rejected=true;}
 Check(rejected,"duplicate payload rejected before replacing valid file");
 Check(File.ReadAllBytes(file).SequenceEqual(before),"rejected save preserves previous bytes");
 Check(!Directory.EnumerateFiles(root,"*.tmp").Any(),"rejection creates no temp files");
 string fresh=Path.Combine(root,"not-created",Guid.NewGuid().ToString("N"),"invalid.vmx.json");
 try{SolutionStore.Save(fresh,Document(duplicate.RootElement.Clone()));throw new Exception("expected rejection");}catch(InvalidDataException){}
 Check(!Directory.Exists(Path.GetDirectoryName(fresh)),"rejection has no destination-directory side effect");
}
File.WriteAllText(Path.Combine(root,"result.json"),JsonSerializer.Serialize(new{passed=true,checks,authority="independent JSON persistence implementation; not native sol"},new JsonSerializerOptions{WriteIndented=true}));
Console.WriteLine($"PASS {checks.Count}; {root}");
