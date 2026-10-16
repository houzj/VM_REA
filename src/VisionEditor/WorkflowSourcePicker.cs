using TreeView=System.Windows.Controls.TreeView;
using HorizontalAlignment=System.Windows.HorizontalAlignment;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisionFramework;
using Button=System.Windows.Controls.Button;
using CheckBox=System.Windows.Controls.CheckBox;
using Orientation=System.Windows.Controls.Orientation;
namespace VisionEditor;
public sealed record WorkflowSourceOption(string NodeId,string Port,DataKind Kind,string Label){public ValueCardinality Cardinality{get;init;}=ValueCardinality.Scalar;public bool IsInput{get;init;} public WorkflowValueReference? Reference{get;init;} public override string ToString()=>Label;}
public sealed class WorkflowSourcePicker:Window
{
 public WorkflowSourceOption? Selection=>Selections.FirstOrDefault();
 public IReadOnlyList<WorkflowSourceOption> Selections{get;private set;}=[];
 public bool IncludeModuleName{get;private set;}=true;
 public WorkflowSourcePicker(IEnumerable<WorkflowSourceOption> options,bool multiple=false)
 {
  System.Windows.Shell.WindowChrome.SetWindowChrome(this,new System.Windows.Shell.WindowChrome {GlassFrameThickness=new Thickness(0),CornerRadius=new CornerRadius(0),NonClientFrameEdges=System.Windows.Shell.NonClientFrameEdges.None,CaptionHeight=0,UseAeroCaptionButtons=false});
  Title=multiple?"批量选择":"订阅关系";Width=370;Height=420;WindowStyle=WindowStyle.None;WindowStartupLocation=WindowStartupLocation.CenterOwner;ShowInTaskbar=false;Background=new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(242,243,245));FontFamily=new System.Windows.Media.FontFamily("Microsoft YaHei");FontSize=14;
  var root=new DockPanel();var title=new DockPanel {Height=30,Background=new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(40,42,48))};DockPanel.SetDock(title,Dock.Top);var close=new Button {Content="×",Width=30};DockPanel.SetDock(close,Dock.Right);close.Click+=(_,_)=>Close();title.Children.Add(close);title.Children.Add(new TextBlock {Text=Title,Foreground=System.Windows.Media.Brushes.White,Margin=new Thickness(10,0,0,0),VerticalAlignment=VerticalAlignment.Center});title.MouseLeftButtonDown+=(_,e)=>{if(e.OriginalSource is TextBlock&&e.ChangedButton==MouseButton.Left)DragMove();};root.Children.Add(title);
  var bar=new DockPanel {Height=32,Background=new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(228,229,231))};DockPanel.SetDock(bar,Dock.Bottom);var include=new CheckBox {Content="包含模块名",IsChecked=true,Margin=new Thickness(18,0,0,0),VerticalAlignment=VerticalAlignment.Center,Visibility=multiple?Visibility.Visible:Visibility.Collapsed};var buttons=new StackPanel {Orientation=Orientation.Horizontal,HorizontalAlignment=HorizontalAlignment.Right};DockPanel.SetDock(buttons,Dock.Right);
  var cancel=new Button {Content="取消",Width=66,Height=24,Margin=new Thickness(0,0,15,0),IsCancel=true};var ok=new Button {Content="确定",Width=66,Height=24,Margin=new Thickness(0,0,15,0),IsDefault=true};buttons.Children.Add(cancel);buttons.Children.Add(ok);bar.Children.Add(buttons);bar.Children.Add(include);root.Children.Add(bar);
  var heading=new TextBlock {Text="选择列表",FontWeight=FontWeights.Bold,Height=52,Margin=new Thickness(18,0,0,0),VerticalAlignment=VerticalAlignment.Center};DockPanel.SetDock(heading,Dock.Top);root.Children.Add(heading);
  var tree=new TreeView {Margin=new Thickness(18,0,18,10),BorderBrush=System.Windows.Media.Brushes.LightGray,BorderThickness=new Thickness(1)};var checks=new List<(CheckBox Box,WorkflowSourceOption Option)>();
  foreach(var group in options.GroupBy(item=>item.NodeId))
  {
   string label=group.First().Label.Contains('.')?group.First().Label[..group.First().Label.LastIndexOf('.')]:group.First().Label;
   var groupBox=new CheckBox {Content=label,IsThreeState=true};var groupChecks=new List<CheckBox>();bool changing=false;
   var parent=new TreeViewItem {Header=multiple?(object)groupBox:label,IsExpanded=true};
   groupBox.Click+=(_,_)=>{changing=true;foreach(var child in groupChecks)child.IsChecked=groupBox.IsChecked==true;changing=false;groupBox.IsChecked=groupChecks.All(c=>c.IsChecked==true);};
   void UpdateGroup(){if(changing||!multiple)return;changing=true;int selected=groupChecks.Count(c=>c.IsChecked==true);groupBox.IsChecked=selected==0?false:selected==groupChecks.Count?true:null;changing=false;}
   foreach(var option in group){var box=new CheckBox {Content=option.Port.Length==0?option.Label:option.Label.Contains('.')?option.Label[(option.Label.LastIndexOf('.')+1)..]:option.Port,Margin=new Thickness(0,3,0,3)};if(!multiple)box.Checked+=(_,_)=>{foreach(var entry in checks)if(entry.Box!=box)entry.Box.IsChecked=false;};groupChecks.Add(box);box.Checked+=(_,_)=>UpdateGroup();box.Unchecked+=(_,_)=>UpdateGroup();checks.Add((box,option));parent.Items.Add(new TreeViewItem {Header=box});}tree.Items.Add(parent);
  }
  ok.Click+=(_,_)=>{var selected=checks.Where(item=>item.Box.IsChecked==true).Select(item=>item.Option).ToArray();if(selected.Length>0){Selections=selected;IncludeModuleName=include.IsChecked==true;DialogResult=true;}};
  root.Children.Add(tree);Content=new Border {Child=root,BorderThickness=new Thickness(1),BorderBrush=System.Windows.Media.Brushes.Black};
 }
}
