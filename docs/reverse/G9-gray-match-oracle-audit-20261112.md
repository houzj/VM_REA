# G9 GrayMatch 原件与 Oracle 审计

日期：2026-11-12

本轮串行复跑：

- `GrayMatch.ModelSlotsChecks`：125 项通过；
- `GrayMatch.SourcePolicyChecks`：13 项通过；
- `DotChecks`、`ScoreArithmeticChecks`、`SmallTemplateChecks`、`TrainingAngleChecks`、`TrainingPyramidChecks` 均在入口拒绝，分别要求 exact native oracle 文件。

这些失败是证据前置条件缺失，不是产品算法失败。缺失 oracle 时不运行独立替代实现，也不把已有托管检查提升为 native parity；相关工具状态保持“运行未确认”。
