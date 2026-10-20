# C04 候选筛选（第二十九轮）

基线：第二十八轮提交 `f18afa7`。本轮只读审查，tool-plugins 未访问。

## 判定

检查 C04.1、C04.2、C04.3 是否存在不依赖原生 `.sol`/`.gro` 业务载荷、且可在 main 独立闭合的托管实施单元。结论：**无新增可计分实施**。

- C04.2 历史临时文件所有权已在第十八轮完成：`HistoryPayloadStore` 与实际 `GraphEditorDocument` 历史 root/part 已覆盖大文档文件化、Undo/Redo 读取、淘汰、清 redo、New/Load、替换与关闭释放，本轮不重复实现。
- C04.1 根类型导入/导出与原始临时 `.gro`/`.sol` 载荷仍依赖 SDK writer/reader。`docs/UNDO-ROUND2-20261019.md` 记录 `ModuleUnRedoOperation`、`SolutionUnRedoOperation` 和 `RecoverStack`；`RecoverStack` 仅移动栈，不回放业务数据，密码取消由 SDK 验证回调决定。main 没有原始载荷格式和回调合同。
- C04.3 的密码取消、方案/流程载荷回放和原生临时文件所有权没有新的源码或受控实例证据。现有 JSON 历史租约是 main 的安全独立适配，不能扩展成通用 SDK 回滚。

因此不改 `src/`，不新增猜测式回滚或格式转换；无测试变更，无产品提交。

## 证据与解除条件

`analysis/decompiled/VM.Core/VM.Core/VmSolution.cs` 的密码检查、Import/Export、Group 回调和 native 同步等待均位于 SDK 边界；`docs/reverse/C-history-payload-batch18.md` 已确认 C04.2 完成并保留 SDK 导入导出/密码恢复开放；`docs/reverse/C-problem-space-batch28.md` 的解除条件本轮未变化。需取得原始 Import/Export/RecoverStack 密码取消的受控调用链及载荷/文件所有权证据，或用户授权独立格式合同。

实施增量：0。总实施进度：41/96 = 42.71%，剩余55。未进行人工桌面、原 SDK、设备或密码输入验收；这些不计实施进度。tool-plugins 保持不动。
