# 畸变共享识别板1/2有限源码映射

2026-10-19，仅只读源码审查；不构建、不运行原厂、不改共享产品。原core SHA85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549。原常量已从匹配SHA的PE按节映射被动读取，见constants.json；不是运行推测。当前源码与原体hash见manifest.json。

## 当前实际已实现，不再列整条缺失

| 原链/入口 | 当前生产成员 | 审查结论 |
| --- | --- | --- |
| 初始kind1→873de0→8b47c0/8bb3a0/8bcff0 | CalibrationBoardSolver.RecognizeLocal板1→NImageSourceGridPyramid.Find→CandidateDetector/Descriptor/GridSelection | 多尺度与候选二假设、quadratic8b6f30精修已有；不能说板1没有任何亚像素精修。Pyramid返回后没有7e33c0重入。 |
| 初始kind3→812930/877f90/877230 | Solver.Circles→CircleCandidates.Find→CircleGrid.Match | 初始factor5、面积/圆度、局部crop、边缘与拟圆、fallback、拓扑/窗口/朝向/排序均有实际成员。 |
| 局部8609a0梯度/85fcd0峰值→链接→轮廓 | CircleEdges.Contours→NonMaximumState/Gradients/LinkPixels | 已实施相应阶段，旧CIRCLE-REFINEMENT-TRACE的“没有改产品”仅历史，不能作当前整条未移植证据。当前初始877b20实际调用8b3d90，818c1a实际调用8609a0；两者不能只因均产contour就宣称全分支等价。 |
| 7075e0及8620c0回退 | CircleEdges.FitCircleContour / CircleCandidates.TryFrameFallback+BinaryHull+EnclosingCircle | 有具体实现；原877230奇异拟合保留旧局部中心也有previousFit。 |

## 已确认真实缺失：7e33c0外层尺度适应重入

畸变core7f69e0:178调用7e4ff0；后者:219调用7e33c0，故不是另一个工具才用的额外入口。7e4ff0先815560初识别，自动选项分支在末尾调此后处理。原7e33c0:132起要求count>3，复制整数grid为float，:152调用7ff3c0，返回1且successFlag1才计算新尺度。:156-176给具体算式，常量分别eps=1.1920928955078125e-7、0.1f、round .5 double、相对阈值.2f；relative=abs(float(new-old))/(float(old)+eps)。新word6不在此clamp。options[7]强制2；options[6]写估计值。这里只称word6估计值，不能把它等同所有callee的降采样factor。

:218 kind1→7dce20，:244 kind3→7dda50。末尾仅callee返回0且输出宽/高分别2..300时采纳，复制新image/index数组、count、flag、dims并写回原options[6]。不存在“分数更优/更多点才采纳”检查。失败/尺寸无效保留初次结果。只有一次可选重入。精确source-slot形式的候选位于ReentryGate.candidate.cs；matrix的decompiler stack-slot尚未强制命名为m0..8，避免错位实施。

当前Solver板1在Pyramid.Find后直接输出；板2Find一次→Match后直接输出；src检索无7e33c0/7dce20/7dda50对应外层gate。故这一项是已确认活动必需步骤未实施，不是单纯float精度差分。

## 参数能否直接复用当前Find

圆点：7dda50的param10是undefined8*，从param10+6实际偏移+0x30（word12）读整数抽样factor，不是word6。逐点抽样source[x*factor,y*factor]，reduced宽/高整数除factor；不做双线性resize。DotMode读param10+7即+0x38（word14），不是word7。+0x24圆度、+0x28/+0x2c边缘阈值转交817c50；817c50检查原图及reduced两轴>=64。畸变7f69e0传base uStack_a8；+0x44对应uStack68高DWORD=engine+0x188，==1进入自动分支时，uStack78低DWORD即word12明确置2。因此重入使用factor2；不能将word6新估计值传作factor。当前CircleCandidates.Find初始默认factor5且使用Preprocess.Resize的半像素triangle反走样float卷积，与7dda50 factor2整数stride抽样不同，需要独立重入抽样入口，不能改坏初始877f90的resize。Find最大候选固定2500；817c50还按窗口积2倍和min(2500,窗口积)限制。CircleGrid.SelectWindow有maximumWidth/Height，但Match/CanonicalComponent没有向外暴露。共享A应按上述实际字段建立重入backend。

棋盘：7dce20原空mask分支实际进入8084b0，而非873de0。8084b0:134可86f880预处理；:152进入863160候选、:163进入867010组网。863160从param8+4/+8/+c读取尺度/阈值/容量，并调用863c20/828960；867010进入867f70/868550/869680/86b020/86c9d0/86d420/86dbf0。当前Pyramid.Find只暴露最大scale指数1..5、max窗口、角度/filter，不能把options[6]新整数scale直接塞maximumScaleExponent。此处应由A提供7dce20/8084b0参数及863160/867010的实际对应实现，不能套现有873de0当原重入backend。

## 有界后续与未知

1. root/A先核7e33c0矩阵栈槽及7ff3c0真实ABI（已有asm可用），将candidate gate接公共识别后处理。
2. 圆点新增与7dda50匹配的整数stride抽样入口和窗口最大候选适配；定向核8609a0→85fcd0与初始8b3d90内部NMS分支差异，避免把当前Contour全当同一算法。不另建C私有识别。
3. 棋盘863160/867010参数表达与生产映射为精确callee断点，原体已有，不需重跑原厂。A核具体枝后补必需backend。
4. 两板构造触发relative>=.2的受控离线回归，分别覆盖fit门失败/重入返回失败/无效dims保留，以及成功整组替换。已有6模型fixture只证明其路径的最终输出，不能关闭重入遗漏。

本片不宣称所有初始候选算法分支已等价，也不把尚未逐语句核的当前CircleEdges称为已知缺代码。数值近零差分与历史prefix内存安全适配保持独立；它们不能解释外层gate缺失。整工具暂不完整。

证据约束：7e4ff0:104的options+0x44==1包围该自动选窗与7e33c0调用；本工具7f69e0参数块投影已核到engine+0x188，具体配置名/初值仍需root/A核认，不能只凭函数可调用宣称每轮必达。从7f69e0还可定位此字段来自engine+0x188，模式1时word12固定2；当前默认值需结合构造/配置映射核认，不擅自说所有模式均触发。


最终活动域补正：见DISTORTION-CALIBRATION-REENTRY-ACTIVE-DOMAIN.md。7c9320真实构造表令engine188=2，本模块SubpixelWinAuto写入成功忽略；mode1外层重入候选仅额外SDK范围，不再作为当前工具正常活动域必需阻塞。活动编码R04仍单列真实依赖。
