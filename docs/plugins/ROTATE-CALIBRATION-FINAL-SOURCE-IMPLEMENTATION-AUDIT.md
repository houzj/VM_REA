> 2026-10-20 最新独立退出：路径生产专项PathChecks25已实际通过，build/run exit0、0warning/error；FilePath前端拒绝/后端失败commit与同路径一次reload/唯一通知已验证。完整有界退出与本次131源PDB快照见ROTATE-CALIBRATION-INDEPENDENT-EXIT-REVIEW.md。下文“待路径验证/typed未落”均属历史；整工具结论已交root审核，本线不自行计数。

> 当前修订（2026-10-19，路径批待验证）：Typed生产179、Reset16、Lifecycle6、旧1259已通过，原14例typed oracle已核；不再是“候选尚未落盘”。新发现OpenFileForCalibDialog前端suffix/目录门与尝试值/history、后端有效目录失败load与同路径重载接缝已落生产但未完成专项运行，因此整工具仍开放。待执行tests/RotateCalibration.PathChecks；既有manifest不覆盖、不宣称本次新DLL已验证。下文较早“未见剩余必修差异/提交计分”是历史结论，以本段为准。
> 2026-10-19 最新退出条件：根线尚未批准Rotate完整计分。Reset16/Lifecycle6/旧1259有限证据保留，但两Int/三Float typed source读取门必须接入并验证；候选与12例原定点oracle已备，尚未产品落盘。

# 旋转标定整工具退出自审初表（2026-10-19）

Map已由根线审计计完整工具，当前转RotateCalibModu。本轮已按原源码落地Reset修复并完成独占构建/验证；Reset16、Lifecycle6、既有原证据套件1259全部通过，源码冻结，提交根线计分审计；测试不替代源码审查。身份索引analysis/rotate-calibration/final-audit/inventory.json。

| 活动 | 源链及既有证据 | 产品映射 | 本轮判断 |
|---|---|---|---|
| 参数/输入模式 | 原AlgorithmTab InputWay无AccessMode单选，0组合POINT/1XY float；RevOutputFlag Boolean，FilePath OpenFileForCalibDialog | RotatePlugin descriptor/PointInputAliases、公共ConfigurationProvider；UI经源XML解析 | 复用旧UiChecks触发器、订阅和大小写路径校验；不得把InputWay当原算法读取 |
| Reset | 原Button_ResetAlgoParams→ResetRunParamBind→SetDefaultParamList；安装Algorithm有效唯一FilePath空默认；实际ISubInfo RevOutputFlag/FilePath | 生产共享provider可选algorithmResetResource/resetParameterWriter；仅Rotate传参，使用公共helper与专属空默认拒绝门，不再走Atomic；Translation未传参语义不变 | 已修并Reset16验证，实际两mode解绑后E1，保留非名单/cache，不另写解绑循环 |
| Reset空默认具体ABI | 原SetParam38980入口要求param3!=0，len0直接E0000001且path/matrix不变；len>0才判末两字节；方案恢复守卫另分支 | 生产默认写入在触碰Session/JSON前返回E0000001，保留旧FilePath/AppliedPath；普通文件编辑路径未改 | 已复用B Frame原体/指令与完整return链，加Rotate自身ctor/vtable PE证明唯一默认以strlen0调用38980，E1到UI catch；不是实际原厂桌面reset观察 |
| 采样运行 | Process376b0读取RefreshSignal/RotNum第0项、float第0项，3..16、累计(序号−1)*本轮角度float | Session.Run/Solver、Plugin.Runtime | 原120数值、34模块状态及失败输出/8文件已有trace；核首样本/总数/失败清采样和remain保留一致 |
| 坐标与中心 | CalibRot392e0/LeastSquare390e0/Inv38f60；只H左上2x2、前三采样拟合、全部累计误差、相等选负向 | RotateSolver对应float角差→double常量3.1415926与矩阵运算 | 旧720数值覆盖，非通用圆拟合；非有限结果独立显式诊断不造0 |
| 标定文件 | LoadFile3a670/LoadCalibFile3a340、SetParam38980，精确末尾ml/al，36bytes，短XML保留尾，打开失败清valid保留bytes | RotateFile/Session.Load，UI大小写严格.xml/.iwcal | 复用8文件trace；XML>9限制属显式安全实现而非原越界复现 |
| 方案/缓存 | SaveModuleData3b590/Load3b3f0，9float+valid，恢复guard首次SetParam只存path | PrepareSave CachedMatrix/Loaded独立节点JSON、Synchronize.Restore，不存采样counter | 格式独立，不宣称原方案ABI；旧RuntimeChecks删除原文件后恢复/计数重启 |
| 显示 | 原RotateDisplay含Image/ShowText/Record/List，原shape与featuregetter trace | DisplayDefinition+LastDisplay输入输出snapshot，公共Build | 旧DisplayChecks已有四对象数据/原status颜色/坐标订阅；核没有需要单独point图元producer，未知不造 |
| 生命周期 | ctor/ResetMember38bb0/DeInitVecData38cc0，Init临时N点SDK创建销毁，DeInit清采样/缓存 | RotateState托管session、scope.State或fallback字典，Reset移除node、配置关闭execution.Dispose | Reset16/Lifecycle6已验证matrix/counter/RevOutput保持、scope RemoveNode/关闭配置/显式Reset；未执行原Init设备测试 |

## 本轮源码→实施→验证退出

Reset唯一空默认桥已闭合并准确接生产；共享provider采用可选委托而非Rotate条件引用，Apply宿主拒写先不提交局部values（明确独立宿主事务）。Reset16包含拒写后续user-data写不携stale值。旧原120组数值/34状态/8文件fixture在本次生产DLL复验，总1259通过；无新增native运行。最终身份见FINAL-VALIDATION。当前正常活动域未见剩余必修差异，提交根线审计。原Init实际环境成功、原厂完整窗口视觉、真实Platform调度、原方案存储ABI仍为明确边界，人工另列。

## Reset桥最新闭合（覆盖上表的待补桥）

已复用B Frame e5cb0/e8220/125100/581e0/42760原体和原指令，证明空默认仍写入且传strlen=0；新增Rotate ctor实际vtable+18→38980的PE/asm身份确认。唯一FilePath空默认返回E0000001、保留原路径/矩阵/采样；已有完整return链把错误带回UI catch。详见ROTATE-CALIBRATION-RESET-SOURCE-CANDIDATE.md。根线批准后已落准确委托版，Reset16/Lifecycle6/原套件1259通过；产品仍冻结，无需新增平台初始化/设备测试。

## typed实际验证与最新剩余项

TypedInput179/Reset16/Lifecycle6/旧1259已在生产DLL通过，见TYPED-FINAL-VALIDATION。FilePath原OpenFileForCalibDialog共享继承的目录/模型attempt策略尚需产品修，整工具保持开放，不因typed通过计分。
