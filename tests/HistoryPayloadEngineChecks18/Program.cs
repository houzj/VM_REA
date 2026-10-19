using System.Text.Json;
using VisionEditor.Core;

if(args.Length!=1)throw new ArgumentException("Supply an owned audit directory.");
string output=Path.Combine(Path.GetFullPath(args[0]),"payload-engine-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
var checks=new List<string>();void Check(bool condition,string label){if(!condition)throw new Exception(label);checks.Add(label);}
var store=new HistoryPayloadStore(output);var engine=new DocumentHistoryEngine<HistoryPayloadLease>(lease=>lease.Dispose()){Capacity=2};
byte value=1;void Register(){engine.BeginRoot();engine.RegisterPart("opaque",()=>store.CreateFile([value]),lease=>value=lease.ReadBytes()[0]);}
Register();string first=engine.CurrentRoot!.Parts[0].OldValue.FilePath!;value=2;Register();string second=engine.CurrentRoot!.Parts[0].OldValue.FilePath!;value=3;Register();
Check(!File.Exists(first)&&File.Exists(second)&&engine.UndoCount==2,"Capacity evicts and deletes oldest root payload through actual engine callback");
string third=engine.CurrentRoot!.Parts[0].OldValue.FilePath!;value=4;engine.Undo();
var moved=engine.NextRedoRoot!.Parts[0];string newValue=moved.NewValue.FilePath!;
Check(value==3&&File.Exists(third)&&File.Exists(newValue)&&engine.RedoCount==1,"Undo migrates root and retains both real old and completed new files");
engine.Redo();Check(value==4&&File.Exists(third)&&File.Exists(newValue),"Redo reads completed payload without releasing moved root");
engine.Undo();value=5;Register();
Check(!File.Exists(third)&&!File.Exists(newValue)&&engine.RedoCount==0,"Admitted branch clears redo and releases both payloads");
engine.Capacity=1;Check(!File.Exists(second)&&engine.UndoCount==1,"Dynamic shrink releases oldest undo payload");
engine.Clear();Check(store.LivePayloadCount==0,"Clear releases all remaining engine payloads");

var shared=store.CreateFile([11]);string sharedPath=shared.FilePath!;
engine.BeginRoot();engine.RegisterPart("a",()=>shared.Retain(),_=>{});
engine.BeginRoot();engine.RegisterPart("b",()=>store.AcquireFile(sharedPath),_=>{});
Check(File.Exists(sharedPath),"Shared tracked path survives old root eviction");
shared.Dispose();Check(File.Exists(sharedPath),"Current root owns shared path after producer release");engine.Clear();Check(!File.Exists(sharedPath),"Shared file removed only when final actual root releases it");

value=7;Register();var adapted=engine.CurrentRoot!.Parts[0];string before=adapted.OldValue.FilePath!;
adapted.AdaptOldValue(_=>store.CreateFile([8]));
Check(!File.Exists(before)&&adapted.OldValue.ReadBytes()[0]==8,"Old payload adaptation releases superseded file");
value=9;engine.Undo();string beforeNew=engine.NextRedoRoot!.Parts[0].NewValue.FilePath!;
engine.NextRedoRoot.Parts[0].AdaptNewValue(_=>store.CreateFile([10]));
Check(!File.Exists(beforeNew),"Completed-new adaptation releases superseded new file");engine.Redo();Check(value==10,"Redo consumes real replacement payload bytes");engine.Clear();

value=20;Register();string locked=engine.CurrentRoot!.Parts[0].OldValue.FilePath!;
using(var handle=new FileStream(locked,FileMode.Open,FileAccess.Read,FileShare.Read)){
  engine.Clear();Check(engine.RetainedReleaseFailureCount==1&&engine.ReleaseErrors.Count>0&&File.Exists(locked),"Actual engine retains failed root and original file when release is locked");
}
Check(File.Exists(locked),"Release failure is not silently retried after blocker leaves");
engine.ReleaseRetainedPayloads();Check(engine.RetainedReleaseFailureCount==0&&!File.Exists(locked),"Explicit controlled retry releases retained failed root once blocker is gone");
Check(engine.ReleaseErrors.Count>0,"Release failure audit remains after explicit recovery");
engine.Dispose();store.Dispose();Check(!Directory.Exists(store.DirectoryPath),"Disposed engine and final store leave no owned live payload directory");
File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=checks.Count,checks,scope="actual managed history engine release callbacks and opaque files; no SDK export or native formats"},new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine("PASS "+checks.Count+" engine-payload checks; audit="+output);
