using System.Windows;
using System.Windows.Media;
namespace VisionEditor;
// Independent equivalent of PerSuperImageButton's four image states; never crop to 16x16.
public sealed class WorkflowImageButton : System.Windows.Controls.Button
{
 public static readonly DependencyProperty NormalSourceProperty=DependencyProperty.Register(nameof(NormalSource),typeof(ImageSource),typeof(WorkflowImageButton));
 public static readonly DependencyProperty HoverSourceProperty=DependencyProperty.Register(nameof(HoverSource),typeof(ImageSource),typeof(WorkflowImageButton));
 public static readonly DependencyProperty PressedSourceProperty=DependencyProperty.Register(nameof(PressedSource),typeof(ImageSource),typeof(WorkflowImageButton));
 public static readonly DependencyProperty DisabledSourceProperty=DependencyProperty.Register(nameof(DisabledSource),typeof(ImageSource),typeof(WorkflowImageButton));
 public ImageSource NormalSource {get=>(ImageSource)GetValue(NormalSourceProperty);set=>SetValue(NormalSourceProperty,value);}
 public ImageSource HoverSource {get=>(ImageSource)GetValue(HoverSourceProperty);set=>SetValue(HoverSourceProperty,value);}
 public ImageSource PressedSource {get=>(ImageSource)GetValue(PressedSourceProperty);set=>SetValue(PressedSourceProperty,value);}
 public ImageSource DisabledSource {get=>(ImageSource)GetValue(DisabledSourceProperty);set=>SetValue(DisabledSourceProperty,value);}
}
public sealed class WorkflowExpandIconConverter:System.Windows.Data.IValueConverter
{
 readonly Dictionary<bool,ImageSource> images=new();
 public object Convert(object value,Type targetType,object parameter,System.Globalization.CultureInfo culture)
 {
  bool expanded=value is true;if(images.TryGetValue(expanded,out var cached))return cached;
  var image=new System.Windows.Media.Imaging.BitmapImage();image.BeginInit();image.CacheOption=System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;image.UriSource=new Uri(System.IO.Path.Combine(AppContext.BaseDirectory,"ui-assets","wpf",expanded?"btn_高级参数_downarrow.png":"btn_高级参数_uparrow.png"));image.EndInit();image.Freeze();images[expanded]=image;return image;
 }
 public object ConvertBack(object value,Type targetType,object parameter,System.Globalization.CultureInfo culture)=>throw new NotSupportedException();
}
