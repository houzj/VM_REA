using Brush=System.Windows.Media.Brush;
using Brushes=System.Windows.Media.Brushes;
using Color=System.Windows.Media.Color;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
namespace VisionPlugins.ImageSource;
public partial class WpfNumericControl:System.Windows.Controls.UserControl {
 public static readonly DependencyProperty TextProperty=DependencyProperty.Register(nameof(Text),typeof(string),typeof(WpfNumericControl),new FrameworkPropertyMetadata("0",FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,Changed));
 public string Text{get=>(string)GetValue(TextProperty);set=>SetValue(TextProperty,value);}
 public event Action<string>? ValueChanged;
 public int Increment{get;set;}=1;
 public int Maximum{get;set;}=int.MaxValue;
 public GridLength SymbolWidth=>new(0);public double SymbolDir=>1;public bool IsReadOnly=>false;
 public Brush TBackground=>Brushes.White;public Brush TBorderBrush=>new SolidColorBrush(Color.FromRgb(202,199,197));
 public string TBtnBgNormal=>"pack://application:,,,/VisionPlugins.ImageSource;component/Resources/combo_ic_top_normal.png";
 public string TBtnBgHover=>"pack://application:,,,/VisionPlugins.ImageSource;component/Resources/combo_ic_top_click.png";
 public WpfNumericControl(){InitializeComponent();}
 static void Changed(DependencyObject o,DependencyPropertyChangedEventArgs e){var control=(WpfNumericControl)o;if(int.TryParse((string)e.NewValue,out int n)&&n>=0&&n<=control.Maximum)control.ValueChanged?.Invoke((string)e.NewValue);else control.SetCurrentValue(TextProperty,(string)e.OldValue);}
 void Step(int delta){ValueText.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();if(int.TryParse(Text,out int n)){long next=Math.Clamp((long)n+(long)delta*Increment,0,Maximum);SetCurrentValue(TextProperty,next.ToString());}}
 void UpButton_Click(object sender,RoutedEventArgs e)=>Step(1);void DownButton_Click(object sender,RoutedEventArgs e)=>Step(-1);
 void ValueText_MouseWheel(object sender,MouseWheelEventArgs e){if(IsKeyboardFocusWithin){Step(e.Delta>0?1:-1);e.Handled=true;}}
}
