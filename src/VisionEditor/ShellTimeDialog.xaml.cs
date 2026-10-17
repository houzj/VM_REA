using System.Windows;

namespace VisionEditor;

public partial class ShellTimeDialog : Window
{
 public sealed record TimeRow(string Name,string ModuleTime,string AlgorithmTime);
 public ShellTimeDialog(double flowMilliseconds,IReadOnlyList<TimeRow> rows)
 {
  InitializeComponent();
  FlowTimeValue.Text=flowMilliseconds.ToString("N2",System.Globalization.CultureInfo.CurrentCulture);
  TimingList.ItemsSource=rows;
 }
}
