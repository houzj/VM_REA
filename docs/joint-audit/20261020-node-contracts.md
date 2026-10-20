# 联合审查 B：图像源 → 图像二值化 → 下游节点的契约

审查快照：main `120611182c8908334298ba47eb9244bb1f9b6307`；只读检查 main 当前源码、原厂安装资源和已保存的反编译文本。未读取或修改 `tool-plugins` 在制文件；本报告没有产品改动。main 工作区已有其他未提交改动，以下均以实际文件为准，不把它们计作已提交基线。

## 1. 原版可确认的入口与数据流

原厂 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/ImageProcessing/IMVSBinaryModu/IMVSBinaryModuAlgorithmTab.xml`（SHA256 `08AE81E7C8BC57B4E095AE2D0B98F08D8AFE8F34EBB065B928DD4336EF87FDD7`）在基本参数页链接 `ImageSourceGroup`、`InputMaskImageGroup` 等组；`D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/Group.xml`（SHA256 `9129B82A0F9906DDE591347498A80E2E8C019C0FB959A721ECFD24FBBB5207DC`）定义 `ImageSource` 的 `EnumEntrys` 初始化为 `GetFrontParamItemsOperation(IMAGE)`，`CurValue` 初始化为 `GetSelectedCombinationOperation(InputImage)`，值改变时调用 `SetCombinationSourceOperation(InputImage)`。输入掩膜的 `MaskImage` 对 `InputMask` 用相同机制。

反编译 `analysis/full-audit/renderer-decompiled/Apps.XmlParser.decompiled.cs` 中 `GetFrontParamItemsOperation.Operate` 调 `VmModule.GetFrontIOWithLeafItemsByName(type)`，再写 `EnumEntrysA`；`GetSelectedCombinationOperation.Operate` 读取目标 `VmIO.SubscriptionInfo` / `GetSubscriptionInfo()` 写 `CurValueA`；`SetCombinationSourceOperation.Operate` 用 `GetIoByPathFromAround` 解析选中路径，写 `InputImage` 对应 `VmIO.RelateIO` / `RelateIndex`，空选择清 `RelateIO`。因此候选和已选文字是运行图及 IO 关系的结果，不能从截图硬编码。`GetFrontIOWithLeafItemsByName` 的内部前驱过滤和 `VmIO` 路径本体仍需取成员体/运行证据；目前只确认到这些调用边界。

已保存源码审计 `docs/plugins/IMAGE-BINARY-SOURCE-AUDIT.md` 记录原厂托管 `IMVSBinaryModuTool.Run` → `serverSDKManager.SyncSelfRun(ID)`，原生 `IMVSBinaryModu.ProcessAlg` → `IImageBinaryTool` → `MVD_Algorithm`；原厂像素算法不在该托管方法体中。已确认 XML 的五个二值化模式均已声明，不能从当前独立实现仅支持硬阈值推断原厂其他模式无效。

## 2. 当前 main 的实际链路和可复现断链

`src/VisionEditor/tool-catalog.json` 把原厂标识 `IMVSBinaryModu` 映射至 `vision.image-binary`。`ImageSourcePlugin.Descriptor` 声明 `Image: DataKind.Image`、可选 `ImageMono8: DataKind.GrayImage`，实际 `Runtime.ExecuteAsync` 的 `Image` 用 `PortValue.Image(image)` 发布；真实灰度帧返回 `GrayImage`，RGB 帧返回 `RgbImage`。`ImageMono8` 仅在 `OutMono8` 且当前帧是 RGB 时输出。

`ImageBinaryPlugin.Descriptor` 把必需输入 `Image` 和可选 `InputMask` 声明成 `GrayImage`；执行代码 `c.Input<GrayImage>("Image")`、可选掩膜 `m.As<GrayImage>()`。`GraphEditorDocument.BindInput`、`GraphValidator`、`WorkflowConfigurationSubscriptionService` 允许声明为 `DataKind.Image` 的来源连到 `GrayImage` 目标；`DataKinds.Coerce` 对图像**只原样返回值，不进行像素格式转换**。但 `DataKinds.CanFlow(RgbImage, GrayImage)` 为 `false`：当图像源实际发布 RGB 帧，`GraphExecutor` 在运行时输入检查记录 `InputTypeMismatch`，**不会进入二值化插件**。若绕过图执行器直接以 RGB 值调用插件运行时，`PortValue.As<GrayImage>()` 的异常由插件捕获并返回 `InvalidRoiOrInput`；这与图执行器结果码是两条不同路径。故 main 当前的断链是**静态连线可建、RGB 实值运行不通过**，不是 `ModuleException`。`InputMask` 存在相同的声明类型与实际帧类型分离风险。不要凭此直接加自动转灰度：先取得原版 `InputImage` 像素格式校验/转换、`InputMask` 校验和错误码证据。

`WorkflowConfigurationSubscriptionService` 只枚举显式 `ExecutionLinks` 的启用祖先、`CanSubscribe` 端口，并按本地 `DataKind`/`IMAGE` 映射过滤；不支持 Group body、跨流程/全局/局部变量输入等范围（见其 `UnsupportedCapabilities` 与 `Snapshot`）。因此 UI 下拉候选、既有图形连线、执行来源是三种不同状态，必须分别核查。`ImageBinaryWindow.InputRow` 走宿主 `InputLabel`/`EditInput`；预览 `InputImage()` 仅接受 `PreviewInput("Image")` 值中的 `GrayImage`。RGB 上游即使图连线有效，窗口预览也可能为空，且与执行失败同源。原厂候选能否包含 RGB 上游，以及何时显示 `图像`/`灰度图像` 两个端口，尚需运行差分。

下游：二值化成功时固定发布 `Image: GrayImage`、`ProcessedPixels: Integer`，仅在 `OutputMaskImage` 时发布 `OutMask: GrayImage`。后继连接到可选 `OutMask` 在 `GraphValidator` 可静态通过；若开关关闭，`GraphExecutor` 后继找不到该输出而记 `UpstreamUnavailable`。这是可解释的本地条件输出，但原厂关闭开关时下游订阅/错误/跳过语义尚未对照。其他插件对 `Image` 的实际格式要求和输出显示选择未逐个审核，本报告不能把“二值化输出可连”推广为全目录下游闭环。

## 3. Group 与保存重开：已有覆盖和盲区

`tests/RoiTool.Checks/Program.cs` 构建 `vision.image-file` → 二值化的 owned Group，验证 ROI 代理修改会改变实际灰度像素、撤销恢复；`tests/RoiTool.Checks/RealBinaryHostChecks.cs` 验证窗口中选择输入订阅、运行、ROI、显示设置，并单独做显示样式 JSON 保存重读。这些是当前托管实现的有效局部验证。

未找到覆盖以下**同一方案端到端矩阵**的既有检查：真实 `vision.image-source` 的 RGB/Gray 两格式 → 二值化输入选择与预览 → 掩膜开关及可选输出下游 → owned Group 中相同连线和作用域 → `SolutionStore.Save/Load` 后从重新实例化的 `GraphEditorDocument`/`SolutionExecutor` 执行。现有 Group 检查使用 `vision.image-file` 灰度夹具，无法排除前述 ImageSource 多格式断链。`SolutionStore` 保存节点、`Connections`、`ExecutionLinks`、`Configuration`、ROI 等文档结构并做结构校验；反序列化成功本身不证明重新创建宿主后的订阅候选、预览、执行或原版 `.sol` 格式等价。

## 4. 后续逆向和联审行动，按证据门槛排序

1. **P0 图像格式合同**：取原版 `IMVSBinaryModu` 的 `InputImage`/`InputMask` 参数接收、像素格式分支、错误码和 ImageSource `Image`/`ImageMono8` 输出路径的原生函数体/必要 ABI；分别给 RGB、MONO8 和可选灰度输出连接做原机或 SDK 受控探针。确定原版是拒绝、转换还是仅在 UI 层过滤后，再决定 main 的 descriptor、验证、预览和执行修改。
2. **P0 候选与连接状态**：以相同流程拓扑核对原版 `GetFrontIOWithLeafItemsByName(IMAGE)` 的前驱/叶输出、启用状态、Group 边界和连接变化通知；同时查 `GetSelectedCombinationOperation` / `SetCombinationSourceOperation` 的路径名与 `RelateIO` 清除。当前桥的 `ExecutionLinks` 祖先假设需以这些源码/运行证据确认。
3. **P1 下游条件输出**：对 `Image`、`OutMask`、`ProcessedPixels` 接两类实际后继，切换 `OutputMaskImage`、运行失败/NoMatch、删除上游、重连和撤销；比较 VM 的订阅可见性、运行状态与结果释放。记录每个消费者的真实像素类型，不凭 `IMAGE` 名称推断。
4. **P1 Group + 持久化闭环**：在同一方案验证普通流程和 owned Group 各一条 ImageSource → Binary → 后继链，保存/重开/再运行、Undo/Redo、复制/删除后的端口身份和 ROI/掩膜关系；JSON 本地保存与原版 `.sol` 另列，不混为同一验收。
5. **P1 算法模式**：五种原厂模式逐一追 `ProcessAlg` 参数 → SDK/`MVD_Algorithm` 入口 → 输出/失败语义。现有单夹具的均值/高斯/OTSU 像素候选匹配不足以上线，Sauvola 仍未匹配；模式差异必须保持显式限制。

跨项目建议：框架线负责 `DataKinds`/端口连接/订阅/Group/保存重开与执行状态；插件线负责 ImageSource 与 ImageBinary 各端口实际图像格式、预览和算法/ROI/结果。需变更共享契约时先写 FCR，说明 producer/consumer/payload/lifecycle/兼容及上述矩阵证据，由用户决定合流；本报告未修改另一工作树或宣布原生等价。
