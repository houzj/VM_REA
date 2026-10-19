using System.Text.Json;
using VisionEditor.Core;

if(args.Length!=1)throw new ArgumentException("Supply an owned audit directory.");
string output=Path.Combine(Path.GetFullPath(args[0]),"payload-checks-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
var checks=new List<string>();void Check(bool value,string label){if(!value)throw new Exception(label);checks.Add(label);}
byte[] bytes=[0,1,2,255];string borrowed=Path.Combine(output,"borrowed.sol");File.WriteAllBytes(borrowed,bytes);
var store=new HistoryPayloadStore(output);string owned=store.DirectoryPath;
bool writeFailed=false;try{store.CreateFile(stream=>{stream.Write(new byte[]{42,43});throw new IOException("controlled-partial-write");});}catch(IOException error)when(error.Message=="controlled-partial-write"){writeFailed=true;}
Check(writeFailed&&store.LivePayloadCount==0&&store.WriteFailures.Count==1,"Producer write failure does not register a partial payload");
Check(!store.WriteFailures[0].FileRetained&&!File.Exists(store.WriteFailures[0].FilePath)&&store.WriteFailures[0].WriteError.Contains("controlled-partial-write"),"Partial write cleans only its newly created file and preserves original diagnostics");
var memory=store.CreateMemory(bytes);bytes[0]=7;Check(memory.ReadBytes()[0]==0,"Memory capture copies caller bytes");var read=memory.ReadBytes();read[0]=8;Check(memory.ReadBytes()[0]==0,"Memory read cannot mutate captured bytes");
var file=store.CreateFile(memory.ReadBytes());string path=file.FilePath!;var shared=file.Retain();var samePath=store.AcquireFile(path);
Check(store.LivePayloadCount==2&&samePath.Kind==HistoryPayloadKind.OwnedFile,"Same-path acquire retains original owned entry");
file.Dispose();file.Dispose();Check(File.Exists(path)&&shared.ReadBytes().SequenceEqual(memory.ReadBytes()),"Old part release is idempotent and preserves shared payload");shared.Dispose();Check(File.Exists(path),"Same-path new part still owns file");samePath.Dispose();samePath.Dispose();Check(!File.Exists(path),"Last shared lease deletes owned file once");
var external=store.AcquireFile(borrowed);external.Dispose();Check(File.Exists(borrowed),"Borrowed solution path is never deleted");
var pending=store.CreateFile([9,8,7]);string pendingPath=pending.FilePath!;store.Dispose();Check(File.Exists(pendingPath),"Store close defers pending lease payload deletion");
Check(pending.ReadBytes().SequenceEqual(new byte[]{9,8,7}),"Existing lease remains readable during deferred close");
bool rejected=false;try{pending.Retain();}catch(ObjectDisposedException){rejected=true;}Check(rejected,"Closed store refuses new ownership");
pending.Dispose();Check(!File.Exists(pendingPath)&&Directory.Exists(owned),"Other live memory lease keeps marker until released");memory.Dispose();Check(!Directory.Exists(owned),"Final lease cleans only empty owned directory");store.Dispose();
var preserved=new HistoryPayloadStore(output);string audit=Path.Combine(preserved.DirectoryPath,"unregistered-audit.txt");File.WriteAllText(audit,"preserve");preserved.Dispose();Check(File.Exists(audit),"Unknown audit file prevents directory cleanup instead of recursive delete");
var lockedStore=new HistoryPayloadStore(output);var locked=lockedStore.CreateFile([4,5]);string lockedPath=locked.FilePath!;bool failed=false;
using(var blocker=new FileStream(lockedPath,FileMode.Open,FileAccess.Read,FileShare.Read)){try{locked.Dispose();}catch(IOException){failed=true;}catch(UnauthorizedAccessException){failed=true;}}
Check(failed&&File.Exists(lockedPath)&&locked.ReadBytes().SequenceEqual(new byte[]{4,5}),"Delete failure preserves exact payload and active ownership");
locked.Dispose();lockedStore.Dispose();Check(!File.Exists(lockedPath),"Explicit caller release after lock ends deletes retained file");
Check(File.ReadAllBytes(borrowed).SequenceEqual(new byte[]{0,1,2,255}),"All operations preserve borrowed file bytes");
File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=checks.Count,checks,writeFailures=store.WriteFailures,scope="opaque ownership and release; no SDK format, native service or installed TempFiles"},new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine("PASS "+checks.Count+" payload checks; audit="+output);
