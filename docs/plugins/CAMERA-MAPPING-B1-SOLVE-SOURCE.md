# B1 相机映射归一化与加权首分叉（源码审查切片）

2026-10-19。S1/S2已回收实际CameraMap求解路径，S3对上轮一个4点和一个16点样本定位首分叉，随后经根线审查实施专属S4最小修复并执行S5。工作目录 `analysis/camera-mapping/b1-solve-source-20261019/`。机器源码索引与前后验证见 `CAMERA-MAPPING-B1-SOURCE-INDEX.json`。

## 实际调用链与源码索引

原厂核心 `MVD_Algorithm.dll` SHA256 `85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549`；模块和包装DLL身份沿用第二轮真实可达性记录。原厂入口不是仅凭名称推断：

`CIMVSCameraMapModu::ProcessAlg@bd20 → CameraMap wrapper Run@2620 → MVD_Algorithm_Process@152b0 (id0x66) → core handle vtable+0x18@1c4e0 → 1df30 → 7c6ba0(op0xb) → 7f6330 → 7d2ea0 → 7ff3c0`。

`152b0` 从真实handle的vtable+0x18调用；既有同版SDK探针 stderr直接记录此槽RVA=1c4e0。`1c4e0`的0x66分支调用1df30；1df30调用7c6ba0的op0xb，后者对应7f6330，继续调用7d2ea0，再调用7ff3c0。不能把MatrixToParam的0x6b/0x12路径混入CameraMap的0x66/op0xb。

| 源码/RVA | 数据和作用 | 状态/出处 |
|---|---|---|
| 模块bd20、SDK wrapper2620 | 清BasicParam、按缓存点顺序追加、Run与结果交接 | 复用既有CLI导出，完整路径/哈希归档 |
| 152b0、1c4e0、1df30、7c6ba0、7f6330、7d2ea0 | export/virtual/id分发，options与实际callee | 复用并核对；本轮read-only Ghidra再导出 |
| 7ff3c0、800170 | float质心/平均距离尺度，源/目标归一化和设计A/b | 本轮源码+汇编；原厂hardware入口补证 |
| 7fcf60、8580e0、7075e0、730e90 | 初始拟合、法方程/求逆、首次反归一化 | 本轮源码+汇编；初始解逐位比较 |
| 7fd4e0、7fec40、730be0 | 再归一化、逆投影残差与sigma、加权法方程和停止 | 本轮源码+汇编与定点入口/返回 |

新的 `.c`、`-instructions.tsv`、`-callers.tsv` 位于专属目录 `source/`。Ghidra 12.1.4只读复用当前工作树已存在的core工程，禁用重新分析；没有改全局配置或切换共享REA活动目标。REA的只读session检查显示未开目标且provider缺配置，故本轮新增原生源码/汇编明确标CLI取得，不冒充REA Evidence ID。730be0旧数据库Function Body只拥有入口块，另用PseudoDisassembler无写入线性解码已知代码区间得到 `180730be0-linear-instructions.tsv`，不能把15条入口指令误当函数全部。

## 原厂矩阵运算顺序

记源归一化矩阵Ns、目标归一化矩阵Nt，首次线性解Hn，`F(X)`为float矩阵相乘、`N(H)`为先计算float `r=1/H[8]`再逐元素float乘r。

1. 7ff3c0构建Ns/Nt与归一化点；800170建A/b，7fcf60调用8580e0→7075e0得到Hn。
2. **7fcf60先反归一化**：`H0=N(F(F(inv(Nt), Hn), Ns))`，把H0写回caller输出。
3. **7fd4e0再归一化**：`H1=N(F(F(Nt, H0), inv(Ns)))`，然后以H1进入7fec40残差与权重迭代。
4. 迭代后7fd4e0再按`N(F(F(inv(Nt), Hupdated), Ns))`输出。

这些运算不是可任意省掉的代数恒等变换。修前产品 `CameraMappingSolver.FitNormalized` 初始FitLinear后直接保留Hn进入权重循环，省掉了2/3的float roundtrip；最终归一化还使用逐元素除法。源码确认实际有该往返，但最终偏移原因不能只凭这段缺失判定。

汇编锚点（均为核心VA，减imagebase得到RVA）：

- `1807fd3cb/1807fd409` 是7fcf60两次730be0调用；`1807fd43a DIVSS`计算一次倒数，`1807fd450/453 MULPS`及`1807fd464 MULSS`消费该倒数。RCX/R9及栈输出指针在调用前逐项设置，已核对完整x64 ABI，未按反编译缺省实参臆断。
- `1807fd8af/1807fd926` 是7fd4e0开头两次矩阵乘法；`1807fd977 DIVSS`后`1807fd98e/991 MULPS`是再次归一化。
- `1807fe936/1807fe975` 是7fd4e0末尾反归一化乘法；`1807fe9a5 DIVSS`后续再乘。
- 730be0线性解码中`180730c88 MULSS → c97 ADDSS → c9b MULSS → c9f ADDSS`，后续第三项继续单精度乘加；不是融合FMA，也不是double累加后再转float。

## 两个既有样本的首次观察分叉

只用第二轮原有float点集，默认固定Data1/Data2各承载半数点，权重参数仍3/20。新增探针以线程局部硬件执行断点采集7ff3c0、800170、7075e0、7fd4e0，以及重用槽位进入7fec40及返回；没有修改原DLL字节、原厂算法状态或其他任务进程。

两例的归一化源/目标点、初始A/b、初始线性解均float位相同。初始线性解之后，**第一次7fec40入口**已观察到系数不同：

| 样本 | 第一个不同系数 | 原厂 | 产品进入残差的Hn |
|---|---|---:|---:|
| 4点 seed-15-extent-10000-grid-2 | H[2] | 0.005576195195317268 | 0.005575415212661028 |
| 16点 seed-15-extent-10-grid-4 | H[0] | 0.5188503265380859 | 0.5188505053520203 |

16点H[2]进一步为原厂`-0.0002052605850622058`、产品`0.0003211253497283906`。当前已单独按回收源码顺序写一个**独立roundtrip诊断**，只接收相同初始解与Ns/Nt，不改产品求解器；它生成的H0与原厂7fd4e0入口9float、H1与原厂7fec40入口8float，在4点/16点两例均**逐位全同**。结果见`roundtrip-diagnostic.json`。这把最早观察到的系数分叉定位到产品省略的往返转换，不只是终值相关性。

16点第一轮原厂sigma`0.0018035866087302566`，产品cutoff`0.0012525965`；二者不是同一量纲，原厂还会把sigma乘options中的factor=2。随后加权法方程A有46/64项不同，首项原厂`6.808017253875732`、产品`6.6218485832214355`；rhs有8/8项不同。原厂执行两次加权线性求解，产品一次。

4点由于原厂把weight强制1，残差入口虽已分叉，两个线性输入/输出仍逐位相同。**不能据此宣称roundtrip造成该例最终偏移差**；最终反归一化、倒数乘法与MatrixToParam还应分开继续追踪。

## 待根线审查的最小修复方案

先只审查和实现7fcf60→7fd4e0这段明确缺失的转换，显式保留float矩阵乘法次序、float倒数后逐元素乘法，以及每次N(H)后的舍入；保留失败/零分母分支并按原厂源码核对，不用double代数化简。让诊断的H0/H1与已捕获两例位同作为首个小验收点，再核对残差、sigma、权重法方程和迭代次数，然后重跑原24例固定0.003及既有118例。

以上是实施前提交根线的方案；其后经批准实施，实际结果见末节，不预报24例全过。7fec40自身逆矩阵/残差数值顺序、最终归一化及MatrixToParam仍可能保留独立差异。公共NPointSolver只由根线审查处理；本线不修改公共核。提交方案时尚未重跑产品；其后实际Runtime结果见末节，第二轮18例失败作为修前基线保留，真实宿主/设备本轮仍未运行。

## 根线审查后实施与验证（S4/S5）

根线批准仅修改专属CameraMappingSolver.FitNormalized。实际变更补上上述float H0/H1往返，保留8个自由系数/隐式第9项=1，并将最终归一化改为float倒数后乘；Ns/Nt显式逆变换按原厂double epsilon保护行列式，归一化分母按原厂float epsilon保护并清零9项。PE常量字节直接核验：RVA162b690是double `2.220446049250313e-16`，162b698是float `1.1920928955078125e-7`，162b69c是float1。没有修改公共NPointSolver、MatrixToParam投影或容差。

修后两个定点：生产源码H0/H1与原厂入口全部位同，归一化点、初始A/b及解、每次已捕获残差和加权法方程/解均位同；16点线性求解次数修前原厂3/产品2，修后3/3。24个原有float点集的当前源码求解矩阵及RatioX/Y均逐位同原厂；该位比较是隔离原样源码诊断，实际生产Runtime结果另列。

实际生产Runtime独立artifacts构建后回放：24/24 Success、216个发布字段按固定`<0.003`比较仍9例失败（4点4/12、16点5/12），由修前18例降为9例，最大残差0.0625。剩余首个差异均TranslateX/Y，正对应固定原厂H的分解差异；未把9例失败改成通过。既有SDK求解回归118 matching、0 mismatching。24例命令退出1，118例命令退出0；前者是保留的真实对照失败。

产品Solver SHA256由 `DFDCEAEF0EF8ADF89E77B8669016C638008BB93F4B70E99F297872BD64ECE273` 变为 `B108B09E7BDF10A5318DCC76C8FF0B5EF99AFF6E833E96B9A1DC30CCDACA0E4C`。LinearAlgebra保持 `A02B1F61C1A49D9E448B44E77D23E478FE00381CF4F0CE122E7C323BD5F091AD`；公共NPointSolver保持 `F39DE6E04388B6BC9AA1B0C4B1B97B65E5C9B174380C7F8BCAA767A1C2A624CC`。

复现新增定点：先执行 `tests/CameraMapping.NativeProbe/Run-B1SolveTrace.ps1`，再用PowerShell7执行 `Trace-B1SourceSnapshot.ps1`、`Compare-B1Trace.ps1` 与 `Check-B1Solver24.ps1`。修前源码/原始诊断保存在专属analysis目录的before子目录；修后实际Runtime、118例与逐阶段原始日志同目录。没有设备、WPF宿主或Graph验收，没有提交推送。B2继续追MatrixToParam内部源码及固定H的偏移首分叉。
