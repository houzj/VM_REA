# main 剩余未提交改动审查

日期：2026-10-19。基线 `1332a0f`。用户授权按原始源码审查 main 剩余改动、验证后整理提交；不修改 tool-plugins，不推送远程。

## 保全范围

审查前22个在制/未跟踪文件已复制到 `analysis/full-audit/main-pending-review-20261019/files/`，同时保存 `HEAD.txt`、`status-before.txt`、`pending.patch`、`files-sha256.csv`。逐文件SHA-256与源文件相同。既有674个顶层审计文件另保存在 `evidence-before/`，含旧日志、截图、结果JSON及失败记录，核验清单为 `evidence-before-sha256.csv`。原始反编译证据不写入、不提交；本轮读取的32份源码/资源/配置哈希分别在 `source-sha256.csv`、`source-ui-sha256.csv` 和 `source-targets-sha256.csv`。备份受Git忽略，仅在本机保留。

本轮新日志在同一审查目录，新增专项截图/方案在 `analysis/full-audit/main-pending-review/`；订阅弹层及全量回归沿用原有输出路径，其旧版本已经保全。截图生成器仅在tests。

## Group IO、订阅与显示

原始证据为 `analysis/decompiled/DynamicObjectConfigControl/DynamicObjectConfigControl/GroupParam.cs::InputOutputTypes/AddCommandCmdExecute/AddLeafIO/AddCombinationIO`，以及 `VM.Core/VM.Core/LeafIOTypeEnum.cs`、`CombinationIOTypeEnum.cs`。UI模板 `DynamicIOConfigControl.xaml` 绑定 `InputOutputTypes/SelectedType/AddCommand`，每项通过 `VmIODecorate.FilterSelecter` 和 `ButtonSelecter_sub` 选择关联IO。多选入口创建 `IOPickData/Picker`，选中后创建叶/组合动态IO并赋 `RelateIO`；输入/输出上限实际按SDK叶IO计数，而不是按画面行数。

本地对应链：Group输入/输出加号→`WorkflowSourcePicker`→`SaveInputs/SaveOutputs`→`CreateBoundGroupWindow` 的 `ConfigureOwnedGroupInputs/SetWorkflowConfiguration` 委托→文档快照、Group接口同步、结果失效→`SolutionStore`保存重开→`SolutionExecutor`真实Group数据。这个序列化及执行器是独立托管实现，没有引入原生ABI或.sol兼容声明。

|候选/问题|审查决定|确认与限制|
|---|---|---|
|输入/输出类型菜单及多选|采纳已支持类型和原生候选名称；未支持项禁用|排除候选的 `pointset→Point数组`、`POLYGON→Region`。Point数组不是native pointset，Region没有native POLYGON端口生产链。补回原始ROIANNULUS对应的不可用ANNULUS项。Region/Components保留独立托管标签；可选3D类型尚未实现。详见FCR-023|
|输出多选为空|修正筛选|`BodySources`提供NodeId/Port但不提供输入专用Reference；按非空NodeId/Port选择真正内部输出。实际多选Width/Height，运行真实BMP并保存重开验证|
|输入/输出/显示来源弹层|采纳180×180树及叶双击写回|原始 `Apps.XmlUI/styles/paramusercontroldatatemplate.xaml::ButtonSelecter_sub` 绑定DropDownCommand、ClickCommand；层级以完整Port路径构建。`WorkflowSourceOption.NodeName`是可空UI元数据，防止本地字段标签丢失模块名，不写方案|
|弹层额外的独立模态选择窗|排除候选实现|原始 `ButtonSelecter.PopSubscribeWindowCommandExecute` 修改共享AlgorithmTabRoot的SelectedSubscribeParam/IsSubscribeInWindow，依赖SubscribeInPopWindowEnableA门控；候选直接ShowDialog另建通用窗口不等于该链。只保留既有树，不复制插件配置框架|
|显示表格/删除|采纳有模板依据的列和订阅入口，修复漏接删除事件|`DynamicDisplayObjectConfigControl.xaml::delBtn`绑定DeleteCommand；`DisplayObjectDecorate.DeleteIOCommandExecute`移除显示及映射IO。当前本地删除由保存委托更新Displays并参与Undo；不宣称具备原生全部映射IO移除、所有显示类型/样式语义|
|Group配置窗口标题栏/搜索图标|采纳为独立WPF外观适配|`DynamicRunParamConfigWin.xaml`确认1100×680及无Cancel；`DynamicRunParamConfigPanal.xaml`确认搜索/筛选和左右区域。自绘标题栏、具体字体颜色及尺寸并非原生模板逐像素复刻；ic_search_normal.png与Apps.XmlUI随包资源SHA-256均为B40E62764E3D78FA9647E3788586DFBCE92E0955342172C624883A605E832179|
|SubprogramCallWindow.xaml候选|整份排除并恢复HEAD|候选初始禁用RunOnce/Repeat且没有相应启用路径，确认按钮覆盖运行区；没有以截图推定的尺寸/布局为合同。本轮用例确认恢复后的按钮初始可操作；运行语义未更改|

原始类型选择、名称生成、组合叶数限制和SDK叶端口注册并未完整重建；本地1024接口项计数、名称清洗、数据精度和强类型记录仍有独立策略。禁用未知类型不意味着支持其原生存量方案。Group装饰器/动态资格/Undo服务仍分别受FCR-018/019/021/022限制。

## 局部变量

证据：`VMControls.WPF/VMControls.WPF.GlobalUI/GlobalVariableDialog.xaml` 的列绑定依次包含名称、RemarkInfo、DisplayType、Value、BindString、DisplayParamsName/ModifiyRunParamsCommand；同类 `.xaml.cs` 定义默认12项分页、CurrentPageNum/PageCount与搜索数据。`Apps.XmlUI.ParamTab/PageNumControl.xaml(.cs)`提供页大小、跳转和总数控件。当前窗口是本地局部变量适配，不是新增全局变量或通讯服务。

采纳列标题/次序及分页入口，保留原有引用文本和点击目标绑定。排除将FrameworkElementFactory对象直接放入Button.Content的候选，避免显示工厂对象或丢失实际引用文本。搜索提示改为覆盖文本，不写入Search.Text；因此名为“请输入变量名称”的真实变量仍可检索。页面跳转按本地1起始页码夹取，原始内部0起始页码不被冒称为相同实现。

数据链仍为控件事件→当前配置快照→`SetWorkflowConfiguration`→本地变量订阅和执行→方案序列化。Tab/可订阅元数据缺口仍开放，但撤回仅凭CustomVisible=False认定局部变量目标应隐藏的旧结论：`GlobalVariableItem.ModifyRunParamsExecute → RunParamsSelectData.GetSubRunParamsTree → AlgorithmTabRoot.GetRunParamsTree`枚举`Tab_Run Params.Collection`，`AddSubChildToTree`仅检查DisplayNameA，订阅资格另经`RunParamsBaseNode.IsSupportSub → ISubInfo/SubscribeVisibleA`。Group候选的`RunParamPickDataExtern`才明确过滤CustomVisibleA，不能混用。随包ImageSourceModuleAlgorithmTab.xml只有Tab_Basic Params，运行态Tab装配及候选集合仍未知；其StitchEnable.CurValue/DefaultValue均为True，不是旧记录暗示的False。相关测试选择本地图像模式不改变运行时序的合法整数参数，未硬编码过滤隐藏差异。FCR-019按此修订。

## MF-05与长期审计记录

|材料|源码复核及决定|
|---|---|
|MF05-PROCEDURE-RUN-CALLCHAIN.md|确认VmProcedure.Run→ProcessManager.SyncExecuteOnce→IMVS_ExecuteOnce_V30→work-status完成标志。修正保存结束回调顺序：GetSolutionSaveEndCallBackInfo先派发OnSolutionSaveEndCallBack，再记录nStatus和完成标志；InternalExport空路径取消和catch也清除_isSaving。修正本地保存入口名为GraphEditorDocument.Save|
|MF05-CONTINUOUS-RUN-CALLCHAIN.md、MF05-RESULT-LIFECYCLE.md|确认ContinuousRunEnable入口、停止回调和按IsClearLastExecuteResult分支清叶输出；VmModule.RefreshResult及ModuleResultView读取当前序号/uint.MaxValue。Program启动配置和SystemConfigManager读键已确认；安装配置哈希匹配且缺键，但ConfigurationHelper回退、设置UI和native缓存仍未知。不改本地无条件清缓存算法|
|GROUP-NATIVE-EDITORS.md、CONTRACT-REQUESTS.md|确认DynamicNameValueItemOfGroup.OpenDialogCommandExecute先SupportDisplayForGroupData再GetSupportDisplayForGroupData；DynamicExtern产生ROISelecterForGroupDecorate。原生ROI模板未接入，保持开放，不将通用托管编辑器当等价|
|AGENTS.md|采纳用户已有的源码级重建标准，不新增自动派工或审批规则|
|FRAMEWORK-SOURCE-SELF-AUDIT.md、清单、HTML|保留历史失败、源码路径及未知边界，更新最新审查入口；修正HTML缺少的段落结束标签。历史清单不因测试通过自动关闭23个领域|

## 验证

构建统一经 `dotnet run --project ... --no-restore -p:BuildInParallel=false -p:UseSharedCompilation=false`；WPF后续模式使用同次产物 `--no-build --no-restore`。

|检查|本轮结果|
|---|---|
|WpfChecks --main-pending-review|20项，exit 0：未知类型拒绝创建、按钮初态、Width/Height多选、显示删除/Undo、真实BMP运行及保存重开、25变量分页、提示文字同名搜索、列顺序|
|WpfChecks完整|456项，exit 0：包含真实图像、订阅、Group循环/累积、配置、保存重开与关闭清理|
|WpfChecks --native-subscription-popup|6项，exit 0：展开、180×180、嵌套字段、完整叶路径写回、解除订阅|
|VisionFramework.ExecutionChecks|71项，exit 0|
|VisionWorkflow.CallChecks|93项，exit 0|

编译0错误；有既有MainWindow nullable及MiniMapWheelParityChecks两处空值装箱警告。目检新增Group显示页、变量搜索页和订阅Popup：控件/结果可见，未作为原生截图差分。用户桌面键鼠、完整控件皮肤、native线程/设备/状态码/ABI仍未验收。本轮没有修改或停止其他任务的进程，没有修改tool-plugins。

CallChecks会自动将新默认WaitTimeMs写入受跟踪示例；本轮生成版本另存generated-example-from-callchecks.vmx.json后恢复该示例，不将测试副产物提交。
