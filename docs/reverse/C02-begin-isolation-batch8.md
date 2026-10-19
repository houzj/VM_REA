# C02 第八批：真实 Begin 的隔离前提

2026-10-19，基线 `5e76aee`。有限子项为真实 Begin→回调消费前的初始化安全边界。本批仅原件静态元数据/CIL，**没有执行真实实例、SDK、logger、设备或原 Value getter**。不重复第七批五例35断言，不改产品或 tool-plugins。C02.3 三层状态仍为源码达成、实现部分、验证部分；本批不新增96项完成分子。

## 原件与方法证据

应用原根 `D:/Program Files/VisionMaster4.4.0/Applications`：`myLibs/VM.Core.dll` SHA256 `936763f6c8a1bf9fcf59f22acebca8feb09b5f3d5701ae77c82bdc81e626190d`；`PublicFile/x64/VM.PlatformSDKCS.dll` SHA256 `34c0c90b00cb75a701685b96670b6490f61376f62eacc0ab29d240157db401c1`。前后哈希在 artifact-integrity.json 一致。

按 reverse-engineer-anything 技能复用同版本源码和既有REA成员64MiB预算失败；不重复巨量成员请求、不虚造REA Evidence。本轮 ReflectionOnlyLoadFrom 元数据读取器输出 Core34、SDK8共42个聚焦原方法体，包含重复用于衔接的旧入口、构造器重载、生成委托，不称42个新增功能或运行测试。最终目录 `analysis/full-audit/batch8-20261019/C02/static-2ac9b5dad29d487cb91a782ef8323c04/`；两份 cil-methods.json 含MVID、token、raw IL哈希、EH、locals及 BeforeFieldInit。复验 `tests/NativeEvidence/C02BeginIsolation/Run.ps1`，只编译/执行自有元数据读取器，绝不 Invoke 原方法。

## 新确认：普通流程构造也先触发方案静态初始化

`analysis/decompiled/VM.Core/VM.Core/VmProcedure.cs:438/447/458` 三个构造重载，token `06000685/686/687`，均在 **IL_001D** 读取 `VmSolution.MaxModuleCount`，早于父构造 `006D/0073`。字段来自初始化器 `forSelfExcuteReceiveStore = new List<...>[VmSolution.MaxModuleCount]`，故不能根据反编译中“空的无参构造体”判定 `new VmProcedure()` 没有方案副作用。

`VmSolution` BeforeFieldInit=false，显式 cctor `060000AC`，IL SHA `7a969000ff719ab096bef078d55e9f01a3a3426be85190b0418380967f85c46d`。真实 WorkStatus 静态入口 `0600017E`、Instance getter、反射调用该静态入口或访问上述静态字段均不能绕过该初始化。cctor构造VirtualSol和instance；IL_0195/019A调用外部 `CurrentPathHelper.Instance.GetCurrentPath()`；IL_01AB打开由该路径拼出的 `ModulePathGenerate.txt`；构造VirtualPro；IL_0255启动refreshParamthread，IL_025B/0261/0267调用另外三个回调线程创建入口。只把当前目录改到自有目录，没有证据可保证外部CurrentPathHelper返回自有位置。

`VmSolution` 私有实例构造体虽空，原CIL仍调用 `VmModuleSet` 父构造；`VmModule` 无参构造设置Inputs/Outputs.Parent并调用InitBase。InitBase写IsActive、各父引用、Params.TimeOut=3000、条件XmlPath及VmParamSubManager。不能将未执行的普通构造等同第七批FormatterServices合成字段壳。没有通过静态字段写入替换单例；这样的反射写入本身也不能当作已经避开cctor的证明。

## 新确认：空 Init 方法不是空初始化

`TempVmSolution.Init` 的方法体确为返回，类型却 BeforeFieldInit=false、cctor `060006E7`。该cctor首先经 `ServerSDKManager.serverSDKManager` 创建版本服务并逐个注册SDK事件；ServerSDKManager cctor `06000585`→构造 `06000586`→ModuleManager构造 `06000058`→`CVmObjectBase.add_OnModuleRegisterCallBack`，后续ProcessManager注册 `add_OnWorkStatusCallBack`。`CVmObjectBase`自身显式cctor `06000739` 的静态初始化是注册之前的服务边界。

SDK CIL显示：ServerSetting非空配置分支在IL_0166调用 `ServerSDKAPI.IMVS_CreateHandle`；另一分支在IL_01A9/01D0调用 `IMVS_PF_CreateHandle` 包装；条件允许时IL_0267调用 `IMVS_StartGlobalScript`；IL_02BE注册native输出回调。当前宿主实际配置选择、包装内启动/设备行为、DLL映射原因与原机线程结果本批未知；仅凭程序集被装载，不能判定上述API已执行。这里确认的是原指令存在及可达分支，不是声称本批运行了SDK。

GlobalScript缓冲区在原代码AllocHGlobal之后、API返回后顺序FreeHGlobal；CIL没有包围该次分配和API调用的finally保证，API抛异常时不可把正常路径Free当保证释放。实际句柄/全局脚本最终生命周期未由本次静态切片闭合。未为得到一个Begin事件去启动这些服务。

## 到事件、结果及异常的边界

既有第四批确认WorkStatus count非零且status非零进入Begin；清值仅clear=true且count>1；Begin之后Enqueue再同步ModuleResult，cache=false时所谓Async事件同栈先发。此次固定真实入口前置，不重复算这套行为新完成。

原 `InvokeOnWorkBeginStatusCallBack` token `06000696` 只创建ValueEventArgs、写Value和ExecuteCount、调用事件；`OnAsyncWorkStatusCallBack` `06000698` 直接转发同args。两叶没有EH，订阅者异常直接外传；真实WorkStatus外围catch记录日志，因此后续Enqueue/同步消费可被前置事件异常截断。仅对未初始化壳手动触发事件叶，即使可安全运行，也不能证明真正Begin、清值时序、SDK回调或资源释放；本批未将它另造为新的低价值运行探针。

## 硬门、退出与下一条件

`Verify.ps1 -EvidenceDirectory <最终目录>` 对原件身份、显式cctor、三个构造字段入口、外部路径、线程启动及两个SDK边界共 **8项静态前提检查通过**，输出isolation-gates.json，actualBeginPermitted=false。这8项是元数据审计检查，不是8项产品验收。

实际Begin运行门保持关闭：下一次须先取得CurrentPathHelper及原初始化全部文件/线程/服务副作用的可控映射，原件/输出路径全部归自有副本；并有不借用他任务服务的自有SDK实例及清理协议、事件订阅和回调队列线程观测/停止方案。若选纯叶方法，则须明确标合成局部验证，不能代替真实初始化链。不得以复制Core、改变AppBase、空Init、空构造体或停止回调队列字段单独满足此门。没有上述新解除条件，不重复派发真实Begin运行。

失败也保留：首轮元数据34+8导出成功，后置hash管道枚举到自身正在写的CSV而失败，已修为快照列表并排除清单；原产物保留在static-37626f2ee4c44d6ead2d36ea8b760c2f。第二轮添加类型初始化元数据后完整成功。Verifier首次把线程Start偏移0255误抄0256而fail closed，按实际CIL修正，attempts.txt保留。早期PowerShell花括号路径语法及不存在的Core/ServerSDKManager、Core/ValueEventArgs源码路径查询只读失败均未触发目标代码。

