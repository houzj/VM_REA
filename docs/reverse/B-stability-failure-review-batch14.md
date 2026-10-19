# 第十四批 B 独立复核：长稳实跑失败及既有存储计分

只读复核固定现场 `analysis/full-audit/batch13-20261019/C/long-run/stability-b9d820423af94e33bdfd798ce9d92ec6`。没有重跑、修改产品/探针、查询其他进程或接触 tool-plugins。

## 本次失败证据充分

summary.json：passed=false、completedDuration=false、请求1800秒、实际413.0780293秒、PID54372、cycles21216、assertions108740、saves1327，cancelled=false。失败是 `FileSystem.MoveFile(overwrite)` 的 UnauthorizedAccessException，经 SolutionStore.Save:23 → GraphEditorDocument.Save:43 → Program:64；不得无证解释为杀软、权限策略或其他进程占用。

43条 samples.jsonl 与 summary计数一致；首样本0.021秒、末413.044秒。status.finished=true 且失败字符串/计数一致。stdout有 `FAIL cycles=21216 elapsed=413.1`。固定GUID evidence-hashes.json 的7个文件逐项重新SHA256核对，0不匹配。内存/线程/句柄预算虽withinBudgets=true，只覆盖短于请求时长的片段，不推翻失败。

launch.json 给启动PID、时刻、程序和1800秒请求，没有 ExitCode；本片未发现外部捕获的实际OS退出码。源码最终 Environment.ExitCode=passed?0:1 只能证明预期退出码，不是外部观测。已有文件足够判本次负载失败与时长不足，不能报告“已运行30分钟通过”或编造实际退出码观测。

Program64 是 `loaded.Save(secondFile)`。1327次成功保存为奇数，与本轮 firstFile 成功、SaveAs失败吻合。现场两个方案均可作JSON解析且各4流程：firstFile根/复制zoom16、Group3；secondFile根/复制zoom4、Group15，后者是前32轮的状态。这与目标保留前次文件相容，但没有失败前SHA快照，不等于本轮已证明逐字节保全；也没有保留失败时editor对象，不能据此断言其FilePath/BaseDirectory/history一定正确。当前GUID无临时文件，符合Save finally清理，但不是原子性全部分支证明。

## 既有测试究竟覆盖什么

- `tests/VisionEditor.SaveHistoryChecks/Program.cs`：真实文件 FileShare.None 锁定 alternate SaveAs，接受 IOException/UnauthorizedAccessException；断言失败后editor文档、base、revision、redo保持、目标原字节不变、无临时文件，解锁后继续保存。23项既有结果见第二批/第十批D报告；本片未重跑。
- `tests/VisionFramework.SaveRoundTripChecks/Program.cs`：重复JSON属性在替换前拒绝，原字节保持、无temp、新目标目录不创建；不是高频保存长稳。
- `tests/VisionFramework.SaveInterruptionProbe/Program.cs`：受控自有子进程写16MiB，捕获暂存文件时暂停/终止该自有对象，确认目标是完整旧/新方案，保留孤立temp，随后显式保存仍成功。它不证明断电或所有OS拒绝原因，更不保证每次保存必成功。

## 对完成计分的限定判断

C07.1“长稳负载/时长及内存线程句柄基线”：工具已交付，实际该基线未完成1800秒且负载失败，不能标验证通过；若实施门槛包含取得完整时长baseline，本次不足以关闭。不能仅以采样代码存在将未取得基线变成完成。

D02.1“独立JSON结构/版本/大小/深度与拒绝”无直接反证，该异常不在结构校验。D02.2原私有资产历史失败仍开放，与本次无关。D02.3“独立写入失败事务与受控进程中断”既有实现旨在失败时保全，而非承诺任何OS条件下必保存成功；本次File.Move拒绝本身并未证明目标破坏或editor事务错误，因此不直接推翻其既有done。应将本次未解释的运行保存失败显式登记，保持D02.3原有限声明，不能用旧测试声称本次失败事务已经复验通过，也不能无证称D02.3回归损坏。

解除本次C07失败需独立查明实际File.Move源/目标路径与失败条件，追加失败前后身份/字节证据并完成原请求时长。没有新证据前保留本次失败，不自动重跑或用重试成功抹去。
