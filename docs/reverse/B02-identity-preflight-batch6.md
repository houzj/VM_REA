# B02 第六批：Group 粘贴身份链源码预审

基线 `48a0760`；子项 `B02.identity-source-preflight`。状态：**静态预审 done**。B02 前置 B01/A01 未关闭，本报告不满足其依赖、不关闭 B02、不授权产品集成。固定范围仅为多选 Copy→Group Paste→身份映射及 IO 重连，不滚动扩展 Group 全功能。

## 证据身份与复验

使用 reverse-engineer-anything 技能；REA `inspect_managed_artifact` 证据 `ev_758f593356d11c17370747b341d4b1992a0fe8ab3fd3176f70d4a6831c3d8517`，完整结果保留在 `analysis/full-audit/batch6-20261019/B02/rea-core-identity.json`。REA 静态身份不等于 C# 反编译或运行。

原件均位于 `D:/Program Files/VisionMaster4.4.0/Applications/myLibs/`：

| 原件 | SHA256 |
|---|---|
| VM.Core.dll | `936763f6c8a1bf9fcf59f22acebca8feb09b5f3d5701ae77c82bdc81e626190d` |
| VMControls.WPF.dll | `99ffa401c8784472913ad43813d68a8a5577890c00e149506e3e5664fa61c97d` |

`tests/NativeEvidence/B02Identity/Run.ps1` 使用 .NET Framework ReflectionOnlyLoadFrom 和 GetMethodBody 读取指定方法；不运行目标构造、静态构造、属性、SDK 或 WPF 窗口。最终退出 0，精确选择 Core 24、WPF 4 个方法体。同名 Init/AddModuleByInfo 重载均带 token、IL 哈希及 EH 区；原件前后哈希相同。早期选择复用导出器时带入两段无关 zoom 方法，最终已移除；最终文件只包含列定选择。

本地原 C# 路径根为 `analysis/decompiled/`；`source-excerpts.json` 固定九段带行号边界及整文件哈希的原文，CIL 文件解码到原成员 token。`output-sha256.csv` 绑定审计文件，`deliverable-sha256.csv` 绑定本报告和复验脚本。没有目标运行、产品改动、原机差分或 SDK 实例。

## 固定调用链与确认结论

1. `VMControls.WPF.DataForUI.VmModuleBaseDecorator.CopyItemsCommandExecute`（原文件 676 起）先 CheckRunning，对模块 GetParamRoot，拒绝全局模块和 `IsLoadFailed ?? true`；清剪贴板后只存 `FullName` 字符串及首项位置。因此保存的是以后再解析的身份路径，并非对象快照。`PasteItemsCommandExecute`（733 起）注册 `RootOperType="CopyModule"`，调用目标集合 `ProcedurePaste(Clipboard.CopyInfos)`。
2. `VM.Core.VmModuleSet.ProcedurePaste`，token `0x0600063D`（原文件 1318–1429），粘贴时逐字符串经 `VmSolution.Instance[sub]` 找当前对象；找不到则 continue。若是 IMVSGroup：`Export()`→`VmSolution.LoadGroup(this,path,null,isFromCopy:true)`→尝试删除临时导出文件；结果为 null 才抛 CopyFailed。把**原对象→新对象**加入 `Dictionary<object,object>`；随后 CopyModuleParam(old.ID,new.ID)、复制 IsForbidden、InitDynamicIO。这不是单纯托管深克隆，也不是按 GUID 建字典。
3. `IMVSGroup.Export`，`0x060005E4`（112–162），在 ApplicationBase/TempFiles 挑 0.gro 或 1..799 的空闲路径，调用 `SyncExportGroupModule(ID,GetDeepLevel(),path,"")`；返回原 ID、FullName、显示名、父 ID/名和路径等 VmModuleDataInfo。父为 solution 时记 10999。**本批未调用**，因此没有创建安装目录 TempFiles，也没有导出或删文件。
4. `VmSolution.LoadGroup`，`0x060000EE`→`internalImportGroup`，`0x060000D5`：传入 `nIdAllocateWay=0`；isFromCopy 只在该方法已核范围内跳过额外导入 root 注册，**并没有变成 native GUID 重分配参数**。导入嵌套 Group 时 SDK 参数为 Root.ID、目标 Group.ID；普通目标为目标.ID、0。设置 importGroupModuleSet，调用 CollectionManager.ImportGroup，再等待 mbImportGroupEndFlag。
5. 原 `MModuleManager_OnImportGroupEndCallBack`，`0x06000195`（VmSolution.cs 4584 起）：先通知 OnGroupImportEndCallBack；错误为 0 时，取 SDK 回调 `nGroupNewId/nProcessId/strGroupNewName` 构造 IMVS_MODULE_INFO，经目标集合 AddModuleByInfo 建模，设置 Parent 并通知 ready，最后 flag=true。订阅者异常可能阻断后续建模/flag；这里只是 EH/调用顺序静态结论，未运行故障。AddModuleByInfo（`0x06000622`）按 Name+ID 复用集合已有对象，否则采用新构造对象；不按 GUID 去重。
6. 身份来源：`VmModule.get_ID`，`0x06000249` 读 moduleInfo.nNodeID；Init(IMVS_MODULE_INFO,ModelBase)，`0x060002BA` 从 SDK strDisplayName 写 name，InitBase/InitIO 后可 AddModuleTo。若 SDK UI 数据已有 GUID，读取 UserData["GUID"] 建 Guid 并设置基类 GUID；否则 Guid.NewGuid，写 UserData 字节并设置 GUID。**不能据后者推断每次复制必生成新 GUID**：native ImportGroup 是否保留/替换该键尚未证明。`PreAddModuleTo` 先设 Parent；`VmFrameTreeBase.get_FullName`（`0x060004E7`）动态沿 Parent 拼 UniqueName，VmModule.UniqueName=Name；FullGUID 同理沿 UniqueGUID。改名先 SetModuDisplayName，迁移前驱的 SourceIndex/TargetIndex 旧名称键，再通知父/Name/OnModuleActionChanged("ReName",旧FullName,新FullName)，不是重写 ID/GUID。
7. 全部选中对象建立映射后，ProcedurePaste 第一轮遍历 values 调 `ModulePaste`（`0x060002E4`），第二轮才 `ModulePasteFinish`（`0x060002E1`）。ModulePaste 反查当前目标对应原对象，重连前驱、复制索引数据、注册历史，再 PasteIO；源 Params.Data 的 IUpdateItemRoot.UpdateRunParamBind 收到目标 FullName 和对象映射；随后虚 Paste、显示扩展 PasteNode。参数代理不宜用字符串替换猜补。本批仅确认该接口边界，不重做 B01 绑定逻辑或推断具体代理实现。
8. `PasteIO`（`0x06000303`，3530–3624）按同名目标 IO 匹配：常量新建；Root.ID=13000 直接保留原引用；来源 Root 命中对象映射时用目标模块.GetIO(原IO.Name)，索引为常量则新建，否则用目标 GetIO。未命中映射分支分别处理 trigger、Group 内部路径、同父和目标父，部分保持原引用与原索引，不能概括成“全部引用都指复制品”或“全部跨 Group 清空”。没有 RelateIO 才递归 Childs。

## 固定验收表与退出判据

本次静态子项的四项退出证据均已齐：入口不是快照、Group native 导入身份来源、两阶段对象映射、GUID 与代理的明确边界。**静态 4/4；实际操作 0/6；整包未关闭。** 以下实际表固定为下一次取得安全独占原机条件时的六项，不把未执行写成 PASS：

| 操作 | 必须记录的验收数据 | 当前 |
|---|---|---|
| 复制后源改名/删除，再粘贴 | CopyInfos 路径、当时对象解析、跳过项和目标集合 | 未执行 |
| 同父复制含子 Group | old/new ID、GUID 字节、Parent、FullName、回调顺序 | 未执行 |
| 跨父粘贴 Group | SDK 容器参数、回调身份、内层与外层引用实际对象 | 未执行 |
| 多选有内部连线和索引 | 两轮 map、RelateIO/RelateIndex、共享原引用例外 | 未执行 |
| 代理参数与显示扩展 | UpdateRunParamBind/PasteNode 收到的映射、实际执行值 | 未执行 |
| 改名→Undo/Redo→保存重开 | ID/GUID/Parent/FullName、订阅与持久化一致性 | 未执行 |

安全前置是自有副本 ApplicationBase/TempFiles、SDK/流程/Group 实例独占并核对原件不变；先证明构造和 TempVmSolution.Init 副作用隔离。不能在安装原件执行本链。B01/A01 依赖、SDK native ID/GUID/嵌套内部映射、实际代理重绑、错误恢复仍开放。本批没有声称原机行为等价；不因本次静态 done 再派同一个身份入口补证，也不以方法数替代整包进度。
