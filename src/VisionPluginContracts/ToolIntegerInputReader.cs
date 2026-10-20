using VisionFramework;
namespace VisionPluginContracts;
// Optional integer getter phase. Lifecycle is the existing execution scope;
// native integer payloads are signed 32-bit. No implicit provider is installed.
public readonly record struct ToolIntegerInputReadResult(uint ErrorCode,int Count,int Value);
public interface IToolIntegerInputReader {
 ToolIntegerInputReadResult Read(ModuleExecutionContext context,string inputName,int index);
}
