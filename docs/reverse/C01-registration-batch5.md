# C01 第五批：Group 注册链与明确退出条件

2026-10-19；main 基线 e6bf62a。有限子项 `C01.group-register`：**静态源码链 done，真实 Group/SDK 命中 blocked**。C01 一级包未完成。本批无产品修改，无原厂代码执行，无 SDK/设备或窗口启动，无共享构建，未访问 tool-plugins。

## 本批进度指标

| 固定验收项 | 结果 | 可复核证据 |
|---|---|---|
| 配置入口→root 注册→关闭清空根 | done：2 原方法 CIL | VMControls.WPF.dll；OpenParamsTabControl、CurTabControlBase_Closed |
| Group Owner 绑定→配置数据→UserData | done：5 原方法 CIL | DynamicObjectConfigControl.dll；GroupParam.get_Owner、ConfigCommandExecute、ChangeGroupInfosToSDK、AddParam、DelUserDataByName |
| IOC 实现/生命周期与原服务转发 | done：15 IOC + 7 撤销方法 CIL，启动调用 1 体 | VMControls.Interface.dll、VM.UnRedo.dll、VisionMaster.exe |
| 旧值/新值捕获及异常范围 | done：13 Core 方法 CIL/EH | ModuleParam、UserData |
| 同一真实 Group 的根/part/SDK 数据观测 | blocked：0 次 | 需专属原实例与被动断点日志；下述解除条件 |

本批合计 **43 个原方法体**，仅为静态覆盖数量，不是 43 项运行通过。固定 5 项中 4 项源码验收完成、1 项阻塞；一级 C01 完成数仍为 0。已有 batch1 的 13 个合成服务动作未重复。

## 原件与审计

REA `inspect_managed_members` Evidence `ev_94e7251f01d24281bffa0e9c0300c8c5f0bd0b9e995a679e1e4847075df3db3f`：VMControls.Interface.dll SHA256 `47c11ff2239bd2577378c4eb202ef6b2aee8fc0e99c3b7f0692c2209eb69c304`，完整返回 `analysis/full-audit/batch5-20261019/C01/rea-interface-members.json`。已有大程序集 REA 全量成员输出预算限制，本批不重复相同大调用；以原件精确类型 ReflectionOnlyLoadFrom 提取 CIL、token、EH、可选参数默认值和 SHA256，不运行目标方法或静态构造。

重跑入口 `tests/NativeEvidence/C01Registration/Run.ps1`，Framework64 csc 编译元数据导出器，不使用共享 dotnet。6 原件执行前后 SHA256 均一致，见 `artifact-integrity.json`；逐方法 `*.cil-methods.json`、指令 `*.cil-disassembly.txt`，原反编译逐行保留 `source-numbered.txt` 与 `source-hashes.csv`。此处 ReflectionOnly 依赖元数据可能由 GAC 解析，不据此声称运行时实际程序集来源；报告绑定的目标自身路径/hash 为原件身份。

## 已闭合的源码缺口

1. **IOC 并非每次 Get 新建撤销服务。** 原 `VisionMaster.App.OnStartup` CIL `IL_02BB–02CA` 将 IUnRedoService、从配置指定程序集发现的实现类型、显式 `ldc.i4.1`、两个 null 传入 Ioc.Set。Ioc.Set token `060000AF` 的 isSingle 默认也为 True；SetInternal `060000AC` 为 true 选择 TypeLifetime.Singleton，随后 RegisterType。Ioc.Get `060000A6` 先 IsRegistered 再 Resolve，未注册返回默认值。只能确认容器注册/解析静态链，未观察真实进程重注册或替换。

2. **Group Owner 来自 XML 顶根模型。** `DynamicObjectConfigControl/GroupParam.cs:37–47`（本批同原件 CIL）缓存 `(GetTopBaseNode() as AlgorithmTabRoot).Owner as VmModuleSet`，不是每个菜单新造 Group。GroupRunParam.ConfigCommandExecute 把此 Owner 赋给 GroupRunParamConfigData 和各装饰项；ParamConfigDataBase.ChangeGroupInfosToSDK 对 Owner.UserData 写组名/ID，AddParam 与 DelUserDataByName 写入或移除各项元数据。配置界面最外层 `VmModuleDecorator.OpenParamsTabControl` 先注册 ModifyConfig 根，再将 VMModule.Params 赋给配置窗口。没有实际对象身份日志，因此不把上述可达性称为原机命中。

3. **对话框抑制缺少 finally，已核原 EH。** GroupRunParam.ConfigCommandExecute token `060001D7` 的 SetForbidRecord(true) 为 `IL_017B`，DialogHandler.Invoke `IL_0188`，返回值 `IL_018D pop`，SetForbidRecord(false) `IL_019A`。3 个 finally 范围分别 `[84,130)`、`[161,335)`、`[523,618)`，只覆盖枚举释放；均不包住十进制 392 的 Invoke。故“存在 finally”不代表抑制能异常复位；没有 Cancel 返回值回滚分支。孤立项清理的 ForEach 在 true 之前，仍可能触发录制。这是静态条件结论，不是已注入故障的运行观察。

4. **抑制检查先于旧值表达式执行。** UnRedoSchedule.Register `06000046` 的 ForbidBuild 检查 `IL_007C–008A`，predicate Compile/Invoke 为 `IL_008F–00A6`；先拒绝 part 时不会采集旧值。ModuleParam.CanRegisterParam `060005CC` 的同模块去重、根 ModifyConfig 和非 IVmProcedure 检查之后才执行表达式；参数 setter 先 RecordParamUnRedo，再 SDK 写入。UserData 则区分全部数据的延迟捕获和单键携带旧新值，不能给所有 setter 套同一顺序。ModuleParam.Param_Update `060005CF` 在后续 Update 时取新值。Param_Undo/Redo 的 SDK SetAllParams catch 仅 `[18,46)`，后续非 Group ReFlashReadyState 位于其外；不能说整个回放异常均被该 catch 吞掉。

## C01 剩余验收与退出条件

不再扩大本切片方法清单。下次 C01 仅执行以下固定原机差分清单，全部满足才申请关闭包：

- 专属副本/专属 TempFiles 原实例里，正常打开真实 Group，记录 root 身份、Owner/ModuleFullName、Ioc 返回实例、首次及重复 part、旧参数真实载荷哈希；同模块两次和不同模块各一次。
- 正常关闭、取消、窗口异常：记录抑制位、动作数、实际 SDK 值；不得由无 finally 的静态结论替代故障结果。
- Undo/Redo 记录真实 NewParamList 捕获点、回放后 SDK/界面值和栈迁移；至少一次不跨线程、一次实际 UI dispatcher 路径。
- CopyModule 优先级、RecoverStack 密码取消、TempFiles 淘汰载荷与完整性仍为 C01 已有缺口；限定独立样例各一例，失败保留，不另开无上限静态切片。

解除当前阻塞需要：可归属本任务的原机工作实例、副本路径/进程身份、能被动采集上述调用的受控调试设施。当前未提供这些运行证据；没有尝试在安装目录构造会清 TempFiles 的 UnRedoSchedule，也未干预其他任务实例。原叶方法不是纯函数：真实捕获依赖 VmSolution/ServerSDKManager 与服务初始化，因此不通过未初始化假对象绕过真实命中验收。

## 保留的失败

`failed-inventory/` 保留初次广列 WPF 类型和 DynamicObject 解析依赖失败（退出 -532462766）；改精确 GetType 和补充原件只读依赖搜索后成功。扩展脚本误替换 PowerShell 条件造成一次找不到原件，已修正；全部 43 体成功后的 manifest 曾因当前重定向日志占用失败，进程退出后重新生成；另一次审计编号命令语法失败发生于执行前。完整说明 `failures.txt`，最终导出日志 `final-export.log`。这些是分析脚本失败，不是产品/原厂行为失败。未知泛型 operand 保留 token 与 unresolved 标记，未猜造调用目标。
