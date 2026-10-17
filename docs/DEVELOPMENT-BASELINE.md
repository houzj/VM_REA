# 双项目开发基线核对（2026-10-17）

本轮为文档与只读 Git 核对；没有统一代码、提交、推送、合并或重置。

| 项目 | 当前核对值 |
|---|---|
| 插件工作树/分支 | worktrees/tool-plugins / tool-plugins |
| 插件 HEAD | 1d668772df8788e88fb79f7b94847ca43a22de29 |
| 主线参考提交/本地 origin/main | 351c778b1403acfd3e6a03da5b0b43850208dc87 |
| 共同祖先 | bf52c1305b6558e1ecbcc8f48f8e21f438c1ea97 |
| 相对 HEAD...351c778 | 插件独有14、主线独有22个提交 |
| 本地 origin/tool-plugins | 1d668772df8788e88fb79f7b94847ca43a22de29 |
| origin 地址 | https://gitee.com/houzj_magichan/vm-reverse-engineering.git |
| 远程实查 | git ls-remote 失败：schannel AcquireCredentialsHandle SEC_E_NO_CREDENTIALS；上面 origin/* 只是本地引用，不能断言远端最新 |
| 工作树 | 大量已跟踪修改及未跟踪产品/证据/测试文件，含正在人工验收的候选；未重置 |

## 安全同步时点

现在可同步职责文档、合同设计与状态信息，不能把未提交候选直接当作可 cherry-pick 交付。先保留并由用户验收公共配置基础链路候选；确认后中文提交/推送，再由主线按文件审查 Contracts、类型系统、WorkflowNodeIdentity/Scope、调试后端、ROI 和 WaitTimeMs 等存量差异。主线集成并回归后，记录双方都可解析的统一基线提交；两项目再从该基线独立开发。

不得为“统一基线”强行覆盖当前工作树。351c778 是当前审查参照，不是已在本线合入的基线；共同祖先也不代表全部新能力存在。用户自行决定两对话的推进/同步时点。

## 最新同步授权

用户已明确授权先提交/推送当前在制成果，再同步主框架，暂缓N点UI修复；本轮可先形成中文检查点提交，人工验收状态仍未通过。保留双方在制文件，待主线逐文件集成后记录新统一基线。此前351c778/14与22统计为当时核对快照，提交同步时重新核对。
