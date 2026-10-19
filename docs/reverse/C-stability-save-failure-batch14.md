# 长稳保存替换失败只读诊断（第十四批C线）

## 原始失败与身份

自有运行：`analysis/full-audit/batch13-20261019/C/long-run/stability-b9d820423af94e33bdfd798ce9d92ec6`。
PID54372，进程创建 `2026-10-19T12:02:03.0225068Z`，记录基线 `b11594396061226f669f2bfe49d8378f0af27b21`，运行时 .NET10.0.12/x64。最终 summary 明确 passed=false、completedDuration=false：413.078秒、21216个已完成循环、108740断言、1327次已完成保存。1800秒目标未达到；诊断资源预算在已测窗口内未超限，不可据此报长稳通过。

原始异常：UnauthorizedAccessException，栈为 FileSystem.MoveFile → SolutionStore.Save:23 → GraphEditorDocument.Save:43 → Probe Program:64。Program:64 对应 `loaded.Save(secondFile)`，目标为本GUID下 `save-as/workload.vmx.json`，不是首次写 `workload.vmx.json`。1327次已完成保存与663组双保存加本轮第一次保存一致；异常发生在第664组第二次保存。当前第一文件根缩放16，第二文件根缩放4，符合本轮21216和上轮21184取模20的状态；没有把失败的第二次保存计入成功次数。

## 已完成只读核对

最终 evidence-hashes 列出的7份原产物逐项SHA256复核一致；结果见 `analysis/full-audit/batch14-20261019/C/failure-evidence-hash-check.json`。没有改写原失败目录或重跑长稳。

只对本次自有目录、两个方案文件及 save-as 子目录读取属性/ACL，保留 `failed-run-file-metadata.json`：两文件当前均Archive、无ReadOnly，owner为本次沙盒用户 CodexSandboxOffline；ACL具有继承Modify，未见Deny。完整自有文件清单无残留.tmp；`failed-run-file-inventory.json`、`last-saved-state.json`保留当前状态。这些是**事后**快照，不证明失败瞬间的权限/属性或外部占用。

探针没有并发Task、后台文件reader或复用未关闭stream：每轮只同步调用Save/Load。SolutionStore.Load的`using var input=File.OpenRead`在返回调用者前释放；Save的临时输出流也在`File.Move`前结束作用域。源代码审查没有发现该路径自身持有目标文件共享句柄跨下一次Save。不能从这一点推论系统其他组件不存在短暂占用。

`tests/VisionEditor.SaveHistoryChecks/Program.cs`已包含独占FileShare.None目标锁期间保存失败、目标字节不变、编辑器路径/基目录/redo不变、临时文件清理断言。本次仅查看该测试，未运行；它证明的是既定受控锁案例设计，并不解释当前失败的实际原因。

## 尚缺证据与最小复验建议

原日志只记录异常类型、消息和堆栈，未记录HResult、临时文件全路径、失败瞬间源/目标属性或持锁实体，不能将原因归为杀毒扫描、沙盒权限、Windows共享或产品句柄泄漏。当前没有对其他进程/窗口进行查询、关闭或干预。

建议根线下一步只创建**新的自有GUID目录**和最小保存探针，不自动续跑1800秒：

1. 固定同一合法小文档，顺序Save→Load→SaveAs→Load循环，保留每次保存序号；可设有限1000对上限，实际结果仅叫保存复验，不计长稳。
2. 在测试层包围保存调用，发生失败后立即记录`exception.ToString()`、HResult十六进制、pid/启动时间、目标存在/属性/ACL读取是否成功、目标前后hash、剩余tmp全路径/属性/长度。所有诊断异常另存，避免覆盖原异常；不能先删除/重建失败目标。
3. 分开运行明确自有控制：未持锁、FileShare.None持锁、允许读取而不允许删除的读共享、允许删除共享。锁只作用于新探针自有文件，释放后核对目标字节及下一次保存。前两种已有测试可复用，后两种需显式输出实际异常/HResult，不能预填预期Windows码。
4. 暂不加广泛重试、不改ACL、不解除只读、不抑制UnauthorizedAccessException；若考虑保存替换的本地容错，先由可复验现象建立有限触发与失败事务合同，并保留原失败。

在找到精确根因之前 C07.1维持未完成。C07.2、C07.3、原SDK/设备与完整宿主长稳没有因本次有限资源采样而关闭。tool-plugins未动。
