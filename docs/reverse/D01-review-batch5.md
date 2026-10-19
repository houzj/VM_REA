# 第五批 D01 保存回调与进度指标独立复核

结论：**D01.save-callback 的静态子项 done；D01 整包及 native writer 不关闭。** 本复核只读取既有反编译、六方法原 CIL/EH、导出器与哈希；没有执行目标、SDK、共享构建或访问 tool-plugins。使用 reverse-engineer-anything 技能的既有证据复用规则，不重复已知 native provider 失败。

## 调用链核对

- `analysis/decompiled/VM.Core/VM.Core/TempVmSolution.cs:15–17` 保存事件订阅与报告一致；该类型有大量其他初始化，不构造其原实例。
- `VM.PlatformSDKCS.SolutionManager::GetSolutionSaveEndCallBackInfo` token `06000600`：IL_000E 委托 Invoke，IL_001B 写 `nSolutionSaveErrorCode`，IL_0022 写 `mWaitSaveSolutionFlag=true`；顺序明确。
- `VM.Core.VmSolution::MSolutionManager_OnSolutionSaveEndCallBack` token `06000164`：`isDisplayProgress=false` 跳 IL_0068，仍在 IL_006E 清 `_isSaving`；启用时先构造事件、复制状态、非零状态查本地化，再于 IL_0061 发委托。日志调用在 IL_008C，晚于 Core 清标志、早于 SDK 回调返回。
- 最终 run 的六方法 metadata 均 `exceptionRegions=[]`，与原 CIL 无 catch/finally 相符。不能因此证明被调成员或更外层 native transport 没有异常保护。
- `SolutionManager.SyncSaveSolution` 现有反编译确认先清 SDK 标志，再调用原保存、等待，SDKMode 下检查回调错误。没有原回调运行或永久等待复现，不把风险写成已复现故障。

精确异常分段：Core 事件/本地化或更早 SDK 订阅者抛错，可能使 Core 清标志与 SDK 两次写入均未到达；Core 日志若抛错，则 **Core 已清 `_isSaving`，SDK 两次写入仍未到达**。报告关于较早异常的判断成立；整合摘要应保留此分段，不能笼统称全部异常导致三个标志均未更新。

## 证据完整性与限制

最终目录 `analysis/full-audit/batch5-20261019/D01/run-3a100c718d2043efa280038298fc47c3/` 两个 methods JSON 合计六体，Core `06000163/164/165`、SDK `060005FE/5FF/600`。八个输出清单文件重算 SHA256 全部匹配；当前安装两 DLL 哈希也与前后完整性记录一致。复核清单保存在同级 `review/review-hashes.csv`。

导出器使用 ReflectionOnlyLoadFrom，只读取方法元数据和 IL，不调用目标方法。选择器按精确类/方法名，最终结果正确；脚本本身没有“必须正好六体”的自动断言，故本批由 JSON 实际清点确认，不能把 exporter exit0 单独当覆盖门槛。旧的多导出 run 不计此次六体覆盖，也不删除。

六个方法是静态证据数量，**不是六项目标运行测试**。没有新增产品实现或兼容策略。native writer/reader 字段归因、最小方案单操作差分及原机重开均未达标；D01.native 仍需可用且获授权的 native 分析能力及独立受控取证条件，原阻塞不因本切片解除。

## 进度口径复核

`docs/FRAMEWORK-REVERSE-PROGRESS.md` 预定四子项均有有限问题、退出证据和停止条件，符合收口要求：A 为初始化隔离可行性，B 为绑定确认写链，C 为 Group 注册采值链，D 为保存回调标志链。A 若只能得到隔离阻塞，应记 blocked；B/C/D 达静态退出门槛可记子项 done，不得冒充运行或整包验收。

整包 **0/32** 与本批子项完成数分开合理。最终应同时列出“本批关闭 / 固定四项”“阻塞 / 四项”、原方法运行/独立操作/原机差分的实数及未运行，并说明整包变化仍为 0。不把六体或累计测试数换算成总完成百分比。下一批必须列可改变关闭状态的有限目标；相同已关闭静态问题停止派发，阻塞项仅在解除条件改变后重开。

复核状态：done。无要求扩大调查范围的阻断问题；日志异常分段为摘要准确性改进。未提交，未推送。
