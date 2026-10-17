using System.Windows;
using System.Windows.Input;
namespace VisionEditor;
public partial class WorkflowWaitTimeWindow:Window
{
 public int WaitTimeMs{get;private set;}
 public WorkflowWaitTimeWindow(int waitTimeMs){InitializeComponent();WaitTimeMs=waitTimeMs;WaitTime.Increment=10;WaitTime.Maximum=100000;WaitTime.Text=waitTimeMs.ToString();}
 void DragTitle(object sender,MouseButtonEventArgs e){if(e.ChangedButton==MouseButton.Left)DragMove();}
 void Apply(object sender,RoutedEventArgs e)
 {
  if(!int.TryParse(WaitTime.Text,out int value)||value is <0 or >100000){Error.Text="请输入 0 到 100000 毫秒。";return;}
  WaitTimeMs=value;DialogResult=true;
 }
 void Cancel(object sender,RoutedEventArgs e)=>Close();
}
