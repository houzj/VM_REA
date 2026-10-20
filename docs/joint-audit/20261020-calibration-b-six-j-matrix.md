# 标定 B 类六工具 × J01–J12 保守对账

对象仅为 N 点、N 图像、标定板、相机映射、映射标定、畸变标定。C 类四项只作为必要消费者引用，图像源和 Group 只核交接。依据截至第十一轮的**联合审核**材料；插件线的 10/10 实施自审不自动转成这里的 J 门完成。此报告只读复核，不增加探针或产品改动。

## 判定口径

下表 `✓` 表示证据所指的**具名边/条件**可关闭，`△` 表示已有原源码与当前链的一部分但 J 门尚缺列出的分支，`—` 表示本次材料没有该工具的专属联合证据。即使出现 `✓`，也不得外推为整个 J 门关闭。**本次六工具完整 J01–J12 关闭数为 0/6**：共同 J01 缺原 SDK 动态实例身份，J02 缺原选择→创建的逐项运行态，J12 缺原厂/用户同构卡死线程栈；其余门也各有独有未证边。十工具既定实施自审 10/10 不因此倒退；这是另一审计范围。

证据简称与逐文件 SHA256（均为 `docs/joint-audit/` 下报告，原件路径/成员/哈希、命令、原始日志哈希在各报告正文）：

| 简称 | 报告 | SHA256 |
| --- | --- | --- |
| I | `20261020-pass6-calibration-original-identity.md` | `1307138D4C2C4FAB204F49A1BA8F4762E91D94952ABDABAD0415B8D7132505CC` |
| H | `20261020-pass6-calibration-catalog-host.md` | `BF14589D29686DFA14C89F378C1496DDFA3BDB5C37966B66C70E819A33151E5C` |
| NP-UI | `20261020-pass6-npoint-ui.md` | `DE3CD19573568A9BA9C8662A0CD144A33500E856C0294480CC6C4F8A795A3B9E` |
| NP-R | `20261020-pass6-calibration-result-chain.md` | `0726E64F5F0FDDAAE7C282966D6BE0ABF29C11641FE469E932441017A0D6DF05` |
| CFG | `20261020-pass7-calibration-config-bridge.md` | `5F2CFF8E299479E2EAF11C29026263879AC003458CA68FA3EBA52F8704E87EF1` |
| NI/B | `20261020-pass8-nimage-board-chain.md` | `C618A2353CB469DFCA2A062B6C61907995A254A39240495F36C7E05D431E7DB0` |
| CAM/MAP | `20261020-pass8-camera-map-chain.md` | `694A49C55012D7CF265C8EDCE4918F9CAA326DC4F1332ABBD047C2F81CCC91EA` |
| DIST | `20261020-pass9-distortion-chain.md` | `6BA6E27B36B865F2FCEC39FB154040646582F354874BB4E37822BCBC5446AFAE` |
| MAP-IMG | `20261020-pass10-map-image-chain.md` | `D170EE0F7B1CD9C95664892EDE90F44A642D96CD53B53D62ECF951FFD2C948D2` |
| B/ROT | `20261020-pass10-board-rotate-chain.md` | `B495DF0DFF88684FDDF902A8DC2CA2FE82350A8842DA3BC9F1544C328B6F01E5` |
| NP-EDIT | `20261020-pass10-npoint-edit-chain.md` | `7591528018911068F4D0BF3B3EBF1719DE8B853C01617BFC85152909FEB6AEAB` |
| NI-STATE | `20261020-pass11-nimage-state-chain.md` | `9FB04432D2DECF37B13AE710F8DEB56536679B2E48DA2C05D1AFCD254D3636DB` |
| NP-HOST | `20261020-pass11-npoint-mainwindow-cost.md` | `106678470DC0FA4D507471804B33D1391DC0C49EC50A93600594518C2B14075E` |

共同证据 `I+H`：安装十项 ToolItemInfo/主 XML/二进制静态身份与当前真实 WPF 分类→registry→AddNode 均已逐项核；标定板原 `ToolItemInfo.name=CalibBoardCalib` 与目录/主 XML 不同。`CFG`：十项普通配置由 provider 进入共享窗；仅 N 图像 `CalibPointTotalNum` 与相机映射 `WeightFactor` 两参数做了写回/撤销/重开。共同尚缺**原厂 SDK 动态实例数值 ID、逐项原选择→CreateModuleToolRe 的运行态边、全控件 Apply/取消/关闭、相同条件权限门**。这三项共同证据只能在每工具矩阵中引用一次，不能算六份独有验收。

### J01–J06：身份、创建、配置、来源、类型、调度

| 工具 | J01 身份/依赖 | J02 发现/创建 | J03 UI/事件 | J04 候选/连接 | J05 类型/载荷 | J06 运行/调度 |
| --- | --- | --- | --- | --- | --- | --- |
| N 点 | △ I：原 DLL/XML/当前 ID 静态对齐；缺动态实例 ID/SDK 依赖加载态。 | △ H：真实当前创建；缺原点选→实例运行边。 | ✓ 点表五 setter→字段参数、当前真实主窗 4/12/32 行 `5N` 次整图 Changed 及单格 +1，NP-UI/NP-EDIT/NP-HOST；△ 失败 setter、导入/清空、取消时历史/原生 SetParam 副作用。 | △ 点对为输入参数/订阅；NP-R 为预填样本，缺原点叶候选、连接/断开/Group 变量时各状态。 | ✓ XML `CalibMatrix` 九 float→当前 `NumberArray`，误连便利 `CalibData` 被拒，NP-R；△ 点表全部类型/短值/非法值。 | ✓ 当前 N 点→加载→Group→坐标系首跑/重开，NP-R；△ 原 SelfRun 准备上游、连续/Stop/超时及错误调度。 |
| N 图像 | △ I：原模块/资源哈希见 NI/B、NI-STATE；缺动态 ID/SDK 装载态。 | △ H：当前创建；缺原运行态。 | ✓ `ImageSource` 原三 operation 和本地 `CalibPointTotalNum` 写入/Undo/Save，CFG/NI/B；△ mutual exclusion、专用导出/关闭/复位全控件。 | △ 原 `GetFrontParamItemsOperation(IMAGE)→GetSelectedCombination→SetCombinationSource` 与当前图 `Image` 连接，CFG/NI/B；缺动态断开/Group/变量候选排序及配置窗热更新。 | ✓ 原 IMAGE 四叶/运动输入，当前 Gray8 九帧与文件 `float[9]`，NI/B/NI-STATE；△ RGB/缺运动/非法维度及原像素 ABI 差分。 | ✓ 当前八有效＋第九空白失败→下一帧新批次，owned/public 状态有别，NI-STATE；△ 原同帧 SDK 失败后的回调、SelfRun、Stop/跨会话。 |
| 标定板 | △ I：原身份及 `name` 异名已证 I/NI/B；缺 SDK 动态创建 ID。 | △ H：当前创建；缺原选择 `CalibBoardCalib`→实际 SDK 名/ID 运行边。 | △ NI/B 原 `ImageSource`/`ROISelecter` 声明、CFG 当前开窗；缺 ROI 实际控件写字节、参数提交/撤销/关闭。 | △ NI/B 原 IMAGE 来源 operation 与当前 Gray8 `Image` 连线；缺热切换/断开/变量/Group 候选。 | ✓ Gray8→99 点/36B `.iwcal`，误交 XML 加载器拒绝；B/ROT 进一步逐 float32 位交给 Rotate；△ 其他板型、ROI、RGB/图层结构。 | ✓ 当前真实图 Board→文件→Rotate 连续三采样/空白失败恢复，B/ROT；△ 原平台同构调度、连续/Stop/超时与失败文件释放。 |
| 相机映射 | △ I/CAM/MAP 原 DLL/XML/当前 ID；缺 SDK 动态 ID。 | △ H 当前创建；缺原选中→创建运行边。 | ✓ 原 `WeightFun→WeightFactor` 显隐/范围与当前 `20→30` 写回、Undo/重开，CFG；△ 其他点叶/所有模式/取消/专属 UI。 | △ 原 XML 为 Target/Object 点对，不应要求 IMAGE；CAM/MAP 当前订阅常量四点；缺原 POINT 选择/断重接/变量边界。 | ✓ 原无直接矩阵端口、36B 九 float 文件；当前双文件进入 Map，CAM/MAP；△ 点对 2–8 组启用/缺项及 native ABI 精度。 | ✓ 当前两 Camera→Map→加载→Group→坐标系文件依赖重开再生成，CAM/MAP；△ 原平台/Stop/超时/多次失败后缓存。 |
| 映射标定 | △ I/CAM/MAP 原 DLL/XML/当前 ID；缺动态 ID。 | △ H 当前创建；缺原运行边。 | △ 原双模式 UI 资源/CFG 开窗；缺双 ROI、示教/编辑器参数写回及取消。 | ✓ 文件模式双路径；图像模式双 `InputImage1/2` 真实接线，CAM/MAP、MAP-IMG；△ 一侧先到 pending、断开重接/Group 变量候选。 | ✓ 36B 双输入→XML `CalibMatrix[9]`→加载；双 Gray8 图各 168 角点，CAM/MAP、MAP-IMG；△ 双 ROI 掩膜/不同图格式与校验错误。 | ✓ 文件模式和双 IMAGE 模式都经真实 owned Group 消费并重开，CAM/MAP、MAP-IMG；△ 原图像异步更新/教学/Stop/超时。 |
| 畸变标定 | △ I/DIST 原 DLL/XML/资源身份；缺动态 ID/SDK 装载态。 | △ H 当前创建；缺原运行边。 | △ DIST 原 IMAGE/ROISelecter→`IUserBytesData` 链，CFG 当前开窗；缺当前 ROI 实际写入/取消和原热 UI 差分。 | ✓ 原 IMAGE operation 与当前 `ImageSource.Image→Distortion.InputImage→NImage.InputImage`，DIST；△ 未连接/断连/变量/Group 候选。 | ✓ 原 `PostCorrectionImageData` 为 `PostCorrectionImage` IMAGE 组合成员，当前 1000×1000 图/76B 文件送 N 图像，DIST；△ RGB/其他板型/ROI 和原像素结构。 | ✓ 当前路径扩展名失败使后继跳过、恢复同 scope 成功，DIST；△ 原完整 SelfRun/Stop/连续、SDK 写错误后的调度。 |

### J07–J12：算法、显示、持久化、生命周期、闭环、性能

| 工具 | J07 算法/结果 | J08 显示 | J09 持久化/历史 | J10 生命周期/并发 | J11 跨节点闭环 | J12 宿主性能/卡死 |
| --- | --- | --- | --- | --- | --- | --- |
| N 点 | △ 原 `ProcessAlg 0x3caf0→WriteXmlData 0x3f990` 与当前一组求解/文件，NP-R；缺同数据集原 SDK 数值及坏点/奇异/失败全分支。 | — 缺原结果显示资源→当前 adapter/窗口的成功、失败及切换节点对照。 | ✓ 当前单格 Undo/Redo、保存重开再求解/加载，NP-EDIT；△ 原参数历史、路径搬迁、导入清空/动态点数。 | △ 点表初始化同步放大已定位，NP-HOST；缺原 setter 失败/重开窗释放和取消旧回调。 | ✓ N 点 XML→加载→owned Group→坐标系，重开复跑，NP-R；△ 原版同构 Group、坏文件和其他矩阵分支。 | ✓ 当前主窗 4/12/32 行开窗产生 20/60/160 次 Changed，NP-HOST；△ 无原版同机分段/用户卡死 UI 栈。 |
| N 图像 | △ 原 `Process 0x20b90→ProcessAlg 0x21450→ProcessOutput 0x21850→WriteXmlData 0x230e0` 的帧序/满批门及当前失败恢复，NI-STATE；缺原 SDK 同帧错误/矩阵差分、Clear 未 Run 旧结果。 | — 缺原每帧/完成/失败覆盖层与当前 display adapter 的热切换/清旧值链。 | ✓ 当前 N 图像参数 Undo/重开 CFG，完成 XML 经 Group 重开 NI-STATE；△ 未完多帧会话/原存储格式和文件移动。 | △ owned Current 与失败公开输出已分开，NI-STATE；缺旧 SDK result 缓存所有分支、关闭/重开取消并发。 | ✓ 九帧 XML→加载→坐标系 NI/B；失败后恢复 XML→owned Group→坐标系/重开 NI-STATE；△ 原版同构和多帧跨 Group。 | — 无本工具配置/九帧重复运行的 UI 心跳及无响应栈。 |
| 标定板 | △ 原 `ProcessAlg 0x20240→ProcessOutput 0x20630`、SDK Export 九 float，NI/B/B/ROT；缺其他板型/ROI/NoMatch 原同图数值及异常释放。 | △ 原 ROI/检测框输出声明 NI/B；缺当前真实 UI 图层在成功/失败/切换后同结果对象。 | △ 当前 Board→Rotate 方案保存重开后核 IMAGE 绑定与五条订阅，B/ROT；该报告未证明重开新 scope 后重新运行，且缺 ROI/工具文件历史、原格式及路径迁移。 | △ 当前失配后不重写旧文件、删文件失败/恢复，B/ROT；缺原 SDK fwrite/fflush/fclose 与宿主取消回调。 | ✓ Board `.iwcal` 九 float→Rotate `LoadCalibFile` 对齐原字段、当前真实图三帧和失败恢复，B/ROT；△ 当前 owned Group 包装与重开后执行、原厂同构桌面/数值全域。 | — 无 Board 配置、采样和显示的 UI 心跳/卡死栈。 |
| 相机映射 | △ 原 `GetProcessInput 0xa710→ProcessAlg 0xbd20→WriteCalibFile 0xd450` 与当前四点/36B，CAM/MAP；缺同输入原 SDK 逐项数值、2–8 组边界与失败。 | — 缺标量/点对显示资源到当前 adapter、切换/失败清理对账。 | ✓ `WeightFactor` 参数 Undo/重开 CFG、两个文件再生成 CAM/MAP；△ 全点叶、路径搬迁/坏文件历史。 | △ 当前删文件重跑产出，CAM/MAP；缺原 SDK 文件句柄/异常释放、窗口并发。 | ✓ 两 Camera 文件→Map→加载→Group→坐标系并重开，CAM/MAP；△ 原版 Group 同构与逐值差分。 | — 无此工具编辑点对/运行的分段耗时与无响应栈。 |
| 映射标定 | △ 原 `ReadFileMatrix 0x227d0` 与 `ProcessAlg 0x21c50` 文件方向；原 `Process 0x21690→GetInputImage 0x203d0/RoiFix 0x22f40` 双图，CAM/MAP、MAP-IMG；缺双 ROI/示教/图像同样本原 SDK 数值。 | — 缺结果图/角点/ROI 原显示资源与当前 adapter 失败清旧值。 | ✓ 双图模式新 XML 首跑、保存重开删文件再产出 MAP-IMG；文件模式同理 CAM/MAP；△ 教学/双 ROI/坏文件历史及原格式。 | △ 双文件输出/图像重新求解已复跑；缺一侧 pending 后取消、并发更新和原回调释放。 | ✓ 两文件模式及双 IMAGE 模式各自→XML→加载→Group→坐标系，CAM/MAP、MAP-IMG；△ 原版同构 Group/不齐帧时序。 | — 无双图加载/拟合 UI 心跳及原/当前卡死栈。 |
| 畸变标定 | △ 原 `Process 0x20550→ProcessAlg 0x20820→ProcessOutput 0x20b70` 与当前一张板图/76B，DIST；缺全板型/ROI/色彩与 SDK 数值、文件 I/O 失败结果。 | △ 原 `PostCorrectionImageData` 显示映射已核，DIST；当前基础帧仅选 `InImage`，缺双图切换/覆盖层/失败清旧图对照。 | ✓ 当前图保存重开保留修正图→N 图像端口，DIST；△ ROI/结果缓存及原路径迁移。 | ✓ 当前错误扩展名失败、N 图像跳过、改回后同 scope 恢复，DIST；△ 原文件写关闭失败、旧 ArtifactSession/显示释放/并发取消。 | ✓ ImageSource→Distortion 修正图→N 图像第 1 帧并重开，DIST；△ N 图像最终矩阵/原厂同构图。 | — 无双图显示/运行的 UI 响应采样及无响应栈。 |

## 可停止重复审核的已闭边与下一批有限缺口

已可复用且**不需再按六工具重复做**：十项安装静态身份与当前实例化 `I/H`；普通配置宿主分流 `CFG`；N 点点表成本 `NP-UI/NP-EDIT/NP-HOST`；N 点/N 图像/相机映射/映射标定/标定板/畸变的上述真实生产或传递边。已有日志和 SHA 足以证明这些**具体状态**，再次跑相同正常夹具不会增加 J 门完成度。

为了避免无终点循环，下一轮应先选**有有限判定结果**的缺口，而非泛称“补源码”：① B 六项 J08 每项确认原显示资源的入口、结果对象身份和当前 adapter，至少一成功/一失败/切换旧结果；② B 六项 J10 各取一个真正未证的资源/回调释放分支，先查原 ctor/Dispose/DeInit/异常区，再决定是否需受控实例；③ J03/J04 只补标定板 ROI、映射标定双 ROI/教学、N 图像批次参数触发与订阅失效，逐项记录具体 XML operation 与当前参数写入；④ J07 只挑实际原 SDK 同输入可运行的差异夹具，分别对比矩阵/点/图，不拿不同样本的旧回归代替。原厂动态 SDK 实例 ID 和原平台同构 Group/调度若受环境限制，则在对应 J01/J02/J06/J11 标记**外部证据缺失**，不能反复换名分轮数。J12 的用户无响应需当次 UI 线程栈/等待链和同构方案；在取得该证据前仅能称 N 点的 `5N` 同步刷新是确定成本机制，不能称其为所有卡死的根因。

当前不存在一项仅凭上述联合报告就能宣布六工具之一“J01–J12 整工具关闭”；这和用户此前授权的工具实施 10/10 自审口径并不冲突。人工桌面验收独立，不计此矩阵。
