using System.IO;
using System.Text.Json;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;

static class Program
{
 static int checks;
 static void Check(bool value,string reason){if(!value)throw new InvalidOperationException(reason);checks++;}
 static Dictionary<string,JsonElement> Defaults(ReadCalibFilePlugin plugin)=>plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());
 static readonly string Xml="<R><CalibInputParam/><CalibOutputParam><P ParamName='SnapImagePointX' DataType='float'><V>12</V></P></CalibOutputParam></R>";
 [STAThread]static int Main()
 {
  string output=Path.GetFullPath(Environment.GetEnvironmentVariable("READCALIBFILE_INPUT_OUTPUT")??"analysis/read-calib-file/input-checks");Directory.CreateDirectory(output);
  try{_=new System.Windows.Application{ShutdownMode=System.Windows.ShutdownMode.OnExplicitShutdown};System.Threading.SynchronizationContext.SetSynchronizationContext(new System.Windows.Threading.DispatcherSynchronizationContext());Run(output);TypedTransportChecks.Run(Check,output);ExtraTransportChecks.Run(Check,output);HostChecks.Run(Check,output);FileFailureChecks.Run(Check,output);File.WriteAllText(Path.Combine(output,"checks.json"),JsonSerializer.Serialize(new{passed=true,checks,originalRuntime=false}));Console.WriteLine($"PASS {checks} ReadCalibFile input checks");return 0;}catch(Exception error){Console.Error.WriteLine(error);return 1;}
 }
 static void Run(string output)
 {
  var plugin=new ReadCalibFilePlugin();var parameters=Defaults(plugin);parameters["CachedFileData"]=JsonSerializer.SerializeToElement(Xml);
  using var scope=new ModuleExecutionScope();var runtime=plugin.CreateRuntime();
  ModuleResult Execute(IReadOnlyDictionary<string,PortValue>? inputs=null,Func<string,int,ModuleNumericInputReadResult?>? read=null)=>runtime.ExecuteAsync(new("loader",output,inputs??new Dictionary<string,PortValue>(),parameters){Scope=scope,ReadNumericInput=read},default).Result;
  foreach(var input in plugin.Descriptor.Inputs.Where(p=>p.Name!="RefreshSignal"))Check(input.Kind==DataKind.NumberArray&&input.SupportsDeferredNumericRead,"pose array/deferred contract "+input.Name);
  Check(Execute(new Dictionary<string,PortValue>{{"SnapImagePointX1",PortValue.Numbers([23,99])}}).Outputs["SnapImagePointX"].As<double>()==23,"materialized array uses first item");
  Check(Execute(new Dictionary<string,PortValue>{{"SnapImagePointX1",PortValue.Numbers([])}}).Outputs["SnapImagePointX"].As<double>()==12,"empty array preserves file value");
  Check(Execute().Outputs["SnapImagePointX"].As<double>()==12,"override is not cached across runs");
  foreach(var result in new[]{new ModuleNumericInputReadResult(0xe0000314,3,99),new(0,0,99),new(0,-1,99)})Check(Execute(read:(name,index)=>name=="SnapImagePointX1"?result:null).Outputs["SnapImagePointX"].As<double>()==12,"failed/nonpositive count preserves file value");
  Check(Execute(read:(name,index)=>name=="SnapImagePointX1"?new(0,4,45):null).Outputs["SnapImagePointX"].As<double>()==45,"execution reader supplies first value");
  var reader=new Reader();scope.RegisterService<IToolNumericInputReader>(reader);int graphCalls=0;
  Check(Execute(read:(name,index)=>{graphCalls++;return new(0,1,777);}).Outputs["SnapImagePointX"].As<double>()==31,"scope reader has highest priority");
  Check(graphCalls==0&&reader.Calls.SequenceEqual(ReadCalibFilePlugin.PoseInputs.Select(n=>n+":0")),"exact nine one-shot reads and original field order");
  var registry=new ModuleRegistry();registry.Register(plugin.Descriptor,plugin.CreateRuntime,plugin.DescribeNode);registry.Register(new Source().Descriptor,()=>new Source());
  var flow=new ProcedureDocument("flow","flow",[new("source",Source.Id,"1","source",[]),new("read",ReadCalibFilePlugin.Id,"1.0.0","read",parameters)],[new("source","Values","read","SnapImagePointX1")]);
  Check(GraphValidator.Validate(flow,registry).IsValid,"actual graph accepts pose array source");
  using var graphScope=new ModuleExecutionScope();var run=new GraphExecutor(registry).RunAsync(flow,output,scope:graphScope).Result;
  Check(run.Status==SessionStatus.Success&&run.Nodes.Single(n=>n.NodeId=="read").Outputs["SnapImagePointX"].As<double>()==54,"actual graph transports array to native first-item consumer");
  foreach(string? selector in new string?[]{null,"1","9","-1"})
  {
   var source=new WorkflowValueReference(WorkflowSourceKind.NodeOutput,"source","Values"){ArrayIndex=selector is null?null:new(selector)};
   var bound=flow with{Connections=[],Configuration=new(){Subscriptions=[new("read","SnapImagePointX1",false,source)]}};
   using var boundScope=new ModuleExecutionScope();var boundRun=new GraphExecutor(registry).RunAsync(bound,output,scope:boundScope).Result;
   double expected=selector is null?54:selector=="1"?98:12;
   Check(boundRun.Status==SessionStatus.Success&&boundRun.Nodes.Single(n=>n.NodeId=="read").Outputs["SnapImagePointX"].As<double>()==expected,"bound All/index reads retain file value on read error: "+selector);
  }
  var optional=flow with{Connections=[],Configuration=new(){Subscriptions=[new("read","SnapImagePointX1",false,new(WorkflowSourceKind.NodeOutput,"source","Values"))]}};
  Source.Omit=true;
  using(var missingScope=new ModuleExecutionScope())
  {
   var missing=new GraphExecutor(registry).RunAsync(optional,output,scope:missingScope).Result;
   Check(missing.Status==SessionStatus.Success&&missing.Nodes.Single(n=>n.NodeId=="read").Outputs["SnapImagePointX"].As<double>()==12,"All missing optional source reaches one-shot reader and preserves file value");
  }
  Source.Fail=true;
  using(var failedScope=new ModuleExecutionScope())
  {
   var failed=new GraphExecutor(registry).RunAsync(optional,output,scope:failedScope).Result;
   Check(failed.Nodes.Single(n=>n.NodeId=="read") is {Status:NodeRunStatus.Failed,ErrorCode:"SubscriptionUnavailable"},"failed producer is not deferred by numeric transport");
  }
  Source.Omit=Source.Fail=false;
 }
 sealed class Reader:IToolNumericInputReader
 {
  public List<string> Calls=[];
  public ToolNumericInputReadResult Read(ModuleExecutionContext context,string inputName,int index){Calls.Add(inputName+":"+index);return inputName=="SnapImagePointX1"?new(0,2,31):new(0xe0000314,0,0);}
 }
 sealed class Source:IModule
 {
  public const string Id="test.readcalib.array";
  public static bool Omit,Fail;
  public ModuleDescriptor Descriptor{get;}=new(Id,"1","source",[],[new("Values",DataKind.NumberArray,false)],[]);
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>ValueTask.FromResult(new ModuleResult(Fail?ModuleStatus.Failed:ModuleStatus.Success,Omit?new Dictionary<string,PortValue>():new Dictionary<string,PortValue>{{"Values",PortValue.Numbers([54,98])}}));
 }
}
