# 平移旋转标定点数与拟合结果端口对照

本报告仅记录 `ProcessOutput` 完成/不完整输出路径的逐字段映射。证据来自原厂模块反编译、原厂托管结果类型和既有受控 DLL 回放；受控回放使用 synthetic ModuleFrame 宿主，不能代替真实 VM 故障/历史端口值测试。

## 点数门控与点数组

原厂 `CIMVSNPointCalibModu::ProcessOutput`（RVA `0x3c3f0`）先计算 `CurNum = min(member current, TotalNum)`，其中 `TotalNum = CalibPointTotalNum + RotPointTotalNum`；随后四次 `FUN_180036ae0` 分别写图像 X/Y 与物理 X/Y 数组，数组长度均为 `CurNum`，最后写 `CalibCurIndex=CurNum` 和 `CalibTotalNum=TotalNum`。在 `0x3a8c0` 完成点数门控后，此输出函数在点数未满时也会写当前前缀；未完成路径写误差默认值和 `WorldRotateAngle=0`，不发布 `Rotate`；完整路径才读取 SDK 结果。

插件实现位置 `TranslationPlugin.Runtime.ExecuteAsync` 使用 `Math.Min(state.Current,total)` 作为四路数组与 `CalibCurIndex` 的长度/数值，`CalibTotalNum=translation+rotation`。数组分别从 `Points[i,0..3]` 对应 ImageX、ImageY、WorldX、WorldY；`Results` 只在求解完成后发出结果字段。现有原厂回放按原始 `writes` 校验输出键集合及数组 float 序列；覆盖空数组、两点/八点未完成、9 点平移、追加 0–3 旋转点、手动运行、相机/组合分支等场景。

## 完成结果映射

`TranslationCalibModuCs.TranslationCalibResult` 的 XML 文档注释确认了均值/最大值语义；原生 `ProcessOutput` 从 SDK 结果对象取值并通过 `VM_M_Set*` 写端口。插件 `TranslationPlugin.Results` 使用同名 `NPointSolveResult` 成员写同名端口。表中“Pixel”及“real/world”是原厂注释用语；源材料未规定长度单位为 mm、inch 等，故不再细化猜测。

| 输出端口 | 原厂 SDK 结果取值点 / VM 写入 | 原厂说明 | 插件映射 |
|---|---|---|---|
| `TransError` | `+0x08` → `VM_M_SetFloat` | 平移估计像素平均误差 | `OffsetError` |
| `TransEstMax` | `+0x38` → `VM_M_SetFloat` | 平移像素最大误差 | `OffsetMaxError` |
| `TransErrMaxPtsNum` | `+0x40` → `VM_M_SetInt` | 最大像素误差对应点号；结果类字段声明为 float，但模块端口 setter 为 int | `OffsetMaxErrorPointNum` → Integer |
| `TransWorldError` | `+0x48` → `VM_M_SetFloat` | 平移估计真实误差 | `OffsetWorldError` |
| `TransErrWorldMax` | `+0x50` → `VM_M_SetFloat` | 平移真实最大误差 | `OffsetWorldMaxError` |
| `RotError` | `+0x10` → `VM_M_SetFloat` | 旋转估计像素平均误差 | `RotateError` |
| `RotErrMax` | `+0x58` → `VM_M_SetFloat` | 旋转像素最大误差 | `RotateMaxError` |
| `RotErrMaxPtsNum` | `+0x60` → `VM_M_SetInt` | 旋转最大像素误差对应点号，结果类和端口均为 int | `RotateMaxErrorPointNum` → Integer |
| `RotWorldError` | `+0x68` → `VM_M_SetFloat` | 旋转真实平均误差 | `RotateWorldError` |
| `RotErrWorldMax` | `+0x70` → `VM_M_SetFloat` | 旋转真实最大误差 | `RotateWorldMaxError` |
| `Scale` / `DeltaX` / `DeltaY` / `Rotate` / `Tilt` / `DirectionRatio` / `PixelPrecision` | 第二 SDK 结果对象的顺序 getter，分别写同名端口 | 尺度、平移、旋转、倾斜、方向比、像素精度 | 同名结果属性；`Rotate` 仅完整分支输出 |

结果对象的其他位置字段：SDK `+0x18` 是图像旋转中心，`+0x20` 是物理旋转中心；模块拆成 `RotCenterImageX/Y` 与 `RotCenterWorldX/Y`。`RotDirectionState` 取模块成员方向值。所有这些结果在完整分支按原生顺序发布；不完整分支写零而非省略大部分误差/姿态字段。

## 误差数值验证与界限

`TranslationCalibration.PluginChecks.Replay` 将每个受控原厂事件的成功 `writes` 与插件结果逐字段比较：输出键集合必须完全相等（仅排除独立计时和 `CalibData` 载体）；数组要求与原厂 float 转换后的序列完全相等；整数精确比较；有限标量在 `max(0.001, abs(expected)*1e-6)` 范围内比较。该报告只基于既有 fixture，不重跑或改写原厂证据文件。已验证完整平移/追加旋转误差、最大误差点号、world error 与当前索引/数组长度在这些回放场景同源；当前未发现可由该证据支持的端口映射差异，故无产品代码改动。

原厂直接来源 SHA-256：

- `TranslationCalibModu.dll`: `a85a7ca08ea265b2cf881b4ff01bf7538a60e9edb7466478476584b08fe25183`
- `native/18003c3f0.c` (`ProcessOutput`, RVA `0x3c3f0`): `fce52a352b3fe6013c736157208d175234483c790c69d807fcc742713bb20d47`
- `TranslationCalibResult.cs`: `70653445747beda537303d916f4545de5fe9874258dc1438180edeba383a25c1` (反编译托管类型，字段的中英文说明与缺省值)

点数组在不完整路径的输出前缀长度、完成后的误差字段回放已由以上方法/fixture 覆盖。真实 VM 对一次输出中途某个 `VM_M_Set*` 失败后的旧字段保留、实际物理量单位，以及 max-error point number 的 UI 展示基数，仍需原厂 VM trace 才能确定；测试中所称 point number 是 SDK 同名值，不自行解释为零基/一基索引。
