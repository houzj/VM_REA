# D1 映射标定 proposal → decoder → payload 源码链

2026-10-19 decoder后续切片已补8个实际纠错叶体并核payload/方向/原点链；返回码按asm纠正：null是0x10000014、非法18/24参数范围是0x10000019，旧分类已替换。最新产品差异/有限退出见 [MAP-CALIBRATION-D1-DECODER-OUTPUT-SOURCE.md](MAP-CALIBRATION-D1-DECODER-OUTPUT-SOURCE.md)；下文初始缺体清单为历史断点，已取叶体不再重复扩。

后续进展：[D1第二切片](MAP-CALIBRATION-D1-PREPROCESS-FEATURE-SOURCE.md)已补本页当时缺失的预处理与局部特征子体，并核实CPU分派、rounding保存/恢复、参数校验与输出失败链。本页23函数/18新体数字保留为首切片快照；当前机器索引已增长，且显式区分Ghidra拆分代码块，不能按入口数推算法完成率。

2026-10-19。阶段：S1 已补取 18 个函数体；S2 已追主干的具体读写和分支，下级缺口仍明确列出；没有进入 S4 产品算法重建或 S5 宿主验收。

## 任务、证据身份与获取方式

问题：真实 `813ca0→813020` 之间，opening 如何提案、以何顺序尝试，何处写 decoder status/payload/orientation/anchor，caller 怎样生成世界原点？

原厂目标 `D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll`，SHA-256 `85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549`。复用本工作树 `analysis/map-calibration/ghidra/core/` 的五份旧导出：80e690、80bcd0、813ca0、80a7f0、813020。旧导出无 REA Evidence ID，不补造。

本轮先调用 REA `binary_session`：open=false，Ghidra provider 为 not_configured（GHIDRA_INSTALL_DIR 未设、该 MCP 进程的 Java runtime_missing）。根据根线明确安排，采用现有私有 Ghidra 12.1.4 工程 readOnly/noanalysis 的定向导出 fallback，没有更改全局配置、其他工作树或活动 REA target。新原始 C、完整函数汇编及 cookie 后尾指令分别保存于 `analysis/map-calibration/d1-source-chain/source/`。它们是普通 Ghidra CLI 原始证据，不称作 REA 取证成功。

机器索引 `analysis/map-calibration/d1-source-chain/source-index.json` 保存全部 23 个函数的来源、C/汇编/tail SHA、直接 callee、关键源码行和未进入本索引的 callee。另保存原始 PE 常量字节和文件偏移，导出脚本验证目标 SHA 后才读取。原始 C 文本只作证据，自动类型和签名须服从以下汇编核查。

## 源码覆盖表

| 操作/分支 | 原厂成员/地址 | 输入→输出及谓词 | 产品位置/状态 |
|---|---|---|---|
| 两板型入口与 caller | 80e690 / 80bcd0 | 候选及拟合→813ca0；仅成功 status=1 才解释 payload 并调用813020 | Map Detect→共享 Recognize；旧导出复用，方向1运行已补证 |
| 空洞 proposal | 813ca0 | 格网任一整数 INT_MIN 计为缺槽；中心槽排除；radius1/2分别要求至少6/17缺槽才投影四角 | 共享 CodeQuadrilaterals 仍独立；主干源码已追 |
| 四角投影 | 800670 | float点、9 float矩阵→两 float；标量 MULSS/ADDSS/DIVSS 无分母异常检查 | 原厂微核源码/汇编已闭合；不代表整条opening已实现 |
| 四向再镜像尝试 | 813ca0 两处80a7f0调用 | 正序循环移位1..4→反序循环移位5..8；成功停止、输出radius与anchor | 共享decoder当前仅接受orientation0；主干源码已追，实图仅方向1 |
| 局部特征提取 | 80a7f0→80ac20→870b90 | 200×200变换提取4个局部标记；提取失败进入20×20固定fallback | 共享NImageFiducials为独立实现；已取主函数体，下级仍待追 |
| 特征到opening坐标 | 80a7f0→7ff3c0 | 4对点拟合失败则status+f0=0；成功投影内部四角，调用86ff00 | 既有拟合核不是新改动；该caller组装源码已追 |
| decoder采样 | 86ff00→8b1710→89faa0 | 最小边尺度满足条件才warp；warp状态1才做图像处理与192位采样 | 独立decoder不能据当前点集等价宣称采样等价；主体已取待细追 |
| 二值图处理 | 85c8a0→8a48e0/8a5e30/8a8040 | workspace及图像→处理图；85c840计算其workspace | 只取wrapper，三个下级函数体未纳入本轮；精确阈值算法未知 |
| 字节及纠错 | 8b0260→8b11a0/8b0670；932480/932370/9322a0 | 192位打包24字节；18个校验结果全0直拷，非0进入定位/修正分支 | 已补体并追顶层，有限域下级尚未全部取体/核汇编 |
| payload写回 | 86ff00 | decoder返回0→workspace d15c..d130的12整数低字节反序写output11c..127，status f0=1、118计数加1 | 共享Identity只消费前6字节；具体写链源码已追 |
| 方向与锚定 | 813ca0→两caller→813020 | 成功尝试号决定1..8；radius+中心索引先写search anchor，caller依方向调整 | 微核8向已补证，真实caller仅方向1；其他实图方向未验证 |

没有从表中的“已取体”推出所有分支源码闭合；下一节列出字段和可确认的条件，最后一节列出可继续检索的精确地址。

## proposal 与 orientation 的具体赋值

813ca0 的 payload 输出结构初始化后，状态读自 `output+0xf0`。radius 从1增至2；每个窗口统计格网第一或第二整数为 INT_MIN 的槽，中心槽不计。常量 `181642ef8=0.6665999889373779f`、`181642e94=0.5f`，门槛为 `(int)((2*r+1)^2 * factor + 0.5f)`，实数值为6、17。汇编 `18081400b CMP ECX,EBX; JL ...` 确认缺槽数 **达到或超过** 门槛才继续，不是“有效角点越多越好”。候选遍历须按原始循环恢复，尤其 radius2 的列起点、负索引与工作区padding关系尚未完成上游边界核查，不能自行正规化为常见滑窗。

通过门槛后，将四个格网角先转float，再由 800670 以caller拟合矩阵投影，保存四角 proposal。此函数新增的反编译和汇编明确第三个 R8 参数是输出点；旧813ca0伪代码只显示两个参数，不能照其签名遗漏隐式活跃的R8。

proposal `Q0..Q3` 首先以 `[Qk,Q(k+1),Q(k+2),Q(k+3)] mod4` 调80a7f0四次；若都未成功，才以 `[Qk,Q(k+3),Q(k+2),Q(k+1)] mod4` 再调四次。向80a7f0传的是减去图像局部原点后的坐标。每次前检查 `output+0xf0==1`，成功即离开余下尝试；第一组写attempt编号1..4，第二组5..8。方向来自 **哪个排列成功**，不是从四角几何最近邻推算，也不是公共 Homography freedom。

成功还写 `radius` 及 `searchAnchor=(candidateColumn+radius,candidateRow+radius)`。caller按方向对世界锚点调整：

| 方向 | anchorColumn | anchorRow |
|---|---|---|
| 1/5 | offsetColumn+searchColumn−2r | offsetRow+searchRow−2r |
| 2/6 | offsetColumn+searchColumn | offsetRow+searchRow−2r |
| 3/7 | offsetColumn+searchColumn | offsetRow+searchRow |
| 4/8 | offsetColumn+searchColumn−2r | offsetRow+searchRow |

参数语义纠正：813ca0 **param5是offsetRow，param6是offsetColumn**。依据四次投影前float的组装及800670的RCX/RDX/R8汇编；前轮header曾反标，两者实测恰为1，无法发现对调。旧trace完整保留，比较器显式翻译旧标签，新trace增加source-confirmed标记并正确命名；原10714字段仍通过。没有用对称样本“补证”新的非对称ABI。

## 80a7f0 的成功、失败和decoder入口

80a7f0先调用80ac20，第四参数打包为200×200。80ac20体存在类型传播警告，伪代码声明void，但汇编caller `18080a931 CALL; 18080a936 TEST EAX,EAX` 和其cookie尾分别明确返回EAX0或1；因此“void函数无返回状态”是错误结论。不能仅看反编译显示签名。

提取失败时回到 canonical 外四角 `(0,0),(20,0),(20,20),(0,20)` 和内四点 `(4,4),(16,4),(12,16),(4,16)`；提取成功时两套采样结果除以10（181642d90四个float均10）。随后4对外四角调用7ff3c0；返回非1清 `output+f0` 并退出。返回1才将内四点投影到局部图像坐标，构造单item输入结构交86ff00。

80ac20内部可定位链为：7ff3c0四角拟合→730e90变换求解→图像重采样→逐局部窗口870b90；局部结果需过其数值区间条件，收齐4点才返回1。870b90又调用6eea70与6a3a70。这两个下级尚未取体，因此不把“特征提取”进一步命名为某个原厂圆/环算法，也不以当前独立fiducial规则填空。

## 86ff00 到 payload 的具体谓词与写入

根据四边长度，采样尺度取 `min(L01/12,L03/12,L23*0.125,L21/12.65)`；常量为PE原字节确认。尺度至少2才继续，>=8取8，否则取整数截断m；warp画布边长16m。canonical四点为 `(2m,2m),(14m,2m),(10m,14m),(2m,14m)`。warp helper 8b1710返回状态1才走后续；越界输出像素写0，坐标经800670后以floor/ceil构造双线性采样。ceilf自动参数显示不完整和整数坐标零分母边界还需汇编，未宣称重采样全部已闭合。

处理图经过85c8a0。86ff00源码用固定表产生192个格位采样，取局部平均，以 `153.0f < average` 的布尔值，异或格位奇偶再异或1，写 workspace+ce00 的192个整数位。8b0260以18、24进入；每8位按128..1打包一字节，共24字节。其顶层先在GF(256)、约简多项式0x11d的运算上生成18个校验结果；全0直接复制24整数，非0进入8b11a0/8b0670分支。这里是源码可见的域运算和分支，不以熟悉的算法名称替代尚未逐条核对的纠错实现。

8b0260伪代码也误显示void；cookie尾明确成功0、空指针0x10000014、参数范围0x10000019、其他分支透传错误。86ff00汇编 `180870843 CALL; TEST EAX; JNZ ...`，只在返回0时把 workspace `d15c,d158,...,d130` 的12个整数低字节依次复制到output `11c..127`。同时投影中心/方向向量，写status+f0=1，output+118计数加1；失败置对应status0。两caller目前仅解释前6字节，HIK7为直接origin字节，HIK9按位重组，继而813020。这个链比“解出六字节就算成功”更严格。

生命周期：86ff00对每个item清理自有workspace，并写输出状态及count；932480还写进程全局表182fcc0c0/182fcc4c0。并发/可重入语义尚未核查，不能把当前独立纯函数实现的线程语义当作原厂证据。813ca0只读整数格网做空洞判定，方向1旧trace确认search/map间格网不变；更早的哨兵生成属于873de0候选/格网链，不能归因于解码成功后mask所有opening。

## 精确缺口、产品边界和下一步

1. **尚未取体：** 80ac20下级730e90、870b90下级6eea70/6a3a70；85c8a0下级8a48e0/8a5e30/8a8040；纠错下级931cd0/931dd0/932120/932570/932440/932080/931c60/85aee0；89faa0下级8a0650等。机器索引按具体函数列出callee未进入本D1索引，不把“未入索引”谎称全仓不存在。
2. **已取待细追/核汇编：** 80ac20局部窗口坐标/循环和4点拟合半径（第四切片已确认，接受区间[15,21]）、86ff00采样格位表顺序、8b1710的ceil边界/浮点顺序、89faa0拟合标志、8b11a0/8b0670的各错误分支与输出反序。缺体先定向补取，不按标准算法猜实现。
3. **ABI未尽：** proposal radius2的padding边界与上游分配；各wrapper保留的寄存器/栈参数；新得到的3个void误判说明必须结合cookie尾恢复返回值。仅当某一具体疑点静态仍不能判定时才设计非对称参数probe。
4. **运行范围：** 复用前轮真实caller8次10714字段，仅方向1。其他方向的proposal排列和成功条件虽已有主干源码，仍无方向2..8真实图像补证；不把前轮微核320例作为八向识别域验证。
5. **实现/宿主：** Map调用SourceLatticeAdapter后仍消费共享独立opening/fiducial/decoder，并保留projective/rotated兼容fallback。公共NImage唯一修改者为根线，本轮没有改算法、接口、持久化，也没有启动新宿主验收。没有提交/推送或跨工作树同步。

下一切片先取85c8a0三个子体与730e90/6eea70/6a3a70，闭合“局部特征如何得到四点”和“192位之前处理图怎样生成”；然后逐条核验纠错支路返回/输出。当前交付为源码索引、字段/分支链和具体取体任务，根线审查后通知“框架层逆向”，不把其写成已实现待验。

## 复现

```powershell
& tests/MapCalibration.NativeProbe/RunD1SourceExport.ps1
& tests/MapCalibration.NativeProbe/RunD1SourceExport.ps1 -Addresses @('1808b0260','180870b90','1808b1710','18085c8a0','18085aff0','18085c840','1808b1690')
& tests/MapCalibration.NativeProbe/RunD1SourceExport.ps1 -Addresses @('1808b11a0','1808b0670','180932370','1809322a0','180932480','18089faa0','180730e60','180800420')
& "$env:USERPROFILE/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe" tests/MapCalibration.NativeProbe/ExportD1SourceIndex.py
```

这些命令依赖本机已有私有Ghidra工程和原厂SDK；导出/报告写当前工作树专属analysis。取体成功与源码闭合、运行补证、实现及人工验收分别记录，不用单一测试数替代阶段。

