# C01 启动链崩溃回归门禁

日期：2026-11-12

## 根因

本机 `VisionEditor/startup-error.log` 给出了启动后退出的可复核异常：

`KeyNotFoundException: The given key 'source' was not present in the dictionary.`

调用路径是旧执行端口的 `MouseLeave` 回调 → `CreateExecutionPort` → `nodeCards[node.Id]`。`RenderGraph` 已清空并重建 `nodeCards` 后，WPF 仍可能向已经脱离可视树的旧端口投递 `MouseLeave`，因此启动阶段的输入同步会把进程带入失败出口。

## 修复

`src/VisionEditor/WorkflowCanvas.cs` 的端口离开处理改为 `TryGetValue`：旧端口没有当前卡片时视为未悬停并收起，不再用字典索引抛异常。这个修复只覆盖已观察到的生命周期异常，不改变连接拓扑或端口语义。

## 验证

- `VisionEditor.StartupShellChecks`：通过，包含 detached execution-port MouseLeave 回归项、欢迎页、方案中心、主窗口、方案实体和关闭链测试。
- 同一测试还验证命令行方案在 `MainWindow` 构造期间进入真实 `GraphEditorDocument`，保留方案 ID、`main` 流程、clean 状态和规范化文件路径。
- `dotnet build src/VisionEditor/VisionEditor.csproj -c Release --no-restore`：0 警告、0 错误。

原厂同输入启动、授权服务和人工桌面验收仍单独标记“运行未确认”。
