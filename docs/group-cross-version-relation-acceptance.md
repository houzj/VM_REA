# Group 运行参数跨版本关联验收

原始依据：`analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/VmModuleExtern.cs` 的 `GetParamsOfEqualTypeTree`/`GetParamsNode` 先按 `StrModuleName` 查相同工具类型，再按参数名 `FindBaseNodeByName` 取候选；该筛选没有模块版本号条件。`ModulesPickData.Init` 将这棵树用于运行参数关联选择。原始代码没有证明不同版本的算法一定相容，因此仍须逐参数验证类型与编辑器契约。

当前差异与修复：Group 关联选择器原来额外要求 `ModuleVersion` 相同，导致同一工具类型跨版本、参数契约一致的目标在 UI 中不可见。本轮移除这一额外条件，保留同一 `ModuleId`、同名参数、输入基数及参数类型/编辑器/ROI 能力的既有检查；运行时原有关系验证继续保留。

专项 `VisionEditor.GroupCrossVersionRelationChecks` 的真实 WPF 窗口验证 6 项通过：版本 1 的参数 `gain=7` 与版本 2 的同名参数 `gain=1` 在关联选择器中连接；关联未同步前真实 Group 输出为 1，点击同步后版本 2 输出为 7；撤销恢复 1，重做恢复 7；保存重开后关系和实际输出均为 7。该测试覆盖本项目两版本同工具的兼容参数，不宣称所有原生跨版本插件互相兼容，也未做真实 VM 动态差分。
