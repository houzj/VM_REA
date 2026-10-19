# B01 第四批：参数根惰性构造、装配副作用与隔离探针

2026-10-19，基线 `8b7b2de`。接续第三批首次填充，本批进入此前绕开的 `ModuleParam.Data == null` 原方法分支。普通原 `ModuleParam`、自有 XML、`Parent=null` 的原惰性构造有16项检查通过；真实 `VmProxyModule` 装配只有源码/CIL证据，未运行。B01整体仍开放。本批不修改产品、合同、台账，不访问tool-plugins，不构建共享dotnet项目。

## 取证身份与证据类型

- Apps.XmlParser原安装副本与CLR实际GAC副本SHA256均为 `78EC7AE1E5ADEFFF29197099C7109B153A3F75D092B7C3E9DC253D8C1B25A8D1`，MVID `d0604401-ab61-48a4-938b-faad16d74e7c`。
- VM.Core原安装副本与CLR实际GAC副本SHA256均为 `936763F6C8A1BF9FCF59F22ACEBCA8FEB09B5F3D5701AE77C82BDC81E626190D`，MVID `290025bc-e040-4cd5-94dd-cd49b9af9c99`。
- 特别纠正按反编译输出目录猜程序集的做法：`Hik.Xml.BaseNode` 的真实定义位于 **VM.Xml.dll**，不在 Apps.XmlParser/VM.Util/VM.Utility。最终CIL每个方法包含自己的modulePath/moduleMvid/moduleSha256，可复核其GAC实际定义。VM.Xml SHA256 `4ACC568B406E054299D1D22497FB9F069A0B9925503CCEDF31FD8382936739C5`，MVID `c390e0a9-9eed-4e8e-9ac2-0d6f9e61802f`。
- 本批REA静态身份Evidence `ev_a016815cda2ef29734610360e49774b919d4211f73e48f77826367ee0e86680c`，49,152字节、19 TypeDef/192 MethodDef、AnyCPU/.NET Framework 4.5、完整静态metadata，无资源；完整结果保留 `analysis/full-audit/batch4-20261019/B01/rea-vm-xml.json`。它不证明运行、依赖装载或C#反编译。
- 本批反编译源复用本地取证；ReflectionOnly只导出相关30个方法体（Apps.XmlParser/其BaseNode基类25，VM.Core 5），部分构造/通知方法与前批有交集，**不是30个全新行为或30项运行覆盖**，未重跑前批8组44项选择器测试。

## 源码与调用链闭环

下列路径相对 `analysis/decompiled/`，具体原token、逐条CIL及异常区域见本批 `*.cil-methods.json` / `*.cil-disassembly.txt`。

1. `VM.Core/VM.Core/VmModule.cs:1800` `InitBase`：`Params.Parent=this`、`TimeOut=3000`，`XmlPath=IVmModule.ModuleDir + StrModuleName + AlgorithmTab.xml`，再建 `VmParamSubManager`。`ModuleDir` getter在762附近从ModulePathList查找或扫描模块目录。真实路径源于模块模型，并非选择器临时拼出。
2. `VM.Core/VM.Core/ModuleParam.cs:125–139`：XmlPath/Data自动属性；`IParent.Parent` getter返回公开Parent字段，setter抛NotImplementedException。`Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/VmModuleExtern.cs:28`：Data为空才构造；**构造完全返回后**才写Data；Data非空但类型错误时直接as返回null，不修正Data。没有lazy锁或构造中提前发布根。
3. `AlgorithmTabRoot.cs:645–671`：IParamsConfig构造转发到 `(XmlPath,(config as IParent)?.Parent,config)`；先赋UserStringData/Owner，Owner为ISubjectModel时调用InitLoad，之后才进入try内loadXml/DynamicSetProperty。**InitLoad在catch范围外**。`DynamicSetProperty`在1026处为空方法，不能据名字推断其执行动态字段加载。
4. `VM.Core/VM.Core/VmProxyModule.cs:16–31,64`：InitLoad读取DepositModule；首次先置`isSubLoaded=true`，随后`VmModuleSet.LoadModuleDLL(StrModuleName)`，查第一个VmModule派生Type，ActivatorWrap以proxy为参数构造deposit。`VmModule.cs:1708–1721`寄生构造共享HostModule.Params/UserData而非复制。这解释了同一个ModuleParam在真实proxy与deposit间的身份关系。源码未见失败回滚isSubLoaded；本批没有运行失败注入，不能给出具体异常后实例表现。
5. `VmModuleSet.cs:681–748`加载链可读ModulePathList，扫描Module(sp)/x64，读取Librarys.Config并LoadAssembly依赖、Cs.dll、Control.dll。不能把GetParamRoot当纯XML getter或用未审查的真实VmModule构造替代受控输入；工具构造后续SDK/日志/设备副作用本批不覆盖。
6. `Apps.XmlParser/Hik.Xml.BaseNode.decompiled.cs:1181–1228` 的原定义实际在VM.Xml：loadXml先设置ActivatorWrap.IgnoreUnknown/defaultFileName，锁内判断XML/替代ZIP；对XML的FileInfo执行 **Attributes=Normal**，然后读/解密或解压、Init。异常重新抛出，最后恢复IgnoreUnknown不是finally。原文件缺失、损坏、未知节点等失败的全局状态恢复不能据正常路径推定。
7. 同源`BaseNode.Init:87`逐属性/子元素解析，构造字段对象时在273/323等处先设Parent再Init；`AlgorithmTabRoot.Init:577`临时改defaultNameSpace，base.Init，恢复命名空间，根Parent设null，然后先发XmlLoaded事件，再OnXmlLoaded。defaultNameSpace恢复同样不是finally。事件先于Tab通知有原CIL顺序证据，未以真实模块事件重放验证。
8. `LinkItem.cs:19–59`/`LinkRealItem.cs` AfterAddedToArray注册XmlLoaded，回调解除注册、查原节点并替换链接，修复父节点及Tab.Collection；这两个`root_XmlLoaded`属于解析链接装配，**不能笼统称为SDK模块回调**。本批未使用链接XML。
9. `AlgorithmTabRoot.OnXmlLoaded:600`在IsOnInit=true期间通知Tabs、左右BottomCommand、ExternParams，finally重置false；`Tab.OnXmlLoaded:234`先逐NameValueItem.XmlLoadedTask，再NotifyTrigger。NameValueItem.XmlLoadedTask初始化Triggers/Initers/DeIniters/ExternIniters中的Setter；`Setter.Init`查目标且对既有Operation调用GetOriginalValue。因此真实资源通知可能读原配置/SDK，不能仅凭“XML加载”认为没有效果。
10. 已装配的根再由上一批确认的GetRunParamsTree取精确`Tab_Run Params`，RunParamsSelectData首次候选包装继续维持原NameValueItem身份。本批验证XML→根→Tab.Collection和父链，没有运行选择器缓存/绑定写入，也不重复声称第三批结果。

## 本批实际原方法运行

复验：`powershell -NoProfile -ExecutionPolicy Bypass -File tests/NativeEvidence/B01LazyRoot/Run.ps1`。Framework64 csc独立编译；自有GUID输出、45秒自有进程超时；原程序集仅加载/读取。没有未初始化VmModule壳、没有构造VmModule/VmSolution、没有SDK初始化、设备、窗口或其他任务进程操作。

最终目录 `analysis/full-audit/batch4-20261019/B01/run-ffc99d0fef714119b4d42e375fa1d123/`，exit0，16/16 true，stderr空：

- NeedNotifyLoaded保留原默认true，未像第三批关闭通知；自有XML只有普通Integer及显示名称，无Initers/Triggers/链接/动态参数。
- 正常new原ModuleParam（Parent/Data默认null），XmlPath指任务自有文件；经原GetParamRoot调用真正惰性构造并缓存。Owner=null，UserStringData保留原参数对象引用，Tab.Collection有ActualLazy且其顶层父节点就是所建根；通知后IsOnInit=false。
- 自有XML预设ReadOnly；原loadXml把属性改为Normal，但字节SHA不变。此项是可复验的实际写副作用，安装XML没有用于loadXml。
- 改写自有XML后再取根，仍返回同一缓存对象和已有Tab，未重新解析。
- 另一个ModuleParam的Data设普通object，原方法返回null且不覆盖Data；无XmlPath的正常ModuleParam仍缓存一个Owner=null的空根。

这是**原托管lazy分支+受控非VM Owner**，不是原厂真实模块参数装配。运行程序集清单记录实际GAC路径/MVID/SHA及原生模块；清单未见iMVS/native PlatformSDK或工具模块，不能把有限清单扩展为所有静态初始化完全无副作用。

## 审计与失败

原安装Apps.XmlParser/VM.Core/VM.Util/VM.Utility及当次探针源码前后SHA清单一致。VM.Xml真实归属是在最后metadata遍历发现，其实际加载SHA见逐方法/运行程序集清单及后续REA身份；**该DLL不在当次显式before/after数组，不声称有其运行前后哈希对**。所有源文件快照哈希见本批source-hashes.csv；全部生成文件快照哈希见manifest.csv，先列文件后生成manifest避免自身递归。

保留4个未达运行阶段的metadata尝试目录：run-8de4...因错误假定BaseNode在Apps.XmlParser且PowerShell NativeCommandError中止（初版stderr未完整落盘）；run-1e738...保存同一TypeLoadException；run-528c...与run-144d...分别错误尝试VM.Util/VM.Utility。最终改从AlgorithmTabRoot实际BaseType链解析类型并逐方法记录真实module，才执行唯一一次原lazy探针。未丢弃失败、未累计失败中的metadata导出作额外覆盖。

## 重建边界与下一步

- 确认：缓存发布点、类型错误Data语义、Owner/共享Params身份链、XML属性写入、InitLoad及通知顺序已有源码/CIL；上述16项边界有原方法受控运行证据。
- 未实现：本批无产品补丁。真实工具配置装配属插件框架边界，不能在main复制配置系统；可由main唯一写入者向既有FCR-019追加本批数据流证据。
- 未验证：真实proxy/deposit构造、Params SDK读写、资源中Setter/Link/动态参数、异常后静态状态恢复、并发lazy重入、原窗口/全局变量订阅持久化/取消操作、SDK订阅回调。
- 解除真实装配运行阻塞需明确所选工具及完整构造链/受控模块路径、配置读写/临时目录/设备副作用清单和可隔离实例；不得拿安装原件或他任务TempFiles试建服务。无此条件，下一可独立静态切片为GlobalVariableItem.SelectedRunParams setter→GlobalVarExtern→VarRunParamInfo/SDK绑定，继续保留原CIL而不猜UI契约。
