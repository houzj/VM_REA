# 相机映射开放差异复核

复核日期：2026-10-19。范围仅限原厂索引行为与原厂桌面证据边界；没有改产品实现，没有启动设备测试，也没有重跑此前的测试组。机器证据汇总见 [camera-mapping-evidence.json](camera-mapping-evidence.json)，既有边界说明见 [原厂 parser 与索引证据](CAMERA-MAPPING-PARSER-INDEX.md)。

## Process 输入到结果字段的逐项调用链（2026-10-19）

在相机映射原厂模块 CLI 反编译中，`Process` RVA `0xb830` 按顺序执行 `GetProcessInput -> ProcessAlg -> ProcessOutput`，其中每步非零即返回；源码见 `analysis/camera-mapping/ghidra/module/18000b830.c`（SHA256 `405011B35674B5C3E28B254721BE1A36E99D7AFEF137FC67278EB7F051C6E40B`）。`GetProcessInput` RVA `0xa710` 按每组 `TargetPointX/Y` 和 `ObjectPointX/Y` 实际数组 count 检查两侧 X/Y 及 object/target count，再从数组 index 0 开始递增展平，来源 `18000a710.c`（SHA256 `92517CF0643E21FC9166E8552470F6C44637CA85D838EC18FE34BD84084AED8F`）。

`ProcessAlg` RVA `0xbd20` 清除原 SDK BasicParam，按展平顺序把每个 object/target 点对加入 CameraMap SDK；随后调用算法 `Run`，从 SDK GetResult 取矩阵送入 MatrixToParam，并执行 MatrixToParam `Process`。只有前述过程成功才置两个内部有效状态为 1，之后按启用状态写标定文件。来源 `18000bd20.c`（SHA256 `C38D617D9C7C2AB775D1B76C29091A6D7F0BDD807DD1E2DEA6568AB13F345FE9`）。`ProcessOutput` RVA `0xc450` 先写 CalibStatus；成功状态下从 CameraMap SDK Result 读 ScaleX/ScaleY，从 MatrixToParam Result 依次读取 TranslateX、TranslateY、Rotate、Scale、Skew、Aspect，再写入同名输出字段，最后提交 ModuRunTime。来源 `18000c450.c`（SHA256 `01338247C382DD899591B2E5CE4672D65C9B7AFFF7C7F3C313AD29C2F49A8EFE`）。因此比例来自相机映射 Result，而其余六个几何分解量来自另一个 MatrixToParam SDK Result；用同一个手写分解公式替代两个结果对象不能仅凭数学相近视作源码相同。

当前 `CameraMappingPlugin.Runtime` 的点组启用与展平、`CameraMappingSolver`、Result 字段投影按上述链实现；手写 MatrixToParam 标量投影仍是独立重建。受控模块 trace `analysis/camera-mapping/native/module-results.jsonl` 含完整透视示例及真实输出字段。`CameraMapping.PluginChecks` 的 `NativeModuleCheck` 将产品该次运行与原厂模块 `writes` 里的每个输出逐字段比较，容差 `<0.003`；同时分别检查二维点、八组绑定、文件和错误生命周期。2026-10-19 本次运行结果：110 项生产配置/Graph/模块检查通过；原厂 SDK 118 组求解差分为 118 matching、0 mismatching（固定矩阵/比例绝对容差 0.003）。这些证据没有显示可安全定位的生产差异，所以未改生产代码，也不声称 MatrixToParam 的分解算法逐指令相同。若要把该投影从独立重建升级为源码同源实现，需要继续反编译/对照 `MVDMatrixToParamCpp` 的 `Process` 与各 Result getter；现有模块输出 trace 可验证字段结果，但不能替代其内部运算源码。

## 原厂索引行为

### 源码确认

- 安装资源 `IMVSCameraMapModuAlgorithmTab.xml`（86621 bytes，SHA256 `9832e201bc27e31413bd54bd92d55464b35c0657f681f7ca0059a695e0a77dfb`）为相机映射 UI 的原厂配置资源。原厂反编译 `ButtonSelecter.IndexOpenSetter` 使用 `GetFrontFilterTreeOperation`，候选来自前驱输出过滤树；候选 UI 只接受 `int`。手工绑定解析则走 `SetFilterSourceOperation.GetBindIndex` 与 `VmModuleExtern.GetIoByPathFromAround`，其搜索范围和 UI 候选树不同，不能互相替代。
- `SetCombinationSourceOperation.Operate` 解析组合来源中的索引，写入 RelateIO 后写 RelateIndex；`VmModule.GetPubInfo` 将无索引编码成 `nIsIndexConstant=1, strIndexContent="All"`，常量 `0`/`1` 分别作为常量索引文本传递。原厂托管源码及具体方法路径见 `CAMERA-MAPPING-PARSER-INDEX.md`。
- 这些方法证明的是 UI 候选、托管绑定及交给原生订阅函数的结构体字段。它们不实现也不揭示 SDK 对动态数组取元素的下标解释。

### 已有运行证据

- `analysis/camera-mapping/parser/operation-factory.json`（SHA256 `A76F657EF2BE8F9CCB76717C1D6E5B5C914215BD7C67336A8340AA9B8612939F`）和 `parser/model/probe.json`（SHA256 `56A9452776EB24E75D7EDAC5DB8F7881415E2C3893776AD70B16CAB4F6F27E1D`）记录官方托管解析器加载资源、创建控件/Setter 及空、常量 0、常量 1 的订阅载荷。三种载荷均标记 `nativeSubscribeExecuted:false`，不能作为原生取值证明。
- `analysis/camera-mapping/parser/platform-entry.jsonl`（SHA256 `E7CEC02D83E7E3D199DEECE3DE3DBCA0689AC2C596E11DA40C725C613ED6AE31`）记录原厂平台入口创建句柄、进程成功，但相机映射模块 Init 返回 `0x80100015`，没有成功的模块订阅或 ModuleFrame 证据。这个失败码不说明索引格式无效或有效。

### 仍未知 / 未发现可修实现差异

平台订阅索引的基数、负数/越界结果、动态数组当前长度与失败后订阅状态仍未知；算法 SDK 点列表索引不能代替工作流订阅索引证据。原厂 SDK BasicParam Getter/Remove 的单版本运行行为后续由本报告的专门受控探针补充，但这不闭合平台订阅索引。现有插件将 indexed binding 保持 unsupported 与已有证据一致，本次未发现反证。

### SDK 接口与模块数组长度的补充复核

- 公开头文件 `D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Includes/Algorithms/CameraMapCpp.h`（SHA256 `0B59CDFC961722FFE4289BCF844FCB508E1CC0BA8CDBD04C9F19FBE09DE74422`）声明 `GetObjectPoint(int nIndex)`、`GetTargetPoint(int nIndex)` 和 `RemoveAlignPairAt(int nIndex)`，但参数注释只有“accord to index”，没有下标基数、有效范围、越界返回或异常规则。`AddAlignPairPoint` 文档标注最多32对、至少2对；它不能说明 Getter/Remove 的索引语义。
- 已有 SDK 受控探针 `tests/CameraMapping.NativeProbe/Probe.cpp`（SHA256 `2D70C2E37E68C743C616D9D2CBA163AABA2485477AA52E0046296BEA127424C3`）仅追加配对、设置参数、运行并读结果矩阵/比例；没有调用 Getter、Remove 或越界输入。SDK trace 因而不补足上述接口未知项。`MVDCameraMapCpp.dll` 中 RVA `180002620` 是 CameraMap 的矩阵导入适配函数，不是 MatrixToParam 算法本体；BasicParam 虚表实现体仍未在该输出中，不能从它推断越界行为。MatrixToParam DLL 的独立反编译边界见本报告末节。
- 可确认一层相邻但不同的行为：模块 `GetProcessInput` 反编译文件 `analysis/camera-mapping/ghidra/module/18000a710.c`（SHA256 `92517CF0643E21FC9166E8552470F6C44637CA85D838EC18FE34BD84084AED8F`）读取每组 `TargetPointX/Y` 和 `ObjectPointX/Y` 的数组元素数量，要求每侧 X/Y 数量相等、对象与目标数量相等；然后从内部数组位置 `0` 开始，按位置递增读取至该数组报告的 count，并按组顺序展平到点对缓存。这确认的是原厂模块从已收到的 ModuleFrame 数组构造算法点列表时的长度与读取顺序，不是工作流 `SubscribeResult` 对索引 0/1 的解释，也不是 SDK `GetObjectPoint(nIndex)` 的 ABI。
- 模块受控运行 `analysis/camera-mapping/native/module-results.jsonl`（SHA256 `A95B14B7AF0641BF7F6244FCBACA361667871F08FBDF95F96252090ABA43D45D`）包含两个组各两点的 `two-vector-groups-four-points` 记录：每个 X/Y GetFloat 报告 count=2，调用序列按组、按数组位置递增，成功输出。这再次确认已传入模块的数组枚举；因为输入已由 synthetic ModuleFrame 注入，不能证明上游订阅如何按索引取数。

本轮可将“模块算法读取动态点数组的长度/迭代次序”升级为源码确认；在 SDK 点 API 受控探针运行之前，剩余未知项包括订阅索引规则、SDK Getter/Remove 运行边界及原生订阅失败后的有效状态。以下小节更新了 SDK API 的单版本运行观察；其他平台订阅未知仍未闭合。没有发现足以改变插件实现的直接证据。

## 三段索引域与生产映射链复核

核对目标：选中来源的订阅索引、模块收到的坐标数组内位置、以及送入 CameraMap SDK 的点序号，是否能由现有证据证明数值相同。

### 源码可确认的链

1. 原厂算法资源 `IMVSCameraMapModuAlgorithmTab.xml` 将 `Data1` 的 `ObjectPoint`、`TargetPoint` 组合选择器分别绑定到 `ObjectPointInput1`、`TargetPointInput1`；对应模块定义 `IMVSCameraMapModu.xml` 将这两个 POINT 组合映射到 `ObjectPointX1/Y1`、`TargetPointX1/Y1` 两个 float 叶。`Data2`..`Data8` 同样使用组合/叶映射。文件哈希见上文与证据 JSON。
2. 托管 `SetCombinationSourceOperation.Operate` 对组合选择值中的 `[index]` 拆出索引文本，设置 `RelateIO` 与 `RelateIndex`。`VmModule.Subscribe` 对复合 IO 按匹配的子节点顺序递归关联；叶 IO 最终调用 `GetPubInfo` 并提交 `SubscribeResult`。`VmModule.GetPubInfo` 将索引模块 ID、索引字符串与常量标志填入发布信息；平台 SDK `ModuleManager.GetSubscribeResult` 将这些字段原样复制到 `IMVS_SUBSCRIBE_INFO.strIndex`、`nIndexModuleID`、`nIsIndexConstant`，再调用 `IMVS_SubscribeResultEx`。这些托管步骤没有把索引转成坐标数组内的位置，也没有在调用前重编号。
3. 算法模块 `GetProcessInput`（`18000a710.c`）从 `TargetPointX/Y%d` 与 `ObjectPointX/Y%d` 输入各自的 GetFloat count，检查每侧坐标长度及 Object/Target 长度相同，再对每组从局部数组位置 0 递增读取到 count-1。`ProcessAlg`（`18000bd20.c`）按生成后的 pair 缓冲顺序遍历，先清空 SDK BasicParam，然后逐项调用 `AddAlignPairPoint(object,target)`。因此源码确认了“模块输入数组位置 → 模块展平缓冲顺序 → SDK Add 调用顺序”的保序路径；它没有调用 `GetObjectPoint(nIndex)`，也没有将任何显式 index 参数传给 Add。

### 是否为同一索引域

**不能证明数值相同，应当视为边界不同的索引域，直到原生订阅运行证据建立映射。** `RelateIndex` 是订阅发布字段上的选择索引，由平台 native `IMVS_SubscribeResultEx` 解释；GetFloat 的 `p` 是已到达模块输入数组里的局部元素位置；SDK 点序是若干次无索引 `AddAlignPairPoint` 的调用顺序。后两者之间可确认按顺序一对一追加。公开头文件没有规定 SDK public `GetObjectPoint/RemoveAlignPairAt(int)` 的 nIndex 语义；本报告后续受控运行记录仅确认一个 SDK build 的 API 行为，仍不能证明 UI / 原生订阅索引与 SDK 点下标相同。

### 现有运行记录能证明什么

`analysis/camera-mapping/native/module-results.jsonl` 为 synthetic ModuleFrame 输入，不调用 platform Init / `IMVS_SubscribeResultEx`。其中每个 GetFloat 记录只有字段名、返回码和 count，并未记录 native 订阅解析出的 source array 原始值与映射来源。故它能复核模块收到给定数组后的 count/算法输出；模块源码能确认该数组之后的迭代/追加顺序；两者都不能证明 A（用户选择的 `RelateIndex`）如何被平台转换成 B（ModuleFrame 数组），也不能证明 C（SDK Getter 数字索引）等于追加序号。既有 platform trace 中模块 Init 在 `0x80100015` 终止，缺少可合法运行的原生订阅链样本。

结论：现有证据不支持写出“UI 的 [n] 最终等于第 n 个 SDK 标定点”之类转换规则。订阅索引基数/越界及 A→B 的动态数组投影仍未知；SDK Getter/Remove 的单版本运行行为已在下文另行记录，但没有实现修复依据。

## 原厂 CameraMap SDK 点 API 受控索引运行记录

为补足 public SDK 点列表方法，本轮另运行不依赖平台的原厂 SDK 探针。测试文件为 `tests/CameraMapping.NativeProbe/IndexContractProbe.cpp`（SHA256 `3eb1b71af446340dd8ab77eb12dc52205311b9a468cfa8bd125588a9968e48fe`）及运行脚本 `RunIndexContractProbe.ps1`（SHA256 `0a78a4fd17ff7bfb194e67c75ab91c0a81c878d5ac64b499c41025c3fd278ddc`）。原始机器记录为 `analysis/camera-mapping/native/index-contract/report.json`（SHA256 `43dd10b7bbd92c7a7a76499ef061ca1571874841c61a901d7ca784c401860c24`）；该 trace 使用 SDK DLL SHA256 `2fdf77e7796ed9500de0b6f65097eb810ee3146ad3dc4fad9585dad292488363`，V4.4.0_build_Apr29_2025。

### 运行边界

- 每一个 operation/index 在新子进程中创建全新 `ICameraMapTool`，只向 `ICameraMapBasicParam` 追加3对唯一值点；不调用平台 Init、订阅、模块、`Run()` 或任何设备。每项连续调用两次。进程限制5秒，隔离潜在的 native 崩溃/挂起。
- 原厂 SDK 实例创建返回0，所有本次操作均在限时内返回，子进程没有异常退出。探针在操作完成后用 `TerminateProcess` 结束进程；此前观察到独立 SDK 实例的常规销毁/卸载可能挂住，所以不让该 teardown 路径拖住或污染索引结果。由此确认了可用的安全入口是“直接 SDK 算法对象 + 子进程隔离”，不需要设备或 VisionMaster 平台。

### Runtime 结果（与源码声明分开）

- 初始对象点为 `(10,1),(20,2),(30,3)`，目标点为 `(101,201),(102,202),(103,203)`，对数为3。`GetObjectPoint` 和 `GetTargetPoint` 的索引0、1、2均稳定返回相应点，重复调用结果相同；索引3和-1两次均抛原厂异常 `0x80100005`，点对数量仍为3。该受控版本的两个 Getter 运行表现为 **0-based**，有效范围为 `[0,count)`。
- `RemoveAlignPairAt(0)` 后数量3→2，点0变为原点1；重复Remove(0)后数量2→1，留下原点2。`RemoveAlignPairAt(1)` 同样按当前索引删除，重复调用第二次删除移位后的当前点1。说明合法删除后紧密移位，重复合法索引不是幂等操作。
- `RemoveAlignPairAt(2)` 首次从数量3变为2；第二次因当前 count=2 抛 `0x80100005`，数量和剩余两点不变。初始索引3及-1每次都抛同码异常，数量和全部点值不变。

这些是该 SDK build 的直接 runtime 观察，不是 `CameraMapCpp.h` 的源码契约：头文件仍未规定下标基数、有效范围、异常码或移位规则。运行结果可以更新本机目标 SDK 行为证据为 0-based 和 `0x80100005`，但不能代替其他版本契约，也不能推出 UI 的 RelateIndex 经原生订阅后与 SDK 点下标相同。它只验证 SDK 内部 Get/Remove 索引域；A（订阅索引）到 B（ModuleFrame 数组）仍因既有平台 Init `0x80100015` 而未知。

## 原厂桌面 UI 证据边界

- 可确认：原厂 XML 资源和官方解析器构造出的控件模型、成员、默认值、类别、触发与 Setter 路径；资源哈希记在证据 JSON 中。官方 parser probe 的来源标记为 `original-parser/synthetic-model`，且 `desktopOperation:false`。
- 不可确认：VisionMaster 原厂桌面窗口的实际布局、模板渲染、交互时序、像素差异及本机/远端文件对话框行为。证据 JSON 将 desktop 明确记为 “original desktop not captured; source-derived screenshot only”。源派生图只能验证当前重建窗口，不代表原厂桌面截图。
- 当前已保存的原厂桌面截图/录屏证据为空；用户之前对当前插件线窗口的验收是当前实现的验收输入，不是原厂桌面截图证据。

## MatrixToParam SDK 源码边界与结果精度

对 `D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVDMatrixToParamCpp.dll` 单独执行 Ghidra 12.1.4 反编译；原 DLL SHA256 `9F761DC58ECC65ED86989B56953A50B9C193D619F88A520B42E07A4FB046F598`，工作树输出在 `analysis/camera-mapping/ghidra/matrix/`。关键直接证据：

- `180001990.c`（SHA256 `6AB8E67A8F0994B916D8F608B19786E2C3CE87C4A8B0C33D3F9032CE0715CACC`）的 `ImportHomoMatrix` 对空指针抛 `0x80100006`；非空时把 9 个连续 float 原样复制到工具对象偏移 `0x1c..0x3f`，未在导入时重排或归一化。
- `180001be0.c`（SHA256 `92F1B28F0F820D7E7FF2B10125D76A5D6D7BE800995EFE06A2ACFC4AAA7FD4A7`）的 `Process` 先把成功标记清零；若存储的 9 个系数全为 `0.0f`，抛 `0x80100004`。否则按原序复制 9 个 float 到算法参数，调用 `MVD_Algorithm_Process(handle, 0x6b, input, output)`。算法返回非零码时包装成 `IMVDException` type 7 并抛出；成功后按算法输出数组下标0..5保存结果，随后置成功标记。该包装层不转换角度、中心或权重，也不重排矩阵。
- Result 适配器 `180001870.c`、`180001880.c`、`180001890.c`、`1800018a0.c`、`1800018b0.c`、`1800018c0.c` 分别读取结果对象偏移 `+8,+0xc,+0x10,+0x14,+0x18,+0x1c`。SDK 公开头文件 `MatrixToParamCpp.h` 的虚方法顺序为 `GetScale, GetOffsetX, GetOffsetY, GetRotate, GetAspect, GetSkew`，故原始输出顺序为 Scale、X、Y、Rotate、Aspect、Skew；CameraMapping 模块随后按 getter 顺序发布 TranslateX、TranslateY、Rotate、Scale、Skew、Aspect。
- `180001110.c`（SHA256 `F3B9F030D1DB0902B3080C6A756A7BFF456A30DBFB46A2C86C97021CB5750734`）将通用 SDK 错误码格式化成异常文本，但 `Process` 保留算法底层返回码本身，没有映射成相机映射模块的 `0x1000010f`。后者是插件独立重建里对逆矩阵/归一化失败所用的模块错误码，不能当作 MatrixToParam SDK 原码。
- 算法核心由 SDK 通用入口 `analysis/full-audit/n-point-solver/core-native/180020480.c`（algorithm id `0x6b`）转发到隐藏算法对象的虚调用 `FUN_1807c6ba0(handle, 0x12, matrix)`。可用反编译材料中 `0x12` 对应实现体不在已分析模块/SDK 输出中；没有可见的逐项矩阵公式、算法错误码分支、角度单位、旋转中心或权重消费代码。公开 header 只定义“尺度、X/Y偏移、旋转角度、Y/X比例、倾斜量”，没有补足这些语义。

因此可源确认 MatrixToParam 的输入顺序、全零前置失败、核心入口/算法失败传播和六个输出槽位；**不可源确认**内部系数公式以及旋转单位/中心/权重。现有受控模块 trace `analysis/camera-mapping/native/module-results.jsonl` 和 110 项插件/模块检查、118 组 SDK 求解差分只能验证选定样本字段值（此前固定绝对容差 `<0.003`），没有覆盖矩阵变换域或边界算法错误码。当前 `CameraMappingPlugin.Runtime` 中逆矩阵后的 `atan2`/`sin`/`cos` 分解仍是独立重建；由于没有原厂内部公式或足以区分模型的逐矩阵原厂结果，不能据此做“按源码修复”，也不能把测试通过宣称成算法源码同源。下一步应以无设备的 MatrixToParam 专项输入矩阵探针增加结果精度证据；在拿到可证差异前不改生产分解代码。

### 专项 SDK 受控运行

为覆盖直接输入矩阵而非仅 CameraMap 生成矩阵的路径，新增 `tests/CameraMapping.NativeProbe/MatrixToParamProbe.cpp`（SHA256 `41F803DFA6FAC6E2F7AB1473DF7667855629C334CFF9F1B13040516115229124`）与 `Run-MatrixToParam.ps1`（SHA256 `CEA74631F586F3E29759CE935DACC8DF948CD5BDADA379F31FF8AF3B002AAEF2`）。8 个矩阵各自新建子进程，直接调用官方 DLL 的 Create、Import、Run 和六个 Getter；探针在输出后用 `TerminateProcess` 结束，避开此前观察到的独立 SDK teardown 挂起。运行 DLL 与上文哈希相同；JSONL trace `analysis/camera-mapping/native/matrix-to-param/results.jsonl` SHA256 `841C13F744CB2126C29C7BC2DFBC38B57858FD0B3A05DD89AA4167C98DABF89E`。

- 运行观察：单位矩阵输出 scale `0.999999881`、其余为零；平移矩阵 `(tx,ty)=(12,-7)` 输出偏移 `(-12,7)`；90°输入旋转矩阵输出 `Rotate=-1.57079637`，与 `-π/2` 数值一致。这个样本支持输出角度以弧度表达，但只是受控观察，不是算法本体源码证明。
- 运行观察：对角缩放 `(2,3)` 得到 `Scale=0.49999994`；混合仿射矩阵和透视矩阵的六字段原值完整保存在 JSONL。旋转/尺度退化样本会出现 `Scale=0`、`Aspect=-8388608`，不应在没有原厂 UI/下游消费证据前替换成“更合理”的值。
- 失败边界：全零矩阵由包装层前置检查抛 `0x80100004`（源码已确认）；非零但奇异的 `[1,0,0;0,1,0;0,0,0]` 到核心后返回 `0x10000019`、module type 7、描述 `Algorithm error.`（直接 runtime 观察，错误触发细节仍在 opaque 算法内）。模块 synthetic trace 中另一组双点奇异求解以模块码 `0x1000010f` 失败，说明不可将这个直接 SDK 原码与模块边界错误码混为一谈。

这组探针没有改变产品实现。它把可观察精度、弧度假设和两类失败码锚定为可重复样本，但没有证明旋转中心、权重语义或一般矩阵的精确分解公式；CameraMapping 生产分解仍待进一步原厂差分。

## 点对配置源级自检：删除与错误路径

原厂配置资源 `analysis/camera-mapping/parser/model/IMVSCameraMapModuAlgorithmTab.xml`（SHA256 `9832E201BC27E31413BD54BD92D55464B35C0657F681F7CA0059A695E0A77DFB`）定义 8 个 `PointPair` 和 `AddButton` 的 `MaxCount=8`；Data1、Data2 的 `Fixed=True`，其余点对未固定。安装版托管反编译 `D:/MyWork/REA逆向测试/analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/PointPair.cs`（SHA256 `78997B8DA0CB63E64976D63A2D47F36C2F1D1F5A10706B3822DE720C7CDC0017`）直接确认：删除命令依次把 `ObjectPoint`、`TargetPoint`、`ObjectPointX`、`TargetPointX`、`ObjectPointY`、`TargetPointY` 的 `CurValueA` 设空，随后无条件把 `CustomVisibleA` 设为 false；可见标记 setter 写 `Value=Name+#0` 并调用 `SetParamUnit()`。流程不检查中间 setter 成功状态、不回滚已清字段、不因一次订阅操作错误中断隐藏。`AddButton_PointPair.cs`（SHA256 `46C164AC53D170C95D4C7767D69F09A3075C6BE753CDFB471B058DD161095645`）确认达到 max 8 时拒绝，正常添加取静态源列表首个隐藏项，将其移动到 AddButton 前再激活。WPF 模板 `Apps.XmlUI/styles/paramusercontroldatatemplate.xaml`（SHA256 `96B98F06FBAF9BC2CF1F481B2A24323ECFC11831C05698E9FA5B18EC294E0F63`）确认点模式/坐标模式分别显示组合点选择器或 X/Y 成对选择器，字段左右各宽 140，行距和分隔线按原模板组织。

此前插件删除实现遇到任一订阅清理失败便立即 return，留下点对可见、`DataN#1`，且后续叶字段不再尝试清理。这与原厂命令的顺序式赋值后无条件隐藏不同。现改为按源字段顺序尽力处理全部六个清理，随后提交隐藏标记；如服务报告失败，仅提示第一个诊断，不对已执行清理做假回滚。专属 WPF/运行回归 `dotnet run --project tests/CameraMapping.PluginChecks -c Release --no-restore`：`PASS 139 camera mapping actual configuration/runtime checks`。测试将拒绝点分别放在六个 `Bind` 序列位置，逐项确认后续调用继续、Data3 均写为 `#0` 并隐藏、只提示首个错误；五个成功清理从服务数据中移除绑定，失败槽保留其绑定，配置参数除 Data3 外不变、图执行次数不变，已有标定文件字节也不变。成功路径仍检查字段次序、隐藏与重加排序。SDK 对照 `dotnet run --project tests/CameraMapping.SolverChecks -c Release --no-restore`：118 matching、0 mismatching（固定矩阵/比例绝对容差 0.003）。复核构建期间 `CameraMappingPointPairControls.cs` SHA256 始终为 `EF4A583E73A51070D078D62DED60728BAEF80876A743059B208EA9F87EE049F7`（`sourceChangedDuringBuild=False`）；NPointSolver SHA256 `F39DE6E04388B6BC9AA1B0C4B1B97B65E5C9B174380C7F8BCAA767A1C2A624CC` 未变化。本修复只影响相机映射控件，不涉及 NPointSolver；注入的订阅服务失败用于验证适配层，不代表已观察到原厂普通运行路径会以相同方式失败。

## 复核结论

本次只确认并归档现有证据能支持的边界，没有发现可以据此修复的代码差异。若后续要闭合索引行为，需要合法初始化的原厂平台算法环境并观测订阅后的真实数组项及失败状态；若要闭合桌面 UI 对照，需要原厂 VisionMaster 对应节点窗口的可核验截图或录屏及操作上下文。当前 `0x80100015` 受限状态下，两者均不得由源码意图或重建窗口推断。

## 显式导出与输入标定路径的源码顺序

原厂 UI 资源 `analysis/camera-mapping/parser/model/IMVSCameraMapModuAlgorithmTab.xml`（SHA256 `9832E201BC27E31413BD54BD92D55464B35C0657F681F7CA0059A695E0A77DFB`）将保存按钮命名为 `SaveCalibPath`，独立于输入路径控件 `CalibPathName`。托管源码 `SaveFileDialog.cs`（SHA256 `BD70974CA4F946998C4DA911BFE1503FDD3A67CD7C821C84E53FA8263BDB642C`）的 `SaveFile(path)` 先同步执行 `IUserStringData[Name]=path`，随后才调用 `CalibPathName.GetParamUnit()`。`ModuleParam.cs`（SHA256 `AF392BF2D3AF006C6A16021A271D67DD8EF2017B268580841874AB6FEDE1DA48`）显示参数 setter 调用 `SetParamValue(..., bThrowException:true)`；`VmModule.cs`（SHA256 `53E2F0F9032C4631B59CF0EB2A00E06D50469D94706E7E370E9CC7E1059C72CE`）把 `IUserStringData` setter 直接转发到该参数容器。因此原厂同步保存失败会在返回 UI 的 `GetParamUnit()` 前抛出。

原生 `CIMVSCameraMapModu::SetParam` 反编译 `analysis/camera-mapping/ghidra/module/18000ce30.c`（SHA256 `32D5350F9D7B7B980DAE1559C4C9018E7CA7F5D830EE90EC6F70D04B2789EDBB`）进一步确认：`SaveCalibPath` 分支先验证 `.iwcal`，在标定有效时缓存导出路径，调用 `ProcessCalibMatrix`，成功后调用 `WriteCalibFile` 并传播其错误码。保存成功后托管 UI 才读取 `CalibPathName`；该读取不把新导出路径写入输入路径字段。故这两个路径有不同职责：导出目标与后续输入/刷新路径不能互相覆盖。

此前插件把所选导出目标先写进 `CalibPathName` 并刷新，再写文件；这既改变输入路径，也使写文件失败时 UI 已经读到新路径。现调整为先从缓存点重算并写入所选导出目标，成功后才 `RefreshParameter(CalibPathName)`；不写该输入参数。`tests/CameraMapping.PluginChecks/Program.cs` 现覆盖成功导出后输入路径仍为原值，以及缺失目录导致导出失败时输入值保持不变、仅报告错误。最新检查：`PASS 140 camera mapping actual configuration/runtime checks`；SDK 求解对照 `118 matching, 0 mismatching`。两次构建前后 `CameraMappingPointPairControls.cs` SHA256 `EF4A583E73A51070D078D62DED60728BAEF80876A743059B208EA9F87EE049F7` 未变（`sourceChangedDuringBuild=False`）；NPointSolver SHA256 `F39DE6E04388B6BC9AA1B0C4B1B97B65E5C9B174380C7F8BCAA767A1C2A624CC` 未变。

该修复映射托管明确的同步调用顺序及原生 SetParam 处理链；测试故障路径使用缺失父目录模拟文件写失败，没有声称观察到原厂 UI 对话框行为或真实设备行为。

## 导出、输出帧与重置生命周期的边界

原厂 `SetParam("SaveCalibPath", ...)`（`18000ce30.c`，SHA256 `32D5350F9D7B7B980DAE1559C4C9018E7CA7F5D830EE90EC6F70D04B2789EDBB`）直接调用 `ProcessCalibMatrix`（`18000c200.c`，SHA256 `5DCFFEBED46D33C02B068CB4C443CEE94D9AA131BCE691D6F024FE4884225482`）后调用 `WriteCalibFile`（`18000d450.c`，SHA256 `864CC55A130A48B25943E65D399414E68DCA6F5BAFE8CF9F21210AD06D03E35D`）。前者清空并重新填充 CameraMap SDK BasicParam、Run、以当前点缓存容量追加数据；后者仅从 CameraMap SDK `GetResult` 取 9 个系数写 36 字节。该显式 SetParam 分支没有调用 `ProcessOutput`，也不调用 MatrixToParam；`ProcessOutput` 在独立函数 `18000c450.c`（SHA256 `01338247C382DD899591B2E5CE4672D65C9B7AFFF7C7F3C313AD29C2F49A8EFE`）里才读取 SDK/MatrixToParam 结果并向外部发布 CalibStatus、比例、几何字段和运行时信息。由此源码可确认显式导出会更新 SDK 内部求解结果/写文件，但不会在该调用链中重写最近一次发布的模块输出字段。

受控原厂模块记录 `analysis/camera-mapping/native/module-results.jsonl`（SHA256 `A95B14B7AF0641BF7F6244FCBACA361667871F08FBDF95F96252090ABA43D45D`）包含 `explicit-invalid-parent` 失败后再次 `SaveCalibPath` 返回 0，以及 `refresh-file-invalid-parent` 执行错误且 `writes:[]` 后 `explicit-after-io-failure` 返回成功的顺序。其后续成功过程证实刷新写文件失败没有清空 native 模块的标定有效状态与可重用点缓存。源码 `ProcessAlg`（`18000bd20.c`，SHA256 `C38D617D9C7C2AB775D1B76C29091A6D7F0BDD807DD1E2DEA6568AB13F345FE9`）先把内部状态置 1，再尝试自动写文件；`ProcessOutput` 另行发布结果，故自动写文件失败时虽然内部状态已置 1，但该次处理不会走成功输出写入。受控 trace 的失败记录 `writes:[]` 与此路径相符。

插件侧无差异需改：`SaveCalibration` 重解算后更新缓存矩阵，再写导出文件；无论显式写文件成功或失败均不改已有 `state.Outputs`。正常执行在开始时清空输出帧；自动刷新文件失败时矩阵/缓存有效标记已在写入前置为成功，但该次执行返回失败且无输出，随后显式保存可使用该缓存。这与源码顺序及上述原厂 trace 相符。新增 `CameraMapping.PluginChecks` 断言显式导出成功和写失败都保留最近一次显示的文本、记录和结果列表。最新专属检查 `PASS 142 camera mapping actual configuration/runtime checks`；SDK 求解对照 `118 matching, 0 mismatching`。这些断言复核了本插件对象的发布帧保持行为，未假称对原厂窗口做像素采集。

重置仍有明确未知边界。原厂 `Button_ResetAlgoParams.ClickCommandCmdExecute`（SHA256 `99FE1704963E58DA603A91F84057EB4A4781759FC92356168868941E27570FDC`）先 `ResetRunParamBind()`，再调用 `VmModule.ResetParam()`；托管 `VmModule.ResetParam`（SHA256 `53E2F0F9032C4631B59CF0EB2A00E06D50469D94706E7E370E9CC7E1059C72CE`）只可见到 `SetDefaultParamList(ID)`，SDK 内部 reset 对 native `ResetMember`、相机映射 SDK BasicParam、点容量、矩阵有效标记及最后一帧的作用没有本地实现/trace。Ghidra 的 `ResetMember` 函数确实把这些字段清零，但当前没有证据证明 `SetDefaultParamList` 会调用它；不能把函数名相近当作调用链证据。因此未改插件现有重置后的会话缓存行为，也未声称其和原厂重置完全一致。闭合此项需要同版本原厂模块运行证据：成功 Process 后 reset，分别检查后续 explicit SaveCalibPath 是否接受、保存系数是否保留旧尾点、以及输出/显示端口是否仍可读。

## 直接矩阵精度差分续进（四线并行，2026-10-19）

新增 [MatrixToParam 专项差分](CAMERA-MAPPING-MATRIX-DIFFERENTIAL.md) 及 [95 例完整记录](CAMERA-MAPPING-MATRIX-DIFFERENTIAL.json)。93 例原厂成功、2 例已知异常；直接执行的当前产品公式快照只有 37 例六个 float 位全部相同，4 例大坐标透视偏移超过既有 0.003 绝对阈值，最大 0.0625（该值量级一个 float ULP）。这更新了前文“未发现可定位差异”的证据边界：现已发现直接 SDK 输入域的精度差异，但尚未证明矩阵可由实际 CameraMap 求解产生，未改产品算法、未放宽阈值，也未声称真实插件/宿主已复现。reset、订阅索引和原厂桌面仍开放。此次没有运行共享托管构建或设备测试。

## 第二轮：真实求解和生产Runtime可达性已补证

[真实求解输出差分](CAMERA-MAPPING-REACHABLE-MATRIX.md)及[完整记录](CAMERA-MAPPING-REACHABLE-MATRIX.json)将上一轮任意H未知项推进到实际点输入。24组点集采用默认Data1/Data2固定活动组，原厂模块正常SDK求解和ProcessOutput均成功；相同H的生产分解快照有9组偏移超过0.003。进一步直接调用实际CameraMappingPlugin.CreateRuntime().ExecuteAsync，24次均成功，216字段固定`<0.003`对照18组失败；4点子集12组中6组失败，16点12组全失败。病态16点样本最早从ScaleX分叉，最大TranslateX差104105.375，不能统称单ULP。真实生产回放命令返回1，明确是差异而非构建失败；源码未改、容差未放宽。实际宿主窗口/Graph/原厂平台订阅仍未验证，reset未知。本轮只确认差异和归档，不猜改公共或产品核。

## B1源码修复：原厂float往返转换

[源码调用链及实施记录](CAMERA-MAPPING-B1-SOLVE-SOURCE.md)、[15函数索引/汇编哈希/前后对照](CAMERA-MAPPING-B1-SOURCE-INDEX.json)已闭合本轮切片：7fcf60先反归一化、7fd4e0再归一化，修前产品省略该float往返。两定点初始解位同而首残差系数不同；按源码顺序独立诊断H0/H1与原厂入口位同。根线审查后专属Solver已补这段转换及倒数乘法归一化，当前24组求解H/比例全部float位同原厂（源码隔离诊断），实际生产Runtime216字段仍9/24失败（修前18），最大Translate偏差0.0625；118例SDK求解对照118/0。失败保留、未放宽0.003。公共NPointSolver未改，B2继续追固定H的MatrixToParam内部差异；宿主/设备未运行，未提交推送。

## B2源码闭合并修复当前分解差异

[MatrixToParam源码与首分叉](CAMERA-MAPPING-B2-MATRIX-SOURCE.md)、[机器证据](CAMERA-MAPPING-B2-MATRIX-SOURCE.json)已将原“核心opaque”边界推进到具体800af0函数、原指令和5个既有定点hardware中间量。根线审查后分别修复Runtime逐元素除法与原厂倒数乘法不同、以及MatrixToParam行列式hoisted乘积顺序与通用Inverse不同；新增专属投影inverse，未改通用Inverse/B1求解器或公共NPointSolver。归一化单修后的3例失败floatbits完整保留；最终实际Runtime24/24成功、216字段固定0.003全部匹配，142专属配置/runtime检查通过，118 SDK求解118/0。直接95矩阵的93成功样本源码投影诊断全部过原阈值，85例六字段位同，2 SDK异常仍单列。上述有限覆盖不证明全域位同；索引、reset、特殊错误映射和原厂桌面/真实宿主差分仍开放，未提交推送。
