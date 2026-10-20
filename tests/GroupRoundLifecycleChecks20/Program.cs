using System.Text.Json;
using VisionEditor.Core;
using VisionFramework;

if(args.Length!=1)throw new ArgumentException("Supply owned audit parent.");
string output=Path.Combine(Path.GetFullPath(args[0]),"group-rounds-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
try{
 var checks=new List<string>();var observations=new List<object>();
 void Check(bool ok,string label){if(!ok)throw new Exception(label);checks.Add(label);Console.WriteLine("PASS "+label);}
 async Task<ExecutionSessionResult> Settle(Task<ExecutionSessionResult> task)=>await task.WaitAsync(TimeSpan.FromSeconds(15));
 foreach(int depth in new[]{1,2,3}){
  using var graph=Graph.Build(depth,true,1,6,2);var c=graph.Control;
  var run=await Settle(graph.Run());
  Check(run.Status==SessionStatus.Success&&c.Indices.SequenceEqual(new[]{1,3,5}),$"Depth {depth}: actual positive-step loop executes exactly three rounds");
  Check(c.Created==3&&c.Disposed==3&&c.Instances.Distinct().Count()==3,$"Depth {depth}: each round receives a new actual factory instance");
  Check(c.MaxLive==1&&c.Live==0&&c.NoOverlap,$"Depth {depth}: previous asynchronous disposal completes before next factory creation");
  Check(c.Refreshes.Count==3&&c.Refreshes.All(r=>r.ScopeId==graph.Scope.Id&&r.ProcedureId==graph.Body&&r.NodeId==graph.Node),$"Depth {depth}: real refresh requests preserve scope/procedure/node ownership each round");
  Check(c.StatesCreated==1&&c.StateUses==3,$"Depth {depth}: persistent node state is owner-scoped while runtime instances remain per-round");
  Check(c.RootEvents.First().Status==NodeRunStatus.Running&&c.RootEvents.Last().Status==NodeRunStatus.Success&&c.Trace[^1].EndsWith(":Success",StringComparison.Ordinal),$"Depth {depth}: root progress completes only after nested round lifecycle returns");
  observations.Add(new{kind="positive",depth,trace=c.Trace.ToArray()});
 }
 using(var once=Graph.Build(1,false,77,88,int.MinValue)){
  Check((await Settle(once.Run())).Status==SessionStatus.Success&&once.Control.Indices.SequenceEqual(new[]{0}),"Disabled looping with saved negative step creates and disposes one instance at index zero");
  Check(once.Control.Created==1&&once.Control.Disposed==1&&once.Control.Refreshes.Count==1,"One-shot invocation has exactly one managed callback and one disposal");
 }
 using(var empty=Graph.Build(1,true,5,2,-1)){
  var run=await Settle(empty.Run());Check(run.Status==SessionStatus.Success&&empty.Control.Created==0&&empty.Control.Refreshes.Count==0,"Empty source-guided descending range creates no runtime instance or callback");
 }
 using(var negative=Graph.Build(2,true,0,3,-1))using(var cancel=new CancellationTokenSource()){
  negative.Control.Cancel=cancel;negative.Control.CancelIndex=-2;
  var run=await Settle(negative.Run(cancel.Token));
  Check(run.Status==SessionStatus.Cancelled&&negative.Control.Indices.SequenceEqual(new[]{0,-1,-2}),"Accepted negative step is cooperatively cancelled without inventing a descending termination rule");
  Check(negative.Control.Created==3&&negative.Control.Disposed==3&&negative.Control.Live==0,"Negative-step cancellation disposes every started round and starts no further round");
 }
 foreach(string failure in new[]{"throw","failed-result","dispose","callback"}){
  using var graph=Graph.Build(3,true,0,5,1);var c=graph.Control;c.Mode=failure;c.FailureIndex=2;
  var run=await Settle(graph.Run());
  Check(run.Status==SessionStatus.Failed&&c.Indices.SequenceEqual(new[]{0,1,2}),failure+": failure in third round prevents fourth factory creation across three Group levels");
  Check(c.Created==3&&c.Disposed==3&&c.Live==0&&c.NoOverlap,failure+": started failed round follows one disposal and no overlap");
  Check(c.Refreshes.Count==3,failure+": only started rounds dispatch real managed refresh callbacks");
  c.Mode="ok";int before=c.Created;var recovered=await Settle(graph.Run());
  Check(recovered.Status==SessionStatus.Success&&c.Created-before==5&&c.Created==c.Disposed,failure+": next full invocation has fresh factory instances and no stale failure");
  Check(c.StatesCreated==1&&c.StateUses==8,failure+": owner state persists across invocations without masquerading as reused runtime instance");
  observations.Add(new{kind=failure,trace=c.Trace.ToArray()});
 }
 using(var blocked=Graph.Build(2,true,0,4,1)){
  var c=blocked.Control;c.PauseDisposeIndex=0;var task=blocked.Run();await c.DisposeEntered.Task.WaitAsync(TimeSpan.FromSeconds(15));
  Check(c.Created==1&&c.Live==1&&!task.IsCompleted,"First round asynchronous disposal is actually paused before invocation completes");
  await Task.Yield();Check(c.Created==1&&c.Refreshes.Count==1,"No second instance or callback begins while previous disposal is paused");
  c.DisposeRelease.TrySetResult();Check((await Settle(task)).Status==SessionStatus.Success&&c.Created==4&&c.Disposed==4&&c.NoOverlap,"Releasing asynchronous disposal permits remaining rounds in strict order");
 }
 using(var cancelled=Graph.Build(3,true,0,5,1))using(var cancel=new CancellationTokenSource()){
  var c=cancelled.Control;c.PauseExecuteIndex=1;var task=cancelled.Run(cancel.Token);await c.ExecuteEntered.Task.WaitAsync(TimeSpan.FromSeconds(15));cancel.Cancel();
  Check(c.Created==2&&!task.IsCompleted,"Execution cancellation occurs during actual second-round await");
  c.ExecuteRelease.TrySetResult();Check((await Settle(task)).Status==SessionStatus.Cancelled&&c.Created==2&&c.Disposed==2,"Mid-execute cancellation waits cleanup and prevents third instance");
  c.PauseExecuteIndex=null;Check((await Settle(cancelled.Run())).Status==SessionStatus.Success&&c.Created==7&&c.Disposed==7,"Fresh invocation runs all rounds after execution cancellation");
 }
 using(var cancelled=Graph.Build(1,true,0,4,1))using(var cancel=new CancellationTokenSource()){
  var c=cancelled.Control;c.PauseDisposeIndex=1;var task=cancelled.Run(cancel.Token);await c.DisposeEntered.Task.WaitAsync(TimeSpan.FromSeconds(15));cancel.Cancel();
  Check(c.Created==2&&!task.IsCompleted,"Cancellation during second-round cleanup cannot skip its pending disposal");
  c.DisposeRelease.TrySetResult();Check((await Settle(task)).Status==SessionStatus.Cancelled&&c.Created==2&&c.Disposed==2,"Cleanup cancellation waits release then suppresses subsequent rounds");
 }
 using(var closing=Graph.Build(3,true,0,5,1)){
  var c=closing.Control;c.PauseExecuteIndex=1;var task=closing.Run();await c.ExecuteEntered.Task.WaitAsync(TimeSpan.FromSeconds(15));int callbacks=c.Refreshes.Count;
  closing.Scope.Dispose();Check(c.StatesDisposed==0&&c.Live==1,"Scope close preserves owned node state while nested execution lease is active");
  closing.Scope.ForProcedure(closing.Body).RequestRefresh(closing.Node);Check(c.Refreshes.Count==callbacks,"Closed scope suppresses new old-owner refresh requests");
  c.ExecuteRelease.TrySetResult();Check((await Settle(task)).Status==SessionStatus.Cancelled&&c.Created==2&&c.Disposed==2,"Scope close cancels later rounds after current instance cleanup");
  Check(c.StatesDisposed==1,"Final nested lease releases persistent node state exactly once");
 }
 using(var first=Graph.Build(1,true,0,2,1))using(var second=Graph.Build(1,true,0,2,1)){
  string reused=Path.Combine(output,"same-identities.vmx.json");first.Document.Save(reused);second.Document.Load(reused);second.Root=first.Root;second.Body=first.Body;second.Node=first.Node;
  await Settle(first.Run());await Settle(second.Run());
  Check(first.Document.SolutionId==second.Document.SolutionId&&first.Node==second.Node&&first.Scope.Id!=second.Scope.Id&&first.Control.StateIdentity!=second.Control.StateIdentity,"Separate scopes with identical document/procedure/node IDs never share persistent round state identity");
  first.Scope.Dispose();Check(first.Control.StatesDisposed==1&&second.Control.StatesDisposed==0,"Closing first owner does not release second owner's round state");
 }
 File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=checks.Count,checks,observations,scope="actual managed registry factories, SolutionExecutor Group rounds, GraphExecutor async disposal, scope request callbacks/progress and persistent owner state; controlled modules, not original SDK callbacks/device execution"},new JsonSerializerOptions{WriteIndented=true}));
 Console.WriteLine($"PASS {checks.Count} round lifecycle checks; audit={output}");return 0;
}catch(Exception error){File.WriteAllText(Path.Combine(output,"failed.json"),JsonSerializer.Serialize(new{passed=false,failure=error.ToString()},new JsonSerializerOptions{WriteIndented=true}));Console.Error.WriteLine(error);Console.Error.WriteLine("Evidence: "+output);return 1;}

sealed class Graph:IDisposable{
 public required GraphEditorDocument Document;public required ModuleExecutionScope Scope;public required Control Control;public required ModuleRegistry Registry;public required string Root,Body,Node;
 public Task<ExecutionSessionResult> Run(CancellationToken token=default)=>new SolutionExecutor(Registry).RunAsync(new(Document.Document,Document.BaseDirectory),Root,new ProgressSink(Control),token,scope:Scope);
 public static Graph Build(int depth,bool enabled,int start,int end,int step){
  var c=new Control();var registry=new ModuleRegistry();WorkflowCalls.Register(registry);var descriptor=new ModuleDescriptor("test.round-lifecycle","1","round",[new("Index",DataKind.Integer)],[new("Value",DataKind.Integer)],[]);registry.Register(descriptor,()=>new RoundModule(descriptor,c));
  var doc=new GraphEditorDocument(registry);string root=doc.CurrentProcedureId;
  for(int layer=0;layer<depth;layer++){
   var values=new Dictionary<string,JsonElement>{{"EnableLoop",JsonSerializer.SerializeToElement(layer==depth-1&&enabled)},{"StartIndex",JsonSerializer.SerializeToElement(start)},{"EndIndex",JsonSerializer.SerializeToElement(end)},{"LoopStep",JsonSerializer.SerializeToElement(step)}};
   string group=doc.AddOwnedGroup(100,100,values);doc.EnterOwnedGroup(group);
  }
  string body=doc.CurrentProcedureId,node=doc.AddNode(descriptor.Id,"1",100,100);doc.SetWorkflowConfiguration(body,new(){Subscriptions=[new(node,"Index",false,new(WorkflowSourceKind.GroupLoopIndex,"LoopIndex"))]});doc.SelectProcedure(root);
  var scope=new ModuleExecutionScope();scope.ProcedureRefreshRequested+=request=>{c.Refreshes.Add(request);if(c.Mode=="callback"&&c.LastIndex==c.FailureIndex)throw new InvalidOperationException("controlled managed refresh consumer failure");};
  return new(){Document=doc,Scope=scope,Control=c,Registry=registry,Root=root,Body=body,Node=node};
 }
 public void Dispose(){Document.Dispose();Scope.Dispose();}
}
sealed class ProgressSink(Control control):IProgress<ExecutionEvent>{public void Report(ExecutionEvent value){control.RootEvents.Add(value);control.Trace.Add("progress:"+value.NodeId+":"+value.Status);}}
sealed class Control{
 public int Created,Disposed,Live,MaxLive,StatesCreated,StatesDisposed,StateUses;public bool NoOverlap=true;public Guid StateIdentity;
 public string Mode="ok";public int FailureIndex=2,LastIndex;public int? CancelIndex,PauseExecuteIndex,PauseDisposeIndex;public CancellationTokenSource? Cancel;
 public List<int> Indices=new(),Instances=new();public List<string> Trace=new();public List<ModuleNodeRequest> Refreshes=new();public List<ExecutionEvent> RootEvents=new();
 public TaskCompletionSource ExecuteEntered=new(TaskCreationOptions.RunContinuationsAsynchronously),ExecuteRelease=new(TaskCreationOptions.RunContinuationsAsynchronously),DisposeEntered=new(TaskCreationOptions.RunContinuationsAsynchronously),DisposeRelease=new(TaskCreationOptions.RunContinuationsAsynchronously);
}
sealed class RoundModule:IModule,IAsyncDisposable{
 readonly Control c;readonly int instance;int index;public ModuleDescriptor Descriptor{get;}
 public RoundModule(ModuleDescriptor descriptor,Control control){Descriptor=descriptor;c=control;instance=++c.Created;if(c.Live!=0)c.NoOverlap=false;c.Live++;c.MaxLive=Math.Max(c.MaxLive,c.Live);c.Instances.Add(instance);c.Trace.Add($"create:{instance}");}
 public async ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token){
  index=(int)context.Inputs["Index"].As<long>();c.LastIndex=index;c.Indices.Add(index);c.Trace.Add($"execute:{instance}:{index}");
  var state=context.Scope!.State(Descriptor.Id,context.NodeId,()=>{c.StatesCreated++;return new Persistent(c);});state.Use();c.StateIdentity=state.Id;
  context.Scope.RequestRefresh(context.NodeId);c.Trace.Add($"callback:{instance}:{index}");if(c.CancelIndex==index)c.Cancel?.Cancel();
  if(c.PauseExecuteIndex==index){c.ExecuteEntered.TrySetResult();await c.ExecuteRelease.Task;}
  if(c.Mode=="throw"&&index==c.FailureIndex)throw new InvalidOperationException("controlled round failure");
  if(c.Mode=="failed-result"&&index==c.FailureIndex)return new(ModuleStatus.Failed,new Dictionary<string,PortValue>(),"RoundFailure","controlled failed round");
  return new(ModuleStatus.Success,new Dictionary<string,PortValue>{{"Value",PortValue.Integer(index)}});
 }
 public async ValueTask DisposeAsync(){c.Trace.Add($"dispose-start:{instance}:{index}");if(c.PauseDisposeIndex==index){c.DisposeEntered.TrySetResult();await c.DisposeRelease.Task;}await Task.Yield();c.Disposed++;c.Live--;c.Trace.Add($"dispose-end:{instance}:{index}");if(c.Mode=="dispose"&&index==c.FailureIndex)throw new InvalidOperationException("controlled round disposal failure");}
}
sealed class Persistent(Control c):IDisposable{bool disposed;public Guid Id{get;}=Guid.NewGuid();public void Use(){if(disposed)throw new InvalidOperationException("disposed round state reused");c.StateUses++;}public void Dispose(){if(disposed)throw new InvalidOperationException("round state double disposal");disposed=true;c.StatesDisposed++;}}
