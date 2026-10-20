using System.Collections.ObjectModel;
using VisionFramework;
using System.Globalization;

namespace VisionPluginContracts;

public enum ConfigurationSubscriptionDomain { InputIo, Combination, Parameter, Display }
public enum ConfigurationSubscriptionStatus { Available, Unsupported, Failed }
public enum ConfigurationSubscriptionIndexKind { All, Literal, Reference }
/// <summary>Identity is never recovered by parsing the localized caption or displayed subscription text.</summary>
public abstract record ConfigurationSubscriptionReference;
public sealed record ConfigurationOutputReference(string ProcedureId,string NodeId,string PortId,string NativeType):ConfigurationSubscriptionReference;
public sealed record ConfigurationGlobalReference(string VariableId,string NativeType):ConfigurationSubscriptionReference;
public sealed record ConfigurationExternalReference(string ChannelId,string NativeType):ConfigurationSubscriptionReference;
public sealed record ConfigurationConstantReference(string NativeType,string Literal):ConfigurationSubscriptionReference;
public sealed record ConfigurationSubscriptionIndex {
 public ConfigurationSubscriptionIndexKind Kind {get;}
 public int? Literal {get;}
 public string? LiteralText {get;init;}
 public ConfigurationSubscriptionReference? Reference {get;}
 private ConfigurationSubscriptionIndex(ConfigurationSubscriptionIndexKind kind,int? literal=null,ConfigurationSubscriptionReference? reference=null){Kind=kind;Literal=literal;Reference=reference;}
 public static ConfigurationSubscriptionIndex All {get;}=new(ConfigurationSubscriptionIndexKind.All);
 public static ConfigurationSubscriptionIndex At(int index)=>At(index.ToString(CultureInfo.InvariantCulture));
 public static ConfigurationSubscriptionIndex At(string literal)=>new(ConfigurationSubscriptionIndexKind.Literal,WorkflowArrayIndex.ParseLiteral(literal)){LiteralText=literal};
 public static ConfigurationSubscriptionIndex From(ConfigurationSubscriptionReference reference) {
  ArgumentNullException.ThrowIfNull(reference);
  if(reference is ConfigurationConstantReference)throw new ArgumentException("Use At for a constant index.",nameof(reference));
  if(ConfigurationSubscriptionValidation.NativeType(reference)!="int")throw new ArgumentException("Index reference must be int.",nameof(reference));
  return new(ConfigurationSubscriptionIndexKind.Reference,reference:reference);
 }
}
public sealed record ConfigurationSubscriptionBinding(ConfigurationSubscriptionReference Reference,ConfigurationSubscriptionIndex Index);
public sealed record ConfigurationSubscriptionDestination(ConfigurationSubscriptionDomain Domain,string Name,string NativeType);
public sealed record ConfigurationSubscriptionQuery(ConfigurationSubscriptionDestination Destination,IReadOnlyList<string> NativeTypes,
 bool IncludeSelf=false,bool IncludeInside=false,bool IncludeEmpty=true,bool IndexOnly=false);
/// <summary>Non-selectable groups have no binding. An explicit empty leaf has IsUnbind=true.</summary>
public sealed record ConfigurationSubscriptionPortMetadata(bool? CanSubscribe=null,bool? Visible=null,bool? IsPrivate=null,
 string? OwnerContainerId=null,IReadOnlyList<string>? NativeFieldPath=null);
public sealed class ConfigurationSubscriptionTreeNode {
 public ConfigurationSubscriptionPortMetadata? Metadata {get;}
 public string StableId {get;}
 public string CaptionKey {get;}
 public IReadOnlyList<ConfigurationSubscriptionTreeNode> Children {get;}
 public ConfigurationSubscriptionBinding? Binding {get;}
 public bool IsUnbind {get;}
 public bool IsSelectable=>Children.Count==0&&(Binding!=null||IsUnbind);
 public ConfigurationSubscriptionTreeNode(string stableId,string captionKey,IEnumerable<ConfigurationSubscriptionTreeNode>? children=null,ConfigurationSubscriptionBinding? binding=null,bool isUnbind=false,ConfigurationSubscriptionPortMetadata? metadata=null) {
  ArgumentException.ThrowIfNullOrEmpty(stableId);StableId=stableId;CaptionKey=captionKey??throw new ArgumentNullException(nameof(captionKey));
  Children=Array.AsReadOnly((children??[]).ToArray());Binding=binding;IsUnbind=isUnbind;Metadata=metadata;
  if(Children.Count>0&&(binding!=null||isUnbind))throw new ArgumentException("A group cannot carry a selectable binding.");
  if(binding!=null&&isUnbind)throw new ArgumentException("An unbind leaf cannot carry a binding.");
 }
}
public sealed record ConfigurationSubscriptionTreeResult(ConfigurationSubscriptionStatus Status,IReadOnlyList<ConfigurationSubscriptionTreeNode> Roots,string? Diagnostic=null,IReadOnlyList<string>? UnsupportedCapabilities=null);
/// <summary>DisplayText is service-produced presentation only. Binding remains the authority.</summary>
public sealed record ConfigurationSubscriptionBindingResult(ConfigurationSubscriptionStatus Status,ConfigurationSubscriptionBinding? Binding,string? DisplayText=null,string? Diagnostic=null){public bool ReadOnly{get;init;}}
public interface IToolConfigurationSubscriptionService {
 ConfigurationSubscriptionTreeResult Enumerate(ConfigurationSubscriptionQuery query);
 ConfigurationSubscriptionBindingResult Read(ConfigurationSubscriptionDestination destination);
 ConfigurationSubscriptionBindingResult Bind(ConfigurationSubscriptionDestination destination,ConfigurationSubscriptionBinding? binding);
}
public static class ConfigurationSubscriptionValidation {
 public static string NativeType(ConfigurationSubscriptionReference reference)=>reference switch {
  ConfigurationOutputReference x=>x.NativeType,ConfigurationGlobalReference x=>x.NativeType,
  ConfigurationExternalReference x=>x.NativeType,ConfigurationConstantReference x=>x.NativeType,ConfigurationPointTupleReference=>"POINT",_=>""};
 public static string? Check(ConfigurationSubscriptionQuery query,ConfigurationSubscriptionBinding binding) {
  ArgumentNullException.ThrowIfNull(query);ArgumentNullException.ThrowIfNull(binding);
  string nativeType=NativeType(binding.Reference);
  if(query.NativeTypes.Count>0&&!query.NativeTypes.Contains(nativeType,StringComparer.Ordinal))return "Reference type is excluded by the original query filter.";
  if(query.IndexOnly&&nativeType!="int")return "Index selection requires an int reference.";
  if(binding.Reference is ConfigurationOutputReference output&&(string.IsNullOrEmpty(output.ProcedureId)||string.IsNullOrEmpty(output.NodeId)||string.IsNullOrEmpty(output.PortId)))return "Output identity is incomplete.";
  if(binding.Reference is ConfigurationGlobalReference global&&string.IsNullOrEmpty(global.VariableId))return "Global variable identity is incomplete.";
  if(binding.Reference is ConfigurationExternalReference external&&string.IsNullOrEmpty(external.ChannelId))return "External channel identity is incomplete.";
  if(binding.Reference is ConfigurationConstantReference constant) {
   bool valid=constant.NativeType switch {
    "int"=>int.TryParse(constant.Literal,NumberStyles.Integer,CultureInfo.InvariantCulture,out _),
    "float"=>float.TryParse(constant.Literal,NumberStyles.Float,CultureInfo.InvariantCulture,out var f)&&!float.IsNaN(f),
    "double"=>double.TryParse(constant.Literal,NumberStyles.Float,CultureInfo.InvariantCulture,out var d)&&!double.IsNaN(d),
    "string"=>true,_=>false};
   if(!valid)return "Invalid typed constant. Original float/double constant validation rejects NaN.";
   if(binding.Index.Kind!=ConfigurationSubscriptionIndexKind.All)return "A scalar constant cannot have an array index.";
  }
  return null;
 }
 public static string? CheckTree(ConfigurationSubscriptionTreeResult tree) {
  var ids=new HashSet<string>(StringComparer.Ordinal);int count=0;
  string? Walk(IEnumerable<ConfigurationSubscriptionTreeNode> nodes,int depth) {
   if(depth>128)return "Subscription tree nesting exceeds 128.";
   foreach(var n in nodes){if(++count>100000)return "Subscription tree exceeds 100000 nodes.";if(!ids.Add(n.StableId))return "Duplicate stable tree identity: "+n.StableId;var error=Walk(n.Children,depth+1);if(error!=null)return error;}return null;
  }
  return Walk(tree.Roots,0);
 }
 /// <summary>Reads the source-declared operation family and native filter, never a label-derived destination.</summary>
 public static ConfigurationSubscriptionQuery? FromElement(ConfigurationElement element,ConfigurationSubscriptionDomain? domain=null) {
  string? F(ConfigurationElement e,string key)=>e.Children.FirstOrDefault(c=>c.Tag==key)?.Text??e.Attributes.GetValueOrDefault(key);
  var drop=element.Children.FirstOrDefault(x=>x.Tag=="DropOpenSetter");
  // Some original EnumerationG declarations initialize this list once instead of a DropOpenSetter.
  if(drop is null&&element.Tag=="EnumerationG")drop=element.Children.FirstOrDefault(x=>x.Tag=="Initers")?.Children.FirstOrDefault(x=>F(x,"OperationName")=="GetFrontParamItemsOperation");
  if(domain==ConfigurationSubscriptionDomain.Parameter) {
   string type=F(element,"ComType")??element.Tag switch {"Integer"=>"int","Float"=>"float","Boolean"=>"int","String" or "OpenFileForCalibDialog"=>"string",_=>""};
   return new(new(ConfigurationSubscriptionDomain.Parameter,(element.Name??"")+(F(element,"ComTailName")??""),type),Array.AsReadOnly(type.Split(';',StringSplitOptions.RemoveEmptyEntries)));
  }
  if(drop==null)return null;
  var op=F(drop,"OperationName");if(op is not "GetFrontFilterTreeOperation" and not "GetFrontCombinationTreeOperation" and not "GetFrontAndSelfFilterTreeOperation" and not "GetFrontParamItemsOperation")return null;
  var selectedDomain=op=="GetFrontAndSelfFilterTreeOperation"?ConfigurationSubscriptionDomain.Display:op=="GetFrontCombinationTreeOperation"?ConfigurationSubscriptionDomain.Combination:ConfigurationSubscriptionDomain.InputIo;
  var types=(F(drop,"OperationParams")??"").Split(';',StringSplitOptions.RemoveEmptyEntries);
  var sets=element.Children.Where(x=>x.Tag=="Triggers").SelectMany(x=>x.Children).SelectMany(x=>x.Children.Where(c=>c.Tag=="Setters")).SelectMany(x=>x.Children);
  var setter=sets.FirstOrDefault(x=>F(x,"OperationName") is "SetResultIOToModuleOperation" or "SetFilterSourceOperation" or "SetCombinationSourceOperation");
  var destination=setter==null?null:F(setter,"OperationParams")?.Split(';').FirstOrDefault();
  if(string.IsNullOrEmpty(destination))return null;
  if(op=="GetFrontParamItemsOperation"&&F(setter!,"OperationName")=="SetCombinationSourceOperation")selectedDomain=ConfigurationSubscriptionDomain.Combination;
  return new(new(selectedDomain,destination,string.Join(';',types)),Array.AsReadOnly(types),IncludeSelf:selectedDomain==ConfigurationSubscriptionDomain.Display);
 }
}
