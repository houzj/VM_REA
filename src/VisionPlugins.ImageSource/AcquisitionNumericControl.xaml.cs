using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
namespace VisionPlugins.ImageSource;
public partial class AcquisitionNumericControl:System.Windows.Controls.UserControl {
 public static readonly DependencyProperty TextProperty=DependencyProperty.Register(nameof(Text),typeof(string),typeof(AcquisitionNumericControl),new FrameworkPropertyMetadata("0",FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,Changed));
 public string Text{get=>(string)GetValue(TextProperty);set=>SetValue(TextProperty,value);}
 public double Minimum{get;set;} public double Maximum{get;set;}=double.MaxValue;public bool Integer{get;set;}public event Action<double>? ValueChanged;
 public GridLength SymbolWidth=>new(0);public double SymbolDir=>1;public bool IsReadOnly=>false;
 public System.Windows.Media.Brush TBackground=>System.Windows.Media.Brushes.White;public System.Windows.Media.Brush TBorderBrush=>new SolidColorBrush(System.Windows.Media.Color.FromRgb(202,199,197));
 public string TBtnBgNormal=>"pack://application:,,,/VisionPlugins.ImageSource;component/Resources/combo_ic_top_normal.png";public string TBtnBgHover=>"pack://application:,,,/VisionPlugins.ImageSource;component/Resources/combo_ic_top_click.png";
 public AcquisitionNumericControl(){InitializeComponent();}
 static void Changed(DependencyObject o,DependencyPropertyChangedEventArgs e){var c=(AcquisitionNumericControl)o;if(double.TryParse((string)e.NewValue,NumberStyles.Float,CultureInfo.InvariantCulture,out double n)&&double.IsFinite(n)&&n>=c.Minimum&&n<=c.Maximum&&(!c.Integer||n==Math.Truncate(n)))c.ValueChanged?.Invoke(n);else c.SetCurrentValue(TextProperty,(string)e.OldValue);}
 void Step(int direction){ValueText.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();if(double.TryParse(Text,NumberStyles.Float,CultureInfo.InvariantCulture,out double value))SetCurrentValue(TextProperty,Math.Clamp(value+direction*(Integer?1:.1),Minimum,Maximum).ToString(Integer?"0":"F2",CultureInfo.InvariantCulture));}
 void UpButton_Click(object sender,RoutedEventArgs e)=>Step(1);void DownButton_Click(object sender,RoutedEventArgs e)=>Step(-1);
 void ValueText_MouseWheel(object sender,MouseWheelEventArgs e){if(IsKeyboardFocusWithin){Step(e.Delta>0?1:-1);e.Handled=true;}}
}
