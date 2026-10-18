# MF-02 工作流列表节点定位

## 原始依据

- `analysis/decompiled/VMControls.WPF/VMControls.WPF.DataForUI/VmProcedureDecorator.cs`：`SearchModulePopupCommand`、`SearchModuleCommandExecute` 根据路径切换当前模块集，并用模块 `Position` 与 `Source` 创建 `SearchModel`。
- `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/ItemsPage.cs`：`OnSelectedModuleChanged` 从 `SearchModel.Source` 选中目标模块。
- `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/ZoomArea.xaml.cs`：`OnSearchModuleInfoChanged` 将目标位置减去视口半宽/半高，分别增加 50/15，再乘当前 `Scale` 设置滚动偏移；负值先归零。

## 本次独立实现与操作验证

- `WorkflowTabs.cs` 的工作流列表叶节点点击先切换流程和选中节点，再按原始 `ZoomArea` 公式定位。先前的实现把 `X * scale` 与未缩放的半个视口相减，在非 100% 缩放下出现偏移。
- 专项 WPF 主窗口中，点击真实 `TreeViewItem` 叶节点，从另一工作流切换到目标工作流，并恢复该流程的 160% 视口；选中节点且弹层关闭。水平/垂直偏移均与源码公式一致，截图在工作树 `analysis/full-audit/canvas-workflow-list-search.png`，截图工具只属于测试项目。
- 定位没有增加文档修订；紧接着撤销恢复的是之前的节点添加，重做后保存并重开仍保留节点及位置。

## 尚未验证的原生差异

- 此工作流列表是当前独立实现；原生 `SearchModulePopupCommand` 的弹层结构、Group 嵌套路径解析、真实 VM 同状态视觉与鼠标交互尚未逐项比对。这里确认的是节点源/选择/定位链和公式在当前 WPF 界面中的行为，不宣称整个搜索弹层等价。
