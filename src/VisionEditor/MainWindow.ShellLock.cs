namespace VisionEditor;

public partial class MainWindow
{
 void RequestWorkAreaLock()
 {
  if(isLocked){ToggleLock();return;}
  if(ShellLockPreference.Read()){ToggleLock();return;}
  var prompt=new ShellLockPrompt{Owner=this};
  bool accepted=prompt.ShowDialog()==true;
  ShellLockPreference.Write(prompt.SkipNextTime);
  if(accepted)ToggleLock();
 }
}
