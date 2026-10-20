using Color=System.Windows.Media.Color;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using W=System.Windows.Controls;
namespace VisionPluginContracts;
public static class ToolConfigurationCalibrationPathControls {
 public static void Register(ToolConfigurationControlRegistry registry,Action<ToolConfigurationControlContext> chooseCalibrationPath,Func<string,string>? localize=null,IToolConfigurationSubscriptionService? subscriptions=null,string chooseButtonName="ChooseCalibrationPath") {
  registry.Register("OpenFileForCalibDialog",c=>PathLeaf(c,chooseCalibrationPath,localize??(s=>s),subscriptions,chooseButtonName));
 }
 /// <summary>Source-control variant for OpenFileForCalibDialog implementations whose invalid edits remain in CurValueA while parameter submission fails.</summary>
 public static void RegisterPreservingRejectedText(ToolConfigurationControlRegistry registry,Func<ToolConfigurationControlContext,string?> selectCalibrationPath,Action<ToolConfigurationControlContext,string>? accepted=null,Func<string,string>? localize=null,IToolConfigurationSubscriptionService? subscriptions=null,string chooseButtonName="ChooseCalibrationPath",Action<ToolConfigurationControlContext,string>? selectionCommitted=null) {
  registry.Register("OpenFileForCalibDialog",c=>PathLeaf(c,_=>{},localize??(s=>s),subscriptions,chooseButtonName,selectCalibrationPath,accepted,true,selectionCommitted));
 }
 internal static W.ControlTemplate IconTemplate(string kind){
  var image=new FrameworkElementFactory(typeof(W.Image),"icon");image.SetValue(W.Image.SourceProperty,new BitmapImage(new Uri("pack://application:,,,/VisionPluginContracts;component/Resources/CalibrationPath_"+kind+"_normal.png")));image.SetValue(W.Image.StretchProperty,Stretch.None);
  var border=new FrameworkElementFactory(typeof(W.Border));border.SetValue(W.Border.CornerRadiusProperty,new CornerRadius(2));border.SetValue(W.Border.BorderThicknessProperty,new Thickness(kind=="edit"?1:0));border.SetValue(W.Border.BorderBrushProperty,new SolidColorBrush(Color.FromRgb(204,204,205)));border.SetValue(W.Border.MarginProperty,new Thickness(2,2,0,1));border.AppendChild(image);
  var template=new W.ControlTemplate(typeof(W.Button)){VisualTree=border};var hover=new Trigger{Property=UIElement.IsMouseOverProperty,Value=true};hover.Setters.Add(new Setter(W.Image.SourceProperty,new BitmapImage(new Uri("pack://application:,,,/VisionPluginContracts;component/Resources/CalibrationPath_"+kind+"_click.png")),"icon"));template.Triggers.Add(hover);if(kind=="path"){hover.Setters.Clear();hover.Setters.Add(new Setter(W.Image.SourceProperty,new BitmapImage(new Uri("pack://application:,,,/VisionPluginContracts;component/Resources/CalibrationPath_path_hover.png")),"icon"));var pressed=new Trigger{Property=System.Windows.Controls.Primitives.ButtonBase.IsPressedProperty,Value=true};pressed.Setters.Add(new Setter(W.Image.SourceProperty,new BitmapImage(new Uri("pack://application:,,,/VisionPluginContracts;component/Resources/CalibrationPath_path_click.png")),"icon"));template.Triggers.Add(pressed);}return template;
 }

 static FrameworkElement PathLeaf(ToolConfigurationControlContext c,Action<ToolConfigurationControlContext> chooseCalibrationPath,Func<string,string> text,IToolConfigurationSubscriptionService? subscriptions,string chooseButtonName,Func<ToolConfigurationControlContext,string?>? selectCalibrationPath=null,Action<ToolConfigurationControlContext,string>? accepted=null,bool preserveRejectedText=false,Action<ToolConfigurationControlContext,string>? selectionCommitted=null){
  var row=new W.Grid{Height=34};row.Resources.MergedDictionaries.Add(new ResourceDictionary{Source=new Uri("/VisionPluginContracts;component/Resources/ConfigurationControls.xaml",UriKind.Relative)});row.ColumnDefinitions.Add(new(){Width=new GridLength(89)});row.ColumnDefinitions.Add(new());
  var label=new W.TextBlock{Text=text(c.Node.CaptionKey),FontSize=14,VerticalAlignment=VerticalAlignment.Center,Foreground=new SolidColorBrush(Color.FromRgb(98,103,108))};row.Children.Add(label);
  var editor=new W.Grid();editor.ColumnDefinitions.Add(new());editor.ColumnDefinitions.Add(new(){Width=GridLength.Auto});W.Grid.SetColumn(editor,1);row.Children.Add(editor);
  var file=new W.Grid();file.ColumnDefinitions.Add(new());file.ColumnDefinitions.Add(new(){Width=GridLength.Auto});
  var input=new W.TextBox{Name=preserveRejectedText&&c.Node.Name=="FilePath"?"RotateCalibrationPathInput":"",FontSize=14,BorderThickness=new(0),Background=System.Windows.Media.Brushes.Transparent,VerticalContentAlignment=VerticalAlignment.Center};
  string? rejectedText=null;
  void CommitPath(string candidate){if(preserveRejectedText)input.Text=candidate;c.State.CommitUser(c.Node,candidate,true);bool rejected=c.Node.Value!=candidate||c.State.Diagnostics.Any(d=>d.Code=="OperationError"&&d.Node==c.Node.Name&&d.Operation=="WriteParameter");if(preserveRejectedText&&rejected){rejectedText=candidate;input.Text=candidate;}else{rejectedText=null;input.Text=c.Node.Value;if(!rejected)accepted?.Invoke(c,candidate);}}
  input.LostFocus+=(_,_)=>{if(input.Text==c.Node.Value)rejectedText=null;else if(input.Text!=rejectedText)CommitPath(input.Text);if(rejectedText==null)input.Text=c.Node.Value;};file.Children.Add(input);
  var choose=new W.Button{Name=chooseButtonName,Width=28,Margin=new(0,-1,0,1),Template=IconTemplate("path")};choose.Click+=(_,_)=>{if(selectCalibrationPath is null)chooseCalibrationPath(c);else if(selectCalibrationPath(c) is{} candidate){CommitPath(candidate);selectionCommitted?.Invoke(c,candidate);}};W.Grid.SetColumn(choose,1);file.Children.Add(choose);
  var border=new W.Border{CornerRadius=new CornerRadius(4),BorderThickness=new(1),BorderBrush=new SolidColorBrush(Color.FromRgb(204,204,205)),Child=file};editor.Children.Add(border);
  var mode=new System.Windows.Controls.Primitives.ToggleButton{Style=(Style)row.FindResource("ConfigurationEditLinkButton")};W.Grid.SetColumn(mode,1);editor.Children.Add(mode);
  var selector=subscriptions==null?null:ToolConfigurationSubscriptionControls.CreateInline(c,subscriptions,true,text);if(selector!=null)editor.Children.Add(selector);
  var unsupported=new W.TextBlock{Text="未支持：订阅服务未提供",Foreground=System.Windows.Media.Brushes.DarkRed,VerticalAlignment=VerticalAlignment.Center,Visibility=Visibility.Collapsed};editor.Children.Add(unsupported);
  mode.Click+=(_,_)=>{if(selector!=null)selector.SetMode(mode.IsChecked==true);mode.IsChecked=bool.TryParse(c.Node.Get("IsSub"),out var sub)&&sub;};
  c.OnRefresh(()=>{input.Text=rejectedText??c.Node.Value;input.ToolTip=input.Text;bool subscribed=bool.TryParse(c.Node.Get("IsSub"),out var sub)&&sub;border.Visibility=subscribed?Visibility.Collapsed:Visibility.Visible;unsupported.Visibility=subscribed&&selector==null?Visibility.Visible:Visibility.Collapsed;if(selector!=null)selector.Visibility=subscribed?Visibility.Visible:Visibility.Collapsed;mode.IsChecked=subscribed;mode.Visibility=c.Node.SubscribeVisible?Visibility.Visible:Visibility.Collapsed;mode.IsEnabled=c.Node.Enabled&&subscriptions!=null;editor.IsEnabled=c.Node.Enabled;});return row;
 }
}
