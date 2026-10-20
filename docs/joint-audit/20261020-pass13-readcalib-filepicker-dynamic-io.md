# 第十三轮：标定加载文件选择与动态输出重建是否同一事件

此项是 B 类标定生产物进入 `ReadCalibFileModu` 的文件/动态端口交接点，工具自身归 C 类。只核**原 `OpenFileDialog4ReadFile` 选择路径→文件解析显示→`VmModule.InitDynamicIO` 是否直接发生**，并对照当前配置桥。第十二轮已测试“文件内容改名/改型→owned Group/坐标系”运行后果，本轮不重跑该图。原件与另一工作树只读；无产品修复或提交。

## 原件身份与 CIL 证据

| 证据 | SHA256 / REA 身份 | 结论 |
| --- | --- | --- |
| 安装 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/ReadCalibFileModu/ReadCalibFileModuControl.dll` | `9C5C2C529B5242F9735A51DEF23C30FE90B2E3CBFE16C1FF31F95D0F1C129300`；REA `inspect_managed_members` `ev_3e521b617d7c86dd6cfccd0125923352f037fad4f91e328ad486e3a36f813bf7` | 原件元数据确认 `Apps.XmlParser.ParamTab.OpenFileDialog4ReadFile` 类型 token `0x02000002`；`ValueChangedCmdExecute` `0x06000001`、`ReadData` `0x06000002`，后者 CIL SHA `751e0b9cdc0c77406638ba4d717d2b4f7ade0bd0a4d4c1ebf310abbb40863c80`。调用边是 override→base `ValueChangedCmdExecute` 与自身 `ReadData`，再→`FindBaseNodeByName` 和 `ReadFileConfigData.ReadData` token `0x06000007`。这些成员没有直接 `InitDynamicIO` 调用边。 |
| 同原件 `ReadFileConfigData.ReadData(bool)` | CIL SHA `7fb67d15f44173e7fcfeeec3c22be7ee0219762cadceb489732e387e4c1a8ada`；其反编译源见下 | 仅 `needAddoutput=true` 时才倒序删除模块现有 `DynamicVmIO/Combination` 并对解析出的 `IOutput` 调 `AddOutput(vmModule)`；默认 false 只更新 `CalibParam` UI 集合。 |
| 原 AlgorithmTab | 安装 `ReadCalibFileModuAlgorithmTab.xml` SHA `5E23FDE5EFD772431940A33AD44F8898B9DC5AEF1A84F5FAD590DC2688F44863` | `FilePath` 是 `OpenFileDialog4ReadFile`，RW、`.xml` 过滤；同 Tab 有 `ReadFileConfigData Name="CalibInputParam"`，`CalibOutputParam` 节被注释；`FilePath` 元素本身没有 `UpdateModuleStateOperation` trigger。 |

反编译对应 `worktrees/tool-plugins/analysis/full-audit/calibration-dependencies/decompiled/ReadCalibFileModuControl/Apps.XmlParser.ParamTab/OpenFileDialog4ReadFile.cs` SHA `17316DA32899A746A9A951132943BA38288C9CBFFE05F011E029318F68E81236`、同目录 `ReadFileConfigData.cs` SHA `DBFC902062DF4093598F0559D7CD0D79B07F7E665152B3CADC874F23A6F664DF`。原 override 实际源码为 `base.ValueChangedCmdExecute(); ReadData();`；私有 `ReadData` 查找 `CalibInputParam` 后**无参**调用 `readFileConfigData.ReadData()`。这与 CIL call target 对齐。可直接关闭“该 override 直接调用 `InitDynamicIO` 或 `ReadData(true)`”这一假设：**没有此直接调用**。不能进一步推断文件选择后端口绝不会通过其他通知重建。

## 父类、通用操作与不可混称的路径

安装 `Apps.XmlParser.dll` SHA `78EC7AE1E5ADEFFF29197099C7109B153A3F75D092B7C3E9DC253D8C1B25A8D1` 的既有反编译副本：

- `analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/OpenFileDialog.cs` SHA `129D9185E0FBDCCD5146EA57C1CA9033AB1D4D0AAF8EEFE29FCA2B1921C40BC4`：本地/远程文件对话框选择后 `SetCurValue`；它先设 `CurValueA=null` 再设所选路径。`CurValueA` setter 在变化时触发虚方法 `ValueChangedCmdExecute`。空值轮基类早退，**派生 override 仍继续 `ReadData(false)`**；实际路径轮基类调用 `SetParamUnit`，成功时通知并 `PreExecuteRun`，派生 override 再读 `GetCalibInputParam` 显示文本。因此一次文件选择可触发两次显示读取，不能只按成功路径简化。
- `analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/NameValueItem.cs` SHA `90B4742E79243F46C46FD6B38187EE7F5BE49D4C6677F994133215A3796FED46`：`SetParamUnit` 检查 RW、`Check`、`SetParamUnitByZmq/SetParamUnitDirect`；`PreExecuteRun` 在允许参数修改执行且触发条件满足时调 `VmModule.Run`。**即使基类 SetParamUnit 失败**，派生 override 的后续 `ReadData()` 仍会被调用，可能读回旧/当前 SDK 参数显示；不能把失败回显等同“成功切文件”。
- `analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/UpdateModuleStateOperation.cs` SHA `791B97AC70AA308BF6157F719E2E7176DDA0BBF3C1E4AFF4358DB9253224C9CB`：这是另一个**通用 operation**，其 `Operate` 会显式 `vmModule.InitDynamicIO()`；还异步延迟读模块 selector/ready 状态。该成员存在不意味着 ReadCalib `FilePath` 使用它，本资源 FilePath 没有对应 Setter/Trigger。
- `analysis/decompiled/VM.Core/VM.Core/VmModule.cs` SHA `53E2F0F9032C4631B59CF0EB2A00E06D50469D94706E7E370E9CC7E1059C72CE`：`InitDynamicIO` 查询 SDK `GetModuInputIOInfo/GetModuOutputIOInfo`，由其返回集合更新 `VmIO`；它确实是端口重建入口，但本轮没找到**文件选择事件到该入口**的原托管调用边。

REA 对安装 `Apps.XmlParser.dll` 的 `inspect_managed_members` 返回证据 `ev_156235b1e447287311a00b3f1dfb4ddd42e103abeef8ad2984955f360ab5230b`，coverage=`unavailable`，具体是 metadata representation 超过 `67108864` 字节预算（reported offset `242508`），未给父类 CIL。上述父类结论限于已有完整反编译源，不伪称此次 REA CIL 已确认父类。需要进一步证伪间接路径时，可对受控副本用专门 .NET CIL 工具抽父类方法，或追 `IUserStringData.SetParam→SDK SetParam/模块动作通知→InitDynamicIO` caller；不能重复同一超限 REA 调用。

## 当前适配与前轮实测边界

当前 `src/VisionPlugins.Calibration/ReadCalibFileConfiguration.cs` 的文件按钮经 `IToolConfigurationFileService.Select`；用户选中时 `State.CommitUser(..., forceWrite:true)`，配置 dispatcher 立即加载/解析，成功将 `FilePath` 与 `OutputSchema` 一次 `ApplyValues` 到图文档，随后 `RefreshData` 更新专属面板。失败路径有独立 host 事务拒绝/恢复策略。这是**当前独立实现**，不能把原 override 默认 `ReadData(false)` 说成它的原版依据。当前 GUI 侧失败/恢复的范围应参考插件 `ReadCalibFile` 专项，联合主框架动态结果则已有 `docs/joint-audit/20261020-pass12-readcalib-schema.md`：成功解析后改名/改型可更新运行 descriptor，但旧 Group 输出引用不自动迁移。该报告的当前真实图与本轮原控件按钮仍是不同入口，不能相互冒充原厂闭环。

## 结论与精确缺证

**关闭一个歧义：** `OpenFileDialog4ReadFile.ValueChangedCmdExecute → ReadFileConfigData.ReadData()` 的直接 UI 更新路径不调用 `VmModule.InitDynamicIO`，也没有 `ReadData(true)` 的默认参数。原文件控件选路径首先走基类写参/可选预运行，再读配置显示。此前若从 `ReadFileConfigData.ReadData(true)` 的实现推称“选 XML 自动删旧 Group 输出并重建”，该推称没有本入口的调用证据。

**仍需的唯一外层桥证：** 文件选择写入 `FilePath` 后，原模块 SDK `SetParam`/`LoadFile` 或 `VmModule.Run` 是否发出动态 IO 通知，再由哪个托管 caller 调 `GetModuOutputIOInfo→InitDynamicIO`；还需确认对象复用/重建和已选 Group 关系是怎么处理的。现有原 `UpdateModuleStateOperation` 不能当作此 caller。原平台同构受控运行可作为反证/佐证，但没有该证据前对原版连接迁移一律记未知。无可依据的产品修复，不新增重复当前主窗探针。
