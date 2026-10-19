using System.Reflection;
using System.Text.Json;
using VisionEditor.Core;
using VisionFramework;

if(args.Length!=1)throw new ArgumentException("Supply owned audit parent.");
string output=Path.Combine(Path.GetFullPath(args[0]),"history-host-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
var checks=new List<string>();void Check(bool value,string name){if(!value)throw new Exception(name);checks.Add(name);Console.WriteLine("PASS "+name);}
int Files(string path)=>Directory.Exists(path)?Directory.GetFiles(path,"*.payload").Length:0;
var document=new GraphEditorDocument(new ModuleRegistry(),output);string id=document.CurrentProcedureId;
document.RenameProcedure(id,"one");string owned=document.HistoryPayloadDirectory!;
Check(Files(owned)==1,"Actual document old part uses one owned payload file");
document.RenameProcedure(id,"two");Check(Files(owned)==2,"Separate edits own separate old files");
document.Undo();Check(document.CurrentProcedure.Name=="one"&&Files(owned)==3,"First actual Undo reads old file and captures new file");
document.Redo();Check(document.CurrentProcedure.Name=="two"&&Files(owned)==3,"View adaptation replaces rather than leaks its previous file");
for(int i=0;i<20;i++){document.Undo();document.Redo();}
Check(Files(owned)==3,"Repeated real replay releases superseded view payload leases");
document.Undo();document.RenameProcedure(id,"branch");Check(!document.CanRedo&&Files(owned)==2,"Real new edit releases invalidated redo old and new payloads");
for(int i=0;i<25;i++)document.RenameProcedure(id,"capacity-"+i);
Check(Files(owned)==15,"Real default capacity evicts and releases the oldest document roots");
string saved=Path.Combine(output,"same-id.vmx.json");document.Save(saved);string identity=document.SolutionId;document.Load(saved);
Check(document.SolutionId==identity&&Files(owned)==0&&!document.CanUndo,"Same-ID Load releases all actual document history payloads");
document.RenameProcedure(id,"after-load");document.NewSolution();Check(Files(owned)==0,"NewSolution releases actual old document root ownership");
document.RenameProcedure(document.CurrentProcedureId,"before-dispose");document.Dispose();document.Dispose();Check(!Directory.Exists(owned),"Document disposal releases owned files and unique directory exactly once");
using(var memory=new GraphEditorDocument(new ModuleRegistry())){memory.RenameProcedure(memory.CurrentProcedureId,"small");Check(memory.HistoryPayloadDirectory is null,"Small default document remains memory-backed without creating a temp directory");}
using(var large=new GraphEditorDocument(new ModuleRegistry())){large.RenameProcedure(large.CurrentProcedureId,new string('x',70000));large.RenameProcedure(large.CurrentProcedureId,"after-large");Check(large.HistoryPayloadDirectory is {} d&&Files(d)>=1,"Large production document automatically spills actual history payload to an owned file");large.Undo();Check(large.CurrentProcedure.Name.Length==70000,"Large production Undo deserializes the original file-backed state");}
var engineStore=new HistoryPayloadStore(output);var engine=new DocumentHistoryEngine<HistoryPayloadLease>(lease=>lease.Dispose());
engine.BeginRoot();engine.RegisterPart("p",()=>engineStore.CreateFile([1,2,3]),_=>{});var lockedPath=engine.CurrentRoot!.Parts.Single().OldValue.FilePath!;
using(var held=new FileStream(lockedPath,FileMode.Open,FileAccess.Read,FileShare.Read)){
 engine.Clear();Check(engine.RetainedReleaseFailureCount==1&&engine.ReleaseErrors.Count==1&&File.Exists(lockedPath),"Locked file release preserves failed root and exact payload ownership");
}
engine.ReleaseRetainedPayloads();Check(engine.RetainedReleaseFailureCount==0&&!File.Exists(lockedPath),"Explicit retry after owned lock ends releases retained failure");
engineStore.Dispose();Check(!Directory.Exists(engineStore.DirectoryPath),"Store closes only after failed-root lease is actually released");
var store=new HistoryPayloadStore(output);var e=new DocumentHistoryEngine<HistoryPayloadLease>(lease=>lease.Dispose());
e.BeginRoot();e.RegisterPart("a",()=>store.CreateFile([8]),_=>{});var shared=e.CurrentRoot!.Parts.Single().OldValue.Retain();e.Capacity=0;
Check(File.Exists(shared.FilePath),"Capacity zero releases root but retained shared-file lease remains valid");shared.Dispose();Check(Files(store.DirectoryPath)==0,"Shared file deletes only on last actual lease");store.Dispose();
var markerDocument=new GraphEditorDocument(new ModuleRegistry(),output);markerDocument.RenameProcedure(markerDocument.CurrentProcedureId,"marker-owned");string markerDirectory=markerDocument.HistoryPayloadDirectory!;
File.WriteAllText(Path.Combine(markerDirectory,".owner"),"changed-owned-marker");markerDocument.Dispose();
Check(markerDocument.HistoryReleaseErrors.Count==1&&Directory.Exists(markerDirectory),"Changed ownership marker preserves directory and records cleanup failure without blocking host shutdown");
File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=checks.Count,checks,scope="actual main history engine/document opaque JSON file ownership; no SDK export/TempFiles format parity"},new JsonSerializerOptions{WriteIndented=true}));
Console.WriteLine("PASS "+checks.Count+" host checks; audit="+output);
