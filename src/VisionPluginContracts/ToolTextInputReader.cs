using VisionFramework;
namespace VisionPluginContracts;
// Full UTF-8 text is owned by the managed provider. Capacity and required bytes
// model the source read/retry protocol; no native pointer is exposed/persisted.
public readonly record struct ToolTextInputReadResult(uint ErrorCode,int Count,string Value,int RequiredBytes);
public interface IToolTextInputReader {
 ToolTextInputReadResult Read(ModuleExecutionContext context,string inputName,int index,int capacity);
}
