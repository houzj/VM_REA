# C07.1 受控长稳基线工具（第十三批）

本轮没有将 C01.1 的 root/part 准则简化为“加一个栈容量”。原服务除容量外还有注册期间 `_unRedoing` 准入、root/part 分流、注册 predicate、未完成 part 的首次 Undo 前更新与执行优先级；当前独立整文档快照不具备全部同形机制。C04 原 SDK 临时文件载荷也没有用 JSON 替代。详见 `analysis/full-audit/mf04-unredo/VM.UnRedo.il` Register/BuildRoot/UpdateActionData 和既有 `docs/UNDO-ROUND2-20261019.md`。

本批改做 C07.1 真正可复验的主框架托管资源基线。实现位于 `tests/FrameworkStabilityProbe13`，不是产品程序集，不运行原厂、SDK、设备、插件或用户桌面实例。

## 预先固定负载与时长

- 最短 1800 秒，最长一天；低于 1800 秒参数拒绝，不能以短跑输出当成长稳完成。
- 每轮新建自有 GraphEditorDocument；两次更名、Undo/Redo、非活动容器缩放、流程复制、Group 创建及独立缩放。每32轮实际 Save/Load/SaveAs，并核对四流程、两级缩放及 clean 状态。每轮至少休息10ms。
- 文档不跨轮累积；存储始终复用 GUID 自有运行目录下两个文件，不无限制造产品或磁盘历史。
- 首120秒预热，每10秒及末尾采集同一自有PID的 PrivateBytes、WorkingSet、ManagedBytes、线程数、句柄数、CPU时长与累计轮数/断言/保存数。
- 本地诊断预算：预热后首5与末5采样中位数增长，私有内存≤64MiB、线程≤8、句柄≤64。它们是明确的当前主机回归包络，不是原厂性能参数，也不证明不存在泄漏。异常、超预算、中断、不足时或采样不足皆保留失败。

## 精确构建与启动命令

由根线串行构建，不与其他产品构建并发：

```powershell
dotnet build tests/FrameworkStabilityProbe13/FrameworkStabilityProbe13.csproj -c Debug
```

根线使用自己刚构建的可执行文件异步启动；`<完整基线SHA>`替换为实际完整SHA：

```powershell
$stabilityParent = 'D:/MyWork/REA逆向测试/analysis/full-audit/batch13-20261019/C/long-run'
$stabilityExe = 'D:/MyWork/REA逆向测试/tests/FrameworkStabilityProbe13/bin/Debug/net10.0/FrameworkStabilityProbe13.exe'
New-Item -ItemType Directory -Force -Path $stabilityParent | Out-Null
$stabilityProcess = Start-Process -FilePath $stabilityExe -ArgumentList @(('"' + $stabilityParent + '"'),'1800','<完整基线SHA>') -WindowStyle Hidden -WorkingDirectory 'D:/MyWork/REA逆向测试' -RedirectStandardOutput ($stabilityParent + '/launcher.stdout.txt') -RedirectStandardError ($stabilityParent + '/launcher.stderr.txt') -PassThru
```

只对本次自有子PID记创建时间/路径并读取；不得枚举后终止同名的其他进程。此命令示例只展示一次启动，不能覆写已有 launcher 日志重跑；需要新尝试时应给新独占父目录并保留失败。实际进程内部还生成 `stability-<GUID>` 防止覆盖历史文件。

## 产物与完成判定

运行目录包括 `owned-run.json`（PID、进程启动时间、原始路径、可执行文件/程序集哈希、基线SHA、环境与固定负载）、`samples.jsonl`、原子更新 `status.json`、`summary.json`、可选 `failure.txt`、最终 `evidence-hashes.json`。没有“自动重试直到通过”。只有足时 summary 和真实退出证据齐备后才考虑该有限托管基线实施自审核完成。

原始差分边界仍保留：`docs/MF05-PROCESS-LIFETIME-AUDIT.md` 仅证明主框架当前托管源码未见显式业务子进程启动，不能推论 SDK/驱动资源安全；本工具不覆盖 WPF Dispatcher、图像/算法、原 SDK 回调、设备、长期并发或全主机资源。C07.2/C07.3 及原厂等价不因本工具通过而关闭。

## 当前执行状态

子线只实现与静态预审，未构建、未启动，未声称足时通过。根线必须在此补实际构建、PID身份和GUID运行路径；下一心跳接收足时产物之前 C07.1 不计完成。

根线实际状态：probe构建exit0（0告警0错误），启动UTC2026-10-19T12:02:03、PID54372，GUID stability-b9d820423af94e33bdfd798ce9d92ec6；最早12:32:03UTC足时。当前仍运行，不记通过。完整精确路径在总台账activeBatch.ownedRuns及本地C/long-run/launch.json。
