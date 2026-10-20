# 联合审核第一包：原厂 IMAGE 候选体与当前真实图执行

基线 main `120611182c8908334298ba47eb9244bb1f9b6307`，2026-10-20。本包审 J04/J05/J12 的明确问题：① 原厂 IMAGE 下拉的来源范围与筛选；② ImageSource 实际 Gray/RGB/可选 Mono8 到 Binary 的图执行状态；③ 卡死是否已有足够证据归因。未改产品实现或 `tool-plugins`。

## 原版 J04：已取得的方法体，不再称“内部完全未知”

安装版 `D:/Program Files/VisionMaster4.4.0/Applications/myLibs/VM.Core.dll` SHA-256 `936763F6C8A1BF9FCF59F22ACEBCA8FEB09B5F3D5701AE77C82BDC81E626190D`。REA `inspect_managed_artifact` 完整确认程序集身份，证据 `ev_fae324921821a748611e953d4e024be4bf5acf63106a591e4d1efd29333051c1`；`inspect_managed_members` 因 metadata representation 64 MiB 限额返回 `unavailable`，证据 `ev_24cc2530eacf9efa362e9e47c387a6ebd90d32e66a8db980132325bfa7616b1c`。因此本包用本机已有 `dotnet-ildasm 0.12.2` 只读导出该**同一安装 DLL** 的 `VmModule.GetFrontIOWithLeafItemsByName` CIL（`DOTNET_ROLL_FORWARD=Major` 仅作用于工具进程，不安装运行时），文件 `analysis/joint-audit/VM.Core.GetFrontIOWithLeafItemsByName.il` SHA-256 `A442D5B02176056DC4BC2BA972713B87CF12916BF54F58A1F9ECF8916C79F71C`。原 DLL 未修改。该体 RVA `0x1FAFC`，CIL 329 字节；与本地 `analysis/decompiled/VM.Core/VM.Core/VmModule.cs` SHA-256 `53E2F0F9032C4631B59CF0EB2A00E06D50469D94706E7E370E9CC7E1059C72CE` 对照。

确切逻辑：`GetFrontParamItems()` 从 `PreModules` 递归收集执行前驱，另加非 Solution 父模块；`GetFrontIOWithLeafItemsByName` 再加入 ID `13000` 全局变量模块，以及当前模块集或父模块集的局部变量模块。若遍历项等于父模块，则从它的 `Inputs` 查候选，其余项从 `Outputs` 查。`GetAllCombinationByType` 递归遍历非叶 `VmIO`，只收 `TypeName` 等于所求类型的组合节点；最后筛 `CanSubscribeA && Visible`，返回各自 `ModulePathName`。这是方法层源码和本体 CIL共同支持的**候选集合规则**。CIL 没有证明 `PreModules` 在各种连线操作中如何维护，也没有证明最终 ComboBox 会保留该列表顺序、原版对 RGB/MONO8 的实际输入校验或自动转换。

当前 `WorkflowConfigurationSubscriptionService.Enumerate` 只取当前 root 流程显式 `ExecutionLinks` 的启用前驱，按本地 descriptor `CanSubscribe` 和类型过滤，按当前文化的标题排序；`UnsupportedCapabilities` 明列全局/局部变量、父容器输入及原生可见元数据。因此现有桥是一个有边界的独立实现，**不能把其候选集合当作原厂 `GetFrontIOWithLeafItemsByName` 的完整实现**。对应跨项目合同已登记 [FCR-031](../CONTRACT-REQUESTS.md)；并未直接扩大订阅范围或复制插件工作树代码。

## 当前 J05：真实两节点图执行，四状态已通过

可重跑检查为 `dotnet run --project tests/FrameworkPluginChain.Checks/FrameworkPluginChain.Checks.csproj -c Release`，本包实际执行 **4/4 通过**。该检查注册真实 `ImageSourcePlugin` 与 `ImageBinaryPlugin`，使用自有 2×2 `GrayImage`/`RgbImage`，通过图像源 SDK 提交路径供给自有 `ModuleExecutionScope`，构造真实 `ProcedureDocument`、`ExecutionLink`、`Connection`，先通过 `GraphEditorDocument` 真实保存、重新加载并断言源端口和 `OutMono8` 值，再由 `GraphValidator` 和 `GraphExecutor` 执行；没有调用用户软件窗口或设备。重跑日志 `analysis/joint-audit/20261020-framework-plugin-chain-checks.log` SHA-256 `27062F9DA81A9DD7F65A6548E087C48A34574573AB8BC063D49509BFA79AA99D`；测试源码 SHA-256 `23977D7FDF6417F947CB45CBA9FF13D28F99F1D65EC14882B1E9C4A5AE3C1BCE`。独立诊断 JSON 为 `analysis/joint-audit/20261020-graph-image-chain-probe.json` SHA-256 `1456329F1B93D0833DB4F326FCD20C9AF98A76B72F86CB55334DDF4621558191`；诊断源码 SHA-256 `2C79971866BD4A775B685A66160680598968CAEC9224422387AF91E1C00E9110`。

| 来源 | 静态图 | 图像源结果 | 二值化结果 | 边界 |
|---|---|---|---|---|
| 灰度帧，接 `Image` | 有效 | `GrayImage` 成功 | 成功，输出 `GrayImage` | 当前实现正例 |
| RGB 帧，接 `Image` | 有效 | `RgbImage` 成功 | 失败 `InputTypeMismatch` | 声明类型准入与实际载荷不一致 |
| RGB 帧，启用 `OutMono8` 并接 `ImageMono8` | 有效 | `GrayImage` 成功 | 成功 | 当前实现的显式灰度输出路径；不代表原厂会自动选择它 |
| RGB 帧，关闭 `OutMono8` 仍接 `ImageMono8` | 有效 | 该输出不存在 | 跳过 `UpstreamUnavailable` | 条件输出的当前行为 |

此前直接调用二值化插件的 RGB 探针返回 `InvalidRoiOrInput`，而完整图执行返回 `InputTypeMismatch`：这是不同入口的错误码，不能混为 `ModuleException`。本包没有将 RGB 输入偷偷转换为灰度，也没有把当前错误消息“Input cardinality mismatch”解释为原厂文字。原版 `InputImage/InputMask` 的像素格式分支仍缺 native 体/受控差分，J05 的原厂等价层**未关闭**。

## J12：卡死仍缺根因证据

2026-10-20 18:16:59 +08:00 的 Windows Application Hang 1002 和用户“先卡住，后操作才弹窗”的时序见 `analysis/joint-audit/20261020-hang-event.json`。本包没有与该进程相同方案/同样图片的受控重现，也没有 UI 线程堆栈或等待链；不能把本次图执行探针的成功/失败与卡死归因关联。下一可执行工作是仅在**自有受控实例**分段记录 `doc.Changed`、`RefreshAll`、路由重算、右侧图像复制、配置事件和后台回调，出现无响应时取线程栈。当前 J12 为 `blocked-exact-evidence: missing UI-thread stack/timing from controlled reproduction`，其余 J01–J11 可继续推进。

本包产品实施单元 **+0**，主框架实施仍 **41/96=42.71%**；新增的是 J04 原方法/CIL、J05 真实图执行检查和可复验失败矩阵。人工验收另列，`tool-plugins` 未动。
