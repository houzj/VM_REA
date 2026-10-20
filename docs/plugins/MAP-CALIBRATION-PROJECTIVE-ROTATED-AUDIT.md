# 映射标定 projective / rotated compatibility 分支审计

审计时间：2026-10-19。只检查映射标定图像识别的原厂特殊重检与当前共享 projective / rotated fallback；未修改产品代码、公共算法或全量报告，未执行设备测试和全量回归。

## 原厂证据与调用条件

原厂运行二进制身份见 `docs/plugins/map-calibration-evidence.json`：`MVD_Algorithm.dll`，52686848 bytes，SHA-256 `85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549`。映射模块身份为 `IMVSMapCalibModu.dll`，250880 bytes，SHA-256 `658d63e2c4e2378e7eb1776d27b966990c0d0e01e29ca57ed82fe9acc740fde8`。原始文件位于本机 VisionMaster 4.4.0 安装目录；本文引用 Ghidra 对算法二进制的反编译结果 `analysis/map-calibration/ghidra/core/1807e4360.c`、`1807e3f50.c`、`1807d3e90.c`。

反编译符号给出的可确认顺序是 `7d3ae0` 输入/参数包装 → `7d3e90` 图像拟合调用 → `7e3f50` 图像候选处理配置及尺寸重试 → `7e4360` 候选几何与失败后的重检。`7e4360` 的可见条件：

1. 候选个数 `*param_8` 必须大于 3；否则函数不进入拟合/重检。
2. 先把候选坐标转换为 float，调用 `7ff3c0` 做初次几何拟合；只有该调用返回 1 且输出标志也为 1，才计算角度/几何差异。
3. 计算出的归一化差异达到全局阈值 `_DAT_181628c24` 后，才按 `param_11[2]` 进入特殊检测器分派。分派值 1–9 分别调用 `7dce20`、`7dd6c0`、`7dda50`、`7dde30`、`806ed0`、`7de3b0`、`7df4f0`、`7dec40`、`7e0480`；未知值走 `7dce20`。其中 case 5 的 `7e4360` 调用 `806ed0` 时传入常量 `0x700000007`。
4. 重检输出只有在两维均满足 `2 <= count <= 300` 时才复制回输出点集及尺寸（反编译表达式为 `(count - 2U) < 299`）。

`7e3f50` 另有尺寸参数调整路径，条件为参数结构偏移 `+0x44` 的整数值等于 1；其输出维度候选受 `[2,300]` 检查，并在构造后以 `[3,300]` 的范围搜索设置。这是图像处理参数重试条件，现有反编译没有把它证明为“旋转”或“透视”开关。`param_11[2]` 的符号内含义可确定为 `7e4360` 的特殊检测分派键；还没有源码/调用数据把其 1–9 的每个值可靠映射到公开 `CalibBoardType` 数字或某个 projective/rotated 名称。

SDK managed 参数定义 `analysis/full-audit/calibration-dependencies/decompiled/IMVSMapCalibModuCs/IMVSMapCalibModuCs/MapCalibParam.cs` 公开板型 6、7、8、9 和自由度 `HomoFreedom`：1 Perspective、2 Affine、3 Similarity。这个自由度控制的是映射拟合模型。当前没有证据表明它启用上述 `7e4360` 的特殊图像检测分派，不能把两者混为一谈。

## 当前共享实现及差异

Map 产品入口 `src/VisionPlugins.MapCalibration/MapCalibrationImageSolver.cs` 的 `DetectImage` 先运行 `NImageSourceLatticeAdapter.Find`，如果 lattice 存在则用 `RecognizeLattice` 尝试解码；任一阶段失败就调用 `NImageBoardRecognition.Find`。共享识别顺序为 upright → projective → rotated（`src/VisionPlugins.Calibration/NImageDetectedBoard.cs`）。因此兼容分支的实际启用条件是 source lattice 未能产生可解码结果，而不是原厂已确认的“候选数 > 3、7ff3c0 初拟合通过且残差超过阈值”。

共享 projective 分支另有独立边界：`NImageProjectiveLattice.Find` 要求图像宽高至少 64、估计 pitch 至少 8、原始候选至少 4、连通分量至少 16；recognized board 需有至少一个可解码 opening/tag。共享 rotated 分支估算梯度角，再按 `0,+0.25,-0.25,+0.5,-0.5` 度偏移尝试旋转图像、双线性重采样并运行 orthogonal detector；角度绝对值小于 `0.001` 弧度时直接走 orthogonal 路径。这些是当前独立实现的实际条件，不是从原厂推导出的边界。

故当前共享分支没有逐条件复现原厂特殊重检：原厂重检以前置几何拟合的输出状态与残差判据开启，当前共享 fallback 以前置 lattice/tag 流程失败开启；原厂按内部 selector 分发 1–9 检测器，当前实现先透视 lattice 搜索、再旋转归一化；两边也没有可证明一一对应的参数/模式映射。它目前只是保留的 independent compatibility route。

## 既有运行证据与缺口

找到一条直接涉及该符号的既有 SDK 阶段报告：`analysis/map-calibration/image-detector-stage-trace/comparison.json` 明确写出 `refinement7e4360Called: false`，范围说明是“此 SDK fixture/options；native 80e690/80bcd0 已返回最终点”。其 `stage-trace.jsonl` 有4个返回记录：board 7 两组、board 9 两组，每组168点；`comparison.json` 记录2016个对照字段2016相同。它证明这些特定 fixture/options 的最终返回没有调用 `7e4360` refinement 分支。它不能证明分支永远不触发，且不能用于推导旋转/透视兼容行为。

这条阶段日志只记录 `phase/stage/sequence/board/count/points`，没有候选预拟合结果、拟合旗标、残差、`_DAT_181628c24` 的运行值、selector `param_11[2]` 或 detector 分派目标。因此它未建立 selector→公开板型映射，也未命中任何 `7e4360` 特殊 detector。原厂资源 `analysis/map-calibration/native/default-config.xml` 把 `HomoFreedom` 的公开值定义为1 Perspective、2 Affine、3 Similarity；parser 导出的 `analysis/map-calibration/parser/model/IMVSMapCalibModuAlgorithmTab.xml` 把公开 `CalibBoardType` 定义为7、6、9、8。两份资源均未定义 `param_11[2]`，也没有把它映射到 `HomoFreedom`；只能确认拟合自由度和板类型是两个公开参数，不能从同值或名称猜内部 selector。

其余 `docs/plugins/MAP-CALIBRATION-DELIVERY.md`、`docs/plugins/MAP-CALIBRATION-IMAGE-COMPARISON.md` 的 SDK 回放覆盖板型6–9、有限原图/裁图、默认和 filter2 等已记录调用，source-grid 接入回放用于确认这些覆盖样本的点/映射输出。它们没有补出 detector selector 或成对的独立透视/旋转输入。因此现有记录没有建立 selector→板型映射，也没有证据表明 projective/rotated 原厂重检已命中；无法比较该分支与共享实现的输出点数边界。

**未闭合项（未知而非假定）：** `param_11[2]` 的公开参数映射、`7e4360` 的阈值运行值和真实命中记录、内部 detector 中哪些处理透视/旋转、原厂可接受的候选/输出上限在 `7e4360` 之外是否还有约束、共享 projective/rotated 对任一原厂样本的像素/点集相位。已确认的唯一 `7e4360` runtime 结论是上述4个SDK阶段调用未命中它。当前没有足够证据修复或宣称等价；不应通过固定合成夹具关闭此项。若后续获得现成 native trace，优先记录 caller 的参数结构、`7ff3c0` 返回/输出旗标、残差与阈值、`param_11[2]`、分派目标及输出 count/尺寸，再以同一原始图像运行共享实现作差分。

定向审计检查：`dotnet run --project tests/MapCalibration.ProjectiveRotatedChecks/MapCalibration.ProjectiveRotatedChecks.csproj -c Release`。此检查只校验反编译控制流、当前独立 fallback 条件和现存四条 native stage 报告的明确范围；它不重放SDK、不生成图像夹具，也不把分支未命中报告当作相位等价证明。
