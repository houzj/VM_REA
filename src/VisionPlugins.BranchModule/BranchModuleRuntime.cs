#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.BranchModule;
// NodeId routing adapter over the source-confirmed integer branch. NumericId
// absence is explicit and never parsed from a GUID or silently represented as 0.
internal sealed class BranchModuleRuntime(Action<ModuleExecutionContext,ModuleResult>? completed=null):IModule {
 public static readonly ModuleDescriptor Description=new("vision.branch-module","1.0.0","分支模块",
  new[]{new PortDescriptor("inputint",DataKind.Integer,false){SupportsDeferredIntegerRead=true}},
  new[]{new PortDescriptor("ModuRunTime",DataKind.Number,false),new("outputint",DataKind.Integer,false){CanSubscribe=false},new("ModuStatus",DataKind.Integer,false){CanSubscribe=false},new("BranchMatch",DataKind.Text,false){CanSubscribe=false}},
  new[]{new ParameterDescriptor("IndexUserSet",ParameterKind.Integer,true,System.Text.Json.JsonSerializer.SerializeToElement(0)),new("ModuleInfoList",ParameterKind.Text,true,System.Text.Json.JsonSerializer.SerializeToElement(""))});
 public ModuleDescriptor Descriptor=>Description;
 static ToolIntegerInputReadResult Read(ModuleExecutionContext c){
  if(c.Scope?.Service<IToolIntegerInputReader>() is {} service)return service.Read(c,"inputint",0);
  if(c.ReadIntegerInput?.Invoke("inputint",0) is {} result)return new(result.ErrorCode,result.Count,result.Value);
  if(c.Inputs.TryGetValue("inputint",out var value)&&value.Kind==DataKind.Integer){var elements=value.Elements().ToArray();if(elements.Length>0)return new(0,elements.Length,unchecked((int)elements[0].As<long>()));}
  return new(0xe0000314,0,0);
 }
 public System.Threading.Tasks.ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext c,System.Threading.CancellationToken cancellationToken){
  cancellationToken.ThrowIfCancellationRequested();System.Threading.Tasks.ValueTask<ModuleResult> Done(ModuleResult value){completed?.Invoke(c,value);return System.Threading.Tasks.ValueTask.FromResult(value);}var read=Read(c);
  if(read.ErrorCode!=0||read.Count<1)return Done(new ModuleResult(read.ErrorCode==0?ModuleStatus.Success:ModuleStatus.Failed,new Dictionary<string,PortValue>(),read.ErrorCode==0?null:"BranchInputRead"){NativeErrorCode=read.ErrorCode,SelectedExecutionSuccessors=Array.Empty<string>()});
  var timer=Stopwatch.StartNew();var rows=BranchRouteTable.Parse(c.Text("ModuleInfoList"));int mode=c.Integer("IndexUserSet");
  var selected=new List<string>();var match=new StringBuilder();bool nodeIdAdapter=false;
  for(int row=0;row<rows.Count;row++){
   // The normal UI admits only signed Int32 text; Defaults/N has native atoi=0.
   int value=rows[row].Value;
   bool accepts=mode==0?read.Value==value:mode==1&&((read.Value>>(row&31))&1)!=0;if(!accepts)continue;
   foreach(var next in c.ExecutionSuccessors)if(next.NodeId==rows[row].NodeId){
    selected.Add(next.NodeId);if(next.NumericId is int id)match.Append("ID").Append(id.ToString(CultureInfo.InvariantCulture)).Append(';');else{nodeIdAdapter=true;match.Append("NodeId:").Append(next.NodeId).Append(';');}
   }
  }
  var outputs=new Dictionary<string,PortValue>{{"ModuRunTime",PortValue.Number((float)timer.Elapsed.TotalMilliseconds)},{"outputint",PortValue.Integer(read.Value)},{"ModuStatus",PortValue.Integer(selected.Count>0?1:0)},{"BranchMatch",PortValue.Text(selected.Count>0?match.ToString():"No Module")}};
  var result=new ModuleResult(ModuleStatus.Success,outputs,Message:nodeIdAdapter?"Independent NodeId identity adapter: BranchMatch is not native ID%d; NumericId source remains unverified.":null){NativeErrorCode=0,SelectedExecutionSuccessors=selected.AsReadOnly()};return Done(result);
 }
}

