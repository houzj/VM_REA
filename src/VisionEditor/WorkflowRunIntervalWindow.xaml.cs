using System.Windows;
using System.Windows.Input;
namespace VisionEditor;
public partial class WorkflowRunIntervalWindow:Window
{
 public int IntervalMs{get;private set;}
 public WorkflowRunIntervalWindow(int intervalMs){InitializeComponent();IntervalMs=intervalMs;Interval.Increment=10;Interval.Maximum=100000;Interval.Text=intervalMs.ToString();}
 void DragTitle(object sender,MouseButtonEventArgs e){if(e.ChangedButton==MouseButton.Left)DragMove();}
 void Apply(object sender,RoutedEventArgs e)
 {
  if(!int.TryParse(Interval.Text,out int value)||value is <0 or >100000){Error.Text="请输入 0 到 100000 毫秒。";return;}
  IntervalMs=value;DialogResult=true;
 }
 void Cancel(object sender,RoutedEventArgs e)=>Close();
}
