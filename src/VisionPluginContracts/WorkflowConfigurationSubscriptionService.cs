using System.Globalization;
using System.IO;
using System.Text.Json;
using VisionFramework;

namespace VisionPluginContracts;

/// <summary>Host-neutral, explicitly limited bridge. It never resolves identity from captions and never runs an algorithm.</summary>
public sealed class WorkflowConfigurationSubscriptionService:IToolConfigurationSubscriptionService {
 readonly Func<ProcedureDocument> getProcedure;readonly ModuleRegistry registry;readonly string targetNodeId;readonly string procedureId;
 readonly Action<ConfigurationSubscriptionDestination,WorkflowValueReference?> apply;
 public static IReadOnlyList<string> UnsupportedCapabilities {get;}=Array.AsReadOnly(new[]{
  "GlobalVariables","LocalVariables","WorkflowInputs","ParentContainerInputs","ArrayIndex",
  "NativeCanSubscribeMetadata","NativeVisibilityMetadata","NativePrivateMetadata","NativeTimestampMetadata","NativeFieldHierarchy","NativeNumericWidth"});
 public WorkflowConfigurationSubscriptionService(Func<ProcedureDocument> getProcedure,ModuleRegistry registry,string targetNodeId,
  Action<ConfigurationSubscriptionDestination,WorkflowValueReference?> apply) {
  this.getProcedure=getProcedure??throw new ArgumentNullException(nameof(getProcedure));this.registry=registry??throw new ArgumentNullException(nameof(registry));
  ArgumentException.ThrowIfNullOrEmpty(targetNodeId);this.targetNodeId=targetNodeId;this.apply=apply??throw new ArgumentNullException(nameof(apply));
  procedureId=(getProcedure()??throw new InvalidDataException("No current procedure.")).Id;
 }
 public ConfigurationSubscriptionTreeResult Enumerate(ConfigurationSubscriptionQuery query) {
  try {
   ArgumentNullException.ThrowIfNull(query);var p=Snapshot();var destination=Target(p,query.Destination);
   if(query.IncludeSelf||query.IncludeInside)throw Unsupported("Self/internal module enumeration is outside this root-flow bridge.");
   if(query.IndexOnly)throw Unsupported("Array index reference enumeration is not represented by WorkflowValueReference.");
   var ancestors=Ancestors(p);var roots=new List<ConfigurationSubscriptionTreeNode>();
   foreach(var node in p.Nodes.Where(n=>ancestors.Contains(n.Id)&&n.Enabled)) {
    var descriptor=registry.GetDescriptor(node);var leaves=new List<ConfigurationSubscriptionTreeNode>();
    foreach(var port in descriptor.Outputs) {
     var native=NativeType(port.Kind);if(native==null||!CanFlow(port.Kind,destination)||!PermittedNative(query.Destination,native))continue;
     if(query.NativeTypes.Count>0&&!query.NativeTypes.Contains(native,StringComparer.Ordinal))continue;
     var reference=new ConfigurationOutputReference(p.Id,node.Id,port.Name,native);
     leaves.Add(new(Id("port",p.Id,node.Id,port.Name),port.Name,binding:new(reference,ConfigurationSubscriptionIndex.All),
      metadata:new ConfigurationSubscriptionPortMetadata()));
    }
    if(leaves.Count>0)roots.Add(new(Id("node",p.Id,node.Id),node.Name,leaves));
   }
   roots=roots.OrderBy(n=>n.CaptionKey,StringComparer.CurrentCulture).ToList();
   if(query.IncludeEmpty)roots.Insert(0,new("unbind","null",isUnbind:true));
   return new(ConfigurationSubscriptionStatus.Available,roots.AsReadOnly(),UnsupportedCapabilities:UnsupportedCapabilities);
  }catch(BridgeException e){return new(e.Status,Array.Empty<ConfigurationSubscriptionTreeNode>(),e.Message,UnsupportedCapabilities);}
   catch(Exception e){return new(ConfigurationSubscriptionStatus.Failed,Array.Empty<ConfigurationSubscriptionTreeNode>(),e.Message,UnsupportedCapabilities);}
 }
 public ConfigurationSubscriptionBindingResult Read(ConfigurationSubscriptionDestination destination) {
  try {var p=Snapshot();var target=Target(p,destination);return Decode(p,destination,target,Stored(p,destination));}
  catch(BridgeException e){return new(e.Status,null,Diagnostic:e.Message);}
  catch(Exception e){return new(ConfigurationSubscriptionStatus.Failed,null,Diagnostic:e.Message);}
 }
 public ConfigurationSubscriptionBindingResult Bind(ConfigurationSubscriptionDestination destination,ConfigurationSubscriptionBinding? binding) {
  try {
   var p=Snapshot();var target=Target(p,destination);WorkflowValueReference? reference=null;
   if(binding!=null) {
    if(binding.Index.Kind!=ConfigurationSubscriptionIndexKind.All)throw Unsupported("Literal/reference array indices are not represented by WorkflowValueReference.");
    var query=new ConfigurationSubscriptionQuery(destination,Array.AsReadOnly(destination.NativeType.Split(';',StringSplitOptions.RemoveEmptyEntries)));
    if(ConfigurationSubscriptionValidation.Check(query,binding) is {} error)throw new InvalidDataException(error);
    reference=Encode(p,destination,target,binding.Reference);
   }
   apply(destination,reference);
   var after=Snapshot();if(after.Id!=p.Id)throw new InvalidDataException("Current procedure changed during subscription apply.");
   var persisted=Stored(after,destination);
   if(persisted!=reference)throw new InvalidDataException("Host apply did not persist the requested typed binding.");
   return Decode(after,destination,Target(after,destination),persisted);
  }catch(BridgeException e){return new(e.Status,null,Diagnostic:e.Message);}
   catch(Exception e){return new(ConfigurationSubscriptionStatus.Failed,null,Diagnostic:e.Message);}
 }
 ProcedureDocument Snapshot() {
  var p=getProcedure()??throw new InvalidDataException("No current procedure.");
  if(p.Id!=procedureId)throw Unsupported("Subscription service is scoped to its original procedure identity.");
  if(p.GroupOwner!=null)throw Unsupported("Only the current root procedure is supported; parent/container inputs require additional metadata.");
  if(string.IsNullOrEmpty(p.Id)||p.Nodes.Any(n=>string.IsNullOrEmpty(n.Id))||p.Nodes.Select(n=>n.Id).Distinct(StringComparer.Ordinal).Count()!=p.Nodes.Count)throw new InvalidDataException("Missing or duplicate procedure/node identity.");
  if(p.Nodes.Count(n=>n.Id==targetNodeId)!=1)throw new InvalidDataException("Missing target node.");return p;
 }
 sealed record TargetSpec(DataKind Kind,PortDescriptor? Port,ParameterDescriptor? Parameter);
 TargetSpec Target(ProcedureDocument p,ConfigurationSubscriptionDestination destination) {
  ArgumentNullException.ThrowIfNull(destination);if(string.IsNullOrEmpty(destination.Name))throw new InvalidDataException("Missing destination identity.");
  var d=registry.GetDescriptor(p.Nodes.Single(n=>n.Id==targetNodeId));TargetSpec result;
  if(destination.Domain==ConfigurationSubscriptionDomain.Parameter) {
   var parameters=d.Parameters.Where(x=>x.Name==destination.Name).ToArray();if(parameters.Length!=1)throw Unsupported("Destination parameter is not uniquely declared: "+destination.Name);
   result=new(WorkflowSubscriptions.ParameterType(parameters[0].Kind),null,parameters[0]);
  }else {
   var ports=d.Inputs.Where(x=>x.Name==destination.Name).ToArray();if(ports.Length!=1)throw Unsupported("Destination input is not uniquely declared: "+destination.Name);
   result=new(ports[0].Kind,ports[0],null);
   if(destination.Domain==ConfigurationSubscriptionDomain.Combination&&!DataKinds.IsImage(result.Kind)&&result.Kind!=DataKind.Circle)throw Unsupported("This framework type does not represent the requested native combination exactly.");
  }
  var native=NativeType(result.Kind);if(native==null)throw Unsupported("No confirmed native type mapping for destination "+result.Kind);
  if(!PermittedNative(destination,native))throw Unsupported("Declared framework destination cannot represent native filter "+destination.NativeType);
  return result;
 }
 static string? NativeType(DataKind kind)=>kind switch {
  DataKind.Integer=>"int",DataKind.Number or DataKind.NumberArray=>"float",DataKind.Text=>"string",
  DataKind.GrayImage or DataKind.RgbImage or DataKind.Image=>"IMAGE",DataKind.Circle=>"CIRCLE",_=>null};
 static bool PermittedNative(ConfigurationSubscriptionDestination destination,string native)=>string.IsNullOrEmpty(destination.NativeType)||destination.NativeType.Split(';').Contains(native,StringComparer.Ordinal);
 static bool CanFlow(DataKind source,TargetSpec target)=>target.Port is {} port?DataKinds.CanFlow(source,port):DataKinds.CanFlow(source,target.Kind);
 HashSet<string> Ancestors(ProcedureDocument p) {
  if(p.ExecutionLinks==null)throw Unsupported("Explicit ExecutionLinks are required; node order and data connections cannot substitute for native predecessors.");
  var ids=p.Nodes.Select(x=>x.Id).ToHashSet(StringComparer.Ordinal);var before=ids.ToDictionary(x=>x,_=>new HashSet<string>(StringComparer.Ordinal),StringComparer.Ordinal);
  foreach(var e in p.ExecutionLinks){if(e==null||!ids.Contains(e.FromNode)||!ids.Contains(e.ToNode))throw new InvalidDataException("Execution link references a missing node.");before[e.ToNode].Add(e.FromNode);}
  var found=new HashSet<string>(StringComparer.Ordinal);var pending=new Stack<string>();pending.Push(targetNodeId);
  while(pending.TryPop(out var id)){if(!found.Add(id))continue;foreach(var previous in before[id])pending.Push(previous);}
  // Reject an ancestor cycle without recursive traversal or relying on storage order.
  var degree=found.ToDictionary(x=>x,x=>before[x].Count(found.Contains),StringComparer.Ordinal);var ready=new Queue<string>(degree.Where(x=>x.Value==0).Select(x=>x.Key));int visited=0;
  var next=found.ToDictionary(x=>x,_=>new List<string>(),StringComparer.Ordinal);foreach(var id in found)foreach(var previous in before[id])if(found.Contains(previous))next[previous].Add(id);
  while(ready.TryDequeue(out var id)){visited++;foreach(var successor in next[id])if(--degree[successor]==0)ready.Enqueue(successor);}
  if(visited!=found.Count)throw new InvalidDataException("Execution predecessor graph contains a cycle.");
  found.Remove(targetNodeId);return found;
 }
 WorkflowValueReference? Stored(ProcedureDocument p,ConfigurationSubscriptionDestination destination) {
  bool parameter=destination.Domain==ConfigurationSubscriptionDomain.Parameter;
  var configured=p.Configuration.Subscriptions.Where(s=>s.NodeId==targetNodeId&&s.Target==destination.Name&&s.IsParameter==parameter).Select(s=>s.Source).ToArray();
  var connected=parameter?Array.Empty<WorkflowValueReference>():p.Connections.Where(c=>c.ToNode==targetNodeId&&c.ToPort==destination.Name).Select(c=>new WorkflowValueReference(WorkflowSourceKind.NodeOutput,c.FromNode,c.FromPort)).ToArray();
  var all=configured.Concat(connected).ToArray();if(all.Length>1)throw new InvalidDataException("Destination has ambiguous/duplicate data connections or configuration subscriptions.");return all.SingleOrDefault();
 }
 WorkflowValueReference Encode(ProcedureDocument p,ConfigurationSubscriptionDestination destination,TargetSpec target,ConfigurationSubscriptionReference reference) {
  switch(reference) {
   case ConfigurationOutputReference output:
    if(output.ProcedureId!=p.Id)throw Unsupported("Cross-procedure references are outside the current root-flow bridge.");
    if(output.NodeId==targetNodeId||!Ancestors(p).Contains(output.NodeId))throw new InvalidDataException("Output source is not an execution predecessor.");
    var node=p.Nodes.SingleOrDefault(n=>n.Id==output.NodeId)??throw new InvalidDataException("Missing source node.");if(!node.Enabled)throw new InvalidDataException("Disabled source cannot supply an execution value.");
    var ports=registry.GetDescriptor(node).Outputs.Where(x=>x.Name==output.PortId).ToArray();if(ports.Length!=1)throw new InvalidDataException("Source output is missing or ambiguous.");
    var native=NativeType(ports[0].Kind);if(native==null||native!=output.NativeType)throw Unsupported("Output native type is not represented by the declared framework port.");
    if(!PermittedNative(destination,native)||!CanFlow(ports[0].Kind,target))throw new InvalidDataException("Source and destination types do not match.");
    return new(WorkflowSourceKind.NodeOutput,node.Id,ports[0].Name);
   case ConfigurationConstantReference constant:
    if(destination.Domain==ConfigurationSubscriptionDomain.Combination)throw Unsupported("Scalar constants cannot represent a native combination.");
    WorkflowValueReference result=constant.NativeType switch {
     "int"=>new(WorkflowSourceKind.IntegerConstant,int.Parse(constant.Literal,NumberStyles.Integer,CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture)),
     "float"=>Number(float.Parse(constant.Literal,NumberStyles.Float,CultureInfo.InvariantCulture)),
     "double"=>Number(double.Parse(constant.Literal,NumberStyles.Float,CultureInfo.InvariantCulture)),
     "string"=>new(WorkflowSourceKind.TextConstant,JsonSerializer.Serialize(constant.Literal)),
     _=>throw Unsupported("Unsupported scalar constant type.")};
    if(!CanFlow(WorkflowSubscriptions.SourceType(p,registry,result),target))throw new InvalidDataException("Constant and destination types do not match.");
    if(target.Parameter is {} parameter) {
     var element=constant.NativeType switch {"int"=>JsonSerializer.SerializeToElement(int.Parse(result.Id,CultureInfo.InvariantCulture)),"string"=>JsonSerializer.SerializeToElement(constant.Literal),_=>JsonSerializer.SerializeToElement(WorkflowSubscriptions.ReadNumberConstant(result.Id))};
     if(!GraphValidator.ValidParameter(parameter,element))throw new InvalidDataException("Constant is outside the declared parameter range/type.");
    }
    return result;
   default:throw Unsupported("Global/local variable, external and other reference kinds are not represented by this bridge.");
  }
 }
 static WorkflowValueReference Number(double number){if(!double.IsFinite(number))throw Unsupported("Framework numeric values require finite numbers; native Infinity is not represented.");return new(WorkflowSourceKind.NumberConstant,JsonSerializer.Serialize(number));}
 ConfigurationSubscriptionBindingResult Decode(ProcedureDocument p,ConfigurationSubscriptionDestination destination,TargetSpec target,WorkflowValueReference? source) {
  if(source==null)return new(ConfigurationSubscriptionStatus.Available,null,"");
  if(source.Kind is WorkflowSourceKind.IntegerConstant or WorkflowSourceKind.NumberConstant or WorkflowSourceKind.TextConstant) {
   if(source.Port!=null)throw new InvalidDataException("Scalar constant has an unexpected port identity.");
   if(!CanFlow(WorkflowSubscriptions.SourceType(p,registry,source),target))throw new InvalidDataException("Stored constant and destination types do not match.");
   if(target.Parameter is {} parameter&&!GraphValidator.ValidParameter(parameter,JsonSerializer.Deserialize<JsonElement>(source.Id)))throw new InvalidDataException("Stored constant is outside the declared parameter range/type.");
  }
  ConfigurationSubscriptionReference reference;string presentation;
  switch(source.Kind) {
   case WorkflowSourceKind.NodeOutput:
    var node=p.Nodes.SingleOrDefault(n=>n.Id==source.Id)??throw new InvalidDataException("Stored binding source node is missing.");
    var output=registry.GetDescriptor(node).Outputs.SingleOrDefault(x=>x.Name==source.Port)??throw new InvalidDataException("Stored binding source port is missing.");
    reference=new ConfigurationOutputReference(p.Id,node.Id,output.Name,NativeType(output.Kind)??throw Unsupported("Stored output type is unsupported."));
    _=Encode(p,destination,target,reference);presentation=node.Name+"."+output.Name+"[]";break;
   case WorkflowSourceKind.IntegerConstant:
    reference=new ConfigurationConstantReference("int",int.Parse(source.Id,NumberStyles.Integer,CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture));presentation=((ConfigurationConstantReference)reference).Literal;
    _=Encode(p,destination,target,reference);break;
   case WorkflowSourceKind.NumberConstant:
    reference=new ConfigurationConstantReference("float",WorkflowSubscriptions.ReadNumberConstant(source.Id).ToString("R",CultureInfo.InvariantCulture));presentation=((ConfigurationConstantReference)reference).Literal;
    _=Encode(p,destination,target,reference);break;
   case WorkflowSourceKind.TextConstant:
    reference=new ConfigurationConstantReference("string",WorkflowSubscriptions.ReadTextConstant(source.Id));presentation=((ConfigurationConstantReference)reference).Literal;
    _=Encode(p,destination,target,reference);break;
   default:throw Unsupported("Stored workflow/local/global/index/array-constant reference cannot be represented by this supported subset.");
  }
  return new(ConfigurationSubscriptionStatus.Available,new(reference,ConfigurationSubscriptionIndex.All),presentation);
 }
 static string Id(string prefix,params string[] fields)=>prefix+":"+JsonSerializer.Serialize(fields);
 static BridgeException Unsupported(string message)=>new(ConfigurationSubscriptionStatus.Unsupported,message);
 sealed class BridgeException(ConfigurationSubscriptionStatus status,string message):Exception(message){public ConfigurationSubscriptionStatus Status {get;}=status;}
}
