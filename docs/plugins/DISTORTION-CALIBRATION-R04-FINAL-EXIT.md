# 畸变工具R04生产整合后的最终实施退出

2026-10-19。基于当前R04真实生产源码和项目引用完成串行复验；不是候选shadow DLL。PluginChecks原6/6严格逐字段通过，RotatedRoiChecks151通过，实际MainWindow HostChecks21/21通过。三个最终运行退出0，原日志与当前源/程序集SHA见analysis/distortion-calibration/r04-production-final/validation.json。验证前157文件快照与验证后hash一致。

| 必需活动范围 | 源码/实施结论 | 自审证据 |
| --- | --- | --- |
| 原三页配置/Operation/触发与底部命令 | 45控件、16操作已逐项对应provider及共享消费者；隐藏可触发项保留；IsAbandon及mode1额外SDK边界已核。 | FINAL-ACTIVE-UI-AUDIT与expanded inventory；本轮Host3页无未解析template。 |
| 图像/POINT/float数组/首项与读源 | generic IMAGE同数量域；scope reader优先、X→Y顺序/031b和0314严格区别；组合绑定两叶与动态index/All。 | 现有Center54/Alias14；本轮Plugin真实center兼容保持。 |
| 板型识别与ROI | 当前生产R04已由根线整合；C调用真实共享BoardSolver，无私有影子。raw与校正后再次识别、axis/rotatedmask/坐标回投。 | 本轮board7/9×3模型原6组；151旋转ROI enclosing168/subset144；Host实际ROI失败→恢复。 |
| 模型/拟合/逆投影/数值顺序 | 14D/2D原double逆再乘、pairwise、表达关联、全normal max、sqrt norm、失败码/初始观察点fallback均已实施；无新的已确认活跃步骤遗漏。 | 既有Radial48/47位、Pixel12×192000/Inverse200点400float全同、12File；数值源本轮未变不重跑。 |
| 自动/显式19float文件与失败恢复 | 原字段方向/保存门、refresh、后缀与fopen型typed失败实现；managed写关闭异常按显式安全适配。 | 既有File89；本轮Plugin6/6的19float全同。 |
| 状态/失败发布门 | 当前输入图快照、ModuStatus/time、旧字段保留但不伪发布；成功→失败→恢复与首次失败。 | 本轮Plugin失败显示专项及HostROI失败状态0。 |
| 显示对象/样式消费者 | 两图层、逐点Status颜色、ROI、具名误差字号、ShowText及legacy记录/列表均有实际consumer；不凭裸named API替代活动组语义。 | 本轮Host真实DrawingGroup path/OK/NG Pens、GlyphRun字号、样式全方案重开消费。 |
| 配置持久化与释放 | JSON真实主宿主save/load重跑，绑定/图像/ROI/样式恢复；Session dispose停止执行并释放ROI lease/viewport/overlay。 | 本轮Host18..21重开；16..17真实detach/lease为空。 |

严格退出结论：在已界定原模块正常活动配置与产品域，当前未发现“已确认源码尚未实施”的必需活动路径。C专属此前center数组遗漏及数值顺序遗漏均已修复；共享R04生产整合及本轮回归已解除此前明确依赖阻塞。可提交根线作整工具“源码逆向+实施完成+自审通过”计数审核；本线不自行修改全局分数。

不等于完整原生bit parity：近零Radial47/48与file17/19差异保留，未无证归heap；历史未初始化越界prefix IRLS及file9/19差分按明确内存安全适配保留，不清零/硬编码冒充。R04 Refined=true成功域本轮未新增原厂运行，根线/D已有源实现与精确有限测试不外推任意样本bit parity。mode1 outer重入是被构造mode2/模块忽略setter隔离的额外SDK范围，候选保留不阻当前活动域。平台.sol私有编码、所有CPU/恶意native ABI及人工验收不是本工具活动实施前置。

执行记录：Host首轮dotnet run漏传必需新输出目录，session73158编译成功后以ArgumentException退出1，完整日志host.log保留。随后直接执行该已构建DLL、正确目录host-output，session47458退出0且21/21。不是产品修复或隐藏失败；没有重建。Host仅已有MainWindow83 CS8603（tmp/final各一次），无编译错误。没有设备或新增原厂运行，没有修改共享产品、提交或推送；所有本线进程已退出，槽已释放。


2026-10-19 编码6/8整工具重开：根线跨工具原fixture发现80f9a0/80cfc0被错套7/9 hole-search；正常活动6/8受影响，原完整计数撤回待修复。另C axis ROI需接source+crop解码运输。见DISTORTION-CALIBRATION-ENCODED68-REOPEN.md及encoded68-reopen候选/12个独立原oracle测试；此前7/9原6/151/21证据保留，不作6/8通过依据。等待根线/D共享集成后复验，不以产品互算当golden。
