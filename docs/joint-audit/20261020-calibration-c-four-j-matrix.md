# 标定 C 类四工具 × J01–J12 保守对账

2026-10-20，只读复核第六至十一轮既有联合审核与 J 门，范围是平移旋转标定、旋转标定、坐标系、标定加载。**没有新探针、产品修复或 `tool-plugins` 改动。** 原件源证与当前方案运行分列；插件项目既有整工具实施自审只供定位，不能替代主框架交接审核。J 定义以 [审核闸门](20261020-audit-objectives-and-drift-controls.md) 为准。本表的“已证”只关闭写明的**有限链段**；“未关”指该工具的完整 J 门仍缺列出的具体边。一次共用目录/Group 核验只引用一次，不重复算四份工作。

## 逐工具矩阵

| 门 | 平移旋转标定 `vision.translation-calibration` | 旋转标定 `vision.rotate-calibration` | 坐标系 `vision.coordinate-module` | 标定加载 `vision.read-calib-file` |
|---|---|---|---|---|
| J01 身份/依赖 | 已证安装 DLL `A85A…F183`、静态目录/XML 与当前 ID [E1]；未关原 SDK 创建后的数值实例 ID、全部依赖装载实况。 | 已证 DLL `B084…BB41`、静态入口 [E1,E8]；未关原 SDK 实例 ID、装载实况。 | 已证 DLL `8547…AF6`、静态入口 [E1,E11]；未关原 SDK 实例 ID、Cpp/SDK 实际装载关系。 | 已证 DLL `86B8…403`、静态入口 [E1,E4]；未关原 SDK 实例 ID、Control/SDK 实际装载关系。 |
| J02 发现/创建 | 当前 MainWindow 目录→注册→factory→AddNode 实测 [E2]；未关原工具箱选中此项到 `CreatModuleByInfo/CreateModuleToolRe` 的动态请求及创建失败/冲突。 | 同一共享当前入口实测 [E2]；同缺原动态请求及失败/冲突。 | 同一共享当前入口实测 [E2]；同缺原动态请求及失败/冲突。 | 同一共享当前入口实测 [E2]；同缺原动态请求及失败/冲突。 |
| J03 配置 UI/事件 | 普通 provider 配置窗可开 [E3]，原文件写门见插件侧源码；未关本工具每个原 XML 控件的 Commit/Reset/取消/失败回滚→原 SetParam 与当前文档同事件对账。 | 普通 provider 可开 [E3]，原 `OpenFileForCalibDialog FilePath` [E10]；未关真实主窗路径失败/同值重试/Reset 的联合事件链。 | 原 `CordinateCreate` 控制 `RoiType/InputWay` [E11]、当前 Host 三合法 radio/ROI 交互见插件侧测试；未关原 ROI 编辑→`this+ec/f0/f4` 序列化/坐标空间与主窗取消/复位同条件。 | 普通 provider 可开、独立调试窗存在 [E3]；未关原路径/刷新 UI 的提交、关闭及动态 schema 通知到当前宿主的同事件链。 |
| J04 来源候选/连接 | 当前 `CalibStatus→RefreshSignal` 与点数组→Group 成功 [E7,E9]；未关原 `GetFrontIOWithLeafItemsByName` 候选/RelateIO 对本工具连接、断开、重选及 Group/变量三态。 | 当前相机/板状态和测试专用样本输入接通 [E8,E10]；未关原五个订阅口的候选/已选值/断接，以及样本源并非原厂自动连接。 | 当前 IMAGE、POINT、Matrix 数据边接通 [E4,E9,E11]；未关原三种模式切换后订阅候选/保留/断开的三态。 | 当前刷新订阅与动态矩阵输出进入 Group [E4,E5]；未关原 FilePath/RefreshSignal 候选操作以及动态叶消失、改名时原通知与已选值。 |
| J05 类型/载荷 | XML `CalibOutputParam/CalibMatrix` 九项→加载器/Group/坐标系真实传递，`CalibData` 直连被拒 [E7]；未关组合/示教输入、Image/ROI/点数边界及错误载荷。 | 原 `.iwcal` 严格 36 字节、九 float 和消费索引 `[0,1,3,4]`，当前相机/板文件实接 [E8,E10]；未关其他输入缺项/错型和 XML 短/超长所有边界。 | 原 `InputImage` 前置、POINT 第 0 项、Matrix Count 1..9、ROI type12 与当前真图载荷 [E4,E5,E9,E11]；未关原 ROI 坐标序列化、动态 scope 多项 batch 与畸形图像 ABI。 | 原 `RefreshSignal`/XML FloatList 动态输出与当前正常/坏 XML 图 [E4,E5]；未关动态端口改名/改型的原宿主订阅传播和所有数据类型条目。 |
| J06 运行/调度 | 当前文件生产→加载→owned Group→坐标系两次执行 [E7]；未关 SelfRun 未运行/已运行/上游已变三状态、连续/Stop/超时、原厂同构调度。 | 当前三轮同 scope 采样、缺文件恢复 [E8,E10]；未关原 SelfRun 上游准备、Stop/超时/切方案 epoch，以及原厂同构调度。 | 当前 Matrix/POINT/Draw 图和暖态保旧 [E4,E5,E9,E11]；未关原 SelfRun、上游图变更后的候选与执行时序、Stop/超时。 | 当前同 scope 六状态刷新与 Group 传播 [E5]；未关原平台动态输出消失时 Group/订阅调度、SelfRun 和取消/超时。 |
| J07 算法/结果 | 原 `Process/ProcessAlg/WriteXmlData` 三体证正常成功写九项 [E7]，当前一个 `state-rotation-three` 夹具通过；未关组合/示教/旋转累计、失败样本和同输入原 SDK 数值差分。 | 原 `SetParam/Load*/Process/CalibRot` 对文件与三样本有体/原指令 [E8,E10]，当前中心 `(50,50)`；未关多角度/退化输入正负夹具与同输入原 SDK 数值差分。 | 原 Draw 矩阵式、POINT 读取顺序、Matrix 前缀和输出保留有体 [E4,E5,E9,E11]；未关原 ROI 坐标空间、原 SDK MatrixToParam 数值差分、首次短矩阵不定尾不作为规格。 | 原 `Process/SetParam/LoadFile` 精确区分读失败保旧与缺输出节清集合 [E5]，当前图复现；未关原所有类型/重复叶/解析异常与当前动态 schema 的逐条差分。 |
| J08 显示/预览 | 当前结果文件/矩阵已核 [E7]；未关原 Translation Display 资源→当前 LastDisplay→实际 MainWindow 的同轮图/指标、失败后切节点旧结果。 | 插件自测涉及显示，但联合链 [E8,E10] 仅核输出；未关原 RotateDisplay 资源与当前主窗同轮结果/失败后切节点。 | 原 `CoordinateModuleDisplay.xml` 六叶/Status 与当前真图显示背景和坐标图形 [E11]；未关失败后切节点/切方案不残旧图，原厂同轮图差分。 | 原 Display 仅 DataRecord 与插件自测有定位；[E5] 只核公开输出；未关动态输出消失/切节点时实际 MainWindow 显示清理。 |
| J09 持久化/历史 | 当前 XML→Group 图保存重开再跑 [E7]；未关本工具参数、点表、路径、教学状态的 Undo/Redo 与原方案格式/恢复调用链。 | 当前文件交接图保存重开保订阅 [E10]；未关累计半途样本、FilePath 失败态、Undo/Redo、原方案恢复。 | 当前 Matrix/POINT 图重开、Group 连接 Undo/Redo、Draw ROI 撤销/重开再跑 [E4,E6,E9,E11]；未关原 ROI/订阅历史与原方案格式；Normalized 是独立适配。 | 当前正常链重开及动态缺输出恢复 [E4,E5,E6]；未关原动态 schema/文件路径历史及端口变更后重开、原方案格式。 |
| J10 生命周期/并发 | 插件自审有 scope/Dispose [P-T]；联合链 [E7] 未核主窗关窗、RemoveNode、旧回调、取消与新方案互串。 | 同 scope 失败清采样/恢复 [E8,E10]；未关真实宿主关窗/RemoveNode/切方案旧回调和原 `DeInitVecData` 对应释放计数。 | 暖态矩阵保留与新 scope 分离 [E5,E11]；插件 Host 已核配置 viewport 关闭释放 [P-C]；未关主窗切方案 epoch、并发取消/错误释放与原 `DeInit/Destroy` 逐边对账。 | 同 scope 读失败保留/重开冷恢复 [E5]；未关配置关闭/RemoveNode/切方案时动态输出监听与旧回调阻断。 |
| J11 跨节点闭环 | **具名正常链段可关闭**：本工具→XML→标定加载→owned Group→坐标系，首跑与重开 [E7]；完整门仍缺组合/失败数据路径与原厂同构图，不重复算共享 Group。 | 相机或板 `.iwcal`→本工具采样，真实生产/消费已闭 [E8,E10]；**完整门未关**：本工具六个中心/误差结果尚无真实下游消费者，亦无原厂同构图。 | **具名正常链段可关闭**：平移旋转/标定加载→Group→本工具 Matrix，另有 POINT 和 Draw 输入图 [E4,E7,E9,E11]；完整门未关本工具 `CorCalibMatrix`/六叶的真实下游消费者与原厂同构图。 | **具名正常链段可关闭**：N点及平移旋转 XML→本工具→Group→坐标系；读失败与缺节传播也实测 [E4,E5,E7]；完整门未关原厂动态输出消失 Group 及改名/改型订阅。 |
| J12 宿主性能/卡死 | 当前本工具无同构图 UI 心跳/分段耗时/卡死栈；[E3] 开窗成功不能关门。 | 同缺；三轮运行 exit0 [E8] 不证明 UI 响应。 | 同缺；[E11] Graph 探针没有交互桌面线程栈。 | 同缺；六次图执行 [E5] 不证明 WPF UI 响应。 |

## 可直接复用的审计证据（当前文件 SHA-256）

| 代号 | 路径 | SHA-256 / 用法 |
|---|---|---|
| J | `docs/joint-audit/20261020-audit-objectives-and-drift-controls.md` | `5C8A621D2B35CF2208D09DC0945B8AC499188D9C1E6BDAE0309BFC24F1945253`；J01–J12 的实际关闭条件。 |
| E1 | `docs/joint-audit/20261020-pass6-calibration-original-identity.md` | `1307138D4C2C4FAB204F49A1BA8F4762E91D94952ABDABAD0415B8D7132505CC`；十项原件静态身份。 |
| E2 | `docs/joint-audit/20261020-pass6-calibration-catalog-host.md` | `BF14589D29686DFA14C89F378C1496DDFA3BDB5C37966B66C70E819A33151E5C`；十项当前真实目录/注册/建节点。 |
| E3 | `docs/joint-audit/20261020-pass7-calibration-config-bridge.md` | `5F2CFF8E299479E2EAF11C29026263879AC003458CA68FA3EBA52F8704E87EF1`；共享开窗与两项代表性编辑，非四工具全 UI。 |
| E4 | `docs/joint-audit/20261020-pass6-calibration-result-chain.md` | `0726E64F5F0FDDAAE7C282966D6BE0ABF29C11641FE469E932441017A0D6DF05`；N点 XML→加载器→Group→坐标系。 |
| E5 | `docs/joint-audit/20261020-pass7-calibration-failure-chain.md` | `A10FECA18C4BF39D448945F880F89DDFD137F6E82CDB611DC4B7D2422F2B8B72`；刷新失败/清集合/热冷短矩阵。 |
| E6 | `docs/joint-audit/20261020-pass7-calibration-history-chain.md` | `349E0105F3626057073B80C014670586ADD4D0777D81889AA00425A3E55B5023`；Group Matrix 连接 Undo/Redo 后运行。 |
| E7 | `docs/joint-audit/20261020-pass8-translation-result-chain.md` | `0551C6A3B9F83ED4DF09B7F271B7058AAD0D8377229E5851354D918A85CAB81E`；第二个 XML 生产者正常闭环。 |
| E8 | `docs/joint-audit/20261020-pass9-rotate-consumer-chain.md` | `B69315F1F72DDD7755248D1729915303E4398F6A1EA298896957E2672D53D8A7`；相机文件→Rotate 连续采样/恢复。 |
| E9 | `docs/joint-audit/20261020-pass9-coordinate-point-chain.md` | `232C966DBDA8EA6A15705434B79F6F65152EE6A843F1D13F8380EEBE46927855`；Group POINT/Matrix 双模式。 |
| E10 | `docs/joint-audit/20261020-pass10-board-rotate-chain.md` | `B495DF0DFF88684FDDF902A8DC2CA2FE82350A8842DA3BC9F1544C328B6F01E5`；原 9 float 文件结构/当前板图采样恢复。 |
| E11 | `docs/joint-audit/20261020-pass11-coordinate-roi-chain.md` | `EBF0514D2EA1D8DDC18574A3E2CCB2DB5A4498AD89E9D44218FDD56F0E63E91D`；原 Draw/显示叶、当前 IMAGE/ROI/Undo。 |

原函数的完整路径及 SHA、原 XML 资源 SHA、测试源码及成功/失败日志 SHA 均在各 E 报告内，不在此复制未提交证据。交叉复核时应**先核报告 SHA，再按其指向的原体和日志核细枝**。`P-T` 为 `docs/plugins/TRANSLATION-IMPLEMENTATION-EXIT.md`，`P-C` 为 `docs/plugins/COORDINATE-MODULE-IMPLEMENTATION-MATRIX.md`；二者仅定位插件侧既有自审，未据此标联合门完成。

## 有终点的下一批最小审核

1. **J11 最短缺边：** 在 Rotate 六输出和 Coordinate `CorCalibMatrix`/六叶中各选真实现存消费者端口；先核原 XML 输出类型/当前端口类型，再作一个生产者→待审工具→真实消费者的当前图，含失败与重开。若无原版合法消费者，记录“原 XML 未定义可接边”而终止该支，不发明矩阵端口。Translation 与 ReadCalib 的已证正常链不再重跑。
2. **J04/J09 动态身份：** 对 ReadCalibFile `LoadFile` 成功后动态叶改名/改型、原 `VmIO.RelateIO`/通知与当前 `GraphExecutor`/Group 的已选订阅作一张三态图（连接前、改名、重开）。需要的是这条**特有**边，而不是重新做缺文件保旧。
3. **J03/J08 实际主窗：** 四工具各取一个特有配置动作与失败→切换节点显示动作。只读追原 AlgorithmTab/Display/XML 到 setter/输出，当前 MainWindow 开窗/关闭与显示 source snapshot 分段计数；共用宿主逻辑只实现/验证一次。
4. **J06/J10/J12 一次共用主框架诊断：** 取同一四工具组合图，在受控交互桌面或明确标记的离屏宿主中记录 UI 心跳、配置/运行/关闭分段、线程 ID；若重现未响应必须采 UI 线程栈/等待链，未重现只关闭该次条件。Stop/切方案/旧 scope 回调用同图的 owner epoch 核。没有线程栈不得称已找到卡死根因。

上述四步并不自动要求设备、原厂平台整图或把未初始化 native 堆值当契约；若某门需这类证据，明确维持未关并给用户可选择的解除条件。当前主框架 41/96 实施指标与本次四工具联合门独立；本只读对账不改变分子。人工验收始终另列。
