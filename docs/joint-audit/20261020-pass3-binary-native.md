# 联合审核第三轮：原版图像二值化原生模块边界

## 取证范围与可复验材料

只读分析安装原件 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/ImageProcessing/IMVSBinaryModu/IMVSBinaryModu.dll`，SHA-256 `C03BCB19EF456206E3ABF4D2A8D5279BE37C12EB5662E191F0965029CDABEB48`。隔离副本同哈希，位于 `analysis/joint-audit/binary-native/stage/`。REA `open_binary` 可识别该件，但其 Ghidra provider 返回 `not_configured`（缺 `GHIDRA_INSTALL_DIR`/Java）；故使用仓内 `.tools/native-analysis/ghidra_12.1.4_PUBLIC` 与独立 JDK，在 `analysis/joint-audit/binary-native/project/` 私有项目反编译。原始函数 C、逐指令汇编、调用边见 `analysis/joint-audit/binary-native/output/`，逐文件哈希见同目录上层 `output-sha256.txt`，控制台与 Ghidra 日志亦在该审计目录。没有加载或修改安装原件、产品代码及 `tool-plugins`。

已导出 `CIMVSImageBinaryModu` 12 个函数：构造 `0x18001e120`、析构 `0x18001e160`、`DeInit 0x18001e260`、`DeleteModuBuf 0x18001e2b0`、`GetParam 0x18001e2e0`、`Init 0x18001e750`、`Process 0x18001e8d0`、`ProcessAlg 0x18001ec00`、`ProcessOutput 0x18001f100`、`ResetMember 0x18001f940`、`SetAlgToolSharedMemNodeNum 0x18001f950`、`SetParam 0x18001f9f0`。Ghidra 对 `ProcessAlg` 报有 unreachable-block 警告，关键参数/ABI 判断均以汇编复核。

## 包装层源码已证

- 安装资源 `IMVSBinaryModuAlgorithmTab.xml` SHA-256 `08AE81E7C8BC57B4E095AE2D0B98F08D8AFE8F34EBB065B928DD4336EF87FDD7` 的 `EnumEntry` 明列 HardThreshold=`1`、MeanThreshold=`2`、GaussianThreshold=`3`、Sauvola=`8`、OTSU=`4`，五项 `IsImplemented=0x1`。这是配置/可选项证据，不是五段像素算法的反编译。
- `Process(0x18001e8d0)` 清状态后通过输入图像对象虚表 `+0x20` 判断值是否为 `1`；符合才进入 `ProcessAlg`、成功再 `ProcessOutput`，否则返回 `0xe0000503`。该虚表成员的语义与 `1` 的像素格式对应关系尚未在原始类型布局中确认，不能写成“只接受 Mono8”。
- `ProcessAlg(0x18001ec00)` 在工具实例空时调用 `CreateImageBinaryToolInstance`，随后将输入 `+0x10` 的图像对象交给 `IImageBinaryTool::SetInputImage`（虚表 `+0x10`）；遍历 ROI，并将输入区域图 `+0x118` 或空指针交给 `SetRegionImage`（虚表 `+0x80`）；随后调用 `Run`（虚表 `+0x70`）。虚表偏移按原厂 `MVDAlgorithmSDK/Includes/Algorithms/ImageBinaryCpp.h` 接口顺序核对，头文件 SHA-256 `51AED98FDC735FAB0808974A9E35540D41431E7E119888DCC8BDF93E67EBC851`。`GenerateMaskImage` 失败会直接返回错误；创建失败会抛出 `0xe0000002`。本函数没有 `BinaryType` 的五路分支，也没有读出 RGB/Mono8 后执行转换的分支。
- `SetParam(0x18001f9f0)` 将 `BinaryType`、比较类型、阈值、均值/高斯核、Sauvola 窗口等键值直接交给 SDK `SetRunParam`（虚表 `+0x58`）。例外：汇编 `0x18001fbd1–0x18001fc1d` 对 `SauvolaAdjustCoef` 依次做 `atof`、`CVTSD2SS`、乘常量 `1000.0f`、`CVTTSS2SI`（向零截断）、`sprintf_s("%d")` 后传 SDK；`0x18001fc4c–0x18001fc98` 对 `SauvolaRange` 同法乘 `10.0f`。常量从原 DLL PE 节数据 RVA `0x2a998`、`0x2a994` 读取。旧伪代码省掉 `sprintf_s` 的变参，不能据此认定数值原样透传。
- `ProcessOutput(0x18001f100)` 先回写 `ModuStatus`，仅当内部状态为 1 时调用 SDK `GetResult`（虚表 `+0x28`）和结果 `GetOutputImage`（结果虚表 `+8`），结果空抛 `0xe0000315`，图像输出发布失败则返回错误；之后发布 ROI 坐标及模块运行信息。没有在每次处理成功/失败末尾销毁工具实例。
- `DeInit(0x18001e260)` 与析构 `0x18001e160` 在实例非空时调用 `DestroyImageBinaryToolInstance`。`DeInit` 汇编 `0x18001e269` 把 `this+0x15d98` 装入 RCX，`0x18001e275` 调销毁入口；因此确证参数为该工具实例，虽然伪代码遗漏了它。调用后置空指针及状态。普通 `ProcessAlg` 的早退/抛错路径本身未逐次销毁此实例；是否由上层异常/生命周期最终调用 `DeInit`，需沿宿主调用链单独确认。

## 下游算法与格式门的精确缺口

`IImageBinaryTool` 的实现位于原版 `MVDImageBinaryCpp.dll`（SHA-256 `BE4E6B8E2A15748144F52C66E810C1479ACAAF2AE05F3739F87C919284C1EB72`），并通过算法 ID `0x25a` 进入 `MVD_Algorithm.dll`（SHA-256 `85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549`；该调用边已由 `docs/plugins/IMAGE-BINARY-SOURCE-AUDIT.md` 的既有私有 Ghidra 证据覆盖）。本轮未取得通用算法库内五模式的分派函数与逐像素循环。因此硬阈值、均值、高斯、Sauvola、OTSU 的比较/边界/遮罩算法不能称源码级闭合；RGB 转换、Mono8 限制和 InputMask 像素格式也不能由包装层“没有检查”推导为 SDK “接受任意格式”。已安装 SDK 头文件只提供接口声明，没有具体格式白名单。

下一项可改变状态的证据是：在同一隔离 Ghidra 工程中从 `MVDImageBinaryCpp.dll` 的 `IImageBinaryTool::Run`/`SetInputImage` 虚表目标和算法 ID `0x25a` 的句柄调用继续追入 `MVD_Algorithm.dll`，恢复五模式 `BinaryType` switch、输入图像格式读取/转换、区域图/Mask 格式读取、异常释放；遇到间接调用时以 vtable 实地址与寄存器活跃性核对。若这些函数体仍无法定位，应以不同格式、五模式、含 Mask/ROI 的受控 SDK 探针形成输入—输出—错误码矩阵，并明确标为运行证据，不能替代源码。

对联合审核 FCR-031 的建议：将“原版输入候选包含 IMAGE”与“原生二值化算法接受具体像素格式”拆成两个命题；前者已有框架源码证据，后者仍未知。暂不把 UI 输入候选过滤成 Mono8，也不凭单图夹具实现四个未证模式。Sauvola 两个参数的传递尺度应按本报告汇编修正审计假设。
