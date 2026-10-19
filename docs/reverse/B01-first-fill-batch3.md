# B01 第三批：原选择器首次填充与身份恢复

2026-10-19。接续 `B01-selection-batch2.md`；本批首次运行原 `GetSubRunParamsTree` 的缓存缺失分支，未重复第二批20个缓存/确认谓词。8组合成输入、44个具名检查通过，只证明原托管方法对这些输入的行为。B01整体仍开放；未修改产品、共享合同、旧探针、tool-plugins，没有共享dotnet构建。

## 原件身份与取证来源

- 原 Apps.XmlUI.dll：`D:/Program Files/VisionMaster4.4.0/Applications/myLibs/Apps.XmlUI.dll`，SHA256 `639D0FB9DFCA81E38DF690E201B73874CE006D1361B689DDC6D5F41F7B0B04FD`，MVID `a00f8b40-7a51-4c6d-8aeb-cad134724f1f`。
- CLR实际Apps.XmlParser从GAC加载：SHA256 `78EC7AE1E5ADEFFF29197099C7109B153A3F75D092B7C3E9DC253D8C1B25A8D1`，MVID `d0604401-ab61-48a4-938b-faad16d74e7c`。
- CLR实际VM.Core从GAC加载：SHA256 `936763F6C8A1BF9FCF59F22ACEBCA8FEB09B5F3D5701AE77C82BDC81E626190D`，MVID `290025bc-e040-4cd5-94dd-cd49b9af9c99`。两者与安装副本身份一致。
- 沿用第二批REA身份 Evidence `ev_0b720b58c29ebc598cf14b4033d1cf52c913afcb9c9d066a53d694437e997b36`；其成员分析超67108864字节metadata预算的完整失败记录仍保留，不重复同一大查询。本批源码为已有反编译源，CIL为CLR Reflection原方法体，**不是新的REA反编译输出**。
- 最终probe.json含126个方法/构造方法的原CIL、build-local token和已解码的调用/字段成员（不是全程序集覆盖，也不是执行覆盖计数）；实际程序集路径/MVID/SHA、进程原生模块清单一并保留。来源哈希见 `analysis/full-audit/batch3-20261019/B01/source-hashes.csv`。

## 源码级数据流

以下路径相对 `analysis/decompiled/`。

1. `VM.Core/VM.Core/VmModule.cs:305` 的Params仅返回 `_params`；ID仅从值类型moduleInfo.nNodeID返回。`ModuleParam.cs:139`附近的Data为自动属性。`Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/VmModuleExtern.cs:28` 的GetParamRoot先检查Data，非null直接 `as AlgorithmTabRoot`，不会进入构造；Data为空才调用 `new AlgorithmTabRoot(param)`。
2. `AlgorithmTabRoot.cs:645–671` 的真实惰性构造从IParent取得Owner，若为ISubjectModel即InitLoad，随后loadXml/DynamicSetProperty。这个真实模块分支仍可能通过模块/参数回调接SDK，本批没有执行。
3. `AlgorithmTabRoot.cs:975–1040`：精确选择名称 `Tab_Run Params` 的Tab.Collection；IDynamicChild展开，普通项走AddSubChildToTree。只有非空DisplayNameA进入树，Between展开子列表；普通Holder保留原NameValueItem引用，显示名经MultiLanguageDisplayNameA。没有CustomVisible/SubscribeVisible总过滤。本批XML只用普通Integer/Float，未执行动态/Between分支。
4. `Apps.XmlUI/Apps.XmlUI.ParamTab/RunParamsSelectData.cs:193–294`：普通IVmModule的ID作cache键，缓存缺失时先GetParamRoot/GetRunParamsTree；只有isAllType=true才把Holder转新RunParamsBaseNode。TargetObject仍为同一个NameValueItem，ParentTree改为当前module Holder；OutputEnabled取原Holder。
5. 同文件230–240：IsFromVarBind时排除other占用；先用当前已选的**模块ID+Name**从局部other列表减去本变量已选名称，再从候选中排除剩余other。局部ToList不会回写OtherSelectedRunParmas字段或改调用方list。
6. 同文件241–251：仅遍历当前moduleID的已选项，按Name对新候选 `FirstOrDefault`，只恢复第一项的OutputEnabled。不使用TargetObject或类型区分同名项，不补树中缺失的项。
7. 同文件252–265：IsFromVarBind且varType非空，保留other或selected同模块同名项，否则保留 `!IsSupportSub || RunParamIoType == varType`。因此同名selected可以让另一类型的候选留下；不可订阅项也可跨IO类型留下。首次填充分支本身不按DisplayType/valueType筛选；那是缓存命中且isAllType=false的另一分支。
8. 最后allrunParams.Add；非空list才更新module.EnumEntrysA。首次直接isAllType=false仍会读取根/树，但不建立缓存，也不显示这批候选。

`RunParamsBaseNode.cs:138–183` 的ModuleID调用NameValueItem.GetVmModule()?.ID；IsSupportSub要求ISubInfo与SubscribeVisibleA；RunParamIoType为ISubInfo.ComType。`NameValueItem.cs:846` GetVmModule优先返回 `_vmModule`，否则向上找AlgorithmTabRoot.Owner。其SubscribeVisibleA为CanSubscribe && SubscribeVisible；CanSubscribe检查直接父Category、其TabType是否Panal，以及Category.SubscribeVisibleA。`Tab.cs:53` TabTypeA只是字段转枚举，Category.SubscribeVisibleA只是字段读取；ValueBase.ComType默认int，Float覆盖为float。以上原getter/CIL及闭包原调用成员保留，未调用CurValue/IsSubA等会取SDK值的属性。

## 受控原方法与合成输入的边界

`tests/NativeEvidence/B01FirstFill/FirstFillProbe.cs`使用原解析器读取任务自有XML；Root.Owner为普通object，IParamsConfig为测试空值实现（字符串空、字节数组空，不转发原ModuleParam索引器），NeedNotifyLoaded=false。因而不向真实VmModule装配，也不执行Owner.InitLoad或root_XmlLoaded SDK装配。

普通模块检查仍须通过原VmModule类型，所以只用 `FormatterServices.GetUninitializedObject(VmModule)` 的未初始化壳。ID默认为0，探针原CIL门槛检查其字段形状、模块无声明finalizer；抑制finalization。Params getter及ModuleParam.Data getter以原CIL的7字节字段读取形式及字段身份作强制门槛。构造普通ModuleParam仅赋Parent和Data，**Data预置合成根**，避开原GetParamRoot的lazy构造。

每个合成NameValueItem的私有 `_vmModule` 明确注入该壳，让原ModuleID getter返回0；这绕过真实Owner寻址，不能据此证明真实模块的Params/Owner连接已经成立。没有调用VmModule构造、Dispose、Name/FullName、Inputs/Outputs、Params索引器、Ioc撤销、SDK初始化或设备。ModuleParam.Parent赋值不读取模块属性。

8组分别重新创建Root/ModuleParam/Holder/selector/cache，记录source target、selected/other包装、结果包装与parent的RuntimeHelpers身份。原GetSubRunParamsTree没有被重写或拦截。44个检查包含每组的target引用、parent、Holder包装引用和调用方other列表引用，不能把这44项当44种用户操作。

| 合成输入组 | 原方法结果 |
|---|---|
| baseline | 6候选，重复Name的Integer/Float都在；包装TargetObject保持原解析对象引用 |
| occupied | other占Busy，候选移除Busy，调用方other列表保留 |
| selected-wins | Busy同时被当前选中和other占用，仍保留；已选指向Float Dup，却仅Integer Dup首包装勾选；两个Dup都按名称保留，即使varType=int |
| type-int | 删除可订阅Float Dup/Wrong，保留不可订阅NoSub |
| type-float | 保留Float Dup/Wrong，以及IO类型为int但不可订阅的NoSub |
| not-bind | 即使给other及varType=int也保留全部6项 |
| first-typed | 缓存缺失时isAllType=false不缓存/显示候选 |
| missing-selected | 已选Missing不在原树，未补入新候选 |

## 复验与审计

复验命令：`powershell -NoProfile -ExecutionPolicy Bypass -File tests/NativeEvidence/B01FirstFill/Run.ps1`。只对该进程临时绕过脚本策略，未改系统设置。Framework64 csc独立编译，自有GUID目录，自有进程45秒超时；仅允许脚本终止它自己启动的probe。

最终目录：`analysis/full-audit/batch3-20261019/B01/run-3c6e700c2cc6439aa0323d6f4eb16cd7/`，exit0，44/44 true，stderr为空。前后清单的探针源码、Apps.XmlUI、Apps.XmlParser、VM.Core哈希一致。实际进程模块清单未见iMVS/native PlatformSDK/工具原生模块；这是清单观察，不能推广为所有托管静态初始化完全无副作用。所有XML、exe与日志都写任务自有输出目录。

- 最终probe.json SHA256 `D7E26F8251AC03D3776F07A9CD9E16C39270324D19A45B1FCB9B96E95DEA5DF3`。
- FirstFillProbe.cs SHA256 `7B89396C9DABFE6BC08D36A2CA3069D94A2C87115EBA1223D03D414B6CEB1BAE`。
- Run.ps1 SHA256 `2487034E137CA0AFEC33E8BE9A64A3940EEFB838708A9C87F1415229DD68A737`。

开发记录全部保留：run-2764...为18谓词初版，run-3f671...因本机python别名不可执行而意外重复初版；没有丢弃该命令错误或累计两次的断言数。随后使用PowerShell编辑，run-f3e679...为44谓词版；最终run-3c6e...仅补充126方法/构造CIL成员清单。没有测试失败被改容差掩盖。

## 尚未关闭与下一步

- 真实模块Params.XmlPath/Data/Owner与ISubjectModel.InitLoad、原root_XmlLoaded专用装配仍未运行。本批刻意绕开，不等同真实VM实例。
- ModuleSet/Group递归、不同真实moduleID同名、动态/Between参数展开、首次null/异常根和本地化资源差异没有本批运行证据。
- 原窗口取消、拒绝消息、全局变量setter和SDK订阅写入仍未执行；既有20谓词不能补这些边界。
- 下一步宜在自有受控模块实例记录真实Params/Data/Owner和SDK调用，先核对初始化副作用；再对原窗口确认/取消及绑定写入采前后快照。无此实例资源时继续提取GlobalVariableItem.SelectedRunParams setter→GlobalVarExtern→绑定成员的原CIL，不能凭常规UI意图实现。
- 对FCR-019可追加本批已确认的首次填充字段/生命周期证据，由main唯一写入；不建议据合成输入直接改插件配置或移除main兼容候选。

