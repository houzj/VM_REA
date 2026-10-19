using System.Text.Json;
using System.Windows.Forms;
using VisionFramework;
using VisionPluginContracts;

namespace ManagedToolFailureFixture19;

// All code and cases here are synthetic and device/native free.
public abstract class PluginSupport
{
 public static int RuntimeCreates;
 public abstract string Id {get;}
 public virtual string Version=>"1.0.0";
 public virtual ToolManifest Manifest=>new(Id,Version,Id,"Fixture","");
 public virtual ModuleDescriptor Descriptor=>Metadata(Id,Version);
 public static ModuleDescriptor Metadata(string id,string version)=>new(id,version,id,[],[new("Value",DataKind.Integer)],[new("Value",ParameterKind.Integer,true,JsonSerializer.SerializeToElement(19))]);
 public IModule CreateRuntime(){RuntimeCreates++;return new FixtureRuntime(Descriptor);}
 public Form CreateEditor(ToolEditorContext context)=>new(){Text="Owned failure fixture"};
 public void Reset(string nodeId){}
 sealed class FixtureRuntime(ModuleDescriptor descriptor):IModule
 {
  public ModuleDescriptor Descriptor=>descriptor;
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken cancellationToken)=>new(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"Value",PortValue.Integer(context.Integer("Value"))}}));
 }
}
public sealed class PrefixPlugin:PluginSupport,IVisionToolPlugin {public override string Id=>"fixture.prefix19";}
public sealed class AliasPlugin:PluginSupport,IVisionToolPlugin {public override string Id=>"fixture.prefix19";}
public sealed class VersionTwoPlugin:PluginSupport,IVisionToolPlugin {public override string Id=>"fixture.prefix19";public override string Version=>"2.0.0";}
public sealed class BuiltinConflictPlugin:PluginSupport,IVisionToolPlugin {public override string Id=>"vision.threshold";}
public sealed class ConstructorFailurePlugin:PluginSupport,IVisionToolPlugin
{
 public ConstructorFailurePlugin()=>throw new InvalidOperationException("owned constructor failure nineteen");
 public override string Id=>"fixture.constructor19";
}
public sealed class NoPublicConstructorPlugin:PluginSupport,IVisionToolPlugin
{
 private NoPublicConstructorPlugin(){}
 public override string Id=>"fixture.private19";
}
public sealed class VolatileDescriptorPlugin:PluginSupport,IVisionToolPlugin
{
 public static bool FailDescriptor;
 public static int DescriptorReads;
 public override string Id=>"fixture.volatile19";
 public override ModuleDescriptor Descriptor {get {DescriptorReads++;var descriptor=base.Descriptor;return FailDescriptor?descriptor with{Inputs=null!}:descriptor;}}
}
public sealed class FlippingManifestPlugin:PluginSupport,IVisionToolPlugin
{
 int reads;
 public override string Id=>"fixture.capture19";
 public override ToolManifest Manifest=>new(++reads==1?"fixture.capture19":"fixture.drift19",Version,Id,"Fixture","");
}
public sealed class ChangingIdentityPlugin:PluginSupport,IVisionToolPlugin
{
 public static string CurrentId="fixture.identity19";
 public override string Id=>CurrentId;
}
public sealed class DependencyPlugin:ManagedToolDependencyFixture19.DependencyBase,IVisionToolPlugin
{
 public ToolManifest Manifest=>new("fixture.dependency19","1.0.0","Dependency","Fixture","");
 public ModuleDescriptor Descriptor=>PluginSupport.Metadata("fixture.dependency19","1.0.0");
 public IModule CreateRuntime()=>throw new NotSupportedException();
 public Form CreateEditor(ToolEditorContext context)=>throw new NotSupportedException();
 public void Reset(string nodeId){}
}
public sealed class NotAPlugin { }
