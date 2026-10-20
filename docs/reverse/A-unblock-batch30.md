# 第三十轮 A 线解除条件核查

- 基线：`f18afa7`（第二十八轮问题空间审查提交）。
- 范围：A06.1/A06.2 运行界面编辑与 BAML；A05.1/A05.3 全局服务与壳入口。
- 结论：未发现新的原始源码、资源语义或运行合同可解除既有阻塞；本轮实施增量 `+0`。

## A06 运行界面编辑

沿用并复核 `docs/MF01-FRONTEND-UI-SOURCE-AUDIT.md` 的 `FrontendUI.Design.dll` 身份与 CIL：`EditFrontendUI → EditFrontendWindow(DisplayMode=1) → FrontendEdit.Close → DoSave`。已有 `editfrontendwindow.baml` 与 `frontendedit.baml` 字节哈希仍只能证明资源存在；`frontendedit.baml` 安全记录解析在 `0x1929` 的未知记录处停止。当前 main 仍没有原始 `DesignSurface`、`DocumentManager` 三缓冲（temp/solution/designer）、MD5/NeedSave 或运行值消费合同。未把空窗口、外层记录或通用 XamlReader 当作实现。

解除条件仍为：取得可验证 BAML record/长度规则和保存消费链，或明确的 main-owned 设计文档/快照合同及事件消费者。

## A05 壳服务

复核 `docs/reverse/A05-full-menu-map-batch17.md`、`A05-services-gate-batch25.md` 与 `A05-shell-close-batch26.md`：全局变量、通信、脚本、触发器、综合配置仍停在外部服务/native 模块入口；相机仅有 partial backend。原 `GlobalVariableModuleTool`/`VmSolution` 证据给出动态 IO、服务回调与原生恢复路径，但没有 main 可消费的 producer→session→execute/callback→error→close 合同。补空 handler、禁用菜单或独立三标量模型均不满足源码级实施门槛。

## 审计

- 未修改产品源码、测试或其他工作树；`tool-plugins` 未访问。
- 保留以上原始证据路径及既有失败记录；未重扫安装原件或运行外部服务。
- 解除条件未满足，A06/A05 不计实施完成。

**本线实施增量：+0。总进度由主线按固定 96 分母汇总。**
