using System.IO;
using Microsoft.Win32;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace VisionEditor;

public interface ISampleSolutionPicker
{
    string? Pick(MainWindow owner, string initialDirectory);
}

sealed class NativeSampleSolutionPicker : ISampleSolutionPicker
{
    public string? Pick(MainWindow owner, string initialDirectory)
    {
        var dialog = new OpenFileDialog
        {
            Title = "打开示例方案",
            Filter = "示例方案|*.vmx.json;*.json",
            InitialDirectory = initialDirectory,
            CheckFileExists = true
        };
        return dialog.ShowDialog(owner) == true ? dialog.FileName : null;
    }
}

internal static class SampleDirectoryPreference
{
    internal static string FilePath { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "VisionEditor", "sample-directory.txt");

    internal static string InitialDirectory()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                var saved = File.ReadAllText(FilePath).Trim();
                if (Directory.Exists(saved)) return saved;
            }
        }
        catch (IOException) { }
        return AppContext.BaseDirectory;
    }

    internal static void Remember(string file)
    {
        var directory = Path.GetDirectoryName(Path.GetFullPath(file))!;
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, directory);
    }
}

public partial class MainWindow
{
    ISampleSolutionPicker sampleSolutionPicker = new NativeSampleSolutionPicker();

    public MainWindow(ISampleSolutionPicker picker) : this()
    {
        sampleSolutionPicker = picker ?? throw new ArgumentNullException(nameof(picker));
    }

    void OpenSampleSolution()
    {
        var path = sampleSolutionPicker.Pick(this, SampleDirectoryPreference.InitialDirectory());
        if (string.IsNullOrWhiteSpace(path)) return;
        SampleDirectoryPreference.Remember(path);
        if (!File.Exists(path)) { StatusText.Text = "示例方案文件不存在"; return; }
        // The native OpenSamples delegates to LoadSolution, which stops continuous run.
        continuousRun = false;
        if (activeRun is not null) { StatusText.Text = "请等待当前流程运行结束"; return; }
        if (!ConfirmPendingChanges()) return;
        doc.Load(path);
        RecentSolutionStore.Update(doc.FilePath!);
        RefreshRecentSolutionsMenu();
    }
}
