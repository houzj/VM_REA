# IfModule 原生 IFText 设置/读回：2026-10-20 源码补证

原件 `IfModule.dll` SHA256 `1594AD9652D50448A80133F51D1CCA21B944EA5CB0CD9EE4BE6864F4D2571A6B`。本笔只核静态反编译，未执行原 DLL。原始输出位于 `analysis/next-batch/condition-native/IfModule/output/18000ca30.c`、`18000ca30-asm.tsv`、`18000b7a0.c`、`18000b7a0-asm.tsv`。

## 已核实的调用与状态顺序

`0x18000ca30` 是参数设置体：空参数名或值先返回 `E0000001`；`LogicalMode` 直接 `atoi` 写 `this+0x5c`。`IFText` 分支在解析前调用 `0x18000e6f0(this+0x360)` 清除旧 0x1b8 字节条件记录向量，然后拷贝输入字符串，按分隔符逐行处理。每行在建立新记录前先检查冒号数至少为二；不满足时跳过。其后存在名称、上下界、备注和类型文本拆解，分别构造 int 或 float 记录，写入同一个 `this+0x360` 向量。整数记录路径调用 `atoi`，并通过 `0x18000ab30` 检查原字符串的小数部分，在正余数情况下将下界加一、上界减一；浮点记录路径用 `strtof`。这不是 `IfModuleControl` 的 `Name:min:max:remark;` 文本对象本身，也不能只靠 `IfConditionText.Restore` 推断 native 数值状态。

`0x18000b7a0` 的 `IFText` 读回体遍历 `this+0x360` 中的记录、拼接文本，并在复制给调用者前写所需长度 `assembled length + 1` 到输出参数；容量不足返回 `E0000012`，不应把零结尾排除在边界外。浮点记录格式为 `%.7f`，整数记录为 `%d`。Ghidra 对 `sprintf_s` 的可变参数只显示格式串；下文已按汇编核对 R9/XMM3 和记录偏移。

## 实施含义与未闭合点

托管 UI 的 `IfDynamicIO.SetParamUnit` 按当前可见行序列化 `IFText`；native setter 清表后重建，属于另一状态层。当前 `src/VisionPlugins.IfModule/IfConditionText.cs` 只实现 UI 字段序列化/恢复，不能宣称原生 IFText parser/round-trip 完成。真实运行还需要从 source 逐一核对行名/类型的拆解、非法值时是否部分提交、容量边界、`0x18000ab30` 的小数判断以及 `sprintf_s` 传值。若某行无效，必须保留已清空和此前已插入的状态顺序，不能先完整校验再一次性替换却称原厂等价。异常/畸形输入可能触及固定大小栈拷贝，应明确独立安全选择，不做危险原样复刻。未发现原厂真实宿主差分证据。

## 动态输出辅助体的 ABI 补证

同一原件的 `0x18000a170` 被 `Process 0x18000b0c0` 调用，逐条读取 `this+0x30..+0x38` 的动态字段表。`int` 分支 `VM_M_GetInt` 成功后按返回 count 逐 index 调 `VM_M_SetInt`；`float` 分支先 `GetFloat`，失败则 `GetInt` 并转换为单精度，再逐 index 调 `SetFloat`；`string` 分支只读 index0，初始容量 0x1ff，遇 `E0000012` 按报告长度加一重分配后再读，成功且 count>0 才 `SetString`。各项失败不直接终止整个辅助循环，函数返回最后一次读取状态；调用它的主 `Process` 另有自己的返回路径。

反编译 `18000a170.c` 显示 `VM_M_SetFloat` 缺 value 实参，这是导入 ABI 推型问题。`18000a170-asm.tsv` 的 `0x18000a332/0x18000a341` 把读取或整数转换后的单精度放入 `XMM3`，随后 `0x18000a35f..0x18000a365` 设 `RCX=输出句柄`、`RDX=字段名`、`R8D=index` 并调用 `VM_M_SetFloat`；循环中的 `0x18000a3ed/0x18000a3f2..0x18000a407` 同样在调用前保持 `XMM3`。因此值确实传入，不能按伪码的三参数外观丢掉浮点数据。`string` 扩容分支及未初始化/失败值的精确计数更新仍需独立核对，不能以这段静态证据宣称所有 SDK 运行差分通过。


## IfModule 动态行 UI 的源码状态/删除链（2026-10-20）

匹配原 `IfModuleControl.dll` SHA256 `A528C4D93007EC9FF4011DA66DB3E5EF7E19AC13BE6FC60389D7F0CAE7DACEB0`，反编译体 `analysis/next-batch/condition-managed/IfModuleControl/Apps.XmlParser.ParamTab/IfModuleDynamicIO.cs`。`AddIO` 强制当前 SelectTypeIndex 的 int/float，成功后生成 `<DisplayName>Out` 为 int、`Min/Max` 为同类型，且不做额外验证。`RemoveIO` 先调 base，再将该行 IntegerBettween/Floatbetween 每个输入界的 IsSubA 置 false、CurValueA 置 null，然后按 `DisplayNameA.StartsWith(被删除显示名)` 删除 Outputio 中候选行；这是前缀副作用，不能替换为只删三个精确名称却声称原厂一致。`InitSource` 先设置 DecimalDigits，再调用 GetParam；原活动桥传3。

`GetParam` 一开始置 IsAllowSet=false，从 `IUserBytesData.Params[IFText]` 取 byte[] 并 UTF8 解码，逐当前可见行按 NameA 寻找分号/冒号项；缺备注字段时会先写两个界，访问 array4[3] 抛错后退出该行内层循环，最终恢复 IsAllowSet=true。Owner 为 null 的早退分支未恢复 true，属于源码可见状态，不应被独立实现擅自归一化。读取异常会记录错误并恢复 true。当前 `IfConditionText` 只涵盖文本部分；动态行添加/删除、订阅清理、来源树、写回和 UI 触发尚未完整实施，不计整工具完成。

`IfDynamicIO.FilterSelecterIf` 原成员还给出来源选择生命周期：组合类型下拉使用 `GetFrontCombinationTreeOperation`，选值触发 `SetCombinationSourceOperation(NameA)`，初建用 `GetSelectedCombinationOperation(NameA)`；非组合类型分别换为 `GetFrontFilterTreeOperation`、`SetFilterSourceOperation(NameA)`、`GetSelectedFilterOperation(NameA)`。当旧 `DropOpenSetter.OperationParams` 非空且与当前 `FilterTypeA` 不同，先清 `CurValueA`，再刷新触发/初建 setter 的 NameA、运行 `InniterBusiness`，最后把 DropOpenSetter 参数设为当前 FilterTypeA。界值控件经 `CurValueChanged→SetParamUnit` 写回，备注 setter 长度>256或包含分号/冒号时提示并拒写，否则同样写回。此调用顺序需接工具专属配置 UI 和共享来源树合同，不能只添加动态端口描述便称界面完成。

原活动配置不是 XML 中已注释的 64 个 `RangeConditionF` 和 `AddButton_If`。`IfModuleAlgorithmTab.xml` 的活动项是 `ButtonSelecter_RangeConF Name=RangeConFFields`，`BrigdgeSetter=GetCategoryOperation(Input)`、`DecimalDigits=3`；反编译 `ButtonSelecter_RangeConF.cs` 的 `InniterBusiness→IfModuleDynamicIO.InitSource(DecimalDigitsA)→GetParam`，`GetSubChildren` 按当前动态行 `FilterTypeA` 选择 `Floatbetween` 或 `IntegerBettween` 的子控件，字符串比较忽略当前区域大小写。注释中的 MaxCount32 与 Range1..64 不属于此活动控件实例，不可据此强造 64 个可见条件行。活动桥的动态行总数上限与实际 WPF 增删触发尚需继续追其父类和模板。

## 静态 PE 字面量与正常记录类型（2026-10-20 补证）

只读脚本 `analysis/next-batch/condition-native/read_pe_literals.py` 按匹配原件 PE 节表把 VA 映射到文件偏移；`0x180012530`=`;`、`0x180012524`=`:`、`0x180012644`=`.`、`0x180012374..376`=`int`。与 `0x18000ca30` 的拆分体对照，每条记录首个冒号前的 `local_450` 是 **Name**，原生随后在此名称中查找 `int`，否则查找 `float`。它不是另一个独立类型字段。匹配托管 `IfDynamicIO.SetParamUnit` 的 `NameA:min:max:remark;` 写法，以及 `IfDynamicIO` 在构造 Min/Max 行时同样以 `NameA.Contains("int")` 选类型。此前“类型文本拆解”的宽泛表述应以这个具体字段链为准。

`0x18000ca30` 在进入任何记录前先清空 `this+0x360`，以 `;` 逐条取记录；少于两个 `:` 的记录跳过。名称既不含 `int` 也不含 `float` 时该记录不插入；`float` 记录的关键界值为空时在已清空且可能已有前序记录的状态上返回 `E0000001`，整数路径则继续按 `atoi` 解析。安全重建不可先完整校验全表再原子替换并称原厂等价。整数小数修整体已在下文补取；读回格式寄存器在下一段核对。不把 native parser 或读回宣称已实施。

读回格式的 ABI 现已另核 `18000b7a0-asm.tsv`：记录的 int 分支先从 `RBX+0xb0`、再从 `RBX+0xac` 放入 `R9D`，格式地址 `R8` 指向 `%d`；float 分支先从 `RBX+0xa8`、再从 `RBX+0xa4` 用 `MOVSS XMM3`、`CVTPS2PD XMM3,XMM3`、`MOVQ R9,XMM3` 放入可变参数槽，`R8` 指向 `%.7f`。对应 setter 另名 `_Min` 写 +b0/+a8、`_Max` 写 +ac/+a4，读回顺序是名称、下界、上界、备注。因此反编译伪码中缺失的两个格式化值确实存在，并经单精度存储后转 double 格式化。函数将组装长度加一写入所需容量，容量不足返回 `E0000012`；这仍是静态 ABI 结论，非原 DLL 运行通过。

## 原生小数拆分体 0x18000ab30（同日补体）

已在同一私有只读 IfModule 工程经 `RunTargets.ps1 -Only IfModule` 取得 `analysis/next-batch/condition-native/IfModule/output/18000ab30.c`、`18000ab30-asm.tsv`、`18000ab30-calls.tsv`，原件哈希仍为上文值。该函数并非数值求余：复制原字符串，以调用方传入的 `.` 分隔串交给 CRT `strtok`，逐非空 token 构建 0x20 字节步长的字符串向量，返回向量 begin/end/capacity。`0x18000ca30` 仅在向量长度大于 1 时对第二 token 作 `atoi`，其值大于零才把 int 下界的 `atoi(原文)` 加 1，或把上界减 1。`strtok` 跳过前导/尾随空 token，所以 `.5` 和 `1.` 都只有一个 token，不触发修整；`1.5` 与 `-1.5` 则触发。此处是已核源码控制流，尚未用原 DLL 运行差分；整数溢出、非数字 token 和固定栈缓冲输入边界仍是未证实运行行为。

生产 `IfConditionEvaluation.ParseIntegerBound` 现对有限正常文本复现这一步序：先取 `atoi` 对应的整数前缀，再按非空第二 `.` token 的正值修整下界/上界；`tests/IfModule.TextChecks` 新增 `1.5/3.5`、`.5/2.`、`-1.5` 的源码定向检查，当前 26 PASS。解析非数字后缀和整数溢出仍使用托管明确拒绝，属于独立安全边界；本检查未运行原 DLL，不能替代整工具的 native IFText 表状态/配置/Host 验证。

## 托管 Tool/Param/Result 的模型与动态参数链（2026-10-20）

匹配托管原件 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Logic/IfModule/IfModuleCs.dll` SHA256 `93588B02297E8AA4B023266B58ABDC1E74CEFFA9B65C3013E475427F8A3FC8B1`，反编译 `D:/MyWork/REA逆向测试/analysis/decompiled/IfModule.WorkflowAudit/IfModuleCs/IfModuleTool.cs`、`IfParam.cs`、`IfResult.cs` 给出另一条不能混同于 UI 文本缓存的链：`IfModuleTool` 的三个构造入口均建立 `IfParam(ID)`；`IsDynamicModu` 和 `IsContainsModel` 均为 true。`GetModel()` 从 `((IUserBytesData)Params)["IFText"]` 读原始 byte[]，`ImportModel(object)` 仅在实参确为 byte[] 时写同一键，两者捕获所有异常而不报告；`Run()` 调 `mModuleManager.SyncSelfRun(ID)`。`ModuResult` 每次新建 `IfResult`，查询 `QueryModuleOneResult(ID)`；有结果时经 `mModuleResultManager.SetModuleResult(ref ..., ref _result, ModuResultMemType)` 填充并返回。`IfResult` 初值为状态/整数结果0、字符串空、两个列表为空，成员包括 `ModuStatus`、`StrResult`、`NIfResult`、`IfItemResult`、`IfItemStatus`。这证实模型是 IFText 字节级传输，不能把 `GetModel` 简化为当前可见行的文本重序列化，也不能把结果列表当作已由托管工具直接计算。

`IfParam.DynamicParams` 每次访问先 `GetBinaryData(ModuleID,"DynamicParams",ref data,60000u)`；非零 SDK 码抛 `VmException`，成功则清空原字典、UTF8 解码，再按 `;` 去空项、按 `,` 去空项解析各名称/类型。仅第二 token 精确等于 `int`、`float`、`string` 的条目建 `ParamItem`；数值型构造参数分别为 `(999999999.0,-999999999.0)` 和 `(100000.0,-100000.0)`，字符串型只传名称。`GetIfItemParam(strName)` 对空名抛 `VmException(-536870902)`，随后必重新读取 `DynamicParams`，只有 `strName+"_Max"`、`+"_Min"`、`+"_ReMark"` 三键俱在才按 Max/Min/ReMark 顺序建 `IfItemParam`，否则返回 null。弃用的 `GetDynamicParam` 同样重读二进制字典，精确名称存在时返回该 `ParamItem`。`LogicalMode` 是单独 `ParamItem("LogicalMode")` 的枚举读写，And=1、Or=2；它不来自 DynamicParams 二进制项。

`IfItemParam.cs` 的 `MaxValue` 与 `MinValue` 分别调用所持 `ParamItem.GetDoubleValue/SetDoubleValue`，私有 `ReMarkValue` 调 `GetStringValue/SetStringValue`。默认无参构造将三个引用均设 null，访问时抛 `VmException(-536870911)`；正常三参构造只保存 Max/Min/ReMark 引用。此封装不会在 `GetIfItemParam` 阶段读取实际数值，真正 SDK 读写发生于这些属性的后续访问。

当前产品尚未接上这一字节模型/SDK 二进制动态参数查询及 `SetModuleResult` 结果映射；尤其不能据 `IfConditionDynamicInventory` 的可见 UI 行推断 SDK 的 `DynamicParams` 已同步。下一实施需先锁定这些边界在现有插件会话/宿主的相应入口，再做可验证的传输和结果状态；SDK 查询失败、列表更新、复制/保存时点需分开取证。

同日新增 `src/VisionPlugins.IfModule/IfConditionDynamicParameters.cs` 是上述成功读出的 `DynamicParams` UTF8 记录的**纯解析快照**，保存原 `;`/`,` 去空项、精确 `int/float/string`、重复名称后项覆盖、Max/Min/ReMark 三键齐备才组 `IfDynamicItemParameters` 的有限行为。`tests/IfModule.TextChecks` 当前32 PASS（先前26再增6），包括空payload清表与缺三键返回null。它既没有调用原SDK `GetBinaryData`，也没有生成原 `ParamItem` 句柄或接插件/Host；非法UTF8显式拒绝属于独立安全选择。模型IFText byte[]与DynamicParams二进制记录仍是不同键和不同状态，不能凭此纯解析快照将整工具计完成。

