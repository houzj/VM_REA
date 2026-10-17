using VisionFramework;

namespace VisionPluginContracts;

public enum ConfigurationRunRejection { None, Busy, NotReady, Loading, AwaitingResult, ContinuousActive, Closed }
public sealed record ConfigurationRunAttempt(ConfigurationRunRejection Rejection,NodeRunRecord? Result=null);
public sealed record ConfigurationExecutionHostStatus(bool SolutionBusy,bool ProcedureBusy,bool Ready,bool Loading,bool RunEnabled);

/// <summary>
/// Ordinary module self-execution policy from BottomCommand. The execute task
/// represents result callback completion, not merely native Run() dispatch.
/// Stop ends scheduling; it does not abort a previously dispatched algorithm.
/// The host supplies the solution-scope cancellation and marshals UI services.
/// </summary>
public sealed class ToolConfigurationExecution : IAsyncDisposable {
 readonly Func<Task<NodeRunRecord>> execute;
 readonly Func<bool> busy,ready,loading,procedureBusy;
 readonly Func<TimeSpan,CancellationToken,Task> delay;
 CancellationTokenSource? polling;
 Task? loop,inFlight;
 bool closed,running;
 public bool IsRunning=>running;
 public bool IsContinuous=>polling is not null&&!polling.IsCancellationRequested;
 public NodeRunRecord? LastResult {get;private set;}
 public Exception? LastError {get;private set;}
 public event Action? Changed;
 public ToolConfigurationExecution(Func<Task<NodeRunRecord>> execute,Func<bool> busy,Func<bool> ready,Func<bool> loading,Func<bool>? procedureBusy=null,Func<TimeSpan,CancellationToken,Task>? delay=null){
  this.execute=execute;this.busy=busy;this.ready=ready;this.loading=loading;this.procedureBusy=procedureBusy??busy;this.delay=delay??((duration,token)=>Task.Delay(duration,token));
 }
 ConfigurationRunRejection Availability(bool startingContinuous=false){
  if(closed)return ConfigurationRunRejection.Closed;
  if(busy())return ConfigurationRunRejection.Busy;
  if(!ready())return ConfigurationRunRejection.NotReady;
  if(loading())return ConfigurationRunRejection.Loading;
  if(running)return ConfigurationRunRejection.AwaitingResult;
  if(IsContinuous&&!startingContinuous)return ConfigurationRunRejection.ContinuousActive;
  return ConfigurationRunRejection.None;
 }
 public async Task<ConfigurationRunAttempt> RunOnceAsync(){
  var rejection=Availability();if(rejection!=ConfigurationRunRejection.None)return new(rejection);
  inFlight=Dispatch();await inFlight;return new(ConfigurationRunRejection.None,LastResult);
 }
 public ConfigurationRunRejection StartContinuous(){
  if(IsContinuous)return ConfigurationRunRejection.ContinuousActive;
  var rejection=Availability(true);if(rejection!=ConfigurationRunRejection.None)return rejection;
  LastError=null;polling=new();loop=Poll(polling.Token);Notify();return ConfigurationRunRejection.None;
 }
 async Task Poll(CancellationToken token){
  try{
   while(!token.IsCancellationRequested){
    // Source has an additional 300ms wait when callback/procedure is busy,
    // followed by the outer loop's 300ms sleep. Do not turn this into 1ms retry.
    if(running||procedureBusy())await delay(TimeSpan.FromMilliseconds(300),token);
    else if(loading()){polling?.Cancel();break;}
    else inFlight=Dispatch();
    await delay(TimeSpan.FromMilliseconds(300),token);
   }
  }catch(OperationCanceledException)when(token.IsCancellationRequested){}
  catch(Exception error){LastError=error;polling?.Cancel();Notify();}
 }
 async Task Dispatch(){
  running=true;LastError=null;LastResult=null;Notify();
  try{LastResult=await execute();}
  catch(Exception error){LastError=error;polling?.Cancel();}
  finally{running=false;Notify();}
 }
 void Notify(){try{Changed?.Invoke();}catch{/* A view observer cannot corrupt the scheduler. */}}
 public async Task StopAsync(){
  var source=polling;if(source is null)return;source.Cancel();if(loop is not null)await loop;
  if(ReferenceEquals(polling,source)){polling=null;loop=null;}source.Dispose();Notify();
 }
 public async ValueTask DisposeAsync(){closed=true;await StopAsync();if(inFlight is not null)await inFlight;}
}
