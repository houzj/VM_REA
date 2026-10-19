# 第十二批 A03.3：显示容器缩放持久化与非活动页历史

基线 `f53aa7b`。本批在 main 的独立方案格式落实每 Procedure/Group 的离散缩放状态；没有访问 tool-plugins，没有运行原厂构造器、SDK或设备。构建及运行由根线串行执行，实际结果以本批总报告和独占审计目录为准。

## 原始依据与适用边界

- `analysis/decompiled/VMControls.WPF/VMControls.WPF.DataForUI/VmModuleSetDecorator.cs`：`ProceZoomIndex` 默认字段9，从所属模型 UserData 同名键读取；setter 更新该容器状态，并通知 `ProceZoomIndex` 和 `PartentZoomIndex`。显式接口 `IZoomIndex.PartentZoomIndex` 转发该属性。
- `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/StatusBar.xaml.cs`：`_ZoomTicks` 为0.1至2.0、间隔0.1的20档；模型索引9对应100%。`DiagramArea.xaml` 将当前显示容器接口以 TwoWay 绑定到缩放控件，不应由另一流程的会话视口覆盖。
- 第三、四批 CIL 审计见 `A03-zoom-history-batch3.md`、`A03-binding-batch4.md`：模型按完整模块身份恢复 UserData，流程装饰器对存活非活动Tab保留各自显示容器；不能改为所有Tab共用单一缩放。
- 同版 WPF 载体 SHA256 `99ffa401c8784472913ad43813d68a8a5577890c00e149506e3e5664fa61c97d`，MVID `d144317f-3992-4275-9b0f-13fb6163e07d`；来源身份及原CIL哈希沿用上述报告。使用 REA 技能复用现有证据，未重复已知资源预算失败调用。

## 已实施数据流

根线串行增加 `ProcedureDocument.CanvasZoomIndex=9`、`SolutionStore` 的0..19校验，以及 `GraphEditorDocument.SetContainerZoomIndex(procedureId,index)`。本线 `WorkflowCanvas` 将实际缩放操作和 Fit 映射为 `(index+1)/10d` 并写入该 API；每次渲染读取当前模型索引，因此同一个显示容器的历史恢复也重新投影。原来的 `canvasViews` 仅保留滚动/平移与方案overview会话视口，不能覆盖流程/Group的持久化缩放。

现有文档 snapshot 自然包含新字段，能在其他Tab执行历史后保全或恢复未活动Group的视图数据。不新增每Tab历史栈，也不擅自将每次缩放创建为独立撤销根。相同索引无操作；非活动容器写入不改变当前选择，不递增结构手势Revision或清空redo。既有文档缺少字段时默认为9。新增字段是本项目 JSON 合同，**不是原生 UserData 或 .sol 字节编码**。

## 自审核

`tests/ContainerZoomChecks` 针对实际 `MainWindow` 与文档执行：根/嵌套Group/其他Tab独立值、真实缩放写回、首次及同页渲染、保存重开、旧格式缺字段、非活动页Undo/Redo、无根历史、不抢选择、保持redo/结构Revision、非法索引、Fit和新方案重置。运行输出保存在 `analysis/full-audit/batch12-20261019/A/`。本线不并行构建；根线保全实际成功与失败日志后决定实施单元完成状态。

## 保留未知

原模型 Undo 仅转发 `ProceZoomIndex`，而显式接口绑定读取 `PartentZoomIndex`，真实原WPF同源通知是否刷新仍未验证；原 root/part 对单独缩放的完整准入也未证明。本实现以现有独立 snapshot 合同承载状态，不将此合同冒称原厂历史服务等价。原厂日志初始化失败与SDK/native格式、原机桌面差分继续单列，人工验收按用户要求后置，不据此抹去已交付的本地实施。

## 本批实际结果

根线串行实际结果：主项目构建exit0；首轮测试因缺System.IO引用编译失败，修正显式using后重跑exit0，29/29通过。实际日志A/wpf-checks.log与A/wpf-checks-retry1.log全部保留，原导航12/12回归通过。
