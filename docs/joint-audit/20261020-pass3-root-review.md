# 联合审核第三轮根线复核

基线：main `d7a8741482df16b02ce2d24030852703daf6d715` 加本轮下述未提交主线修复。三条子线分别独占[原生二值化](20261020-pass3-binary-native.md)、[owned Group 图像链](20261020-pass3-image-group-chain.md)、[真实图像 WPF 响应性](20261020-pass3-ui-image-run.md)的报告、探针及 ignored 审计目录。根线读代码、复核结果并串行验证；没有改动 `worktrees/tool-plugins`。

## 本轮改变了什么

1. **原厂二值化包装层边界更清楚。** 原 `IMVSBinaryModu.dll` SHA-256 `C03BCB19EF456206E3ABF4D2A8D5279BE37C12EB5662E191F0965029CDABEB48` 的 12 个 `CIMVSImageBinaryModu` 函数已在隔离 Ghidra 工程导出 C/汇编/调用边，逐文件哈希见 `analysis/joint-audit/binary-native/output-sha256.txt`。`ProcessAlg(0x18001ec00)` 把 InputImage、RegionImage 交下游 `IImageBinaryTool` 并调用 Run；`SetParam(0x18001f9f0)` 将 BinaryType 透传，SauvolaAdjustCoef ×1000、SauvolaRange ×10 后按 float 向零截断成整数文本。`DeInit(0x18001e260)` 的 RCX 实参是 tool 实例。原五种模式的像素循环、RGB/Mono8/Mask 格式门仍在 `MVDImageBinaryCpp.dll` / `MVD_Algorithm.dll` 下游，**没有闭合 J05/J07**。不得因 XML 五枚举或包装层没有转换分支就补四个算法、筛掉 RGB 候选或宣称所有格式可执行。
2. **当前 owned Group 运输经真实执行复核。** `tests/FrameworkImageGroupChain.Checks` 从 `GraphEditorDocument` 建 ImageSource→Buffer→Binary→Binary，导出 Group Result 供父流程第三个 Binary 消费；五次 `SolutionExecutor` 运行覆盖成功、零容量失败后无旧输出、Undo、Redo、保存重开并新 scope 重交图。根线复跑通过。证明当前 Gray/硬阈值的 J09/J11 指定切片，**不证明原厂 Group/.sol 或其他像素类型**。
3. **当前宿主在大帧受控路径可响应。** `tests/WorkflowImageResponsiveness.Checks` 在离屏真实 `MainWindow` 上运行 4096² ImageSource→Binary，根线复跑：入口 50.70 ms 返回、总 361.02 ms、运行未完成期间 7 次 UI 心跳、Binary Success；取消为 Cancelled；运行中关闭后 owner 失效、scope 取消且无迟到 session。该路径未重现永久卡死。原版 `VmProcedureDecorator.RunOnceCommandExe→VmProcedure.Run(bIsWait:false)→ProcessManager.SyncExecuteOnce→IMVS_ExecuteOnce_V30` 仅证托管层不进 `PrecisionWait`，原生调用耗时/线程未知；用户空闲后的无响应仍缺当次 UI 栈，**J12 未闭合**。
4. **主线修复延迟进度回调串运行。** 工作线程化后，旧 `Progress<ExecutionEvent>` 回调可能晚于下一次 `BeginWorkflowResults` 或方案替换而写 `liveDynamicDescriptors`。`DynamicOutputPresentation.CreateExecutionProgress` 现在核 `ExecutionOwnerStamp` 和流程运行序号；`WorkflowExecution.RunCore` 在 UI 上捕获二者。它是当前宿主的并发安全修复，不冒称原厂回调拓扑。`WorkflowDispatcherChecks` 5/5 覆盖 UI 脉冲、正常完成、当前轮进度、旧轮拒绝、旧方案同 `main` ID 拒绝。

## 根线自审与证据

| 检查 | 本轮结果 | 限度 |
|---|---|---|
| `dotnet run --project tests/WorkflowDispatcherChecks/WorkflowDispatcherChecks.csproj -c Release` | 5/5 | 受控 WPF 模块，不是原厂回调 |
| `dotnet run --project tests/ExecutionOwnerLifecycleChecks17/ExecutionOwnerLifecycleChecks17.csproj -c Release -- analysis/joint-audit/root-owner-regression` | 16/16 | 当前 host owner/关闭路径 |
| `dotnet run --project tests/FrameworkImageGroupChain.Checks/FrameworkImageGroupChain.Checks.csproj -c Release` | 五次执行及身份/重开检查通过 | Gray/硬阈值、本地 VMX |
| `dotnet run --project tests/WorkflowImageResponsiveness.Checks/WorkflowImageResponsiveness.Checks.csproj -c Release --no-build` | 正常/取消/关闭三场景通过 | 离屏 WPF，不是用户桌面验收；非原故障栈 |

原生报告 SHA-256 `D4F5FBADEDC6108BEAB4798A66F482F1EA19221EECFB7A182426E73B21C18CE1`；Group 报告 `64109BAF03A11C19C66060F8169F5920B1B2942634BB2447BDA227D321B2AF96`；WPF 报告 `DF4BCB6E9A8E71C0A7F0087BF0FD06C5E318A58D85DF4B8E73D49FD106BB4AFD`。原生中间输出、Group VMX/日志、UI 首三次失败及最终 JSON 都保留在各自 `analysis/joint-audit/` 独占目录，未把失败覆盖为通过。Group VMX 含随机 ID，复跑 SHA 会变化。

## 尚未闭合的审核门

- J05/J07：沿 `MVDImageBinaryCpp.dll` 的 Run/SetInputImage 与算法 ID `0x25a` 进入 `MVD_Algorithm.dll`，取得五模式分派、像素边界、RGB/Mono8/RegionImage 格式校验及异常释放的原始指令；若暂不可得，做标明为**运行证据**的同版本 SDK 正负例矩阵。
- J09/J11：补原版 Group 和 `.sol` 的相同连接、失败、撤销/保存重开语义；当前探针不能跨层冒充原厂结果。
- J10/J12：对其余已接入工具的后台运行 UI 亲和/资源释放逐个审核；用户同构方案空闲卡顿时需受控实例的 UI 线程栈和等待链。没有栈不得宣称根因已修。
- 完整目标仍是已映射 19 对象 × J01–J12 的逐边矩阵。本轮闭合的是若干**切片**，没有新的 96 个框架具名单元经全范围实施自审，所以实施总进度保持 **41/96 = 42.71%**；人工验收单列。
