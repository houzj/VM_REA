using VisionFramework;
namespace VisionPluginContracts;
/// <summary>POINT and X/Y are views over the same stored leaf bindings. Managed batch atomicity
/// is a document safety rule, not a claim about native per-leaf failure rollback.</summary>
public sealed class PointAliasConfigurationSubscriptions:IToolConfigurationSubscriptionService {
 readonly IToolConfigurationSubscriptionBatchService inner;readonly Func<ProcedureDocument> flow;readonly ModuleRegistry registry;readonly string target,procedure;readonly IReadOnlyList<ToolPointInputAlias> aliases;
 public PointAliasConfigurationSubscriptions(IToolConfigurationSubscriptionBatchService inner,Func<ProcedureDocument> flow,ModuleRegistry registry,string target,IReadOnlyList<ToolPointInputAlias> aliases){this.inner=inner;this.flow=flow;this.registry=registry;this.target=target;this.aliases=aliases;procedure=flow().Id;}
 static ConfigurationSubscriptionDestination Axis(string name)=>new(ConfigurationSubscriptionDomain.InputIo,name,"float");
 static ConfigurationSubscriptionDestination Combination(string name)=>new(ConfigurationSubscriptionDomain.Combination,name,"POINT");
 ToolPointInputAlias? Alias(ConfigurationSubscriptionDestination d)=>d.Domain is ConfigurationSubscriptionDomain.Parameter or ConfigurationSubscriptionDomain.Display?null:aliases.SingleOrDefault(a=>a.Combination==d.Name||a.X==d.Name||a.Y==d.Name);
 ProcedureDocument Snapshot(){var p=flow();if(p.Id!=procedure||p.Nodes.Count(n=>n.Id==target)!=1)throw new InvalidOperationException("POINT binding owner is no longer active.");return p;}
 ConfigurationSubscriptionBinding? Component(ConfigurationSubscriptionBinding? binding,string axis){
  if(binding is null)return null;if(binding.Index.Kind!=ConfigurationSubscriptionIndexKind.All)throw new NotSupportedException("POINT index projection is not represented by the workflow document.");
  if(binding.Reference is not ConfigurationOutputReference output||output.NativeType!="POINT"||output.ProcedureId!=procedure)throw new NotSupportedException("POINT source must be a same-procedure typed output.");
  var source=Snapshot().Nodes.Single(n=>n.Id==output.NodeId);var descriptor=registry.GetDescriptor(source);if(!descriptor.Outputs.Any(p=>p.Name==output.PortId&&p.Kind==DataKind.Point))throw new InvalidOperationException("POINT source is missing.");
  var leaf=descriptor.Outputs.SingleOrDefault(p=>p.Name==output.PortId+"."+axis);if(leaf is null)throw new NotSupportedException("POINT component projection is unavailable.");
  return new(new ConfigurationOutputReference(procedure,output.NodeId,leaf.Name,"float"),ConfigurationSubscriptionIndex.All);
 }
 ConfigurationSubscriptionBinding? Parent(ConfigurationSubscriptionBinding? x,ConfigurationSubscriptionBinding? y){
  if(x?.Reference is not ConfigurationOutputReference a||y?.Reference is not ConfigurationOutputReference b||a.ProcedureId!=procedure||b.ProcedureId!=procedure||a.NodeId!=b.NodeId||x.Index!=y.Index)return null;
  var node=Snapshot().Nodes.SingleOrDefault(n=>n.Id==a.NodeId);if(node is null)return null;
  var port=registry.GetDescriptor(node).Outputs.FirstOrDefault(p=>p.Kind==DataKind.Point&&a.PortId==p.Name+".X"&&b.PortId==p.Name+".Y");
  return port is null?null:new(new ConfigurationOutputReference(procedure,a.NodeId,port.Name,"POINT"),x.Index);
 }
 (ConfigurationSubscriptionBindingResult X,ConfigurationSubscriptionBindingResult Y) Leaves(ToolPointInputAlias alias){
  var x=inner.Read(Axis(alias.X));var y=inner.Read(Axis(alias.Y));if(x.Status!=ConfigurationSubscriptionStatus.Available||y.Status!=ConfigurationSubscriptionStatus.Available)return (x,y);
  var legacy=inner.Read(Combination(alias.Combination));if(legacy.Status==ConfigurationSubscriptionStatus.Available&&legacy.Binding is {} binding){if(x.Binding is null)x=new(ConfigurationSubscriptionStatus.Available,Component(binding,"X"),legacy.DisplayText+".X");if(y.Binding is null)y=new(ConfigurationSubscriptionStatus.Available,Component(binding,"Y"),legacy.DisplayText+".Y");}
  return (x,y);
 }
 public ConfigurationSubscriptionBindingResult Read(ConfigurationSubscriptionDestination destination){
  var alias=Alias(destination);if(alias is null)return inner.Read(destination);
  try{Snapshot();var (x,y)=Leaves(alias);if(x.Status!=ConfigurationSubscriptionStatus.Available)return x;if(y.Status!=ConfigurationSubscriptionStatus.Available)return y;
   if(destination.Name==alias.X)return x;if(destination.Name==alias.Y)return y;
   if(x.Binding is null&&y.Binding is null)return new(ConfigurationSubscriptionStatus.Available,null);
   var parent=Parent(x.Binding,y.Binding);if(parent is not null){var output=(ConfigurationOutputReference)parent.Reference;var node=Snapshot().Nodes.Single(n=>n.Id==output.NodeId);return new(ConfigurationSubscriptionStatus.Available,parent,node.Name+"."+output.PortId);}
   return new(ConfigurationSubscriptionStatus.Available,new(new ConfigurationPointTupleReference(x.Binding,y.Binding),ConfigurationSubscriptionIndex.All),"("+(x.DisplayText??"null")+","+(y.DisplayText??"null")+")"){ReadOnly=true};
  }catch(Exception error){return new(ConfigurationSubscriptionStatus.Failed,null,Diagnostic:error.Message);}
 }
 public ConfigurationSubscriptionBindingResult Bind(ConfigurationSubscriptionDestination destination,ConfigurationSubscriptionBinding? binding){
  var alias=Alias(destination);if(alias is null)return inner.Bind(destination,binding);
  try{Snapshot();ConfigurationSubscriptionBinding? x,y;
   if(destination.Name==alias.Combination){x=Component(binding,"X");y=Component(binding,"Y");}
   else{var leaves=Leaves(alias);if(leaves.X.Status!=ConfigurationSubscriptionStatus.Available||leaves.Y.Status!=ConfigurationSubscriptionStatus.Available)throw new InvalidOperationException("Existing POINT leaf binding is unavailable.");x=destination.Name==alias.X?binding:leaves.X.Binding;y=destination.Name==alias.Y?binding:leaves.Y.Binding;}
   var result=inner.BindMany(new[]{new ConfigurationSubscriptionChange(Axis(alias.X),x),new(Axis(alias.Y),y),new(Combination(alias.Combination),null)});
   return result.Status==ConfigurationSubscriptionStatus.Available?Read(destination):new(result.Status,null,Diagnostic:result.Diagnostic);
  }catch(NotSupportedException error){return new(ConfigurationSubscriptionStatus.Unsupported,null,Diagnostic:error.Message);}catch(Exception error){return new(ConfigurationSubscriptionStatus.Failed,null,Diagnostic:error.Message);}
 }
 public ConfigurationSubscriptionTreeResult Enumerate(ConfigurationSubscriptionQuery query){
  var alias=Alias(query.Destination);if(alias is null||query.Destination.Name!=alias.Combination)return inner.Enumerate(query);
  try{
   Snapshot();if(query.IndexOnly||query.IncludeSelf||query.IncludeInside)return new(ConfigurationSubscriptionStatus.Unsupported,[],"Unsupported POINT query scope or index.");
   var x=inner.Enumerate(new(Axis(alias.X),new[]{"float"},IncludeEmpty:false));if(x.Status!=ConfigurationSubscriptionStatus.Available)return x;
   IEnumerable<ConfigurationSubscriptionTreeNode> Walk(IEnumerable<ConfigurationSubscriptionTreeNode> nodes)=>nodes.SelectMany(n=>new[]{n}.Concat(Walk(n.Children)));
   var roots=new List<ConfigurationSubscriptionTreeNode>();
   foreach(var source in Snapshot().Nodes){var leaves=new List<ConfigurationSubscriptionTreeNode>();foreach(var port in registry.GetDescriptor(source).Outputs.Where(p=>p.Kind==DataKind.Point)){
     var point=new ConfigurationSubscriptionBinding(new ConfigurationOutputReference(procedure,source.Id,port.Name,"POINT"),ConfigurationSubscriptionIndex.All);var field=Component(point,"X");if(!Walk(x.Roots).Any(n=>n.Binding==field))continue;
     leaves.Add(new("point:"+source.Id+":"+port.Name,port.Name,binding:point));
    }if(leaves.Count>0)roots.Add(new("point-node:"+source.Id,source.Name,leaves));}
   if(query.IncludeEmpty)roots.Insert(0,new("point-unbind","null",isUnbind:true));return new(ConfigurationSubscriptionStatus.Available,roots);
  }catch(Exception error){return new(ConfigurationSubscriptionStatus.Failed,[],error.Message);}
 }
}
