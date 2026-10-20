# 第三十轮 D 线：D01.1 保存/加载托管入口与完成标志

## 结论

本轮按原始反编译源码追到原生 SDK 调用及回调状态，D01.1 的 **source=done** 可复核；但 main 没有原生 `.sol` SDK 会话和回调源，现有 `SolutionStore` 只读写独立 `.vmx.json`。在这条链上补同名回调或完成标志只会给 JSON 操作套上原厂术语，不能满足 D01.1 的完整实施门槛。因此本轮 D01.1 **+0**，保持 implementation=not_started。未改产品或 tool-plugins。

## 原始入口到状态的调用链

1. `VisionMaster.Project.ProjectManager.SaveSolution` 决定原路径或另存为；带进度路径调用 `SaveSolution(path,password,afterSaveTask)`，先在 UI Dispatcher 执行 `SaveCalibUserData`，再 `VmSolution.Export`。成功后更新路径/名称通知、启动后续任务。`LoadSolution` 先 `CheckSolution` 格式/密码，再读版本、处理旧方案/通信状态，进入后续 SDK 加载。来源：`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.Project/ProjectManager.cs`。
2. `VM.Core.VmSolution.InternalExport` 拦截并发 `_isSaving`，进入时置 true，取消选择或异常置 false；调用 `SolutionManager.SyncSaveSolution`。`MSolutionManager_OnSolutionSaveEndCallBack` 从原生状态构造结束事件，最后置 `_isSaving=false`。`VmSolution.Import` 暂挂载 load-end handler，在 finally 中摘除；end handler 按错误码改 `SolutionPath`/`SolutionPassword`。`MSolutionManager_OnSolutionLoadBeginCallBack` 置 `isLoading=true`；end 置 false，处理失败实例、模块加载状态并派发事件。来源：`analysis/decompiled/VM.Core/VM.Core/VmSolution.cs`，重点 `InternalExport`、`Import` 与 `MSolutionManager_OnSolution*CallBack`。
3. `VM.PlatformSDKCS.SolutionManager.SyncSaveSolution` 清除保存错误码与等待标志，调用 `ServerSDKAPI.IMVS_SaveSolution`，同步等待 end callback；SDK 模式下对非零结束码抛 `VmException`。加载对称地调用 `IMVS_LoadSolution`，重置 `ParamSeted`，等待 load-end；结束处理先向事件订户派发，再记录错误码并置等待标志，加载还重置 1000 个流程标志。来源：`analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/SolutionManager.cs`。这个顺序是托管反编译静态观察，尚无原厂运行回调时序差分。

## main 对照及边界

`src/VisionEditor/MainWindow.ShellSaveSolution.cs::Save` 与 `MainWindow.ShellOpenSolution.cs::OpenSolutionFromFileMenu` 经 `GraphEditorDocument` 使用 `src/VisionFramework/SolutionStore.cs` 的 JSON 格式；其保存为本地临时文件写入与替换，加载拒绝重复 JSON 属性及无效结构。该独立格式的保存、加载已存在，但没有 `IMVS_SaveSolution/IMVS_LoadSolution`、原生状态码、`VmSolution` 回调或 `SyncWaitor` 消费者。它不能作为 D01.1 的原生托管入口闭合实现计分。

## D04.1 备选复核

`src/FrontendDesignMigration/DesignConverter.cs` 已实施 `Root/V4.0` 设计 XML 的已确认托管转换分支，见 `docs/reverse/D-frontend-migration-implementation-batch13.md`。`docs/reverse/D04-migration-gate-batch27.md` 已核定剩余 native solution/process/group 容器转换、模板/编码与版本/失败恢复缺少证据。本轮没有新的原始转换分支或样本，D04.1 保持 partial，亦 **+0**，不重复计已实现的 XML 分支。

## 精确解除条件

要关闭 D01.1，需可调用的原厂 SDK 受控副本/隔离会话，取得一次保存和一次加载的 begin/progress/end 回调、同步等待、错误/取消样本，并核对 `IMVS_SaveSolution/LoadSolution` 原生返回与结束码。只有在原生容器适配器与会话所有权确定后，才能在 main 接入这些回调及标志。D04.1 另需真实旧/新版本样本与 CompatibleTool 的受控转换，或可追到实际容器 writer/reader 的源码级证据。

本轮未执行原厂 SDK、未访问其他任务进程或工作树；既有 D01.2 只读样本边界解析仍按原台账计分，不挪作原生导入器。
