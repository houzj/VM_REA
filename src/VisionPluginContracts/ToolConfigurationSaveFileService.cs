using System.IO;
using System.Windows;

namespace VisionPluginContracts;

public sealed record ConfigurationSaveFileRequest(string ModuleKey,string CurrentPath,string Filter,string Title,string DefaultExtension="",bool Remote=false);
public interface IToolConfigurationSaveFileService {
 ConfigurationFileSelection SelectSave(Window? owner,ConfigurationSaveFileRequest request);
 ConfigurationFileHistoryResult Remember(string moduleKey,string selectedFile);
}
/// <summary>Save protocol is separate from selecting existing files. Actual export stays in the module adapter.</summary>
public sealed class WpfConfigurationSaveFileService:IToolConfigurationSaveFileService {
 readonly ConfigurationFileHistory history;
 readonly Func<Window?,ConfigurationSaveFileRequest,string,ConfigurationFileSelection> picker;
 public WpfConfigurationSaveFileService(ConfigurationFileHistory? history=null,Func<Window?,ConfigurationSaveFileRequest,string,ConfigurationFileSelection>? picker=null){this.history=history??new();this.picker=picker??Pick;}
 public ConfigurationFileSelection SelectSave(Window? owner,ConfigurationSaveFileRequest request){
  if(request.Remote)return new(ConfigurationFileStatus.Unsupported,[],"Remote save/upload protocol is not implemented.");
  if(string.IsNullOrWhiteSpace(request.ModuleKey))return new(ConfigurationFileStatus.Failed,[],"Missing module history key.");
  try{
   var previous=history.Get(request.ModuleKey);if(!previous.Success)return new(ConfigurationFileStatus.Failed,[],previous.Diagnostic);
   var directory=previous.Directory??"";
   if(!string.IsNullOrWhiteSpace(request.CurrentPath)){var current=Path.GetDirectoryName(Path.GetFullPath(request.CurrentPath));if(Directory.Exists(current))directory=current!;}
   if(!Directory.Exists(directory))directory="";
   var result=picker(owner,request,directory);
   if(result.Status==ConfigurationFileStatus.Selected&&(result.Paths.Count!=1||string.IsNullOrWhiteSpace(result.Paths[0])||!Directory.Exists(Path.GetDirectoryName(Path.GetFullPath(result.Paths[0])))))return new(ConfigurationFileStatus.Failed,[],"Save picker must return one path with an existing parent directory.");
   return result;
  }catch(Exception error)when(error is IOException or UnauthorizedAccessException or InvalidOperationException or ArgumentException or NotSupportedException){return new(ConfigurationFileStatus.Failed,[],error.Message);}
 }
 public ConfigurationFileHistoryResult Remember(string moduleKey,string selectedFile)=>history.Remember(moduleKey,selectedFile);
 static ConfigurationFileSelection Pick(Window? owner,ConfigurationSaveFileRequest request,string directory){
  var dialog=new Microsoft.Win32.SaveFileDialog{Title=request.Title,Filter=request.Filter,DefaultExt=request.DefaultExtension,InitialDirectory=directory,CheckPathExists=true,OverwritePrompt=true};
  bool chosen=owner is null?dialog.ShowDialog()==true:dialog.ShowDialog(owner)==true;
  return chosen?new(ConfigurationFileStatus.Selected,[dialog.FileName]):new(ConfigurationFileStatus.Cancelled,[]);
 }
}
