# 参数窗口期间的画布节点交互（MF-02）

## 原生证据

- `analysis/decompiled/VMControls.WPF/base/DiagramItem.xaml.cs` 的 `OnPreviewMouseLeftButtonDown`（约 471–484 行）：`IsOpenParams` 时单击直接标记 `Handled`，不进入节点选中及拖动分支；双击另有处理。
- `analysis/decompiled/VMControls.WPF/base/diagramitemtemplate.xaml`（约 1856–1858 行）：`IsOpenParams=True` 时 `ContextMenu=null`。
- 此状态由 `DiagramArea` / `ProcedureConfigControl` 传播给节点。本项目已有 `parameterEditorOpen`，此前只限制编辑提交，未限制节点的单击选中与菜单。

## 实现与验收

本次在 WPF 画布节点鼠标入口消费参数窗口打开期间的单击，并在渲染节点时不挂接右键菜单；窗口关闭并重绘后恢复。未改 Group 与插件配置框架。

`VisionEditor.NodeParameterWindowChecks` 打开真实非模态流程配置窗口和 Group 配置窗口，以 WPF routed mouse 事件检查节点单击、右键菜单、状态恢复、修订号及保存重开。专项目测通过，截图 `analysis/full-audit/node-parameter-window-open.png` 和 `node-group-parameter-window-open.png` 已目检画布节点状态。截图工具只在测试项目。

## 边界

测试使用真实 WPF 窗口和 routed event 注入；物理桌面鼠标和原生 VM 的逐像素差分尚未验证。原生右键是否同时修改节点选择状态未由上述源码确定，因此本次只处理菜单是否存在，不改变现有右键选中逻辑。双击打开行为保持既有实现，未声称完全一致。
