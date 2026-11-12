using System.Windows;
using System.Windows.Controls;
using VisionEditor.Core;
using VisionFramework;
using Binding = System.Windows.Data.Binding;

namespace VisionEditor;

/// <summary>
/// 右侧「当前结果」页接到节点数据投影层。
/// 契约：docs/reverse/W01-workspace-baseline-contract-20261111.md 第 3.3 节——
/// 列集合由当前节点的输出类型决定，切换节点即换列；同节点重复运行只刷新行。
/// </summary>
public partial class MainWindow
{
    readonly NodeProjectionSession nodeProjection = new();

    /// <summary>测试用：当前投影快照。</summary>
    internal NodeProjection CurrentProjection => nodeProjection.Current;

    void RefreshNodeProjection()
    {
        if (windowClosed || doc is null) return;
        var node = selected is null ? null : doc.CurrentProcedure.Nodes.FirstOrDefault(candidate => candidate.Id == selected);
        ModuleDescriptor? descriptor = null;
        if (node is not null)
        {
            try { descriptor = modules.GetDescriptor(node); }
            catch (KeyNotFoundException) { descriptor = null; }
        }
        NodeRunRecord? record = null;
        if (node is not null) results.TryGetValue(node.Id, out record);

        var rebuilt = nodeProjection.Update(node, descriptor, record, ToolHelp(node));
        if (rebuilt) RebuildCurrentResultColumns();
        CurrentResultGrid.ItemsSource = null;
        CurrentResultGrid.ItemsSource = nodeProjection.Current.Rows;
        if (HelpText is not null) HelpText.Text = nodeProjection.Current.Help;
        RefreshModuleHistory();

        var hasTable = nodeProjection.Current.HasTable;
        CurrentResultTab.Visibility = hasTable ? Visibility.Visible : Visibility.Collapsed;
        if (!hasTable && ReferenceEquals(BottomTabs.SelectedItem, CurrentResultTab)) BottomTabs.SelectedItem = HistoryTab;
        ApplyResultTabVisibility();
    }

    /// <summary>历史结果页：绑当前选中模块的执行记录（原厂 hstTab → ModuleDisplayInfoList）。</summary>
    void RefreshModuleHistory()
    {
        if (HistoryGrid is null) return;
        HistoryGrid.ItemsSource = doc.HasCurrentProcedure && selected is not null
            ? ModuleRuns.HistoryRows(doc.CurrentProcedureId, selected)
            : Array.Empty<ModuleHistoryRow>();
    }

    void RebuildCurrentResultColumns()
    {
        CurrentResultGrid.Columns.Clear();
        var columns = nodeProjection.Current.Columns;
        for (var index = 0; index < columns.Count; index++)
        {
            CurrentResultGrid.Columns.Add(new DataGridTextColumn
            {
                Header = columns[index].Header,
                Binding = new Binding("Cells[" + index.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]"),
                Width = index == 0 ? new DataGridLength(64) : new DataGridLength(1, DataGridLengthUnitType.SizeToCells)
            });
        }
    }

    void BottomTabsChanged(object sender, SelectionChangedEventArgs e) => ApplyResultTabVisibility();

    void ApplyResultTabVisibility()
    {
        if (CurrentResultGrid is null || HistoryGrid is null) return;
        // 隐藏当前页或夹在折叠页时，WPF 可能把选中项留在不可见页上，这里统一回落到历史结果。
        if (BottomTabs.SelectedItem is TabItem selectedTab && selectedTab.Visibility != Visibility.Visible)
            BottomTabs.SelectedItem = HistoryTab;
        var onCurrent = ReferenceEquals(BottomTabs.SelectedItem, CurrentResultTab);
        var onHistory = ReferenceEquals(BottomTabs.SelectedItem, HistoryTab);
        var showCurrent = onCurrent && nodeProjection.Current.HasTable;
        CurrentResultGrid.Visibility = showCurrent ? Visibility.Visible : Visibility.Collapsed;
        HistoryGrid.Visibility = onHistory ? Visibility.Visible : Visibility.Collapsed;
        CurrentResultEmpty.Visibility = onCurrent && !nodeProjection.Current.HasTable
            ? Visibility.Visible : Visibility.Collapsed;
        if (HelpPane is not null)
            HelpPane.Visibility = ReferenceEquals(BottomTabs.SelectedItem, HelpTab) ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// 帮助页文本。原厂 moduleHelp 绑工具自身的 HelperContent；
    /// 本实现没有工具帮助正文来源，因此用工具描述符的真实结构（端口/参数）生成可读说明，
    /// 不使用编造的介绍文字。
    /// </summary>
    string ToolHelp(NodeDocument? node)
    {
        if (node is null) return string.Empty;
        ModuleDescriptor? descriptor = null;
        try { descriptor = modules.GetDescriptor(node); }
        catch (KeyNotFoundException) { return string.Empty; }
        var text = new System.Text.StringBuilder();
        text.AppendLine(descriptor.DisplayName + "（" + descriptor.Id + " " + descriptor.Version + "）");
        text.AppendLine();
        text.AppendLine("输入端口：");
        if (descriptor.Inputs.Count == 0) text.AppendLine("  （无）");
        foreach (var port in descriptor.Inputs)
            text.AppendLine("  " + port.Name + " · " + port.Kind + (port.Required ? " · 必需" : " · 可选"));
        text.AppendLine();
        text.AppendLine("输出端口：");
        if (descriptor.Outputs.Count == 0) text.AppendLine("  （无）");
        foreach (var port in descriptor.Outputs)
            text.AppendLine("  " + port.Name + " · " + port.Kind + (port.Required ? " · 必需" : " · 可选"));
        text.AppendLine();
        text.AppendLine("参数：");
        if (descriptor.Parameters.Count == 0) text.AppendLine("  （无）");
        foreach (var parameter in descriptor.Parameters)
            text.AppendLine("  " + parameter.Name + " · " + parameter.Kind + (parameter.Required ? " · 必需" : " · 可选"));
        return text.ToString().TrimEnd();
    }
}
