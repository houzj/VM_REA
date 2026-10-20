# Map ROI 解码前投影修复与针对回归

2026-10-19，按根线授权应用专属 `MapCalibrationImageSolver.cs`。原路径crop先decode、再filter world点/offset，现为crop原像素→SourceLatticeAdapter→local float+.5取mask稳定压紧→XY float加bbox origin→SourceGrid重建（旧W/H、filtered minima）→原图RecognizeLattice。两图ROI独立走同序，Box/solver与新帧angle修复未改。

源依据80e690/80bcd0：873de0返回后mask过滤复制XY/ID，保留返回grid尺寸，然后才求ID minima/dense；proposal fit/decode使用原图。根线已审并应用shared RebuildForProjectedPoints及Board两处context运输，本切片仅Map专属代码。

运行与独立产物 `analysis/map-calibration/r04-map-roi-verification/results.json`：三项目均build0、run0，0警告0错误。

| 针对入口 | 结果 | 实际证明 |
|---|---|---|
| MapCalibration.GridRoiChecks | 84/84精确 | 四板型7/9/6/8各21既有crop格网；点数、原ID顺序、XY floatbits |
| MapCalibration.RoiRuntimeChecks | PASS7361 | 原厂ROI case0..8，真实插件双ROI；原码/点数，成功case完整source/target/projection点序/ID/floatbits、矩阵/scalar/error |
| MapCalibration.SourceRecognitionChecks | 4/4精确 | 7/9型原两图各168点完整顺序/ID/XY floatbits |

保留原预期，未用容差放宽失败；实际测试进程完整退出0，managed槽已报根线释放。运行器随后改为必选单项目，后续不会自动进入另一个构建抢槽；上述记录来自修改前一次三项目串行运行。

兼容边界：ROI移除了旧crop失败后旋转/projective独立fallback，fullimage现有兼容分支保留。上述原厂fixture无回归；没有额外旋转/透视ROI兼容实验，不能把“有限原厂测试通过”写成“旧兼容域无退化”。不静默回旧先decode再mask顺序；最终A/B/SourceGrid接入将去除独立geometry先验门。当前A草稿仍未编译，不据本修复计整工具完成。人工验收独立记录，不阻挡已完成实现/自动验证的工具统计。
