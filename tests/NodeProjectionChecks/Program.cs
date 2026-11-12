using System.Text.Json;
using VisionCore;
using VisionEditor.Core;
using VisionFramework;

/// <summary>
/// 节点结果投影层断言。口径来自
/// docs/reverse/W01-workspace-baseline-contract-20261111.md 第 3.3/3.4 节：
/// 当前结果列随工具输出类型变化、历史结果固定三列、换源需要重建列。
/// </summary>
internal static class Program
{
    static int checks;

    static int Main()
    {
        try
        {
            VerifyComponentColumns();
            VerifyScalarColumns();
            VerifyImageOnlyNode();
            VerifyCircleColumns();
            VerifyEmptyNode();
            VerifySessionIncremental();
            VerifySessionSwitchRebuilds();
            VerifyClear();
            VerifyLogStore();
            VerifyModuleLedger();
            Console.WriteLine($"NodeProjectionChecks: {checks} 项投影断言全部通过。");
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("FAILED: " + error.Message);
            return 1;
        }
    }

    static NodeDocument Node(string id, string moduleId, string name) =>
        new(id, moduleId, "1.0", name, new Dictionary<string, JsonElement>());

    static ModuleDescriptor Descriptor(string id, params PortDescriptor[] outputs) =>
        new(id, "1.0", id, [], outputs, []);

    static PortDescriptor Out(string name, DataKind kind) => new(name, kind);

    static NodeRunRecord Record(string nodeId, params (string Name, PortValue Value)[] outputs) =>
        new(nodeId, NodeRunStatus.Success, outputs.ToDictionary(pair => pair.Name, pair => pair.Value), 12.5);

    static void VerifyComponentColumns()
    {
        var descriptor = Descriptor("Blob", Out("Image", DataKind.GrayImage), Out("Components", DataKind.Components));
        var components = new[]
        {
            new Component(100, 0, 0, 9, 9, 4.5, 4.5),
            new Component(25, 20, 20, 24, 24, 22.0, 22.0)
        };
        var projection = NodeProjectionBuilder.Build(
            Node("blob1", "Blob", "Blob分析1"), descriptor,
            Record("blob1", ("Image", PortValue.Image(Image(4, 4))), ("Components", PortValue.Components(components))));

        Check(projection.Columns.Count == 6, "连通域输出生成 6 列（序号/面积/中心X/中心Y/宽度/高度）");
        Check(string.Join(",", projection.Columns.Select(c => c.Header)) == "序号,面积,中心X,中心Y,宽度,高度",
            "列标题：" + string.Join(",", projection.Columns.Select(c => c.Header)));
        Check(projection.Rows.Count == 2, "每个连通域一行");
        Check(projection.Rows[0].Cells[1] == "100", "面积取自 Component.Area");
        Check(projection.Rows[0].Cells[4] == "10", "宽度按 MaxX-MinX+1 计算");
        Check(projection.HasTable, "连通域节点启用当前结果表");
        Check(projection.Images.SequenceEqual(new[] { "Image" }), "图像输出被单独列出");
        Check(projection.TimingText == "12.50", "耗时取节点记录");
    }

    static void VerifyScalarColumns()
    {
        var descriptor = Descriptor("Counter", Out("Count", DataKind.Integer));
        var projection = NodeProjectionBuilder.Build(
            Node("n1", "Counter", "计数1"), descriptor,
            Record("n1", ("Count", PortValue.Integer(7))));

        Check(projection.Columns.Count == 2 && projection.Columns[1].Header == "Count", "标量输出生成 序号/端口名 两列");
        Check(projection.Rows.Single().Cells[1] == "7", "标量值写入单元格");
    }

    static void VerifyImageOnlyNode()
    {
        var descriptor = Descriptor("ImageSource", Out("Image", DataKind.GrayImage));
        var projection = NodeProjectionBuilder.Build(
            Node("s1", "ImageSource", "图像源1"), descriptor,
            Record("s1", ("Image", PortValue.Image(Image(8, 6)))));

        Check(projection.Images.SequenceEqual(new[] { "Image" }), "纯图像节点只列出图像输出");
        Check(!projection.HasTable, "纯图像节点不启用当前结果表");
        Check(projection.Columns.Count == 3 && projection.Columns[0].Header == "输出名称", "无表格输出时回落为 输出名称/类型/值");
        Check(projection.Rows.Single().Cells[0] == "Image", "回落表行内容为输出名");
    }

    static void VerifyCircleColumns()
    {
        var descriptor = Descriptor("Circle", Out("Circle", DataKind.Circle));
        var projection = NodeProjectionBuilder.Build(
            Node("c1", "Circle", "圆查找1"), descriptor,
            Record("c1", ("Circle", PortValue.Circle(new CircleFit(10.5, 20.25, 3.5, 0.125)))));

        Check(string.Join(",", projection.Columns.Select(c => c.Header)) == "序号,中心X,中心Y,半径,拟合误差",
            "圆输出列：" + string.Join(",", projection.Columns.Select(c => c.Header)));
        Check(projection.Rows.Single().Cells[3] == "3.500", "半径按三位小数显示");
    }

    static void VerifyEmptyNode()
    {
        var projection = NodeProjectionBuilder.Build(null, null, null);
        Check(ReferenceEquals(projection, NodeProjection.Empty), "无节点时返回 Empty 投影");
        Check(!projection.HasTable && projection.Rows.Count == 0, "Empty 投影不携带表格");
    }

    static void VerifySessionIncremental()
    {
        var session = new NodeProjectionSession();
        var descriptor = Descriptor("Blob", Out("Components", DataKind.Components));
        var node = Node("b1", "Blob", "Blob分析1");
        var components = new[] { new Component(10, 0, 0, 2, 2, 1, 1) };

        session.Update(node, descriptor, Record("b1", ("Components", PortValue.Components(components))));
        Check(session.RebuildCount == 1 && session.ColumnsRebuilt, "首次投影重建列");

        session.Update(node, descriptor, Record("b1", ("Components", PortValue.Components(components.Concat(components).ToArray()))));
        Check(session.RebuildCount == 1 && !session.ColumnsRebuilt, "同一节点重复运行不重建列");
        Check(session.Current.Rows.Count == 2, "重复运行只刷新行");
    }

    static void VerifySessionSwitchRebuilds()
    {
        var session = new NodeProjectionSession();
        var blob = Descriptor("Blob", Out("Components", DataKind.Components));
        var circle = Descriptor("Circle", Out("Circle", DataKind.Circle));

        session.Update(Node("b1", "Blob", "Blob分析1"), blob,
            Record("b1", ("Components", PortValue.Components(new[] { new Component(1, 0, 0, 0, 0, 0, 0) }))));
        session.Update(Node("c1", "Circle", "圆查找1"), circle,
            Record("c1", ("Circle", PortValue.Circle(new CircleFit(1, 2, 3, 0)))));

        Check(session.RebuildCount == 2, "切换到不同输出类型必须重建列");
        Check(session.Current.Columns.Count == 5 && session.Current.Title == "圆查找1", "投影切换到新节点");
    }

    static void VerifyClear()
    {
        var session = new NodeProjectionSession();
        var descriptor = Descriptor("Blob", Out("Components", DataKind.Components));
        session.Update(Node("b1", "Blob", "Blob分析1"), descriptor,
            Record("b1", ("Components", PortValue.Components(new[] { new Component(1, 0, 0, 0, 0, 0, 0) }))));
        session.Clear();
        Check(ReferenceEquals(session.Current, NodeProjection.Empty), "Clear 后回到 Empty 投影");
        Check(!session.ColumnsRebuilt, "Clear 复位列重建标记");
    }

    /// <summary>契约 W01 第 4 节：默认折叠、筛选 1/2/3/4、筛选不丢记录。</summary>
    static void VerifyLogStore()
    {
        var log = new ShellLogStore();
        Check(log.IsFolded, "日志默认折叠");
        Check(log.Visible.Count == 0, "初始无日志");

        log.Append(ShellLogKind.Info, "流程执行完成");
        log.Append(ShellLogKind.Error, "图像源缺失");
        log.Append(ShellLogKind.Warning, "结果为 NG");
        Check(log.Visible.Count == 3, "默认显示全部三条");
        Check(log.Visible[0].Kind == ShellLogKind.Warning, "最新一条在最前");
        Check(log.Latest?.Event == "结果为 NG", "Latest 指向最新记录");

        Check(log.SetFilter(3) == ShellLogKind.Error, "筛选码 3 对应错误");
        Check(log.Visible.Count == 1 && log.Visible[0].Event == "图像源缺失", "错误筛选只显示错误");
        Check(log.All.Count == 3, "筛选不丢弃记录");

        log.SetFilter(1);
        Check(log.Visible.Count == 3, "切回全部恢复三条");
        Check(log.SetFilter(2) == ShellLogKind.Warning && log.Visible.Count == 1, "筛选码 2 只显示警告");
        log.SetFilter(1);

        Check(log.SetFolded(false) == false, "SetFolded(false) 表示展开");
        Check(!log.IsFolded, "展开状态被记住");

        for (var index = 0; index < ShellLogStore.MaximumRecords + 20; index++) log.Append(ShellLogKind.Info, "第 " + index + " 条");
        Check(log.All.Count == ShellLogStore.MaximumRecords, "日志上限 500 条");
        Check(log.Visible.Count == ShellLogStore.MaximumRecords, "可见集合同样受限");

        log.Clear();
        Check(log.Visible.Count == 0 && log.All.Count == 0, "Clear 清空日志");
    }

    /// <summary>
    /// 契约 W01 第 3.4 节：历史结果是**模块级**记录，
    /// 三列为 执行序号 / 时间 / 模块数据，模块数据形如 [模块名(端口 值; …)]。
    /// </summary>
    static void VerifyModuleLedger()
    {
        var ledger = new ModuleRunLedger();
        var blobRecord = new NodeRunRecord("b1", NodeRunStatus.Success,
            new Dictionary<string, PortValue>
            {
                ["Components"] = PortValue.Components(new[] { new Component(10, 0, 0, 2, 2, 1, 1) }),
                ["BinaryImage"] = PortValue.Image(Image(4, 4))
            }, 3.5);
        var sourceRecord = new NodeRunRecord("s1", NodeRunStatus.Success,
            new Dictionary<string, PortValue> { ["Image"] = PortValue.Image(Image(8, 8)) }, 1.25);

        ledger.AppendSession("p1", new ExecutionSessionResult(SessionStatus.Success, [sourceRecord, blobRecord]),
            nodeId => nodeId == "b1" ? "Blob分析1" : "图像源1");

        Check(ledger.ExecutionCount == 1, "一次会话计一次执行序号");
        Check(ledger.For("p1", "b1").Count == 1 && ledger.For("p1", "s1").Count == 1, "每个节点各写一条模块级记录");

        var blobEntry = ledger.Latest("p1", "b1")!;
        Check(blobEntry.Sequence == 1, "执行序号写入记录");
        Check(blobEntry.NodeName == "Blob分析1", "记录携带模块名");
        Check(blobEntry.DataText.StartsWith("[Blob分析1("), "模块数据以模块名开头：" + blobEntry.DataText);
        Check(blobEntry.DataText.Contains("Components 1 个"), "模块数据含连通域数量");
        Check(blobEntry.DataText.Contains("BinaryImage 4x4"), "模块数据含图像尺寸");

        var rows = ledger.HistoryRows("p1", "b1");
        Check(rows.Count == 1 && rows[0].RowIndex == 1, "历史行序号来自执行序号");
        Check(rows[0].AddTime.Length >= 19, "历史行时间为 yyyy-MM-dd HH:mm:ss:fff");
        Check(rows[0].Content == blobEntry.DataText, "历史行内容即模块数据");

        ledger.AppendSession("p1", new ExecutionSessionResult(SessionStatus.Success, [blobRecord, sourceRecord]),
            nodeId => nodeId);
        Check(ledger.ExecutionCount == 2 && ledger.For("p1", "b1")[0].Sequence == 2, "第二次执行序号递增且最新在前");
        Check(ledger.For("p1", "c9").Count == 0, "未执行节点无记录");

        ledger.ClearProcedure("p1");
        Check(ledger.For("p1", "b1").Count == 0, "ClearProcedure 清空该流程记录");

        ledger.AppendSession("p2", new ExecutionSessionResult(SessionStatus.Success, [blobRecord]), nodeId => nodeId);
        ledger.Clear();
        Check(ledger.ExecutionCount == 0 && ledger.For("p2", "b1").Count == 0, "Clear 复位账本");
    }

    static GrayImage Image(int width, int height)
    {
        var pixels = new byte[width * height];
        Array.Fill(pixels, (byte)255);
        return new GrayImage(width, height, pixels);
    }

    static void Check(bool condition, string description)
    {
        if (!condition) throw new Exception(description);
        checks++;
        Console.WriteLine("PASS: " + description);
    }
}
