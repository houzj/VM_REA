using System.IO;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace VisionEditor;

public interface ISolutionFilePicker
{
    string? Pick(MainWindow owner, string initialDirectory);
}

sealed class NativeSolutionFilePicker : ISolutionFilePicker
{
    public string? Pick(MainWindow owner, string initialDirectory)
    {
        var dialog = new OpenFileDialog
        {
            Title = "打开方案",
            Filter = "流程方案|*.vmx.json;*.json",
            InitialDirectory = initialDirectory,
            CheckFileExists = true
        };
        return dialog.ShowDialog(owner) == true ? dialog.FileName : null;
    }
}

internal static class OpenSolutionDirectoryPreference
{
    internal static string FilePath { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "VisionEditor", "open-directory.txt");

    internal static string InitialDirectory()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                var path = File.ReadAllText(FilePath).Trim();
                if (Directory.Exists(path)) return path;
            }
        }
        catch (IOException) { }
        return AppContext.BaseDirectory;
    }

    internal static void Remember(string file)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, Path.GetDirectoryName(Path.GetFullPath(file))!);
    }
}

public partial class MainWindow
{
    ISolutionFilePicker solutionFilePicker = new NativeSolutionFilePicker();

    public MainWindow(ISampleSolutionPicker samples, ISolutionFilePicker solutions) : this(samples)
    {
        solutionFilePicker = solutions ?? throw new ArgumentNullException(nameof(solutions));
    }

    void OpenSolutionFromFileMenu()
    {
        // The native StopRun command disables continuous run before the file picker.
        continuousRun = false;
        var path = solutionFilePicker.Pick(this, OpenSolutionDirectoryPreference.InitialDirectory());
        if (string.IsNullOrWhiteSpace(path)) return;
        OpenSolutionDirectoryPreference.Remember(path);
        // Our independent executor must finish its current round before replacing its document.
        if (activeRun is not null) { StatusText.Text = "请等待当前流程运行结束"; return; }
        if (!ConfirmPendingChanges()) return;
        doc.Load(path);
        RecentSolutionStore.Update(doc.FilePath!);
        RefreshRecentSolutionsMenu();
    }
}
