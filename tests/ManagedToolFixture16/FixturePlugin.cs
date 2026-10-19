using System.Text.Json;
using System.Windows.Forms;
using VisionFramework;
using VisionPluginContracts;

namespace ManagedToolFixture16;

// Entirely synthetic host-owned fixture. No vendor code, files, native calls or devices.
public sealed class FixturePlugin:IVisionToolPlugin
{
 public ToolManifest Manifest=>new("fixture.dynamic","1.0.0","Dynamic fixture","Fixture","");
 public ModuleDescriptor Descriptor=>FixtureRuntime.Metadata;
 public IModule CreateRuntime()=>new FixtureRuntime();
 public Form CreateEditor(ToolEditorContext context)=>new(){Text="Synthetic fixture editor"};
 public void Reset(string nodeId){}
}
public sealed class NotAPlugin { public NotAPlugin(){} }
public sealed class ThrowingPlugin:IVisionToolPlugin
{
 public ThrowingPlugin()=>throw new InvalidOperationException("owned fixture constructor failure");
 public ToolManifest Manifest=>throw new NotSupportedException();
 public ModuleDescriptor Descriptor=>throw new NotSupportedException();
 public IModule CreateRuntime()=>throw new NotSupportedException();
 public Form CreateEditor(ToolEditorContext context)=>throw new NotSupportedException();
 public void Reset(string nodeId){}
}
public sealed class FixtureRuntime:IModule
{
 public static ModuleDescriptor Metadata=>new("fixture.dynamic","1.0.0","Dynamic fixture",[],[new("Value",DataKind.Integer)],[new("Value",ParameterKind.Integer,true,JsonSerializer.SerializeToElement(42))]);
 public ModuleDescriptor Descriptor=>Metadata;
 public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken cancellationToken)
 {
  cancellationToken.ThrowIfCancellationRequested();
  return new(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"Value",PortValue.Integer(context.Integer("Value"))}}));
 }
}
