# A03 第六批：进程局部注册表隔离实验

2026-10-19，基线 `48a0760`。`A03.process-local-isolation-proof` 的注册表映射部分已实际证明；原日志程序集装载、配置监视和输出隔离尚未运行，完整 A03 初始化隔离仍开放。本批在此退出，不追加 logger、装饰器、SDK 或窗口运行。产品及 tool-plugins 未修改。

## 固定门槛与指标

| 执行前固定门槛 | 结果 |
|---|---|
| 自有短寿命子进程建立HKCU volatile GUID树，映射HKLM并读回sentinel | 通过：create=0、override=0、sentinel匹配 |
| 父进程前后只读指定宿主HKLM默认值，无宿主HKLM写入 | 通过：before==after；不是全注册表完整性证明 |
| 撤销映射、关闭handle、仅清理本次GUID树、保全失败 | 通过：restore=0、close=0、own-key-cleanup=PASS |
| 证明原程序集装载/配置/watch/输出约束后才运行LogHelper | 未执行：剩余约束尚未证明，不加载原厂程序集 |

固定门槛3/4通过、1未执行。新增独立隔离基础设施成功运行1，原厂方法运行0、原机差分0、产品修复0、完整包关闭0；完整初始化隔离阻塞解除0。

## 探针与范围

遵循 reverse-engineer-anything 指南，复用第五批原LogHelper/CurrentPathHelper CIL与日志配置证据，不重复静态派发。此次是独立安全基础设施，不冒称VM原行为。

`tests/NativeEvidence/A03ProcessIsolation/Run.ps1` 用系统.NET Framework64 csc编译自有Probe.cs，每次生成独立GUID输出和进程。父进程只读 HKLM\SOFTWARE\WOW6432Node\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\VisionMaster 默认值。子进程建立 HKCU\Software\REA-A03-GUID volatile根和volatile子树，填入自有owned-appbase sentinel，再用RegOverridePredefKey将仅此子进程HKLM映射到该根。finally恢复映射、关闭handle、只删除该GUID树。未写宿主HKLM，未加载Apps.Log/Apps.BaseLog/VM DLL，未接触其他任务进程。

## 实际记录与纠错

以下目录均相对 analysis/full-audit/batch6-20261019/A03/：

| 目录 | 结果与证据地位 |
|---|---|
| run-e2bcab252dfa46389595b1ca23fe5f76 | 初版UIntPtr(uint)在x64零扩展HKEY且访问掩码过宽；rc5/exit10保留，不据此归因权限 |
| run-cb5acf1e1dc94386b991023f1eac02ce | 缩小访问掩码但HKEY尚未修正；同错保留，不作最终权限判断 |
| 中间编译失败目录 | 对SafeRegistryHandle直接调ToInt64编译错误，compile.log保留，未运行该版 |
| run-b22166c627fa4a87a062b972013aecf3 | 修正IntPtr(int)，与托管实际handle一致，沙箱内仍rc5/exit10；不能推断宿主ACL全貌 |
| controlled-execution/run-c49591e5ea4244bdb1616c65997a585d | 根线自动审批受限探针后根创建成功，子树默认nonvolatile被Windows拒绝；exit13，finally清理自有键成功 |
| controlled-execution/run-12a3ee8bcddc45adac9369c3908619bc | volatile子树修正后完整成功，exit0、PID38396；create/override/restore/close均0，sentinel匹配，own-key-cleanup=PASS，宿主指定值不变 |

根线复核安装Windows SDK 10.0.26100.0/um/winreg.h:109–110，HKEY宏为LONG再ULONG_PTR；hkey-header-definitions.txt保留原行。最终HKCU=-2147483647、HKLM=-2147483646，与托管DangerousGetHandle一致。初版HKEY、SafeRegistryHandle编译和volatile子树错误均是探针错误，不计原厂行为。

根线仅对明确限定的自有探针使用自动审批的沙箱外执行，未收到审批拒绝。不把沙箱内rc5写成审批拒绝或所有环境均不可创建。旧报告report-before-controlled-result.md及所有失败/最终成功保留。

## 剩余边界与退出

映射成功只解决注册表路径重定向能力。尚未复制和验证完整自有DLL依赖，未记录实际GAC/LoadFrom绑定，未验证原LogHelper实际选择自有配置，未观察ConfigureAndWatch、后台队列、RollingFileAppender和全部输出路径。原装饰器隔离、绑定通知和桌面等价均未证明。

下一项只能是有限的原日志装载/配置/watch/output约束验证：先证明复制件与实际装载路径/哈希、全部监视输出均受控，再运行初始化。不能跳到装饰器或SDK。当前停止，不重复成功的注册表实验。

每次目录保留实际生成的编译日志、exe、registry-proof.log、stdout-stderr.log、host-observation.json和哈希；编译失败不补造运行记录。外层evidence-hashes.json覆盖失败、成功、源码与本报告，清单自身排除。
