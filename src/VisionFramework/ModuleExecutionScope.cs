namespace VisionFramework;

public enum ExecutionMode { SelfRun, Continuous }

/// <summary>Owns persistent node state and services for one opened solution.</summary>
public sealed class ModuleExecutionScope:IDisposable {
 readonly object gate=new();
 readonly Dictionary<(string Module,string Node),object> states=new();
 readonly Dictionary<Type,object> services=new();
 readonly HashSet<object> ownedServices=new(ReferenceEqualityComparer.Instance);
 readonly CancellationTokenSource stopping=new();
 bool closing,cancellationComplete,releaseStarted;int leases;
 public Guid Id {get;}=Guid.NewGuid();
 public CancellationToken Stopping=>stopping.Token;
 public event Action<string>? RunRequested;
 public event Action<string>? RefreshRequested;
 public T State<T>(string module,string node,Func<T> create)where T:class {
  lock(gate){ObjectDisposedException.ThrowIf(closing,this);var key=(module,node);if(!states.TryGetValue(key,out var value))states.Add(key,value=create());return (T)value;}
 }
 public void RegisterService<T>(T service,bool owned=true)where T:class{ArgumentNullException.ThrowIfNull(service);lock(gate){ObjectDisposedException.ThrowIf(closing,this);if(services.TryGetValue(typeof(T),out var previous)&&!ReferenceEquals(previous,service)&&ownedServices.Remove(previous)&&previous is IDisposable disposable)disposable.Dispose();services[typeof(T)]=service;if(owned)ownedServices.Add(service);}}
 public T? Service<T>()where T:class{lock(gate)return services.TryGetValue(typeof(T),out var value)?(T)value:null;}
 public void RequestRun(string node)=>RunRequested?.Invoke(node);
 public void RequestRefresh(string node)=>RefreshRequested?.Invoke(node);
 public IDisposable Enter(){lock(gate){ObjectDisposedException.ThrowIf(closing,this);leases++;return new Lease(this);}}
 public void RemoveNode(string module,string node){object? value;lock(gate){if(!states.Remove((module,node),out value))return;}if(value is IDisposable disposable)disposable.Dispose();}
 public void RetainNodes(IEnumerable<(string Module,string Node)> current){var keep=current.ToHashSet();(string Module,string Node)[] removed;lock(gate)removed=states.Keys.Where(k=>!keep.Contains(k)).ToArray();foreach(var key in removed)RemoveNode(key.Module,key.Node);}
 public void Dispose(){lock(gate){if(closing)return;closing=true;}try{stopping.Cancel();}finally{bool release;lock(gate){cancellationComplete=true;release=TryRelease();}if(release)Release();}}
 bool TryRelease(){if(!closing||!cancellationComplete||leases!=0||releaseStarted)return false;releaseStarted=true;return true;}
 void Exit(){bool release;lock(gate){leases--;release=TryRelease();}if(release)Release();}
 void Release(){object[] values;lock(gate){values=states.Values.Concat(ownedServices).Distinct(ReferenceEqualityComparer.Instance).ToArray();states.Clear();services.Clear();ownedServices.Clear();}foreach(var value in values)if(value is IDisposable disposable)disposable.Dispose();stopping.Dispose();}
 sealed class Lease(ModuleExecutionScope scope):IDisposable {ModuleExecutionScope? owner=scope;public void Dispose()=>Interlocked.Exchange(ref owner,null)?.Exit();}
}

public sealed class ModuleRunControl {
 int stopped;
 public ExecutionMode Mode {get;init;}
 public bool StopRequested=>Volatile.Read(ref stopped)!=0;
 public void RequestStop()=>Interlocked.Exchange(ref stopped,1);
}
