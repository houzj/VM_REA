using System.Text.Json;
using VisionEditor.Core;

if(args.Length!=1)throw new ArgumentException("Supply an owned audit directory.");
string output=Path.Combine(Path.GetFullPath(args[0]),"history-engine-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
var checks=new List<string>();
void Check(bool value,string label){if(!value)throw new Exception(label);checks.Add(label);Console.WriteLine("PASS "+label);}
var engine=new DocumentHistoryEngine<int>();int value=10,captures=0;
int Capture(){captures++;return value;}
Check(!engine.Undo()&&!engine.Redo()&&!engine.IsReplaying,"Empty replay is harmless and resets admission depth");
Check(!engine.RegisterPart("orphan",Capture,x=>value=x)&&captures==0,"Orphan part rejected without old capture");
engine.SetForbidRecord(true);engine.BeginRoot();Check(!engine.CurrentRoot!.ForbidBuild,"Forbid on empty stack does not leak to next root");
Check(engine.RegisterPart("module",Capture,x=>value=x),"Ordinary part admitted");
Check(captures==1&&!engine.CurrentRoot!.Parts[0].IsDataComplete,"Registration captures only old value");
value=20;value=30;
Check(engine.Undo()&&value==10&&captures==2&&engine.RedoCount==1,"First Undo captures final new value then restores old");
Check(engine.Redo()&&value==30&&captures==2,"Redo replays captured new without recapture");
Check(engine.Undo()&&value==10&&captures==2,"Second Undo reuses completed payload");
engine.Redo();engine.SetForbidRecord(true);engine.SetForbidRecord(true);engine.SetForbidRecord(false);
Check(!engine.CurrentRoot!.ForbidBuild,"Forbid flag is boolean not nesting count");
engine.SetForbidRecord(true);int before=captures;
Check(!engine.RegisterPart("blocked",Capture,x=>value=x)&&captures==before,"Forbidden root rejects part before capture");
engine.BeginRoot("next");Check(!engine.CurrentRoot!.ForbidBuild,"New root admission ignores old root forbid flag");
bool Unique(DocumentHistoryRoot<int> root,string key)=>root.Parts.All(part=>part.Key!=key);
value=40;engine.RegisterPart("shared",Capture,x=>value=x,canRegister:Unique);value=45;
before=captures;Check(!engine.RegisterPart("shared",Capture,x=>value=x,canRegister:Unique)&&captures==before,"Predicate merge preserves first old and avoids duplicate capture");
value=50;engine.Undo();Check(value==40,"Merged part restores first old value");engine.Redo();Check(value==50,"Merged part captures final new once");

var trace=new List<string>();
DocumentHistoryEngine<int> Ordered(string operation){var e=new DocumentHistoryEngine<int>();e.BeginRoot(operation);int state=0;
  foreach(var item in new[]{("a",0),("b",1),("c",2),("d",1),("e",0)}){string key=item.Item1;e.RegisterPart(key,()=>state,x=>trace.Add((x==0?"U":"R")+key),item.Item2);}
  state=1;return e;}
var normal=Ordered("Edit");normal.Undo();Check(string.Join(",",trace)=="Ud,Ub,Ue,Uc,Ua","Ordinary Undo priority1 reverse then other reverse");
trace.Clear();normal.Redo();Check(string.Join(",",trace)=="Rb,Rd,Ra,Rc,Re","Ordinary Redo priority1 forward then other forward");
trace.Clear();var copied=Ordered("CopyModule");copied.Undo();Check(string.Join(",",trace)=="Ue,Ud,Uc,Ub,Ua","CopyModule Undo reverses all without priority split");
trace.Clear();copied.Redo();Check(string.Join(",",trace)=="Ra,Rb,Rd,Re,Rc","CopyModule Redo executes non2 then priority2 forward");

var replay=new DocumentHistoryEngine<int>();int replayValue=1;bool rejectedPart=false,rejectedRoot=false;
replay.BeginRoot();replay.RegisterPart("a",()=>replayValue,x=>{replayValue=x;rejectedPart=!replay.RegisterPart("recursive",()=>99,_=>{});rejectedRoot=!replay.BeginRoot();});replayValue=2;
replay.Undo();Check(rejectedPart&&rejectedRoot&&replay.UndoCount==0&&replay.RedoCount==1,"Replay admission prevents callback from registering history");
Check(!replay.IsReplaying,"Successful replay releases admission counter");

var captureError=new DocumentHistoryEngine<int>();int captureCalls=0,applies=0;
captureError.BeginRoot();captureError.RegisterPart("fails-update",()=>{captureCalls++;if(captureCalls==2){Check(captureError.CurrentRoot!.Parts[0].IsDataComplete,"Completion flag is set before new capture");throw new InvalidOperationException("capture-fixture");}return 7;},_=>applies++);
Check(!captureError.Undo()&&captureError.LastError?.Message=="capture-fixture","Capture error is retained without escaping service");
Check(captureError.UndoCount==1&&captureError.RedoCount==0&&applies==0&&!captureError.IsReplaying,"Capture failure occurs before stack transfer and resets admission");
Check(captureError.Undo()&&captureCalls==2&&applies==1,"Second Undo skips already marked failed capture like original service");

var applyError=new DocumentHistoryEngine<int>();bool throwApply=true;applyError.BeginRoot();applyError.RegisterPart("fails-apply",()=>1,_=>{if(throwApply)throw new InvalidOperationException("apply-fixture");});
Check(!applyError.Undo()&&applyError.LastError?.Message=="apply-fixture","Apply error is retained");
Check(applyError.UndoCount==0&&applyError.RedoCount==1&&!applyError.IsReplaying,"Apply error preserves original pre-apply stack migration without invented rollback");
throwApply=false;Check(applyError.Redo()&&applyError.LastError is null,"Later replay clears previous error and may proceed");

var branching=new DocumentHistoryEngine<int>();int branch=1;branching.BeginRoot();branching.RegisterPart("a",()=>branch,x=>branch=x);branch=2;branching.Undo();branching.BeginRoot("new");
Check(branching.RedoCount==1,"Root creation alone does not clear redo");branching.SetForbidRecord(true);branching.RegisterPart("x",()=>3,_=>{});
Check(branching.RedoCount==1,"Rejected part does not clear redo");branching.SetForbidRecord(false);branching.RegisterPart("b",()=>branch,x=>branch=x);
Check(branching.RedoCount==0,"Admitted new part clears redo branch");

var capacity=new DocumentHistoryEngine<int>();for(int i=0;i<18;i++){capacity.BeginRoot();capacity.RegisterPart("x",()=>i,_=>{});}
Check(capacity.UndoCount==15,"Default history capacity retains last15 roots");
capacity.Undo();capacity.Undo();capacity.Capacity=5;
Check(capacity.UndoCount==5&&capacity.RedoCount==0,"Shrink clears redo then evicts oldest undo roots");
capacity.Capacity=0;Check(capacity.UndoCount==0&&!capacity.BeginRoot()&&!capacity.RegisterPart("x",()=>1,_=>{}),"Zero capacity admits no root or orphan part");
capacity.Clear();Check(capacity.UndoCount==0&&capacity.RedoCount==0&&capacity.LastError is null,"Clear resets both stacks and retained error");

var host=new DocumentHistoryEngine<(int structure,int zoom)>();var current=(structure:1,zoom:2);host.BeginRoot();host.RegisterPart("document",()=>current,x=>current=x);current=(2,3);host.Undo();current=(1,9);
host.NextRedoRoot!.Parts[0].AdaptOldValue(old=>(old.structure,current.zoom));host.Redo();Check(current==(2,3),"Explicit host adaptation leaves structural/new payload unchanged");host.Undo();Check(current==(1,9),"Explicit host view adaptation preserves interim zoom in old payload");
host.Redo();current=(2,7);var completedPart=host.CurrentRoot!.Parts[0];completedPart.AdaptNewValue(next=>(next.structure,current.zoom));host.Undo();host.Redo();
Check(current==(2,7),"Explicit host completed-new adaptation preserves late zoom without recapturing structure");
Check(completedPart.NewValue==(2,7)&&completedPart.OldValue==(1,9),"Diagnostic payload getters expose distinct old and completed new values");
var unready=new DocumentHistoryEngine<int>();unready.BeginRoot();unready.RegisterPart("a",()=>3,_=>{});bool rejectedAdapt=false;try{unready.CurrentRoot!.Parts[0].AdaptNewValue(x=>x+1);}catch(InvalidOperationException){rejectedAdapt=true;}
Check(rejectedAdapt&&!unready.CurrentRoot!.Parts[0].IsDataComplete,"New adaptation cannot bypass first Undo capture");
var lazy=new DocumentHistoryEngine<int>();lazy.SetIsLazyLoading(true);lazy.SetIsLazyLoading(true);
Check(!lazy.BeginRoot()&&lazy.LazyLoadingDepth==2,"Nested lazy loading blocks root admission");lazy.SetIsLazyLoading(false);Check(!lazy.BeginRoot(),"One lazy exit keeps nested admission blocked");
lazy.SetIsLazyLoading(false);Check(lazy.BeginRoot(),"Final lazy exit enables admission");lazy.SetIsLazyLoading(true);
Check(!lazy.RegisterPart("a",()=>1,_=>{}),"Lazy loading blocks part admission too");lazy.SetIsLazyLoading(false);lazy.SetIsLazyLoading(false);
Check(lazy.LazyLoadingDepth==-1&&lazy.RegisterPart("a",()=>1,_=>{}),"Signed lazy counter matches original underflow admission instead of invented clamp");
var rootPredicate=new DocumentHistoryEngine<int>();Check(!rootPredicate.BeginRoot("conditional",_=>true),"Root predicate with no existing root is rejected");rootPredicate.BeginRoot();
Check(!rootPredicate.BeginRoot("rejected",_=>false)&&rootPredicate.UndoCount==1,"Root predicate rejection does not create another root");
File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=checks.Count,checks,scope="managed root/part service contract; no SDK payload, TempFiles, password or native format claims"},new JsonSerializerOptions{WriteIndented=true}));
Console.WriteLine("PASS "+checks.Count+" checks; audit="+output);
