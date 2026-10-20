using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using VisionCore;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
using VisionPlugins.CalibrationBoard;
using VisionPlugins.ImageSource;

if(args.Length!=1)throw new ArgumentException("Own output directory required");
var output=Path.GetFullPath(args[0]);Directory.CreateDirectory(output);
int checks=0;void Check(bool ok,string message){if(!ok)throw new Exception(message);Console.WriteLine($"PASS {++checks}: {message}");}
string Sha(string path)=>Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));
var image=new ImageSourcePlugin();var board=new CalibrationBoardPlugin();var rotate=new RotateCalibrationPlugin();
var registry=new ModuleRegistry();var plugins=new ToolPluginRegistry();
foreach(var plugin in new IVisionToolPlugin[]{image,board,rotate})plugins.Register(plugin,registry);
registry.Register(SampleSource.Definition,()=>new SampleSource());
Dictionary<string,JsonElement> Defaults(IVisionToolPlugin plugin)=>plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());
var ip=Defaults(image);ip["ImageSourceType"]=JsonSerializer.SerializeToElement(3);ip["StitchEnable"]=JsonSerializer.SerializeToElement(false);ip["OutMono8"]=JsonSerializer.SerializeToElement(true);
var path=Path.Combine(output,"board.iwcal");if(File.Exists(path))File.Delete(path);
var bp=Defaults(board);bp["CalibPathName"]=JsonSerializer.SerializeToElement(path);bp["RefreshFileEnable"]=JsonSerializer.SerializeToElement(true);
var rp=Defaults(rotate);rp["FilePath"]=JsonSerializer.SerializeToElement(path);
using var graph=new GraphEditorDocument(registry);
var source=graph.AddNode(image.Manifest.Id,image.Manifest.Version,0,0,ip);
var producer=graph.AddNode(board.Manifest.Id,board.Manifest.Version,200,0,bp);
var sample=graph.AddNode(SampleSource.Id,"1",400,0);
var consumer=graph.AddNode(rotate.Manifest.Id,rotate.Manifest.Version,600,0,rp);
graph.Connect(source,"Image",producer,"InputImage");graph.ConnectExecution(producer,sample);graph.ConnectExecution(sample,consumer);
var bindings=new[]{("RotNum","Count"),("ImagePointX","X"),("ImagePointY","Y"),("WorldRotateAngle","Angle"),("RefreshSignal","Refresh")}
 .Select(p=>new WorkflowSubscription(consumer,p.Item1,false,new(WorkflowSourceKind.NodeOutput,sample,p.Item2))).ToArray();
graph.SetWorkflowConfiguration(graph.CurrentProcedureId,new(){Subscriptions=bindings});
var pixels=Enumerable.Repeat((byte)255,480*400).ToArray();
for(var y=50;y<350;y++)for(var x=60;x<420;x++)pixels[y*480+x]=(byte)((((x-60)/30+(y-50)/30)&1)==0?0:255);
using var scope=new ModuleExecutionScope();
async Task<IReadOnlyDictionary<string,NodeRunRecord>> Run(byte[] frame){image.SubmitImage(scope.ForProcedure(graph.CurrentProcedureId),source,new GrayImage(480,400,frame));var result=await new SolutionExecutor(registry).RunAsync(new(graph.Document,graph.BaseDirectory),"main",scope:scope);var records=result.Nodes.ToDictionary(n=>n.NodeId);Console.WriteLine($"TRACE board={records[producer].Status}/0x{records[producer].NativeErrorCode:x8} rotate={records[consumer].Status}/0x{records[consumer].NativeErrorCode:x8} remain={(records[consumer].Outputs.TryGetValue("remainCount",out var r)?r.As<long>().ToString():"missing")}");return records;}
string? firstHash=null;
for(int i=0;i<3;i++){
 var records=await Run(pixels);Check(records[producer].Status==NodeRunStatus.Success&&records[producer].Outputs["CalibPointNum"].As<long>()==99,$"real board frame {i+1} produces 99 calibration points");
 Check(File.Exists(path)&&new FileInfo(path).Length==36,$"real board frame {i+1} writes 36-byte iwcal");
 var matrix=CalibrationBoardFile.Decode(File.ReadAllBytes(path));var stateMatrix=records[producer].Outputs["CalibData"].As<CalibrationData>().ImageToWorld;
 Check(matrix.Select(BitConverter.DoubleToInt64Bits).SequenceEqual(stateMatrix.Select(v=>BitConverter.DoubleToInt64Bits((double)(float)v))),$"file float32 matrix matches board published CalibData frame {i+1}");
 var rotateMatrix=new float[9];Check(RotateCalibrationFile.Read(path,rotateMatrix,out var loaded)==0&&loaded&&rotateMatrix.Select(BitConverter.SingleToInt32Bits).SequenceEqual(matrix.Select(v=>BitConverter.SingleToInt32Bits((float)v))),$"Rotate loader reads board nine float fields exactly frame {i+1}");
 Check(records[consumer].Status==NodeRunStatus.Success&&records[consumer].Outputs["remainCount"].As<long>()==2-i,$"Rotate consumes producer file and sequential sample {i+1}");firstHash??=Sha(path);
}
var saved=Path.Combine(output,"board-rotate.vmx.json");graph.Save(saved);using(var reopened=new GraphEditorDocument(registry)){reopened.Load(saved);Check(reopened.CurrentProcedure.Connections.Any(c=>c.FromNode==source&&c.ToNode==producer)&&reopened.CurrentProcedure.Configuration.Subscriptions.Count==5,"saved graph preserves IMAGE binding and Rotate sample subscriptions");}
File.Delete(path);var failed=await Run(Enumerable.Repeat((byte)127,480*400).ToArray());
Check(failed[producer].Status==NodeRunStatus.NoMatch&&!File.Exists(path),"blank image fails board recognition and does not recreate calibration file");
Check(failed[consumer].Status==NodeRunStatus.Failed&&failed[consumer].NativeErrorCode==0xe0000008,"Rotate forced refresh of missing board file fails with uninitialized matrix");
var recovered=await Run(pixels);Check(recovered[producer].Status==NodeRunStatus.Success&&File.Exists(path),"valid image recovers board file production");
Check(recovered[consumer].Status==NodeRunStatus.Success&&recovered[consumer].Outputs["remainCount"].As<long>()==2,"Rotate reloads recovered file and restarts first sample after failure");
File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{checks,passed=true,calibrationFile=path,calibrationHash=Sha(path),firstHash,manualAccepted=false}));

sealed class SampleSource:IModule {
 public const string Id="test.calibration.board-rotate.sample";static int next;
 public static ModuleDescriptor Definition{get;}=new(Id,"1","sample",[],[new("X",DataKind.NumberArray),new("Y",DataKind.NumberArray),new("Angle",DataKind.NumberArray),new("Count",DataKind.Integer),new("Refresh",DataKind.Integer)],[]);
 public ModuleDescriptor Descriptor=>Definition;
 public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token){int i=Interlocked.Increment(ref next)-1;double t=2*Math.PI*(i%3)/3;return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"X",PortValue.Numbers([50+10*Math.Cos(t)])},{"Y",PortValue.Numbers([50+10*Math.Sin(t)])},{"Angle",PortValue.Numbers([120])},{"Count",PortValue.Integer(3)},{"Refresh",PortValue.Integer(1)}}));}
}
