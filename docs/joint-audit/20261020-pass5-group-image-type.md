# 第五轮 J05/J06/J09/J11：Group 内图像类型传递到真实下游

## 问题与证据边界

原厂 `ImageSourceModule.xml` 的 `ImageData` 与 `ImageDataMono8` 分别是 IMAGE 组合；`IMVSBinaryModu.xml` 的 `InputImage` 是 IMAGE 组合并带像素格式叶。原 `VM.Core.dll` 的 `VmModule.GetFrontIOWithLeafItemsByName("IMAGE")` CIL（`analysis/joint-audit/VM.Core.GetFrontIOWithLeafItemsByName.il`，SHA-256 `A442D5B02176056DC4BC2BA972713B87CF12916BF54F58A1F9ECF8916C79F71C`）按组合 TypeName 和可订阅/可见筛候选，不按像素格式叶筛；原版 Binary 运行期的 RGB/Mono8 格式门仍未由该体证明。

原厂 `IMVSBinaryModu.xml` SHA-256 `30C47A31236E4EA48F6C30DA4FCBA33B03E17B5C3CD470C0A89C2247470DBB86` 将 `OutputMask.OutMask` 及宽、高、格式叶设为 `IsForce=false`、`AllowAutoSubscribeByOther=false`。同版本 `VM.Core.dll` 的 `VmProcedure` 原体 CIL 新导出于 `analysis/joint-audit/group-optional/VM.Core.VmProcedure.il` SHA-256 `8BFAB5216AC4DF8D35E670256940448E2EB19DDE7FC026C73B86DB6655DBE4CE`：`SetIoParams`（RVA `0x2FE10`）将 Group 输入/输出交给 `GetGroupIoList` 后调用 `ProcessManager.SetGroupIO`；`GetGroupIoList`（RVA `0x30E1C`）枚举所有 `DynamicVmIO` 叶，`AddFilterToList`（RVA `0x30EA4`）只写类型号和 128 字节名称。原 `IMVS_PROCEDURE_IO_INFO` 结构仅有 `nValueType` 和 `strParamName`，没有可选标志。故模块端 `IsForce=false` **并未作为这个 Group IO 声明字段直接传入 SDK**；SDK 执行时缺席值如何影响 Group 状态仍未证。

当前代码 `ImageSourcePlugin.Runtime` 在 RGB 且 `OutMono8=true` 时另发布 `ImageMono8`，而 `GraphExecutor` 对接入 `GrayImage` 的 RGB 实值报 `InputTypeMismatch`。为检验**工具和主框架交接链**而非算法数值，本轮新增 `tests/FrameworkImageTypeGroup.Checks`：实际 `ImageSource → ImageBinary` 位于 owned Group 内，Group 的 `GrayImage` 输出再连接父流程的第二个 Binary；使用同一图结构切换来源端口和源参数，最后保存/重开并用新 `ModuleExecutionScope` 执行。没有直接注入 Binary 或伪造 Group 输出。

| 状态 | body Binary | Group | 父流程真实下游 |
|---|---|---|---|
| Gray，经 `Image` | 成功并发布 Gray | 成功 | 成功并发布 Gray |
| RGB，经 `Image` | `InputTypeMismatch` | `SubprogramFailed` | `ExecutionPredecessorUnavailable` 跳过 |
| RGB，经 `ImageMono8`，转换开关关闭 | `UpstreamUnavailable` 跳过 | `SubprogramFailed` | `ExecutionPredecessorUnavailable` 跳过 |
| RGB，经 `ImageMono8`，转换开关开启 | 成功并发布 Gray | 成功 | 成功并发布 Gray |
| 保存重开上述成功图，新执行 scope | 成功并发布 Gray | 成功 | 成功并发布 Gray |

测试逐态断言真实节点状态、错误码、Binary 和父流程消费者实际 `GrayImage` 载荷。另将 Binary 的 `OutMask` 暴露为 Group 第二输出：开关关闭时 Binary 自身成功且普通 `Image` 输出存在，但当前 `WorkflowRunContext.Complete` 因 `MaskResult` 缺席把整个 Group 标为 `SubprogramFailed`，父流程连接普通 `Result` 的消费者也被跳过；开关开启时两输出具备、父流程成功。这是**当前主框架与工具可选输出交接的真实七态**，不能由原厂 XML `OutMask IsForce=false` 单独推定原厂 Group 遇到缺席输出应如何处理。

`dotnet run --project tests/FrameworkImageTypeGroup.Checks/FrameworkImageTypeGroup.Checks.csproj -c Release` 为 7/7。最终日志 `analysis/joint-audit/image-type-group/checks.log` SHA-256 `50E1118B24C03C295687D8969EFB63E2186A7F5B8F724063103A417EB8F20300`，测试源码 SHA-256 `3CC7656813F79A421051088F938AF11B9C9F81078F800D8ABF0DD61C288C57A6`。首次探针在已接线输入上再次 `Connect` 收到当前编辑器的 `Input already connected`，属于测试步骤错误；修正为先 `Disconnect` 再连接后全部通过，产品代码未改。

这证明当前主框架连接/Group/执行/保存与可选输出的上述七态，但**不**证明原厂 RGB 应自动转换、原厂 Group 可选输出语义或二值化五模式数值符合，也未覆盖输入 Mask、取消及旧结果释放。下一证据是原版 SDK 的格式门和五模式/错误输出边，以及原版 Group 对 `IsForce=false` 输出缺席时的状态；有证据后才裁定当前 Group 是否应局部保留普通输出。J05/J06/J09/J11 只关闭这些特定状态切片，整门仍开放。
