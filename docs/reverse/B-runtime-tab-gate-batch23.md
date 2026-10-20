# 第二十三批 B：运行 Tab 与变量候选闭合复核

基线 `a6aac9d0aaad027ff4c1bf107b4701a230b4caa7`。本片只复核 B01.1/B01.2 是否出现新的源码依据或可消费生产合同；不重复此前 B01 静态探针，不修改共享 Contracts、窗口实现或 tool-plugins。

## 复核结论

没有新的解除条件，B01.1/B01.2 本轮均保持未闭合，实施增量 `0`。

- B01.1 仍缺真实装配后的 `Tab_Run Params` 生产快照、动态/Between 展开身份及 `XmlLoaded` 生命周期。当前 main 的 `WorkflowLocalVariablesWindow` 仅从 `WorkflowConfiguration` 的 descriptor/未连接 input 构造兼容候选；它没有原 `GetParamRoot → AlgorithmTabRoot → GetRunParamsTree` 的模块根、代理身份或动态链接消费入口。
- B01.2 现有窗口仍只按 `DataKind`/占用关系提供局部候选和确认事务。原源码要求 `DisplayType`、`ISubInfo`/`SubscribeVisibleA`、`ComType`、`ModuleID+Name` 身份、首次全类型缓存与变量/普通上限分支；`WorkflowSourceOption`/`ParameterDescriptor` 没有这些生产语义。FCR-019/FCR-021 未出现新的已提交 producer，因此增加缓存或字段会是主线自造第二套插件配置模型。

## 证据与边界

原源码依据仍为 `RunParamsSelectData.cs`（首次/缓存候选与确认）、`RunParamsBaseNode.cs`（`ModuleID`、`ISubInfo`、`ComType`）、`AlgorithmTabRoot.cs`/`NameValueItem.cs`（装配与 `XmlLoaded`）以及 `GlobalVariableItem.cs`/`GlobalVarExtern.cs`（取消前旧快照和绑定写入）。这些路径已在 B01 batch1–5、B16、B21 记录，当前复核只核对 main 文件和合同索引未发生 producer 交付。

当前 main 的 `WorkflowLocalVariablesWindow` 与 `WorkflowVariableTargetsWindow` 是可用的独立兼容实现；它们不应被标为原 VM 运行 Tab。未发现可在 B 线独占范围内安全实施并达到完整门槛的改动，故不添加产品补丁、不新增镜像测试。

## 解除条件

需要用户选定并提交 FCR-019 的实际运行 Tab 元数据/生命周期 producer（含稳定来源与模块身份），以及 FCR-021 的 Group 候选资格/CloneSource/展示能力 producer。收到后由 main 消费端单独实施并自审；未收到前不重派 B01。

审计哈希见 `analysis/full-audit/batch23-20261020/B/runtime-tab-gate-hashes.json`。tool-plugins 未访问、未修改。
