using System.Text.Json;
using VisionFramework;
using VisionCore;
namespace VisionPluginContracts;
// Optional dedicated parameter factory; Group retains its atomic multi-target write callback.
public sealed record ToolParameterEditorContext(NodeDocument Node,ParameterDescriptor Parameter,GrayImage? Image,Action<IReadOnlyDictionary<string,JsonElement>,IReadOnlyList<RoiDocument>?> Apply);
public interface IWpfToolParameterEditorFactory {System.Windows.Window CreateParameterEditor(ToolParameterEditorContext context);}
