using System.IO;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace VisionEditor;

public interface ISolutionSavePicker
{
    string? Pick(MainWindow owner, string initialDirectory);
}

sealed class NativeSolutionSavePicker : ISolutionSavePicker
{
    public string? Pick(MainWindow owner, string initialDirectory)
    {
        var dialog = new SaveFileDialog
        {
            Title = "另存为方案",
            Filter = "流程方案|*.vmx.json",
            FileName = "solution.vmx.json",
            InitialDirectory = initialDirectory,
            AddExtension = true,
            DefaultExt = ".vmx.json"
        };
        return dialog.ShowDialog(owner) == true ? dialog.FileName : null;
    }
}

internal static class SaveSolutionDirectoryPreference
{
    internal static string FilePath { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "VisionEditor", "save-directory.txt");

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
        return string.Empty;
    }

    internal static void Remember(string file)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, Path.GetDirectoryName(Path.GetFullPath(file))!);
    }
}

public partial class MainWindow
{
    ISolutionSavePicker solutionSavePicker = new NativeSolutionSavePicker();

    public MainWindow(ISampleSolutionPicker samples, ISolutionFilePicker solutions, ISolutionSavePicker saves)
        : this(samples, solutions)
    {
        solutionSavePicker = saves ?? throw new ArgumentNullException(nameof(saves));
    }

    bool Save(bool asNew)
    {
        var file = doc.FilePath;
        var selectedPath = asNew || file is null;
        if (selectedPath)
        {
            file = solutionSavePicker.Pick(this, SaveSolutionDirectoryPreference.InitialDirectory());
            if (string.IsNullOrWhiteSpace(file)) return false;
        }
        if (file is null) return false;
        doc.Save(file);
        if (selectedPath)
            SaveSolutionDirectoryPreference.Remember(file);
        RecentSolutionStore.Update(doc.FilePath!);
        RefreshRecentSolutionsMenu();
        return true;
    }
}
