# 框架—节点联合审查：图像二值化与无响应（2026-10-20）

基线：main `120611182c8908334298ba47eb9244bb1f9b6307`。本轮为审查和独立诊断探针，没有修改产品代码、没有修改 `tool-plugins`、没有操作现存软件进程。用户反馈与截图只用于选定检查状态，不替代原版行为规格。

## 已确认的两类事实

1. **本次无响应存在，但触发源未定。** 用户说明软件在准备继续操作前已感觉卡住，下一次操作后 Windows 才显示“VisionEditor 未响应”。Windows Application Hang 事件 1002 的时间为 2026-10-20 18:16:59 +08:00，报告 ID `678d5618-11c6-4faa-b0a7-859ba00d34c3`，路径和截图/EXE 哈希保存在 `analysis/joint-audit/20261020-hang-event.json`。事件没有 UI 线程堆栈，WER 归档内容本轮无权读取。不能归因于那次点击、N 点标定或二值化。当前源码中 `doc.Changed → RefreshAll → GraphValidator/RenderGraph → RefreshRight` 是同步 UI 路径；路由可进入有上限的大图搜索，右侧可同步复制图像。这是需测量的候选，不是本次原因结论。
2. **二值化存在跨层断链。** 原版安装 XML 声明五个 BinaryType（1/2/3/8/4）及输入 `InputImage`/`InputMask`、输出 `OutputImage`/`OutputMask`；当前独立窗口可选择五项，但当前运行时 `BinaryType != 1` 返回 `BinaryModeUnavailable`，仅硬阈值有独立 CPU 实现。原版托管 `IMVSBinaryModuTool.Run → serverSDKManager.SyncSelfRun(ID)` 可复验；原生 `ProcessAlg`、五模式参数/像素/错误释放体本轮未恢复，不能由 XML 或旧单夹具记录推成原版算法等价。

## 图像格式合同的精确边界

当前 `ImageSourcePlugin` 输出描述符 `Image: DataKind.Image`，实际可为 GrayImage 或 RgbImage；`ImageBinaryPlugin` 输入描述符 `Image: DataKind.GrayImage`。`GraphValidator` 用声明类型，因 `DataKinds.CanFlow(Image, GrayImage)` 为真而允许静态连接。运行时 `GraphExecutor` 对实际值再调 `PortTypes.AcceptsInput`，RGB 值此处为 false，普通图执行记 `InputTypeMismatch`；直接绕过图执行器调用二值化运行时，则 `Input<GrayImage>` 失败并返回 `InvalidRoiOrInput`。这两种错误路径不能混写成 `ModuleException`。掩膜输入也有同类类型合同风险。

独立探针 `analysis/joint-audit/BinaryTransportProbe/` 对插件直接运行：2×2 GrayImage 成功；2×2 RgbImage 的 `acceptedByGraph=false` 且直接插件调用返回 `InvalidRoiOrInput`。输出 `analysis/joint-audit/20261020-binary-transport-probe.json` SHA-256 `E3A14B1EFAFAC46F9886526A9DE5760BF73C53BB0453537C1264B554578607EB`。此探针只验证当前实现的类型边界，不说明原厂是否接收 RGB。原厂 `Group.xml` 和 `Apps.XmlParser` 反编译确认来源候选走 `GetFrontParamItemsOperation(IMAGE)`、已选输入读 `GetSelectedCombinationOperation(InputImage)`、选择写 `RelateIO/RelateIndex`。其内部候选方法现已由本包取出 CIL，见[第一包执行记录](20261020-first-package-execution.md)；`PreModules` 维护与原厂像素格式处理仍未知。不能凭截图加入自动转灰度。

## 联合调用链与现存验证边界

main 工具箱映射 `IMVSBinaryModu → vision.image-binary`，注册插件后拖入节点；双击经 `OpenToolConfiguration` 构造独立编辑器。当前订阅服务维护绑定，配置窗口的 `CommitLive → Apply → doc.Edit → Changed` 可能每次字段提交触发整图刷新；单节点自运行读取既有上游缓存，整流程通过 `SolutionExecutor` 沿图执行，这两条运行路径不可互相替代。当前 Group/ROI 和部分主窗口测试使用灰度夹具，没有覆盖同一方案中 ImageSource RGB/灰度两种实际帧 → 二值化 → 下游 → owned Group → 保存重开 → 再运行的闭环。原版 `.sol` 和当前 JSON 持久化亦分别对待。

下游 `OutMask` 在当前实现为条件输出；禁用时后继可能在运行阶段收到 `UpstreamUnavailable`。原版 XML 的 `OutputMask` 可选且禁止自动订阅，但禁用后的下游状态和旧结果清除须追原生 `ProcessOutput` 与框架订阅事件后才能判等价。

## 下一轮源级工作与解除条件

1. **无响应定因（main）。** 在自有受控副本/实例上对同一方案和图像尺寸记录 UI Dispatcher 心跳、`doc.Changed`、验证、路由、`RefreshRight/ToBitmap`、配置提交、后台回调的分段耗时与线程 ID。分别跑空闲、上游刷新、单次/连续运行、打开/关闭配置；若重现，取该受控实例的 UI 线程堆栈/等待链后才修根因。不得操作用户正在使用的窗口或进程。
2. **原版输入合同（联合）。** 取原 `GetFrontIOWithLeafItemsByName(IMAGE)` 的成员体、`InputImage`/`InputMask` 像素格式校验和源输出调用链，并用受控 RGB/MONO8/灰度输出、连接/断开/重连对照候选、预览、SelfRun 与全流程。原厂拒绝、转换或 UI 过滤须由指令或差分证明后，再决定当前 descriptor/桥接/错误语义。
3. **二值化五模式（插件所有者）。** 对当前哈希的原生 `Init/SetParam/Process/ProcessAlg/ProcessOutput/DeInit` 与 SDK `Run` 取指令、ABI、异常/释放分支；再用多个受控夹具核参数/像素/ROI/Mask/失败旧结果。旧审计所引用的 Ghidra 原始导出与数值探针在当前工作区缺失，REA 原生 provider 本轮 `not_configured`；要么核身份恢复原产物，要么重新导出。未拿到前保持“独立实现/未实现”标签。
4. **整链回归（双方按所有权）。** 同一流程覆盖 ImageSource → Binary → 实际后继、条件输出、Group、Undo/Redo、保存重开及失败/删除释放。主框架负责类型/订阅/Group/运行/存储，插件线负责实际像素、编辑器、算法和工具结果；共享合同变化先登记 FCR，由用户决定合流，不直接改 `tool-plugins`。

本轮无产品实施单元关闭，框架实施进度维持 **41/96 = 42.71%**（人工验收不计入），标定工具既定授权范围维持 **10/10 = 100%**；这些数字不代表原厂全域等价。本轮新增的是可复核的联合审查、无响应事件证据和图像格式诊断探针。

分线报告：[宿主](20261020-framework-host.md)、[节点合同](20261020-node-contracts.md)、[原模块/SDK](20261020-image-binary-native.md)。

后续统一审核目标、12 道证据闸门及按工具推广顺序见[联合审核目标与防偏差闸门](20261020-audit-objectives-and-drift-controls.md)。
