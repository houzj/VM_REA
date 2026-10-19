using System.Text.Json;
using System.IO;
using VisionFramework;
namespace VisionPluginContracts;
public sealed record ToolManifest(string Id,string Version,string Name,string Category,string Icon);
public enum ToolEditorMode { Desktop, SdkEmbedded }
public sealed record ToolEditorContext(NodeDocument Node,string BaseDirectory,Action<Dictionary<string,JsonElement>> Apply,Func<Dictionary<string,JsonElement>,Task<NodeRunRecord>> Run,int? DisplayIndex=null){
 public IToolDebugSession? Session{get;init;}public Func<string,PortValue?>? PreviewInput{get;init;}public Func<VisionCore.GrayImage?>? PreviewImage{get;init;}public Action<IReadOnlyDictionary<string,RoiDisplayStyle>>? ApplyDisplayStyles{get;init;}public Func<string,string>? InputLabel{get;init;}public Action<string>? ClearInput{get;init;}public Action<string,System.Windows.Window>? EditInput{get;init;}public Action<Dictionary<string,JsonElement>,IReadOnlyList<RoiDocument>,IReadOnlyCollection<string>>? ApplyConfigurationChange{get;init;}public Action<Dictionary<string,JsonElement>,IReadOnlyList<RoiDocument>>? ApplyConfiguration{get;init;}
 public IToolConfigurationFolderService? ConfigurationFolders{get;init;}public IToolConfigurationPresentationService? ConfigurationPresentation{get;init;}public IToolConfigurationFileService? ConfigurationFiles{get;init;}public IToolConfigurationNotificationService? ConfigurationNotifications{get;init;}public Func<ConfigurationExecutionHostStatus>? ConfigurationExecutionStatus{get;init;}public Func<bool>? ConfigurationActive{get;init;}public ModuleExecutionScope? Scope{get;init;}public Func<ResourceCatalog>? Resources{get;init;}public IToolConfigurationSubscriptionService? ConfigurationSubscriptions{get;init;}public ToolEditorMode Mode{get;init;}public Func<System.Windows.Window,string,bool,DataKind,string?>? PickSubscription{get;init;}public Func<string,bool,string?>? SubscriptionLabel{get;init;}public Func<string,int?>? InputInteger{get;init;}public Action<string,int?>? SetInputInteger{get;init;}public Func<string,double?>? InputNumber{get;init;}public Action<string,double?>? SetInputNumber{get;init;}public Func<string,IReadOnlyList<double>?>? InputNumbers{get;init;}public Action<string,IReadOnlyList<double>?>? SetInputNumbers{get;init;}public Func<string,string?>? InputText{get;init;}public Action<string,string?>? SetInputText{get;init;}public Func<IReadOnlyList<CameraInfo>>? Cameras{get;init;}public Action? ConfigureCameras{get;init;}public Action? ConfigureLights{get;init;}
};

public interface IVisionToolPlugin {ToolManifest Manifest{get;}ModuleDescriptor Descriptor{get;}IModule CreateRuntime();Form CreateEditor(ToolEditorContext context);void Reset(string nodeId);}
/// <summary>Pure node-specific input/output description from persisted parameters; no file I/O or runtime mutation.</summary>
public interface IToolNodeDescriptorProvider {ModuleDescriptor DescribeNode(NodeDocument node);}
public sealed class ToolPluginRegistry {
 readonly Dictionary<string,IVisionToolPlugin> plugins=new();
 public IEnumerable<IVisionToolPlugin> Plugins=>plugins.Values;
 bool registering;
 public void Register(IVisionToolPlugin plugin,ModuleRegistry modules)=>Register(plugin,modules,null,null);
 // Current-host identity guard; capture external metadata once before any registry mutation.
 public void Register(IVisionToolPlugin plugin,ModuleRegistry modules,string? expectedId,string? expectedVersion){
  ArgumentNullException.ThrowIfNull(plugin);ArgumentNullException.ThrowIfNull(modules);
  if(registering)throw new InvalidOperationException("Reentrant plugin registration");
  registering=true;
  try{
   var manifest=plugin.Manifest;var descriptor=plugin.Descriptor;
   if(manifest.Id!=descriptor.Id||manifest.Version!=descriptor.Version||expectedId is not null&&manifest.Id!=expectedId||expectedVersion is not null&&manifest.Version!=expectedVersion)throw new ArgumentException("Plugin identity mismatch");
   if(plugins.ContainsKey(manifest.Id))throw new InvalidOperationException("Duplicate plugin registration");
   // ModuleRegistry freezes and validates before publishing. No plugin getter runs after it commits.
   modules.Register(descriptor,plugin.CreateRuntime,plugin is IToolNodeDescriptorProvider dynamicProvider?dynamicProvider.DescribeNode:null);
   plugins.Add(manifest.Id,plugin);
  }finally{registering=false;}
 }

 public IVisionToolPlugin? Find(string id)=>plugins.GetValueOrDefault(id);
 public SolutionDocument PrepareSave(SolutionDocument document,string baseDirectory)=>PrepareSave(document,baseDirectory,null);
 public SolutionDocument PrepareSave(SolutionDocument document,string baseDirectory,ModuleExecutionScope? scope){
  var catalog=new ResourceCatalog(baseDirectory,document.Resources);var resources=document.Resources.Where(r=>!plugins.Values.Any(p=>p is IToolAssetPersistence&&r.Id.StartsWith("plugin:"+p.Manifest.Id+":",StringComparison.Ordinal))).ToDictionary(r=>r.Id,StringComparer.Ordinal);
  var procedures=document.Procedures.Select(procedure=>procedure with{Nodes=procedure.Nodes.Select(node=>{
   if(Find(node.ModuleId) is not IToolAssetPersistence participant)return node;
   var snapshot=participant.PrepareSave(node,baseDirectory,catalog,scope?.ForProcedure(procedure.Id));foreach(var resource in snapshot.Resources)resources[resource.Id]=resource;return snapshot.Node;
  }).ToArray()}).ToArray();return document with{Procedures=procedures,Resources=resources.Values.ToArray()};
 }
}

public sealed record ToolImageEntry(string Path,bool Selected);
public sealed record ToolImageOutput(string Port,string Label);
public interface IToolImageOutputProvider {IReadOnlyList<ToolImageOutput> ImageOutputs{get;}}
public interface IToolImageListProvider {bool SupportsSingleImageExecution=>true;bool IsImageListVisible(NodeDocument node)=>true;IReadOnlyList<ToolImageEntry> GetImages(VisionFramework.NodeDocument node);IReadOnlyList<ToolImageEntry> GetImages(NodeDocument node,ModuleExecutionScope scope)=>GetImages(node);}
public interface IToolImagePreviewProvider {VisionCore.IImageData LoadPreview(NodeDocument node,string path,string baseDirectory,ResourceCatalog? resources=null);Stream OpenEncodedImage(NodeDocument node,string path,string baseDirectory,ResourceCatalog? resources=null);}
public sealed record ToolAssetSnapshot(NodeDocument Node,IReadOnlyList<ResourceDocument> Resources);
public interface IToolAssetPersistence {ToolAssetSnapshot PrepareSave(NodeDocument node,string baseDirectory,ResourceCatalog resources);ToolAssetSnapshot PrepareSave(NodeDocument node,string baseDirectory,ResourceCatalog resources,ModuleExecutionScope? scope)=>PrepareSave(node,baseDirectory,resources);}
