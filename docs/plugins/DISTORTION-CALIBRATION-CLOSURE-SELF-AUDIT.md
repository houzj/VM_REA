# 畸变支持域与最后自审矩阵

2026-10-19。原厂源码证据、离线差分和产品安全选择分别记录；完整工具数仍0/10。

| 必要路径 | 现有具体覆盖 | 本轮补齐/边界 |
|---|---|---|
| 模型1/2/3及编码板7/9 | PluginChecks原6fixture：19float、168 raw/corrected点、估计/评估误差、逐点误差/status | 本轮在同scope补失败→恢复，不重复扩大原厂执行 |
| 固定核WeightFun1/2/3×三个自由度 | FixedKernelChecks 36真实primary/324float；34零残差边界含奇异IRLS | fixture证据范围，不声称所有非有限/溢出均与native等价 |
| 径向模型/中心 | RadialChecks八个既有原厂seed/14变量样例；FitChecks生产19float对照 | 已记录近零径向/历史workspace差异；不伪造历史prefix |
| 图像输入/ROI | PluginChecks缺失输入、失败图像；RotatedRoiChecks151项原图/矫正mask | 空mask错误是独立安全映射；Mono8/尺寸/单矩形为产品声明域 |
| 自动保存 | FileChecks缺文件、已有marker、refresh开关、空path、目录、错误扩展 | PluginChecks新增实际runtime到合法后缀目录的打开失败→80100010，再恢复 |
| 显式保存/产物生命期 | FileChecks有效76byte、错误扩展、运行前80100003、失败后原payload保留、Dispose释放 | 不把独立文件编解码视为外层.sol格式 |
| 失败显示 | PluginChecks成功→失败、首次有效图失败、保留未发布旧字段 | 新增同node/scope成功→失败→成功，核新状态/原图/点位 |
| 配置重开 | ConfigurationChecks实际provider、原XML模板、参数拒绝、显隐分支 | 新增JSON往返后实际provider重建：模型、UI-only选择、依赖显隐、被拒值 |
| 外层方案/真实宿主 | 共享框架接缝及原平台API链有报告 | 节点参数重开不等于外层.sol编码或完整真实宿主自审；根线整合范围 |

历史prefix策略仍需明确其兼容范围与影响，禁止固定值/清零冒充原厂；安全托管实现不承诺重现未初始化池字节。非有限/任意恶意ABI/平台.sol内部编码不作为无限取证任务；本表仅对明确支持域计实施与自审，人工验收单列。


本轮专属修复：FileLifecycle合法后缀打开失败以前被runtime通用I/O catch归入8010000e，现按SDK Export3b20的打开失败80100010产生typed异常，自动runtime与显式ArtifactSession.Export均消费此码。错误后缀自动8010000e/显式e0000204保留。托管额外检查write/close失败是安全适配；原厂fwrite/fflush/fclose返回值未检查，不能据此宣称所有I/O失败域等价。

本轮验证：修复后PluginChecks退出0，原6/6逐字段一致，自动打开失败与同node恢复通过；FileChecks退出0共89项，包含显式ArtifactSession.Export与自动保存两条合法后缀目录打开失败80100010。配置重开检查结果见validation JSON。

## 工具全部实施与完整原生等价分开审查

| 活动能力 | 实施结论与具体成员 | 剩余类型 |
|---|---|---|
| 原三页配置、中心点来源/显隐/参数范围 | Configuration.cs:18-24实际参数adapter及user-data；31订阅；43公共原模板；16项provider检查通过 | 未发现此片的必需未实施成员；订阅服务缺失时明确拒绝属于宿主接缝 |
| 单次/连续/停止/关闭/重置 | Configuration.cs:26、34-38、54，执行通过context.Run，重置需宿主atomic callback，取消等待由公共execution拥有 | 实际接线已实施；整套宿主调度/重置服务属于共享集成自审，不能声称此专属测试覆盖 |
| 模型与权重 | Solver.cs:15-40，模型1/2/3；Weight1独立OLS、2/3 IRLS与factor*.1，原主调用/34退化边界有既有差分 | 实施存在；**仍有已知支持域数值差异**，见下段，不把它删除或称完全等价 |
| 自动/显式文件 | Plugin.cs:274-282、Configuration.cs:29→Plugin.cs:68→ArtifactSession.Export；FileLifecycle打开错误本轮修复 | 自动/显式89项与runtime通过；.sol编码/任意设备文件系统依赖不扩为本工具源码要求 |
| ROI/当前图 | Configuration.cs:44→公共ROI会话；Plugin.cs:109-151全图/单矩形/旋转mask；151项旋转检查 | 已实施；空mask安全错误、非矩形/多ROI拒绝是明确产品域，原配置只有单Box/FullScreen |
| 状态/结果/恢复 | Plugin.cs:197捕获当前图、253-286成功结果、300-316失败snapshot；原6fixture及成功→失败→恢复通过 | 结果setter中途失败的原非原子发布不由本产品typed字典API复制，接口依赖边界 |
| 八显示对象/必要样式 | Plugin.cs:74-95、Display.cs:11-13，共享逐点Color由根线修复；本工具两个image独立对象、named error font、ROI/point/text均有投影 | 活动配置样式消费已接；named顶层3对象Visible通用store写入与legacy显示域差异此前上报根线，原活动XML并未直接给这3对象独立checkbox，不重复列成缺按钮 |
| 配置持久化/重开 | Configuration.cs:18-24及47-50，参数JSON四项provider重开通过；reset保留user/display四域 | 本工具参数保存接缝实施；外层节点ROI/多图/整方案往返由共享宿主拥有，本片不以.sol内部编码未知阻挡工具实施 |
| 释放 | Configuration.cs:54解绑execution.Changed、roiControls.Dispose、execution.DisposeAsync；Plugin.cs:23-35 scope State释放artifact/输入/输出/diagnostic；fallback Reset:52-61 | scope显式释放已实施，ArtifactSession.Dispose已有文件测试；无scope兼容fallback须调用Reset，不能声称插件GC自动替代它 |

已知数值差异不能略去：既有 fit-verification/production-files.json 中 board2/model2/warp0 是17/19字段同位、最大9.683425383768964E-26，估计均值相同；board2/model3/warp0 是9/19字段同位，最大0.0001678464177530259，估计均值原2.365097E-05/产品4.373962E-05。均产出有限19float成功结果，并非测试失败或功能未接入；它们是明确支持域数值差异。FitChecks:61-64明确保留这两个差异，未隐藏为全通过等价。另固定IRLS历史prefix相关82成功矩阵697/738同值、最大0.0071191043仍是已记录差异。C1已证明池历史越界读，安全托管不复制越界不是缺一个清零步骤；差异必须随工具发布说明，不能靠写死prefix掩盖。

本轮未新发现活动UI无实现的专属成员；修复了确定性保存打开错误码。是否计“工具全部实施”应以根线对共享宿主接缝/样式活动项自审以及上述明确安全数值差异的产品支持说明为准，不能将其自动改写成“完整原生等价”，也不再把原厂桌面像素/人工验收/设备或任意heap复现作为前置。

## 真实主程序自审补充

tests/DistortionCalibration.HostChecks 通过实际MainWindow注册与工具配置入口运行，根线修复通用IMAGE订阅兼容后可挂载1000×1000原fixture。新增检查使用真实lease的private DrawOverlay，在测试内暂时替换Snapshot.Display注入有限fixture，再检查DrawingGroup树：path保留原蓝线、两个marker分别为绿/红Pen；具名text存在字号23且颜色对应的GlyphRun。注入只在tests且finally恢复，不作为原厂动态ABI证据。

真实ROI编辑通过公用Editor SetDrawKind/BeginPointer/EndPointer提交角落Box到节点，触发真实执行失败ModuStatus0；删除ROI触发全图恢复成功。实际ToolDisplayObjectStore将point样式写节点，再由整方案save/load后重跑/重开配置检查恢复。关闭核viewport.Parent=null、ImageOverlayRenderer=null、宿主ROI lease集合空。最终运行统计以center-alias-host-validation.json为准，不用旧12项基本宿主检查冒充新增覆盖。

工具专属活动接缝此前缺POINT alias，现已实际实施并离线验证；根线拥有真实MainWindow注册/公共IMAGE输入修复与共享ROI overlay消费者修复，需在整体源码交接中一并包含。人工验收/原厂像素比对/设备与平台.sol编码不计完成前置；已知支持域数值差异继续按本报告前段保留，不以测试通过改写为完全原生等价。

## 严格实施退出结论（真实宿主21项后）

活动UI接缝、组合中心别名、ROI真实编辑失败恢复、八对象投影、状态与文件、配置参数接缝及整方案重开和关闭释放均已有实施与本轮自审；21项HostChecks退出0，8项PointAliasChecks、原6/6PluginChecks退出0。人工验收、原厂桌面像素、设备、任意heap/恶意ABI不作为退出前置。

但不能把全部数值残余归为安全heap适配：DistortionCalibrationRadialFit.cs Optimize/Solve使用独立Gauss-Jordan直接解正规方程。原180854e90.c:861调用180731610处理正规矩阵，880再180730d20矩阵乘构造delta；731610具体方法体/ABI与必要callee尚未在本工具证据内完成对应，因此“原求逆/乘法的具体运算顺序→当前直接求解”是有限的数值源码/实施残余，不是要求无限重建heap。阶段active、Jacobian、Nielsen阻尼与停止/200循环已实现，不把这一缺口扩大为整个优化器缺失。

独立RadialChecks47/48最终float同位、另一近零k2差2.49e-16，最大映射差2.812e-9像素；board2/model2/warp0的17/19字段近零差异尚未证明来自历史prefix，须与上述线性代数顺序一起有限核查。board2/model3/warp0及5个IRLS调用则已有历史workspace证据，安全不复制越界读的差异继续单列。两类不能合并隐去，也不能凭误差很小自动宣称源码顺序已实施。

严格阻止无条件“源码逆向+实施全部完成”的当前具体项为这条14变量LM线性代数顺序；未发现新的活动UI未实施路径。根线拥有完整工具计数决定，本线不提前增加工具数。

源顺序残余继续推进：已定向取得double731610/730d20及行列交换、854e90/853820调用与返回尾；新专属double helper实现原全主元inverse→multiply，完整14维inactive导数0、pair累加与原平方和。原null break亦改为阶段失败typed1010300b。先前列为严格阻塞的具体原逆/乘顺序现已实施，待短失败传播/12点集自审；不得把仍47/48的近零k2位差说成已消失或全部heap。

## 最终实施退出矩阵（最终四项全部通过）

2026-10-19。此节覆盖上面的临时待验证/逆矩阵顺序阻塞结论。完整工具数由根线统一审计，本线不自行改全局计数。

| 原必需残余 | 实施与证据 | 退出 |
|---|---|---|
| 14D直接求解/压缩活动维度 | 854e90→731610/730d20；RadialFit.Optimize固定14维inactive导数0；RadialLinearAlgebra.Solve全主元double逆后乘 | 已关闭 |
| 2D解析determinant求解/关联 | 同854e90 param9=1/stride2；InverseRadial使用同double helper、正残差delta相加；实际kx*k2/k2*ky与原Jacobian分项 | 已关闭；200点400/400float同位 |
| 初始mu最大值 | 757490全normal raw max，初值-DBL_MAX原字节；两个分支不以PSD推断对角 | 已关闭 |
| rho/归约 | 82a5b0 pairwise左累加→SQRT；两个分支保norm后平方，stageRMS独立 | 已关闭 |
| 14D inverse失败break | 855654 return0→853d7f return0→外层1010300b；typed失败使Detailed无File | 已关闭；受控失败传播通过 |
| 2D inverse失败输出 | 857a60失败取初始observed，非最后迭代点、非外层1010300b | 已关闭；受控fallback通过 |
| 中心alias、旋转ROI、文件打开码、状态恢复 | 原POINT同两叶；公共alias；原masked识别；typed80100010；失败snapshot/恢复 | 均已实施并专项通过 |
| 活动三页UI/八对象/样式/持久化/释放 | 前表具体成员+实际MainWindow；ROI编辑失败恢复/DrawOverlay Pen与GlyphRun/整方案重开/lease释放 | Host21、provider16、alias8通过 |

最终四项退出0：Radial48项（47/48最终float，maxmap2.8167050913907588E-9）；Pixel12×192000像素全同、200逆点400/400float同；12原点集文件全部成功（10组19/19）；Plugin6/6完整19float/168raw与corrected点/两条EvaluationError/rounded error-status、中心compat和失败恢复通过。未新增原厂执行/设备/人工验收，未硬编码prefix或清零伪等价，也未扩大既有阈值。

**当前未发现已确认却未实施的范围内活动成员/算法步骤；源码逆向、实施、自审可提交根线按完整工具标准审查。不能称完整原生bit parity。** 14D/2D/max/norm/失败门均已落实；不能仅凭bit不同反推缺函数，也不能删去运行差分。

已知差分明确分类：

- 历史越界读安全适配：5次IRLS/41字段及board2/model3/warp0（9/19、max0.0001678464177530259，mean原2.365097E-05/产品4.373962E-05）沿C1/C2历史pool-prefix证据；安全容器不重放未初始化越界读，不是填固定值，也不扩大归因到全部差异。
- 独立浮点运行差分：Radial一个近零k2仍47/48；board2/model2/warp0仍17/19，max从9.683425383768964E-26降至1.9588993998448165E-26，均值同。未证明来自heap，不能归为heap；当前未据此发现缺失活动分支/步骤。
- 原生额外API/依赖/安全边界：任意n/非有限/恶意ABI、完整pool压力/全部OS生命周期、平台.sol字节、当前空AdditionPath的可选供应商、托管额外检查write/close、裸字符串SetParam无效目录/编码错误与typed参数接口、全外mask安全错误、裸字典POINT+单叶混源兼容。原活动UI路径预检查会拒绝写入；不能把直接Context注入无效缓存当作原SetParam已接受。上述不冒充原生全部API/错误码等价，也不再无限阻塞已实施工具。
- 原活动UI外named顶层三对象通用Visible API差异此前已交共享所有者；未伪装为原UI缺按钮。共享MainWindow/IMAGE兼容/ROI overlay由根线持有并应一并交接。
- 人工验收、原厂桌面像素与设备不计进度，不作为此退出前置；实际宿主21项是受控自审。

### 最终退出补正：中心数组活动域

根线发现原POINT/float树没有数量限制，scalar-only描述符曾遗漏真实可达数组域；已由CENTER-ARRAY-READ原谓词、1e2b0/20550与Frame空payload错误证据闭合，不能将其排为额外API。现两轴NumberArray/AcceptInteger/deferred opt-in，复用共同读源与All/selector；scope reader优先，X→Y/index0/Count丢弃/031b missing与0314空payload分别实现。中心设置顺序前移到识别之前。新54项实际runtime/Graph与14项实际树/alias通过，6/6原fixture通过；新MainWindow宿主与源SHA以新增final-validation为准。旧numeric四项与旧validation保留为历史，不将其source SHA伪称新中心版本。


## 2026-10-19 中心数组修复后的最终实施自审

最终串行验证已全部退出0：CenterInputChecks54、PointAliasChecks14、PluginChecks原6/6、实际MainWindow HostChecks21/21。真实Tee日志为analysis/distortion-calibration/c4-source/center-{input-checks,point-alias,plugin,host}-run.log；Host包含ROI编辑→失败→恢复、DrawOverlay实际Pen/GlyphRun、样式整方案重载及关闭释放。Host构建仅已有MainWindow83 CS8603警告。最终源码/证据哈希见analysis/distortion-calibration/radial-source-order/final-validation.json。

此前中心数组遗漏已实施并验证；14D/2D原逆矩阵再乘、残差关联、全矩阵max、sqrt范数及失败传播均已按具体原体收敛。按当前工具活动UI、模型/权重、文件、ROI、状态、显示、持久化、释放范围自审，未发现已确认源码仍未实施的活动路径，可交根线作整工具完成审计。人工验收独立，不作前置。

此结论不是完整原生bit parity：47/48近零系数差异及12文件中的17/19近零差异仍保留，未证明来自heap；历史未初始化越界读取采用明确内存安全适配，不能重现所有历史prefix。19float另一9/19历史相关差异仍公开。平台.sol编码、通用WPF内部和额外native ABI属于已列依赖边界；不以此抹去本工具真实支持域数值差分。旧validation与修前数值基线保留，数值源未再改，未重复大Pixel/12File。


## 最终审计补正：共享识别仍是整工具必需依赖

上一段“可交整工具完成”只核C专属成员，不能覆盖完整调用链，现撤销整工具完成候选。DistortionCalibrationRecognition.cs的Recognize/RecognizeCropped/RecognizeMasked全部调用CalibrationBoardSolver；产品接受板型1/2/6/7/8/9，分别棋盘、圆点及四编码板。运行raw识别和校正图再次识别都走该依赖。它不是额外native API边界。

已确认真实实施缺口：共享编码识别R04 proposal/anchor/方向候选尚待D/root整合，当前SourceGrid运输和6组非编码fixture通过不能代替。棋盘/圆点候选精修仍有A线source映射残余，应以A线最新逐成员审计为闭合条件；不能用本工具普通图像成功把它归为纯数值差分，也不把旧历史文档的所有未移植条目当作当前事实。共享成员由根线/A/D拥有，C不另建第二套识别或越权改共享。

C专属已确认遗漏清单目前为空，中心数组及数值顺序修复自审通过；整工具“范围内源码逆向+全部实施”仍受共享识别必需路径阻塞，暂不能计完整。历史prefix内存安全适配和近零bit差分是另一维度，不取代这个真实缺口。


最终活动域补正：见DISTORTION-CALIBRATION-REENTRY-ACTIVE-DOMAIN.md。7c9320真实构造表令engine188=2，本模块SubpixelWinAuto写入成功忽略；mode1外层重入候选仅额外SDK范围，不再作为当前工具正常活动域必需阻塞。活动编码R04仍单列真实依赖。


2026-10-19 R04真实生产整合最终补正：见DISTORTION-CALIBRATION-R04-FINAL-EXIT.md。当前Plugin原6/6、RotatedROI151、实际Host21/21均退出0，已解除此前明确共享R04阻塞；当前未发现已确认源码尚未实施的必需活动路径，交根线整工具计数审计。原始近零/历史prefix安全适配差分、额外SDK mode1与未新增Refined=true原厂运行边界保留。最终新manifest为analysis/distortion-calibration/r04-production-final/validation.json；旧pending结论仅历史。


2026-10-19 编码6/8整工具重开：根线跨工具原fixture发现80f9a0/80cfc0被错套7/9 hole-search；正常活动6/8受影响，原完整计数撤回待修复。另C axis ROI需接source+crop解码运输。见DISTORTION-CALIBRATION-ENCODED68-REOPEN.md及encoded68-reopen候选/12个独立原oracle测试；此前7/9原6/151/21证据保留，不作6/8通过依据。等待根线/D共享集成后复验，不以产品互算当golden。
