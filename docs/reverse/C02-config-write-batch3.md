# C02 第三批：配置写入分流、缓存与重启

2026-10-19；main 基线 `6a2cf54`。本切片只补原托管配置写入链，不重复第二批等待检查，不修改产品或共享合同。C02 完整调度包仍开放。

## 关键更正：同名 SaveConfig 不同存储

原 `VM.Utility.ConfigurationHelper` 的同名重载不可合并解释。第二批“SaveConfig 存在并写 cfg”应限定为文档重载。

| 原入口 | 路径/存储与结果 |
|---|---|
| `GetConfig(string)` | `ConfigurationManager.AppSettings`，读取进程配置的 appSettings。 |
| `SaveConfig(string,string)` | 先和 `GetConfig(key)` 比较，相同则返回；写 `AppDomain.CurrentDomain.BaseDirectory / (ApplicationName + ".exe.config")`，修改第一组 appSettings 中所有具有匹配 key 且有 value 属性的子节点；不插入缺项。刷新 appSettings，然后复制新文件为 `<name>_temp<extension>`。 |
| `GetConfigValue(string)` | 从当前工作目录的 `ApplicationName + ".cfg"` 加载 XDocument；保留静态文档缓存，按唯一节点/唯一 CurValue 返回文本。缺项/重复项等异常返回 null。 |
| `SaveConfig(XDocument)`、`SaveConfig(XmlDocument)` | 当前工作目录的 `ApplicationName + ".cfg"`，FileMode.Create、写入、独占、WriteThrough；UTF8、Indent=true，完成后复制同目录 `_temp.cfg`。不更新或清空 `_cfgDocument` / `_isLazyLoadCfg`。 |

三个 SaveConfig 都返回 void，内部捕获 TransactionAbortedException 和一般 Exception 并记录日志；不能用正常返回证明保存成功。TransactionScope 不是文件写入的原子替换或回滚证据：cfg 是直接 Create 覆盖，备份发生在写入完成之后，备份内容是新值。

原类方法清点已记录于本轮 write.log 的 token/signature/raw-CIL 哈希。没有 `SetConfigValue` 或 `SetConfig` 成员，不能沿不存在的通用 setter 补调用链。另有私有 `Save(string)`，CIL 仅 nop/ret；INIWriteItems/INIWriteValue/INISetStringValue、INIDeleteKey/Section、INIEmptySection、INISetDefaultOpenPath/RemotePath 属另一组显式 INI/Win32 接口，本轮不调用，不把它们当 cfg 写入口。

## 设置页的真实入口

以下为已取原源码的静态链，未运行设置窗口：

`VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/SoftWareData.cs:806 LoadXmlConfigData`
→ `AlgorithmTabRoot.loadXml("Resources//SoftWareItems.xml")`
→ AdvancedSetting 的 `NameValueItem.InniterBusiness`
→ `GetConfigOperation.Operate` 读 **GetConfig/appSettings**。

以原资源 `SoftWareItems.xml:33–50` 的 `BooleanConfirm Name="IfSlientExecute"` 为具体对象：Initers/GetConfigOperation，ConfirmSetters/SaveConfigOperaion_BoolToInt，且 IsModifyReStart=True。`GetConfigOperation` 对 bool 先 int.TryParse（仅1为true），再 bool.TryParse；不读取 cfg。

`SoftWareData` 保存入口在 `:1136–1167` 调用多项 string SaveConfig，并调用 `ExecuteSetParam`；`:840–855` 枚举 IConfirm.ConfirmData。原 `Apps.XmlParser.ParamTab/BooleanConfirm.cs` 的 ConfirmData 遍历 ConfirmSetters；`Setter.Operate` 将 OperationParams 按 `;` 分割；`SaveConfigOperaion_BoolToInt.Operate` 取父 NameValueItem.ComName（若有 ExtraParams 则用首项），Convert.ToBoolean(Value)，调用 **string SaveConfig**，保存 "1"/"0"。`SaveConfigOperation` 则保存 Value.ToString。不能把此链归为 cfg 编辑器。

另一条实际文档写入链为 `VisionMaster.Project/ProjectManager.cs:868–891 SaveRecentlySolutions`：从 ApplicationBase 的 cfg 读取、移除/添加 ArrayOfSolutionItemInfo，交给 SaveConfig(XDocument)；后者写 CWD，前者读 ApplicationBase，二者是否相同取决于实际进程 CWD。旧 `VisionMasterMain/SoftwarePage.xaml.cs:148–205 SoftwareApply` 使用 XmlDocument cfg 重载，是另一程序集/窗口来源，不能无条件代替已审 VisionMaster.exe 设置页。

## IsClearLastExecuteResult 不可接错入口

`SystemConfigManager.cs:123–139` 只从 GetConfigValue 取得 `=="1"` 并缓存 nullable bool；setter 只修改属性缓存。`Program.cs:224` 在启动链复制到 `IVmSolution.IsClearLastExecuteResult`。本轮边界内的原 exe 源码及安装 SoftWareItems.xml 未找到该字段的设置页写入口；这是有界查找，不宣称整个产品不可修改。

`VM.Core/VmSolution.cs:4115–4134` 清理分支是 **nExecuteCount 非零、nWorkStatus 非零、开关true、nExecuteCount>1**；枚举 Root==proc 的模块，对 Outputs.leafIOsWithoutTimeStamp 调 SetNoneValueWithoutRaiseEvent，并置 IsEmpty=true，然后调用 Begin 回调。它不是删除全 session，也不等同 SDK 全缓存清理。`VmModule.RefreshResult:2612` 在开关false且nNum>0时选择 array[0].nExecuteCount；`ModuleResultView.xaml.cs:1353` 也消费开关决定计数过滤。

若 main 后续实现保留策略，需先由 root 形成合同：producer 是启动配置/工作状态回调，payload 至少含有效开关、根流程身份、执行计数、开始阶段、模块/叶输出范围；consumer 是结果缓存、主动刷新和结果视图；生命周期是启动快照及执行开始条件，错误回调是否到达仍未知。当前 `WorkflowExecution.RunCore` 无条件移除 session 的独立策略不能仅增加一个 bool 就宣称复刻上述多消费者。本切片不改 GraphExecutor、Contracts、MainWindow 或共享 FCR。

## 原类受控探针

`tests/NativeEvidence/C02Config/Run.ps1` 使用 Framework csc 编译独占进程，通过反射调用原 helper。独占 exe 基目录与 `working/` CWD 刻意分开；ApplicationName=Probe。只复制原 VM.Utility/Apps.Log（log4net 若存在），不复制安装配置为执行配置、不初始化 VM/SDK/设备。已先核对 helper 静态构造与三个 SaveConfig/BackupFile CIL：写入目标全部为独占路径，日志环境也由该 Probe 配置提供。

三进程 **17 项断言通过，exit0**：

- 写入13项：appSettings初值/刷新、cfg隔离、string备份、缺key不新增、两种文档重载写CWD、备份含新值、已加载cfg缓存不失效、配置存储分离；以目录占据 cfg 目标验证写失败被吞，随后恢复本轮文件。
- 重启2项：新进程读取 appSettings=new、cfg=2，证明原 helper 的进程缓存边界，不是实际 VisionMaster 重启验收。
- 重复项2项：重复cfg目标返回null；重复appSettings匹配项全部改写。

成功：`analysis/full-audit/batch3-20261019/C02/run-d0d17df10bb94d1b9d6f4a66718af0c4/`。首次 `run-f42775a8d2a24ab6841e17e59f69431e/` 在缺失可选 log4net.dll 的复制阶段失败，尚未编译/执行；保留目录，脚本改为可选依赖后重跑，不隐藏该失败。哈希清单在写入前先拍文件列表，排除自身。

安装 cfg、exe.config、VM.Utility.dll 前后 SHA 未变。强名称实际选择 GAC VM.Utility，其 SHA `65ebb5edabf77465fa01f8c6d70efdcbd4b6a87f510911249cd93d73649d1cdf` 与已分析安装原件相同；loaded-hashes.csv 同时保留 Apps.Log 等真实加载路径。没有强行声明均从副本加载。

## 可追溯证据与开放项

复用第二批同版本 REA `ev_17a12d045406658be0d3ec4eaacc51b23a6108dc714f50ac1f3ff3abd51366d1`，MVID `32e8ac89-6335-43c6-aa3e-61d8c0a670e5`；未为相同元数据重复请求 REA。原 CIL 来自 `analysis/full-audit/batch2-20261019/C02/VM.Utility.il`，SHA `ae2c2abe8251c6261dddaa8c77e3bfb5825d95fe8c1fc4472bf69c30b8b26677`；本轮 evidence-inputs.csv 保留所引用源文件和资源哈希。

| 原方法 token / RVA | raw-CIL SHA256（不含头/EH/locals） |
|---|---|
| SaveConfig(string,string) `0600012d / 52a0` | `b568faaa3246321c888546e4d7cfa2aa83ef16d4dcf004aca0ddab01b90e2e7e` |
| SaveConfig(XDocument) `06000146 / 5aa8` | `9b173f8953f57f955472f7976ceaecf89d75de5f4c4c01d9e299b24609aa4f5f` |
| SaveConfig(XmlDocument) `06000147 / 5bfc` | `3ef74199eb5b15b89a1d4e43d6d7814dbf33f7d05e955bd68f8c20b1b3dc40cc` |
| BackupFile `06000148 / 5d50` | `b94cfab3f9c0388b6d447b2d3a33ca9e70f586963b80a64b5a1f81f89d5e8ce0` |

仍开放：真实主窗口有效 CWD/启动值、设置页实际保存交互及重启、并发写入/断电恢复、SDK工作状态与错误回调、结果保留真实操作差分。17项叶方法断言不关闭C02、不证明native调度或完整产品一致性。
