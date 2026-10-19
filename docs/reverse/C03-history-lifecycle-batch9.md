# C03 第九批：历史、底部耗时及方案替换的有限源码链

基线 `f11bc13`；2026-10-19。只读提前调查，父项 C02、D02 未关闭；没有产品修改、SDK/原程序/logger 执行或跨项目合流。

## 证据及复验

沿用 `docs/MF05-RESULT-LIFECYCLE.md` 的入口，新增原文件 C# 与精确 CIL。运行 `tests/NativeEvidence/C03HistoryLifecycle/Run.ps1` 仅启动自有 .NET Framework 元数据读取器，使用 `ReflectionOnlyLoadFrom`、`GetMethodBody`，不调用目标方法或初始化目标类型。

最终审计目录：`analysis/full-audit/batch9-20261019/C03/replay-caf705878e5440a5babd7313ee83fe84/`。18 个 Core 方法体、11 个 WPF 方法体、8 个 EXE 方法体，合计 37 个；逐方法 token、IL SHA-256、MVID、异常区及完整程序集 SHA 见各 `*.cil-methods.json`。输入运行前后 SHA 一致，见 `input-hashes.json`。这些是静态方法体数量，不是运行断言或总计划加分。

REA 对实际 `Applications/myLibs/VMControls.WPF.dll` 识别证据 `ev_3eb1fd75ae8c1c32c6ae332a1cf0b6d1ea816b3794cf22eee230719d5af4dcd6`，完整返回保存在上一层 `rea-identity.json`；不把自有 CIL 导出标成 REA 反编译。原反编译及资源路径统一位于 `analysis/decompiled/`，其当前 SHA 索引见 `source-hashes.csv`。

## 原入口至状态/事件链

1. `VisionMaster.AppMainWindow.TitleMenueItemData.CreateNewSolution` 完成保存决策后关闭通信、`FrontendWindowData.CreateNewDesign`、`ResetResource`、`ProjectManager.CreateNewSolution`。`ResetResource` 关闭全局通信、相机、全局脚本/变量等窗口；该体没有历史或耗时置零语句。`ProjectManager.CreateNewSolution` 注册方案撤销/保存旧方案后调用 `IVmSolution.CreatNewInstance`；不应把整个函数缩写成无条件清空。
2. `VM.Core.VmSolution.IVmSolution.CreatNewInstance` 调 `TempVmSolution.Init`、清方案密码、SDK `CloseSolution`，再 `Instance = new VmSolution` 并建默认流程。`set_Instance` 先遍历旧 `AllModules` 设置 `IsActive=false`，保存旧实例，替换静态 instance，再 `RestoreDefaultDisplay(SolObj, ObjectEventArgs{Param=旧实例})`。**它没有逐个 Dispose 旧模块，也没有在该 setter 清空静态待通知字典。**
3. `VisionMaster.MainWindowData.VmSolution_RestoreDisplay` 从 Param 取旧实例，旧 `PropertyChanged -= SourcePropertyChanged`，新实例先减再加同一处理器。它没有清原生历史缓存或直接写耗时。`DoClose` 的已见方案/脚本/代理/服务/导入事件退订记录在 CIL；不推断所有应用订阅全部解除。
4. 载入：`MSolutionManager_OnSolutionLoadBeginCallBack` 设置 `isLoading=true` 后发托管事件；MainWindowData 处理器关通信并 ResetResource。`MSolutionManager_OnUIGlobalProcessObjectIDCallBack` 重新分配 `_updateContinueStatus` 并 `Instance = new VmSolution`。载入结束先 `isLoading=false`；非零状态且不属于 `_failReflashFlag` 特例（错误码 `-536870093`）时又换新实例。故“失败载入始终保留旧方案”不成立；该静态链也不足以证明 SDK 内部文件/结果缓存原子回滚。

## 底部耗时的真实绑定

原 `VMControls.WPF/VMControls.WPF.Base/StatusBar.xaml`：

- `TimeTemplate` 的工具及算法文字直接 OneWay 绑定 `SelectedItemSource.ModuleTime` / `SelectedItemSource.AlgorithmTime`；不读取 StatusBar 上同名 DP 的默认 0。
- `ProcessTemplate` 绑定当前内容的 `ProcessTime`；`ProcessTimeTemplate` 的内容是 `SelectedItemSource`。
- `StatusBar.OnSelectedItemChanged` 对 IDecorate 取 DecorateSource，其他对象直接赋源，null 则 `SelectedItemSource=null`。`OnSelectedItemSourceChanged` 只计算 IsProcedure/IsModule；`OnCurDiagramAreaSourceChanged` 关闭计时窗口，没有写 ModuleTime/AlgorithmTime。
- `VM.Core.VmModule.SetModuleState` 从 `IMVS_REPORT_MODULE_STATUS_INFO` 回填 AlgorithmTime、ModuleTime、ProcessTime；对应属性通知驱动读者。

因此已经定位“计时从哪个对象读、哪个状态写入”，但尚未沿宿主最终所选项绑定证明新建/每类载入状态必定走 null/新模块，也没有交互桌面前后对照。**不能声称底部一律归零或必定保留旧数字。**

## 模块历史、解绑与全局缓存边界

`ModuleResultView.DoSourceChanged`：存在旧源时 Dispose 当前 decorator，然后 ClearDataStore，再给新源建立并 Init 新 decorator；无新源置 null，最后 UpdateModuleView。`ClearDataStore` 仅清两种显示计数、isRendering 及受锁 datalist，不等于清 SDK 缓存。

`ModuleDisplayDecorator.Dispose → DeInit` 先解除模块异步结果、流程异步状态、参数重载、前置模块/输入集合及显示对象 ValueChanged 等弱事件；清视图、dataRecords、ShapeList、ResultData；停止并解绑 RecordTimer，renderDataCenter.DeInit。随后 Dispose 清 `ModuleDisplayInfoList`，置空 AlgorithmItems/ModuleParams/DecorateSource。这是当前 decorator 的历史清理，不能外推所有方案/SDK 全局历史。

手动 `ModuleResultView.Menu_Clear_Click` 则清当前 `ModuleDisplayInfoList` 并调用 `ClearModuleHistoryResult(ModuleSource.ID)`，明确为模块 ID 边界。`VmSolution.SetModuleResultBuffer/GetModuleResultBuffer` 只委托 `VmInterfaceManager`，实际 native 缓存所有权/清除时机仍未恢复。

另一个跨方案风险边界：`CallBackExecutAndProcessTime` 的两个编译器生成 lambda 也已导出。后台循环持锁交换静态 `_waitDoneProcedureCache` 为新字典，随后对旧键查**当时**的 `instance.AllModules.TryGetValue(id)`，通知 ProcessTime、ExecuteCount，睡眠 150ms。所见 set_Instance/ResetInstance 不清该静态字典。只能确认该查找策略；不能凭静态源码宣称一定发生或必然避免 ID 重用造成的旧通知。

## 结论、失败及退出

C03.3 三部分逐项：历史（模块 decorator 清理已证，宿主所有方案切源及原生全局历史未证）；底部耗时（源/写入/模板已证，宿主完整替换操作与失败 UI 状态未证）；全局结果缓存（SDK 委托已证，native 缓存生命周期未证）。因此 **源码保持 partial，不能升 done**；实现/验证未改变，本片对 96 项 done 分子贡献 0。

保留失败：最初两次 csc 使用正斜杠相对路径被旧编译器误作开关，未生成读取器；首次完整 Run 的三个导出成功但输出哈希阶段自读正在写的 output-hashes.csv 发生共享冲突。后者完整目录 `replay-9399dac39f8a4e4198c7fe535e699edf` 保留，脚本过滤清单自身后最终运行退出 0。初始 Core 14 体导出增加四个实际 lambda 后为最终 18 体，不累加虚报。

下一有限问题：宿主工作区/所选项的 DP 链是否在方案实例更换时切源，以及 `VmInterfaceManager → native` 结果缓存重置的精确消费者。禁止为了补该项运行有设备/临时目录副作用的原方案服务。
