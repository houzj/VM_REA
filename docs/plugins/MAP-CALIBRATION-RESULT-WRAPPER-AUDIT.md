# 映射标定结果包装与普通结果面板语义

本报告追踪两层不同的结果载荷：Map 算法 SDK 的 `IMapCalibTool::GetResult()`、模块输出/历史结果，以及托管 Map 公开 `ModuResult` 和 VM 普通结果面板。它只证明对应源码路径的条件行为；不把算法 SDK 空指针等同于 VM 模块历史查询为空。

## 目标版本与一手证据

目标安装版本是 `D:\Program Files\VisionMaster4.4.0`。Map 模块目标程序集 `Applications/Module(sp)/x64/Calibration/IMVSMapCalibModu/IMVSMapCalibModuCs.dll` SHA-256：`6C8EA8FD06A2FA0D9E2C2F9A2F4B23D735E0D4FCB4D838721592B401B1C429F6`。其反编译项目在 `analysis/full-audit/calibration-dependencies/decompiled/IMVSMapCalibModuCs`；源码 SHA-256 如下。

| 证据文件 | 直接可确认的行为 | SHA-256 |
|---|---|---|
| `IMVSMapCalibModuTool.cs` | `ModuResult` getter 每次先创建新 `MapCalibResult`，再查询模块历史结果；仅 `HasValue` 时调用 `SetModuleResult`，最终返回本次新建对象。 | `04201A7CC19A75ACEBBE335C1C65FF179179EEBE679C22E6A783D5B3E98E06E2` |
| `MapCalibResult.cs` | 构造器初始化 `ModuStatus`、`CalibStatus`、比例、误差、计数、尺度等数值为 0；角点/ROI 列表创建为空列表。 | `E79646991863E73319F8D28C32734DB1A9CFFD88B529A5CAA2DA63618B344706` |
| `VM.PlatformSDKCS/ModuleManager.cs` | `QueryModuleOneResult(id, bResBuffFirst=true)` 查询历史结果索引 0、数量 1；API 返回非零时抛 `VmException`；结果指针为零或 `nNum != 1` 时返回 nullable null。 | `783A45B1518F713223ECA2B5E59291BE973552C3E08FEF0EA0D4BFCCC2FBD922` |
| `VM.PlatformSDKCS/ModuleResultManager.cs` | `SetModuleResult` 从该条模块历史结果设置 ErrorCode、ExecuteCount、ModuRunTime，再按反射属性名/类型匹配非空、非零值数的字段。缺少字段时不写属性；在 Map getter 使用的新对象上，构造默认值保持不变。 | `FD3960EE308FB739729AF43589035D628544BB5B26941BE5EB6F3250A41739E7` |
| `VM.Core/VmSolution.cs` | `MModuleManager_OnModuleResultCallBack → SetValue` 在特定新执行/自执行条件下清空 VM 输出 `_value` 与 `_valueList`；随后映射本次 `pInfo`，缺席字段在 `isLoopEnd != false` 时走 `SetValueFromDefault`。 | `C141E2C53913B233B768782D6757FAE5997ECAF638B35DB2DDF98F5EF951A13E` |
| `VM.Core/VmIO.cs` | `SetValueFromDefault` 对 int/float/double/string 创建单元素 null 默认值；`DisplayValue` 格式化数值项，null 项不产生文本；`Value` 访问可触发 `Root.RefreshResult`，而 `DisplayValue` 直接格式化当前存储值。 | `15E1832243422B6BFA2714F300B796D54C365950573639EB1039C5DF58E99CB0` |
| `VMControls.WPF/ModuleResultView.xaml.cs` 和 `.xaml` | 完成回调后结果树刷新叶输出 `DisplayValue`；UI 的结果树绑定 `ResultData`，当前结果表绑定 `ModuleCurrentResultList`。`RefreshDataListResult` 在 `needClear=true` 时清理 XML `datalist` ValueStore，数据为 null 时清空表格当前行。 | `E165657162322FD6B1CE15758847BED07F8C1E86A637815DF8E87FE19439BB88`; XAML `56AC6BBF9B8839311F4F8EAF0DC4407FC3F9527B39F3AD942080C0BB6C2948FF` |
| `VMControls.WPF.DataForUI/ModuleDisplayDecorator.cs` | `RefreshCurrentResultList(null,...)` 清空当前列表；非空时按输入行数裁剪旧行，再覆盖当前列。回调记录 `AddRecord` 根据 XML record 内容写 `ErrorInfo`，无文本时设为 null。 | `CEDA24B7FB93BCD9F02B77B23D42A839B1F737BF70CF3ABC6B7786D11765740E` |

当前 Map `AlgorithmTab` 资源 `analysis/map-calibration/parser/model/IMVSMapCalibModuAlgorithmTab.xml` SHA-256 `CC91E61790265B3DAD51EC37BB87CF65EDD6B1FAD718630253B390DDC0A46BF2` 定义输入、运行参数和结果叠加对象；它不是平台实际提交/回收模块结果的代码，也不证明失败时会产生哪条模块历史结果。

## 对失败和空结果的分层结论

### Map 公开托管 `ModuResult`

Map getter 自己没有跨调用缓存 `_result`。每次访问都会新建默认零/空对象，因此 **如果 `QueryModuleOneResult` 返回 null**，调用者拿到的是当前新对象的零值和空列表，不会拿到 getter 上次返回的 `MapCalibResult`。如果 query 返回一条记录，`SetModuleResult` 把该条记录中命名匹配且有值的字段写入新对象；记录缺字段时剩余字段仍是本次对象的默认零/空。Query API 报错会抛异常，而不是走 null 默认对象分支。

`QueryModuleOneResult` 查的是 Platform SDK 的模块历史结果缓冲，不查算法 SDK 的 `IMapCalibTool::GetResult()`。因此上一轮 NativeProbe 观测到算法 SDK `GetResult()` 在 Image 算法失败后变 null，只能说明算法实例当前结果不可用；它不能单独回答托管 `ModuResult` 的 query 会得到旧的模块历史记录、新记录还是 null。

### VM 输出 IO 与普通结果树

源码确认的是一个有条件的更新策略：收到某条模块结果回调后，VM `SetValue` 对符合条件的执行先清除旧输出值和列表；把该记录中的每个输出名写入相应 `VmIO`；对本条记录未出现的 IO 调用 `SetValueFromDefault`（循环未结束分支除外）。Map Image 失败定向回放确实在模块 IO host 观察到本轮 `ModuStatus=0`、`CalibStatus=0`，故若宿主将这次输出结果作为模块结果回调提交，结果树应以明确的 0 覆盖前值。若某个定义输出没有进入结果记录，则该 VM IO 默认是 null 项；数值 `DisplayValue` 会显示空文本，而不是默认数字 0。

`ModuleResultView` 的结果树刷新只是对叶 IO 触发 `DisplayValue` 属性变更；`DisplayValue` 读取当前 `_value`，本身不查询模块结果。普通 `Value` 访问在执行计数不匹配时才可能调用 `RefreshResult`；而 Query 无记录时 `VmModule.RefreshResult` 提前返回。故如果宿主对失败执行**没有**产生/派发模块结果记录，现有托管路径没有证据表明旧 `_value` 会在结果树刷新时自动清掉：旧显示值可能留存。若回调确实送达，则上段的清除、映射、默认值分支生效。

当前源材料不能证明 Map 原厂模块遇到 Process 提前错误 `0xE0000314` 是否仍会形成 Platform 历史结果记录/回调，也不能证明算法错误 `0x10103002` 的模块 IO writes 最终是否被 Platform 当成本轮结果入库。上一轮 synthetic host 只替换 ModuleFrame IO，不运行 VM Platform 结果管理器；NativeProbe 结果文件 `analysis/map-calibration/state-lifecycle-asserted-20261019/results.jsonl` SHA-256 `3D7C3960123DCB7D79E1404DE72295CD2DF5740A195220EAD6AA3F81824B5B1F` 也不能补这个宿主事件边界。

### 普通结果列表 / 错误提示区域

UI 当前结果表来自模块 XML 的 `datalist` Display Object，而不是 `MapCalibResult` 自动属性。源码仅能确认：在显式 `needClear=true` 的刷新中会先清 XML `datalist` 的缓存，`curDatas == null` 会清空当前行；刷新不清除时，数据行按当前数量缩短或覆盖。历史记录的 `ErrorInfo` 来自 XML `dataRecords` 对模块结果结构的解析；`AddRecord` 对本轮没有 record 文本时写 null，XAML 只在 `ErrorInfo` 为 null 时隐藏告警行。当前 Map 原厂资源没有证明失败结果行的具体模板文本或宿主在无结果 callback 时如何触发 datalist 清理，所以不推断其失败 UI 文案/旧行策略。

## 尚需的最小运行证据

要把上述条件分支确定为 Map 的原厂失败呈现，需要在真实 VisionMaster Platform 对同一节点捕获一段完整链：每轮 native Process 的返回码、Platform `QueryModuleOneResult` 返回的 `nNum/nExecuteCount/nErrorCode/nResultNum` 与关键 Map 字段、`MModuleManager_OnModuleResultCallBack` 是否触发及其结果记录，以及之后该输出 `VmIO._value/DisplayValue` 和结果表行。至少覆盖成功→Image 空帧提前返回、成功→Image 算法失败两段。不能用 SDK `GetResult()` 指针、合成 ModuleFrame 输出调用或 passing managed 单元测试替代这条证据。

本轮只新增源码证据报告；未改生产代码、其他工具或主框架；未做设备测试、未提交。

## VM 调度到 `SetValue` 的回调边界补查

静态追踪确认了 VM 端的消费路径，但没有确认原生 Platform 是否会为每一种 Map `Process` 返回形态生成结果事件：

| 证据文件 | 可直接确认的路径 | SHA-256 |
|---|---|---|
| `VM.PlatformSDKCS/CVmObjectBase.cs` | Platform 输出类型 `23` 被反序列化为 `IMVS_MODULE_RESULT_INFO_LIST_EX`，仅在 `OnModuleResultExCallBack` 非空时派发；该分支没有检查 Map `Process` 返回码，也不直接生成模块结果。 | `59E713604FA8AF86FEA0C4711CF16E8BE6C1B3E6BCB7670CC39475437F60A8DB` |
| `VM.PlatformSDKCS/ModuleManager.cs` | 构造器把 `GetModuleResultCallBack` 订阅到 `CVmObjectBase.OnModuleResultExCallBack`；收到事件后执行 `PreProData` 并调用公开 `OnModuleResultCallBack`。 | `783A45B1518F713223ECA2B5E59291BE973552C3E08FEF0EA0D4BFCCC2FBD922` |
| `VM.Core/TempVmSolution.cs` | 静态构造器将 `VmSolution.MModuleManager_OnModuleResultCallBack` 订阅到上述 `ModuleManager.OnModuleResultCallBack`。 | `2683DD70DC4AA88FA2D263E9117BDC00B1153DEEDA52E03417DFA7EE42859721` |
| `VM.Core/VmSolution.cs` | 结果事件到达后查找模块并进入 `SetValue`。对于普通非循环回调，清旧 `_value/_valueList` 的条件为 `module.ExecuteCount != result.nExecuteCount`、`bIsSelfExecute == 1`、`!NeedMergeValueInLoop` 或模块为 `VmProcedure` 任一成立；随后写本条命名输出，缺席输出设置默认 null。 | `C141E2C53913B233B768782D6757FAE5997ECAF638B35DB2DDF98F5EF951A13E` |
| `VM.Core/VmModule.cs` | 主动 `RefreshResult` 先按模块 ID 和 Root 执行计数查询历史；若返回 null 则立即返回，不触碰 IO。若结果存在且执行计数需更新，则逐条转成托管结果并调用同一 `SetValue`，但 `isCallBack:false`。 | `53E2F0F9032C4631B59CF0EB2A00E06D50469D94706E7E370E9CC7E1059C72CE` |

这条托管消费链只能证明“事件/查询结果已存在时怎样更新 VM”，不能证明 Platform 原生调度如何把插件 `Process` 返回码转换为类型 23 事件或历史缓冲项。Map 模块的原厂 `Process` 早退（如无图像的 `0xE0000314`）前没有执行 `ProcessOutput` 写 IO；Image 求解错误（如 `0x10103002`）则已经写出状态 `0/0`。前者是否导致完全无结果事件、后者是否即便返回错误仍产生含 `0/0` 的结果事件，反编译的 Map 插件源码和 VM 托管接收源码都没有回答：派发决定落在未见的原生 Platform 执行/结果存储层。

因此，**不能静态判定这两种失败会不会进入 `VmModule.SetValue`，也不能宣称错误执行一定进入或不进入历史结果**。一旦类型 23 事件或 `RefreshResult` 查询到记录，错误状态不会被 VM 回调适配层按 `nErrorCode` 自动过滤；`SetValue` 按记录中实际存在的字段写值，清旧行为取决于上表列出的执行条件。若没有事件、没有可查询记录，则该 VM 路径不清旧 `_value`；结果树仅刷新显示属性时可能保留旧展示值。原厂算法 SDK `GetResult()` 成功/失败以及 synthetic ModuleFrame `writes` 不是 Platform 历史结果证据。

在本地 Map 专属 `analysis/map-calibration` 日志/trace 中没有发现 `0x80100015` 或 Platform Init 失败记录；代码中的 `0x80100015` 仅在 Map 错误码文字映射函数中出现，不能据此说 Platform 初始化失败。当前实际缺口是缺少真实 VM Platform 对上述两种错误轮次的结果事件/历史查询 trace，而非本轮查明的 Init 错误。没有可复核的平台 trace 时，本轮到此停止，不做设备测试、不改生产代码、不提交。
