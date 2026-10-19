using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;

namespace VisionEditor;

internal static class RecentSolutionStore
{
    static string filePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "VisionEditor", "recent-solutions.json");
    static Entry[]? entries;

    internal static string FilePath
    {
        get => filePath;
        set
        {
            if (string.Equals(filePath, value, StringComparison.Ordinal)) return;
            filePath = value;
            entries = null;
        }
    }

    internal sealed record Entry(string Path, DateTime LastTime);

    internal static IReadOnlyList<Entry> Read()
    {
        if (entries is null)
        {
            try
            {
                entries = File.Exists(FilePath)
                    ? JsonSerializer.Deserialize<Entry[]>(File.ReadAllText(FilePath)) ?? [] : [];
            }
            catch (JsonException) { return []; }
            catch (IOException) { return []; }
            catch (UnauthorizedAccessException) { return []; }
        }
        // SolutionItemInfo.GeTimeSpan uses Duration, including future timestamps.
        var now = DateTime.Now;
        entries = entries.Where(entry => (now - entry.LastTime).Duration().Days <= 99)
            .OrderByDescending(entry => entry.LastTime).ToArray();
        return entries;
    }

    internal static void Update(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;
        var full = System.IO.Path.GetFullPath(path);
        entries = Read().Where(entry => !string.Equals(entry.Path, full, StringComparison.OrdinalIgnoreCase))
            .Prepend(new Entry(full, DateTime.Now)).Take(6).ToArray();
        Persist();
    }

    internal static void Remove(string path)
    {
        entries = Read().Where(entry => !string.Equals(entry.Path, path, StringComparison.OrdinalIgnoreCase)).ToArray();
        Persist();
    }

    static void Persist()
    {
        // ProjectManager publishes its in-memory recent list even if SaveRecentlySolutions fails.
        try
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(FilePath)!);
            File.WriteAllText(FilePath, JsonSerializer.Serialize(entries));
        }
        catch (IOException ex) { System.Diagnostics.Trace.TraceWarning("Save recent solutions: " + ex.Message); }
        catch (UnauthorizedAccessException ex) { System.Diagnostics.Trace.TraceWarning("Save recent solutions: " + ex.Message); }
    }
}

public partial class MainWindow
{
    MenuItem? recentSolutionsMenu;

    void InitializeRecentSolutionsMenu(MenuItem fileMenu)
    {
        recentSolutionsMenu = new MenuItem { Header = "最近打开" };
        recentSolutionsMenu.SubmenuOpened += (_, _) => RefreshRecentSolutionsMenu();
        fileMenu.Items.Add(recentSolutionsMenu);
        RefreshRecentSolutionsMenu();
    }

    void RefreshRecentSolutionsMenu()
    {
        if (recentSolutionsMenu is null) return;
        recentSolutionsMenu.Items.Clear();
        foreach (var entry in RecentSolutionStore.Read())
        {
            var item = new MenuItem { Header = System.IO.Path.GetFileNameWithoutExtension(entry.Path), ToolTip = entry.Path, Tag = entry.Path,
                Style = (System.Windows.Style)FindResource("ShellFileMenuItem") };
            item.Click += (_, _) => Safe(() => OpenRecentSolution(entry.Path));
            recentSolutionsMenu.Items.Add(item);
        }
        recentSolutionsMenu.IsEnabled = recentSolutionsMenu.Items.Count > 0;
    }

    void OpenRecentSolution(string path)
    {
        continuousRun = false;
        if (activeRun is not null) { StatusText.Text = "请等待当前流程运行结束"; return; }
        if (!ConfirmPendingChanges()) return;
        if (!File.Exists(path))
        {
            if (MessageBox.Show(this, "方案文件不存在。是否从最近打开中移除？", "打开方案",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                RecentSolutionStore.Remove(path);
            RefreshRecentSolutionsMenu();
            return;
        }
        doc.Load(path);
        RecentSolutionStore.Update(path);
        RefreshRecentSolutionsMenu();
    }
}
