using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
namespace VisionUi;
// NumericControl + SliderArrangeFloat: 32px, 20px repeat arrows, text-click popup and focus-only wheel.
public sealed class RoiNumericField:Grid {
 public TextBox Input{get;}=new();readonly Slider slider;readonly Popup popup;readonly double minimum,maximum;readonly bool integer;bool updating;
 public event EventHandler? ValueCommitted;
 public bool IsSliderOpen{get=>popup.IsOpen;set=>popup.IsOpen=value;}
 public RoiNumericField(double value,double minimum=-30000,double maximum=30000,bool integer=false){this.minimum=minimum;this.maximum=maximum;this.integer=integer;Height=32;var frame=new Border{BorderThickness=new(1),BorderBrush=new SolidColorBrush(Color.FromRgb(180,181,184)),CornerRadius=new(2),Background=Brushes.White,Margin=new(0,2,0,2)};var grid=new Grid();grid.ColumnDefinitions.Add(new());grid.ColumnDefinitions.Add(new(){Width=new(20)});Input.Background=Brushes.Transparent;Input.Foreground=new SolidColorBrush(Color.FromRgb(78,80,84));Input.BorderThickness=new(0);Input.Margin=new(7,0,2,0);Input.VerticalContentAlignment=VerticalAlignment.Center;InputMethod.SetIsInputMethodEnabled(Input,false);grid.Children.Add(Input);var arrows=new Grid();arrows.RowDefinitions.Add(new());arrows.RowDefinitions.Add(new());SetColumn(arrows,1);for(int i=0;i<2;i++){int direction=i==0?1:-1;var button=new RepeatButton{Content=i==0?"▲":"▼",FontSize=7,Focusable=false,BorderThickness=new(0),Background=Brushes.Transparent,Foreground=Input.Foreground,Tag=i==0?"RoiNumericUp":"RoiNumericDown"};SetRow(button,i);button.Click+=(_,_)=>Step(direction);arrows.Children.Add(button);}grid.Children.Add(arrows);frame.Child=grid;Children.Add(frame);
 slider=new(){Minimum=minimum,Maximum=maximum,Value=Math.Clamp(value,minimum,maximum),Width=210,Margin=new(10),TickFrequency=1,IsSnapToTickEnabled=integer};popup=new(){Child=new Border{Background=Brushes.White,BorderBrush=Brushes.Gray,BorderThickness=new(1),Child=slider},Placement=PlacementMode.Bottom,StaysOpen=false,PlacementTarget=this,AllowsTransparency=true};Input.Text=value.ToString("G",CultureInfo.InvariantCulture);Input.PreviewMouseLeftButtonDown+=(_,_)=>popup.IsOpen=true;Input.PreviewMouseWheel+=(_,e)=>{if(Input.IsKeyboardFocused){Step(e.Delta>0?1:-1);e.Handled=true;}};Input.LostKeyboardFocus+=(_,_)=>{SyncSlider();ValueCommitted?.Invoke(this,EventArgs.Empty);};Input.PreviewKeyDown+=(_,e)=>{if(e.Key==Key.Enter){SyncSlider();ValueCommitted?.Invoke(this,EventArgs.Empty);popup.IsOpen=false;e.Handled=true;}else if(e.Key==Key.Escape)popup.IsOpen=false;};slider.ValueChanged+=(_,_)=>{if(!updating){Input.Text=slider.Value.ToString("G",CultureInfo.InvariantCulture);ValueCommitted?.Invoke(this,EventArgs.Empty);}};Unloaded+=(_,_)=>popup.IsOpen=false;
 }
 public void Step(int direction){if(!double.TryParse(Input.Text,NumberStyles.Float,CultureInfo.InvariantCulture,out var value)||!double.IsFinite(value))return;double next=Math.Clamp(value+direction,minimum,maximum);if(integer)next=Math.Round(next);Input.Text=next.ToString("G",CultureInfo.InvariantCulture);SyncSlider();ValueCommitted?.Invoke(this,EventArgs.Empty);}
 void SyncSlider(){if(double.TryParse(Input.Text,NumberStyles.Float,CultureInfo.InvariantCulture,out var n)&&double.IsFinite(n)){updating=true;slider.Value=Math.Clamp(n,minimum,maximum);updating=false;}}
}
