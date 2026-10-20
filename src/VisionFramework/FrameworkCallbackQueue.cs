namespace VisionFramework;

public enum CallbackEnqueueResult { Dispatched, Queued, Overflow }

/// <summary>
/// Independent managed counterpart of the source-confirmed double-buffer callback queue.
/// Closing stops future batches without joining or draining; it does not cancel publishers.
/// Worker subscriber faults are retained and stop the worker instead of escaping a thread.
/// </summary>
public sealed class FrameworkCallbackQueue<T> : IDisposable
{
 readonly object gate=new();
 readonly Action<T> dispatch;
 readonly Thread worker;
 T?[] receive;
 T?[] send;
 int receiveCount;
 bool alive=true;
 bool cacheEnabled;
 long dropped;
 readonly List<Exception> dispatchErrors=[];
 public FrameworkCallbackQueue(Action<T> dispatch,int capacity=10000)
 {
  ArgumentNullException.ThrowIfNull(dispatch);ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
  this.dispatch=dispatch;receive=new T[capacity];send=new T[capacity];
  worker=new Thread(Work){IsBackground=true,Priority=ThreadPriority.BelowNormal,Name="VisionFramework callback queue"};worker.Start();
 }
 public bool CacheEnabled{get{lock(gate)return cacheEnabled;}set{lock(gate)cacheEnabled=value;}}
 public int Capacity{get{lock(gate)return receive.Length;}}
 public int PendingCount{get{lock(gate)return receiveCount;}}
 public long DroppedCount{get{lock(gate)return dropped;}}
 public bool IsClosed{get{lock(gate)return !alive;}}
 public IReadOnlyList<Exception> DispatchErrors{get{lock(gate)return dispatchErrors.ToArray();}}
 public CallbackEnqueueResult Enqueue(T value)
 {
  lock(gate){
   if(cacheEnabled){
    if(receiveCount==receive.Length){dropped++;return CallbackEnqueueResult.Overflow;}
    receive[receiveCount++]=value;return CallbackEnqueueResult.Queued;
   }
  }
  dispatch(value);return CallbackEnqueueResult.Dispatched;
 }
 // As in the original, expansion discards queued receives. Call before active dispatch.
 public void ExpandCapacity(int newLength)
 {
  lock(gate){if(newLength<=receive.Length)return;receive=new T[newLength];send=new T[newLength];receiveCount=0;}
 }
 public void PublishWorkStatus(T value,Action synchronousResult,bool hasExecuteCount=true)
 {
  ArgumentNullException.ThrowIfNull(synchronousResult);
  if(hasExecuteCount)Enqueue(value);
  synchronousResult();
 }
 public void PublishModuleResult(T value,Action synchronousResult)
 {
  ArgumentNullException.ThrowIfNull(synchronousResult);synchronousResult();Enqueue(value);
 }
 public void Close(){lock(gate)alive=false;}
 public void Dispose()=>Close();
 void Work()
 {
  while(true){
   T?[]? batch=null;int count=0;
   lock(gate){
    if(!alive)return;
    if(receiveCount>0){(send,receive)=(receive,send);batch=send;count=receiveCount;receiveCount=0;}
   }
   if(batch is not null){
    for(int i=0;i<count;i++){
     try{dispatch(batch[i]!);batch[i]=default;}
     catch(Exception error){lock(gate){dispatchErrors.Add(error);alive=false;}return;}
    }
   }
   Thread.Sleep(1);
  }
 }
}
