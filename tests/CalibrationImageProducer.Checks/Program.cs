using System.IO;
using System.Text.Json;
using VisionCore;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;
using VisionPlugins.CalibrationBoard;
using VisionPlugins.ImageSource;

var output = Path.GetFullPath("analysis/joint-audit/pass8-cal-image");
Directory.CreateDirectory(output);
var nImageXml = Path.Combine(output, "nimage-host", "host-result.xml");
void Check(bool condition, string message) { if (!condition) throw new Exception(message); Console.WriteLine("PASS " + message); }
Check(File.Exists(nImageXml), "fresh NImageCalibration.HostChecks XML exists");
var loaded = ReadCalibFileData.Read(nImageXml);
Check(loaded.ErrorCode == 0 && loaded.Data?.Values().TryGetValue("CalibMatrix", out var nImageMatrix) == true &&
      nImageMatrix!.Kind == DataKind.NumberArray && nImageMatrix.As<IReadOnlyList<double>>().Count == 9,
    "actual N-image XML presents a nine-float CalibMatrix to calibration loader");

var image = new ImageSourcePlugin();
var board = new CalibrationBoardPlugin();
var reader = new ReadCalibFilePlugin();
var coordinate = new CoordinateModulePlugin();
var registry = new ModuleRegistry();
var plugins = new ToolPluginRegistry();
foreach (var plugin in new IVisionToolPlugin[] { image, board, reader, coordinate }) plugins.Register(plugin, registry);
Dictionary<string, JsonElement> Defaults(IVisionToolPlugin plugin) =>
    plugin.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
var imageParams = Defaults(image);
imageParams["ImageSourceType"] = JsonSerializer.SerializeToElement(3);
imageParams["StitchEnable"] = JsonSerializer.SerializeToElement(false);
imageParams["OutMono8"] = JsonSerializer.SerializeToElement(true);
var boardParams = Defaults(board);
var boardPath = Path.Combine(output, "board-result.iwcal");
boardParams["CalibPathName"] = JsonSerializer.SerializeToElement(boardPath);
boardParams["RefreshFileEnable"] = JsonSerializer.SerializeToElement(true);
var readParams = Defaults(reader);
readParams["FilePath"] = JsonSerializer.SerializeToElement(nImageXml);
readParams["OutputSchema"] = JsonSerializer.SerializeToElement(JsonSerializer.Serialize(loaded.Data!.Ports()));
var coordinateParams = Defaults(coordinate);
coordinateParams["CordinateCreate"] = JsonSerializer.SerializeToElement(1);
coordinateParams["InputWay"] = JsonSerializer.SerializeToElement(2);

using var graph = new GraphEditorDocument(registry);
var sourceId = graph.AddNode(image.Manifest.Id, image.Manifest.Version, 0, 0, imageParams);
var boardId = graph.AddNode(board.Manifest.Id, board.Manifest.Version, 200, 0, boardParams);
var readerId = graph.AddNode(reader.Manifest.Id, reader.Manifest.Version, 400, 0, readParams);
var coordinateId = graph.AddNode(coordinate.Manifest.Id, coordinate.Manifest.Version, 600, 0, coordinateParams);
graph.Connect(sourceId, "Image", boardId, "InputImage");
graph.Connect(sourceId, "Image", coordinateId, "InputImage");
graph.Connect(readerId, "CalibMatrix", coordinateId, "CalibMatrix");
graph.ConnectExecution(boardId, readerId);
var pixels = Enumerable.Repeat((byte)255, 480 * 400).ToArray();
for (var y = 50; y < 350; y++) for (var x = 60; x < 420; x++)
    pixels[y * 480 + x] = (byte)((((x - 60) / 30 + (y - 50) / 30) & 1) == 0 ? 0 : 255);
using var scope = new ModuleExecutionScope();
image.SubmitImage(scope.ForProcedure(graph.CurrentProcedureId), sourceId, new GrayImage(480, 400, pixels));
var session = await new SolutionExecutor(registry).RunAsync(new(graph.Document, graph.BaseDirectory), "main", scope: scope);
var records = session.Nodes.ToDictionary(n => n.NodeId);
foreach (var id in new[] { sourceId, boardId, readerId, coordinateId })
    Console.WriteLine($"TRACE {id} {records[id].Status}/{records[id].ErrorCode}: {records[id].Message}");
Check(records[boardId].Status == NodeRunStatus.Success && records[boardId].Outputs["CalibPointNum"].As<long>() == 99,
    "real image-source Gray8 IMAGE binding drives checkerboard module to 99 points");
Check(records[boardId].Outputs["CalibPointX"].Kind == DataKind.NumberArray &&
      records[boardId].Outputs["CalibPointX"].As<IReadOnlyList<double>>().Count == 99 &&
      records[boardId].Outputs["CalibData"].Kind == DataKind.Calibration,
    "board publishes separate numeric point list and independent calibration object");
Check(File.Exists(boardPath) && new FileInfo(boardPath).Length == 36 && CalibrationBoardFile.Decode(File.ReadAllBytes(boardPath)).Length == 9,
    "board module writes 36-byte float32 iwcal file");
Check(ReadCalibFileData.Read(boardPath).ErrorCode == 0xe0000201,
    "XML calibration loader rejects board binary iwcal instead of inventing a conversion");
Check(records[readerId].Status == NodeRunStatus.Success &&
      records[readerId].Outputs["CalibMatrix"].As<IReadOnlyList<double>>().Count == 9,
    "real graph loader republishes N-image XML matrix as numeric array");
Check(records[coordinateId].Status == NodeRunStatus.Success &&
      records[coordinateId].Outputs["CorCalibMatrix"].As<IReadOnlyList<double>>().Count == 9,
    "N-image XML matrix reaches coordinate consumer through graph input");
var path = Path.Combine(output, "producer-transport.vmx.json");
graph.Save(path);
using var reopened = new GraphEditorDocument(registry);
reopened.Load(path);
Check(reopened.CurrentProcedure.Connections.Count == 3 &&
      reopened.CurrentProcedure.Nodes.Single(n => n.Id == readerId).Parameters["FilePath"].GetString() is { Length: > 0 },
    "three typed data links and N-image XML path survive solution reopen");
