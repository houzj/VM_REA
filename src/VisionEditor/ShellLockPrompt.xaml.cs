using System.Windows;

namespace VisionEditor;

public partial class ShellLockPrompt : Window
{
 public bool SkipNextTime=>SkipPrompt.IsChecked==true;
 public ShellLockPrompt(){InitializeComponent();}
 void ConfirmClick(object sender,RoutedEventArgs e)=>DialogResult=true;
 void CancelClick(object sender,RoutedEventArgs e)=>DialogResult=false;
}
