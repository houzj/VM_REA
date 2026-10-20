# 平移旋转标定标量输出位型对照

## 比较方法

原厂受控回放 `OriginalTranslationProbe.cpp` 的 `setFloat` 接收 `float`，并由 `num(float)` 以 `%.9g` 写入 JSON。九位有效数字足以恢复 binary32；因此本次把 evidence 的原厂十进制值读回 `float` 并读取 IEEE-754 原始位型。插件侧对 `PortValue.Number` 同样转换为 `float` 后取位型。这比较的是输出端口最终 binary32 值，而不是对 JSON 十进制文本做字节比较。

原厂反编译 `CIMVSNPointCalibModu::ProcessOutput`（RVA `0x3c3f0`）从 SDK 结果对象取 float 到局部变量，再直接调用 `VM_M_SetFloat`；该路径没有额外的 double 转换或格式化。`TranslationPlugin.Results` 则把 `NPointSolveResult` 的 double 字段转成 float 后建端口值。对照结果显示差异已在 SDK getter 与插件 `NPointSolver` 结果值之间出现，不能归因于 JSON 日志十进制舍入或 `ProcessOutput` 的额外数值转换。

## 位型差异

基于现有 `translation-calibration-evidence.json` 的成功标量输出比较，发现 **172 个**原厂/插件 binary32 不同值，分布在 18 个字段。全部仍落在现有数值容差内；故本轮没有收紧为位级断言，也没有扩大容差。

最大绝对差：

- `append-rotation / RotCenterWorldY`：原厂 `1390.2864`，bits `0x44ADC92A`；插件 `1390.2858`，bits `0x44ADC925`；差 `0.0006`（同号相差 5 个相邻 float 编码）。
- `append-rotation / DeltaY`：原厂 `753.4807`，bits `0x443C5EC4`；插件 `753.4802`，bits `0x443C5EBC`；差 `0.0005`。
- `append-rotation / RotCenterImageY`：原厂 `753.4807`，bits `0x443C5EC4`；插件 `753.4803`，bits `0x443C5EBD`；差 `0.0004`。

另有接近零的符号翻转：`complete-translation / DeltaX` 原厂 `+2.1274268e-6` (`0x360EC4EA`)，插件 `-1.9807087e-6` (`0xB604EC51`)，绝对差 `4.1081355e-6`；`DeltaY` 原厂 `+2.2741465e-6` (`0x36189D8A`)，插件 `-3.3011809e-6` (`0xB65D89DB`)，绝对差 `5.5753274e-6`。近零符号差在方向比等字段的多个场景也出现，不能把其当作日志舍入后强制归零。

按字段最大绝对差：

| 字段 | 位型不同数 | 本组最大绝对差 |
|---|---:|---:|
| `DeltaX` | 11 | 0.000216 |
| `DeltaY` | 7 | 0.0005 |
| `DirectionRatio` | 25 | 7e-7 |
| `PixelPrecision` | 3 | 5e-7 |
| `Rotate` | 19 | 3e-7 |
| `RotCenterImageX` | 9 | 0.000228 |
| `RotCenterImageY` | 5 | 0.0004 |
| `RotCenterWorldX` | 11 | 0.000226 |
| `RotCenterWorldY` | 7 | 0.0006 |
| `RotErrMax` | 11 | 2.3284e-5 |
| `RotError` | 11 | 1.134e-5 |
| `RotErrWorldMax` | 11 | 4.891e-5 |
| `RotWorldError` | 11 | 2.3826e-5 |
| `Scale` | 3 | 1.8e-7 |
| `Tilt` | 19 | 3.65e-7 |
| `TransError` | 3 | 5e-7 |
| `TransErrWorldMax` | 3 | 4e-6 |
| `TransEstMax` | 3 | 4e-6 |

## 已追到与尚未知

平移结果由插件 `NPointSolver.Solve` 生成，包括选点拟合、SDK风格 float 逆矩阵、反投影残差、float 累加平均与误差/像素精度计算；旋转结果由 `SolveRotation` 生成，包括角度映射、中心拟合、最大/平均残差和 world 误差。插件端 `Results` 是同名字段的薄映射并在边界处转成 float。原厂 `ProcessOutput` 的 getter/vtable 槽位及 setter 顺序已记录于 [字段映射](TRANSLATION-RESULT-FIELD-MAP.md)，但当前原厂模块输出不暴露相应 SDK 结果对象内部 double/float 中间矩阵或误差累计值。因此虽然能证实输出结果的位型差异，并能排除 ProcessOutput 写端转换为原因，仍不能把误差归结到某一个原厂 SDK 中间运算步骤；本轮不据差异调参或改算法。

检查保留既定标量数值容差，同时在每次回放时生成完整的位型差异清单供复核。最近一次 `TranslationCalibration.PluginChecks` 为 `7465 checks passed`，退出码 0；清单 `analysis/full-audit/translation-calibration/checks/scalar-bit-differences.txt` SHA-256：`fe4e1a70eed3189a7019f3a788dcfc0abcff09cf705aa991da81ab7ebc92a5d2`。原始 probe `OriginalTranslationProbe.cpp` SHA-256：`003bce4dfc1348eed67a29e8f10961841814004ecc857e75700b90c8f0503183`；原厂证据 JSON SHA-256：`58d502cb0d8d7dbd145bd81776193402816408cdb24450cf621b73d68ae729c9`；检查程序 SHA-256：`e77fa6130d644ec5fd52b4e9818251d475023e3caeae32b853efcafd7df8185e`；`NPointSolver.cs` SHA-256：`f39de6e04388b6bc9aa1b0c4b1b97b65e5c9b174380c7f8bcaa767a1c2a624cc`。
