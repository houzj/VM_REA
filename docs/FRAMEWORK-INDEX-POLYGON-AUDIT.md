# Group 索引交互与 Polygon/Fixture 数据逆向阶段记录

更新：2026-10-17。原生证据与独立实现必须分开判断；本批没有完成真实 VM 同状态点击差分。

## 已接通的 Group 子集

原始依据见 [数组索引编辑器审计](NATIVE-ARRAY-INDEX-EDITOR-AUDIT.md)：`Apps.XmlParser.ParamTab.ButtonSelecter` 的 `IndexOpenSetter`、光标方括号判定、`IndexClick` 和 `SetFilterSourceOperation.RelateIndex`，以及 `Apps.XmlUI/styles/paramusercontroldatatemplate.xaml` 的 180×180 正文 Popup。框架现在在 Group 的数组中断操作数中显示 `[索引]`；光标进入配对方括号时刷新前置标量整数来源树；双击叶子后写结构化 `GroupOperand.IndexSource`，供实际循环中断运行读取。数字编辑会解除订阅，撤销和保存重开仍按原有 Group 事务处理。

`NativeGroupIndexEditor` 仅实现此 Group 入口。主来源仍用独立按钮，旧 `…` 模态选择器暂保留；原生 240×480 独立订阅窗、统一 `模块.端口[索引]` 文本、Holder 层级、完整变量树及可见样式未实现。不要把当前窗口称为 `ButtonSelecter_sub` 完整移植。

实际 WPF 窗口操作检查：方括号位置检测、180×180 来源树打开、双击局部整数来源、真实图像源 SN `[0,1]` 按索引变量 1 在第二轮中断、输出两圆、改数字解除订阅、撤销恢复、保存重开及关闭累积后再次运行。记录 `analysis/full-audit/native-index-wpf-run.txt`，共 416 项窗口事件/宿主操作通过；`analysis/full-audit/wpf-group-dynamic-index-compound.png` 可检查整窗，弹层局部图由测试辅助输出 `wpf-group-index-popup.png`。这些不是桌面鼠标或 VM 原生对比验收。

## Polygon 与 Fixture 的边界

`VM.Core.DynamicOutput.GetOutputPolygonArray`、`IVmROIPolygon.Num/Points` 与 Blob XML 的 `BlindPolygonPointNum/BlindPolygonPointsX/Y` 确认 Polygon 对象边界及点序。新增独立托管 `PolygonResult`/`PolygonBatch` 保存 `Num`、X/Y、每轮数量、序列化往返，21 项边界检查通过。坐标空间和轮次是本项目显式元数据，**尚未进入** `DataKind`、Group 端口、原生算法工具或 UI，不能算实际 Polygon 运行闭合；也不能用现有 Region 容器冒充原生输出。原生 ABI、空对象与数值精度仍待差分。

[Fixture 字段审计](FIXTURE-NATIVE-FIELD-AUDIT.md)确认十个数值叶及模块可见性约束；当前只具备根值/数组传输，没有十叶订阅。原生 `Fixture()` 默认 Scale 为零，当前正缩放验证与其不同；目前未改这个约束，以免把未验证的零尺度套进几何变换。需要在后续批次把“传输有效”和“变换可用”拆开，再接可见端口及实际运行投影。

## 后续闭合条件

1. Group 索引：统一来源和括号文本、独立订阅窗双模式、选源保留索引；源代码与实际 UI/执行逐项对照。
2. Polygon：先确定真实插件输出描述符与调用点，再接端口、累积、叶树及算法输出；验证不丢对象边界。
3. Fixture：按输出可见性暴露十叶、验证标量/数组与重命名、保存、真实工具输出；零尺度传输和几何变换单独处理。

原始证据仍在本机 `analysis/decompiled` 和已安装 VM 文件中；两份专项审计含精确文件路径及差异。本批无插件工作树修改，截图辅助仍只在测试项目。
