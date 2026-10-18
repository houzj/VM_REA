# Group 运行参数类别的展开状态

原始 `analysis/decompiled/DynamicObjectConfigControl/Apps.XmlParser.ParamTab/GroupRunParam.cs` 的 `ConfigCommandExecute` 在编辑窗口返回、重建集合后，按类别 ID 恢复 `IsExpanded`。`DynamicRunParamConfigControl.xaml` 将类别作为树节点显示；`Apps.Style/treeviewstyle.xaml` 将默认树节点展开，`IsSelected` 与展开状态分离。当前 WPF 配置窗口每次 `Rebuild` 强制展开全部类别，折叠后改名或增删类别会使原类别重新展开。

现按稳定类别 ID 记录窗口内展开状态，重建时恢复；新类别默认展开。窗口状态不写入方案，不改变类别、代理及运行数据。专项 `VisionEditor.GroupCategoryExpansionChecks` 在真实 WPF 窗口折叠类别并改名、新增类别，删除另一类别后重建；八项检查覆盖展开状态、实际 Group 输出 7 与保存重开后的类别/运行数据。删除另一类别通过配置回调触发重建，未模拟原生确认弹窗的桌面鼠标操作。

此处只闭合展开状态这一独立差异；类别树的完整原生视觉和右栏工具装饰器仍未验收，后者已登记 FCR-016。
