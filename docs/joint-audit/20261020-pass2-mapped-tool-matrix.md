# 第二轮：已映射节点的框架—插件联合审核矩阵

审查快照：`main` `120611182c8908334298ba47eb9244bb1f9b6307`；仅读取当前主工作区与原厂安装资源，未接触 `worktrees/tool-plugins` 在制文件。`src/VisionEditor/tool-catalog.json` 当前有 **143** 个条目、**19** 个非空 `ModuleId`、**124** 个尚未映射；19 个中 **18** 个是 `MainWindow` 构造时注册的工具插件，`IMVSGroup → workflow.subprogram` 是内建流程调用，须由框架线单审。该数字是当前未提交目录快照，不是原厂安装的工具总数，也不是 96 项实施进度。

## 审核矩阵

`已核`只表示本轮定位到当前注册和对应源码/既有审计入口；**不**表示 J01–J12 已全部关闭。原厂行为若仅有先前报告而原始产物本轮缺席，标为“历史索引”。J01 的原版文件身份、J03–J11 的完整原始链和运行状态均需逐节点复核；J12 是宿主共性门，事件已证实无响应而线程栈未取得。

| 分类 | 原目录 ID → 当前 ModuleId | 本轮已核入口 | 联合审核优先缺口/J 门 |
|---|---|---|---|
| 采集 | `ImageSourceModule` → `vision.image-source` | `ImageSourcePlugin`、图像源原/当前合同见 `20261020-node-contracts.md` | P0：`Image` 声明可连 Gray 输入而 RGB 实值失败；原像素分支/候选 J04–J05、结果切换 J08、Group/重开 J09–J11 |
| 采集 | `ImageAcquisitionModule` → `vision.image-acquisition` | `ImageAcquisitionPlugin`；`docs/plugins/IMAGE-ACQUISITION.md` 历史索引 | 设备分支未测试；八路输出实际载荷、关闭/重连与下游 J05/J09–J11 |
| 采集 | `ImageBufferModule` → `vision.image-buffer` | `ImageBufferPlugin`、原 `ImageBufferModule.xml`；本报告下文具体链 | 缓存跨运行/方案边界及可选槽位的后继 J05–J06/J09–J11；原生导出本轮缺席 |
| 采集 | `LightModule` → `vision.light` | `LightPlugin` 注册 | 设备连接/回调及失败释放 J05–J06/J10；不以模拟为原设备证据 |
| 定位 | `IMVSFixtureModu` → `vision.fixture` | `FixturePlugin`；`docs/plugins/FIXTURE-SOURCE-RECOVERY.md` | Fixture 复合输入到下游变换、实例重开与订阅 J04–J05/J09–J11 |
| 标定 | `IMVSNPointCalibModu` → `vision.n-point-calibration` | `NPointPlugin`、`docs/ISSUE-REVERSE-ANALYSIS-20261020.md` | 编辑点全量 Apply 与原字段 setter 写入粒度不同；同条件计时、取消/恢复 J03/J12；算法/输入 J05–J07 |
| 标定 | `IMVSNImageCalibModu` → `vision.n-image-calibration` | `NImagePlugin`、标定退出审计历史索引 | typed IMAGE 候选、动态叶、Group/重开 J04–J05/J09–J11 |
| 标定 | `IMVSCalibBoardCalibModu` → `vision.calibration-board` | `CalibrationBoardPlugin`；`docs/plugins/CALIBRATION-BOARD-WHOLE-TOOL-EXIT.md` 历史索引 | 原生 ABI/设备边界及组合输入消费者 J05/J07/J11；不能以 10/10 工具自审替代全宿主闭环 |
| 标定 | `IMVSCameraMapModu` → `vision.camera-mapping` | `CameraMappingPlugin`；`docs/plugins/CAMERA-MAPPING-IMPLEMENTATION-EXIT.md` 历史索引 | 订阅数组索引、输入路径、输出缓存跨方案 J04–J05/J09–J11 |
| 标定 | `IMVSMapCalibModu` → `vision.map-calibration` | `MapCalibrationPlugin`；既有 Map 退出材料 | 输入 IMAGE 初始化到输出矩阵真实消费者 J04–J05/J11 |
| 标定 | `IMVSImageCalibModu` → `vision.distortion-calibration` | `DistortionCalibrationPlugin`；既有 Distortion 退出材料 | ROI/图像像素与返回矩阵、旧结果清理 J05/J07–J11 |
| 标定 | `TranslationCalibModu` → `vision.translation-calibration` | `TranslationPlugin`；既有校准退出材料 | 复合类型/订阅/Group 输出消费者 J04–J05/J11 |
| 标定 | `RotateCalibModu` → `vision.rotate-calibration` | `RotateCalibrationPlugin`；既有校准退出材料 | 输入姿态/角度单位、执行后状态与恢复 J05–J07/J09–J11 |
| 标定 | `CoordinateModule` → `vision.coordinate-module` | `CoordinateModulePlugin`、`docs/plugins/COORDINATE-MODULE-IMPLEMENTATION-MATRIX.md` 历史索引 | IMAGE 候选初始化与坐标输出消费者 J04–J05/J11 |
| 标定 | `ReadCalibFileModu` → `vision.read-calib-file` | `ReadCalibFilePlugin` | 文件资源路径、重开后运行/错误释放 J06/J09–J11 |
| 图像处理 | `IMVSBinaryModu` → `vision.image-binary` | `ImageBinaryPlugin`；`20261020-node-contracts.md` 已有实际 Graph 四态探针 | P0：五模式 UI、仅硬阈值执行；RGB 类型断链；原生模式体、可选掩膜、实际下游 J04–J11 |
| 逻辑 | `BranchModule_STD` → `vision.branch-module` | `BranchModulePlugin`、`docs/plugins/BRANCH-MODULE-SOURCE-RECOVERY.md` 历史索引 | 真实后继顺序/筛选与 Group、撤销重开 J05–J06/J09–J11 |
| 逻辑 | `BranchStringCpmL` → `vision.branch-string` | `BranchStringPlugin`；原 ToolItem 名与 DLL 名不同，见 `docs/plugins/BRANCH-STRING-IDENTITY.md` | 字符编码/输入读取、选择后继、重开 J05–J06/J09–J11 |
| 逻辑/框架 | `IMVSGroup` → `workflow.subprogram` | `WorkflowCalls.ModuleId`、`GraphExecutor` 的 `callProcedure` 分支 | 它不是第 19 个插件。owned Group 输入/返回、循环、跨作用域候选、保存重开 J04–J06/J09–J11 由框架线处理 |

入口证据：`tool-catalog.json` 各项非空 ModuleId；`src/VisionEditor/MainWindow.xaml.cs:48` 注册 18 个插件；`src/VisionFramework/WorkflowCalls.cs:8` 声明 Group ID；`src/VisionFramework/GraphExecutor.cs` 的 `WorkflowCalls.ModuleId` 调用支路。目录 `null` 项在 `ToolNavigation` 中不能作为已实现工具拖入运行；不能把 143 当已接入数。审查表中的后续 J 项是**待关闭的具体门**，并未因存在既有插件单项检查而自动通过。

## 重点追踪：缓存图像跨运行状态链

原厂资源 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Collection/ImageBufferModule/ImageBufferModule.xml` 直接声明 `InputImage` 为 IMAGE Combination，子叶 `InImage/Width/Height/PixelFormat`，另有两个可选整数输入 `RefreshFlag`、`BufferCount`；输出 `ModuRunTime`、`ModuStatus` 和 15 个 `OutImageN` IMAGE Combination。`ImageBufferModuleAlgorithmTab.xml` 将两个整数配置绑定到 `ButtonSelecter`。这只证明**声明与配置入口**，不证明本轮已经重放原生 FIFO/异常分支。`docs/plugins/IMAGE-BUFFER.md` 历史记录的原生 `Process` RVA `0xde30`、`CopyImage` RVA `0xe6a0` 和 55 组 SDK 结果是检索索引；它引用的 `analysis/full-audit/image-buffer` 在此工作区不存在，因此本轮不得把它们标记为重新核验的源码或运行证据。

当前实际链为 `ToolNavigation.LoadToolCategory` / `MainWindow` 注册 → `GraphEditorDocument` 节点/连线 → `GraphExecutor.RunAsync` 收集连接或订阅并执行 `DataKinds.Coerce` → `ImageBufferPlugin.Runtime.ExecuteAsync`。运行体默认 `BufferCount=15`、`RefreshFlag=1`，先校验数量 `0..15`；刷新为 1 时读 `InputImage`，复制 Gray/RGB/Raw 的实际字节到 `ModuleExecutionScope.State(module,node)` 的队列，超过数量从队首弹出；不刷新则保留队列。成功时顺序发布 `OutImage0Image` 起的已占槽，另发布宽/高/格式与状态/时间；失败时发布状态/时间，但 `GraphExecutor` 在 `PreserveFailedOutputs=false` 时只记录失败，后继不见这些输出。后继连到未占的可选图像槽，`GraphExecutor` 按 `UpstreamUnavailable` 跳过。因而“原厂保留旧队列”和“当前流程中后继可见旧图像”不能画等号，须按同一运行序列分别测。

`ImageBufferWindow.cs` 的输入源用宿主 `PickSubscription`、整数可输入常量或订阅；按钮触发 `context.Run`，连续运行和关闭停止由窗口自己维护。`ModuleExecutionScope.cs:34,41,44-48` 把状态按 procedure/module/node 隔离，删除节点/替换方案时清理；`GraphExecutor` 每次创建新的 runtime，但复用 scope 中的队列。这是**当前独立实现**，与原厂保存/加载队列及跨 Group 作用域不能从同名端口直接等同。测试和历史文档声称本地 FIFO/重载通过；本轮只静态核到路径，未运行新的原厂 SDK 或 UI 实例。

可证伪的下一次操作：在自有同一方案里用 RGB/Gray 混合帧执行 `ImageSource → ImageBuffer → 二值化`，至少比较 `OutImage0` 和 `OutImage1`，切换 `RefreshFlag=0/1`、`BufferCount=0/15/16`，并在缓存未满时连空槽；随后进入 owned Group、保存重开，再运行。逐步记录 `GraphExecutor` 的输入真实 Kind、队列长度/像素哈希、每个 `NodeRunRecord` 错误码。原厂侧需重新取得与安装 DLL SHA 一致的 `Process/CopyImage/Save/Load` 源/汇编或 SDK 重放；再判断“空槽缺席”和错误后旧缓存保留是否与本地外层执行语义一致。

## 防偏差结论与交接

1. **当前能确认的范围**：目录 19/143 映射和 18+Group 注册、缓存图像原 XML 端口结构、当前 `GraphExecutor → ImageBufferPlugin` 的载荷/状态/错误路径。图像源→二值化 RGB 断链已有独立实测，不用缓存图像的静态分析替代它。
2. **不能提前关闭的门**：所有节点的 J04–J11 跨方案/Group 真实链，原厂图像格式与结果释放，二值化四个未执行模式，J12 卡死线程归因。既有 10/10 标定工具实施自审是另一明确范围，不等于这张矩阵 19 项已与原厂全域等价。
3. **下一轮最小实证包**：先用同一 ImageSource→Buffer→Binary→实际下游图完成 J05/J06/J11 的成功和失败矩阵；Group、重开、撤销扩充 J09/J10；原始 Ghidra/SDK 证据补齐后才裁定插件侧改动。框架拥有连接/调度/作用域/持久化合同，插件线拥有工具端口、运行与配置；跨边界提 FCR，由用户决定采用。

本轮是审核与证据纠偏，没有产品代码实施，也没有把任一 96 项框架单元计入新增完成。

## 追加实证：真实 GraphExecutor 的 ImageSource → ImageBuffer → ImageBinary 八态

可重跑检查：`dotnet run --project tests/FrameworkImageBufferChain.Checks/FrameworkImageBufferChain.Checks.csproj -c Release`。测试仅用合成的 2×2 Gray/RGB 帧，通过真实 `ImageSourcePlugin.SubmitImage`、`GraphExecutor`、`ImageBufferPlugin` 与 `ImageBinaryPlugin`，共用一个 `ModuleExecutionScope`/procedure ID 让缓存跨次运行保留；每次改变整数常量订阅与下游槽位。它同时断言静态 Graph 有效、每步 source/buffer/binary 状态和错误、Buffer 每个已占槽的真实 `DataKind` 与首字节，以及 `InvalidBufferCount` 的原生错误码。测试没有调用原厂 SDK、设备或 WPF 窗口，结果只属于当前实现。

| 顺序 | 输入/控制/消费者 | 本地实际结果 |
|---|---|---|
| 1 | Gray、刷新 1、数量 15 → slot0 | slot0 Gray 首字节 10，Binary Success |
| 2 | RGB、刷新 1、数量 15 → slot0 | slot0 仍 Gray 10、slot1 为 RGB 33，Binary 取旧 Gray 成功 |
| 3 | RGB、刷新 0、数量 15 → slot1 | 缓存保持 `[Gray10, RGB33]`，Binary `InputTypeMismatch` |
| 4 | Gray、刷新 1、数量 0 → slot0 | 队列清空，Binary `UpstreamUnavailable` |
| 5 | Gray、刷新 1、数量 15 → 空 slot1 | 只有 slot0 Gray，Binary `UpstreamUnavailable` |
| 6 | RGB、刷新 1、数量 16 → slot0 | Buffer `InvalidBufferCount`、native `0xe000031c`，Binary `ExecutionPredecessorUnavailable` |
| 7 | RGB、刷新 0、数量 15 → slot0 | 数量无效的上轮没有改变缓存；slot0 仍 Gray10，Binary Success |
| 8 | RGB、刷新 1、数量 15 → slot1 | slot1 RGB33，Binary `InputTypeMismatch` |

八态全部通过。日志位于 `analysis/joint-audit/mapped-tool-matrix/framework-image-buffer-chain.log`，SHA256 `3C91F51DFB127AE8A4EE4C60DAA3F2752CDB89A354F399322A4D727FCABCA2E9`。检查源码独占 `tests/FrameworkImageBufferChain.Checks/`。首次将成功 `NativeErrorCode=0` 误期望为 `null` 的断言失败，实际输出是 `gray-refresh-15-slot0: source=Success, buffer=Success//0 ...`；随后断言明确按 0 视为成功码并重跑 8/8。该失败属测试期望错误，不改产品，也不掩盖 RGB/空槽/数量 16 的真实失败记录。

本实证关闭的是**当前实现 J05/J06 的有限八态切片**。尚缺同图 Group、保存重开/撤销、原厂 SDK/原生函数重取和 UI 卡死堆栈；不能据此把整项 J05/J06/J09–J12 判为完成。
