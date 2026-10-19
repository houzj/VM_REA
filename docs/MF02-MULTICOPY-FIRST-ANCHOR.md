# MF-02 多节点 Ctrl 拖放复制锚点

## 源码调用链和可证实行为

- `D:/MyWork/REA逆向测试/analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/DiagramPage.xaml.cs`：`GetItemsByRect`（约 381–389 行）从 Canvas 子元素 `DiagramItem` 中按现有枚举顺序收集完整落入选区的项；`DiagramPage_PreviewMouseMove`（约 623–685 行）把 `CurBoxOfItems` 作为拖动数据，并根据 Ctrl 选择 `DragDropEffects.Copy`；其 `_dragDrop`（约 688–703 行）将目标位置写入拖放数据。
- `D:/MyWork/REA逆向测试/analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/ItemsPage.cs`：`GetDropItems`（约 322–343 行）根据框选节点的最小坐标与投放位置计算每个节点的目标位置，并逐个按画布边界 clamp；`OnDrop`（约 365–404 行）在 Ctrl 且允许 Copy 时先触发 `OnCopyItems`，再触发 `OnPasteItems`，`DiagramEventArgs.Point` 取 `dropItems.First()[1]`。
- `D:/MyWork/REA逆向测试/analysis/decompiled/VMControls.WPF/VMControls.WPF.DataForUI/VmModuleBaseDecorator.cs`：`CopyItemsByDropCommandExecute`（约 715–731 行）将每项的原模块身份写入全局 Clipboard，并以首项 drop point 作为 Clipboard.Position；`PasteItemsCommandExecute`（约 733–810 行）调用 `IVmModuleSet.ProcedurePaste`，给复制映射中的首项设置 `ts.Point`，后续项用其原位置减首项原位置的差值平移。后续项 clamp 到 `DiagramPageWidth - 150`；首项只做 5 单位舍入和负值下限，不应用同一上界。
- `D:/MyWork/REA逆向测试/analysis/decompiled/VM.Core/VM.Core/VmModuleSet.cs`：`ProcedurePaste`（约 1318–1430 行）按 Clipboard 列表创建原节点到副本的映射。装饰器随后遍历映射更新位置和描述。
- **行为结论**：多节点 Ctrl-copy 的 clone 组以拖放列表第一项的单项投放坐标作为锚点；其余 clone 保留原节点相对第一项的偏移。各源 ghost 虽在拖动期间单独 clamp，copy 的新组不是以所有 ghost 的最小 x/y 作为粘贴锚点。

## 已实现的本地修复

`src/VisionEditor/WorkflowCanvas.cs` 的 `CommitNodeMove` 根据复制片段第一项对应的 ghost 位置，加上片段最小源坐标与首项源坐标之差，换算为 `GraphEditorDocument.PasteNodes` 所需的最小坐标锚点。这样 `PasteNodes` 以片段最小坐标平移时，首个 clone 仍落在源代码规定的第一项 drop point；其余节点继续按原相对偏移，由已有的 `PasteNodes` 坐标上界处理。

## WPF 检查状态

新增 `tests/VisionEditor.CanvasChecks/Program.cs` 场景：按画布顺序创建第一项 `(4800,100)`、第二项 `(100,250)`，Ctrl-copy 拖动 `+500` 后验证源 ghost 分别 clamp 到 `x=4890`、`x=600`。根据上面的原生数据流，新副本预期为第一项 `(4890,100)`、第二项 `(190,250)`；验证撤销、重做和保存重开。

**运行结果：已通过 WPF 检查。** 命令 `dotnet run --project tests/VisionEditor.CanvasChecks/VisionEditor.CanvasChecks.csproj -c Release` 完整退出，观察到 57 条 `PASS`，无失败；构建仅有已有 `MainWindow.xaml.cs(83,224)` `CS8603` 可空警告。新增的四项操作检查通过：两 ghost 各自边界夹取、复制节点位置为 `(4890,100)` 和 `(190,250)`、Undo 删除副本并保留原节点、Redo 后保存重开位置稳定。测试使用显示的 WPF Window 与实际画布元素，由夹具调用画布开始/预览/提交入口，不是物理鼠标驱动。

测试在拖动预览时分别保存滚动前后截图：`analysis/full-audit/canvas-multicopy-right-edge-preview-near.png` 与 `canvas-multicopy-right-edge-preview-far.png`。目检确认近端视口可见低坐标原节点，远端视口可见 x=4800 原节点及 x=4890 橙色预览框；截图对应同一未提交拖动状态。截图只验证本地 WPF 实际渲染，不作为 VM 视觉差分证据。若目标 VM 的运行时 Dictionary 枚举顺序与复制列表顺序不同，仍需记录该差异；不因当前测试通过而声称全域原生等价。

## 证据边界

- **源码确认**：框选项来源、单项 drop 目标的计算和 clamp、Ctrl Copy 的两个事件调用顺序、Clipboard 首项位置、Paste 首项与后续项的坐标策略、首/后续项目上界差异。
- **本地独立实现**：本地选区/ghost 通过 WPF 画布实现；`PasteNodes` 使用本地 `GraphFragment` 及自己的节点标识与连接模型，没有调用 `ProcedurePaste` 或原 `Clipboard`。
- **未知**：真实 VM 窗口的物理鼠标拖放、硬件 OLE 修饰键状态和大规模/嵌套 Group 复制没有在本轮原生程序复验；本测试使用真实 WPF Window 并由测试夹具调用手势入口/预览/提交方法，不等同于桌面输入。复制映射依赖目标运行时 Dictionary 枚举顺序；须由当前检查的 VM runtime 完整操作确认该顺序再关闭此差异。
