# 运行 Tab 装配链第二轮静态复核

2026-10-19。本轮仅复核原始反编译文本与随包 XML，不执行厂商代码、不创建工作树、不修改产品或 tool-plugins。补充上一轮遗漏的 Hik.Xml.BaseNode 装配层；不能把静态结论表述为原机窗口结果。

## 装配与回调顺序

以下路径相对仓库根目录；ParamTab 简写为 `analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/`。

1. `analysis/decompiled/Apps.XmlParser/Hik.Xml.BaseNode.decompiled.cs:87–123`：Init 依次遍历 XML 属性、子元素，通过 `GetType().GetField(name)` 查找字段。`SetValueByTypeCode:735起` 忽略找不到或 literal 字段，字符串直接取 InnerText（替换文字 `&nbsp;`）。不是依据显示标签推导字段。
2. `BaseNode.SetObjectValue:239起` 创建字段类型；BaseNode 子节点先设父节点再 Init；集合交给 `LoadNodeListE:443–541`。集合 XML 名称映射命名空间与首字母大写，Property 特判；TypeTable 未注册类型跳过。IList 分支先 Add(Init(...))，再调用 AfterAddedToArray；NameList 分支没有同样的 AfterAddedToArray 调用。克隆分支也不能等同 XML 装配分支。
3. `AlgorithmTabRoot.cs:577–638`：暂设默认命名空间→base.Init→恢复命名空间→清空根父节点；NeedNotifyLoaded（:29 默认 true）为真时先派发 XmlLoaded，再 OnXmlLoaded。后者在 IsOnInit=true/finally=false 内逐 Tab、底部按钮和 ExternParams 调用初始化任务。
4. `Tab.cs:148–190`：base.Init 后 InitCollection 将 Category.Items 扁平化并建立名称索引。`Tab.OnXmlLoaded:234–260` 对集合 NameValueItem 调 XmlLoadedTask、登记 ROI，随后 NotifyTrigger。`NameValueItem.cs:596起` 初始化 Trigger/Initers 等 Setter，因此需要区分“字段装配完成”与“业务触发完成”。`Setter.cs:83–110` 的 Init 解析目标并读取原值，Operate 才调用 Operation/设置目标属性；不能把所有 Initers 一概视为已经执行任意 Operation。

该通用装配链没有发现 `Tab_Basic Params` 隐式改名为 `Tab_Run Params`。Tab.Name 是 public 字段（Tab.cs:19），NameA setter（:71–81）只是赋值通知。此结论限于所述方法，不排除外部专用模块或动态操作写入。

## 三种扩展节点不能省略

- `LinkItem.cs:17–60`：AfterAddedToArray 记录容器并订阅根 XmlLoaded。回调先退订，按 LinkName 查找 NameValueItem 并 Clone，在原索引替换容器占位，设置 CustomVisible/父节点；同时替换 Tab.Collection 中的占位。没有创建新的 Tab。
- `LinkRealItem.cs:13–36`：Init 订阅根事件，回调退订后把命名项赋给父 NameValueItem 的 `TargetPropertyName + "A"`。这是对象引用链接，不能当作普通列表克隆。
- `GroupLinkItem.cs:13–39`：集合加入后立即移除自身，经 GroupsRoot 的 LinkName 查找并克隆 GroupInfoItem，将组 Items 追加到容器并重设父节点。找不到组时占位已移除。`GroupsRoot.cs:21–76` 缓存单例，从 CurrentPath 的 `Module(sp)/Group.xml` 加载并建立名称索引，加载异常被吞掉。

因此不能声称 XML Items 与最终候选一一对应；但这些已查扩展节点也没有证明基础 Tab 自动变为运行 Tab。全体已提取 `.cs` 搜索仅发现 LinkItem、LinkRealItem 订阅 XmlLoaded；这不是全部随包二进制覆盖证明。

## Params 路径、缓存与代理副作用

`VmModuleExtern.cs:28–36` 仅在 ModuleParam.Data 为 null 时构造 AlgorithmTabRoot，然后返回缓存。`VM.Core/VM.Core/ModuleParam.cs:112,126` 的 XmlPath、Data 均为可写自动属性。`VmModule.cs:885–895` 的显式 IParamsConfig.XmlPath setter 只转发 Params.XmlPath，未清空 Data。故单独更换 XmlPath 不足以证明现存缓存自动重载；外部调用方可替换 Data，但本轮搜索未找到专用图像源替换的已提取源码。

`VmModule.cs:1798–1812` 初始化 Params.Parent、TimeOut 和 `<StrModuleName>AlgorithmTab.xml`。根构造 `AlgorithmTabRoot.cs:648–671` 在 loadXml 之前调用 Owner 的 ISubjectModel.InitLoad；注意 path 参数已在进入构造前求值，不能笼统声称 InitLoad 改 XmlPath 会让当前构造重新读取新路径。

`VmProxyModule.cs:14–29,64–67` 的 InitLoad 会访问 DepositModule：先标记已加载，再加载模块程序集、取 VmModule 派生类型并以代理构造。`VmModule.cs:1713–1721` 寄生初始化共享 HostModule.Params/UserData。这条链证明类型加载/专用构造可能发生在解析 XML 前；没有证明任意模块构造必然修改 Tabs。重入、构造失败后的恢复和实际图像源派生类行为仍未知。

`BaseNode.loadXml:1182–1226` 还会修改输入文件 Attributes，并设置静态 IgnoreUnknown；重置不在 finally。本轮不调用它进行所谓只读验证，避免改动原件和静态状态。它支持 XML、缺 XML 时同名 ZIP、XML 加密标记解密路径；本轮仅静态确认分支，未验证加密/ZIP资源实际执行。

## 条件结论与运行探针最小方案

安装资源 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Collection/ImageSourceModule/ImageSourceModuleAlgorithmTab.xml:6` 仅一个 `Tab_Basic Params`；含 StitchEnable（:389起），未查到上述三种 Link 节点。若使用此 XML、标准装配链，且无外部写入/专用扩展，则 `AlgorithmTabRoot.GetRunParamsTree:975–1040` 对精确 `Tab_Run Params` 查找为空。仍不能把该条件结论当成原机实际变量树；初始化 Trigger、专用模块、资源替换与运行版本均应核对。

下一步只需在用户指定的原机测试实例采集三阶段快照：GetParamRoot 前（模块实际类型、Params对象身份、XmlPath、Data身份、文件哈希）、XmlLoaded 前后（Tabs的类型/Name/对象身份、Category/Collection节点类型与Name）、GetRunParamsTree 返回（稳定身份、DisplayType、ISubInfo/SubscribeVisible/ComType）。记录程序集路径/哈希、NeedNotifyLoaded、父节点和代理/寄生关系，避免提前调用懒加载属性改变现场。对图像源分别做拼接开/关、关闭重开目标窗口、取消/确认；禁止执行任意导出 XAML。测试辅助代码应留 tests，不能进产品程序集。

本轮没有实施第二套插件配置框架，也不据此删除 main 的兼容候选。FCR-019 仍需由插件侧提供实际运行 Tab 元数据和生命周期，main 消费工作流边界合同。

## 证据身份与验证范围

独占本地目录 `analysis/full-audit/variable-tab-round2-20261019/source-hashes.csv` 保留13个原件的完整绝对路径与 SHA256。关键值：BaseNode `2994D6AC9669EF78305E3BC4CAB998A4F42E15A0B2B95EBB669C870DC2AB8968`；AlgorithmTabRoot `31C8AAFDBCCD0E034316A54E2959F451E135298FAD9F618E8234BC6CBC1BC72E`；Tab `B5BCB77C35AAE9303C3870BCEFC7E491BA5664D8F71468D952A8F7B6174AF022`；安装 ImageSource XML `001A57675192867594D3BBC7EFA94462C19F056583659D759CEB5648E55091E0`。

根任务保留的 `static-managed-evidence.json`：REA Evidence `ev_57239de329d23530fb9532699cbbf82435b82f964e6a1f6f6022340fa665efc5`，原始 VM.Xml.dll SHA256 `4acc568b406e054299d1d22497fb9f069a0b9925503ccedf31fd8382936739c5`，MVID `c390e0a9-9eed-4e8e-9ac2-0d6f9e61802f`。本子任务未覆盖该文件。反编译文本及静态元数据属于静态证据，未新增原机运行、窗口验收、构建或产品测试结果。
