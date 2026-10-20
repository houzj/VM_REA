# 标定板 EdgeThreshLow/High 桥接追踪

## 2026-10-19 原厂硬件断点补证：撤回旧的实参数量推断

本节覆盖下方历史“模块/SDK只传名称、值未传入或桥接完全未知”的结论。新增 `EdgeSlotsProbe.cpp` 与 `RunEdgeSlots.ps1` 在同一个自有原厂模块及同一个SDK工具实例中，分别通过模块SetParam和直接SDK SetRunParam写入三组Low/High。探针通过自有线程调试寄存器在SDK `RVA 0x2980`、算法导出 `MVD_Algorithm_SetValueByString` 及实际圆点候选 `RVA 0x877230` 观察入口；不改算法机器码或参数缓冲，不初始化平台、不使用设备。模块八个IO导入仍由既有synthetic host适配，故不是原厂平台桌面测试。

源码汇编 `sdk-disassembly.txt:2287` 确认SDK函数仅把RCX改为`[RCX+0x28]`后调用导入函数；**没有给RDX/R8重新赋值不等于没有传参**。本次24条setter入口事件直接证明：两入口的RDX始终指向对应`EdgeThreshLow/High`名字、R8始终指向对应值字符串，且RDX/R8地址逐次相同。模块调用也传递了值，不只是名字。先前根据Ghidra所示单参调用推出“未传value”的解释错误，现撤回；下方保留的是历史调查材料。

| 同实例阶段 | 调用入口 | Low/High写入 | 圆点877230入口实测 |
|---|---|---|---|
| 0 / 3 | 模块 / SDK | 20 / 40 | options+4=20，options+8=40 |
| 1 / 4 | 模块 / SDK | 0 / 40 | options+4=0，options+8=40 |
| 2 / 5 | 模块 / SDK | 20 / 80 | options+4=20，options+8=80 |

877230的第10个实参来自x64栈`RSP+0x50`，三个4-byte槽依次是circularity float bits、Low int、High int。此次Circularity=20，第一槽bits为`1045220556`；六次实际caller返回RVA均`0x8785c8`（877f90包装内）。每次ProcessAlg均code0/status1。该结果闭合这一路圆点参数传递，不把其他板型函数`7dda50`的`+0x24/+0x28/+0x2c`直接等同于此结构，更不推导任意图像的灵敏度或全部默认参数生命周期。

正式运行：`analysis/calibration-board/edge-slots-41513e16a10747518180e96ced68c18c/runtime.jsonl`，SHA256 `b6d964b5d0ffc495fa5f4573f39c701fa40c757a84d6c8a8468cd66993f5bfcb`；脚本断言六次运行成功、24次ABI事件的名称/值/地址原样传递、六对阈值完全一致。源码`EdgeSlotsProbe.cpp` SHA256 `bf8df6a6c08abada1ead9bb2b3280bf6db4489a40e0efcc745d79eaa72f524b7`，脚本SHA256 `ac529aeab2e716eca9e2d2441802d2c55143fd1f5366c2ab939ed4986b23214c`。SDK/模块/核心身份均由运行器校验；核心SHA256 `85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549`，其他身份见summary.json。早期未命中7dda50的尝试没有作为通过证据，也不据未命中判定值未传递。

本轮只新增探针、改正证据/指导，不改产品算法、共享合同或容差；当前独立圆点候选已经消费Low/High。原有棋盘三输出指纹相同仍只说明该图不敏感，不能继续用它支持“setter未传值”。

## 历史静态调查（以上述补证为准）

本报告追踪原厂 XML 中 `EdgeThreshLow` / `EdgeThreshHigh` 从模块 `SetParam` 进入校准板 SDK 的路径。结论只覆盖静态源码和汇编可证实的调用，不据此推断运行时参数槽位。

## 已确认调用链

1. 原厂 `IMVSCalibBoardCalibModu::SetParam @ 0x180020d40` 将 `EdgeThreshLow` 和 `EdgeThreshHigh` 列入识别名单。识别后没有对二者重命名或写入本地阈值字段；此分支把模块成员 `this+0x15d98`（SDK 工具对象）赋给局部变量，并调用该对象 vtable `+0x60`。这一调用只传入 `param_1`（参数名），没有把 `param_2`（字符串值）作为该虚调用实参传递。
2. 原厂 SDK 工厂 `CreateCalibBoardCalibToolInstance @ 0x180001000` 把工具对象首字段设为 vtable 地址 `0x180007b38`。该 vtable 的 `+0x60` 表项是 `0x180002980`（RVA `0x2980`）。表项地址由工厂写入的 vtable 指针和 SDK DLL `.rdata` 指针表交叉确认。
3. `FUN_180002980 @ 0x180002980` 只读取工具对象 `this+0x28` 的算法句柄，并调用 `MVD_Algorithm_SetValueByString(handle)`。该函数体没有读取传入名称；反汇编在调用外部例程前只设置 `RCX` 为句柄，没有把 `RDX` / `R8` 中的名称或值传给该例程。
4. SDK 初始化函数 `FUN_180003b70 @ 0x180003b70` 在工具对象 `+0x28` 建立算法句柄，并把 BasicParam 指针放在 `+0x30`。模块 `SetParam` 早先的 `GetBasicParam` 调用用于固定的板型、坐标模式、物理尺寸等专门字段；`EdgeThreshLow/High` 走后面的通用分支并重新选用工具对象。不能把这里的工具 vtable `+0x60` 与 BasicParam vtable 混为一谈。
5. 原厂 XML 资源只确认这两个字段存在、Low 默认 `20`、High 默认 `40`。核心算法路径 `FUN_1807dda50` 后续读取候选运行结构 `+0x24/+0x28/+0x2c`，细化路径把其中阈值槽位交给阈值分类器；现有源码仍没有一条从这两个 XML 名称和值到这三个偏移的已证实写入链。

## 证据来源与哈希

| 证据 | 地址/内容 | SHA-256 |
|---|---|---|
| 原厂 `IMVSCalibBoardCalibModu.dll` | `SetParam @ 0x180020d40`，Low/High 名称分支和虚调用 | `bd62ee940a2e299c0c47e41909e64ca78c09d7717e7cefc629dbfb9e21105c1d`（原厂 DLL）；反编译 `analysis/calibration-board/native-decompile/output/180020d40.c`: `dcf7ef94f929d6aa28501de637cab5298238a8322ce9b74249ca9993027a3361` |
| 原厂 `MVDCalibBoardCalibCpp.dll` | SDK 工厂 vtable、工具槽位与 BasicParam 初始化 | `0a65d7a9e84a3ec6f662b4f05daa5a88351690ae4fbecd1748427c13c56fa308` |
| SDK 工具 `vtable+0x60` 目标 | `FUN_180002980`，调用算法例程时只传句柄 | `analysis/calibration-board/sdk-decompile/output/180002980.c`: `00cafbfb6be09bdbfc67ba6724bfa6f4353565aef9e02d7feba52cfa17d6bee8` |
| SDK 汇编导出 | 工厂存入 `0x180007b38`；工具表 `+0x60 -> 0x180002980`；目标只把 `this+0x28` 放入 `RCX` 后调用导入函数 | `analysis/calibration-board/sdk-disassembly.txt`: `fbd12527929dc66773475ff1c0c9e56b702ed10ba76e5fa21efba86671788be2` |
| SDK 初始化反编译 | `FUN_180003b70` 创建算法句柄与独立 BasicParam 对象 | `analysis/calibration-board/sdk-decompile/output/180003b70.c`: `48cdfec41f15af2b08482ad0529c9b28d83fd66e86d887ea55a10e96dec2fb4d` |
| 原厂 XML | Low `20`、High `40` | `src/VisionPlugins.CalibrationBoard/Resources/AlgorithmTab.xml`: `76c9cd661a359216041b902cce8fae115566726707e749cc42c0bb9722bcbf91` |
| 定向 Ghidra 导出助手 | 导出 SDK 工厂调用链、`0x180002690` 与 `0x180002980` | `tests/CalibrationBoard.NativeProbe/BoardSdkAudit.java`（本工作树当前文件） |

## 可证结论、未知与修正判断

可证的是：模块识别了两个名称，随后把名字单独传给 SDK 工具 vtable `+0x60`；该槽对应函数只以工具对象中的算法句柄调用 `MVD_Algorithm_SetValueByString`，函数体不消费名称和值。现有证据因此没有显示 Low/High 被映射到算法配置参数，也没有显示被写入 `FUN_1807dda50` 读取的 `+0x24/+0x28/+0x2c`。

未知的是：导入的 `MVD_Algorithm_SetValueByString` 对句柄内部隐藏状态的副作用；这两个 XML 参数在真实 SDK 运行时是否另由未覆盖调用链写入；以及其与算法阈值结构槽位的任何映射。已有公共 SDK 合成样本只保留最终点数量和首点坐标，没有记录 SetParam 同实例前后状态、句柄参数块原始字节或 `FUN_1807dda50` 的输入槽值，因此不能用输出差异反推内部偏移。

目前没有可安全落地的产品修正。将 XML 名称直接映射为 `+0x28/+0x2c` 会超出原厂证据；也不能仅凭当前虚调用分支判定 SDK 最终运行行为。闭合此差异所需的最小新增证据是：同一原厂 SDK 实例上分别只改 Low、只改 High，并同步记录 `SetParam` 返回值、工具 vtable `+0x60` 入参、`MVD_Algorithm_SetValueByString` 的完整 ABI/副作用，以及 `FUN_1807dda50` 入口 `+0x24/+0x28/+0x2c` 原始 4-byte 值。该探针可用合成图像完成，不需要设备。

本轮是静态逆向证据补充，没有设备测试，没有改产品算法，也没有宣称完整原厂参数行为已复现。此前 `CALIBRATION-BOARD-CIRCLE-REFINEMENT-TRACE.md` 将 SDK `vtable+0x60` 目标列为未导出；本报告通过工厂 vtable 指针补出了目标 `0x180002980`，并将未知边界收敛为该目标和外部算法例程的运行时数据副作用。
