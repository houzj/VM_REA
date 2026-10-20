# R04-A opening实施候选与真实分支记录

2026-10-19，尚未编译/上架。专属proposal已写四体：NImageSourceOpening、LocalPrelocator、LocalFeature、CircleFit。它们不在已过C# PASS7的B补丁中，A未经C#执行不能借B计完成。

80a7f0固定200×200规范外角→80ac20源7ff3c0配置(1,3,2,max20,delta1e-4)→730e90逆→独立于B的truncate+四项float weighted采样+.5→四100×100象限→75fbc0前定位→70f4e0 30窗/旋转profile/当前正峰→70d2e0 model1/strategy2→80ac20只检查半径[15,21]（不额外gate status/RMS）。四点全成功时/10，否则原default20×20 inner4；再源7ff3c0 canonical→proposal，投影inner交B单item采样。

NPointSolver.FitNativePoints仍是既有独立重建，有已记录float差异；A没有以89faa0 sqrt2归一化或double Homography代替它。此复用是否在当前四点fit产生原像素/精化谓词差异，要靠待执行A入口比较，不能由同名fit视为闭合。

前定位当前CPU7/10/11固定配置[10,50,8,1,10,1]：Sobel signed shorts、向量nearest与scalar tail+.5/returnedmax区别，ceil至少1门，728060半步半径/权重，7299e0 mode1整数票，strict首峰及门5。普通CPU零幅值差异未跨路径补过滤，A明确未支持ordinary branch。

LocalFeature保持float半步起角、中心角/窗口角独立递减；42×6，通用优化中心内11bit、边界nearest/row valid ranges与forward/reverse尾2点；中心外6c2d50 float权重按左右夹取、forward半段/reverse半段逐double坐标顺序。轴角bucket+首角相位真达轴核则明确unsupported，不能悄悄通用替代。此实现草稿未运行，圆峰数量/坐标亦需逐层审。

CircleFit固定model1/strategy2/max20/trim0/cutoff5：double矩、float模型/残差、median*2.96516+floatEPS、严格残差<5、权1或scale/(r+FLT_MIN)，失败回初始模型M0且保留最后mask，最终RMS用选中模型重算；不会以独立典型RANSAC代替。PE常量在circle-pe-constants.json。

## 唯一原厂opening定点

复用现有真实SDK source图 `analysis/map-calibration/image-input/native-source.gray` 与image-anchor-caller-trace sequence1原opening：[222.496078,227.96312,339.595917,227.95932,339.596008,345.15213,222.494217,345.155426]，未新增图像或方向样本。

同hashcore直接80a7f0与80ac20十参，原trace在r04-opening-oracle/probe.jsonl：CPU11；80a7f0 flag1/count1，12byte=[3,3,0,10,0,0,3,120,30,63,136,66]；**80ac20 refinementStatus=0、inner全0**。所以本实图实际走原default opening，不能将payload成功称作refined-opening成功。原200×200warp及四crop均已dump，native进程退出0。

同四crop单独6eea70原输出(84,45),(56,75),(66,47),(34,60)。replay_prelocator.py独立Python逐操作复演四位置全匹配；recorded peaks=48/44/49/41，后两项数量/峰值为独立复演，原此次仅动态观测位置/status，不冒充原变量trace。C#LocalPrelocator仍未跑。

R04OpeningChecks.csproj/OpeningProgram.cs已准备，从真实proposal一路运行候选A+B，记录normalized与原warp逐像素差异、独立原crop prelocator以及自身normcrop prelocator、refined/unsupported、最终12byte。验收必须明确原80ac20为0是否同分支；只12byte匹配不等于A精化/通用图像域闭合。不能失败时强行default来通过这个输入。

## ROI顺序接入

80e690原顺序crop→873de0→mask local float+.5取样并稳定压紧原ID→XY float加bbox→保留原grid尺寸、按剩余ID重算min→dense→fit→原图decode。根线已应用shared context投影补丁并过边界8组；Map专属ROI顺序补丁已按根授权应用，针对三回归运行中。删除旧ROI识别后mask的兼容fallback可能退化，必须独列原厂parity失败与兼容退化，不能静默回旧路径。人工验收独立，A+B+SourceGrid真实链自审仍未闭合。

补充2026-10-19：原6eea70的scoreMap基址由72b170→6ee930确认等于caller scratch（后续辅助区在align128(4WH)后），故同一4crop probe保存native-score-k.i32，未增加新输入或执行其他评分函数。独立Python复演对每幅100×100全部10000个int分数**四幅均0差**，不再只以峰坐标相同推中间数值。C#候选仍待编译，其原crop scoreMap对照已加入OpeningProgram。

已新增NImageSourceBoardRecognition组合候选：SourceGrid.OrderedPoints原ID→XY固定fit→project→813ca0桥→A→B→实际direction/anchor/813020点序；ActualInner保留A原输入四点供tag描述。不再依赖Nearest/固定方向1；source失败/明确unsupported不会静默进入legacy decoder。host必须显式提供经核对的MeanDispatch，未把Avx2命名当原CPUID分类器。此组合尚未编译或上架。SourceLatticeAdapter的独立geometry先验门仍须在最终接入中去除，允许仅SourceGrid进入源分支；不能在A前要求现独立CodeQuadrilaterals先成功。
