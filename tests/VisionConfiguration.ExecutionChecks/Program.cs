using System.Threading.Channels;
using VisionFramework;
using VisionPluginContracts;

static class Program {
 static async Task<int> Main(){try{await Run();return 0;}catch(Exception error){Console.Error.WriteLine(error);return 1;}}
 static async Task Run(){
  int checks=0;void Check(bool ok,string message){checks++;if(!ok)throw new Exception(message);}
  NodeRunRecord Record(int n)=>new("node",NodeRunStatus.Success,new Dictionary<string,PortValue>{{"n",PortValue.Integer(n)}},1);
  bool busy=false,ready=true,loading=false;
  var queue=Channel.CreateUnbounded<TaskCompletionSource<NodeRunRecord>>();
  var waits=Channel.CreateUnbounded<(TimeSpan Duration,TaskCompletionSource Done)>();
  int calls=0;
  Task<NodeRunRecord> Execute(){calls++;var completion=new TaskCompletionSource<NodeRunRecord>(TaskCreationOptions.RunContinuationsAsynchronously);queue.Writer.TryWrite(completion);return completion.Task;}
  Task Delay(TimeSpan duration,CancellationToken token){var done=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);token.Register(()=>done.TrySetCanceled(token));waits.Writer.TryWrite((duration,done));return done.Task;}
  await using var service=new ToolConfigurationExecution(Execute,()=>busy,()=>ready,()=>loading,delay:Delay);
  busy=true;Check(service.StartContinuous()==ConfigurationRunRejection.Busy&&calls==0,"Global run excludes self execution");busy=false;
  ready=false;Check((await service.RunOnceAsync()).Rejection==ConfigurationRunRejection.NotReady&&calls==0,"Unready parameters prevent dispatch");ready=true;
  loading=true;Check((await service.RunOnceAsync()).Rejection==ConfigurationRunRejection.Loading&&calls==0,"Loading solution prevents dispatch");loading=false;
  var once=service.RunOnceAsync();var one=await queue.Reader.ReadAsync();
  Check((await service.RunOnceAsync()).Rejection==ConfigurationRunRejection.AwaitingResult&&calls==1,"Repeated once waits for result callback, never overlaps");
  one.SetResult(Record(1));Check((await once).Result?.Outputs["n"].As<long>()==1&&!service.IsRunning,"Callback completes once and releases gate");
  Check(service.StartContinuous()==ConfigurationRunRejection.None,"Continuous starts only after ready idle state");
  var two=await queue.Reader.ReadAsync();var wait1=await waits.Reader.ReadAsync();
  Check(calls==2&&wait1.Duration==TimeSpan.FromMilliseconds(300),"First continuous dispatch is immediate, followed by original 300ms sleep");
  wait1.Done.SetResult();var wait2=await waits.Reader.ReadAsync();
  Check(calls==2&&wait2.Duration==TimeSpan.FromMilliseconds(300),"Pending callback enters additional busy wait without another dispatch");
  wait2.Done.SetResult();var wait3=await waits.Reader.ReadAsync();
  Check(calls==2&&wait3.Duration==TimeSpan.FromMilliseconds(300),"Busy wait is followed by outer loop wait");
  var finished=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);service.Changed+=()=>{if(!service.IsRunning&&service.LastResult?.Outputs["n"].As<long>()==2)finished.TrySetResult();};
  two.SetResult(Record(2));await finished.Task;wait3.Done.SetResult();
  var three=await queue.Reader.ReadAsync();await waits.Reader.ReadAsync();
  Check(calls==3&&service.IsContinuous&&service.IsRunning,"Following callback next polling cycle dispatches another sample");
  await service.StopAsync();Check(!service.IsContinuous&&service.IsRunning&&!three.Task.IsCompleted,"Stop ends scheduling and preserves already dispatched operation");
  three.SetResult(Record(3));await service.DisposeAsync();Check(!service.IsRunning&&service.LastResult?.Outputs["n"].As<long>()==3,"Closing awaits owned outstanding callback without aborting it");
  Check((await service.RunOnceAsync()).Rejection==ConfigurationRunRejection.Closed,"Closed configuration cannot dispatch");
  await using var broken=new ToolConfigurationExecution(()=>throw new InvalidOperationException("controlled execution failure"),()=>false,()=>true,()=>false);
  await broken.RunOnceAsync();Check(broken.LastError?.Message=="controlled execution failure"&&broken.LastResult is null&&!broken.IsRunning,"Execution exceptions are captured with no stale result or unhandled dialog");
  Console.WriteLine($"{checks} configuration execution checks passed; source scheduling gates, no devices.");
 }
}
