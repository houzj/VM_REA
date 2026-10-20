using System.IO;
using System.Text.Json;
using VisionFramework;

// Requires root-reviewed typed-input candidate. Prepared before shared application.
internal static class TypedTransportChecks
{
 internal static void Run(Action<bool,string> check,string output)
 {
  var raw=new Dictionary<string,PortValue>{{"Value",PortValue.IntegerArray([10,20])}};
  var reader=new WorkflowNumericInputRead(raw,new Dictionary<string,WorkflowArrayIndex?>{{"Value",null}},_=>throw new InvalidDataException());
  check(reader.Read("Value",0)==new ModuleNumericInputReadResult(0xe0000314,0,0),"integer raw container does not silently become successful GetFloat");
  check(reader.ReadInteger("Value",0)==new ModuleIntegerInputReadResult(0,2,10),"independent GetInt phase preserves All count");
  var indexed=new WorkflowNumericInputRead(raw,new Dictionary<string,WorkflowArrayIndex?>{{"Value",new("1")}},_=>throw new InvalidDataException());
  check(indexed.ReadInteger("Value",0)==new ModuleIntegerInputReadResult(0,1,20),"GetInt indexed success count1");
  check(new WorkflowNumericInputRead(raw,new Dictionary<string,WorkflowArrayIndex?>{{"Value",new("9")}},_=>throw new InvalidDataException()).ReadInteger("Value",0)==new ModuleIntegerInputReadResult(0xe0000014,0,0),"GetInt wrapper positive bounds count0");
  var registry=new ModuleRegistry();registry.Register(new Source().Descriptor,()=>new Source());registry.Register(new Consumer().Descriptor,()=>new Consumer());
  var flow=new ProcedureDocument("typed","typed",[new("source",Source.Id,"1","source",[]),new("consumer",Consumer.Id,"1","consumer",[])],[]){Configuration=new(){Subscriptions=[new("consumer","Value",false,new(WorkflowSourceKind.NodeOutput,"source","Value"))]}};
  check(GraphValidator.Validate(flow,registry).IsValid,"opted-in integer array binding validates");
  using var scope=new ModuleExecutionScope();var run=new GraphExecutor(registry).RunAsync(flow,output,scope:scope).Result;var result=run.Nodes.Single(n=>n.NodeId=="consumer");
  check(result.Status==NodeRunStatus.Success&&result.Outputs["Count"].As<long>()==2&&result.Outputs["Value"].As<long>()==10&&result.Outputs["FloatError"].As<long>()==0xe0000314,"actual Graph keeps integer phase distinct and preserves raw cardinality");
  var cached=new[]{new NodeRunRecord("source",NodeRunStatus.Success,raw,0)};var debug=new NodeDebugExecutor(registry).RunAsync(flow,"consumer",output,new Dictionary<string,JsonElement>(),cached).Result;
  check(debug.Status==NodeRunStatus.Success&&debug.Outputs["Count"].As<long>()==2&&debug.Outputs["Value"].As<long>()==10,"actual cached debug transports raw integer array into isolated graph");
  var strict=new Consumer().Descriptor with{Id="readcalib.strict",Inputs=[new("Value",DataKind.Integer,false)]};registry.Register(strict,()=>new Consumer());
  var strictFlow=flow with{Nodes=[flow.Nodes[0],flow.Nodes[1] with{ModuleId=strict.Id}]};
  check(!GraphValidator.Validate(strictFlow,registry).IsValid,"unopted scalar Integer target keeps strict array rejection");
  string longText=new('界',200);var textValues=new Dictionary<string,PortValue>{{"Text",PortValue.TextArray([longText,"second"])}};
  var textReader=new WorkflowNumericInputRead(textValues,new Dictionary<string,WorkflowArrayIndex?>{{"Text",null}},_=>throw new InvalidDataException());
  check(textReader.ReadText("Text",0,511)==new ModuleTextInputReadResult(0xe0000012,0,"",601),"UTF8 capacity probe includes nul and failure count0");
  check(textReader.ReadText("Text",0,602)==new ModuleTextInputReadResult(0,2,longText,601),"text retry preserves full first array value and All count");
  check(new WorkflowNumericInputRead(textValues,new Dictionary<string,WorkflowArrayIndex?>{{"Text",new("1")}},_=>throw new InvalidDataException()).ReadText("Text",0,511)==new ModuleTextInputReadResult(0,1,"second",7),"text selected item success count1");
  registry.Register(new TextConsumer().Descriptor,()=>new TextConsumer());
  var textFlow=flow with{Nodes=[flow.Nodes[0],flow.Nodes[1] with{ModuleId=TextConsumer.Id}],Configuration=new(){Subscriptions=[new("consumer","Text",false,new(WorkflowSourceKind.NodeOutput,"source","Text"))]}};
  using(var textScope=new ModuleExecutionScope()){
   var textRun=new GraphExecutor(registry).RunAsync(textFlow,output,scope:textScope).Result;var textResult=textRun.Nodes.Single(n=>n.NodeId=="consumer");
   check(textResult.Status==NodeRunStatus.Success&&textResult.Outputs["Value"].As<string>()==longText&&textResult.Outputs["Count"].As<long>()==2,"actual Graph keeps TextArray raw through capacity retry");
  }
  var textCached=new[]{new NodeRunRecord("source",NodeRunStatus.Success,textValues,0)};var textDebug=new NodeDebugExecutor(registry).RunAsync(textFlow,"consumer",output,new Dictionary<string,JsonElement>(),textCached).Result;
  check(textDebug.Status==NodeRunStatus.Success&&textDebug.Outputs["Value"].As<string>()==longText,"actual cached debug preserves text callback and UTF8 retry");
 }
 sealed class Source:IModule
 {
  public const string Id="readcalib.typed-source";
  public ModuleDescriptor Descriptor{get;}=new(Id,"1","source",[],[new PortDescriptor("Value",DataKind.Integer){Cardinality=ValueCardinality.Array},new PortDescriptor("Text",DataKind.Text){Cardinality=ValueCardinality.Array}],[]);
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"Value",PortValue.IntegerArray([10,20])},{"Text",PortValue.TextArray([new string('界',200),"second"])}}));
 }
 sealed class Consumer:IModule
 {
  public const string Id="readcalib.typed-consumer";
  public ModuleDescriptor Descriptor{get;}=new(Id,"1","consumer",[new PortDescriptor("Value",DataKind.NumberArray,false,true){SupportsDeferredNumericRead=true,SupportsDeferredIntegerRead=true}],[new("Value",DataKind.Integer),new("Count",DataKind.Integer),new("FloatError",DataKind.Integer)],[]);
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token){var floating=context.ReadNumericInput!("Value",0)!.Value;var integer=context.ReadIntegerInput!("Value",0)!.Value;return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"Value",PortValue.Integer(integer.Value)},{"Count",PortValue.Integer(integer.Count)},{"FloatError",PortValue.Integer(floating.ErrorCode)}}));}
 }
 sealed class TextConsumer:IModule {
  public const string Id="readcalib.text-consumer";
  public ModuleDescriptor Descriptor{get;}=new(Id,"1","consumer",[new PortDescriptor("Text",DataKind.Text,false){SupportsDeferredTextRead=true}],[new("Value",DataKind.Text),new("Count",DataKind.Integer)],[]);
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token){var text=context.ReadTextInput!("Text",0,511)!.Value;if(text.ErrorCode==0xe0000012)text=context.ReadTextInput!("Text",0,text.RequiredBytes+1)!.Value;return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"Value",PortValue.Text(text.Value)},{"Count",PortValue.Integer(text.Count)}}));}
 }
}
