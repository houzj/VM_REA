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
 Task? active;
 Task? stopTask;
 CancellationTokenSource? cancellation;
 bool closing;
 ToolDebugState state;
 public ToolDebugSession(Func<IReadOnlyDictionary<string,JsonElement>,CancellationToken,Task<NodeRunRecord>> execute,int intervalMs=300){this.execute=execute??throw new ArgumentNullException(nameof(execute));if(intervalMs<0)throw new ArgumentOutOfRangeException(nameof(intervalMs));this.intervalMs=intervalMs;}
 public ToolDebugState State{get{lock(gate)return state;}}
 public bool IsBusy{get{lock(gate)return HasPendingWork;}}
 bool HasPendingWork => active is { IsCompleted: false } || stopTask is { IsCompleted: false };
 static Dictionary<string,JsonElement> Snapshot(IReadOnlyDictionary<string,JsonElement> values)=>values.ToDictionary(p=>p.Key,p=>p.Value.Clone(),StringComparer.Ordinal);
 Task<T> Start<T>(ToolDebugState mode,CancellationToken token,Func<CancellationToken,Task<T>> action)
 {
  lock (gate)
  {
   ObjectDisposedException.ThrowIf(closing, this);
   if (HasPendingWork)
    throw new InvalidOperationException("Debug session is already running.");
   state = mode;
   stopTask = null;
   var owner = CancellationTokenSource.CreateLinkedTokenSource(token);
   cancellation = owner;
   var task = Execute();
   active = task;
   return task;

   async Task<T> Execute()
   {
    await Task.Yield();
    try { return await action(owner.Token); }
    finally
    {
     bool disposeOwner;
     lock (gate)
     {
      cancellation = null;
      disposeOwner = stopTask is null;
      if (disposeOwner)
       state = closing ? ToolDebugState.Disposed : ToolDebugState.Idle;
     }
     // An explicit stop owns the source until both the run and cancellation callbacks finish.
     if (disposeOwner)
      owner.Dispose();
    }
   }
  }
 }
 public Task<NodeRunRecord> RunOnceAsync(IReadOnlyDictionary<string,JsonElement> values,CancellationToken cancellationToken=default){var snapshot=Snapshot(values);return Start(ToolDebugState.RunningOnce,cancellationToken,token=>execute(snapshot,token));}
 public async Task RunContinuousAsync(Func<IReadOnlyDictionary<string,JsonElement>> values,Action<NodeRunRecord> completed,CancellationToken cancellationToken=default)
 {
  ArgumentNullException.ThrowIfNull(values);ArgumentNullException.ThrowIfNull(completed);
  await Start(ToolDebugState.RunningContinuous,cancellationToken,async token=>{while(true){token.ThrowIfCancellationRequested();var record=await execute(Snapshot(values()),token);token.ThrowIfCancellationRequested();completed(record);if(record.Status is NodeRunStatus.Failed or NodeRunStatus.TimedOut or NodeRunStatus.Cancelled)break;if(intervalMs>0)await Task.Delay(intervalMs,token);else await Task.Yield();}return true;});
 }
 public Task StopAsync()
 {
  lock (gate)
  {
   if (stopTask is { IsCompleted: false })
    return stopTask;
   if (active is { IsCompleted: false } run && cancellation is { } owner)
   {
    state = ToolDebugState.Stopping;
    stopTask = DrainStopAsync(run, owner, owner.CancelAsync());
    return stopTask;
   }
   return active is { } previousRun ? WaitForRunAsync(previousRun) : Task.CompletedTask;
  }
 }

 async Task DrainStopAsync(Task run, CancellationTokenSource owner, Task callbacks)
 {
  try { await Task.WhenAll(WaitForRunAsync(run), callbacks); }
  finally
  {
   owner.Dispose();
   lock (gate)
    state = closing ? ToolDebugState.Disposed : ToolDebugState.Idle;
  }
 }

 static async Task WaitForRunAsync(Task run)
 {
  try { await run; }
  catch (OperationCanceledException) { }
 }

 public async ValueTask DisposeAsync()
 {
  lock (gate)
  {
   closing = true;
   if (!HasPendingWork)
    state = ToolDebugState.Disposed;
  }
  try { await StopAsync(); }
  finally { lock (gate) state = ToolDebugState.Disposed; }
 }
}
