# 第十七批完整原菜单资源逐项对照

资源具名节点 53；唯一可执行resolver 43。这些是审计分母，不是96项实施进度的新分母。

|资源ID|完整路径|源方法/动态项|工具栏|当前状态|当前锚点或缺口|
|---|---|---|---|---|---|
|0|Lang_File|||structural-folder|资源容器；不是可执行命令|
|0.0|Lang_File → Lang_NewSol|CreateNewSolution||implemented-local|src/VisionEditor/MainWindow.xaml.cs:173|
|0.1|Lang_File → Lang_OpenSol|OpenSolution|{'groupName': 'group0', 'priority': '1'}|implemented-local|src/VisionEditor/MainWindow.ShellOpenSolution.cs:62|
|0.2|Lang_File → Lang_OpenLastSol|GetRecentlySolutions||implemented-local|src/VisionEditor/MainWindow.ShellRecentSolutions.cs:82|
|0.3|Lang_File → Lang_OpenExmSol|OpenSamples||implemented-local|src/VisionEditor/MainWindow.ShellSampleSelection.cs:64|
|0.4|Lang_File → Lang_SaveSol|SaveSolution|{'groupName': 'group0', 'priority': '0'}|implemented-local|src/VisionEditor/MainWindow.ShellSaveSolution.cs:65|
|0.5|Lang_File → Lang_SaveAsSol|SaveAsSolution||implemented-local|src/VisionEditor/MainWindow.ShellSaveSolution.cs:65|
|0.6|Lang_File → Lang_ExportAlgorithmCode|ExportAlgorithm||not-implemented|原生算法代码导出后端尚无完整实现|
|0.7|Lang_File → Lang_ImportProcess|ImportProcess||not-implemented|原生流程导入及ID/模型落地开放|
|0.8|Lang_File → Lang_Exit|Exit||implemented-local|src/VisionEditor/MainWindow.ShellFileMenu.cs:24|
|1|Lang_Set|||structural-folder|资源容器；不是可执行命令|
|1.0|Lang_Set → 权限设置|AuthSet||not-implemented|账户权限服务及持久化开放|
|1.1|Lang_Set → 软件设置|VMSet||not-implemented|完整软件设置服务开放|
|1.2|Lang_Set → 方案设置|Solset||not-implemented|原方案管理设置数据链开放|
|1.3|Lang_Set → 运行策略|ProcRunSet||implemented-local|src/VisionEditor/MainWindow.ShellRunMode.cs:5|
|1.4|Lang_Set → 采集策略|CollectSet||not-implemented|采集策略后端开放|
|2|Lang_Tools|||structural-folder|资源容器；不是可执行命令|
|2.0|Lang_Tools → Lang_CreateCalibGuide|CreateCalibGuide||not-implemented|标定引导服务与生命周期开放|
|2.1|Lang_Tools → 深度学习训练工具|OpenExe; CheckPath||not-implemented|原外部exe检查/启动入口未接入，不启动原厂工具|
|2.2|Lang_Tools → 运行环境检测工具|OpenExe; CheckPath||not-implemented|原外部exe检查/启动入口未接入，不启动原厂工具|
|2.3|Lang_Tools → Calibration Board Generate Tool|OpenExeByLang; CheckPathByLang||not-implemented|原按语言选择外部exe入口未接入，不启动原厂工具|
|2.4|Lang_Tools → 运行异常采集机器人工具|OpenExe; CheckPath||not-implemented|原外部exe检查/启动入口未接入，不启动原厂工具|
|2.5|Lang_Tools → 查找表工具|OpenModuleParamTool; CheckPath||not-implemented|原查找表工具/参数服务入口未接入|
|3|Lang_System|||structural-folder|资源容器；不是可执行命令|
|3.0|Lang_System → Lang_Log|LogShow||not-implemented|原日志窗口与日志服务消费未交付|
|3.1|Lang_System → Lang_CommSet|CommSet|{'groupName': 'group2', 'priority': '2'}|not-implemented|通信全局模块与会话开放，现工具栏禁用|
|3.2|Lang_System → Lang_ControllerSet|LightSet|{'groupName': 'group1', 'priority': '1'}|not-implemented|原全局光源管理入口尚未接入（工具插件并不等同此入口）|
|3.3|Lang_System → Lang_CameraSet|CameraSet|{'groupName': 'group1', 'priority': '0'}|partial-backend|src/VisionEditor/MainWindow.xaml.cs:52|
|4|Lang_Help|||structural-folder|资源容器；不是可执行命令|
|4.0|Lang_Help → Lang_Language|||structural-folder|资源容器；不是可执行命令|
|4.0.0|Lang_Help → Lang_Language → %中文%|ChSet||not-implemented|全局语言切换与全部资源刷新开放|
|4.0.1|Lang_Help → Lang_Language → %English%|EnSet||not-implemented|全局语言切换与全部资源刷新开放|
|4.1|Lang_Help → Lang_HelpDoc|HelpDoc||not-implemented|本地帮助路径及启动链未接入|
|4.2|Lang_Help → Lang_OnlineHelpDoc|OnlineHelpDoc||implemented-local|src/VisionEditor/MainWindow.ShellHelp.cs:15|
|4.3|Lang_Help → Lang_VerInfo|VerInfo||implemented-local|src/VisionEditor/MainWindow.ShellAbout.cs:5|
|4.4|Lang_Help → Lang_MoreInfo|MoreInfo||not-implemented|更多版本组件信息窗口未接入|
|4.5|Lang_Help → Lang_Club|OpenVClub||not-implemented|V社区入口未接入|
|4.6|Lang_Help → Lang_WecPage|WecPage||not-implemented|原欢迎页及数据链未接入|
|5.0|[hidden-toolbar] → Lang_Undo|Undo; GetUnRedoStack|{'groupName': 'group0', 'priority': '2'}|implemented-local|src/VisionEditor/MainWindow.ShellHistory.cs:7|
|5.0.0|[hidden-toolbar] → Lang_Undo → Lang_Records|UndoList||not-implemented|历史下拉/明细列表尚未实施，不以普通Undo替代|
|5.1|[hidden-toolbar] → Lang_Redo|Redo; GetUnRedoStack|{'groupName': 'group0', 'priority': '3'}|implemented-local|src/VisionEditor/MainWindow.ShellHistory.cs:8|
|5.2|[hidden-toolbar] → Lang_Locking|AreaLocking; GetMainData|{'groupName': 'group0', 'priority': '4'}|implemented-local|src/VisionEditor/MainWindow.ShellLock.cs:5|
|5.2.0|[hidden-toolbar] → Lang_Locking → Lang_Locking|AreaLocking||implemented-local|src/VisionEditor/MainWindow.ShellLock.cs:5|
|5.2.1|[hidden-toolbar] → Lang_Locking → Lang_Unlock|AreaUnLocking||implemented-local|src/VisionEditor/MainWindow.xaml.cs:113|
|5.3|[hidden-toolbar] → Lang_GlobalVariable|GlobalVariable|{'groupName': 'group2', 'priority': '1'}|not-implemented|原全局变量模块及专用会话开放，现工具栏禁用|
|5.4|[hidden-toolbar] → Lang_GlobalScript|GlobalScript|{'groupName': 'group2', 'priority': '4'}|not-implemented|全局脚本引擎与会话开放，现工具栏禁用|
|5.5|[hidden-toolbar] → Lang_GlobalTrigger|GlobalTrigger|{'groupName': 'group2', 'priority': '3'}|not-implemented|全局触发器与会话开放，现工具栏禁用|
|5.6|[hidden-toolbar] → Lang_SigRun|SigRun; GetSolution|{'groupName': 'group3', 'priority': '0'}|implemented-local|src/VisionEditor/MainWindow.xaml.cs:52|
|5.7|[hidden-toolbar] → Lang_Run|GetSolution|{'groupName': 'group3', 'priority': '1'}|structural-folder|资源容器；不是可执行命令|
|5.7.0|[hidden-toolbar] → Lang_Run → Lang_MutiRun|MutiRun||implemented-local|src/VisionEditor/MainWindow.xaml.cs:52|
|5.7.1|[hidden-toolbar] → Lang_Run → Lang_StopRun|StopRun||implemented-local|src/VisionEditor/MainWindow.xaml.cs:52|
|5.8|[hidden-toolbar] → Lang_ComprehensiveConfig|ComprehensiveConfig|{'groupName': 'group3', 'priority': '2'}|not-implemented|综合配置服务开放，现工具栏禁用|
|5.9|[hidden-toolbar] → Lang_FrontEdit|EditFrontendUI|{'groupName': 'group3', 'priority': '3'}|not-implemented|真实设计器/缓冲及关闭依赖开放（A06）|

窗口PreviewKeyDown当前CIL为nop/ret；不创建F5或CtrlS等未经原证的全局热键。源XML没有声明快捷键字段。当前画布快捷键有焦点/输入控件门槛，按既有画布证据单列。

本表完整记录外部服务缺口；不把disabled按钮或假窗口算成服务实现，不将普通Undo等同UndoList。
