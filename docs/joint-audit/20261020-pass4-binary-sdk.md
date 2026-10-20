# 联合审核第四轮：图像二值化 SDK 原生调用边

## 本轮证据与边界

原厂 `D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVDImageBinaryCpp.dll` SHA-256 `BE4E6B8E2A15748144F52C66E810C1479ACAAF2AE05F3739F87C919284C1EB72`，原厂 `MVD_Algorithm.dll` SHA-256 `85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549`。两者先复制到 `analysis/joint-audit/binary-sdk/stage/` 并逐件校验哈希；私有 Ghidra 项目、脚本、控制台和导出都在该审计目录。包装层函数原始 C/汇编/调用边位于 `output-MVDImageBinaryCpp/`；88 个导出文件的逐文件 SHA-256 在 `imagebinary-output-sha256.txt`（该清单自身 SHA-256 `E951AB963CC9765E7513FF3505AE0B8951FC330B813C5A2E8A175200C5690C31`）。本轮没有改安装原件、产品或 `tool-plugins`。

53 MB 通用算法 DLL 的首次 Ghidra 导入进入 `ANALYZING all memory and code`，约 11 分钟后 `console-MVD_Algorithm.log:370,396,404` 明确出现 `java.lang.OutOfMemoryError: Java heap space`；未运行到 `SdkSourceAudit.java`，故没有目标函数导出。已通过当前审计会话中断该**自有**分析进程，保留完整失败日志与隔离工程；没有操作其他任务的进程。因此以下“SDK 已证”只指 `MVDImageBinaryCpp.dll`；不能称算法库源码已恢复。已有 REA Ghidra provider 仍是 `not_configured`，本轮用本机离线 Ghidra 的隔离副本，不依赖该 provider。

## 由真实虚表和 ABI 复核的调用链

- `CreateImageBinaryToolInstance(0x180001000)` 分配 `0x70` 字节对象，将虚表写为 `0x180007958`，再调初始化 `0x180002a30`。逐个读取 DLL PE 节中的虚表指针：`+0x10→0x180001ac0` (`SetInputImage`)、`+0x58→0x180002000` (`SetRunParam`)、`+0x70→0x1800021b0` (`Run`)，与原厂 `ImageBinaryCpp.h` 的接口顺序吻合。不是根据方法名猜测间接调用目标。
- 初始化 `0x180002a30` 汇编 `0x180002ab8–0x180002aca`：`RCX=&object[0x28]`、`EDX=0x25a`、`R8D=0`，调用导入 `MVD_Algorithm_CreateHandle`。据此重新证实算法 ID `0x25a` 和句柄持有字段 `+0x28`，不再仅依赖旧文字报告。创建失败转原生异常，错误置于本地异常结构；函数还创建结果、调试输出和区域图助手。
- `SetInputImage(0x180001ac0)` 对空图抛 `0x80100006`，否则把图指针存到 `object+0x10`。汇编 `0x180001b15–0x180001b27` 之后用 `RCX=object+8` 尾调用预处理 Mask 工具虚表 `+0x10`，入参 RDX 仍是输入图指针；Ghidra 伪代码漏写该实参。这里没有把 RGB 直接转成灰度的代码。
- `SetRunParam(0x180002000)` 除 `ImageBuffNum` 单独经 `atoi` 写入 `object+0x68` 外，以持有句柄调用 `MVD_Algorithm_SetValueByString`；非零错误封装为原生异常。汇编表明参数名和值保留原入口 RDX/R8，不能把伪代码只显示一个实参理解为丢参。上游 `BinaryType`、Sauvola 参数由此进入通用算法句柄。
- `Run(0x1800021b0)` 检查输入图 `object+0x10` 与结果对象 `+0x38` 非空；设置 ROI/Mask 或 RegionImage 的图像结构，必要时创建输出图，随后汇编 `0x180002855–0x180002869` 以 `RCX=handle(object+0x28)`、`EDX=0x25a`、`R8=&input`、`R9=&output` 调 `MVD_Algorithm_Process`。`object+0x40` 区域图非空时，其宽、高、格式、首行数据被打包为 Mask 输入；为空时可从预处理 Mask 工具生成，或走 ROI 参数。`Run` 的错误分支抛异常，成功将运行状态设 1。
- 图像结构映射 `FUN_180004100(0x180004100)` 对像素格式数值 `1/2/3`、`100–105`、`200–205`、`300–307` 原样返回，其他返回 `0`；`FUN_180004320(0x180004320)` 将输入图格式、尺寸、数据指针/行信息装入算法输入。仅可证“数值映射”；这些数值的正式 `IMvdImage` 枚举名称及算法库收到 `0` 后如何处理尚未核对，不能写成特定 RGB/Mono8 白名单。
- 销毁导出 `DestroyImageBinaryToolInstance(0x180001080)` 对空实例返回 `0x80100006`；非空经对象虚表 `+0x0→0x1800019f0` 析构。析构进入 `FUN_180002d00` 后可释放对象，但 `FUN_180002d00` 的具体持有句柄/Mask/图像所有权顺序尚未导出，不能断言失败路径无泄漏。

## 未闭合与可改变状态的下一证据

通用库的 `MVD_Algorithm_Process`/`SetValueByString` 导出仅是调用入口。尚未恢复 ID `0x25a` 对应的五模式分派、硬阈值/均值/高斯/Sauvola/OTSU 像素循环、边界扩展、相等比较、输入图与 Mask 的格式门、Run 抛错到句柄回收的完整路径。本轮全量 Ghidra 自动分析被 Java 堆内存耗尽阻断，精确失败日志在 `analysis/joint-audit/binary-sdk/console-MVD_Algorithm.log`。下轮可用 `-noanalysis` 在新的私有工程导入，再只分析三个导出体及 ID `0x25a` 的分派表引用，逐步扩展被调用函数，保留每次错误日志；不能对已失败工程直接宣称函数索引完整。

一项可复验的独立运行探针：在自有临时副本与输出目录调用同版本 SDK `CImageBinaryTool`，固定一张 2×2 或 4×4 具有不同 RGB 通道值的彩色图及对应 Mono8 图，分别遍历 `BinaryType=1/2/3/8/4`，再分别加入 Mono8、RGB24 RegionImage/Mask；每次记录输入图 `GetPixelFormat` 数值、SDK 返回或异常码、输出图格式/尺寸/逐像素 SHA-256、未响应/释放结果。该探针只能给运行差分，不替代模式与格式门的源码级结论，也不得在安装原件目录或他任务的 TempFiles 下创建会清目录的服务。
