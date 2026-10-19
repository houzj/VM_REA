# B01 batch1：原厂托管 Tab 装配的隔离运行证据

2026-10-19；main 基线 `2649c3b268df351a1585c30546a0115c4fb3753e`。本项推进 B01 的通用装配/候选谓词子问题，**不关闭真实 VM 模块 Params.Data、专用图像源构造或运行窗口验收**。未修改产品、共享台账或 tool-plugins。

## 身份与范围

- 原厂 `D:/Program Files/VisionMaster4.4.0/Applications/myLibs/Apps.XmlParser.dll` SHA256 `78ec7ae1e5adefff29197099c7109b153a3f75d092b7c3e9dc253d8c1b25a8d1`，MVID `d0604401-ab61-48a4-938b-faad16d74e7c`。REA target-free 静态身份 `ev_b3a71726ac88c3bfa26cb2a7f52cae18a1c0b089aa5306d29d074a63ad2ee5f7`，完整记录 `analysis/full-audit/batch1-20261019/B01/parser-identity.json`。
- VM.Xml 原件 SHA256 `4acc568b406e054299d1d22497fb9f069a0b9925503ccedf31fd8382936739c5`，MVID `c390e0a9-9eed-4e8e-9ac2-0d6f9e61802f`；REA 本次身份 `ev_a016815cda2ef29734610360e49774b919d4211f73e48f77826367ee0e86680c`。
- CLR 实际从 GAC 解析上述两 DLL；探针记录实际路径/哈希/MVID，与安装副本完全相同，未凭 LoadFrom 参数断言实际载入位置。
- owner 是 `new object()`，参数为本地字典实现 IParamsConfig；不满足 ISubjectModel，不触发 InitLoad、代理寄生构造或真实 ModuleParam。未创建窗口、不初始化 SDK、不调用 DoIniter/GetItemsValueUnit、不加载 ImageSourceModuleControl。不改变安装 XML；loadXml 仅接受本地副本。
- 依赖解析会加载 VM.Core/VM.PlatformSDKCS 等**托管程序集**；记录的进程模块路径中未见 iMVS/原生 PlatformSDK/ImageSource 模块。这不是所有托管静态构造副作用均不存在的全局证明。

## 源码链及执行选择

源码均在仓库 `analysis/decompiled/`：

1. `Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/AlgorithmTabRoot.cs:650`：UserStringData/Owner 赋值，ISubjectModel.InitLoad，然后 loadXml，异常记录。plain owner 有意隔离专用模块链。
2. `Apps.XmlParser/Hik.Xml.BaseNode.decompiled.cs:1187`：文件 loadXml 设置 IgnoreUnknown，改输入文件 Attributes=Normal，再 Init；复位不在 finally。探针先将副本设为 ReadOnly，执行后确认 Normal，原件内容哈希保持不变。
3. `AlgorithmTabRoot.cs:577`：base.Init → XmlLoaded → OnXmlLoaded；以 NeedNotifyLoaded 控制后两者。安装 XML 用 false，只证明通用字段/集合装配；最小合成 XML 用 true，验证正常回调。
4. `NameValueItem.cs:596/693/706` 与 `Setter.cs:83/92/97`：装配可创建 Operation；XmlLoadedTask 调 Setter.Init，主要解析目标并读取原值；InniterBusiness 才调用 Initers.Operate。但 `Tab.OnXmlLoaded` 后续 NotifyTrigger 可进入 TriggerBusiness（NameValueItem.cs:659），因此不能把正常 XML 装配称为绝无业务副作用。本次不在含真实图像源业务 Trigger 的 XML 上开启回调。
5. `LinkItem.cs:17/31`：AfterAddedToArray 订阅 XmlLoaded；事件退订、FindBaseNodeByName、Clone、按原索引替换容器和 Tab.Collection，并重设父节点。
6. `AlgorithmTabRoot.cs:975/999/1030`：精确选 `Tab_Run Params`；集合中动态子项展开；DisplayNameA 非空才包装 Holder；该层不检查 CustomVisible 或 SubscribeVisible。

最终 probe.json 的 cil 段记录原厂 Init/GetRunParamsTree/AddSubChildToTree、BaseNode.Init、LinkItem.root_XmlLoaded、Setter.Init/Operate 的 metadata token 和原 CIL 字节；不是用本地重新实现的函数代替原函数。输入反编译源哈希另存 source-hashes.csv。

## 新增运行观察

最终独占运行目录：`analysis/full-audit/batch1-20261019/B01/run-b475ebc57e8044c8bf4c04592a6f1399/`。

| 输入/操作 | 源码预期 | 实测及边界 |
|---|---|---|
| 安装 ImageSource XML 的逐字节副本，回调关闭 | 标准装配不自行改名 | 1 个 Tab，仍名 Tab_Basic Params，Collection 23 项，GetRunParamsTree 返回0。专用 Control 未注册，因此原 XML 共25项中的 CameraID、SolSaveImageData 被 ignoreUnknown 跳过；不能外推真实模块最终集合。 |
| ReadOnly 副本经 loadXml | Attributes=Normal | 副本变 Normal；副本与安装原件内容哈希仍为 `001a57675192867594d3bbc7efa94462c19f056583659d759ceb5648e55091e0`。 |
| 合成 RunTab，NeedNotifyLoaded=false | LinkItem 仍占位 | Collection 为 Hidden、Blank、Visible、LinkItem；未调用其候选方法，避免把未解析占位当 NameValueItem。 |
| 同一合成 XML，NeedNotifyLoaded=true | XmlLoaded 解决链接 | Collection 仍4项，最后项变 Visible 的独立 Integer 克隆；源项 visible=true，克隆 visible=false，ReferenceEquals=false。 |
| Hidden(CustomVisible=false、SubscribeVisible=false)、Blank(DisplayName空)、Visible与隐藏克隆 | 本层以 DisplayName 非空筛选 | 返回3个 Holder：Hidden、Visible源项、Visible克隆；Blank排除。HolderTarget 对象身份与集合一致，未按名称去重。此结论仅是原根树，不覆盖后续 RunParamsSelectData 订阅过滤。 |

运行 exit0，13个具名谓词均为 true。前两次开发运行均已保留（一次基础9谓词、一次添加模块/CIL清单）；最终13谓词结果以本节目录为准，不叠加成操作次数。无产品构建、原机桌面、实际 VM 方案或SDK算法执行结果。

## 可复验产物与下一步

执行 `powershell/pwsh -File tests/NativeEvidence/B01/Run.ps1`；仅调用 Framework64 csc 编译独占 EXE，运行限45秒；超时只终止该启动的自有进程。每次生成新 GUID 输出目录，原文件不会覆盖。探针源码与辅助脚本均位于 tests。

- 最终 probe.json SHA256 `44E054CC46AE04438E99BC45C96AF05C5740FF788FCE80B3379EEE9DCCFE4888`。
- TabAssemblyProbe.cs SHA256 `F08EE953724CF29C3DD573061765C5B7C5E4B439A74487E94D7B5DCD36A4E553`。
- Run.ps1 SHA256 `B13BD0FAFCFA5AB2E37E7F443B88C72B538BB37B5717CC3FC920BF508E7A02EF`。

本次独立探针不是产品重构；不据此删除 main 兼容候选，也不创建第二套插件配置框架。B01实际剩余门槛：指定任务自有原厂实例采集 GetParamRoot 前后 Params/Data/XmlPath、代理 InitLoad/寄生对象身份，注册真实专用节点后采集 XmlLoaded 前后 Tabs/Collection，并验证拼接开关与选择窗口重开/类型切换的候选缓存。原厂专用构造是否替换 Tab、订阅关系如何影响候选、实际设备/SDK回调均继续 unknown；后续由 main 消费 FCR-019 合同，插件项目仍是配置模型生产者。
