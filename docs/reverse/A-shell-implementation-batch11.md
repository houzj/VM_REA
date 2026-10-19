# 第十一批 A：最近方案持久化失败后的壳保存结果

基线 `b894e8c`。本片修改主框架最近方案生命周期，不涉及工具配置、原 VM SDK 或 `tool-plugins`。范围是已成功保存方案之后，最近方案偏好写失败不应使壳保存命令失败。

## 原始源码与差异

- 原反编译源码 `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.Project/ProjectManager.cs`，`GetRecentlySolutions`、`UpdateRecentlySolutions`、`RemoveRecentSolution`、`SaveRecentlySolutions`。完整源码 SHA256：`1AE41A0841DD65F5DFA7461078C370822E61826877C42E299939AF1FD47F9370`。
- `UpdateRecentlySolutions` 先修改 `_recentlySolutions`，调用 `SaveRecentlySolutions`，再 `Program.MainData.RefreshRecentSolution`；`SaveRecentlySolutions` 在其内部捕获异常并写日志。删除路径也先改内存再保存/刷新。原始保存偏好失败不会回滚最近列表，也不会把异常传给方案保存调用者。
- 同目录 `SolutionItemInfo.cs` 的 `GeTimeSpan` 对时间差调用 `Duration()`；上游清理条件为 `.Days > 99`。完整源码 SHA256：`35C37E2DCE6B6F6AEF2EAD51C3DA56552A5877C23A6CFEBA62C9EA7AB2B58A87`。因此远未来时间同样会过期。
- 修改前 `MainWindow.Save` 已成功调用 `doc.Save`，随后 `RecentSolutionStore.Update` 直接写偏好文件；此处 I/O 异常向上泄漏，壳报告失败且不刷新菜单。每次读取都来自磁盘，不能保持原始源码中的已发布内存状态。未来 100 天以上条目也因负时间差一直保留。

这里复用既有本地原反编译源码，未新建 REA Evidence ID，也未执行原程序集。原代码路径/方法为证据；截图和常规 UI 设计不作为依据。

## 实施

独占产品文件 `src/VisionEditor/MainWindow.ShellRecentSolutions.cs`：

1. 最近列表首次读取后保留内存快照，独立偏好路径变更时清空快照，避免测试/用户路径之间泄漏。
2. 更新、删除先发布内存列表，再持久化。仅捕获 `IOException` 与 `UnauthorizedAccessException`，使用 `Trace.TraceWarning` 记录；程序错误继续传播。
3. 已有快照不再因磁盘失败被重新读取覆盖；首次读取失败保留可重试状态。
4. 保留最近顺序、六项上限、空更新不动作。时间过期按原始 `Duration().Days <= 99` 对齐。

目录与 JSON 持久化仍是独立兼容实现；没有复制 VM `.cfg`，当前 Windows 路径规范化/忽略大小写规则沿用既有实现，未宣称原配置格式或所有异常行为相同。自有实现在存储路径切换时会重新读，不把该测试接缝视作 VM 行为。

## 自审核和可复验检查

`tests/ShellImplementationChecks11` 包含真实存储失败与恢复，并通过反射进入实际壳 `Save` 路径：真实 `doc.Save` 写方案成功后，最近偏好父目录为普通文件，确认保存返回成功、文档路径及 clean 状态、内存最近记录和实际菜单仍更新；取消另存不改方案字节。测试偏好全部重定向到本批 GUID 目录，既不写用户偏好也不启动原 VM。窗口仅构造供逻辑检查，不作人工或用户桌面验收。

主智能串行构建并运行退出码 0，16 项断言全部通过；本子线未另行构建。日志为 `analysis/full-audit/batch11-20261019/A/check.log`，实际结果为 `analysis/full-audit/batch11-20261019/A/run-c7a05c780fc6406ca10d18af1befa88f/summary.json`。日志保留既有 `MainWindow.xaml.cs` CS8603 警告，不声称零警告。源码输入、补丁及运行证据哈希保留在本批 A 审计目录。本片只闭合有限最近列表失败路径，A05.2 原已完成，不额外计总进度，也不把 A05 全包或原生格式标为完成。
