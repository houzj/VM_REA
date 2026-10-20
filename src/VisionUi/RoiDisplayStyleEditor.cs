using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using VisionFramework;
namespace VisionUi;

// Source: ImageViewConfig[_subContent], ParamUserControlDataTemplate.xaml:2503-2556.
public sealed class RoiDisplayStyleEditor : DockPanel {
 RoiDisplayStyle current;
 readonly Grid fields=new();readonly List<FrameworkElement> widthControls=[];
 Action<string> refreshOk=null!,refreshNg=null!;
 public RoiDisplayStyle Value=>current;
 public event Action<RoiDisplayStyle>? Changed;
 public Popup Popup{get;}
 public ToggleButton Eye{get;}
 public ToggleButton Settings{get;}
 public RoiNumericField OpacityField{get;}
 public RoiNumericField WidthField{get;}
 public RoiDisplayStyleEditor(string label,RoiDisplayStyle value,bool strokeThicknessConfigEnable=true){
  value.Validate();Resources.MergedDictionaries.Add(SourceControlResources.Current);current=value;Height=32;Tag="DisplayStyle:"+label;
  Children.Add(new TextBlock{Text=label,Width=89,FontSize=14,Foreground=Brushes.Gray,VerticalAlignment=VerticalAlignment.Center});
  var buttons=new UniformGrid{Width=100,Columns=2,HorizontalAlignment=HorizontalAlignment.Left};
  Eye=new(){Style=(Style)FindResource("eyeToggleButton"),Width=32,Height=32,IsChecked=value.Visible,Tag="DisplayEye:"+label};
  Settings=new(){Style=(Style)FindResource("NativeDisplayPopupButton"),Width=32,Height=32,Tag="DisplayPopup:"+label};
  buttons.Children.Add(Eye);buttons.Children.Add(Settings);Children.Add(buttons);
  fields.ColumnDefinitions.Add(new(){Width=new(102)});fields.ColumnDefinitions.Add(new());for(int i=0;i<4;i++)fields.RowDefinitions.Add(new(){Height=new(32)});
  void Add(int row,string name,FrameworkElement field){var text=new TextBlock{Text=name,Margin=new(10,0,0,0),VerticalAlignment=VerticalAlignment.Center,Foreground=Brushes.Gray,FontSize=14};Grid.SetRow(text,row);fields.Children.Add(text);Grid.SetColumn(field,1);Grid.SetRow(field,row);fields.Children.Add(field);if(row==3){widthControls.Add(text);widthControls.Add(field);}}
  Add(0,"OK颜色",ColorField(value.OkColor,c=>Set(current with{OkColor=c}),out refreshOk));
  Add(1,"NG颜色",ColorField(value.NgColor,c=>Set(current with{NgColor=c}),out refreshNg));
  OpacityField=new(value.Opacity,0,1,increment:.01);WidthField=new(value.StrokeThickness,1,10);
  OpacityField.Input.Tag="DisplayOpacity:"+label;WidthField.Input.Tag="DisplayWidth:"+label;
  Add(2,"透明度",OpacityField);Add(3,"粗细",WidthField);
  OpacityField.ValueCommitted+=(_,_)=>{if(double.TryParse(OpacityField.Input.Text,NumberStyles.Float,CultureInfo.InvariantCulture,out var n)&&double.IsFinite(n))Set(current with{Opacity=Math.Clamp(n,0,1)});RefreshValue(current);};
  WidthField.ValueCommitted+=(_,_)=>{if(double.TryParse(WidthField.Input.Text,NumberStyles.Float,CultureInfo.InvariantCulture,out var n)&&double.IsFinite(n))Set(current with{StrokeThickness=Math.Clamp(n,1,10)});RefreshValue(current);};
  var gradient=new LinearGradientBrush();gradient.GradientStops.Add(new(Color.FromRgb(252,252,252),0));gradient.GradientStops.Add(new(Color.FromRgb(252,252,252),.15));gradient.GradientStops.Add(new(Color.FromRgb(247,247,247),1));
  Popup=new(){PlacementTarget=Settings,Placement=PlacementMode.Bottom,HorizontalOffset=-90,StaysOpen=false,AllowsTransparency=true,Child=new Border{Width=260,Margin=new(0,4,0,0),Padding=new(0,6,6,6),CornerRadius=new(4),BorderThickness=new(1),BorderBrush=new SolidColorBrush(Color.FromRgb(204,204,205)),Background=gradient,Child=fields}};
  SetStrokeThicknessConfig(strokeThicknessConfigEnable);
  Settings.Checked+=(_,_)=>Popup.IsOpen=true;Settings.Unchecked+=(_,_)=>Popup.IsOpen=false;
  Popup.Closed+=(_,_)=>{Settings.IsChecked=false;foreach(var toggle in fields.Children.OfType<ToggleButton>())toggle.IsChecked=false;OpacityField.IsSliderOpen=WidthField.IsSliderOpen=false;};
  Eye.Click+=(_,_)=>Set(current with{Visible=Eye.IsChecked==true});Unloaded+=(_,_)=>Popup.IsOpen=false;
 }
 public void SetStrokeThicknessConfig(bool enabled){fields.RowDefinitions[3].Height=enabled?new GridLength(32):new GridLength(0);foreach(var control in widthControls)control.Visibility=enabled?Visibility.Visible:Visibility.Collapsed;((Border)Popup.Child).Height=enabled?150:120;}
 // Authoritative host readback: never raises Changed and does not rebuild an open popup.
 public void RefreshValue(RoiDisplayStyle value){value.Validate();current=value;Eye.IsChecked=value.Visible;OpacityField.Input.Text=value.Opacity.ToString("G",CultureInfo.InvariantCulture);WidthField.Input.Text=value.StrokeThickness.ToString("G",CultureInfo.InvariantCulture);refreshOk(value.OkColor);refreshNg(value.NgColor);}
 void Set(RoiDisplayStyle value){value.Validate();current=value;Changed?.Invoke(value);RefreshValue(current);}
 static FrameworkElement ColorField(string initial,Action<string> change,out Action<string> refresh){
  string value=initial;var button=new ToggleButton{Height=32,Tag="DisplayColor",HorizontalContentAlignment=HorizontalAlignment.Stretch};
  var swatch=new Border{Width=24,MinHeight=10,Margin=new(4)};var text=new TextBlock{FontSize=14,Margin=new(10,0,0,0),VerticalAlignment=VerticalAlignment.Center,IsHitTestVisible=false};
  void Paint(string colorText){value=colorText;var color=(Color)ColorConverter.ConvertFromString(value);swatch.Background=new SolidColorBrush(color);text.Text=$"{color.R,3},{color.G,3},{color.B,3}";}
  refresh=Paint;Paint(initial);var content=new StackPanel{Orientation=Orientation.Horizontal};content.Children.Add(swatch);content.Children.Add(text);button.Content=content;
  var border=new FrameworkElementFactory(typeof(Border),"bd");border.SetValue(Border.BackgroundProperty,Brushes.Transparent);border.SetValue(Border.BorderBrushProperty,new SolidColorBrush(Color.FromRgb(204,204,205)));border.SetValue(Border.BorderThicknessProperty,new Thickness(1));border.SetValue(FrameworkElement.MarginProperty,new Thickness(0,2,0,2));border.AppendChild(new FrameworkElementFactory(typeof(ContentPresenter)));
  var template=new ControlTemplate(typeof(ToggleButton)){VisualTree=border};var hover=new Trigger{Property=IsMouseOverProperty,Value=true};hover.Setters.Add(new Setter(Border.BorderBrushProperty,new DynamicResourceExtension("Col_Theme"),"bd"));template.Triggers.Add(hover);button.Template=template;
  var popup=new Popup{PlacementTarget=button,Placement=PlacementMode.Bottom,StaysOpen=false,AllowsTransparency=true};
  void Commit(string color){change(color);Paint(value);}
  button.Checked+=(_,_)=>{var palette=new SourceColorPalette();var advanced=new SourceAdvancedColorPicker{SelectedCurrentColor=new SolidColorBrush((Color)ColorConverter.ConvertFromString(value))};var tabs=new TabControl();tabs.Resources.MergedDictionaries.Add(new ResourceDictionary{Source=new Uri("/VisionUi;component/SourceColorPickerStyles.xaml",UriKind.Relative)});tabs.Items.Add(new TabItem{Header="默认",Content=palette});tabs.Items.Add(new TabItem{Header="更多",Content=advanced});palette.Selected+=Commit;advanced.ColorChanged+=c=>Commit(c.ToString());popup.Child=new Border{Background=Brushes.White,Child=tabs};popup.IsOpen=true;};
  button.Unchecked+=(_,_)=>popup.IsOpen=false;popup.Closed+=(_,_)=>button.IsChecked=false;button.Unloaded+=(_,_)=>popup.IsOpen=false;return button;
 }
}
