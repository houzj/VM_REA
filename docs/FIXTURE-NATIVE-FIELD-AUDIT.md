# Fixture 原生字段及当前托管映射核对

## 证据与范围

本报告只核对已反编译的托管入口、安装包模块 XML、SDK 数据类和当前框架代码；未在原生 VM 中执行同状态差分，也未分析位置修正算法的原生机器码。因此字段名称和传输顺序有静态证据，角度单位、坐标变换公式、异常和数值舍入仍未证实。

| 证据 | 关键位置 | SHA-256 |
| --- | --- | --- |
| `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Location/IMVSFixtureModu/IMVSFixtureModu.xml` | 输出 `Position Correction Info` 及十叶 | `577A67C5BC661A4C314C6EB1879CAED1C82789F91D33D04B256B1FBCEB585AC6` |
| `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/ImageProcessing/IMVSImageFixtureModu/IMVSImageFixtureModu.xml` | 输入/隐藏输出 FIXTURE | `05A6E9B9B25D484A6D43521A403433597FD2A486047298EFA899DE8C31C5DCFD` |
| `analysis/decompiled/VM.Core/VM.Core/DynamicOutput.cs` | `GetOutputFixtureArray` | `F995E91E30AE21C1B17E9746D5657C4AB6FC9A868F315D4DFB9031B264DDF92D` |
| `analysis/decompiled/VM.Core/VM.Core/DynamicInput.cs` | FIXTURE 输入的十项 float 封送 | `2F46BC4E116CC953DAF835C6719F429ABE3A11218DAC6201BF105626237589BD` |
| `analysis/decompiled/VM.Core/VM.Core/VmModule.cs` | `GetVmIOFixtureValue` 按叶数组重组 | `53E2F0F9032C4631B59CF0EB2A00E06D50469D94706E7E370E9CC7E1059C72CE` |
| `analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/Fixture.cs` | 公共数据类与缺省构造 | `DD040114EAC9542C4EAEA8987DA0B0B86456A4D3116CBEAA34A4F06068BC6C45` |

`IMVSFixtureModu.xml` 的外层 `Position Correction Info` 为 `Style="FIXTURE" IsPrefer="true"`，其十个数值叶均为 `float`、`IsForce="false"`、`IsArray="false"`。两组 POINT 子树没有显式 `IsArray` 属性；不能把这一缺省属性推断为原生跨轮累积数组。`IMVSImageFixtureModu.xml` 的同型输入无显式数组属性，输出 `Output Position Correction Info` 连同子树和十叶均标 `Visible="false"`。故展示叶字段应按**模块输出可见性**决定，不能仅凭类型 FIXTURE 向所有模块公开。

## 原生十叶与托管值

| 原生层级/叶 | 当前 `FixtureValue` 对应成员 | 当前可作为数值叶投影？ |
| --- | --- | --- |
| `Fixtured Point.FixtureInitPointX` | `Reference.X` | 是，标量 Number |
| `Fixtured Point.FixtureInitPointY` | `Reference.Y` | 是，标量 Number |
| `FixtureInitAngle` | `Reference.Angle` | 是，标量 Number；单位未差分验证 |
| `FixtureInitScaleX` | `Reference.ScaleX` | 是，标量 Number |
| `FixtureInitScaleY` | `Reference.ScaleY` | 是，标量 Number |
| `Unfixtured Point.FixtureRunPointX` | `Run.X` | 是，标量 Number |
| `Unfixtured Point.FixtureRunPointY` | `Run.Y` | 是，标量 Number |
| `FixtureRunAngle` | `Run.Angle` | 是，标量 Number；单位未差分验证 |
| `FixtureRunScaleX` | `Run.ScaleX` | 是，标量 Number |
| `FixtureRunScaleY` | `Run.ScaleY` | 是，标量 Number |

映射的方向来自 `DynamicInput` 将 `stInitPoint.X/Y, fInitAngle, fInitScaleX/Y, stRunPoint.X/Y, fRunAngle, fRunScaleX/Y` 顺次写入十个单元素 float 值；`VmModule.GetVmIOFixtureValue` 从相同层级的十组 `float?[]` 重组基准和运行信息；`DynamicOutput.GetOutputFixtureArray` 把 `IVmFixture[]` 转为 `List<VM.PlatformSDKCS.Fixture>`。其中空元素会被输出入口跳过，空数组与 null 的行为亦与当前托管值不同。

`IMVSFixtureModuDisplay.xml` 把基准点 X/Y 映射到图上 `BasePoint`，运行点 X/Y 映射到 `RunningPoint`，并显示模块状态。此证据说明两组位置点的展示用途，不证明当前 `FixtureValue.Transform/WarpMask` 的公式与原生一致。

## 当前实现差异与最小闭合建议

当前 `src/VisionFramework/RoiRuntimeValues.cs` 的 `FixtureValue(Reference, Run)` 能保存十个 double 值，`src/VisionFramework/PortArrays.cs` 支持标量及 `FixtureArray`、逐项取值和嵌套累积扁平化，`GroupBreak.Accumulatable` 包含 Fixture。已有调用检查用真实径向圆结果跨两层 Group 验证托管 Fixture 数组传输。`src/VisionFramework/CompoundOutputFields.cs` 当前**没有 Fixture 叶**，因此根 Fixture 可以传输和累积，但不能从已有输出订阅十个数值叶；这是本次可闭合的明确缺口。

最小实现是对**有可见 FIXTURE 输出、且根值为有效 `FixtureValue`**的描述符增加上表十个可选 `Number` 标量端口，并在同一次执行结果中从根值投影对应值。对于 `FixtureArray`，十叶宜保持 `NumberArray` 并逐项投影；数组长度跟随根值，不能用上一轮缓存填洞。普通订阅、Group 输出和累积继续沿现有端口类型检查执行。输出被标记 `Visible=false` 的模块不得自动公开叶端口。实施时须验证字段选择界面、运行取值、数组展开、重命名/删除传播以及保存重开；这项仅为建议，**此报告未实施产品代码，也未声称功能闭合**。

当前 `FixturePose.Validate` 要求 `ScaleX`、`ScaleY` 严格大于零，且默认值为 1。原生公共 `Fixture()` 构造把四个缩放值设为 0；已检查的 XML 和输入封送没有同样的正数校验。这是合同差异，尤其当原生输出缺省/零尺度时当前 `PortValue.Fixture` 会拒绝它。修复前需要明确零尺度值作为**数据传输**时的处理和用它做几何变换时的错误路径，不可简单宣称当前校验符合原生。托管 Number 是 double，原生叶是 float；当前数组和 Group 累积是独立实现，不能宣称原生 ABI、数组元数据、float 精度或变换公式等价。

## 核对结论

- **已确认**：FIXTURE 十叶的源名称、树层级、float 类型、所属基准/运行 pose；位置修正模块输出可见，图像修正模块同形输出不可见；托管输出入口是数组 API。
- **当前独立实现**：`FixtureValue`/`FixtureArray`/Group 扁平累积和仿射变换；十叶尚未进入当前投影层。
- **待验证**：输出端实际数组基数、空位/null 传播、零/负尺度、角度单位和方向、精度/舍入、原生执行器的组合叶订阅及跨轮报告行为、位置修正算法差分。
