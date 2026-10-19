# A03.3 独立实施预审与测试矩阵（C线，第十二批）

审查对象：根线拟将 `ProcedureDocument.CanvasZoomIndex`（缺省9）接入当前容器缩放、保存及文档历史。本文是静态预审，测试矩阵不是已通过结果；根线串行构建与记录实际执行。未访问 tool-plugins。

## 当前数据流与风险

`GraphEditorDocument.Copy`、`Rebase`、Group 克隆使用 record `with`，新增标量字段自然保留。`Capture`复制完整文档，`Restore`先重定位再把当前状态压入另一栈，故缩放自然随相应文档快照恢复，不需要另造缩放历史栈。`Save`经 PrepareSave、Rebase、SolutionStore.Save 后才提交编辑器状态，应验证实际 PrepareSave 不丢字段。

setter 不应走 `Edit` 或 `RecordEditHistory`，也不应清 redo，否则单纯缩放制造结构撤销根或丢失未来状态。应检查相同值 no-op、无效范围和不存在流程失败前不修改状态；持久化值更改应置 dirty，以进入关闭保存提示。Changed 是否触发须与宿主同步刷新一致。

`Revision` 当前被 EditProcedure 草稿判断和实际鼠标/连线手势边界消费。若缩放也增加 Revision，草稿中单纯缩放可能升级成一次结构 Edit，且原地缩放取消既有手势；应显式选择并测试。若将其定义为结构版本，则缩放只置 dirty/通知而不增 Revision 较合适，但这需根线在实施合同说明。

宿主 RenderGraph 必须从实际容器字段读取缩放，即使 CurrentProcedureId 未变化；只依赖 canvasViews 会在 Undo/非活动Tab恢复后显示陈旧值。Overview 是独立会话视图，不应把其缩放写入当前流程。Slider 在程序性恢复期间需 syncingViewport 抑制回写；Load 首次 Render 不可由旧值覆盖持久值。

## 可复验矩阵

1. 原格式 JSON 无字段：缺省9；边界0/19保存并重开；Load 对-1/20结构拒绝。
2. 新建默认9，setter同值无变化；非法值/不存在流程失败且文档、dirty、栈和选中流程保持原状。
3. 写非活动流程，当前选中不变；切回显示目标值；父流程/Group各自独立。
4. zoom→结构Edit→zoom→Undo→Redo：按两个完整文档状态回放字段；单纯zoom没有单独undo项。
5. Undo后zoom：redo保留；Redo能恢复原目的快照值。
6. 相同页Undo及非活动页Undo后再切回，宿主显示均使用恢复字段而非canvasViews旧scale。
7. DuplicateProcedure/Group克隆继承原字段，随后两者独立修改；Prune/Separate/同步名称不重置字段。
8. SaveAs跨基目录、PrepareSave、正常Load保留字段；写失败不提交编辑器路径或dirty变化。
9. 鼠标Ctrl轮、Slider、Fit对当前流程写离散索引；0delta轮无写；Overview缩放不污染流程。
10. Load后首次渲染和Slider程序性恢复不覆盖已保存值；Changed刷新无递归setter循环。

原SDK UserData ProceZoomIndex 的原生文件编码不在本适配中冒称已复刻；持久化载体是当前独立文档格式。人工验收后置，不阻挡实施完成自审核，但本预审自身不代替实际测试与集成。

## 本批实际结果

根线串行实际结果：ViewportDocumentChecks12 exit0，29项全部通过；日志C/core-checks.log及GUID目录保存JSON、summary。此为main独立适配自审核，不运行原SDK。
