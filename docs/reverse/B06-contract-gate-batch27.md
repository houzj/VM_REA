# 第二十七轮 B 线：Group 配置、ROI 与结果消费契约门槛

基线按主线当前提交读取；本片只复核 B06.2/B06 剩余边界，不读取或修改 `tool-plugins`。

## 源码证据

原始 `ROISelecter` 实现 `IGroupRunParam.DisplayForGroupType` 返回 `RoiConfigControl`，并由 `DynamicExtern.GetSupportDisplayForGroupData` 将其装饰为 `ROISelecterForGroupDecorate`；`GroupRunParam` 在选中运行参数后先解析关联模块，再查询该能力，缺失时才落到 `CommonParamDecorate`。`ROISelecterForGroupDecorate` 直接转发 ROI 集合、选择、删除、复制/粘贴能力，并从关联模块懒取图像；`ROIDataStore` 携带参数字节、图像尺寸及可选序号。这条链依赖插件侧真实 `IGroupRunParam` producer、ROI 多字段协议和展示工厂。

当前 main 的 `GroupInputRunControls`/`GroupRunConfigurationWindow` 只消费 `ModuleDescriptor`、`ParameterDescriptor`、`IWpfToolParameterEditorFactory` 及托管 `DataKind`/`WorkflowValueReference`。它已提供有限的 Group 参数候选、订阅写回、托管 ROI 编辑器和结果显示，但没有原生 `GetSupportDisplayForGroupData`、`DisplayForGroupType`、`CloneSource`、多字段 ROI 字节/序号、工具专属展示生命周期或真实 ROI producer。`VisionPluginContracts/Contracts.cs` 的 descriptor 也没有这些能力字段。

## 结论

B06.2 仍不能在 main 独占范围内完整关闭：补字段或新增 ROI 模板会复制插件配置框架，且不能证明原生写回/运行消费。B06.3 的托管结果消费（`GroupOutputAccumulator`、`ExecutionSessionResult.WorkflowDisplays/WorkflowOutputs`）已有独立实现，但原生结果对象、跨 Group 每轮刷新/清理、ROI/Display 关联和 ABI 仍无 producer/consumer 证据，不能以现有托管结果类型宣称原生等价。B06.1 的 FCR-021 资格合同同样未解除。

实施增量：**+0**。未修改产品源码、合同或共享文件；未访问 `tool-plugins`。未知项保留：原生参数字节布局、ROI 坐标/序号语义、装饰器首次展示触发、Group SelfRun 每轮刷新顺序、结果对象所有权及释放。

## 解除条件

需要插件侧经审查的 producer SHA 与版本化能力快照（稳定参数身份、Group 资格、`DisplayForGroupType`、ROI 类型/坐标空间/数量、图像来源、应用/取消、结果 payload 生命周期），由 main 仅消费并逐入口验证配置→执行→结果→保存重开。无该证据不实施局部兼容补丁。

## 证据哈希

详见 `analysis/full-audit/batch27-20261020/B/b06-contract-gate.json`。
