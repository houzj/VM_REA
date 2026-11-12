# C03 Blob 同输入差分交付

当前各项交付条件、已完成证据和未通过的原厂门槛见 [C02 / C03 退出审计](C02-C03-delivery-audit-20261111.md)。

计划基线 `f5051d10 / 61338643ee9d`。已完成原件身份核对、静态调用边补证和本机直接 SDK 两次复跑。原厂宿主侧仍待恢复历史方案与运行环境。

## 原件和输入

本机安装根为 `C:/Program Files/VisionMaster4.4.0`。

| 文件 | 本机 SHA-256 | 核验 |
|---|---|---|
| `Applications/VisionMaster.exe` | `CF5730DF126C9C244E13E454CB664909F7E6A2BDF78B5314D03F6DC64E76D291` | 版本 4.4.2.1567986，历史宿主记录未提供 EXE 哈希 |
| `Applications/Module(sp)/x64/Location/IMVSBlobFindModu/IMVSBlobFindModu.dll` | `4DE695D8C3051045C9010739686F94C32D66A36EE486207FE749FB183FFF4C9A` | 与归档 Blob 模块字节一致 |
| `MVDAlgorithmSDK/Runtime/x64/MVDBlobFindCpp.dll` | `D819529A0A29A935C994CBF8136A8841834B90437A2D842DD22D407BF8743932` | 与归档 Blob SDK 包装 DLL 字节一致，完整依赖链身份待核 |
| `Applications/PublicFile/x64/iMVS-6000PlatformSDK.dll` | `F5129789D7CF29DCB5F1B772D58159478D1110F1380905BCD27C38ED5F072A4E` | 历史过程探针与重复性记录使用 `B62D7E15588CCDA6671228AEBC11FEC3E27ACA878F0E03067E44B17D5A54A9A1`。匹配的归档件只作静态复验 |
| `D:/MyWork/测试图片/标定/1.jpg` | `3A77112B982DF494A74CD756CD412D045210BADE88604BE52AB875C972CA303E` | 与直接 SDK 历史输入一致 |

历史宿主使用 `test1108.sol`、流程1、图像源1→Blob分析1。当前工作区和样图目录未找到该方案。按历史 SDK 记录，图像为 1280×960、格式值 1，参数为 ThresholdType=2、LowThreshold=100、HightThreshold=150、Polarity=0、connectivity=8、MinArea=10、MaxArea=9999999、FindNum=100。归档默认 ROIAreaType=0 仅能说明模式字段，活动 ROI/Mask 集合及两侧 SDK Run 前的图像字节待核。

### 手验前身份检查

在工作区根目录运行，`-InstallRoot` 可指定另一台机器的安装根，`-Solution` 可指定待手验的方案文件。

```powershell
pwsh -File tests/NativeEvidence/C03/Get-HostIdentityPreflight.ps1 -Image 'D:/MyWork/测试图片/标定/1.jpg'
```

脚本记录方案、样图、十个宿主文件和七个直接 SDK 文件的路径、长度和 SHA-256，PE 文件另记版本。宿主参照来自归档逆向原件，直接 SDK 参照来自已复跑矩阵。存在文件但缺历史参照的项标为 `reference-unavailable`。原厂启动、注册及实际加载 DLL 的核验留在运行取证步骤。

本机预检通过了样图和直接 SDK 身份。Blob 模块与归档一致，`iMVS-6000PlatformSDK.dll`、`PlatformManager.dll`、`ModuleFrame.dll` 和 `ModuleProxy.dll` 均有归档哈希差异，且历史方案仍缺失。用另一个 BMP、一个独立方案及不存在的安装根复核，报告分别识别样图不符、方案历史身份未证和安装文件缺失。当前汇总 SHA-256 为 `53BB86119C7C58295F1A2B072092A288AB94EAF16FF865A7E1DE01A0AC41B128`。[预检脚本](../../tests/NativeEvidence/C03/Get-HostIdentityPreflight.ps1) · [本机原件清单](../../analysis/full-audit/batch1-20261107/C03/host-identity-a28da3dbf1394c1d92d8145253651002/summary.json)

## 同输入差分和调用边

| 观察点 | 原厂宿主历史观察 | 直接 SDK 历史观察 |
|---|---|---|
| 界面或 SDK 汇总 | 45 / 971486 | 47 / 973202，归档重复运行一致 |
| 首个已记录面积 | 716928 | 718420 |
| 结果向量与图像 | 归档可查 5 行界面值，完整向量、BinaryImage、BlobImage 待取得 | 47 行及两个图像缓冲哈希已归档 |

上表是历史可见输出差异。首个运行时分歧点仍待捕获。下节复跑只覆盖直接 SDK 一侧。

### 本机直接 SDK 复跑

用现有 `tests/NativeEvidence/C03/OriginalSdkCppProbe.cpp` 编译 x64 控制台探针，链接随包 Blob、GlobalInit、Image 和 Shape 库，将本机算法 SDK 的 `Runtime/x64` 放在探针搜索路径首位。使用经 SHA-256 验证的同一张 `1.jpg`，为窄字符接口临时提供 ASCII 相对路径。按上表参数运行，无显式 ROI/Mask 调用，开启 BinaryImage 和 BlobImage 输出。当前探针源码哈希与历史记录不同，两个版本分别登记。

两次运行各启动一个独立探针进程。进程退出码、全局初始化、工具创建和图像创建均为 0。两次都得到 `47 / 973202`，每次输出的 47 行各 10 个数值与历史直接 SDK 记录逐项一致，BinaryImage 和 BlobImage 的 SHA-256 也一致。向量比较覆盖日志输出精度，图像比较覆盖完整缓冲字节。MSVC 编译出现随包头文件字符编码警告，构建成功。测试未启动 VisionMaster 窗口，未查询历史宿主进程的注册表或图像。

复跑入口为 x64 探针加两个参数 `probe.exe <ASCII样图路径> <新输出目录>`，每次用单独进程和输出目录。汇总包含输入、源码、可执行文件、SDK 搜索路径上的依赖哈希、退出码与日志哈希。汇总 SHA-256 为 `5DB1568E30FA75EEBC5E9052555F6508F5C6CA13591147B385B6259DF6D8C659`。[两次运行的证据汇总](../../analysis/full-audit/batch1-20261107/C03/sdk-direct-local-20261110/summary.json)

- `CIMVSBlobFindModu::ProcessAlg`（VA `0x18001FEC0`）用输入 `+0x10` 设置 SDK 图像并重置区域状态。模式 1 直接传 `+0x118`。其他模式下，首集合恰一项且第二集合为空则直传单范围。否则走模块 vtable `+0x48`。生成失败提前返回，成功后设置生成区域。追加启用掩膜节点还要求第二集合为空、输入 `+0x110` 非零。
- 本机同 SHA PE 的 vtable `+0x48` 确认指向 RVA `0x9C80`。在该入口，VA `0x180009CC1–0x180009CD9` 检查 `this+0x20`，为空时把该槽地址传入 IAT `0x18002D4D0`。IAT 描述符从 `0x18002D4C8` 起，第二个 x64 槽是 `MVDPreproMaskCpp.dll!CreatePreproMaskToolInstance`。因此可确认助手是按槽状态延迟创建。汇编只证明此分支存在，活动宿主是否经过该入口及生成区域内容待核。
- 所查调用链在 SDK vtable `+0x78` 运行，随后由 `+0x30` 取结果。空结果抛出 `0xe0000315`。`ProcessOutput` 再次取结果并投影数量和总面积，图像输出各有独立标志。

### GenerateMaskImage 整体调用链复核

对同 SHA 模块的 `.pdata` 范围 `[0x9C80, 0xA49F)` 反汇编。使用本机随包 `PreproMaskCpp.h`、`MVDImageCpp.h` 编译只含接口调用的 x64 汇编探针，未链接或执行探针。将编译所得的虚表偏移与原模块调用点逐一对照。

| 数据流 | 原模块指令与随包 ABI |
|---|---|
| 五参输入 | `ProcessAlg` 在 `0x180020007` 设置 R8 为输入 `+0x20`，在 `0x180020043–0x180020052` 写第五参数低字节为 0，并以 RCX=模块、RDX=输入、R9=输入 `+0x38` 调用虚表。入口保存四个寄存器，`0x180009D3F` 读取的正是该第五参数字节（入口栈 `+0x28`）。 |
| 选择重建分支 | 第五参数为 0 时跳至 `0x18000A2FF`，绕过 `0x180009D4D` 开始的已有区域及输入宽高比较。因此一旦本处 `ProcessAlg` 走入掩膜生成路径，已有区域比较不会决定它是否重建。这个结论仅限已核实的调用点。 |
| 构建助手输入 | `0x18000A305` 的 `+0x50` 对应 `ClearRegions`。两个循环按各自的 begin/end 计算 8 字节条目数，`0x18000A331`、`0x18000A360` 的 `+0x28` 对应 `AddRegion`，先添加输入 `+0x20` 的集合并传 `true`，再添加输入 `+0x38` 的集合并传 `false`。随后 `0x18000A36C` 以输入 `+0x10` 的图像调用 `SetInputImage`（`+0x10`）。 |
| 运行与回写 | `0x18000A37F` 调助手 `Run`（`+0x20`），`0x18000A388` 调 `GetOutputImage`（`+0x18`），在 `0x18000A38B` 写入模块输入记录 `+0x118`。再次取输出图像后，`0x18000A3A8` 的图像接口 `+0xB8` 与 `IMvdImage::GetData` 对应，键为 `SharedMemName`（字符串 RVA `0x2EC98`），返回值写 `+0x120`。成功路径置 `+0x110=1`。 |

这条路径的活动性仍取决于 `ProcessAlg` 的模式与集合基数。取证时先记录两集合的条目数和顺序。若进入生成分支，在助手 Run 前后比较输入图像、区域与输出图像的缓冲哈希，再验证 `+0x118` 如何交给 Blob SDK。随包接口和调用方指令可确定该模块的编排，SDK 内部栅格化、缓冲内容、共享内存指针所有权和原厂宿主实际输入仍须独立验证。

证据 SHA-256：函数体汇编 `9F16CD3E8932203D6D140B9C64ED56F9E31857D70F7C684451D0C45612CD6491`，调用点汇编 `C6E47FE2247AAE87AB25EBEFA806152882C9DE3D8AC2717E19C6A519DA61B74B`，接口探针汇编 `E1395101067F6656C068955E8CB728692FA4CF32EDB3C2701A11A1CDF03B97F8`。随包头文件分别为 `913EDCA2A6E8FEB2F81D8C8026F5D2439FF42CF1B6E32110DC5D8D062BD47F23` 和 `5709D69647C9EE4A4BEC096D9FF260CD629B8A43676C80F4BD876C7B4E4EFEBF`。[完整函数体](../../analysis/full-audit/batch1-20261107/C03/generate-mask-body-20261110.asm.txt) · [调用点](../../analysis/full-audit/batch1-20261107/C03/generate-mask-caller-20261110.asm.txt) · [ABI 探针源码](../../analysis/full-audit/batch1-20261107/C03/generate-mask-sdk-abi.cpp) · [ABI 探针汇编](../../analysis/full-audit/batch1-20261107/C03/generate-mask-sdk-abi.asm)

### 全屏选择到原生代理的写入链

原厂 Blob 算法页的 `ROISelecter Name="RoiType"` 为 `RW`、`SelectType=Mul`，并启用全屏。托管 `ROISelecter.SelectFullScreen`（`ROISelecter.cs:2105`）清除当前 ROI、停止绘制，再调用 `SetValue`。列表为空时，`NormalizeReversParse`（`2649–2662`）对该选择器生成 `01 00`。独立的外部屏蔽区选择器使用 `Avoid`，空列表生成 `00 00`。历史 SDK 日志观察到两个键的读取长度均为 2，字节值来自源码分支，日志只记录长度。

| 调用边 | 已核实行为 |
|---|---|
| `ROISelecter.SetValue → SetParamUnit → InnerSetValue` | 写入权限通过且序列化结果与 `PreValue` 不同时更新缓存。若需要图像尺寸，先用字符串键 `DrawImageSize` 写入 `width,height`，再通过 `IUserBytesData[ComName]` 提交二进制。随后调用 `SetRoiSeq` 和 `PreExecuteRun`。`SetBinaryData` 捕获异常并标记失败，其返回值未用于阻止这两个后续调用。 |
| `ModuleParam.IUserBytesData → ModuleManager.SetBinaryData → IMVS_SetBinaryData` | 普通键按模块 ID、键名、字节数组和超时值传给 SDK。托管包装复制原始字节，将 UTF-8 键写入固定缓冲，按原长度提交数据指针。这里可确认全屏的两字节载荷通过参数写入接口下传，载荷本身不含图像宽高或矩形坐标。 |
| `PlatformManager!PF_M_SetBinaryData → SPSetBinaryData → VM_PROXY_RecvServerCommand` | 归档版的 `0x0BD2` 通道在 VA `0x1805F65B9` 使用缓存代理入口。加载侧在 `0x1805D37DE` 将 `+0x7D510` 槽地址传给 `0x18063BC40`。该函数构造宽字符串路径，追加 UTF-16 `.dll`，于 `0x18063BD7D` 通过 IAT `0x180760148` 调用 `KERNEL32!LoadLibraryW`，随后在 `0x18063BD83` 写回返回句柄。具体配置路径值待核。 |
| 归档 `ModuleProxy.dll!VM_PROXY_RecvServerCommand → 0x180049CB0 → 0x18004A610` | 导出表给出非转发入口 RVA `0x20BD0`。入口保留原第 1、3、4、5 参数及第 2 参数低 32 位，重排后在 `0x180020C40` 调用下一层。下一层检查命令指针及长度非零，在 `0x180049D3E` 调用 `0x18004A610`。此处尚未确认 Blob 的具体参数消费者，也未记录历史宿主实际加载的代理路径。 |

托管来源是归档反编译的 `Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/ROISelecter.cs:1315`、`:1353`、`:2314`、`VM.Core/VM.Core/ModuleParam.cs:55` 和 `VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/ModuleManager.cs:1564`。本机 Blob 算法页 XML SHA-256 为 `34A0CC5423A3D66B92DC272E0F50B013310E5180E801C648005383C3ADDDFDEC`。归档 `PlatformManager.dll` 和 `ModuleProxy.dll` SHA-256 分别为 `06D86F51BEBFEBA357D1B79297E20A3B0A70580A40AF7B77C8ABDD6C38CA0624` 和 `76F09A40184B2B5F99B7173EDDE23A716D339E3C64955696D6927495EFDF68A9`。复核时用 `dumpbin /disasm` 查看表中调用点，用 `dumpbin /exports` 核对导出，按 PE 导入描述符和 thunk 顺序核对 IAT。此段为跨归档原件的静态取证。[历史 SDK 长度日志](../../analysis/full-audit/batch1-20261107/C03/runtime-sdk-log-roi-binary-length-20261109.json) · [平台命令入口](B05-binary-binding-batch9.md) · [代理加载指令](../../analysis/full-audit/b08-native-20261030/evidence11/0x18063bc40-18063bc40.asm.txt) · [代理入口指令](../../analysis/full-audit/b08-native-20261030/evidence13/VM_PROXY_RecvServerCommand-180020bd0.asm.txt)

### 原生全屏矩形从参数到 SDK 的路径

在与本机及历史归档同 SHA-256 的 Blob 模块内，直接从导出和调用指令恢复了普通图像模式下的全屏路径。下表地址为 RVA，映像基址为 `0x180000000`。

| 阶段 | 原模块行为 |
|---|---|
| 写入并解码 | `CIMVSBlobFindModu::SetParam`（`0x22500`）的回退调用在 `0x23A66` 进入基类 `SetParam`（`0x155E0`）。匹配 `RoiType\0` 后将字节、长度和 `this+0x38` 传给 `AnalyzeMultipleROIMsg`（`0x77A0`）。输入 `01 00` 通过两次长度检查。首字节经索引表 `0x82A8` 得到 0，跳转表 `0x827C` 的第 0 项指向 `0x78C9`。此分支追加一个 40 字节矩形记录，前五个 float 为 `0.5, 0.5, 1, 1, 0`。成功尾部 `0x80D3–0x8146` 将条目写到原始 ROI 状态并写入类型 1。 |
| 缩放到图像坐标 | 基类 `Process`（`0x10840`）在 `0x108B3` 通过 vtable `+0x40` 进入 `0xE810`。读取图像后，`0xE9CC–0xE9FD` 默认取当前帧宽高。特定状态为 1 且两项缓存尺寸均非零时，取 `this+0x11A4/+0x11A8`。这两项缓存可由 `DrawImageSize` 写入分支设置。类型 1 和非空矩形集合通过 `0xEC2F–0xEC4E` 后，`0xECC0–0xED26` 逐条将 X 和宽乘图像宽，Y 和高乘图像高，原样复制角度，并把 20 字节结果写入临时输入记录 `+0x98` 的集合。 |
| 建立原生 ROI | `Process` 在 `0x1090B` 调 `0x10A70`。模式 0 的矩形路径从 `0x10CF7` 清空工作 ROI，按 20 字节条目遍历，必要时在 `0x10D9F` 通过 `MVDShapeCpp!CreateRectangleInstance` 创建形状。接着在 `0x10E37–0x10E93` 用五个矩形 setter 写入中心、尺寸和角度，将指针追加到最终输入的 `+0x20` 集合。独立的外部 ROI 解析入口 `0x83B0` 对两字节输入在 `0x8428` 跳过多边形体读取，并写零条目数。没有其他屏蔽区来源时，最终输入的 `+0x38` 无外部范围。 |
| 交给 Blob SDK | 基类在 `0x10921` 先调用 `0x13CC0` 处理输入，再将同一输入交给 vtable `+0x38` 对应的 `CIMVSBlobFindModu::Process`（`0x1FA80`）。后者在 `0x1FBD6` 将该输入原样传给 `ProcessAlg`。当最终集合为一条主范围和零条外部范围时，`0x20038` 调用 SDK 单范围 setter。 |

因此，模式 0、无其他屏蔽区、有效缩放尺寸为 1280×960 时，全屏参数会生成中心 `(640,480)`、宽高 `(1280,960)`、角度 0 的显式矩形。这与直接 SDK 复跑中得到 **45 / 971486** 的设置相符。静态链已确定差异最早可出现于 SDK Run 前的区域设置。历史宿主的实际模式、缩放尺寸、区域集合和输入缓冲仍缺逐帧观测，尤其需记录辅助输入和 `0x13CC0` 前后的状态。也仍缺原生代理实际投递到该模块的命令记录。

复核原件为 `analysis/full-audit/b08-blob-low-20261030/IMVSBlobFindModu.dll`，SHA-256 为 `4DE695D8C3051045C9010739686F94C32D66A36EE486207FE749FB183FFF4C9A`。使用 `dumpbin /exports`、`/imports`、`/pdata` 和 `/disasm`。按 PE 节表将跳转表和 vtable RVA 换算到文件偏移，vtable 导出 RVA `0x302C0` 的 `+0x38/+0x40` 分别是 `0x1FA80/0xE810`。汇编摘录 SHA-256 为 `8BD0035CBB667BF6E96D00E4ABF32CEB6B85276CB3F49132C1D47A07A2C568ED`。[调用链汇编](../../analysis/full-audit/batch1-20261107/C03/fullscreen-native-chain-20261110.asm.txt)

### 宿主输入缓冲到 Blob SDK

同 SHA 的 Blob 模块从 ModuleFrame 读取已准备好的像素缓冲。以下地址均为 RVA，图像接口偏移用随包头文件编译 x64 ABI 探针复核。

| 观察点 | 已核实行为 |
|---|---|
| ModuleFrame 取图 | `0xE989 → 0x1B8C0` 使用键 `InImage`，在 `0x1B71B` 调 `ModuleFrame!VM_M_GetImage`（IAT `0x2D580`，索引 0）。该调用返回数据描述。接着经 `VM_M_GetInt` 读取 `InImageWidth`、`InImageHeight`、`InImagePixelFormat`。格式值 `0x01080001` 映射至 SDK 格式 1，行距取宽度并核对 `行距 × 高度` 与输入字节数。取图失败在 `0xE992` 返回 `0xE0000510`，非正宽高另有前置拒绝。 |
| 封装现有内存 | `0x10A70` 从 `this+0x11D8` 取得图像对象，空槽时用 `MVDImageCpp!CreateImageInstance` 建立。它把每个通道的指针、行距及 `行距 × 高度` 写入 `MVD_IMAGE_DATA_INFO`。在 `0x10CB7` 调 vtable `+0x40` 对应的 `IMvdImage::InitImage(width,height,format,data)`，随包接口注明该重载直接引用数据缓冲。随后 `0x10CE4` 将图像对象写入最终输入 `+0x10`。 |
| 进入 SDK | 派生 `Process` 在 `0x1FB7D` 调图像 vtable `+0x20` 读取像素格式。值为 1 才进入 `ProcessAlg`，其他值在 `0x1FE90` 返回 `0xE0000503`。算法在 `0x1FFC3` 将同一输入记录 `+0x10` 的图像对象交给 Blob SDK 的 `SetInputImage`。 |

直接 SDK 探针使用的是另一个重载 `InitImage(file, MVD_PIXEL_MONO_08)`，在探针进程中直接解码 JPG。宿主 Blob 入口接收上游已解码的数据，两边文件 SHA 相同只能固定文件身份。应先比较 `VM_M_GetImage` 返回的数据与 Blob `SetInputImage` 接收的通道，再与直接 SDK 的行距和缓冲 SHA-256 对齐。若像素已在前一观察点分歧，应向上游图像源追查。若两个像素观察点都一致，再比较区域 setter、几何和最终 Run 结果。

原件与上节相同。用 `dumpbin /disasm:bytes` 与两个 DLL 的导入表复核调用点。指令摘录 SHA-256 为 `183697CA65E1367490CBC5696790CA42A80F16A1ED32E0591DAE0ADF87AFEBDC`，ABI 汇编 SHA-256 为 `928C1AAE37DA99C49802E1FCDCA6235094B297777A77DA361D349938444E102D`。ABI 探针仅编译，未链接或执行。[调用与 IAT 摘录](../../analysis/full-audit/batch1-20261107/C03/host-input-image-chain-20261110.asm.txt) · [ABI 探针源码](../../analysis/full-audit/batch1-20261107/C03/host-input-image-abi.cpp) · [ABI 汇编](../../analysis/full-audit/batch1-20261107/C03/host-input-image-abi.asm)

### 进入算法前的检测几何与可选掩膜输出

同一 Blob 原件的 `0x13CC0` 有两个需要独立取证的阶段。地址均为 RVA。

| 阶段 | 已核实行为 |
|---|---|
| 发布检测几何 | `this+0x28=0` 时，按输入 `+0x20/+0x28` 遍历主区域。矩形分支在 `0x142BF–0x14408` 读取形状的五个值，写入输入 `+0xA4–0xB4`，经 `0x1DCB0` 写 `DetectCenterX/Y`、`DetectWidth/Height`、`DetectAngle`。IAT `0x2D5A0` 是 `ModuleFrame!VM_M_SetFloat`，包装将栈上的索引送入 R8D，将几何浮点值送入 XMM3。该段的可见写入是检测字段和 ModuleFrame 输出。 |
| 可选发布掩膜 | 基类 `SetParam` 的 `OutputMaskImage` 分支在 `0x15BAC` 写 `this+0x2C`。精确匹配 `False\0` 时写 0，`0x14A45` 随即跳过掩膜输出。开关开启且输入 `+0x110=0` 时，`0x14AD6` 调 vtable `+0x48`，第五参数低字节为 **1**，允许进入助手的已有区域比较路径。返回后 `0x14ADB` 清 `+0x110`。开关开启且标志已有值时，直接取 `+0x118/+0x120` 发布。 |
| 输出和错误流 | `0x1DC60 → 0x1D680` 使用 `OutMask`、`OutMaskWidth`、`OutMaskHeight`、`OutMaskPixelFormat`，经 `VM_M_SetImageEx` 和 `VM_M_SetInt` 发布。基类调用者在 `0x10926–0x10975` 检查并按日志级别记录 `0x13CC0` 的非零返回，随后仍到达 `0x1098C` 调用派生 `Process`，由其 EAX 覆盖返回码。 |

归档中模块 16 的默认参数导出记录 `OutputMaskImage=False`。该值写入实例时，进入派生算法前会跳过这条可选掩膜输出路径。派生 `ProcessAlg` 的区域选择仍独立执行，其中掩膜生成调用传第五参数 **0**。以后采集 `GenerateMaskImage` 时要按调用者与第五参数区分这两条路径，并分别记录输入 `+0x110/+0x118/+0x120`。这份默认参数导出是独立样本，`test1108.sol` 运行时的模块字段和像素仍待观测。

复验用上述同 SHA 原件运行 `dumpbin /disasm:bytes` 和 `dumpbin /imports:ModuleFrame.dll`，按八字节 IAT 槽核对 `0x2D570/0x2D5A0/0x2D5A8`，按 PE 节表读取 `OutputMaskImage` RVA `0x2F3A8` 和 `False` RVA `0x2DFD8`。指令摘录 SHA-256 为 `587C408A581AF44B7D2C6C7543B148025D63A448F09FFA7567220C7FE3D4E1BD`。[调用与 IAT 摘录](../../analysis/full-audit/batch1-20261107/C03/host-pre-run-mask-20261110.asm.txt) · [历史参数导出](../../analysis/full-audit/batch1-20261107/C03/runtime-roi-default-state.json)

## 放行条件

### 区域路径与角度字段的受控复跑

同一张图、同一组 Blob 参数，五种区域设置各用两个新进程复跑。默认模式之外，`full-roi` 调用 `SetROI` 传入中心 (640, 480)、宽 1280、高 960 的矩形。`generated-full-roi` 先用相同几何运行 `PreproMask`，再将输出交给 `SetRegionImage`，没有额外设置 SDK 区域助手。探针按该模式保存掩膜图像，仍只测试直接 SDK。

| 区域设置 | 两次结果 | 观察 |
|---|---|---|
| 默认 | 47 / 973202 | 基准 |
| 直传矩形 ROI | **45 / 971486** | 数量和总面积与历史宿主一致 |
| 生成全图掩膜 | 47 / 973202 | 掩膜 1,228,800 字节均为 255。向量、BinaryImage 和 BlobImage 与默认模式逐项一致 |
| 全部在图外的屏蔽多边形 | 两次退出 2 | 在 Blob Run 抛异常，无结果。旧有其他探针的结果另存 |
| 图内屏蔽多边形 | 47 / 960402 | 两次向量和图像哈希一致 |

直传 ROI 的前五行与历史界面在面积、质心 X/Y、周长、长/短轴、圆形度、矩形度均逐项一致。原 `ProcessOutput`（VA `0x180020310`）用外接框对象的槽 `+0xD0` 写 `Angle`，用 Blob 对象的 `+0x40` 写 `BlobAngle`。额外两次直传测试同时读取 `GetBoxInfo()->GetAngle()` 和 `GetAngle()`，两次 45 对角度一致。历史界面前五个角度均与外接框角度一致，分别为 0、0、12.33909、10.30485、-17.10273。

这证明直传全图矩形足以复现已保存的宿主汇总和前五行所比较的字段。`ProcessAlg` 的单范围分支与该解释一致，实际宿主选中此分支、完整宿主向量以及原始图像仍待运行取证。

复跑使用 `probe.exe <ASCII样图路径> <新输出目录> <空参数文件> <模式>`。编译链接需额外加入 `MVDPreproMaskCpp.lib`。矩阵汇总 SHA-256 为 `fc307bb6d6337db381be8f20a518181e9af011a76e1a4280dcda904adc2849ef`，追加的角度汇总为 `6dbe012e2bd7818b30a965768c57754c23b0b33760e8bbefb26eac24be3692ab`。两份汇总分别记录对应探针的源码和产物身份。[区域矩阵](../../analysis/full-audit/batch1-20261107/C03/sdk-region-matrix-20261110-final/summary.json) · [角度投影](../../analysis/full-audit/batch1-20261107/C03/sdk-region-matrix-20261110-final/angle-projection.json) · [原输出投影](../../analysis/full-audit/b08-geometry-output-20261031/0x180020310-180020310.c)

### 一键复跑 SDK 差分

在 x64 Visual Studio Developer PowerShell 中，从工作区运行：

```powershell
pwsh -File tests/NativeEvidence/C03/Run-SdkRegionMatrix.ps1 -Image 'D:/MyWork/测试图片/标定/1.jpg'
```

可用 `-SdkRoot` 指定 SDK 安装目录，用 `-OutputRoot` 指定证据父目录。脚本先核验 JPG 与归档的七个 SDK 运行文件哈希，再编译 x64 探针。五种区域设置、两种内存输入以及默认区域和显式矩形下额外的图像开关组合各运行两次，共 26 次。每次校验退出码、输入方式、输入像素、Blob 汇总、完整打印向量和已开启的图像缓冲 SHA-256。显式矩形另校验全部外接框与 Blob 角度，图外屏蔽分支校验异常发生在 `Run`。结果以归档直接 SDK 数据为参考，`summary.json` 记录每次运行及差异。

探针在 `Run` 返回或捕获 `Run` 异常时、释放实例前，用 `CreateToolhelp32Snapshot` 保存本进程当时装载的模块路径。脚本核验七个基线 DLL 实际来自所选 SDK 目录，并登记该目录所装载的 DLL 哈希。SDK 目录同名依赖被其他路径替换时判为差异，Windows 系统路径下的 Microsoft VC 运行库另列在原始模块清单。隔离复验将同字节的 `swscale-6.dll` 放到探针目录，十次都识别出实际装载路径偏离。快照覆盖指定时点，未记录此前已卸载的模块。

先前 10 次文件输入均匹配，实际装载的 21 个 SDK 运行目录 DLL 的路径和哈希一致。探针保存 `SetInputImage` 前解码的单通道缓冲，均为 1,228,800 字节，行距 1280，SHA-256 为 `0DB38D6CEF0FDDC6CEBECD13BA49A3AFBBA76E0C58DA4B63322BA73ED5FDED47`。历史宿主像素缓冲未归档，后续可用该值核对相同观察点。先前汇总 SHA-256 为 `1C5D71D01519D7EBD0AC45FF5108E8D12A5E9933AF6768957CA17800B56403D8`。[运行脚本](../../tests/NativeEvidence/C03/Run-SdkRegionMatrix.ps1) · [归档预期](../../tests/NativeEvidence/C03/sdk-region-baseline.json) · [文件输入汇总](../../analysis/full-audit/batch1-20261107/C03/sdk-region-matrix-64966191f6d447cab1bdb203f2647197/summary.json)

追加的内存对照从对应文件探针保存的 `input.bin` 启动独立进程，固定 Mono8、1280×960、行距 1280，用与宿主 Blob 相同签名的 `InitImage(width,height,format,data)` 构造图像对象。脚本将每次文件解码及内存封装后的输入像素都与上面的固定哈希核对。扩充后的 **14 次运行均匹配**。四组文件与内存配对的输入、完整打印向量、BinaryImage、BlobImage 以及 21 个实际装载 SDK DLL 的路径和哈希均一致。默认区域仍为 **47 / 973202**，显式矩形仍为 **45 / 971486**。原始输入少一个或多一个字节的两组负例都在图像初始化阶段退出，未进入 Blob Run。

这组对照固定了相同像素下初始化重载的影响，宿主上游解码、活动 ROI 和实际送入 SDK 的像素仍待观察。扩充汇总 SHA-256 为 `9836211FC939239E128AABC72C7B264DA4CB1B9C3FF5A607CA81BF51510970FF`。[14 次对照汇总](../../analysis/full-audit/batch1-20261107/C03/sdk-region-matrix-9678126f6ad940eca0c549260a93d400/summary.json) · [原始输入负例](../../analysis/full-audit/batch1-20261107/C03/sdk-region-matrix-9678126f6ad940eca0c549260a93d400/raw-input-negative.json)

### 显式全图矩形的像素与连通域差分

对上述矩阵中两对独立的默认区域与显式矩形运行，按字节核对输入相同，再比较两个 SDK 输出。下表的边缘距离从最外一行或一列起算为 0。两对结果一致。

| SDK 输出 | 从前景变为背景 | 位置 | 新增前景 |
|---|---:|---|---:|
| `BinaryImage` | 1722 | 全部位于距离 0 | 0 |
| `BlobImage` | 1716 | 距离 0 至 3 依次为 1702、9、3、2 | 0 |

对 `BlobImage` 按参数中的 8 连通做标记。默认与矩形两侧的完整面积序列分别与 SDK 的 47 项和 45 项打印向量一致。按两侧的重叠像素关联连通域，有 42 个完全保留。其余五个如下，坐标为默认图中该连通域按行扫描的首个前景像素。

| 首像素 (X, Y) | 默认面积 | 显式矩形面积 | 面积变化 |
|---|---:|---:|---:|
| (0, 0) | 718420 | 716928 | −1492 |
| (659, 743) | 252568 | 252375 | −193 |
| (14, 684) | 202 | 195 | −7 |
| (1002, 0) | 13 | 0 | −13 |
| (967, 0) | 11 | 0 | −11 |

最后两个区域在矩形版 `BinaryImage` 中分别剩 7 和 5 个前景像素，均小于本次 `MinArea=10`，在 `BlobImage` 中完全消失。五项变化合计 −1716。这一空间差分可用于比对后续采集的宿主缓冲。其证据范围为直接 SDK，对原厂宿主仍需核对实际区域和像素。

复跑用 `node tests/NativeEvidence/C03/Compare-SdkRegionOutputs.cjs <SDK矩阵输出目录>`。产物记录两轮输入、日志和两个输出缓冲的 SHA-256。差分汇总 SHA-256 为 `757D25CCA4C1159294AC29B359F64E95E1196EA3FF78CB1D9B24DCC8B4061933`。[比较脚本](../../tests/NativeEvidence/C03/Compare-SdkRegionOutputs.cjs) · [空间差分](../../analysis/full-audit/batch1-20261107/C03/sdk-region-matrix-9678126f6ad940eca0c549260a93d400/spatial-diff.json)

本轮再次对 `sdk-region-matrix-4dd306f7d33a447dba8e0793ad073807` 执行同一比较脚本，两次默认区域与全图矩形输入、输出缓冲和连通域面积均通过重复性断言。机器可读结果保存为 [sdk-buffer-comparison.json](../../analysis/full-audit/batch1-20261107/C03/sdk-region-matrix-4dd306f7d33a447dba8e0793ad073807/sdk-buffer-comparison.json)，SHA-256 为 `739A8CFAAC7A60EA1E565ED34AA16C88FDFB63FDAF188AD4DB2F17BEBC484707`。

### 图像开关与 SDK 结果的边界

原厂 `SetParam` 的键名保留原件拼写。`BolbImageEnable` 在 RVA `0x230A3/0x230AD` 写 `this+0x15EAD`，随后通过 `GetBasicParam`（工具槽 `+0x28`）调用基本参数槽 `+0x20` 的 `SetShowBlobImageStatus`。`BinaryImageEnable` 在 `0x23106/0x23110` 写 `this+0x15EAE`，调用基本参数槽 `+0x30` 的 `SetShowBinaryImageStatus`。两者在参数写入时就传入 SDK。

同一字节也控制 `ProcessOutput`。在 `0x2064B` 检查二值图开关，经结果槽 `+0x18` 取得 `GetBinaryImage`，在 `0x2069D` 调用图像发布函数。在 `0x2070A` 检查 Blob 图开关，经结果槽 `+0x20` 取得 `GetBlobImage`，在 `0x2075C` 发布。两段都在数量和总面积写入之后。若开启的图像未写出，应分别检查开关、SDK getter 返回值和发布函数返回码。

直接 SDK 扩充矩阵 **26 次全部匹配**。同一输入在默认区域和显式矩形下分别验证四种图像开关组合，每种两次。关闭的 SDK getter 返回空指针，开启的图像哈希和双开基线一致。每种区域内的完整打印向量保持一致，默认区域仍为 **47 / 973202**，显式矩形仍为 **45 / 971486**。此结论覆盖当前固定样本。历史宿主应另外采集实际开关字节、结果 getter 和发布返回码。

原件与前述 Blob DLL 相同。用 `dumpbin /disasm` 核对参数、结果调用点，并从 PE 节读取键名 RVA `0x31018/0x31028`。随包 `BlobFindCpp.h` 的 SHA-256 为 `4305608BC0BF620C85BCAF5862A34D39F2F96218B8064B438ED6BD81F3C2C805`。ABI 探针只编译，确认了工具、基本参数和结果三张虚表的独立偏移。矩阵汇总 SHA-256 为 `6F4C2461EA4BF7A14277DC5615C22F513490F86A41C33414C615678DC9A99EE4`。[参数指令](../../analysis/full-audit/b08-blob-low-20261030/output/0x180022500-180022500.asm.txt) · [发布指令](../../analysis/full-audit/b08-geometry-output-20261031/0x180020310-180020310.asm.txt) · [ABI 汇编](../../analysis/full-audit/batch1-20261107/C03/output-flags-abi.asm) · [26 次矩阵](../../analysis/full-audit/batch1-20261107/C03/sdk-region-matrix-4dd306f7d33a447dba8e0793ad073807/summary.json)

### 手动宿主验证

历史 `test1108.sol` 缺失只限制历史方案复现。当前实现具备独立 JSON 方案的创建、保存、重开和执行能力，因此可以建立当前版本的新 C03 基线。原厂启动屏障、引导页、SDK 自动加载、`.sol` 容器和方案直启场景链仍未实现，不能把当前 JSON 入口写成原厂启动链。新基线不宣称复现历史方案，而是单独记录当前方案身份、运行环境和输入哈希。

当前基线建议使用同 SHA 的 `1.jpg` 建立文件图像源 → Blob 分析，设置前述八个参数。分别保存默认全图且无屏蔽区、显式全图矩形且无屏蔽区两种配置，各独立运行两次，记录方案 SHA-256、运行环境哈希、区域设置、汇总、完整结果向量和输出图像缓冲。两套基线分别归档后，即可继续当前版本的宿主与直接 SDK 差分。历史 `45 / 971486` 仍单独标记为不可复现的历史结果。

已只读检查工作区及 `analysis.zip` 的 ZIP 中央目录。归档共 283535 项，只有 8 个方案文件，无 `test1108` 名称或 `.prc` 项。三个 B08 `fixture.sol` 的 SHA-256 相同，均为 `B91A7736F467757E15A55AF7836E22364AE153478E17C69AAEB69E0C30674D0B`。历史记录显示，B08 自建 Blob 探针只部分加载方案且创建 Blob 失败，触发字符串探针在加载方案时失败。因此这三个副本没有建立 C03 的同输入基线。[B08 自建 Blob 记录](../../analysis/full-audit/b08-owned-runtime-20261030/b08-observations.json) · [B08 方案加载记录](../../analysis/full-audit/b08-trigger-runtime-20261030/trigger-observations.json)

运行时取证先在模块 `SetParam` 入口记录键、字节和长度，在 `0xE9FD` 记录实际缩放宽高，并在 `0x10921` 前后记录两个集合的基数和几何。随后确认 `ProcessAlg` 模式及实际 setter，比较 Run 前像素和区域缓冲，以及 GetResult 完整向量、两种角度和输出图像。记录目标进程注册状态及失败码。仅当定位到已证实的产品差异后，才决定归属。Blob 区域预处理和角度投影为工具专属线索，共享 ROI 载荷是否需要调整仍待证据。

[历史宿主记录](../../analysis/full-audit/batch1-20261107/C03/original-vm-interactive-evidence-20261109.json) · [直接 SDK 向量与缓冲哈希](../../analysis/full-audit/batch1-20261107/C03/native-sdk-run/original-sdk-direct-cpp-runtime-evidence.json) · [本轮 PE 虚表](../../analysis/full-audit/batch1-20261107/C03/local-preflight-20261110-vtable.json) · [本轮指令及导入表](../../analysis/full-audit/batch1-20261107/C03/local-preflight-20261110-asm.txt)
