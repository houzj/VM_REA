# D01 第五批：保存回调完成标志的有界源码子项

基线 `e6bf62acf5ffebec39475dd3182d2acd05ea56d5`。子项 D01.save-callback 的静态门槛已完成：追 SDK 回调→Core 事件→保存/等待标志，核对原 CIL 与异常区。原生 writer、单操作保存差分、实际 SDK 回调均未完成，D01 整包仍 blocked。本批不扩展为额外文件格式猜测，不运行原类或 SDK。

## 精确证据

按 REA 技能复用同版本反编译；大程序集成员预算/native provider 阻塞不重复查询，补充 ReflectionOnlyLoadFrom 定向原 CIL。来源 `D:/Program Files/VisionMaster4.4.0/Applications/myLibs/VM.Core.dll`（SHA256 `936763f6c8a1bf9fcf59f22acebca8feb09b5f3d5701ae77c82bdc81e626190d`）、`PublicFile/x64/VM.PlatformSDKCS.dll`（`34c0c90b00cb75a701685b96670b6490f61376f62eacc0ab29d240157db401c1`），前后哈希相等。

`tests/NativeEvidence/D01SaveCallbacks/Run.ps1` 最终 exit0，准确导出 6 个原方法体，均无 EH 区：Core token 06000163/164/165；SDK 060005FE/5FF/600。完整 IL 字节哈希、操作数和身份在 `analysis/full-audit/batch5-20261019/D01/run-3a100c718d2043efa280038298fc47c3/`。不是 6 项运行测试。初次复制导出器多带入旧回调闭包的输出保留在另一 run 目录，6 个无关方法不计新增覆盖；修正选择后才形成最终切片。

## 已确认调用链和条件

以下源码相对 `analysis/decompiled/`：

1. `VM.Core/VM.Core/TempVmSolution.cs::.cctor` 订阅 SDK SolutionManager 的保存开始/进度/结束到 Core 对应静态成员。该构造还注册大量其他服务，未执行；注册原 CIL 已由第四批 C02 取得，不重复导出。
2. `VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/SolutionManager.cs:1065–1079`：Begin/Progress 直接发对应委托。End **先发 OnSolutionSaveEndCallBack，再写 nSolutionSaveErrorCode，最后 mWaitSaveSolutionFlag=true**。06000600 的 Invoke 在 IL_000E，写错误码 IL_001B、写完成标志 IL_0022；无 catch/finally。
3. `VM.Core/VM.Core/VmSolution.cs:3421–3449`：Begin/Progress 受 Instance.isDisplayProgress 门控。End 也先检查该门控；启用时构建 EndEventArgs，复制 ClientType/Status，错误时查本地化失败标题，然后发外部 OnSolutionSaveEndCallBack。之后才 `_isSaving=false`，再日志。门控关闭不发该外部进度事件，但正常路径仍清保存标志。
4. SDK End 委托同步进入 Core，Core 又同步发 UI/其他订阅者。因此任何较早订阅者或本地化步骤抛错，都可能阻止后续 Core `_isSaving=false`，并沿委托返回阻止 SDK 错误码/完成标志写入。两层原 CIL 均无保护异常区，不能把方法名 End 当作 finally 或可靠提交完成保证。另须区分更晚的日志异常：日志在 Core `_isSaving=false` 之后、SDK 两标志之前；若在此抛错，Core 已清而 SDK 尚未写，不能说所有异常都让三个标志共同保留旧值。
5. `SolutionManager.cs:305–323::SyncSaveSolution` 的既有源码显示先清错误/等待标志、调用 IMVS_SaveSolution、即时错误则抛、清 ParamSeted、SyncWaitor.Wait，再仅 SDKMode 检查异步错误。与上面的标志写入顺序相连，说明托管正常返回/错误上报依赖回调完成路径；**没有运行证据证明订阅者异常时一定永久等待或原生进程一定崩溃**，不得外推 callback transport 的异常处理。

这是源码确认的控制流风险边界，不是已复现原厂 bug，也不是 `.sol` writer 格式或事务证据。无产品补丁、无独立近似实现。

## 退出与剩余范围

本子项状态：**done（静态调用链）**，原方法运行和原机差分：not-run。停止扩大该问题。

D01 整包仍缺：可用且获授权的 native decompiler/provider、精确 native writer/reader 字段与写入链、同最小原方案的单操作保存差分及重开验收。沿用原阻塞，不重试相同 readiness，不把 6 个回调当成 writer 闭合。

若将来验证上述异常路径，先隔离原 SDK/日志/后台线程构造并定义自有实例及输出路径，再记录回调进入、事件顺序、两层标志和返回；当前不构造 VmSolution/SolutionManager，不接触安装原件或 tool-plugins。
