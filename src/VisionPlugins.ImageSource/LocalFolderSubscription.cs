using System.IO;
using VisionFramework;

namespace VisionPlugins.ImageSource;

/// <summary>Session-owned watcher; consumes coalesced dirty state during execution.</summary>
internal sealed class LocalFolderSubscription:IDisposable {
 readonly object gate=new();
 readonly FileSystemWatcher watcher;
 readonly ModuleExecutionScope scope;
 readonly string node;
 int dirty=1;bool disposed;
 internal bool TriggerRun {get;set;}
 internal string DirectoryPath {get;}
 internal LocalFolderSubscription(string directory,ModuleExecutionScope scope,string node){
  DirectoryPath=Path.GetFullPath(directory);this.scope=scope;this.node=node;
  watcher=new(DirectoryPath){IncludeSubdirectories=false,NotifyFilter=NotifyFilters.FileName|NotifyFilters.DirectoryName|NotifyFilters.LastWrite,InternalBufferSize=65536};
  watcher.Created+=(_,_)=>Changed(true);watcher.Deleted+=(_,_)=>Changed(true);watcher.Changed+=(_,_)=>Changed(false);
  // Original actions 4/5 only log rename; they do not set dirty or request a run.
  watcher.Error+=(_,_)=>Changed(false);watcher.EnableRaisingEvents=true;
 }
 void Changed(bool requestRun){lock(gate){if(disposed)return;Interlocked.Exchange(ref dirty,1);if(requestRun&&TriggerRun)scope.RequestRun(node);}}
 internal bool ConsumeDirty()=>Interlocked.Exchange(ref dirty,0)!=0;
 public void Dispose(){lock(gate){if(disposed)return;disposed=true;watcher.Dispose();}}
}
