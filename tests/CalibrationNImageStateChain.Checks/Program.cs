using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionCore;
using VisionEditor.Core;
using VisionFramework;
using VisionModules;
using VisionPluginContracts;
using VisionPlugins.Calibration;
using VisionPlugins.ImageSource;

static class Program {
 static int checks;static void Check(bool yes,string name){if(!yes)throw new Exception(name);Console.WriteLine($"PASS {++checks}: {name}");}
 static JsonElement J<T>(T value)=>JsonSerializer.SerializeToElement(value);
 static string Sha(string path)=>Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));
 [STAThread]static async Task<int> Main(string[] args){try{
  if(args.Length!=1)throw new ArgumentException("Own output directory required");var output=Path.GetFullPath(args[0]);Directory.CreateDirectory(output);
  var image=new ImageSourcePlugin();var nimage=new NImagePlugin();var reader=new ReadCalibFilePlugin();var coordinate=new CoordinateModulePlugin();var registry=new ModuleRegistry();BuiltinModules.RegisterAll(registry);var catalog=new ToolPluginRegistry();foreach(var plugin in new IVisionToolPlugin[]{image,nimage,reader,coordinate})catalog.Register(plugin,registry);
  Dictionary<string,JsonElement> Defaults(IVisionToolPlugin p)=>p.Descriptor.Parameters.ToDictionary(x=>x.Name,x=>x.DefaultValue!.Value.Clone());
  var ip=Defaults(image);ip["ImageSourceType"]=J(3);ip["StitchEnable"]=J(false);ip["OutMono8"]=J(true);
  var xml=Path.Combine(output,"recovered.xml");if(File.Exists(xml))File.Delete(xml);
  var np=Defaults(nimage);np["CalibPointTotalNum"]=J(9);np["RotPointTotalNum"]=J(0);np["CalibPathName"]=J(xml);np["RefreshFileEnable"]=J(true);
  using var graph=new GraphEditorDocument(registry);var source=graph.AddNode(image.Manifest.Id,image.Manifest.Version,0,0,ip);var target=graph.AddNode(nimage.Manifest.Id,nimage.Manifest.Version,200,0,np);graph.Connect(source,"Image",target,"InputImage");
  var fixture=Path.GetFullPath("tests/NImageCalibration.PluginChecks/Fixtures/hik-i-help-derived.png");Check(File.Exists(fixture),"source-derived coded-board fixture available");var decoder=BitmapDecoder.Create(new Uri(fixture),BitmapCreateOptions.PreservePixelFormat,BitmapCacheOption.OnLoad);var bmp=new FormatConvertedBitmap(decoder.Frames[0],PixelFormats.Gray8,null,0);int width=bmp.PixelWidth,height=bmp.PixelHeight;var bytes=new byte[width*height];bmp.CopyPixels(bytes,width,0);
  GrayImage Frame(int i){int dx=i%3*10,dy=i/3*10;var shifted=Enumerable.Repeat((byte)255,bytes.Length).ToArray();for(int y=0;y<height-dy;y++)bytes.AsSpan(y*width,width-dx).CopyTo(shifted.AsSpan((y+dy)*width+dx,width-dx));return new(width,height,shifted);}
  using var scope=new ModuleExecutionScope();
  async Task<NodeRunRecord> Run(int i,bool blank){var subscriptions=new[]{("MovePhysicalPointX",(double)(i%3)),("MovePhysicalPointY",(double)(i/3)),("RotateAngle",0d)}.Select(p=>new WorkflowSubscription(target,p.Item1,false,new(WorkflowSourceKind.NumberArrayConstant,JsonSerializer.Serialize(new[]{p.Item2})))).ToArray();graph.SetWorkflowConfiguration(graph.CurrentProcedureId,new(){Subscriptions=subscriptions});image.SubmitImage(scope.ForProcedure(graph.CurrentProcedureId),source,blank?new GrayImage(width,height,Enumerable.Repeat((byte)127,bytes.Length).ToArray()):Frame(i));var result=await new SolutionExecutor(registry).RunAsync(new(graph.Document,graph.BaseDirectory),"main",scope:scope);var node=result.Nodes.Single(n=>n.NodeId==target);Console.WriteLine($"TRACE i={i} blank={blank} status={node.Status} code=0x{node.NativeErrorCode:x8} publicCur={(node.Outputs.TryGetValue("CurNum",out var count)?count.As<long>().ToString():"absent")} ownedCur={nimage.Inspect(new(graph.CurrentProcedure.Nodes.Single(n=>n.Id==target),graph.BaseDirectory,_=>{},_=>throw new NotSupportedException()){Scope=scope.ForProcedure(graph.CurrentProcedureId)}).Current}");return node;}
  for(int i=0;i<8;i++){var item=await Run(i,false);Check(item.Status==NodeRunStatus.Success&&item.Outputs["CurNum"].As<long>()==i+1,$"prefix frame {i+1} retained in owned sequence");}
  var failed=await Run(8,true);Check(failed.Status==NodeRunStatus.Failed&&failed.NativeErrorCode==0x10103000,"ninth blank frame fails recognition only at full-batch boundary");
  var editor=new ToolEditorContext(graph.CurrentProcedure.Nodes.Single(n=>n.Id==target),graph.BaseDirectory,_=>{},_=>throw new NotSupportedException()){Scope=scope.ForProcedure(graph.CurrentProcedureId)};
  Check(nimage.Inspect(editor).Current==9&&!File.Exists(xml),"failed full batch retains nine owned frames but publishes no XML");
  try{nimage.Export(editor,Path.Combine(output,"should-not-exist.xml"));throw new Exception("failed batch unexpectedly exported");}catch(NImageExportException e){Check(e.NativeErrorCode==0x80100011,"explicit export after recognition failure reports absent SDK result");}
  var first=await Run(0,false);Check(first.Status==NodeRunStatus.Success&&first.Outputs["CurNum"].As<long>()==1,"next frame starts a fresh batch after failed full sequence");
  for(int i=1;i<9;i++){var item=await Run(i,false);Check(item.Status==NodeRunStatus.Success&&item.Outputs["CurNum"].As<long>()==i+1,$"recovery frame {i+1} accepted");}
  var data=ReadCalibFileData.Read(xml);Check(File.Exists(xml)&&data.ErrorCode==0&&data.Data?.Values().TryGetValue("CalibMatrix",out var matrix)==true&&matrix.As<IReadOnlyList<double>>().Count==9,"recovered NImage batch writes readable nine-float XML");var xmlHash=Sha(xml);
  var cfg=Defaults(reader);cfg["FilePath"]=J(xml);cfg["OutputSchema"]=J(JsonSerializer.Serialize(data.Data!.Ports()));var cp=Defaults(coordinate);cp["CordinateCreate"]=J(1);cp["InputWay"]=J(2);
  using var chain=new GraphEditorDocument(registry);var gsource=chain.AddNode(image.Manifest.Id,image.Manifest.Version,0,0,ip);var group=chain.AddNode(WorkflowCalls.ModuleId,WorkflowCalls.Version,200,0);var body=chain.EnterOwnedGroup(group);var load=chain.AddNode(reader.Manifest.Id,reader.Manifest.Version,0,0,cfg);chain.SetWorkflowConfiguration(body,new(){Outputs=[new("matrix","CalibMatrix",DataKind.NumberArray,load,"CalibMatrix")]});chain.LeaveGroup();var consumer=chain.AddNode(coordinate.Manifest.Id,coordinate.Manifest.Version,400,0,cp);chain.ConnectExecution(gsource,group);chain.Connect(group,"CalibMatrix",consumer,"CalibMatrix");chain.Connect(gsource,"Image",consumer,"InputImage");
  using var chainScope=new ModuleExecutionScope();image.SubmitImage(chainScope.ForProcedure(chain.CurrentProcedureId),gsource,Frame(0));var executed=await new SolutionExecutor(registry).RunAsync(new(chain.Document,chain.BaseDirectory),"main",scope:chainScope);var hits=executed.Nodes.ToDictionary(n=>n.NodeId);Check(hits[group].Status==NodeRunStatus.Success&&hits[group].Outputs["CalibMatrix"].As<IReadOnlyList<double>>().Count==9,"owned Group emits XML loader matrix to parent");Check(hits[consumer].Status==NodeRunStatus.Success&&hits[consumer].Outputs["CorCalibMatrix"].As<IReadOnlyList<double>>().Count==9,"Coordinate consumes Group matrix and live image");
  var saved=Path.Combine(output,"nimage-group.vmx.json");chain.Save(saved);using var reopen=new GraphEditorDocument(registry);reopen.Load(saved);Check(reopen.Document.Procedures.Any(p=>p.GroupOwner is not null)&&reopen.CurrentProcedure.Connections.Any(c=>c.FromNode==group&&c.ToNode==consumer),"saved solution retains owned Group body and matrix wire");using var reopenedScope=new ModuleExecutionScope();image.SubmitImage(reopenedScope.ForProcedure(reopen.CurrentProcedureId),gsource,Frame(0));var rerun=await new SolutionExecutor(registry).RunAsync(new(reopen.Document,reopen.BaseDirectory),"main",scope:reopenedScope);Check(rerun.Nodes.Single(n=>n.NodeId==consumer).Status==NodeRunStatus.Success,"reopened owned Group reruns XML to Coordinate chain");
  File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{checks,passed=true,xmlHash,fixtureHash=Sha(fixture),manualAccepted=false}));return 0;
 }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
