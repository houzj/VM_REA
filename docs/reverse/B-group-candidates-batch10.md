# 第十批 B：Group 参数候选搜索与重复写入实施

基线 `877671f`。范围仅 main 自有 `GroupRunConfigurationWindow`；不访问或修改 tool-plugins，不修改插件描述符、Contracts、配置模板、执行器。遵循 REA 技能，复用本地已反编译原始成员，不将本次产品测试称作原厂运行。

## 原始链与实际改动

`DynamicObjectConfigControl.UI.Data.RunParamPickData.Init` → `RunParamPickDataExtern.GetInsideRunParamTree` → `GetRunParamTreeNode` → `ExpendHolder.EnumEntrysA`：候选取参数 `MultiLanguageDisplayNameA.Contains(searchWord)`，没有把模块名拼进搜索字符串，也没有忽略大小写。`ParamTypeList` 的类别通过原生类型名列表筛选，绝非按托管 DataKind 推断。

已修正主窗口 `BuildTree`：现有托管兼容候选使用 `option.Name.Contains(search.Text, StringComparison.Ordinal)`；输入模块名不再把模块下所有参数误搜出来；`Gain` 与 `gain` 分开匹配。当前 descriptor 没有原参数本地化显示名，因此 **option.Name 仍是独立兼容显示名，未声称完整本地化等价**。

`ExpendHolder.EnumEntrysA` 经 `ParamConfigDataBase.CheckParam` 及 CheckedParams 恢复已选包装；`GroupRunParamConfigData.CheckCommandExecute` 分派新增/移除，再 `AddParam` 或 `DeleteParam` 写原配置。主线叶勾选事件此前无当前配置身份检查；已修正为新增前检查 `(NodeId, Parameter, IsInput)`，重复已选事件不新增代理、不改现存 ID。此幂等防护是主线重建安全实现，不冒称原程序具有相同防护指令。

## 自审核及验证

新增 `tests/GroupCandidateImplementationChecks`，12 个 WPF 控件检查覆盖模块名搜索排除、大小写、空搜索、真实候选勾选写回、旧控件重复事件、精确移除、保存拒绝与重试、重建已选状态。fixture 不执行工具；离屏窗口仅用于实际控件事件自检，不计用户桌面或原厂差分。根线已串行构建并运行本项目：退出码 0，12/12 检查通过，日志 `analysis/full-audit/batch10-20261019/B/check.log`。保留已有 `MainWindow.xaml.cs:123` 的 CS8603 警告，本片未修改该文件。实际产品搜索/幂等修改自审核通过，不扩大到原生候选资格完整。

自审核：只改一行搜索和一处新增前身份防护；未重定义类别、代理关联格式或执行消费；保存失败仍由已有 Change catch 显示并 Rebuild；旧代理保持其 ID。原件反编译文本 SHA256 在 `analysis/full-audit/batch10-20261019/B/source-hashes.json`。

## 无法据此关闭的边界

B06.1 保持 implementation=partial。FCR-021 所需 `IGroupRunParam`、`CustomVisibleA`、`GroupRunParamEnable`、`AccessMode`、`DisplayForGroupType`、原生类型名、本地化名及展示工厂在主线当前 ParameterDescriptor 中不存在。此处不猜补布尔值，也不新增第二套插件配置模型。最小 seam 仍是插件描述符物化后提供可选资格快照，由 Group 候选消费者按已证谓词处理；需用户选定共享接口合流再实施。

继续读原始 `GroupRunParamConfigData.DeleteParam` 和 UniquePath 后，不能证明全部多目标关联都应在左树按占用勾选；本次 **没有** 把 `Relations` 擅自当作主来源占用。原原生 SDK 路径身份、CloneSource 编辑和多字段断开/恢复次序亦不由本次推断。

因此本批有真实产品落地，但此单元未完整：96 单元实施分子增量为 0。人工验收后置不是上述缺实现边界的替代原因。
