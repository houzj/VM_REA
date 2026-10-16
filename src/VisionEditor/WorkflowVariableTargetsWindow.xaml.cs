using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisionFramework;
using Button=System.Windows.Controls.Button;
using Orientation=System.Windows.Controls.Orientation;
namespace VisionEditor;

public partial class WorkflowVariableTargetsWindow:Window
{
 readonly WorkflowSourceOption[] options;
 readonly List<WorkflowSourceOption> selected;
 public IReadOnlyList<WorkflowSourceOption> Selections=>selected.ToArray();
 public WorkflowVariableTargetsWindow(IEnumerable<WorkflowSourceOption> options,IEnumerable<WorkflowSourceOption> selected){this.options=options.ToArray();this.selected=selected.ToList();InitializeComponent();RefreshTree();RefreshSelected();}
 void RefreshTree()
 {
  if(TargetTree is null)return;TargetTree.Items.Clear();DataKind? filter=Filter.SelectedIndex switch{1=>DataKind.Integer,2=>DataKind.Number,3=>DataKind.Text,_=>null};
  foreach(var group in options.Where(o=>filter is null||o.Kind==filter).GroupBy(o=>o.NodeId))
  {
   var first=group.First();var heading=new TreeViewItem {Header=first.Label.Contains('.')?first.Label[..first.Label.LastIndexOf('.')]:first.NodeId,IsExpanded=true};
   foreach(var o in group)heading.Items.Add(new TreeViewItem {Header=o.Label[(o.Label.LastIndexOf('.')+1)..],Tag=o,ToolTip=o.Label});TargetTree.Items.Add(heading);
  }
 }
 void AddTarget(WorkflowSourceOption option){if(!selected.Any(o=>Same(o,option))){selected.Add(option);RefreshSelected();}}
 static bool Same(WorkflowSourceOption a,WorkflowSourceOption b)=>a.NodeId==b.NodeId&&a.Port==b.Port&&a.IsInput==b.IsInput;
 void RefreshSelected()
 {
  SelectedTargets.Children.Clear();foreach(var option in selected){var row=new DockPanel {Height=32};var remove=new Button {Content="×",Width=28,Height=24,Margin=new Thickness(4,0,8,0),Tag=option};DockPanel.SetDock(remove,Dock.Right);remove.Click+=(_,_)=>{selected.Remove(option);RefreshSelected();};row.Children.Add(remove);row.Children.Add(new TextBlock {Text=option.Label,ToolTip=option.Label,Margin=new Thickness(10,0,0,0),VerticalAlignment=VerticalAlignment.Center,MaxWidth=250,TextTrimming=TextTrimming.CharacterEllipsis,Foreground=System.Windows.Media.Brushes.DimGray});SelectedTargets.Children.Add(row);}
 }
 void ChooseTarget(object sender,MouseButtonEventArgs e){if(TargetTree.SelectedItem is TreeViewItem {Tag:WorkflowSourceOption option}){AddTarget(option);e.Handled=true;}}
 void FilterChanged(object sender,SelectionChangedEventArgs e){if(IsInitialized)RefreshTree();}
 void Confirm(object sender,RoutedEventArgs e)=>DialogResult=true;
 void Cancel(object sender,RoutedEventArgs e)=>Close();
 void DragTitle(object sender,MouseButtonEventArgs e){if(e.ChangedButton==MouseButton.Left&&e.OriginalSource is not Button)DragMove();}
}
