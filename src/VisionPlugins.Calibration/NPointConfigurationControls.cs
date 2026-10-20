using Color=System.Windows.Media.Color;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionPluginContracts;
using W=System.Windows.Controls;
namespace VisionPlugins.Calibration;

/// <summary>Factories selected by the shipped N-point template keys, independent of top-level host.</summary>
public static class NPointConfigurationControls {
 internal static W.ControlTemplate IconTemplate(string kind){
  var image=new FrameworkElementFactory(typeof(W.Image),"icon");image.SetValue(W.Image.SourceProperty,new BitmapImage(new Uri("pack://application:,,,/VisionPlugins.Calibration;component/Resources/NPoint_"+kind+"_normal.png")));image.SetValue(W.Image.StretchProperty,Stretch.None);
  var border=new FrameworkElementFactory(typeof(W.Border));border.SetValue(W.Border.CornerRadiusProperty,new CornerRadius(2));border.SetValue(W.Border.BorderThicknessProperty,new Thickness(kind=="edit"?1:0));border.SetValue(W.Border.BorderBrushProperty,new SolidColorBrush(Color.FromRgb(204,204,205)));border.SetValue(W.Border.MarginProperty,new Thickness(2,2,0,1));border.AppendChild(image);
  var template=new W.ControlTemplate(typeof(W.Button)){VisualTree=border};var hover=new Trigger{Property=UIElement.IsMouseOverProperty,Value=true};hover.Setters.Add(new Setter(W.Image.SourceProperty,new BitmapImage(new Uri("pack://application:,,,/VisionPlugins.Calibration;component/Resources/NPoint_"+kind+"_click.png")),"icon"));template.Triggers.Add(hover);if(kind=="path"){hover.Setters.Clear();hover.Setters.Add(new Setter(W.Image.SourceProperty,new BitmapImage(new Uri("pack://application:,,,/VisionPlugins.Calibration;component/Resources/NPoint_path_hover.png")),"icon"));var pressed=new Trigger{Property=System.Windows.Controls.Primitives.ButtonBase.IsPressedProperty,Value=true};pressed.Setters.Add(new Setter(W.Image.SourceProperty,new BitmapImage(new Uri("pack://application:,,,/VisionPlugins.Calibration;component/Resources/NPoint_path_click.png")),"icon"));template.Triggers.Add(pressed);}return template;
 }

 public static void Register(ToolConfigurationControlRegistry registry,Action<ToolConfigurationControlContext> openPointEditor,Action<ToolConfigurationControlContext> chooseCalibrationPath,Func<string,string>? localize=null,IToolConfigurationSubscriptionService? subscriptions=null){
  var text=localize??(s=>s);var standard=new ToolConfigurationControlRegistry();ToolConfigurationStandardControls.Register(standard,text,subscriptions:subscriptions);
  registry.Register("EditButton_Calib",c=>{standard.TryCreate("Integer",c,out var integer);var row=new W.Grid{Height=32};row.ColumnDefinitions.Add(new());row.ColumnDefinitions.Add(new(){Width=new GridLength(36)});if(integer!=null)row.Children.Add(integer);var edit=new W.Button{Width=32,Height=32,Margin=new(4,0,0,0),ToolTip="编辑标定点",Name="NPointEditPoints"};edit.Template=IconTemplate("edit");edit.Click+=(_,_)=>openPointEditor(c);W.Grid.SetColumn(edit,1);row.Children.Add(edit);c.OnRefresh(()=>edit.IsEnabled=c.Node.Enabled);return row;});
  RegisterCalibrationPath(registry,chooseCalibrationPath,text,subscriptions);
 }
 public static void RegisterCalibrationPath(ToolConfigurationControlRegistry registry,Action<ToolConfigurationControlContext> chooseCalibrationPath,Func<string,string>? localize=null,IToolConfigurationSubscriptionService? subscriptions=null){
  ToolConfigurationCalibrationPathControls.Register(registry,chooseCalibrationPath,localize,subscriptions,"NPointChooseCalibrationPath");
 }
}
