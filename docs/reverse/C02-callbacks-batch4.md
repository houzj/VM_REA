# C02 第四批：工作状态回调、输出失效与队列生命周期

2026-10-19；main 基线 `8b7b2de`。本切片为原程序集静态源码/CIL/EH取证，没有运行 VmSolution、SDK、设备或原机窗口，没有产品改动。完整 C02 仍开放；不重复配置17项、等待20项运行。

## 证据与复验

使用 reverse-engineer-anything 技能，复用既有反编译版本与 REA 失败证据：SDK 成员清点 `ev_de13b657e3113b360efba931e280afdfd22fb5cd1901b38dcf2f58204aa718b7`、WPF `ev_dd0a0670e77771079905d007438370d5ca2fd94b3ea25e9acef8513d1a0cea0f` 都超过67108864字节预算，不能声称其已给出本轮方法体。本轮补充 ReflectionOnlyLoadFrom 元数据导出，获取 **33个原方法体**：Core30、WPF2、SDK1，包含编译器生成的委托、locals和异常区。不是新增 REA Evidence，也不是33项运行测试。

复验入口 `tests/NativeEvidence/C02Callbacks/Run.ps1`，仅用 Framework csc 编译元数据读取器。目标程序集通过 ReflectionOnlyLoadFrom/ReflectionOnlyLoad 加载，绝不 Invoke/实例化原类型。导出器中的 OpCodes 反射只执行系统反射元数据操作。输出每次新建 run-GUID；最新完整目录：

`analysis/full-audit/batch4-20261019/C02/run-39a74caa35a44a6ea064a1a576a2f3af/`

安装根 `D:/Program Files/VisionMaster4.4.0/Applications/`：

| 原件 | SHA256 |
|---|---|
| myLibs/VM.Core.dll | 936763f6c8a1bf9fcf59f22acebca8feb09b5f3d5701ae77c82bdc81e626190d |
| myLibs/VMControls.WPF.dll | 99ffa401c8784472913ad43813d68a8a5577890c00e149506e3e5664fa61c97d |
| PublicFile/x64/VM.PlatformSDKCS.dll | 34c0c90b00cb75a701685b96670b6490f61376f62eacc0ab29d240157db401c1 |

各 JSON 保存 MVID、方法 token、raw IL SHA、locals及EH，txt保存含分支目标和token解析的CIL；artifact-integrity.json确认三原件前后SHA相同。第一轮26方法的输出也保留在C02根目录；随后加入Root、注册静态构造、运行状态投递、视图委托，形成33方法完整切片。一次 PowerShell 花括号路径语法失败在 attempts.txt 保留，未运行任何目标代码。

## 入口到 Begin/End 的精确顺序

原路径以下相对 `analysis/decompiled/`：

1. `VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/CVmObjectBase.cs:834–836` 转发 WorkStatus 事件；`ProcessManager.cs:135` 注册 GetWorkStatusCallBackInfo。该方法 `:630–650` 在 status==0 且 processID-10000 位于[0,1000)时先写 mProcedureNormalFlag/mProcessRunFlag，P2P条件满足时写命令索引标志，然后向 Core 发事件。本轮未验证 native→CVmObjectBase 到达时机。
2. `VM.Core/VM.Core/TempVmSolution.cs:20` 注册 `VmSolution.MProcessManager_OnWorkStatusCallBack`。对应Core token `060006e7` 静态构造已取CIL，但未执行。
3. `VmSolution.cs:4067–4173`（token `0600017e`）先调用静态 OnWorkStatusEvent；没有流程时返回。有流程先发 OnContinuousRunStatusCallBack，再检查 processID 是否存在/值是否非null。故这两个上层事件不代表目标流程有效。
4. count!=0 且 status==0：写 proc.ExecuteCount/ProcessTime，加入待通知缓存，canModuleSelfExecute=true；只枚举 **Parent==proc 的直接子模块**，对 ReportResultEnable==false 的 IMVSGroup 调 RaiseGroupSubModuleValueChanged；随后 InvokeOnWorkEndStatusCallBack。
5. count!=0 且 **status!=0**：只有 clear开关true且count>1，枚举 **Root==proc** 的模块，遍历 Outputs.leafIOsWithoutTimeStamp，逐叶 SetNoneValueWithoutRaiseEvent，再令模块 IsEmpty=true；之后 InvokeOnWorkBeginStatusCallBack。第一轮不走这段清理。
6. Begin/End后构造携带原proc对象和workStatus/count的tuple，EnqueueForCallBack；随后调用 proc.OnModuleResultCallBack。count==0 跳过 Begin/End和tuple，但仍到达同步 OnModuleResultCallBack。
7. 若流程或方案 ContinuousRunEnable 则返回；否则更新waitNotifyPages。这里 IsRunning **仅 status==1**，与上面 status!=0 的 Begin分支不同，不能擅自将所有非零状态等同忙碌。页已有OperID非零时改2，否则0。

`VmProcedure.cs:715/727/739` 的三个事件方法只是构造 ValueEventArgs/发事件，不保证事件订阅者不抛错。WorkStatus 外层 catch(Exception) 仅记录日志：任何更早事件/清理抛错都会截断后续步骤，没有该方法内回滚或保证 End 的 finally。CIL中的多个 finally 是枚举器Dispose/Monitor.Exit，不能当整个执行的恢复释放。

## “清结果”实际只清一个字段

`VmIO.cs:934` 的 `SetNoneValueWithoutRaiseEvent` token `06000524`、raw IL SHA `361e8712c1b44dc1d1c1e18d659f5e09e6ef8ea60eae4ba954a9fb8bf75062cc` 只检查 `_value != null` 后写 `_value=null`。它不清 `_valueList`、ExecuteCount、isNull、订阅、RaiseValueChanged，也不发 ValueChanged。

`VmModule.cs:151` 的 Root getter 沿 Parent 向上找 VmProcedure并缓存；流程自身不是自动Root==自身。因此准确范围是 Root==proc的模块输出非时间戳叶，不能表述为“清整个流程/整个session/所有输出缓存”，不能类推为 IDisposable资源释放。该静态字段写入也不等同消费者最终显示为空：VmIO.Value另有RelateIO和MergeIOLoopValue路径，本轮未运行其组合。

## 队列不是必然异步，停止不是取消执行

`VmSolution.cs:4345–4447`：

| 原入口 | 源码/CIL确定行为 |
|---|---|
| .cctor `060000ac` | 回调缓存开关默认false，长度10000；初始化后创建后台队列线程。还读取ModulePathGenerate.txt并创建多条其它后台线程，所以不为叶方法试验构造VmSolution。 |
| EnqueueForCallBack `06000188` | 开关false时直接DequeueForCallBack；WorkStatus的AsyncWorkStatusCallBack因而同栈发生在同步ModuleResultCallBack之前。开关true时在锁内写数组；满时只记overflow日志，丢弃该项，无阻塞重试。 |
| DequeueForCallBack `06000187` | tuple持有原VmIO/VmModule对象；按Value是否为WorkStatus选择AsyncWorkStatus，否则选AsyncModuleResult。此方法不核对当前solution实例、generation或取消状态。 |
| `<CreateCallBackQueue>b__492_0` `0600071c` | 交换收发数组后按索引逐项Dequeue，再置槽null；委托本体仅锁finally，没有catch。某订阅者抛错时不能保证后面的清槽和后续项执行；线程最终后果未运行验证，不能断言产品必崩。 |
| ExpandCallBackQueueLength `06000189` | 仅更大长度时把计数清零并重建两数组；源码注释要求未加载方案时调用，方法内无此状态校验，不宜在活动方案做探针。 |
| ShutDownCallBackQueue `0600018a` | callbackthread非null时仅置isCallBackThreadAlive=false和线程字段null；不Join、不排空、不解绑、不取消SDK执行，不等待正在分发的委托。 |

模块结果 SetValue 的正常非循环分支（`VmSolution.cs:4322`）与WorkStatus顺序相反：先同步 OnModuleResultCallBack，再Enqueue。不能复用一个“所有回调都先同步后异步”的框架规则。SetValue的局部输出复位还包含 `_valueList.Clear`、isNull等，不能与上述Begin清理合并解释。

## 多消费者与释放边界

`VmModule.RefreshResult:2596–2659` token `060002db` 受Root/运行/禁用/计数相同早退约束；通过 QueryModuleRecentlyResult 请求root计数。clear=false且nNum>0时改用array[0].nExecuteCount筛选历史记录；处理后仍把模块/Inputs/Outputs计数写为Root.ExecuteCount。SDK列表在finally调用Release；只catch VmException。该finally覆盖取到列表之后的异常路径，不能推广成回调tuple或整个session释放。

`VMControls.WPF/VMControls.WPF/ModuleResultView.xaml.cs:1340–1374`，UpdateModuleViewResultShow委托原CIL已补：clear=true用ExecuteCount，false用uint.MaxValue，把这个计数传给图像 GetValue、ActiveUpdateShapes。它是显示选择条件，既不删除历史也不证明原图像本体已释放。

本轮只确认源代码行为，不新增独立实现。下一步应对同一受控原实例记录输入状态/计数、Begin、同步/队列事件、叶字段与消费值，分别覆盖count0/1/2、status0/1及其它已确认枚举值、cache两态、订阅者抛错、换方案旧tuple、队列关闭和连续停止。实际SDK回调到达顺序、线程异常结果、取消到达保证、方案替换后的旧tuple处置、内存/图像释放仍未知。没有设备/SDK实例/交互桌面差分；33原方法体不能关闭C02或宣称完整调度一致。
