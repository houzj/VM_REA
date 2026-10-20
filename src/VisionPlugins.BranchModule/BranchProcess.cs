#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
namespace VisionPlugins.BranchModule;
public readonly record struct BranchInputRead(uint ErrorCode,int Count,int Value);
public sealed record BranchRouteRow(int ModuleId,int Value);
public sealed record BranchNextModule(int ModuleId,string HostNodeId);
public sealed record BranchNextRead(uint ErrorCode,int Count,IReadOnlyList<BranchNextModule> Modules);
public interface IBranchProcessHost {
 bool HandlesValid {get;}
 int CurrentModuleId {get;}
 BranchInputRead ReadInputInt(string name,int index);
 BranchNextRead GetNextModules();
 uint SetNextModules(IReadOnlyList<BranchNextModule> selected);
 double QueryMilliseconds();
 uint SetRuntime(float milliseconds);
 uint SetInt(string name,int index,int value);
 uint SetString(string name,int index,string value);
 uint ForwardDynamicOutputs();
}
// Exact normal-memory Process control flow from 1800078f0. Host I/O failures
// remain observable callback results even when the original ignores them.
// Managed strings intentionally do not reproduce native char[256] overflow.
public static class BranchProcess {
 public static uint Run(IBranchProcessHost host,int mode,IReadOnlyList<BranchRouteRow> rows){
  if(!host.HandlesValid)return 0xe000000a;
  var read=host.ReadInputInt("inputint",0);
  if(read.ErrorCode!=0||read.Count<1){host.SetNextModules(Array.Empty<BranchNextModule>());return read.ErrorCode;}
  double start=host.QueryMilliseconds();var next=host.GetNextModules();
  if(next.Count>next.Modules.Count)throw new InvalidOperationException("Independent managed host boundary: invalid next-list Count.");
  var selected=new List<BranchNextModule>();var names=new System.Text.StringBuilder();
  if(next.Count>0){
   for(int row=0;row<rows.Count;row++){
    bool match=mode==0?read.Value==rows[row].Value:mode==1&&((read.Value>>(row&31))&1)!=0;
    if(!match)continue;
    for(int i=0;i<next.Count;i++)if(rows[row].ModuleId==next.Modules[i].ModuleId){selected.Add(next.Modules[i]);names.Append("ID").Append(next.Modules[i].ModuleId.ToString(CultureInfo.InvariantCulture)).Append(';');}
   }
  }
  host.SetNextModules(selected.AsReadOnly());
  bool found=names.Length!=0;string branch=found?names.ToString():"No Module";
  float elapsed=(float)(host.QueryMilliseconds()-start);host.SetRuntime(elapsed);
  host.SetInt("outputint",0,read.Value);host.SetInt("ModuStatus",0,found?1:0);
  host.SetString("BranchMatch",0,branch);host.SetInt("OutputForEnd",0,host.CurrentModuleId);
  host.ForwardDynamicOutputs();return 0;
 }
}
