using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;

static class Program
{
    const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    static T Field<T>(object owner, string name) => (T)owner.GetType().GetField(name, Private)!.GetValue(owner)!;
    static object? Call(object owner, string name, params object[] args) => owner.GetType().GetMethod(name, Private)!.Invoke(owner, args);
    static T? Find<T>(DependencyObject root, Func<T, bool> predicate) where T : DependencyObject
    {
        if (root is T item && predicate(item)) return item;
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            if (Find(VisualTreeHelper.GetChild(root, i), predicate) is { } child) return child;
        return null;
    }
    static void Check(bool condition, string label)
    {
        if (!condition) throw new InvalidOperationException(label);
        Console.WriteLine("PASS " + label);
    }
    [STAThread]
    static int Main()
    {
        var output = Path.GetFullPath("analysis/joint-audit/pass11-cal-npoint-mainwindow");
        Directory.CreateDirectory(output);
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(app.Dispatcher));
        var shell = new MainWindow {
            Left = -20000, Top = -20000, ShowActivated = false, ShowInTaskbar = false,
            WindowStartupLocation = WindowStartupLocation.Manual
        };
        try
        {
            shell.Show();
            var graph = Field<GraphEditorDocument>(shell, "doc");
            graph.NewSolution();
            var rows = new List<object>();
            foreach (var (calib, rotate) in new[] { (4, 0), (9, 3), (16, 16) })
            {
                var id = graph.AddNode(NPointPlugin.Id, "1.0.0", 100, 0);
                var node = graph.CurrentProcedure.Nodes.Single(n => n.Id == id);
                var values = node.Parameters.ToDictionary(p => p.Key, p => p.Value.Clone());
                values["CalibPointTotalNum"] = JsonSerializer.SerializeToElement(calib);
                values["RotPointTotalNum"] = JsonSerializer.SerializeToElement(rotate);
                graph.SetParameters(id, values);
                shell.GetType().GetField("selected", Private)!.SetValue(shell, id);
                Call(shell, "RefreshAll");
                Call(shell, "OpenToolConfiguration", id);
                var host = Field<ToolConfigurationWindowHost>(shell, "toolConfigurationWindows");
                var config = (ToolConfigurationModelWindow)host.Current!;
                config.ConfigurationView.Tabs.SelectedIndex = 0;
                app.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
                var button = Find<Button>(config, b => b.Name == "NPointEditPoints")
                    ?? throw new InvalidOperationException("Actual configuration point-edit button missing");
                long beforeRevision = graph.Revision;
                int changed = 0;
                graph.Changed += CountChanged;
                void CountChanged() => changed++;
                Exception? callbackError = null;
                long openRevisions = -1, editRevisions = -1;
                int openChanges = -1, editChanges = -1;
                var clock = Stopwatch.StartNew();
                app.Dispatcher.BeginInvoke(() => {
                    try
                    {
                        var popup = app.Windows.Cast<Window>().Single(w => w.Content is NPointConfigurationPointEditor);
                        var editor = (NPointConfigurationPointEditor)popup.Content;
                        openRevisions = graph.Revision - beforeRevision;
                        openChanges = changed;
                        Check(editor.Rows.Count == calib + rotate, "actual point editor row count " + (calib + rotate));
                        long beforeEdit = graph.Revision;
                        int beforeEditChanges = changed;
                        Check(editor.TrySetCell(0, 2, "2.500"), "actual point editor accepts one cell edit");
                        editRevisions = graph.Revision - beforeEdit;
                        editChanges = changed - beforeEditChanges;
                        popup.Close();
                    }
                    catch (Exception error)
                    {
                        callbackError = error;
                        foreach (var popup in app.Windows.Cast<Window>().Where(w => w.Content is NPointConfigurationPointEditor).ToArray()) popup.Close();
                    }
                }, DispatcherPriority.ApplicationIdle);
                button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                clock.Stop();
                graph.Changed -= CountChanged;
                if (callbackError is not null) throw callbackError;
                Console.WriteLine($"rows={calib + rotate} openRevision={openRevisions} openChanged={openChanges} editRevision={editRevisions} editChanged={editChanges} modalMs={clock.Elapsed.TotalMilliseconds:F1}");
                Check(openRevisions == (calib + rotate) * 5 && openChanges == openRevisions,
                    "actual MainWindow point opening performs five full graph edits per row");
                Check(editRevisions == 1 && editChanges == 1,
                    "actual MainWindow single cell change performs one graph edit");
                rows.Add(new { count = calib + rotate, openRevisions, openChanges, editRevisions, editChanges,
                    modalMs = clock.Elapsed.TotalMilliseconds });
                Call(shell, "CloseToolConfiguration");
            }
            File.WriteAllText(Path.Combine(output, "summary.json"), JsonSerializer.Serialize(new { passed = true, rows,
                offscreen = true, manualAccepted = false }));
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
        finally { Call(shell, "CloseToolConfiguration"); shell.Close(); app.Shutdown(); }
    }
}
