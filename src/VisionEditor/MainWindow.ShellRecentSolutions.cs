using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;

namespace VisionEditor;

internal static class RecentSolutionStore
{
    internal static string FilePath { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "VisionEditor", "recent-solutions.json");

    internal sealed record Entry(string Path, DateTime LastTime);

    internal static IReadOnlyList<Entry> Read()
    {
        try
        {
            if (!File.Exists(FilePath)) return [];
            return (JsonSerializer.Deserialize<Entry[]>(File.ReadAllText(FilePath)) ?? [])
                .Where(entry => DateTime.Now - entry.LastTime < TimeSpan.FromDays(100))
                .OrderByDescending(entry => entry.LastTime).ToArray();
        }
        catch (JsonException) { return []; }
        catch (IOException) { return []; }
    }

    internal static void Update(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;
        var full = System.IO.Path.GetFullPath(path);
        var entries = Read().Where(entry => !string.Equals(entry.Path, full, StringComparison.OrdinalIgnoreCase))
            .Prepend(new Entry(full, DateTime.Now)).Take(6).ToArray();
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(entries));
    }

    internal static void Remove(string path)
    {
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(Read()
            .Where(entry => !string.Equals(entry.Path, path, StringComparison.OrdinalIgnoreCase))));
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
