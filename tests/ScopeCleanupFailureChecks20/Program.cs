using System.Collections;
using System.Reflection;
using System.Text.Json;
using VisionFramework;

string? output=null;
try{
 if(args.Length is <1 or >2)throw new ArgumentException("Usage: owned-output-parent [observe-unfixed]");
 bool observe=args.Length==2&&args[1]=="observe-unfixed";output=Path.Combine(Path.GetFullPath(args[0]),"scope-cleanup-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
 var checks=new List<string>();void Check(bool value,string label){if(!value)throw new Exception(label);checks.Add(label);Console.WriteLine("PASS "+label);}
 int Diagnostics(ModuleExecutionScope scope)=>typeof(ModuleExecutionScope).GetProperty("CleanupFailures")?.GetValue(scope) is IEnumerable values?values.Cast<object>().Count():0;
 bool TokenDisposed(ModuleExecutionScope scope){var source=(CancellationTokenSource)typeof(ModuleExecutionScope).GetField("stopping",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(scope)!;try{_=source.Token;return false;}catch(ObjectDisposedException){return true;}}
 int badAttempts=0,laterAttempts=0;var resources=new ModuleExecutionScope();
 resources.State("fixture","first",()=>new BadResource(()=>badAttempts++));resources.RegisterService<ILaterResource>(new LaterResource(()=>laterAttempts++));
 string? resourceError=null;try{resources.Dispose();}catch(Exception error){resourceError=error.ToString();}
 bool tokenReleased=TokenDisposed(resources);int resourceDiagnostics=Diagnostics(resources);
 File.WriteAllText(Path.Combine(output,"resource-observation.json"),JsonSerializer.Serialize(new{badAttempts,laterAttempts,resourceError,tokenReleased,resourceDiagnostics},new JsonSerializerOptions{WriteIndented=true}));
 if(observe){Check(badAttempts==1&&laterAttempts==0&&resourceError is not null,"Original first state disposal fault interrupts later owned service");Check(!tokenReleased,"Original interrupted resource loop skips cancellation source disposal");}
 else{Check(badAttempts==1&&laterAttempts==1&&resourceError is null,"Local hardening attempts all owned resources without throwing into host close");Check(tokenReleased&&resourceDiagnostics>=1,"Resource failure retained diagnostically and cancellation source disposed");}
 resources.Dispose();Check(badAttempts==1,"No automatic retry of failed resource after scope already closed");
 int callbacks=0,cancelService=0;var cancelling=new ModuleExecutionScope();cancelling.RegisterService<ILaterResource>(new LaterResource(()=>cancelService++));
 using var registration=cancelling.Stopping.Register(()=>{callbacks++;throw new InvalidOperationException("controlled-cancel-callback");});
 string? cancelError=null;try{cancelling.Dispose();}catch(Exception error){cancelError=error.ToString();}
 int cancelDiagnostics=Diagnostics(cancelling);bool cancelTokenReleased=TokenDisposed(cancelling);
 File.WriteAllText(Path.Combine(output,"cancel-observation.json"),JsonSerializer.Serialize(new{callbacks,cancelService,cancelError,cancelTokenReleased,cancelDiagnostics},new JsonSerializerOptions{WriteIndented=true}));
 if(observe)Check(callbacks==1&&cancelError is not null,"Original own cancellation callback error propagates from Dispose");
 else Check(callbacks==1&&cancelError is null&&cancelDiagnostics>=1,"Own cancellation callback failure is retained without escaping close");
 Check(cancelService==1&&cancelTokenReleased,"Cancellation case still disposes owned service and token");cancelling.Dispose();Check(callbacks==1&&cancelService==1,"Repeated close does not rerun callback or resource disposal");
 int pendingDisposed=0;var pending=new ModuleExecutionScope();pending.RegisterService<ILaterResource>(new LaterResource(()=>pendingDisposed++));var lease=pending.Enter();pending.Dispose();Check(pendingDisposed==0,"Pending lease still defers resources under cleanup failure hardening");lease.Dispose();Check(pendingDisposed==1,"Final pending lease releases resources once");
 var result=new{observeUnfixed=observe,passed=checks.Count,checks,badAttempts,laterAttempts,resourceError,tokenReleased,resourceDiagnostics,callbacks,cancelService,cancelError,cancelTokenReleased,cancelDiagnostics,
   scope="Own managed ModuleExecutionScope objects, resource/cancellation failures only; local hardening characterization, not original VM release semantics or device operations."};
 File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(result,new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine((observe?"OBSERVED UNFIXED":"PASS cleanup hardening")+"; audit="+output);return 0;
}catch(Exception error){try{if(output is not null)File.WriteAllText(Path.Combine(output,"failed.json"),JsonSerializer.Serialize(new{error=error.ToString()},new JsonSerializerOptions{WriteIndented=true}));}catch(Exception e){Console.Error.WriteLine(e);}Console.Error.WriteLine(error);return 1;}

interface ILaterResource { }
sealed class LaterResource(Action dispose):ILaterResource,IDisposable{public void Dispose()=>dispose();}
sealed class BadResource(Action attempt):IDisposable{public void Dispose(){attempt();throw new InvalidOperationException("controlled-first-resource-dispose");}}
