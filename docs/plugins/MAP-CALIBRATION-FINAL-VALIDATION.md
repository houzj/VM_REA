> 2026-10-19 最新重开：根线发现六个Primitive输入仍为scalar As读取，已撤回Map完整计分。旧通过与manifest保留为有限历史证据；typed六口候选已按原caller落Map专属，等待实际TypedInput/Graph/InputState/Region验证及根审。见MAP-CALIBRATION-TYPED-INPUT-SOURCE.md。

# Map 专属正常活动闭合验证（2026-10-19）

本轮按源码审计发现的三项真实差异落地：Region声明端口接入已批准窄互操作合同；Detect输出改为正常成员初始化两项零值，不从配置矩形造框；普通Reset改为原Algorithm.xml六项顺序默认和实际订阅解绑，复用公共helper。已落的Display资源消费一起验证。没有修改共享产品、引入legacyfallback或更改等待/失败frame策略。

## 冻结身份与实际运行

analysis/map-calibration/map-final-validation/final-manifest.json SHA **79573bd2fdcdf911a4117c2f62e350d53652f4ed1cce8840aee25569fe52ff48**；生产Map DLL SHA **387073790a164aa6082a4ecd69d0e7f5c5edc44c1f04d4709097b5301a8ba4e4**。五项专属源码均匹配落盘冻结身份，四个日志组和依赖DLL身份在manifest。公共typed批完成后，在根线授予的独占managed槽串行运行，脚本最终退出0，槽已释放；运行后无产品修改。

| 实际项目 | 构建/运行 | 结果与意义 |
|---|---|---|
| MapCalibration.DisplayChecks | 0/0，0warning/error | PASS15：生产资源InImage2、三组角点XY/color、图像与图元可见开关、无虚构CalibOriPoint、File模式门 |
| MapCalibration.RegionChecks | 0/0，0warning/error | PASS25：生产DLL Resolve边界；frame0/4实际识别对照配置ROI基线，首半帧冲突不锁存、最终轮Region生效、源矩形/目标全图独立，Detect五数组各两项0，输出Region不伪造配置框，另一侧冲突明确 |
| MapCalibration.ResetChecks | 0/0，0warning/error | PASS27：原六有效默认、五支持ISubInfo模式解绑、取消无更改、无Atomic/Run、非名单模式/Teach/路径/Refresh保留、中途HomoFreedom拒写但后续生效/错误通知、关闭配置保留缓存/显式Reset移除 |
| MapCalibration.RoiRuntimeChecks | 0/0，0warning/error | PASS7361：原9个双ROI实际图像cases，Error/点数/ID原序/XYbits/Hbits/error/scalars仍与原SDK trace一致 |

所有新项目仅ProjectReference生产程序集，没有同名候选Compile遮蔽。Reset测试为STA真实Configuration session；Display测试注入受控结果核资源映射，不能冒充VisionMaster桌面；Region生产图像路径是真实执行。

## 整工具退出矩阵提交根线审计

| 链 | 源码/实施/验证退出 |
|---|---|
| D1当前caller像素→opening→24符号纠错 | 既有源体/asm及R03原oracle、Opening原H/逆H/40000byte/score/payload对照；已接生产，无旧Gaussian/GF fallback；当前CPU按原predicate检测11，不硬编码自动结果 |
| D2格网/选择/冲突 | 原14必需体和源Grid身份/稠密槽/INT_MIN/后写覆盖/首成功顺序已映射；负列线性wrap和unknown missing区间判定已落实，不仿造prefix0 |
| D3真实方向输出 | 原proposal→正反循环→decoder direction/payload/origin→anchor→813020→WorldIndex实际字段链接生产，6/8四cell与7/9缺槽caller区分；既有真实168点和四型axis12/mask4/full4原输出对照，不用直接320例微核取代识别证据 |
| D4配置/两模式/Teach/cache/显示/释放 | 最终源码→实现矩阵见FINAL-SOURCE-IMPLEMENTATION-AUDIT；本轮正常活动Region/Detect/Reset/Display差异均实施且验证；既有File/ImageFiles/state-lifecycle保存刷新/失败缓存证据继续有效，未因本轮变更重跑无关数值 |

本线在上述已恢复的正常活动支持域内未发现尚待实施的必修差异，提交根线按用户“逆向+实施+自审”标准复核。测试通过不等同所有原厂平台/CPU/外部接口等价。明确边界：其它CPU数值backend、原VM无回调历史显示策略、废弃参数执行及未取外部API不扩作已证支持；独立Region优先级/生命周期按已批准合同标识。人工启动验收另列待根线执行，不阻挡用户规定计分。
