# 联合审核第五轮：Binary 主框架交接与核心入口

## 本轮结论

本轮先核主框架送入插件的图像格式、插件参数/运行/结果/错误/释放链路，再沿原版算法 ID `0x25a` 定点追到通用库。**这是代码级联合审核；工具 UI、后台和五模式算法符合性仍是后续完整审核目标，不把其他与工具无关的框架层纳入此轮。** 当前 `src/VisionPlugins.ImageBinary/ImageBinaryPlugin.cs`（SHA-256 `14F8FEA1D216613001341BA6D6794184AEC6C9C9EF40319D7D94E0B462CB1A35`）的编辑器有五种选择，但运行时 `ExecuteAsync` 在 `BinaryType != 1` 时返回 `BinaryModeUnavailable`，输入端口限 `GrayImage`，硬阈值为独立 CPU 实现。这是明确的当前实现边界，不能称原版五模式已复刻。

## 原件、方法与原始材料

原 `MVD_Algorithm.dll` SHA-256 `85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549`，隔离副本位于 `analysis/joint-audit/binary-core/stage/` 且哈希相同。上一轮全量 Ghidra 因 Java 堆内存耗尽，本轮新私有 `project/` 采用 `-noanalysis`，仅从导出及直接虚表目标反编译；脚本/命令在 `CoreExportAudit.java`、`CoreFocusedAudit.java`、`Run-CoreExportAudit.ps1`、`Run-CoreFocusedAudit.ps1`，控制台原始日志在同审计目录。导出 C/有限反汇编在 `output/` 与 `focused/`，逐文件哈希见 `focused-sha256.txt`（清单 SHA-256 `B14E8A8CAFEA51C2D7BE32E9DCA2840B10AF5EEA632AB44803DAF520302FA213`）。`-noanalysis` 的多数定点函数虽可反编译，Ghidra 函数 body 仅 1 byte，`asm.txt` 为空；这限制对其内部 ABI 的独立汇编结论。调用入口寄存器则由上一轮 `MVDImageBinaryCpp.dll` 的实汇编 `0x180002855–0x180002869` 验证。一次更深 `0x180d54d40` 定点尝试返回 `NO_FUNCTION`，随后已停止自有进程；不拿缺失体推断模式算法。

## 源码级调用链与失败边

1. 原版 `MVDImageBinaryCpp.dll` 已核工厂 `0x180001000`、虚表 `0x180007958`：`SetInputImage +0x10→0x180001ac0`，`SetRunParam +0x58→0x180002000`，`Run +0x70→0x1800021b0`。构造 `0x180002a30` 的实汇编以 `RCX=&handle`、`EDX=0x25a`、`R8D=0` 调 `MVD_Algorithm_CreateHandle`；运行实汇编以 `RCX=handle`、`EDX=0x25a`、`R8=&input`、`R9=&output` 调 `MVD_Algorithm_Process`。`SetRunParam` 除 `ImageBuffNum` 外把参数名/值交给 `MVD_Algorithm_SetValueByString`。这些入口在上一轮 `analysis/joint-audit/binary-sdk/output-MVDImageBinaryCpp/` 的 C/汇编及哈希清单中可核。
2. 本轮新取通用库导出：`CreateHandle 0x180014930` 对 `0x259 ≤ ID < 0x26f`（含 `0x25a`）调用构造 `0x180060cd0`；该构造写虚表 `0x181ebc438`。从 DLL PE 节直接读该表：`+0x18→0x1800612d0`、`+0x28→0x180066610`、`+0x0→0x180060e20`。`MVD_Algorithm_Process(0x1800152b0)` 对空句柄返回 `0x80100006`、库未准备返回 `0x80100015`，否则调用句柄虚表 `+0x18`；`SetValueByString(0x180015460)` 同样检查后调用虚表 `+0x28`，保留参数名和值。这里的寄存器实参由上游包装层汇编交叉核对，不能因下游伪代码省略参数而认定丢参。
3. `0x1800612d0` 内部对 `ID=0x25a` 选择 `0x180062640`，随后调用公共收尾 `0x1800622d0`；参数入口 `0x180066610` 经 `0x180062420` 查参数名，未知名回 `0x8010001e`，未初始化存储回 `0x80100003`，再调用底层字符串值写入。`0x180062640` 建立图像/ROI/Mask 参数并调用更深的算法入口 `FUN_180d54d40`；该深层体本轮没有恢复，故五模式具体像素循环、格式拒绝谓词、边界和相等比较仍未知。
4. 原厂 SDK 像素格式头 `MVD_ImageParamDefine.h` SHA-256 `90D74ACF244C3D5C8FA37550D1BB684F523E7F1DC2ABFA2E6DA52955568EEC77` 将 `1` 定义为 `MVD_PIXEL_MONO_08`、`201` 为 `MVD_PIXEL_RGB_RGB24_C3`。上一轮 `MVDImageBinaryCpp::FUN_180004100` 对两者均原值映射，并交给算法输入结构；因此格式错误不是包装层在映射时把 RGB201 丢弃。包装层无法证明核心会接受 RGB；实际核心门需看更深源码或受控 SDK 差分。
5. 核心 `MVD_Algorithm_DestroyHandle(0x180015270)` 空句柄回 `0x80100006`，非空经虚表 `+0x0` 析构并销毁日志实例；当前对象析构 `0x180060e20→0x180060e50` 清自有缓存、结果及参数状态。原包装层 `DestroyImageBinaryToolInstance` 在模块 DeInit/析构被调用，普通 Run 失败未见每次销毁；上层最终释放时序仍依赖宿主生命周期，不能宣称异常路径已无泄漏。`0x1800612d0` 调公共运行收尾 `0x1800622d0`，但深层抛错及回收是否都到达该路径尚未闭合。

## 一项受控运行差分，仅用于消解格式歧义

先审过旧 `tests/NativeImageBinaryProbe/Run-Modes.ps1`：它会向历史固定 `analysis/source-audit-regression/20261018/native-binary-modes/` 写文件，且旧产物在当前工作树不存在；因此**没有运行旧脚本或覆盖历史路径**。本轮将同版本 SDK 依赖复制到 `analysis/joint-audit/binary-core/probe-actualformat/runner/`，独立编译 `BinaryFormatProbe.cs`（SHA-256 `88C5CEE5C9FD6E772EF10065BC87BDD7BA75118828AE64B111E0340E7F754A6F`），只在自有输出目录执行，既无设备也无安装目录写入。源 20×16 彩色 BMP SHA-256 `7A6DC68E81BFE4365CAB661EECE892155A4B81BD7E4529A1398D6ADFF3B9AA2A`；`results.csv` SHA-256 `55CBAD3C3DEAA4C6A59AB4BE263F9B1AC02CB1FE51448FBD4FDDB6431876A79D`，输出/依赖/执行程序逐件哈希在 `probe-sha256.txt`（清单 SHA-256 `1345F69A14ABB4CB936A4B3F20FB9150B45D408AEEE5EA0BC779B4866D39D8CD`）。

探针逐次新建图像和工具，记录 `InitImage` 后的**实际** `PixelFormat`：实际 Mono8 下 `BinaryType=1/2/3/8/4` 均成功且各生成 20×16 `Format8bppIndexed` 输出；实际 RGB_RGB24_C3 下相同五模式均抛 `VisionDesigner.MvdException` 的 `MVALG_STS_ERR_IMG_FORMAT`，其 SDK 枚举数值经反射核为 `0x10000007`。这一组结果证实当前 SDK 在该夹具拒绝 RGB201，不说明所有彩色格式、所有图像/Mask 组合或原桌面宿主的完整行为；也不能单凭输出成功反推出五模式的原始像素公式。

## 下一步

先继续主框架 `ImageSource` RGB/附加 Mono8 → typed IMAGE 候选 → `ImageBinary` 配置 → `Run` 的代码级对账：当前端口为 `GrayImage`，而原版框架候选可列 IMAGE；应区分“候选可见”“订阅后原 SDK 格式门拒绝”和“独立兼容转换”三个状态，不能由此简单过滤 RGB 候选或静默灰度转换。再以定点反编译/受控差分补 `InputMask`、ROI、输出/错误释放和五模式算法；保留源码、当前实现、探针三种证据等级，不把此轮一张图的五模式成功当算法符合性完成。
