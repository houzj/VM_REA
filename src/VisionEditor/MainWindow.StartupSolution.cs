using System.Diagnostics;
using System.IO;
using System.Text.Json;
using VisionFramework;
using VisionPlugins.ImageBinary;
using VisionPlugins.ImageSource;

namespace VisionEditor;

internal static class StartupSolutionStore
{
    internal static string RootDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VisionEditor", "Solutions");
    internal static string DefaultPath => Path.Combine(RootDirectory, "default.vmx.json");
    internal static string? CommandLinePath { get; set; }
    internal static string? UnsupportedOriginalSolutionPath { get; set; }
    internal static bool StartNewSolution { get; set; }
}

public partial class MainWindow
{
    void LoadStartupSolution()
    {
        if (StartupSolutionStore.StartNewSolution)
        {
            StartupSolutionStore.StartNewSolution = false;
            doc.NewSolution(true);
            StatusText.Text = "已创建新方案，请添加流程或工具";
            return;
        }
        var commandLinePath = StartupSolutionStore.CommandLinePath;
        if (!string.IsNullOrWhiteSpace(commandLinePath) && File.Exists(commandLinePath) && TryLoadStartup(commandLinePath))
        {
            ReportStartupSolution(commandLinePath);
            return;
        }
        var recent = RecentSolutionStore.Read().FirstOrDefault(entry =>
            File.Exists(entry.Path) &&
            !string.Equals(Path.GetFileName(entry.Path), "demo.vmx.json", StringComparison.OrdinalIgnoreCase));
        if (recent is not null)
        {
            var isDefault = string.Equals(Path.GetFullPath(recent.Path), Path.GetFullPath(StartupSolutionStore.DefaultPath), StringComparison.OrdinalIgnoreCase);
            if (isDefault)
                PrepareDefaultStartupSolution(recent.Path);
            if (TryLoadStartup(recent.Path))
            {
                if (isDefault) EnsureDefaultExecutionGraph(recent.Path);
                ReportStartupSolution(recent.Path);
                return;
            }
        }
        var path = StartupSolutionStore.DefaultPath;
        try
        {
            Directory.CreateDirectory(StartupSolutionStore.RootDirectory);
            PrepareDefaultStartupSolution(path);
            if (TryLoadStartup(path))
            {
                EnsureDefaultExecutionGraph(path);
                ReportStartupSolution(path);
                return;
            }
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or InvalidDataException)
        {
            Trace.TraceWarning("无法创建持久化启动方案：" + error.Message);
        }
        doc.NewSolution(true);
        StatusText.Text = StartupSolutionStore.UnsupportedOriginalSolutionPath is { Length: > 0 } original
            ? $"原厂方案格式暂未接入：{original}"
            : "已创建内存启动方案，请保存方案";
    }

    void PrepareDefaultStartupSolution(string path)
    {
        try
        {
            Directory.CreateDirectory(StartupSolutionStore.RootDirectory);
            CopyStartupAssets(Path.Combine(AppContext.BaseDirectory, "assets"), Path.Combine(StartupSolutionStore.RootDirectory, "assets"));
            if (!File.Exists(path) || IsLegacyDemoSolution(path))
                SolutionStore.Save(path, BlankStartupSolution());
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or InvalidDataException)
        {
            Trace.TraceWarning("默认方案准备未完成：" + error.Message);
        }
    }

    void ReportStartupSolution(string path)
    {
        var procedure = doc.CurrentProcedure;
        var executionLinks = ProcedureGraph.ExecutionEdges(procedure).Count();
        StatusText.Text = $"已恢复方案：{doc.Document.Name} · {procedure.Name} · {procedure.Nodes.Count} 个工具 · {executionLinks} 条执行连接";
    }

    static SolutionDocument BlankStartupSolution() => new(
        SolutionStore.CurrentVersion,
        Guid.NewGuid().ToString("N"),
        "新建方案",
        [new ProcedureDocument("main", "Main", [], [], [])],
        []);

    static bool IsLegacyDemoSolution(string path)
    {
        try
        {
            var solution = SolutionStore.Load(path);
            return solution.Name == "图像源插件验证" && solution.Procedures.Count == 1 &&
                solution.Procedures[0].Nodes.Count == 1 &&
                solution.Procedures[0].Nodes[0].ModuleId == "vision.image-source";
        }
        catch (Exception error) when (error is IOException or InvalidDataException or JsonException)
        {
            return false;
        }
    }

    bool TryLoadStartup(string path)
    {
        try
        {
            doc.Load(path);
            RecentSolutionStore.Update(path);
            return true;
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or InvalidDataException or KeyNotFoundException)
        {
            Trace.TraceWarning("启动方案加载失败：" + error.Message);
            return false;
        }
    }

    void EnsureDefaultExecutionGraph(string path)
    {
        var procedure = doc.CurrentProcedure;
        if (procedure.Nodes.Count != 1) return;
        var source = procedure.Nodes.SingleOrDefault(node => node.ModuleId == "vision.image-source");
        if (source is null || procedure.Nodes.Any(node => node.ModuleId == ImageBinaryPlugin.Id)) return;
        var binary = doc.AddNode(ImageBinaryPlugin.Id, "1.0.0", source.X + 360, source.Y);
        doc.BindInput(source.Id, "Image", binary, "Image", addExecutionLink: true);
        doc.Save(path);
        RecentSolutionStore.Update(path);
    }

    static void CopyStartupAssets(string source, string destination)
    {
        if (!Directory.Exists(source)) return;
        Directory.CreateDirectory(destination);
        foreach (var file in Directory.EnumerateFiles(source, "*", SearchOption.TopDirectoryOnly))
            File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), overwrite: true);
    }
}
