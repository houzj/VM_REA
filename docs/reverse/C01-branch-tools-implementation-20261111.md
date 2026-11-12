# 分支模块与分支字符串工具交付核对

分支模块和分支字符串已经有独立的源码级退出报告，本轮复核了它们能直接接入当前主界面和通用图执行器。分支模块覆盖整数值/位模式、后继选择、结果显示、配置表、拓扑变化、复制、保存重开和 32 条连接限制。分支字符串覆盖精确文本匹配、UTF-8 容量、空字符串、调试标志、表格复制和保存重开。

无界面复核结果如下。

- `BranchModule.PluginChecks` 通过 44 项。
- `BranchString.PluginChecks` 通过 56 项。

汇总位于 [branch host regression](../../analysis/full-audit/branch-host/run-04daf380f40644d99ba6139174502f26/summary.json)。原厂 native DLL 没有在本轮执行。现有退出报告中的 NumericId、native heap 和原厂全域边界继续有效。

这两个工具加入全量工具状态台账的“已有专项自审”数量。162 个目录工具中，当前已有 12 个工具具备独立的完整范围自审记录，剩余目录仍需要逐个恢复和实施。
