# 第三十轮 B 线：producer/ROI/全局变量合同解除条件核查

基线：`f18afa7`（第二十八轮问题空间审查后 main）。本轮只检查 B07/FCR-018、FCR-021、FCR-023 以及 B05 全局变量/gvar 是否出现可被 main 消费的、版本化且已提交的 producer 交付；不读取、不修改 `tool-plugins`。

## 核对结果

1. `git log --all` 的当前可见提交仍止于 `f18afa7`，没有新的 producer 交付提交、版本化合同提交或交接 SHA。`docs/CONTRACT-REQUESTS.md` 中 FCR-018、FCR-021、FCR-023 仍明确标注请求未实现；FCR-007 要求真实 ROI 算法工具在插件侧声明精确 ROI 能力、坐标语义、字段联动、消费方式和异步会话生命周期，当前没有新的插件侧交付可审。
2. main 的 `VisionPluginContracts` 仍只有既有托管 seam：`ToolEditorContext` 的 `ApplyConfigurationChange`、`PreviewInput`、`ApplyDisplayStyles`、`IWpfToolEditorFactory` 及 `ParameterEditorDefinition.Roi` 等。它们可供已在 main 的独立 ImageBinary 集成使用，但不等价于原生 `PointF(.X/.Y/.R)`、Group 参数装饰器 producer、GlobalVariableModuleTool 动态 IO 或 `.gvar` 往返。
3. `ToolPluginRegistry.PrepareSave` 仅消费已实现的可选 `IToolAssetPersistence`。这条资产快照合同不提供全局变量模块、动态 IO、订阅引用计数、原生结果 payload 或插件 ROI producer 的版本化交付，因此不能据此扩展 B05/B06/B07。
4. 原全局变量证据（`docs/reverse/B-global-module-review-batch13.md`、`B-identity-global-gate-batch25.md`）仍显示 `GlobalVariableModuleTool` 通过动态 IO、`VariableManager` 和原生持久化恢复；main 当前局部变量路径不能替代其方案级全局模块、数组/复合类型、改型清理、执行写回和 `.gvar` 导入导出。

## 结论

本轮 B 线没有解除条件，没有产品源码修改，实施增量 **+0**。不得在 main 复制插件配置框架、伪造 producer 或把现有 ImageBinary 的独立托管 ROI editor 宣称为插件线/native 等价。B07/B05 继续 partial/blocked，待用户选择并收到带 producer SHA、合同版本、payload/lifecycle/compatibility/acceptance evidence 的插件交付，或取得新的原生函数/资源证据后再审。

`tool-plugins` 未访问、未修改；未执行原厂模块、设备或桌面验收。
