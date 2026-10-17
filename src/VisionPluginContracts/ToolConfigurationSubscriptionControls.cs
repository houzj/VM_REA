using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color=System.Windows.Media.Color;
using Grid=System.Windows.Controls.Grid;
using TextBox=System.Windows.Controls.TextBox;
using TreeView=System.Windows.Controls.TreeView;
using Image=System.Windows.Controls.Image;
using Brushes=System.Windows.Media.Brushes;
using HorizontalAlignment=System.Windows.HorizontalAlignment;

namespace VisionPluginContracts;

public static class ToolConfigurationSubscriptionControls {
 public static void Register(ToolConfigurationControlRegistry registry,IToolConfigurationSubscriptionService? service,Func<string,string>? localize=null,string language="Chinese") {
  var text=localize??(x=>x);
  registry.Register("ButtonSelecter",c=>{
   var selector=CreateInline(c,service,false,text);
   var row=new Grid{Height=32};row.ColumnDefinitions.Add(new(){Width=new GridLength(language=="English"?120:89)});row.ColumnDefinitions.Add(new(){Width=new GridLength(1,GridUnitType.Star)});
   var label=new TextBlock{FontSize=14,FontWeight=FontWeights.Normal,Foreground=Brushes.Gray,VerticalAlignment=VerticalAlignment.Center,TextTrimming=TextTrimming.CharacterEllipsis};
   row.Children.Add(label);Grid.SetColumn(selector,1);row.Children.Add(selector);c.OnRefresh(()=>{label.Text=text(c.Node.CaptionKey);label.ToolTip=text(c.Node.Get("Description")??c.Node.CaptionKey);});return row;
  });
 }
 /// <summary>Embeds the same source-confirmed selector composition inside subscribed numeric controls.</summary>
 public static ToolConfigurationSubscriptionSelector CreateInline(ToolConfigurationControlContext context,IToolConfigurationSubscriptionService? service,bool parameter=false,Func<string,string>? localize=null) {
  var query=ConfigurationSubscriptionValidation.FromElement(context.Element,parameter?ConfigurationSubscriptionDomain.Parameter:null);
  var selector=new ToolConfigurationSubscriptionSelector(context.State,context.Node,query,service,localize);
  context.OnRefresh(selector.Refresh);return selector;
 }
}

/// <summary>Independent DeleteTextBox/link popup/tree reconstruction. Leaf identities are typed objects, not displayed text.</summary>
public sealed class ToolConfigurationSubscriptionSelector:Grid {
 readonly ToolConfigurationState state;readonly ConfigurationNodeState node;readonly ConfigurationSubscriptionQuery? query;
 readonly IToolConfigurationSubscriptionService? service;readonly Func<string,string> localize;
 readonly TextBox value=new(){Height=28,VerticalContentAlignment=VerticalAlignment.Center,FontSize=14,FontWeight=FontWeights.Normal,BorderThickness=new Thickness(1),Foreground=Brushes.Gray};
 readonly ToggleButton toggle=new(){Width=32,Height=28,Background=Brushes.Transparent,BorderThickness=new Thickness(0)};
 readonly Popup popup=new(){Placement=PlacementMode.Bottom,HorizontalOffset=-150,StaysOpen=false,AllowsTransparency=true};
 readonly TextBlock message=new(){TextWrapping=TextWrapping.Wrap,Foreground=Brushes.DarkRed,MaxWidth=180};
 bool choosingIndex;
 public TreeView Tree {get;}=new(){Width=180,Height=180,FontWeight=FontWeights.Normal,BorderThickness=new Thickness(0),Background=Brushes.Transparent,FontSize=14};
 public ConfigurationSubscriptionBinding? SelectedBinding=>node.SubscriptionBinding;
 public string? Diagnostic {get;private set;}
 public ToolConfigurationSubscriptionSelector(ToolConfigurationState state,ConfigurationNodeState node,ConfigurationSubscriptionQuery? query,IToolConfigurationSubscriptionService? service,Func<string,string>? localize=null) {
  this.state=state;this.node=node;this.query=query;this.service=service;this.localize=localize??(x=>x);
  Height=32;ColumnDefinitions.Add(new(){Width=new GridLength(1,GridUnitType.Star)});ColumnDefinitions.Add(new(){Width=new GridLength(32)});
  Children.Add(value);SetColumn(toggle,1);Children.Add(toggle);
  var images=new[]{"normal","click","disable"}.ToDictionary(x=>x,x=>new BitmapImage(new Uri("pack://application:,,,/VisionPluginContracts;component/Resources/subscription_link_"+x+".png")));
  var icon=new Image{Source=images["normal"],Stretch=Stretch.None};toggle.Content=icon;
  toggle.IsEnabledChanged+=(_,_)=>icon.Source=images[toggle.IsEnabled?"normal":"disable"];
  toggle.MouseEnter+=(_,_)=>icon.Source=images["click"];toggle.MouseLeave+=(_,_)=>icon.Source=images[toggle.IsEnabled?"normal":"disable"];
  toggle.Checked+=(_,_)=>Open();toggle.Unchecked+=(_,_)=>popup.IsOpen=false;
  popup.PlacementTarget=toggle;popup.Closed+=(_,_)=>{toggle.IsChecked=false;choosingIndex=false;};
  var content=new StackPanel();content.Children.Add(Tree);content.Children.Add(message);
  popup.Child=new Border{Child=content,Background=new SolidColorBrush(Color.FromRgb(247,247,247)),BorderBrush=Brushes.Silver,BorderThickness=new Thickness(1),CornerRadius=new CornerRadius(4),Padding=new Thickness(0,6,0,6),Margin=new Thickness(0,4,0,0)};
  Tree.MouseDoubleClick+=(_,e)=>{if(Tree.SelectedItem is TreeViewItem item&&item.Tag is ConfigurationSubscriptionTreeNode entry&&entry.IsSelectable){Select(entry);e.Handled=true;}};
  Tree.KeyDown+=(_,e)=>{if(e.Key==Key.Enter&&Tree.SelectedItem is TreeViewItem item&&item.Tag is ConfigurationSubscriptionTreeNode entry&&entry.IsSelectable){Select(entry);e.Handled=true;}};
  value.SelectionChanged+=(_,_)=>{
   if(!value.IsKeyboardFocused||value.IsReadOnly||node.SubscriptionBinding==null||value.Text.Length==0)return;
   int open=value.Text.LastIndexOf('[',Math.Max(0,Math.Min(value.CaretIndex,value.Text.Length)-1));
   int close=value.Text.IndexOf(']',Math.Min(value.CaretIndex,value.Text.Length));
   if(open>=0&&close>=value.CaretIndex)OpenIndex();
  };
  value.KeyDown+=(_,e)=>{if(e.Key==Key.Enter){CommitConstant(value.Text);e.Handled=true;}else if(e.Key==Key.Escape)Refresh();};
  value.LostKeyboardFocus+=(_,_)=>{if(value.IsReadOnly)return;string expected=query?.Destination.Domain==ConfigurationSubscriptionDomain.Parameter?node.Get("Subscription")??"":node.Value;if(value.Text!=expected)CommitConstant(value.Text);};
  Unloaded+=(_,_)=>Close();Refresh();Read();
 }
 void Fail(string operation,string diagnostic){Diagnostic=diagnostic;message.Text=diagnostic;value.ToolTip=diagnostic;state.ReportSubscriptionDiagnostic(node,operation,diagnostic);}
 public void Refresh(){value.IsReadOnly=bool.TryParse(node.Get("ReadOnly"),out var readOnly)&&readOnly;value.Text=query?.Destination.Domain==ConfigurationSubscriptionDomain.Parameter?node.Get("Subscription")??"":node.Value;value.ToolTip=Diagnostic??value.Text;IsEnabled=node.Enabled;toggle.Visibility=query?.Destination.Domain==ConfigurationSubscriptionDomain.Parameter&&!node.SubscribeVisible?Visibility.Collapsed:Visibility.Visible;}
 public void Close(){popup.IsOpen=false;toggle.IsChecked=false;choosingIndex=false;}
 public void Read() {
  if(query==null){Fail("Read","Unsupported source-declared subscription query.");return;}
  if(service==null){Fail("Read","Subscription tree service is unavailable.");return;}
  try {var result=service.Read(query.Destination);if(state.ApplySubscription(node,result,query.Destination.Domain==ConfigurationSubscriptionDomain.Parameter))Refresh();else Fail("Read",result.Diagnostic??"Subscription read did not provide an authoritative value.");}
  catch(Exception e){Fail("Read",e.Message);}
 }
 public void Open()=>OpenTree(false);
 public void OpenIndex()=>OpenTree(true);
 void OpenTree(bool index){
  choosingIndex=index;
  Tree.Items.Clear();message.Text="";
  if(query==null||service==null){Fail("Enumerate","Subscription tree service is unavailable.");popup.IsOpen=true;return;}
  try {
   var result=service.Enumerate(index?query with {NativeTypes=Array.AsReadOnly(new[]{"int"}),IndexOnly=true}:query);if(result.Status!=ConfigurationSubscriptionStatus.Available){Fail("Enumerate",result.Diagnostic??result.Status.ToString());popup.IsOpen=true;return;}
   var error=ConfigurationSubscriptionValidation.CheckTree(result);if(error!=null){Fail("Enumerate",error);popup.IsOpen=true;return;}
   TreeViewItem Build(ConfigurationSubscriptionTreeNode n) {
    var item=new TreeViewItem{Header=localize(n.CaptionKey),Tag=n,IsExpanded=false,MinHeight=22,Focusable=true};
    foreach(var child in n.Children)item.Items.Add(Build(child));return item;
   }
   foreach(var entry in result.Roots)Tree.Items.Add(Build(entry));Diagnostic=null;
   Tree.ToolTip=result.UnsupportedCapabilities?.Count>0?string.Join("; ",result.UnsupportedCapabilities):null;
   popup.IsOpen=true;
  }catch(Exception e){Fail("Enumerate",e.Message);popup.IsOpen=true;}
 }
 public bool Select(ConfigurationSubscriptionTreeNode entry) {
  if(!entry.IsSelectable){Fail("Bind","A grouping node cannot be bound.");return false;}
  if(entry.IsUnbind&&query?.IncludeEmpty!=true){Fail("Bind","Unbind is excluded by query.");return false;}
  if(choosingIndex) {
   if(entry.IsUnbind)return SetIndex(ConfigurationSubscriptionIndex.All);
   if(entry.Binding?.Reference is ConfigurationConstantReference constant&&int.TryParse(constant.Literal,out var number))return SetIndex(ConfigurationSubscriptionIndex.At(number));
   if(entry.Binding!=null) {
    try{return SetIndex(ConfigurationSubscriptionIndex.From(entry.Binding.Reference));}
    catch(ArgumentException e){Fail("Index",e.Message);return false;}
   }
   return false;
  }
  return Bind(entry.Binding);
 }
 public bool Bind(ConfigurationSubscriptionBinding? binding) {
  if(query==null||service==null){Fail("Bind","Subscription binding service is unavailable.");return false;}
  if(binding!=null&&ConfigurationSubscriptionValidation.Check(query,binding) is {} error){Fail("Bind",error);return false;}
  try {
   var result=service.Bind(query.Destination,binding);
   if(!state.ApplySubscription(node,result,query.Destination.Domain==ConfigurationSubscriptionDomain.Parameter)){Fail("Bind",result.Diagnostic??"Subscription binding failed.");Refresh();return false;}
   Diagnostic=null;message.Text="";Refresh();popup.IsOpen=false;return true;
  }catch(Exception e){Fail("Bind",e.Message);Refresh();return false;}
 }
 public bool SetMode(bool enabled) {
  if(query?.Destination.Domain!=ConfigurationSubscriptionDomain.Parameter){Fail("Mode","Only parameter subscriptions have an edit/subscribe mode.");return false;}
  if(service==null){Fail("Mode","Subscription binding service is unavailable.");return false;}
  if(!enabled&&!Bind(null))return false;
  state.PersistSubscriptionMode(node,enabled);
  if(enabled)Read();Refresh();return true;
 }
 public bool SetIndex(ConfigurationSubscriptionIndex index) {
  if(node.SubscriptionBinding==null){Fail("Index","No typed source reference is bound.");return false;}
  return Bind(node.SubscriptionBinding with {Index=index});
 }
 public bool CommitConstant(string literal) {
  if(query==null){Fail("Constant","No subscription query.");return false;}
  if(string.IsNullOrEmpty(literal))return Bind(null);
  // Editing an index reuses the authoritative existing reference; the caption never resolves a new identity.
  string presented=query.Destination.Domain==ConfigurationSubscriptionDomain.Parameter?node.Get("Subscription")??"":node.Value;
  int oldOpen=presented.LastIndexOf('['),newOpen=literal.LastIndexOf('[');
  if(node.SubscriptionBinding!=null&&newOpen>=0&&literal.EndsWith(']')) {
   string expectedPrefix=oldOpen<0?presented:presented[..oldOpen];
   if(literal[..newOpen]!=expectedPrefix){Fail("Index","A displayed source caption cannot replace reference identity.");Refresh();return false;}
   string index=literal[(newOpen+1)..^1];
   if(index.Length==0)return SetIndex(ConfigurationSubscriptionIndex.All);
   if(int.TryParse(index,out var position)&&position>=0)return SetIndex(ConfigurationSubscriptionIndex.At(position));
   Fail("Index","Select an integer index reference from the tree.");Refresh();return false;
  }
  // Text input is accepted only as a typed literal. It is never parsed as a module/global/index identity.
  foreach(var type in query.NativeTypes) {
   var binding=new ConfigurationSubscriptionBinding(new ConfigurationConstantReference(type,literal),ConfigurationSubscriptionIndex.All);
   if(ConfigurationSubscriptionValidation.Check(query,binding)==null)return Bind(binding);
  }
  Fail("Constant","Input is not a valid typed constant; select reference identities from the tree.");Refresh();return false;
 }
}
