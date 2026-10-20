# 第四轮 J04：当前配置输入源显示与实际连线一致

本轮仅修 main 拥有的插件配置宿主显示桥，不修改节点端口、算法或 `worktrees/tool-plugins`。输入来自用户“未连接为空、建立连接显示上游图像输出”的差异信号；实现依据另核原始成员，不把截图文字当合同。

## 原版成员与当前数据流

同版本反编译 `analysis/full-audit/renderer-decompiled/Apps.XmlParser.decompiled.cs` SHA-256 `A476F6266BE680F524A71003CE4A340F66378675637E34AC57721E861C674B49`：`GetSelectedCombinationOperation.Operate`（32558 行起）从目标 `VmIO.SubscriptionInfo` 读取已选关系，空时读 `GetSubscriptionInfo()`，写到配置数据 `CurValueA`；`SetCombinationSourceOperation.Operate`（33071 行起）用所选路径解析 `VmIO` 并设置或清除目标 `RelateIO/RelateIndex`。原厂选择显示由当前 IO 关系驱动；其路径的本地化和实时通知时序仍未由此确定。

当前 `GraphEditorDocument.Connect/Disconnect/Save/Load` 把数据连线保存在 `ProcedureDocument.Connections`；`ToolExecutionScope.ConnectToolEditorServices` 提供 `ToolEditorContext.SubscriptionLabel` 给 ImageBuffer 等共享插件编辑器。修复前该标签只读 `WorkflowConfiguration.Subscriptions`：画布已连接 `ImageSource.Image → ImageBuffer.InputImage`，但没有单独订阅记录时，标签为空。真实执行器会读取 `Connections`，因此配置视图与运行拓扑不同步。`src/VisionEditor/ToolExecutionScope.cs` 现先保留原订阅逻辑；无订阅的输入端口从当前流程的 `Connections` 找源节点和端口，断开后自然为空。参数订阅不读取图连线。

## 可复验状态与限度

新增 `tests/WorkflowInputSourcePresentation.Checks` 在真实 `MainWindow`/`GraphEditorDocument` 及 ImageSource、ImageBuffer 插件上核七态：未连接空；建立连接显示当前源节点及端口（实际构造 `ImageBufferWindow` 并核 `ValueInputImage` 文本）；断开清空；重连恢复；保存重开后新配置上下文及新编辑窗口仍显示；类型化订阅替换画布连线后保留同一源身份；清订阅后空、Undo 后恢复。命令：`dotnet run --project tests/WorkflowInputSourcePresentation.Checks/WorkflowInputSourcePresentation.Checks.csproj -c Release`，7/7 通过；最终日志 `analysis/joint-audit/input-source-presentation/checks.log` SHA-256 `35B255988E524AD52A028E6AE37D7A6CB2B38B2C584952746612A6A8D89D5A37`，自有 VMX 同目录。首构建因探针缺 `System.IO` using 报 CS0103，记录在 `first-build-failure.txt`；补测试源码后通过，产品代码未因该错误再改。自动相机发现关闭，未打开用户桌面窗口或设备。

这是 **confirmed-current** 的 J04 显示/连接一致性修复，原版操作体仅支持“从实际 IO 关系读取已选来源”方向，不证明当前标签字符串与原版 `ModulePathName` 完全一致，也不证明配置窗保持打开时能收到外部连线变化通知。`SubscriptionLabel` 合同未增字段；修复在 main 的宿主桥中，不需要插件线在制文件。完整 J04 仍缺原厂排序、刷新通知、变量/Group 跨层及 `.sol` 重开差分。本切片属既有框架合同修复，不单独增加 96 项实施分子。
