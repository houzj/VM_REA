using System.Reflection;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using SunEyeVision.Workflow;
using SunEyeVision.Plugin.SDK.Core;
using SunEyeVision.Plugin.SDK.Execution.Parameters;
using SunEyeVision.Plugin.SDK.Execution.Results;
using SunEyeVision.Plugin.SDK.Logging;

// Real linked Workflow source; existing managed dependency baselines are recorded separately.
// No camera, image pixels, vendor SDK execution, UI or external process is exercised.
var observations=new List<object>();
var logger=DispatchProxy.Create<ILogger,NoLog>();
var engine=(WorkflowExecutionEngine)RuntimeHelpers.GetUninitializedObject(typeof(WorkflowExecutionEngine));
typeof(WorkflowExecutionEngine).GetField("_logger",BindingFlags.Instance|BindingFlags.NonPublic)!.SetValue(engine,logger);
var flags=BindingFlags.Instance|BindingFlags.NonPublic;
var execute=typeof(WorkflowExecutionEngine).GetMethod("ExecuteAlgorithmNode",flags)!;

// Exercise the actual input synthesis and ToolExecutor path with one scalar parameter.
var globals=new Dictionary<string,object?>{{"X",1L}};
var request=new WorkflowRunRequest("A",globalVariables:globals);
globals["X"]=2L;
if((long)request.GlobalVariables["X"]! != 1L)throw new Exception("Request snapshot contract changed");
var context=new WorkflowContext {WorkflowId="A",GlobalVariables=new Dictionary<string,object?>(request.GlobalVariables)};
var produced=new GenericToolResults {Status=ExecutionStatus.Success};produced.AddResultItem("X",2L,ResultItemType.Numeric);
context.SetNodeResult("producer",new NodeExecutionResult {NodeId="producer",Success=true,Status=ExecutionStatus.Success,Result=produced});
var capture=new ProbeTool((r)=>{var read=r.RuntimeParameters!.GetRequired<long>(nameof(ProbeParameters.X));if(read!=1L)throw new Exception("Global snapshot changed");observations.Add(new {name="ordinary_global_binding",read,seed=1L,producerOutput=2L,producerAlreadyRecorded=true});return ValueTask.FromResult<ToolResults>(new GenericToolResults {Status=ExecutionStatus.Success});});
var consumer=new AlgorithmNode("consumer","consumer","consumer",capture){Parameters=new ProbeParameters()};
consumer.ParamSettings.SetSetting(ParamSetting.CreateBinding(nameof(ProbeParameters.X),"__GlobalVars__","X"));
await (Task<NodeExecutionResult>)execute.Invoke(engine,new object[]{consumer,context})!;
if(capture.Calls!=1)throw new Exception("Consumer did not execute");

// Two actual asynchronous tools. The slow one deliberately does not cooperate with cancellation.
using var cts=new CancellationTokenSource();using var session=new WorkflowRunSession("cancel-probe",cts.Token);
session.TryStart();
var entered=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var release=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var slowExited=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
int lateWrites=0;
var slow=new ProbeTool(async _=>{entered.TrySetResult();await release.Task;Interlocked.Increment(ref lateWrites);slowExited.TrySetResult();return new GenericToolResults {Status=ExecutionStatus.Success};});
var fast=new ProbeTool(async _=>{await entered.Task;return new GenericToolResults {Status=ExecutionStatus.Success};});
var workflow=new Workflow("cancel-probe","cancel-probe");
workflow.Nodes.Add(new AlgorithmNode("slow","slow","slow",slow){Parameters=new ProbeParameters()});
workflow.Nodes.Add(new AlgorithmNode("fast","fast","fast",fast){Parameters=new ProbeParameters()});
var cancelContext=new WorkflowContext {WorkflowId=workflow.Id,CancellationToken=session.CancellationToken};
var dependency=WorkflowExecutionDependencyGraph.Compile(workflow);
var profiles=workflow.Nodes.ToDictionary(n=>n.Id,n=>new ToolExecutionProfile(ToolConcurrencyKind.ExternalDataWait));
engine.NodeExecutionFinished+=(_,e)=>{if(e.NodeId=="fast")cts.Cancel();};
var queue=typeof(WorkflowExecutionEngine).GetMethod("ExecuteWorkflowWithReadyQueue",flags)!;
var run=(Task<ExecutionResult>)queue.Invoke(engine,new object[]{session,workflow,cancelContext,dependency,profiles})!;
bool cancelled=false;
try {await run.WaitAsync(TimeSpan.FromSeconds(15));}catch(OperationCanceledException){cancelled=true;}
if(!cancelled||slowExited.Task.IsCompleted)throw new Exception("Selected cancellation path not reproduced");
observations.Add(new{name="queue_returns_before_noncooperative_tool_exits",cancelled,slowStillRunning=!slowExited.Task.IsCompleted,lateWritesAtReturn=lateWrites});
release.TrySetResult();await slowExited.Task.WaitAsync(TimeSpan.FromSeconds(10));
observations.Add(new{name="noncooperative_tool_can_finish_side_effect_after_queue_return",lateWrites});
if(lateWrites!=1)throw new Exception("Late synthetic side effect was not observed");
var assemblies=AppDomain.CurrentDomain.GetAssemblies().Where(a=>!a.IsDynamic&&a.GetName().Name!.StartsWith("SunEye",StringComparison.Ordinal)).Select(a=>new {name=a.GetName().Name,path=a.Location,mvid=a.ManifestModule.ModuleVersionId,sha256=Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(a.Location))).ToLowerInvariant()}).ToArray();
var result=new {scope="Real linked current SunEye Workflow/SDK/Core/Infrastructure sources on net10.0-windows; synthetic tools and private scheduler entry, not complete net9 product UI/manager execution",passed=3,observations,assemblies,guiStarted=false,cameraCalled=false,vendorSdkExecuted=false,otherProjectModified=false,fullHostParity=false};
File.WriteAllText(args[0],JsonSerializer.Serialize(result,new JsonSerializerOptions{WriteIndented=true}));
Console.WriteLine(JsonSerializer.Serialize(result));

public class ProbeParameters:ToolParameters {public long X{get;set;}}
public sealed class ProbeTool(Func<ToolExecutionRequest,ValueTask<ToolResults>> body):IAsyncToolPlugin
{
 public int Calls;public Type ParamsType=>typeof(ProbeParameters);public Type ResultType=>typeof(GenericToolResults);
 public IReadOnlyList<OutputVariable> OutputVariables=>Array.Empty<OutputVariable>();
 public ToolResults Run(ToolParameters p)=>throw new InvalidOperationException("Async path expected");
 public ValueTask<ToolResults> ExecuteAsync(ToolExecutionRequest request){Interlocked.Increment(ref Calls);return body(request);}
}
public class NoLog:DispatchProxy {protected override object? Invoke(MethodInfo? method,object?[]? args)=>method?.ReturnType==typeof(void)?null:method!.ReturnType.IsValueType?Activator.CreateInstance(method.ReturnType):null;}
