using System.IO;
using System.Text.Json;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.BranchString;
public sealed class BranchStringPlugin:IVisionToolPlugin,IToolConfigurationProvider,IWpfToolEditorFactory,IToolDisplayProvider,IToolAssetPersistence,IToolExecutionTopologyProvider,IToolNodeCopyProvider,IToolNodeRestoreProvider {
 public const string Id="vision.branch-string";
 public ToolManifest Manifest{get;}=new(Id,"1.0.0","分支字符串","logic","catalog/BranchStringCpmLImageEnable.png");
 public ModuleDescriptor Descriptor=>BranchStringRuntime.Description;
 public ToolDisplayDefinition DisplayDefinition{get;}=new("Input Params:{0} , Branch RunWay:{1} ",["StringMatch","BranchMatch"],["ModuStatus","BranchMatch"]);
 sealed class DisplayState{public readonly object Gate=new();public Dictionary<string,PortValue>? Inputs,Outputs;public ToolDisplayDiagnostic? Diagnostic;}
 readonly object gate=new();readonly Dictionary<string,DisplayState> fallback=new();
 DisplayState State(string node,string directory,ModuleExecutionScope? scope){if(scope is not null)return scope.State(Id,node,()=>new DisplayState());string key=Path.GetFullPath(directory)+"|"+node;lock(gate){if(!fallback.TryGetValue(key,out var value))fallback[key]=value=new();return value;}}
 public void Reset(string nodeId){lock(gate)foreach(var key in fallback.Keys.Where(k=>k.EndsWith("|"+nodeId,StringComparison.Ordinal)).ToArray())fallback.Remove(key);}
 public IModule CreateRuntime()=>new BranchStringRuntime((context,result)=>{var state=State(context.NodeId,context.BaseDirectory,context.Scope);lock(state.Gate){state.Inputs=context.Inputs.ToDictionary(p=>p.Key,p=>p.Value);state.Outputs=result.Outputs.ToDictionary(p=>p.Key,p=>p.Value);state.Diagnostic=result.Status==ModuleStatus.Failed?new(result.NativeErrorCode??0,result.Message??result.ErrorCode??"Branch String input unavailable"):null;}});
 public ToolDisplayFrame? LastDisplay(NodeDocument node,string baseDirectory,ModuleExecutionScope? scope){var state=State(node.Id,baseDirectory,scope);lock(state.Gate){if(state.Outputs is null)return null;var result=ToolDisplayService.Build(DisplayDefinition,ToolDisplayService.Read(node.Parameters,DisplayDefinition),state.Inputs??[],state.Outputs,diagnostic:state.Diagnostic);string status=state.Outputs.TryGetValue("ModuStatus",out var s)?s.As<long>().ToString(System.Globalization.CultureInfo.InvariantCulture):"",match=state.Outputs.TryGetValue("BranchMatch",out var m)?m.As<string>():"";return result with{RecordText="Module Status:"+status+";Branch MatchWay:"+match};}}
 internal static string Resource(string name){using var stream=typeof(BranchStringPlugin).Assembly.GetManifestResourceStream("VisionPlugins.BranchString.Resources."+name)??throw new InvalidDataException("Missing original Branch String resource: "+name);using var reader=new StreamReader(stream);return reader.ReadToEnd();}
 public ToolConfigurationSession CreateConfiguration(ToolEditorContext context)=>BranchStringConfiguration.Create(context,Descriptor);
 public System.Windows.Window CreateWpfEditor(ToolEditorContext context)=>new ToolConfigurationModelWindow(context,CreateConfiguration(context));
 public System.Windows.Forms.Form CreateEditor(ToolEditorContext context)=>throw new InvalidOperationException("Use the common WPF configuration host.");
 public ToolAssetSnapshot PrepareSave(NodeDocument node,string baseDirectory,ResourceCatalog resources)=>new(node,[]);
 public NodeDocument ApplyExecutionTopologyChange(NodeDocument node,IReadOnlyList<ExecutionSuccessor> previous,IReadOnlyList<ExecutionSuccessor> current){if(node.ModuleId!=Id)throw new ArgumentException("Unexpected Branch String node.");if(previous.Select(n=>n.NodeId).SequenceEqual(current.Select(n=>n.NodeId)))return node;return SetTable(node,BranchStringTable.Topology(BranchStringTable.ReadCanonical(Rows(node)),Convert(previous),Convert(current)));}
 public NodeDocument PrepareCopiedNode(NodeDocument source,NodeDocument copy,IReadOnlyDictionary<string,string> nodeMap,IReadOnlyList<ExecutionSuccessor> copiedSuccessors){if(source.ModuleId!=Id||copy.ModuleId!=Id)throw new ArgumentException("Unexpected Branch String copy node.");var seeded=BranchStringTable.Topology([],[],Convert(copiedSuccessors));string? mapped=BranchStringTable.CopyRows(Rows(source),nodeMap);return SetTable(copy,mapped is null?seeded:BranchStringTable.ApplySetter(mapped));}
 // Root's optional restore interface will bind this pure method after review.
 // Save/opening configuration do not call the native setter roundtrip.
 public NodeDocument PrepareRestoredNode(NodeDocument node){if(node.ModuleId!=Id)throw new ArgumentException("Unexpected restored Branch String node.");return SetTable(node,BranchStringTable.ApplySetter(Rows(node)));}
 static string Rows(NodeDocument node)=>node.Parameters.TryGetValue("ModuleInfoList",out var rows)?rows.GetString()??"":"";
 static IReadOnlyList<BranchStringSuccessor> Convert(IReadOnlyList<ExecutionSuccessor> nodes)=>nodes.Select(n=>new BranchStringSuccessor(n.NodeId,n.NumericId)).ToArray();
 static NodeDocument SetTable(NodeDocument node,IReadOnlyList<BranchStringRow> rows){var parameters=node.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());parameters["ModuleInfoList"]=JsonSerializer.SerializeToElement(BranchStringTable.Serialize(rows));return node with{Parameters=parameters};}
}

