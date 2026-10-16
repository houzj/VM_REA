using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VisionFramework;
using Brush=System.Windows.Media.Brush;
using Brushes=System.Windows.Media.Brushes;
using Color=System.Windows.Media.Color;
using Image=System.Windows.Controls.Image;
using Button=System.Windows.Controls.Button;
using HorizontalAlignment=System.Windows.HorizontalAlignment;
namespace VisionEditor;
public partial class MainWindow
{
 // Evidence: VMControls.WPF/base/diagramitemtemplate.xaml, GROUP (110 x 40).
 Brush GroupBackground(NodeDocument node,bool selected)=>new ImageBrush(Asset(!node.Enabled?"group_bg_forbidden.png":selected?"group_bg_focus.png":"group_bg.png")){Stretch=Stretch.None};
 Canvas CreateGroupCard(NodeDocument node,bool selected,Canvas card)
 {
  var body=new Border{Width=110,Height=40,Background=GroupBackground(node,selected),BorderThickness=new Thickness(0)};
  var grid=new Grid();grid.ColumnDefinitions.Add(new ColumnDefinition{Width=GridLength.Auto});grid.ColumnDefinitions.Add(new ColumnDefinition());
  var normal=Asset("ic_group_configuration_normal.png");
  var icon=new Image{Source=normal,Stretch=Stretch.None};
  var button=new Button{Content=icon,Margin=new Thickness(7,0,0,0),Padding=new Thickness(0),Background=Brushes.Transparent,BorderThickness=new Thickness(0),ToolTip="Group 参数设置",IsEnabled=node.Enabled&&!isLocked&&activeRun is null};
  var template=new ControlTemplate(typeof(Button));var presenter=new FrameworkElementFactory(typeof(ContentPresenter));template.VisualTree=presenter;button.Template=template;
  button.MouseEnter+=(_,_)=>icon.Source=Asset("ic_group_configuration_click.png");button.MouseLeave+=(_,_)=>icon.Source=normal;
  button.Click+=(_,e)=>{e.Handled=true;OpenNode(node.Id);};
  grid.Children.Add(button);
  var text=new TextBlock{Text=node.Name,FontSize=12,FontWeight=FontWeights.Bold,Foreground=new SolidColorBrush(Color.FromRgb(55,58,60)),VerticalAlignment=VerticalAlignment.Center,HorizontalAlignment=HorizontalAlignment.Center,TextTrimming=TextTrimming.CharacterEllipsis};Grid.SetColumn(text,1);grid.Children.Add(text);
  body.Child=grid;Canvas.SetLeft(body,10);Canvas.SetTop(body,10);card.Children.Add(body);return card;
 }
}
