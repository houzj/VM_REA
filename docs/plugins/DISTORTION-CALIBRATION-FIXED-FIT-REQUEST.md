# 畸变标定：固定点集归一化求解需求

生产者：公共标定数值核（由根线统一提供）；消费者：DistortionCalibration，原厂1807ed0c0透视/径向/组合拟合及180853820径向种子H。

所需窄入口直接进入已恢复1807ff3c0的固定全量点集核；不得经过Solve外层RANSAC、四点枚举或32点限制。输入为成对float坐标、自由度1/2/3、原厂权重类型及WeightFactor、iterationLimit（畸变20）、coefficientDeltaThreshold（畸变1e-4）；返回实际H9和明确失败，不能用零/单位阵补失败。沿用原厂float归一化、初次单次正规方程、IRLS二次正规方程、FloatInverse(Tw)、float逆投影残差。兼容上现有Solve保持不变，新增独立入口，调用者负责图像网格预处理和点序。

源码调用记录（当前工作树analysis/distortion-calibration/core-live-eb19412b1b45482281d7ce5d0c087aa4/output/1807ed0c0.c）：

- model1透视，第870行：源为按最近中心点/四邻最短间距锚定的理想像素网格，目标为实际图像点；自由度来自HomoFreedom，WeightFun!=1/4时直接7ff3c0。源options局部uStack_b0=0x14（20）、uStack_ac=0x38d1b717（float0.0001），uStack_a8=参数结构+0x20，其他zero域。被调1807fd4e0:748已确认options+0xc为迭代上限，因此此调用明确20次；742行options+0x10是sqrt(系数delta平方和)停止门槛1e-4。现NPoint固定50不能直接沿用；窄入口需显式iterationLimit=20，保持NPoint现有50默认兼容。
- model2径向，第953行：逆径向后的点集与按实际网格ID缩放的理想格点，用于校准/误差；相同固定核，无RANSAC外层。
- model3组合，第1067行：锚定理想像素网格→逆径向后的点集，source局部uStack_c8=0x14、uStack_c4=0x38d1b717、uStack_c0=参数结构+0x20；相同核。
- 最终误差/框架变换还有第1145行调用，独立于文件H矩阵的最终物理布局。

WeightFun==1走1808345e0，WeightFun==4走180832830，不能把这些分支都替换为7ff3c0并称源码等价。当前公开UI只有1/2/3，4为原生额外分支。

验收证据：固定168点已有NImage原厂81/81矩阵float位回归必须保持；新增畸变SDK24例中按阶段隔离网格输入→输出，保留矩阵、残差及失败边界，不以最终图像测试替代拟合核验证。请求已发根线统一转记CONTRACT-REQUESTS，子线不修改公共契约。


补充返回载荷：默认权重固定核需同时提供原生 mean inverse reprojection error，不能由 double 数学逆重算。84 组真实 entry/return 捕获现包括 meanError、returnRax、完整点对和 H9；FitChecks 已对 82 成功/2 失败施加显式断言。meanError 的调用方含义依模型不同，model1 后续物理布局拟合会覆盖文件 H 的初次误差。

另一路源码证据（core-live-b15b9e4642e6403392bf33bf7bc225b8/output）：WeightFun1 的 1808345e0 使用 double 质心累计再乘 float 1/count，独立归一化和 8580e0 线性求解；其后 1808006e0 用显式 float 行列式/伴随逆来累计逆投影距离。因此窄 7ff3c0 API 只覆盖默认权重/IRLS 分支，不能将它包装为包含 Weight1 的所有原生权重算法。此分支需要单独追证并由公共核所有者协调复用，工具不复制公共求解器。

已实现公共 NPointSolver.FitNativePoints：默认 iterationLimit=50、coefficientDeltaThreshold=1e-4f，nativeWeightFactor 原 float 传入，畸变显式传20次；保留倒数乘归一化后 H33 原 float 值。Weight1 已独立恢复 8345e0，不再作为未支持分支：double 质心累计×float倒数点数、float距离与sqrt2缩放、8580e0单次正规方程、(Tw'Tw)^-1Tw'目标归一化伪逆、四次float矩阵乘法，尤其 Gram 行列式使用原先float乘积临时值括号。84真实点组×3自由度共252原厂调用，248成功/4失败边界一致；2232/2232成功字段逐位一致（正式零差断言）。此结果不扩展到7ff3c0的Weight2/3。

公共入口使用统一0x10103002适配失败码（不是声称7ff3c0返回该uint）；Weight4仍明确NotSupportedException。旧Solve及其私有两参RefineNormalized保留原来的50次、旧逐项除归一化路径，新入口不改变原调用者行为。

接口语义修正（以本段为准）：相同 WeightFun 整数不代表跨工具相同上游分派。最终拆为 FitNativePoints（始终对应7ff3c0，含Weight1常权IRLS）与 FitNativeLeastSquaresPoints（对应8345e0独立OLS）。畸变源7ed0c0的Weight1调用后者，Map源7d3e90即使Weight1仍调用前者。Map新12组实际Weight1主fit经前者108/108 float字段逐位一致；后者252组证据依旧2232/2232位一致。畸变84次7ff3c0回放82成功/2失败边界全匹配，534/738字段同位、max0.007117897仍未闭合。此次拆分后旧NPoint404组/11066字段仍0意外、24已知差异、788病态/并列记录不变。

### DC-WORKSPACE-001：畸变固定拟合的排序工作区前缀上下文（2026-10-18，在制）

- 生产者：插件侧标定运行会话/原厂兼容 scratch 容器（布局与初始化仍待恢复）；消费者：共享 7ff3c0→7fd4e0→7fec40 数值核及畸变标定。跨项目仅记录合同，不修改主框架工作树。公共核生产源码暂不改动。
- 已实现入口澄清：FitNativePoints 直接对应7ff3c0，含Weight1常权IRLS；FitNativeLeastSquaresPoints 对应畸变7ed0c0的8345e0独立OLS。Map即使Weight1也用前者。畸变options+0xc已从7fd4e0源码确证是20次上限，Map为50，delta均1e-4f；上节“options20不能直接当迭代次数/尚未实现”是旧阶段记录，以此处为准。旧Solve路径保持兼容。
- 所需载荷：完整float点对、原生options（自由度、权重、迭代、delta、factor）、前置H，以及排序缓冲前缀的原始float bits/边界、scratch容器来源和会话身份。当前最小受控fixture使用2n+32个前缀float；这只是覆盖已捕获12次回放的边界，不是已恢复的通用容器尺寸。禁止按病例硬编码sigma或统一填sentinel=1。
- 生命周期：原厂E2E SDK复用workspace与独立清零workspace在相同输入/前置H/options下返回不同结果。需继续恢复分配布局、前次写入来源、每次拟合/参数重置/模块关闭时保留或清理规则，之后才能决定scope状态和持久化策略；不把工作区脏值直接写进方案参数，不能把未知生命周期伪装为无状态数学核。
- 源证据：7fec40插入排序没有显式index>=0保护，会读写排序缓冲前缀。seq56 E2E前1float为0x3f800000(1f)，sigma=1.48257971；相同归一化点/系数、清零前缀时原核sigma=.00767620793。seq67分别3.61378813与2.20921592e-7。两套7fd4e0入口1296字节完全相同，初始设计/RHS1386字段同位；首次差异在权重生成后的正规矩阵。已排除旧trace序列关联错误。
- 验收证据：tests/DistortionCalibration.NativeProbe/{RunTraceMedianContext,RunMedianContextReplay}.ps1；docs/plugins/distortion-calibration-workspace-evidence.json保存6组完整输入/前缀与12个原厂输出。直接原核6组、清零/捕获上下文各一次均复现；受限托管排序回放只在捕获数组内访问，越过边界显式失败。原厂SDK完整容器布局与生命周期尚缺，不能据此声称全工具等价。
- 兼容要求：保持旧Solve404组/11066字段回归及Map Weight1 12组108字段、独立OLS252组2232字段结果；任何共享内核修正须先协调并单独验收。当前Workspace证据是上下文合同需求，不是强制所有调用者使用原厂越界行为的全局开关。

DC-WORKSPACE-001内部布局补证：7ff3c0以A(x)=Align128(x)排列workspace。normalizedSource偏移2*A(8n)、normalizedTarget偏移3*A(8n)、designA偏移4*A(8n)、rhs再加A(64n)、Ti再加A(8n)、Tw再加128、pointErrors再加128、weights再加A(4n)、medianSort再加A(16n)。7fd4e0只写2n个float权重，medianSort[-1]因此位于该区尾部未写padding。n=77时weights=8832，medianSort=10112，实际权重写入止9448，余664字节未写；sentinel偏移10108。内部布局已从原厂源码恢复，仍缺外层SDK分配/清理/复用与前次写入来源，不对未定义旧值规定统一默认。受限托管context replay正式12/12原厂sigma逐float位相同。

生产接入补记：FitNativePoints仅新入口恢复原厂初次H反归一化→重新归一化、float逆投影乘法关联及float停止delta；旧Solve不改。DistortionCalibration.FixedKernelChecks正式36组/324字段逐位通过，W1/2/3和三自由度，默认50/1e-4，nativeWeightFactor原值传入。畸变调用显式20；独立OLS仍由FitNativeLeastSquaresPoints分派。Weight4拒绝。84畸变trace成功矩阵697/738字段同位，其余5调用41字段与DC-WORKSPACE-001关联，不用扩大阈值或统一padding值掩盖。

分析载荷需求已实现：AnalyzeNativeIrlsMatrix/AnalyzeNativeLeastSquaresMatrix(points,H)返回(uint ErrorCode,float MeanInverseError,float[]? PointErrors)，原H不改；两个API分别对应7ff3c0尾段/8006e0，原SSE的float逆与预计算顺序一致。MeanChecks正式164均值/11852逐点字段（两个API各82原H）逐位通过。MatrixToNativeParameters为800af0六float分解，官方SDK/core109矩阵1284字段逐位，107成功/2失败；不含额外PixelPrecision。零残差早退仅新native路径取消，Board34边界/Map324位通过，旧Solve保持；workspace生命周期未闭合。

DC-WORKSPACE-001前值来源新证：2026-10-19专属destroy/recreate探针将下一次80点圆点medianSort[-1]地址提前硬件监视，捕获前置99点棋盘设计矩阵第83点field15的`-targetY*sourceY`写入；RVA8003da、RAX/RBX/R10/XMM4和实际bits均有6次一致观察。该负位值0xbf1dd89e在23个后继实例46次运行保持，而相同圆点48次独立baseline保持零；94份圆点输出一致。详见DISTORTION-CALIBRATION.md“跨实例同址复用与前值写入来源”和distortion-calibration-recreated-workspace-evidence.json。此项确认跨n的布局重叠和部分旧值来源；未恢复生产宿主的分配scope、通用初值或正前缀差异case的最近写入，不请求公共核硬编码前缀。

DC-WORKSPACE-001正前缀补证（2026-10-19第二轮）：原12-case/84固定调用序列复现seq56/67捕获sigma。seq56前缀1f最近来源为前实例seq46 n80权重第95float，原7fd4e0 RVA7fdcfb重复权重写；seq46-inner+9340与后seq56-inner+10108同址，inner前移768bytes。seq67前缀0x39c6af7a最近来源为前实例seq63的7fec40插入排序越界写，RVA7ff183、RCX=-2、目的median[-1]。两次销毁前后目标可读且bits不变，下一工具main复用；不能由此断言allocation未free。专属ExportPositiveWorkspaceEvidence.ps1逐字节输入、前缀/sigma及地址寄存器和生命周期断言通过，详见distortion-calibration-positive-workspace-evidence.json。本轮不请求公共kernel硬编码workspace；真实宿主分配scope及安全兼容策略仍待协调。

C1最新源码证据（2026-10-19）：实际模块DeInit/析构会Destroy SDK工具，core对象每轮Process正常分派返回后以mask1归还type0 scratch；MVDCommon全局池FreePoolMemory只改busy/时间，复用不清payload，CRT free在清理路径。详见DISTORTION-CALIBRATION-C1-WORKSPACE-LIFECYCLE.md及配套源码索引。原先未取得的所有者/归还主链现已缩小为明确路径；这不把任意原厂旧值变成可持久化输入，也不授权公共核用固定prefix。容量/异常全分支及真实宿主加载绑定、并发仍须分别验证。

### C2排序与权重边界源码切片（2026-10-19）

完整分支索引见 `DISTORTION-CALIBRATION-C2-SORT-BOUNDARIES.md`，机器证据见 `distortion-calibration-c2-sort-evidence.json`。按PE pdata边界补齐7fd4e0/7fec40原指令与security-cookie后的epilog：排序输入为n个逐点残差，偶数取上中位；插入排序没有j≥0保护，严格新最小值可读median[-1]，前缀大于key时才继续负索引写入。不能把单个前值或2n+32捕获范围当作通用安全边界。权重为2n个重复X/Y值；WeightFun 3超界分支写EPS（2^-23），并非0，依据7fd944/7fdd85。

本轮仅复用6组既有原厂快照做安全离线回放，12个sigma逐位相同、493个排序float逐位相同，另有4个独立分支见证；这些不是新增原厂执行或全域等价证明。没有产品或公共核改动。非有限/非法n实际合同、全部上游分派/归一化（C3）、矩阵运算退化及C1异常/尺寸helper和真实宿主并发仍开放。

### C3配置到拟合分派第一源码切片（2026-10-19）

见 `DISTORTION-CALIBRATION-C3-CONFIG-DISPATCH.md` 与 `distortion-calibration-c3-dispatch-evidence.json`。模块WeightFun旧名转CalibBoardWeightFun；首字符4被模块忽略，core数字ID40002的4在Process应用时会写为算法context的3（缓存原值另存），低层832830分支存在不证明模块可达。真实SDK vtable/参数寄存器、7f7050字段缩放、7ed0c0三模型和三拟合入口已建立分层索引；WeightFactor20传2f，IRLS上限由本地options另写20。同名Weight1在畸变为独立OLS，在映射工具为常权IRLS。probe中的warp是测试像素生成标记，不是原厂参数。只做专属源码/文档，字符串schema全链和模型点构造精确表达式等剩余项显式保留；未改公共核或产品、未新增原厂执行。


C3-2最新源码：`DISTORTION-CALIBRATION-C3-SCHEMA-STATE.md`及`distortion-calibration-c3-schema-evidence.json`已取得原core XML206（REA PE资源证据）、四字段完整ID/default/range和转换/缓存主链。7020/7c6830 operation6只校验；7790缓存原始ID/value并置dirty；Process前7120以operation1应用，Weight4才在algorithm context转3，schema DOM与读回缓存不能称作3。内嵌schema含Ransac4而模块UI无4；模块字符串首字符4仍提前忽略。本轮静态16锚点/65文件哈希检查通过，无原厂执行或产品变更；数值异常token、失败回滚、完整持久化与模型网格/中心精确选择仍开放。

C3模型选点有限切片见 `DISTORTION-CALIBRATION-C3-MODEL-POINTS.md` 与 `distortion-calibration-c3-model-point-evidence.json`：用户中心经SDK向零截断用于a最近点，等距保留先索引；b按四邻域(-x,+x,-y,+y)每向首匹配、严格更短更新，等长保留先方向。径向LM中心初值独立取图像半宽/半高，初始H内部重新拟合；853820并不接收caller H缓冲，其第5/6参为径向系数和中心输出。12原指令锚点与常量/17源文件哈希检查通过，无新原厂运行或产品修改。旋转网格/原点及gridScale全分支仍开放。

C3 gridScale/原点补证见 `DISTORTION-CALIBRATION-C3-GRID-ORIGIN.md` 与 `distortion-calibration-c3-grid-evidence.json`：SDK常量1f经0x67+0c→op0xf+24→options+10，独立于角度和量化边长。X邻接距离变化率严格>0.1f时保留原网格，否则旋转/量化选原点，再按原标签差幅值及旋转符号重建Q；量化值不是最终Q。MODEL-POINTS已补扫描→处理块→回跳CFG表，四邻域仍为(-x,+x,-y,+y)，不能按离散处理块地址排序。grid检查15锚点/8文件哈希通过，无新原厂运行/产品修改；非有限、溢出、无邻接和零距离实际合同仍开放。

C3文件保存有限切片见 `DISTORTION-CALIBRATION-C3-FILE-SAVE.md` 与 `distortion-calibration-c3-file-evidence.json`：19项字段由核心blob→SDK对象+2c→76byte原样写出，独立于另9项主H；重映射为输出像素到输入采样，组合模型先透视再径向。GetResult受Run有效标志门控，但直接Export只查对象存在；自动保存与显式SaveCalibPath及Refresh门控已定位源码/实际path寄存器。原writer不查fwrite/flush/close返回，产品finite/header/尺寸校验属独立选择。本轮16锚点/32文件哈希检查，无新原厂运行/产品修改；catch映射/全部内层失败/路径异常运行仍开放。整卡阶段审计保守5/32，见DISTORTION-CALIBRATION-STAGE-AUDIT.md，切片通知不计整卡S7。

C3模块异常发布有限切片见 `DISTORTION-CALIBRATION-C3-CATCH-OUTPUT.md` / `distortion-calibration-c3-catch-evidence.json`。被动PE EH严格匹配三主函数，恢复4 try组、16 handler与6实际续点：ProcessAlg第一组含创建/Input/ROI/SDK Run，第二组为GetResult/路径/自动Export；捕获码与BASIC success/错误/时间分开写，非主伪码恒0。ProcessOutput先发ModuStatus/runtime，success0跳过结果字段；其返回码被Process忽略，发布失败不等价于最外层Process错误。原厂无已发布字段回滚证据。12锚点/65源文件检查通过，无新原厂异常运行或产品修改；EH外/handler内异常及框架最终消费仍开放，整卡阶段计数不因切片自动增加。
