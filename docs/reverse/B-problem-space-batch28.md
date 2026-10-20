# 第二十八轮 B 问题空间：main 可独立完成的数据合同筛选

- 基线：`fbcf222f08505555f3a06db9fa116cc21e4fd90e`
- 问题：在不复制 `tool-plugins` 配置框架的前提下，是否存在可由 main 单独闭合的 producer→消费→持久化数据合同？
- 范围：B01、B04、B06、B08 未闭子单元；只读 main 源码及已有反编译证据，未访问或修改 `tool-plugins`。

## 候选筛选

| 候选 | main 已有能力 | 缺失的源码级合同 | 判定 |
|---|---|---|---|
| B01 运行 Tab/变量目标 | `WorkflowRunParameters`、配置校验和托管绑定 | 原始 `BaseNode.XmlLoaded/GetRunParamsTree` 实例对象、SDK 订阅返回与取消确认 | 阻塞 |
| B04.3 复合叶/累积 | `PortValue`、`PolygonBatch` 独立托管容器、明确拒绝未知数组 | 原生 pointset/POLYGON/bytearray producer、`ProcessGroupOutputV44` 载荷/所有权/清理及 Group 回写 | 阻塞 |
| B06.2/B06.3 ROI/结果消费 | `GroupComplexEditor`、`RoiGeometry`、`GroupOutputAccumulator` 托管消费路径 | 插件 `IGroupRunParam` producer、`DisplayForGroupType`、ROI 字节/序号、跨轮 native 结果生命周期 | 阻塞 |
| B08 组合验收 | 已有托管 Group/ROI/历史组合检查 | B07 真实插件交付、原机对照及跨边界 payload | 阻塞 |

## 结论

本问题空间没有可安全计分的新增实施单元。main 可以独立维护已经确认的托管数据合同（Group 输入/输出代理、ROI 几何验证、配置持久化和显式拒绝未知复合类型），但这些合同已经实施并有对应自审，重复修改不会增加 B01/B04/B06/B08 的实施计数。将 `PolygonBatch`、ROI 编辑器或结果累积器扩展成原生等价路径，需要缺失的 producer/ABI/所有权证据，属于猜测或复制插件配置框架。

实施增量：**+0**。本报告只记录筛选结果和解除条件，没有产品源码改动。

## 精确解除条件

1. B01：提供同版本原始运行 Tab 实例，记录 `XmlLoaded → GetRunParamsTree → RunParamsSelectData` 的对象/类型/订阅/取消载荷，并能在 main 责任边界消费。
2. B04.3：提供真实 producer 到 `ProcessGroupOutputV44` 的 pointset/POLYGON/bytearray 字段、基数、packing、所有权、失败和释放证据。
3. B06：插件侧提交带 SHA 的 Group 参数/ROI producer 能力快照（资格、字段、坐标空间、序号、应用/取消、结果 payload 生命周期），main 仅消费该合同。
4. B08：B07 真实插件交付后，才可执行跨层组合闭合；此前不得把托管组合检查升级为原机等价。

## 证据

- `docs/reverse/B04-types-close-batch26.md`
- `docs/reverse/B06-contract-gate-batch27.md`
- `src/VisionFramework/WorkflowRunParameters.cs`
- `src/VisionFramework/GroupOutputAccumulator.cs`
- `src/VisionFramework/PolygonRuntimeValues.cs`
- `src/VisionEditor.Core/GraphEditorDocument.Groups.cs`
