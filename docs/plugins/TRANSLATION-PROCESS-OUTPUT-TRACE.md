# 平移旋转标定执行与运行时输出顺序

## 原厂源码锚点

- 原生 DLL：`TranslationCalibModu.dll`，SHA-256 `a85a7ca08ea265b2cf881b4ff01bf7538a60e9edb7466478476584b08fe25183`。
- Ghidra 反编译 `CIMVSNPointCalibModu::Process`，RVA `0x3a8c0`，文件 `analysis/full-audit/translation-calibration/native/18003a8c0.c`，SHA-256 `bd39fdc8fdaa99709f88ff7084e2e3ddf6190f288090cc2b1e14f6832b14f9f3`。`GetProcessInput` 返回非零时函数直接返回；进入 `ProcessAlg` 后若返回非零也直接返回；两者均在 `ProcessOutput` 调用之前。零长度输入则跳过算法仍调用输出路径。
- 反编译 `CIMVSNPointCalibModu::ProcessOutput`，RVA `0x3c3f0`，文件 `analysis/full-audit/translation-calibration/native/18003c3f0.c`，SHA-256 `fce52a352b3fe6013c736157208d175234483c790c69d807fcc742713bb20d47`。输出路径先写 `ModuStatus`，其成功后写 `VM_M_SetModuleRuntimeInfo`，再写 `CalibStatus`、点数组和拟合结果。未完成分支写零误差/姿态等默认值和 `WorldRotateAngle=0`，不写 `Rotate`；完成分支从算法结果写平移/旋转误差、旋转中心、最大误差点索引及姿态输出。

## 差异与修复

插件原先无论 `Process` 成功或提前失败，都在执行末尾加入 `ModuRunTime`。这与 RVA `0x3a8c0` 的两条提前返回路径不符：缺少必需输入或 `ProcessAlg` 报错时没有进入 `ProcessOutput`，因此不应产生模块运行时写入。

现仅在无执行错误、已经走完插件输出构造时加入 `ModuRunTime`。专属检查覆盖三种对照：缺图像输入 `0xe000031c` 无运行时输出；X/Y 长度不等 `0xe0000001` 无运行时输出；两个已绑定但均为空的数组走成功的不完整输出路径，包含运行时输出。该修改实现由原厂直接返回顺序支持。

## 输出边界清单

- **点数门控和不完整输出：已覆盖。** 受控原厂证据与回放覆盖未满点数时 `CalibCurIndex`/数组前缀、零误差值，以及 `WorldRotateAngle` 存在、`Rotate` 缺席的差异；相应专属回归在 `TranslationCalibration.PluginChecks`。
- **完成输出字段：已覆盖于既有对照。** 原厂 `ProcessOutput` 输出顺序及受控回放对平移/旋转误差、中心、姿态字段和输出存在性已有验证，本轮未重跑额外 native 实验。
- **执行输入/算法错误：本轮闭合运行时字段缺席。** 原厂提前返回锚点与专属输入错误检查相符。
- **部分输出写入失败后的宿主历史值：未知。** 原厂 `ProcessOutput` 按字段顺序调用 `VM_M_Set*`，失败立即返回，故源码可确定已成功前缀先写、后续字段不调用；但独立 `ModuleResult` 以结果字典表示输出，未证明真实 VM 在后续运行失败时如何保留先前帧值。已有 `synthetic-output-failure` 仅是受控合成宿主，不能当成真实 VM 保留语义证据。
- **`RotErrMaxPtsNum` 是两个明确例外。** 在完整结果路径，RVA `0x3c3f0` 调用 `VM_M_SetInt(..., "RotErrMaxPtsNum", ...)` 后不检查返回值，立即继续读取并写 `RotWorldError`；在未完成路径也不检查该 setter 返回值，继续写 `RotWorldError`、`RotErrWorldMax` 及后续标量。相邻的多个 setter 则检查非零返回并立即结束。此处只能确认“调用返回码被忽略且源码继续执行”，不能由此推出 VM 是否落盘该端口或其旧值是否保留。独立插件以一次性结果字典构造完整帧，不能表达这个逐 setter 的异常边界；没有改写插件为虚构的等价行为。
- **SDK 两个结果对象有不同的空指针分支。** 完整输出中，先经第一个 SDK getter（vtable `+0x10`）取结果对象；若为空，源码记录 GetResult fail 并以 `0x80100203` 抛异常。若对象非空，再从其 vtable `+0x10` 取得误差/中心对象；此指针为空时跳过该组误差/中心/最大误差端口写入。随后从第一个对象 vtable `+0x18` 取得标量结果对象；该指针为空时也仅跳过 Scale、Delta、Rotate、Tilt、DirectionRatio、PixelPrecision 写入。原厂 `ProcessAlg`（RVA `0x3b0e0`）在算法返回码为零时调用第一个 getter；若为空，只把局部 `local_2f8` 置零，仍沿 `local_338 == 0` 成功返回路径退出。之后 `ProcessOutput` 再调用该 getter 时会进入上述 `0x80100203` 抛异常分支。现有成功回放只覆盖 getter 返回对象非空的场景，未证明 SDK 在什么输入或初始化状态下会返回空值；插件求解器当前返回完整结果对象，因此不模拟这些未触发的缺席分支。
- **运行时间数值单位/起止精度：未知。** 源码证明运行时仅在 `ProcessOutput` 写入；没有据此推断独立 Stopwatch 数值与原厂定时起止点逐位相同。
- **实例生命周期不能解释正常重复运行中的 getter 空值。** 构造函数初始化 SDK 实例成员 `this+0x15d98` 为 null；`Init` 调用 `ResetMember`，在成员为空时创建实例，随后调用虚函数槽 `+0x60`。`ResetMember` 本身只清模块状态字段，不销毁或置空 SDK 实例。`ProcessAlg` 在同一实例上调用虚函数槽 `+0x38`（运行前与运行后），本函数中未见替换 `this+0x15d98` 的写入。只有 `DeInit` 调用 `DestroyNPointCalibToolInstance` 并将成员置 null；下一次初始化/算法入口再创建实例。因而已确认的调用链是同一存活实例的 ProcessAlg getter 与随后 ProcessOutput getter；生命周期源码没有证明 getter 在哪些数据条件下会空，也没有证据显示普通 ResetMember 会导致实例/结果对象被销毁。虚函数槽 `+0x38/+0x60` 的具体 SDK 方法名称及 SDK 内部对象所有权未由当前反编译符号确定。

生命周期源码 SHA-256：`Init` RVA `0x3a140` `ADF7224185C74ACE4174F1FBF60211099C20537D9A44CD82F6EAD81D3507D4A3`；构造函数 RVA `0x37760` `DCDD803F2FBF43452E3A60B4D71BE0630C066216E2B5CCFE190264ABD0A3D8E9`；`DeInit` RVA `0x37a10` `18BDD42AC442CCD33DE03A7EDF43D21C5EF05636B1904CA1F1F0E8C3B13E1124`；`ResetMember` RVA `0x3d6d0` `0354A0E87B102819B86937FEA5B3C1C18A812E806F9968C4E941FFEF06A87BAA`；`ProcessAlg` RVA `0x3b0e0` `5110E0BC7C211EB2EB3789C788B69AF1A0F3CB24B2B10E82632739CBEC804087`。

## 验证与文件身份

`dotnet run --project tests/TranslationCalibration.PluginChecks/TranslationCalibration.PluginChecks.csproj -c Release`：`TranslationCalibration.PluginChecks: 7465 checks passed`，退出码 0。`TranslationPlugin.cs` 本轮前 SHA-256 为 `eb2b5b4c7d90d7c4d67056d73d75cf3dd1638e62616aca26b7a44cb2e9f74314`，修改并验证后为 `2d5d15ca326c74b7e94dbaa3663580d8c892f6e14e985a5937f23138139e2069`（`sourceChanged`）；专属检查源码 SHA-256 为 `aa8cfb39a23edc5de0344df29ca92f12e33b1d45bed17df5927287e3b164b67e`。

本项不做设备测试、不改主框架，也不替代人工桌面验收。运行时间精度和真实 VM 的历史输出值保留行为仍需原厂宿主 trace 才能闭合。
