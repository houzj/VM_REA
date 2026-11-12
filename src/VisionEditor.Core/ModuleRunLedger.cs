using System.Globalization;
using VisionCore;
using VisionFramework;

namespace VisionEditor.Core;

/// <summary>
/// 一条模块级执行记录。对应原厂 <c>ModuleDisplayInfoList</c> 的一行：
/// 执行序号、时间、模块数据；额外保留状态、耗时与输出端口，供图像/当前结果/模块结果三个视图复用。
/// </summary>
public sealed record ModuleRunEntry(
    long Sequence,
    DateTime Timestamp,
    string ProcedureId,
    string NodeId,
    string NodeName,
    NodeRunStatus Status,
    double ElapsedMilliseconds,
    IReadOnlyDictionary<string, PortValue> Outputs)
{
    public string TimeText => Timestamp.ToString("yyyy-MM-dd HH:mm:ss:fff", CultureInfo.InvariantCulture);

    /// <summary>历史结果「模块数据」列文案：<c>[模块名(端口) 值; …]</c>。</summary>
    public string DataText
    {
        get
        {
            if (Outputs.Count == 0) return "[" + NodeName + " " + Status + "]";
            var parts = Outputs.Select(pair => pair.Key + " " + Format(pair.Value));
            return "[" + NodeName + "(" + string.Join("; ", parts) + ")]";
        }
    }

    static string Format(PortValue value)
    {
        if (value.Cardinality == ValueCardinality.Array)
        {
            if (value.Kind == DataKind.NumberArray)
                return "[" + string.Join(", ", value.As<IReadOnlyList<double>>().Select(number => number.ToString("F3", CultureInfo.InvariantCulture))) + "]";
            if (value.Kind == DataKind.Point)
                return "[" + string.Join("; ", value.As<IReadOnlyList<VisionCore.Point2>>().Select(point => point.X.ToString("F3", CultureInfo.InvariantCulture) + "," + point.Y.ToString("F3", CultureInfo.InvariantCulture))) + "]";
            if (value.Value is System.Collections.IEnumerable enumerable)
                return "[" + string.Join(", ", enumerable.Cast<object>().Select(item => Convert.ToString(item, CultureInfo.InvariantCulture))) + "]";
        }
        return value.Kind switch
        {
        DataKind.GrayImage => value.As<GrayImage>().Width + "x" + value.As<GrayImage>().Height,
        DataKind.Components => value.As<IReadOnlyList<VisionCore.Component>>().Count.ToString(CultureInfo.InvariantCulture) + " 个",
        DataKind.Number => Convert.ToDouble(value.Value, CultureInfo.InvariantCulture).ToString("F3", CultureInfo.InvariantCulture),
        DataKind.Integer => Convert.ToInt64(value.Value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture),
        DataKind.Boolean => value.As<bool>() ? "True" : "False",
        DataKind.Text => value.As<string>(),
        DataKind.Point when value.Cardinality == ValueCardinality.Array =>
            "[" + string.Join("; ", value.As<IReadOnlyList<VisionCore.Point2>>().Select(point => point.X.ToString("F3", CultureInfo.InvariantCulture) + "," + point.Y.ToString("F3", CultureInfo.InvariantCulture))) + "]",
        DataKind.Point => value.As<VisionCore.Point2>().X.ToString("F3", CultureInfo.InvariantCulture) + "," + value.As<VisionCore.Point2>().Y.ToString("F3", CultureInfo.InvariantCulture),
        DataKind.Circle => value.As<VisionCore.CircleFit>().Radius.ToString("F3", CultureInfo.InvariantCulture),
        _ => Convert.ToString(value.Value, CultureInfo.InvariantCulture) ?? string.Empty
        };
    }
}

/// <summary>
/// 模块级执行账本。取代原先的流程级摘要：历史结果、当前结果、图像列表都从这里取数。
/// 契约见 docs/reverse/W01-workspace-baseline-contract-20261111.md 第 3.4 节（原厂 hstTab 绑 ModuleDisplayInfoList）。
/// </summary>
public sealed class ModuleRunLedger
{
    public const int MaxEntriesPerNode = 200;

    readonly Dictionary<string, List<ModuleRunEntry>> byNode = new(StringComparer.Ordinal);
    long sequence;

    /// <summary>执行次数（流程级），与状态栏「执行序号」一致。</summary>
    public long ExecutionCount { get; private set; }

    /// <summary>把一次会话的全部节点记录写入账本。</summary>
    public void AppendSession(string procedureId, ExecutionSessionResult session, Func<string, string>? nameOf = null)
    {
        ArgumentNullException.ThrowIfNull(session);
        ExecutionCount++;
        var at = DateTime.Now;
        foreach (var record in session.Nodes)
        {
            var name = nameOf?.Invoke(record.NodeId) ?? record.NodeId;
            var entry = new ModuleRunEntry(ExecutionCount, at, procedureId, record.NodeId, name,
                record.Status, record.ElapsedMilliseconds, record.Outputs);
            var key = Key(procedureId, record.NodeId);
            if (!byNode.TryGetValue(key, out var list)) byNode[key] = list = new List<ModuleRunEntry>();
            list.Insert(0, entry);
            if (list.Count > MaxEntriesPerNode) list.RemoveRange(MaxEntriesPerNode, list.Count - MaxEntriesPerNode);
        }
    }

    /// <summary>某个模块的记录（最新在前）。</summary>
    public IReadOnlyList<ModuleRunEntry> For(string procedureId, string nodeId) =>
        byNode.TryGetValue(Key(procedureId, nodeId), out var list) ? list : Array.Empty<ModuleRunEntry>();

    /// <summary>某次执行中某模块的记录。</summary>
    public ModuleRunEntry? Latest(string procedureId, string nodeId)
    {
        var list = For(procedureId, nodeId);
        return list.Count == 0 ? null : list[0];
    }

    /// <summary>当前选中模块的历史行（供 hstTab 绑定）。</summary>
    public IReadOnlyList<ModuleHistoryRow> HistoryRows(string procedureId, string nodeId) =>
        For(procedureId, nodeId).Select(entry => new ModuleHistoryRow(entry.Sequence, entry.TimeText, entry.DataText)).ToArray();

    public void ClearProcedure(string procedureId)
    {
        foreach (var key in byNode.Keys.Where(key => key.StartsWith(procedureId + "\u0000", StringComparison.Ordinal)).ToArray())
            byNode.Remove(key);
    }

    public void Clear()
    {
        byNode.Clear();
        ExecutionCount = 0;
    }

    static string Key(string procedureId, string nodeId) => procedureId + "\u0000" + nodeId;
}

/// <summary>历史结果表格行：执行序号 / 时间 / 模块数据。</summary>
public sealed record ModuleHistoryRow(long RowIndex, string AddTime, string Content);
