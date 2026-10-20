using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.ContourMatch;

/// <summary>Source-backed configuration and result policy. Matching backend is not yet ported.</summary>
public sealed partial class ContourMatchPlugin : IVisionToolPlugin, IWpfToolEditorFactory, IToolConfigurationProvider
{
    public const string Id = "vision.contour-match";
    public ToolManifest Manifest { get; } = new(Id,"1.0.0","轮廓匹配","location","catalog/IMVSContourMatchModuImageEnable.png");
    public ModuleDescriptor Descriptor { get; } = new(Id,"1.0.0","轮廓匹配",
        [new("InputImage",DataKind.Image)], [], ContourParameterDescriptors.Parameters)
        { RoiEditor = new([RoiKind.Rectangle,RoiKind.Polygon],Maximum:1,FullImage:true,Exclusions:false) };
    public System.Windows.Forms.Form CreateEditor(ToolEditorContext context) => throw new InvalidOperationException("Use WPF editor factory.");
    public System.Windows.Window CreateWpfEditor(ToolEditorContext context) => new ToolConfigurationModelWindow(context,CreateConfiguration(context));
    public IModule CreateRuntime() => new Runtime(this);
    public void Reset(string nodeId) { }
    private sealed class Runtime(ContourMatchPlugin owner) : IModule
    {
        public ModuleDescriptor Descriptor => owner.Descriptor;
        public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            // Explicit implementation boundary, not a native return code or a zero-match success.
            return ValueTask.FromResult(new ModuleResult(ModuleStatus.Failed,new Dictionary<string,PortValue>(),
                "ContourMatchingKernelNotImplemented","Original single/multiple-template kernels and training/model ownership have not yet been ported."));
        }
    }
    private static string Resource(string name) => ContourSourceConfiguration.Resource(name);
    private static ToolDisplayDefinition DisplayDefinition { get; } = new("Module Status:{0}",["ModuStatus"],["ModuRunTime","ModuStatus","MatchNum"],"InputImage");
}
