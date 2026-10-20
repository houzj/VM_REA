using System.IO;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.BranchModule;
public sealed partial class BranchModulePlugin:IVisionToolPlugin,IToolConfigurationProvider,IWpfToolEditorFactory,IToolDisplayProvider,IToolAssetPersistence,IToolExecutionTopologyProvider,IToolNodeCopyProvider {
 public const string Id="vision.branch-module";
 public ToolManifest Manifest{get;}=new(Id,"1.0.0","分支模块","logic","catalog/BranchModule_STDImageEnable.png");
 public ModuleDescriptor Descriptor=>BranchModuleRuntime.Description;
 public ToolDisplayDefinition DisplayDefinition{get;}=new("Module Status:{0};Branch MatchWay:{1}",["ModuStatus","BranchMatch"],["ModuStatus"]);
 sealed class DisplayState {public readonly object Gate=new();public Dictionary<string,PortValue>? Inputs,Outputs;public ToolDisplayDiagnostic? Diagnostic;}
 readonly Dictionary<string,DisplayState> fallback=new();readonly object gate=new();
 DisplayState State(string node,string directory,ModuleExecutionScope? scope){if(scope is not null)return scope.State(Id,node,()=>new DisplayState());string key=Path.GetFullPath(directory)+"|"+node;lock(gate){if(!fallback.TryGetValue(key,out var value))fallback[key]=value=new();return value;}}
 public void Reset(string nodeId){lock(gate)foreach(var key in fallback.Keys.Where(k=>k.EndsWith("|"+nodeId,StringComparison.Ordinal)).ToArray())fallback.Remove(key);}
 public IModule CreateRuntime()=>new BranchModuleRuntime((context,result)=>{var state=State(context.NodeId,context.BaseDirectory,context.Scope);lock(state.Gate){state.Inputs=context.Inputs.ToDictionary(x=>x.Key,x=>x.Value);state.Outputs=result.Outputs.ToDictionary(x=>x.Key,x=>x.Value);state.Diagnostic=result.Status==ModuleStatus.Failed?new(result.NativeErrorCode??0,result.Message??result.ErrorCode??"Branch input unavailable"):null;}});
 public ToolDisplayFrame? LastDisplay(NodeDocument node,string baseDirectory,ModuleExecutionScope? scope){var state=State(node.Id,baseDirectory,scope);lock(state.Gate)return state.Outputs is null?null:ToolDisplayService.Build(DisplayDefinition,ToolDisplayService.Read(node.Parameters,DisplayDefinition),state.Inputs??[],state.Outputs,diagnostic:state.Diagnostic);}
 internal static string Resource(string name){using var stream=typeof(BranchModulePlugin).Assembly.GetManifestResourceStream("VisionPlugins.BranchModule.Resources."+name)??throw new InvalidDataException("Missing original Branch resource: "+name);using var reader=new StreamReader(stream);return reader.ReadToEnd();}
 public ToolConfigurationSession CreateConfiguration(ToolEditorContext context)=>BranchModuleConfiguration.Create(context,Descriptor);
 public System.Windows.Window CreateWpfEditor(ToolEditorContext context)=>new ToolConfigurationModelWindow(context,CreateConfiguration(context));
 public System.Windows.Forms.Form CreateEditor(ToolEditorContext context)=>throw new InvalidOperationException("Use the common WPF tool configuration host.");
 public ToolAssetSnapshot PrepareSave(NodeDocument node,string baseDirectory,ResourceCatalog resources)=>new(node,[]);
}


