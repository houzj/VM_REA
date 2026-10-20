# 图像二值化：原版模块／SDK 证据链联合审查（C 线，2026-10-20）

范围：只读检查原版 VisionMaster 4.4.0 安装资源、托管程序集、公开 SDK 头文件及既有审计证据。本文只给框架—节点联合审查提供原版边界和取证优先级；没有修改产品、运行安装目录模块或触碰 `tool-plugins`。用户截图和产品现状不代替原版源码契约。

## 身份与复验状态

原模块目录为 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/ImageProcessing/IMVSBinaryModu/`。本轮直接重算：`IMVSBinaryModu.dll` SHA-256 `C03BCB19EF456206E3ABF4D2A8D5279BE37C12EB5662E191F0965029CDABEB48`，`IMVSBinaryModuCs.dll` `7AEAA27038210D58604C4675787048050403EE2885A77B76A5AF0575952290D6`；`IMVSBinaryModu.xml` `30C47A31236E4EA48F6C30DA4FCBA33B03E17B5C3CD470C0A89C2247470DBB86`，`IMVSBinaryModuAlgorithm.xml` `EEBEE2E22C782DC0247DD5F3C62C96F27E542271B681A2D60AACD1400DF07900`，`IMVSBinaryModuAlgorithmTab.xml` `08AE81E7C8BC57B4E095AE2D0B98F08D8AFE8F34EBB065B928DD4336EF87FDD7`，`IMVSBinaryModuCs.xml` `80DF3CE60B1386F705C77425CDE34D0E15F4FB216FE749F400E7452DE5A50443`，`IMVSBinaryModuDisplay.xml` `0D0C41CFCF7897B61041AEF0B3FE8035A468C19C892054852001E7EE085F1C8B`，`ToolItemInfo.xml` `1D5200C180CDCF0E1283E366F52CC78AB61826985406A9391F9F0536129A1834`。这些与 `docs/plugins/IMAGE-BINARY-SOURCE-AUDIT.md` 所列身份一致。

SDK `MVDAlgorithmSDK/Includes/Algorithms/ImageBinaryCpp.h` 本轮 SHA-256 `51AED98FDC735FAB0808974A9E35540D41431E7E119888DCC8BDF93E67EBC851`；`ReferencedAssemblies/Algorithms/MVDImageBinary.Net.dll` `A2F413DE33E60095A51AE47D211F32ACD344DFA14DCD32A61CAA2C1E3DB09F92`；`Runtime/x64/MVDImageBinaryCpp.dll` `BE4E6B8E2A15748144F52C66E810C1479ACAAF2AE05F3739F87C919284C1EB72`。本地 `analysis/decompiled/.../ImageBinaryModuParams.cs` 与 `ImageBinaryModuTool.cs` 的哈希分别为 `B1FDBC801659E768D7B84A659FA632077BE8C80D6FC6799DB97C6ED8F113B388`、`26C706F81A37E8FEB9893BFD5580970167E9209C13E8DF4C09E33E8BA8CE5739`，与旧审计一致。

**旧审计可复验性修正：** `docs/plugins/IMAGE-BINARY-SOURCE-AUDIT.md` 引用的 `analysis/source-audit-regression/20261018/ghidra-binary/output/`、`ghidra-imagebinary-sdk/output/`、`native-binary-modes/evidence.json`、`compare.log` 和 `supplemental/RoiTool.Checks.final.stdout.log` 在当前主工作树 `analysis/` 中均不存在。因此“`ProcessAlg` 的具体调用顺序”“算法 ID `0x25a`”“单夹具像素匹配/383 像素差异”“221 项检查通过”属于**有历史文字记录但本轮无法复核原始产物**，不能作为新实现的充分依据。原始安装 DLL 和测试源码仍在；缺失的是此前生成的反编译、探针输出及日志，不等于历史执行从未发生。

REA 本轮直接只读复核托管目标：`inspect_managed_artifact` 证据 `ev_0ab04b936fe48e659487b049512013217b52c69bbb102b6969e9f3bacc46db34`，完整元数据覆盖、目标哈希与上面一致；`inspect_managed_members` 证据 `ev_10151faecb39fa6f099678a61e1c5b1d9fbee890fb4c01745b4285023d76793d`，成员覆盖标记 `partial`，未报告解析 issue。原生目标 `open_binary` 确认 PE x64 和上述 DLL 哈希；`binary_session` 显示 Ghidra `not_configured`（`GHIDRA_INSTALL_DIR` 未设置，Java/Ghidra 执行文件均不可用），IDA 亦未配置。本轮不安装、不改 provider 环境，所以没有获得新的原生函数体或指令证据。

## 逐段链路与证据边界

| 段 | 本轮直接确认 | 尚缺的调用链证据 |
|---|---|---|
| 输入及订阅 | `IMVSBinaryModuAlgorithmTab.xml` 第 6–8 行链接 `ImageSourceGroup`；原安装 `Module(sp)/Group.xml` 第 5–37 行的 `ImageSource` 用 `GetFrontParamItemsOperation(IMAGE)` 填 `EnumEntrys`，用 `GetSelectedCombinationOperation(InputImage)` 初始化选择，变化时用 `SetCombinationSourceOperation(InputImage)` 写关系。模块 XML 第 31–96 行定义必需 `InputImage`（image、宽、高、像素格式）及可选 `InputMask`、`InputROI`、`InputROIAnnulus`、Fixture、屏蔽多边形字符串。 | 这些操作的具体 CIL/宿主执行、连接变更与撤销/保存、运行时像素缓冲所有权，须由框架线连接插件线；XML 本身不证明默认连接、具体显示字符串或错误行为。 |
| 参数与五模式 | `Algorithm.xml` 给出 `BinaryType=1` 与各运行参数默认值；`AlgorithmTab.xml` 中 `BinaryType` 值 1、2、3、8、4 均标 `IsImplemented=0x1`，触发器分别控制硬阈值、均值、高斯、Sauvola、自动阈值附属字段的可见性。`IMVSBinaryModuCs.ImageBinaryParam` 的相应属性在托管 CIL 中把值交给通用参数对象；旧公共 SDK 源码缺 Sauvola 字段，不能替代当前安装模块 XML/程序集。 | UI 变更是否即刻 `SetParam`、参数应用顺序、边界非法值及运行时序列化均未由这些声明证明。五个选项可见也不证明五算法的数值公式。 |
| ROI/掩膜 | Tab XML 链接 `OutputMaskImageGroup`、`InputMaskImageGroup`、`RoiSelectGroup`、`InheritWayGroup2`、`ExternROITypeGroup`、`AssistGroup`；`ROISelecter RoiType` 声明 `Box SectorRing Polygon Sector`、最多 16 个。托管 `ImageBinaryRoiManager` 有矩形、环形、盲区多边形、MaskImage 的委托 setter。SDK `IImageBinaryTool` 公开 `SetROI`、`AddMask`、`ClearMasks`、`RemoveMaskAt`、`SetRegionImage` 和 `GetRegionImageAssistant`；头文件说 `SetROI(NULL)` 为全区、`AddMask` 仅支持多边形，并要求区域图与 ROI 配置一致。 | 原模块在何条件调用各 SDK setter、ROI 集合是逐个覆盖/合并还是另行变换、mask 与区域图先后及失败清理，必须看 `ProcessAlg` 指令/ABI；不能由接口存在推导。 |
| 执行 | `IMVSBinaryModuCs.IMVSBinaryModuTool.Run` token `0x06000007`（7 条 CIL）通过 `get_serverSDKManager` 与模块 ID 调 `SyncSelfRun`；`get_ModuResult` token `0x06000004` 调 `QueryModuleOneResult`，取得结果后再 `SetModuleResult`。REA 成员证据如上。SDK 头公开 `IImageBinaryTool::Run`、`GetResult`、`GetAlgRunTime`，并导出 `CreateImageBinaryToolInstance`/`DestroyImageBinaryToolInstance`。 | 从 `SyncSelfRun` 到模块 `Process`/`ProcessAlg` 的实际平台桥、SDK vtable 槽和通用算法入口未在本轮恢复。旧报告的 `0x25a` 为待重获原生体后确认的历史结论。 |
| 输出、错误、释放 | 模块 XML 第 99–185 行声明必需 `ModuRunTime`、`ModuStatus`、`OutputImage`，可选 `OutputMask`、ROI/盲区等；`OutputMask` 子字段 `AllowAutoSubscribeByOther=False`。SDK 头公开结果图、`IImageBinaryResult::Clear` 与销毁工具导出。 | 成功/失败分支的发布门、旧结果是否清除、错误码映射、`Clear` 与 destroy 何时调用、异常安全/所有权均未由 XML/头文件确定。原生 `ProcessOutput`/`DeInit` 及 EH/汇编需重新取证。 |

## 后续源码级逆向优先级（给框架／插件联合线）

1. **P0 原模块实际运行闭环。** 对当前哈希 `C03B...ABEB48` 的 `Init`、`SetParam`、`Process`、`ProcessAlg`、`ProcessOutput`、`DeInit`、异常 funclet 恢复完整伪代码和调用点汇编；逐分支记录输入缺失/像素格式、ROI/Mask/区域图、参数 setter、`Run`、结果提取、输出发布、错误返回及释放。每个虚调用确认 vtable 槽和寄存器／栈实参，不能把伪代码省参当真实 ABI。生成的文本/证据哈希写本工作树的新受控目录；禁止在原安装目录输出。
2. **P0 框架到节点追踪。** 从 `Group.xml` 的三个输入源操作体及原主框架连接事务追到 `InputImage` `RelateIO`、订阅候选、运行时输入组合结构；对比当前主框架连接／保存／撤销／Group 执行与插件执行上下文。重点验证来源断开、重接及上游输出变更后候选/像素值/掩膜是否同步，不以控件显示正确代替执行正确。
3. **P1 SDK 和核心算法。** 取当前哈希 `BE4E...284C1EB72` 的工厂、Run vtable、`SetRunParam`、`SetROI`/Mask/Region、结果与销毁体，再以真实调用 ID/结构追 `MVD_Algorithm.dll` 的分派及五模式函数；核对 ABI、行步长、边界扩展、相等比较、参数类型和所有权。历史 `0x25a` 及单夹具均值/高斯/OTSU 推测必须由原指令和多夹具差分重新证实；Sauvola 优先查具体分支而非继续枚举意图公式。
4. **P1 受控操作探针。** 在自有复制输入、输出路径中建立 mode×ROI×mask×像素格式×重复运行的有限矩阵，记录原 SDK／原模块两层的参数、返回码、图像哈希和释放计数；加入空/非法输入、跨轮旧结果与失败恢复。原模块可能清理工作目录，探针必须先分析副作用并使用自有受控实例；安装原件和其他任务 TempFiles 不得作服务目录。SDK 单独运行与原 VM 桌面验收分列，不能互相冒充。
5. **P2 审计证据修复。** 如旧 `analysis/source-audit-regression` 产物在其他合法审计包中找到，核哈希/原 DLL 身份后恢复可读索引；若没有，按 1–4 重新生成，不以旧报告数字作当前完成率。下一位审查者只需重用上面的固定 SHA、REA 托管 Evidence ID 和原安装 XML，即可复现本轮确认范围。

判断：当前可确定的是**输入/参数/输出的声明边界和托管运行入口**，不能确定原生调用、五种像素算法或失败释放的完整行为。图像二值化与其他节点的显著差异需要在 P0/P1 联合链路上收口；在此之前，任何从截图或单夹具公式直接修产品的改动均应标为独立兼容实现，不能称原版复刻。
