using System.Text.Json;
using VisionFramework;
namespace VisionPluginContracts;
public sealed record ToolManifest(string Id,string Version,string Name,string Category,string Icon);
public sealed record ToolEditorContext(NodeDocument Node,string BaseDirectory,Action<Dictionary<string,JsonElement>> Apply,Func<Dictionary<string,JsonElement>,Task<NodeRunRecord>> Run,int? DisplayIndex=null){public IToolDebugSession? Session{get;init;}};
public interface IVisionToolPlugin {ToolManifest Manifest{get;}ModuleDescriptor Descriptor{get;}IModule CreateRuntime();Form CreateEditor(ToolEditorContext context);void Reset(string nodeId);}
public sealed class ToolPluginRegistry {
 readonly Dictionary<string,IVisionToolPlugin> plugins=new();
 public IEnumerable<IVisionToolPlugin> Plugins=>plugins.Values;
 public void Register(IVisionToolPlugin plugin,ModuleRegistry modules){if(plugin.Manifest.Id!=plugin.Descriptor.Id||plugin.Manifest.Version!=plugin.Descriptor.Version)throw new ArgumentException("Plugin identity mismatch");if(!plugins.TryAdd(plugin.Manifest.Id,plugin))throw new InvalidOperationException("Duplicate plugin");modules.Register(plugin.Descriptor,plugin.CreateRuntime);}
 public IVisionToolPlugin? Find(string id)=>plugins.GetValueOrDefault(id);
}

public sealed record ToolImageEntry(string Path,bool Selected);
public interface IToolImageListProvider {IReadOnlyList<ToolImageEntry> GetImages(VisionFramework.NodeDocument node);}
