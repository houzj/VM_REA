# 标定板 EdgeThreshLow/High 原厂模块对照

2026-10-19更正：下方棋盘输出指纹不变的运行事实仍有效，但“SDK没有消费名称/值”的ABI推断已被后续同实例硬件断点推翻。模块和SDK两条setter路径均把name/value通过RDX/R8传给算法；实际圆点877230的options+4/+8精确随Low/High改变。六组新证据、身份与剩余板型边界见 [桥接追踪最新节](CALIBRATION-BOARD-EDGE-THRESHOLD-BRIDGE-TRACE.md)，不可再按下方历史静态反编译参数列表判定值未传入。

本报告记录 `EdgeThreshLow` / `EdgeThreshHigh` 经原厂模块 `SetParam` 的可观测效果。结果只适用于下述同实例、同一合成棋盘图的受控样本；没有把单一样本未变化外推为全板型等价，也没有改产品实现。

## 受控回放

测试入口：`tests/CalibrationBoard.NativeProbe/RunEdgeThresholdModule.ps1`，底层由 `ModuleProbe.cpp` 构造原厂 `CIMVSCalibBoardCalibModu` 实例并加载真实 `MVDCalibBoardCalibCpp.dll`。探针只替换原厂模块导入的 `ModuleFrame.dll` IO 函数，不改模块或 SDK 机器码，不执行平台 Init，不使用设备。

同一个模块实例、同一个 SDK 工具实例、同一张 480×400 合成棋盘图及相同 ROI 下，依次只改变一项阈值：

| 阶段 | Low | High | 原厂 SetParam | 原厂 GetParam | ProcessAlg | ProcessOutput 对照 |
|---|---:|---:|---:|---|---|---|
| 基线 | 20 | 40 | 均返回 0 | 分别回显 `20` / `40` | code 0，status 1 | 23 项，点数 99，误差 0.00435560197，首点 (89.5030746, 79.5030746)，FNV-1a64 `1e9fa3be488401c6` |
| 仅改 Low | 0 | 40 | 返回 0 | 分别回显 `0` / `40` | code 0，status 1 | 与基线完整输出指纹相同 |
| 仅改 High | 20 | 80 | 返回 0 | 分别回显 `20` / `80` | code 0，status 1 | 与基线完整输出指纹相同 |

回放 JSONL：`analysis/calibration-board/edge-threshold-module-run-9da93d937e0f418c80e7f1ccb7b08426/runtime.jsonl`；构建日志：同目录 `build.log`，标准错误为空。原始文件的完整 23 项输出均记录在 JSONL 中。该测试样本说明模块存储和查询的配置值确实改变，但当前合成棋盘输出没有可观测变化。

## 原厂源码及调用链

1. XML `AlgorithmTab.xml` 中两项默认值是 Low=20、High=40。资源 SHA-256：`76c9cd661a359216041b902cce8fae115566726707e749cc42c0bb9722bcbf91`。
2. 原厂模块 `IMVSCalibBoardCalibModu.dll` 的 `SetParam @ RVA 0x20d40` 将两个名称纳入识别分支。对于这两个名称，局部别名保持为空；反编译调用 SDK 工具对象 `this+0x15d98` 的 vtable `+0x60` 时只传入参数名，没有把 `SetParam` 的值字符串作为该虚调用实参。原厂模块导出 `GetParam` 在同一探针实例中则回显存储的值。因此，模块配置存储已被设置，不等价于已证明算法工具句柄收到阈值值。
3. SDK 工厂 `CreateCalibBoardCalibToolInstance` 建立对象 vtable；`+0x60` 对应 SDK 函数 `FUN_180002980`。其反编译只读取工具对象 `+0x28` 算法句柄并调用外部 `MVD_Algorithm_SetValueByString(handle)`。该函数体不消费传入参数名/值。外部导入函数对句柄内部状态的副作用没有在当前探针中暴露。
4. 核心算法函数 `FUN_1807dda50` 会读取输入结构 `+0x24/+0x28/+0x2c` 参与边缘阈值分类，但当前模块探针只能观测算法结果与模块输出，不能读取该私有结构入口字节。因此尚无可证的 XML 名称/值 → SDK 参数名/句柄/阈值槽位映射。

`GetParam` 的原厂导出 ABI 由实际 DLL 导出名确认：`?GetParam@CIMVSCalibBoardCalibModu@@UEAAHPEBDPEADHPEAH@Z`。SetParam 是 `?SetParam@CIMVSCalibBoardCalibModu@@UEAAHPEBD0H@Z`。

## 证据身份

| 证据 | SHA-256 |
|---|---|
| 原厂 `IMVSCalibBoardCalibModu.dll` | `bd62ee940a2e299c0c47e41909e64ca78c09d7717e7cefc629dbfb9e21105c1d` |
| 原厂 `MVDCalibBoardCalibCpp.dll` | `0a65d7a9e84a3ec6f662b4f05daa5a88351690ae4fbecd1748427c13c56fa308` |
| `native-decompile/output/180020d40.c` (SetParam) | `dcf7ef94f929d6aa28501de637cab5298238a8322ce9b74249ca9993027a3361` |
| `sdk-decompile/output/180002980.c` (SDK vtable +0x60 target) | `00cafbfb6be09bdbfc67ba6724bfa6f4353565aef9e02d7feba52cfa17d6bee8` |
| 受控 JSONL | `45ff3178ac26946c0edad14dd6012a32e1e667d838fe70d608a3da261cc6a40e` |
| 专属探针 `ModuleProbe.cpp` | `6e0bb35584ad93ac637201b3406b16c29e3b453603e399f59da542c4aa735dfc` |
| 专属运行器 `RunEdgeThresholdModule.ps1` | `5bbf43a077a2e9bffb795f2ff3a6e0392f5ddc4d1244edfbe8d4090df6af13f0` |
| 受控探针二进制 `probe.exe` | `ba085350cdfa7e0fd472f84316ef773008282d2ba49b02161ae8607a38de8910` |

## 边界与下一步证据

尚不能判断阈值是否在其他图案、不同板型或非默认算法模式下改变判定。输出不变只是这个 99 点棋盘样本的结果，不证明内部阈值槽未变；模块 `GetParam` 也只证明其可查询配置值。继续闭合需要在同一原厂模块实例中使用一组已由原厂 SDK 结果证明对 Low/High 敏感的图像/板型，同时在原厂 SDK 算法入口读取 `+0x24/+0x28/+0x2c`，并跟踪 `MVD_Algorithm_SetValueByString` 的具体句柄副作用。当前并无满足该映射的原厂接口或运行 trace，故保留未知，不增加产品映射或放宽测试容差。

## 验证

- 专属原厂模块回放：`pwsh -NoProfile -File tests/CalibrationBoard.NativeProbe/RunEdgeThresholdModule.ps1` 成功，脚本断言 6 次 GetParam 回显及三份完整输出指纹一致；设备测试未做。
- 稳定边界回归：`pwsh -NoProfile -File tests/CalibrationBoard.Checks/Run.ps1 -BoundaryRuntimeOnly`，exit 0，`sourceChangedDuringBuild=false`，`PASS 3 native boundary-runtime aggregate checks.` 记录：`analysis/calibration-board/verification-60ba47102b614936b496a530c02e8b22/summary.json`。
- Windows PowerShell 5.1 执行既有 `Run.ps1` 时因 .NET Framework 缺少 `IO.Path.GetRelativePath` 无法开始；以本机 pwsh 7.6.5 重跑成功。这是运行器兼容性问题，本轮未修改其实现。
