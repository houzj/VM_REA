using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;

internal static class Program
{
    static readonly string[] Keys = ["ImagePointX", "ImagePointY", "WorldPointX", "WorldPointY", "DeltaAngle"];
    static void Check(bool ok, string reason) { if (!ok) throw new Exception(reason); Console.WriteLine("PASS " + reason); }
    [STAThread]
    static int Main()
    {
        try { MainAsync().GetAwaiter().GetResult(); return 0; }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
    static async Task MainAsync()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        _ = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        string dir = Path.GetFullPath("analysis/joint-audit/pass10-cal-npoint-edit");
        Directory.CreateDirectory(dir);
        string file = Path.Combine(dir, "npoint-edited.xml");
        string solution = Path.Combine(dir, "npoint-edited.vmx.json");
        File.Delete(file); File.Delete(solution);
        var registry = new ModuleRegistry();
        var tools = new ToolPluginRegistry();
        var point = new NPointPlugin();
        var loader = new ReadCalibFilePlugin();
        tools.Register(point, registry); tools.Register(loader, registry);
        var values = point.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
        values["CalibPointGet"] = JsonSerializer.SerializeToElement(1);
        values["CalibPointTotalNum"] = JsonSerializer.SerializeToElement(4);
        values["RotPointTotalNum"] = JsonSerializer.SerializeToElement(0);
        values["WeightFun"] = JsonSerializer.SerializeToElement(1);
        values["RefreshFileEnable"] = JsonSerializer.SerializeToElement(true);
        values["CalibPathName"] = JsonSerializer.SerializeToElement(file);
        var pairs = new (double ix, double iy, double wx, double wy)[] { (0,0,0,0), (10,0,20,0), (0,10,0,20), (10,10,20,20) };
        for (int row = 0; row < 4; row++)
        {
            values["ImagePointX" + row] = JsonSerializer.SerializeToElement(pairs[row].ix);
            values["ImagePointY" + row] = JsonSerializer.SerializeToElement(pairs[row].iy);
            values["WorldPointX" + row] = JsonSerializer.SerializeToElement(pairs[row].wx);
            values["WorldPointY" + row] = JsonSerializer.SerializeToElement(pairs[row].wy);
        }
        var readValues = loader.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
        readValues["FilePath"] = JsonSerializer.SerializeToElement(file);
        readValues["OutputSchema"] = JsonSerializer.SerializeToElement(JsonSerializer.Serialize(new[] { new ReadCalibFilePort("CalibMatrix", DataKind.NumberArray) }));
        using var graph = new GraphEditorDocument(registry);
        string pointId = graph.AddNode(NPointPlugin.Id, "1.0.0", 0, 0, values);
        string readerId = graph.AddNode(ReadCalibFilePlugin.Id, "1.0.0", 180, 0, readValues);
        graph.Connect(pointId, "CalibStatus", readerId, "RefreshSignal");
        int applies = 0, changes = 0;
        graph.Changed += () => changes++;
        NodeDocument CurrentNode() => graph.CurrentProcedure.Nodes.Single(n => n.Id == pointId);
        ToolEditorContext Context() => new(CurrentNode(), graph.BaseDirectory, Apply, _ => throw new NotSupportedException());
        void Apply(Dictionary<string, JsonElement> next)
        {
            applies++;
            graph.SetProcedureNodeParameters(graph.CurrentProcedureId, pointId, next);
            point.UpdateConfiguration(Context(), next);
        }
        point.UpdateConfiguration(Context(), CurrentNode().Parameters);
        long revision = graph.Revision;
        // Same editor and same callback algorithm as NPointConfiguration.CreateConfiguration,
        // but without showing an off-desktop modal window or claiming an interactive click.
        var editor = new NPointConfigurationPointEditor(point.Snapshot(Context()), 4, (row, axis, value) =>
        {
            if (!float.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out var number) || !float.IsFinite(number)) return false;
            var next = CurrentNode().Parameters.ToDictionary(p => p.Key, p => p.Value.Clone());
            var snapshot = point.Snapshot(Context());
            for (int i = 0; i < 32; i++) for (int a = 0; a < 5; a++)
                next[Keys[a] + i] = JsonSerializer.SerializeToElement((double)snapshot[i,a]);
            next[Keys[axis] + row] = JsonSerializer.SerializeToElement((double)number);
            Apply(next);
            return true;
        }, _ => false, () => false, () => null, () => null);
        Console.WriteLine($"opening 4 rows: apply={applies}, changed={changes}, revision+={graph.Revision-revision}");
        Check(applies == 20 && changes == 20 && graph.Revision - revision == 20, "opening point editor commits five fields per row, each as a whole-node graph edit");
        int before = applies;
        Check(editor.TrySetCell(0, 2, "2.500"), "one cell edit accepted by real point editor setter");
        Console.WriteLine($"one cell: apply+={applies-before}, world-x0={CurrentNode().Parameters["WorldPointX0"].GetDouble():G9}");
        Check(applies == before + 1 && CurrentNode().Parameters["WorldPointX0"].GetDouble() == 2.5, "one cell writes one graph revision");
        graph.Undo();
        Check(CurrentNode().Parameters["WorldPointX0"].GetDouble() == 0, "Undo restores pre-edit cell value");
        graph.Redo();
        Check(CurrentNode().Parameters["WorldPointX0"].GetDouble() == 2.5, "Redo restores edited cell value");
        graph.Save(solution);
        using var reopened = new GraphEditorDocument(registry);
        reopened.Load(solution);
        Check(reopened.CurrentProcedure.Nodes.Single(n => n.Id == pointId).Parameters["WorldPointX0"].GetDouble() == 2.5, "saved and reopened point parameter persists");
        using var scope = new ModuleExecutionScope();
        var result = await new SolutionExecutor(registry).RunAsync(new(reopened.Document, reopened.BaseDirectory), "main", scope: scope);
        var runs = result.Nodes.ToDictionary(n => n.NodeId);
        Console.WriteLine($"run: NPoint={runs[pointId].Status}/0x{runs[pointId].NativeErrorCode:x8}; Read={runs[readerId].Status}/0x{runs[readerId].NativeErrorCode:x8}; xml={File.Exists(file)}");
        Check(runs[pointId].Status == NodeRunStatus.Success && runs[pointId].Outputs["CalibStatus"].As<long>() == 1, "edited points reach NPoint solver");
        Check(File.Exists(file) && runs[readerId].Status == NodeRunStatus.Success && runs[readerId].Outputs["CalibMatrix"].As<IReadOnlyList<double>>().Count == 9, "edited run writes XML and downstream loader parses matrix");
        Console.WriteLine($"XML-sha256={Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(file)))}");
    }
}
