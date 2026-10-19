using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using VisionFramework;

// Only terminates a Process instance created by this launcher, never an enumerated PID.
if(args.Length==3&&args[0]=="--owned-child"){
    string target=Path.GetFullPath(args[1]),token=args[2];
    if(Environment.GetEnvironmentVariable("VM_SAVE_PROBE_TOKEN")!=token)throw new InvalidOperationException("Owned-child token missing");
    var next=new SolutionDocument(1,"interruption",new string('n',16*1024*1024),[new("p","p",[],[])],[]);
    File.WriteAllText(Path.Combine(Path.GetDirectoryName(target)!,"child-ready.txt"),token);
    for(int i=0;i<4;i++)SolutionStore.Save(target,next);
    return 0;
}
if(args.Length!=1)throw new ArgumentException("Supply audit directory");
string root=Path.Combine(Path.GetFullPath(args[0]),"interruption-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);
var observations=new List<object>();bool observed=false,passed=false;int checks=0;
void Check(bool value,string label){if(!value)throw new Exception(label);checks++;Console.WriteLine("PASS "+label);}
try{
 for(int attempt=0;attempt<3&&!observed;attempt++){
    string owned=Path.Combine(root,"attempt-"+attempt);Directory.CreateDirectory(owned);string target=Path.Combine(owned,"target.json");
    var before=new SolutionDocument(1,"interruption","original",[new("p","p",[],[])],[]);SolutionStore.Save(target,before);
    string token=Guid.NewGuid().ToString("N");string exe=Environment.ProcessPath!;
    var start=new ProcessStartInfo(exe){UseShellExecute=false,CreateNoWindow=true,RedirectStandardOutput=true,RedirectStandardError=true,WorkingDirectory=owned};
    if(Path.GetFileNameWithoutExtension(exe).Equals("dotnet",StringComparison.OrdinalIgnoreCase))start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location);
    start.ArgumentList.Add("--owned-child");start.ArgumentList.Add(target);start.ArgumentList.Add(token);start.Environment["VM_SAVE_PROBE_TOKEN"]=token;
    using var child=Process.Start(start)??throw new Exception("Own child did not start");int pid=child.Id;DateTime started=child.StartTime.ToUniversalTime();
    var stdout=child.StandardOutput.ReadToEndAsync();var stderr=child.StandardError.ReadToEndAsync();bool suspended=false,killed=false;string[] temp=[];int? suspendStatus=null;
    try{
      var watch=Stopwatch.StartNew();
      while(!child.HasExited&&watch.Elapsed<TimeSpan.FromSeconds(20)){
        if(Directory.EnumerateFiles(owned,".target.json.*.tmp").Any()){
          suspendStatus=Native.NtSuspendProcess(child.Handle);suspended=suspendStatus==0;
          if(suspended){temp=Directory.GetFiles(owned,".target.json.*.tmp");if(temp.Length>0){observed=true;break;}Native.NtResumeProcess(child.Handle);suspended=false;}
        }
        Thread.Sleep(1);
      }
      if(observed||!child.HasExited){child.Kill(entireProcessTree:false);killed=true;}
      child.WaitForExit();
    }finally{if(!child.HasExited){if(suspended)Native.NtResumeProcess(child.Handle);child.Kill(entireProcessTree:false);child.WaitForExit();}}
    File.WriteAllText(Path.Combine(owned,"stdout.log"),stdout.GetAwaiter().GetResult());File.WriteAllText(Path.Combine(owned,"stderr.log"),stderr.GetAwaiter().GetResult());
    var loaded=SolutionStore.Load(target);Check(loaded.Id=="interruption"&&(loaded.Name=="original"||(loaded.Name.Length==16*1024*1024&&loaded.Name.All(c=>c=='n'))),"attempt "+attempt+": target contains a complete old or new document");
    var leftovers=Directory.GetFiles(owned,".target.json.*.tmp");
    observations.Add(new{attempt,pid,started,token,observedTemporaryBeforeKill=observed,suspendStatus,killed,exitCode=child.ExitCode,targetSha256=Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(target))),targetBytes=new FileInfo(target).Length,temporaryFiles=leftovers.Select(p=>new{path=p,bytes=new FileInfo(p).Length,sha256=Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(p)))}).ToArray()});
    if(observed){Check(killed&&leftovers.Length>0,"owned process interruption leaves observed uncommitted temporary file");
      var recovery=before with{Name="recovered"};SolutionStore.Save(target,recovery);Check(SolutionStore.Load(target).Name=="recovered","explicit subsequent save succeeds with orphan temporary file retained");
      Check(leftovers.All(File.Exists),"subsequent save does not silently delete another temporary file");}
 }
 passed=observed;Console.WriteLine($"observedInterruption={observed}; passed={passed}");
 return passed?0:2;
}finally{File.WriteAllText(Path.Combine(root,"result.json"),JsonSerializer.Serialize(new{passed,observed,checks,observations,authority="own-process independent SolutionStore interruption; not power loss/native VM",root},new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine("Evidence: "+root);}
static class Native{
 [DllImport("ntdll.dll")]internal static extern int NtSuspendProcess(IntPtr process);
 [DllImport("ntdll.dll")]internal static extern int NtResumeProcess(IntPtr process);
}
