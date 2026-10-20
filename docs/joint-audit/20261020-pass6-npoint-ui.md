# Pass 6 / B 标定数据生成：N 点标定编辑点链

范围：仅 N 点标定专有点表、参数和可观察的下游边。共用配置窗和图编辑器只作为调用边界；本报告不重审其全部功能，也不将截图主观速度当规格。

## 原版源码链与当前链

| 边 | 原版直接证据 | 当前实现与判定 |
|---|---|---|
| 打开点表 | `IMVSNPointCalibModuControl.NPointList.UserControl_Loaded → Init` 取 `CalibPointTotalNum + RotPointTotalNum`；每行五次 `GetFloatParamValue` 经 `IUserStringData` 读并转 `f3`；对象初始化器给 `PointList` 五属性赋值。`PointList` 各 setter 在 `OwnerParam != null` 时逐一向 `IUserStringData[字段名+ID]` 写，失败则不更新文本。XAML 五个可编辑列均 `LostFocus`。 | `NPointConfigurationPointEditor` 建行时同样逐格调用 `commit`。但该 `commit` 进入 `NPointConfiguration.PointValues()`，**每格**克隆全部 32×5 点值后 `Apply` 整份参数；再 `NPointPlugin.Synchronize` 扫 32×5 键。实际主窗口 `ToolConfigurationHost.Apply` 进一步对每格调用 `GraphEditorDocument.SetProcedureNodeParameters`，后者验证参数并执行完整文档编辑/历史记录。逐格写回语义有原版依据；每格复制整个图文档属于当前独立实现，不能宣称原版同成本。 |
| 单格编辑 | XAML `LostFocus` 把新文本送 `PointList` 对应属性，setter 一次写对应 `IUserStringData` 键，成功才通知该属性。没有原版整表提交入口。 | `PointRow.Set` 一次 `commit`、一次属性通知；生产适配器经 `PointValues → Apply → Document.SetProcedureNodeParameters`；本次 4/9/32 点探针各确认**改单格一次 host Apply**且文档读回该字段。未观察到循环执行或隐式求解。 |
| 原输入→运行结果 | 本次未重新解析原生 `GetProcessInput/ProcessAlg/ProcessOutput`；已有专属源审见 `docs/plugins/N-POINT-WHOLE-TOOL-SELF-AUDIT.md` 和原生追踪。本次确认当前 `NPointPlugin.Runtime.ExecuteAsync`：手动模式读同步状态中的点数组，按平移/旋转分段构造 `NPointPair/NPointRotation`，交 `NPointSolver.Solve`，成功发布数值和 `CalibData`。 | **当前源码链已证，原版完整数值等价不由本探针证明。** 原输入端口连接到节点、真实运行输出须引用已有专属全链验收，不因本次点表测试计为新闭环。 |
| 结果→真实标定消费者 | 当前 `NPointPlugin.Results` 发布 `DataKind.Calibration` 的 `CalibData`。本轮对 `src` 的该类型引用搜索没有找到任何工具以 `DataKind.Calibration` 作为输入；`标定加载`另从文件发布此类型。 | **此边未证：不能因生产者有输出便称存在真实下游消费者。** 若目标是 N 点 XML 给后续工具使用，应选定具体消费者及端口/文件模式，用原版读取链和真实图执行补证。 |
| 保存/重开 | 原点表 `ButtonApply_Click` 仅关闭窗口；点 setter 之前已写参数。当前 `NPointConfiguration.SaveCalibration` 调 `Export → NPointFile.Write`，配置窗 `Apply` 写图文档。 | 本次验证了编辑后图文档内该字段值；没有执行本探针中的保存/重开。既有 N 点专项测试覆盖保存与载入范围，但本次不把它移作“编辑→消费”全链证明。 |

## 受控探针

`dotnet run --project tests/NPointEditCost.Checks/NPointEditCost.Checks.csproj -c Release`，单进程 Release、离屏 WPF、真实 `NPointPlugin.CreateConfiguration`、真实 `GraphEditorDocument.SetProcedureNodeParameters` 回调。点数 4、9、32（32 设为合法 16 平移＋16 旋转），分别打开点表，再将最后行角度改成 `1.25` 并读回文档。原始 stdout 见忽略目录 `analysis/joint-audit/pass6-npoint/run.log`。

| 点数 | 打开期间文档 Apply | 打开调用耗时 | 单格文档 Apply | 单格调用耗时 |
|---:|---:|---:|---:|---:|
| 4 | 20 | 951.4 ms | 1 | 10.0 ms |
| 9 | 45 | 563.2 ms | 1 | 3.8 ms |
| 32 | 160 | 1041.9 ms | 1 | 2.5 ms |

**判定：** 开窗触发 `5×点数` 次完整图文档编辑，这一放大由源码和回调计数共同确认。耗时是单次离屏受控样本，含 JIT/WPF/机器负载；不推出真实交互桌面分布，也不归因永久卡死。表中单格并未呈随点数单调恶化，用户报告的慢可能还有配置宿主布局、其他节点图规模、历史负载或窗口事件因素。本探针没有对原版进行同机同步计时；不能报原版速度比。

**修复方向及约束：** 可在插件配置与图文档之间研究保持原版逐字段 setter/失败副作用的前提下，避免开窗初始化给图编辑器建立 `5N` 份完整历史快照；但必须先核原版 `IUserStringData` 写入是否在全部初值相等、失败或重复打开时仍产生可见副作用，并定义图历史/保存的事务语义。仅凭本次成本探针不能直接删除初始化 setter 或合并成一次批写，也不能动在制 `tool-plugins`。下一步可增加真实主窗口大图历史负载的受控诊断，和原版同字段写入的 getter/setter trace。

## 证据身份与复验限制

| 证据 | SHA-256 |
|---|---|
| 原 `PointList.cs` 反编译体 | `A396060C73DBF56B1A7FB7849A4BCD17FE5CBC3D04AD8356F68D4B3B1C9B3813` |
| 原 `NPointList.xaml.cs` 反编译体 | `B972D12AF77B27DC3A19E00629C77F3BED58D3FD7A266DD0AA28050A281B5A46` |
| 原 `NPointList.xaml` | `92945CE0D5ED92F98F12187A0EB92F1673516760B894F0CD52F89FC2C7341D79` |
| 当前 `NPointConfigurationPointEditor.cs` | `6FA439D2EA67625152CC07A96C2C3B5F301E4407984232E25077C0B288640091` |
| 当前 `NPointConfiguration.cs` | `FEA7744DC344CA708995394AE40C059322B092100D5C672159015FF2C9EF9B55` |
| 当前 `NPointPlugin.cs` | `DB9482BB034D9F98F436A932C51804843BB20A18A67AA1EEA28D874DBF1574C0` |
| 当前 `ToolConfigurationHost.cs` | `2B1F110A2AB7B4CBF55B973109F6A99DB56E111D90BFEAD811854D98D018B5B1` |
| 当前 `GraphEditorDocument.cs` | `4A44A59C96449A286EAC7162E42A0B42DD012669F49E5AA810E1CE131799B558` |
| 本探针 `Program.cs` | `4C021646131F561A4CDB354131A844403E552C7286B3E3F551BC3E8CE9522B95` |
| 原始运行日志 | `E1B7ED168CE665FE8CCA65B760DE51D8BAEAC6123C9FECDA7D1FA1BB2C7CC192` |

原体位于 `analysis/full-audit/renderer-gap-projects/IMVSNPointCalibModuControl/IMVSNPointCalibModuControl/`。本报告未修改产品、共享代码、另一工作树或他任务窗口。
