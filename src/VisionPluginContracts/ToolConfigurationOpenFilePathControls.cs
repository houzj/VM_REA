using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brushes=System.Windows.Media.Brushes;
using Color=System.Windows.Media.Color;
using W=System.Windows.Controls;

namespace VisionPluginContracts;

/// <summary>Common source-compatible path leaf used by XML OpenFileDialog configuration nodes.</summary>
public static class ToolConfigurationOpenFilePathControls
{
 public static void Register(ToolConfigurationControlRegistry registry,Action<ToolConfigurationControlContext> chooseFile,Func<string,string>? localize=null)
 {
  ArgumentNullException.ThrowIfNull(registry);ArgumentNullException.ThrowIfNull(chooseFile);var text=localize??(s=>s);
  registry.Register("OpenFileDialog",c=>Create(c,chooseFile,text));
 }

 static FrameworkElement Create(ToolConfigurationControlContext c,Action<ToolConfigurationControlContext> choose,Func<string,string> text)
 {
  var row=new W.Grid{Height=34};row.ColumnDefinitions.Add(new(){Width=new GridLength(89)});row.ColumnDefinitions.Add(new());
  var label=new W.TextBlock{Text=text(c.Node.CaptionKey),FontSize=14,VerticalAlignment=VerticalAlignment.Center,Foreground=new SolidColorBrush(Color.FromRgb(98,103,108))};row.Children.Add(label);
  var editor=new W.Grid();editor.ColumnDefinitions.Add(new());editor.ColumnDefinitions.Add(new(){Width=GridLength.Auto});
  var input=new W.TextBox{FontSize=14,BorderThickness=new(0),Background=Brushes.Transparent,VerticalContentAlignment=VerticalAlignment.Center};
  input.LostFocus+=(_,_)=>{if(input.Text!=c.Node.Value)c.State.CommitUser(c.Node,input.Text,true);input.Text=c.Node.Value;};editor.Children.Add(input);
  var icon=new W.Image{Name="pathIcon",Source=Icon("normal"),Stretch=Stretch.None};var buttonBorder=new W.Border{Margin=new Thickness(0,-1,0,1),Child=icon};
  var button=new W.Button{Padding=new Thickness(0),BorderThickness=new Thickness(0),Background=Brushes.Transparent,Content=buttonBorder};button.Click+=(_,_)=>choose(c);button.MouseEnter+=(_,_)=>icon.Source=Icon("hover");button.MouseLeave+=(_,_)=>icon.Source=Icon("normal");button.PreviewMouseLeftButtonDown+=(_,_)=>icon.Source=Icon("click");button.PreviewMouseLeftButtonUp+=(_,_)=>icon.Source=Icon("hover");W.Grid.SetColumn(button,1);editor.Children.Add(button);
  var border=new W.Border{CornerRadius=new CornerRadius(4),BorderThickness=new Thickness(1),BorderBrush=new SolidColorBrush(Color.FromRgb(204,204,205)),Child=editor};W.Grid.SetColumn(border,1);row.Children.Add(border);
  c.OnRefresh(()=>{input.Text=c.Node.Value;input.ToolTip=c.Node.Value;label.Text=text(c.Node.CaptionKey);input.IsEnabled=c.Node.Enabled;button.IsEnabled=c.Node.Enabled;});return row;
 }
 static BitmapImage Icon(string state)=>new(new Uri($"pack://application:,,,/VisionPluginContracts;component/Resources/OpenFilePath_{state}.png"));
}
