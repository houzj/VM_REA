# 第十七轮：标定板节点删除、撤销与方案替换的宿主所有权

范围只含“标定”分类的 `vision.calibration-board` 及其共用执行域生命周期。此报告关闭当前重建版 J10 的一个**具名资源回收边**，不代表十工具 J10、原版 SDK 释放或用户长期卡死已关闭。

原版 `VM.Core.VmModule.Dispose→InnerDispose` 先标 `IsOnDispose`，收集 Group/父模块关系并 `ClearVarBindResultRelation`，随后删除子模块、清理前驱/参数绑定，`CommonDispose` 断开 `NextModules`、取消父容器订阅并分别释放 Inputs/Outputs，最后从父 `Modules`/全局 `AllModules` 删除、调用 `CloseModuleInterface→mModuleManager.CloseOneModule(ID)`，再登记删除历史及模块变更事件。原始反编译体：`analysis/decompiled/VM.Core/VM.Core/VmModule.cs:2678,2686,4153–4220`，SHA-256 `53E2F0F9032C4631B59CF0EB2A00E06D50469D94706E7E370E9CC7E1059C72CE`。这说明原版删除是实体模块/IO/SDK 级释放，不能只以图节点隐藏推断等价。

当前主窗 `MainWindow.xaml.cs:48` 把 `doc.Changed` 接到 `RetainExecutionNodes`；`ToolExecutionScope.cs:45` 扫描当前所有流程节点，将不再存在的节点传给 `ModuleExecutionScope.RetainProcedureNodes`；`ModuleExecutionScope.cs:41–44` 对 `(procedure,module,node)` 归属的状态执行 `RemoveNode` 与 `IDisposable.Dispose`。`doc.SolutionReplaced→ReplaceExecutionScope` 则释放旧方案所有权并建立新域。相关当前源 SHA-256：主窗 `034C0DAE3E04A9B592031D87E1C1349C23B14091B04564C295267A97FCAA1B4A`，执行宿主 `BB92B32267FF332B4E0955EF3DC600A8BC7B2F7B38A8804128C146DAE439A52A`，域 `FE6B2880402886F3A3E40E8A1BB93B5E180C17364899F1C520B0C587DAED2B1B`。

新探针 `tests/CalibrationLifecycleOwnership.Checks/Program.cs`（SHA-256 `FEB6D0C2DC68C3AA8549610802F96A5DFB1E92D7AC2B62B3BB607C86544BBDB0`）构建真实 `MainWindow`、注册在真实标定板节点键下的可计数 `IDisposable` 状态，调用文档真实 `DeleteNode→Undo→NewSolution`。6/6 通过：删除释放一次，图节点消失；撤销保留节点身份但创建新状态；方案替换再次释放；旧域重复释放不重复清理。可复跑：`dotnet run --project tests/CalibrationLifecycleOwnership.Checks/CalibrationLifecycleOwnership.Checks.csproj -c Release --artifacts-path analysis/joint-audit/pass17-lifecycle-build-v2`；结果 `analysis/joint-audit/pass17-lifecycle-board-run.log`。探针使用离屏 WPF 宿主，**不是**用户桌面手工验收，也没有运行板标定算法或原厂 SDK。首跑暴露探针 STA/版本号/方案替换后旧域引用三处夹具错误，修正后复跑成功；失败未冒充产品缺陷。

已证：当前标定板节点的 scoped 托管状态不会因删除→撤销重建→方案替换在宿主内复用旧对象。未证：原厂同构 UI 删除时 SDK CloseOneModule 成功/失败、板原生内部资源、异步任务删除竞争和用户遇到的长期无响应线程栈；当前插件 `standalone` 回退路径未被本 scoped 探针覆盖。未因有限 6/6 提升主框架 41/96 实施分子，也不触及 `tool-plugins` 工作树。
