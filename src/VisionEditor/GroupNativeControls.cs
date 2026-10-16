using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Button=System.Windows.Controls.Button;
using CheckBox=System.Windows.Controls.CheckBox;
using Image=System.Windows.Controls.Image;
using ButtonBase=System.Windows.Controls.Primitives.ButtonBase;
namespace VisionEditor;
public partial class SubprogramCallWindow {
 static readonly Dictionary<string,BitmapImage> groupAssets=new();
 static BitmapImage GroupAsset(string name){if(groupAssets.TryGetValue(name,out var found))return found;var asset=new BitmapImage();asset.BeginInit();asset.UriSource=new Uri(System.IO.Path.Combine(AppContext.BaseDirectory,"ui-assets",name));asset.CacheOption=BitmapCacheOption.OnLoad;asset.EndInit();asset.Freeze();groupAssets[name]=asset;return asset;}
 // Apps.XmlUI BooleanControl/Boolean_sub: exact on/off resource pixels, no text behind switch.
 static void NativeGroupSwitch(CheckBox control){var image=new FrameworkElementFactory(typeof(Image));image.Name="State";image.SetValue(Image.SourceProperty,GroupAsset("group_swith_off.png"));image.SetValue(Image.StretchProperty,Stretch.None);var template=new ControlTemplate(typeof(CheckBox)){VisualTree=image};var enabled=new Trigger{Property=ToggleButton.IsCheckedProperty,Value=true};enabled.Setters.Add(new Setter(Image.SourceProperty,GroupAsset("group_swith_on.png"),"State"));template.Triggers.Add(enabled);control.Template=template;control.Width=48;control.Height=32;control.Padding=new Thickness(0);control.Margin=new Thickness(0);control.HorizontalAlignment=System.Windows.HorizontalAlignment.Left;}
 internal static void NativeGroupIcon(Button control,string stem){var image=new FrameworkElementFactory(typeof(Image));image.Name="Icon";image.SetValue(Image.SourceProperty,GroupAsset("group_"+stem+"_normal.png"));image.SetValue(Image.StretchProperty,Stretch.None);var template=new ControlTemplate(typeof(Button)){VisualTree=image};foreach(var state in new[]{UIElement.IsMouseOverProperty,ButtonBase.IsPressedProperty}){var trigger=new Trigger{Property=state,Value=true};trigger.Setters.Add(new Setter(Image.SourceProperty,GroupAsset("group_"+stem+(state==UIElement.IsMouseOverProperty&&System.IO.File.Exists(System.IO.Path.Combine(AppContext.BaseDirectory,"ui-assets","group_"+stem+"_hover.png"))?"_hover.png":"_click.png")),"Icon"));template.Triggers.Add(trigger);}if(System.IO.File.Exists(System.IO.Path.Combine(AppContext.BaseDirectory,"ui-assets","group_"+stem+"_disable.png"))){var disabled=new Trigger{Property=UIElement.IsEnabledProperty,Value=false};disabled.Setters.Add(new Setter(Image.SourceProperty,GroupAsset("group_"+stem+"_disable.png"),"Icon"));template.Triggers.Add(disabled);}control.Template=template;control.Padding=new Thickness(0);control.Height=32;control.Margin=new Thickness(0);}
 static Grid NativeOutputRow(){var row=new Grid{Margin=new Thickness(0,0,5,6),Height=32};foreach(var width in new[]{new GridLength(70),new GridLength(68),new GridLength(1,GridUnitType.Star),new GridLength(53),new GridLength(42)})row.ColumnDefinitions.Add(new(){Width=width});return row;}
}
