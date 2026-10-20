using System.IO;
using System.Text.Json;
using System.Windows;

namespace VisionPluginContracts;

public enum ConfigurationFileStatus { Selected, Cancelled, Unsupported, Failed }
public sealed record ConfigurationFileRequest(string ModuleKey,string CurrentPath,string Filter,string Title,bool Multiselect=false,bool Remote=false,bool CheckFileExists=true,bool CheckPathExists=true);
public sealed record ConfigurationFileSelection(ConfigurationFileStatus Status,IReadOnlyList<string> Paths,string? Diagnostic=null);
public sealed record ConfigurationFileHistoryResult(bool Success,string? Directory=null,string? Diagnostic=null);
public interface IToolConfigurationFileService {
 ConfigurationFileSelection Select(Window? owner,ConfigurationFileRequest request);
 ConfigurationFileHistoryResult Remember(string moduleKey,string selectedFile);
}
public sealed record ConfigurationFilePickerOptions(string InitialDirectory,string Filter,string Title,bool Multiselect,bool CheckFileExists=true,bool CheckPathExists=true);
/// <summary>Independent user JSON mapping of original ConfigurationHelper's per-module INI open directory.</summary>
public sealed class ConfigurationFileHistory {
 static readonly System.Collections.Concurrent.ConcurrentDictionary<string,object> gates=new(StringComparer.OrdinalIgnoreCase);
 readonly string file;readonly object gate;
 public string StoragePath=>file;
 public ConfigurationFileHistory(string? directory=null){file=Path.Combine(directory??Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"VisionReconstruction","Configuration"),"open-paths.json");gate=gates.GetOrAdd(Path.GetFullPath(file),_=>new());}
 Dictionary<string,string> Read()=>File.Exists(file)?JsonSerializer.Deserialize<Dictionary<string,string>>(File.ReadAllText(file))??[]:[];
 public ConfigurationFileHistoryResult Get(string moduleKey){try{lock(gate)return new(true,Read().GetValueOrDefault(moduleKey));}catch(Exception error)when(error is IOException or UnauthorizedAccessException or JsonException){return new(false,Diagnostic:error.Message);}}
 public ConfigurationFileHistoryResult Remember(string moduleKey,string selectedFile){
  if(string.IsNullOrWhiteSpace(moduleKey))return new(false,Diagnostic:"Missing module history key.");
  try{var directory=Path.GetDirectoryName(Path.GetFullPath(selectedFile));if(directory is null||!Directory.Exists(directory))return new(false,Diagnostic:"Selected directory does not exist.");lock(gate){var values=Read();values[moduleKey]=directory;Directory.CreateDirectory(Path.GetDirectoryName(file)!);var temporary=file+"."+Guid.NewGuid().ToString("N")+".tmp";try{File.WriteAllText(temporary,JsonSerializer.Serialize(values));File.Move(temporary,file,true);}finally{if(File.Exists(temporary))File.Delete(temporary);}return new(true,directory);}}
  catch(Exception error)when(error is IOException or UnauthorizedAccessException or JsonException or ArgumentException or NotSupportedException){return new(false,Diagnostic:error.Message);}
 }
}
/// <summary>Shared desktop implementation; choosing does not change history until the caller completes SetCurValue.</summary>
public sealed class WpfConfigurationFileService:IToolConfigurationFileService {
 readonly ConfigurationFileHistory history;readonly Func<Window?,ConfigurationFilePickerOptions,ConfigurationFileSelection> picker;
 public WpfConfigurationFileService(ConfigurationFileHistory? history=null,Func<Window?,ConfigurationFilePickerOptions,ConfigurationFileSelection>? picker=null){this.history=history??new();this.picker=picker??Pick;}
 public ConfigurationFileSelection Select(Window? owner,ConfigurationFileRequest request){
  if(request.Remote)return new(ConfigurationFileStatus.Unsupported,[],"Original IsUploadServer remote file protocol is not implemented.");
  if(string.IsNullOrWhiteSpace(request.ModuleKey))return new(ConfigurationFileStatus.Failed,[],"Missing module history key.");
  try{
   var stored=history.Get(request.ModuleKey);if(!stored.Success)return new(ConfigurationFileStatus.Failed,[],stored.Diagnostic);
   var initial=stored.Directory??"";
   if(File.Exists(request.CurrentPath))initial=Path.GetDirectoryName(Path.GetFullPath(request.CurrentPath))??initial;
   if(!Directory.Exists(initial))initial="";
   var result=picker(owner,new(initial,request.Filter,request.Title,request.Multiselect,request.CheckFileExists,request.CheckPathExists));
   if(result.Status==ConfigurationFileStatus.Selected&&(result.Paths.Count==0||!request.Multiselect&&result.Paths.Count!=1||result.Paths.Any(p=>request.CheckFileExists&&!File.Exists(p)||request.CheckPathExists&&!Directory.Exists(Path.GetDirectoryName(Path.GetFullPath(p))))))return new(ConfigurationFileStatus.Failed,[],"Picker did not return existing files.");
   return result;
  }catch(Exception error)when(error is IOException or UnauthorizedAccessException or ArgumentException or InvalidOperationException or NotSupportedException){return new(ConfigurationFileStatus.Failed,[],error.Message);}
 }
 public ConfigurationFileHistoryResult Remember(string moduleKey,string selectedFile)=>history.Remember(moduleKey,selectedFile);
 static ConfigurationFileSelection Pick(Window? owner,ConfigurationFilePickerOptions options){var picker=new Microsoft.Win32.OpenFileDialog{InitialDirectory=options.InitialDirectory,Filter=options.Filter,Title=options.Title,Multiselect=options.Multiselect,CheckFileExists=options.CheckFileExists,CheckPathExists=options.CheckPathExists};bool selected=owner is null?picker.ShowDialog()==true:picker.ShowDialog(owner)==true;return selected?new(ConfigurationFileStatus.Selected,picker.FileNames):new(ConfigurationFileStatus.Cancelled,[]);}
}
public enum ConfigurationNotificationKind { Information, Success, Error, Warning }
public enum ConfigurationNotificationStatus { Displayed, Suppressed, Failed }
public sealed record ConfigurationNotification(ConfigurationNotificationKind Kind,string Message,string Caption="",ToolEditorMode Mode=ToolEditorMode.Desktop);
public sealed record ConfigurationNotificationResult(ConfigurationNotificationStatus Status,string? Diagnostic=null);
public interface IToolConfigurationNotificationService { ConfigurationNotificationResult Notify(Window? owner,ConfigurationNotification notification); }
/// <summary>Shared independent WPF dialog presentation, not the original HikMessageBox visual template.</summary>
public sealed class WpfConfigurationNotificationService:IToolConfigurationNotificationService {
 readonly Action<Window?,ConfigurationNotification> show;
 public WpfConfigurationNotificationService(Action<Window?,ConfigurationNotification>? presenter=null){show=presenter??Show;}
 public ConfigurationNotificationResult Notify(Window? owner,ConfigurationNotification notification){if(notification.Mode==ToolEditorMode.SdkEmbedded)return new(ConfigurationNotificationStatus.Suppressed);try{var dispatcher=owner?.Dispatcher??System.Windows.Application.Current?.Dispatcher;if(dispatcher is not null&&!dispatcher.CheckAccess())dispatcher.Invoke(()=>show(owner,notification));else show(owner,notification);return new(ConfigurationNotificationStatus.Displayed);}catch(Exception error)when(error is InvalidOperationException or ArgumentException){return new(ConfigurationNotificationStatus.Failed,error.Message);}}
 static void Show(Window? owner,ConfigurationNotification notification){var icon=notification.Kind==ConfigurationNotificationKind.Error?MessageBoxImage.Error:notification.Kind==ConfigurationNotificationKind.Warning?MessageBoxImage.Warning:MessageBoxImage.Information;if(owner is null)System.Windows.MessageBox.Show(notification.Message,notification.Caption,MessageBoxButton.OK,icon);else System.Windows.MessageBox.Show(owner,notification.Message,notification.Caption,MessageBoxButton.OK,icon);}
}
