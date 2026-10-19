# B01 第五批：全局变量目标绑定写入（有限子项）

2026-10-19，基线 `e6bf62a`。本批子项 `B01.binding-write` 的目标是追清 SelectedRunParams 到 SDK 请求的参数、对象身份、调用顺序、取消和失败边界。**源码子项完成，真实操作子项阻塞；B01 一级包未完成。** 本批没有产品改动，没有重复前三批选择器或第四批惰性根探针，也未访问 tool-plugins。

## 进度指标和停止边界

| 固定问题 | 本批结果 | 操作验证 |
|---|---|---|
| BW1：原绑定入口、命令、确认/取消分支 | done，XAML→命令→ShowDialog→setter 可定位 | 未运行原窗口 |
| BW2：目标清单身份、空值和字段投影 | done，Owner 为原 TargetObject 引用，ModuleID 为目标模块 ID | 本批未运行转换 |
| BW3：清单写入与逐目标 SDK 订阅顺序 | done，先 ModifyVar 再删旧/绑新，参数完整 | 未初始化 SDK |
| BW4：订阅成功后状态与缓存处理 | done，写 @参数名 UserData 后 SetInit | 未运行真实对象 |
| BW5：取消/失败/释放边界 | done，静态定位范围内没有整体回滚，native 保留未知 | 无失败注入 |
| BW6：持久化权威与往返边界 | done，分清内存清单、SDK ModifyVar、UI UserData 三者；不冒充已取得 .sol 字节映射 | 未保存/重开原 .sol |

本批 **6/6 个限定源码问题完成，0 个原机操作通过，0 个一级包关闭**。36 个相关原方法体只是审计定位数，包含跨批 getter/封装方法，不是 36 项行为完成或 36 项运行测试。此表的 done 指“问题已有准确答案，包括明确的未验证边界”，不把未知 native 行为折算成已验证。下一批不再扩大本子项，转入下面固定的 B01 剩余验收门槛；未满足受控实例条件时记录 blocked 并选择别的依赖满足工作包。

## 原件与取证

复验入口 `tests/NativeEvidence/B01Binding/Run.ps1`，仅用 Framework64 csc 编译元数据导出器并 ReflectionOnlyLoadFrom；不调用原构造、属性、静态初始化或方法，不创建 VM/SDK/设备/窗口。成功产物：`analysis/full-audit/batch5-20261019/B01/cil-a0e4402108dd4c6782fda1329e9712d4/`。9 个 WPF、6 个 XmlUI、11 个 XmlParser（含5个捕获辅助方法）、5 个 Core、5 个 SDK 方法体，共36。每项包含 token、原 IL SHA256、指令和异常区。每程序集 JSON 分别记录声明原件与 ReflectionOnly 实际解析路径/哈希；不可凭路径参数断言实际 CLR 归属。

原安装文件前后 SHA256 一致，详见 `artifact-integrity.json`：

- VMControls.WPF `99FFA401C8784472913AD43813D68A8A5577890C00E149506E3E5664FA61C97D`。
- Apps.XmlUI `639D0FB9DFCA81E38DF690E201B73874CE006D1361B689DDC6D5F41F7B0B04FD`。
- Apps.XmlParser `78EC7AE1E5ADEFFF29197099C7109B153A3F75D092B7C3E9DC253D8C1B25A8D1`。
- VM.Core `936763F6C8A1BF9FCF59F22ACEBCA8FEB09B5F3D5701AE77C82BDC81E626190D`。
- VM.PlatformSDKCS `34C0C90B00CB75A701685B96670B6490F61376F62EACC0AB29D240157DB401C1`。

复用同哈希 REA 身份：Apps.XmlParser `ev_b3a71726ac88c3bfa26cb2a7f52cae18a1c0b089aa5306d29d074a63ad2ee5f7`（batch1 parser-identity.json）。本批没有重复已知会超出 representation budget 的整程序集成员请求，也未伪造新 REA Evidence ID。源码快照哈希、元数据检查和产物清单在本批 B01 根目录保留；不存在本批原方法运行结果。

## 原入口与数据流

以下源码路径均相对 `analysis/decompiled/`，对应精确 token/指令在 CIL 文本中。

1. `VMControls.WPF/VMControls.WPF.GlobalUI/GlobalVariableDialog.xaml:709–714`：目标文本 LeftClick 和链接按钮都绑定拼写为 `ModifiyRunParamsCommand` 的同一命令。`GlobalVariableItem.cs:730` 的 `ModifyRunParamsExecute`（token `060006D5`）先把现有选择投影到 `_OldSelectedRunParams`，再检查 DecorateSource；无数据源只记日志退出。VmIO.Root 作为 RunParamsSelectData.DecorateSource；Parent 同级中 **Name 不等于当前 Name** 的其他项提供占用清单，没有按对象身份排除同名项。
2. `Apps.XmlUI/Apps.XmlUI.ParamTab/RunParamsSelectWindow.xaml.cs:26` 构造保存 SelectRunParams/OtherSelectedRunParmas/SelectedVarType，并设 IsFromVarBind。`Close_OnClick` 只有 GetSelectedRunParams 返回 true 才设 DialogResult=true；调用方仅 ShowDialog()==true 才重新赋 SelectedRunParams。这是取消不执行赋值的静态保证；不证明缓存包装节点 OutputEnabled 未被修改，也不证明真实窗口取消完整事务回滚。
3. `GlobalVariableItem.cs:597` setter（token `060006D2`）按 **List 引用相等** 短路，不做集合值比较；先替换 `_SelectedRunParams`，再拼 `FunllParamsNameA` 为分号文本/换行tooltip，切换 IsBindTarget，最后通知 SelectedRunParams。空列表分支不清 DisplayTooltipName。原 CIL 没有此 setter 的 SDK 异常回滚；不能从 UI 外观推导绑定已成功。
4. `GlobalVariableDialog.xaml.cs:590,602,685` 用 WeakEventManager 注册 ValueChanged；`:803` 的 SelectedRunParams 分支重新投影新清单，取 GetOldSelectedRunParams 后调用 `VariableManager.ModifyLinkModuleForSub(name,type,old,new)`。该分支本身没有包围的独立 try/catch；同方法其他属性的 catch 不能误算进本分支。
5. `GlobalVarExtern.cs:170`（GetLinkModuleInfoByRunParams）保留输入顺序，每项投影 nModuleID=(int)ModuleID.Value、Name、RunParamIoType、IsSupportSub、Owner=TargetObject；null/空列表返回 null，没有去重或复制 Owner。ModuleID 缺失会在 Nullable.Value 取值失败，不能默认为0。`RunParamsBaseNode.cs` ModuleID 从 NameValueItem.GetVmModule().ID 或 DynamicIO.IoDataSource.Root.ID 来；IsSupportSub 仅 NameValueItem+ISubInfo+SubscribeVisibleA 满足，DynamicIO 分支返回 false。Type 由 ISubInfo.ComType 提供，不是 DisplayType。
6. `Apps.XmlParser.CapabilityAudit/Apps.XmlParser.Variable/VariableManager.cs:1414`（token `0600022B`）顺序是 `ModifyLinkModule(varName,new)` 然后 `SetRunParamBind(varName,varType,old,new)`。前者 `:1478` 将 VAR_LINKMODULE、修正后的变量名 UTF8 固定256字节、目标参数名128字节/类型16字节/模块ID及数量打包；列表分配到 HGlobal，指针写入 IMVS_MODIFY_VAR，再同步赋 DataSource.Params 的 IUserBytesData["ModifyVar"]，finally 释放所分配指针。空清单 nValueNum=0。SDK 是否同步复制该指针及磁盘容器格式本批没有运行证据，不能把 HGlobal 地址当持久化格式。
7. `VM.Core/VM.Core/ModuleParam.cs:55–110` 的显式 IUserBytesData setter（token `060005C1`）在 Parent.IsOnDispose 时直接返回；含模型时先登记 ModelUnRedoOperation；普通 ModifyVar key 走 ModuleManager.SetBinaryData，非零会抛 **普通 Exception**。因此不是所有失败都以 VmException 出现；也不能仅凭 ModifyLinkModule 返回就保证原件一定写成功（disposed 分支无写入）。
8. `VariableManager.cs:1330` SetRunParamBind（token `0600022A`）先遍历旧清单：只跳过 !IsSupportSub，不要求旧 Type 等于当前 varType；按ID从 VmSolution 获取模块，删除订阅成功才写 `UserData["@"+Name]="false" UTF8+NUL`，再 `(Owner as ISubInfo).SetInit()`。原代码未检查 as 结果为 null；Owner 错配可能在 SDK 已成功后抛错。
9. 再遍历新清单，要求 IsSupportSub 且 **varType==item.Type**，模块找不到跳过。先查目标现有绑定，符合其他局部变量/13000条件时可能 ModifyLinkModuleByParamBind 通知旧变量反向解绑（含字符串解析与数组索引截断）。随后 SetModuleParamBinding 参数为目标参数名/类型、SUBSCRIBE_VAR_VARIABLE、常量空串、**当前变量 DataSource.ID**、当前变量名、SUBSCRIBE_INDEX_ALL、索引常量 **"All"**，余索引ID0/名空串。成功后写 @Name=true，再 SetInit；失败只记日志继续，没有恢复已处理旧项或新项。
10. `VM.Core/VM.Core/VmParamSubManager.cs:35,163` 构造 nNum=1、容量128的绑定/解绑结构；Owner.ID 是被绑定参数的模块ID，目标变量模块ID在 stBindValue，不能反转。GetVarType只映射 int/float/string/double，其余 INVALID。Set/Del wrapper 捕获 VmException/Exception 返回 false；`VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/ModuleManager.cs:1953` 对应 `IMVS_SetModuleParamBinding`/`IMVS_DelModuleParamBinding` 返回非零抛 VmException。
11. `Apps.XmlParser.ParamTab/ValueBase.cs:251` SetInit只令 isInit=false、_SubData=null；这是缓存失效，不是立刻重新执行算法。NameValueItem 的其他具体 ISubInfo 类型仍需其各自实现，不能把 ValueBase 行为外推给所有节点。
12. 反向载入 `GlobalVarExtern.GetRunParams` 从 IMVS_LINKMODULE 的模块ID/UTF8参数名重新查真实 Params 根的 NameValueItem，找不到目标不加入选择列表，OutputEnabled=true。这连接 SDK 返回目标与上批参数装配，但真实 .sol save→SDK恢复→此转换的字节往返没有跑；不能宣称绑定持久化完成。

## 失败与实现边界

原调用顺序有至少三层可见状态：UI选择/文本、ModifyVar清单、逐目标订阅与UserData。已检查方法没有统一回滚，native 请求本身是否事务性未知。旧订阅删除失败会继续新绑定；新绑定失败保留先前成功项，Owner.SetInit/字符串解析的普通异常可能中断剩余处理。HGlobal 序列化在 try 前执行，不能把 finally 的覆盖范围夸大到每一种分配后打包失败。

本批不构造真实 VmModule、VariableManager.DataSource 或 SDK ServerManager：后者连接原服务、模型历史、模块查找与参数读写，安全隔离条件尚未满足。没有运行失败被掩盖；两个源码查找路径错误（错误报告名、旧SDK输出路径）属于只读定位错误，未进入任何目标执行，最终路径及原指令已核实。没有新增 UI/模型补丁；main 不能据此复制插件配置框架或直接写工具私有模型。

## B01 剩余固定验收清单

一级包关闭前保留以下四项门槛；后续应对这些门槛给出通过、失败或精确 blocked，不继续任意扩展源码切片：

1. **真实装配实例**：在审计完整构造/服务路径且自有 TempFiles 的受控原实例，确认 Params.Data/Tabs/XmlLoaded/身份与已取得静态链匹配。当前 blocked：没有已审计且隔离的原模块/SDK实例；安装原XML不可用作写副作用探针。
2. **真实选择操作矩阵**：首次、缓存、占用、消失、同名、类型及确认/取消，用同一原窗口对象观察旧/新 List 和包装节点身份；前三批合成输入通过不替代此门槛。当前 unverified。
3. **实际绑定与失败矩阵**：在上述实例覆盖清空、同目标、跨变量转绑、类型不匹配、删除/新增失败及Owner失效；记录SDK返回/目标列表/UI状态/UserData，证明或否定回滚边界。当前 blocked：依赖门槛1；仅做静态源码不再算新增此门槛进度。
4. **保存重开与主宿主消费**：原SDK绑定状态保存/重开复验；FCR-019消费字段/所有权由主智能记录，用户决定插件交接后才验主宿主真实消费；包括取消操作和重开缺目标。当前 unverified，跨项目交接未授权自动采用。

进度应报告“本子项源码6/6，真实操作0/4门槛，B01未关闭”，不报告推测的 B01 完成百分比。
