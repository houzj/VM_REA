namespace VisionFramework;

public enum ExecutionMode { SelfRun, Continuous }
public sealed record ModuleNodeRequest(Guid ScopeId,string ProcedureId,string NodeId);
public sealed record ModuleScopeCleanupFailure(string Phase,string ResourceType,Exception Error);

/// <summary>Owns persistent node state and services for one opened solution.</summary>
public sealed class ModuleExecutionScope:IDisposable {
 readonly object gate=new();
 readonly Dictionary<(string Procedure,string Module,string Node),object> states=new();
 readonly ModuleExecutionScope? owner;
 public string ProcedureId {get;}="";
 public ModuleExecutionScope(){}
 ModuleExecutionScope(ModuleExecutionScope root,string procedure){owner=root;ProcedureId=procedure;}
 ModuleExecutionScope Root=>owner??this;
 /// <summary>Borrowed procedure-bound view. Dispose does not close the owning solution.</summary>
 public ModuleExecutionScope ForProcedure(string procedure){ArgumentException.ThrowIfNullOrWhiteSpace(procedure);return ProcedureId==procedure?this:new(Root,procedure);}
 readonly Dictionary<Type,object> services=new();
 readonly HashSet<object> ownedServices=new(ReferenceEqualityComparer.Instance);
 readonly CancellationTokenSource stopping=new();
 readonly List<ModuleScopeCleanupFailure> cleanupFailures=new();
 readonly List<object> failedResources=new();
 public IReadOnlyList<ModuleScopeCleanupFailure> CleanupFailures {get{var root=Root;lock(root.gate)return root.cleanupFailures.ToArray();}}
 void RecordCleanupFailure(string phase,object resource,Exception error){lock(gate){cleanupFailures.Add(new(phase,resource.GetType().FullName??resource.GetType().Name,error));failedResources.Add(resource);}}
 bool closing,cancellationComplete,releaseStarted;int leases;
 readonly Guid id=Guid.NewGuid();
 public Guid Id=>Root.id;
 public CancellationToken Stopping=>Root.stopping.Token;
 public event Action<ModuleNodeRequest>? ProcedureRunRequested;
 public event Action<ModuleNodeRequest>? ProcedureRefreshRequested;
 public event Action<string>? RunRequested;
 public event Action<string>? RefreshRequested;
 public T State<T>(string module,string node,Func<T> create)where T:class {
  var root=Root;lock(root.gate){ObjectDisposedException.ThrowIf(root.closing,this);var key=(ProcedureId,module,node);if(!root.states.TryGetValue(key,out var value))root.states.Add(key,value=create());return (T)value;}
 }
 public void RegisterService<T>(T service,bool owned=true)where T:class{if(owner is not null){owner.RegisterService(service,owned);return;}ArgumentNullException.ThrowIfNull(service);lock(gate){ObjectDisposedException.ThrowIf(closing,this);if(services.TryGetValue(typeof(T),out var previous)&&!ReferenceEquals(previous,service)&&ownedServices.Remove(previous)&&previous is IDisposable disposable)disposable.Dispose();services[typeof(T)]=service;if(owned)ownedServices.Add(service);}}
 public T? Service<T>()where T:class{if(owner is not null)return owner.Service<T>();lock(gate)return services.TryGetValue(typeof(T),out var value)?(T)value:null;}
 public void RequestRun(string node){var root=Root;if(root.closing)return;root.ProcedureRunRequested?.Invoke(new(root.Id,ProcedureId,node));if(ProcedureId.Length==0)root.RunRequested?.Invoke(node);}
 public void RequestRefresh(string node){var root=Root;if(root.closing)return;root.ProcedureRefreshRequested?.Invoke(new(root.Id,ProcedureId,node));if(ProcedureId.Length==0)root.RefreshRequested?.Invoke(node);}
 public IDisposable Enter(){if(owner is not null)return owner.Enter();lock(gate){ObjectDisposedException.ThrowIf(closing,this);leases++;return new Lease(this);}}
 public void RemoveNode(string module,string node){var root=Root;object? value;lock(root.gate){if(!root.states.Remove((ProcedureId,module,node),out value))return;}if(value is IDisposable disposable)disposable.Dispose();}
 public void RetainNodes(IEnumerable<(string Module,string Node)> current){var keep=current.Select(k=>(ProcedureId,k.Module,k.Node));RetainProcedureNodes(keep,ProcedureId);}
 public void RetainProcedureNodes(IEnumerable<(string Procedure,string Module,string Node)> current)=>RetainProcedureNodes(current,null);
 void RetainProcedureNodes(IEnumerable<(string Procedure,string Module,string Node)> current,string? only){var root=Root;var keep=current.ToHashSet();(string Procedure,string Module,string Node)[] removed;lock(root.gate)removed=root.states.Keys.Where(k=>(only is null||k.Procedure==only)&&!keep.Contains(k)).ToArray();foreach(var key in removed){if(key.Procedure.Length==0)root.RemoveNode(key.Module,key.Node);else root.ForProcedure(key.Procedure).RemoveNode(key.Module,key.Node);}}
 public void Dispose(){if(owner is not null)return;lock(gate){if(closing)return;closing=true;}try{stopping.Cancel();}catch(Exception error){RecordCleanupFailure("cancel",stopping,error);}finally{bool release;lock(gate){cancellationComplete=true;release=TryRelease();}if(release)Release();}}
 bool TryRelease(){if(!closing||!cancellationComplete||leases!=0||releaseStarted)return false;releaseStarted=true;return true;}
 void Exit(){bool release;lock(gate){leases--;release=TryRelease();}if(release)Release();}
 void Release(){object[] values;lock(gate){values=states.Values.Concat(ownedServices).Distinct(ReferenceEqualityComparer.Instance).ToArray();states.Clear();services.Clear();ownedServices.Clear();}foreach(var value in values)if(value is IDisposable disposable)try{disposable.Dispose();}catch(Exception error){RecordCleanupFailure("resource",value,error);}try{stopping.Dispose();}catch(Exception error){RecordCleanupFailure("token",stopping,error);}}
 sealed class Lease(ModuleExecutionScope scope):IDisposable {ModuleExecutionScope? owner=scope;public void Dispose()=>Interlocked.Exchange(ref owner,null)?.Exit();}
}

public sealed class ModuleRunControl {
 int stopped;
 public ExecutionMode Mode {get;init;}
 public bool StopRequested=>Volatile.Read(ref stopped)!=0;
 public void RequestStop()=>Interlocked.Exchange(ref stopped,1);
}
