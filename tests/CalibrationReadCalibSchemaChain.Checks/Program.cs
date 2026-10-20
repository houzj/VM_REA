using System.Text.Json;
using VisionCore;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
using VisionPlugins.ImageSource;

var root=Path.GetFullPath("analysis/joint-audit/pass12-cal-c-schema");
Directory.CreateDirectory(root);
var path=Path.Combine(root,"dynamic.xml");
string MatrixXml(string name,string type="FloatList")=>
    $"<R><CalibInputParam/><CalibOutputParam><P ParamName='{name}' DataType='{type}'>"+
    (type=="FloatList"?"<V>1</V><V>0</V><V>0</V><V>0</V><V>1</V><V>0</V><V>0</V><V>0</V><V>1</V>":"<V>1</V>")+
    "</P></CalibOutputParam></R>";
var original=MatrixXml("OldMatrix");
File.WriteAllText(path,original);
var registry=new ModuleRegistry();
WorkflowCalls.Register(registry);
var tools=new ToolPluginRegistry();
var loader=new ReadCalibFilePlugin();var coordinate=new CoordinateModulePlugin();var image=new ImageSourcePlugin();
tools.Register(loader,registry);tools.Register(coordinate,registry);tools.Register(image,registry);
Dictionary<string,JsonElement> Defaults(IVisionToolPlugin tool)=>tool.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());
var readValues=Defaults(loader);
readValues["FilePath"]=JsonSerializer.SerializeToElement(path);
readValues["OutputSchema"]=JsonSerializer.SerializeToElement(ReadCalibFilePlugin.Schema(ReadCalibFileData.Parse(original).Data!));
var coordinateValues=Defaults(coordinate);
coordinateValues["CordinateCreate"]=JsonSerializer.SerializeToElement(1);
coordinateValues["InputWay"]=JsonSerializer.SerializeToElement(2);
var imageValues=Defaults(image);
imageValues["ImageSourceType"]=JsonSerializer.SerializeToElement(3);
imageValues["StitchEnable"]=JsonSerializer.SerializeToElement(false);
using var graph=new GraphEditorDocument(registry);
var group=graph.AddNode(WorkflowCalls.ModuleId,WorkflowCalls.Version,0,0);
var body=graph.EnterOwnedGroup(group);
var read=graph.AddNode(ReadCalibFilePlugin.Id,"1.0.0",0,0,readValues);
graph.SetWorkflowConfiguration(body,new(){
    Subscriptions=[new(read,"RefreshSignal",false,new(WorkflowSourceKind.IntegerConstant,"1"))],
    Outputs=[new("matrix","Matrix",DataKind.NumberArray,read,"OldMatrix")]
});
graph.LeaveGroup();
var source=graph.AddNode(image.Manifest.Id,image.Manifest.Version,0,180,imageValues);
var consumer=graph.AddNode(CoordinateModulePlugin.Id,"1.0.0",200,180,coordinateValues);
graph.Connect(group,"Matrix",consumer,"CalibMatrix");
graph.Connect(source,"Image",consumer,"InputImage");
void Check(bool condition,string message){if(!condition)throw new InvalidOperationException(message);Console.WriteLine("PASS "+message);}
async Task<(NodeRunRecord Read,NodeRunRecord Group,NodeRunRecord Consumer,ExecutionSessionResult Child)> Run(GraphEditorDocument doc,ModuleExecutionScope scope,string label){
    image.SubmitImage(scope.ForProcedure(doc.CurrentProcedureId),source,new GrayImage(2,2,[10,20,30,40]));
    var session=await new SolutionExecutor(registry).RunAsync(new(doc.Document,doc.BaseDirectory),"main",scope:scope);
    var child=session.Calls[group];
    var result=(child.Nodes.Single(n=>n.NodeId==read),session.Nodes.Single(n=>n.NodeId==group),session.Nodes.Single(n=>n.NodeId==consumer),child);
    var dynamicPorts=string.Join(",",child.NodeDescriptors[read].Outputs.Where(p=>p.Name is "OldMatrix" or "NewMatrix").Select(p=>p.Name+":"+p.Kind));
    Console.WriteLine($"{label}: read={result.Item1.Status}/{result.Item1.ErrorCode}, dynamic={dynamicPorts}, group={result.Item2.Status}/{result.Item2.ErrorCode}, coordinate={result.Item3.Status}/{result.Item3.ErrorCode}, errors={string.Join(';',child.OutputErrors)}");
    return result;
}
using var scope=new ModuleExecutionScope();
var baseline=await Run(graph,scope,"before-rename");
Check(baseline.Read.Status==NodeRunStatus.Success&&baseline.Group.Status==NodeRunStatus.Success&&baseline.Consumer.Status==NodeRunStatus.Success,"OldMatrix FloatList reaches Group and Coordinate");
File.WriteAllText(path,MatrixXml("NewMatrix"));
var renamed=await Run(graph,scope,"renamed");
Check(renamed.Read.Status==NodeRunStatus.Success&&renamed.Child.NodeDescriptors[read].Outputs.Any(p=>p.Name=="NewMatrix")&&!renamed.Read.Outputs.ContainsKey("OldMatrix"),"refresh publishes renamed runtime descriptor and no old payload");
Check(renamed.Group.Status==NodeRunStatus.Failed&&renamed.Consumer.Status==NodeRunStatus.Skipped&&renamed.Child.OutputErrors.Any(e=>e.Contains("Matrix")),"stale Group selection fails without silently rebinding NewMatrix");
Check(graph.CurrentProcedure.Connections.Any(c=>c.FromNode==group&&c.ToNode==consumer&&c.FromPort=="Matrix"),"document connection identity remains selected after runtime rename");
var saved=Path.Combine(root,"dynamic-schema.vmx.json");graph.Save(saved);
using var reopened=new GraphEditorDocument(registry);reopened.Load(saved);
using var freshScope=new ModuleExecutionScope();
var replay=await Run(reopened,freshScope,"reopened-rename");
Check(replay.Group.Status==NodeRunStatus.Failed&&replay.Consumer.Status==NodeRunStatus.Skipped,"save/reopen does not secretly migrate old Group output source");
File.WriteAllText(path,MatrixXml("OldMatrix","int"));
var changedType=await Run(graph,scope,"changed-type");
Check(changedType.Read.Status==NodeRunStatus.Success&&changedType.Child.NodeDescriptors[read].Outputs.Any(p=>p.Name=="OldMatrix"&&p.Kind==DataKind.Integer),"same name refresh changes dynamic type to Integer");
Check(changedType.Group.Status==NodeRunStatus.Failed&&changedType.Consumer.Status==NodeRunStatus.Skipped&&changedType.Child.OutputErrors.Any(e=>e.Contains("type mismatch")),"Group NumberArray output rejects current Integer payload");
File.WriteAllText(path,original);
var restored=await Run(graph,scope,"restored");
Check(restored.Read.Status==NodeRunStatus.Success&&restored.Group.Status==NodeRunStatus.Success&&restored.Consumer.Status==NodeRunStatus.Success,"same scope recovers when exact original name and kind return");
var recoveredReopen=await Run(reopened,freshScope,"reopened-restored");
Check(recoveredReopen.Group.Status==NodeRunStatus.Success&&recoveredReopen.Consumer.Status==NodeRunStatus.Success,"reopened graph recovers after source file restoration");
Console.WriteLine("CalibrationReadCalibSchemaChain.Checks: rename/type/reopen/recovery graph states passed");
