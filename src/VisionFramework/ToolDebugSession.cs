using System.Text.Json;
namespace VisionFramework;

public enum ToolDebugState { Idle, RunningOnce, RunningContinuous, Stopping, Disposed }
public interface IToolDebugSession:IAsyncDisposable
{
 ToolDebugState State{get;}
 bool IsBusy{get;}
 Task<NodeRunRecord> RunOnceAsync(IReadOnlyDictionary<string,JsonElement> values,CancellationToken cancellationToken=default);
 Task RunContinuousAsync(Func<IReadOnlyDictionary<string,JsonElement>> values,Action<NodeRunRecord> completed,CancellationToken cancellationToken=default);
 Task StopAsync();
}
public sealed class ToolDebugSession:IToolDebugSession
{
 readonly object gate=new();readonly Func<IReadOnlyDictionary<string,JsonElement>,CancellationToken,Task<NodeRunRecord>> execute;readonly int intervalMs;
 Task? active;CancellationTokenSource? cancellation;bool closing;ToolDebugState state;
 public ToolDebugSession(Func<IReadOnlyDictionary<string,JsonElement>,CancellationToken,Task<NodeRunRecord>> execute,int intervalMs=300){this.execute=execute??throw new ArgumentNullException(nameof(execute));if(intervalMs<0)throw new ArgumentOutOfRangeException(nameof(intervalMs));this.intervalMs=intervalMs;}
 public ToolDebugState State{get{lock(gate)return state;}}
 public bool IsBusy{get{lock(gate)return active is {IsCompleted:false};}}
 static Dictionary<string,JsonElement> Snapshot(IReadOnlyDictionary<string,JsonElement> values)=>values.ToDictionary(p=>p.Key,p=>p.Value.Clone(),StringComparer.Ordinal);
 Task<T> Start<T>(ToolDebugState mode,CancellationToken token,Func<CancellationToken,Task<T>> action)
 {
  lock(gate){ObjectDisposedException.ThrowIf(closing,this);if(active is {IsCompleted:false})throw new InvalidOperationException("Debug session is already running.");state=mode;var owner=CancellationTokenSource.CreateLinkedTokenSource(token);cancellation=owner;var task=Execute();active=task;return task;
   async Task<T> Execute(){await Task.Yield();try{return await action(owner.Token);}finally{lock(gate){if(ReferenceEquals(cancellation,owner)){cancellation=null;state=closing?ToolDebugState.Disposed:ToolDebugState.Idle;}}owner.Dispose();}}
  }
 }
 public Task<NodeRunRecord> RunOnceAsync(IReadOnlyDictionary<string,JsonElement> values,CancellationToken cancellationToken=default){var snapshot=Snapshot(values);return Start(ToolDebugState.RunningOnce,cancellationToken,token=>execute(snapshot,token));}
 public async Task RunContinuousAsync(Func<IReadOnlyDictionary<string,JsonElement>> values,Action<NodeRunRecord> completed,CancellationToken cancellationToken=default)
 {
  ArgumentNullException.ThrowIfNull(values);ArgumentNullException.ThrowIfNull(completed);
  await Start(ToolDebugState.RunningContinuous,cancellationToken,async token=>{while(true){token.ThrowIfCancellationRequested();var record=await execute(Snapshot(values()),token);token.ThrowIfCancellationRequested();completed(record);if(record.Status is NodeRunStatus.Failed or NodeRunStatus.TimedOut or NodeRunStatus.Cancelled)break;if(intervalMs>0)await Task.Delay(intervalMs,token);else await Task.Yield();}return true;});
 }
 public async Task StopAsync(){Task? wait;lock(gate){wait=active;if(wait is {IsCompleted:false}){state=ToolDebugState.Stopping;cancellation?.Cancel();}}if(wait is not null){try{await wait;}catch(OperationCanceledException){}}}
 public async ValueTask DisposeAsync(){lock(gate){closing=true;if(active is not {IsCompleted:false})state=ToolDebugState.Disposed;}try{await StopAsync();}finally{lock(gate)state=ToolDebugState.Disposed;}}
}
