# 畸变整工具闭合清单

2026-10-19。按用户新口径，只按完整工具计数；全局数由根线统一审计，本文件不自行更新全局计数。C1–C4仅为内部追溯卡，不再把阶段数量或函数/测试数量作为总完成率。下列源码、独立实现与验收不能互相替代。

| 范围 | 已有可复用原厂证据 | 整工具尚需的必需闭合项 | 最短下一步/边界 |
|---|---|---|---|
| C1 workspace所有权 | Create/Init/Run/Destroy→公共pool；busy归还、复用、CRT free、清理线程及已有正/负prefix来源 | 产品对历史未初始化prefix差异的明确兼容选择；当前工具入口容量/Init失败与结果失效的必要分支；宿主加载/会话边界确认 | 不扩大枚举其它插件/OS堆所有调用；公共pool的已证来源足够解释差异。不得用清零/固定历史值冒充原厂等价。根线须汇总兼容策略及用户决定 |
| C2排序/权重 | 两函数完整循环/原指令、n与2n、上中位、负下标、12sigma/493字段离线一致 | 支持输入域/非有限/退化处理与产品差异对照；安全确定性核如何公开native未定义差异；相关分支离线回归 | 原生越界不是实现任务。对当前产品实际支持的模型/权重与退化样例做有边界对照，不把所有矩阵库内部实现都当工具闭合前提 |
| C3模型/文件/结果 | schema→缓存→Process应用、模型选点/原点/尺度、19float方向与保存门控、native catch/输出门控 | 全支持模型/权重实际字段与已实现产品逐项核对；输入失败/运行失败/输出失败与旧结果、显式/自动保存、路径错误的产品验证；当前SDK范围与旧public包装区分 | 源体索引已具备大部分主路径。优先列产品与原厂首分叉，再请求串行构建/回归；外层框架调度/错误消费者由根线对接，不能当独立畸变源码实现 |
| C4配置/ROI/显示/持久化 | 多图CheckedImages恢复迁移/记录、普通ROI按钮与关闭、22byte协议与自身native parser/坐标、空AdditionPath、字号桥/保存恢复/呈现 | 其余原Display对象与IsDisplay/颜色/线宽等必要属性和成功失败显示清理；重命名/复制/重置键规则；本产品参数/ROI/多图/显示设置方案往返；真实宿主自审；用户手验单列 | 本轮方案Save/Load边界报告完成托管到平台API和按需重载链。平台.sol字段编码与WPF内部绑定释放列依赖边界；不无限取整个框架。独立项目保存格式须由真实宿主往返验收 |

已有独立产品位置：DistortionCalibrationConfiguration通过ToolConfigurationParameterAdapter、ToolDisplayObjectStore、ToolDisplayFeatureStore及共享ROI会话运行；ConfigurationUserData/DisplaySettings/DisplayInputBindings/DisplayObjectSettings保存在节点参数，reset显式保留它们。该行为是当前产品选择，尚需与原厂工具期望逐项验收，不能因属性存在就算整工具完成。

最短闭合顺序：①以原Display.xml逐对象核结果/样式/持久化必要分支并形成产品差异清单；②只补本工具确定性缺口，公共能力交根线唯一所有者；③对固定原fixture覆盖支持模式、失败/恢复、显式自动保存及方案往返；④根线串行构建、真实宿主运行后启动供用户手验。完成口径以逆向、实施及自审全部完成为准；人工验收单列，不计入进度，也不阻挡已完成工具计数。当前仍有实施/自审残余，所以不增加完整工具数。

源报告索引：C1-WORKSPACE-LIFECYCLE、C2-SORT-BOUNDARIES、C3-CONFIG-DISPATCH/SCHEMA-STATE/MODEL-POINTS/GRID-ORIGIN/FILE-SAVE/CATCH-OUTPUT，以及C4-IDENTITY-LIFECYCLE/ROI系列/ADDITION-SERVICE/DISPLAY-FONT/SOLUTION-DISPLAY。历史报告的阶段百分比仅是旧口径，不继续作为当前总进度。


2026-10-19 实施更新：旋转 Box 原图/矫正图识别已按实际 SDK mask→候选筛选→编码解码→原图回投链复用公共实现；离线151检查通过（168/144点），原6模型及失败显示回归通过。详见 C4-ROTATED-ROI；全外空mask是独立安全映射。此项不再列为实施缺口，整工具其余支持域/失败保存/宿主自审与历史prefix策略仍开放。

最后自审补齐：CLOSURE-SELF-AUDIT记录合法后缀打开失败80100010修复、actual runtime成功→失败→恢复与6/6原fixture、89项自动/显式文件生命期、16项实际配置provider节点JSON重开（render diagnostics=[]）。这不冒充原厂.sol字节或整个宿主流程验证；历史prefix兼容选择仍单列。

最终专属退出见CLOSURE-SELF-AUDIT末节及RADIAL-SOURCE-ORDER。14D/2D/max/norm/失败传播、中心alias与21项真实宿主自审均闭合；当前未发现已确认却未实施的范围内活动步骤，交根线审完整工具。旧表待办是此前追溯，不覆盖此最终退出；近零运行差分与历史prefix安全适配仍公开，不宣称全bit parity。


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

重开后的另一个活动reset缺口已按原15默认列表接公共ToolConfigurationAlgorithmReset，C provider同步宿主effective读回；专项116通过，详见RESET-SOURCE-LIST。6/8 source+crop产品运输和共享分派已由根线集成，但当前最终Encoded68正确AngleDegrees命名版及旧Plugin/ROI/Host复验仍待队列；之前测试错误差异不作产品缺陷结论，也不提前计完整。

最终重开闭合见DISTORTION-CALIBRATION-ENCODED68-RESET-FINAL-EXIT.md：6/8十二原oracle全部ID/序/float位同，两个实际完整运行链成功；Reset116及旧Plugin6/ROI151/Host21全退出0，248源快照无变化。当前未发现已确认源码尚未实施的必需活动成员，提交根线重新审计计数，不自行覆盖全局台账。边界与旧数值差分继续明示，不宣称完整原生等价。
