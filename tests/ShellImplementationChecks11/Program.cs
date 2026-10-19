using System.Collections;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using VisionEditor;
using VisionEditor.Core;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        var root = Path.GetFullPath(args.FirstOrDefault() ?? "analysis/full-audit/batch11-20261019/A/checks");
        root = Path.Combine(root, "run-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        var results = new List<string>();
        void Check(bool pass, string name) { if (!pass) throw new Exception(name); results.Add(name); Console.WriteLine("PASS " + name); }
        const BindingFlags stat = BindingFlags.Static | BindingFlags.NonPublic;
        const BindingFlags inst = BindingFlags.Instance | BindingFlags.NonPublic;
        var assembly = typeof(MainWindow).Assembly;
        var store = assembly.GetType("VisionEditor.RecentSolutionStore")!;
        void SetStore(string path) => store.GetProperty("FilePath", stat)!.SetValue(null, path);
        void Update(string path) => store.GetMethod("Update", stat)!.Invoke(null, [path]);
        void Remove(string path) => store.GetMethod("Remove", stat)!.Invoke(null, [path]);
        string[] Paths() => ((IEnumerable)store.GetMethod("Read", stat)!.Invoke(null, null)!).Cast<object>()
            .Select(e => (string)e.GetType().GetProperty("Path")!.GetValue(e)!).ToArray();
        string Solution(string name) => Path.Combine(root, name + ".vmx.json");

        var blocked = Path.Combine(root, "blocked-parent");
        File.WriteAllText(blocked, "fixture file prevents parent directory creation");
        var blockedStore = Path.Combine(blocked, "recent.json");
        SetStore(blockedStore);
        Update(Solution("one"));
        Check(Paths().SequenceEqual([Solution("one")]), "Write failure retains newly published recent item");
        Update(Solution("two"));
        Check(Paths().SequenceEqual([Solution("two"), Solution("one")]), "Repeated failure retains ordered memory snapshot");
        Remove(Solution("two"));
        Check(Paths().SequenceEqual([Solution("one")]), "Remove remains effective when persistence fails");
        Update("");
        Check(Paths().Length == 1, "Empty path is a no-op during persistence failure");
        File.Delete(blocked);
        Directory.CreateDirectory(blocked);
        Update(Solution("three"));
        Check(File.Exists(blockedStore) && File.ReadAllText(blockedStore).Contains("three"), "Next update persists retained memory after storage recovery");
        var other = Path.Combine(root, "other.json");
        SetStore(other);
        Check(Paths().Length == 0, "Changing storage path cannot leak prior recent entries");
        for (int i = 0; i < 8; i++) Update(Solution("item-" + i));
        Check(Paths().Length == 6 && Paths()[0] == Solution("item-7") && !Paths().Contains(Solution("item-1")), "New items remain limited to the most recent six");
        Update(Solution("item-4"));
        Check(Paths().Length == 6 && Paths()[0] == Solution("item-4"), "Updating an existing entry reorders without duplication");

        var dated = Path.Combine(root, "dated.json");
        File.WriteAllText(dated, JsonSerializer.Serialize(new[] {
            new { Path = Solution("past-retained"), LastTime = DateTime.Now.AddDays(-99).AddHours(-20) },
            new { Path = Solution("past-expired"), LastTime = DateTime.Now.AddDays(-101) },
            new { Path = Solution("future-expired"), LastTime = DateTime.Now.AddDays(101) }
        }));
        SetStore(dated);
        Check(Paths().SequenceEqual([Solution("past-retained")]), "Source Duration and integer Days expire old and distant future entries");
        var corrupt = Path.Combine(root, "corrupt.json");
        File.WriteAllText(corrupt, "{"); SetStore(corrupt);
        Check(Paths().Length == 0, "Malformed first read is safely empty");
        File.WriteAllText(corrupt, "[]"); Update(Solution("recovered"));
        Check(Paths().SequenceEqual([Solution("recovered")]), "Failed initial read does not prevent later recovery");

        // Exercise the actual shell Save path with isolated preferences and no original VM runtime.
        foreach (var name in new[] { "ShellLockPreference", "ShellRunningModePreference", "SaveSolutionDirectoryPreference", "OpenSolutionDirectoryPreference", "SampleDirectoryPreference" })
            assembly.GetType("VisionEditor." + name)!.GetProperty("FilePath", stat)!.SetValue(null, Path.Combine(root, name + ".json"));
        var blockedShell = Path.Combine(root, "blocked-shell"); File.WriteAllText(blockedShell, "fixture");
        SetStore(Path.Combine(blockedShell, "recent.json"));
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        var picker = new Picker { Path = Solution("saved") };
        var window = new MainWindow(picker, picker, picker);
        try
        {
            var doc = (GraphEditorDocument)typeof(MainWindow).GetField("doc", inst)!.GetValue(window)!;
            doc.RenameProcedure(doc.CurrentProcedureId, "A11-save-persistence-failure");
            var saved = (bool)typeof(MainWindow).GetMethod("Save", inst)!.Invoke(window, [true])!;
            Check(saved && File.Exists(picker.Path), "Actual shell Save reports success despite recent preference write failure");
            Check(doc.FilePath == picker.Path && !doc.IsDirty, "Actual successful save commits document path and clean state");
            Check(Paths().SequenceEqual([picker.Path!]), "Actual Save refresh publishes its recent item in memory");
            var recentMenu = (System.Windows.Controls.MenuItem)typeof(MainWindow).GetField("recentSolutionsMenu", inst)!.GetValue(window)!;
            Check(recentMenu.Items.Count == 1 && recentMenu.IsEnabled, "Actual Save refreshes recent menu after preference failure");
            var before = File.ReadAllText(picker.Path!);
            picker.Path = null;
            Check(!(bool)typeof(MainWindow).GetMethod("Save", inst)!.Invoke(window, [true])! && File.ReadAllText(doc.FilePath!) == before,
                "Cancelled Save As keeps successfully saved document and bytes");
        }
        finally { window.Close(); app.Shutdown(); }
        File.WriteAllText(Path.Combine(root, "summary.json"), JsonSerializer.Serialize(new { passed = results.Count, results }, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine(root);
    }
}

sealed class Picker : ISampleSolutionPicker, ISolutionFilePicker, ISolutionSavePicker
{
    public string? Path { get; set; }
    public string? Pick(MainWindow owner, string initialDirectory) => Path;
}
