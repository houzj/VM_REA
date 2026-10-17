using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using TextBox=System.Windows.Controls.TextBox;
using TreeView=System.Windows.Controls.TreeView;
using Brushes=System.Windows.Media.Brushes;

namespace VisionEditor;

// Source-guided subset of ButtonSelecter_sub's bracket-index editor. Group keeps its
// typed operand contract; the native module tree and detached subscribe window are
// separate work and are not emulated here.
public sealed class NativeGroupIndexEditor : Grid
{
 readonly Func<IEnumerable<WorkflowSourceOption>> options;
 readonly Action<WorkflowSourceOption> select;
 public TextBox Text { get; }
 public Popup IndexPopup { get; }
 public TreeView IndexTree { get; }

 public NativeGroupIndexEditor(string display, Func<IEnumerable<WorkflowSourceOption>> options,
  Action<WorkflowSourceOption> select)
 {
  this.options=options;this.select=select;
  Text=new TextBox{Text=display,VerticalContentAlignment=VerticalAlignment.Center};
  Children.Add(Text);
  IndexTree=new TreeView{Width=180,Height=180,FontSize=14,BorderThickness=new Thickness(0),Background=Brushes.Transparent};
  IndexPopup=new Popup{PlacementTarget=Text,Placement=PlacementMode.Bottom,
   StaysOpen=false,AllowsTransparency=true,
   Child=new Border{Width=180,Height=180,BorderBrush=Brushes.Gray,BorderThickness=new Thickness(1),
    Background=Brushes.White,Child=IndexTree}};
  Children.Add(IndexPopup);
  Text.SelectionChanged+=(_,_)=>UpdatePopup();
  Text.GotKeyboardFocus+=(_,_)=>UpdatePopup();
  Text.PreviewKeyDown+=(_,e)=>{if(e.Key==Key.Escape&&IndexPopup.IsOpen){IndexPopup.IsOpen=false;e.Handled=true;}};
  Unloaded+=(_,_)=>IndexPopup.IsOpen=false;
 }

 public static bool IsInsideIndex(string value,int caret)
 {
  if(caret<0||caret>value.Length||value.Length<2)return false;
  var open=caret==0?-1:value.LastIndexOf('[',caret-1);
  if(open<0)return false;
  var previousClose=value.LastIndexOf(']',caret-1);
  if(previousClose>open)return false;
  var close=value.IndexOf(']',caret);
  return close>=0&&value.IndexOf('[',open+1,close-open-1)<0;
 }

 void UpdatePopup()
 {
  if(!Text.IsKeyboardFocusWithin||!IsInsideIndex(Text.Text,Text.CaretIndex))
  {IndexPopup.IsOpen=false;return;}
  IndexTree.Items.Clear();
  foreach(var option in options())
  {
   var item=new TreeViewItem{Header=option.Label,Tag=option};
   item.MouseDoubleClick+=(_,e)=>{if(e.ChangedButton!=MouseButton.Left)return;
    e.Handled=true;IndexPopup.IsOpen=false;select(option);};
   IndexTree.Items.Add(item);
  }
  IndexPopup.IsOpen=true;
 }
}
