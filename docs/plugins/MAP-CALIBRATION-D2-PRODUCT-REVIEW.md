# D2共享产品只读映射与最小差异请求

2026-10-19。根线已审查D2原卡并逐项验证14必需C体hash，批准S1/S2。专属阶段表已更新D2=S0/S1/S2、D组4/32（12.5%）、整卡0/4；S3–S7仍开放。本轮是S4前置只读审查，没有修改公共NImage/grid产品、运行托管构建/测试或执行负索引输入。

## 审查快照与标签

`ExportD2ProductReviewIndex.py`保存12个当前产品文件hash/行锚点至`analysis/map-calibration/d2-product-review/product-index.json`，避免用之后工作树变化冒充此刻审查。原件哈希仍`85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549`，D2原生14体索引见退出审计。

下表“源确认”指原始赋值与条件已确认；“安全选择”指托管明确避免原生跨区域写/非法索引，不据此称位等价；“需S3”指运行上下文或特定输入影响未验证。部分产品已有源相位/严格有限fixture通过记录，本轮未重新执行，不能据静态审查刷新通过数。

## 逐项文件/方法映射

| D2行为 | 当前产品文件/方法（src/VisionPlugins.Calibration） | 判断 |
|---|---|---|
| seed索引90000槽−2，Count/dims失败保留 | NImageSourceGridSeeds.cs：Create3x3/Create2x2、NImageSourceGridState | 源确认：入口−2、内部失败矩阵清0/dims0、早退Count保留已有对应 |
| 新边−1、score最大值、四方向 | NImageSourceGridPadding.cs：AddBorder | 源确认：Occupied排除−1/−2、边界扫描、移动和新增边赋值有对应 |
| 目标/候选互斥 | NImageSourceGridAssignment.cs：Assign | 源确认：strict <选择、整行/整列失效、assigned/used写回有对应 |
| 扩边返回0/1/2及direction-- | NImageSourceGridExpansion.cs：Expand；GridSelection.cs：Find | 源确认：孔洞/新边/无进展与caller重试有对应 |
| 剪枝Count/flags/index写序 | NImageSourceGridPruning.cs：Prune | index>=0同时门控Count--与used清理；原厂只门控Count--。这是安全选择，见S01 |
| seed失败九槽flags清理 | NImageSourceGridSelection.cs：Find/ClearSelected | index>=0过滤跨区写；源无该过滤。安全选择，不复制UB |
| seed平分后替换、seed跳步/早停 | NImageSourceGridSelection.cs：Find | 源确认：score<=bestScore、fallback75%、>1000、count>9跳步等对应 |
| 跨尺度/两假设平分、scale/完整矩形退出 | NImageSourceGridPyramid.cs：Find | 源确认：first新平分优先、历史平分保留，source XY乘scale及退出条件对应 |
| 第一假设评分读取尾槽 | NImageSourceGridPyramid.cs：AdjustedScore/firstOutputSlots | 显式托管零初始化/保留数组尾槽代替原始workspace上下文；安全选择+需S3，见S02 |
| 两wrapper最终双int INT_MIN dense grid | NImageSourceLatticeAdapter.cs：Find，仅替换Points | 产品载荷只有有效稀疏点，没有原厂dense双int/尺寸/前缀状态。不是“漏Array.Fill”小修；属于R02/D1接口依赖 |
| 同ID槽后写XY覆盖 | NImageSourceGridSelection.Find保证按cell唯一输出；RecognizeLattice可接受任意Points | 正常source生产链唯一；外部重复载荷未做同槽覆盖，边界差异见R02 |
| decoder首成功立即停止 | NImageDetectedBoard.cs：Recognize | 首identity作为锚定，但foreach仍扫描所有quad，收集全部tags并以tags.Any过滤点；明确执行差异R01 |
| 813ca0 r1/r2/缺槽门/8排列 | NImageSourceLatticeAdapter.Find→legacy FindLattice提供CodeQuadrilaterals；NImageCodeDecoder.TryDecode | 原厂proposal由dense缺槽搜索产生，产品仍独立几何；decoder内部虽循环镜像/旋转，Recognize只接受Orientation0。D1依赖，不能据D2源码开八向 |

## 最小共享请求R01：把选锚定的首成功边界明确落到执行

**源确认差异：** 原厂813ca0第一次output+f0==1后不再尝试后续proposal；当前Recognize在foreach中不会break，返回Tags虽只含first，后续成功opening仍参与points的Inside排除。首identity相同不能证明执行/点过滤一致。

**最小目标：** 共享识别层应有明确“选中锚定tag”阶段，首有效identity选中后停止选锚定扫描；若当前兼容路线仍依赖多opening掩码，其集合需作为独立兼容输入/诊断阶段显式传递，不能冒称原厂selected tag集合。禁止简单把tags改一项却静默改变全部历史点过滤。

Producer为共享NImageBoardRecognition；Consumer为MapCalibrationImageSolver.DetectImage及其他RecognizeLattice调用者。payload仍可保留现有NImageDetectedBoard/单selected tag；多opening补偿不混入selected identity。生命周期纯一次识别调用，无持久化/宿主契约变更。

**验收优先复用：** DetectorBoundaryChecks已有真实normal/reverse quad序列和各首identity；image-execution-source-grid已有源/目标168点及顺序。先比较“全部proposal”和“截至首成功前缀”运行结果、实际处理proposal数及过滤点差异，再由共享所有者按D1证据决定保留显式compat mask的位置。本轮只提出请求，未运行/改动。

## 最小共享请求R02：保留原厂grid上下文，避免在识别边界重猜

**源确认差异：** NImageSourceLatticeAdapter.Find先要求独立geometry非null，再调用source grid；最终仅用grid.Points覆盖legacy lattice.Points，仍借legacy Pitch/CodeQuadrilaterals。NImageLattice无source Width/Height、整数min offset和dense缺槽上下文；Recognize还通过另一次拟合/Nearest计算anchor，并按Inside和WorldIndex排序。原厂wrapper先建dense ID哨兵并按输入顺序覆盖，再由813ca0选中位置/attempt直接写anchor。

**最小目标：** 后续D1原厂proposal接入所用内部载荷需显式保留source格宽高、有效ID/XY的原顺序及min offset；dense builder按双int INT_MIN初始化、同ID后写覆盖，float缺槽的未知值不伪造原厂初值。正常source producer已有唯一cell，不需要在每个消费层重复去重。外部RecognizeLattice重复载荷须明确是否走此source-grid适配，不能默认独立稀疏list等同原厂wrapper。

这是给共享所有者的内部载荷/阶段请求，现阶段不要求修改host Contracts，也不要求删除legacy fallback。radius2负列只能由明确安全策略处理，禁止借dense builder引入负数组访问。是否需要相邻保留段上下文属于S3，不从零数组推原厂。

**验收优先复用：** GridSelectionChecks/原厂完整工作区含已有−1/−2/孔洞、ordered cell；GridPadding/Assignment/Expansion/PruningChecks包含实际各方向输入。正常grid结果和legacy兼容结果分开对照。人工构造重复ID只作为一个明确边界用例，不能拿模拟输出代替原厂重复输入golden；需要原厂补证时另给安全入口，不触发未知负写。

## 保持的独立安全选择（不是要求恢复原生UB）

**S01 负flags写被丢弃。** 当前Find/ClearSelected/Prune对index>=0才写used，保护托管数组。原厂F−8/F−4可能修改邻接第二假设ID保留段；两者不应宣称任意workspace同位。请求仅为共享接口/证据记录中持续明确该安全选择；在未有必要S3前不增加“模拟prefix写”的产品机制，也不要求关闭过滤。既有GridSeed/Pruning/SelectionChecks能验证合法域逻辑状态，不能验证相邻原生段副作用。

**S02 尾槽上下文。** Pyramid的firstOutputSlots明确new float[]，初值0；原厂对应保留槽值由SDK工作区决定。旧四图真实尾槽均0，只支持这些fixture。最小后续为复用现有grid-trace尾槽证据核对映射；若要提供显式context应由算法内部对象承担，无任意workspace残留产品参数，未有源证据不要硬编码历史字节。

**S03 容量与取消。** GridSelection拒绝候选>90000、数组长度/索引检查和CancellationToken是独立安全合同；原厂flags保留可随N加大但indices固定300×300，不据此直接扩大产品支持。此类差异不阻塞D2源码退出，不在本轮申请恢复原生未定义写。

## 不提出无证据产品修复

assignment一对一、seed平分、跨尺度平分和padding−1的主干已有直接对应；本轮没有发现足以要求公共补丁的已验证数值反例。保留现有合法域实现，优先验证R01/R02阶段边界。D1的8向真实decoder、量化/float运行补证及旧兼容fallback不得包装为D2已完成产品等价。

本轮只读12文件快照、文档映射和差异请求完成，未实现R01/R02或重跑已有测试，故不计D2 S4完成，也不计S3/S5/S6/S7。专属阶段表保持根线批准后的4/32、整卡0/4。
