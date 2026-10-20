# 映射标定 Opening 搜索与锚定边界复核

状态：原厂搜索停止语义已由反编译源码确认；现实现遍历范围存在可复核差异；多成功候选对最终输出的影响仍未通过运行差分确认。没有据此声称识别闭合。

## 原厂证据

- 原厂 SDK：`D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll`，SHA-256 `85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549`。
- Map 模块：`D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSMapCalibModu/IMVSMapCalibModu.dll`，SHA-256 `658d63e2c4e2378e7eb1776d27b966990c0d0e01e29ca57ed82fe9acc740fde8`。
- Ghidra 只读反编译：`analysis/map-calibration/ghidra/core/18080e690.c`（HIK7）和 `18080bcd0.c`（HIK9）均先准备格点/候选数据，再调用 `180813ca0`；其成功状态路径随后调用 `180813020` 将已选 payload 映射到格点输出。两者都没有对每个成功 tag 重复调用身份映射。
- `180813ca0.c` 从半径/窗口参数生成多个 quadrilateral proposal，对每个 proposal 调 `18080a7f0` 做局部开口/标记候选求解，并检查共享状态 `*(int *)(puVar12 + 0x1e)`。成功时写 payload/rotation/anchor 等输出，状态置为 1；外层随后遇 `if (iVar8 == 1) break;`，停止余下 proposal 搜索。该顺序是函数控制流证据，不是从结果猜测。
- `180813020.c` 根据 orientation 分支遍历已分配格点，只把非哨兵点按 anchor/offset 映射为输出索引与 XY；Map caller 只将成功搜索返回的单 payload 交给该阶段。
- 原厂自有进程回放 `analysis/map-calibration/image-tag-trace/tag-trace.jsonl`：4 次调用覆盖图像0/4 × HIK7/9；每次 `statusBefore=0`，一次 `decode-attempt` 后 `status=1`。这与首成功停止相符，但每个案例都只有一次候选尝试，不能证明多成功候选图像的点集结果。

## 与现实现的具体差异

`src/VisionPlugins.Calibration/NImageDetectedBoard.cs` 的独立 `Recognize` 对 `lattice.CodeQuadrilaterals` 全部执行 fiducial 检测和 payload 解码，把所有有效项加入 `tags`；选 `tags[0]` 作为身份锚点时，格点过滤又用 `tags.Any(tag => Inside(...))` 对全部成功开口屏蔽点。原厂 `813ca0` 则在首成功后停止搜索。因此在存在多个独立有效 opening 的输入中，后续 opening 会在托管实现中影响可用点，而原厂搜索链不会继续消费它们。这个差异可由原厂控制流和现实现源码直接复核。

需要限定结论：当前 4 个真实 SDK case 未触发第二次尝试，既没有证明这类图像一定产生不同输出，也没有证明 proposal 的顺序/坐标与托管 quadrilateral 顺序相同。不得基于这一控制流证据擅改共享 Calibration 识别器或宣称 Map 端到端不等价的具体幅度。

## 定向检查与复现

```powershell
dotnet run --project tests/MapCalibration.AnchorChecks/MapCalibration.AnchorChecks.csproj -c Release
```

检查验证 Ghidra source 的两板型调用顺序、`813ca0` 成功退出分支、4 次原厂 trace 的状态/尝试数，以及当前独立实现全量遍历和多 opening mask 的具体代码形态。机器输出在 `analysis/map-calibration/anchor-audit/source-boundary.json`，包含输入 trace 与导出函数文本哈希；它是源码差异审计检查，不是多候选运行等价测试，也没有覆盖/重写主线的8项回归报告。

Ghidra 定向导出复现：`powershell -NoProfile -ExecutionPolicy Bypass -Command "& './tests/MapCalibration.NativeProbe/RunCore.ps1' -Addresses @('18080a7f0','180813020')"`；`813ca0` 已从既有 core 项目定向导出。`RunCore.ps1` 的工具路径改为从当前 worktree 推导原 checkout，避免脚本内硬编码中文绝对路径在 PowerShell 编码转换后失效；同时把 Ghidra 的 `JAVA_TOOL_OPTIONS` stderr 提示作为日志接收，不再把成功运行当作 terminating error。

## 尚需的证据

要修复此行为差异，需一个包含两个互不重叠且均可成功解码开口的实际 SDK 输入，并同时捕获：proposal 生成顺序、逐 proposal decode status、最终 anchor 和映射后的全点 ID/顺序/XY。现有 `ImageTagTraceProbe` 只记录进入 `813ca0` 后的成功 payload，没有记录每个 proposal 的完整坐标/失败原因，也未产生多成功输入。因此本轮只记录边界，不改变 Map 产品识别或共享 NImage 算法。
