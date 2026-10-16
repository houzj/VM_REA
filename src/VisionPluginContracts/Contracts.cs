using System.Text.Json;
using System.IO;
using VisionFramework;
namespace VisionPluginContracts;
public sealed record ToolManifest(string Id,string Version,string Name,string Category,string Icon);
public enum ToolEditorMode { Desktop, SdkEmbedded }
public sealed record ToolEditorContext(NodeDocument Node,string BaseDirectory,Action<Dictionary<string,JsonElement>> Apply,Func<Dictionary<string,JsonElement>,Task<NodeRunRecord>> Run,int? DisplayIndex=null){
 public ModuleExecutionScope? Scope {get;init;}
 public ToolEditorMode Mode {get;init;}
 public Func<System.Windows.Window,string,bool,DataKind,string?>? PickSubscription {get;init;}
 public Func<string,bool,string?>? SubscriptionLabel {get;init;}
 public Func<string,int?>? InputInteger {get;init;}
 public Action<string,int?>? SetInputInteger {get;init;}
 public Func<IReadOnlyList<CameraInfo>>? Cameras {get;init;}
 public Action? ConfigureCameras {get;init;}
}
public interface IVisionToolPlugin {ToolManifest Manifest{get;}ModuleDescriptor Descriptor{get;}IModule CreateRuntime();Form CreateEditor(ToolEditorContext context);void Reset(string nodeId);}
public sealed class ToolPluginRegistry {
 readonly Dictionary<string,IVisionToolPlugin> plugins=new();
 public IEnumerable<IVisionToolPlugin> Plugins=>plugins.Values;
 public void Register(IVisionToolPlugin plugin,ModuleRegistry modules){if(plugin.Manifest.Id!=plugin.Descriptor.Id||plugin.Manifest.Version!=plugin.Descriptor.Version)throw new ArgumentException("Plugin identity mismatch");if(!plugins.TryAdd(plugin.Manifest.Id,plugin))throw new InvalidOperationException("Duplicate plugin");modules.Register(plugin.Descriptor,plugin.CreateRuntime);}
 public IVisionToolPlugin? Find(string id)=>plugins.GetValueOrDefault(id);
 public SolutionDocument PrepareSave(SolutionDocument document,string baseDirectory)=>PrepareSave(document,baseDirectory,null);
 public SolutionDocument PrepareSave(SolutionDocument document,string baseDirectory,ModuleExecutionScope? scope){
  var catalog=new ResourceCatalog(baseDirectory,document.Resources);var resources=document.Resources.Where(r=>!plugins.Values.Any(p=>p is IToolAssetPersistence&&r.Id.StartsWith("plugin:"+p.Manifest.Id+":",StringComparison.Ordinal))).ToDictionary(r=>r.Id,StringComparer.Ordinal);
  var procedures=document.Procedures.Select(procedure=>procedure with{Nodes=procedure.Nodes.Select(node=>{
   if(Find(node.ModuleId) is not IToolAssetPersistence participant)return node;
   var snapshot=participant.PrepareSave(node,baseDirectory,catalog,scope);foreach(var resource in snapshot.Resources)resources[resource.Id]=resource;return snapshot.Node;
  }).ToArray()}).ToArray();return document with{Procedures=procedures,Resources=resources.Values.ToArray()};
 }
}

public sealed record ToolImageEntry(string Path,bool Selected);
public sealed record ToolImageOutput(string Port,string Label);
public interface IToolImageOutputProvider {IReadOnlyList<ToolImageOutput> ImageOutputs{get;}}
public interface IToolImageListProvider {bool SupportsSingleImageExecution=>true;IReadOnlyList<ToolImageEntry> GetImages(VisionFramework.NodeDocument node);IReadOnlyList<ToolImageEntry> GetImages(NodeDocument node,ModuleExecutionScope scope)=>GetImages(node);}
public interface IToolImagePreviewProvider {VisionCore.IImageData LoadPreview(NodeDocument node,string path,string baseDirectory,ResourceCatalog? resources=null);Stream OpenEncodedImage(NodeDocument node,string path,string baseDirectory,ResourceCatalog? resources=null);}
public sealed record ToolAssetSnapshot(NodeDocument Node,IReadOnlyList<ResourceDocument> Resources);
public interface IToolAssetPersistence {ToolAssetSnapshot PrepareSave(NodeDocument node,string baseDirectory,ResourceCatalog resources);ToolAssetSnapshot PrepareSave(NodeDocument node,string baseDirectory,ResourceCatalog resources,ModuleExecutionScope? scope)=>PrepareSave(node,baseDirectory,resources);}
