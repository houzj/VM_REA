using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Button=System.Windows.Controls.Button;
using Image=System.Windows.Controls.Image;
using Orientation=System.Windows.Controls.Orientation;
using HorizontalAlignment=System.Windows.HorizontalAlignment;
using Brushes=System.Windows.Media.Brushes;
using Color=System.Windows.Media.Color;
using ColorConverter=System.Windows.Media.ColorConverter;

namespace VisionPluginContracts;

/// <summary>Ordinary compact configuration host. Source chain:
/// ParamsTabControlBase -> BaseWindow/ParamsConfigControlBase -> ParamsTabUserControl.
/// With-render and embedded hosts consume the same session/view separately.</summary>
public sealed class ToolConfigurationModelWindow : Window {
 readonly ToolConfigurationSession session;
 public ToolConfigurationView ConfigurationView {get;}
 public ToolConfigurationModelWindow(ToolEditorContext context,ToolConfigurationSession session){
  this.session=session;
  Resources.MergedDictionaries.Add(new ResourceDictionary{Source=new Uri("/VisionPluginContracts;component/Resources/ConfigurationWindow.xaml",UriKind.Relative)});
  ConfigurationView=new(session.State,session.Controls,session.Language,session.Localize);
  var size=ConfigurationView.DesiredConfigurationSize;Width=size.Width;Height=size.Height;
  Title=(context.DisplayIndex is int index?index+" ":"")+context.Node.Name;
  WindowStyle=WindowStyle.None;ResizeMode=ResizeMode.CanMinimize;ShowInTaskbar=false;
  WindowStartupLocation=WindowStartupLocation.CenterOwner;
  Background=Brush("#F2F3F5");Foreground=Brush("#4E5054");FontFamily=new("Microsoft YaHei");FontSize=12;
  var root=new Grid();root.RowDefinitions.Add(new(){Height=GridLength.Auto});root.RowDefinitions.Add(new());
  var header=new Grid{Background=Brush("#262626"),Height=32};
  header.ColumnDefinitions.Add(new());header.ColumnDefinitions.Add(new(){Width=GridLength.Auto});
  var caption=new StackPanel{Orientation=Orientation.Horizontal,VerticalAlignment=VerticalAlignment.Center};
  caption.Children.Add(new TextBlock{Text=Title,Foreground=Brushes.White,FontSize=13,FontWeight=FontWeights.Bold,Margin=new Thickness(10,0,0,0),VerticalAlignment=VerticalAlignment.Center});
  var help=new Button{Width=30,Height=26,Margin=new Thickness(8,0,8,0),ToolTip=session.Localize("Lang_Help"),Content=Asset("btn_help.png",14),Background=Brushes.Transparent,BorderThickness=new Thickness(0)};
  help.Style=(Style)FindResource("ConfigurationHelpButton");help.Click+=(_,_)=>session.OpenHelp?.Invoke();caption.Children.Add(help);header.Children.Add(caption);
  var close=new Button{Width=32,Height=32,Margin=new Thickness(2,0,8,0),Background=Brushes.Transparent,BorderThickness=new Thickness(0),Content=Asset("windowclose.png",32),HorizontalAlignment=HorizontalAlignment.Right};
  close.Style=(Style)FindResource("ConfigurationCloseButton");close.Click+=(_,_)=>Close();Grid.SetColumn(close,1);header.Children.Add(close);
  header.MouseLeftButtonDown+=(_,e)=>{if(e.ChangedButton==System.Windows.Input.MouseButton.Left)DragMove();};
  root.Children.Add(header);Grid.SetRow(ConfigurationView,1);root.Children.Add(ConfigurationView);
  var frame=new Border{BorderBrush=Brush("#323232"),BorderThickness=new Thickness(1),CornerRadius=new CornerRadius(1,1,2,2),Child=root};
  var wrapper=new Grid();wrapper.Children.Add(frame);wrapper.Children.Add(new Border{Background=Brush("#FD4F41"),Height=1,VerticalAlignment=VerticalAlignment.Top,IsHitTestVisible=false});Content=wrapper;
  session.CloseRequested+=Close;Closed+=(_,_)=>{session.CloseRequested-=Close;ConfigurationView.Dispose();session.Dispose();};
 }
 static SolidColorBrush Brush(string value)=>new((Color)ColorConverter.ConvertFromString(value));
 static Image Asset(string name,double size)=>new(){Width=size,Height=size,Stretch=Stretch.Uniform,Source=new BitmapImage(new Uri("pack://application:,,,/VisionPluginContracts;component/Resources/"+name))};
}
