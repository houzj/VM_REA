using VisionFramework;

namespace VisionPluginContracts;

/// <summary>A single numeric input read, including a count known on a failed read.
/// ErrorCode belongs to the provider's source protocol; consumers interpret it explicitly.</summary>
public readonly record struct ToolNumericInputReadResult(uint ErrorCode,int Count,double Value);

/// <summary>Optional synchronous input service registered on ModuleExecutionScope.
/// A count probe and a later value read are separate calls and may observe different
/// source states. Implementations return source read failures instead of throwing them.
/// No provider is installed implicitly and no native handle or state is persisted.</summary>
public interface IToolNumericInputReader
{
 ToolNumericInputReadResult Read(ModuleExecutionContext context,string inputName,int index);
}
