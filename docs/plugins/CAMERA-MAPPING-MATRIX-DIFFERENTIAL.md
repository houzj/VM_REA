# MatrixToParam 直接矩阵精度差分（2026-10-19）

本切片只增加原厂无设备运行证据及可重现诊断，没有修改产品算法，也没有把单独公式快照作为真实插件宿主测试。完整输入矩阵、原厂六字段、快照六字段、逐字段绝对误差和 float 位比较见 [机器记录](CAMERA-MAPPING-MATRIX-DIFFERENTIAL.json)。既有 118 组 CameraMap 求解差分的结论仍限定于其原有样本。

## 调用链和证据类型

原厂 `MVDMatrixToParamCpp.dll` 的包装层 `analysis/camera-mapping/ghidra/matrix/180001be0.c` 将参数送入 `MVD_Algorithm_Process(..., 0x6b, ...)`；其内部核心公式仍未在该反编译范围内闭合。SDK 正常创建的对象直接执行 `ImportHomoMatrix -> Run -> GetResult -> GetScale/GetOffsetX/GetOffsetY/GetRotate/GetAspect/GetSkew`。没有执行平台 Init，没有重定向算法调用，没有设备操作。

`MatrixProjectionPrecisionProbe.cpp` 沿用既有独立 SDK 探针，只将浮点文本由 9 位扩至 17 位，避免十进制截断混入 float 位判断。每个矩阵使用独立子进程；输出后仅结束探针自己的进程，避免已知 SDK teardown 挂起，不终止其他任务进程。

`Compare-MatrixProjectionSnapshot.ps1` 从当前产品直接读取完整 `CameraMappingLinearAlgebra` 和 epsilon/角度/scale/skew 语句，用隔离 Add-Type 生成诊断；归一化、六字段投影采用同一表达式。诊断源码保存为 `analysis/camera-mapping/matrix-differential-20261019/projection-snapshot.cs`，运行前后两个产品文件哈希一致。它执行的是源码公式快照，并未实例化 CameraMappingPlugin，不能升级为产品端到端回归。

## 本轮实际结果

95 个矩阵覆盖 13 个旋转角、精确 90°、9 组正负/小尺度、5 组正负齐次缩放、微小可逆矩阵、两个失败边界，以及固定 seed 20261019 的 64 个大坐标透视矩阵。

- 原厂 93 例成功；全零矩阵异常 `0x80100004`，非零奇异矩阵异常 `0x10000019`，与既有观察一致。
- 93 例成功中，37 例六字段全部 float 位相同。其余样本存在浮点差异，不能宣称逐位等价。
- 4 例大坐标透视矩阵的偏移误差超过既有 0.003 绝对阈值。本轮仍保留阈值，不将这些差异隐藏为通过。

| 样本 | 字段 | 原厂 | 产品公式快照 | 绝对差 |
|---|---|---:|---:|---:|
| seeded-projective-15 | OffsetX | 113994.5859375 | 113994.578125 | 0.0078125 |
| seeded-projective-16 | OffsetY | -73598.6640625 | -73598.65625 | 0.0078125 |
| seeded-projective-33 | OffsetX | -599278.75 | -599278.6875 | 0.0625 |
| seeded-projective-61 | OffsetX | 91989.5234375 | 91989.515625 | 0.0078125 |

最大偏移差是该数量级的一个 float ULP。最大 Scale、Rotate、Aspect、Skew 绝对差分别为 `5.960464477539063e-8`、`2.980232238769531e-8`、`9.5367431640625e-7`、`5.960464477539063e-8`。这些数值只描述本样本集，不是全域误差上界。精确 90°继续观察到 `Scale=0`、`Aspect=-8388608`；没有将其改成通常数学定义的结果。

## 交接与未闭合项

本轮确认的是**直接原厂 SDK 输入域的真实精度差异**。尚未证明这些输入会由实际 CameraMap 求解器生成，更未证明真实宿主在相同流程输入下产生该偏差；因此没有据此猜改产品求逆或分解顺序。下一步应先在原厂 CameraMap 输出中重现这些矩阵邻域，再追踪 MatrixToParam 核心求逆/归一化指令顺序，以定位是否为相同精度差异。reset 对点缓存、有效标记及最后输出帧的影响仍未知，本轮没有获得新的 reset 运行证据。

复现：先运行 `powershell -NoProfile -ExecutionPolicy Bypass -File tests/CameraMapping.NativeProbe/Run-MatrixDifferential.ps1`，再用 PowerShell 7 运行 `tests/CameraMapping.NativeProbe/Compare-MatrixProjectionSnapshot.ps1`。原生构建和日志全部位于本工作树专属 analysis 目录；没有使用共享 dotnet bin/obj，没有重新运行此前托管回归。

证据哈希：原厂 DLL `9F761DC58ECC65ED86989B56953A50B9C193D619F88A520B42E07A4FB046F598`；产品 `CameraMappingPlugin.cs` 为 `AE7E4EF6F016192FD76A8B60C74A2912BD5C3FA7F3F454E0D9D54C8F60566FF0`；`CameraMappingLinearAlgebra.cs` 为 `A02B1F61C1A49D9E448B44E77D23E478FE00381CF4F0CE122E7C323BD5F091AD`。其余输入、脚本、原始 trace 和比较文件 SHA256 保存在机器记录中。
