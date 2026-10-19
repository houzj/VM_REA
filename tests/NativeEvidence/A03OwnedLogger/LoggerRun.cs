using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
class LoggerRun {
 static string Hash(string p) { using(var s=File.OpenRead(p)) using(var h=new SHA256Managed()) return BitConverter.ToString(h.ComputeHash(s)).Replace("-",""); }
 static void Main(string[] args) {
  string root=AppDomain.CurrentDomain.BaseDirectory;
  Directory.SetCurrentDirectory(root);
  Action<string> log=s=>File.AppendAllText(Path.Combine(root,"logger-observation.log"),DateTime.UtcNow.ToString("o")+" "+s+Environment.NewLine);
  foreach(string n in new[]{"Apps.BaseLog.dll","Apps.Log.dll"}) {
   string copy=Path.Combine(root,n); var a=Assembly.LoadFrom(copy);
   log("loaded="+a.FullName+" path="+a.Location+" sha="+Hash(a.Location));
   if(Hash(copy)!=Hash(a.Location)) throw new Exception("Loaded binary hash differs from owned copy");
   foreach(var r in a.GetReferencedAssemblies()) log("reference="+r.FullName);
  }
  var helper=Assembly.LoadFrom(Path.Combine(root,"Apps.Log.dll")).GetType("Apps.Log.LogHelper",true);
  string selected=(string)helper.GetMethod("GetCurrentVersionInstallPath",BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,null);
  log("selected="+selected);
  if(!String.Equals(Path.GetFullPath(selected),Path.Combine(root,"myLibs","Apps.BaseLog.config"),StringComparison.OrdinalIgnoreCase)) throw new Exception("Wrong config");
  var thread=(Thread)helper.GetField("refreshParamthread",BindingFlags.Static|BindingFlags.NonPublic).GetValue(null);
  log("queueThread="+thread.ManagedThreadId+" background="+thread.IsBackground+" alive="+thread.IsAlive);
  int watcherCount=0;
  var config=Assembly.LoadFrom(Path.Combine(root,"Apps.BaseLog.dll")).GetType("Apps.BaseLog.Config.XmlConfigurator",true);
  foreach(var f in config.GetFields(BindingFlags.Static|BindingFlags.NonPublic|BindingFlags.Public)) {
   object v=f.GetValue(null); var dict=v as System.Collections.IDictionary;
   if(dict==null) continue;
   foreach(System.Collections.DictionaryEntry e in dict) {
    log("watchDictionary="+f.Name+" key="+e.Key+" valueType="+e.Value.GetType().FullName);
    foreach(var wf in e.Value.GetType().GetFields(BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public)) {
     var w=wf.GetValue(e.Value) as FileSystemWatcher;
     if(w!=null) { watcherCount++; log("watcher="+w.Path+" filter="+w.Filter+" enabled="+w.EnableRaisingEvents); if(!Path.GetFullPath(w.Path).StartsWith(root,StringComparison.OrdinalIgnoreCase)) throw new Exception("Watcher escaped owned root"); }
    }
   }
  }
  if(watcherCount==0) throw new Exception("No actual watcher observed");
  if(!thread.IsAlive || !thread.IsBackground) throw new Exception("Logger queue thread not running as expected");
  string marker="REA_A03_OWNED_"+Guid.NewGuid().ToString("N");
  helper.GetMethod("Info",new[]{typeof(object)}).Invoke(null,new object[]{marker});
  bool found=false;
  for(int i=0;i<40&&!found;i++) { Thread.Sleep(250); foreach(string p in Directory.GetFiles(root,"*.log",SearchOption.AllDirectories)) if(!p.EndsWith("logger-observation.log")&&File.ReadAllText(p).Contains(marker)){log("markerFound="+p+" marker="+marker);found=true;} }
  if(!found) throw new Exception("Marker not found in owned outputs");
  foreach(var a in AppDomain.CurrentDomain.GetAssemblies()) { if(!a.IsDynamic) log("assembly="+a.FullName+" path="+a.Location+" sha="+Hash(a.Location)); }
  log("logger-proof=PASS; no decorator/SDK loaded; watcher reload not exercised");
 }
}


