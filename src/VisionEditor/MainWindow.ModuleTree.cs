using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using VisionEditor.Core;
using VisionFramework;
using TreeViewItem = System.Windows.Controls.TreeViewItem;

namespace VisionEditor;

/// <summary>
/// 「模块结果」页树形渲染。契约 W01 第 3.1 节：
/// 原厂该页按 节点 → 输出端口 → 值 的层级展示，而不是固定三列表格。
/// </summary>
public partial class MainWindow
{
    static readonly Brush TreeForeground = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC));

    void RefreshModuleResultTree(NodeDocument node)
    {
        if (ModuleResultTree is null) return;
        var rows = results.TryGetValue(node.Id, out var record) ? ModuleResultRows(node, record) : [];
        var root = new TreeViewItem
        {
            Header = node.Name + "（" + node.ModuleId + "）",
            Foreground = TreeForeground,
            IsExpanded = true,
            FontWeight = FontWeights.Bold
        };
        foreach (var row in rows.OfType<ModuleResultViewRow>())
        {
            var port = new TreeViewItem
            {
                Header = row.Port + " · " + row.Kind,
                Foreground = TreeForeground,
                IsExpanded = true
            };
            port.Items.Add(new TreeViewItem { Header = row.Value, Foreground = TreeForeground, FontWeight = FontWeights.Normal });
            root.Items.Add(port);
        }
        if (root.Items.Count == 0)
            root.Items.Add(new TreeViewItem { Header = "尚未执行，无输出", Foreground = TreeForeground });
        ModuleResultTree.ItemsSource = new[] { root };
    }
}
