using System.Windows;
using System.Windows.Interop;
namespace VisionPluginContracts;
public sealed record ConfigurationFolderRequest(string CurrentPath,bool Remote=false);
public sealed record ConfigurationFolderSelection(ConfigurationFileStatus Status,string? Path=null,string? Diagnostic=null);
public interface IToolConfigurationFolderService {ConfigurationFolderSelection Select(Window? owner,ConfigurationFolderRequest request);}
/// <summary>Shared directory chooser; source OpenFolderDialog validates the chosen path separately before committing.</summary>
public sealed class WpfConfigurationFolderService:IToolConfigurationFolderService {
 readonly Func<Window?,ConfigurationFolderRequest,ConfigurationFolderSelection> picker;
 public WpfConfigurationFolderService(Func<Window?,ConfigurationFolderRequest,ConfigurationFolderSelection>? picker=null){this.picker=picker??Pick;}
 public ConfigurationFolderSelection Select(Window? owner,ConfigurationFolderRequest request){if(request.Remote)return new(ConfigurationFileStatus.Unsupported,Diagnostic:"Remote directory browsing protocol is not implemented.");try{return picker(owner,request);}catch(Exception error)when(error is ArgumentException or InvalidOperationException or System.ComponentModel.Win32Exception){return new(ConfigurationFileStatus.Failed,Diagnostic:error.Message);}}
 static ConfigurationFolderSelection Pick(Window? owner,ConfigurationFolderRequest request){using var dialog=new System.Windows.Forms.FolderBrowserDialog{SelectedPath=request.CurrentPath,ShowNewFolderButton=true};var result=owner is null?dialog.ShowDialog():dialog.ShowDialog(new NativeOwner(new WindowInteropHelper(owner).Handle));return result==System.Windows.Forms.DialogResult.OK?new(ConfigurationFileStatus.Selected,dialog.SelectedPath.Trim()):new(ConfigurationFileStatus.Cancelled);}
 sealed class NativeOwner(IntPtr handle):System.Windows.Forms.IWin32Window{public IntPtr Handle=>handle;}
}
public enum ConfigurationPresentationKind { ImageSourceMode }
public enum ImageSourcePresentationMode { Local=1, Camera=2, Sdk=3 }
public enum ConfigurationPresentationStatus { Supported, Unsupported, Failed }
public sealed record ConfigurationPresentationRequest(string NodeId,ConfigurationPresentationKind Kind,ImageSourcePresentationMode SourceMode);
public sealed record ConfigurationPresentationResult(ConfigurationPresentationStatus Status,string? Diagnostic=null);
/// <summary>Typed presentation effects from original PropertyManager.DisplayExecute; not arbitrary method execution.</summary>
public interface IToolConfigurationPresentationService {ConfigurationPresentationResult Apply(ConfigurationPresentationRequest request);}
