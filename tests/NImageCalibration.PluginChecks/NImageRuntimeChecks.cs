using VisionCore;
using VisionPlugins.Calibration;
public static class NImageRuntimeChecks
{
 public static int Run(){int checks=0;void Check(bool c,string n){checks++;if(!c)throw new Exception(n);}var runtime=new NImageRuntime();var p=new NImageSequenceParameters(4,0);var tiny=new GrayImage(1,1,new byte[1]);
  for(int i=0;i<3;i++){var batch=runtime.Add(tiny,p,7);Check(batch.Sequence.Current==i+1&&batch.ErrorCode==0&&!batch.Ready,"native validation deferred until full batch");}
  var rejected=runtime.Add(tiny,p,7);Check(rejected.ErrorCode==0x10000019&&rejected.Sequence.Current==4&&!rejected.Ready,"full batch validates original minimum size");Check(runtime.Add(tiny,p,7).Sequence.Current==1,"next execution starts new batch after failed full batch");runtime.Add(tiny,p,7);Check(runtime.Add(tiny,p,7,1).Sequence.Current==1,"clear revision resets owned frames");runtime.Clear();Check(runtime.Inspect(4).Current==0,"explicit lifecycle clear");
  var blank=new GrayImage(64,64,new byte[4096]);for(int i=0;i<3;i++)runtime.Add(blank,p,7);Check(runtime.Add(blank,p,7).ErrorCode==0x10103000,"blank coded-board recognition fails with original detection error");runtime.Clear();using var cancelled=new CancellationTokenSource();cancelled.Cancel();try{runtime.Add(blank,p,7,cancellationToken:cancelled.Token);throw new Exception("Cancellation ignored");}catch(OperationCanceledException){checks++;}Check(runtime.Inspect(4).Current==0,"cancel before capture leaves state unchanged");return checks;
 }
}
