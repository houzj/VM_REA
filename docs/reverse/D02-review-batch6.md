# D02 第六批独立复核：自有保存进程中断

结论：**接受 D02.process-interruption 本次有界独立实现操作证据，不关闭完整 D02**。复核基线 `48a076000a784dd3400494f34c21567c12e61d6c`，未改产品、探针或 tool-plugins，未启动、暂停或终止任何进程，未重新构建。

## 审查范围与证据权威

已读 AGENTS.md、PARALLEL-WORK.md、REA 技能，以及 `src/VisionFramework/SolutionStore.cs::Save/Load`、`tests/VisionFramework.SaveInterruptionProbe/Program.cs`、项目引用、`D02-interruption-batch6.md` 和实际日志/result.json。这里复核的是完整当前源码和既有运行文件，不需要重新调用原厂二进制分析；没有新增 REA 或原厂运行证据。

探针引用真实 Framework；父进程先保存旧方案，子进程仅接收独占路径及环境 token，真实调用 SolutionStore.Save。暂停使用 Process.Start 返回对象的 Handle，Kill(entireProcessTree:false) 也仅作用于该对象；未查询其他 PID 或窗口。源码中 finally 只处理此对象。此权限边界与报告一致。

生产 Save 先验证并序列化，再在同目录 CreateNew 唯一临时文件，Write、Flush(true)、关闭流、Move(overwrite:true)，最后删除本次临时文件。强杀不会保证执行 finally；探针没有插入产品测试钩子。

## 保留文件独立核对

原运行 `analysis/full-audit/batch6-20261019/D02/interruption-87f94fd90c0947d194ab46f77d1bbb04/`，日志显示四断言通过、observed=true。复核只读结果见 `analysis/full-audit/batch6-20261019/D02/review/retained-file-check.json`：

- 遗留临时文件仍在，SHA256 为 `1808FCCB3969B516ABF24A38A34FE5602D19D0581521463965DC17478A2CD098`，与 result 一致；可解析 JSON，Id=interruption，名称为 16,777,216 个 n，包含一条 procedure。
- 当前目标可解析，名称为 recovered，证明后续显式保存结果仍保留。子进程 child-ready token 与父进程记录相同。
- 仅在内存中将当前目标唯一 Name 字符串 recovered 替换为 original，重新计算原编码字节 SHA，得到 `AB593B2ED3EFA9994CA6A1181ECF334863995A56E72B09EFBDF5A28C45360664`，精确匹配中断后、恢复前记录。这与“本次中断留下完整旧目标”一致。没有改写目标文件。

## 断言强度及限制

四断言分别覆盖：可由真实 Load 接受且 Id/Name 为旧或新；暂停时看到的临时文件在强杀后仍存在；随后显式保存可加载 recovered；该显式保存不删除之前的临时文件。此次内容和报告没有冲突。

第一断言只比较 Id/Name 并依赖 Load 的结构验证，**不是完整对象图逐字段或逐字节等价断言**。本次另有旧文档摘要和上述离线复核支持具体旧文件结论；不能将断言推广到任意复杂方案的完整性证明。临时文件本次可解析，不证明它在暂停时已经 Flush(true)，也不能定位强杀落在 Write、Flush 或 Move 前的哪一个阶段。运行后 current target 已被显式恢复保存覆盖，原中断目标只保留摘要；此保全限制应保留。

父子 token/handle 和目录隔离足以支持本次自有进程身份，未独立证明操作系统电源故障语义。没有证明所有中断时机、跨卷/网络盘、自动恢复、孤立文件回收或 PrepareSave 外部资产事务，更不涉及原厂 `.sol` writer。FCR-024 失败仍独立开放。

## 完成率分层

本次可结束有限子项 D02.process-interruption；完整 D02 还要求其余失败类别及编辑器状态证据，不能由四断言替代。总体完成百分比必须使用总计划完整包关闭数 / 32 × 100%；按本批起点仍为 **0/32 = 0%**。新增独立实现操作证据应单列，不能折算总体百分比。
