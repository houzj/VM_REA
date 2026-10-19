# A03：流程 Tab、Group 显示与缩放持久化（第二批）

2026-10-19；main 基线 `6e5b05c`。本批关闭的是下述静态子问题，不关闭 A03 工作包。未重跑 A01 的 17 项断言，未执行原厂 WPF/SDK、设备或桌面操作，未修改产品、共享台账或 tool-plugins。

## 身份和可复验产物

- 原件：`D:/Program Files/VisionMaster4.4.0/Applications/myLibs/VMControls.WPF.dll`，SHA256 `99ffa401c8784472913ad43813d68a8a5577890c00e149506e3e5664fa61c97d`，MVID `d144317f-3992-4275-9b0f-13fb6163e07d`。
- REA `inspect_managed_members` Evidence `ev_dd0a0670e77771079905d007438370d5ca2fd94b3ea25e9acef8513d1a0cea0f`：原件身份可得，成员覆盖 **unavailable**，具体限制是 metadata representation 超过 67108864 字节预算（offset 730996）。完整结果保留在 `analysis/full-audit/batch2-20261019/A03/rea-members-result.json`。没有把空 methods 误报为方法不存在，也没有重复相同调用。
- 补充工具：`tests/NativeEvidence/A03/ExportTabsCil.cs` 使用 .NET Framework `ReflectionOnlyLoadFrom`，仅读取元数据、方法体与异常区，不执行目标构造器、属性或方法。命令 `& tests/NativeEvidence/A03/Run.ps1`，exit 0，导出 **22 个方法体**。这是静态取证结果，不是 22 项原厂运行测试。
- 输出：`analysis/full-audit/batch2-20261019/A03/cil-methods.json`（原始 CIL SHA256、token、异常区）、`cil-disassembly.txt`（带精确 IL offset 的操作数解析）、`source-sha256.csv`（15 个源码/资源/对照文件身份）、`stdout-stderr.log`、`output-sha256.csv`。脚本比较取证前后原件哈希，未变。
- 原始反编译根为 `analysis/decompiled/VMControls.WPF/`；以下文件路径均相对仓库根，行号绑定本批 `source-sha256.csv`，不能跨版本视为恒定地址。

## 新增确认一：Tab 的所属流程和当前显示 Group 是两种身份

证据类型：source-confirmed，关键分支已对照原件 CIL；没有 runtime-observed。

1. `VMControls.WPF/VMControls.WPF.DataForUI/VmProcedureDecorator.cs:78–95` 的 `DisplayModuleSet` setter **先**设置 `IsSelected=true`，再按对象引用不同赋值，依次通知 `DisplayModuleSet`、`IsProcedure`、`IsDependentGroup`。`.ctor:376–379` 仅令初始显示为 `this`，不是每次切 Tab 回根。
2. `ProcedureConfigControl.xaml:523–538`：TabControl 的 ItemsSource 是 `DecoratedProcedures`，SelectedItem 是 `DecoratorSelectedTabItem`；Tab 内容、DiagramArea.Procedure 和节点 Children 使用当前流程装饰器的 `DisplayModuleSet`，`SubSource` 则仍是该流程装饰器。
3. Group 双击的专用路径在 `base/diagramitemtemplate.xaml:1828–1838`：GROUP 样式把 MouseDoubleClick 绑定到 DiagramArea.SubSource.OpenGroupCommand，参数为当前 Group 装饰器。这条路径不能与 `ProcedureConfigControl.DiagramArea_ItemDoubleClick:644–659` 的参数编辑入口混为一谈。
4. `VmProcedureDecorator.OpenGroupCommandExecute:537–565`：禁用或明确加载失败的 Group 返回；有密码先验证，成功才替换 DisplayModuleSet；其余对象直接替换。`UpCommandExecute:672–684` 从**显示对象模型**取 Parent；Parent 等于所属流程 DecorateSource 时显示 `this`，否则新建 VmGroupDecorator 包装 Parent。不是复用先前的 Group 装饰器历史栈。
5. `ProcedureConfigControl.OnDecoratorSelectedTabItemChanged:871–877` 只把新选 Tab 赋给 DecoratorSelectedItem；`OnDecoratorSelectedItemChanged:886–946` 再区分 IVmProcedure，发布 SelectedPro 或 SelectedModule，LoadingFlag 控制 SelectedItem 发布。它们没有把该流程的 DisplayModuleSet 重置为流程自身。
6. `ContentPresenter_DataContextChanged:1482–1502`：选中流程内容调用 SelectedItemToSelectedTabItem；Group 内容设置 DecoratorSelectedSubTabItem 后调用 SelectedItemToSelectedSubTabItem。`OnDecoratorSelectedSubTabItemChanged:879–884` 本身没有派生动作，不能把空回调当作导航逻辑。

由此可确认每个仍存活的流程装饰器独立保有 DisplayModuleSet。切换 A→B→A 不应通过 Tab 选择回调主动丢弃 A 的显示 Group。WPF 内容模板是否保留具体视图实例、失效 Group/删除/Undo 后如何重建，仍待原机操作，不能仅凭字段存在推断全部行为。

## 新增确认二：运行按钮始终针对所属流程，而非当前显示 Group

`ProcedureConfigControl.xaml:349–352` 的运行命令绑定在流程装饰器上。`VmProcedureDecorator.RunOnceCommandExe:461–477` 先 RunCheck，再取 **DecorateSource as VmProcedure**，调用 `Run(null, bIsWait:false)`；Continuous 分支 `494–515` 取 **DecorateSource as IVMRun**，切换 ContinuousRunEnable。两者均不从 DisplayModuleSet 取执行目标。CIL 分别为 token `0x06000A32` / `0x06000A35`。

main 对照：`src/VisionEditor/WorkflowExecution.cs:54–62` 和 `WorkflowTabs.cs:122–125` 显式传入顶层 procedure.Id，基本符合这个身份分离。这里没有证据支持把显示 Group 直接作为 Tab 运行按钮的独立执行根。

## 新增确认三：缩放不是仅存于 ZoomArea 控件

`VmModuleSetDecorator.cs:17,33–76,162–173`：

- 私有默认 `proceZoomIndex=9`；`ZoomArea.xaml.cs:23–29` 的 20 个档位为 0.1 至 2.0，索引 9 对应 1.0。
- getter 每次读取 `(DecorateSource as IVmModule).UserData["ProceZoomIndex"]`。不存在则返回字段默认；存在则 UTF8GetString 后 int.Parse。**getter 无 catch 或范围校验**，不能臆造损坏值自动回 100% 的原厂规则。
- setter 先尝试重读已有值（分别捕获 VmException 和 Exception 并记日志），与新值不同才更新字段，并写 UTF8GetBytesPadZero(index.ToString())。写失败显示消息，然后仍通知 `ProceZoomIndex` 与 `PartentZoomIndex`。字段先写、持久写可能失败，不能表述成原子提交。
- 显式 `IZoomIndex.PartentZoomIndex` 直接代理该属性。`DiagramArea.xaml:51–65` 的 DataContext 是 Procedure（即当前显示装饰器），ZoomArea.DiagramZoomIndex 与 StatusBar.ZoomIndex 均双向绑定这个接口属性。因此流程与 Group 均有**模型自己的缩放键**，不是仅所属顶层流程共用一个值。
- 用户数据读写后续源链为 `analysis/decompiled/VM.Core/VM.Core/UserData.cs:28–105,136–157` → ModuleManager GetUIParamValueByKey/SetUIParamValue；这一段引用源码，未在本批新增 VM.Core 的 CIL 或实际保存重开探针。

历史边界：UserData setter 对普通非动态模块读旧值、写新值再 RecordSingleData；动态模块按 ModifyConfig/DeleteModule/CopyModule 分支决定整包或单键记录。`RecordSingleData:123–134` 注册 UserDataUnRedoOperation，恢复 `178–203` 按 ModuleFullName 查找模块并 SetUserData。**有注册调用不等于每次缩放自动创建根操作**；具体是否进入历史仍取决于 C01 研究的当前 root/part 和过滤条件，本批没有执行 Undo/Redo。

平移边界：本批审查的 ZoomArea 成员只看到 ScrollViewer/ViewArea 计算，没有发现与 ProceZoomIndex 同级的偏移持久键；搜索范围是该类和上述装饰器，不构成“全程序绝不保存平移”的证明。非活动 Tab 的实际 ScrollViewer 生命周期、重开偏移与保存容器还未验证。

## main 对照和可实施差异

| 项目 | main 当前代码 | 差异与处理建议 |
|---|---|---|
| 非活动 Tab 的显示 Group | `WorkflowTabs.cs:54–60` 总是 SelectProcedure(rootId)，非活动 Tab 用根流程渲染（97–104） | 与每流程 DisplayModuleSet 保留不同；后续可增加每顶层流程的有效显示容器映射，覆盖 A深入Group→B→A、删除Group、Undo/Redo、重开。需要完整身份失效策略，**本批不改产品**。 |
| 缩放保存 | `WorkflowCanvas.cs:32,78–85` 用 canvasViews 的 Scale/X/Y 做会话缓存；本批没找到模型缩放字段或保存接入 | 源码确认应区分每显示容器的缩放持久数据；独立 vmx.json 可提供明确独立字段，不能标成已实现原生 UiParamData 兼容。先核对存储合同再实施。 |
| 方案替换时会话隔离 | `WorkflowCanvas.cs:63` SolutionReplaced 只清 automaticRoutes；canvasProcedure/canvasViews 没清理；`GraphEditorDocument.cs:28–36` 新方案再次使用 main，加载也可能复用流程 ID | 独立实现的身份隔离缺口：旧/新方案同流程 ID 时不会进入 RenderGraph 的切换分支，可能沿用旧视口。可由 root 选择最小清理：SolutionReplaced 清 canvasViews、canvasProcedure，并令新视图按新方案初始化。此建议基于 main 数据流，并非伪称原厂源码的清理实现。 |
| 历史作用域 | main 全方案 Snapshot 包含 ProcedureId；原导航 setter 不注册 root | 不能据此改成每 Tab 独立 Undo 栈。非活动 Tab Undo 的实际焦点/显示变化需 C01/A03 联合补证。 |

## 精确 CIL 锚点

完整 22 项见 cil-methods.json，以下 SHA256 均仅覆盖原始 IL 字节，**不含**方法头、locals、异常区；异常区另存，不以哈希替代语义审查。

| 成员 | token | IL SHA256 |
|---|---|---|
| set_DisplayModuleSet | 0x06000A14 | 5b11429f3f87a3b84e322a896678943e03c8e1c899a1c4ed9f8de9b0a09de181 |
| OpenGroupCommandExecute | 0x06000A3C | 58d615ba312f048ed7b0814d48373c0bf295a229788af86de0808c255d3dba1d |
| UpCommandExecute | 0x06000A45 | f4dde624c1149840b18775fde124c7c83b89a9051acee91bde5914eeb24737db |
| get_ProceZoomIndex | 0x060008E8 | 8805f21265390af228b0d16f5313bdf523fcdb8093c2a5f4f3835f2d17fdd8be |
| set_ProceZoomIndex | 0x060008E9 | 36208bb5f9dd05bccebe71295b2ce44489d53b5a3e6289711466d502aae82ae5 |
| ContentPresenter_DataContextChanged | 0x060002E4 | 39c63f12c9b9ccfaa9b18ef72668b273a6ac6094a8b508ee319ed680f0858186 |
| VmModuleBaseDecorator..cctor | 0x0600099E | 61f5203bb7c63510d2db3e032ce27b377d6442424ec20cee89d55d5c12334ce7 |

## 安全审查与未运行原因

完整基类初始化审查发现 `VmModuleBaseDecorator.cs:485–517` 的静态构造器会调用 LogHelper.Info，并经 CurrentPathHelper 读取 ModulePathGenerate.txt。原件 CIL `0x0600099E` 已确认这些调用。普通 new 或为运行探针绕过构造器并不能天然消除类型初始化副作用；没有把 FormatterServices/GetUninitializedObject 当作安全保证。因此本批不实例化这些原类、不配置原厂日志，不初始化 SDK。metadata-only exporter 只编译/运行自己的静态读取程序，不属于真实导航探针。

## nextAction / 关闭门槛

1. A03.01 runtime：在任务自有 app base 中先隔离并验证 CurrentPathHelper、日志配置、ModulePathGenerate 与 WPF 资源依赖，再执行原装饰器 Group进入/返回及 A→B→A；观测 DecorateSource/DisplayModuleSet/SelectedItem 身份。不操控他任务实例。
2. A03.02 storage：独占 SDK 最小方案分别改变流程与嵌套 Group 缩放，保存新文件并重开；证明 ProceZoomIndex 键归属、UTF8+NUL 载荷、删除/重建行为。由 D 线关联原生读写，不能仅凭 ZIP 字节命名完成。
3. A03.03 history：C01 原服务确定 root/part 后，操作非活动 Tab 的编辑、缩放、Undo/Redo，记录显示对象/焦点/历史载荷；不先设计每 Tab 栈。
4. 集成前由 root 串行审查最小方案替换视口清理；若采用，要验证同ID新建/加载和不同ID加载、概览切换，无需为静态报告运行共享 WPF 构建。本批未实施任何上述产品建议。

### 本地会话缓存清理的最小回归步骤（交 root，尚未执行）

仅清 canvasViews/canvasProcedure 仍不足：`WorkflowCanvas.cs:77` 在 workflowOverview 时提前调用 RenderWorkflowOverview 返回；`WorkflowOverview.cs:27–30` 切换又会先写当前视口到 `__solution`。替换方案时还需初始化当前 scale/panX/panY，避免概览绕过常规切换分支而沿用旧值。这仍是本地方案身份隔离修复，不提供原生缩放持久化。

| 操作 | 应验证的独立实现结果 |
|---|---|
| 方案 A 的 main 调至150%并偏移，NewSolution 仍为 main ID | 新画布100%、零偏移，旧方案缓存不再可取。 |
| A、B 两份独立 vmx.json 都有 main，A 改缩放后 Load B | B 初始化视口，不能因同ID跳过清理。 |
| 概览170%并偏移时 Load B；再切流程、切回概览 | 加载后概览立即初始化，切换不会把旧概览重新写入新方案缓存。 |
| 同方案两流程各设置不同会话缩放与偏移并反复切换 | 各自缓存保持；清理只发生在 SolutionReplaced，而非所有 Changed。 |
| Load 空流程方案，然后添加流程；Load 不同ID方案 | 不访问无效流程、不恢复删除ID的旧缓存，概览入口继续可用。 |

本包状态建议 `evidence-ready`（新增静态切片），runtime、operation、differential 与完整保存重开均保持开放。
