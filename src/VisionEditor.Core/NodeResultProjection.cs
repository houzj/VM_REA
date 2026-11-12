using System.Globalization;
using VisionCore;
using VisionFramework;

namespace VisionEditor.Core;

/// <summary>
/// 结果列定义。列集合由节点输出类型决定，对应原厂
/// <c>ModuleResultView.RefreshDataListResult</c> 里由 <c>Colume</c> 特征生成的列表头。
/// </summary>
public sealed record ResultColumn(string Key, string Header);

/// <summary>结果行。Cells 与 <see cref="NodeProjection.Columns"/> 一一对应。</summary>
public sealed record ResultRow(IReadOnlyList<string> Cells);

/// <summary>
/// 单个节点在右侧面板上的完整投影：当前结果表、可用图像、帮助、耗时。
/// 只承载「显示用」数据，不持有 SDK 缓冲区。
/// </summary>
public sealed record NodeProjection(
    string? NodeId,
    string Title,
    IReadOnlyList<ResultColumn> Columns,
    IReadOnlyList<ResultRow> Rows,
    IReadOnlyList<string> Images,
    string Help,
    string TimingText,
    bool HasTable)
{
    public static readonly NodeProjection Empty = new(null, string.Empty, [], [], [], string.Empty, string.Empty, false);

    /// <summary>列结构签名，用于判断是否需要重建表头（对应原厂 NeedRebuildColume）。</summary>
    public string ColumnSignature => string.Join("|", Columns.Select(column => column.Key));

    public int RowCount => Rows.Count;
}

/// <summary>
/// 节点结果投影构造器。纯函数，不依赖 WPF，便于单元断言。
/// 契约依据：docs/reverse/W01-workspace-baseline-contract-20261111.md 第 3.3 / 3.4 / 3.6 节。
/// </summary>
public static class NodeProjectionBuilder
{
    const string NumberHeader = "序号";

    public static NodeProjection Build(
        NodeDocument? node,
        ModuleDescriptor? descriptor,
        NodeRunRecord? record,
        string help = "")
    {
        if (node is null) return NodeProjection.Empty;

        var outputs = record?.Outputs ?? (IReadOnlyDictionary<string, PortValue>)new Dictionary<string, PortValue>();
        var title = string.IsNullOrWhiteSpace(node.Name) ? descriptor?.DisplayName ?? node.ModuleId : node.Name;
        var images = ImageOutputs(descriptor, outputs);
        var timing = record is null ? string.Empty : record.ElapsedMilliseconds.ToString("F2", CultureInfo.InvariantCulture);
        var helpText = string.IsNullOrWhiteSpace(help) ? string.Empty : help;

        var tabular = TabularOutput(descriptor, outputs);
        if (tabular is null)
        {
            // 回落表只用于「模块结果」式键值展示；「当前结果」页按原厂语义要求工具声明
            // datalist 能力（CurResultTabItem.Visibility 取决于该标志），因此 HasTable=false。
            var fallback = FallbackTable(outputs);
            return new NodeProjection(node.Id, title, fallback.Columns, fallback.Rows, images, helpText, timing, false);
        }

        // 列结构由「工具能力」决定而不是由本次是否有结果决定：
        // 未运行时仍要显示表头与空行，对应原厂 GetDisplayList() 的静态 datalist 声明。
        var (columns, rows) = tabular.Value.Value is { } actual
            ? BuildTabular(tabular.Value.Port, actual)
            : (EmptyColumns(tabular.Value.Port, tabular.Value.Kind), (IReadOnlyList<ResultRow>)[]);
        return new NodeProjection(node.Id, title, columns, rows, images, helpText, timing, true);
    }

    /// <summary>
    /// 选择承载「当前结果」表格的输出。优先级：Components &gt; Circle &gt; Point &gt; NumberArray &gt; 标量。
    /// 与原厂一致：只有声明了 datalist 能力的输出才占用当前结果页，其余回落为键值表。
    /// </summary>
    static (string Port, DataKind Kind, PortValue? Value)? TabularOutput(ModuleDescriptor? descriptor, IReadOnlyDictionary<string, PortValue> outputs)
    {
        var order = descriptor?.Outputs.Select(output => output.Name).ToArray() ?? [];
        // 先按工具声明的输出能力判定（未运行时也要有列）。
        foreach (var kind in new[] { DataKind.Components, DataKind.Circle, DataKind.Point, DataKind.NumberArray })
        {
            var declared = descriptor?.Outputs.FirstOrDefault(output => output.Kind == kind);
            if (declared is null) continue;
            outputs.TryGetValue(declared.Name, out var actual);
            return (declared.Name, kind, actual);
        }
        if (outputs.Count == 0) return null;
        foreach (var name in OrderedNames(outputs, order))
        {
            var value = outputs[name];
            if (!DataKinds.IsImage(value.Kind) && value.Kind is not DataKind.Region and not DataKind.Fixture and not DataKind.Calibration)
                return (name, value.Kind, value);
        }
        return null;
    }

    /// <summary>未运行时的列骨架，与有结果时的列定义保持一致。</summary>
    static IReadOnlyList<ResultColumn> EmptyColumns(string port, DataKind kind) => kind switch
    {
        DataKind.Components =>
        [
            new ResultColumn("Number", NumberHeader),
            new ResultColumn("Area", "面积"),
            new ResultColumn("CenterX", "中心X"),
            new ResultColumn("CenterY", "中心Y"),
            new ResultColumn("Width", "宽度"),
            new ResultColumn("Height", "高度")
        ],
        DataKind.Circle =>
        [
            new ResultColumn("Number", NumberHeader),
            new ResultColumn("CenterX", "中心X"),
            new ResultColumn("CenterY", "中心Y"),
            new ResultColumn("Radius", "半径"),
            new ResultColumn("RadialRmsError", "拟合误差")
        ],
        DataKind.Point =>
        [
            new ResultColumn("Number", NumberHeader),
            new ResultColumn("X", "X"),
            new ResultColumn("Y", "Y")
        ],
        _ =>
        [
            new ResultColumn("Number", NumberHeader),
            new ResultColumn(port, port)
        ]
    };

    static IEnumerable<string> OrderedNames(IReadOnlyDictionary<string, PortValue> outputs, IReadOnlyList<string> order)
    {
        foreach (var name in order)
            if (outputs.ContainsKey(name)) yield return name;
        foreach (var name in outputs.Keys)
            if (!order.Contains(name, StringComparer.Ordinal)) yield return name;
    }

    static DataKind KindOf(PortValue value) => value.Kind;

    static (IReadOnlyList<ResultColumn> Columns, IReadOnlyList<ResultRow> Rows) BuildTabular(string port, PortValue value)
    {
        if (value.Kind == DataKind.Components)
        {
            var components = value.As<IReadOnlyList<Component>>();
            var columns = new[]
            {
                new ResultColumn("Number", NumberHeader),
                new ResultColumn("Area", "面积"),
                new ResultColumn("CenterX", "中心X"),
                new ResultColumn("CenterY", "中心Y"),
                new ResultColumn("Width", "宽度"),
                new ResultColumn("Height", "高度")
            };
            var rows = components.Select((component, index) => new ResultRow(new[]
            {
                (index + 1).ToString(CultureInfo.InvariantCulture),
                component.Area.ToString(CultureInfo.InvariantCulture),
                F(component.CenterX),
                F(component.CenterY),
                (component.MaxX - component.MinX + 1).ToString(CultureInfo.InvariantCulture),
                (component.MaxY - component.MinY + 1).ToString(CultureInfo.InvariantCulture)
            })).ToArray();
            return (columns, rows);
        }

        if (value.Kind == DataKind.Circle)
        {
            var circle = value.As<CircleFit>();
            var columns = new[]
            {
                new ResultColumn("Number", NumberHeader),
                new ResultColumn("CenterX", "中心X"),
                new ResultColumn("CenterY", "中心Y"),
                new ResultColumn("Radius", "半径"),
                new ResultColumn("RadialRmsError", "拟合误差")
            };
            var rows = new[]
            {
                new ResultRow(new[] { "1", F(circle.CenterX), F(circle.CenterY), F(circle.Radius), F(circle.RadialRmsError) })
            };
            return (columns, rows);
        }

        if (value.Kind == DataKind.Point)
        {
            var columns = new[]
            {
                new ResultColumn("Number", NumberHeader),
                new ResultColumn("X", "X"),
                new ResultColumn("Y", "Y")
            };
            var rows = value.Cardinality == ValueCardinality.Array
                ? value.As<IReadOnlyList<Point2>>().Select((point, index) =>
                    new ResultRow(new[] { (index + 1).ToString(CultureInfo.InvariantCulture), F(point.X), F(point.Y) })).ToArray()
                : new[] { new ResultRow(new[] { "1", F(value.As<Point2>().X), F(value.As<Point2>().Y) }) };
            return (columns, rows);
        }

        if (value.Kind == DataKind.NumberArray)
        {
            var numbers = value.As<IReadOnlyList<double>>();
            var columns = new[]
            {
                new ResultColumn("Number", NumberHeader),
                new ResultColumn(port, port)
            };
            var rows = numbers.Select((number, index) => new ResultRow(new[]
            {
                (index + 1).ToString(CultureInfo.InvariantCulture),
                F(number)
            })).ToArray();
            return (columns, rows);
        }

        var scalarColumns = new[]
        {
            new ResultColumn("Number", NumberHeader),
            new ResultColumn(port, port)
        };
        var scalarRows = new[] { new ResultRow(new[] { "1", Scalar(value) }) };
        return (scalarColumns, scalarRows);
    }

    /// <summary>没有任何表格型输出时的回落：输出名称 / 类型 / 值。</summary>
    static (IReadOnlyList<ResultColumn> Columns, IReadOnlyList<ResultRow> Rows) FallbackTable(IReadOnlyDictionary<string, PortValue> outputs)
    {
        var columns = new[]
        {
            new ResultColumn("Port", "输出名称"),
            new ResultColumn("Kind", "类型"),
            new ResultColumn("Value", "值")
        };
        var rows = outputs.Select(pair => new ResultRow(new[] { pair.Key, Label(pair.Value), Scalar(pair.Value) })).ToArray();
        return (columns, rows);
    }

    static IReadOnlyList<string> ImageOutputs(ModuleDescriptor? descriptor, IReadOnlyDictionary<string, PortValue> outputs)
    {
        var names = (descriptor?.Outputs ?? []).Where(output => DataKinds.IsImage(output.Kind)).Select(output => output.Name).ToList();
        foreach (var pair in outputs)
            if (DataKinds.IsImage(pair.Value.Kind) && !names.Contains(pair.Key, StringComparer.Ordinal)) names.Add(pair.Key);
        return names;
    }

    static string Label(PortValue value) =>
        value.Kind + (value.Cardinality == ValueCardinality.Array ? "[]" : string.Empty);

    static string Scalar(PortValue value)
    {
        if (value.Cardinality == ValueCardinality.Array)
        {
            if (value.Kind == DataKind.NumberArray)
                return "[" + string.Join(", ", value.As<IReadOnlyList<double>>().Select(F)) + "]";
            if (value.Value is System.Collections.IEnumerable enumerable)
                return "[" + string.Join(", ", enumerable.Cast<object>().Select(Convert.ToString)) + "]";
        }
        return value.Kind switch
        {
            DataKind.GrayImage => Describe(value.As<GrayImage>()),
            DataKind.Components => value.As<IReadOnlyList<Component>>().Count.ToString(CultureInfo.InvariantCulture) + " 个连通域",
            DataKind.Circle => F(value.As<CircleFit>().Radius),
            DataKind.Point when value.Cardinality == ValueCardinality.Array =>
                "[" + string.Join(", ", value.As<IReadOnlyList<Point2>>().Select(point => F(point.X) + ", " + F(point.Y))) + "]",
            DataKind.Point => F(value.As<Point2>().X) + ", " + F(value.As<Point2>().Y),
            DataKind.Boolean => value.As<bool>() ? "True" : "False",
            _ => Convert.ToString(value.Value, CultureInfo.InvariantCulture) ?? string.Empty
        };
    }

    static string Describe(GrayImage image) => image.Width.ToString(CultureInfo.InvariantCulture) + " × " + image.Height.ToString(CultureInfo.InvariantCulture) + " MONO8";

    static string F(double value) => value.ToString("F3", CultureInfo.InvariantCulture);
}

/// <summary>
/// 统一选中态：持有「当前选中节点 + 其结果投影」，并按列签名做增量重建。
/// 对应原厂 <c>ModuleResultView.ModuleSourceChanged</c> → <c>DoSourceChanged</c> 的换源语义，
/// 差别在于这里换源只重建列，不销毁 SDK 缓冲区。
/// </summary>
public sealed class NodeProjectionSession
{
    NodeProjection current = NodeProjection.Empty;

    public NodeProjection Current => current;

    /// <summary>累计列重建次数，用于断言「同节点重复运行不重建列」。</summary>
    public int RebuildCount { get; private set; }

    /// <summary>最近一次更新是否重建了表头。</summary>
    public bool ColumnsRebuilt { get; private set; }

    /// <summary>
    /// 更新投影。返回 true 表示列结构发生变化、需要重建表头。
    /// </summary>
    public bool Update(NodeDocument? node, ModuleDescriptor? descriptor, NodeRunRecord? record, string help = "")
    {
        var next = NodeProjectionBuilder.Build(node, descriptor, record, help);
        var rebuild = !string.Equals(next.ColumnSignature, current.ColumnSignature, StringComparison.Ordinal);
        ColumnsRebuilt = rebuild;
        if (rebuild) RebuildCount++;
        current = next;
        return rebuild;
    }

    public void Clear()
    {
        current = NodeProjection.Empty;
        ColumnsRebuilt = false;
    }
}
