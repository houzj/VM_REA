# C03 原厂 Blob 运行差分取证

## 取证目标

对与自研 C02 相同的 ImageSource 图像输入，取得原厂 Blob 的 `BlobStatus`、`BlobNum`、`BinaryImage` 和 `BlobImage` 元数据，并保留输入参数、模块 ID、执行顺序和失败状态。

## 固定输入

- 输入图像：`analysis/images/circle-find-sample.bmp`
- 原厂模块：`IMVSBlobFindModu`
- 原厂 XML 身份：`IMVSBlobFindModu.xml` SHA-256 `F1B2F279717DCE3EC61C11C41F858BD100359095BCF13672422A3C73EFE47F6C`
- 先使用 `ThresholdType=BinaryImage`、`Connectivity=8`，避免把阈值算法差异混入第一轮。

## 必须保留

- 原厂参数快照和模块/流程 ID
- `Server.log`、`PlatformSDK.log`、`ModuleFrame.log`、`Ui.log` 对应时间窗
- `BlobStatus`、`BlobNum`、每个结果字段数组
- `BinaryImage`、`BlobImage` 的宽、高、像素格式、字节长度和 SHA-256
- 空结果和输入失败的错误码

## 当前状态

已获得原厂 Blob 参数窗口的桌面 AX 证据，并取得一张绑定有效输入且结果表非空的原厂运行截图。当前仍未取得可导出的精确字段数组和图像字节快照。当前 AX 树确认：

- 参数窗口包含 `Tab_Basic Params`、`Tab_Run Params` 和 `ResultShow` 三个页签。
- 结果显示包含 `Tab_Check Result`、`Tab_Image Display` 和文本显示分组。
- 图像显示下拉项包含 `Blob分析1.显示Blob图像`、`Blob分析1.显示二值化图像`、`图像源1.输出图像`、`图像源1.灰度图像数据` 和 `Blob分析1.输出掩膜`。
- 当前结果表头包含序号、面积、周长、质心X、质心Y、角度、长轴、短轴和圆形度。
- 参数窗口提供 `执行`、`连续执行`、`停止执行` 和 `确定` 操作。

已观察到用户点击 `执行` 后参数窗口关闭，主界面回到流程画布。该次 AX 状态显示 ImageSource 图像队列为 `0/0`，因此没有可确认的 Blob 输入帧，历史结果表也未出现可读的 Blob 数值。日志目录中未发现新的 Blob 字段日志。

`runtime-result-not-captured`。这条状态只标记早期一次空队列执行尝试。该次执行动作已发生，但因 ImageSource 当前没有可用图像队列，不能把它当作同一 BMP 输入的原厂运行快照。后续用户截图已证明有效输入下存在非空结果表，但仍未提供可导出的字段数组和图像字节。

## 用户提供的有效运行截图

用户随后提供了原厂 Blob 参数与结果截图 `D:/Temp/codex-clipboard-4b71a880-0d32-4185-9bd7-348c3eb61046.png`，SHA-256 为 `0760FEFB82BC072985A26E78B7CC2B185B05EDD0ED896F692296CC8E75D8698F`。

该截图确认：

- Blob 参数窗口标题为 `1 Blob分析`。
- `输入源` 已绑定为 `0 图像源1.图像`，不再是空输入。
- 结果区显示非空的 Blob 表格，能看到面积、质心 X/Y、角度、长轴等列和多行数值。
- 结果显示区存在 Blob 图像缩略图，说明结果图像显示路径已被触发。

该截图是 `runtime-confirmed-original-ui` 证据，仍不单独证明完整 BlobNum、BinaryImage/BlobImage 原始字节或所有字段数组的精确值。

用户随后提供了两张参数页截图：

- 基本参数页 `D:/Temp/codex-clipboard-45c0fbbb-bbfa-4e7e-a218-47af502f5126.png`，SHA-256 `9D76235371B65D35CB01FB189E5BB4A6F5CF0BBE1C9E23110B14249252B94120`。可见输入源为 `0 图像源1.图像`，并显示 ROI 类型、ROI 创建、屏蔽区和位置修正入口。
- 运行参数页 `D:/Temp/codex-clipboard-387f6526-d712-4f34-87e5-0e6a977eada4.png`，SHA-256 `CEDF47C9FC138FEB1CF4CD9C3B06EF0DCFBCC09FE12E3171E46E2821C9483C2A`。可见阈值方式为单阈值、极性为亮于背景、低阈值为 100、查找个数为 100、面积范围为 10 到 9999999，并显示轮廓、Blob 图像和二值化图像输出控制。

这两张图把原厂当前参数状态补齐为 `runtime-confirmed-original-ui`，算法结果数值仍单独计量。

其他用户截图进一步覆盖了原厂高级运行参数和结果显示配置：

- 运行参数截图 `codex-clipboard-fc37d201-f626-45e7-9bee-6780da37974f.png`，SHA-256 `DEEBC73047D7117A37FDAB712C1D74484D480D8BA592D085D45CC8C40F3FC950`。可见直方图信息、角度、矩形宽高、周长、长短轴、圆形度、矩形度和质心偏移等能力开关。
- 运行参数截图 `codex-clipboard-33538bd3-4cff-433f-bcc0-893672ae4c90.png`，SHA-256 `276A62FEF70C7D52EF1654CD76B5D616CB1354177B05138FA0BA242876716E76`。可见轴比范围、排序特征为面积、排序方式为降序、连通性为 8 连通以及最大内接矩形和像素超限控制。
- 结果显示截图 `codex-clipboard-eca6f675-d36c-42f2-8d60-eb124c7e88d3.png`，SHA-256 `5715F9C8B819E66893819E7454E9B7C3DF4C319B9D241C9958482F8412BDA94C`，以及 `codex-clipboard-d0b8d79e-760b-493a-a412-1178395e3622.png`，SHA-256 `2CFC9330296B0D94023080D9CE71BE74BB82DDC12C9C3D9B54903B7F06F50C30`。可见 Blob 数量、Blob 个数判断、Blob 面积判断、Blob 总面积、质心 X/Y、Box 角度、中心 X/Y、Box 高宽、周长、长短轴和圆/矩形度判断项。
- 结果显示截图 `codex-clipboard-92ae7554-4277-4e1e-927a-5da8a4939a29.png`，SHA-256 `2A76E38FD0DCEE9F1DB721634CFEE07AB22E15C70232206A3AAD5BC7C2149623`，确认图像显示对象包括 Blob、最大内接矩形、检测区域、轮廓点、文本、屏蔽区多边形和扇环检测区域。
- 文本显示截图 `codex-clipboard-5958a20a-7db9-43bb-982b-273faa9ab21d.png`，SHA-256 `C0133654CF0811085A1CE5C425498B01D234CDDE103279197277005780568580`，可见内容模板 `Blob面积:{面积}`、OK/NG 颜色、字号 10、透明度 1 和位置 X/Y 参数。

这些截图将 C03 的原厂 UI/参数证据扩展到高级筛选、排序、结果判定、图像显示和文本显示层，但仍不替代原始结果数组或图像字节证据。

## 已确认的精确取证路径

源码已经把缺失的运行结果边界定位到 SDK 结果回调，而不是参数窗口。`ImvsSdkPFSync.cs` 的模块类型 `7u` 分支从
`stResModuInfo.pData` 读取 `IMVS_PF_BLOBFIND_MODU_INFO`，以 `stBasicModuInfo.nModuleID` 写入
`mBlobFind.Results`。同步路径调用 `BlobFindResult(IMVS_PF_BLOBFIND_MODU_INFO)`，该构造函数会按 `iBlobNum` 深拷贝
`pstSingleBlobInfoPtr`，并按 `stBinaImage.iImgDataLen` 和 `stBlobImage.iImgDataLen` 深拷贝两段图像缓存。异步路径先把
原生结构体交给回调，再清空 `mBlobFind.Results`。因此下一次精确采集应在同步结果对象仍可读的时间窗内记录模块 ID、
`iModuStatu`、`fTotalArea`、`iBlobNum`、单体数组、两段图像的尺寸/格式/长度和 SHA-256，并单独记录异步回调清理后的
空缓存行为。

对应一级证据为 `analysis/decompiled/iMVS-6000PlatformSDKCS.PublicAudit/iMVS_6000PlatformSDKCS.SyncPlatformSDKCS/ImvsSdkPFSync.cs`
（模块类型 7 分支）、同目录 `BlobFindResult.cs`（深拷贝和释放路径）、以及
`analysis/decompiled/iMVS-6000PlatformSDKCS.PublicAudit/iMVS_6000PlatformSDKCS/ImvsSdkPFDefine.cs` 中
`IMVS_PF_BLOBFIND_MODU_INFO` 和 `IMVS_PF_SINGLEBLOB_INFO` 定义。该路径是下一项 SDK/ABI 取证动作，不把截图中的可见
数值臆测为完整结果数组。

托管同步结果构造器的六类深拷贝规则已单独固化在 `analysis/full-audit/batch1-20261107/C03/original-blob-managed-copy-contract.json`。它明确记录了单体数组、二值图像、Blob 图像、轮廓点、检测区域和圆弧数组的计数条件、元素类型、分配和析构释放路径。该合同仍是 source-confirmed 生命周期证据，不包含本次 VM 运行的实际指针、长度或字节。

## 相邻独立实现证据

扫描到另一套自研工程留下的 Blob 参数页和运行测试日志，例如 `D:/temp/blob-probe-gray.log` 与
`D:/temp/canonical-final-blob.log`。这些日志显示了独立实现的参数页面组装和 11 项运行测试通过情况，不能证明
VisionMaster 原厂算法或结果字节，因此只作为自研工具链参考，不纳入 C03 原厂差分和完成百分比。

## 2026-11-08 REA 原生鉴别

对原厂 `IMVSBlobFindModu.dll` 的 REA 鉴别确认该文件是 x86_64 原生 PE，CLI 目录不存在，因此没有可由托管分析器读取的 CIL 或托管元数据。原生深度分析当前因 Ghidra 和 IDA 未配置、Hopper 不支持当前 Windows 主机而不可用。完整诊断和证据 ID 记录在 `analysis/full-audit/batch1-20261107/C03/rea-native-provider-diagnostic.json`。该结果确认当前取证边界，不替代原生函数体、ABI 或结果缓冲区证据。

另外通过 PE 导出目录取得了原厂 Blob 模块的 16 个导出符号。`Process`、`ProcessAlg` 和 `ProcessOutput` 分别位于 RVA `0x1FA80`、`0x1FEC0` 和 `0x20310`，与已保存的 native decompilation 边界相互对应。完整名称、序号和 RVA 见 `analysis/full-audit/batch1-20261107/C03/original-blob-native-export-table.json`。导出目录只能确认入口身份，不能推出 ABI 寄存器、结构体偏移或结果字节。

新增的只读映射探针使用 `DONT_RESOLVE_DLL_REFERENCES` 成功映射原厂 DLL，并解析 `CreateModule`、`DestroyModule`、`Process`、`ProcessAlg` 和 `ProcessOutput` 五个符号。该探针不执行 `DllMain`，不解析依赖，也不调用任何模块入口。运行结果见 `analysis/full-audit/batch1-20261107/C03/original-blob-native-map-probe.json`，可作为后续受控 native 调用前的加载前置证据。

随后使用 `LOAD_WITH_ALTERED_SEARCH_PATH` 做正常代码加载探针，Windows 返回错误 126。静态字符串显示其依赖包括 `MVDBlobFindCpp.dll`、`MVDImageCpp.dll`、`MVDPositionFixCpp.dll`、`MVDPreproMaskCpp.dll` 和 `MVDShapeCpp.dll`，这些依赖未出现在当前探针搜索目录，因此没有调用 `CreateModule`。结果见 `analysis/full-audit/batch1-20261107/C03/original-blob-native-factory-probe.json`。该失败解释了为什么当前环境不能直接取得 native 结果，不等同于原厂模块本身无法运行。

将原厂 `MVDAlgorithmSDK\\Runtime\\x64`、`Applications\\ModuleProxy\\x64` 和 `Applications\\PublicFile\\x64` 加入 DLL 搜索目录后，加载成功并实际进入 `CreateModule`。独立进程在 `CreateModule` 调用点前后以 `0xC0000005` 退出，未返回模块指针，也未到达 `DestroyModule`。这确认 factory 需要完整的 VM 初始化上下文或额外 ABI 前置条件，但不能把该崩溃解释为原厂运行失败。阶段标记和结果已更新到 `analysis/full-audit/batch1-20261107/C03/original-blob-native-factory-probe.json`。

同时保存了三个入口各自 64 字节的原始文件字节、PE 映像地址、文件偏移和 SHA-256，见 `analysis/full-audit/batch1-20261107/C03/original-blob-native-entry-bytes.json`。这些字节是 shipped-artifact 证据，未进行未经工具支持的反汇编解释。

REA 的 PE 资源解析还确认该 DLL 只有一个完整的 RT_MANIFEST 资源，语言为 1033，内容声明 `asInvoker` 执行级别。资源不包含结果 schema、参数表或图像缓冲区合同。详情见 `analysis/full-audit/batch1-20261107/C03/original-blob-pe-resource-contract.json`。

对原厂 `IMVSBlobFindModuCs.dll` 做了完整托管成员和调用边检查。`IMVSBlobFindModuTool.Run` 调用 `serverSDKManager.SyncSelfRun`，`get_ModuResult` 调用 `QueryModuleOneResult`、`SetModuleResult` 并构造 `BlobFindResult`。这证明原厂托管包装层不直接裸调 `CreateModule`，而是依赖 ServerSDK manager 的初始化和结果缓存链。该合同见 `analysis/full-audit/batch1-20261107/C03/original-blob-managed-wrapper-contract.json`，也解释了独立 factory 探针在原厂 VM 外崩溃的边界。

进一步恢复了 `SyncSelfRun` 的前置条件、模块 ID 范围检查、native `IMVS_SelfExecuteOneModule` 调用、状态检查、范围专属完成标志等待，以及 `QueryModuleOneResult` 对 native 历史结果列表的单结果筛选。完整调用合同见 `analysis/full-audit/batch1-20261107/C03/original-blob-sync-self-run-contract.json`。下一项运行时取证应围绕已初始化的 `CVmObjectBase.mhandle` 和完成回调，而不是直接构造 native factory。

完成标志来源也已闭环。`GetReportModuleStatusCallBackInfo` 只在 `bIsSelfRun == 1` 时，按模块 ID 范围设置对应的 self-execute flag，然后触发 `OnReportModuleStatusCallBack`。这发生在 `SyncSelfRun` 的 `PrecisionWait` 返回之前，随后才进入 Blob 历史结果查询。该顺序和取证窗口见 `analysis/full-audit/batch1-20261107/C03/original-blob-self-run-completion-callback-contract.json`。

回调线上的 native wire contract 也已恢复。平台事件码 `825` 在 `CVmObjectBase` 中解码 `IMVS_REPORT_MODULE_STATUS_INFO_LIST_EX`，复制 `nNum` 条记录，再由 `ModuleManager` 更新完成 flag。每条记录包含模块 ID、过程/模块状态、模块/算法/过程耗时和 `bIsSelfRun`。字段和事件顺序见 `analysis/full-audit/batch1-20261107/C03/original-blob-report-callback-wire-contract.json`。

`CVmObjectBase` 的初始化链也已定位。它根据配置走 `IMVS_CreateHandle` 或 `IMVS_PF_CreateHandle`，可能启动 GlobalScript，然后把 `delegateOutputCallBackFunc` 注册到 `IMVS_RegisterOutputPlatformInfoCallBack`。因此 `CVmObjectBase.mhandle`、回调注册和全局脚本准备都是 Blob native factory 之前的必要上下文。合同见 `analysis/full-audit/batch1-20261107/C03/original-blob-platform-handle-init-contract.json`。

按原厂托管声明重建并用 `Marshal.SizeOf/OffsetOf` 验证，单条状态记录托管顺序布局为 28 字节，`bIsSelfRun` 位于偏移 24；列表头托管顺序布局为 32 字节，状态记录指针位于偏移 8。该布局基线见 `analysis/full-audit/batch1-20261107/C03/original-blob-report-callback-managed-layout.json`，不把托管布局直接当成未经验证的 native packing。
## 2026-11-08 原厂模块结果页截图

用户补充了 9 张原厂 `模块结果` 页截图，证实当前运行结果包含 45 个 Blob、总面积 971486、二值化图和 Blob 图像均为 1280×960、像素格式 17301505，检测区域中心为 640/480，尺寸为 1280×960，角度为 0。截图还覆盖了质心、最小外接矩形、Blob 矩形框、周长、长短轴、圆形度、矩形度、分数、ROI 圆弧、轮廓点集、屏蔽区多边形和输出掩膜字段。

截图哈希和逐张证据范围见 `analysis/full-audit/batch1-20261107/C03/original-blob-runtime-result-screenshot-evidence.json`。横向显示区域截断了大量长数组，因此完整 45 元素向量和原始图像字节仍未闭合。

后续 3 张全屏截图又确认了同一运行的当前结果表、历史结果和帮助页。历史结果记录时间 `2026-11-08 12:46:07.866`，模块数据行显示 `Blob信息44`、`Blob信息43`、`Blob信息42`、`Blob信息41`，并包含面积、周长、质心和角度字段。帮助页显示原厂 Blob 的操作步骤为连接输入图像、设置 ROI、设置算法参数、运行。对应哈希和范围已追加到同一份截图证据 JSON。

全屏结果表中前 3 行已逐字段转录到证据 JSON。第 0 行面积 716928、周长 11217.29、质心 632.207/383.7975、角度 0、长短轴 1279/701、圆形度 0.4195392、矩形度 0.7996261。第 1 行面积 252375、周长 4794.603、质心 633.6377/860.7609、角度 0、长短轴 1279/217、圆形度 0.1881841、矩形度 0.9093186。第 2 行面积 428、周长 152.5685、质心 1131.661/692.3037、角度 12.33909、长短轴 68.58927/11.3185、圆形度 0.09263133、矩形度 0.5513134。

## 2026-11-08 本地结果差分边界

现有本地纵向切片报告记录了 Blob 个数 `1`、总面积 `1327102`。原厂截图记录了 Blob 个数 `45`、总面积 `971486`。两组值是已观察到的差异，但当前证据没有证明输入图像、ROI、阈值、极性、连通性或预处理完全相同。原厂截图没有暴露源图像身份，本地报告使用 `analysis/images/circle-find-sample.bmp`，因此不能把该差异归因于算法实现。边界和下一项取证动作已固化在 `analysis/full-audit/batch1-20261107/C03/original-vs-local-visible-differential.json`。下一步必须先取得同一输入和参数的本地运行轨迹，再比较完整结果向量和图像字节。

## 2026-11-08 安装样例输入核验

在原厂安装目录 `D:\\Program Files\\VisionMaster4.4.0\\MVDAlgorithmSDK\\Samples\\Data` 发现 `IMG_01.jpg` 至 `IMG_04.jpg` 四个随附样例。四者均为 1280×960，与原厂截图的图像元数据一致。对这四个 shipped inputs 做了独立的 8 连通、阈值 100、最小面积 10 探针。亮于阈值的区域数和面积依次为 `61/644744`、`91/615072`、`89/606957`、`72/508381`，暗于阈值的区域数和面积依次为 `112/658339`、`98/688263`、`130/696235`、`122/796617`。没有一个直接得到原厂截图中的 `45/971486`。这说明尺寸匹配不能证明选帧匹配，原厂运行还需要具体极性、ROI、预处理或其它参数状态。原始探针输出见 `analysis/full-audit/batch1-20261107/C03/installed-vm-sample-pixel-probe.json`，探针代码保留在 `tests/NativeEvidence/C03/InstalledSamplePixels.cs`，其结果属于 independent-implementation，不替代原厂运行证据。

## 2026-11-08 原厂 UI 实际输入文件的本地基线

原厂 `Ui.log` 明确记录了本次运行载入的 `D:\MyWork\图片库\标定\1.jpg` 至 `12.jpg`，每个文件均为 1280×960。已对这 12 个文件逐一执行本地图像源到 Blob 的闭环，文件 SHA-256、会话状态和结果写入 `analysis/full-audit/batch1-20261107/C03/original-input-local-blob-baseline.json`。

探针结果全部为本地会话成功，但每帧输入的本地解码灰度最小值为 51 至 55，非零像素为 1228800，Blob 结果因此均为 `1 / 1228800`。这不是原厂 `45 / 971486` 的算法差分结论，而是当前本地 Blob 只实现了 `ThresholdType=1` 的独立切片，且直接对原始灰度图做连通域。原厂运行时参数导出确认默认 `ThresholdType=2`、`LowThreshold=100`、`HightThreshold=150`，因此同一输入的可比运行仍需先恢复原厂单阈值模式的极性和阈值处理，再比较 Blob 向量。

## 2026-11-08 同一输入的阈值方向探针

对 `1.jpg` 至 `12.jpg` 做了独立的 8 连通、最小面积 10 像素探针，结果见 `analysis/full-audit/batch1-20261107/C03/original-actual-input-pixel-probe.json`。以原厂当前页显示的 `1.jpg` 为例，亮前景、阈值 100 的包含边界值规则得到 `47 / 973204`，最大三个区域为 `718424、252563、428`。原厂截图得到 `45 / 971486`，最大三个区域为 `716928、252375、428`。暗前景在阈值 100 下得到 `159 / 250869`，方向明显不符当前原厂结果。

这项结果把极性候选收窄到亮前景、阈值约 100 的路径，但仍属于 independent-implementation。差异可能来自原厂 JPEG 灰度转换、ROI/边界处理、连通域筛选或其它未恢复参数，尚不能据此修改产品实现或宣称单阈值公式已闭合。

同一探针随后用本地 TurboJPEG 解码后的像素做了独立 `brightInclusive` 变体。对 `1.jpg` 得到 `47 / 973202`，最大三个区域为 `718420、252568、428`，与 System.Drawing 结果仅有个位数像素差异，说明主要差异已收窄到原厂处理链中的阈值边界、ROI/边缘和筛选规则，而不是输入文件身份。

## 2026-11-08 SDK 接口和运行参数契约

原厂 SDK 头文件与参数 XML 进一步确认了执行顺序。`IBlobFindTool` 先接收 `SetInputImage` 和可选 `SetROI`，再通过 `SetRunParam` 设置运行参数并调用 `Run`，结果通过 `GetResult`、`GetBlobNum`、`GetBlobInfo` 和 `GetBlobTotalArea` 读取。完整来源哈希和调用链记录在 `analysis/full-audit/batch1-20261107/C03/original-blob-sdk-runtime-contract.json`。

`BlobFind.xml` 明确把 `ThresholdType=2` 定义为单阈值，把 `Polarity=0` 定义为目标物体亮于背景，连通性默认 8。当前 VM 的运行时导出覆盖了这些字段，并把阈值改为 100/150。XML 和头文件没有给出 native 像素比较、ROI 边缘或筛选排序的实现公式，因此这些仍需保留为未决项。

## 2026-11-08 原厂 ProcessAlg 调用链证据

已从保存的原厂 native 反编译函数 `CIMVSBlobFindModu::ProcessAlg` 恢复 Blob 算法包装层的操作顺序：输入校验 → 创建或复用缓存的 SDK Blob 实例 → 设置输入/区域状态 → 按模块模式选择直接区域或 `GenerateMaskImage` → 添加启用的屏蔽区 → 调用 SDK 运行 → 从 `GetResult` 读取结果并写入模块输出。SDK 创建失败、mask 生成失败和空结果分别有明确错误边界；SDK 实例在模块上缓存。

这条证据把本地还原的第一层目标从“得到相近 Blob 数量”改为先保证调用链和失败边界一致。反编译调用点能确认操作顺序，但 SDK vtable 槽位、寄存器 ABI、灰度/阈值公式和 ROI 栅格化仍未被 native 级验证。完整证据卡见 `analysis/full-audit/batch1-20261107/C03/original-blob-processalg-call-chain-contract.json`。

## 2026-11-08 原厂 SDK C++ ABI 直接运行证据

本机存在可用的 MSVC x64 编译器和原厂 SDK x64 导入库。使用原厂 `BlobFindCpp.h`、`MVDImageCpp.h`、`GlobalInitCpp.h` 编译 C++ 探针，并把 SDK Runtime、ModuleProxy、PublicFile 目录加入搜索路径后，直接调用原厂 `CreateGlobalInitToolInstance`、`CreateBlobFindToolInstance`、`IMvdImage::InitImage`、`IBlobFindTool::SetInputImage/SetRunParam/Run` 和结果接口成功返回。

对 `D:\MyWork\图片库\标定\1.jpg`、阈值方式 2、低阈值 100、高阈值 150、亮于背景、8 连通、最小面积 10、最大面积 9999999、查找数 100 的运行结果为 `47 / 973202`。完整 47 个 Blob 字段向量以及二值图/Blob 图原始缓冲区已落盘，证据见 `analysis/full-audit/batch1-20261107/C03/native-sdk-run/original-sdk-direct-cpp-runtime-evidence.json`，两个图像缓冲区各为 1280×960、MONO8、1228800 字节。

该结果与本地 bright@100 探针逐项一致，但与原厂 VM 截图的 `45 / 971486` 不一致。由此可以排除“本地算法只是猜测”的解释：本地当前切片已经复现原厂 SDK 直接调用结果；剩余差异位于 VM 外层的输入图像进入方式、ROI/屏蔽区、运行区域图像、筛选/排序或其它模块参数状态。原生 SDK 直接运行本身已不再是取证阻塞点。

## 2026-11-08 原厂 SDK 12 帧输入扫查

对 `Ui.log` 记录的 `1.jpg` 至 `12.jpg`，使用同一套原厂 x64 C++ SDK、同一组运行参数和同一 ABI 逐帧执行。结果依次为：`47/973202`、`50/977000`、`45/986444`、`51/964047`、`61/962495`、`72/954959`、`70/994718`、`62/997593`、`46/987481`、`82/944158`、`52/961870`、`70/983259`。没有任何一帧直接得到 VM 截图的 `45/971486`；第 3 帧虽然 Blob 数量为 45，但总面积不同。

因此可以排除“只要换成日志中的另一帧就能复现”的解释。当前差异继续定位在 VM 传入 SDK 前后的输入/运行区域、ROI/屏蔽区、筛选或其它状态。逐帧证据见 `analysis/full-audit/batch1-20261107/C03/native-sdk-12/original-sdk-12-input-sweep.json`。

## 2026-11-08 ModuleFrame 参数与 SDK 参数边界

把原厂 `ModuleFrame` 运行时导出的 108 项参数逐项通过正确 C++ ABI 送入 `IBlobFindTool::SetRunParam` 后，只有 41 项被 SDK 接受，67 项抛出参数异常。被拒绝的项目主要是 `SelectBy*`、结果判定、LimitJudge、输出和 VM 运行时专用字段；它们属于模块包装层或结果判断层，并不是 SDK 的直接运行参数。

这证明不能把 108 项导出字符串盲目重放到 SDK 来解释 VM 结果。直接 SDK 仍返回 `47/973202`，VM 截图为 `45/971486`；差异需要继续追踪模块 wrapper 对这些字段的映射、LimitJudge 和运行区域/Mask 生命周期。边界证据见 `analysis/full-audit/batch1-20261107/C03/native-sdk-runtime-108/sdk-vs-moduleframe-parameter-boundary.json`。

## 2026-11-08 原厂 Blob 包装层 SetParam 与 LimitJudge 映射

新增源级证据 `analysis/full-audit/batch1-20261107/C03/original-blob-setparam-limitjudge-wrapper-mapping.json`。`CIMVSBlobFindModu::SetParam` 将阈值、排序、连通性、输出限制等一组字段转发到缓存 SDK 工具；`SelectByCircularuty` 会翻译为 SDK 的 `SelectByCircularity`，`MinCenterBias/MaxCenterBias` 会先转换为整数文本并转发为 `MinCenterBiasEx/MaxCenterBiasEx`。Blob 数量、总面积、质心、角度、中心、外接框、周长、长短轴、圆形度、矩形度等 Enable/Low/High 限制保存在模块状态，由 `LimitJudge` 在 `ProcessAlg` 之后、`ProcessOutput` 之前逐 Blob AND 汇总到判定缓冲区。由此确认 `ModuleFrame` 导出的 108 个字段不是 SDK `SetRunParam` 的一对一参数面，不能整体直接重放给 SDK。

对应原始函数体：`analysis/full-audit/b08-blob-low-20261030/output/0x180022500-180022500.c`、`analysis/full-audit/b08-limitjudge-20261103/output/0x18001f720-18001f720.c`。具体字段、状态偏移和错误码保存在上述 JSON；SDK vtable 的最终 ABI 名称仍标记为未决。

## 2026-11-08 ProcessAlg 的 ROI/Mask 分支与 ProcessOutput 边界

对保存的 `ProcessAlg` 和 `ProcessOutput` 函数体重新核对后，确认差异位置可以进一步收窄。`ProcessAlg` 先把输入图像送入缓存 SDK 工具，再按模块模式选择直接使用输入结构中的运行区域，或调用 `GenerateMaskImage` 生成运行区域并追加所有启用的屏蔽区；随后才调用 SDK 运行。`GetResult` 为空时抛出 `0xe0000315`。`ProcessOutput` 在结果已经存在之后读取 Blob 数量和总面积，再复制 BlobStatus、几何数组、灰度/评分字段和可选的二值图/Blob 图。因此 `45/971486` 与 `47/973202` 的核心差异必须优先检查 SDK 运行前的 `+0x118` 区域对象、生成的运行区域图像和启用屏蔽区，不能归因于结果复制阶段。

结构化证据见 `analysis/full-audit/batch1-20261107/C03/original-blob-processalg-roi-output-contract.json`。这里仍保留 vtable 槽位符号和生成运行区域像素内容两个未决项。

## 2026-11-08 Blob ModuleIO 的 ROI/Mask 资源契约

原厂 `IMVSBlobFindModu.xml` 进一步确认，Blob 模块的输入面包含 `InputImage`、矩形 `InputROI`、扇环 `InputROIAnnulus` 和 `InputMask`；输出面包含多 ROI、扇环 ROI、`BlindPolygon` 和 `OutputMask`。这说明 `ProcessAlg` 的区域对象不只是一个简单的矩形参数。资源只确认组合、图形类型和绑定字段，未确认 `GenerateMaskImage` 的栅格化及像素规则，因此当前仍不能用 XML 直接推导 VM 的二值结果。证据见 `analysis/full-audit/batch1-20261107/C03/original-blob-moduleio-roi-mask-resource-contract.json`。

## 2026-11-08 原厂 Blob 模块 vtable 与 GenerateMaskImage 入口

从原厂 `IMVSBlobFindModu.dll` 的 `.rdata` vtable（RVA `0x302C0`）读取到包装层虚表入口，并与已保存的导出/反编译结果交叉核对：`Process` 位于 vtable `+0x38`，`SetParam` 位于 `+0x18`；`ProcessAlg` 在非直接区域分支调用当前对象 vtable `+0x48`，对应 DLL 内部 RVA `0x9C80`。该入口是 `GenerateMaskImage` 的候选实现入口，调用参数包含模块输入、ROI/Mask 集合和附加状态。入口的完整函数体尚未恢复，因此生成图像布局和像素规则仍保持未决。

虚表原始字节、文件偏移和交叉核对记录在 `analysis/full-audit/batch1-20261107/C03/original-blob-vftable-region-dispatch.json`。这把下一步从“猜测区域处理”收窄到一个明确的原生入口 `IMVSBlobFindModu.dll+0x9C80`。

## 2026-11-08 逆向效率改造

把原先临时执行的 PE 段定位、vtable 读取和 RVA 换算固化为 `tests/NativeEvidence/C03/Extract-BlobVtableDispatch.ps1`。后续更换原厂 DLL 或 vtable 地址时可以批量重跑并自动产出同一格式证据，避免重复手工偏移计算。当前 C03 的深挖顺序固定为：先批量抽取导出/vtable/调用点，再只对影响 `ProcessAlg` 前 ROI/Mask 数据流的入口做函数体恢复，最后才做一次 VM/SDK 运行差分。
