# R03 单词失败原厂 oracle（2026-10-19）

同一原厂 MVD_Algorithm.dll（SHA256 `85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549`）直接调用 RVA `8b0260`。复用既有40例 probe 的 x64 五参 ABI：`int(int* bits,int checks,int symbols,int* output,void* scratch)`，固定符号byte0..23、18校验、MSB先输入192个int位。没有平台Init、设备或授权操作。

原始记录：`analysis/map-calibration/r03-failure-oracle/probe.jsonl`；代码/二进制身份：同目录 `provenance.json`；退出：`process.json`。结果原EAX=-1000（0xfffffc18），24个输出保持-999，输入不变，输出前后16 int和工作区外侧哨兵不变，进程退出0。这里只证明被检查边界未写，不推广为所有内存均无写入。工作区128字节对齐、容量65536；85aff0(18,24)源码需求1280字节。复用既有probe生命周期，fflush后仅TerminateProcess自身，避免core卸载挂起。

独立复演的degree9/roots1与8b11a0根数不匹配返回-1000源码连接；本次未观测原进程内部degree/root变量，不冒充内部动态trace。候选失败返回现有直接原厂oracle，C#候选尚未编译，未修改共享产品。

附加Log[0]证据：VA182fcc4c0在同hash PE .data虚拟尾部（RVA2fcc4c0，sectionRVA1f38000，virtualSize10fb7b0，rawSize1077e00，偏移10944c0），初始4字节由loader零填充为0。见 `log0-pe-initial.json`。此前“932480不写即永远未知”表述过强；932480不写Log0与进程初值0可同时成立。后续写者的引用审查单列，不将GF常见实现当证据。

## 当前调用链 Log0 写者边界修正

932480仅写Log[1]及GF循环非零value（多项式0x11d的255项），未写Log0；已恢复8b0260/8b11a0/8b0670及932440/85aee0/931c60传入表用于查表，未恢复到对Log0的写入。因此本候选按当前串行调用链保留PE初值0，不再因分子0安全拒绝；931c60的“先写0再落入查表覆盖”保持原操作。独立托管表不模拟外部任意写入或并发破坏。私有noanalysis Ghidra引用查询结果为空，不能证明全模块无引用，更不能声称所有SDK调用永远不会改表；本结论只覆盖当前已恢复调用链。

候选已修正，revision-oracle-log0.json记录本轮类/补丁/原trace/PE初值hash；旧proposal-index.json为前一审查快照。git apply --check通过，仍未应用共享、未编译C#。
