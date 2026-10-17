using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace VisionPluginContracts;

public enum ConfigurationChangeKind { Initialization, Programmatic, UserCommit, Command }
public enum ConfigurationOperationKind {
 Unknown, ReadParameter, WriteParameter, PersistUserValue, ReadInputBinding, WriteInputBinding,
 EnumerateInputs, ReadParameterBinding, WriteParameterBinding, ClearParameterBinding,
 Run, SaveCalibration, Clear, Close, ResetParameters, OpenFile, SaveFile, Custom, RunContinuous, StopContinuous, ClearCalibrationPoints, ReadUserValue
}
public sealed record ConfigurationOperationRequest(ConfigurationOperationKind Kind, string OriginalName,
 ConfigurationNodeState Source, ConfigurationNodeState? Target, IReadOnlyList<string> Parameters,
 ConfigurationChangeKind ChangeKind, string? Value = null) {
 public string? TargetName {get;init;}
 public ConfigurationSubscriptionBinding? SubscriptionBinding {get;init;}
}
public sealed record ConfigurationChoice(string CaptionKey,string Value);
public sealed record ConfigurationOperationResult(bool Handled, string? Value = null, string? Error = null,
 IReadOnlyList<ConfigurationChoice>? Choices = null);
public interface IToolConfigurationDispatcher {
 ConfigurationOperationResult Dispatch(ConfigurationOperationRequest request);
}
public interface IToolConfigurationRule {
 void Apply(ToolConfigurationState state, ConfigurationOperationRequest request);
 void Revert(ToolConfigurationState state, ConfigurationOperationRequest request);
}
public sealed record ConfigurationDiagnostic(string Code, string Operation, string? Node, string Message);

/// <summary>Mutable instance state; definition elements remain immutable. Changed fires synchronously on the caller thread.</summary>
public sealed class ConfigurationNodeState {
 // Explicit source-confirmed CLR inheritance, not a name-prefix guess. Integer64 is deliberately excluded.
 static readonly HashSet<string> Integer32Types=new(StringComparer.Ordinal){"Integer","EditButton_Calib","IntegerBx","IntegerConfirm","Integer_CircleFind","Integer_CirclePairInsp","Integer_CaliperHeight","Integer_CaliperHeight2","Integer_IdeaWidth"};
 static readonly HashSet<string> Enum32Types=new(StringComparer.Ordinal){"Enumeration","EnumerationAllCom","EnumerationBx","EnumerationBxDynamic","EnumerationCom","EnumerationDataCenter","EnumerationGain","EnumerationTrigger","EnumerationVar","Enumeration_ModuleSelector"};
 static readonly HashSet<string> Radio32Types=new(StringComparer.Ordinal){"RadioSelecter","RadioSelecter3","RadioSelecterEx2","RadioSelecterEx3"};
 readonly ToolConfigurationState owner;
 readonly Dictionary<string,string> values = new(StringComparer.Ordinal);
 internal ConfigurationNodeState(ToolConfigurationState owner, ConfigurationElement element) {
  this.owner=owner; Element=element;
  foreach(var a in element.Attributes) values[a.Key]=a.Value;
  foreach(var e in element.Children.Where(x=>x.Children.Count==0)) values[e.Tag]=e.Text;
  Choices=Array.AsReadOnly((element.Children.FirstOrDefault(x=>x.Tag=="EnumEntrys")?.Children??[]).Select(e=>new ConfigurationChoice(e.Children.FirstOrDefault(c=>c.Tag=="DisplayName")?.Text??"",NormalizeEntryValue(e.Children.FirstOrDefault(c=>c.Tag=="Value")?.Text??"0"))).ToArray());
  if(!values.ContainsKey("CurValue")) values["CurValue"]=element.Tag switch {
   "Boolean" or "Expand"=>"False", "Integer" or "Float" or "RadioSelecter" or "Enumeration"=>"0", _=>"" };
  if(IsInteger32)foreach(var field in new[]{"CurValue","DefaultValue","MinValue","MaxValue","IncValue"})values[field]=ParseInt32(values.GetValueOrDefault(field,field=="IncValue"?"1":"0"));
  else if(Enum32Types.Contains(element.Tag)||Radio32Types.Contains(element.Tag))foreach(var field in new[]{"CurValue","DefaultValue"})values[field]=ParseInt32(values.GetValueOrDefault(field,"0"));
 }
 static string NormalizeEntryValue(string value)=>ParseInt32(value);
 // Original Hik.Util.StringUtil.ParseInt: signed Int32 HexNumber; malformed/overflow returns 0.
 static string ParseInt32(string value) {
  var hex=value.StartsWith("0x",StringComparison.OrdinalIgnoreCase);
  return (int.TryParse(hex?value[2..]:value,hex?NumberStyles.HexNumber:NumberStyles.Integer,CultureInfo.InvariantCulture,out var number)?number:0).ToString(CultureInfo.InvariantCulture);
 }
 public bool IsInteger32=>Integer32Types.Contains(Element.Tag);
 internal string NormalizeTypedProperty(string property,string value)=>
  IsInteger32&&property is "CurValue" or "DefaultValue" or "MinValue" or "MaxValue" or "IncValue" ||
  (Enum32Types.Contains(Element.Tag)||Radio32Types.Contains(Element.Tag))&&property is "CurValue" or "DefaultValue"?ParseInt32(value):value;
 public ConfigurationSubscriptionBinding? SubscriptionBinding {get;internal set;}
 public IReadOnlyList<ConfigurationChoice> Choices {get;internal set;}=Array.Empty<ConfigurationChoice>();
 public ConfigurationElement Element {get;}
 public string? Name=>Element.Name??Get("Name");
 public bool IsCategory=>Element.Tag is "Category" or "CategoryNoHead";
 public bool IsPanel {
  get {
   if(Element.Tag!="Tab")return false;var raw=Get("TabType")??"0";
   return int.TryParse(raw,NumberStyles.Integer,CultureInfo.InvariantCulture,out var decimalType)&&decimalType==1||raw.StartsWith("0x",StringComparison.OrdinalIgnoreCase)&&int.TryParse(raw[2..],NumberStyles.HexNumber,CultureInfo.InvariantCulture,out var number)&&number==1;
  }
 }
 public ConfigurationNodeState? Parent=>owner.NearestParent(Element.Parent);
 public ConfigurationTemplateResolution Template=>owner.ResolveTemplate(this);
 public string DisplayType=>Template.EffectiveDisplayType??"";
 public string CaptionKey=>Get("DisplayName") ?? Name ?? "";
 public string Value=>Get("CurValue")??"";
 public bool CustomVisible=>Bool("CustomVisible",true);
 public bool Visible=>CustomVisible && (Element.Tag!="Tab"||Bool("IsVisible",true)) && (!IsCategory||owner.Nodes.Any(x=>ReferenceEquals(x.Parent,this)&&x.CustomVisible)) && (Parent?.Visible??true);
 public bool Enabled=>Bool("Enable",true) && (Parent?.Enabled??true);
 public bool CanSubscribe=>Parent?.IsCategory!=true || Parent.Parent?.IsPanel!=true&&Parent.Bool("SubscribeVisible",true);
 public bool SubscribeVisible=>Bool("SubscribeVisible",true)&&CanSubscribe;
 public bool IsDeleteSubscribe=>Bool("IsDeleteSubscribe",true) && (Parent?.IsDeleteSubscribe??true);
 public bool CanTriggerRun=>IsCategory?Bool("CanTriggerRun",false):(Parent?.CanTriggerRun??false);
 public string? Get(string property) { property=Normalize(property); return values.GetValueOrDefault(property); }
 internal bool Bool(string property,bool fallback)=>bool.TryParse(Get(property),out var b)?b:fallback;
 internal bool Assign(string property,string value) {property=Normalize(property); if(values.GetValueOrDefault(property)==value)return false; values[property]=value;return true;}
 internal static string Normalize(string name)=>name.EndsWith('A')?name[..^1]:name;
}

/// <summary>Evidence-scoped XML rules engine. It never invokes arbitrary CLR methods from XML.</summary>
public sealed class ToolConfigurationState {
 // PointPair declares these fields as ButtonSelecter instances in Apps.XmlParser.ParamTab.PointPair.
 // XML field tags are local member names, not globally named configuration controls.
 static readonly HashSet<string> PointPairSelectors=new(StringComparer.Ordinal){"ObjectPoint","TargetPoint","ObjectPointX","TargetPointX","ObjectPointY","TargetPointY","ObjectPointAngle","TargetPointAngle"};
 static bool IsPointPairSelector(ConfigurationElement element)=>element.Parent?.Tag=="PointPair"&&PointPairSelectors.Contains(element.Tag);
 readonly Dictionary<ConfigurationElement,ConfigurationNodeState> byElement=[];
 readonly Dictionary<ConfigurationElement,IReadOnlyList<ConfigurationElement>> childOrders=[];
 readonly Dictionary<ConfigurationElement,bool> working=[];
 readonly Dictionary<string,IToolConfigurationRule> rules=new(StringComparer.Ordinal);
 readonly Dictionary<string,Func<ConfigurationNodeState,string?>> displayTypeResolvers=new(StringComparer.Ordinal);
 readonly Dictionary<ConfigurationElement,ConfigurationOperationRequest> ruleRequests=[];
 readonly Dictionary<ConfigurationElement,bool> originalVisibility=[];
 readonly List<ConfigurationDiagnostic> diagnostics=[];
 readonly Dictionary<ConfigurationNodeState,string> committedValues=[];
 readonly IToolConfigurationDispatcher? dispatcher;
 int depth; int subscriptionWriteDepth; bool initializing; ConfigurationNodeState? pendingRun;
 public ToolConfigurationDefinition Definition {get;}
 public IReadOnlyList<ConfigurationNodeState> Nodes {get;}
 public IReadOnlyList<ConfigurationDiagnostic> Diagnostics=>diagnostics.AsReadOnly();
 public IReadOnlyList<string> UnsupportedOperations=>diagnostics.Where(x=>x.Code=="UnsupportedOperation").Select(x=>x.Operation).Distinct().ToArray();
 public bool ExecutionEnabled {get;set;}
 public bool IsBusy {get;set;}
 public event Action? Changed;
 public ToolConfigurationState(ToolConfigurationDefinition definition, IToolConfigurationDispatcher? dispatcher=null) {
  Definition=definition??throw new ArgumentNullException(nameof(definition));this.dispatcher=dispatcher;
  Walk(definition.Root);Nodes=Array.AsReadOnly(byElement.Values.ToArray());
  foreach(var node in Nodes)committedValues[node]=node.Value;
 }
 void Walk(ConfigurationElement e) {
  if(e.Tag is "Category" or "CategoryNoHead" or "Tab" || e.Parent?.Tag is "Items" or "BottomExcuteButtons" or "BottomExcuteButtonsLeft" or "ExternParamsList" || IsPointPairSelector(e))
   byElement[e]=new(this,e);
  foreach(var c in e.Children)Walk(c);
 }
 public void RegisterDisplayTypeResolver(string nodeType,Func<ConfigurationNodeState,string?> resolver) {
  ArgumentException.ThrowIfNullOrEmpty(nodeType);ArgumentNullException.ThrowIfNull(resolver);displayTypeResolvers.Add(nodeType,resolver);
 }
 internal ConfigurationTemplateResolution ResolveTemplate(ConfigurationNodeState node) {
  var evaluated=displayTypeResolvers.TryGetValue(node.Element.Tag,out var resolver)?resolver(node):null;
  var result=ConfigurationTemplateCatalog.Resolve(IsPointPairSelector(node.Element)?"ButtonSelecter":node.Element.Tag,evaluated);
  if(!result.IsRecognized&&node.Element.Tag is not "Tab" and not "Category")Diagnose("UnsupportedTemplate",node.Element.Tag,node,result.Support+": "+result.Rule);
  return result;
 }
 public void RegisterRule(string originalOperationName,IToolConfigurationRule rule) {
  ArgumentException.ThrowIfNullOrEmpty(originalOperationName);ArgumentNullException.ThrowIfNull(rule);
  rules.Add(originalOperationName,rule);
 }
 public ConfigurationNodeState? Find(string name)=>Nodes.FirstOrDefault(x=>x.Name==name);
 public ConfigurationNodeState? GetNode(ConfigurationElement element)=>byElement.GetValueOrDefault(element);
 public IReadOnlyList<ConfigurationElement> GetChildren(ConfigurationElement parent)=>childOrders.GetValueOrDefault(parent)??parent.Children;
 /// <summary>Runtime collection ordering only; immutable source XML and persisted algorithm parameters are untouched.</summary>
 public void MoveBefore(ConfigurationElement element,ConfigurationElement before) {
  if(ReferenceEquals(element,before))return;
  var parent=element.Parent;
  if(parent is null||!ReferenceEquals(parent,before.Parent)||!byElement.ContainsKey(element)||!byElement.ContainsKey(before))throw new ArgumentException("Move requires registered sibling controls.");
  var ordered=GetChildren(parent).ToList();ordered.Remove(element);ordered.Insert(ordered.IndexOf(before),element);childOrders[parent]=ordered.AsReadOnly();Changed?.Invoke();
 }
 internal ConfigurationNodeState? NearestParent(ConfigurationElement? e) {for(;e!=null;e=e.Parent)if(byElement.TryGetValue(e,out var n))return n;return null;}
 static string? Field(ConfigurationElement e,string key)=>e.Children.FirstOrDefault(x=>x.Tag==key)?.Text??e.Attributes.GetValueOrDefault(key);
 static IEnumerable<ConfigurationElement> Section(ConfigurationElement e,string section)=>e.Children.FirstOrDefault(x=>x.Tag==section)?.Children??[];
 public void Initialize() {
  initializing=true; depth++;
  try {
   foreach(var n in Nodes) {
    if((n.Get("AccessMode")??"").Contains('R',StringComparison.OrdinalIgnoreCase)&&n.Element.Tag is not "Tab" and not "Category") {
     var read=Dispatch(new(ConfigurationOperationKind.ReadParameter,"ReadParameter",n,null,[],ConfigurationChangeKind.Initialization));
     if(read.Handled&&read.Value!=null)Change(n,"CurValue",read.Value,ConfigurationChangeKind.Initialization);
    }
    if(n.Element.Tag=="RadioSelecter"&&!(n.Get("AccessMode")??"").Contains('R',StringComparison.OrdinalIgnoreCase)) {
     var key="Param."+(n.Name??"")+(n.Get("ComTailName")??"");
     var readUser=Dispatch(new(ConfigurationOperationKind.ReadUserValue,"GetUserData",n,null,[key],ConfigurationChangeKind.Initialization));
     if(readUser.Handled)Change(n,"CurValue",readUser.Value??n.Get("DefaultValue")??n.Value,ConfigurationChangeKind.Initialization);
    }
    if(n.IsInteger32||n.Element.Tag is "Float" or "Boolean" or "String" or "OpenFileForCalibDialog") {
     var mode=Dispatch(new(ConfigurationOperationKind.ReadUserValue,"GetSubscriptionMode",n,null,["@"+(n.Name??"")+(n.Get("ComTailName")??"")],ConfigurationChangeKind.Initialization));
     if(mode.Handled&&bool.TryParse(mode.Value,out var subscribed))n.Assign("IsSub",subscribed.ToString());
    }
    committedValues[n]=n.Value;
   }
   foreach(var n in Nodes)foreach(var setter in Section(n.Element,"Initers"))Operate(n,setter,ConfigurationChangeKind.Initialization);
   foreach(var n in Nodes){Trigger(n,"CurValue",ConfigurationChangeKind.Initialization);Trigger(n,"CustomVisible",ConfigurationChangeKind.Initialization);}
  } finally {depth--;initializing=false;pendingRun=null;Changed?.Invoke();}
 }
 public void SetProgrammatic(string name,string property,string value)=>SetProgrammatic(Find(name)??throw new KeyNotFoundException(name),property,value);
 public void SetProgrammatic(ConfigurationNodeState node,string property,string value)=>Change(node,property,value,ConfigurationChangeKind.Programmatic);
 /// <summary>Explicit authoritative readback, corresponding to GetParamUnit. Unlike an arbitrary
 /// programmatic presentation change, this updates the rollback baseline after a successful read.</summary>
 public bool RefreshParameter(ConfigurationNodeState node) {
  var result=Dispatch(new(ConfigurationOperationKind.ReadParameter,"ReadParameter",node,null,[],ConfigurationChangeKind.Programmatic));
  if(!result.Handled||result.Error is not null||result.Value is null)return false;
  Change(node,"CurValue",result.Value,ConfigurationChangeKind.Programmatic);committedValues[node]=node.Value;return true;
 }
 public void CommitUser(string name,string value,bool forceWrite=false)=>CommitUser(Find(name)??throw new KeyNotFoundException(name),value,forceWrite);
 public void CommitUser(ConfigurationNodeState node,string value,bool forceWrite=false) {
  if(!node.Enabled){Diagnose("DisabledCommit","CommitUser",node,"Parameter is disabled.");return;}
  if(node.Element.Tag=="Expand"){Change(node,"CurValue",value,ConfigurationChangeKind.UserCommit);committedValues[node]=node.Value;return;}
  var declaredAccess=node.Get("AccessMode")??"";
  if(!declaredAccess.Contains('W',StringComparison.OrdinalIgnoreCase)&&!declaredAccess.Contains('O',StringComparison.OrdinalIgnoreCase)){Diagnose("ReadOnlyCommit","CommitUser",node,"Parameter is read-only.");return;}
  if(node.Element.Tag=="String"&&!node.Bool("IsSub",false)&&int.TryParse(node.Get("MaxLen"),out var maxBytes)&&maxBytes>0&&Encoding.UTF8.GetByteCount(value)>maxBytes){Diagnose("InvalidValue","String.CurValueA",node,"UTF-8 value exceeds MaxLen bytes.");return;}
  if(node.IsInteger32||node.Element.Tag=="Float") {
   if(!double.TryParse(value,NumberStyles.Float,CultureInfo.InvariantCulture,out var number)||!double.IsFinite(number)||node.IsInteger32&&!int.TryParse(value,NumberStyles.Integer,CultureInfo.InvariantCulture,out _)) {Diagnose("InvalidValue","CommitUser",node,"A finite numeric value is required.");return;}
   if(double.TryParse(node.Get("MinValue"),NumberStyles.Float,CultureInfo.InvariantCulture,out var min)&&number<min || double.TryParse(node.Get("MaxValue"),NumberStyles.Float,CultureInfo.InvariantCulture,out var max)&&number>max) {Diagnose("InvalidValue","CommitUser",node,"Value lies outside the declared range.");return;}
  }
  depth++;
  try {
   Change(node,"CurValue",value,ConfigurationChangeKind.UserCommit);
   if(!forceWrite&&committedValues.GetValueOrDefault(node)==node.Value)return;
   var access=node.Get("AccessMode")??"";
   bool writable=access.Contains('W',StringComparison.OrdinalIgnoreCase);
   var kind=writable?ConfigurationOperationKind.WriteParameter:ConfigurationOperationKind.PersistUserValue;
   var result=Dispatch(new(kind,kind.ToString(),node,null,kind==ConfigurationOperationKind.PersistUserValue?["Param."+(node.Name??"")+(node.Get("ComTailName")??"")]:[],ConfigurationChangeKind.UserCommit,node.Value));
   if(!result.Handled||result.Error!=null) {
    if(writable)Change(node,"CurValue",committedValues[node],ConfigurationChangeKind.Programmatic);
    return;
   }
   // A typed adapter may return an accepted value. Keep the model and its
   // committed baseline consistent with that explicit host result.
   if(result.Value is not null&&result.Value!=node.Value)Change(node,"CurValue",result.Value,ConfigurationChangeKind.Programmatic);
   committedValues[node]=node.Value;
   if(writable&&node.CanTriggerRun)pendingRun=node;
  } finally {Finish();}
 }
 public void Invoke(ConfigurationNodeState node) {
  if(!node.Enabled){Diagnose("DisabledCommand",node.Element.Tag,node,"Command is disabled.");return;}
  if(node.Element.Tag=="Button_ResetAlgoParams") {Dispatch(new(ConfigurationOperationKind.ResetParameters,"Button_ResetAlgoParams",node,null,[],ConfigurationChangeKind.Command));return;}
  if(node.Element.Tag=="SaveFileDialog"){Dispatch(new(ConfigurationOperationKind.SaveFile,"SaveFileDialog",node,null,[],ConfigurationChangeKind.Command));Changed?.Invoke();return;}
  var setter=node.Element.Children.FirstOrDefault(x=>x.Tag=="CommandSetter");
  if(setter==null){Diagnose("UnsupportedOperation",node.Element.Tag,node,"No supported command setter.");return;}
  Operate(node,setter,ConfigurationChangeKind.Command);Changed?.Invoke();
 }
 public bool ApplySubscription(ConfigurationNodeState node,ConfigurationSubscriptionBindingResult result,bool parameter=false) {
  if(result.Status!=ConfigurationSubscriptionStatus.Available){Diagnose("SubscriptionService",result.Status.ToString(),node,result.Diagnostic??"Subscription service is unavailable.");return false;}
  if(result.Binding!=null&&result.DisplayText==null){Diagnose("SubscriptionPresentation","ReadSubscription",node,"Bound reference is missing service-produced presentation text.");return false;}
  node.SubscriptionBinding=result.Binding;subscriptionWriteDepth++;
  try {
   if(parameter){node.Assign("Subscription",result.DisplayText??"");if(result.Binding!=null)Change(node,"IsSub","True",ConfigurationChangeKind.Programmatic);Changed?.Invoke();}
   else Change(node,"CurValue",result.DisplayText??"",ConfigurationChangeKind.Programmatic);
  }
  finally {subscriptionWriteDepth--;}
  return true;
 }
 public void PersistSubscriptionMode(ConfigurationNodeState node,bool enabled) {
  Dispatch(new(ConfigurationOperationKind.PersistUserValue,"SetSubscriptionMode",node,null,["@"+(node.Name??"")+(node.Get("ComTailName")??"")],ConfigurationChangeKind.UserCommit,enabled.ToString()));
  Change(node,"IsSub",enabled.ToString(),ConfigurationChangeKind.Programmatic);
 }
 public void ReportDiagnostic(ConfigurationDiagnostic diagnostic) {
  ArgumentNullException.ThrowIfNull(diagnostic);if(!diagnostics.Contains(diagnostic)){diagnostics.Add(diagnostic);Changed?.Invoke();}
 }
 public void ReportSubscriptionDiagnostic(ConfigurationNodeState node,string operation,string message)=>Diagnose("SubscriptionService",operation,node,message);
 public void SetSubscription(ConfigurationNodeState node,ConfigurationSubscriptionBinding? binding,string? presentation) {
  if(!node.SubscribeVisible){Diagnose("DisabledSubscription","SetSubscription",node,"Subscription is not available for this parameter.");return;}
  if(binding!=null&&presentation==null){Diagnose("SubscriptionPresentation","SetSubscription",node,"Typed binding requires service-produced presentation.");return;}
  var result=Dispatch(new(ConfigurationOperationKind.WriteParameterBinding,"SetModuleParamBindOperation",node,null,[],ConfigurationChangeKind.UserCommit){SubscriptionBinding=binding});
  if(result.Handled&&result.Error==null){node.SubscriptionBinding=binding;node.Assign("Subscription",presentation??"");if(binding!=null)node.Assign("IsSub","True");Changed?.Invoke();}
 }
 public void OpenChoices(ConfigurationNodeState node) {var setter=node.Element.Children.FirstOrDefault(x=>x.Tag=="DropOpenSetter");if(setter!=null)Operate(node,setter,ConfigurationChangeKind.Programmatic);}
 void Change(ConfigurationNodeState n,string property,string value,ConfigurationChangeKind kind) {
  property=ConfigurationNodeState.Normalize(property);value=n.NormalizeTypedProperty(property,value);
  if(property=="CurValue"&&n.IsInteger32&&int.TryParse(value,out var incoming)&&int.TryParse(n.Value,out var current)&&current!=incoming&&int.TryParse(n.Get("IncValue"),out var step)&&step==0) {Diagnose("InvalidValue","Integer.CurValueA",n,"Original integer quantization step is zero.");return;}
  value=Canonical(n,property,value);
  if(property=="CurValue"&&n.Element.Tag=="Expand"&&bool.TryParse(value,out var expanded))ExpandSubscriptionFlags(n,expanded,kind);
  if(!n.Assign(property,value))return;
  depth++;
  try {
   if(property=="CustomVisible" && !n.CustomVisible && n.IsDeleteSubscribe && !string.IsNullOrEmpty(n.Get("Subscription"))) {
    var cleared=Dispatch(new(ConfigurationOperationKind.ClearParameterBinding,"ClearParameterBinding",n,null,[],kind){SubscriptionBinding=n.SubscriptionBinding});
    if(cleared.Handled&&cleared.Error==null) {
     n.Assign("Subscription","");n.SubscriptionBinding=null;
     var read=Dispatch(new(ConfigurationOperationKind.ReadParameter,"ReadParameter",n,null,[],kind));
     if(read.Handled&&read.Error==null&&read.Value!=null){Change(n,"CurValue",read.Value,kind);committedValues[n]=n.Value;}
    }else Diagnose("SubscriptionClearFailed","ClearParameterBinding",n,"Binding identity was retained because the host did not confirm removal.");
   }
   Trigger(n,property,kind);
  } finally {Finish();}
 }
 void ExpandSubscriptionFlags(ConfigurationNodeState source,bool expanded,ConfigurationChangeKind kind) {
  var visited=new HashSet<ConfigurationNodeState>();var pending=new Stack<ConfigurationNodeState>();pending.Push(source);
  while(pending.TryPop(out var item)) {
   if(!visited.Add(item))continue;
   foreach(var setter in Section(item.Element,"Triggers").SelectMany(t=>Section(t,"Setters"))) {
    var target=Target(item,setter);if(target==null)continue;
    if(target.IsInteger32||target.Element.Tag is "Float" or "Boolean" or "String")Change(target,"IsDeleteSubscribe",expanded.ToString(),kind);
    else if(target.Element.Children.Any(x=>x.Tag is "Integers" or "Floats" or "SubChildren"))Diagnose("UnsupportedExpansion",target.Element.Tag,source,"ISubChild subscription children require explicit model support.");
    pending.Push(target);
   }
  }
 }
 void Finish(){depth--;if(depth!=0)return;var run=pendingRun;pendingRun=null;if(run!=null&&!initializing&&ExecutionEnabled&&!IsBusy)Dispatch(new(ConfigurationOperationKind.Run,"PreExecuteRun",run,null,[],ConfigurationChangeKind.UserCommit));Changed?.Invoke();}
 static string Canonical(ConfigurationNodeState n,string p,string v) {
  if(p is "CustomVisible" or "Enable" or "SubscribeVisible" || p=="CurValue"&&n.Element.Tag is "Boolean" or "Expand")return bool.TryParse(v,out var b)?b.ToString():v;
  if(p=="CurValue"&&n.IsInteger32&&int.TryParse(v,NumberStyles.Integer,CultureInfo.InvariantCulture,out var integer)) {
   if(int.TryParse(n.Value,out var old)&&old==integer)return n.Value;
   int min=int.TryParse(n.Get("MinValue"),out var lower)?lower:0,max=int.TryParse(n.Get("MaxValue"),out var upper)?upper:0,step=int.TryParse(n.Get("IncValue"),out var increment)?increment:1;
   if(integer>=min&&integer<=max&&step!=0)return unchecked(((integer-min)/step)*step+min).ToString(CultureInfo.InvariantCulture);
   return integer.ToString(CultureInfo.InvariantCulture);
  }
  if(p=="CurValue"&&n.Element.Tag=="Float"&&double.TryParse(v,NumberStyles.Float,CultureInfo.InvariantCulture,out var d)) {
   if(double.TryParse(n.Value,NumberStyles.Float,CultureInfo.InvariantCulture,out var old)&&old==d)return n.Value;
   int digits=int.TryParse(n.Get("DecimalDigits"),out var i)?i:2;return Math.Round(d,Math.Clamp(digits,0,15)).ToString(CultureInfo.InvariantCulture);
  }
  return v;
 }
 void Trigger(ConfigurationNodeState n,string property,ConfigurationChangeKind kind) {
  if(depth>128){Diagnose("TriggerRecursion","",n,"Trigger nesting exceeded 128; operation stopped.");return;}
  var triggers=Section(n.Element,"Triggers").Where(t=>t.Tag=="Trigger"?Field(t,"Property")==property:Section(t,"Conditions").Any(c=>Field(c,"Property")==property)).ToArray();
  foreach(var t in triggers)foreach(var s in Section(t,"Setters"))if(working.GetValueOrDefault(s)) {
   if(ruleRequests.TryGetValue(s,out var request))rules[request.OriginalName].Revert(this,request);
   else {var target=Target(n,s);if(target!=null)Change(target,"CustomVisible",originalVisibility.GetValueOrDefault(s).ToString(),kind);}working[s]=false;
  }
  foreach(var t in triggers)if(Matches(n,t))foreach(var s in Section(t,"Setters"))Operate(n,s,kind);
 }
 bool Matches(ConfigurationNodeState n,ConfigurationElement t) {
  if(t.Tag=="Trigger"){var expected=Field(t,"Value");return expected==null||Equivalent(n.Get(Field(t,"Property")??""),expected);}
  var checks=Section(t,"Conditions").Select(c=>{
   var prop=Field(c,"Property")??"";var dot=prop.IndexOf('.');var source=dot<0?n:Find(prop[..dot]);return Equivalent(source?.Get(dot<0?prop:prop[(dot+1)..]),Field(c,"Value")??"");}).ToArray();
  return string.Equals(Field(t,"LogicCondition"),"or",StringComparison.OrdinalIgnoreCase)?checks.Any(x=>x):checks.All(x=>x);
 }
 static bool Equivalent(string? actual,string expected) {
  if(actual==null)return false;
  return expected.Split('|').Any(x=>actual==x || bool.TryParse(actual,out var a)&&bool.TryParse(x,out var b)&&a==b || double.TryParse(actual,NumberStyles.Float,CultureInfo.InvariantCulture,out var d)&&double.TryParse(x,NumberStyles.Float,CultureInfo.InvariantCulture,out var e)&&d==e);
 }
 ConfigurationNodeState? Target(ConfigurationNodeState n,ConfigurationElement s) {
  var name=Field(s,"TargetName");if(string.IsNullOrEmpty(name))return null;
  var category=n.Parent;while(category!=null&&!category.IsCategory)category=category.Parent;
  return Nodes.FirstOrDefault(x=>x.Name==name&&x.Parent==category)??Find(name);
 }
 void Operate(ConfigurationNodeState source,ConfigurationElement setter,ConfigurationChangeKind kind) {
  var operation=Field(setter,"OperationName")??"";var target=Target(source,setter);var args=(Field(setter,"OperationParams")??"").Split(';',StringSplitOptions.None);
  switch(operation) {
   case "":break;
   case "VisibleOperation":
   case "HiddenOperation":
   case "CategoryHiddenOperation":
    if(operation=="CategoryHiddenOperation"&&target?.IsCategory!=true){Diagnose("MissingTarget",operation,source,"Category target not found.");break;}
    if(target==null){Diagnose("MissingTarget",operation,source,"Visible target not found.");break;}
    originalVisibility.TryAdd(setter,operation!="VisibleOperation");working[setter]=true;Change(target,"CustomVisible",(operation=="VisibleOperation").ToString(),kind);break;
   case "DoInitOperaion":
    if(target!=null)foreach(var s in Section(target.Element,"Initers"))Operate(target,s,kind);break;
   default:
    if(rules.TryGetValue(operation,out var rule)) {
     var request=new ConfigurationOperationRequest(ConfigurationOperationKind.Custom,operation,source,target,Array.AsReadOnly(args),kind,source.Value){TargetName=Field(setter,"TargetName")};
     ruleRequests[setter]=request;working[setter]=true;rule.Apply(this,request);break;
    }
    var opKind=Kind(operation,args);
    if(subscriptionWriteDepth>0&&opKind is ConfigurationOperationKind.WriteInputBinding or ConfigurationOperationKind.WriteParameterBinding)break;
    if(opKind==ConfigurationOperationKind.WriteInputBinding&&source.SubscriptionBinding==null&&!string.IsNullOrEmpty(source.Value)){Diagnose("MissingTypedReference",operation,source,"Display text cannot supply binding identity.");break;}
    var result=Dispatch(new(opKind,operation,source,target,Array.AsReadOnly(args),kind,source.Value){TargetName=Field(setter,"TargetName"),SubscriptionBinding=source.SubscriptionBinding});
    if(result.Handled&&result.Choices!=null)(target??source).Choices=Array.AsReadOnly(result.Choices.ToArray());
    if(result.Handled&&result.Value!=null)Change(target??source,"CurValue",result.Value,kind);
    break;
  }
  if(Field(setter,"PropertyName") is {} property&&target!=null)Change(target,property,Field(setter,"Value")??"",kind);
 }
 static ConfigurationOperationKind Kind(string name,string[] args)=>name switch {
  "GetSelectedDisplayFilterOperation" or "GetSelectedCombinationOperation" or "GetSelectedFilterOperation"=>ConfigurationOperationKind.ReadInputBinding,
  "SetResultIOToModuleOperation" or "SetCombinationSourceOperation" or "SetFilterSourceOperation"=>ConfigurationOperationKind.WriteInputBinding,
  "GetFrontAndSelfFilterTreeOperation" or "GetFrontParamItemsOperation" or "GetFrontCombinationTreeOperation" or "GetFrontFilterTreeOperation"=>ConfigurationOperationKind.EnumerateInputs,
  "GetModuleParamBindOperation"=>ConfigurationOperationKind.ReadParameterBinding,
  "SetModuleParamBindOperation"=>ConfigurationOperationKind.WriteParameterBinding,
  "ExcuteMethodOperation"=>args.FirstOrDefault() switch {
   "CloseParamsTabControl"=>ConfigurationOperationKind.Close,"SaveCalibPath"=>ConfigurationOperationKind.SaveCalibration,
   "Clear"=>ConfigurationOperationKind.Clear,"SelfExcuteModule"=>ConfigurationOperationKind.Run,
   "ContinuousSelfExcuteModule"=>ConfigurationOperationKind.RunContinuous,"StopSelfExcuteModule"=>ConfigurationOperationKind.StopContinuous,
   "ClearCalibPoint"=>ConfigurationOperationKind.ClearCalibrationPoints,_=>ConfigurationOperationKind.Unknown},
  _=>ConfigurationOperationKind.Unknown
 };
 ConfigurationOperationResult Dispatch(ConfigurationOperationRequest r) {
  if(r.Kind is ConfigurationOperationKind.Run or ConfigurationOperationKind.RunContinuous && IsBusy){Diagnose("ExecutionBusy",r.OriginalName,r.Source,"Execution request suppressed while busy.");return new(false);}

  if(r.Kind==ConfigurationOperationKind.Unknown){Diagnose("UnsupportedOperation",r.OriginalName,r.Source,"Unrecognized operation or method; no reflection performed.");return new(false);}
  ConfigurationOperationResult result;
  try {result=dispatcher?.Dispatch(r)??new(false);}
  catch(Exception error) {
   var message=error.GetType().Name+": "+error.Message;
   Diagnose("OperationError",r.OriginalName,r.Source,message);
   return new(false,Error:message);
  }
  if(!result.Handled)Diagnose("UnsupportedOperation",r.OriginalName,r.Source,"Typed operation has no registered implementation.");
  if(result.Error!=null)Diagnose("OperationError",r.OriginalName,r.Source,result.Error);
  return result;
 }
 void Diagnose(string code,string operation,ConfigurationNodeState node,string message) {
  var d=new ConfigurationDiagnostic(code,operation,node.Name,message);if(!diagnostics.Contains(d))diagnostics.Add(d);
 }
}
