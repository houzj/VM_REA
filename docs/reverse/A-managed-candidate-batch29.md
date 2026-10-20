# 第二十九轮 A 线：A02/A05 托管候选筛选

## 基线与范围

基线沿用第二十八轮提交 `f18afa7`（当前工作区可能包含本轮其他线报告，未修改其文件）。本线只筛选 A02 主窗口状态/DPI 与 A05 菜单、工具栏、服务入口中可在 main 独立闭合的托管实施单元；不进行人工桌面验收，不访问或修改 `tool-plugins`。

## 源码级核对

1. `src/VisionEditor/WindowWorkArea.cs` 的 `OnSourceInitialized` 挂接 `WM_GETMINMAXINFO`，按 `MonitorFromWindow`/`GetMonitorInfo` 将工作区物理像素边界写入 `WindowLimits.MaxPosition/MaxSize`；这条几何状态链已经存在，A02 不产生新增实施。
2. `src/VisionEditor/MainWindow.ShellWindowState.cs` 已将 `WindowState` 映射到最大化/还原 Tooltip 与资源图标；同样属于既有实现，不能重复计分。剩余多屏/DPI 同状态矩阵是后置人工/原机验收，不构成新的托管闭环。
3. A05 已有文件、历史、帮助、运行和工作区锁等真实入口消费（见 `docs/reverse/A05-shell-close-batch26.md`）。剩余全局变量、通信、触发、脚本、综合配置、采集/权限等入口仍在 `TitleMenueItemData` 到外部服务或原生模块会话处断开；缺少 producer→session→execute/callback→error→close 合同。添加空 handler、禁用按钮或窗口不能形成状态、持久化和执行闭环。

## 结论

本轮没有发现可以由现有原始源码证据独立闭合的 A02/A05 实施单元，**实施增量 +0**。不修改产品源码，不重复已关闭 A02 能力，不伪造 A05 服务后端。

解除条件：A02 需要新的原绑定/几何状态消费证据；A05 需要具体服务 producer、会话、执行/回调、错误和关闭释放链，或用户明确选择独立兼容合同后再实施。`tool-plugins` 未访问、未修改。
