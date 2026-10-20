# 标定板圆点二次细化源码追踪

## 已证实的路径

圆点模式从 SDK 的板型分派进入 `MVD_Algorithm.dll` 内部。按当前原厂副本和 Ghidra 12.1.4 反编译结果，公开圆点类型映射为 core kind 3；`FUN_1807e33c0` 的 kind-3 分支在 callsite `0x1807e3cf9` 调 `FUN_1807dda50`。后者在 `0x1807ddcb6` 调 `FUN_180817c50`；该函数在 `0x180818c1a` 对每个检测候选调用 `FUN_1808609a0`。这些均为 RVA：`0x7e3cf9`、`0x7ddcb6`、`0x818c1a`（模块 ImageBase `0x180000000`）。二进制无可用源码符号；`FUN_...` 是 Ghidra 地址命名，不把推断名称说成原厂符号。

`FUN_1807dda50` 的原生参数 `param_5[0..3]` 在函数体中按 `x,y,width,height` 使用。它以 `param_1 + y*param_2 + x` 定位原图 ROI，再按配置中的缩放因子逐行/列采样到运行时灰度缓冲；宽高分别是 ROI 宽高除以因子的整数商。参数 `param_10+0x38 == 2` 时，源码逐像素反相此缩放图。当前已记录的默认因子为 5。随后它调用 `FUN_180817c50`，并把本地候选坐标加上 ROI 的 `param_5[0]`、`param_5[1]`，写入返回点数组。

`FUN_180817c50` 先在缩放灰度图上做候选筛选。对候选窗口，它以缩放坐标乘因子换回 ROI 全分辨率坐标，按源代码的 `-6` 起点及 `窗口尺寸*因子+12` 计算局部范围，并裁到当前图边界。`0x180818c1a` 调 `FUN_1808609a0` 时，RCX 指向该全分辨率局部灰度窗口，EDX 是行步长，R8 是高低 32 位分别装宽/高的打包尺寸，R9 是输出指针；其余参数走 Win64 调用栈：入口 `[RSP+0x28]` 为成功标志指针、`[RSP+0x30]` 为三个 float 状态/阈值字段指针、`[RSP+0x38]` 为结果容器指针、`[RSP+0x40]` 为工作缓冲指针。这一布局由 callee 的入口取参和 caller 在 callsite 的寄存器/栈写入互证。

`FUN_1808609a0` 返回值是原生错误码；`*参数5` 表示输出是否可用，成功时 `*参数4` 指向结果容器。其结果仍是内部边缘/轮廓记录，不是一个直接的圆心值。caller 遍历记录并选择边缘记录，调用 `FUN_1807075e0` 对选出的点求圆心；成功路径把局部窗口起点加到拟合中心后写入候选输出。若这一路失败，caller 回到当前局部灰度窗口调用 `FUN_1808620c0` 作为另一条定位路径。随后 `FUN_180817c50` 输出点坐标/网格记录，`FUN_1807dda50` 再加回 ROI 原点。

因此“二次细化”不是将全图直接交给一个独立的 `SubPixelWindowSize` 算法：可见调用链包含缩放图候选、全分辨率候选窗口、边缘记录、圆拟合与失败路径。这里描述的是反编译可见的数据流，不据常见算法为不可读中间结构赋语义。

## SetParam / 参数边界

原厂模块 `SetParam @ 0x180020d40` 的证据表明：`SubpixelWin`、`SubpixelWinAuto` 的别名分支以成功返回，但没有进入 SDK setter；`SubpixelWinSizeUserSetValue` 只做原生名字替换为 `SubPixelWindowSize` 后尝试写入，当前反编译分支也未能证明该值抵达圆点细化函数。资源将相关控件标为隐藏/废弃。后续内层追踪已证明细化的阈值状态来自候选调用结构，并具有强/弱阈值语义；但原厂 SDK 对象将 XML 名称值写入该结构具体偏移的桥接没有源码或运行 trace 证明。因此，不能把它们具体认定为 XML 中的 `EdgeThreshLow/High` 对应槽位，更不能认定为 `SubPixelWindowSize` 或 `Circularity`。详细槽位审计见下节。

## 证据、差分与未闭合边界

原厂公共 SDK 受控合成图运行保留在 `analysis/calibration-board/circle-refinement/runtime.jsonl`：360 组圆/椭圆/方形、对比度、圆度和边缘阈值组合，292 成功、68 返回 `0x10000107`；每个成功行只记录最终首点坐标和点数，不能分离每次 `FUN_1808609a0` 的输入窗口、返回记录、拟合中心及失败分支。另一方面，现有 `calibration-board-candidate-fixtures.json` 来自原厂纯候选阶段，`CandidateChecks` 会将独立初始候选和公共 SDK 最终首点逐项比对；这能验证已记录样本的端到端输出约束，但不能证明内部细化中间结构等价。

本轮执行 `tests/CalibrationBoard.Checks/Run.ps1 -AllCandidates`，5183 项通过，未做设备/桌面测试。`candidate-differences-full.json` 的360个记录均通过、记录坐标最大差为0；该比较只覆盖fixture中采集的候选和 SDK 最终首点/数量。

没有改变产品算法：调用路径和窗口尺寸证据仍不足以完整移植 `FUN_1808609a0` 的 SIMD 边缘记录布局、候选淘汰与圆心输出，也没有逐次运行的内部缓冲捕获。下一步如要闭合此项，需要可复核的受控原厂 trace：在 `0x180818c1a` 记录寄存器、四个栈参数、局部灰度窗口原始字节；在返回地址记录 EAX、成功标志与结果容器；同时在 `FUN_180817c50` 输出点记录处记录 ROI 相对坐标。之后才能构建逐窗口、逐记录、逐坐标的差分测试。现有 360 条 SDK 最终结果不可替代这些内部 trace。

原厂 DLL、资源、反编译文本和 trace 探针 SHA256 清单见 [同目录 evidence JSON](../../analysis/calibration-board/circle-refinement-call-chain-evidence.json)。生成的反编译文本在当前工作树 `analysis/`，完整原始 Ghidra 项目/数据库不纳入版本控制。

## 新增的内部消费与阈值参数流追踪（2026-10-19）

在 `MVD_Algorithm.dll` 再向内追到 `FUN_18085fcd0`（RVA `0x85fcd0`；反编译文件 SHA256 `0123e1fd94d8c82695c4ba31a6de6fa2fb666ae67c4ed469d8af106be736a598`）。`FUN_1808609a0` 先对候选局部灰图调用两支梯度函数，产生两张 signed-short 梯度平面；同时生成 float 梯度幅值平面。随后把三张平面、宽度/行步长、打包宽高、阈值和输出存储传给 `FUN_18085fcd0`。因此细化 helper 不直接读灰度图：灰图影响其输出的已证实路径是“局部灰图 → 两支梯度计算 → 梯度/幅值平面 → fcd0”。现有梯度 helper 的单元差分证据见标定板专属分析资料；本轮未新增对它们的语义命名。

`FUN_18085fcd0` 把传入的 `int` 工作平面初始化为 `-1`；对局部图内部像素，根据 signed-short 梯度选择插值方向，在梯度幅值平面取相邻值插值并执行局部峰值判断，满足阈值后计算浮点亚像素坐标，写入坐标对列表。之后按强阈值/弱阈值把接受点标记为两种类别，并在相邻标记间传播、剔除无效点，最终写回点列表及数量。由此可以确认 `FUN_1808609a0` 后续拿到的边缘/轮廓记录由这些亚像素点与标签组织而来；仍不能仅凭此还原后续所有轮廓选择或圆拟合结果。

参数数据流可追到候选调用者的局部结构槽位，但 XML 名称到槽位仍有一段缺证。`FUN_1807dda50` 将 `param_10+0x24/+0x28/+0x2c` 三个 4-byte 值连续复制到栈块，并把该地址作为 `FUN_180817c50` 的 `param_11`。后者验证 `param_11[0]` 的范围；验证 `[1]`、`[2]` 均在 0..255，且 `[2] >= [1]`。准备 `FUN_1808609a0` 参数时，写入的三项依次是 `(int)param_11[2]`、`(int)param_11[1]`、`param_11[2] == param_11[1]` 的布尔值。最后一项作为 float 位型传递；等值时产生 `0x00000001`，正好触发 `FUN_1808609a0` 的自动阈值分支。

`FUN_18085fcd0` 的分支能区分传入双阈值的作用：第二个阈值用于弱边缘候选门限，第一个阈值用于把候选点标为强边缘。因此在运行参数块内，`param_11[2]` 被用作强/高阈值，`param_11[1]` 用作弱/低阈值，与 XML 中 `EdgeThreshHigh`、`EdgeThreshLow` 的语义相符。插件 `SetParam` 确认会按名字把 `EdgeThreshLow` 和 `EdgeThreshHigh` 转发给 SDK 对象，但当前可读的 setter 代码没有揭示 SDK 对象如何把这两个具名值装入 MVD 算法的 `param_10+0x28/+0x2c`。故“算法运行槽位的强/弱语义”是源码确认；“XML 的 Low/High 具体各落在哪个偏移”仍未知，不能把语义一致当作结构偏移实证。

要闭合该未知，所需新增证据应至少包含以下其一：① SDK 对象针对 setter/getter `EdgeThreshLow/High` 的实现或带字段偏移的符号/结构定义；② 原厂进程中先分别写入一组不相等且可区分的低/高值，再在 `FUN_1807dda50` 入口同步记录 `param_10+0x24/+0x28/+0x2c` 三个原始 4-byte 值，并在 `0x180818c1a` 记录 `float[3]`；③ 对这两个 XML key 做单变量扫描并捕获同一组结构槽位变化的 native trace。需要完整运行差分时，trace 还应同时记录局部灰图（尺寸、stride、原始字节）、两梯度平面、幅值平面、`fcd0` 的强/弱阈值、输出标签平面和坐标对数量/内容。现有 360 组公共 SDK fixture 仅含最终首点/数量，不能代替这些缓冲级字段。本轮没有依据这些 fixture 猜测参数偏移，也没有修改产品算法。


对 SDK 桥接的后续追踪修订见 [CALIBRATION-BOARD-EDGE-THRESHOLD-BRIDGE-TRACE.md](CALIBRATION-BOARD-EDGE-THRESHOLD-BRIDGE-TRACE.md)：已从工厂 vtable 恢复 `CCalibBoardCalibToolImpl` slot `+0x60 -> FUN_180002980`。该目标只从工具对象读取算法 handle 并调用 `MVD_Algorithm_SetValueByString(handle)`，没有读取或转发具名 EdgeThresh key/value；模块调用点也只传 key。故先前“目标尚未导出”的边界已闭合，但 Low/High 到算法阈值槽位的映射仍未知。原厂 DLL 的公共 SDK runtime JSONL 仍无 setter 或 config bytes 记录。
