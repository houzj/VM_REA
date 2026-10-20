# 第二十九轮 B 线：局部变量/流程 IO 实施候选筛选

基线：当前 main（以本轮协调台账为准）。本线只审查 B05 局部变量与流程 IO，不访问或修改 `tool-plugins`。

## 源码证据

- `src/VisionEditor/WorkflowLocalVariablesWindow.xaml.cs` 的 `Commit`、`TypeChanged`、`BindSource`、`BindTargets`、`DeleteVariable`、`AddVariable` 及分组操作已经形成托管闭环：配置读写、变量类型、来源绑定、目标订阅、删除清理和数量上限均有明确调用链。
- `src/VisionFramework/WorkflowSubscriptions.cs` 通过 `WorkflowSourceKind.LocalVariable` 解析变量类型；缺失变量明确抛出 `InvalidDataException`，不是静默降级。
- `src/VisionEditor/GroupInputRunControls.cs` 和 `WorkflowConfigurationWindow.xaml.cs` 已将局部变量作为 Group 输入候选，并按 `DataKind`/标量基数过滤。
- 既有检查 `tests/VisionEditor.GroupVariableTypeChecks`、`GroupVariableBindingChecks`、`VisionWorkflow.DataChecks`、`VisionFramework.ArrayChecks` 已覆盖类型切换清理、来源绑定、运行时读写和数组-标量拒绝。

## 候选判定

本轮没有新的可独立闭合实施单元。局部变量托管路径已实施；增加更复杂类型、动态 IO、全局变量或原生 gvar 导入导出会跨越 `GlobalVariableModuleTool` 的动态 IO、原生参数订阅引用计数和 `.gvar/.sol` 持久化未知边界，不能在 main 中凭意图补齐，也不能复制插件框架。

结论：**实施 +0；B05 保持部分完成/原生阻塞**。未修改产品源码、测试或 `tool-plugins`。

## 解除条件

需要原始动态 IO/变量类型合同、订阅引用计数释放、全局变量模块执行读写以及 gvar/sol 编码与恢复证据；取得这些证据后再拆分可实施单元。
