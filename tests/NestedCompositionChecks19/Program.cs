using System.Text.Json;
using VisionEditor.Core;
using VisionFramework;

if(args.Length!=1)throw new ArgumentException("Supply owned audit parent.");
string output=Path.Combine(Path.GetFullPath(args[0]),"nested-composition-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
try{
var checks=new List<string>();var observations=new List<object>();
void Check(bool value,string label){if(!value)throw new Exception(label);checks.Add(label);Console.WriteLine("PASS "+label);}
int Files(string path)=>Directory.Exists(path)?Directory.GetFiles(path,"*.payload").Length:0;
var control=new FixtureControl();var registry=new ModuleRegistry();WorkflowCalls.Register(registry);
var descriptor=new ModuleDescriptor("test.nested-composition","1","stage",[],[new("Out",DataKind.Integer)],[]);
registry.Register(descriptor,()=>new Stage(descriptor,control));
using var doc=new GraphEditorDocument(registry,output);using var scope=new ModuleExecutionScope();string root=doc.CurrentProcedureId;
string outer=doc.AddOwnedGroup(100,100);string outerBody=doc.EnterOwnedGroup(outer);
string middle=doc.AddOwnedGroup(100,100);string middleBody=doc.EnterOwnedGroup(middle);
string inner=doc.AddOwnedGroup(100,100);string innerBody=doc.EnterOwnedGroup(inner);
string prefix=doc.AddNode(descriptor.Id,"1",100,100),target=doc.AddNode(descriptor.Id,"1",250,100),suffix=doc.AddNode(descriptor.Id,"1",400,100);
doc.ConnectExecution(prefix,target);doc.ConnectExecution(target,suffix);doc.SelectProcedure(root);control.Target=target;control.Prefix=prefix;control.Suffix=suffix;
string payloads=doc.HistoryPayloadDirectory??throw new InvalidOperationException("Expected actual owned file history directory.");
Check(Files(payloads)>0,"Three nested owned Groups are built by actual document edits with file-backed history");
var executor=new SolutionExecutor(registry);
Task<ExecutionSessionResult> Run(CancellationToken token=default)=>executor.RunAsync(new(doc.Document,doc.BaseDirectory),root,cancellationToken:token,scope:scope);
ExecutionSessionResult Inner(ExecutionSessionResult result)=>result.Calls[outer].Calls[middle].Calls[inner];
async Task<ExecutionSessionResult> Settled(Task<ExecutionSessionResult> task)=>await task.WaitAsync(TimeSpan.FromSeconds(15));
control.Mode="ok";var success=await Settled(Run());
Check(success.Status==SessionStatus.Success&&Inner(success).Nodes.Count==3,"Root plus three Group levels execute all three real managed stages");
Check(control.Created==control.Disposed,"Successful nested run awaits every asynchronous module disposal");
foreach(string fault in new[]{"throw","failed-result","dispose"}){
 control.Mode=fault;int sideBefore=control.PrefixEffects;var failed=await Settled(Run());var leaf=Inner(failed);
 Check(failed.Status==SessionStatus.Failed&&failed.Calls[outer].Status==SessionStatus.Failed&&failed.Calls[outer].Calls[middle].Status==SessionStatus.Failed&&leaf.Status==SessionStatus.Failed,fault+": failure propagates through all three Group boundaries");
 Check(leaf.Nodes.Single(n=>n.NodeId==target).Status==NodeRunStatus.Failed&&leaf.Nodes.Single(n=>n.NodeId==suffix).Status==NodeRunStatus.Skipped,fault+": failed target blocks execution successor");
 Check(control.PrefixEffects==sideBefore+1,fault+": already completed prefix effect is retained; no rollback invented");
 Check(control.Created==control.Disposed,fault+": every created module finishes disposal including exceptional target");
 observations.Add(new{fault,status=failed.Status.ToString(),outer=failed.Calls[outer].Status.ToString(),inner=leaf.Status.ToString(),nodes=leaf.Nodes.Select(n=>new{n.NodeId,status=n.Status.ToString(),n.ErrorCode}).ToArray()});
 control.Mode="ok";var recovered=await Settled(Run());Check(recovered.Status==SessionStatus.Success&&Inner(recovered).Nodes.All(n=>n.Status==NodeRunStatus.Success),fault+": next complete nested invocation succeeds without stale failure");
}
using(var cancel=new CancellationTokenSource()){
 control.PrepareWait();int before=control.PrefixEffects;var pending=Run(cancel.Token);await control.Entered.Task.WaitAsync(TimeSpan.FromSeconds(15));cancel.Cancel();
 Check(!pending.IsCompleted,"Cancellation is requested while nested target is actually awaiting controlled release");control.Release.TrySetResult();var cancelled=await Settled(pending);
 Check(cancelled.Status==SessionStatus.Cancelled,"Mid-target cancellation reaches root terminal Cancelled");
 Check(control.PrefixEffects==before+1&&!control.ExecutedAfterWait,"Cancellation does not roll back prefix or execute later suffix");
 Check(control.Created==control.Disposed,"Cancelled nested execution awaits module disposal");
}
control.Mode="ok";Check((await Settled(Run())).Status==SessionStatus.Success,"Next nested run succeeds after mid-execution cancellation");
doc.RenameProcedure(innerBody,"edited nested");doc.Undo();Check(doc.Document.Procedures.Single(p=>p.Id==innerBody).Name!="edited nested","Post-run Undo restores nested identity and old document value");
doc.Redo();Check(doc.Document.Procedures.Single(p=>p.Id==innerBody).Name=="edited nested","Post-run Redo restores nested edit through actual file payload");
string saved=Path.Combine(output,"nested.vmx.json");doc.Save(saved);string identity=doc.SolutionId;doc.Load(saved);
Check(doc.SolutionId==identity&&Files(payloads)==0&&!doc.CanUndo,"Same-ID Load preserves nested graph and releases previous history roots");
Check((await Settled(Run())).Status==SessionStatus.Success,"Reopened nested graph runs successfully after history release");
doc.DeleteNode(outer);scope.RetainProcedureNodes(doc.Document.Procedures.SelectMany(p=>p.Nodes.Select(n=>(p.Id,n.ModuleId,n.Id))));
Check(doc.Document.Procedures.All(p=>p.Id!=outerBody&&p.Id!=middleBody&&p.Id!=innerBody),"Deleting stopped outer Group removes all three owned descendants");
Check(control.StatesCreated==control.StatesDisposed,"Deleting stopped nested graph releases all its retained runtime source states");
doc.Undo();Check(doc.Document.Procedures.Any(p=>p.Id==innerBody),"Undo restores deleted nested graph from owned history payload");
Check((await Settled(Run())).Status==SessionStatus.Success,"Restored nested graph creates fresh live state and runs again");
control.PrepareWait();var pendingClose=Run();await control.Entered.Task.WaitAsync(TimeSpan.FromSeconds(15));int stateBeforeClose=control.StatesDisposed;
scope.Dispose();doc.NewSolution();
Check(Files(payloads)==0&&!doc.CanUndo,"Replacement during captured nested execution releases current document history roots");
Check(control.StatesDisposed==stateBeforeClose,"Closing owned scope defers retained nested state while execution lease is active");
doc.RenameProcedure(doc.CurrentProcedureId,"new owner");Check(Files(payloads)>0,"New document independently owns new history payload while old invocation drains");
doc.Dispose();Check(!Directory.Exists(payloads),"Explicit document close releases only its owned file history directory");
control.Release.TrySetResult();var closed=await Settled(pendingClose);
Check(closed.Status==SessionStatus.Cancelled&&control.Created==control.Disposed,"Old captured nested invocation cancels and disposes modules after owner close");
Check(control.StatesCreated==control.StatesDisposed,"Final nested execution lease releases each retained scope state once");
scope.Dispose();doc.Dispose();Check(control.StatesCreated==control.StatesDisposed,"Repeated close cannot release nested state a second time");
// Complement the deepest cancellation/lifetime matrix with success and actual
// failure propagation at each smaller supported ownership depth.
foreach(int depth in new[]{1,2}){
 var shortControl=new FixtureControl();var shortRegistry=new ModuleRegistry();WorkflowCalls.Register(shortRegistry);
 shortRegistry.Register(descriptor,()=>new Stage(descriptor,shortControl));
 using var shortDocument=new GraphEditorDocument(shortRegistry,output);using var shortScope=new ModuleExecutionScope();string shortRoot=shortDocument.CurrentProcedureId;
 var path=new List<string>();for(int i=0;i<depth;i++){string group=shortDocument.AddOwnedGroup(100,100);path.Add(group);shortDocument.EnterOwnedGroup(group);}
 string shortPrefix=shortDocument.AddNode(descriptor.Id,"1",100,100),shortTarget=shortDocument.AddNode(descriptor.Id,"1",250,100),shortSuffix=shortDocument.AddNode(descriptor.Id,"1",400,100);
 shortDocument.ConnectExecution(shortPrefix,shortTarget);shortDocument.ConnectExecution(shortTarget,shortSuffix);shortDocument.SelectProcedure(shortRoot);
 shortControl.Prefix=shortPrefix;shortControl.Target=shortTarget;shortControl.Suffix=shortSuffix;
 async Task<ExecutionSessionResult> ShortRun()=>await new SolutionExecutor(shortRegistry).RunAsync(new(shortDocument.Document,shortDocument.BaseDirectory),shortRoot,scope:shortScope).WaitAsync(TimeSpan.FromSeconds(15));
 var shallow=await ShortRun();Check(shallow.Status==SessionStatus.Success,$"Depth {depth}: nested success uses actual executor");
 shortControl.Mode="throw";shallow=await ShortRun();bool allFailed=shallow.Status==SessionStatus.Failed;foreach(string group in path){shallow=shallow.Calls[group];allFailed&=shallow.Status==SessionStatus.Failed;}
 Check(allFailed&&shallow.Nodes.Single(n=>n.NodeId==shortSuffix).Status==NodeRunStatus.Skipped,$"Depth {depth}: exception crosses every boundary and blocks suffix");
 shortControl.Mode="ok";Check((await ShortRun()).Status==SessionStatus.Success,$"Depth {depth}: next run recovers after failure");
 string shortPayloads=shortDocument.HistoryPayloadDirectory!;shortDocument.Dispose();shortScope.Dispose();
 Check(!Directory.Exists(shortPayloads)&&shortControl.Created==shortControl.Disposed&&shortControl.StatesCreated==shortControl.StatesDisposed,$"Depth {depth}: close releases all module/state/file history ownership");
}
File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=checks.Count,checks,observations,control.Created,control.Disposed,control.StatesCreated,control.StatesDisposed,scope="actual Core document, SolutionExecutor supported Group depths1/2/3; depth3 managed failure/cancel/disposal and owned file history composition; no WPF host, vendor SDK, ABI, device or native rollback claims"},new JsonSerializerOptions{WriteIndented=true}));
Console.WriteLine($"PASS {checks.Count} nested composition checks; audit={output}");
return 0;
}catch(Exception error){File.WriteAllText(Path.Combine(output,"failed.json"),JsonSerializer.Serialize(new{passed=false,failure=error.ToString(),scope="controlled managed nested composition; failed checks are retained"},new JsonSerializerOptions{WriteIndented=true}));Console.Error.WriteLine(error);Console.Error.WriteLine("Evidence: "+output);return 1;}

sealed class FixtureControl{
 public string Target="",Prefix="",Suffix="",Mode="ok";public int Created,Disposed,PrefixEffects,StatesCreated,StatesDisposed;public bool ExecutedAfterWait;
 public TaskCompletionSource Entered=new(TaskCreationOptions.RunContinuationsAsynchronously),Release=new(TaskCreationOptions.RunContinuationsAsynchronously);
 public void PrepareWait(){Mode="wait";ExecutedAfterWait=false;Entered=new(TaskCreationOptions.RunContinuationsAsynchronously);Release=new(TaskCreationOptions.RunContinuationsAsynchronously);}
}
sealed class Stage(ModuleDescriptor descriptor,FixtureControl control):IModule,IAsyncDisposable{
 readonly int instance=Interlocked.Increment(ref control.Created);string? node;bool isTarget;
 public ModuleDescriptor Descriptor=>descriptor;
 public async ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token){
  node=context.NodeId;isTarget=node==control.Target;
  context.Scope?.State(descriptor.Id,node,()=>{Interlocked.Increment(ref control.StatesCreated);return new State(()=>Interlocked.Increment(ref control.StatesDisposed));});
  if(!isTarget){if(node==control.Prefix)Interlocked.Increment(ref control.PrefixEffects);if(node==control.Suffix)control.ExecutedAfterWait=true;return new(ModuleStatus.Success,new Dictionary<string,PortValue>{{"Out",PortValue.Integer(instance)}});}
  if(control.Mode=="throw")throw new InvalidOperationException("owned-nested-stage-failure");
  if(control.Mode=="failed-result")return new(ModuleStatus.Failed,new Dictionary<string,PortValue>(),"OwnedFailure","controlled failed result");
  if(control.Mode=="wait"){control.Entered.TrySetResult();await control.Release.Task;}
  return new(ModuleStatus.Success,new Dictionary<string,PortValue>{{"Out",PortValue.Integer(instance)}});
 }
 public async ValueTask DisposeAsync(){await Task.Yield();Interlocked.Increment(ref control.Disposed);if(isTarget&&control.Mode=="dispose")throw new InvalidOperationException("owned-dispose-failure");}
}
sealed class State(Action release):IDisposable{bool disposed;public void Dispose(){if(disposed)throw new Exception("state double dispose");disposed=true;release();}}
