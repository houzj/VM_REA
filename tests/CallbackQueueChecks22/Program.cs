using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using VisionFramework;
string? output=null;
try{
 if(args.Length!=1)throw new ArgumentException("Supply owned output parent.");output=Path.Combine(Path.GetFullPath(args[0]),"callback-queue-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
 var checks=new List<string>();void Check(bool value,string label){if(!value)throw new Exception(label);checks.Add(label);Console.WriteLine("PASS "+label);}
 void Until(Func<bool> condition){var watch=Stopwatch.StartNew();while(!condition()&&watch.Elapsed<TimeSpan.FromSeconds(10))Thread.Sleep(1);if(!condition())throw new TimeoutException("Owned queue wait deadline.");}
 var sequence=new List<string>();int publisher=Environment.CurrentManagedThreadId;int consumer=0;
 using(var direct=new FrameworkCallbackQueue<string>(x=>{sequence.Add(x);consumer=Environment.CurrentManagedThreadId;})){
  Check(direct.Capacity==10000&&!direct.CacheEnabled,"Default capacity 10000 and cache false");
  direct.PublishWorkStatus("async-work",()=>sequence.Add("sync-work"));Check(sequence.SequenceEqual(["async-work","sync-work"])&&consumer==publisher,"Uncached work status dispatch precedes sync result on same thread");
  sequence.Clear();direct.PublishModuleResult("async-module",()=>sequence.Add("sync-module"));Check(sequence.SequenceEqual(["sync-module","async-module"]),"Uncached module sync result precedes async dispatch");
  sequence.Clear();direct.PublishWorkStatus("unexpected",()=>sequence.Add("sync-zero"),false);Check(sequence.SequenceEqual(["sync-zero"]),"Zero count omits queued work callback but retains sync result");
  direct.Close();direct.Enqueue("after-close");Check(sequence.Last()=="after-close","Close does not forbid uncached publisher dispatch");
 }
 using(var entered=new ManualResetEventSlim())using(var release=new ManualResetEventSlim()){
  int count=0;var received=new ConcurrentQueue<int>();
  using var buffered=new FrameworkCallbackQueue<int>(x=>{if(x==-1){entered.Set();if(!release.Wait(TimeSpan.FromSeconds(10)))throw new TimeoutException();}else{received.Enqueue(x);Interlocked.Increment(ref count);}});
  buffered.CacheEnabled=true;buffered.Enqueue(-1);Check(entered.Wait(TimeSpan.FromSeconds(10)),"Own worker entered controlled active send batch");
  for(int i=0;i<10000;i++)CheckEnqueue(buffered.Enqueue(i));
  Check(buffered.PendingCount==10000&&buffered.Enqueue(10000)==CallbackEnqueueResult.Overflow&&buffered.DroppedCount==1,"Receive capacity remains 10000 during active send and overflow drops current item");
  release.Set();Until(()=>Volatile.Read(ref count)==10000);Check(received.SequenceEqual(Enumerable.Range(0,10000)),"Receive batch preserves exact FIFO order without dropped item");
 }
 using(var firstEntered=new ManualResetEventSlim())using(var firstRelease=new ManualResetEventSlim())using(var secondEntered=new ManualResetEventSlim())using(var secondRelease=new ManualResetEventSlim()){
  var received=new ConcurrentQueue<int>();using var q=new FrameworkCallbackQueue<int>(x=>{received.Enqueue(x);if(x==0){firstEntered.Set();if(!firstRelease.Wait(TimeSpan.FromSeconds(10)))throw new TimeoutException();}if(x==1){secondEntered.Set();if(!secondRelease.Wait(TimeSpan.FromSeconds(10)))throw new TimeoutException();}});q.CacheEnabled=true;q.Enqueue(0);Check(firstEntered.Wait(TimeSpan.FromSeconds(10)),"First batch blocker entered");q.Enqueue(1);q.Enqueue(2);firstRelease.Set();Check(secondEntered.Wait(TimeSpan.FromSeconds(10)),"Second batch has been swapped before close");q.Enqueue(3);q.Close();secondRelease.Set();Until(()=>received.Contains(2));Thread.Sleep(20);Check(received.SequenceEqual([0,1,2])&&q.PendingCount==1,"Close finishes already swapped batch and does not drain waiting receive");Check(q.Enqueue(4)==CallbackEnqueueResult.Queued&&q.PendingCount==2,"Original-style cached enqueue remains accepted after shutdown without restart");
 }
 using(var expanded=new FrameworkCallbackQueue<int>(_=>{})){expanded.Close();expanded.CacheEnabled=true;expanded.Enqueue(1);expanded.ExpandCapacity(9999);Check(expanded.Capacity==10000&&expanded.PendingCount==1,"Smaller expand is a no-op");expanded.ExpandCapacity(10001);Check(expanded.Capacity==10001&&expanded.PendingCount==0,"Larger expand resets pending receive");}
 using(var fault=new FrameworkCallbackQueue<int>(_=>throw new InvalidOperationException("owned subscriber failure"))){fault.CacheEnabled=true;fault.Enqueue(1);Until(()=>fault.DispatchErrors.Count==1);Check(fault.IsClosed&&fault.DispatchErrors[0].Message=="owned subscriber failure","Independent safety adapter retains worker subscriber fault and stops worker");}
 File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=checks.Count,checks},new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine(output);return 0;
}catch(Exception error){if(output is not null)File.WriteAllText(Path.Combine(output,"failed.json"),JsonSerializer.Serialize(new{error=error.ToString()},new JsonSerializerOptions{WriteIndented=true}));Console.Error.WriteLine(error);return 1;}
static void CheckEnqueue(CallbackEnqueueResult result){if(result!=CallbackEnqueueResult.Queued)throw new Exception("Unexpected early receive overflow.");}
