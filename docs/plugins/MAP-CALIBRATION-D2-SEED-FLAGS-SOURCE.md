# D2第三切片：seed flags负索引清理

后续补充： [D2退出审计](MAP-CALIBRATION-D2-SOURCE-EXIT-AUDIT.md) 已复用8c00f0原始C/asm定位−1的扩边写源；本报告“seed本身未写−1”结论保留，具体输入是否在剪枝选中−1仍未运行补证。

2026-10-19。只追8bcff0的3×3/2×2 seed初始化、失败清理、flags基址及邻接保留段。未执行任何未知负索引输入，不将上个整数格网padding切片当作本切片heap安全/越界证明。

## 身份、取体与ABI

MVD_Algorithm.dll SHA256 `85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549`。沿用私有Ghidra12.1.4只读fallback，无REA Evidence ID或全局配置改动。新增8c1780、8c1340、8c0fa0 C/asm/cookie-tail，与已取8bcff0/873de0共5入口，索引/PE常数位于`analysis/map-calibration/d2-seed-flags/source-index.json`。

8c1780/8c1340伪代码void不可信；cookie后分别明确返回0或1。caller 8bcff0在1808bd231/235/241组装seedIndex、flags、scratch三个栈参数，再调用seed。有效签名为：候选28字节数组、N、state指针、dims指针、seedIndex、flags基址F、neighbor-cost scratch。state+0为Count，+8为indices指针，+16为scores指针；dims为独立两个int。

8bcff0第9参数是F，进入时从栈装到RSI（1808bd07d）；不是float flags，伪代码float值1.4013e−45实际对应整数位1。后文所有flags写入均是4字节整数。

## −2首次写入与确定失败分支

8c1780入口循环45000次，每次两个槽：清F[2i]/F[2i+1]=0，并写indices[2i]/indices[2i+1]=−2，共90000槽（asm1808c181b..1850）。随后才判断N<9：小于9直接返回0（1852..1863、cookie后XOR EAX），没有先填3×3槽，也未写Count或dims。此前Count/dims的保留与indices新置−2必须分开。

8bcff0收到返回0后直接跳1808bd68f，对索引矩阵固定九个位置：

`[0,1,2,300,301,302,600,601,602]`

逐个MOVSXD取索引再写`dword[F+index*4]=0`。1808bd699..6f8没有index>=0谓词。于是**在源码N=1..8且进入seed调用的路径**，这些九槽均仍−2，形成对F−8的九次重复写0。这是具体源码可达分支，不是本轮执行得到的结果；N=0时外层seed循环不进入，不能说N=0也执行九次清理。

3×3成功却worstScore>−6，也跳同一九项清理后尝试2×2（1808bdabe..b28）；此时九槽已由seed填入候选index，写目标通常是flags有效项。阈值−6由PE181647dec确认。

## 其他失败与正常写序

N>=9时8c1780先将seed写indices[301]，通过8c0fa0选四边/四角并逐个写flags[index]=1，最后检查8个neighbor cost是否满足`abs(cost−1e10)<1`。命中时先按九槽清flags，再把整个300×300 index矩阵memset0、dims置0、返回0；Count未在此失败尾明确重置。caller随后仍进行固定九项清理，此时index矩阵已0，故这一步重清F[0]，不能一概称所有seed失败都清F−8。

8c1340入口把90000索引置−2，但不整体清flags；写四个2×2位置[0,1,300,301]及flags。neighbor cost的同一失败判据命中时清四项flags、整个index矩阵memset0、dims置0，返回0；正常成功写Count4/dims2×2并返回1。3×3正常成功对应Count9/dims3×3。首次−2和后续memset0是不同生命周期状态，0不能被当作缺槽哨兵。

8c0fa0已补体，选中index初值0，返回只能来自0或扫描候选index（合法N域）；无候选可用时cost为1e10并可返回index0。没有源码支持把该失败index擅自改成−1。**本次两个seed初始化体都只明确写−2，没有写−1；历史prefix−4来源不能据此宣称已闭合。** 后续若追扩边/剪枝或受控fixture残留，应另立切片。

2×2成功但后续几何评分不接受时，8bcff0的清理循环由两个dims原始槽决定：外层第0槽>0、内层第1槽>0；矩阵row stride300，成对清理及奇数尾项均直接写F[index]=0，没有负索引过滤。当前seed dims通常0/2/3，仍不能将一般循环正规化为“仅清有效Count项”。正常选中流程结束另有memset(F,0,N*4)；其范围也不包括F−8/F−4。

## flags实际基址与邻接块

873de0将同一flags/scratch起点传给两次8bcff0。以S为873de0收到的工作区起点，candidateCapacity为其图面积/最大格容量计算的候选保留数，定义：

```
C=align128(candidateCapacity*28)
I=align128(imageWidth*imageHeight)
G=align128(capW*capH*8)
P=S+C+I
```

| 段 | 地址/保留长度 |
|---|---|
| 第一假设XY | P，G |
| 第一假设整数ID | P+G，G |
| 第二假设XY | P+2G，G |
| 第二假设整数ID | P+3G，G |
| flags基址F/后续格网scratch | P+4G，后续scratch需求 |

所以F−8/F−4属于**第二假设ID输出保留段尾**，可能是其最后声明pair的两个int，也可能是128对齐padding。它们不是flags自行分配的正式prefix。该段由两假设执行顺序和写入count影响，不能据同一连续workspace就证明其值可任意覆盖而无影响。

8bcff0内部flags保留长度`A=align128(max(N,90000)*4)`，indices基址F+A，固定300×300 int占360000字节；score区在F+A+0x57e80，另一scratch区F+A+0x15f980。flags初始化/后续清理只按N个，3×3 seed额外清90000个；这是容量和初始化长度的区别。873de0的F不是SDK heap块首，物理heap起址/总容量与所有相邻段生存期仍未知。

## 正常/失败地址表

| 状态/谓词 | index来源 | 实际写地址 | 已核边界 |
|---|---|---|---|
| seed正常占用/释放 | seed或neighbor选中的i | F+4i | 当前合法候选index；无运行覆盖泛化 |
| 3×3 N=1..8早退后caller清理 | 入口统一−2 | F−8，重复九次 | 确切源码分支，无新执行 |
| 3×3内部cost失败清理 | 已填九个candidate index | F+4i；随后index整区清0 | caller下一清理通常变F[0] |
| 3×3成功但score>−6 | 已填九槽 | F+4i，之后进2×2 | 与N<9未填槽不同 |
| 2×2内部cost失败 | 已填四槽 | F+4i；index区清0/dims0 | Count可能保留，不能用Count断言仍有格点 |
| 2×2几何评分拒绝 | dims圈定的矩阵槽 | F+4*indices[cell] | 循环无负值过滤 |
| 假设某槽为−1 | 本切片未找到生产写源 | F−4 | 仅地址公式，不宣称已可达/已执行 |

这些都是置flags状态，不是对每个候选调用free。原厂独立GridPermutationProbe为诊断预留128-byte前缀属于既有实验安排，不能反推SDK在F前有正式128-byte安全区。

## 仍未知与计分

已闭合−2早退→无条件九项清理→F−8及相邻保留段；未闭合−1来源、当次负写的原始字节/对后续第二假设输出的实际影响、SDK heap所有权和分配总容量。没有把前一radius2读padding结论用于证明这里的写heap外或安全。后续只需追明确−1写源/相邻段生存期，不新增触发未知写入的输入。

索引/PE原始常数与diff格式检查完成，无原厂运行、产品修改或宿主验收；整卡阶段仍2/32、整卡0/4，局部报告不加完整阶段。
