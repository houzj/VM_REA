using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TextBox=System.Windows.Controls.TextBox;
using TreeView=System.Windows.Controls.TreeView;
using Image=System.Windows.Controls.Image;
using Brushes=System.Windows.Media.Brushes;
namespace VisionEditor;

/// <summary>
/// Source-guided subscription side of Apps.XmlUI ButtonSelecter_sub.
/// Index editing is deliberately absent until the caller supplies native-equivalent index metadata.
/// </summary>
public sealed class NativeCompoundSourceSelector : Grid
{
 readonly Func<IEnumerable<WorkflowSourceOption>> options;
 readonly Action<WorkflowSourceOption> selected;
 public TextBox RelationText {get;}
 public ToggleButton LinkButton {get;}
 public Popup SubscriptionPopup {get;}
 public TreeView SubscriptionTree {get;}
 public NativeCompoundSourceSelector(string label,Func<IEnumerable<WorkflowSourceOption>> options,Action<WorkflowSourceOption> selected)
 {
  this.options=options;this.selected=selected;Height=32;
  ColumnDefinitions.Add(new ColumnDefinition());ColumnDefinitions.Add(new ColumnDefinition{Width=new GridLength(32)});
  RelationText=new TextBox{Text=label,IsReadOnly=true,BorderThickness=new Thickness(0),Background=Brushes.Transparent,FontSize=14,VerticalContentAlignment=VerticalAlignment.Center,ToolTip=label};
  Children.Add(RelationText);
  LinkButton=new ToggleButton{Width=32,Height=32,ToolTip="订阅关系",Background=Brushes.Transparent,BorderThickness=new Thickness(0)};
  var image=new FrameworkElementFactory(typeof(Image));image.Name="Icon";image.SetValue(Image.WidthProperty,32d);image.SetValue(Image.HeightProperty,32d);image.SetValue(Image.SourceProperty,Asset("normal"));
  var template=new ControlTemplate(typeof(ToggleButton)){VisualTree=image};
  foreach(var state in new[]{(UIElement.IsMouseOverProperty,"hover"),(ToggleButton.IsCheckedProperty,"click")}){var trigger=new Trigger{Property=state.Item1,Value=true};trigger.Setters.Add(new Setter(Image.SourceProperty,Asset(state.Item2),"Icon"));template.Triggers.Add(trigger);}LinkButton.Template=template;SetColumn(LinkButton,1);Children.Add(LinkButton);
  SubscriptionTree=new TreeView{Width=180,Height=180,BorderThickness=new Thickness(0),Background=Brushes.Transparent,FontSize=14};
  SubscriptionPopup=new Popup{PlacementTarget=LinkButton,Placement=PlacementMode.Bottom,HorizontalOffset=-150,VerticalOffset=4,AllowsTransparency=true,StaysOpen=false,Child=new Border{BorderThickness=new Thickness(1),BorderBrush=Brushes.Gray,CornerRadius=new CornerRadius(4),Background=Brushes.White,Padding=new Thickness(0,6,0,6),Child=SubscriptionTree}};
  Children.Add(SubscriptionPopup);
  LinkButton.Checked+=(_,_)=>{RefreshOptions();SubscriptionPopup.IsOpen=true;};
  LinkButton.Unchecked+=(_,_)=>SubscriptionPopup.IsOpen=false;
  SubscriptionPopup.Closed+=(_,_)=>LinkButton.IsChecked=false;
  Unloaded+=(_,_)=>SubscriptionPopup.IsOpen=false;
  PreviewKeyDown+=(_,e)=>{if(e.Key==Key.Escape&&SubscriptionPopup.IsOpen){SubscriptionPopup.IsOpen=false;e.Handled=true;}};
 }
 static BitmapImage Asset(string state)=>new(new Uri("/VisionUi;component/SourceAssets/combo_ic_关联_"+state+".png",UriKind.Relative));
 void RefreshOptions()
 {
  SubscriptionTree.Items.Clear();
  foreach(var group in options().GroupBy(o=>o.NodeId)){
   var first=group.First();
   // A null-source action is a leaf, not an empty expandable module heading.
   if(group.Key.Length==0){foreach(var item in group)SubscriptionTree.Items.Add(Leaf(item,item.Label));continue;}
   var suffix="."+first.Port;var title=first.Port.Length>0&&first.Label.EndsWith(suffix,StringComparison.Ordinal)?first.Label[..^suffix.Length]:first.NodeId;
   var root=new TreeViewItem{Header=title,IsExpanded=true};SubscriptionTree.Items.Add(root);
   var branches=new Dictionary<string,TreeViewItem>(StringComparer.Ordinal);
   foreach(var option in group){
    var segments=option.Port.Split('.',StringSplitOptions.RemoveEmptyEntries);ItemsControl parent=root;string path="";
    for(int i=0;i<segments.Length-1;i++){path+=segments[i]+".";if(!branches.TryGetValue(path,out var branch)){branch=new TreeViewItem{Header=segments[i],IsExpanded=true};branches.Add(path,branch);parent.Items.Add(branch);}parent=branch;}
    parent.Items.Add(Leaf(option,segments.Length>0?segments[^1]:option.Label));
   }
  }
 }
 TreeViewItem Leaf(WorkflowSourceOption option,string title)
 {
  var leaf=new TreeViewItem{Header=title,Tag=option,ToolTip=option.Label};
  leaf.MouseDoubleClick+=(_,e)=>{if(leaf.Items.Count!=0||e.ChangedButton!=MouseButton.Left)return;e.Handled=true;SubscriptionPopup.IsOpen=false;RelationText.Text=option.Label;RelationText.ToolTip=option.Label;selected(option);};
  return leaf;
 }
}
