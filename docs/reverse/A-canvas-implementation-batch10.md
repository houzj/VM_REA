# A线第十批：流程Tab保留当前显示Group的产品实施

## 源码依据与实际差异

复用第二/四批已确认原程序集 `VMControls.WPF.dll`，SHA256 `99ffa401c8784472913ad43813d68a8a5577890c00e149506e3e5664fa61c97d`、MVID `d144317f-3992-4275-9b0f-13fb6163e07d`。本批不重新运行原类，不重复累计反编译方法数。

- `analysis/decompiled/VMControls.WPF/VMControls.WPF.DataForUI/VmProcedureDecorator.cs` 的 `DisplayModuleSet`、`OpenGroupCommandExecute`、`UpCommandExecute`：所属流程装饰器独立保有当前显示模型；进入Group替换显示模型，返回使用显示模型的Parent。
- `analysis/decompiled/VMControls.WPF/VMControls.WPF/ProcedureConfigControl.xaml` 的Tab模板：显示内容绑定 `DisplayModuleSet`，而不是始终绑定顶层流程本体。
- 精确原CIL及身份沿用 `docs/reverse/A03-tabs-batch2.md`、`docs/reverse/A03-binding-batch4.md`；`set_DisplayModuleSet` token `06000A14`，IL SHA256 `5b11429f3f87a3b84e322a896678943e03c8e1c899a1c4ed9f8de9b0a09de181`。
- main原 `WorkflowTabs.SelectWorkflow` 切回流程时直接选择根ID，非活动Tab标题使用根模型，导致 A→B→A 丢失A所显示的Group。该差异已有源码合同，直接实施。

## 实施

`GroupCanvasNavigation.cs` 增加每顶层流程的当前显示容器会话映射：保存当前有效容器，解析时校验存在性和所属根，显式Group目标保持原样。`WorkflowTabs.cs` 由根线串行接入切Tab和非活动标题；现有返回父级流程不变，刷新时自然更新当前显示容器。`WorkflowCanvas.cs` 在已有 `SolutionReplaced` 回调清映射，避免新方案复用main ID泄漏旧Group身份。

删除容器后回退有效根以及方案替换清理是本地文档生命周期保护；没有把这些保护的所有细节冒称原厂运行观测。不引入新的持久格式、缩放存储、历史栈或插件框架。

## 自审核与验证

`tests/CanvasImplementationChecks` 对实际MainWindow和GraphEditorDocument调用执行导航检查，覆盖两层Group、A→B→A、返回后记忆更新、删除回退、方案替换同ID、显式Group目标。由根线串行构建运行，结果写入本批审计目录；提交前以实际执行日志为准。测试不Show窗口，不构成人工桌面验收或原厂差分。

首轮根线完整产品构建成功，保留既有 `MainWindow.xaml.cs:123 CS8603` 警告；导航检查 **10/10通过**，日志另存 `analysis/full-audit/batch10-20261019/A/check-first-10.log`。根线自审核另补工作流列表显式节点导航：节点定位先选其所属流程根，避免误用Tab记忆进入旧Group。新增所属根与节点selection两项后，最终串行运行 **12/12通过，exit 0**，实际日志 `analysis/full-audit/batch10-20261019/A/check.log`。本片自审核通过，不据此宣称人工桌面验收或原厂运行等价。

本片修复既定A03.1/A03.2范围内的实际缺口；两单元原已实施done，不重复计分。A03.3缩放持久化/非活动历史仍开放，不以会话导航修复冒称闭合。

审计：`analysis/full-audit/batch10-20261019/A/`。无tool-plugins访问或修改，无原厂SDK/设备执行，无自动提交或推送。
