# 灰度匹配模型生命周期及归一化续片

2026-10-20。主模块同hash b7327ffa…当前49个导出C，索引SHA256 `445ae62ebcfed9e61f4a5c26a3c1f30a7b2137a4202bb89fb6c30d5d48229a89`；核心85d17ecc…当前31个导出C，索引 `94aba4c509d9cd54acbae43211361e75182ae24d9678bce6d75eaadb1196e558`。计数仅供索引，不是完整源码/整工具进度；.pdata分片限制保留。

## 模型生命周期具体补体

- manager构造73d0：写moduleId，初始化critical section，**chosen+D9F0=-1**，清槽/count、pendingName和chosen文本；修正候选先前未经证明的默认0。Clear8ed0只清槽/count范围，保chosen及pendingName，不能按Reset意图清整对象。
- Rename8be0只清并写所选260字节Name；Up9c00/Down9130查前/后最近buffer非空槽，9d80交换完整0x228槽，不只是相邻名称。
- 原生100槽与UI上限不同：继承的FeaturepatMatchConfigPanalDataDecorate.MaxModelNum=36仅限制正常Add/Load，受IsRunning忙门保护。配置恢复GetModuleList并不按36截断，它逐CandidateName读到空/错误为止，然后读BasicData/Selected；已有模型缺UI数据仍以IsCorrect=false入列表。不得用UI36限制原生存量恢复。
- UI上移/下移先成功调用native，再移动IList、交换五组UserData并调整Id/ImageKey；源码交换/删除未清PointsNums，不能未经说明实施“顺便清理”并称源等价。
- GetNext7f40找首buffer空槽、写260字节临时文本、outLength不含null；满100时返回0且不写out/length，typed API须显式“无可用值”，不能造100。
- CandidateName7790：index<100；有buffer才复制260字节Name，outLength=strlen+1；空槽返回0、清260字节及length0。正常调用依赖足够原caller容量，不仿未界定小缓冲ABI。
- CandidateData76f0：buffer空E0000001；先写所需byte长度，容量不足E0000012；成功原byte拷贝。index>=100原体返回0不写out，typed安全表面显式拒绝，不能把未写状态填空数组冒等价。
- Selected7de0检查index<count；有buffer时格式化0/1，实际只复制1byte，再strlen得outLength（原caller需初始化终止区）；空buffer首byte0/length0。此处与CandidateName的null长度规则不同，不能统一GetString计数。

候选 `analysis/gray-match/proposals/GrayMatchModelSlots.cs` 已含锁、深复制、count/chosen/active/rename/扫描换位/压紧删除/typed getter；正常byte-Add模型域。原危险index100、负容量、超大/未终止Name及空buffer的名字状态等边界明确拒绝，不模拟越界。尚未落生产或运行ModelSlotsChecks；UI36/列表UserData/文件格式消费者仍待接。

## 真正标量归一化，已取源码与原生oracle

c9ace0精确顺序：sum乘自身、除signed int32 count、由sumSquares减；没有Count0检查。c9ad00：先MULSD correctionFactor*sum、由productSum减；COMISD **zero,numerator**后JNC，ordered numerator<=0返回原常量，unordered进入计算；计算numerator²/(denominatorFactor*variance+常量)。原PE1816d0200 raw `2d431cebe2361a3f` 为 **double0.0001**，不能猜机器epsilon、0分或常规NCC归一化。

`tests/GrayMatch.NativeProbe/NormalizationProbe.cpp` 的已核ABI分别为double(double,double,int32)及double(5个double)，第五double在rsp+28，返回XMM0。同hash纯标量11case（5variance、6score）build/run0，包括正/负/零差、零/负count、NaN及乘减舍入；另读取加载后原常量bit。测试无CreateHandle/Platform Init/设备/授权动作，flush后self退出避开独立DLL卸载链。

证据 `analysis/gray-match/normalization-native/manifest.json` SHA256 `66977c30406e7deab681ef92763a8c6f6e6c43321398e7351b3efe16aaac5cc7`，含原件/C/asm/probe/oracle与候选hash。NaN score原生输出7ff8000000000000，证明不能直接照伪C `0<n`返回floor。候选 `GrayMatchScoreArithmetic.cs` 与严格12项入口已备，未生产/未managed运行。零count/NaN是安全标量叶边界，不声称正常有效模型上游可达。

## 整工具当前实施退出矩阵

| 必需链 | 当前实际状态 | 退出条件 |
|---|---|---|
| 身份/原资源/字段 | 原DLL/CS/Control/33默认/12显示库存，逐setter到局部/SDK映射 | 后续新体继续核具体参数/模型trigger |
| 限值 | 生产LimitPolicy，真实DLL13项通过 | 接入结果集合/原序/Frame错误状态后复验 |
| 点积 | 生产DotKernel，原生20case/60bit及生产65项通过 | 接入实际1390行布局及分派，不称纯点积即匹配 |
| 模型槽/查询 | 必需正常生命周期体已取，typed候选/测试备好 | 根线审、生产接入及UI36/metadata消费者验证 |
| 标量归一化 | 原生11case及PE/asm确认，候选12项未运行 | 源审落生产并原位对照，随后接实际map/peak精度阶段 |
| 当前1390训练/搜索 | wrapper/core大体已取，确定1390/CPU/dot/归一化/候选选择链 | 缺模型/图像布局、训练分配与pyramid/采样callee、搜索编排逐操作移植 |
| 文件/配置模型 | CreateGrayPatEVA与正常Save/Load及UserData顺序已恢复 | WriteLibFile封装、必要decoder、原控件/operation/触发/失败副作用实施 |
| ROI/fixture/Frame | 模块ROI非空首项、SDK类型/角门、Mask setter边界已取；XML ROISelection=Box | 复用同身份Frame具体预处理/本轮空ROI保留与输入错误门，不能按意图扩大Shape |
| 输出/显示/缓存 | 模块ProcessOutput与Display12映射可追，当前未产品连接 | 原数组/序/状态关联、输出失败门、scope及显示保持/释放整体自审 |
| 实际Host | 未注册/未启动 | 上述正常后端完成后由根线集成，真实配置/训练/图像运行/显示/保存重开/释放验证 |

完整工具仍未完成，人工验收另列，Blob独立保持库存后排。没有用库存、测试数或数值叶替代整工具完成。
# Production model-slot and scalar-normalization validation update

Root-approved candidates are now production `GrayMatchModelSlots.cs` and `GrayMatchScoreArithmetic.cs`; both test projects consume ProjectReference, with no local candidate Compile shadow. Final production results: ModelSlots125 (including explicit non-NUL260 safety exception and original empty-slot Delete decrement), normalization12 strict same-hash oracle double bits, Dot65 and source-policy13 regressions. Final builds all zero warnings/errors and runs exit0. Manifest `analysis/gray-match/model-score-managed/manifest.json` SHA103A0FCDAB422655A4493643D14C45B6BD5D25D348F6D10039479D4582D433F5 contains native-oracle identity, production/test DLL/PDBs, source snapshot/diff and raw logs.

Preserved failures: conditional tuple literal required `0u` for production compilation; missing normalization oracle argument; the new unterminated-name test initially expected a code rather than the explicitly designed NotSupportedException; rebuilding while that own crashed apphost remained caused file-lock failure. Only two precisely verified own test apphost PIDs were ended under root authorization. Final tests retain the safety boundary rather than alter it. Source delta after the initial snapshot is solely the tuple uint literal correction. Source SHA ModelSlots6933CA8E60DB21954453C4868EDAEACD6C4C9656B9BB5812AAD0BDB68DDE2DE7; Score4B42FFDEA58AEEA4820D0A837C3F1424D98E94718243B3BA0382C27F11C7D007.

These are leaves/state infrastructure, not complete training/search or a registered GrayMatch plugin. Shared model renderer proposal is separately documented in `GRAY-MATCH-SHARED-MODEL-RENDERER-CONTRACT.md`; actual map double-to-float storage and single-precision square-root candidate boundary are in `GRAY-MATCH-SCORE-MAP-PRECISION-SOURCE.md`. Earlier candidate/not-run statements below are historical and superseded only for these two production components.

