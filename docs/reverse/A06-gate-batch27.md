# 第二十七轮 A06.1/A06.2 运行界面编辑与布局门槛

- 基线：`fbcf222f08505555f3a06db9fa116cc21e4fd90e`（第二十六轮本地提交）
- 范围：A06.1 EditFrontendUI 运行界面编辑入口/关闭链；A06.2 FrontendEdit BAML 布局与显示消费。
- 结论：本轮实施增量 `+0`，没有把窗口占位、截图或资源色表计为实施。

## 复核证据

`docs/MF01-FRONTEND-UI-SOURCE-AUDIT.md` 已固定原始安装程序集 `FrontendUI.Design.dll`（SHA-256 `60c0c7b1f404ad8bab4504b9b928148f84c7694a335331c4e4340b7c8017cdf5`、MVID `0de25f5a-f9cd-4617-9954-2ea69f747be1`），并由定向 CIL 确认：`TitleMenueItemData.EditFrontendUI` 创建 `EditFrontendWindow`、设 Owner、模态显示；构造设置 `FrontendWindowData.DisplayMode=1`，绑定 `VmSolution.IsRunning`/`ContinuousRunEnable` 到 `IsBusy`；`OnClosing` 调 `FrontendEdit.Close(true)`，按 `DoSave` 结果决定取消或解绑 VM 事件。`FrontendEdit.Close` 的 DesignSurface、ToolBox、PropertyGrid 释放和 `DoSave` 的保存/重置分支也有 CIL 证据。

现有资源只确认 `editfrontendwindow.baml` payload 3466 字节、`frontendedit.baml` payload 25751 字节及哈希，且此前安全解析在偏移 `0x1929` 遇 `Unsupported record 0`。通用 WPF BAML reader 可能执行 `MarkupExtension.ProvideValue`，因此不能将未知记录直接送入 `XamlReader` 或凭控件字段还原视觉树。

## 未关闭缺口

1. A06.1 的真实 `DocumentManager` 三缓冲（temp/solution/designer）、MD5/NeedSave、保存重开和运行值追踪消费仍缺 main 侧可写合同；现有 `MainWindow` 没有原始设计面消费者。
2. A06.2 缺少只解码 records、禁止类型构造/转换器/MarkupExtension 的安全 BAML parser；控件尺寸、布局、样式、命令绑定和显示结果生产/消费未闭合。
3. 原始 FrontendUI.Design 依赖的 `DesignSurface`、`FrontendWindowData` 外部服务与 native `.sol`/运行值路径未取得完整调用约定；main 不能实现独立替代并宣称原机等价。

解除条件：取得可验证的 BAML record/长度规则和配置保存/消费链，或取得明确的 main-owned managed 缓冲/显示合同及原始事件消费者。届时先实现最小字段/生命周期链，再单独验证 UI 与原机差异。

## 结论

本轮无安全可落地源码级改动；不修改产品源码，不触及 `tool-plugins`，实施总进度保持 `41/96 = 42.71%`，剩余 `55`。
