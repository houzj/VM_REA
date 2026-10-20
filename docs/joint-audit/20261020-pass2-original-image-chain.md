# 第二包 A 线：原版 IMAGE 候选、前驱和像素格式边界

范围：J04/J05 的原厂侧。只读安装版资源、原始 DLL 和现存反编译；新导出的 CIL 仅在 `analysis/joint-audit/original-image-chain/`。没有启动原厂、改安装目录、修改产品或 `tool-plugins`。静态结论不能冒充桌面差分。

## 身份和可复验原始证据

| 对象 | SHA-256 | 用途 |
|---|---|---|
| 安装 `Applications/myLibs/VM.Core.dll` | `936763F6C8A1BF9FCF59F22ACEBCA8FEB09B5F3D5701AE77C82BDC81E626190D` | `VmModule` / `VmIO` 托管体 |
| 本轮 `analysis/joint-audit/original-image-chain/VM.Core.VmModule.il` | `23C811E317DCB5AEFF53536A38D8230C1686C569F953AEDFB76D416020B382BD` | `dotnet-ildasm 0.12.2` 对安装 DLL `--item VmModule` 的只读导出；仅进程内 `DOTNET_ROLL_FORWARD=Major` |
| 本轮 `analysis/joint-audit/original-image-chain/VM.Core.VmIO.il` | `2EA83CBA47DB3546C7C52055EB1CFD3D3EF2F239847FA5584CCDBFF93C44B8F4` | 同法 `--item VmIO` |
| 安装 `Module(sp)/Group.xml` | `9129B82A0F9906DDE591347498A80E2E8C019C0FB959A721ECFD24FBBB5207DC` | `ImageSourceGroup` / `InputMaskImageGroup` 绑定定义 |
| 安装 `ImageSourceModule/ImageSourceModule.xml` | `0598746F0603BF6DE66DFA34E74C69F382E98F58437496EE5E598A3B88F6CD64` | 输出 `ImageData` / `ImageDataMono8` IMAGE 组合 |
| 安装 `IMVSBinaryModu/IMVSBinaryModu.xml` | `30C47A31236E4EA48F6C30DA4FCBA33B03E17B5C3CD470C0A89C2247470DBB86` | `InputImage` / `InputMask` IMAGE 组合 |
| 安装 `ImageSourceModule.dll` | `91EA3659BE97E51EDEAB0C913779D4555D51DCD0D6288086FBF9916D35B942DB` | 现存 `analysis/full-audit/image-source-native-0.c` 对应目标 |
| 安装 `IMVSBinaryModu.dll` | `C03BCB19EF456206E3ABF4D2A8D5279BE37C12EB5662E191F0965029CDABEB48` | 待恢复二值化原生输入校验 |

REA `open_binary` 重新确认最后一个原生目标及 SHA。其 `binary_session` 显示 Ghidra `not_configured`（`GHIDRA_INSTALL_DIR`、Java、headless 均缺），因此本轮没有新的二值化 native 伪代码/汇编，也没有伪造 REA Evidence ID。旧报告指向的 `analysis/source-audit-regression/20261018/ghidra-binary/output/18001ec00.c` 当前不存在；该历史描述只能作检索线索。

## J04：前驱集合的实际构造与更新

1. `VmModule.PreModules` 首次 getter 在 `nextAndPreModules_IsLoaded == false` 时先设 true，再调用虚方法 `LoadNextAndPreModules`。原 CIL `VM.Core.VmModule.il:5587-5615`；反编译 `analysis/decompiled/VM.Core/VM.Core/VmModule.cs:513-522`。
2. `LoadNextAndPreModules` 从同一父 `VmModuleSet.Modules` 取尚未加载的模块并加入 `this`，将各 ID 送给 `ModuleManager.GetMultiModuleConnect`。SDK 包装再调用 `ServerSDKAPI.IMVS_GetMultiModuleConnect`，成功后解析动态返回结构并调用 `IMVS_ReleaseDynamicMemory`。每条 SDK 后继 ID 在父容器模块集中匹配，填 `_NextModules` 与目标 `_PreModules`，然后给 `_NextModules.CollectionChanged` 挂 `NextModules_CollectionChanged`。原 CIL `VM.Core.VmModule.il:8837-9029`；反编译 `VmModule.cs:2120-2163`、`VM.PlatformSDKCS/ModuleManager.cs:1136-1159`。因此第一次取候选用的前驱有原生 SDK 连接表来源，并非纯 UI 连线列表。
3. 连线集合后续增删进入 `NextModules_CollectionChanged`：移除时先 `DisConnectOneModule(source ID,target ID)`，再从目标 `PreModules` 移除源、调用目标 `UnSubModule()`、本源 `ModuleOperate(null)`；添加时先 `ConnectOneModule`，若不存在则给目标 `PreModules` 加源，再 `ModuleOperate(null)`。非全局模块注册撤销/重做；撤销委托又通过 `Dispatcher.Invoke` 修改源 `NextModules`。原 CIL `VM.Core.VmModule.il:10306-10634`，反编译 `VmModule.cs:2465-2516,5274-5297`，SDK 包装 `ModuleManager.cs:182-203`。基类 `ModuleOperate` 为空虚方法（`VmModule.cs:2982`）；具体模块覆写需逐个查，不能推断所有节点的候选 UI 会实时刷新。
4. `GetFrontParamItems` 递归遍历 `PreModules`，然后附加非 Solution 父模块；`GetFrontIOWithLeafItemsByName` 再附加全局 ID 13000 和当前/父容器的局部变量模块。父模块取 `Inputs`，其他模块取 `Outputs`；非叶 IMAGE 组合按 `TypeName` **字符串精确相等**查找，末端过滤 `CanSubscribeA && Visible` 并映射到 `ModulePathName`。旧 CIL `analysis/joint-audit/VM.Core.GetFrontIOWithLeafItemsByName.il` 和本轮完整类 CIL `VM.Core.VmModule.il:16055-16445` 一致；反编译 `VmModule.cs:3702-3721,3953-3997,4049-4075`。
5. 候选的两个布尔状态通知不能混写：`VmIO.Visible` 变更触发 `OnPropertyChanged("Visible")`（`VM.Core.VmIO.il:2895-2921`），`CanSubscribeA` setter 只写字段、**没有**属性通知（`VM.Core.VmIO.il:3375-3385`）。`PreModules` 自身无已见 `PropertyChanged` 通知链；它是集合，成员变化可观察，但本轮未证实 IMAGE 下拉订阅了其 `CollectionChanged`。`VmModule.PreModules` 私有 setter 的 CIL 实际把 `value` 传给 `_NextModules` 字段并标名 `PreModules`（`VM.Core.VmModule.il:5616-5630`）；这是原 DLL 内的怪异 setter，尚未发现日常候选链调用它，不能据此实现同样写错行为。

## J04/J05：配置候选与像素格式不是同一关

- `Group.xml:5-37` 的 `ImageSourceGroup` 在初始化时对 `EnumEntrys` 运行 `GetFrontParamItemsOperation(IMAGE)`，随后用 `GetSelectedCombinationOperation(InputImage)` 恢复选值；选值变化触发 `SetCombinationSourceOperation(InputImage)`。`Group.xml:2621-2702` 的输入掩膜选择也按 **同一个 `IMAGE` 候选查询**初始化，改变时写 `InputMask`。`GetFrontParamItemsOperation.Operate` 将候选写入 `EnumEntrysA`；`EnumerationG.EnumEntrysA` 换列表时把旧 `CurValue` 清空并通知 `EnumEntrysA`，随后选值初始化是恢复步骤（相应反编译 `Apps.XmlParser.ParamTab` 类）。这证明两种输入控件同用 IMAGE 候选集合，不证明两者运行像素格式约束相同。
- `ImageSourceModule.xml:49-65` 输出 `ImageData` 和 `ImageDataMono8` 两组 IMAGE 组合，各自包含图像、宽、高、像素格式叶；`IMVSBinaryModu.xml:31-47` 的 `InputImage`/`InputMask` 也分别包含像素格式叶。`GetFrontIOWithLeafItemsByName("IMAGE")` 的上述 CIL 只检查组合 `TypeName`、`CanSubscribeA`、`Visible`，**没有检查像素格式叶的当前值**。因此候选可出现与运行时不兼容的 IMAGE 项；不能把“下拉可选”解释成“RGB 必可执行”。原厂当前实际候选的排序/即时刷新仍须原桌面差分。
- 已保存的原 ImageSource `CMyAlgorithmManager::Process` 伪代码 `analysis/full-audit/image-source-native-0.c:620-648,956-1007` 显示主图经 `SetImageOutput` 交付，RGB24（常量 `0x02180014`）且附加灰度开关成立时另走 `ConvertRgbToMono8 → SetImageOutputMono8`；它是两个输出路径，不是下游二值化暗中自动转换的证据。该现存 Ghidra 伪代码 SHA-256 `600FAA687D61B61ACBD7207FE2EDEF569647D6BF91F5F34AFBE5719ECCFF61CA`，应在歧义处再核汇编/ABI。
- 二值化 SDK 头 `MVDAlgorithmSDK/Includes/Algorithms/ImageBinaryCpp.h:66-150` 声明 `SetInputImage(IMvdImage*)`、`Run` 等接口，但未给像素格式白名单。当前缺 `IMVSBinaryModu::ProcessAlg` 的可重验原生体/汇编、`MVDImageBinaryCpp` 到通用算法库的格式校验分支与相同帧原厂执行结果。因此 **原厂接收 RGB、显式拒绝、内部转灰度、输入掩膜格式约束和具体错误码均未知**。不能从本地 `GrayImage` 运行结果推成原厂合同。

## 对 FCR-031 的具体修订建议（本线未修改共享文件）

1. 将“`PreModules` 关系构造仍需取证”改为“首次访问从 `GetMultiModuleConnect` 懒加载，同容器连接由 `NextModules.CollectionChanged` 经 `ConnectOneModule/DisConnectOneModule` 同步前驱，撤销委托也改该集合；原厂各 UI 实时刷新/动态模块跨层路径未证”。增加上面 CIL SHA 与对应 RVA/行号。
2. 将“可见性通知未知”细化为“`Visible` 有属性通知，`CanSubscribeA` 无属性通知；候选操作 `ReOperate()` 空，尚无证据证明对连接/布尔变化自动重跑”。合同接受证据应加入连接、断开、Undo/Redo、窗口保持打开时变更可见性与候选内容，分开检查初始化和刷新。
3. 将 `InputImage` 与 `InputMask` 都列为消费 IMAGE 候选的端口；候选合同只按 IMAGE 组合和可订阅/可见筛选，不按图像像素格式。实际像素能力应是独立运行门，不要因候选存在就把主 RGB 帧静态放行到灰度消费端，亦不要擅自自动转灰。
4. 明确源 `ImageData` 与 `ImageDataMono8` 是两个原版输出组合，现有本地 `Image`/`ImageMono8` 只是映射；FCR 的接受矩阵应分别记候选、选值/持久化、实际 `PixelFormat`、Binary 执行和错误/释放。

## 仍需的下一证据

1. 在**自有受控副本/实例**恢复 `IMVSBinaryModu::ProcessAlg`、其输入格式分支及下游 `MVDImageBinaryCpp` / 通用算法调用，留伪代码与对应汇编/ABI；原生 REA provider 未配置时可用现有合法离线工具，但必须绑定 DLL 哈希，不能借旧报告替代。对 RGB24、MONO8、附加 Mono8、两种掩膜输入分别做相同帧原 SDK 差分，记录完整错误码和输出像素格式。
2. 原厂配置窗口在保持打开时连接/断开/撤销、前驱禁用、Group 父输入与全局/局部变量变更的候选刷新差分；检查 `GetFrontParamItemsOperation` 的调用时点和最终显示顺序。没有这一步，不能把懒加载后的静态集合推成 UI 实时合同。
3. 跟踪 `.sol` 保存重开后的 `RelateIO/RelateIndex` 与 SDK 多模块连线表一致性；以上 CIL 只证明正常增删路径，不证明异常回滚、保存格式或重开后的 UI 同步。

本线 J04 的 **候选集合来源、筛选和连接维护静态路径已证**；J04 的实时 UI/保存差分、J05 的原生像素格式运行门仍未关闭。本线没有新增框架实施单元。
